using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoSenseAPI.Data;
using EcoSenseAPI.Models;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatosSaludController : ControllerBase
    {
        private readonly EcoSenseContext _context;

        public DatosSaludController(EcoSenseContext context)
        {
            _context = context;
        }

        [HttpGet("{idUsuario}")]
        public async Task<ActionResult<DatoSalud>> GetDatoSalud(int idUsuario)
        {
            var datoSalud = await _context.DatosSalud.FindAsync(idUsuario);
            if (datoSalud == null) return NotFound(new { mensaje = "No se encontraron datos de salud para este usuario" });
            return datoSalud;
        }

        [HttpPost]
        public async Task<ActionResult> CreateDatoSalud(DatoSalud datoSalud)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(datoSalud.IdUsuario);
                if (usuario == null)
                    return BadRequest(new { mensaje = "El usuario no existe" });

                var datoExistente = await _context.DatosSalud.FindAsync(datoSalud.IdUsuario);
                if (datoExistente != null)
                    return BadRequest(new { mensaje = "Ya existen datos de salud para este usuario" });

                _context.DatosSalud.Add(datoSalud);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Datos de salud creados exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpPut("{idUsuario}")]
        public async Task<IActionResult> UpdateDatoSalud(int idUsuario, DatoSalud datoSalud)
        {
            try
            {
                if (idUsuario != datoSalud.IdUsuario)
                    return BadRequest(new { mensaje = "El ID del usuario no coincide" });

                var datoExistente = await _context.DatosSalud.FindAsync(idUsuario);
                if (datoExistente == null)
                    return NotFound(new { mensaje = "No se encontraron datos de salud para este usuario" });

                // Actualizar valores
                datoExistente.FechaNacimiento = datoSalud.FechaNacimiento;
                datoExistente.Genero = datoSalud.Genero;
                datoExistente.AlturaCm = datoSalud.AlturaCm;
                datoExistente.PesoKg = datoSalud.PesoKg;
                datoExistente.EnfermedadesRespiratorias = datoSalud.EnfermedadesRespiratorias;
                datoExistente.Alergias = datoSalud.Alergias;
                datoExistente.Observaciones = datoSalud.Observaciones;

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Datos de salud actualizados exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpDelete("{idUsuario}")]
        public async Task<IActionResult> DeleteDatoSalud(int idUsuario)
        {
            try
            {
                var datoSalud = await _context.DatosSalud.FindAsync(idUsuario);
                if (datoSalud == null)
                    return NotFound(new { mensaje = "No se encontraron datos de salud para este usuario" });

                _context.DatosSalud.Remove(datoSalud);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Datos de salud eliminados exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("{idUsuario}/recomendaciones")]
        public async Task<ActionResult> GetRecomendaciones(int idUsuario)
        {
            try
            {
                var datoSalud = await _context.DatosSalud.FindAsync(idUsuario);
                if (datoSalud == null)
                    return NotFound(new { mensaje = "No se encontraron datos de salud para este usuario" });

                // Lógica de recomendaciones basada en datos de salud
                var recomendaciones = GenerarRecomendaciones(datoSalud);

                return Ok(new { recomendaciones });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        private List<string> GenerarRecomendaciones(DatoSalud datoSalud)
        {
            var recomendaciones = new List<string>();

            // Recomendaciones por edad
            var edad = DateTime.Now.Year - datoSalud.FechaNacimiento.Year;
            if (edad >= 65)
            {
                recomendaciones.Add("Como adulto mayor, es especialmente importante monitorear la calidad del aire");
            }

            // Recomendaciones por enfermedades respiratorias
            if (!string.IsNullOrEmpty(datoSalud.EnfermedadesRespiratorias))
            {
                if (datoSalud.EnfermedadesRespiratorias.ToLower().Contains("asma"))
                {
                    recomendaciones.Add("Mantenga niveles de PM2.5 por debajo de 25 µg/m³");
                    recomendaciones.Add("Evite exposición prolongada cuando CO₂ > 800 ppm");
                }
                if (datoSalud.EnfermedadesRespiratorias.ToLower().Contains("epoc"))
                {
                    recomendaciones.Add("Use mascarilla cuando PM10 > 50 µg/m³");
                    recomendaciones.Add("Considere purificadores de aire en espacios cerrados");
                }
            }

            // Recomendaciones por alergias
            if (!string.IsNullOrEmpty(datoSalud.Alergias))
            {
                if (datoSalud.Alergias.ToLower().Contains("polen"))
                {
                    recomendaciones.Add("Evite salir en días con alta concentración de partículas");
                }
                if (datoSalud.Alergias.ToLower().Contains("polvo"))
                {
                    recomendaciones.Add("Mantenga humedad entre 40-60% para reducir ácaros");
                }
            }

            if (recomendaciones.Count == 0)
            {
                recomendaciones.Add("Mantenga un ambiente con buena ventilación");
                recomendaciones.Add("Monitoree regularmente la calidad del aire");
            }

            return recomendaciones;
        }
    }
}