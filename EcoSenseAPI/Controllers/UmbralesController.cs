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
            try
            {
                var dispositivo = await _context.Dispositivos.FindAsync(umbral.IdDispositivo);
                if (dispositivo == null)
                    return BadRequest(new { mensaje = "El dispositivo no existe." });

                // Verificar si ya existe un umbral para este dispositivo
                var umbralExistente = await _context.ConfiguracionUmbrales
                    .FirstOrDefaultAsync(u => u.IdDispositivo == umbral.IdDispositivo);

                if (umbralExistente != null)
                    return BadRequest(new { mensaje = "Ya existe un umbral para este dispositivo." });

                umbral.FechaConfiguracion = DateTime.Now;
                _context.ConfiguracionUmbrales.Add(umbral);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Umbral creado exitosamente", idUmbral = umbral.IdUmbral });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUmbral(int id, ConfiguracionUmbral umbral)
        {
            try
            {
                if (id != umbral.IdUmbral)
                    return BadRequest(new { mensaje = "El ID del umbral no coincide." });

                var umbralExistente = await _context.ConfiguracionUmbrales.FindAsync(id);
                if (umbralExistente == null)
                    return NotFound(new { mensaje = "El umbral no existe." });

                // Actualizar valores
                umbralExistente.Co2Max = umbral.Co2Max;
                umbralExistente.Pm1Max = umbral.Pm1Max;
                umbralExistente.Pm2_5Max = umbral.Pm2_5Max;
                umbralExistente.Pm10Max = umbral.Pm10Max;
                umbralExistente.TemperaturaMax = umbral.TemperaturaMax;
                umbralExistente.HumedadMax = umbral.HumedadMax;
                umbralExistente.PresionMax = umbral.PresionMax;
                umbralExistente.FechaConfiguracion = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Umbral actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}
