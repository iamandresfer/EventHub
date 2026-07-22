using System.ComponentModel.DataAnnotations;

namespace EventHub._01.Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s'-]+$", ErrorMessage = "El nombre solo puede contener letras, espacios, guiones y apóstrofes.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El alias es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El alias no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "El alias solo puede contener letras, números, guiones y guiones bajos.")]
        [Display(Name = "Alias")]
        public string Alias { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [MaxLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^[\d\+\-\(\)\s]{8,20}$", ErrorMessage = "El teléfono no es válido.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula y un número.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }
    }
}
