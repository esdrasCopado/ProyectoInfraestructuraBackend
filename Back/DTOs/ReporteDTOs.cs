namespace SolicitudServidores.Back.DTOs
{
    public class Reporte11ItemDto
    {
        public string FolioSolicitud { get; set; } = string.Empty;
        public string Dependencia { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string Contacto { get; set; } = string.Empty;
        public string EstatusProcesamieto { get; set; } = string.Empty;
        public DateTime? FechaCreacion { get; set; }
    }

    public class Reporte12ItemDto
    {
        public string FolioSolicitud { get; set; } = string.Empty;
        public string Dependencia { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string Contacto { get; set; } = string.Empty;
        public string EstatusProcesamieto { get; set; } = string.Empty;
        public string? IpServidor { get; set; }
        public string? AdministradorServidor { get; set; }
        public string? DescripcionProyecto { get; set; }
        public string SistemaOperativo { get; set; } = string.Empty;
        public int Vcpu { get; set; }
        public int Ram { get; set; }
        public int Almacenamiento { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }

    public class Reporte12ResponseDto
    {
        public List<Reporte12ItemDto> Items { get; set; } = new();
        public int TotalVcpu { get; set; }
        public int TotalRam { get; set; }
        public int TotalAlmacenamiento { get; set; }
    }

    public class Reporte13ItemDto
    {
        public string FolioSolicitud { get; set; } = string.Empty;
        public string Dependencia { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string ContactoResponsable { get; set; } = string.Empty;
        public string EstatusProcesamieto { get; set; } = string.Empty;
        public string? IpServidor { get; set; }
        public string? AdministradorServidor { get; set; }
        public string? DescripcionProyecto { get; set; }
        public string SistemaOperativo { get; set; } = string.Empty;
        public int Vcpu { get; set; }
        public int Ram { get; set; }
        public int Almacenamiento { get; set; }
        public List<string> SubdominiosAprobados { get; set; } = new();
        public List<string> Vpns { get; set; } = new();
    }

    // ─── 2.1 VPN ──────────────────────────────────────────────────────────

    public class Reporte21ItemDto
    {
        public string FolioSolicitud { get; set; } = string.Empty;
        public string Dependencia { get; set; } = string.Empty;
        public string? Responsable { get; set; }
        public string? ContactoResponsable { get; set; }
        public string? Estado { get; set; }
        public string? IpServidor { get; set; }
        public string? IdentificadorVpn { get; set; }
        public string? UsuarioAsignado { get; set; }
        public DateOnly? FechaCreacion { get; set; }
        public DateOnly? FechaVencimiento { get; set; }
        public int? Vigencia { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
    }

    // ─── 2.2 Subdominios ──────────────────────────────────────────────────

    public class Reporte22ItemDto
    {
        public string NombreUrl { get; set; } = string.Empty;
        public string? Estado { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public string? Responsable { get; set; }
        public string Hostname { get; set; } = string.Empty;
        public string? IpServidor { get; set; }
        public string FolioSolicitud { get; set; } = string.Empty;
    }

    // ─── 3.1 Vulnerabilidades ─────────────────────────────────────────────

    public class Reporte31ItemDto
    {
        public string FolioSolicitud { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public string? IpServidor { get; set; }
        public string Dependencia { get; set; } = string.Empty;
        public string? EtapaVulnerabilidades { get; set; }
        public bool ParchesAplicados { get; set; }
        public DateTime? FechaParches { get; set; }
        public string? UsuarioParches { get; set; }
        public bool XdrInstalado { get; set; }
        public DateTime? FechaXdr { get; set; }
        public string? UsuarioXdr { get; set; }
        public bool WafConfigurado { get; set; }
        public DateTime? FechaWaf { get; set; }
        public bool EvidenciaValidada { get; set; }
        public DateTime? FechaValidacionEvidencia { get; set; }
    }

    // ─── 3.2 Comunicaciones por IP ────────────────────────────────────────

    public class Reporte32ItemDto
    {
        public string FolioSolicitud { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public string? IpServidor { get; set; }
        public string Dependencia { get; set; } = string.Empty;
        public bool ComunicacionValidada { get; set; }
        public DateTime? FechaValidacionComunicacion { get; set; }
        public string? UsuarioValidacionComunicacion { get; set; }
    }

    // ─── 4.1 Estatus de solicitudes ───────────────────────────────────────

    public class Reporte41EstatusDto
    {
        public string Etapa { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int Total { get; set; }
    }

    public class Reporte41ResponseDto
    {
        public List<Reporte41EstatusDto> Resumen { get; set; } = new();
        public int TotalSolicitudes { get; set; }
    }

    // ─── 4.2 Recursos totalizados ─────────────────────────────────────────

    public class Reporte42ItemDto
    {
        public string FolioSolicitud { get; set; } = string.Empty;
        public string Dependencia { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public int Vcpu { get; set; }
        public int Ram { get; set; }
        public int Almacenamiento { get; set; }
    }

    public class Reporte42ResponseDto
    {
        public List<Reporte42ItemDto> Items { get; set; } = new();
        public int TotalVcpu { get; set; }
        public int TotalRam { get; set; }
        public int TotalAlmacenamiento { get; set; }
        public int TotalServidores { get; set; }
    }
}
