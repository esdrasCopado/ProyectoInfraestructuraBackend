using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolicitudServidores.Back.DTOs;
using SolicitudServidores.Services.Interfaces;

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

        // ──────────────────────────────────────────────
        // Consultas
        // ──────────────────────────────────────────────

        /// <summary>
        /// Devuelve todas las solicitudes. Sin parámetros retorna la lista completa;
        /// con ?pagina=N aplica paginación de 20 registros por página.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pagina = 0,
            [FromQuery] int cantidad = 20)
        {
            return Ok(await _service.GetAllAsync(pagina, cantidad));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var solicitud = await _service.GetByIdAsync(id);
            if (solicitud == null) return NotFound();
            return Ok(solicitud);
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> GetByUsuario(long idUsuario)
        {
            return Ok(await _service.GetByUsuarioAsync(idUsuario));
        }

        [HttpGet("dashboard/resumen")]
        public async Task<IActionResult> GetDashboardResumen()
        {
            return Ok(await _service.GetDashboardResumenAsync());
        }

        [HttpGet("notificaciones/nuevas")]
        public async Task<IActionResult> GetNotificacionesNuevas()
        {
            return Ok(await _service.GetNotificacionesNuevasAsync());
        }

        // ──────────────────────────────────────────────
        // Mutaciones
        // ──────────────────────────────────────────────

        /// <summary>RF02 — Solo la Dependencia/Cliente o el Administrador General pueden crear solicitudes.</summary>
        [HttpPost]
        [Authorize(Roles = "Dependencia / Cliente,Administrador General")]
        public async Task<IActionResult> Create([FromBody] CreateSolicitudRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                return BadRequest("El título de la solicitud es requerido.");

            var creada = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }

        /// <summary>Actualización parcial de campos de la solicitud (PATCH semántico).</summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateSolicitudRequest request)
        {
            var actualizada = await _service.UpdateAsync(id, request);
            if (actualizada == null) return NotFound();
            return Ok(actualizada);
        }

        /// <summary>RF04 — Solo el Administrador de Centro de Datos o el Administrador General cambian el estado.</summary>
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Administrador de Centro de Datos,Administrador General")]
        public async Task<IActionResult> ActualizarEstado(long id, [FromBody] ActualizarEstadoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Estado))
                return BadRequest("El estado es requerido.");

            var actualizada = await _service.ActualizarEstadoAsync(id, request);
            if (actualizada == null) return NotFound();
            return Ok(actualizada);
        }

        /// <summary>Marca la notificación de la solicitud como leída.</summary>
        [HttpPatch("{id}/notificacion-leida")]
        public async Task<IActionResult> MarcarNotificacionLeida(long id)
        {
            var actualizada = await _service.MarcarNotificacionLeidaAsync(id);
            if (actualizada == null) return NotFound();
            return Ok(actualizada);
        }

        /// <summary>RF01 — Solo el Administrador General puede eliminar solicitudes.</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador General")]
        public async Task<IActionResult> Delete(long id)
        {
            var eliminada = await _service.DeleteAsync(id);
            if (eliminada == null) return NotFound();
            return Ok(eliminada);
        }
    }
}
