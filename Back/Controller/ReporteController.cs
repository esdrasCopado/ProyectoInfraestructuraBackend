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
        /// <remarks>
        /// Devuelve todas las solicitudes con información del responsable y estatus de procesamiento.
        /// Se puede filtrar por rango de fecha de creación.
        /// </remarks>
        /// <param name="fechaInicio">Fecha inicial del rango (yyyy-MM-dd)</param>
        /// <param name="fechaFin">Fecha final del rango (yyyy-MM-dd)</param>
        [HttpGet("solicitudes/por-dependencia")]
        [ProducesResponseType(typeof(List<Reporte11ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SolicitudesPorDependencia(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var query = _db.Solicitudes
                .Include(s => s.Usuario)
                .AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(s => s.Fecha_creacion >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(s => s.Fecha_creacion <= fechaFin.Value);

            var result = await query
                .OrderBy(s => s.Usuario!.Rol)
                .ThenBy(s => s.Fecha_creacion)
                .Select(s => new Reporte11ItemDto
                {
                    FolioSolicitud       = s.Folio,
                    Dependencia          = s.Usuario != null ? s.Usuario.Rol : string.Empty,
                    Responsable          = s.Usuario != null ? s.Usuario.NombreCompleto : string.Empty,
                    Contacto             = s.Usuario != null ? s.Usuario.Correo : string.Empty,
                    EstatusProcesamieto  = s.EtapaActual,
                    FechaCreacion        = s.Fecha_creacion,
                })
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>1.2 Recursos solicitados (totalizado)</summary>
        /// <remarks>
        /// Lista cada servidor asociado a una solicitud con sus recursos (vCPU, RAM, Almacenamiento).
        /// Incluye la suma total al final. Se puede filtrar por rango de fecha.
        /// </remarks>
        /// <param name="fechaInicio">Fecha inicial del rango (yyyy-MM-dd)</param>
        /// <param name="fechaFin">Fecha final del rango (yyyy-MM-dd)</param>
        [HttpGet("solicitudes/recursos-solicitados")]
        [ProducesResponseType(typeof(Reporte12ResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RecursosSolicitados(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var solicitudQuery = _db.Solicitudes
                .Include(s => s.Usuario)
                .Include(s => s.Servidores)
                .AsQueryable();

            if (fechaInicio.HasValue)
                solicitudQuery = solicitudQuery.Where(s => s.Fecha_creacion >= fechaInicio.Value);
            if (fechaFin.HasValue)
                solicitudQuery = solicitudQuery.Where(s => s.Fecha_creacion <= fechaFin.Value);

            var solicitudes = await solicitudQuery.ToListAsync();

            var items = solicitudes
                .SelectMany(s => s.Servidores.Select(srv => new Reporte12ItemDto
                {
                    FolioSolicitud        = s.Folio,
                    Dependencia           = s.Usuario?.Rol ?? string.Empty,
                    Responsable           = s.Usuario?.NombreCompleto ?? string.Empty,
                    Contacto              = s.Usuario?.Correo ?? string.Empty,
                    EstatusProcesamieto   = s.EtapaActual,
                    IpServidor            = srv.Ip,
                    AdministradorServidor = srv.ResponsableInfraestructura,
                    DescripcionProyecto   = s.Descripcion,
                    SistemaOperativo      = srv.SistemaOperativo,
                    Vcpu                  = srv.Nucleos,
                    Ram                   = srv.Ram,
                    Almacenamiento        = srv.Almacenamiento,
                    FechaCreacion         = s.Fecha_creacion,
                }))
                .ToList();

            var response = new Reporte12ResponseDto
            {
                Items              = items,
                TotalVcpu          = items.Sum(i => i.Vcpu),
                TotalRam           = items.Sum(i => i.Ram),
                TotalAlmacenamiento = items.Sum(i => i.Almacenamiento),
            };

            return Ok(response);
        }

        /// <summary>1.3 Reporte por IP</summary>
        /// <remarks>
        /// Dado una dirección IP, devuelve el servidor correspondiente con su solicitud,
        /// subdominios aprobados y VPNs asociadas.
        /// </remarks>
        /// <param name="ip">Dirección IP del servidor a consultar</param>
        [HttpGet("solicitudes/por-ip")]
        [ProducesResponseType(typeof(List<Reporte13ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReportePorIp([FromQuery] string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return BadRequest("El parámetro 'ip' es requerido.");

            var servidores = await _db.Servidores
                .Include(s => s.Solicitud)
                    .ThenInclude(sol => sol!.Usuario)
                .Include(s => s.Subdominios)
                .Include(s => s.VPNs)
                .Where(s => s.Ip == ip)
                .ToListAsync();

            var result = servidores.Select(srv => new Reporte13ItemDto
            {
                FolioSolicitud        = srv.Solicitud?.Folio ?? string.Empty,
                Dependencia           = srv.Solicitud?.Usuario?.Rol ?? string.Empty,
                Responsable           = srv.Solicitud?.Usuario?.NombreCompleto ?? string.Empty,
                ContactoResponsable   = srv.Solicitud?.Usuario?.Correo ?? string.Empty,
                EstatusProcesamieto   = srv.Solicitud?.EtapaActual ?? string.Empty,
                IpServidor            = srv.Ip,
                AdministradorServidor = srv.ResponsableInfraestructura,
                DescripcionProyecto   = srv.Solicitud?.Descripcion,
                SistemaOperativo      = srv.SistemaOperativo,
                Vcpu                  = srv.Nucleos,
                Ram                   = srv.Ram,
                Almacenamiento        = srv.Almacenamiento,
                SubdominiosAprobados  = srv.Subdominios
                    .Where(sub => sub.Estado == "Aprobado")
                    .Select(sub => sub.Nombre_url)
                    .ToList(),
                Vpns = srv.VPNs
                    .Select(v => v.Folio)
                    .ToList(),
            }).ToList();

            return Ok(result);
        }

        // ─── 2. Infraestructura ───────────────────────────────────────────

        /// <summary>2.1 Reporte de VPNs</summary>
        /// <remarks>Lista todas las VPNs con su estado, fechas y servidor asociado. Filtrable por estado.</remarks>
        /// <param name="estado">Filtro opcional por estado (ej. Activa, Expirada)</param>
        [HttpGet("infraestructura/vpn")]
        [ProducesResponseType(typeof(List<Reporte21ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReporteVpn([FromQuery] string? estado)
        {
            var query = _db.VPNs
                .Include(v => v.Servidor)
                    .ThenInclude(s => s!.Solicitud)
                .Include(v => v.Usuario)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(v => v.Estado == estado);

            var result = await query
                .OrderBy(v => v.Servidor!.Hostname)
                .Select(v => new Reporte21ItemDto
                {
                    FolioVpn       = v.Folio,
                    Tipo           = v.Tipo,
                    Estado         = v.Estado,
                    FechaAsignacion = v.Fecha_asignacion,
                    FechaExpiracion = v.Fecha_Expiracion,
                    Responsable    = v.Usuario != null ? v.Usuario.NombreCompleto : null,
                    Hostname       = v.Servidor != null ? v.Servidor.Hostname : string.Empty,
                    IpServidor     = v.Servidor != null ? v.Servidor.Ip : null,
                    FolioSolicitud = v.Servidor != null && v.Servidor.Solicitud != null ? v.Servidor.Solicitud.Folio : string.Empty,
                })
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>2.2 Reporte de subdominios</summary>
        /// <remarks>Lista todos los subdominios con su estado, fechas y servidor asociado. Filtrable por estado.</remarks>
        /// <param name="estado">Filtro opcional por estado (ej. Aprobado, Pendiente, Expirado)</param>
        [HttpGet("infraestructura/subdominios")]
        [ProducesResponseType(typeof(List<Reporte22ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReporteSubdominios([FromQuery] string? estado)
        {
            var query = _db.Subdominios
                .Include(sub => sub.Servidor)
                    .ThenInclude(s => s!.Solicitud)
                .Include(sub => sub.Usuario)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(sub => sub.Estado == estado);

            var result = await query
                .OrderBy(sub => sub.Nombre_url)
                .Select(sub => new Reporte22ItemDto
                {
                    NombreUrl      = sub.Nombre_url,
                    Estado         = sub.Estado,
                    FechaAsignacion = sub.Fecha_asignacion,
                    FechaExpiracion = sub.Fecha_Expiracion,
                    Responsable    = sub.Usuario != null ? sub.Usuario.NombreCompleto : null,
                    Hostname       = sub.Servidor != null ? sub.Servidor.Hostname : string.Empty,
                    IpServidor     = sub.Servidor != null ? sub.Servidor.Ip : null,
                    FolioSolicitud = sub.Servidor != null && sub.Servidor.Solicitud != null ? sub.Servidor.Solicitud.Folio : string.Empty,
                })
                .ToListAsync();

            return Ok(result);
        }

        // ─── 3. Seguridad ─────────────────────────────────────────────────

        /// <summary>3.1 Vulnerabilidades por servidor</summary>
        /// <remarks>
        /// Muestra el estado de seguridad de cada servidor: etapa de vulnerabilidades,
        /// parches aplicados, XDR instalado, WAF configurado y evidencias validadas.
        /// </remarks>
        [HttpGet("seguridad/vulnerabilidades")]
        [ProducesResponseType(typeof(List<Reporte31ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReporteVulnerabilidades()
        {
            var servidores = await _db.Servidores
                .Include(s => s.Solicitud)
                    .ThenInclude(sol => sol!.Usuario)
                .OrderBy(s => s.Hostname)
                .ToListAsync();

            var result = servidores.Select(s => new Reporte31ItemDto
            {
                FolioSolicitud          = s.Solicitud?.Folio ?? string.Empty,
                Hostname                = s.Hostname,
                IpServidor              = s.Ip,
                Dependencia             = s.Solicitud?.Usuario?.Rol ?? string.Empty,
                EtapaVulnerabilidades   = s.EtapaVulnerabilidades,
                ParchesAplicados        = s.ParchesAplicados,
                FechaParches            = s.FechaParches,
                UsuarioParches          = s.UsuarioParches,
                XdrInstalado            = s.XdrInstalado,
                FechaXdr                = s.FechaXdr,
                UsuarioXdr              = s.UsuarioXdr,
                WafConfigurado          = s.WafConfigurado,
                FechaWaf                = s.FechaConfiguracionWaf,
                EvidenciaValidada       = s.EvidenciaValidada,
                FechaValidacionEvidencia = s.FechaValidacionEvidencia,
            }).ToList();

            return Ok(result);
        }

        /// <summary>3.2 Comunicaciones validadas por IP</summary>
        /// <remarks>
        /// Devuelve el estado de validación de comunicación de cada servidor,
        /// con fecha y usuario que realizó la validación. Filtrable por estado de validación.
        /// </remarks>
        /// <param name="soloValidadas">Si es true, devuelve únicamente las comunicaciones validadas</param>
        [HttpGet("seguridad/comunicaciones-por-ip")]
        [ProducesResponseType(typeof(List<Reporte32ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ComunicacionesPorIp([FromQuery] bool? soloValidadas)
        {
            var query = _db.Servidores
                .Include(s => s.Solicitud)
                    .ThenInclude(sol => sol!.Usuario)
                .AsQueryable();

            if (soloValidadas == true)
                query = query.Where(s => s.ComunicacionValidada);

            var result = await query
                .OrderBy(s => s.Ip)
                .Select(s => new Reporte32ItemDto
                {
                    FolioSolicitud               = s.Solicitud != null ? s.Solicitud.Folio : string.Empty,
                    Hostname                     = s.Hostname,
                    IpServidor                   = s.Ip,
                    Dependencia                  = s.Solicitud != null && s.Solicitud.Usuario != null
                                                       ? s.Solicitud.Usuario.Rol : string.Empty,
                    ComunicacionValidada         = s.ComunicacionValidada,
                    FechaValidacionComunicacion  = s.FechaValidacionComunicacion,
                    UsuarioValidacionComunicacion = s.UsuarioValidacionComunicacion,
                })
                .ToListAsync();

            return Ok(result);
        }

        // ─── 4. Resumen ejecutivo ─────────────────────────────────────────

        /// <summary>4.1 Estatus de solicitudes</summary>
        /// <remarks>
        /// Devuelve un resumen agrupado por etapa y estado de todas las solicitudes,
        /// junto con el total general.
        /// </remarks>
        [HttpGet("resumen/estatus-solicitudes")]
        [ProducesResponseType(typeof(Reporte41ResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> EstatusSolicitudes()
        {
            var solicitudes = await _db.Solicitudes
                .GroupBy(s => new { s.EtapaActual, s.Estado })
                .Select(g => new Reporte41EstatusDto
                {
                    Etapa  = g.Key.EtapaActual,
                    Estado = g.Key.Estado,
                    Total  = g.Count(),
                })
                .OrderBy(r => r.Etapa)
                .ThenBy(r => r.Estado)
                .ToListAsync();

            var response = new Reporte41ResponseDto
            {
                Resumen          = solicitudes,
                TotalSolicitudes = solicitudes.Sum(r => r.Total),
            };

            return Ok(response);
        }

        /// <summary>4.2 Recursos totalizados</summary>
        /// <remarks>
        /// Suma total de vCPU, RAM y almacenamiento de todos los servidores registrados,
        /// con desglose por solicitud.
        /// </remarks>
        [HttpGet("resumen/recursos-totalizados")]
        [ProducesResponseType(typeof(Reporte42ResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RecursosTotalizados()
        {
            var solicitudes = await _db.Solicitudes
                .Include(s => s.Usuario)
                .Include(s => s.Servidores)
                .Where(s => s.Servidores.Any())
                .OrderBy(s => s.Folio)
                .ToListAsync();

            var items = solicitudes.Select(s => new Reporte42ItemDto
            {
                FolioSolicitud = s.Folio,
                Dependencia    = s.Usuario?.Rol ?? string.Empty,
                Responsable    = s.Usuario?.NombreCompleto ?? string.Empty,
                Vcpu           = s.Servidores.Sum(srv => srv.Nucleos),
                Ram            = s.Servidores.Sum(srv => srv.Ram),
                Almacenamiento = s.Servidores.Sum(srv => srv.Almacenamiento),
            }).ToList();

            var response = new Reporte42ResponseDto
            {
                Items               = items,
                TotalVcpu           = items.Sum(i => i.Vcpu),
                TotalRam            = items.Sum(i => i.Ram),
                TotalAlmacenamiento = items.Sum(i => i.Almacenamiento),
                TotalServidores     = solicitudes.Sum(s => s.Servidores.Count),
            };

            return Ok(response);
        }
    }
}
