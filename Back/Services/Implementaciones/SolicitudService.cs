using SolicitudServidores.DTOs;
using SolicitudServidores.Models;
using SolicitudServidores.Repositories.Interfaces;
using SolicitudServidores.Services.Interfaces;

namespace SolicitudServidores.Services.Implementaciones
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _repo;

        private static readonly HashSet<string> EstatusValidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "pendiente", "en_validacion", "aprovisionado",
            "en_pruebas", "publicado", "rechazado", "finalizado"
        };

        public SolicitudService(ISolicitudRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Solicitud>> GetAllAsync(int pagina = 0, int cantidad = 20)
        {
            if (pagina <= 0)
                return await _repo.GetAll();

            return await _repo.GetAllPaged(pagina, cantidad);
        }

        public Task<Solicitud?> GetByIdAsync(long id) => _repo.GetById(id);

        public Task<Solicitud?> GetByFolioAsync(string folio) => _repo.GetByFolio(folio);

        public Task<IEnumerable<Solicitud>> GetByDependencyAsync(int dependencyId)
            => _repo.GetByDependency(dependencyId);

        public Task<IEnumerable<Solicitud>> GetByEstatusAsync(string estatus)
            => _repo.GetByEstatus(estatus);

        public async Task<SolicitudDashboardDto> GetDashboardAsync()
        {
            var todas = (await _repo.GetAll()).ToList();

            return new SolicitudDashboardDto
            {
                Total = todas.Count,

                PorEstatus = todas
                    .GroupBy(s => s.Estatus)
                    .Select(g => new EstatusCountDto { Estatus = g.Key, Total = g.Count() })
                    .OrderByDescending(x => x.Total),

                PorTipoUso = todas
                    .GroupBy(s => string.IsNullOrWhiteSpace(s.TipoUso) ? "sin_tipo" : s.TipoUso)
                    .Select(g => new TipoUsoCountDto { TipoUso = g.Key, Total = g.Count() }),

                TopDependencias = todas
                    .GroupBy(s => s.Dependency?.Name ?? "Sin dependencia")
                    .Select(g => new DependenciaCountDto { Dependencia = g.Key, Total = g.Count() })
                    .OrderByDescending(x => x.Total)
                    .Take(10),
            };
        }

        public async Task<Solicitud> CreateAsync(CreateSolicitudRequest request, long createdBy)
        {
            ValidarRequest(request);

            var folio = GenerarFolio();

            // Garantizar unicidad del folio (colisión muy improbable pero posible)
            while (await _repo.ExistsFolio(folio))
                folio = GenerarFolio();

            var solicitud = new Solicitud
            {
                Folio                    = folio,
                DependencyId             = request.DependencyId,
                AdminContactId           = request.AdminContactId,
                DescripcionUso           = request.DescripcionUso.Trim(),
                NombreServidor           = request.NombreServidor.Trim(),
                NombreAplicacion         = request.NombreAplicacion?.Trim(),
                TipoUso                  = request.TipoUso.Trim().ToLower(),
                FechaArranqueDeseada     = request.FechaArranqueDeseada,
                VigenciaMeses            = request.VigenciaMeses > 0 ? request.VigenciaMeses : 12,
                CaracteristicasEspeciales = request.CaracteristicasEspeciales?.Trim(),
                TipoRequerimiento        = request.TipoRequerimiento.Trim().ToLower(),
                EsClonacion              = request.EsClonacion,
                IpServidorBase           = request.IpServidorBase?.Trim(),
                NombreServidorBase       = request.NombreServidorBase?.Trim(),
                SistemaOperativo         = request.SistemaOperativo?.Trim(),
                RamSolicitadaGb          = request.RamSolicitadaGb,
                VcpuSolicitado           = request.VcpuSolicitado,
                AlmacenamientoSolicitadoGb = request.AlmacenamientoSolicitadoGb,
                MotorBaseDatos           = request.MotorBaseDatos?.Trim(),
                ReglasFirewall           = request.ReglasFirewall?.Trim(),
                IntegracionesExternas    = request.IntegracionesExternas?.Trim(),
                ConectividadOtras        = request.ConectividadOtras?.Trim(),
                Estatus                  = "pendiente",
                CreatedBy                = createdBy,
                CreatedAt                = DateTime.UtcNow,
                UpdatedAt                = DateTime.UtcNow,
            };

            return await _repo.Create(solicitud);
        }

        public async Task<Solicitud?> UpdateAsync(long id, UpdateSolicitudRequest request, long updatedBy)
        {
            var existente = await _repo.GetById(id);
            if (existente == null) return null;

            existente.AdminContactId            = request.AdminContactId           ?? existente.AdminContactId;
            existente.DescripcionUso            = request.DescripcionUso?.Trim()   ?? existente.DescripcionUso;
            existente.NombreServidor            = request.NombreServidor?.Trim()   ?? existente.NombreServidor;
            existente.NombreAplicacion          = request.NombreAplicacion?.Trim() ?? existente.NombreAplicacion;
            existente.TipoUso                   = request.TipoUso?.Trim().ToLower() ?? existente.TipoUso;
            existente.FechaArranqueDeseada      = request.FechaArranqueDeseada     ?? existente.FechaArranqueDeseada;
            existente.VigenciaMeses             = request.VigenciaMeses            ?? existente.VigenciaMeses;
            existente.CaracteristicasEspeciales = request.CaracteristicasEspeciales?.Trim() ?? existente.CaracteristicasEspeciales;
            existente.TipoRequerimiento         = request.TipoRequerimiento?.Trim().ToLower() ?? existente.TipoRequerimiento;
            existente.EsClonacion               = request.EsClonacion              ?? existente.EsClonacion;
            existente.IpServidorBase            = request.IpServidorBase?.Trim()   ?? existente.IpServidorBase;
            existente.NombreServidorBase        = request.NombreServidorBase?.Trim() ?? existente.NombreServidorBase;
            existente.SistemaOperativo          = request.SistemaOperativo?.Trim() ?? existente.SistemaOperativo;
            existente.RamSolicitadaGb           = request.RamSolicitadaGb          ?? existente.RamSolicitadaGb;
            existente.VcpuSolicitado            = request.VcpuSolicitado           ?? existente.VcpuSolicitado;
            existente.AlmacenamientoSolicitadoGb = request.AlmacenamientoSolicitadoGb ?? existente.AlmacenamientoSolicitadoGb;
            existente.MotorBaseDatos            = request.MotorBaseDatos?.Trim()   ?? existente.MotorBaseDatos;
            existente.ReglasFirewall            = request.ReglasFirewall?.Trim()   ?? existente.ReglasFirewall;
            existente.IntegracionesExternas     = request.IntegracionesExternas?.Trim() ?? existente.IntegracionesExternas;
            existente.ConectividadOtras         = request.ConectividadOtras?.Trim() ?? existente.ConectividadOtras;
            existente.UpdatedBy                 = updatedBy;

            return await _repo.Update(existente);
        }

        public async Task<Solicitud?> ActualizarEstatusAsync(long id, ActualizarEstatusRequest request, long updatedBy)
        {
            if (!EstatusValidos.Contains(request.Estatus))
                throw new ArgumentException(
                    $"Estatus inválido: '{request.Estatus}'. Valores permitidos: {string.Join(", ", EstatusValidos)}");

            var existente = await _repo.GetById(id);
            if (existente == null) return null;

            existente.Estatus   = request.Estatus.ToLower();
            existente.UpdatedBy = updatedBy;

            return await _repo.Update(existente);
        }

        public async Task<Solicitud?> AsignarServidorAsync(long solicitudId, long serverId, long updatedBy)
        {
            var existente = await _repo.GetById(solicitudId);
            if (existente == null) return null;

            return await _repo.AsignarServidor(solicitudId, serverId);
        }

        public Task<Solicitud?> SoftDeleteAsync(long id) => _repo.SoftDelete(id);

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string GenerarFolio()
            => $"SOL-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}";

        private static void ValidarRequest(CreateSolicitudRequest r)
        {
            if (r.DependencyId <= 0)
                throw new ArgumentException("El campo 'dependencyId' es requerido.");
            if (string.IsNullOrWhiteSpace(r.DescripcionUso))
                throw new ArgumentException("El campo 'descripcionUso' es requerido.");
            if (string.IsNullOrWhiteSpace(r.NombreServidor))
                throw new ArgumentException("El campo 'nombreServidor' es requerido.");
            if (r.TipoUso != "interno" && r.TipoUso != "publicado")
                throw new ArgumentException("El campo 'tipoUso' debe ser 'interno' o 'publicado'.");
            if (r.RamSolicitadaGb < 1)
                throw new ArgumentException("El campo 'ramSolicitadaGb' debe ser >= 1.");
            if (r.VcpuSolicitado < 1)
                throw new ArgumentException("El campo 'vcpuSolicitado' debe ser >= 1.");
            if (r.AlmacenamientoSolicitadoGb < 1)
                throw new ArgumentException("El campo 'almacenamientoSolicitadoGb' debe ser >= 1.");
        }
    }
}
