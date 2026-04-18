using Microsoft.EntityFrameworkCore;
using SolicitudServidores.DBContext;
using SolicitudServidores.Helpers;
using SolicitudServidores.Models;
using SolicitudServidores.Repositories.Interfaces;

namespace SolicitudServidores.Repositories.Implementaciones
{
    public class SolicitudRepository : ISolicitudRepository
    {
        private readonly DataContext _context;

        public SolicitudRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Solicitud> Create(Solicitud solicitud)
        {
            await _context.Solicitudes.AddAsync(solicitud);
            await _context.SaveChangesAsync();
            return await GetById(solicitud.Id) ?? solicitud;
        }

        public async Task<Solicitud?> Delete(long id)
        {
            var solicitudModel = await buscarTodo().FirstOrDefaultAsync(c => c.Id == id);
            if (solicitudModel == null)
                return null;

            _context.Solicitudes.Remove(solicitudModel);
            await _context.SaveChangesAsync();
            return solicitudModel;
        }

        public Task<bool> ExistsSolicitud(long id)
        {
            return _context.Solicitudes.AnyAsync(x => x.Id == id);
        }

        public async Task<List<Solicitud>> GetAll(QueryUserPaging query)
        {
            var solicitudes = buscarTodo().OrderByDescending(s => s.Fecha_creacion);
            var skipNumber = (query.NumPage - 1) * query.NumSize;

            return await solicitudes
                .Skip(skipNumber)
                .Take(query.NumSize)
                .ToListAsync();
        }

        private IQueryable<Solicitud> buscarTodo()
        {
            return _context.Solicitudes
                .Include(i => i.Usuario)
                .Include(i => i.Servidores)
                    .ThenInclude(s => s.VPNs)
                .Include(i => i.Servidores)
                    .ThenInclude(s => s.Subdominios)
                .Include(i => i.Servidores)
                    .ThenInclude(s => s.WAFs)
                .Include(i => i.Servidores)
                    .ThenInclude(s => s.EvidenciasPruebas);
        }

        public async Task<IEnumerable<Solicitud>> GetAll()
        {
            return await buscarTodo()
                .OrderByDescending(s => s.Fecha_creacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Solicitud>> GetAllByUsuario(long id_usuario)
        {
            return await buscarTodo()
                .Where(c => c.Id_Usuario == id_usuario)
                .OrderByDescending(s => s.Fecha_creacion)
                .ToListAsync();
        }

        public async Task<Solicitud?> GetById(long id)
        {
            return await buscarTodo()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Solicitud?> Update(Solicitud solicitud)
        {
            _context.ChangeTracker.Clear();

            var solicitudModel = await _context.Solicitudes
                .Include(s => s.Servidores)
                    .ThenInclude(s => s.VPNs)
                .Include(s => s.Servidores)
                    .ThenInclude(s => s.Subdominios)
                .Include(s => s.Servidores)
                    .ThenInclude(s => s.WAFs)
                .Include(s => s.Servidores)
                    .ThenInclude(s => s.EvidenciasPruebas)
                .FirstOrDefaultAsync(x => x.Id == solicitud.Id);

            if (solicitudModel == null)
                return null;

            solicitudModel.Id_Usuario = solicitud.Id_Usuario;
            solicitudModel.Titulo = solicitud.Titulo;
            solicitudModel.Folio = solicitud.Folio;
            solicitudModel.Estado = solicitud.Estado;
            solicitudModel.EtapaActual = solicitud.EtapaActual;
            solicitudModel.Prioridad = solicitud.Prioridad;
            solicitudModel.ResponsableActual = solicitud.ResponsableActual;
            solicitudModel.UsuarioUltimaActualizacion = solicitud.UsuarioUltimaActualizacion;
            solicitudModel.FechaActualizacion = solicitud.FechaActualizacion;
            solicitudModel.FechaRequerida = solicitud.FechaRequerida;
            solicitudModel.CartaResponsivaFolio = solicitud.CartaResponsivaFolio;
            solicitudModel.ComentariosSeguimiento = solicitud.ComentariosSeguimiento;
            // Fecha_creacion no se actualiza: es inmutable una vez creada la solicitud
            solicitudModel.Arquitectura = solicitud.Arquitectura;
            solicitudModel.Descripcion = solicitud.Descripcion;
            solicitudModel.Servicios = solicitud.Servicios;
            solicitudModel.NotificacionNueva = solicitud.NotificacionNueva;
            solicitudModel.TareasPendientes = solicitud.TareasPendientes;

            // Solo reemplaza los servidores si el request envía servidores nuevos (Id = 0).
            // Si el controlador no tocó la colección, los servidores existentes tienen Id > 0
            // y no deben ser borrados ni re-insertados.
            bool hayServidoresNuevos = solicitud.Servidores.Any(s => s.Id == 0);
            if (hayServidoresNuevos)
            {
                foreach (var servidor in solicitudModel.Servidores.ToList())
                {
                    _context.VPNs.RemoveRange(servidor.VPNs);
                    _context.Subdominios.RemoveRange(servidor.Subdominios);
                    _context.WAFs.RemoveRange(servidor.WAFs);
                    _context.EvidenciasPruebas.RemoveRange(servidor.EvidenciasPruebas);
                }
                _context.Servidores.RemoveRange(solicitudModel.Servidores);
                solicitudModel.Servidores = solicitud.Servidores;
            }

            await _context.SaveChangesAsync();
            return await GetById(solicitudModel.Id);
        }
    }
}
