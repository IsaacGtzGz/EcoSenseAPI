using EcoSenseAPI.Models;
using EcoSenseAPI.Data;
using EcoSenseAPI.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> EnviarMensaje([FromBody] MensajeContacto mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje.Nombre) || string.IsNullOrWhiteSpace(mensaje.Email) || string.IsNullOrWhiteSpace(mensaje.Mensaje))
                return BadRequest("Todos los campos son obligatorios.");

            // Solo enviar correo al administrador, NO guardar en la base de datos
            string asunto = "Nuevo mensaje de contacto";
            string cuerpo = $"Nombre: {mensaje.Nombre}\nEmail: {mensaje.Email}\nMensaje: {mensaje.Mensaje}";
            await _emailService.SendEmailAsync("isaacgtzgz@gmail.com", asunto, cuerpo);

            return Ok(new { mensaje = "Mensaje enviado por correo correctamente" });
        }
        [HttpGet]
        public IActionResult ObtenerMensajes()
        {
            var mensajes = _context.MensajesContacto.OrderByDescending(m => m.Fecha).ToList();
            return Ok(mensajes);
        }
    }
}
