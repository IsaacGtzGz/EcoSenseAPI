using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class ConfiguracionUmbral
    {
        [Key]
        public int IdUmbral { get; set; }

        public int IdDispositivo { get; set; }
        public int Co2Max { get; set; }
        public float Pm1Max { get; set; }
        public float Pm2_5Max { get; set; }
        public float Pm10Max { get; set; }
        public float TemperaturaMax { get; set; }
        public float HumedadMax { get; set; }
        public float PresionMax { get; set; }
        public DateTime FechaConfiguracion { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Dispositivo? Dispositivo { get; set; }
    }
}
