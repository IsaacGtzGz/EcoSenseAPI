
using EcoSenseAPI.Models;
using EcoSenseAPI.Data;
using EcoSenseAPI.Services;
using Microsoft.AspNetCore.Mvc;
using EcoSenseAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactoController : ControllerBase
    {
        private readonly EcoSenseContext _context;
        private readonly EmailService _emailService;
        public ContactoController(EcoSenseContext context)
        {
            _context = context;
            _emailService = new EmailService();
        }

        [HttpPost("responder")]
        public async Task<IActionResult> ResponderContacto([FromBody] RespuestaContactoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Asunto) || string.IsNullOrWhiteSpace(dto.Mensaje))
                return BadRequest("Todos los campos son obligatorios.");

            await _emailService.SendEmailAsync(dto.Email, dto.Asunto, dto.Mensaje);
            return Ok(new { mensaje = "Respuesta enviada correctamente" });
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensaje([FromBody] MensajeContacto mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje.Nombre) || string.IsNullOrWhiteSpace(mensaje.Email) || string.IsNullOrWhiteSpace(mensaje.Mensaje))
                return BadRequest("Todos los campos son obligatorios.");

            // Guardar el mensaje en la base de datos
            mensaje.Fecha = DateTime.Now;
            _context.MensajesContacto.Add(mensaje);
            await _context.SaveChangesAsync();

            // Enviar correo al administrador
            string asunto = "Nuevo mensaje de contacto";
            string cuerpo = $"Nombre: {mensaje.Nombre}\nEmail: {mensaje.Email}\nMensaje: {mensaje.Mensaje}";
            await _emailService.SendEmailAsync("isaacgtzgz@gmail.com", asunto, cuerpo);

            return Ok(new { mensaje = "Mensaje guardado y enviado por correo correctamente" });
        }

        [HttpGet]
        public IActionResult ObtenerMensajes()
        {
            var mensajes = _context.MensajesContacto.OrderByDescending(m => m.Fecha).ToList();
            return Ok(mensajes);
        }

        [HttpGet("mensajes")]
        public async Task<IActionResult> GetMensajes()
        {
            var mensajes = await _context.MensajesContacto
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
            return Ok(mensajes);
        }
    }
}