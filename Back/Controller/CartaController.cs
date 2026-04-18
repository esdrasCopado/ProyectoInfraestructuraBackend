using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolicitudServidores.DBContext;
using SolicitudServidores.DTOs;
using SolicitudServidores.Models;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SolicitudServidores.Controllers
{
    [ApiController]
    [Route("api/cartas")]
    public class CartaController : ControllerBase
    {
        private readonly DataContext _context;

        private static readonly Regex EmailRegex    = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex TelefonoRegex = new(@"^\(\d{3}\) \d{3}-\d{4}$",    RegexOptions.Compiled);

        private static readonly HashSet<string> TiposUso          = new(StringComparer.OrdinalIgnoreCase) { "interno", "publicado" };
        private static readonly HashSet<string> TiposRequerimiento = new(StringComparer.OrdinalIgnoreCase) { "estandar", "especial" };
        private static readonly HashSet<string> Arquitecturas      = new(StringComparer.OrdinalIgnoreCase) { "virtual", "fisica", "hibrida", "nube" };
        private static readonly HashSet<string> Modalidades        = new(StringComparer.OrdinalIgnoreCase) { "nuevo", "renovacion", "clonacion", "serverBase" };
        private static readonly HashSet<string> SistemasOperativos = new(StringComparer.OrdinalIgnoreCase) { "windows", "linux", "otro" };
        private static readonly HashSet<string> TiposVpn          = new(StringComparer.OrdinalIgnoreCase)
        {
            "Usuario VPN de dependencia",
            "Usuario VPN para proveedor",
            "Actualizacion de usuario VPN"
        };
        private static readonly HashSet<string> TiposDisco        = new(StringComparer.OrdinalIgnoreCase) { "SSD", "HDD", "NVMe" };

        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public CartaController(DataContext context) => _context = context;

        // ──────────────────────────────────────────────────────────────────────
        // GET /api/cartas
        // ──────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cartas = await _context.Cartas
                .OrderByDescending(c => c.CreadoEn)
                .ToListAsync();
            return Ok(cartas);
        }

        // ──────────────────────────────────────────────────────────────────────
        // GET /api/cartas/{id}
        // ──────────────────────────────────────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var carta = await _context.Cartas.FindAsync(id);
            if (carta == null) return NotFound();
            return Ok(carta);
        }

        // ──────────────────────────────────────────────────────────────────────
        // GET /api/cartas/{id}/pdf
        // ──────────────────────────────────────────────────────────────────────
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadPdf(long id)
        {
            var carta = await _context.Cartas.FindAsync(id);
            if (carta == null) return NotFound();
            return File(BuildSimplePdf(carta), "application/pdf", $"Carta-{carta.Folio}.pdf");
        }

        // ──────────────────────────────────────────────────────────────────────
        // POST /api/cartas
        // ──────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartaRequestDTO dto)
        {
            var error = Validar(dto);
            if (error != null) return BadRequest(error);

            // ── Generar folio de carta ──────────────────────────────────────
            var year  = DateTime.UtcNow.Year;
            var count = await _context.Cartas.CountAsync(c => c.CreadoEn.Year == year);
            var folio = $"ATDT-{year}-{(count + 1):D4}";

            if (await _context.Cartas.AnyAsync(c => c.Folio == folio))
                return Conflict(new { error = "DUPLICATE_FOLIO", message = $"El folio {folio} ya existe." });

            // ── Derivar datos útiles ────────────────────────────────────────
            var specs   = dto.Specs!;
            var desc    = dto.Descripcion!;
            var infra   = dto.Infraestructura!;
            var ar      = dto.AreaRequirente!;
            var as_     = dto.AdminServidor!;
            var resp    = dto.Responsiva!;

            DateTime? fechaRequerida = DateTime.TryParse(desc.FechaArranque, out var fa) ? fa.Date : null;

            // Almacenamiento total: suma de discos si se envían, si no el campo plano
            int almacenamientoTotal = specs.DiscosDuros?.Sum(d => d.Capacidad) is int suma && suma > 0
                ? suma
                : specs.Almacenamiento;

            // Primer responsable de VPN (para campos legacy y para responsableActual)
            var primeraVpn = infra.Vpns?.FirstOrDefault();

            // ── Crear Solicitud + Servidor ──────────────────────────────────
            var solicitud = new Solicitud
            {
                Titulo = string.IsNullOrWhiteSpace(desc.NombreAplicacion)
                    ? desc.NombreServidor ?? "Solicitud desde carta responsiva"
                    : desc.NombreAplicacion,
                Folio             = $"SOL-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                Estado            = "Pendiente",
                EtapaActual       = "Carta responsiva",
                Prioridad         = "Media",
                ResponsableActual = as_.Responsable ?? ar.Responsable,
                FechaRequerida    = fechaRequerida,
                Fecha_creacion    = DateTime.UtcNow,
                Arquitectura      = specs.TipoRequerimiento ?? string.Empty,
                Descripcion       = desc.DescripcionServidor,
                Servicios         = specs.MotorBD ?? specs.Integraciones ?? "Infraestructura",
                CartaResponsivaFolio       = folio,
                ComentariosSeguimiento     = desc.CaracteristicasEspeciales,
                TareasPendientes           = "Validar carta responsiva, aprovisionar servidor y completar checklist operativo.",
                NotificacionNueva          = true,
                Servidores = new List<Servidor>
                {
                    new()
                    {
                        Estado       = "Pendiente",
                        Hostname     = desc.NombreServidor ?? string.Empty,
                        TipoUso      = string.Equals(desc.TipoUso, "publicado", StringComparison.OrdinalIgnoreCase) ? "Publicado" : "Interno",
                        Funcion      = desc.NombreAplicacion ?? desc.DescripcionServidor ?? string.Empty,
                        SistemaOperativo  = specs.SistemaOperativoOtro ?? specs.SistemaOperativo ?? string.Empty,
                        Nucleos           = specs.VCores,
                        Ram               = specs.MemoriaRam,
                        Almacenamiento    = almacenamientoTotal,
                        Descripcion       = desc.DescripcionServidor,
                        PlantillaRecursos = string.Equals(specs.TipoRequerimiento, "estandar", StringComparison.OrdinalIgnoreCase) ? "Estándar" : "General",
                        EtapaOperativa    = "Provisionamiento",
                        ResponsableInfraestructura = as_.Responsable,
                        TareasPendientes           = "Asignar recursos, validar conectividad, VPN, WAF y evidencias.",
                        ObservacionesSeguimiento   = desc.CaracteristicasEspeciales,
                        RequiereRevisionAnual      = true,
                        SolicitudPublicacion       = infra.Subdominios?.Count > 0,

                        // Una VPN por cada entrada en la lista
                        VPNs = infra.Vpns?.Select(v => new VPN
                        {
                            Tipo   = v.TipoVpn ?? "VPN solicitada por carta",
                            Estado = "Pendiente",
                            Folio  = $"VPN-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}"
                        }).ToList() ?? new List<VPN>(),

                        // Un Subdominio por cada URL
                        Subdominios = infra.Subdominios?.Select(url => new Subdominio
                        {
                            Nombre_url = url,
                            Estado     = "Solicitado"
                        }).ToList() ?? new List<Subdominio>()
                    }
                }
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            // ── Crear registro de Carta ─────────────────────────────────────
            var carta = new Carta
            {
                Folio          = folio,
                CreadoEn       = DateTime.UtcNow,
                SolicitudId    = solicitud.Id,
                SolicitudFolio = solicitud.Folio,

                // Area Requirente
                AR_Sector      = ar.Sector!,
                AR_Dependencia = ar.Dependencia!,
                AR_Responsable = ar.Responsable!,
                AR_Cargo       = ar.Cargo!,
                AR_Telefono    = ar.Telefono!,
                AR_Correo      = ar.Correo!,

                // Admin Servidor
                AS_Proveedor   = as_.Proveedor!,
                AS_Dependencia = as_.Dependencia!,
                AS_Responsable = as_.Responsable!,
                AS_Cargo       = as_.Cargo!,
                AS_Telefono    = as_.Telefono!,
                AS_Correo      = as_.Correo!,

                // Descripcion
                DESC_DescripcionServidor      = desc.DescripcionServidor!,
                DESC_NombreServidor           = desc.NombreServidor!,
                DESC_NombreAplicacion         = desc.NombreAplicacion!,
                DESC_TipoUso                  = desc.TipoUso!,
                DESC_FechaArranque            = desc.FechaArranque!,
                DESC_Vigencia                 = desc.Vigencia!,
                DESC_CaracteristicasEspeciales = desc.CaracteristicasEspeciales,

                // Specs
                SPECS_TipoRequerimiento     = specs.TipoRequerimiento!,
                SPECS_Arquitectura          = specs.Arquitectura,
                SPECS_Modalidad             = specs.Modalidad!,
                SPECS_SistemaOperativo      = specs.SistemaOperativo!,
                SPECS_SistemaOperativoOtro  = specs.SistemaOperativoOtro,
                SPECS_VCores                = specs.VCores,
                SPECS_MemoriaRam            = specs.MemoriaRam,
                SPECS_Almacenamiento        = almacenamientoTotal,
                SPECS_DiscosDuros           = specs.DiscosDuros != null
                    ? JsonSerializer.Serialize(specs.DiscosDuros, JsonOpts)
                    : null,
                SPECS_MotorBD               = specs.MotorBD,
                SPECS_Puertos               = specs.Puertos,
                SPECS_Integraciones         = specs.Integraciones,
                SPECS_OtrasSpecs            = specs.OtrasSpecs,
                SPECS_IpActual              = specs.IpActual,
                SPECS_NombreServidorActual  = specs.NombreServidorActual,
                SPECS_TipoRenovacion        = specs.TipoRenovacion,

                // Infraestructura
                INFRA_Subdominios          = infra.Subdominios != null
                    ? JsonSerializer.Serialize(infra.Subdominios, JsonOpts)
                    : null,
                INFRA_SubdominioSolicitado = infra.Subdominios?.FirstOrDefault(),
                INFRA_Puerto               = infra.Puerto,
                INFRA_RequiereSSL          = infra.RequiereSSL,
                INFRA_Vpns                 = infra.Vpns != null
                    ? JsonSerializer.Serialize(infra.Vpns, JsonOpts)
                    : null,
                // Primer VPN descompuesta (retrocompatibilidad)
                INFRA_VpnResponsable = primeraVpn?.VpnResponsable,
                INFRA_VpnCargo       = primeraVpn?.VpnCargo,
                INFRA_VpnTelefono    = primeraVpn?.VpnTelefono,
                INFRA_VpnCorreo      = primeraVpn?.VpnCorreo,

                // Responsiva
                RESP_Firmante       = resp.Firmante!,
                RESP_NumEmpleado    = resp.NumEmpleado!,
                RESP_PuestoFirmante = resp.PuestoFirmante!,
                RESP_AceptaTerminos = resp.AceptaTerminos
            };

            _context.Cartas.Add(carta);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                cartaId        = carta.Id,
                folio          = carta.Folio,
                solicitudId    = carta.SolicitudId,
                solicitudFolio = carta.SolicitudFolio
            });
        }

        // ──────────────────────────────────────────────────────────────────────
        // Validaciones
        // ──────────────────────────────────────────────────────────────────────

        private object? Validar(CartaRequestDTO dto)
        {
            if (dto.AreaRequirente == null)  return Error("areaRequirente",  "El objeto areaRequirente es requerido.");
            if (dto.AdminServidor == null)   return Error("adminServidor",   "El objeto adminServidor es requerido.");
            if (dto.Descripcion == null)     return Error("descripcion",     "El objeto descripcion es requerido.");
            if (dto.Specs == null)           return Error("specs",           "El objeto specs es requerido.");
            if (dto.Infraestructura == null) return Error("infraestructura", "El objeto infraestructura es requerido.");
            if (dto.Responsiva == null)      return Error("responsiva",      "El objeto responsiva es requerido.");

            // areaRequirente
            var ar = dto.AreaRequirente;
            if (string.IsNullOrWhiteSpace(ar.Sector))      return Error("areaRequirente.sector",      "El campo sector es requerido.");
            if (string.IsNullOrWhiteSpace(ar.Dependencia)) return Error("areaRequirente.dependencia", "El campo dependencia es requerido.");
            if (string.IsNullOrWhiteSpace(ar.Responsable)) return Error("areaRequirente.responsable", "El campo responsable es requerido.");
            if (string.IsNullOrWhiteSpace(ar.Cargo))       return Error("areaRequirente.cargo",       "El campo cargo es requerido.");
            if (string.IsNullOrWhiteSpace(ar.Telefono))    return Error("areaRequirente.telefono",    "El campo telefono es requerido.");
            if (!TelefonoRegex.IsMatch(ar.Telefono!))      return Error("areaRequirente.telefono",    "Formato inválido. Usar (DDD) DDD-DDDD.");
            if (string.IsNullOrWhiteSpace(ar.Correo))      return Error("areaRequirente.correo",      "El campo correo es requerido.");
            if (!EmailRegex.IsMatch(ar.Correo!))           return Error("areaRequirente.correo",      "El correo del área requirente no tiene un formato válido.");

            // adminServidor
            var as_ = dto.AdminServidor;
            if (string.IsNullOrWhiteSpace(as_.Proveedor))   return Error("adminServidor.proveedor",   "El campo proveedor es requerido.");
            if (string.IsNullOrWhiteSpace(as_.Dependencia)) return Error("adminServidor.dependencia", "El campo dependencia es requerido.");
            if (string.IsNullOrWhiteSpace(as_.Responsable)) return Error("adminServidor.responsable", "El campo responsable es requerido.");
            if (string.IsNullOrWhiteSpace(as_.Cargo))       return Error("adminServidor.cargo",       "El campo cargo es requerido.");
            if (string.IsNullOrWhiteSpace(as_.Telefono))    return Error("adminServidor.telefono",    "El campo telefono es requerido.");
            if (!TelefonoRegex.IsMatch(as_.Telefono!))      return Error("adminServidor.telefono",    "Formato inválido. Usar (DDD) DDD-DDDD.");
            if (string.IsNullOrWhiteSpace(as_.Correo))      return Error("adminServidor.correo",      "El campo correo es requerido.");
            if (!EmailRegex.IsMatch(as_.Correo!))           return Error("adminServidor.correo",      "El correo del administrador no tiene un formato válido.");

            // descripcion
            var desc = dto.Descripcion;
            if (string.IsNullOrWhiteSpace(desc.DescripcionServidor)) return Error("descripcion.descripcionServidor", "El campo descripcionServidor es requerido.");
            if (string.IsNullOrWhiteSpace(desc.NombreServidor))      return Error("descripcion.nombreServidor",      "El campo nombreServidor es requerido.");
            if (string.IsNullOrWhiteSpace(desc.NombreAplicacion))    return Error("descripcion.nombreAplicacion",    "El campo nombreAplicacion es requerido.");
            if (string.IsNullOrWhiteSpace(desc.FechaArranque))       return Error("descripcion.fechaArranque",       "El campo fechaArranque es requerido.");
            if (string.IsNullOrWhiteSpace(desc.Vigencia))            return Error("descripcion.vigencia",            "El campo vigencia es requerido.");
            if (string.IsNullOrWhiteSpace(desc.TipoUso) || !TiposUso.Contains(desc.TipoUso!))
                return Error("descripcion.tipoUso", "El campo tipoUso debe ser 'interno' o 'publicado'.");

            // specs
            var sp = dto.Specs;
            if (string.IsNullOrWhiteSpace(sp.TipoRequerimiento) || !TiposRequerimiento.Contains(sp.TipoRequerimiento!))
                return Error("specs.tipoRequerimiento", "El campo tipoRequerimiento debe ser 'estandar' o 'especial'.");
            if (!string.IsNullOrWhiteSpace(sp.Arquitectura) && !Arquitecturas.Contains(sp.Arquitectura!))
                return Error("specs.arquitectura", "El campo arquitectura debe ser 'virtual', 'fisica', 'hibrida' o 'nube'.");
            if (string.IsNullOrWhiteSpace(sp.Modalidad) || !Modalidades.Contains(sp.Modalidad!))
                return Error("specs.modalidad", "El campo modalidad debe ser 'nuevo', 'renovacion', 'clonacion' o 'serverBase'.");
            if (string.IsNullOrWhiteSpace(sp.SistemaOperativo) || !SistemasOperativos.Contains(sp.SistemaOperativo!))
                return Error("specs.sistemaOperativo", "El campo sistemaOperativo debe ser 'windows', 'linux' o 'otro'.");
            if (sp.VCores < 1)    return Error("specs.vCores",    "El campo vCores debe ser mayor o igual a 1.");
            if (sp.MemoriaRam < 1) return Error("specs.memoriaRam", "El campo memoriaRam debe ser mayor o igual a 1.");

            // Almacenamiento: aceptar si viene en DiscosDuros O en el campo plano
            bool tieneDiscos = sp.DiscosDuros?.Count > 0;
            if (!tieneDiscos && sp.Almacenamiento < 1)
                return Error("specs.almacenamiento", "El campo almacenamiento debe ser mayor o igual a 1 cuando no se envían discosDuros.");
            if (tieneDiscos)
            {
                foreach (var (disco, idx) in sp.DiscosDuros!.Select((d, i) => (d, i)))
                {
                    if (disco.Capacidad < 1)
                        return Error($"specs.discosDuros[{idx}].capacidad", "La capacidad del disco debe ser mayor o igual a 1.");
                    if (!string.IsNullOrWhiteSpace(disco.Tipo) && !TiposDisco.Contains(disco.Tipo!))
                        return Error($"specs.discosDuros[{idx}].tipo", "El tipo de disco debe ser 'SSD', 'HDD' o 'NVMe'.");
                }
            }

            // Campos de renovacion
            if (string.Equals(sp.Modalidad, "renovacion", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(sp.IpActual))
                    return Error("specs.ipActual", "El campo ipActual es requerido para modalidad 'renovacion'.");
                if (string.IsNullOrWhiteSpace(sp.NombreServidorActual))
                    return Error("specs.nombreServidorActual", "El campo nombreServidorActual es requerido para modalidad 'renovacion'.");
            }

            // infraestructura — VPNs (opcional pero si viene, validar formato)
            var infra = dto.Infraestructura;
            if (infra.Vpns != null)
            {
                foreach (var (vpn, idx) in infra.Vpns.Select((v, i) => (v, i)))
                {
                    if (!string.IsNullOrWhiteSpace(vpn.TipoVpn) && !TiposVpn.Contains(vpn.TipoVpn!))
                        return Error($"infraestructura.vpns[{idx}].tipoVpn",
                            "El tipoVpn debe ser 'Usuario VPN de dependencia', 'Usuario VPN para proveedor' o 'Actualizacion de usuario VPN'.");
                    if (!string.IsNullOrWhiteSpace(vpn.VpnTelefono) && !TelefonoRegex.IsMatch(vpn.VpnTelefono!))
                        return Error($"infraestructura.vpns[{idx}].vpnTelefono", "Formato inválido. Usar (DDD) DDD-DDDD.");
                    if (!string.IsNullOrWhiteSpace(vpn.VpnCorreo) && !EmailRegex.IsMatch(vpn.VpnCorreo!))
                        return Error($"infraestructura.vpns[{idx}].vpnCorreo", "El correo de VPN no tiene un formato válido.");
                }
            }

            // responsiva
            var resp = dto.Responsiva;
            if (!resp.AceptaTerminos)                             return Error("responsiva.aceptaTerminos",  "Se deben aceptar los términos para continuar.");
            if (string.IsNullOrWhiteSpace(resp.Firmante))        return Error("responsiva.firmante",        "El campo firmante es requerido.");
            if (string.IsNullOrWhiteSpace(resp.NumEmpleado))     return Error("responsiva.numEmpleado",     "El campo numEmpleado es requerido.");
            if (string.IsNullOrWhiteSpace(resp.PuestoFirmante))  return Error("responsiva.puestoFirmante",  "El campo puestoFirmante es requerido.");

            return null;
        }

        private static object Error(string campo, string message) => new
        {
            error   = "VALIDATION_FAILED",
            message,
            campo
        };

        // ──────────────────────────────────────────────────────────────────────
        // PDF mínimo (texto plano)
        // ──────────────────────────────────────────────────────────────────────

        private static byte[] BuildSimplePdf(Carta carta)
        {
            var lines = new[]
            {
                $"Carta Responsiva de Aprovisionamiento - {carta.Folio}",
                $"Solicitud: {carta.SolicitudFolio ?? "Sin folio asociado"}",
                $"Dependencia: {carta.AR_Dependencia}",
                $"Responsable: {carta.AR_Responsable}",
                $"Administrador del servidor: {carta.AS_Responsable}",
                $"Servidor: {carta.DESC_NombreServidor}",
                $"Aplicación: {carta.DESC_NombreAplicacion}",
                $"Tipo de uso: {carta.DESC_TipoUso}",
                $"Fecha de arranque: {carta.DESC_FechaArranque}",
                $"Vigencia: {carta.DESC_Vigencia}",
                $"Arquitectura: {carta.SPECS_Arquitectura ?? "N/D"}",
                $"vCores: {carta.SPECS_VCores}  RAM: {carta.SPECS_MemoriaRam} GB  Almacenamiento: {carta.SPECS_Almacenamiento} GB",
                $"Subdominio(s): {carta.INFRA_SubdominioSolicitado ?? "N/D"}",
                $"Firmante: {carta.RESP_Firmante}"
            };

            var contentBuilder = new StringBuilder();
            contentBuilder.AppendLine("BT");
            contentBuilder.AppendLine("/F1 12 Tf");
            contentBuilder.AppendLine("72 780 Td");
            foreach (var raw in lines)
            {
                contentBuilder.AppendLine($"({EscapePdfText(raw)}) Tj");
                contentBuilder.AppendLine("0 -18 Td");
            }
            contentBuilder.AppendLine("ET");

            var stream  = contentBuilder.ToString();
            var objects = new List<string>
            {
                "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj",
                "2 0 obj << /Type /Pages /Count 1 /Kids [3 0 R] >> endobj",
                "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj",
                $"4 0 obj << /Length {Encoding.ASCII.GetByteCount(stream)} >> stream\n{stream}endstream endobj",
                "5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj"
            };

            var pdf = new StringBuilder();
            pdf.AppendLine("%PDF-1.4");
            var offsets = new List<int>();
            foreach (var obj in objects)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
                pdf.AppendLine(obj);
            }

            var xrefPos = Encoding.ASCII.GetByteCount(pdf.ToString());
            pdf.AppendLine($"xref\n0 {objects.Count + 1}");
            pdf.AppendLine("0000000000 65535 f ");
            foreach (var off in offsets) pdf.AppendLine($"{off:D10} 00000 n ");
            pdf.AppendLine($"trailer << /Size {objects.Count + 1} /Root 1 0 R >>");
            pdf.AppendLine("startxref");
            pdf.AppendLine(xrefPos.ToString());
            pdf.Append("%%EOF");

            return Encoding.ASCII.GetBytes(pdf.ToString());
        }

        private static string EscapePdfText(string text) =>
            text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }
}
