using System.Threading.Tasks;

namespace EventHub._02.Bussines.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string userName, string otpCode);
        Task SendPasswordResetOtpEmailAsync(string toEmail, string userName, string otpCode);
        Task SendWelcomeEmailAsync(string toEmail, string userName);
    }
}
