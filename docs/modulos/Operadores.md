---
tipo: modulo
proyecto: EventHub
modulo: Operadores
estado: Completado
fecha: 2026-08-04
tags:
  - modulo
  - operadores
  - crew
---

# Módulo: Operadores / Crew

## Estado
> [!success] Completado — unificados
> `tbl_crew_operadores` migrada a `tbl_operadores` con FK `ope_eve_id` a eventos.

## Alcance implementado
- **BD:** migración `MergeOperadoresCrew.sql` — agrega `ope_eve_id`, `ope_num_cuenta`, `ope_banco`; migra datos; FK `tbl_tareas` → `tbl_operadores`.
- **Data:** entidad `Operador` con `EventoId`, `NumeroCuenta`, `Banco`; `CrewOperador` eliminada.
- **Bussines:** `OperadorService` absorbe la lógica de Crew (`GetConEventos`, `RemoverDeEvento`, `GetPorEvento`, `GetActivos`); `CrewService` eliminado.
- **Web:**
  - `OperadoresController` — CRUD + MisTareas + RemoverDeEventoAjax
  - `CrewController` simplificado como wrapper (usa `OperadorService`)
  - Vistas: `Operadores/*`, `Crew/IndexGlobal.cshtml`

## Detalles relevantes
- Un operador puede estar asignado a un evento (via `ope_eve_id`) o ser global (`null`).
- Relación 1:1 con usuario por email (`usu_email` ↔ `ope_email`).

## Enlaces
- Plan: [[superpowers/plans/2026-08-04-unify-operadores-crew|Unificar Operadores + Crew]]
- Módulo: [[modulos/Tareas]]
