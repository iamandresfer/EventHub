using System.ComponentModel.DataAnnotations;

namespace EventHub._01.Web.Models
{
    public class VerifyResetOtpViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "El código debe ser de 4 dígitos.")]
        [Display(Name = "Código de verificación")]
        public string Code { get; set; }
    }
}
