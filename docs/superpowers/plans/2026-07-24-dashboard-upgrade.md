# Dashboard & System Upgrade Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Upgrade the EventHub dashboard and system with professional charts, improved UX, new features (operator photos, task categories, notifications bell, operator task view), and fixes (currency formatting, emoji removal, presupuesto URL).

**Architecture:** Changes span across 3 layers: Data (entity new fields), Business (DTOs, services, interfaces), and Web (views, controllers). Chart.js will be added via CDN for professional dashboard charts. Notifications use the existing `tbl_notificaciones` entity.

**Tech Stack:** ASP.NET MVC 5, .NET Framework 4.8, Entity Framework 6, Bootstrap 5.2, jQuery 3.7, Chart.js 4.x (CDN), vanilla JS.

## Global Constraints
- ASP.NET MVC 5 on .NET Framework 4.8 (no .NET Core, no npm)
- No new NuGet packages - use CDN for Chart.js
- Follow existing code patterns: inline CSS in `<style>`, vanilla JS in `@section scripts`
- All monetary values format as `$X,XXX.XX` (en-US style with 2 decimals)
- Remove ALL emojis from UI text; use SVG icons only
- Currency format: `.ToString("C2", new System.Globalization.CultureInfo("en-US"))`

---

## Task 1: Currency Formatting & Emoji Removal (Global)

**Files:**
- Modify: ALL `.cshtml` files that use `.ToString("C2")`
- Modify: `_Layout.cshtml` if any emojis in sidebar

**Interfaces:**
- Consumes: None
- Produces: Consistent `$X,XXX.XX` format across all views

- [ ] **Step 1: Create a helper method for currency formatting**

Create a static helper class in the Web project:

File: `EventHub.01.Web\Helpers\CurrencyHelper.cs`
```csharp
using System.Globalization;

namespace EventHub._01.Web.Helpers
{
    public static class CurrencyHelper
    {
        private static readonly CultureInfo UsCulture = new CultureInfo("en-US");

        public static string FormatCurrency(this decimal value)
        {
            return value.ToString("C2", UsCulture);
        }

        public static string FormatCurrency(this decimal? value)
        {
            return (value ?? 0).ToString("C2", UsCulture);
        }
    }
}
```

- [ ] **Step 2: Add `@using` to `_Layout.cshtml` for the helper**

In `_Layout.cshtml`, after the existing `@using System.Web.Optimization` line, add:
```razor
@using EventHub._01.Web.Helpers
```

- [ ] **Step 3: Replace all `.ToString("C2")` with `.FormatCurrency()` across views**

Files to modify and line references:
- `Views/Home/Index.cshtml` line 130: `@e.PresupuestoEstimado.ToString("C2")` → `@e.PresupuestoEstimado.FormatCurrency()`
- `Views/Eventos/Index.cshtml` lines 126, 172: same replacement
- `Views/Eventos/Details.cshtml` lines 91, 95, 102: same replacement
- `Views/Presupuesto/Index.cshtml` lines 35, 39, 44, 60, 82: same replacement

- [ ] **Step 4: Remove emojis from all views**

Scan and replace emojis in these files:
- `Views/Eventos/Details.cshtml` line 80: `💰 Presupuesto` → `Presupuesto`
- `Views/Eventos/Details.cshtml` line 108: `👤 Cliente` → `Cliente`
- `Views/Eventos/Details.cshtml` line 119: `📋 Tareas` → `Tareas`
- `Views/Eventos/Details.cshtml` line 144: `👥 Crew Asignado` → `Crew Asignado`
- `Views/Presupuesto/Index.cshtml` line 21: `💰 Presupuesto` → `Presupuesto`
- `Views/Presupuesto/Index.cshtml` line 51: `📊 Desglose por Categoría` → `Desglose por Categoría`
- `Views/Presupuesto/Index.cshtml` line 68: `📋 Gastos Registrados` → `Gastos Registrados`
- `Views/Presupuesto/Index.cshtml` lines 85-86: emoji icons in edit/delete buttons → SVG icons
- `Views/Crew/Index.cshtml` line 85: emoji edit icon → already SVG
- `Views/Home/Index.cshtml` line 125: `📅` date emoji → remove
- `Views/Eventos/Details.cshtml` lines 126-130: `✓` and `⏳` → replace with SVG icons or colored dots

- [ ] **Step 5: Update JavaScript currency parsing in sorting**

In `Views/Eventos/Index.cshtml` line 353, the currency parsing regex works with both formats. No change needed since the format will now be consistent `$X,XXX.XX`.

---

## Task 2: Dashboard Charts with Chart.js

**Files:**
- Modify: `Views/Home/Index.cshtml`
- Modify: `Views/Shared/_Layout.cshtml` (add Chart.js CDN)

**Interfaces:**
- Consumes: `DashboardDto` (existing)
- Produces: Interactive donut + bar charts on dashboard

- [ ] **Step 1: Add Chart.js CDN to `_Layout.cshtml`**

Before the closing `</body>` tag, after the existing script bundles, add:
```html
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.7/dist/chart.umd.min.js"></script>
```

- [ ] **Step 2: Add chart section to Dashboard view**

After the stat cards grid (line 61) and before the "Eventos" section, add a new chart row:

```html
<!-- Charts Row -->
<div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 24px;">
    <div class="card">
        <h3 style="margin: 0 0 16px 0; font-size: 14px; font-weight: 600;">Eventos por Estado</h3>
        <div style="position: relative; height: 220px;">
            <canvas id="chartEstados"></canvas>
        </div>
    </div>
    <div class="card">
        <h3 style="margin: 0 0 16px 0; font-size: 14px; font-weight: 600;">Actividad Mensual</h3>
        <div style="position: relative; height: 220px;">
            <canvas id="chartActividad"></canvas>
        </div>
    </div>
</div>
```

