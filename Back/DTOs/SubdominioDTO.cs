
using System.ComponentModel.DataAnnotations.Schema;
namespace AgendaContactosSGD.DTOs.EventoDTOs
    
{
    public class SubdominioDTO
    {
        public long Id { get; set; }
        public long Id_servidor { get; set; }
        public long Id_usuario_responsable { get; set; }
        public string Nombre_URL { get; set; } = null!;
        public DateTime? Fecha_asignacion { get; set; }
        public DateTime? Fecha_Expiracion { get; set; }
        public string? Estado { get; set; }

    }
}
