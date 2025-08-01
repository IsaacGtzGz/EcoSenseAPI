using EcoSenseAPI.Data;
using EcoSenseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertasController : ControllerBase
    {
        private readonly EcoSenseContext _context;

        public AlertasController(EcoSenseContext context)
        {
            _context = context;
        }

        // GET: api/Alertas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alerta>>> ObtenerTodas()
        {
            return await _context.Alertas
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        // GET: api/Alertas/dispositivo/1
        [HttpGet("dispositivo/{idDispositivo}")]
        public async Task<ActionResult<IEnumerable<Alerta>>> ObtenerPorDispositivo(int idDispositivo)
        {
            var alertas = await _context.Alertas
                .Where(a => _context.Lecturas
                    .Any(l => l.IdLectura == a.IdLectura && l.IdDispositivo == idDispositivo))
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return Ok(alertas);
        }

        // GET: api/Alertas/recientes/10
        [HttpGet("recientes/{cantidad}")]
        public async Task<ActionResult<IEnumerable<Alerta>>> ObtenerRecientes(int cantidad)
        {
            var alertas = await _context.Alertas
                .OrderByDescending(a => a.Timestamp)
                .Take(cantidad)
                .ToListAsync();

            return Ok(alertas);
        }

        // GET: api/Alertas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alerta>> ObtenerPorId(long id)
        {
            var alerta = await _context.Alertas.FindAsync(id);

            if (alerta == null)
                return NotFound("Alerta no encontrada.");

            return Ok(alerta);
        }

        // DELETE: api/Alertas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAlerta(long id)
        {
            var alerta = await _context.Alertas.FindAsync(id);

            if (alerta == null)
                return NotFound("Alerta no encontrada.");

            _context.Alertas.Remove(alerta);
            await _context.SaveChangesAsync();

            return Ok("Alerta eliminada correctamente.");
        }
    }
}
