using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudServidores.Models
{
    [Table("solicitud", Schema = "public")]
    public class Solicitud
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("id_usuario")]
        public long? Id_Usuario { get; set; }

        [ForeignKey(nameof(Id_Usuario))]
        public virtual Usuario? Usuario { get; set; }

        [Column("titulo")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Column("folio")]
        [StringLength(80)]
        public string Folio { get; set; } = string.Empty;

        [Column("etapa_actual")]
        [StringLength(120)]
        public string EtapaActual { get; set; } = "Registro";

        [Column("prioridad")]
        [StringLength(40)]
        public string Prioridad { get; set; } = "Media";

        [Column("responsable_actual")]
        [StringLength(150)]
        public string? ResponsableActual { get; set; }

        [Column("usuario_ultima_actualizacion")]
        [StringLength(150)]
        public string? UsuarioUltimaActualizacion { get; set; }

        [Column("fecha_actualizacion", TypeName = "date")]
        public DateTime? FechaActualizacion { get; set; }

        [Column("fecha_requerida", TypeName = "date")]
        public DateTime? FechaRequerida { get; set; }

        [Column("carta_responsiva_folio")]
        [StringLength(80)]
        public string? CartaResponsivaFolio { get; set; }

        [Column("comentarios_seguimiento", TypeName = "text")]
        public string? ComentariosSeguimiento { get; set; }

        [Column("Estado")]
        [StringLength(120)]
        public string Estado { get; set; } = "Pendiente";

        [Column("Fecha_Creacion", TypeName = "date")]
        public DateTime? Fecha_creacion { get; set; } = DateTime.UtcNow;

        [Column("Arquitectura")]
        [StringLength(200)]
        public string Arquitectura { get; set; } = string.Empty;

        [Column("Descripcion", TypeName = "text")]
        public string? Descripcion { get; set; }

        [Column("Servicios")]
        [StringLength(300)]
        public string Servicios { get; set; } = string.Empty;

        [Column("notificacion_nueva")]
        public bool NotificacionNueva { get; set; } = true;

        [Column("tareas_pendientes", TypeName = "text")]
        public string? TareasPendientes { get; set; }

        public virtual ICollection<Servidor> Servidores { get; set; } = new List<Servidor>();
    }
}
