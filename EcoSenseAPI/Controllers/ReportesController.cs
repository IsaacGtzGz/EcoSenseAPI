using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoSenseAPI.Data;
using EcoSenseAPI.Models;
using System.Text;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

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

                // Header del CSV con información del reporte
                csv.AppendLine("🍃 REPORTE ECOSENSE - SISTEMA DE MONITOREO AMBIENTAL");
                csv.AppendLine($"📅 Período: {request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}");
                csv.AppendLine($"⏰ Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                csv.AppendLine($"📊 Total de registros: {lecturas.Count()}");
                csv.AppendLine(""); // Línea vacía

                // Headers de datos con emojis
                csv.AppendLine("📅 Fecha,📡 Dispositivo,💨 CO₂(ppm),🌫️ PM1.0(µg/m³),☁️ PM2.5(µg/m³),🌪️ PM10(µg/m³),🌡️ Temperatura(°C),💧 Humedad(%),🔘 Presión(hPa)");

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

                // Usar ContentRootPath si WebRootPath es null, pero sin duplicar wwwroot
                var basePath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                var reportesPath = Path.Combine(basePath, "reportes");
                var filePath = Path.Combine(reportesPath, fileName);

                // Crear directorio si no existe
                Directory.CreateDirectory(reportesPath);

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

                // Usar ContentRootPath si WebRootPath es null, pero sin duplicar wwwroot
                var basePath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                var reportesPath = Path.Combine(basePath, "reportes");
                var filePath = Path.Combine(reportesPath, fileName);

                // Crear directorio si no existe
                Directory.CreateDirectory(reportesPath);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    var document = new Document(PageSize.A4.Rotate()); // Horizontal para tablas
                    PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Título principal con mejor estilo
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                    var title = new Paragraph($"🍃 Reporte EcoSense - {DateTime.Now:dd/MM/yyyy}", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 20f;
                    document.Add(title);

                    // Línea separadora
                    var line = new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2);
                    document.Add(new Chunk(line));
                    document.Add(new Paragraph(" ", FontFactory.GetFont(FontFactory.HELVETICA, 8))); // Espacio

                    // Información del reporte con iconos
                    var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.DARK_GRAY);
                    var infoBoldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.BLACK);

                    var infoParagraph = new Paragraph();
                    infoParagraph.Add(new Chunk("📅 Período: ", infoBoldFont));
                    infoParagraph.Add(new Chunk($"{request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}", infoFont));
                    infoParagraph.SpacingAfter = 8f;
                    document.Add(infoParagraph);

                    var totalParagraph = new Paragraph();
                    totalParagraph.Add(new Chunk("📊 Total de registros: ", infoBoldFont));
                    totalParagraph.Add(new Chunk($"{lecturas.Count()}", infoFont));
                    totalParagraph.SpacingAfter = 8f;
                    document.Add(totalParagraph);

                    var fechaParagraph = new Paragraph();
                    fechaParagraph.Add(new Chunk("⏰ Generado: ", infoBoldFont));
                    fechaParagraph.Add(new Chunk($"{DateTime.Now:dd/MM/yyyy HH:mm}", infoFont));
                    fechaParagraph.SpacingAfter = 15f;
                    document.Add(fechaParagraph);

                    // Tabla de datos con mejor estilo
                    var table = new PdfPTable(8);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 2f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f });

                    // Headers con colores
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                    var headerBgColor = new BaseColor(46, 125, 50); // Verde EcoSense

                    var headers = new string[] { "📅 Fecha", "📡 Dispositivo", "💨 CO₂ (ppm)", "🌫️ PM2.5 (µg/m³)",
                                               "☁️ PM10 (µg/m³)", "🌡️ Temp (°C)", "💧 Humedad (%)", "🔘 Presión (hPa)" };

                    foreach (var header in headers)
                    {
                        var cell = new PdfPCell(new Phrase(header, headerFont));
                        cell.BackgroundColor = headerBgColor;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 8f;
                        table.AddCell(cell);
                    }

                    // Datos con colores alternados
                    var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                    var altRowColor = new BaseColor(248, 249, 250); // Gris muy claro

                    var rowIndex = 0;
                    foreach (var lectura in lecturas.Take(100))
                    {
                        var isEvenRow = rowIndex % 2 == 0;
                        var cellColor = isEvenRow ? BaseColor.WHITE : altRowColor;

                        var cells = new string[] {
                            lectura.Timestamp.ToString("dd/MM HH:mm"),
                            lectura.IdDispositivo.ToString(),
                            lectura.Co2?.ToString() ?? "N/A",
                            lectura.Pm2_5?.ToString() ?? "N/A",
                            lectura.Pm10?.ToString() ?? "N/A",
                            lectura.Temperatura?.ToString() ?? "N/A",
                            lectura.Humedad?.ToString() ?? "N/A",
                            lectura.Presion?.ToString() ?? "N/A"
                        };

                        foreach (var cellText in cells)
                        {
                            var cell = new PdfPCell(new Phrase(cellText, dataFont));
                            cell.BackgroundColor = cellColor;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 6f;
                            table.AddCell(cell);
                        }
                        rowIndex++;
                    }

                    document.Add(table);

                    // Nota si hay más registros
                    if (lecturas.Count() > 100)
                    {
                        var noteFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10, BaseColor.GRAY);
                        var note = new Paragraph($"📝 Nota: Se muestran los primeros 100 registros de {lecturas.Count()} totales.", noteFont);
                        note.Alignment = Element.ALIGN_CENTER;
                        note.SpacingBefore = 15f;
                        document.Add(note);
                    }

                    // Footer
                    document.Add(new Paragraph(" "));
                    var footerLine = new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2);
                    document.Add(new Chunk(footerLine));

                    var footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY);
                    var footer = new Paragraph("🍃 EcoSense - Sistema de Monitoreo Ambiental", footerFont);
                    footer.Alignment = Element.ALIGN_CENTER;
                    footer.SpacingBefore = 10f;
                    document.Add(footer);

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