- [ ] **Step 3: Add Chart.js initialization in scripts section**

Add to the `@section scripts` block:

```javascript
// Chart: Eventos por Estado (Donut)
var ctxEstados = document.getElementById('chartEstados');
if (ctxEstados) {
    new Chart(ctxEstados, {
        type: 'doughnut',
        data: {
            labels: ['Planificación', 'Pre-Producción', 'Ejecución', 'Finalizados'],
            datasets: [{
                data: [@Model.EventosPlanificacion, @(Model.EventosActivos - Model.EventosEjecucion - Model.EventosPlanificacion), @Model.EventosEjecucion, @Model.EventosFinalizados],
                backgroundColor: ['#f59e0b', '#8b5cf6', '#3b82f6', '#6b7280'],
                borderWidth: 0,
                hoverOffset: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '65%',
            plugins: {
                legend: { position: 'bottom', labels: { padding: 12, usePointStyle: true, pointStyle: 'circle', font: { size: 12 } } }
            }
        }
    });
}

// Chart: Actividad reciente (Barras)
var ctxActividad = document.getElementById('chartActividad');
if (ctxActividad) {
    var months = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic'];
    var now = new Date();
    var labels = [];
    var data = [];
    for (var i = 5; i >= 0; i--) {
        var d = new Date(now.getFullYear(), now.getMonth() - i, 1);
        labels.push(months[d.getMonth()]);
        data.push(0);
    }
    @foreach (var e in Model.ProximosEventos)
    {
        <text>
        (function() {
            var eMonth = new Date(@e.FechaInicio.Year, @e.FechaInicio.Month - 1).getMonth();
            var eYear = @e.FechaInicio.Year;
            var nowYear = new Date().getFullYear();
            var nowMonth = new Date().getMonth();
            for (var i = 5; i >= 0; i--) {
                var ref = new Date(nowYear, nowMonth - i, 1);
                if (ref.getMonth() === eMonth && ref.getFullYear() === eYear) {
                    data[5 - i]++;
                }
            }
        })();
        </text>
    }
    new Chart(ctxActividad, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Eventos',
                data: data,
                backgroundColor: '#3b82f6',
                borderRadius: 6,
                borderSkipped: false,
                barThickness: 24
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1, font: { size: 11 } }, grid: { color: '#f1f5f9' } },
                x: { grid: { display: false }, ticks: { font: { size: 11 } } }
            }
        }
    });
}
```

---

## Task 3: Dashboard Cards with Event Banners + Quick Actions + Finished Events

**Files:**
- Modify: `Views/Home/Index.cshtml`

**Interfaces:**
- Consumes: `DashboardDto.ProximosEventos` (includes `CoverPhotoUrl`)
- Produces: Cards with real event banners, quick action buttons, finished events section

- [ ] **Step 1: Add Quick Action buttons below page header**

After the `page-header` div (line 9), add:

```html
<!-- Quick Actions -->
<div style="display: flex; gap: 8px; margin-bottom: 24px;">
    <a href="@Url.Action("Create", "Eventos")" class="btn btn-primary btn-sm">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
        Nuevo Evento
    </a>
    <a href="@Url.Action("Create", "Clientes")" class="btn btn-outline btn-sm">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="8.5" cy="7" r="4"/><line x1="20" y1="8" x2="20" y2="14"/><line x1="23" y1="11" x2="17" y2="11"/></svg>
        Nuevo Cliente
    </a>
    <a href="@Url.Action("Index", "Crew")" class="btn btn-outline btn-sm">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="8.5" cy="7" r="4"/><line x1="20" y1="8" x2="20" y2="14"/><line x1="23" y1="11" x2="17" y2="11"/></svg>
        Nuevo Operador
    </a>
</div>
```

- [ ] **Step 2: Update Cards View with event banner or generic gradient**

Replace the Cards View section (lines 112-139). For each event card, use the CoverPhotoUrl if available, otherwise use a generic blue electric gradient:

```razor
<!-- Cards View -->
<div id="cardsView" style="display:none;">
    <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px;">
        @foreach (var e in Model.ProximosEventos)
        {
            var hasImage = !string.IsNullOrEmpty(e.CoverPhotoUrl);
            var gradient = hasImage ? "" :
                e.Estado == "Planificacion" ? "linear-gradient(135deg, #2563eb, #7c3aed)" :
                e.Estado == "PreProduccion" ? "linear-gradient(135deg, #2563eb, #8b5cf6)" :
                e.Estado == "Ejecucion" ? "linear-gradient(135deg, #1d4ed8, #2563eb)" :
                "linear-gradient(135deg, #334155, #475569)";
            <div style="background: var(--bg-card); border: 1px solid var(--border); border-radius: 12px; overflow: hidden; cursor: pointer; transition: box-shadow 0.2s, border-color 0.2s;" onclick="window.location='@Url.Action("Details", "Eventos", new { id = e.Id })'"
                 onmouseover="this.style.boxShadow='0 4px 12px rgba(0,0,0,0.08)'; this.style.borderColor='var(--primary)'"
                 onmouseout="this.style.boxShadow='none'; this.style.borderColor='var(--border)'">
                <div style="height: 120px; @(hasImage ? $"background-image: url('{e.CoverPhotoUrl}'); background-size: cover; background-position: center;" : $"background: {gradient};") position: relative;">
                    <div style="position: absolute; top: 8px; right: 8px; background: rgba(255,255,255,0.9); padding: 2px 8px; border-radius: 10px; font-size: 11px; font-weight: 600;">@e.Estado</div>
                    <div style="position: absolute; bottom: 8px; left: 8px; color: white; font-size: 11px; opacity: 0.9;">@e.FechaInicio.ToString("dd/MM/yyyy")</div>
                </div>
                <div style="padding: 14px;">
                    <div style="font-weight: 700; font-size: 14px; margin-bottom: 4px;">@e.Nombre</div>
                    <div style="font-size: 12px; color: var(--text-muted); margin-bottom: 8px;">@e.ClienteNombre • @e.VenueNombre</div>
                    <div style="font-size: 12px; color: #10b981; font-weight: 600;">@e.PresupuestoEstimado.FormatCurrency()</div>
                </div>
            </div>
        }
    </div>
    @if (Model.ProximosEventos.Count == 0)
    {
        <div style="text-align:center;padding:40px;color:var(--text-muted);">No hay eventos proximos</div>
    }
</div>
```

