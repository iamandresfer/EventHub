# Gestión de Eventos, Clientes y Dashboard — Especificación

## Objetivo

Construir la base de gestión de eventos y clientes en EventHub, incluyendo el mapeo de entidades EF, DTOs, servicios, controladores, vistas, y un layout con sidebar lateral. El dashboard principal mostrará métricas resumidas.

## Arquitectura

- **Entidades EF** manuales (patrón existente de `Usuario.cs`) para `tbl_eventos`, `tbl_clientes`, `tbl_tipos_evento`, `tbl_venues`
- **Capa de negocio** con DTOs + interfaces + servicios
- **Controladores MVC** con autorización por roles (`[Authorize]`)
- **Layout con sidebar** lateral responsive, reemplazando el navbar actual
- **Design System**: colores `#2563EB` primary, `#059669` accent, fuente Plus Jakarta Sans

## Entidades (Capa Data)

### Cliente → `tbl_clientes`
- `[Table("tbl_clientes")]`
- Propiedades mapeadas con `[Column("cli_...")]`
- Clasificación: Nuevo, Regular, VIP, Estrategico

### TipoEvento → `tbl_tipos_evento`
- Catálogo simple: Id, Nombre, Descripcion

### Venue → `tbl_venues`
- Propiedades principales: Id, Nombre, Direccion, Ciudad, CapacidadMaxima, Estado

### Evento → `tbl_eventos`
- FK a Cliente, Venue, TipoEvento
- Navegación virtual a cada uno
- Estados: Planificacion, PreProduccion, Ejecucion, Finalizado, Cancelado

### EventHubContext
- Nuevos DbSet: Clientes, Eventos, Venues, TiposEvento

## DTOs y Servicios (Capa Bussines)

### DTOs
- `ClienteDto` — listado y formulario
- `EventoListDto` — para tabla con nombres de relaciones
- `EventoFormDto` — para creación/edición
- `DashboardDto` — métricas para el home

### Servicios
- `IClienteService` / `ClienteService` — CRUD completo
- `IEventoService` / `EventoService` — CRUD + cambio de estado + filtros por rol
- `IVenueService` / `VenueService` — venues disponibles
- `ITipoEventoService` / `TipoEventoService` — listar tipos

## Layout

Sidebar lateral izquierdo (240px fijo) con:
- Logo + nombre
- Links: Dashboard, Eventos, Clientes, Venues
- Menú contextual por rol (abajo)
- Usuario actual + Cerrar sesión al final
- Hamburguesa en móvil para colapsar

## Controladores y Vistas

### EventosController
- `GET Index` — lista con filtros (estado, fecha, búsqueda)
- `GET Details/{id}` — detalle completo
- `GET/POST Create` — formulario con selects
- `GET/POST Edit/{id}` — edición
- `POST Delete/{id}` — cambiar estado a Cancelado

### ClientesController
- `GET Index` — lista con búsqueda
- `GET/POST Create` — formulario
- `GET/POST Edit/{id}` — edición

### HomeController (modificado)
- `GET Index` — dashboard con métricas: total eventos, por estado, próximos, clientes activos

## Vistas

```
Views/Shared/_Layout.cshtml       — Sidebar + content
Views/Eventos/Index.cshtml        — Tabla responsive
Views/Eventos/Details.cshtml      — Detalle con info completa
Views/Eventos/Create.cshtml       — Formulario
Views/Eventos/Edit.cshtml         — Formulario (reutiliza Create)
Views/Clientes/Index.cshtml       — Tabla responsive
Views/Clientes/Create.cshtml      — Formulario
Views/Clientes/Edit.cshtml        — Formulario
Views/Home/Index.cshtml           — Dashboard con métricas
```

## Anti-patterns a evitar
- Emojis como íconos (usar SVGs de Lucide)
- Sin `cursor:pointer` en elementos clickeables
- Sin focus states visibles
- Sin validación en formularios
