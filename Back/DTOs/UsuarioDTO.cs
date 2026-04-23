namespace SolicitudServidores.DTOs
{
    public class UsuarioDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();
        public int RoleId { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public int? DependencyId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? NumeroEmpleado { get; set; }
        public string? Cargo { get; set; }
        public string? Phone { get; set; }
        public bool Activo { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
