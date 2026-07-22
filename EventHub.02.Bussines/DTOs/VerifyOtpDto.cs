namespace EventHub._02.Bussines.DTOs
{
    public class VerifyOtpDto
    {
        public int UserId { get; set; }
        public string Code { get; set; }
        public bool RememberBrowser { get; set; }
    }
}
