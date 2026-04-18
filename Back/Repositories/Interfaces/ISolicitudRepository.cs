using SolicitudServidores.Helpers;
using SolicitudServidores.Models;
using Microsoft.EntityFrameworkCore;

namespace SolicitudServidores.Repositories.Interfaces
{
    public interface ISolicitudRepository
    {
        Task<Solicitud> Create(Solicitud solicitud);
        Task<Solicitud?> Update(Solicitud solicitud);
        Task<Solicitud?> Delete(long id);
        Task<Solicitud?> GetById(long id);
        Task<bool> ExistsSolicitud(long id);
        Task<List<Solicitud>> GetAll(QueryUserPaging queryUser);
        Task<IEnumerable<Solicitud>> GetAll();
        Task<IEnumerable<Solicitud>> GetAllByUsuario(long id_usuario);

        /// <summary>
        /// Verifica si existe un usuario por su correo
        /// </summary>
        /// <param name="correo">Correo del usuario</param>
        /// <returns>Devuelve true por si existe un usuario con ese correo, si no es false</returns>
        //Task<bool> ExistsUsuario(string correo);
        //IEnumerable<string> GetRoles();

    }
}
