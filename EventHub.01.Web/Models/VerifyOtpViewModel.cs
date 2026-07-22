using System.ComponentModel.DataAnnotations;

namespace EventHub._01.Web.Models
{
    public class VerifyOtpViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe ser de 6 dígitos.")]
        [Display(Name = "Código de verificación")]
        public string Code { get; set; }

        [Display(Name = "Recordar este navegador")]
        public bool RememberBrowser { get; set; }
    }
}
