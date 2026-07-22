using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El RUC es obligatorio.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El RUC debe tener 13 dígitos.")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "El RUC debe contener solo 13 dígitos.")]
        [Display(Name = "RUC")]
        public string Ruc { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un email válido.")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Ingrese un teléfono válido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [StringLength(150, ErrorMessage = "El contacto no puede exceder 150 caracteres.")]
        [Display(Name = "Contacto")]
        public string Contacto { get; set; }

        [Display(Name = "Clasificación")]
        public string Clasificacion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }
    }
}
