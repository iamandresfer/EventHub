---
tipo: modulo
proyecto: EventHub
modulo: Presupuesto
estado: En desarrollo
fecha: 2026-08-04
tags:
  - modulo
  - presupuesto
  - financiero
---

# Módulo: Presupuesto (Gastos / Ingresos)

## Estado
> [!warning] En desarrollo
> Presupuesto con gastos, ingresos y gráficos ECharts.

## Alcance implementado
- **BD:** `tbl_gastos` y `tbl_ingresos` (scripts `create_tbl_gastos.sql`, `create_tbl_ingresos.sql`); `tbl_ingresos` con categoría y foto (script `ALTER_add_categoria_foto_columns.sql`).
- **Data:** entidades `Gasto`, `Ingreso`.
- **Bussines:** `GastoService`, `IngresoService` con DTOs (`GastoDto`, `GastoFormDto`, `IngresoDto`, `IngresoFormDto`).
- **Web:** `PresupuestoController` + vistas `Presupuesto/*`; gráficos con ECharts y header de sección unificado.

## Detalles relevantes
- `tbl_eventos` mantiene `eve_presupuesto_estimado` (nullable) como referencia.

## Enlaces
- SQL: [[sql/create_tbl_gastos]], [[sql/create_tbl_ingresos]], [[sql/ALTER_add_categoria_foto_columns]]
- Módulo: [[modulos/Eventos]]
