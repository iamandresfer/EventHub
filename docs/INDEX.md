---
title: EventProduction Hub - Índice
tags:
  - project
  - eventhub
  - index
proyecto: EventHub
estado: En desarrollo
version: v0
---

# 📊 EventProduction Hub — Índice

Sistema integral de gestión de eventos (Ecuador). ASP.NET MVC 5 + EF6 + SQL Server.

> [!tip] Documento maestro
> Panorama rápido: [[ALCANCE]] · Contexto completo (visión, BD, flujos, guía técnica): [[opencode]]

## Estado del proyecto

- **Estado:** MVP en desarrollo
- **Plazo:** Octubre 2026
- **Base de datos:** `EventHubv01` (SQL Server 2019+)
- **Stack:** C# ASP.NET MVC + EF6 + SQL Server

> [!todo] Estado en vivo
> Consulta [[Proyecto.base]] para ver módulos, planes y migraciones con filtros y fórmulas.
> La bitácora de desarrollo está en [[BITACORA]].

## Módulos

| Módulo | Nota | Estado |
|--------|------|--------|
| Autenticación / Seguridad | [[modulos/Seguridad]] | Completado (MVP) |
| Eventos y Clientes | [[modulos/Eventos]] | Completado (MVP) |
| Tareas y Cronogramas | [[modulos/Tareas]] | Completado (MVP) |
| Operadores / Crew | [[modulos/Operadores]] | Completado (unificado) |
| Presupuesto (Gastos/Ingresos) | [[modulos/Presupuesto]] | Completado (MVP) |
| Notificaciones | [[modulos/Notificaciones]] | Completado (MVP) |
| Dashboard / KPIs | [[modulos/Dashboard]] | Completado (MVP) |

## Arquitectura

- [[arquitectura/Arquitectura.canvas|Arquitectura 3 capas]]
- [[arquitectura/DiagramaTablas.canvas|Diagrama de tablas]]

## Planes y specs

### Planes (superpowers)
- [[superpowers/plans/2026-08-04-unify-operadores-crew|Unificar Operadores + Crew (2026-08-04)]]
- [[superpowers/plans/2026-07-24-ui-improvements|UI Improvements (2026-07-24)]]
- [[superpowers/plans/2026-07-24-dashboard-upgrade|Dashboard Upgrade (2026-07-24)]]
- [[superpowers/plans/2026-07-20-gestion-eventos-clientes-dashboard|Gestión Eventos/Clientes + Dashboard (2026-07-20)]]
- [[superpowers/plans/2026-07-20-auth-svg-icons-fix|Auth + SVG Icons Fix (2026-07-20)]]

### Specs (diseño)
- [[superpowers/specs/2026-07-24-ui-improvements-design|UI Improvements — Design]]
- [[superpowers/specs/2026-07-20-gestion-eventos-clientes-dashboard|Eventos/Clientes — Design]]
- [[superpowers/specs/2026-07-20-auth-svg-icons-fix-design|Auth — Design]]

## Sesiones

- [[BITACORA|Bitácora de desarrollo]] — cronología con resumen y link a cada sesión.
- Notas de sesión en `docs/sesiones/` (nombres `YYYY-MM-DD_tema`):
  - [[sesiones/2026-08-07_habilitacion-cli-obsidian|2026-08-07 — Habilitación CLI + Obsidian]]

## Base de datos (scripts SQL)

- [[sql/ALTER_add_categoria_foto_columns|ALTER: categoria foto columns]]
- [[sql/alter_eventos_presupuesto_nullable|ALTER: presupuesto nullable]]
- [[sql/create_tbl_gastos|CREATE: tbl_gastos]]
- [[sql/create_tbl_ingresos|CREATE: tbl_ingresos]]

## Vault

- `docs/` es el vault de Obsidian del proyecto (trackeado en git).
- La configuración del vault vive en `docs/.obsidian/`.
- El agente consulta notas con `obsidian read/search/base:query`.
- El CLI se habilita con `scripts/obsidian-cli-setup.ps1` (registra `C:\Program Files\Obsidian` en el PATH y activa el toggle en Settings → Acerca de → "Interfaz de línea de comandos"). Tras ejecutarlo, abre una terminal nueva.
- `scripts/obsidian-cli-setup.ps1 -Start` abre Obsidian con el vault `docs`.
- `scripts/nueva-sesion.ps1 -Tema "..."` crea la nota de sesión del día (`YYYY-MM-DD_tema`).
- El CLI opera sobre el vault enfocado en Obsidian; abre `docs/` en Obsidian antes de consultarlo por CLI.

## Enlaces externos

- [Documentación: docs.eventhub.local](https://docs.eventhub.local)
- [Help Obsidian CLI](https://help.obsidian.md/cli)
