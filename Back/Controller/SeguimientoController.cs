using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolicitudServidores.DTOs;
using SolicitudServidores.Services.Interfaces;

namespace SolicitudServidores.Controllers
{
    [ApiController]
    [Route("api/seguimiento")]
    [Authorize]
    public class SeguimientoController : ControllerBase
    {
        private readonly ISeguimientoService _service;

        public SeguimientoController(ISeguimientoService service)
        {
            _service = service;
        }

        /// <summary>Devuelve las 14 etapas de una solicitud ordenadas por número.</summary>
        [HttpGet("solicitud/{solicitudId}")]
        public async Task<IActionResult> GetBySolicitud(long solicitudId)
        {
            return Ok(await _service.GetBySolicitudAsync(solicitudId));
        }

        /// <summary>Devuelve una etapa específica.</summary>
        [HttpGet("solicitud/{solicitudId}/etapa/{etapaNumero}")]
        public async Task<IActionResult> GetEtapa(long solicitudId, int etapaNumero)
        {
            var etapa = await _service.GetEtapaAsync(solicitudId, etapaNumero);
            if (etapa == null) return NotFound();
            return Ok(etapa);
        }

        /// <summary>RF02 — Crea las 14 etapas en estado 'pendiente' al registrar una solicitud.</summary>
        [HttpPost("solicitud/{solicitudId}/inicializar")]
        [Authorize(Roles = "Administrador de Centro de Datos,Administrador General")]
        public async Task<IActionResult> Inicializar(long solicitudId)
        {
            var etapas = await _service.InicializarEtapasAsync(solicitudId);
            return Ok(etapas);
        }

        /// <summary>Avanza el estado de una etapa (en_proceso | completado | rechazado).</summary>
        [HttpPatch("solicitud/{solicitudId}/etapa/{etapaNumero}")]
        public async Task<IActionResult> AvanzarEtapa(
            long solicitudId,
            int etapaNumero,
            [FromBody] AvanzarEtapaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("El campo 'status' es requerido.");

            try
            {
                var etapa = await _service.AvanzarEtapaAsync(solicitudId, etapaNumero, request);
                if (etapa == null) return NotFound();
                return Ok(etapa);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
