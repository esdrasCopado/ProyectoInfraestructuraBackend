using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudServidores.Models
{
    [Table("cartas", Schema = "public")]
    public class Carta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("folio")]
        public string Folio { get; set; } = null!;

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("id_solicitud")]
        public long? SolicitudId { get; set; }

        [Column("solicitud_folio")]
        public string? SolicitudFolio { get; set; }

        // ── Area Requirente ───────────────────────────────────────────────────
        [Column("ar_sector")]       public string AR_Sector { get; set; } = null!;
        [Column("ar_dependencia")]  public string AR_Dependencia { get; set; } = null!;
        [Column("ar_responsable")]  public string AR_Responsable { get; set; } = null!;
        [Column("ar_cargo")]        public string AR_Cargo { get; set; } = null!;
        [Column("ar_telefono")]     public string AR_Telefono { get; set; } = null!;
        [Column("ar_correo")]       public string AR_Correo { get; set; } = null!;

        // ── Admin Servidor ────────────────────────────────────────────────────
        [Column("as_proveedor")]    public string AS_Proveedor { get; set; } = null!;
        [Column("as_dependencia")]  public string AS_Dependencia { get; set; } = null!;
        [Column("as_responsable")]  public string AS_Responsable { get; set; } = null!;
        [Column("as_cargo")]        public string AS_Cargo { get; set; } = null!;
        [Column("as_telefono")]     public string AS_Telefono { get; set; } = null!;
        [Column("as_correo")]       public string AS_Correo { get; set; } = null!;

        // ── Descripcion ───────────────────────────────────────────────────────
        [Column("desc_descripcion_servidor")]       public string DESC_DescripcionServidor { get; set; } = null!;
        [Column("desc_nombre_servidor")]            public string DESC_NombreServidor { get; set; } = null!;
        [Column("desc_nombre_aplicacion")]          public string DESC_NombreAplicacion { get; set; } = null!;
        [Column("desc_tipo_uso")]                   public string DESC_TipoUso { get; set; } = null!;
        [Column("desc_fecha_arranque")]             public string DESC_FechaArranque { get; set; } = null!;
        [Column("desc_vigencia")]                   public string DESC_Vigencia { get; set; } = null!;
        [Column("desc_caracteristicas_especiales")] public string? DESC_CaracteristicasEspeciales { get; set; }

        // ── Specs ─────────────────────────────────────────────────────────────
        [Column("specs_tipo_requerimiento")]        public string SPECS_TipoRequerimiento { get; set; } = null!;
        [Column("specs_arquitectura")]              public string? SPECS_Arquitectura { get; set; }
        [Column("specs_modalidad")]                 public string SPECS_Modalidad { get; set; } = null!;
        [Column("specs_sistema_operativo")]         public string SPECS_SistemaOperativo { get; set; } = null!;
        [Column("specs_sistema_operativo_otro")]    public string? SPECS_SistemaOperativoOtro { get; set; }
        [Column("specs_vcores")]                    public int SPECS_VCores { get; set; }
        [Column("specs_memoria_ram")]               public int SPECS_MemoriaRam { get; set; }
        [Column("specs_almacenamiento")]            public int SPECS_Almacenamiento { get; set; }
        /// <summary>JSON: array de { capacidad, tipo, etiqueta }.</summary>
        [Column("specs_discos_duros")]              public string? SPECS_DiscosDuros { get; set; }
        [Column("specs_motor_bd")]                  public string? SPECS_MotorBD { get; set; }
        [Column("specs_puertos")]                   public string? SPECS_Puertos { get; set; }
        [Column("specs_integraciones")]             public string? SPECS_Integraciones { get; set; }
        [Column("specs_otras_specs")]               public string? SPECS_OtrasSpecs { get; set; }
        // Solo para modalidad "renovacion"
        [Column("specs_ip_actual")]                 public string? SPECS_IpActual { get; set; }
        [Column("specs_nombre_servidor_actual")]    public string? SPECS_NombreServidorActual { get; set; }
        [Column("specs_tipo_renovacion")]           public string? SPECS_TipoRenovacion { get; set; }

        // ── Infraestructura ───────────────────────────────────────────────────
        /// <summary>JSON: array de strings (URLs de subdominios).</summary>
        [Column("infra_subdominios")]               public string? INFRA_Subdominios { get; set; }
        /// <summary>Primer subdominio (retrocompatibilidad).</summary>
        [Column("infra_subdominio_solicitado")]     public string? INFRA_SubdominioSolicitado { get; set; }
        [Column("infra_puerto")]                    public string? INFRA_Puerto { get; set; }
        [Column("infra_requiere_ssl")]              public bool INFRA_RequiereSSL { get; set; }
        /// <summary>JSON: array de VpnCartaDTO.</summary>
        [Column("infra_vpns")]                      public string? INFRA_Vpns { get; set; }
        // Primer VPN descompuesta (retrocompatibilidad — nullable)
        [Column("infra_vpn_responsable")]           public string? INFRA_VpnResponsable { get; set; }
        [Column("infra_vpn_cargo")]                 public string? INFRA_VpnCargo { get; set; }
        [Column("infra_vpn_telefono")]              public string? INFRA_VpnTelefono { get; set; }
        [Column("infra_vpn_correo")]                public string? INFRA_VpnCorreo { get; set; }

        // ── Responsiva ────────────────────────────────────────────────────────
        [Column("resp_firmante")]        public string RESP_Firmante { get; set; } = null!;
        [Column("resp_num_empleado")]    public string RESP_NumEmpleado { get; set; } = null!;
        [Column("resp_puesto_firmante")] public string RESP_PuestoFirmante { get; set; } = null!;
        [Column("resp_acepta_terminos")] public bool RESP_AceptaTerminos { get; set; }
    }
}
