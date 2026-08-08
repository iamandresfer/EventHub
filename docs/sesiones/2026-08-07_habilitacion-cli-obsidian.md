---
tipo: sesion
proyecto: EventHub
fecha: "2026-08-07"
modulo: "Herramientas"
estado: "En curso"
tags:
  - sesion
---

# 📋 Sesión — Habilitación CLI + Obsidian

## Cambios realizados

### Herramientas y contexto
- Habilitado el CLI de Obsidian (`scripts/obsidian-cli-setup.ps1`) y registrado `C:\Program Files\Obsidian` en el PATH.
- El vault `docs/` queda registrado como vault de Obsidian (config en `docs/.obsidian/`).
- Nuevo script `scripts/nueva-sesion.ps1` para crear sesiones con nombre `YYYY-MM-DD_tema`, setear properties y arrastrar pendientes.
- Nuevo switch `-Start` en `scripts/obsidian-cli-setup.ps1` (abre Obsidian + vault `docs`).
- `AGENTS.md` actualizado con ritual de inicio/cierre de sesión y skills de Obsidian.
- `BITACORA.md` con backfill de sesiones desde git log (Sesiones 2 y 3).
- Creada nota `ALCANCE.md` (panorama de alcance y funcionalidades) e integrada al ritual de inicio de sesión.

### Base de Datos
- Sin cambios.

### EventHub.03.Data
- Sin cambios.

### EventHub.02.Bussines
- Sin cambios.

### EventHub.01.Web
- Sin cambios.

## Pendientes
- [ ] Decidir el commit de estos cambios de herramientas.
- [ ] Revisar y ajustar la guía de sesiones con el nuevo flujo.

## Próximos pasos sugeridos
- Continuar módulos En curso (Presupuesto, Notificaciones, Dashboard/KPIs).
- Aprovechar `Proyecto.base` y `obsidian tasks` al inicio de cada sesión.

## Enlaces
- Índice: [[INDEX]]
- Bitácora: [[BITACORA]]
- Estado en vivo: [[Proyecto.base]]
