using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class TareaDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaLimite { get; set; }
        public int? AsignadoAId { get; set; }
        public string AsignadoANombre { get; set; }
        public int Orden { get; set; }
    }

    public class TareaFormDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El evento es obligatorio")]
        public int EventoId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; }

        public DateTime? FechaLimite { get; set; }
        
        public int? AsignadoAId { get; set; }
        
        public int Orden { get; set; }
    }
}
