using SolicitudServidores.Back.DTOs;
using SolicitudServidores.DTOs;
using SolicitudServidores.Models;

namespace SolicitudServidores.Services.Interfaces
{
    public interface ISolicitudService
    {
        Task<IEnumerable<Solicitud>> GetAllAsync(int pagina = 0, int cantidad = 20);
        Task<Solicitud?> GetByIdAsync(long id);
        Task<IEnumerable<Solicitud>> GetByUsuarioAsync(long idUsuario);
        Task<DashboardResumenDto> GetDashboardResumenAsync();
        Task<IEnumerable<Solicitud>> GetNotificacionesNuevasAsync();
        Task<Solicitud> CreateAsync(CreateSolicitudRequest request);
        Task<Solicitud?> UpdateAsync(long id, UpdateSolicitudRequest request);
        Task<Solicitud?> ActualizarEstadoAsync(long id, ActualizarEstadoRequest request);
        Task<Solicitud?> MarcarNotificacionLeidaAsync(long id);
        Task<Solicitud?> DeleteAsync(long id);
    }
}
