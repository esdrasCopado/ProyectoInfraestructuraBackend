using SolicitudServidores.Models;

namespace SolicitudServidores.Repositories.Interfaces
{
    public interface ISeguimientoRepository
    {
        Task<IEnumerable<Seguimiento>> GetBySolicitudId(long solicitudId);
        Task<Seguimiento?> GetByEtapa(long solicitudId, int etapaNumero);
        Task<Seguimiento?> GetById(long id);
        Task<List<Seguimiento>> InicializarEtapas(long solicitudId);
        Task<Seguimiento?> Update(Seguimiento seguimiento);
    }
}
