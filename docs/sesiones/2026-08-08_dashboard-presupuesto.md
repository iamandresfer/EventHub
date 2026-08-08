---
tipo: sesion
proyecto: EventHub
fecha: "2026-08-08"
modulo: "Dashboard"
estado: "Completado"
tags:
  - sesion
---

# 📋 Sesión — Cierre MVP: Dashboard consolidado + casi-en-vivo

## Cambios realizados

### EventHub.02.Bussines
- `DashboardDto` extendido: `TotalPresupuestoEstimado`, `TotalGastado`, `TotalRecaudado`, `EjecucionGasto` (decimal?), `TotalOperadores`, `OperadoresActivos`, `TareasHoy` (`List<TareaHoyDto>`).
- Nuevo DTO `TareaHoyDto` (`DTOs/TareaHoyDto.cs`, registrado en el `.csproj` — proyecto no SDK-style).
- `EventoService.GetDashboardAsync()`: agrega sumas financieras (`SumAsync` sobre `tbl_eventos`), conteos de crew y tareas de hoy (helpers `CalcularEjecucionGastoAsync` y `ObtenerTareasHoyAsync`).

### EventHub.01.Web
- `HomeController.ObtenerKpis()` [GET] JSON con todos los KPIs del dashboard.
- `Home/Index.cshtml`:
  - Fila de 5 cards nuevos: **Presupuesto Estimado, Total Gastado, Total Recaudado, Ejecución de Gasto (%**, rojo si >100%) y **Crew / Operadores**.
  - Bloque **"Tareas de hoy"**: lista con estado (color), evento y responsable; cada tarea enlaza al kanban `Eventos/Tareas/{id}`.
  - IDs en las cards de stats existentes para refresco.
  - Polling de **15s** a `ObtenerKpis` que actualiza todos los cards y la lista sin recargar; pausa con `document.hidden` y refresca al volver a la pestaña.
- `_Layout.cshtml`: polling del **badge de notificaciones cada 30s** (reusa `ObtenerRecientes`, pausa con `document.hidden`).

## Validación
- Build limpio (`EventHub.v0.slnx`).
- SQL verificado: sumas financieras globales (estimado 160500, gastado 52100, recaudado 50018.50), crew 7/7, 1 tarea para hoy; `eve_gasto_real`/`eve_total_ingresos` consistentes con la suma de `tbl_gastos`/`tbl_ingresos` por evento.

## Docs
- `modulos/Dashboard.md`, `modulos/Presupuesto.md`, `modulos/Notificaciones.md` → `estado: Completado`.
- `INDEX.md`: tabla de módulos actualizada.
- `BITACORA.md`: entrada Sesión 6.

## Pendientes
- [ ] Smoke test manual en navegador: dashboard (cards + tareas de hoy + polling), presupuesto UI, kanban, crew, email.
- [ ] SignalR post-MVP para reemplazar el polling (dashboard + notificaciones).
