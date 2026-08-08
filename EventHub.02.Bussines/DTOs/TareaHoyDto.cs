using System;

namespace EventHub._02.Bussines.DTOs
{
    public class TareaHoyDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int EventoId { get; set; }
        public string EventoNombre { get; set; }
        public string Estado { get; set; }
        public string OperadorNombre { get; set; }
        public DateTime? FechaLimite { get; set; }
        public int Orden { get; set; }
    }
}
