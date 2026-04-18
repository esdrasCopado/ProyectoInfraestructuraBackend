using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudServidores.Models
{
    [Table("subdominio", Schema = "public")]
    public class Subdominio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("id_servidor")]
        public long? Id_Servidor { get; set; }

        [ForeignKey(nameof(Id_Servidor))]
        public virtual Servidor? Servidor { get; set; }

        [Column("id_usuario")]
        public long? Id_usuario { get; set; }

        [ForeignKey(nameof(Id_usuario))]
        public virtual Usuario? Usuario { get; set; }

        [Column("nombre_url")]
        [MaxLength]
        public string Nombre_url { get; set; } = string.Empty;

        [Column("fecha_asignacion", TypeName = "date")]
        public DateTime? Fecha_asignacion { get; set; }

        [Column("fecha_expiracion", TypeName = "date")]
        public DateTime? Fecha_Expiracion { get; set; }

        [Column("estado")]
        [MaxLength]
        public string? Estado { get; set; }
    }
}
