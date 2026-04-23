using SolicitudServidores.DTOs;
using SolicitudServidores.Models;

namespace SolicitudServidores.Services.Interfaces
{
    public interface ISeguimientoService
    {
        Task<IEnumerable<Seguimiento>> GetBySolicitudAsync(long solicitudId);
        Task<Seguimiento?> GetEtapaAsync(long solicitudId, int etapaNumero);
        Task<List<Seguimiento>> InicializarEtapasAsync(long solicitudId);
        Task<Seguimiento?> AvanzarEtapaAsync(long solicitudId, int etapaNumero, AvanzarEtapaRequest request);
    }
}
