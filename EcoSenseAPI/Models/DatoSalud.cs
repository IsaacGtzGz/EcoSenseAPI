using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class DatoSalud
    {
        [Key]
        public int IdUsuario { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public ushort AlturaCm { get; set; }
        public ushort PesoKg { get; set; }
        public string? EnfermedadesRespiratorias { get; set; }
        public string? Alergias { get; set; }
        public string? Observaciones { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
