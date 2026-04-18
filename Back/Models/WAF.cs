using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudServidores.Models
{
    [Table("waf", Schema = "public")]
    public class WAF
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

        [Column("fecha", TypeName = "date")]
        public DateTime? Fecha { get; set; }

        [Column("estado")]
        [MaxLength]
        public string? Estado { get; set; }

        [Column("observaciones")]
        [MaxLength]
        public string? Observaciones { get; set; }
    }
}
