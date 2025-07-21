using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class ReporteExportado
    {
        [Key]
        public int IdReporte { get; set; }

        public int IdUsuario { get; set; }
        public DateTime RangoInicio { get; set; }
        public DateTime RangoFin { get; set; }
        public string TipoReporte { get; set; }
        public DateTime FechaExportacion { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
