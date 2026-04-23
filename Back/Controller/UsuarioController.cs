using Microsoft.AspNetCore.Mvc;
using SolicitudServidores.Back.DTOs;
using SolicitudServidores.DTOs;
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
        public async Task<IActionResult> GetAll([FromQuery] Helpers.QueryUserPaging query)
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
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _repo.GetRoles());
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
            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Apellidos) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Nombre, apellidos, correo y contraseña son requeridos.");
            }

            if (await _repo.ExistsUsuario(request.Email.Trim()))
                return Conflict("Ya existe un usuario con ese correo.");

            var usuario = new Usuario
            {
                Nombre         = request.Nombre.Trim(),
                Apellidos      = request.Apellidos.Trim(),
                Email          = request.Email.Trim().ToLower(),
                PasswordHash   = Encriptar.EncriptarSHA256(request.Password),
                RoleId         = request.RoleId,
                DependencyId   = request.DependencyId,
                NumeroEmpleado = request.NumeroEmpleado?.Trim(),
                Cargo          = request.Cargo?.Trim(),
                Phone          = request.Phone?.Trim(),
                Activo         = true
            };

            var creado = await _repo.Create(usuario);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, MapToDto(creado));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateUsuarioRequest request)
        {
            var existente = await _repo.GetById(id);
            if (existente == null) return NotFound();

            existente.Nombre         = request.Nombre?.Trim()         ?? existente.Nombre;
            existente.Apellidos      = request.Apellidos?.Trim()      ?? existente.Apellidos;
            existente.Email          = request.Email?.Trim().ToLower() ?? existente.Email;
            existente.RoleId         = request.RoleId                 ?? existente.RoleId;
            existente.DependencyId   = request.DependencyId           ?? existente.DependencyId;
            existente.NumeroEmpleado = request.NumeroEmpleado?.Trim() ?? existente.NumeroEmpleado;
            existente.Cargo          = request.Cargo?.Trim()          ?? existente.Cargo;
            existente.Phone          = request.Phone?.Trim()          ?? existente.Phone;
            existente.Activo         = request.Activo                 ?? existente.Activo;

            if (!string.IsNullOrWhiteSpace(request.Password))
                existente.PasswordHash = Encriptar.EncriptarSHA256(request.Password);

            var actualizado = await _repo.Update(existente);
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

        [HttpPatch("{id}/password")]
        public async Task<IActionResult> ChangePassword(long id, [FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("La contraseña no puede estar vacía.");

            var actualizado = await _repo.ChangePassword(id, Encriptar.EncriptarSHA256(request.Password));
            if (actualizado == null) return NotFound();
            return Ok(MapToDto(actualizado));
        }

        private static UsuarioDTO MapToDto(Usuario u) => new()
        {
            Id             = u.Id,
            Nombre         = u.Nombre,
            Apellidos      = u.Apellidos,
            RoleId         = u.RoleId,
            RolNombre      = u.Rol?.Nombre ?? string.Empty,
            DependencyId   = u.DependencyId,
            Email          = u.Email,
            NumeroEmpleado = u.NumeroEmpleado,
            Cargo          = u.Cargo,
            Phone          = u.Phone,
            Activo         = u.Activo,
            LastLoginAt    = u.LastLoginAt,
            CreatedAt      = u.CreatedAt
        };
    }

    public record ChangePasswordRequest(string Password);
}
