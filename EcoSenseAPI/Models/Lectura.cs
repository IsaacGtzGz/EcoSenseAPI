using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class Lectura
    {
        [Key]
        public long IdLectura { get; set; }

        public int IdDispositivo { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int? Co2 { get; set; }
        public float? Pm1_0 { get; set; }
        public float? Pm2_5 { get; set; }
        public float? Pm10 { get; set; }
        public float? Temperatura { get; set; }
        public float? Humedad { get; set; }
        public float? Presion { get; set; }

        [JsonIgnore]
        public Dispositivo? Dispositivo { get; set; }

        [JsonIgnore]
        public List<Alerta> Alertas { get; set; } = new();
    }
}
