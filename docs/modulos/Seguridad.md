---
tipo: modulo
proyecto: EventHub
modulo: Seguridad
estado: Completado
fecha: 2026-07-16
tags:
  - modulo
  - seguridad
  - auth
---

# Módulo: Seguridad y Autenticación

## Estado
> [!success] Completado (MVP)
> Autenticación con bcrypt, OTP por email, 2FA y recuperación de contraseña.

## Alcance implementado
- **BD:** `tbl_usuarios` con campos OTP/2FA; `usu_pass_hash` como `NVARCHAR(256)` para bcrypt (script `ALTER_usuario_security.sql`).
- **Data:** entidad `Usuario` + `EventHubContext` (EF6 Database-First).
- **Bussines:** `IAuthService`/`AuthService`, DTOs (`LoginDto`, `RegisterDto`, `ForgotPasswordDto`, `ResetPasswordDto`, `VerifyOtpDto`).
- **Web:** `AuthController` (7 acciones), `_AuthLayout.cshtml` responsivo con dark/light, vistas Login/Register/ForgotPassword/ResetPassword/Verify2FA/AccessDenied.
- **Config:** Forms Authentication + placeholders SMTP en `Web.config`.

## Pendientes
- [ ] Credenciales SMTP reales en `Web.config`
- [ ] Logo e identidad visual (placeholder actual)
- [ ] Perfil de usuario y cambio de contraseña desde panel
- [ ] SignalR para notificaciones en tiempo real (post-MVP)

## Enlaces
- Plan: [[superpowers/plans/2026-07-20-auth-svg-icons-fix|Auth SVG Icons Fix]]
- Spec: [[superpowers/specs/2026-07-20-auth-svg-icons-fix-design|Auth SVG Icons Fix — Design]]
- Bitácora: [[BITACORA#Sesión 1 — 2026-07-16|Sesión 1]]
