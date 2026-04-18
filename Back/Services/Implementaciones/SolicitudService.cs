using SolicitudServidores.Back.DTOs;
using SolicitudServidores.DTOs;
using SolicitudServidores.Helpers;
using SolicitudServidores.Models;
using SolicitudServidores.Repositories.Interfaces;
using SolicitudServidores.Services.Interfaces;

namespace SolicitudServidores.Services.Implementaciones
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _repo;

        public SolicitudService(ISolicitudRepository repo)
        {
            _repo = repo;
        }

        // ──────────────────────────────────────────────
        // Consultas
        // ──────────────────────────────────────────────

        public async Task<IEnumerable<Solicitud>> GetAllAsync(int pagina = 0, int cantidad = 20)
        {
            if (pagina <= 0)
                return await _repo.GetAll();

            var query = new QueryUserPaging { NumPage = pagina, NumSize = cantidad };
            return await _repo.GetAll(query);
        }

        public Task<Solicitud?> GetByIdAsync(long id)
            => _repo.GetById(id);

        public Task<IEnumerable<Solicitud>> GetByUsuarioAsync(long idUsuario)
            => _repo.GetAllByUsuario(idUsuario);

        public async Task<IEnumerable<Solicitud>> GetNotificacionesNuevasAsync()
        {
            var todas = await _repo.GetAll();
            return todas.Where(s => s.NotificacionNueva);
        }

        public async Task<DashboardResumenDto> GetDashboardResumenAsync()
        {
            var solicitudes = (await _repo.GetAll()).ToList();

            return new DashboardResumenDto
            {
                Total = solicitudes.Count,
                Nuevas = solicitudes.Count(s => s.NotificacionNueva),
                Pendientes = solicitudes.Count(s =>
                    string.Equals(s.Estado, "Pendiente", StringComparison.OrdinalIgnoreCase)),
                EnProceso = solicitudes.Count(s =>
                    string.Equals(s.Estado, "En progreso", StringComparison.OrdinalIgnoreCase)),
                Terminadas = solicitudes.Count(s =>
                    string.Equals(s.Estado, "Terminada", StringComparison.OrdinalIgnoreCase)),
                PorEtapa = solicitudes
                    .GroupBy(s => string.IsNullOrWhiteSpace(s.EtapaActual) ? "Sin etapa" : s.EtapaActual)
                    .Select(g => new EtapaCountDto { Etapa = g.Key, Total = g.Count() })
                    .OrderByDescending(g => g.Total),
                PorPrioridad = solicitudes
                    .GroupBy(s => string.IsNullOrWhiteSpace(s.Prioridad) ? "Sin prioridad" : s.Prioridad)
                    .Select(g => new PrioridadCountDto { Prioridad = g.Key, Total = g.Count() })
                    .OrderByDescending(g => g.Total)
            };
        }

        // ──────────────────────────────────────────────
        // Mutaciones
        // ──────────────────────────────────────────────

        public async Task<Solicitud> CreateAsync(CreateSolicitudRequest request)
        {
            var solicitud = new Solicitud
            {
                Id_Usuario = request.IdUsuario,
                Titulo = request.Titulo.Trim(),
                Folio = string.IsNullOrWhiteSpace(request.Folio)
                    ? GenerarFolio()
                    : request.Folio.Trim(),
                Estado = string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado!,
                EtapaActual = string.IsNullOrWhiteSpace(request.EtapaActual) ? "Registro" : request.EtapaActual!,
                Prioridad = string.IsNullOrWhiteSpace(request.Prioridad) ? "Media" : request.Prioridad!,
                ResponsableActual = request.ResponsableActual,
                UsuarioUltimaActualizacion = request.UsuarioUltimaActualizacion ?? request.ResponsableActual,
                FechaActualizacion = request.FechaActualizacion ?? DateTime.UtcNow,
                FechaRequerida = request.FechaRequerida,
                CartaResponsivaFolio = request.CartaResponsivaFolio,
                ComentariosSeguimiento = request.ComentariosSeguimiento,
                Fecha_creacion = DateTime.UtcNow,
                Arquitectura = request.Arquitectura?.Trim() ?? string.Empty,
                Descripcion = request.Descripcion,
                Servicios = request.Servicios?.Trim() ?? string.Empty,
                NotificacionNueva = request.NotificacionNueva,
                TareasPendientes = request.TareasPendientes,
                Servidores = request.Servidores?.Select(MapServidor).ToList() ?? new List<Servidor>()
            };

            return await _repo.Create(solicitud);
        }

        public async Task<Solicitud?> UpdateAsync(long id, UpdateSolicitudRequest request)
        {
            var existente = await _repo.GetById(id);
            if (existente == null) return null;

            existente.Id_Usuario = request.IdUsuario ?? existente.Id_Usuario;
            existente.Titulo = request.Titulo ?? existente.Titulo;
            existente.Folio = string.IsNullOrWhiteSpace(request.Folio)
                ? existente.Folio
                : request.Folio.Trim();
            existente.Estado = request.Estado ?? existente.Estado;
            existente.EtapaActual = request.EtapaActual ?? existente.EtapaActual;
            existente.Prioridad = request.Prioridad ?? existente.Prioridad;
            existente.Arquitectura = request.Arquitectura ?? existente.Arquitectura;
            existente.Descripcion = request.Descripcion ?? existente.Descripcion;
            existente.Servicios = request.Servicios ?? existente.Servicios;
            existente.ResponsableActual = request.ResponsableActual ?? existente.ResponsableActual;
            existente.UsuarioUltimaActualizacion = request.UsuarioUltimaActualizacion
                ?? request.ResponsableActual
                ?? existente.UsuarioUltimaActualizacion;
            existente.CartaResponsivaFolio = request.CartaResponsivaFolio ?? existente.CartaResponsivaFolio;
            existente.ComentariosSeguimiento = request.ComentariosSeguimiento ?? existente.ComentariosSeguimiento;
            existente.TareasPendientes = request.TareasPendientes ?? existente.TareasPendientes;
            existente.FechaActualizacion = request.FechaActualizacion ?? DateTime.UtcNow;

            if (request.FechaRequerida.HasValue)
                existente.FechaRequerida = request.FechaRequerida.Value;

            if (request.NotificacionNueva.HasValue)
                existente.NotificacionNueva = request.NotificacionNueva.Value;

            if (request.Servidores != null)
                existente.Servidores = request.Servidores.Select(MapServidor).ToList();

            return await _repo.Update(existente);
        }

        public async Task<Solicitud?> ActualizarEstadoAsync(long id, ActualizarEstadoRequest request)
        {
            var existente = await _repo.GetById(id);
            if (existente == null) return null;

            existente.Estado = request.Estado;
            if (request.EtapaActual != null)
                existente.EtapaActual = request.EtapaActual;
            if (request.ResponsableActual != null)
                existente.ResponsableActual = request.ResponsableActual;
            existente.UsuarioUltimaActualizacion = request.UsuarioUltimaActualizacion
                ?? request.ResponsableActual
                ?? existente.UsuarioUltimaActualizacion;
            if (request.ComentariosSeguimiento != null)
                existente.ComentariosSeguimiento = request.ComentariosSeguimiento;
            existente.FechaActualizacion = DateTime.UtcNow;

            return await _repo.Update(existente);
        }

        public async Task<Solicitud?> MarcarNotificacionLeidaAsync(long id)
        {
            var existente = await _repo.GetById(id);
            if (existente == null) return null;

            existente.NotificacionNueva = false;
            return await _repo.Update(existente);
        }

        public async Task<Solicitud?> DeleteAsync(long id)
        {
            if (!await _repo.ExistsSolicitud(id)) return null;
            return await _repo.Delete(id);
        }

        // ──────────────────────────────────────────────
        // Mappers (DTO → entidad de dominio)
        // ──────────────────────────────────────────────

        private static Servidor MapServidor(CreateServidorRequest r) => new()
        {
            Id_Solicitud = r.IdSolicitud,
            Estado = string.IsNullOrWhiteSpace(r.Estado) ? "Pendiente" : r.Estado!,
            Expiracion = r.Expiracion,
            Hostname = r.Hostname?.Trim() ?? string.Empty,
            Ip = string.IsNullOrWhiteSpace(r.Ip) ? null : r.Ip.Trim(),
            TipoUso = string.IsNullOrWhiteSpace(r.TipoUso) ? "Interno" : r.TipoUso!,
            Funcion = r.Funcion?.Trim() ?? string.Empty,
            SistemaOperativo = r.SistemaOperativo?.Trim() ?? string.Empty,
            RequiereLlaveLicencia = r.RequiereLlaveLicencia ?? false,
            LlaveOS = string.IsNullOrWhiteSpace(r.LlaveOS) ? null : r.LlaveOS.Trim(),
            Nucleos = r.Nucleos ?? 2,
            Ram = r.Ram ?? 8,
            Almacenamiento = r.Almacenamiento ?? 100,
            Descripcion = r.Descripcion,
            PlantillaRecursos = string.IsNullOrWhiteSpace(r.PlantillaRecursos) ? "General" : r.PlantillaRecursos!,
            EtapaOperativa = string.IsNullOrWhiteSpace(r.EtapaOperativa) ? "Provisionamiento" : r.EtapaOperativa!,
            ResponsableInfraestructura = r.ResponsableInfraestructura,
            UsuarioUltimaActualizacion = r.UsuarioUltimaActualizacion ?? r.ResponsableInfraestructura,
            FechaUltimaActualizacion = r.FechaUltimaActualizacion ?? DateTime.UtcNow,
            FechaAsignacionIp = r.FechaAsignacionIp,
            TareasPendientes = r.TareasPendientes,
            ObservacionesSeguimiento = r.ObservacionesSeguimiento,
            EtapaVulnerabilidades = r.EtapaVulnerabilidades,
            RequiereRevisionAnual = r.RequiereRevisionAnual ?? true,
            UltimaRevisionAnual = r.UltimaRevisionAnual,
            ComunicacionValidada = r.ComunicacionValidada ?? false,
            FechaValidacionComunicacion = r.FechaValidacionComunicacion,
            UsuarioValidacionComunicacion = r.UsuarioValidacionComunicacion,
            ParchesAplicados = r.ParchesAplicados ?? false,
            FechaParches = r.FechaParches,
            UsuarioParches = r.UsuarioParches,
            XdrInstalado = r.XdrInstalado ?? false,
            FechaXdr = r.FechaXdr,
            UsuarioXdr = r.UsuarioXdr,
            CredencialesEntregadas = r.CredencialesEntregadas ?? false,
            FechaEntregaCredenciales = r.FechaEntregaCredenciales,
            UsuarioCredenciales = r.UsuarioCredenciales,
            WafConfigurado = r.WafConfigurado ?? false,
            FechaConfiguracionWaf = r.FechaConfiguracionWaf,
            UsuarioWaf = r.UsuarioWaf,
            EvidenciaValidada = r.EvidenciaValidada ?? false,
            FechaValidacionEvidencia = r.FechaValidacionEvidencia,
            UsuarioValidacionEvidencia = r.UsuarioValidacionEvidencia,
            SolicitudPublicacion = r.SolicitudPublicacion ?? false,
            FechaPublicacion = r.FechaPublicacion,
            UsuarioPublicacion = r.UsuarioPublicacion,
            FechaVulnerabilidades = r.FechaVulnerabilidades,
            UsuarioVulnerabilidades = r.UsuarioVulnerabilidades,
            VPNs = r.VPNs?.Select(MapVpn).ToList() ?? new List<VPN>(),
            Subdominios = r.Subdominios?.Select(MapSubdominio).ToList() ?? new List<Subdominio>(),
            WAFs = r.WAFs?.Select(MapWaf).ToList() ?? new List<WAF>(),
            EvidenciasPruebas = r.EvidenciasPruebas?.Select(MapEvidencia).ToList() ?? new List<EvidenciasPruebas>()
        };

        private static VPN MapVpn(VpnRequestDTO r) => new()
        {
            Id_usuario_Responsable = r.IdUsuarioResponsable,
            Tipo = r.Tipo ?? string.Empty,
            Fecha_asignacion = r.FechaAsignacion,
            Fecha_Expiracion = r.FechaExpiracion,
            Estado = r.Estado,
            Folio = GenerarFolioVpn()
        };

        private static string GenerarFolioVpn()
            => $"VPN-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}";

        private static Subdominio MapSubdominio(SubdominioRequestDTO r) => new()
        {
            Id_usuario = r.IdUsuario,
            Nombre_url = r.NombreUrl,
            Fecha_asignacion = r.FechaAsignacion,
            Fecha_Expiracion = r.FechaExpiracion,
            Estado = r.Estado
        };

        private static WAF MapWaf(WafRequestDTO r) => new()
        {
            Id_usuario = r.IdUsuario,
            Fecha = r.Fecha,
            Estado = r.Estado,
            Observaciones = r.Observaciones
        };

        private static EvidenciasPruebas MapEvidencia(EvidenciaPruebaRequestDTO r) => new()
        {
            Id_usuario = r.IdUsuario,
            Ruta_pdf = r.RutaPdf,
            Fecha = r.Fecha,
            EstadoValidacion = string.IsNullOrWhiteSpace(r.EstadoValidacion) ? "Pendiente" : r.EstadoValidacion!,
            Observaciones = r.Observaciones
        };

        private static string GenerarFolio()
            => $"SOL-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
    }
}
