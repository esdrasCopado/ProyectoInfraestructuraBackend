using System;

namespace SolicitudServidores.Back.Models
{
    public class Usuario2
    {
        //int
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? PasswordHash { get; set; }
        public string? Rol { get; set; } // "Solicitante", "Revisor", "Admin"
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
