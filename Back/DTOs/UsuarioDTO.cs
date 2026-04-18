namespace SolicitudServidores.DTOs
{
    public class UsuarioDTO
    {
        public long Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Permisos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Puesto { get; set; }
        public string? Celular { get; set; }
        public string? NumeroPuesto { get; set; }
        public string? ImagenUrl { get; set; }
        public List<string> PermisosCategoria { get; set; } = new();
    }
}
