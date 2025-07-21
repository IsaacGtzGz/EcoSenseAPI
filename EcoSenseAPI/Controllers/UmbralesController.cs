using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoSenseAPI.Data;
using EcoSenseAPI.Models;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UmbralesController : ControllerBase
    {
        private readonly EcoSenseContext _context;

        public UmbralesController(EcoSenseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConfiguracionUmbral>>> GetUmbrales()
        {
            return await _context.ConfiguracionUmbrales
                .Include(u => u.Dispositivo)
                .ToListAsync();
        }

        [HttpGet("{idDispositivo}")]
        public async Task<ActionResult<ConfiguracionUmbral>> GetUmbralPorDispositivo(int idDispositivo)
        {
            var umbral = await _context.ConfiguracionUmbrales
                .FirstOrDefaultAsync(u => u.IdDispositivo == idDispositivo);

            if (umbral == null) return NotFound();
            return umbral;
        }

        [HttpPost]
        public async Task<ActionResult> CrearUmbral(ConfiguracionUmbral umbral)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(umbral.IdDispositivo);
            if (dispositivo == null)
                return BadRequest("El dispositivo no existe.");

            umbral.Dispositivo = dispositivo;

            _context.ConfiguracionUmbrales.Add(umbral);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Umbral creado exitosamente" });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUmbral(int id, ConfiguracionUmbral umbral)
        {
            if (id != umbral.IdUmbral) return BadRequest();

            _context.Entry(umbral).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUmbral(int id)
        {
            var umbral = await _context.ConfiguracionUmbrales.FindAsync(id);
            if (umbral == null)
                return NotFound();

            _context.ConfiguracionUmbrales.Remove(umbral);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
