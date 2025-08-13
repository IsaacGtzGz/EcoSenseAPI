using Microsoft.AspNetCore.Mvc;
using EcoSenseAPI.Models;
using EcoSenseAPI.Data;
using System;
using System.Linq;
using EcoSenseAPI.Services;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionesController : ControllerBase
    {
        private readonly EcoSenseContext _context;
        private readonly EmailService _emailService;
        public CotizacionesController(EcoSenseContext context)
        {
            _context = context;
            _emailService = new EmailService();
        }

        [HttpPost]
        public IActionResult SolicitarCotizacion([FromBody] CotizacionSolicitud solicitud)
        {
            solicitud.Fecha = DateTime.Now;
            var tipo = (solicitud.TipoProducto ?? "").Trim().ToLowerInvariant();
            if (tipo != "ecosense")
            {
                return BadRequest(new { mensaje = "Solo se acepta el producto EcoSense.", costo = 0 });
            }
            decimal precioBase = 48500M;
            decimal costo = precioBase * solicitud.Cantidad;
            bool tieneRequerimientos = !string.IsNullOrWhiteSpace(solicitud.Requerimientos);
            if (tieneRequerimientos)
            {
                costo *= 1.10M; // Suma 10% extra
            }
            solicitud.Costo = Math.Round(costo, 0);
            _context.Add(solicitud);
            _context.SaveChanges();

            // Enviar correo al administrador
            string asunto = "Nueva solicitud de cotización";
            string cuerpo = $"Nombre: {solicitud.Nombre}\nEmail: {solicitud.Correo}\nTipo de producto: {solicitud.TipoProducto}\nCantidad: {solicitud.Cantidad}\nRequerimientos: {solicitud.Requerimientos}\nCosto estimado: ${solicitud.Costo} MXN";
            if (tieneRequerimientos)
            {
                cuerpo += "\n(Incluye 10% extra por requerimientos especiales)";
            }
            _emailService.SendEmailAsync("deloso2ig@gmail.com", asunto, cuerpo).Wait();

            return Ok(new { mensaje = "Solicitud guardada y enviada por correo", costo = solicitud.Costo });
        }
    }
}
