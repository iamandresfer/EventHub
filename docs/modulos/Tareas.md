---
tipo: modulo
proyecto: EventHub
modulo: Tareas
estado: Completado
fecha: 2026-07-24
tags:
  - modulo
  - tareas
  - cronograma
---

# Módulo: Tareas y Cronogramas

## Estado
> [!success] Completado (MVP)
> Tareas por evento con asignación de operadores, adjuntos y estados.

## Alcance implementado
- **Data:** entidades `Tarea`, `TareaAdjunto` (FK a `tbl_operadores` tras la unificación crew).
- **Bussines:** `TareaService` con DTOs (`TareaDto`, `TareaFormDto`, `TareaAdjuntoDto`).
- **Web:** `EventosController` (acciones Tareas, Create/EditTareaAjax), vista `Eventos/Tareas.cshtml` con drag & drop entre columnas de estado.

## Detalles relevantes
- La tarea referencia a `Operador` (no `CrewOperador`) desde la unificación 2026-08-04.
- Campos clave: `act_estado` (Pendiente, EnProgreso, Completado), `act_orden` para ordenar columnas kanban.

## Enlaces
- Plan: [[superpowers/plans/2026-08-04-unify-operadores-crew|Unificar Operadores + Crew]]
- Módulo: [[modulos/Operadores]]
