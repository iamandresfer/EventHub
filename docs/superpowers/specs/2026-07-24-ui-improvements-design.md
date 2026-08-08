---
tipo: spec
proyecto: EventHub
modulo: UI
fecha: 2026-07-24
estado: En curso
---

# EventHub UI Improvements - Design Spec

**Date:** 2026-07-24  
**Status:** Approved  
**Scope:** 10 features across email notifications, task cards, sidebar navigation, table sorting, event filtering, dashboard redesign, event details, and budget management

---

## Feature 1: "Ver Tarea" Button in Notification Emails

### Problem
Notification emails contain task info but no way to navigate directly to the task.

### Solution
Add a styled CTA button in the `ConstruirBodyEmail` method that links to the Kanban board for the event.

### Changes

**`EventHub.02.Bussines/Services/NotificacionService.cs`**
- Read `AppBaseUrl` from `ConfigurationManager.AppSettings["AppBaseUrl"]` in constructor (same pattern as `EmailService`)
- Modify `ConstruirBodyEmail` signature to accept `eventoId` parameter
- Add an HTML button/link: `<a href="{appBaseUrl}/Eventos/Tareas/{eventoId}">Ver Tarea →</a>`
- The `eventoId` is already passed to `CrearYEnviar` — just thread it to `ConstruirBodyEmail`

**No controller changes needed** — `eventoId` is already passed in all 3 notification call sites.

---

## Feature 2: Fix "Invalid Date" on Task Cards

### Problem
ASP.NET MVC 5 `JsonResult` serializes dates as `/Date(milliseconds)/`. The JS does `.substring(0,10)` which produces `/Date(1234` — not a valid date.

### Solution
Add `parseServerDate()` helper in `Tareas.cshtml` that handles the `/Date(ms)/` format and returns `yyyy-MM-dd`.

### Changes
**`EventHub.01.Web/Views/Eventos/Tareas.cshtml`**
- Add `parseServerDate()` function
- Replace `t.FechaLimite.substring(0,10)` with `parseServerDate(t.FechaLimite)`

---

## Feature 3: Crew Section in Sidebar

### Problem
Crew is only accessible per-event. User wants a global view.

### Solution
Add "Crew" link in sidebar below "Eventos" + new page listing all operators.

### Changes

**`EventHub.01.Web/Views/Shared/_Layout.cshtml`**
- Add nav item after "Eventos" with Crew link

**`EventHub.01.Web/Controllers/CrewController.cs`**
- Add parameterless `Index()` action using `_operadorService.GetActivos()`

**`EventHub.01.Web/Views/Crew/IndexGlobal.cshtml`** (new)
- Card grid of all operators with search filter

---

## Feature 4: Column Sorting on Tables

### Problem
No way to sort tables by columns.

### Solution
Client-side JavaScript sorting on Events and Clients tables. Click header → ascending → descending → reset.

### Behavior
- Strings: alphabetical (accent-aware for Spanish)
- Dates: chronological
- Numbers/Currency: numeric value
- Status: custom order (Planificación → Pre-Producción → Ejecución → Finalizado → Cancelado)
- Visual: ▲/▼ indicators on active sort column

### Changes
**`EventHub.01.Web/Views/Eventos/Index.cshtml`** — sortable headers + JS
**`EventHub.01.Web/Views/Clientes/Index.cshtml`** — sortable headers + JS

---

## Feature 5: Separate "Finished Events" Table

### Problem
Finished events clutter the main list.

### Solution
Split into two sections: active events table + collapsible finished events section below.

### Changes
**`EventHub.01.Web/Views/Eventos/Index.cshtml`**
- Split Model into active (Planificación, Pre-Producción, Ejecución) and finished (Finalizado, Cancelado)
- Collapsible "Eventos Finalizados (N)" section with toggle
- Both tables get column sorting

---

## Feature 6: Visual Dashboard with Vista Cards

### Problem
Current dashboard is basic — just 6 stat cards + a plain table. Needs a modern, visual design.

### Solution
Redesign dashboard with card-based event display (inspired by buenplantickets/meet2go but focused on event production efficiency, not ticketing).

