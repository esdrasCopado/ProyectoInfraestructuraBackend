using Microsoft.AspNetCore.Mvc;
using SolicitudServidores.Back.DTOs;
using SolicitudServidores.DTOs;
using SolicitudServidores.Helpers;
using SolicitudServidores.Models;
using SolicitudServidores.Repositories.Interfaces;
using SolicitudServidores.Utilities;

namespace SolicitudServidores.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioController(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryUserPaging query)
        {
            var usuarios = await _repo.GetAll(query);
            return Ok(usuarios.Select(MapToDto));
        }

        [HttpGet("todos")]
        public async Task<IActionResult> GetAllSinPaginacion()
        {
            var usuarios = await _repo.GetAll();
            return Ok(usuarios.Select(MapToDto));
        }

        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            return Ok(_repo.GetRoles());
        }

        [HttpGet("roles/descripcion")]
        public IActionResult GetRolesConDescripcion()
        {
            return Ok(_repo.GetRolesConDescripcion());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var usuario = await _repo.GetById(id);
            if (usuario == null) return NotFound();
            return Ok(MapToDto(usuario));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NombreCompleto) ||
                string.IsNullOrWhiteSpace(request.Correo) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Nombre, correo y contraseña son requeridos.");
            }

            if (await _repo.ExistsUsuario(request.Correo))
                return Conflict("Ya existe un usuario con ese correo.");

            var rol = string.IsNullOrWhiteSpace(request.Rol)
                ? (request.Permisos ?? "Dependencia / Cliente")
                : request.Rol;

            var usuario = new Usuario
            {
                NombreCompleto = request.NombreCompleto.Trim(),
                Correo = request.Correo.Trim(),
                Password = Encriptar.EncriptarSHA256(request.Password),
                Permisos = request.Permisos ?? rol ?? "Dependencia / Cliente",
                Rol = rol ?? "Dependencia / Cliente",
                Puesto = request.Puesto,
                Celular = request.Celular,
                NumeroPuesto = request.NumeroPuesto
            };

            var creado = await _repo.Create(usuario);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, MapToDto(creado));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateUsuarioRequest request)
        {
            var existente = await _repo.GetById(id);
            if (existente == null) return NotFound();

            existente.NombreCompleto = request.NombreCompleto ?? existente.NombreCompleto;
            existente.Correo = request.Correo ?? existente.Correo;
            existente.Permisos = request.Permisos ?? existente.Permisos;
            existente.Rol = request.Rol ?? request.Permisos ?? existente.Rol;
            existente.Puesto = request.Puesto ?? existente.Puesto;
            existente.Celular = request.Celular ?? existente.Celular;
            existente.NumeroPuesto = request.NumeroPuesto ?? existente.NumeroPuesto;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                existente.Password = Encriptar.EncriptarSHA256(request.Password);
            }

            var permisos = request.PermisosCategoria
                ?? existente.PermisoCategorias?.Select(p => p.Categoria).ToList()
                ?? new List<string>();

            var actualizado = await _repo.Update(existente, permisos);
            if (actualizado == null) return NotFound();

            return Ok(MapToDto(actualizado));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var eliminado = await _repo.Delete(id);
            if (eliminado == null) return NotFound();
            return Ok(MapToDto(eliminado));
        }

        private static UsuarioDTO MapToDto(Usuario u) => new()
        {
            Id = u.Id,
            NombreCompleto = u.NombreCompleto,
            Rol = string.IsNullOrWhiteSpace(u.Rol) ? u.Permisos : u.Rol,
            Permisos = u.Permisos,
            Correo = u.Correo,
            Puesto = u.Puesto,
            Celular = u.Celular,
            NumeroPuesto = u.NumeroPuesto,
            ImagenUrl = u.ImagenUrl,
            PermisosCategoria = u.PermisoCategorias?.Select(p => p.Categoria).ToList() ?? new()
        };
    }
}
