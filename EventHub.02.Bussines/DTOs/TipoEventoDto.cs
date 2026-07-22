using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class TipoEventoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}
