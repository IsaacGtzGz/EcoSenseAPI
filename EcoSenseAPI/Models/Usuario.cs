using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        public required string Nombre { get; set; }
        public required string Correo { get; set; }
        public required string Contraseña { get; set; }
        public string? Telefono { get; set; }
        public required string Empresa { get; set; }
        // Roles permitidos: Administrador, Usuario, Cliente
        public required string Rol { get; set; }

        public DatoSalud? DatoSalud { get; set; }

        [JsonIgnore]
        public List<Dispositivo> Dispositivos { get; set; } = new();

        [JsonIgnore]
        public List<ReporteExportado> ReportesExportados { get; set; } = new();
    }
}
