using System.ComponentModel.DataAnnotations;

namespace EventHub._01.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
