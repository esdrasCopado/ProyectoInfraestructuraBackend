using SolicitudServidores.DTOs;
using SolicitudServidores.Models;

namespace SolicitudServidores.Services.Interfaces
{
    public interface ISeguimientoService
    {
        Task<IEnumerable<Seguimiento>> GetBySolicitudAsync(long solicitudId);
        Task<Seguimiento?> GetEtapaAsync(long solicitudId, int etapaNumero);
        Task<int?> GetEtapaActualAsync(long solicitudId);
        Task<Seguimiento?> UpdateEtapaAsync(long solicitudId, int etapa);

        Task<Seguimiento?> UpdateStatusAsync(long solicitudId, string status);
        Task<List<Seguimiento>> InicializarEtapasAsync(long solicitudId);
        Task<Seguimiento?> AvanzarEtapaAsync(long solicitudId, int etapaNumero, AvanzarEtapaRequest request);
    }
}
