using EcoSenseAPI.Data;
using EcoSenseAPI.Models;
using EcoSenseAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly EcoSenseContext _context;

        public AuthController(EcoSenseContext context)
        {
            _context = context;
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

            return Ok(new
            {
                mensaje = "Login exitoso",
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Rol
            });
        }

    }
}
