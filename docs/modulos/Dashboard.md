---
tipo: modulo
proyecto: EventHub
modulo: Dashboard
estado: En desarrollo
fecha: 2026-07-24
tags:
  - modulo
  - dashboard
  - kpi
---

# Módulo: Dashboard / KPIs

## Estado
> [!warning] En desarrollo
> Dashboard con KPIs y gráficos (upgrade con ECharts).

## Alcance implementado
- **Bussines:** `DashboardDto`, `EventoService.GetAllAsync` con includes de Cliente/Venue/TipoEvento.
- **Web:** `HomeController` + vista `Home/*` con KPIs en tiempo real, cronograma del día, alertas de pagos vencidos y estado de inventario.
- Upgrade 2026-07-24: gráficos profesionales con ECharts, mejoras UX.

## Enlaces
- Plan: [[superpowers/plans/2026-07-24-dashboard-upgrade|Dashboard Upgrade]]
- Módulo: [[modulos/Eventos]], [[modulos/Presupuesto]]
