using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolicitudServidores.Back.DTOs;
using SolicitudServidores.DBContext;

namespace SolicitudServidores.Controllers
{
    [ApiController]
    [Route("api/reporte")]
    public class ReporteController : ControllerBase
    {
        private readonly DataContext _db;

        public ReporteController(DataContext db)
        {
            _db = db;
        }

        // ─── 1. Solicitudes ───────────────────────────────────────────────

        /// <summary>1.1 Solicitudes por dependencia</summary>
        [HttpGet("solicitudes/por-dependencia")]
        [ProducesResponseType(typeof(List<Reporte11ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SolicitudesPorDependencia(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var query = _db.Solicitudes
                .Include(s => s.CreadoPor).ThenInclude(u => u!.Rol)
                .AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(s => s.CreatedAt >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(s => s.CreatedAt <= fechaFin.Value);

            var result = await query
                .OrderBy(s => s.CreadoPor!.Rol!.Nombre)
                .ThenBy(s => s.CreatedAt)
                .Select(s => new Reporte11ItemDto
                {
                    FolioSolicitud      = s.Folio,
                    Dependencia         = s.CreadoPor != null ? s.CreadoPor.Rol!.Nombre : string.Empty,
                    Responsable         = s.CreadoPor != null ? s.CreadoPor.Nombre + " " + s.CreadoPor.Apellidos : string.Empty,
                    Contacto            = s.CreadoPor != null ? s.CreadoPor.Email : string.Empty,
                    EstatusProcesamieto = s.Estatus,
                    FechaCreacion       = s.CreatedAt,
                })
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>1.2 Recursos solicitados (totalizado)</summary>
        [HttpGet("solicitudes/recursos-solicitados")]
        [ProducesResponseType(typeof(Reporte12ResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RecursosSolicitados(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var solicitudQuery = _db.Solicitudes
                .Include(s => s.CreadoPor).ThenInclude(u => u!.Rol)
                .Include(s => s.Servidor)
                .AsQueryable();

            if (fechaInicio.HasValue)
                solicitudQuery = solicitudQuery.Where(s => s.CreatedAt >= fechaInicio.Value);
            if (fechaFin.HasValue)
                solicitudQuery = solicitudQuery.Where(s => s.CreatedAt <= fechaFin.Value);

            var solicitudes = await solicitudQuery.ToListAsync();

            var items = solicitudes
                .Where(s => s.Servidor != null)
                .Select(s => new Reporte12ItemDto
                {
                    FolioSolicitud        = s.Folio,
                    Dependencia           = s.CreadoPor?.Rol?.Nombre ?? string.Empty,
                    Responsable           = s.CreadoPor != null ? s.CreadoPor.Nombre + " " + s.CreadoPor.Apellidos : string.Empty,
                    Contacto              = s.CreadoPor?.Email ?? string.Empty,
                    EstatusProcesamieto   = s.Estatus,
                    IpServidor            = s.Servidor!.Ip,
                    AdministradorServidor = s.Servidor.ResponsableInfraestructura,
                    DescripcionProyecto   = s.DescripcionUso,
                    SistemaOperativo      = s.Servidor.SistemaOperativo,
                    Vcpu                  = s.Servidor.Nucleos,
                    Ram                   = s.Servidor.Ram,
                    Almacenamiento        = s.Servidor.Almacenamiento,
                    FechaCreacion         = s.CreatedAt,
                })
                .ToList();

            var response = new Reporte12ResponseDto
            {
                Items               = items,
                TotalVcpu           = items.Sum(i => i.Vcpu),
                TotalRam            = items.Sum(i => i.Ram),
                TotalAlmacenamiento = items.Sum(i => i.Almacenamiento),
            };

            return Ok(response);
        }

        /// <summary>1.3 Reporte por IP</summary>
        [HttpGet("solicitudes/por-ip")]
        [ProducesResponseType(typeof(List<Reporte13ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReportePorIp([FromQuery] string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return BadRequest("El parámetro 'ip' es requerido.");

            var servidores = await _db.Servidores
                .Include(s => s.Solicitud)
                    .ThenInclude(sol => sol!.CreadoPor)
                        .ThenInclude(u => u!.Rol)
                .Include(s => s.ServerSubdominios)
                    .ThenInclude(ss => ss.Subdominio)
                .Include(s => s.ServerVpns)
                    .ThenInclude(sv => sv.Vpn)
                .Where(s => s.Ip == ip)
                .ToListAsync();

            var result = servidores.Select(srv => new Reporte13ItemDto
            {
                FolioSolicitud        = srv.Solicitud?.Folio ?? string.Empty,
                Dependencia           = srv.Solicitud?.CreadoPor?.Rol?.Nombre ?? string.Empty,
                Responsable           = srv.Solicitud?.CreadoPor != null
                                            ? srv.Solicitud.CreadoPor.Nombre + " " + srv.Solicitud.CreadoPor.Apellidos
                                            : string.Empty,
                ContactoResponsable   = srv.Solicitud?.CreadoPor?.Email ?? string.Empty,
                EstatusProcesamieto   = srv.Solicitud?.Estatus ?? string.Empty,
                IpServidor            = srv.Ip,
                AdministradorServidor = srv.ResponsableInfraestructura,
                DescripcionProyecto   = srv.Solicitud?.DescripcionUso,
                SistemaOperativo      = srv.SistemaOperativo,
                Vcpu                  = srv.Nucleos,
                Ram                   = srv.Ram,
                Almacenamiento        = srv.Almacenamiento,
                SubdominiosAprobados  = srv.ServerSubdominios
                    .Where(ss => ss.Subdominio.Status == "aprobado")
                    .Select(ss => ss.Subdominio.ApprovedName ?? ss.Subdominio.RequestedName)
                    .ToList(),
                Vpns = srv.ServerVpns
                    .Select(sv => sv.Vpn.VpnId.ToString())
                    .ToList(),
            }).ToList();

            return Ok(result);
        }

        // ─── 2. Infraestructura ───────────────────────────────────────────

        /// <summary>2.1 Reporte de VPNs</summary>
        [HttpGet("infraestructura/vpn")]
        [ProducesResponseType(typeof(List<Reporte21ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReporteVpn([FromQuery] string? estado)
        {
            var query = _db.VPNs
                .Include(v => v.ServerVpns)
                    .ThenInclude(sv => sv.Servidor)
                        .ThenInclude(s => s!.Solicitud)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(v => v.Estado == estado);

            var vpns = await query.ToListAsync();

            var result = vpns.Select(v =>
            {
                var servidor = v.ServerVpns.Select(sv => sv.Servidor).FirstOrDefault();
                return new Reporte21ItemDto
                {
                    FolioVpn        = v.VpnId.ToString(),
                    Tipo            = v.VpnType,
                    Estado          = null,
                    FechaAsignacion = v.CreatedAt,
                    FechaExpiracion = null,
                    Responsable     = v.Responsable,
                    Hostname        = servidor?.Hostname ?? string.Empty,
                    IpServidor      = servidor?.Ip,
                    FolioSolicitud  = servidor?.Solicitud?.Folio ?? string.Empty,
                };
            }).ToList();

            return Ok(result);
        }

        /// <summary>2.2 Reporte de subdominios</summary>
        [HttpGet("infraestructura/subdominios")]
        [ProducesResponseType(typeof(List<Reporte22ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReporteSubdominios([FromQuery] string? estado)
        {
            var query = _db.Subdominios
                .Include(sub => sub.ServerSubdominios)
                    .ThenInclude(ss => ss.Servidor)
                        .ThenInclude(s => s!.Solicitud)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(sub => sub.Status == estado);

            var subdominios = await query.ToListAsync();

            var result = subdominios.Select(sub =>
            {
                var servidor = sub.ServerSubdominios.Select(ss => ss.Servidor).FirstOrDefault();
                return new Reporte22ItemDto
                {
                    NombreUrl       = sub.ApprovedName ?? sub.RequestedName,
                    Estado          = sub.Status,
                    FechaAsignacion = sub.AssignedAt,
                    FechaExpiracion = sub.ExpiresAt,
                    Responsable     = null,
                    Hostname        = servidor?.Hostname ?? string.Empty,
                    IpServidor      = servidor?.Ip,
                    FolioSolicitud  = servidor?.Solicitud?.Folio ?? string.Empty,
                };
            }).ToList();

            return Ok(result);
        }

        // ─── 3. Seguridad ─────────────────────────────────────────────────

        /// <summary>3.1 Vulnerabilidades por servidor</summary>
        [HttpGet("seguridad/vulnerabilidades")]
        [ProducesResponseType(typeof(List<Reporte31ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReporteVulnerabilidades()
        {
            var servidores = await _db.Servidores
                .Include(s => s.Solicitud)
                    .ThenInclude(sol => sol!.CreadoPor)
                        .ThenInclude(u => u!.Rol)
                .OrderBy(s => s.Hostname)
                .ToListAsync();

            var result = servidores.Select(s => new Reporte31ItemDto
            {
                FolioSolicitud           = s.Solicitud?.Folio ?? string.Empty,
                Hostname                 = s.Hostname,
                IpServidor               = s.Ip,
                Dependencia              = s.Solicitud?.CreadoPor?.Rol?.Nombre ?? string.Empty,
                EtapaVulnerabilidades    = s.EtapaVulnerabilidades,
                ParchesAplicados         = s.ParchesAplicados,
                FechaParches             = s.FechaParches,
                UsuarioParches           = s.UsuarioParches,
                XdrInstalado             = s.XdrInstalado,
                FechaXdr                 = s.FechaXdr,
                UsuarioXdr               = s.UsuarioXdr,
                WafConfigurado           = s.WafConfigurado,
                FechaWaf                 = s.FechaConfiguracionWaf,
                EvidenciaValidada        = s.EvidenciaValidada,
                FechaValidacionEvidencia = s.FechaValidacionEvidencia,
            }).ToList();

            return Ok(result);
        }

        /// <summary>3.2 Comunicaciones validadas por IP</summary>
        [HttpGet("seguridad/comunicaciones-por-ip")]
        [ProducesResponseType(typeof(List<Reporte32ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ComunicacionesPorIp([FromQuery] bool? soloValidadas)
        {
            var query = _db.Servidores
                .Include(s => s.Solicitud)
                    .ThenInclude(sol => sol!.CreadoPor)
                        .ThenInclude(u => u!.Rol)
                .AsQueryable();

            if (soloValidadas == true)
                query = query.Where(s => s.ComunicacionValidada);

            var result = await query
                .OrderBy(s => s.Ip)
                .Select(s => new Reporte32ItemDto
                {
                    FolioSolicitud                = s.Solicitud != null ? s.Solicitud.Folio : string.Empty,
                    Hostname                      = s.Hostname,
                    IpServidor                    = s.Ip,
                    Dependencia                   = s.Solicitud != null && s.Solicitud.CreadoPor != null
                                                        ? s.Solicitud.CreadoPor.Rol!.Nombre
                                                        : string.Empty,
                    ComunicacionValidada          = s.ComunicacionValidada,
                    FechaValidacionComunicacion   = s.FechaValidacionComunicacion,
                    UsuarioValidacionComunicacion = s.UsuarioValidacionComunicacion,
                })
                .ToListAsync();

            return Ok(result);
        }

        // ─── 4. Resumen ejecutivo ─────────────────────────────────────────

        /// <summary>4.1 Estatus de solicitudes</summary>
        [HttpGet("resumen/estatus-solicitudes")]
        [ProducesResponseType(typeof(Reporte41ResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> EstatusSolicitudes()
        {
            var solicitudes = await _db.Solicitudes
                .GroupBy(s => s.Estatus)
                .Select(g => new Reporte41EstatusDto
                {
                    Etapa  = g.Key,
                    Estado = g.Key,
                    Total  = g.Count(),
                })
                .OrderBy(r => r.Etapa)
                .ToListAsync();

            var response = new Reporte41ResponseDto
            {
                Resumen          = solicitudes,
                TotalSolicitudes = solicitudes.Sum(r => r.Total),
            };

            return Ok(response);
        }

        /// <summary>4.2 Recursos totalizados</summary>
        [HttpGet("resumen/recursos-totalizados")]
        [ProducesResponseType(typeof(Reporte42ResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RecursosTotalizados()
        {
            var solicitudes = await _db.Solicitudes
                .Include(s => s.CreadoPor).ThenInclude(u => u!.Rol)
                .Include(s => s.Servidor)
                .Where(s => s.Servidor != null)
                .OrderBy(s => s.Folio)
                .ToListAsync();

            var items = solicitudes.Select(s => new Reporte42ItemDto
            {
                FolioSolicitud = s.Folio,
                Dependencia    = s.CreadoPor?.Rol?.Nombre ?? string.Empty,
                Responsable    = s.CreadoPor != null ? s.CreadoPor.Nombre + " " + s.CreadoPor.Apellidos : string.Empty,
                Vcpu           = s.Servidor!.Nucleos,
                Ram            = s.Servidor.Ram,
                Almacenamiento = s.Servidor.Almacenamiento,
            }).ToList();

            var response = new Reporte42ResponseDto
            {
                Items               = items,
                TotalVcpu           = items.Sum(i => i.Vcpu),
                TotalRam            = items.Sum(i => i.Ram),
                TotalAlmacenamiento = items.Sum(i => i.Almacenamiento),
                TotalServidores     = items.Count,
            };

            return Ok(response);
        }
    }
}
