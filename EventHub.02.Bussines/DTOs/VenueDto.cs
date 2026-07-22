using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class VenueDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [StringLength(50, ErrorMessage = "La ciudad no puede exceder 50 caracteres.")]
        [Display(Name = "Ciudad")]
        public string Ciudad { get; set; }

        [Display(Name = "Capacidad Máxima")]
        public int? CapacidadMaxima { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }
    }
}
