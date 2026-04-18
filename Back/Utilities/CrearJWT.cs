using Microsoft.IdentityModel.Tokens;
using SolicitudServidores.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SolicitudServidores.Utilities
{
    public class CrearJWT
    {
        private readonly IConfiguration _configuration;

        public CrearJWT(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerarToken(Usuario modelo)
        {
            var rol = string.IsNullOrWhiteSpace(modelo.Rol) ? modelo.Permisos : modelo.Rol;
            var userClaim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, modelo.Id.ToString()),
                new Claim(ClaimTypes.Name, modelo.NombreCompleto),
                new Claim(ClaimTypes.Email, modelo.Correo),
                new Claim(ClaimTypes.Role, rol),
                new Claim("ImagenUrl", modelo.ImagenUrl ?? string.Empty),
                new Claim("Permisos", modelo.Permisos ?? string.Empty)
            };

            var jwtKey = _configuration["JWT:key"] ?? _configuration["JWT_SECRET"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("No se encontró la llave JWT en la configuración.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfig = new JwtSecurityToken(
                claims: userClaim,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
