using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EventHub._01.Web.Models;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController()
        {
            var context = new EventHubContext();
            var emailService = new EmailService();
            _authService = new AuthService(context, emailService);
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
            {
                if (result.Message.Contains("bloqueada") || result.Message.Contains("bloqueado"))
                {
                    Session["FailedLoginAttempts"] = (int)(Session["FailedLoginAttempts"] ?? 0) + 1;
                }
                else
                {
                    Session["FailedLoginAttempts"] = 0;
                }
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            Session["FailedLoginAttempts"] = 0;

            if (result.Requires2FA)
            {
                Session["Pending2FAUserId"] = result.UserId;
                Session["Pending2FAUserName"] = result.UserName;
                return RedirectToAction("Verify2FA");
            }

            SetAuthCookie(result.UserId.Value, result.UserName, result.Role, model.RememberMe);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new RegisterDto
            {
                Alias = model.Alias,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Nombre = model.Nombre,
                Telefono = model.Telefono
            };

            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            ViewBag.SuccessMessage = result.Message;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.GeneratePasswordResetOtpAsync(model.Email);

            Session["ResetEmail"] = model.Email;
            return RedirectToAction("VerifyResetCode");
        }

        [HttpGet]
        public ActionResult VerifyResetCode()
        {
            if (Session["ResetEmail"] == null)
                return RedirectToAction("ForgotPassword");

            var model = new VerifyResetOtpViewModel
            {
                Email = (string)Session["ResetEmail"]
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyResetCode(VerifyResetOtpViewModel model)
        {
            if (Session["ResetEmail"] == null)
                return RedirectToAction("ForgotPassword");

            if (!ModelState.IsValid)
                return View(model);

            model.Email = (string)Session["ResetEmail"];
            var isValid = await _authService.VerifyPasswordResetOtpAsync(model.Email, model.Code);

            if (!isValid)
            {
                ModelState.AddModelError("", "Código inválido o expirado.");
                return View(model);
            }

            Session["ResetVerified"] = true;
            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            if (Session["ResetEmail"] == null || Session["ResetVerified"] == null)
                return RedirectToAction("ForgotPassword");

            var model = new ResetPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (Session["ResetEmail"] == null || Session["ResetVerified"] == null)
                return RedirectToAction("ForgotPassword");

            if (!ModelState.IsValid)
                return View(model);

            var dto = new ResetPasswordDto
            {
                Email = (string)Session["ResetEmail"],
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword
            };

            var result = await _authService.ResetPasswordAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            Session.Remove("ResetEmail");
            Session.Remove("ResetVerified");

            ViewBag.SuccessMessage = result.Message;
            return View();
        }

        [HttpGet]
        public ActionResult Verify2FA()
        {
            if (Session["Pending2FAUserId"] == null)
                return RedirectToAction("Login");

            var model = new VerifyOtpViewModel
            {
                UserId = (int)Session["Pending2FAUserId"]
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Verify2FA(VerifyOtpViewModel model)
        {
            if (Session["Pending2FAUserId"] == null)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            model.UserId = (int)Session["Pending2FAUserId"];
            var isValid = await _authService.VerifyOtpAsync(model.UserId, model.Code);

            if (!isValid)
            {
                ModelState.AddModelError("", "Código inválido o expirado.");
                return View(model);
            }

            Session.Remove("Pending2FAUserId");
            Session.Remove("Pending2FAUserName");

            var context = new EventHubContext();
            var usuario = await context.Usuarios.FindAsync(model.UserId);
            if (usuario != null)
            {
                SetAuthCookie(usuario.Id, usuario.Nombre, usuario.Rol, false);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendOtp()
        {
            if (Session["Pending2FAUserId"] == null)
                return RedirectToAction("Login");

            var userId = (int)Session["Pending2FAUserId"];
            var otpCode = await _authService.GenerateOtpAsync(userId);

            var context = new EventHubContext();
            var usuario = await context.Usuarios.FindAsync(userId);
            if (usuario != null)
            {
                var emailService = new EmailService();
                await emailService.SendOtpEmailAsync(usuario.Email, usuario.Nombre, otpCode);
            }

            TempData["OtpResent"] = true;
            return RedirectToAction("Verify2FA");
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }

        private void SetAuthCookie(int userId, string userName, string role, bool persistent)
        {
            var userData = $"{userId}|{role}";
            var ticket = new FormsAuthenticationTicket(
                1,
                userName,
                DateTime.Now,
                DateTime.Now.Add(persistent ? TimeSpan.FromDays(30) : TimeSpan.FromHours(8)),
                persistent,
                userData
            );
            var encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };
            Response.Cookies.Add(cookie);
        }
    }
}
