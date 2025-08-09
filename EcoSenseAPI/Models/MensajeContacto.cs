namespace EcoSenseAPI.Models
{
    public class MensajeContacto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Email { get; set; }
        public required string Mensaje { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
