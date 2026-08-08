---
tipo: plan
proyecto: EventHub
modulo: UI
fecha: 2026-07-24
estado: En curso
---

# EventHub UI Improvements Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement 10 UI improvements: email button, date fix, crew sidebar, column sorting, finished events, hide code, visual dashboard, event details, budget management, and VS debugging preservation.

**Architecture:** ASP.NET MVC 5 with Razor views, jQuery-based AJAX, Entity Framework 6 Database-First. All custom CSS inline in `_Layout.cshtml`. No SPA framework — vanilla JS with `fetch()`.

**Tech Stack:** .NET Framework 4.8.1, ASP.NET MVC 5.2.9, jQuery 3.7.0, Bootstrap 5.2.3, Entity Framework 6.5.1, SQL Server 2019+

## Global Constraints

- .NET Framework 4.8.1 (NOT .NET Core)
- ASP.NET MVC 5 with Razor views
- All custom CSS is inline in `_Layout.cshtml` `<style>` blocks
- All custom JS is inline in views via `@section scripts {}`
- No npm/webpack/build tools — pure browser JS
- Forms Authentication with cookie-based auth
- CSRF token required on all POST requests via `@Html.AntiForgeryToken()`
- AJAX pattern: `fetch()` with `FormData` + CSRF token, returns JSON `{ success: bool, message: string }`
- Do NOT modify Web.config connection strings, bundle config, or route config unless adding new routes
- New files must be added to the .csproj if needed (VS requires this)

---

## Feature 2: Fix "Invalid Date" on Task Cards

### Task 1: Add parseServerDate helper and fix date display

**Files:**
- Modify: `EventHub.01.Web/Views/Eventos/Tareas.cshtml:467-497`

**Interfaces:**
- Consumes: `data.tarea.FechaLimite` from AJAX response (ASP.NET `/Date(ms)/` format)
- Produces: Correctly formatted date string for display

- [ ] **Step 1: Add parseServerDate helper function**

In `Tareas.cshtml`, inside the `@section scripts` block, add this function before `formatDateArg`:

```javascript
function parseServerDate(dateStr) {
    if (!dateStr) return '';
    var match = dateStr.match(/\/Date\((\d+)\)\//);
    if (match) {
        var d = new Date(parseInt(match[1]));
        var day = String(d.getDate()).padStart(2, '0');
        var month = String(d.getMonth() + 1).padStart(2, '0');
        var year = d.getFullYear();
        return year + '-' + month + '-' + day;
    }
    return dateStr.substring(0, 10);
}
```

- [ ] **Step 2: Fix the date assignment in guardarTarea success handler**

Change line 497 from:
```javascript
FechaLimite: t.FechaLimite ? t.FechaLimite.substring(0,10) : '',
```
to:
```javascript
FechaLimite: parseServerDate(t.FechaLimite),
```

- [ ] **Step 3: Verify the fix**

After creating a new task with a date via the modal, confirm the task card shows the correct date (dd/MM/yyyy format) with no "Invalid Date".

- [ ] **Step 4: Commit**

```bash
git add EventHub.01.Web/Views/Eventos/Tareas.cshtml
git commit -m "fix: resolve Invalid Date on newly created task cards"
```

---

## Feature 1: "Ver Tarea" Button in Notification Emails

### Task 2: Add event URL to notification email body

**Files:**
- Modify: `EventHub.02.Bussines/Services/NotificacionService.cs`

**Interfaces:**
- Consumes: `_appBaseUrl` from config, `eventoId` from `CrearYEnviar` parameters
- Produces: Updated HTML email body with "Ver Tarea" button

- [ ] **Step 1: Add AppBaseUrl field to NotificacionService**

Add a private field and read it in the constructor:

```csharp
private readonly string _appBaseUrl;

public NotificacionService()
{
    _context = new EventHubContext();
    _appBaseUrl = System.Configuration.ConfigurationManager.AppSettings["AppBaseUrl"] ?? "https://localhost:44353";
}
```

- [ ] **Step 2: Update ConstruirBodyEmail to accept eventoId and render button**

Replace the `ConstruirBodyEmail` method with:

