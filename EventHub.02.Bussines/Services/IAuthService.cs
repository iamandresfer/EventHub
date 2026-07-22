using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public bool Requires2FA { get; set; }
    }

    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto);
        Task<AuthResult> LoginAsync(LoginDto dto);
        Task<string> GenerateOtpAsync(int userId);
        Task<bool> VerifyOtpAsync(int userId, string code);
        Task<AuthResult> GeneratePasswordResetOtpAsync(string email);
        Task<bool> VerifyPasswordResetOtpAsync(string email, string code);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
