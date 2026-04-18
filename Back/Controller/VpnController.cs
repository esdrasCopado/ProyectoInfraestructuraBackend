using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolicitudServidores.DBContext;

namespace SolicitudServidores.Controllers
{
    [ApiController]
    [Route("api/vpn")]
    [Authorize]
    public class VpnController : ControllerBase
    {
        private readonly DataContext _context;

        public VpnController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// CA-01 — Lista todas las VPNs asociadas al usuario responsable.
        /// GET /api/vpn?idUsuario=5
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetByUsuario([FromQuery] long idUsuario)
        {
            var vpns = await _context.VPNs
                .Where(v => v.Id_usuario_Responsable == idUsuario)
                .Include(v => v.Servidor)
                    .ThenInclude(s => s!.Solicitud)
                .OrderByDescending(v => v.Fecha_asignacion)
                .Select(v => new
                {
                    v.Id,
                    v.Folio,
                    v.Tipo,
                    v.Estado,
                    v.Fecha_asignacion,
                    v.Fecha_Expiracion,
                    v.Id_usuario_Responsable,
                    Servidor = v.Servidor == null ? null : new
                    {
                        v.Servidor.Id,
                        v.Servidor.Hostname,
                        v.Servidor.Ip,
                        v.Servidor.Estado,
                        Solicitud = v.Servidor.Solicitud == null ? null : new
                        {
                            v.Servidor.Solicitud.Id,
                            v.Servidor.Solicitud.Folio,
                            v.Servidor.Solicitud.Titulo,
                            v.Servidor.Solicitud.Estado
                        }
                    }
                })
                .ToListAsync();

            return Ok(vpns);
        }

        /// <summary>
        /// CA-02 — Busca una VPN por su folio.
        /// GET /api/vpn/folio/VPN-20240415-123456-789
        /// </summary>
        [HttpGet("folio/{folio}")]
        public async Task<IActionResult> GetByFolio(string folio)
        {
            var vpn = await _context.VPNs
                .Where(v => v.Folio == folio)
                .Include(v => v.Servidor)
                    .ThenInclude(s => s!.Solicitud)
                .Include(v => v.Usuario)
                .Select(v => new
                {
                    v.Id,
                    v.Folio,
                    v.Tipo,
                    v.Estado,
                    v.Fecha_asignacion,
                    v.Fecha_Expiracion,
                    v.Id_usuario_Responsable,
                    UsuarioResponsable = v.Usuario == null ? null : new
                    {
                        v.Usuario.Id,
                        v.Usuario.NombreCompleto,
                        v.Usuario.Correo
                    },
                    Servidor = v.Servidor == null ? null : new
                    {
                        v.Servidor.Id,
                        v.Servidor.Hostname,
                        v.Servidor.Ip,
                        v.Servidor.SistemaOperativo,
                        v.Servidor.Estado,
                        v.Servidor.EtapaOperativa,
                        Solicitud = v.Servidor.Solicitud == null ? null : new
                        {
                            v.Servidor.Solicitud.Id,
                            v.Servidor.Solicitud.Folio,
                            v.Servidor.Solicitud.Titulo,
                            v.Servidor.Solicitud.Estado,
                            v.Servidor.Solicitud.EtapaActual
                        }
                    }
                })
                .FirstOrDefaultAsync();

            if (vpn == null) return NotFound(new { message = $"No se encontró ninguna VPN con el folio '{folio}'." });
            return Ok(vpn);
        }
    }
}