- [ ] **Step 3: Add Finished Events section after Proximos Eventos**

After the cardsView div and before the `<style>` block, add a finished events section. Modify the `HomeController` to also pass `EventosFinalizados` data:

In `HomeController.cs`, the `GetDashboardAsync()` already returns `EventosFinalizados` count. We need to also include finalizado events in the model. Check if the existing `DashboardDto` has a list for them.

Actually, the existing `DashboardDto` only has `ProximosEventos` (top 5 upcoming). We need to add a new property `EventosFinalizadosRecientes` to show recent finished events on the dashboard.

**File: `EventHub.02.Bussines\DTOs\DashboardDto.cs`** - Add:
```csharp
public List<EventoListDto> EventosFinalizadosRecientes { get; set; } = new List<EventoListDto>();
```

**File: `EventHub.02.Bussines\Services\EventoService.cs`** - In `GetDashboardAsync()`, add query for recent finished events:
```csharp
var finalizadosRecientes = await context.Eventos
    .Where(e => e.Estado == "Finalizado")
    .OrderByDescending(e => e.FechaCreacion)
    .Take(5)
    .Select(e => new EventoListDto { ... })
    .ToListAsync();
dashboard.EventosFinalizadosRecientes = finalizadosRecientes;
```

Then in the dashboard view, after the cards view section:
```html
<!-- Finished Events -->
@if (Model.EventosFinalizadosRecientes != null && Model.EventosFinalizadosRecientes.Any())
{
    <div style="margin-top: 24px;">
        <h3 style="font-size:18px;font-weight:700;margin:0 0 12px 0;">Eventos Finalizados</h3>
        <div class="card" style="padding:0;overflow:hidden;">
            <table class="table">
                <thead>
                    <tr>
                        <th>Evento</th>
                        <th>Cliente</th>
                        <th>Fecha</th>
                        <th>Presupuesto</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var e in Model.EventosFinalizadosRecientes)
                    {
                        <tr style="cursor:pointer;" onclick="window.location='@Url.Action("Details", "Eventos", new { id = e.Id })'">
                            <td style="font-weight:600;">@e.Nombre</td>
                            <td>@e.ClienteNombre</td>
                            <td>@e.FechaInicio.ToString("dd/MM/yyyy")</td>
                            <td>@e.PresupuestoEstimado.FormatCurrency()</td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>
}
```

---

## Task 4: Fix Presupuesto Sidebar URL + Event Selector View

**Files:**
- Modify: `Views/Shared/_Layout.cshtml` (sidebar link)
- Modify: `Controllers/PresupuestoController.cs`
- Modify: `Views/Presupuesto/Index.cshtml` (add event selector when eventoId=0)
- Create: `Views/Presupuesto\Seleccionar.cshtml` (event picker)

**Interfaces:**
- Consumes: `IEventoService.GetAllAsync()`
- Produces: Presupuesto shows event selector first, then budget view

- [ ] **Step 1: Update sidebar link to `Presupuesto/Index` without eventoId**

In `_Layout.cshtml` line 543, change:
```razor
<a href="@Url.Action("Index", "Presupuesto", new { eventoId = 0 })" ...>
```
to:
```razor
<a href="@Url.Action("Index", "Presupuesto")" ...>
```

- [ ] **Step 2: Update PresupuestoController to handle no eventoId**

```csharp
public async Task<ActionResult> Index(int? eventoId)
{
    if (!eventoId.HasValue || eventoId.Value == 0)
    {
        // Show event selector
        var eventos = await _eventoService.GetAllAsync();
        ViewBag.Eventos = eventos;
        return View("Seleccionar");
    }

    var evento = await _eventoService.GetByIdAsync(eventoId.Value);
    if (evento == null) return HttpNotFound();

    ViewBag.Evento = evento;
    ViewBag.Gastos = _gastoService.ObtenerPorEvento(eventoId.Value);
    ViewBag.Resumen = _gastoService.ObtenerResumenPorCategoria(eventoId.Value);

    return View(new GastoFormDto { EventoId = eventoId.Value });
}
```

- [ ] **Step 3: Create the event selector view**

