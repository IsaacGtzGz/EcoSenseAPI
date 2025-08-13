using System;
namespace EcoSenseAPI.Models
{
    public class CotizacionSolicitud
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? TipoProducto { get; set; }
        public int Cantidad { get; set; }
        public string? Requerimientos { get; set; }
        public decimal Costo { get; set; }
        public DateTime Fecha { get; set; }
    }
}
