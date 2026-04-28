using Microsoft.EntityFrameworkCore;
using SolicitudServidores.DBContext;
using SolicitudServidores.Models;
using SolicitudServidores.Repositories.Interfaces;

namespace SolicitudServidores.Repositories.Implementaciones
{
    public class SeguimientoRepository : ISeguimientoRepository
    {
        private readonly DataContext _context;

        private static readonly (int Numero, string Nombre)[] EtapasCanónicas =
        {
            (1,  "carta_responsiva"),
            (2,  "validacion_recursos"),
            (3,  "creacion_servidor"),
            (4,  "comunicaciones"),
            (5,  "parches"),
            (6,  "xdr_agente"),
            (7,  "vpn"),
            (8,  "subdominio"),
            (9,  "credenciales"),
            (10, "waf"),
            (11, "evidencias"),
            (12, "validacion_evidencias"),
            (13, "solicitud_publicacion"),
            (14, "vulnerabilidades"),
        };

        public SeguimientoRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seguimiento>> GetBySolicitudId(long solicitudId)
        {
            return await _context.Seguimientos
                .Include(s => s.CompletadoPor)
                .Where(s => s.SolicitudId == solicitudId)
                .OrderBy(s => s.EtapaNumero)
                .ToListAsync();
        }

        public async Task<Seguimiento?> GetByEtapa(long solicitudId, int etapaNumero)
        {
            return await _context.Seguimientos
                .Include(s => s.CompletadoPor)
                .FirstOrDefaultAsync(s => s.SolicitudId == solicitudId && s.EtapaNumero == etapaNumero);
        }

        public async Task<int?> GetEtapaActual(long solicitudId)
        {
            var seguimiento = await _context.Seguimientos
                .Include(s => s.CompletadoPor)
                .FirstOrDefaultAsync(s => s.SolicitudId == solicitudId);

            //si no se encuentra un seguimiento con el id insertado
            //se manda -1 para significar que llego nulo
            if (seguimiento == null) return -1; 

            return seguimiento.EtapaNumero;
        }

        public async Task<Seguimiento?> UpdateEtapa(long solicitudId, int etapa)
        {
            var seguimiento = await _context.Seguimientos
                .Include(s => s.CompletadoPor)
                .FirstOrDefaultAsync(s => s.SolicitudId == solicitudId);

            if (seguimiento == null) return null;
            seguimiento.EtapaNumero = etapa;

            await _context.SaveChangesAsync();

            return await GetById(seguimiento.SeguimientoId);
        }

        public async Task<Seguimiento?> UpdateStatus(long solicitudId, string status)
        {
            var seguimiento = await _context.Seguimientos
                .Include(s => s.CompletadoPor)
                .FirstOrDefaultAsync(s => s.SolicitudId == solicitudId);

            if (seguimiento == null) return null;
            seguimiento.Status = status;

            await _context.SaveChangesAsync();

            return await GetById(seguimiento.SeguimientoId);
        }

        public async Task<Seguimiento?> GetById(long id)
        {
            return await _context.Seguimientos
                .Include(s => s.CompletadoPor)
                .FirstOrDefaultAsync(s => s.SeguimientoId == id);
        }

        public async Task<List<Seguimiento>> InicializarEtapas(long solicitudId)
        {
            var etapas = EtapasCanónicas.Select(e => new Seguimiento
            {
                SolicitudId  = solicitudId,
                EtapaNumero  = e.Numero,
                EtapaNombre  = e.Nombre,
                Status       = "pendiente",
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow,
            }).ToList();

            await _context.Seguimientos.AddRangeAsync(etapas);
            await _context.SaveChangesAsync();
            return etapas;
        }

        public async Task<Seguimiento?> Update(Seguimiento seguimiento)
        {
            var existente = await _context.Seguimientos
                .FirstOrDefaultAsync(s => s.SeguimientoId == seguimiento.SeguimientoId);

            if (existente == null) return null;

            existente.Status           = seguimiento.Status;
            existente.FechaInicio      = seguimiento.FechaInicio;
            existente.FechaCompletado  = seguimiento.FechaCompletado;
            existente.CompletadoBy     = seguimiento.CompletadoBy;
            existente.Observaciones    = seguimiento.Observaciones;
            existente.UpdatedAt        = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetById(existente.SeguimientoId);
        }
    }
}
