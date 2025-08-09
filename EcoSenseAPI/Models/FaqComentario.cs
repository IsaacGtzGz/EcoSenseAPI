using System;
namespace EcoSenseAPI.Models
{
    public class FaqComentario
    {
        public int Id { get; set; }
        public string? Pregunta { get; set; }
        public string? Respuesta { get; set; }
        public string? Autor { get; set; }
        public DateTime Fecha { get; set; }
        public bool Destacado { get; set; }
    }
}
