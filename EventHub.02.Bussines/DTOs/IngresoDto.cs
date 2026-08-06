using System;

namespace EventHub._02.Bussines.DTOs
{
    public class IngresoDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public string Tipo { get; set; }
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public string Notas { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
