using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class GastoFormDto
    {
        [Required(ErrorMessage = "El concepto es obligatorio")]
        [MaxLength(200)]
        public string Concepto { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Today;

        [MaxLength(200)]
        public string Proveedor { get; set; }

        public string Notas { get; set; }

        public int EventoId { get; set; }
    }
}
