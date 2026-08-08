---
tipo: modulo
proyecto: EventHub
modulo: Presupuesto
estado: Completado
fecha: 2026-08-08
tags:
  - modulo
  - presupuesto
  - financiero
---

# Módulo: Presupuesto (Gastos / Ingresos)

## Estado
> [!success] Completado (MVP)
> Presupuesto con gastos, ingresos y gráficos ECharts.

## Alcance implementado
- **BD:** `tbl_gastos` y `tbl_ingresos` (scripts `create_tbl_gastos.sql`, `create_tbl_ingresos.sql`).
- **Data:** entidades `Gasto`, `Ingreso`.
- **Bussines:** `GastoService`, `IngresoService` con DTOs (`GastoDto`, `GastoFormDto`, `IngresoDto`, `IngresoFormDto`); recalculo automático de totales en `tbl_eventos` (`eve_gasto_real`, `eve_total_ingresos`).
- **Web:** `PresupuestoController` + vistas `Presupuesto/*`; gráficos con ECharts y header de sección unificado.

## Validación 2026-08-08
- Verificado con SQL: `eve_gasto_real` y `eve_total_ingresos` coinciden con la suma de `tbl_gastos`/`tbl_ingresos` por evento.
- Smoke test manual pendiente (crear/editar/eliminar gasto e ingreso desde la UI).

## Detalles relevantes
- `tbl_eventos` mantiene `eve_presupuesto_estimado` (nullable) como referencia.
- Nota: `ALTER_add_categoria_foto_columns.sql` NO toca `tbl_ingresos`; agrega `tar_categoria` a `tbl_tareas` y columnas de foto (`ope_foto_url`/`cro_foto_url`) a operadores/crew.

## Enlaces
- SQL: [[sql/create_tbl_gastos]], [[sql/create_tbl_ingresos]], [[sql/alter_eventos_presupuesto_nullable]]
- Módulo: [[modulos/Eventos]]
