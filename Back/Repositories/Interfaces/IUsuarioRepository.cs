using SolicitudServidores.Helpers;
using SolicitudServidores.Models;
using Microsoft.EntityFrameworkCore;

namespace SolicitudServidores.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Create(Usuario usuario);
        Task<Usuario?> Update(Usuario usuario,List<string> nuevosPermisos);
        Task<Usuario?> UpdateImgUser(long id, string? imgUrl);
        Task<Usuario?> Delete(long id);
        Task<Usuario?> GetById(long id);
        Task<Usuario?> GetByEmail(string email);
        Task<Usuario?> ChangePassword(long id, string password);
        string GenerateNewPassword();
        Task<List<Usuario>> GetAll(QueryUserPaging queryUser);
        Task<List<Usuario>> GetAllForEmail(QueryUserPaging queryUser);
        Task<List<Usuario>> GetAllForName(QueryUserPaging queryUser);
        Task<List<Usuario>> GetAll();


        /// <summary>
        /// Verifica si existe un usuario por su correo
        /// </summary>
        /// <param name="correo">Correo del usuario</param>
        /// <returns>Devuelve true por si existe un usuario con ese correo, si no es false</returns>
        Task<bool> ExistsUsuario(string correo);
        IEnumerable<string> GetRoles();
        IEnumerable<object> GetRolesConDescripcion();

    }
}