```csharp
private string ConstruirBodyEmail(string tipo, string nombreDestino, string mensaje,
    string nombreEvento, string tareaTitulo, int? eventoId)
{
    var icono = tipo switch
    {
        "TareaCreada" => "📋",
        "TareaCompletada" => "✅",
        "TareaVencida" => "⚠️",
        "FechaModificada" => "📅",
        _ => "📌"
    };

    var color = tipo switch
    {
        "TareaCreada" => "#4361ee",
        "TareaCompletada" => "#10b981",
        "TareaVencida" => "#ef4444",
        "FechaModificada" => "#f59e0b",
        _ => "#6c757d"
    };

    var botonVerTarea = eventoId.HasValue
        ? $"<p style='text-align:center; margin:24px 0;'><a href='{_appBaseUrl}/Eventos/Tareas/{eventoId.Value}' style='display:inline-block; padding:12px 24px; background:{color}; color:white; text-decoration:none; border-radius:8px; font-weight:600;'>Ver Tarea →</a></p>"
        : "";

    return $@"
        <div style='font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, sans-serif; max-width: 500px; margin: 0 auto; padding: 32px;'>
            <div style='text-align: center; margin-bottom: 24px;'>
                <span style='font-size: 48px;'>{icono}</span>
            </div>
            <h2 style='color: #1a1a2e; text-align: center;'>Hola {nombreDestino}</h2>
            <div style='background: {color}10; border-left: 4px solid {color}; padding: 16px; border-radius: 0 8px 8px 0; margin: 20px 0;'>
                <p style='color: #374151; font-size: 15px; margin: 0;'>{mensaje}</p>
            </div>
            {(nombreEvento != null ? $"<p style='color: #6c757d; font-size: 14px;'><strong>Evento:</strong> {nombreEvento}</p>" : "")}
            {(tareaTitulo != null ? $"<p style='color: #6c757d; font-size: 14px;'><strong>Tarea:</strong> {tareaTitulo}</p>" : "")}
            {botonVerTarea}
            <hr style='border: none; border-top: 1px solid #dee2e6; margin: 24px 0;'>
            <p style='color: #adb5bd; font-size: 12px; text-align: center;'>EventProduction Hub - Sistema Integral de Gestión de Eventos</p>
        </div>";
}
```

- [ ] **Step 3: Update CrearYEnviar to pass eventoId to ConstruirBodyEmail**

Update the call inside `CrearYEnviar` method (around line 128):

```csharp
var body = ConstruirBodyEmail(tipo, nombreDestino, mensaje, nombreEvento, tareaTitulo, eventoId);
```

- [ ] **Step 4: Verify email sends correctly**

Create a task with a user assigned. Check the notification email contains a "Ver Tarea →" button linking to `/Eventos/Tareas/{eventoId}`.

- [ ] **Step 5: Commit**

```bash
git add EventHub.02.Bussines/Services/NotificacionService.cs
git commit -m "feat: add 'Ver Tarea' button to notification emails"
```

---

## Feature 3: Crew Section in Sidebar

### Task 3: Add Crew link to sidebar navigation

**Files:**
- Modify: `EventHub.01.Web/Views/Shared/_Layout.cshtml` (after Eventos nav item)

- [ ] **Step 1: Add Crew nav item after Eventos**

In `_Layout.cshtml`, after the Eventos `</a>` tag, add:

```html
<a href="@Url.Action("Index", "Crew")" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "Crew" ? "active" : "")" data-tooltip="Crew">
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
    <span class="nav-label">Crew</span>
</a>
```

- [ ] **Step 2: Verify sidebar shows Crew link**

Refresh any page. Confirm Crew link appears below Eventos with correct icon and active state.

- [ ] **Step 3: Commit**

```bash
git add EventHub.01.Web/Views/Shared/_Layout.cshtml
git commit -m "feat: add Crew navigation link to sidebar"
```

### Task 4: Add global Crew Index action and view

**Files:**
- Modify: `EventHub.01.Web/Controllers/CrewController.cs`
- Create: `EventHub.01.Web/Views/Crew/IndexGlobal.cshtml`

- [ ] **Step 1: Add Index action (parameterless) to CrewController**

Add this method to `CrewController.cs`:

```csharp
[HttpGet]
public ActionResult Index()
{
    var operadores = _operadorService.GetActivos();
    return View("IndexGlobal", operadores);
}
```

- [ ] **Step 2: Create IndexGlobal.cshtml view**

Create `EventHub.01.Web/Views/Crew/IndexGlobal.cshtml` with:
- Model: `IEnumerable<EventHub._02.Bussines.DTOs.OperadorDto>`
- Page header with back button
- Search input
- Card grid using existing `.crew-grid` and `.crew-card` styles
- Each card: avatar, name, role, email, phone, cedula, status
- Empty state message
- JS for search filtering

- [ ] **Step 3: Verify the page loads**

Navigate to `/Crew`. Confirm all active operators display in card grid with search working.

- [ ] **Step 4: Commit**

```bash
git add EventHub.01.Web/Controllers/CrewController.cs EventHub.01.Web/Views/Crew/IndexGlobal.cshtml
git commit -m "feat: add global Crew index page with all operators"
```

---

## Feature 4: Column Sorting on Tables

### Task 5: Add column sorting to Events table

**Files:**
- Modify: `EventHub.01.Web/Views/Eventos/Index.cshtml`

- [ ] **Step 1: Add sortable class to th elements**

Replace the `<thead>` section:

```html
<thead>
    <tr>
        <th class="sortable" data-col="0" data-type="string">Evento</th>
        <th class="sortable" data-col="1" data-type="string">Cliente</th>
        <th class="sortable" data-col="2" data-type="string">Lugar</th>
        <th class="sortable" data-col="3" data-type="date">Fecha</th>
        <th class="sortable" data-col="4" data-type="status">Estado</th>
        <th class="sortable" data-col="5" data-type="currency">Presupuesto</th>
        <th style="text-align:right;">Acciones</th>
    </tr>
</thead>
```

