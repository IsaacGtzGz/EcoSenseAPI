using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string? Telefono { get; set; }
        public string Empresa { get; set; } = string.Empty; // Cambio a obligatorio (el documento dice opcional, pero lo vamos a usar para sectorizar)
        public string Rol { get; set; }      

        public DatoSalud? DatoSalud { get; set; }

        [JsonIgnore]
        public List<Dispositivo> Dispositivos { get; set; } = new();

        [JsonIgnore]
        public List<ReporteExportado> ReportesExportados { get; set; } = new();
    }
}
