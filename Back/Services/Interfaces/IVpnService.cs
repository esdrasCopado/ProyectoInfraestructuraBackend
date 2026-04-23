using SolicitudServidores.DTOs;
using SolicitudServidores.Models;

namespace SolicitudServidores.Services.Interfaces
{
    public interface IVpnService
    {
        Task<IEnumerable<VPN>> GetAllAsync();
        Task<VPN?> GetByIdAsync(int id);
        Task<IEnumerable<VPN>> GetByServidorAsync(long serverId);
        Task<VPN> CreateAsync(CreateVpnRequest request);
        Task<VPN?> UpdateAsync(int id, UpdateVpnRequest request);
        Task<VPN?> DeleteAsync(int id);
        Task<ServerVpn> AsignarAServidorAsync(int vpnId, long serverId);
        Task<bool> DesasignarDeServidorAsync(int vpnId, long serverId);
    }
}
