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
- **Bussines:** `NotificacionService` con DTOs (`NotificacionDto`); `INotificacionService`.
- **Web:** campana de notificaciones en `_Layout.cshtml`; generación de notificaciones desde tareas/crew en `EventosController`.

## Pendientes
- [ ] SignalR para notificaciones en tiempo real (post-MVP)
- [ ] Marcar leídas / persistencia de estado de lectura

## Enlaces
- Módulo: [[modulos/Operadores]], [[modulos/Tareas]]
