

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SolicitudServidores.Models
{
    public class PermisoCategoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("categoria")]
        public string Categoria { get; set; } = null!;

        [Required]
        [Column("id_usuario")]
        public long IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual Usuario? Usuario { get; set;}  
    }   
}

