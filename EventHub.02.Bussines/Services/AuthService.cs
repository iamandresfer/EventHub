using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class AuthService : IAuthService
    {
        private readonly EventHubContext _context;
        private readonly IEmailService _emailService;

        public AuthService(EventHubContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return new AuthResult { Success = false, Message = "Las contraseñas no coinciden." };

            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return new AuthResult { Success = false, Message = "El email ya está registrado." };

            if (await _context.Usuarios.AnyAsync(u => u.Alias == dto.Alias))
                return new AuthResult { Success = false, Message = "El alias ya está en uso." };

            var usuario = new Usuario
            {
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12),
                PasswordSalt = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Rol = "ProductorCampo",
                FechaRegistro = DateTime.Now,
                PasswordCambio = DateTime.Now,
                Intentos = 0,
                Bloqueo = false,
                Estado = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            await _emailService.SendWelcomeEmailAsync(dto.Email, dto.Nombre);

            return new AuthResult
            {
                Success = true,
                Message = "Registro exitoso. Ya puedes iniciar sesión.",
                UserId = usuario.Id,
                UserName = usuario.Nombre,
                Role = usuario.Rol
            };
        }

        public async Task<AuthResult> LoginAsync(LoginDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return new AuthResult { Success = false, Message = "Credenciales inválidas." };

            if (!usuario.Estado)
                return new AuthResult { Success = false, Message = "Cuenta desactivada. Contacta al administrador." };

            if (usuario.Bloqueo && usuario.BloqueoFecha.HasValue)
            {
                if (usuario.BloqueoFecha.Value > DateTime.Now)
                    return new AuthResult { Success = false, Message = $"Cuenta bloqueada hasta {usuario.BloqueoFecha.Value:HH:mm}. Demasiados intentos fallidos." };
                usuario.Bloqueo = false;
                usuario.Intentos = 0;
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                usuario.Intentos++;
                if (usuario.Intentos >= 5)
                {
                    usuario.Bloqueo = true;
                    usuario.BloqueoFecha = DateTime.Now.AddMinutes(15);
                }
                await _context.SaveChangesAsync();
                return new AuthResult { Success = false, Message = "Credenciales inválidas." };
            }

            usuario.Intentos = 0;
            usuario.UltimoAcceso = DateTime.Now;
            await _context.SaveChangesAsync();

            var otpCode = await GenerateOtpAsync(usuario.Id);
            await _emailService.SendOtpEmailAsync(usuario.Email, usuario.Nombre, otpCode);

            return new AuthResult
            {
                Success = true,
                Requires2FA = true,
                UserId = usuario.Id,
                UserName = usuario.Nombre,
                Message = "Código de verificación enviado a tu correo."
            };
        }

        public async Task<string> GenerateOtpAsync(int userId)
        {
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null) return null;

            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var otpCode = (BitConverter.ToUInt32(bytes, 0) % 900000 + 100000).ToString();

                usuario.OtpCode = otpCode;
                usuario.OtpExpiry = DateTime.Now.AddMinutes(10);

                await _context.SaveChangesAsync();

                return otpCode;
            }
        }

        public async Task<bool> VerifyOtpAsync(int userId, string code)
        {
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null) return false;

            if (usuario.OtpCode != code)
                return false;

            if (!usuario.OtpExpiry.HasValue || usuario.OtpExpiry.Value < DateTime.Now)
                return false;

            usuario.OtpCode = null;
            usuario.OtpExpiry = null;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<AuthResult> GeneratePasswordResetOtpAsync(string email)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null)
                return new AuthResult { Success = false, Message = "Si el email existe, recibirás un código de recuperación." };

            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var otpCode = (BitConverter.ToUInt32(bytes, 0) % 10000).ToString("D4");

                usuario.OtpCode = otpCode;
                usuario.OtpExpiry = DateTime.Now.AddMinutes(10);
                await _context.SaveChangesAsync();

                await _emailService.SendPasswordResetOtpEmailAsync(usuario.Email, usuario.Nombre, otpCode);
            }

            return new AuthResult { Success = true, Message = "Si el email existe, recibirás un código de recuperación." };
        }

        public async Task<bool> VerifyPasswordResetOtpAsync(string email, string code)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null) return false;

            if (usuario.OtpCode != code) return false;

            if (!usuario.OtpExpiry.HasValue || usuario.OtpExpiry.Value < DateTime.Now) return false;

            return true;
        }

        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return new AuthResult { Success = false, Message = "Las contraseñas no coinciden." };

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null)
                return new AuthResult { Success = false, Message = "Sesión inválida. Solicita un nuevo código." };

            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, usuario.PasswordHash))
                return new AuthResult { Success = false, Message = "La nueva contraseña no puede ser igual a la actual." };

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword, workFactor: 12);
            usuario.PasswordCambio = DateTime.Now;
            usuario.OtpCode = null;
            usuario.OtpExpiry = null;
            usuario.Intentos = 0;
            usuario.Bloqueo = false;
            await _context.SaveChangesAsync();

            return new AuthResult { Success = true, Message = "Contraseña actualizada exitosamente." };
        }
    }
}
