using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcoSenseAPI.Models;
using EcoSenseAPI.Data;

namespace EcoSenseAPI.Controllers
{
    public class DestacarDto
    {
        public bool Destacado { get; set; }
    }

    public class ResponderDto
    {
        public string? Respuesta { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class FaqController : ControllerBase
    {
        private readonly EcoSenseContext _context;
        public FaqController(EcoSenseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ObtenerComentarios()
        {
            var comentarios = _context.FaqComentarios.OrderByDescending(c => c.Fecha).ToList();
            return Ok(comentarios);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarComentario([FromBody] FaqComentario comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario.Pregunta))
                return BadRequest("La pregunta/comentario es obligatorio.");
            comentario.Fecha = System.DateTime.Now;
            _context.FaqComentarios.Add(comentario);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Comentario guardado correctamente" });
        }

        [HttpPut("{id}/responder")]
        public async Task<IActionResult> ResponderComentario(int id, [FromBody] ResponderDto body)
        {
            var comentario = _context.FaqComentarios.FirstOrDefault(c => c.Id == id);
            if (comentario == null) return NotFound();
            comentario.Respuesta = body.Respuesta;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}/destacar")]
        public async Task<IActionResult> DestacarComentario(int id, [FromBody] DestacarDto body)
        {
            var comentario = _context.FaqComentarios.FirstOrDefault(c => c.Id == id);
            if (comentario == null) return NotFound();
            comentario.Destacado = body.Destacado;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarComentario(int id)
        {
            var comentario = _context.FaqComentarios.FirstOrDefault(c => c.Id == id);
            if (comentario == null) return NotFound();
            _context.FaqComentarios.Remove(comentario);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Comentario eliminado" });
        }
    }
}