Note: Código column is removed (Feature #8).

- [ ] **Step 2: Add sortable CSS to the style block**

```css
th.sortable {
    cursor: pointer;
    user-select: none;
    position: relative;
    padding-right: 20px !important;
}
th.sortable:hover {
    background: var(--primary-light);
}
th.sortable::after {
    content: '';
    position: absolute;
    right: 6px;
    top: 50%;
    transform: translateY(-50%);
    font-size: 10px;
    color: var(--text-muted);
}
th.sort-asc::after { content: '▲'; color: var(--primary); }
th.sort-desc::after { content: '▼'; color: var(--primary); }
```

- [ ] **Step 3: Add sorting JavaScript**

Add before existing event-row click handler:

```javascript
(function() {
    var statusOrder = { 'Planificacion': 0, 'PreProduccion': 1, 'Ejecucion': 2, 'Finalizado': 3, 'Cancelado': 4 };
    var currentSort = { col: -1, dir: 'none' };

    document.querySelectorAll('th.sortable').forEach(function(th) {
        th.addEventListener('click', function() {
            var col = parseInt(th.dataset.col);
            var type = th.dataset.type;
            var tbody = th.closest('table').querySelector('tbody');
            var rows = Array.from(tbody.querySelectorAll('tr:not(:has(td[colspan]))'));

            var dir = 'asc';
            if (currentSort.col === col && currentSort.dir === 'asc') dir = 'desc';
            else if (currentSort.col === col && currentSort.dir === 'desc') dir = 'none';

            document.querySelectorAll('th.sortable').forEach(function(h) {
                h.classList.remove('sort-asc', 'sort-desc');
            });

            if (dir === 'none') {
                currentSort = { col: -1, dir: 'none' };
                return;
            }

            th.classList.add(dir === 'asc' ? 'sort-asc' : 'sort-desc');
            currentSort = { col: col, dir: dir };

            rows.sort(function(a, b) {
                var aVal = a.cells[col].textContent.trim();
                var bVal = b.cells[col].textContent.trim();
                var cmp = 0;

                if (type === 'date') {
                    var aDate = parseEventDate(aVal);
                    var bDate = parseEventDate(bVal);
                    cmp = aDate - bDate;
                } else if (type === 'currency') {
                    var aNum = parseFloat(aVal.replace(/[^0-9.,]/g, '').replace(',', '.')) || 0;
                    var bNum = parseFloat(bVal.replace(/[^0-9.,]/g, '').replace(',', '.')) || 0;
                    cmp = aNum - bNum;
                } else if (type === 'status') {
                    cmp = (statusOrder[aVal] || 0) - (statusOrder[bVal] || 0);
                } else {
                    cmp = aVal.localeCompare(bVal, 'es');
                }

                return dir === 'asc' ? cmp : -cmp;
            });

            rows.forEach(function(row) { tbody.appendChild(row); });
        });
    });

    function parseEventDate(str) {
        var parts = str.split('/');
        if (parts.length === 3) return new Date(parts[2], parts[1] - 1, parts[0]);
        return new Date(str);
    }
})();
```

- [ ] **Step 4: Verify sorting works**

Click each column header on Events page. Verify ascending/descending/reset behavior with indicators.

- [ ] **Step 5: Commit**

```bash
git add EventHub.01.Web/Views/Eventos/Index.cshtml
git commit -m "feat: add column sorting to events table"
```

### Task 6: Add column sorting to Clients table

**Files:**
- Modify: `EventHub.01.Web/Views/Clientes/Index.cshtml`

- [ ] **Step 1: Add sortable class to th elements**

```html
<thead>
    <tr>
        <th class="sortable" data-col="0" data-type="string">Nombre</th>
        <th class="sortable" data-col="1" data-type="string">RUC</th>
        <th class="sortable" data-col="2" data-type="string">Email</th>
        <th class="sortable" data-col="3" data-type="string">Contacto</th>
        <th class="sortable" data-col="4" data-type="string">Clasificación</th>
        <th>Estado</th>
        <th style="text-align:right;">Acciones</th>
    </tr>
</thead>
```

- [ ] **Step 2: Add sortable CSS and sorting JS**

Same CSS as Events table. Simpler JS (string comparison only, no date/currency/status).

- [ ] **Step 3: Verify sorting works**

Click each column header on Clients page. Verify sorting.

- [ ] **Step 4: Commit**

```bash
git add EventHub.01.Web/Views/Clientes/Index.cshtml
git commit -m "feat: add column sorting to clients table"
```

---

## Feature 5: Separate "Finished Events" Table

### Task 7: Split events into active and finished tables

**Files:**
- Modify: `EventHub.01.Web/Views/Eventos/Index.cshtml`

- [ ] **Step 1: Split the model at the top of the view**

After `@{ ViewBag.Title = "Eventos"; }`, add:

```html
@{
    var activeEvents = Model.Where(e => e.Estado != "Finalizado" && e.Estado != "Cancelado").ToList();
    var finishedEvents = Model.Where(e => e.Estado == "Finalizado" || e.Estado == "Cancelado").ToList();
}
```

- [ ] **Step 2: Replace single table with two sections**

Main table renders `activeEvents`. Below it, add collapsible section:

```html
@if (finishedEvents.Any())
{
    <div style="margin-top:24px;">
        <button type="button" id="toggleFinished" style="background:none;border:none;cursor:pointer;display:flex;align-items:center;gap:8px;font-size:15px;font-weight:600;color:var(--text-muted);padding:8px 0;font-family:inherit;">
            <svg id="toggleArrow" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="transition:transform 0.2s;"><path d="M6 9l6 6 6-6"/></svg>
            Eventos Finalizados (@finishedEvents.Count)
        </button>
        <div id="finishedSection" style="display:none;">
            <div class="card" style="padding:0;overflow:hidden;margin-top:8px;">
                <table class="table">
                    <!-- Same headers as main table -->
                    <tbody>
                        @foreach (var item in finishedEvents)
                        {
                            <tr class="event-row" data-href="@Url.Action("Details", new { id = item.Id })" style="cursor:pointer;">
                                <!-- Same columns as main table but Estado is read-only (no dropdown) -->
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
}
```

- [ ] **Step 3: Add toggle behavior**

```javascript
var toggleBtn = document.getElementById('toggleFinished');
if (toggleBtn) {
    toggleBtn.addEventListener('click', function() {
        var section = document.getElementById('finishedSection');
        var arrow = document.getElementById('toggleArrow');
        if (section.style.display === 'none') {
            section.style.display = 'block';
            arrow.style.transform = 'rotate(180deg)';
        } else {
            section.style.display = 'none';
            arrow.style.transform = 'rotate(0deg)';
        }
    });
}
```

- [ ] **Step 4: Verify the split**

Active events in main table, finished in collapsible section. Sorting works on both.

- [ ] **Step 5: Commit**

```bash
git add EventHub.01.Web/Views/Eventos/Index.cshtml
git commit -m "feat: separate active and finished events into two tables"
```

---

## Feature 8: Hide Event Code Column

### Task 8: Remove Código from all table displays

**Files:**
- Modify: `EventHub.01.Web/Views/Home/Index.cshtml`
- Modify: `EventHub.01.Web/Views/Eventos/Index.cshtml`
- Modify: `EventHub.01.Web/Views/Clientes/Details.cshtml`

- [ ] **Step 1: Remove from Dashboard table**

In `Home/Index.cshtml`, remove the `<th>Código</th>` header and the `<td>@e.Codigo</td>` cell from each row.

- [ ] **Step 2: Remove from Events table**

Already done in Task 5 (headers don't include Código). Verify the `<td>` cells are also removed.

- [ ] **Step 3: Remove from Client Details table**

In `Clientes/Details.cshtml`, remove Código column if present.

- [ ] **Step 4: Commit**

```bash
git add EventHub.01.Web/Views/Home/Index.cshtml EventHub.01.Web/Views/Eventos/Index.cshtml EventHub.01.Web/Views/Clientes/Details.cshtml
git commit -m "feat: hide event code column from all table displays"
```

---

## Feature 6: Visual Dashboard with Vista Cards

### Task 9: Redesign Dashboard with Vista Cards style

**Files:**
- Modify: `EventHub.01.Web/Views/Home/Index.cshtml`
- Modify: `EventHub.02.Bussines/DTOs/DashboardDto.cs` (if needed)

**Interfaces:**
- Consumes: `DashboardDto` (stats + ProximosEventos list)
- Produces: Visual dashboard with stat cards + table/cards toggle

- [ ] **Step 1: Redesign stat cards section**

Replace the existing `<div class="stats-grid">` with 4 modern stat cards:

```html
<div style="display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 24px;">
    <div style="background: white; border: 1px solid var(--border); border-radius: 12px; padding: 20px;">
        <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 12px;">
            <div style="width: 40px; height: 40px; background: #eff6ff; border-radius: 10px; display: flex; align-items: center; justify-content: center;">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#3b82f6" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
            </div>
            <div>
                <div style="font-size: 12px; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.5px;">Eventos Activos</div>
                <div style="font-size: 28px; font-weight: 700; color: var(--text);">@Model.EventosActivos</div>
            </div>
        </div>
        <div style="font-size: 12px; color: #10b981;">@Model.EventosEjecucion en ejecución</div>
    </div>
    <!-- Repeat pattern for Presupuesto, Tareas, Clientes -->
</div>
```

- [ ] **Step 2: Add table/cards view toggle**

Replace the "Próximos Eventos" section with:

```html
<div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px;">
    <h3 style="font-size:18px;font-weight:700;margin:0;">Próximos Eventos</h3>
    <div style="display: flex; gap: 4px;">
        <button type="button" id="viewTable" class="view-toggle active" onclick="switchView('table')">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="8" y1="6" x2="21" y2="6"/><line x1="8" y1="12" x2="21" y2="12"/><line x1="8" y1="18" x2="21" y2="18"/><line x1="3" y1="6" x2="3.01" y2="6"/><line x1="3" y1="12" x2="3.01" y2="12"/><line x1="3" y1="18" x2="3.01" y2="18"/></svg>
            Tabla
        </button>
        <button type="button" id="viewCards" class="view-toggle" onclick="switchView('cards')">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/></svg>
            Cards
        </button>
    </div>
</div>
```

- [ ] **Step 3: Add table view (no Código column)**

```html
<div id="tableView">
    <div class="card" style="padding:0;overflow:hidden;">
        <table class="table">
            <thead>
                <tr>
                    <th class="sortable" data-col="0" data-type="string">Evento</th>
                    <th class="sortable" data-col="1" data-type="string">Cliente</th>
                    <th class="sortable" data-col="2" data-type="string">Lugar</th>
                    <th class="sortable" data-col="3" data-type="date">Fecha</th>
                    <th class="sortable" data-col="4" data-type="status">Estado</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var e in Model.ProximosEventos)
                {
                    <tr style="cursor:pointer;" onclick="window.location='@Url.Action("Details", "Eventos", new { id = e.Id })'">
                        <td style="font-weight:600;">@e.Nombre</td>
                        <td>@e.ClienteNombre</td>
                        <td>@e.VenueNombre</td>
                        <td>@e.FechaInicio.ToString("dd/MM/yyyy")</td>
                        <td><span class="badge badge-@e.Estado.ToLower()">@e.Estado</span></td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>
```

- [ ] **Step 4: Add cards view**

```html
<div id="cardsView" style="display:none;">
    <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px;">
        @foreach (var e in Model.ProximosEventos)
        {
            var gradient = e.Estado switch {
                "Planificacion" => "linear-gradient(135deg, #f59e0b, #d97706)",
                "PreProduccion" => "linear-gradient(135deg, #8b5cf6, #7c3aed)",
                "Ejecucion" => "linear-gradient(135deg, #3b82f6, #1d4ed8)",
                _ => "linear-gradient(135deg, #6b7280, #4b5563)"
            };
            <div style="background: var(--bg-card); border: 1px solid var(--border); border-radius: 12px; overflow: hidden; cursor: pointer;" onclick="window.location='@Url.Action("Details", "Eventos", new { id = e.Id })'">
                <div style="height: 120px; background: @gradient; position: relative;">
                    <div style="position: absolute; top: 8px; right: 8px; background: rgba(255,255,255,0.9); padding: 2px 8px; border-radius: 10px; font-size: 11px; font-weight: 600;">@e.Estado</div>
                    <div style="position: absolute; bottom: 8px; left: 8px; color: white; font-size: 11px; opacity: 0.9;">📅 @e.FechaInicio.ToString("dd/MM/yyyy")</div>
                </div>
                <div style="padding: 14px;">
                    <div style="font-weight: 700; font-size: 14px; margin-bottom: 4px;">@e.Nombre</div>
                    <div style="font-size: 12px; color: var(--text-muted); margin-bottom: 8px;">@e.ClienteNombre • @e.VenueNombre</div>
                    <div style="font-size: 12px; color: #10b981; font-weight: 600;">@e.PresupuestoEstimado.ToString("C2")</div>
                </div>
            </div>
        }
    </div>
</div>
```

- [ ] **Step 5: Add view toggle CSS and JS**

```css
.view-toggle {
    display: inline-flex;
    align-items: center;
    gap: 4px;
    padding: 6px 12px;
    border: 1px solid var(--border);
    border-radius: 6px;
    background: var(--bg-card);
    color: var(--text-muted);
    font-size: 13px;
    cursor: pointer;
    font-family: inherit;
}
.view-toggle.active {
    background: var(--primary);
    color: white;
    border-color: var(--primary);
}
```

```javascript
function switchView(view) {
    document.getElementById('tableView').style.display = view === 'table' ? '' : 'none';
    document.getElementById('cardsView').style.display = view === 'cards' ? '' : 'none';
    document.getElementById('viewTable').classList.toggle('active', view === 'table');
    document.getElementById('viewCards').classList.toggle('active', view === 'cards');
    localStorage.setItem('dashboard-view', view);
}

// Restore saved view
var savedView = localStorage.getItem('dashboard-view') || 'table';
switchView(savedView);
```

- [ ] **Step 6: Add sorting JS for dashboard table**

Same sorting code as Events table (Task 5).

- [ ] **Step 7: Verify the dashboard**

Confirm:
- 4 stat cards with icons and values
- Table view shows events without Código
- Cards view shows event cards with gradient, status, date
- Toggle persists across page reloads (localStorage)
- Sorting works on table view

- [ ] **Step 8: Commit**

```bash
git add EventHub.01.Web/Views/Home/Index.cshtml
git commit -m "feat: redesign dashboard with Vista Cards style and table/cards toggle"
```

---

## Feature 7: Improved Event Details Dashboard

### Task 10: Redesign Event Details page

**Files:**
- Modify: `EventHub.01.Web/Views Eventos/Details.cshtml`
- Modify: `EventHub.01.Web/Controllers/EventosController.cs`
- Modify: `EventHub.02.Bussines/DTOs/EventoListDto.cs` (if needed)

- [ ] **Step 1: Add data loading in EventosController.Details**

Update the `Details` action to load additional data:

```csharp
public async Task<ActionResult> Details(int id)
{
    var evento = await _eventoService.GetByIdAsync(id);
    if (evento == null) return HttpNotFound();

    var context = new EventHubContext();
    
    // Load task counts
    var tareas = context.Tareas.Where(t => t.EventoId == id).ToList();
    ViewBag.TareasTotales = tareas.Count;
    ViewBag.TareasCompletadas = tareas.Count(t => t.Estado == "Completado");
    ViewBag.TareasPendientes = tareas.Count(t => t.Estado != "Completado");
    
    // Load crew count and list
    var crewService = new CrewService();
    var crew = crewService.ObtenerCrewPorEvento(id);
    ViewBag.CrewCount = crew.Count;
    ViewBag.CrewList = crew.Take(5).ToList();

    return View(evento);
}
```

- [ ] **Step 2: Redesign the banner section**

Keep existing banner with gradient, but remove the Codigo from meta (keep it internally). Add action buttons.

- [ ] **Step 3: Add quick stats row below banner**

```html
<div style="display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; padding: 16px 32px; background: var(--bg-secondary); border-bottom: 1px solid var(--border);">
    <div style="text-align: center;">
        <div style="font-size: 24px; font-weight: 700; color: var(--text);">@ViewBag.TareasTotales</div>
        <div style="font-size: 12px; color: var(--text-muted);">Tareas Totales</div>
    </div>
    <div style="text-align: center;">
        <div style="font-size: 24px; font-weight: 700; color: #10b981;">@ViewBag.TareasCompletadas</div>
        <div style="font-size: 12px; color: var(--text-muted);">Completadas</div>
    </div>
    <div style="text-align: center;">
        <div style="font-size: 24px; font-weight: 700; color: #f59e0b;">@ViewBag.TareasPendientes</div>
        <div style="font-size: 12px; color: var(--text-muted);">Pendientes</div>
    </div>
    <div style="text-align: center;">
        <div style="font-size: 24px; font-weight: 700; color: #8b5cf6;">@ViewBag.CrewCount</div>
        <div style="font-size: 12px; color: var(--text-muted);">Operadores</div>
    </div>
</div>
```

- [ ] **Step 4: Add 2-column content grid**

Left column: Budget card + Client card
Right column: Recent tasks + Crew avatars

- [ ] **Step 5: Verify the page**

Confirm banner, stats row, and 2-column grid render correctly with real data.

- [ ] **Step 6: Commit**

```bash
git add EventHub.01.Web/Views/Eventos/Details.cshtml EventHub.01.Web/Controllers/EventosController.cs
git commit -m "feat: redesign event details with dashboard completo style"
```

---

## Feature 9: Budget Management Section

### Task 11: Create Gasto entity and database table

**Files:**
- Create: `EventHub.03.Data/Entities/Gasto.cs`
- Create: SQL script for tbl_gastos

- [ ] **Step 1: Create Gasto entity**

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_gastos")]
    public class Gasto
    {
        [Key]
        [Column("gas_id")]
        public int Id { get; set; }

        [Required]
        [Column("gas_eve_id")]
        public int EventoId { get; set; }

        [Required]
        [Column("gas_categoria")]
        [MaxLength(50)]
        public string Categoria { get; set; }

        [Required]
        [Column("gas_descripcion")]
        [MaxLength(200)]
        public string Descripcion { get; set; }

        [Required]
        [Column("gas_monto")]
        public decimal Monto { get; set; }

        [Column("gas_proveedor")]
        [MaxLength(150)]
        public string Proveedor { get; set; }

        [Column("gas_fecha")]
        public DateTime Fecha { get; set; }

        [Column("gas_notas")]
        public string Notas { get; set; }

        [Required]
        [Column("gas_fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }
    }
}
```

- [ ] **Step 2: Create SQL script**

```sql
CREATE TABLE [dbo].[tbl_gastos] (
    [gas_id]            INT IDENTITY(1,1) NOT NULL,
    [gas_eve_id]        INT NOT NULL,
    [gas_categoria]     NVARCHAR(50) NOT NULL,
    [gas_descripcion]   NVARCHAR(200) NOT NULL,
    [gas_monto]         DECIMAL(18,2) NOT NULL,
    [gas_proveedor]     NVARCHAR(150) NULL,
    [gas_fecha]         DATETIME NOT NULL,
    [gas_notas]         NVARCHAR(MAX) NULL,
    [gas_fecha_creacion] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_tbl_gastos] PRIMARY KEY CLUSTERED ([gas_id] ASC),
    CONSTRAINT [FK_tbl_gastos_tbl_eventos] FOREIGN KEY ([gas_eve_id]) REFERENCES [dbo].[tbl_eventos] ([eve_id])
);
```

- [ ] **Step 3: Execute SQL script in SSMS**

Run the script against the EventHubv01 database.

- [ ] **Step 4: Commit**

```bash
git add EventHub.03.Data/Entities/Gasto.cs
git commit -m "feat: add Gasto entity for budget tracking"
```

### Task 12: Add DbSet and update EventHubContext

**Files:**
- Modify: `EventHub.03.Data/EventHubContext.cs`

- [ ] **Step 1: Add DbSet<Gasto>**

```csharp
public DbSet<Gasto> Gastos { get; set; }
```

- [ ] **Step 2: Commit**

```bash
git add EventHub.03.Data/EventHubContext.cs
git commit -m "feat: add Gastos DbSet to EventHubContext"
```

### Task 13: Create GastoService and DTOs

**Files:**
- Create: `EventHub.02.Bussines/Services/IGastoService.cs`
- Create: `EventHub.02.Bussines/Services/GastoService.cs`
- Create: `EventHub.02.Bussines/DTOs/GastoDto.cs`
- Create: `EventHub.02.Bussines/DTOs/GastoFormDto.cs`

- [ ] **Step 1: Create DTOs**

```csharp
// GastoDto.cs
public class GastoDto
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public string Categoria { get; set; }
    public string Descripcion { get; set; }
    public decimal Monto { get; set; }
    public string Proveedor { get; set; }
    public DateTime Fecha { get; set; }
    public string Notas { get; set; }
    public DateTime FechaCreacion { get; set; }
}

// GastoFormDto.cs
public class GastoFormDto
{
    [Required(ErrorMessage = "El evento es obligatorio")]
    public int EventoId { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public string Categoria { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    public decimal Monto { get; set; }

    public string Proveedor { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public string Notas { get; set; }
}
```

- [ ] **Step 2: Create IGastoService**

```csharp
public interface IGastoService
{
    List<GastoDto> ObtenerPorEvento(int eventoId);
    GastoDto ObtenerPorId(int id);
    GastoDto Crear(GastoFormDto model);
    GastoDto Actualizar(GastoFormDto model);
    bool Eliminar(int id);
    decimal ObtenerTotalGastado(int eventoId);
}
```

- [ ] **Step 3: Create GastoService**

```csharp
public class GastoService : IGastoService
{
    private readonly EventHubContext _context;

    public GastoService()
    {
        _context = new EventHubContext();
    }

    public List<GastoDto> ObtenerPorEvento(int eventoId)
    {
        return _context.Gastos
            .Where(g => g.EventoId == eventoId)
            .OrderByDescending(g => g.Fecha)
            .Select(g => new GastoDto
            {
                Id = g.Id,
                EventoId = g.EventoId,
                Categoria = g.Categoria,
                Descripcion = g.Descripcion,
                Monto = g.Monto,
                Proveedor = g.Proveedor,
                Fecha = g.Fecha,
                Notas = g.Notas,
                FechaCreacion = g.FechaCreacion
            })
            .ToList();
    }

    public GastoDto ObtenerPorId(int id)
    {
        var g = _context.Gastos.Find(id);
        if (g == null) return null;
        return new GastoDto
        {
            Id = g.Id, EventoId = g.EventoId, Categoria = g.Categoria,
            Descripcion = g.Descripcion, Monto = g.Monto, Proveedor = g.Proveedor,
            Fecha = g.Fecha, Notas = g.Notas, FechaCreacion = g.FechaCreacion
        };
    }

    public GastoDto Crear(GastoFormDto model)
    {
        var gasto = new Gasto
        {
            EventoId = model.EventoId,
            Categoria = model.Categoria,
            Descripcion = model.Descripcion,
            Monto = model.Monto,
            Proveedor = model.Proveedor,
            Fecha = model.Fecha,
            Notas = model.Notas,
            FechaCreacion = DateTime.Now
        };
        _context.Gastos.Add(gasto);
        _context.SaveChanges();

        // Update GastoReal on Evento
        var evento = _context.Eventos.Find(model.EventoId);
        if (evento != null)
        {
            evento.GastoReal = _context.Gastos.Where(g => g.EventoId == model.EventoId).Sum(g => g.Monto);
            _context.SaveChanges();
        }

        return ObtenerPorId(gasto.Id);
    }

    public GastoDto Actualizar(GastoFormDto model)
    {
        var gasto = _context.Gastos.Find(model.Id);
        if (gasto == null) return null;

        gasto.Categoria = model.Categoria;
        gasto.Descripcion = model.Descripcion;
        gasto.Monto = model.Monto;
        gasto.Proveedor = model.Proveedor;
        gasto.Fecha = model.Fecha;
        gasto.Notas = model.Notas;
        _context.SaveChanges();

        // Update GastoReal
        var evento = _context.Eventos.Find(model.EventoId);
        if (evento != null)
        {
            evento.GastoReal = _context.Gastos.Where(g => g.EventoId == model.EventoId).Sum(g => g.Monto);
            _context.SaveChanges();
        }

        return ObtenerPorId(gasto.Id);
    }

    public bool Eliminar(int id)
    {
        var gasto = _context.Gastos.Find(id);
        if (gasto == null) return false;

        var eventoId = gasto.EventoId;
        _context.Gastos.Remove(gasto);
        _context.SaveChanges();

        // Update GastoReal
        var evento = _context.Eventos.Find(eventoId);
        if (evento != null)
        {
            var total = _context.Gastos.Where(g => g.EventoId == eventoId).Sum(g => (decimal?)g.Monto) ?? 0;
            evento.GastoReal = total;
            _context.SaveChanges();
        }

        return true;
    }

    public decimal ObtenerTotalGastado(int eventoId)
    {
        return _context.Gastos.Where(g => g.EventoId == eventoId).Sum(g => g.Monto);
    }
}
```

- [ ] **Step 4: Commit**

```bash
git add EventHub.02.Bussines/Services/IGastoService.cs EventHub.02.Bussines/Services/GastoService.cs EventHub.02.Bussines/DTOs/GastoDto.cs EventHub.02.Bussines/DTOs/GastoFormDto.cs
git commit -m "feat: add GastoService, interface, and DTOs for budget management"
```

### Task 14: Create PresupuestoController

**Files:**
- Create: `EventHub.01.Web/Controllers/PresupuestoController.cs`

- [ ] **Step 1: Create controller**

```csharp
using System;
using System.Linq;
using System.Web.Mvc;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class PresupuestoController : Controller
    {
        private readonly IGastoService _gastoService;
        private readonly IEventoService _eventoService;

        public PresupuestoController()
        {
            _gastoService = new GastoService();
            var context = new EventHubContext();
            _eventoService = new EventoService(context);
        }

        public async Task<ActionResult> Index(int? eventoId)
        {
            var eventos = await _eventoService.GetAllAsync();
            var eventosActivos = eventos.Where(e => e.Estado != "Finalizado" && e.Estado != "Cancelado").ToList();

            ViewBag.Eventos = new SelectList(eventosActivos, "Id", "Nombre", eventoId);
            ViewBag.EventoSeleccionado = eventoId;

            if (eventoId.HasValue)
            {
                var gastos = _gastoService.ObtenerPorEvento(eventoId.Value);
                var evento = eventos.FirstOrDefault(e => e.Id == eventoId.Value);

                ViewBag.PresupuestoEstimado = evento?.PresupuestoEstimado ?? 0;
                ViewBag.TotalGastado = _gastoService.ObtenerTotalGastado(eventoId.Value);
                ViewBag.Disponible = ViewBag.PresupuestoEstimado - ViewBag.TotalGastado;
                ViewBag.PorcentajeEjecutado = ViewBag.PresupuestoEstimado > 0
                    ? (int)(ViewBag.TotalGastado / ViewBag.PresupuestoEstimado * 100)
                    : 0;

                return View(gastos);
            }

            return View(new List<GastoDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearGastoAjax(GastoFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            try
            {
                var result = _gastoService.Crear(model);
                return Json(new { success = true, gasto = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarGastoAjax(GastoFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            try
            {
                var result = _gastoService.Actualizar(model);
                if (result == null)
                    return Json(new { success = false, message = "Gasto no encontrado" });
                return Json(new { success = true, gasto = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarGastoAjax(int id)
        {
            try
            {
                var result = _gastoService.Eliminar(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add EventHub.01.Web/Controllers/PresupuestoController.cs
git commit -m "feat: add PresupuestoController with CRUD actions"
```

### Task 15: Create Presupuesto Index view

**Files:**
- Create: `EventHub.01.Web/Views/Presupuesto/Index.cshtml`

- [ ] **Step 1: Create the view**

Full view with:
- Event selector dropdown
- 3 summary cards (Estimado, Gastado, Disponible)
- Progress bar
- Category grid (Sonido, Iluminación, Catering, Logística, Decoración, Personal, Otro)
- Expense list table
- Add/Edit expense modal
- Delete confirmation
- Search/filter

- [ ] **Step 2: Add Presupuesto nav link to sidebar**

In `_Layout.cshtml`, after the Clientes nav item:

```html
<a href="@Url.Action("Index", "Presupuesto")" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "Presupuesto" ? "active" : "")" data-tooltip="Presupuesto">
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>
    <span class="nav-label">Presupuesto</span>
</a>
```

- [ ] **Step 3: Verify the page**

Navigate to `/Presupuesto`. Select an event. Confirm summary cards, progress bar, categories, and expense list render correctly. Test CRUD operations.

- [ ] **Step 4: Commit**

```bash
git add EventHub.01.Web/Views/Presupuesto/Index.cshtml EventHub.01.Web/Views/Shared/_Layout.cshtml
git commit -m "feat: add Presupuesto page with budget management UI"
```

---

## Feature 10: Preserve VS Debugging Configuration

### Task 16: Final verification

- [ ] **Step 1: Build the solution**

```
msbuild EventHub.v0.slnx /t:Build /p:Configuration=Debug
```

Expected: Build succeeded, 0 errors

- [ ] **Step 2: Run from Visual Studio**

Open in Visual Studio, press F5. Confirm the application starts and all pages load.

- [ ] **Step 3: Manual smoke test**

Verify each feature:
1. Create a task with a date → card shows correct date
2. Notification email has "Ver Tarea" button
3. Crew sidebar link works
4. Column sorting works on Events and Clients tables
5. Finished events in separate collapsible section
6. Event code hidden from all tables
7. Dashboard shows card view with toggle
8. Event details shows stats, budget, tasks, crew
9. Presupuesto page works with CRUD

- [ ] **Step 4: Final commit (if any fixes needed)**

```bash
git add -A
git commit -m "chore: final verification fixes"
```
