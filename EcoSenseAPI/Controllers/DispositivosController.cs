using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoSenseAPI.Data;
using EcoSenseAPI.Models;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispositivosController : ControllerBase
    {
        private readonly EcoSenseContext _context;

        public DispositivosController(EcoSenseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dispositivo>>> GetDispositivos()
        {
            return await _context.Dispositivos.Include(d => d.Usuario).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dispositivo>> GetDispositivo(int id)
        {
            var dispositivo = await _context.Dispositivos
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.IdDispositivo == id);

            if (dispositivo == null) return NotFound();
            return dispositivo;
        }

        [HttpPost]
        public async Task<ActionResult<Dispositivo>> PostDispositivo(Dispositivo dispositivo)
        {
            var usuario = await _context.Usuarios.FindAsync(dispositivo.IdUsuario);
            if (usuario == null)
                return BadRequest("El usuario no existe.");

            dispositivo.Usuario = usuario;

            _context.Dispositivos.Add(dispositivo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDispositivo), new { id = dispositivo.IdDispositivo }, dispositivo);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutDispositivo(int id, Dispositivo dispositivo)
        {
            if (id != dispositivo.IdDispositivo) return BadRequest();

            _context.Entry(dispositivo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDispositivo(int id, [FromQuery] bool force = false)
        {
            try
            {
                var dispositivo = await _context.Dispositivos.FindAsync(id);
                if (dispositivo == null)
                    return NotFound(new { mensaje = "Dispositivo no encontrado" });

                // Verificar si hay lecturas asociadas
                var lecturas = await _context.Lecturas.Where(l => l.IdDispositivo == id).ToListAsync();
                var tieneLecturas = lecturas.Any();

                if (tieneLecturas && !force)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se puede eliminar el dispositivo porque tiene lecturas asociadas",
                        sugerencia = "Use force=true para eliminar todo en cascada, o considere desactivar el dispositivo",
                        lecturas = lecturas.Count,
                        eliminarUrl = $"/api/dispositivos/{id}?force=true"
                    });
                }

                var alertasEliminadas = 0;

                // Si force=true, eliminar todo en cascada
                if (force && tieneLecturas)
                {
                    // 1. Eliminar alertas asociadas a las lecturas
                    var lecturasIds = lecturas.Select(l => l.IdLectura).ToList();
                    var alertas = await _context.Alertas.Where(a => lecturasIds.Contains(a.IdLectura)).ToListAsync();
                    if (alertas.Any())
                    {
                        _context.Alertas.RemoveRange(alertas);
                        alertasEliminadas = alertas.Count;
                    }

                    // 2. Eliminar lecturas
                    _context.Lecturas.RemoveRange(lecturas);
                }

                // 3. Verificar y eliminar configuraciones de umbrales
                var umbrales = await _context.ConfiguracionUmbrales.Where(u => u.IdDispositivo == id).ToListAsync();
                if (umbrales.Any())
                {
                    _context.ConfiguracionUmbrales.RemoveRange(umbrales);
                }

                // 4. Finalmente eliminar el dispositivo
                _context.Dispositivos.Remove(dispositivo);
                await _context.SaveChangesAsync();

                var mensaje = force
                    ? $"Dispositivo '{dispositivo.Nombre}' y todos sus datos asociados eliminados correctamente"
                    : $"Dispositivo '{dispositivo.Nombre}' eliminado correctamente";

                return Ok(new
                {
                    mensaje = mensaje,
                    datosEliminados = new
                    {
                        lecturas = force ? lecturas.Count : 0,
                        alertas = alertasEliminadas,
                        umbrales = umbrales.Count
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }
    }
}
