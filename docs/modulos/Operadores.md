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
> [!success] Completado — unificados en una sola tabla
> Todo el personal vive en una única tabla **`tbl_crew_operadores`** (2026-08-08). Antes había dos: `tbl_operadores` (viva) y `tbl_crew_operadores` (legacy con datos duplicados).

## Alcance implementado
- **BD (2026-08-08):** script `UnificarOperadoresCrew.sql` — backup de las 3 tablas, dropeo de la legacy, `sp_rename 'tbl_operadores' → 'tbl_crew_operadores'`, FK recreadas (`FK_tarea_crew_operador`, `FK_crew_operador_evento`).
- **Data:** entidad `Operador` mapeada a `tbl_crew_operadores` (columnas `ope_*`); `CrewOperador` eliminada.
- **Bussines:** `OperadorService` absorbe la lógica de Crew (`GetConEventos`, `RemoverDeEvento`, `GetPorEvento`, `GetActivos`); `CrewService` eliminado.
- **Web:**
  - `OperadoresController` re-habilitado (2026-08-08: estaba fuera del csproj, era código muerto). CRUD + búsqueda + toggle + remover de evento + **subida de foto** vía AJAX.
  - `CrewController` como módulo unificado: `Crew/Index` (sin parámetros) sirve el CRUD global rico; `Crew/Index?eventoId=` sirve el crew por evento.
  - Nav único "Crew" en `_Layout.cshtml` (eliminado el item duplicado "Operadores").
  - Vistas: `Operadores/Index.cshtml` (CRUD + foto con preview), `Operadores/MisTareas.cshtml`, `Crew/Index.cshtml` (crew por evento).
- **Fix 2026-08-08:** `GetConEventos()` fallaba con `NotSupportedException` (List<> dentro del LINQ a SQL); corregido con proyección plana a SQL + materialización en memoria.

## Flujo por email (MisTareas)
- `Operadores/MisTareas?email=...` es anónimo (whitelist en `Web.config`) y lista las tareas del operador por su email.
- Los emails de notificación (TareaCreada / FechaModificada / TareaCompletada) enlazan al botón "Ver mis tareas" → `Operadores/MisTareas?email=...`.
- Pago de deuda técnica 2026-08-08: `EditTareaAjax` delegado a `TareaService.ActualizarTarea`; `Tarea.CreadoPorId` poblado al crear.

## Detalles relevantes
- Un operador puede estar asignado a un evento (via `ope_eve_id`) o ser global (`null`).
- Relación 1:1 con usuario por email (`usu_email` ↔ `ope_email`).
- Foto del operador: `ope_foto_url`, subida a `~/Content/uploads/operadores/`.

## Enlaces
- Plan: [[superpowers/plans/2026-08-04-unify-operadores-crew|Unificar Operadores + Crew]]
- SQL: [[sql/UnificarOperadoresCrew]]
- Módulo: [[modulos/Tareas]]
