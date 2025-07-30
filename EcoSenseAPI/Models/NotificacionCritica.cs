using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSenseAPI.Models
{
    public class NotificacionCritica
    {
        [Key]
        public int IdNotificacion { get; set; }

        public int IdUsuario { get; set; }
        public string Mensaje { get; set; }
        public bool Leido { get; set; } = false;
        public DateTime Fecha { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
