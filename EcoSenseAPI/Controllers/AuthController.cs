using EcoSenseAPI.Data;
using EcoSenseAPI.Models;
using EcoSenseAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly EcoSenseContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(EcoSenseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Registro de usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo))
                return BadRequest("El correo ya está registrado.");

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado exitosamente." });
        }

        // Login de usuario
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == loginDto.Correo && u.Contraseña == loginDto.Contraseña);

            if (usuario == null)
                return Unauthorized("Correo o contraseña incorrectos.");

            // Generar token JWT
            var token = GenerateJwtToken(usuario);

            return Ok(new
            {
                token = token,
                usuario = new
                {
                    idUsuario = usuario.IdUsuario,
                    nombre = usuario.Nombre,
                    rol = usuario.Rol,
                    correo = usuario.Correo
                }
            });
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EcoSense-SuperSecretKey-32Characters!!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var token = new JwtSecurityToken(
                issuer: "EcoSenseAPI",
                audience: "EcoSenseFront",
                claims: claims,
                expires: DateTime.Now.AddHours(24), // Token válido por 24 horas
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}