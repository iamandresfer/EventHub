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
  - `OperadoresController` re-habilitado (2026-08-08: estaba fuera del csproj, era código muerto). CRUD + búsqueda + toggle + remover de evento vía AJAX.
  - `CrewController` como wrapper (usa `OperadorService`).
  - Vistas: `Operadores/Index.cshtml` (CRUD), `Operadores/MisTareas.cshtml`, `Crew/Index.cshtml` (crew por evento), `Crew/IndexGlobal.cshtml`.
  - Nav "Operadores" en `_Layout.cshtml`.

## Flujo por email (MisTareas)
- `Operadores/MisTareas?email=...` es anónimo (whitelist en `Web.config`) y lista las tareas del operador por su email.
- Los emails de notificación (TareaCreada / FechaModificada / TareaCompletada) enlazan al botón "Ver mis tareas" → `Operadores/MisTareas?email=...`.
- Pago de deuda técnica 2026-08-08: `EditTareaAjax` delegado a `TareaService.ActualizarTarea`; `Tarea.CreadoPorId` poblado al crear.

## Detalles relevantes
- Un operador puede estar asignado a un evento (via `ope_eve_id`) o ser global (`null`).
- Relación 1:1 con usuario por email (`usu_email` ↔ `ope_email`).

## Enlaces
- Plan: [[superpowers/plans/2026-08-04-unify-operadores-crew|Unificar Operadores + Crew]]
- Módulo: [[modulos/Tareas]]
