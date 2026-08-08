# Instrucciones para Agentes IA

**Punteros rápidos**
- Hub del vault (contexto del proyecto): `docs/INDEX.md`
- Documento técnico maestro (visión, BD, flujos): `docs/opencode.md`
- Alcance y funcionalidades (panorama): `docs/ALCANCE.md`
- Estado en vivo de módulos/planes/specs: `docs/Proyecto.base`
- Bitácora cronológica de sesiones: `docs/BITACORA.md`
- Notas de sesión: `docs/sesiones/`

## Antes de trabajar en este proyecto (inicio de sesión)

1. **Abrir Obsidian con el vault `docs`** (si no está abierto):
   ```powershell
   .\scripts\obsidian-cli-setup.ps1 -Start
   ```
2. **Verificar el CLI**: `obsidian vault` (debe responder `docs`). En terminales que no tengan `obsidian` en el PATH, usar la ruta completa `C:\Program Files\Obsidian\Obsidian.com`.
3. **Cargar contexto** (4 comandos):
   - `obsidian read file='ALCANCE'` → panorama del proyecto (alcance y funcionalidades).
   - `obsidian read file='BITACORA'` → últimas sesiones.
   - `obsidian base:query file="Proyecto.base" view="En curso" format=md` → qué está en curso.
   - `obsidian tasks todo` → pendientes abiertos (incluye los de las notas de sesión).
4. **Si es una sesión nueva**, crear la nota:
   ```powershell
   .\scripts\nueva-sesion.ps1 -Tema "tu-tema" -Modulo "Modulo" 
   ```
   El script genera `docs/sesiones/YYYY-MM-DD_tema.md`, setea las properties y arrastra los pendientes abiertos de la sesión anterior.

## Al cerrar una sesión

- Completar la nota `docs/sesiones/YYYY-MM-DD_tema.md` (cambios por capa, pendientes con `- [ ]`, próximos pasos).
- Añadir la entrada corta en `docs/BITACORA.md` (el script ya la crea; ampliarla con un resumen).
- Actualizar `estado` de los módulos afectados en `docs/modulos/*.md` (frontmatter `estado`) para que `Proyecto.base` refleje la realidad.

## Consultas al vault (skills de Obsidian)

- Cargar las skills cuando apliquen: `obsidian-cli`, `obsidian-bases`, `obsidian-markdown` (y `json-canvas`, `defuddle` según la tarea).
- Preferir el CLI sobre archivo plano cuando Obsidian esté abierto:
  - `obsidian read file=...` / `obsidian search query=...`
  - `obsidian base:query file="Proyecto.base" view="..." format=md`
  - `obsidian tasks todo`
  - `obsidian property:set name="estado" value="..." file="modulos/X"`
- Sin Obsidian abierto, leer los `.md` como archivos planos (el vault es una carpeta del repo).

## Arquitectura del código

- Para preguntas de arquitectura/relaciones del código .NET, usar la skill `graphify` (grafo en `graphify-out/`).

## Convenciones

- Notas de sesión: `docs/sesiones/YYYY-MM-DD_tema.md` (template `docs/templates/Sesion.md`).
- Frontmatter consistente en las notas: `tipo`, `proyecto`, `fecha`, `modulo`, `estado`, `tags`.
- Scripts: `scripts/obsidian-cli-setup.ps1 -Start` (abre Obsidian + vault docs) y `scripts/nueva-sesion.ps1` (crea sesión).
