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
        public async Task<IActionResult> DeleteDispositivo(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);
            if (dispositivo == null) return NotFound();

            _context.Dispositivos.Remove(dispositivo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
