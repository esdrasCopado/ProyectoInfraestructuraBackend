using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudServidores.Models
{
    [Table("usuarios", Schema = "public")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("nombre_completo")]
        [StringLength(120)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [Column("rol")]
        [StringLength(80)]
        public string Rol { get; set; } = "Dependencia / Cliente";

        [Required]
        [Column("permisos", TypeName = "text")]
        public string Permisos { get; set; } = "Dependencia / Cliente";

        [Required]
        [Column("correo")]
        [StringLength(80)]
        public string Correo { get; set; } = string.Empty;

        [Column("puesto")]
        [StringLength(120)]
        public string? Puesto { get; set; }

        [Column("celular")]
        [StringLength(30)]
        public string? Celular { get; set; }

        [Column("numero_puesto")]
        [StringLength(40)]
        public string? NumeroPuesto { get; set; }

        [Column("imagen", TypeName = "text")]
        public string? ImagenUrl { get; set; }

        [Required]
        [Column("contrasena")]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        public virtual List<PermisoCategoria> PermisoCategorias { get; set; } = new();
    }
}