File: `Views/Presupuesto/Seleccionar.cshtml`
```razor
@model dynamic
@{
    ViewBag.Title = "Presupuesto";
    var eventos = ViewBag.Eventos as List<EventHub._02.Bussines.DTOs.EventoListDto> ?? new List<EventHub._02.Bussines.DTOs.EventoListDto>();
}

<div class="page-header">
    <div style="display:flex;align-items:center;gap:12px;">
        <a href="@Url.Action("Index","Home")" class="btn btn-outline btn-sm" style="padding:6px 10px;">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M19 12H5"/><path d="M12 19l-7-7 7-7"/></svg>
        </a>
        <div>
            <h1>Presupuesto</h1>
            <p>Selecciona un evento para ver su presupuesto</p>
        </div>
    </div>
</div>

<div class="action-bar">
    <div class="search-box">
        <input type="text" id="searchEventos" class="form-control" placeholder="Buscar evento..." style="width:240px" />
    </div>
</div>

<div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 16px;">
    @foreach (var e in eventos)
    {
        var hasImage = !string.IsNullOrEmpty(e.CoverPhotoUrl);
        var gradient = hasImage ? "" : "linear-gradient(135deg, #2563eb, #7c3aed)";
        <div class="evento-select-card" data-search="@e.Nombre @e.ClienteNombre @e.VenueNombre"
             onclick="window.location='@Url.Action("Index", new { eventoId = e.Id })'"
             style="background: var(--bg-card); border: 1px solid var(--border); border-radius: 12px; overflow: hidden; cursor: pointer; transition: box-shadow 0.2s, border-color 0.2s;"
             onmouseover="this.style.boxShadow='0 4px 12px rgba(0,0,0,0.08)'; this.style.borderColor='var(--primary)'"
             onmouseout="this.style.boxShadow='none'; this.style.borderColor='var(--border)'">
            <div style="height: 100px; @(hasImage ? $"background-image: url('{e.CoverPhotoUrl}'); background-size: cover; background-position: center;" : $"background: {gradient};") position: relative;">
                <div style="position: absolute; top: 8px; right: 8px; background: rgba(255,255,255,0.9); padding: 2px 8px; border-radius: 10px; font-size: 11px; font-weight: 600;">@e.Estado</div>
            </div>
            <div style="padding: 14px;">
                <div style="font-weight: 700; font-size: 15px; margin-bottom: 4px;">@e.Nombre</div>
                <div style="font-size: 12px; color: var(--text-muted); margin-bottom: 6px;">@e.ClienteNombre • @e.VenueNombre</div>
                <div style="display: flex; justify-content: space-between; align-items: center;">
                    <span style="font-size: 12px; color: var(--text-muted);">@e.FechaInicio.ToString("dd/MM/yyyy")</span>
                    <span style="font-size: 13px; font-weight: 600; color: #10b981;">@e.PresupuestoEstimado.FormatCurrency()</span>
                </div>
            </div>
        </div>
    }
</div>

@section scripts {
    <script>
        document.getElementById('searchEventos').addEventListener('input', function() {
            var q = this.value.toLowerCase();
            document.querySelectorAll('.evento-select-card').forEach(function(card) {
                var text = (card.dataset.search || '').toLowerCase();
                card.style.display = text.includes(q) ? '' : 'none';
            });
        });
    </script>
}
```

---

## Task 5: Task Categories + Remove "Responsable" from Tasks

**Files:**
- Modify: `EventHub.03.Data\Entities\Tarea.cs` (add Categoria field)
- Modify: `EventHub.02.Bussines\DTOs\TareaDto.cs` (add Categoria)
- Modify: `EventHub.02.Bussines\Services\TareaService.cs` (handle Categoria)
- Modify: `Views/Eventos\Tareas.cshtml` (add category dropdown, remove AsignadoA dropdown)
- Modify: `Controllers\EventosController.cs` (remove responsable from task, send email to operator)

**Interfaces:**
- Consumes: None
- Produces: Tasks have categories, notifications go to operator only

- [ ] **Step 1: Add Categoria field to Tarea entity**

In `EventHub.03.Data\Entities\Tarea.cs`, add after the `Orden` property:
```csharp
[MaxLength(50)]
[Column("tar_categoria")]
public string Categoria { get; set; } // Montaje, Desmontaje, Audio/Video, Logistica, Otro
```

- [ ] **Step 2: Add Categoria to TareaDto and TareaFormDto**

In `EventHub.02.Bussines\DTOs\TareaDto.cs`:

Add to `TareaDto`:
```csharp
public string Categoria { get; set; }
```

Add to `TareaFormDto`:
```csharp
[StringLength(50)]
public string Categoria { get; set; }
```

- [ ] **Step 3: Update TareaService to handle Categoria**

In `EventHub.02.Bussines\Services\TareaService.cs`:
- In `ObtenerTareasPorEvento` select: add `Categoria = t.Categoria`
- In `CrearTarea`: add `Categoria = dto.Categoria`
- In `CrearTarea` return DTO: add `Categoria = nuevaTarea.Categoria`
- In `ObtenerPorId`: add `Categoria = t.Categoria`

- [ ] **Step 4: Update Tareas.cshtml - Add category dropdown, remove AsignadoA**

In `Views/Eventos/Tareas.cshtml`:

1. Remove the "Responsable (Usuario)" form-group (lines 172-175)
2. Replace with a category dropdown:
```html
<div class="form-group">
    <label>Categoría</label>
    <select name="Categoria" id="formCategoria" class="form-control">
        <option value="">Sin categoría</option>
        <option value="Montaje">Montaje</option>
        <option value="Desmontaje">Desmontaje</option>
        <option value="Audio/Video">Audio/Video</option>
        <option value="Iluminación">Iluminación</option>
        <option value="Logística">Logística</option>
        <option value="Decoración">Decoración</option>
        <option value="Personal">Personal</option>
        <option value="Catering">Catering</option>
        <option value="Otro">Otro</option>
    </select>
</div>
```

3. Update the `tasksDict` JavaScript to include `Categoria`
4. Update `editarTarea()` to set `formCategoria` value
5. Add category badge to kanban cards (colored pill)
6. Update `guardarTarea()` to send Categoria

- [ ] **Step 5: Add category color mapping to kanban cards**

