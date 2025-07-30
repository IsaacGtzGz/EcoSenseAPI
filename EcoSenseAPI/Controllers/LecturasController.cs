using EcoSenseAPI.Data;
using EcoSenseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LecturasController : ControllerBase
    {
        private readonly EcoSenseContext _context;

        public LecturasController(EcoSenseContext context)
        {
            _context = context;
        }

        [HttpPost("nueva")]
        public async Task<IActionResult> AgregarLectura([FromBody] Lectura lectura)
        {
            // 1. Verificamos que el dispositivo exista
            var dispositivo = await _context.Dispositivos
                .Include(d => d.ConfiguracionUmbrales)
                .FirstOrDefaultAsync(d => d.IdDispositivo == lectura.IdDispositivo);

            if (dispositivo == null)
                return NotFound("Dispositivo no encontrado.");

            // 2. Guardamos la lectura
            lectura.Timestamp = DateTime.Now;
            _context.Lecturas.Add(lectura);
            await _context.SaveChangesAsync(); // primero guardamos para tener IdLectura

            // 3. Evaluamos los umbrales más recientes
            var umbral = dispositivo.ConfiguracionUmbrales
                .OrderByDescending(u => u.FechaConfiguracion)
                .FirstOrDefault();

            if (umbral == null)
                return Ok("Lectura guardada, pero no hay umbrales configurados.");

            List<Alerta> alertas = new();

            if (lectura.Co2.HasValue && lectura.Co2 > umbral.Co2Max)
                alertas.Add(CrearAlerta("CO2", lectura.Co2.Value, umbral.Co2Max, lectura.IdLectura));

            if (lectura.Pm2_5.HasValue && lectura.Pm2_5 > umbral.Pm2_5Max)
                alertas.Add(CrearAlerta("PM2.5", lectura.Pm2_5.Value, umbral.Pm2_5Max, lectura.IdLectura));

            if (lectura.Pm10.HasValue && lectura.Pm10 > umbral.Pm10Max)
                alertas.Add(CrearAlerta("PM10", lectura.Pm10.Value, umbral.Pm10Max, lectura.IdLectura));

            if (lectura.Temperatura.HasValue && lectura.Temperatura > umbral.TemperaturaMax)
                alertas.Add(CrearAlerta("Temperatura", lectura.Temperatura.Value, umbral.TemperaturaMax, lectura.IdLectura));

            if (lectura.Humedad.HasValue && lectura.Humedad > umbral.HumedadMax)
                alertas.Add(CrearAlerta("Humedad", lectura.Humedad.Value, umbral.HumedadMax, lectura.IdLectura));

            if (lectura.Presion.HasValue && lectura.Presion > umbral.PresionMax)
                alertas.Add(CrearAlerta("Presión", lectura.Presion.Value, umbral.PresionMax, lectura.IdLectura));

            if (alertas.Count > 0)
            {
                _context.Alertas.AddRange(alertas);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                mensaje = "Lectura guardada correctamente",
                alertasGeneradas = alertas.Count
            });
        }

        private Alerta CrearAlerta(string tipo, float valor, float umbral, long idLectura)
        {
            return new Alerta
            {
                TipoAlerta = tipo, // valores claros como "CO2", "PM2.5", "Temperatura", etc.
                ValorMedido = valor,
                Umbral = umbral,
                IdLectura = idLectura,
                Timestamp = DateTime.Now
            };
        }
    }
}
