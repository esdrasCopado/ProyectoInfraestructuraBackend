using SolicitudServidores.DTOs;

namespace SolicitudServidores.Back.DTOs
{
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public string? Token { get; set; }
        public UsuarioDTO? User { get; set; }
    }

    public class UserDTO
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Rol { get; set; }
    }

    public class CreateUsuarioRequest
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Rol { get; set; }
        public string? Permisos { get; set; }
        public string? Puesto { get; set; }
        public string? Celular { get; set; }
        public string? NumeroPuesto { get; set; }
        public List<string>? PermisosCategoria { get; set; }
    }

    public class UpdateUsuarioRequest
    {
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Password { get; set; }
        public string? Rol { get; set; }
        public string? Permisos { get; set; }
        public string? Puesto { get; set; }
        public string? Celular { get; set; }
        public string? NumeroPuesto { get; set; }
        public List<string>? PermisosCategoria { get; set; }
    }

    public class CreateSolicitudRequest
    {
        public long? IdUsuario { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Folio { get; set; }
        public string Arquitectura { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Servicios { get; set; } = string.Empty;
        public string? Estado { get; set; }
        public string? EtapaActual { get; set; }
        public string? Prioridad { get; set; }
        public string? ResponsableActual { get; set; }
        public string? UsuarioUltimaActualizacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaRequerida { get; set; }
        public string? CartaResponsivaFolio { get; set; }
        public string? ComentariosSeguimiento { get; set; }
        public bool NotificacionNueva { get; set; } = true;
        public string? TareasPendientes { get; set; }
        public List<CreateServidorRequest>? Servidores { get; set; }
    }

    public class UpdateSolicitudRequest
    {
        public long? IdUsuario { get; set; }
        public string? Titulo { get; set; }
        public string? Folio { get; set; }
        public string? Estado { get; set; }
        public string? EtapaActual { get; set; }
        public string? Prioridad { get; set; }
        public string? Arquitectura { get; set; }
        public string? Descripcion { get; set; }
        public string? Servicios { get; set; }
        public string? ResponsableActual { get; set; }
        public string? UsuarioUltimaActualizacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaRequerida { get; set; }
        public string? CartaResponsivaFolio { get; set; }
        public string? ComentariosSeguimiento { get; set; }
        public bool? NotificacionNueva { get; set; }
        public string? TareasPendientes { get; set; }
        public List<CreateServidorRequest>? Servidores { get; set; }
    }

    public class CreateServidorRequest
    {
        public long? IdSolicitud { get; set; }
        public string? Estado { get; set; }
        public DateTime? Expiracion { get; set; }
        public string? Hostname { get; set; }
        public string? Ip { get; set; }
        public string? TipoUso { get; set; }
        public string? Funcion { get; set; }
        public string? SistemaOperativo { get; set; }
        public bool? RequiereLlaveLicencia { get; set; }
        public string? LlaveOS { get; set; }
        public int? Nucleos { get; set; }
        public int? Ram { get; set; }
        public int? Almacenamiento { get; set; }
        public string? Descripcion { get; set; }
        public string? PlantillaRecursos { get; set; }
        public string? EtapaOperativa { get; set; }
        public string? ResponsableInfraestructura { get; set; }
        public string? UsuarioUltimaActualizacion { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public DateTime? FechaAsignacionIp { get; set; }
        public string? TareasPendientes { get; set; }
        public string? ObservacionesSeguimiento { get; set; }
        public string? EtapaVulnerabilidades { get; set; }
        public bool? RequiereRevisionAnual { get; set; }
        public DateTime? UltimaRevisionAnual { get; set; }
        public bool? ComunicacionValidada { get; set; }
        public DateTime? FechaValidacionComunicacion { get; set; }
        public string? UsuarioValidacionComunicacion { get; set; }
        public bool? ParchesAplicados { get; set; }
        public DateTime? FechaParches { get; set; }
        public string? UsuarioParches { get; set; }
        public bool? XdrInstalado { get; set; }
        public DateTime? FechaXdr { get; set; }
        public string? UsuarioXdr { get; set; }
        public bool? CredencialesEntregadas { get; set; }
        public DateTime? FechaEntregaCredenciales { get; set; }
        public string? UsuarioCredenciales { get; set; }
        public bool? WafConfigurado { get; set; }
        public DateTime? FechaConfiguracionWaf { get; set; }
        public string? UsuarioWaf { get; set; }
        public bool? EvidenciaValidada { get; set; }
        public DateTime? FechaValidacionEvidencia { get; set; }
        public string? UsuarioValidacionEvidencia { get; set; }
        public bool? SolicitudPublicacion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public string? UsuarioPublicacion { get; set; }
        public DateTime? FechaVulnerabilidades { get; set; }
        public string? UsuarioVulnerabilidades { get; set; }
        public List<VpnRequestDTO>? VPNs { get; set; }
        public List<SubdominioRequestDTO>? Subdominios { get; set; }
        public List<WafRequestDTO>? WAFs { get; set; }
        public List<EvidenciaPruebaRequestDTO>? EvidenciasPruebas { get; set; }
    }

    public class UpdateServidorRequest : CreateServidorRequest
    {
    }

    public class VpnRequestDTO
    {
        public long? IdUsuarioResponsable { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public string? Estado { get; set; }
    }

    public class SubdominioRequestDTO
    {
        public long? IdUsuario { get; set; }
        public string NombreUrl { get; set; } = string.Empty;
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public string? Estado { get; set; }
    }

    public class WafRequestDTO
    {
        public long? IdUsuario { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Estado { get; set; }
        public string? Observaciones { get; set; }
    }

    public class EvidenciaPruebaRequestDTO
    {
        public long? IdUsuario { get; set; }
        public string RutaPdf { get; set; } = string.Empty;
        public DateTime? Fecha { get; set; }
        public string? EstadoValidacion { get; set; }
        public string? Observaciones { get; set; }
    }

    public class ActualizarEstadoRequest
    {
        public string Estado { get; set; } = string.Empty;
        public string? EtapaActual { get; set; }
        public string? ResponsableActual { get; set; }
        public string? UsuarioUltimaActualizacion { get; set; }
        public string? ComentariosSeguimiento { get; set; }
    }

    public class RecursoServidorPredeterminadoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public int Nucleos { get; set; }
        public int Ram { get; set; }
        public int Almacenamiento { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
