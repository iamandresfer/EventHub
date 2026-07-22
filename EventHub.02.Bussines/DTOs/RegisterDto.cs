namespace EventHub._02.Bussines.DTOs
{
    public class RegisterDto
    {
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
    }
}
