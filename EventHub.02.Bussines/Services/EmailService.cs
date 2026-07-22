using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EventHub._02.Bussines.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly bool _smtpSsl;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _appBaseUrl;

        public EmailService()
        {
            _smtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "localhost";
            _smtpPort = int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out var port) ? port : 25;
            _smtpUser = ConfigurationManager.AppSettings["SmtpUser"] ?? "";
            _smtpPass = ConfigurationManager.AppSettings["SmtpPass"] ?? "";
            _smtpSsl = bool.TryParse(ConfigurationManager.AppSettings["SmtpSsl"], out var ssl) && ssl;
            _fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"] ?? "noreply@eventhub.local";
            _fromName = ConfigurationManager.AppSettings["SmtpFromName"] ?? "EventProduction Hub";
            _appBaseUrl = ConfigurationManager.AppSettings["AppBaseUrl"] ?? "https://localhost:44353";
        }

        public async Task SendOtpEmailAsync(string toEmail, string userName, string otpCode)
        {
            var subject = "EventHub - Código de Verificación 2FA";
            var body = $@"
                <div style='font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, sans-serif; max-width: 400px; margin: 0 auto; padding: 32px;'>
                    <h2 style='color: #1a1a2e;'>Hola {userName},</h2>
                    <p style='color: #6c757d; font-size: 15px;'>Tu código de verificación de dos pasos es:</p>
                    <div style='font-size: 36px; letter-spacing: 12px; text-align: center; padding: 20px; background: #f5f7fa; border-radius: 12px; font-weight: 700; color: #4361ee; margin: 20px 0;'>{otpCode}</div>
                    <p style='color: #6c757d; font-size: 14px;'>Este código expira en 10 minutos.</p>
                    <p style='color: #6c757d; font-size: 14px;'>Si no solicitaste este código, ignora este mensaje.</p>
                    <hr style='border: none; border-top: 1px solid #dee2e6; margin: 24px 0;'>
                    <p style='color: #adb5bd; font-size: 12px;'>EventProduction Hub - Sistema Integral de Gestión de Eventos</p>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetOtpEmailAsync(string toEmail, string userName, string otpCode)
        {
            var subject = "EventHub - Código de Recuperación de Contraseña";
            var body = $@"
                <div style='font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, sans-serif; max-width: 400px; margin: 0 auto; padding: 32px;'>
                    <h2 style='color: #1a1a2e;'>Hola {userName},</h2>
                    <p style='color: #6c757d; font-size: 15px;'>Recibimos una solicitud para restablecer tu contraseña. Tu código de verificación es:</p>
                    <div style='font-size: 42px; letter-spacing: 16px; text-align: center; padding: 20px; background: #f5f7fa; border-radius: 12px; font-weight: 700; color: #4361ee; margin: 20px 0;'>{otpCode}</div>
                    <p style='color: #6c757d; font-size: 14px;'>Este código expira en 10 minutos.</p>
                    <p style='color: #6c757d; font-size: 14px;'>Si no solicitaste restablecer tu contraseña, ignora este mensaje.</p>
                    <hr style='border: none; border-top: 1px solid #dee2e6; margin: 24px 0;'>
                    <p style='color: #adb5bd; font-size: 12px;'>EventProduction Hub - Sistema Integral de Gestión de Eventos</p>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "EventHub - Te damos la Bienvenida";
            var body = $@"
                <div style='font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, sans-serif; max-width: 400px; margin: 0 auto; padding: 32px;'>
                    <h2 style='color: #1a1a2e;'>¡Bienvenido {userName}!</h2>
                    <p style='color: #6c757d; font-size: 15px;'>Tu cuenta ha sido creada exitosamente.</p>
                    <p style='color: #6c757d; font-size: 15px;'>Ya puedes iniciar sesión y comenzar a gestionar tus eventos.</p>
                    <p style='text-align: center; margin: 24px 0;'><a href='{_appBaseUrl}/Auth/Login' style='display: inline-block; padding: 12px 24px; background: #4361ee; color: white; text-decoration: none; border-radius: 8px; font-weight: 600;'>Iniciar Sesión</a></p>
                    <hr style='border: none; border-top: 1px solid #dee2e6; margin: 24px 0;'>
                    <p style='color: #adb5bd; font-size: 12px;'>EventProduction Hub - Sistema Integral de Gestión de Eventos</p>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.EnableSsl = _smtpSsl;
                    client.UseDefaultCredentials = string.IsNullOrEmpty(_smtpUser);

                    if (!string.IsNullOrEmpty(_smtpUser))
                    {
                        client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    }

                    var message = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    message.To.Add(toEmail);

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EmailService] Error enviando email: {ex.Message}");
            }
        }
    }
}
