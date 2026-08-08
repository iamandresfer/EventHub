---
tipo: modulo
proyecto: EventHub
modulo: Dashboard
estado: Completado
fecha: 2026-08-08
tags:
  - modulo
  - dashboard
  - kpi
---

# Módulo: Dashboard / KPIs

## Estado
> [!success] Completado (MVP)
> Dashboard consolidado con KPIs, gráficos ECharts y refresco casi-en-vivo por polling.

## Alcance implementado
- **Bussines:** `DashboardDto` extendido (2026-08-08) con: financiero global (`TotalPresupuestoEstimado`, `TotalGastado`, `TotalRecaudado`, `EjecucionGasto`), crew (`TotalOperadores`, `OperadoresActivos`) y `TareasHoy` (`List<TareaHoyDto>`). `EventoService.GetDashboardAsync()` y helpers `CalcularEjecucionGastoAsync()`/`ObtenerTareasHoyAsync()`.
- **Web:** `HomeController.Index` + vista `Home/Index.cshtml` con KPIs en tiempo real, donut de estados, actividad, próximos eventos y eventos finalizados.
- Upgrade 2026-07-24: gráficos profesionales con ECharts, mejoras UX.
- Consolidación 2026-08-08: fila de cards financieros (estimado/gastado/recaudado/% ejecución) + card crew + bloque "Tareas de hoy" (enlaza al kanban `Eventos/Tareas/{id}`).

## Casi en vivo (2026-08-08)
- `HomeController.ObtenerKpis()` (JSON) + polling de 15s en `Home/Index.cshtml` que actualiza todos los cards y la lista de tareas de hoy sin recargar; pausa con `document.hidden` y refresca al volver la pestaña visible.
- Badge de notificaciones con polling de 30s desde `_Layout.cshtml` (ver [[modulos/Notificaciones]]).
- Reemplazable por SignalR post-MVP.

## Enlaces
- Plan: [[superpowers/plans/2026-07-24-dashboard-upgrade|Dashboard Upgrade]]
- Módulo: [[modulos/Eventos]], [[modulos/Presupuesto]]
