using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudServidores.Models
{
    [Table("vpn", Schema = "public")]
    public class VPN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("id_servidor")]
        public long Id_servidor { get; set; }

        [ForeignKey(nameof(Id_servidor))]
        public virtual Servidor? Servidor { get; set; }

        public long? Id_usuario_Responsable { get; set; }

        public virtual Usuario? Usuario { get; set; }

        [Column("tipo")]
        [MaxLength]
        public string Tipo { get; set; } = string.Empty;

        [Column("fecha_asignacion", TypeName = "date")]
        public DateTime? Fecha_asignacion { get; set; }

        [Column("fecha_expiracion", TypeName = "date")]
        public DateTime? Fecha_Expiracion { get; set; }

        [Column("estado")]
        [MaxLength]
        public string? Estado { get; set; }

        [Column("folio")]
        [MaxLength]
        public string Folio { get; set; } = string.Empty;
    }
}
