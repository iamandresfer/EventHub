---
tipo: modulo
proyecto: EventHub
modulo: Eventos
estado: Completado
fecha: 2026-07-20
tags:
  - modulo
  - eventos
  - clientes
---

# Módulo: Eventos y Clientes

## Estado
> [!success] Completado (MVP)
> CRUD de eventos y clientes con dashboard, sidebar y entidades EF.

## Alcance implementado
- **Data:** entidades `Evento`, `Cliente`, `TipoEvento`, `Venue` mapeadas a `tbl_eventos`, `tbl_clientes`, etc.
- **Bussines:** `EventoService`, `ClienteService`, `TipoEventoService`, `VenueService` con DTOs (`EventoListDto`, `EventoFormDto`, `ClienteDto`).
- **Web:**
  - `EventosController` — CRUD + detalle
  - `ClientesController` — CRUD
  - `HomeController` — Dashboard con KPIs
  - Vistas: `Eventos/Index|Create|Edit|Details`, `Clientes/Index|Create|Edit|Details`

## Detalles relevantes
- `tbl_eventos` ahora permite presupuesto nullable (script `alter_eventos_presupuesto_nullable.sql`).
- Relación evento → cliente → venue → tipo de evento en dashboard.

## Enlaces
- Plan: [[superpowers/plans/2026-07-20-gestion-eventos-clientes-dashboard|Gestión Eventos/Clientes + Dashboard]]
- Spec: [[superpowers/specs/2026-07-20-gestion-eventos-clientes-dashboard|Eventos/Clientes — Design]]
