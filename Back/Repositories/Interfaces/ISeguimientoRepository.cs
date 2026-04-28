using SolicitudServidores.Models;

namespace SolicitudServidores.Repositories.Interfaces
{
    public interface ISeguimientoRepository
    {
        Task<IEnumerable<Seguimiento>> GetBySolicitudId(long solicitudId);
        Task<Seguimiento?> GetByEtapa(long solicitudId, int etapaNumero);
        Task<int?> GetEtapaActual(long solicitudId);
        Task<Seguimiento?> UpdateEtapa(long solicitudId, int etapa);
        Task<Seguimiento?> UpdateStatus(long solicitudId, string status);
        Task<Seguimiento?> GetById(long id);
        Task<List<Seguimiento>> InicializarEtapas(long solicitudId);
        Task<Seguimiento?> Update(Seguimiento seguimiento);
    }
}