In the card rendering (both Razor and JS-generated), add a category badge:
```razor
@if (!string.IsNullOrEmpty(t.Categoria))
{
    var catColors = new Dictionary<string, string> {
        { "Montaje", "#3b82f6" }, { "Desmontaje", "#6b7280" }, { "Audio/Video", "#8b5cf6" },
        { "Iluminación", "#f59e0b" }, { "Logística", "#10b981" }, { "Decoración", "#ec4899" },
        { "Personal", "#06b6d4" }, { "Catering", "#f97316" }, { "Otro", "#6b7280" }
    };
    var catColor = catColors.ContainsKey(t.Categoria) ? catColors[t.Categoria] : "#6b7280";
    <span style="display:inline-block; font-size:10px; padding:1px 6px; border-radius:4px; background:@(catColor)15; color:@catColor; font-weight:600;">@t.Categoria</span>
}
```

- [ ] **Step 6: Update EventosController to send notifications to operator instead of responsable**

In `Controllers/EventosController.cs`:

**CreateTareaAjax** (line 308-330): Change notification logic:
```csharp
// Send notification to the assigned OPERATOR (CrewOperador), not responsable
if (result.CrewOperadorId.HasValue && !string.IsNullOrEmpty(result.CrewOperadorEmail))
{
    var context = new EventHubContext();
    var evento = context.Eventos.Find(model.EventoId);
    var eventoNombre = evento?.Nombre ?? "Evento";

    var mensaje = $"Se te asigno la tarea \"{result.Titulo}\" en el evento \"{eventoNombre}\".";
    if (result.FechaLimite.HasValue)
        mensaje += $" Fecha limite: {result.FechaLimite.Value.ToString(\"dd/MM/yyyy\")}.";

    _notificacionService.CrearYEnviar(
        "TareaCreada",
        mensaje,
        result.CrewOperadorEmail,
        result.CrewOperadorNombre,
        model.EventoId,
        result.Id,
        _emailService,
        eventoNombre,
        result.Titulo
    );
}
```

**EditTareaAjax** (line 364-382): Same change - notify operator instead of responsable.

**UpdateTareaStatusAjax** (line 400-422): Same change.

Also remove the `AsignadoAId` field from the task form since the operator is the only assignee now.

---

## Task 6: Operator Profile Photo + Operator Task View

**Files:**
- Modify: `EventHub.03.Data\Entities\Operador.cs` (add FotoUrl)
- Modify: `EventHub.03.Data\Entities\CrewOperador.cs` (add FotoUrl)
- Modify: `EventHub.02.Bussines\DTOs\OperadorDto.cs` (add FotoUrl)
- Modify: `EventHub.02.Bussines\DTOs\CrewOperadorDto.cs` (add FotoUrl)
- Modify: `Views/Crew/IndexGlobal.cshtml` (show photo, add create button)
- Modify: `Views/Crew/Index.cshtml` (show photo)
- Create: `Views/Operadores/MisTareas.cshtml` (operator read-only task view)
- Modify: `Controllers/OperadoresController.cs` (add MisTareas action, photo upload)

**Interfaces:**
- Consumes: TareaService, CrewService
- Produces: Operators have optional photos, can view their tasks

- [ ] **Step 1: Add FotoUrl to Operador and CrewOperador entities**

In `EventHub.03.Data\Entities\Operador.cs`, add:
```csharp
[MaxLength(500)]
[Column("ope_foto_url")]
public string FotoUrl { get; set; }
```

In `EventHub.03.Data\Entities\CrewOperador.cs`, add:
```csharp
[MaxLength(500)]
[Column("cro_foto_url")]
public string FotoUrl { get; set; }
```

- [ ] **Step 2: Add FotoUrl to DTOs**

In `OperadorDto`: add `public string FotoUrl { get; set; }`
In `OperadorFormDto`: add `public string FotoUrl { get; set; }`
In `CrewOperadorDto`: add `public string FotoUrl { get; set; }`
In `CrewOperadorFormDto`: add `public string FotoUrl { get; set; }`

- [ ] **Step 3: Update Services to handle FotoUrl**

In `OperadorService.cs`:
- In all `.Select()` projections, add `FotoUrl = o.FotoUrl`
- In `Create()`, add `FotoUrl = dto.FotoUrl`
- In `Update()`, add `existing.FotoUrl = dto.FotoUrl`

In `CrewService.cs`:
- In all `.Select()` projections, add `FotoUrl = c.FotoUrl`
- In `CrearCrew()`, add `FotoUrl = dto.FotoUrl` to the new CrewOperador

- [ ] **Step 4: Update Crew/IndexGlobal.cshtml - Add create button + show photos**

Replace the avatar div to show photo if available:
```razor
<div class="crew-avatar">
    @if (!string.IsNullOrEmpty(op.FotoUrl))
    {
        <img src="@op.FotoUrl" alt="@op.Nombre" style="width:100%;height:100%;border-radius:50%;object-fit:cover;" />
    }
    else
    {
        @(op.Nombre.Length > 0 ? op.Nombre.Substring(0, 1).ToUpper() : "?")
    }
</div>
```

Add a "Nuevo Operador" button in the action bar:
```html
<button type="button" class="btn btn-primary" onclick="openOperadorModal()">
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
    Nuevo Operador
</button>
```

Add create/edit modal with photo upload field (hidden input + preview).

- [ ] **Step 5: Update Crew/Index.cshtml - Show photos in crew cards**

Same avatar photo logic as Step 4.

- [ ] **Step 6: Create operator task view (read-only)**

File: `Views/Operadores/MisTareas.cshtml`

This view allows operators (logged in with their email) to see tasks assigned to them in read-only mode.

