using Microsoft.EntityFrameworkCore;
using SolicitudServidores.DBContext;
using SolicitudServidores.Helpers;
using SolicitudServidores.Models;
using SolicitudServidores.Models.Enum;
using SolicitudServidores.Repositories.Interfaces;

namespace SolicitudServidores.Repositories.Implementaciones
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DataContext _context;

        public UsuarioRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Usuario> Create(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return await GetById(usuario.Id) ?? usuario;
        }

        public async Task<Usuario?> Delete(long id)
        {
            var usuarioModel = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (usuarioModel == null)
                return null;

            _context.Usuarios.Remove(usuarioModel);
            await _context.SaveChangesAsync();
            return usuarioModel;
        }

        public Task<bool> ExistsUsuario(string correo)
        {
            return _context.Usuarios.AnyAsync(x => x.Correo == correo);
        }

        public async Task<List<Usuario>> GetAll(QueryUserPaging query)
        {
            var usuarios = _context.Usuarios
                .Include(p => p.PermisoCategorias)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Role))
            {
                usuarios = FilterForRole(usuarios, query.Role);
            }

            var skipNumber = (query.NumPage - 1) * query.NumSize;
            return await usuarios
                .OrderBy(u => u.Id)
                .Skip(skipNumber)
                .Take(query.NumSize)
                .ToListAsync();
        }

        public async Task<List<Usuario>> GetAll()
        {
            return await _context.Usuarios
                .Include(p => p.PermisoCategorias)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public async Task<List<Usuario>> GetAllForEmail(QueryUserPaging query)
        {
            var usuarios = _context.Usuarios
                .Include(p => p.PermisoCategorias)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Role))
            {
                usuarios = FilterForRole(usuarios, query.Role);
            }

            var skipNumber = (query.NumPage - 1) * query.NumSize;
            return await usuarios
                .OrderBy(u => u.Correo)
                .Skip(skipNumber)
                .Take(query.NumSize)
                .ToListAsync();
        }

        public async Task<List<Usuario>> GetAllForName(QueryUserPaging query)
        {
            var usuarios = _context.Usuarios
                .Include(p => p.PermisoCategorias)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Role))
            {
                usuarios = FilterForRole(usuarios, query.Role);
            }

            var skipNumber = (query.NumPage - 1) * query.NumSize;
            return await usuarios
                .OrderBy(u => u.NombreCompleto)
                .Skip(skipNumber)
                .Take(query.NumSize)
                .ToListAsync();
        }

        public async Task<Usuario?> GetById(long id)
        {
            return await _context.Usuarios
                .Include(p => p.PermisoCategorias)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public IEnumerable<string> GetRoles()
        {
            return RolExtensions.ObtenerTodosLosRoles();
        }

        public IEnumerable<object> GetRolesConDescripcion()
        {
            return RolExtensions.ObtenerTodosLosRolesConDescripcion();
        }

        public async Task<Usuario?> Update(Usuario usuario, List<string> nuevosPermisos)
        {
            var usuarioModel = await _context.Usuarios
                .Include(u => u.PermisoCategorias)
                .FirstOrDefaultAsync(x => x.Id == usuario.Id);

            if (usuarioModel == null)
                return null;

            var viejosPermisos = usuarioModel.PermisoCategorias.ToList();
            if (viejosPermisos.Count > 0)
            {
                _context.PermisoCategorias.RemoveRange(viejosPermisos);
            }

            usuarioModel.NombreCompleto = usuario.NombreCompleto;
            usuarioModel.Correo = usuario.Correo;
            usuarioModel.Permisos = usuario.Permisos;
            usuarioModel.Rol = string.IsNullOrWhiteSpace(usuario.Rol) ? usuario.Permisos : usuario.Rol;
            usuarioModel.Puesto = usuario.Puesto;
            usuarioModel.Celular = usuario.Celular;
            usuarioModel.NumeroPuesto = usuario.NumeroPuesto;

            if (!string.IsNullOrWhiteSpace(usuario.Password))
            {
                usuarioModel.Password = usuario.Password;
            }

            usuarioModel.PermisoCategorias = nuevosPermisos
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(permiso => new PermisoCategoria
                {
                    Categoria = permiso,
                    IdUsuario = usuarioModel.Id
                })
                .ToList();

            await _context.SaveChangesAsync();
            return await GetById(usuarioModel.Id);
        }

        public async Task<Usuario?> GetByEmail(string email)
        {
            return await _context.Usuarios
                .Include(p => p.PermisoCategorias)
                .FirstOrDefaultAsync(u => u.Correo == email);
        }

        public async Task<Usuario?> UpdateImgUser(long id, string? imgUrl)
        {
            var usuarioModel = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (usuarioModel == null)
                return null;

            usuarioModel.ImagenUrl = imgUrl;
            await _context.SaveChangesAsync();
            return usuarioModel;
        }

        private IQueryable<Usuario> FilterForRole(IQueryable<Usuario> usuarios, string role)
        {
            return usuarios.Where(u =>
                u.Permisos.Contains(role) ||
                u.Rol.Contains(role));
        }

        public async Task<Usuario?> ChangePassword(long id, string password)
        {
            var usuarioModel = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (usuarioModel == null)
                return null;

            usuarioModel.Password = password;
            await _context.SaveChangesAsync();
            return usuarioModel;
        }

        public string GenerateNewPassword()
        {
            return RandomString(8);
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
