---
title: Bitácora de Desarrollo
tags:
  - project
  - bitacora
tipo: bitacora
proyecto: EventHub
---

# 📋 Bitácora de Desarrollo - EventProduction Hub

> [!info] Índice del proyecto
> Volver a [[INDEX]] · Estado vivo en [[Proyecto.base]]

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

## Sesión 2 — 2026-07-22: Versión inicial + notificaciones y crew por evento

### Cambios Realizados

#### Repositorio
- Commit inicial: EventHub MVC con autenticación, eventos, clientes y tareas (`f770020`).
- Excluido `.opencode/skills` de la detección de lenguaje de GitHub (`44bbddb`).
- Sistema de notificaciones + crew por evento (`538c50e`).
- Fix: archivos nuevos agregados al `.csproj` para compilación (`55cce8e`).

### Notas
- Los specs de diseño de Auth/UI (2026-07-20 y 2026-07-24) viven en `docs/superpowers/specs/` (trabajo de diseño sin commits).

## Sesión 3 — 2026-08-05: Presupuesto (ingresos), ECharts y operadores unificados

### Cambios Realizados

#### Repositorio
- Módulo de presupuesto con ingresos, gráficos ECharts, header de sección unificado y operadores (`4483866`).

#### Módulos afectados
- Presupuesto (Gastos/Ingresos) → En desarrollo (`docs/modulos/Presupuesto.md`).
- Operadores / Crew → unificado (plan `2026-08-04-unify-operadores-crew`).

## Sesión 4 — 2026-08-07: Habilitación CLI + Obsidian

### Cambios Realizados
- Habilitado el CLI de Obsidian + vault `docs` registrado.
- Scripts `nueva-sesion.ps1` (crea sesión) y `obsidian-cli-setup.ps1 -Start` (abre Obsidian + vault).
- `AGENTS.md` con ritual de inicio/cierre de sesión y skills de Obsidian.
- Backfill de sesiones 2 y 3 desde git log.
- Creada nota `ALCANCE.md` (panorama del proyecto) e integrada al ritual de inicio.

### Próximos Pasos
- Revisar la guía de sesiones y continuar los módulos En curso.

Nota de sesión: [[2026-08-07_habilitacion-cli-obsidian]]

## Sesión 5 — 2026-08-08: Estabilizacion MVP

### Cambios Realizados

#### Repositorio
- Checkpoint `3253f97`: credenciales SMTP movidas a `Web.Smtp.config` (gitignored) + `.example`; incluyó también los cambios de herramientas de la Sesión 4.

#### EventHub.01.Web
- Habilitado `OperadoresController` (estaba fuera del `.csproj`, código muerto) con CRUD/búsqueda/toggle/remover AJAX (`Views/Operadores/Index.cshtml`).
- Vista `Views/Crew/Index.cshtml` (crew por evento) + nav "Operadores" en `_Layout`.
- `Web.config`: `Operadores/MisTareas` anónimo para el flujo por email.

#### EventHub.02.Bussines
- `TareaService.ActualizarTarea` + `CrearTarea(creadoPorId)` puebla `CreadoPorId`.
- `NotificacionService`: email de notificación enlaza a `Operadores/MisTareas?email=...`.

#### Docs
- Corregido error en `Presupuesto.md` (script ALTER: `tar_categoria`/foto operadores, no `tbl_ingresos`); `Operadores.md` con wiring nuevo; `Notificaciones.md`/`Tareas.md` alineados.

### Pendientes
- Rotar app password SMTP + smoke test manual en navegador (detalle en la nota).

Nota de sesión: [[2026-08-08_estabilizacion-mvp]]

## Sesión 6 — 2026-08-08: Cierre MVP — Dashboard consolidado + casi-en-vivo

### Cambios Realizados

#### Repositorio
- Commit: unificación operadores+crew en `tbl_crew_operadores` (`a939f76`); fix sintaxis Razor nav Crew (`b0615b5`).

#### EventHub.02.Bussines
- `DashboardDto` extendido (financiero global, crew, `TareasHoy`) + DTO `TareaHoyDto`.
- `EventoService.GetDashboardAsync()` con sumas financieras, conteos de crew y tareas de hoy.

#### EventHub.01.Web
- `HomeController.ObtenerKpis()` (JSON).
- `Home/Index.cshtml`: cards financieros (estimado/gastado/recaudado/% ejecución) + card crew + bloque "Tareas de hoy"; polling 15s con pausa por pestaña oculta.
- `_Layout.cshtml`: polling del badge de notificaciones cada 30s.

#### Docs
- Módulos `Dashboard`, `Presupuesto`, `Notificaciones` → `Completado` (MVP). `INDEX.md` actualizado.

### Próximos Pasos
- Smoke test manual en navegador.
- SignalR post-MVP para reemplazar el polling.

Nota de sesión: [[2026-08-08_dashboard-presupuesto]]
