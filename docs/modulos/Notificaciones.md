---
tipo: modulo
proyecto: EventHub
modulo: Notificaciones
estado: En desarrollo
fecha: 2026-08-04
tags:
  - modulo
  - notificaciones
---

# Módulo: Notificaciones

## Estado
> [!warning] En desarrollo
> Sistema de notificaciones por evento + campana en el header.

## Alcance implementado
- **Data:** entidad `Notificacion` (`tbl_notificaciones`).
- **Bussines:** `NotificacionService` con DTOs (`NotificacionDto`); `INotificacionService`; envío de email asíncrono (fire-and-forget con contexto propio).
- **Web:** campana de notificaciones en `_Layout.cshtml`; generación de notificaciones desde tareas en `EventosController`; `HomeController` (ObtenerRecientes, MarcarLeida, MarcarTodasLeidas).
- **Flujo operador (2026-08-08):** el email de notificación enlaza a `Operadores/MisTareas?email=...` (sin login) en vez del kanban autenticado.

## Pendientes
- [ ] SignalR para notificaciones en tiempo real (post-MVP)

## Enlaces
- Módulo: [[modulos/Operadores]], [[modulos/Tareas]]
