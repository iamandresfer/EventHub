using System;

namespace EventHub._02.Bussines.DTOs
{
    public class TareaAdjuntoDto
    {
        public int Id { get; set; }
        public int TareaId { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Tipo { get; set; }
        public int? Tamanio { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
