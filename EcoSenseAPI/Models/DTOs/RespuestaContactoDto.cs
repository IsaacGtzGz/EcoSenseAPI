using System;
namespace EcoSenseAPI.Models.DTOs
{
    public class RespuestaContactoDto
    {
        public required string Email { get; set; }
        public required string Asunto { get; set; }
        public required string Mensaje { get; set; }
    }
}
