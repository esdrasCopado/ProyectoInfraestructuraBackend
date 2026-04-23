using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolicitudServidores.DTOs;
using SolicitudServidores.Services.Interfaces;
using System.Security.Claims;

namespace SolicitudServidores.Controllers
{
    [ApiController]
    [Route("api/solicitud")]
    [Authorize]
    public class SolicitudController : ControllerBase
    {
        private readonly ISolicitudService _service;

        public SolicitudController(ISolicitudService service)
        {
            _service = service;
        }

        // ── Consultas ─────────────────────────────────────────────────────────

        /// <summary>Lista solicitudes. Con ?pagina=N aplica paginación de `cantidad` por página.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pagina   = 0,
            [FromQuery] int cantidad = 20)
            => Ok(await _service.GetAllAsync(pagina, cantidad));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var sol = await _service.GetByIdAsync(id);
            if (sol == null) return NotFound();
            return Ok(sol);
        }

        [HttpGet("folio/{folio}")]
        public async Task<IActionResult> GetByFolio(string folio)
        {
            var sol = await _service.GetByFolioAsync(folio);
            if (sol == null) return NotFound();
            return Ok(sol);
        }

        [HttpGet("dependencia/{dependencyId:int}")]
        public async Task<IActionResult> GetByDependencia(int dependencyId)
            => Ok(await _service.GetByDependencyAsync(dependencyId));

        /// <summary>Filtra por estatus: pendiente | en_validacion | aprovisionado | en_pruebas | publicado | rechazado | finalizado</summary>
        [HttpGet("estatus/{estatus}")]
        public async Task<IActionResult> GetByEstatus(string estatus)
            => Ok(await _service.GetByEstatusAsync(estatus));

        [HttpGet("dashboard/resumen")]
        public async Task<IActionResult> GetDashboard()
            => Ok(await _service.GetDashboardAsync());

        // ── Mutaciones ────────────────────────────────────────────────────────

        /// <summary>Crea una solicitud directa (sin carta responsiva). El folio se genera automáticamente.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSolicitudRequest request)
        {
            var userId = ObtenerUserId();
            if (userId == null)
                return Unauthorized();

            try
            {
                var creada = await _service.CreateAsync(request, userId.Value);
                return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "VALIDATION_FAILED", message = ex.Message });
            }
        }

        /// <summary>Actualización parcial de los campos técnicos de la solicitud.</summary>
        [HttpPatch("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateSolicitudRequest request)
        {
            var userId = ObtenerUserId();
            if (userId == null) return Unauthorized();

            var actualizada = await _service.UpdateAsync(id, request, userId.Value);
            if (actualizada == null) return NotFound();
            return Ok(actualizada);
        }

        /// <summary>RF04 — Cambia el estatus del flujo. Solo administradores de centro de datos.</summary>
        [HttpPatch("{id:long}/estatus")]
        [Authorize(Roles = "Administrador de Centro de Datos,Administrador General")]
        public async Task<IActionResult> ActualizarEstatus(long id, [FromBody] ActualizarEstatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Estatus))
                return BadRequest("El campo 'estatus' es requerido.");

            var userId = ObtenerUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var actualizada = await _service.ActualizarEstatusAsync(id, request, userId.Value);
                if (actualizada == null) return NotFound();
                return Ok(actualizada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "VALIDATION_FAILED", message = ex.Message });
            }
        }

        /// <summary>RF04 — Vincula un servidor aprovisionado a la solicitud.</summary>
        [HttpPatch("{solicitudId:long}/servidor/{serverId:long}")]
        [Authorize(Roles = "Administrador de Infraestructura,Administrador de Centro de Datos,Administrador General")]
        public async Task<IActionResult> AsignarServidor(long solicitudId, long serverId)
        {
            var userId = ObtenerUserId();
            if (userId == null) return Unauthorized();

            var actualizada = await _service.AsignarServidorAsync(solicitudId, serverId, userId.Value);
            if (actualizada == null) return NotFound();
            return Ok(actualizada);
        }

        /// <summary>Soft delete — marca la solicitud como eliminada sin borrarla de la base de datos.</summary>
        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Administrador General")]
        public async Task<IActionResult> Delete(long id)
        {
            var eliminada = await _service.SoftDeleteAsync(id);
            if (eliminada == null) return NotFound();
            return Ok(eliminada);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private long? ObtenerUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("id")?.Value;
            return long.TryParse(claim, out var id) ? id : null;
        }
    }
}