### Design: "Vista Cards" Style
- **Stats row:** 4 cards with icons, values, and contextual info (e.g., "↑ 3 este mes", "8 urgentes", "5 VIP")
- **Events section:** Toggle between Table view and Cards view
- **Card view:** Event cards showing cover photo gradient, status badge, date, client, venue, budget, and a mini progress bar
- **No event code column** — hide Código from display (keep in data but don't show)

### Changes

**`EventHub.01.Web/Views/Home/Index.cshtml`** — Complete redesign:
- Modern stat cards with icons, gradients, and sub-info
- Table/Cards view toggle (localStorage persistence)
- Cards: cover photo gradient, status badge, date, name, client+venue, budget, progress bar
- Remove "Código" column from both views

**`EventHub.02.Bussines/DTOs/DashboardDto.cs`**
- Add `EventosProximos` count and any additional stats if needed

---

## Feature 7: Improved Event Details Dashboard

### Problem
Current event details page is minimal — banner + 4 basic cards. Needs more functionality.

### Solution
"Dashboard Completo" style with banner, quick stats grid, and detailed information cards.

### Design
- **Banner:** Event name, status badge, client/venue/dates, action buttons (Tareas, Crew, Editar)
- **Quick stats row:** 4 mini cards (Tareas Totales, Completadas, Pendientes, Operadores)
- **Content grid (2 columns):**
  - Left: Budget card (estimated vs spent, progress bar, remaining), Client card (name, email, phone)
  - Right: Recent tasks list (with status icons), Crew avatars preview

### Changes

**`EventHub.01.Web/Views\Eventos/Details.cshtml`** — Complete redesign:
- Banner with gradient background (existing, enhanced)
- Quick stats row with task/crew counts
- 2-column grid: Budget + Client on left, Tasks + Crew on right
- Budget card shows estimated vs spent with progress bar
- "Ver detalle" link to budget section

**`EventHub.01.Web/Controllers/EventosController.cs`**
- `Details` action: Load additional data (task counts, crew count, crew list)

**`EventHub.02.Bussines/DTOs/EventoListDto.cs`**
- Add fields for task counts, crew count if not present

---

## Feature 8: Hide Event Code Column

### Problem
Event code (Código) is internal, not useful for daily display.

### Solution
Remove from all table displays. Keep in database and entity, just don't render in tables.

### Changes
- `Home/Index.cshtml` — remove Código column
- `Eventos/Index.cshtml` — remove Código column
- `Eventos/Details.cshtml` — remove from banner meta (keep internal)
- `Clientes/Details.cshtml` — remove Código column if present

---

## Feature 9: Budget Management Section

### Problem
No way to track individual expenses per event. Only `PresupuestoEstimado` and `GastoReal` exist on the Evento entity.

### Solution
Create a new `tbl_gastos` table with full CRUD, categories, and automatic balance calculation.

### Database Design

**New Entity: `Gasto` (EventHub.03.Data.Entities)**
```csharp
[Table("tbl_gastos")]
public class Gasto
{
    [Key] [Column("gas_id")]
    public int Id { get; set; }

    [Required] [Column("gas_eve_id")]
    public int EventoId { get; set; }

    [Required] [Column("gas_categoria")]
    [MaxLength(50)]
    public string Categoria { get; set; } // Sonido, Iluminación, Catering, Logística, Decoración, Personal, Otro

    [Required] [Column("gas descripcion")]
    [MaxLength(200)]
    public string Descripcion { get; set; }

    [Required] [Column("gas_monto")]
    public decimal Monto { get; set; }

    [Column("gas_proveedor")]
    [MaxLength(150)]
    public string Proveedor { get; set; }

    [Column("gas_fecha")]
    public DateTime Fecha { get; set; }

    [Column("gas_notas")]
    public string Notas { get; set; }

    [Required] [Column("gas_fecha_creacion")]
    public DateTime FechaCreacion { get; set; }

    [ForeignKey("EventoId")]
    public virtual Evento Evento { get; set; }
}
```

**Predefined Categories:**
- Sonido 🎵
- Iluminación 💡
- Catering 🍽️
- Logística 🚚
- Decoración 🎨
- Personal 👷
- Otro 📦

### UI Design
- **Page:** `/Presupuesto/Index` with event selector dropdown
- **Summary cards:** Estimated / Spent / Available
- **Progress bar:** Visual budget execution percentage
- **Category grid:** 4-column grid showing spending per category
- **Expense list:** Table with category icon, description, provider, date, amount, edit/delete buttons
- **Add expense:** Modal form with category, description, amount, provider, date, notes

### Changes

**New files:**
- `EventHub.03.Data/Entities/Gasto.cs`
- `EventHub.02.Bussines/Services/IGastoService.cs`
- `EventHub.02.Bussines/Services/GastoService.cs`
- `EventHub.02.Bussines/DTOs/GastoDto.cs`
- `EventHub.02.Bussines/DTOs/GastoFormDto.cs`
- `EventHub.01.Web/Controllers/PresupuestoController.cs`
- `EventHub.01.Web/Views/Presupuesto/Index.cshtml`

**Modified files:**
- `EventHub.03.Data/EventHubContext.cs` — add `DbSet<Gasto>`
- `EventHub.01.Web/Views/Shared/_Layout.cshtml` — add "Presupuesto" nav link in sidebar
- `EventHub.01.Web/App_Start/RouteConfig.cs` — add route if needed

**SQL Script:**
- Create `tbl_gastos` table script

### Sidebar Navigation Order
1. Dashboard
2. Eventos
3. Crew
4. Clientes
5. **Presupuesto** ← new

---

## Feature 10: Preserve VS Debugging Configuration

### Constraint
The project runs from Visual Studio. No changes to:
- `Web.config` connection strings (unless adding a new context)
- `EventHub.01.Web.csproj` project structure (unless adding new files)
- Authentication/authorization configuration
- Bundle configuration
- Route configuration (unless adding new routes)

### Approach
- All new views go in existing View folders
- New controllers follow existing pattern
- New entities added to existing `EventHubContext`
- SQL scripts provided separately for manual execution
- No npm/webpack/build tool changes

---

## Dependencies Between Features

- Feature #4 (sorting) before #5 (finished events table)
- Feature #6 (dashboard redesign) before #7 (event details) — shared visual language
- Feature #8 (hide code) is part of #6
- Feature #9 (budget) is independent but benefits from #7 (event details shows budget card)
- Feature #10 (VS compat) applies to all features

## Implementation Order

1. Feature 2: Fix "Invalid Date" (quick bugfix)
2. Feature 1: Email "Ver Tarea" button
3. Feature 3: Crew sidebar + global page
4. Feature 4: Column sorting
5. Feature 5: Finished events table
6. Feature 8: Hide event code column
7. Feature 6: Visual dashboard with Vista Cards
8. Feature 7: Improved event details dashboard
9. Feature 9: Budget management section
