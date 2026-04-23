using SolicitudServidores.DTOs;
using SolicitudServidores.Models;
using SolicitudServidores.Repositories.Interfaces;
using SolicitudServidores.Services.Interfaces;

namespace SolicitudServidores.Services.Implementaciones
{
    public class SeguimientoService : ISeguimientoService
    {
        private readonly ISeguimientoRepository _repo;

        public SeguimientoService(ISeguimientoRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Seguimiento>> GetBySolicitudAsync(long solicitudId)
            => _repo.GetBySolicitudId(solicitudId);

        public Task<Seguimiento?> GetEtapaAsync(long solicitudId, int etapaNumero)
            => _repo.GetByEtapa(solicitudId, etapaNumero);

        public async Task<List<Seguimiento>> InicializarEtapasAsync(long solicitudId)
        {
            // Evitar duplicar etapas si ya fueron inicializadas
            var existentes = await _repo.GetBySolicitudId(solicitudId);
            if (existentes.Any())
                return existentes.ToList();

            return await _repo.InicializarEtapas(solicitudId);
        }

        public async Task<Seguimiento?> AvanzarEtapaAsync(long solicitudId, int etapaNumero, AvanzarEtapaRequest request)
        {
            var etapa = await _repo.GetByEtapa(solicitudId, etapaNumero);
            if (etapa == null) return null;

            var statusValidos = new[] { "en_proceso", "completado", "rechazado" };
            if (!statusValidos.Contains(request.Status))
                throw new ArgumentException($"Status inválido: {request.Status}. Valores permitidos: {string.Join(", ", statusValidos)}");

            if (etapa.FechaInicio == null && request.Status == "en_proceso")
                etapa.FechaInicio = DateTime.UtcNow;

            if (request.Status is "completado" or "rechazado")
                etapa.FechaCompletado = DateTime.UtcNow;

            etapa.Status        = request.Status;
            etapa.Observaciones = request.Observaciones ?? etapa.Observaciones;
            etapa.CompletadoBy  = request.CompletadoBy  ?? etapa.CompletadoBy;

            return await _repo.Update(etapa);
        }
    }
}
