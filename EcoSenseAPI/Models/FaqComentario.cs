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
        public int? Estrellas { get; set; } // Valoración de 1 a 5
        public int? UsuarioId { get; set; } // Nuevo: usuario dueño del comentario
    }
}
