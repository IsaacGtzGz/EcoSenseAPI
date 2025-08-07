using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoSenseAPI.Data;
using EcoSenseAPI.Models;
using System.Text;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EcoSenseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly EcoSenseContext _context;
        private readonly IWebHostEnvironment _environment;

        public ReportesController(EcoSenseContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Reportes/datos?fechaInicio=2025-01-01&fechaFin=2025-08-04&idDispositivo=1&sensor=CO2
        [HttpGet("datos")]
        public async Task<ActionResult> ObtenerDatosReporte(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] int? idDispositivo = null,
            [FromQuery] string? sensor = null)
        {
            try
            {
                var query = _context.Lecturas
                    .Where(l => l.Timestamp >= fechaInicio && l.Timestamp <= fechaFin);

                if (idDispositivo.HasValue)
                {
                    query = query.Where(l => l.IdDispositivo == idDispositivo.Value);
                }

                var lecturas = await query
                    .OrderByDescending(l => l.Timestamp)
                    .ToListAsync();

                // Filtrar por sensor específico si se solicita
                var datos = lecturas.Select(l => new
                {
                    l.IdLectura,
                    l.IdDispositivo,
                    l.Timestamp,
                    CO2 = sensor == "CO2" || sensor == null ? l.Co2 : null,
                    PM1 = sensor == "PM1" || sensor == null ? l.Pm1_0 : null,
                    PM25 = sensor == "PM25" || sensor == null ? l.Pm2_5 : null,
                    PM10 = sensor == "PM10" || sensor == null ? l.Pm10 : null,
                    Temperatura = sensor == "Temperatura" || sensor == null ? l.Temperatura : null,
                    Humedad = sensor == "Humedad" || sensor == null ? l.Humedad : null,
                    Presion = sensor == "Presion" || sensor == null ? l.Presion : null
                });

                return Ok(new
                {
                    totalRegistros = datos.Count(),
                    rangoFechas = new { inicio = fechaInicio, fin = fechaFin },
                    datos = datos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener datos", detalle = ex.Message });
            }
        }

        // POST: api/Reportes/generar-csv
        [HttpPost("generar-csv")]
        public async Task<ActionResult> GenerarReporteCSV([FromBody] ReporteRequest request)
        {
            try
            {
                var lecturas = await ObtenerLecturasFiltradas(request);

                var csv = new StringBuilder();
                csv.AppendLine("Fecha,Dispositivo,CO2(ppm),PM1.0(µg/m³),PM2.5(µg/m³),PM10(µg/m³),Temperatura(°C),Humedad(%),Presión(hPa)");

                foreach (var lectura in lecturas)
                {
                    csv.AppendLine($"{lectura.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                                 $"{lectura.IdDispositivo}," +
                                 $"{lectura.Co2?.ToString() ?? "N/A"}," +
                                 $"{lectura.Pm1_0?.ToString() ?? "N/A"}," +
                                 $"{lectura.Pm2_5?.ToString() ?? "N/A"}," +
                                 $"{lectura.Pm10?.ToString() ?? "N/A"}," +
                                 $"{lectura.Temperatura?.ToString() ?? "N/A"}," +
                                 $"{lectura.Humedad?.ToString() ?? "N/A"}," +
                                 $"{lectura.Presion?.ToString() ?? "N/A"}");
                }

                var fileName = $"reporte_ecosense_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(_environment.WebRootPath, "reportes", fileName);

                // Crear directorio si no existe
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await System.IO.File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);

                // Guardar registro del reporte
                await GuardarRegistroReporte(request.IdUsuario, request.FechaInicio, request.FechaFin, "CSV", filePath);

                return Ok(new
                {
                    mensaje = "Reporte CSV generado exitosamente",
                    archivo = fileName,
                    url = $"/reportes/{fileName}",
                    totalRegistros = lecturas.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al generar reporte CSV", detalle = ex.Message });
            }
        }

        // POST: api/Reportes/generar-pdf
        [HttpPost("generar-pdf")]
        public async Task<ActionResult> GenerarReportePDF([FromBody] ReporteRequest request)
        {
            try
            {
                var lecturas = await ObtenerLecturasFiltradas(request);

                var fileName = $"reporte_ecosense_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(_environment.WebRootPath, "reportes", fileName);

                // Crear directorio si no existe
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    var document = new Document(PageSize.A4.Rotate()); // Horizontal para tablas
                    PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Título
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                    var title = new Paragraph($"Reporte EcoSense - {DateTime.Now:dd/MM/yyyy}", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);

                    document.Add(new Paragraph(" ")); // Espacio

                    // Información del reporte
                    var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    document.Add(new Paragraph($"Período: {request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}", infoFont));
                    document.Add(new Paragraph($"Total de registros: {lecturas.Count()}", infoFont));
                    document.Add(new Paragraph(" "));

                    // Tabla de datos
                    var table = new PdfPTable(8);
                    table.WidthPercentage = 100;

                    // Headers
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
                    table.AddCell(new PdfPCell(new Phrase("Fecha", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Dispositivo", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("CO2 (ppm)", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("PM2.5 (µg/m³)", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("PM10 (µg/m³)", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Temp (°C)", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Humedad (%)", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Presión (hPa)", headerFont)));

                    // Datos (limitar a 100 registros para PDF)
                    var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
                    foreach (var lectura in lecturas.Take(100))
                    {
                        table.AddCell(new PdfPCell(new Phrase(lectura.Timestamp.ToString("dd/MM HH:mm"), dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(lectura.IdDispositivo.ToString(), dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(lectura.Co2?.ToString() ?? "N/A", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(lectura.Pm2_5?.ToString() ?? "N/A", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(lectura.Pm10?.ToString() ?? "N/A", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(lectura.Temperatura?.ToString() ?? "N/A", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(lectura.Humedad?.ToString() ?? "N/A", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(lectura.Presion?.ToString() ?? "N/A", dataFont)));
                    }

                    document.Add(table);

                    if (lecturas.Count() > 100)
                    {
                        document.Add(new Paragraph($"Nota: Se muestran los primeros 100 registros de {lecturas.Count()} totales.", infoFont));
                    }

                    document.Close();
                }

                // Guardar registro del reporte
                await GuardarRegistroReporte(request.IdUsuario, request.FechaInicio, request.FechaFin, "PDF", filePath);

                return Ok(new
                {
                    mensaje = "Reporte PDF generado exitosamente",
                    archivo = fileName,
                    url = $"/reportes/{fileName}",
                    totalRegistros = lecturas.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al generar reporte PDF", detalle = ex.Message });
            }
        }

        // GET: api/Reportes/historial/{idUsuario}
        [HttpGet("historial/{idUsuario}")]
        public async Task<ActionResult> ObtenerHistorialReportes(int idUsuario)
        {
            var reportes = await _context.ReportesExportados
                .Where(r => r.IdUsuario == idUsuario)
                .OrderByDescending(r => r.FechaExportacion)
                .Take(20)
                .ToListAsync();

            return Ok(reportes);
        }

        // Método privado para obtener lecturas filtradas
        private async Task<List<Lectura>> ObtenerLecturasFiltradas(ReporteRequest request)
        {
            var query = _context.Lecturas
                .Where(l => l.Timestamp >= request.FechaInicio && l.Timestamp <= request.FechaFin);

            if (request.IdDispositivo.HasValue)
            {
                query = query.Where(l => l.IdDispositivo == request.IdDispositivo.Value);
            }

            return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
        }

        // Método privado para guardar registro del reporte
        private async Task GuardarRegistroReporte(int idUsuario, DateTime fechaInicio, DateTime fechaFin, string tipo, string rutaArchivo)
        {
            var reporte = new ReporteExportado
            {
                IdUsuario = idUsuario,
                RangoInicio = fechaInicio,
                RangoFin = fechaFin,
                TipoReporte = tipo,
                RutaArchivo = rutaArchivo,
                FechaExportacion = DateTime.Now
            };

            _context.ReportesExportados.Add(reporte);
            await _context.SaveChangesAsync();
        }
    }

    // DTO para request de reportes
    public class ReporteRequest
    {
        public int IdUsuario { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? IdDispositivo { get; set; }
        public string? TipoSensor { get; set; }
    }
}