```razor
@model IEnumerable<EventHub._02.Bussines.DTOs.TareaDto>
@{
    ViewBag.Title = "Mis Tareas";
}

<div class="page-header">
    <h1>Mis Tareas</h1>
    <p>Tareas asignadas como operador</p>
</div>

<div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px;">
    @foreach (var estado in new[] { "Pendiente", "EnProgreso", "Completado" })
    {
        var tareasEstado = Model.Where(t => t.Estado == estado).ToList();
        var headerColor = estado == "Pendiente" ? "#f59e0b" : estado == "EnProgreso" ? "#3b82f6" : "#10b981";
        <div>
            <h3 style="font-size:14px;font-weight:600;margin-bottom:12px;color:@headerColor;">
                @estado (@tareasEstado.Count)
            </h3>
            <div style="display:flex;flex-direction:column;gap:8px;">
                @foreach (var t in tareasEstado)
                {
                    <div class="card" style="padding:14px;">
                        <div style="font-weight:600;font-size:14px;margin-bottom:4px;">@t.Titulo</div>
                        @if (!string.IsNullOrEmpty(t.Categoria))
                        {
                            <span style="display:inline-block;font-size:10px;padding:1px 6px;border-radius:4px;background:#eff6ff;color:#2563eb;font-weight:600;margin-bottom:6px;">@t.Categoria</span>
                        }
                        @if (!string.IsNullOrEmpty(t.Descripcion))
                        {
                            <div style="font-size:12px;color:var(--text-muted);margin-bottom:6px;">@t.Descripcion</div>
                        }
                        @if (t.FechaLimite.HasValue)
                        {
                            <div style="font-size:11px;color:var(--text-muted);">Fecha limite: @t.FechaLimite.Value.ToString("dd/MM/yyyy")</div>
                        }
                    </div>
                }
                @if (!tareasEstado.Any())
                {
                    <div style="text-align:center;padding:20px;color:var(--text-muted);font-size:13px;">Sin tareas</div>
                }
            </div>
        </div>
    }
</div>
```

- [ ] **Step 7: Add MisTareas action to OperadoresController**

```csharp
[AllowAnonymous]
public ActionResult MisTareas(string email)
{
    if (string.IsNullOrEmpty(email))
        return View("MisTareas", new List<TareaDto>());

    var context = new EventHubContext();
    var tareaService = new TareaService();
    var crew = context.CrewOperadores.FirstOrDefault(c => c.Email == email);
    if (crew == null)
        return View("MisTareas", new List<TareaDto>());

    var tareas = context.Tareas
        .Where(t => t.CrewOperadorId == crew.Id)
        .OrderBy(t => t.Estado).ThenBy(t => t.Orden)
        .Select(t => new TareaDto { ... })
        .ToList();

    return View("MisTareas", tareas);
}
```

Note: The `MisTareas` action uses `[AllowAnonymous]` so operators can access via a link with their email. For a more secure approach, this could be enhanced with token-based auth later.

---

## Task 7: Notification Bell in Layout

**Files:**
- Modify: `Views/Shared/_Layout.cshtml` (add bell icon + dropdown)
- Modify: `Controllers/HomeController.cs` (or new API endpoint for notifications)

**Interfaces:**
- Consumes: `NotificacionService.ObtenerRecientes()`, `NotificacionService.ContarNoLeidas()`
- Produces: Facebook-style notification bell with dropdown

- [ ] **Step 1: Add notification bell to the layout header area**

In `_Layout.cshtml`, add a notification section just before the sidebar footer, or better yet, in the `main-content` area as a floating element. Add it to the sidebar footer area:

Actually, add it as a fixed element in the top-right of the main content area. In `_Layout.cshtml`, after `<main class="main-content">`, add:

```html
<!-- Notification Bell -->
<div id="notifBell" style="position:fixed; top:16px; right:32px; z-index:500;">
    <button onclick="toggleNotifPanel()" style="background:var(--bg-card); border:1px solid var(--border); border-radius:10px; width:40px; height:40px; display:flex; align-items:center; justify-content:center; cursor:pointer; position:relative;">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="var(--text-muted)" stroke-width="2"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/></svg>
        <span id="notifBadge" style="display:none; position:absolute; top:-4px; right:-4px; background:#ef4444; color:white; font-size:10px; font-weight:700; width:18px; height:18px; border-radius:50%; display:flex; align-items:center; justify-content:center;"></span>
    </button>
    <div id="notifPanel" style="display:none; position:absolute; top:48px; right:0; width:360px; background:var(--bg-card); border:1px solid var(--border); border-radius:12px; box-shadow:0 8px 24px rgba(0,0,0,0.12); overflow:hidden; max-height:400px; overflow-y:auto;">
        <div style="padding:14px 16px; border-bottom:1px solid var(--border); font-weight:700; font-size:14px;">Notificaciones</div>
        <div id="notifList" style="padding:0;">
            <div style="text-align:center; padding:24px; color:var(--text-muted); font-size:13px;">Cargando...</div>
        </div>
        <div style="padding:10px 16px; border-top:1px solid var(--border); text-align:center;">
            <button onclick="marcarTodasLeidas()" style="background:none; border:none; color:var(--primary); font-size:12px; font-weight:600; cursor:pointer;">Marcar todo como leido</button>
        </div>
    </div>
</div>
```

- [ ] **Step 2: Add notification API endpoint**

In `HomeController.cs` (or create a `NotificacionesController.cs`), add:

```csharp
[HttpGet]
public ActionResult ObtenerRecientes()
{
    var notifService = new NotificacionService();
    var notificaciones = notifService.ObtenerRecientes(15);
    var noLeidas = notifService.ContarNoLeidas();

    return Json(new { success = true, notificaciones = notificaciones, noLeidas = noLeidas }, JsonRequestBehavior.AllowGet);
}

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult MarcarLeida(int id)
{
    var notifService = new NotificacionService();
    notifService.MarcarComoLeida(id);
    return Json(new { success = true });
}

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult MarcarTodasLeidas()
{
    var context = new EventHubContext();
    var noLeidas = context.Notificaciones.Where(n => !n.Leida).ToList();
    foreach (var n in noLeidas) n.Leida = true;
    context.SaveChanges();
    return Json(new { success = true });
}
```

