using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartSales.Business.Services;
using SmartSalesAPI.DTO.Auth;

namespace SmartSalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioServices _usuarioServices;
        private readonly IConfiguration _configuration;

        public AuthController(UsuarioServices usuarioServices, IConfiguration configuration)
        {
            _usuarioServices = usuarioServices;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var nombre = request.Nombre?.Trim() ?? string.Empty;
            var password = request.Password ?? string.Empty;

            var usuario = await _usuarioServices.BuscarUsuarioPorNombreExacto(nombre);
            if (usuario is null || !usuario.Estado)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            var storedPassword = usuario.ContrasenaHash ?? string.Empty;
            var validPassword = storedPassword.StartsWith("$2")
                ? BCrypt.Net.BCrypt.Verify(password, storedPassword)
                : storedPassword == password;

            if (!validPassword)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            var key = _configuration["Jwt:Key"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, usuario.Nombre ?? string.Empty),
                new Claim(ClaimTypes.Role, usuario.Rol ?? string.Empty)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResponseDTO
            {
                Token = jwt,
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre ?? string.Empty,
                Rol = usuario.Rol ?? string.Empty
            });
        }
    }
}