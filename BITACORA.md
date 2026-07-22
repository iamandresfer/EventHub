# 📋 Bitácora de Desarrollo - EventProduction Hub

## Sesión 1 — 2026-07-16: Módulo de Seguridad y Autenticación

### Cambios Realizados

#### Base de Datos
- Modificada `tbl_usuarios`: agregados campos para OTP por email, 2FA, recuperación de contraseña
- Cambiado `usu_pass_hash` de `VARBINARY(256)` a `NVARCHAR(256)` para soportar bcrypt
- Creado script `ALTER_usuario_security.sql` con los cambios aplicables a DB existente
- Actualizado `dbo_general.sql` con schema completo

#### Capa de Datos (EventHub.03.Data)
- Agregado Entity Framework 6 (Database-First manual)
- Agregado BCrypt.Net-Next
- Creado `EventHubContext.cs` con DbContext y Fluent API
- Creada entidad `Usuario.cs` mapeada a `tbl_usuarios`

#### Capa de Negocio (EventHub.02.Bussines)
- Creado `IAuthService.cs` / `AuthService.cs`
- DTOs: `LoginDto`, `RegisterDto`, `ForgotPasswordDto`, `ResetPasswordDto`, `VerifyOtpDto`
- Lógica de registro con bcrypt, login con validación, OTP, reset de contraseña

#### Capa de Presentación (EventHub.01.Web)
- Creado `_AuthLayout.cshtml` con diseño responsivo y toggle dark/light mode
- Creado `AuthController.cs` con 7 acciones (Login, Register, Logout, ForgotPassword, ResetPassword, Verify2FA, AccessDenied)
- ViewModels: `LoginViewModel`, `RegisterViewModel`, `ForgotPasswordViewModel`, `ResetPasswordViewModel`, `VerifyOtpViewModel`
- Vistas: Login, Register, ForgotPassword, ResetPassword, Verify2FA, AccessDenied
- Configurado Forms Authentication en Web.config
- Configurado SMTP placeholders en Web.config
- Actualizado `_ViewStart.cshtml` para usar `_AuthLayout` en carpeta Auth

### Pendientes
- Configurar credenciales SMTP reales en Web.config
- Definir logo e identidad visual (actualmente placeholder)
- Implementar perfil de usuario y cambio de contraseña desde panel
- Integrar SignalR para notificaciones en tiempo real (post-MVP)

### Próximos Pasos Sugeridos
- Módulo de Gestión de Usuarios (CRUD) para administradores
- Dashboard principal con KPIs
- Módulo de Clientes
