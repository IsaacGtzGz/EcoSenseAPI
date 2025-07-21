using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class Alerta
    {
        [Key]
        public long IdAlerta { get; set; }

        public long IdLectura { get; set; }
        public string TipoAlerta { get; set; }
        public float ValorMedido { get; set; }
        public float Umbral { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Lectura? Lectura { get; set; }
    }
}
