using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class Dispositivo
    {
        [Key]
        public int IdDispositivo { get; set; }

        public string Mac { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
        public string Sector { get; set; } // Para clasificación futura por áreas

        public int IdUsuario { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        [JsonIgnore]
        public List<ConfiguracionUmbral> ConfiguracionUmbrales { get; set; } = new();

        [JsonIgnore]
        public List<Lectura> Lecturas { get; set; } = new();
    }
}