- [ ] **Step 3: Add JavaScript for notification panel**

In `_Layout.cshtml`, add to the script section:

```javascript
function toggleNotifPanel() {
    var panel = document.getElementById('notifPanel');
    if (panel.style.display === 'none') {
        panel.style.display = 'block';
        loadNotificaciones();
    } else {
        panel.style.display = 'none';
    }
}

function loadNotificaciones() {
    fetch('@Url.Action("ObtenerRecientes", "Home")')
    .then(r => r.json())
    .then(data => {
        if (data.success) {
            updateBadge(data.noLeidas);
            var list = document.getElementById('notifList');
            if (data.notificaciones.length === 0) {
                list.innerHTML = '<div style="text-align:center;padding:24px;color:var(--text-muted);font-size:13px;">Sin notificaciones</div>';
                return;
            }
            list.innerHTML = data.notifications || data.notificaciones.map(function(n) {
                var tipoIcon = n.Tipo === 'TareaCreada' ? '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#3b82f6" stroke-width="2"><rect x="3" y="3" width="18" height="18" rx="2"/><path d="M9 12l2 2 4-4"/></svg>'
                    : n.Tipo === 'TareaCompletada' ? '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#10b981" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>'
                    : '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#f59e0b" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>';
                var bgColor = n.Leida ? 'transparent' : 'var(--primary-light)';
                return '<div onclick="marcarLeida(' + n.Id + ')" style="display:flex;gap:10px;padding:12px 16px;background:' + bgColor + ';cursor:pointer;border-bottom:1px solid var(--border);">' +
                    '<div style="flex-shrink:0;margin-top:2px;">' + tipoIcon + '</div>' +
                    '<div style="flex:1;min-width:0;">' +
                    '<div style="font-size:13px;line-height:1.4;">' + n.Mensaje + '</div>' +
                    '<div style="font-size:11px;color:var(--text-muted);margin-top:4px;">' + new Date(n.FechaCreacion).toLocaleString('es-ES') + '</div>' +
                    '</div></div>';
            }).join('');
        }
    });
}

function updateBadge(count) {
    var badge = document.getElementById('notifBadge');
    if (count > 0) {
        badge.style.display = 'flex';
        badge.textContent = count > 99 ? '99+' : count;
    } else {
        badge.style.display = 'none';
    }
}

function marcarLeida(id) {
    var fd = new FormData();
    fd.append('id', id);
    fd.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);
    fetch('@Url.Action("MarcarLeida", "Home")', { method: 'POST', body: fd })
    .then(r => r.json())
    .then(data => { if (data.success) loadNotificaciones(); });
}

function marcarTodasLeidas() {
    var fd = new FormData();
    fd.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);
    fetch('@Url.Action("MarcarTodasLeidas", "Home")', { method: 'POST', body: fd })
    .then(r => r.json())
    .then(data => { if (data.success) loadNotificaciones(); });
}

// Load unread count on page load
(function() {
    fetch('@Url.Action("ObtenerRecientes", "Home")')
    .then(r => r.json())
    .then(data => { if (data.success) updateBadge(data.noLeidas); });
})();

// Close panel when clicking outside
document.addEventListener('click', function(e) {
    var bell = document.getElementById('notifBell');
    if (!bell.contains(e.target)) {
        document.getElementById('notifPanel').style.display = 'none';
    }
});
```

- [ ] **Step 4: Hide bell on mobile or adjust positioning**

For mobile responsive, add to the `@@media (max-width: 768px)` block:
```css
#notifBell { top: 12px; right: 60px; }
```

---

## Task 8: DashboardDto Update for Finished Events

**Files:**
- Modify: `EventHub.02.Bussines\DTOs\DashboardDto.cs`
- Modify: `EventHub.02.Bussines\Services\EventoService.cs`

**Interfaces:**
- Consumes: EventoService.GetDashboardAsync()
- Produces: DashboardDto with EventosFinalizadosRecientes

- [ ] **Step 1: Update DashboardDto - add finished events list and CoverPhotoUrl to ProximosEventos**

File: `EventHub.02.Bussines\DTOs\DashboardDto.cs`

```csharp
using System.Collections.Generic;

namespace EventHub._02.Bussines.DTOs
{
    public class DashboardDto
    {
        public int TotalEventos { get; set; }
        public int EventosActivos { get; set; }
        public int EventosPlanificacion { get; set; }
        public int EventosEjecucion { get; set; }
        public int EventosFinalizados { get; set; }
        public int TotalClientes { get; set; }
        public int ClientesActivos { get; set; }
        public List<EventoListDto> ProximosEventos { get; set; }
        public List<EventoListDto> EventosFinalizadosRecientes { get; set; } = new List<EventoListDto>();
    }
}
```

- [ ] **Step 2: Update EventoService.GetDashboardAsync() - add CoverPhotoUrl to ProximosEventos select + finished events query**

File: `EventHub.02.Bussines\Services\EventoService.cs`

Replace the `GetDashboardAsync` method (lines 158-189):

