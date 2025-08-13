using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoSenseAPI.Data;
using EcoSenseAPI.Models;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly EcoSenseContext _context;

        public UsuariosController(EcoSenseContext context)
        {
            _context = context;
        }

        private readonly Services.EmailService _emailService = new Services.EmailService();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Si el usuario es cliente, enviar correo con credenciales
            if (usuario.Rol?.ToLower() == "cliente" && !string.IsNullOrWhiteSpace(usuario.Correo))
            {
                var body = $"Hola {usuario.Nombre},\n\nTu cuenta ha sido creada en EcoSense.\n\nUsuario: {usuario.Correo}\nContraseña: {usuario.Contraseña}\n\nPuedes iniciar sesión en la plataforma y cambiar tu contraseña cuando lo desees.\n\nSaludos,\nEquipo EcoSense";
                await _emailService.SendEmailAsync(usuario.Correo, "Tus credenciales de acceso a EcoSense", body);
            }

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario) return BadRequest();

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
