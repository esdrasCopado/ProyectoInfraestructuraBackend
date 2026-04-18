using Microsoft.AspNetCore.Mvc;
using SolicitudServidores.Back.DTOs;
using SolicitudServidores.DTOs;
using SolicitudServidores.Helpers;
using SolicitudServidores.Repositories.Interfaces;
using SolicitudServidores.Utilities;

namespace SolicitudServidores.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly CrearJWT _crearJWT;

        public AuthController(IUsuarioRepository usuarioRepo, CrearJWT crearJWT)
        {
            _usuarioRepo = usuarioRepo;
            _crearJWT = crearJWT;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Correo y contraseña son requeridos.");

            var usuario = await _usuarioRepo.GetByEmail(request.Email);
            if (usuario == null)
                return Unauthorized("Credenciales inválidas.");

            var passwordHash = Encriptar.EncriptarSHA256(request.Password);
            if (usuario.Password != passwordHash)
                return Unauthorized("Credenciales inválidas.");

            var token = _crearJWT.GenerarToken(usuario);

            var userDto = new UsuarioDTO
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Rol = string.IsNullOrWhiteSpace(usuario.Rol) ? usuario.Permisos : usuario.Rol,
                Correo = usuario.Correo,
                Permisos = usuario.Permisos,
                Puesto = usuario.Puesto,
                Celular = usuario.Celular,
                NumeroPuesto = usuario.NumeroPuesto,
                ImagenUrl = usuario.ImagenUrl,
                PermisosCategoria = usuario.PermisoCategorias.Select(p => p.Categoria).ToList()
            };

            return Ok(new LoginResponse { Token = token, User = userDto });
        }
    }
}