```csharp
public async Task<DashboardDto> GetDashboardAsync()
{
    var now = DateTime.Now;

    return new DashboardDto
    {
        TotalEventos = await _context.Eventos.CountAsync(),
        EventosActivos = await _context.Eventos.CountAsync(e => e.Estado != "Finalizado" && e.Estado != "Cancelado"),
        EventosPlanificacion = await _context.Eventos.CountAsync(e => e.Estado == "Planificacion"),
        EventosEjecucion = await _context.Eventos.CountAsync(e => e.Estado == "Ejecucion" || e.Estado == "PreProduccion"),
        EventosFinalizados = await _context.Eventos.CountAsync(e => e.Estado == "Finalizado"),
        TotalClientes = await _context.Clientes.CountAsync(),
        ClientesActivos = await _context.Clientes.CountAsync(c => c.Estado),
        ProximosEventos = await _context.Eventos
            .Include(e => e.Cliente)
            .Include(e => e.Venue)
            .Where(e => e.FechaInicio >= now && e.Estado != "Finalizado" && e.Estado != "Cancelado")
            .OrderBy(e => e.FechaInicio)
            .Take(5)
            .Select(e => new EventoListDto
            {
                Id = e.Id,
                Codigo = e.Codigo,
                Nombre = e.Nombre,
                ClienteNombre = e.Cliente.Nombre,
                VenueNombre = e.Venue.Nombre,
                FechaInicio = e.FechaInicio,
                Estado = e.Estado,
                PresupuestoEstimado = e.PresupuestoEstimado,
                CoverPhotoUrl = e.CoverPhotoUrl
            })
            .ToListAsync(),
        EventosFinalizadosRecientes = await _context.Eventos
            .Include(e => e.Cliente)
            .Include(e => e.Venue)
            .Where(e => e.Estado == "Finalizado" || e.Estado == "Cancelado")
            .OrderByDescending(e => e.FechaCierre)
            .Take(5)
            .Select(e => new EventoListDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                ClienteNombre = e.Cliente.Nombre,
                FechaInicio = e.FechaInicio,
                Estado = e.Estado,
                PresupuestoEstimado = e.PresupuestoEstimado
            })
            .ToListAsync()
    };
}
```

---

## Task 9: SQL Migration for New Columns

**Files:**
- Create: `docs/sql/ALTER_add Categoria_foto_columns.sql`

**Interfaces:**
- Consumes: Entity changes from Tasks 1, 5, 6
- Produces: SQL script for DB schema update

- [ ] **Step 1: Create ALTER TABLE script**

```sql
-- Add Categoria to tbl_tareas
ALTER TABLE tbl_tareas ADD tar_categoria NVARCHAR(50) NULL;

-- Add FotoUrl to tbl_operadores
ALTER TABLE tbl_operadores ADD ope_foto_url NVARCHAR(500) NULL;

-- Add FotoUrl to tbl_crew_operadores
ALTER TABLE tbl_crew_operadores ADD cro_foto_url NVARCHAR(500) NULL;
```

---

## Task 10: Verification

- [ ] **Step 1: Build the solution**
```
msbuild EventHub.v0.slnx /t:Build
```

- [ ] **Step 2: Verify no compilation errors**

- [ ] **Step 3: Run the application and test each feature**

---

## Summary of All Files Modified

| File | Changes |
|------|---------|
| `EventHub.01.Web\Helpers\CurrencyHelper.cs` | NEW - Currency formatting helper |
| `EventHub.01.Web\Views\Shared\_Layout.cshtml` | Add Chart.js CDN, notification bell, sidebar fix, `@using Helpers` |
| `EventHub.01.Web\Views\Home\Index.cshtml` | Charts, banner cards, quick actions, finished events, emoji removal |
| `EventHub.01.Web\Views\Presupuesto\Index.cshtml` | Currency formatting, emoji removal |
| `EventHub.01.Web\Views\Presupuesto\Seleccionar.cshtml` | NEW - Event picker for budget |
| `EventHub.01.Web\Views\Eventos\Tareas.cshtml` | Category dropdown, remove AsignadoA, category badges |
| `EventHub.01.Web\Views\Eventos\Index.cshtml` | Currency formatting |
| `EventHub.01.Web\Views\Eventos\Details.cshtml` | Currency formatting, emoji removal |
| `EventHub.01.Web\Views\Crew\IndexGlobal.cshtml` | Photo display, create button |
| `EventHub.01.Web\Views\Crew\Index.cshtml` | Photo display |
| `EventHub.01.Web\Views\Operadores\MisTareas.cshtml` | NEW - Operator task view |
| `EventHub.01.Web\Controllers\PresupuestoController.cs` | Optional eventoId |
| `EventHub.01.Web\Controllers\HomeController.cs` | Notification API endpoints, finished events |
| `EventHub.01.Web\Controllers\EventosController.cs` | Notify operator instead of responsable |
| `EventHub.01.Web\Controllers\OperadoresController.cs` | MisTareas action |
| `EventHub.02.Bussines\DTOs\TareaDto.cs` | Add Categoria |
| `EventHub.02.Bussines\DTOs\OperadorDto.cs` | Add FotoUrl |
| `EventHub.02.Bussines\DTOs\CrewOperadorDto.cs` | Add FotoUrl |
| `EventHub.02.Bussines\DTOs\DashboardDto.cs` | Add EventosFinalizadosRecientes |
| `EventHub.02.Bussines\Services\TareaService.cs` | Handle Categoria |
| `EventHub.02.Bussines\Services\OperadorService.cs` | Handle FotoUrl |
| `EventHub.02.Bussines\Services\CrewService.cs` | Handle FotoUrl |
| `EventHub.02.Bussines\Services\EventoService.cs` | Finished events query |
| `EventHub.03.Data\Entities\Tarea.cs` | Add Categoria column |
| `EventHub.03.Data\Entities\Operador.cs` | Add FotoUrl column |
| `EventHub.03.Data\Entities\CrewOperador.cs` | Add FotoUrl column |
| `docs/sql/ALTER_add_columns.sql` | NEW - DB migration |
