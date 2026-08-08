---
tipo: plan
proyecto: EventHub
modulo: Operadores
fecha: 2026-08-04
estado: Completado
---

# Unify Operadores + CrewOperadores Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Merge `tbl_crew_operadores` into `tbl_operadores` — one table, one entity, event assignment via `eve_id` FK.

**Architecture:** Add `eve_id` (nullable FK) to `tbl_operadores`. Migrate data from `tbl_crew_operadores`. Drop `tbl_crew_operadores`. Update all code references: entity, DTOs, services, controllers, views.

**Tech Stack:** ASP.NET MVC 5, Entity Framework 6, SQL Server, Razor views

## Global Constraints

- .NET Framework 4.8.1, C# 7.0+
- Entity Framework 6 with Code First (attributes)
- SQL Server database `EventHubv01`
- Existing migration SQL scripts in `EventHub.03.Data/Migrations/` and `docs/sql/`

---

## File Structure

### Files to DELETE (7 files)
- `EventHub.03.Data\Entities\CrewOperador.cs`
- `EventHub.02.Bussines\DTOs\CrewOperadorDto.cs`
- `EventHub.02.Bussines\Services\ICrewService.cs`
- `EventHub.02.Bussines\Services\CrewService.cs`
- `EventHub.01.Web\Controllers\CrewController.cs`
- `EventHub.01.Web\Views\Crew\Index.cshtml`
- `EventHub.01.Web\Views\Crew\IndexGlobal.cshtml`

### Files to MODIFY (13 files)
- `EventHub.03.Data\Entities\Operador.cs` — add `EventoId` FK
- `EventHub.03.Data\Entities\Tarea.cs` — FK → Operador instead of CrewOperador
- `EventHub.03.Data\EventHubContext.cs` — remove `CrewOperadores` DbSet
- `EventHub.02.Bussines\DTOs\OperadorDto.cs` — merge CrewOperador fields, update `OperadorEventoDto`
- `EventHub.02.Bussines\DTOs\TareaDto.cs` — rename `CrewOperador*` → `Operador*`
- `EventHub.02.Bussines\Services\IOperadorService.cs` — update signatures
- `EventHub.02.Bussines\Services\OperadorService.cs` — absorb CrewService logic
- `EventHub.02.Bussines\Services\TareaService.cs` — update references
- `EventHub.01.Web\Controllers\EventosController.cs` — update task assignment + crew loading
- `EventHub.01.Web\Controllers\OperadoresController.cs` — update references
- `EventHub.01.Web\Views\Eventos\Tareas.cshtml` — rename CrewOperador* refs
- `EventHub.01.Web\Views\Eventos\Details.cshtml` — update crew list type
- `EventHub.01.Web\Views\Shared\_Layout.cshtml` — update nav link

### Files to CREATE (1 file)
- `EventHub.03.Data\Migrations\MergeOperadoresCrew.sql` — migration script

---

## Task 1: Database Migration Script

**Files:**
- Create: `EventHub.03.Data\Migrations\MergeOperadoresCrew.sql`

**Description:** Write the SQL migration that adds `eve_id` to `tbl_operadores`, migrates data from `tbl_crew_operadores`, and drops the old table.

- [ ] **Step 1: Create migration SQL**

```sql
-- Migration: Unify tbl_operadores + tbl_crew_operadores
-- Date: 2026-08-04

-- 1. Add event FK to tbl_operadores
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_operadores') AND name = 'ope_eve_id')
BEGIN
    ALTER TABLE tbl_operadores ADD ope_eve_id INT NULL;
    ALTER TABLE tbl_operadores ADD CONSTRAINT FK_operador_evento
        FOREIGN KEY (ope_eve_id) REFERENCES tbl_eventos(eve_id);
    CREATE INDEX IX_operador_evento ON tbl_operadores(ope_eve_id);
END
GO

-- 2. Add missing columns from CrewOperador to Operador (if not present)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_operadores') AND name = 'ope_num_cuenta')
    ALTER TABLE tbl_operadores ADD ope_num_cuenta NVARCHAR(50) NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_operadores') AND name = 'ope_banco')
    ALTER TABLE tbl_operadores ADD ope_banco NVARCHAR(100) NULL;
GO

-- 3. Migrate data: for each CrewOperador, update the linked Operador with event assignment
UPDATE o
SET o.ope_eve_id = c.cro_eve_id,
    o.ope_num_cuenta = c.cro_num_cuenta,
    o.ope_banco = c.cro_banco
FROM tbl_operadores o
INNER JOIN tbl_crew_operadores c ON o.ope_id = c.cro_ope_id
WHERE c.cro_ope_id IS NOT NULL;
GO

-- 4. For CrewOperadores without a linked Operador (operadorId IS NULL),
--    create a new Operador record and link it
INSERT INTO tbl_operadores (ope_nombre, ope_cedula, ope_email, ope_telefono, ope_rol, ope_estado, ope_fecha_creacion, ope_foto_url, ope_eve_id, ope_num_cuenta, ope_banco)
SELECT c.cro_nombre, c.cro_cedula, c.cro_email, c.cro_telefono, c.cro_rol, c.cro_estado, c.cro_fecha_creacion, c.cro_foto_url, c.cro_eve_id, c.cro_num_cuenta, c.cro_banco
FROM tbl_crew_operadores c
WHERE c.cro_ope_id IS NULL;
GO

-- 5. Update tbl_tareas FK to reference tbl_operadores instead of tbl_crew_operadores
--    First drop the old FK
DECLARE @fkName NVARCHAR(200);
SELECT @fkName = name FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('tbl_tareas')
  AND referenced_object_id = OBJECT_ID('tbl_crew_operadores');
IF @fkName IS NOT NULL
    EXEC('ALTER TABLE tbl_tareas DROP CONSTRAINT ' + @fkName);
GO

-- Rename column for clarity (optional, keeps backward compat)
-- tar_crew_operador_id will now reference ope_id
-- Update the values: CrewOperador.Id → the linked Operador.Id
UPDATE t
SET t.tar_crew_operador_id = o.ope_id
FROM tbl_tareas t
INNER JOIN tbl_crew_operadores c ON t.tar_crew_operador_id = c.cro_id
INNER JOIN tbl_operadores o ON c.cro_ope_id = o.ope_id
WHERE c.cro_ope_id IS NOT NULL;
GO

-- For tasks linked to CrewOperadores that were created without an Operador link,
-- the new Operador records were created above. Find them.
UPDATE t
SET t.tar_crew_operador_id = o.ope_id
FROM tbl_tareas t
INNER JOIN tbl_crew_operadores c ON t.tar_crew_operador_id = c.cro_id
INNER JOIN tbl_operadores o ON o.ope_eve_id = c.cro_eve_id AND o.ope_nombre = c.cro_nombre
WHERE c.cro_ope_id IS NULL;
GO

-- Add FK from tbl_tareas to tbl_operadores
ALTER TABLE tbl_tareas ADD CONSTRAINT FK_tarea_operador
    FOREIGN KEY (tar_crew_operador_id) REFERENCES tbl_operadores(ope_id);
GO

-- 6. Drop tbl_crew_operadores
-- (Uncomment after verifying data migration)
-- DROP TABLE tbl_crew_operadores;
-- GO

PRINT 'Migration complete. Verify data before dropping tbl_crew_operadores.';
GO
```

- [ ] **Step 2: Commit**

```bash
git add EventHub.03.Data/Migrations/MergeOperadoresCrew.sql
git commit -m "migration: add SQL script to unify operadores and crew_operadores"
```

---

## Task 2: Update Operador Entity

**Files:**
- Modify: `EventHub.03.Data\Entities\Operador.cs`

**Description:** Add `EventoId` FK, `NumeroCuenta`, `Banco` properties. Add navigation property to `Evento`.

- [ ] **Step 1: Update Operador entity**

Replace the full file content of `EventHub.03.Data\Entities\Operador.cs`:

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_operadores")]
    public class Operador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ope_id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("ope_nombre")]
        public string Nombre { get; set; }

        [MaxLength(20)]
        [Column("ope_cedula")]
        public string Cedula { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("ope_email")]
        public string Email { get; set; }

        [MaxLength(20)]
        [Column("ope_telefono")]
        public string Telefono { get; set; }

        [MaxLength(100)]
        [Column("ope_rol")]
        public string Rol { get; set; }

        [Column("ope_estado")]
        public bool Estado { get; set; } = true;

        [Column("ope_fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [MaxLength(500)]
        [Column("ope_foto_url")]
        public string FotoUrl { get; set; }

        [Column("ope_eve_id")]
        public int? EventoId { get; set; }

        [MaxLength(50)]
        [Column("ope_num_cuenta")]
        public string NumeroCuenta { get; set; }

        [MaxLength(100)]
        [Column("ope_banco")]
        public string Banco { get; set; }

        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add EventHub.03.Data/Entities/Operador.cs
git commit -m "refactor: add EventoId FK to Operador entity"
```

---

## Task 3: Update Tarea Entity

**Files:**
- Modify: `EventHub.03.Data\Entities\Tarea.cs`

**Description:** Change FK from `CrewOperador` to `Operador`. Rename property from `CrewOperadorId` to `OperadorId`.

- [ ] **Step 1: Update Tarea entity**

Replace `CrewOperadorId` / `CrewOperador` with `OperadorId` / `Operador`:

```csharp
// Lines 38-39: change from
[Column("tar_crew_operador_id")]
public int? CrewOperadorId { get; set; }
// to
[Column("tar_crew_operador_id")]
public int? OperadorId { get; set; }

// Lines 58-59: change from
[ForeignKey("CrewOperadorId")]
public virtual CrewOperador CrewOperador { get; set; }
// to
[ForeignKey("OperadorId")]
public virtual Operador Operador { get; set; }
```

- [ ] **Step 2: Commit**

```bash
git add EventHub.03.Data/Entities/Tarea.cs
git commit -m "refactor: Tarea FK now references Operador instead of CrewOperador"
```

---

## Task 4: Update EventHubContext

**Files:**
- Modify: `EventHub.03.Data\EventHubContext.cs`

**Description:** Remove `CrewOperadores` DbSet.

- [ ] **Step 1: Remove DbSet**

Delete line: `public DbSet<CrewOperador> CrewOperadores { get; set; }`

- [ ] **Step 2: Commit**

```bash
git add EventHub.03.Data/EventHubContext.cs
git commit -m "refactor: remove CrewOperadores DbSet from context"
```

---

## Task 5: Update DTOs

**Files:**
- Modify: `EventHub.02.Bussines\DTOs\OperadorDto.cs`
- Modify: `EventHub.02.Bussines\DTOs\TareaDto.cs`

**Description:** Merge CrewOperador fields into OperadorDto. Rename CrewOperador* in TareaDto to Operador*.

- [ ] **Step 1: Update OperadorDto.cs**

Add `EventoId` field to `OperadorDto`. Update `OperadorEventoDto` to remove `CrewOperadorId` (use `OperadorId` instead since the Operador IS the crew member now). Update comment.

```csharp
// In OperadorDto, add:
public int? EventoId { get; set; }

// In OperadorEventoDto, change:
// OLD: public int CrewOperadorId { get; set; }
// NEW: (remove CrewOperadorId, it's just Operador.Id now)
// Update comment:
/// Relacion operador → evento (ope_eve_id en tbl_operadores).
```

- [ ] **Step 2: Update TareaDto.cs**

Rename all `CrewOperador*` properties to `Operador*`:

```csharp
// In TareaDto:
// OLD: public int? CrewOperadorId { get; set; }
// NEW: public int? OperadorId { get; set; }
// OLD: public string CrewOperadorNombre { get; set; }
// NEW: public string OperadorNombre { get; set; }
// OLD: public string CrewOperadorEmail { get; set; }
// NEW: public string OperadorEmail { get; set; }

// In TareaFormDto:
// OLD: public int? CrewOperadorId { get; set; }
// NEW: public int? OperadorId { get; set; }
```

- [ ] **Step 3: Commit**

```bash
git add EventHub.02.Bussines/DTOs/OperadorDto.cs EventHub.02.Bussines/DTOs/TareaDto.cs
git commit -m "refactor: update DTOs to use Operador instead of CrewOperador"
```

---

## Task 6: Update IOperadorService + OperadorService

**Files:**
- Modify: `EventHub.02.Bussines\Services\IOperadorService.cs`
- Modify: `EventHub.02.Bussines\Services\OperadorService.cs`

**Description:** Absorb CrewService logic. Update `GetConEventos` to use Operador's own `EventoId`. Update `RemoverDeEvento` to clear `EventoId` instead of deleting a CrewOperador record. Add crew-by-event query.

- [ ] **Step 1: Update IOperadorService**

```csharp
// Change RemoverDeEvento signature:
// OLD: bool RemoverDeEvento(int crewOperadorId);
// NEW: bool RemoverDeEvento(int operadorId);

// Add:
List<OperadorDto> GetPorEvento(int eventoId);
```

- [ ] **Step 2: Update OperadorService**

Rewrite `GetConEventos` to use `Operador.EventoId` instead of querying `CrewOperadores`:

```csharp
public List<OperadorConEventosDto> GetConEventos()
{
    return _context.Operadores
        .Select(o => new OperadorConEventosDto
        {
            Id = o.Id,
            Nombre = o.Nombre,
            Cedula = o.Cedula,
            Email = o.Email,
            Telefono = o.Telefono,
            Rol = o.Rol,
            Estado = o.Estado,
            FechaCreacion = o.FechaCreacion,
            Eventos = o.EventoId.HasValue ? new List<OperadorEventoDto>
            {
                new OperadorEventoDto
                {
                    EventoId = o.Evento.Id,
                    EventoNombre = o.Evento.Nombre,
                    EventoCodigo = o.Evento.Codigo,
                    Estado = o.Estado,
                    Rol = o.Rol
                }
            } : new List<OperadorEventoDto>()
        })
        .ToList();
}
```

Rewrite `RemoverDeEvento`:

```csharp
public bool RemoverDeEvento(int operadorId)
{
    var operador = _context.Operadores.Find(operadorId);
    if (operador == null) return false;
    operador.EventoId = null;
    _context.SaveChanges();
    return true;
}
```

Add `GetPorEvento`:

```csharp
public List<OperadorDto> GetPorEvento(int eventoId)
{
    return _context.Operadores
        .Where(o => o.EventoId == eventoId && o.Estado)
        .Select(o => new OperadorDto
        {
            Id = o.Id,
            Nombre = o.Nombre,
            Cedula = o.Cedula,
            Email = o.Email,
            Telefono = o.Telefono,
            Rol = o.Rol,
            Estado = o.Estado,
            FechaCreacion = o.FechaCreacion,
            FotoUrl = o.FotoUrl,
            EventoId = o.EventoId
        })
        .ToList();
}
```

Remove all references to `CrewOperadores` from the file.

- [ ] **Step 3: Commit**

```bash
git add EventHub.02.Bussines/Services/IOperadorService.cs EventHub.02.Bussines/Services/OperadorService.cs
git commit -m "refactor: OperadorService absorbs CrewService logic"
```

---

## Task 7: Update TareaService

**Files:**
- Modify: `EventHub.02.Bussines\Services\TareaService.cs`

**Description:** Replace all `CrewOperadorId` / `CrewOperador` references with `OperadorId` / `Operador`.

- [ ] **Step 1: Find-and-replace in TareaService.cs**

Replace all occurrences:
- `t.CrewOperadorId` → `t.OperadorId`
- `t.CrewOperador` → `t.Operador`
- `nuevaTarea.CrewOperadorId` → `nuevaTarea.OperadorId`
- `nuevaTarea.CrewOperador` → `nuevaTarea.Operador`
- `dto.CrewOperadorId` → `dto.OperadorId`
- `CrewOperadorNombre` → `OperadorNombre`
- `CrewOperadorEmail` → `OperadorEmail`

There are ~12 occurrences across `ObtenerTareasPorEvento`, `CrearTarea`, and `ObtenerPorId`.

- [ ] **Step 2: Commit**

```bash
git add EventHub.02.Bussines/Services/TareaService.cs
git commit -m "refactor: TareaService uses Operador instead of CrewOperador"
```

---

## Task 8: Update EventosController

**Files:**
- Modify: `EventHub.01.Web\Controllers\EventosController.cs`

**Description:** Replace CrewService usage with OperadorService. Update task creation/editing to use `OperadorId`. Remove `EnsureCrewOperador`. Update crew loading in Details and Tareas actions.

- [ ] **Step 1: Update Details action (lines 68-72)**

```csharp
// OLD:
var crewService = new CrewService();
var crew = crewService.ObtenerCrewPorEvento(id);
ViewBag.CrewCount = crew.Count;
ViewBag.CrewList = crew.Take(6).ToList();

// NEW:
var operadorService = new OperadorService();
var crew = operadorService.GetPorEvento(id);
ViewBag.CrewCount = crew.Count;
ViewBag.CrewList = crew.Take(6).ToList();
```

- [ ] **Step 2: Update Tareas action (lines 286-289)**

Already updated to use `OperadorService.GetActivos()`. Verify it uses `OperadorDto` with `Id` and `Nombre`.

- [ ] **Step 3: Remove EnsureCrewOperador method (lines 291-319)**

Delete the entire `EnsureCrewOperador` method.

- [ ] **Step 4: Update CreateTareaAjax**

```csharp
// Remove the EnsureCrewOperador call block. The OperadorId from the dropdown
// is already the Operador.Id (from tbl_operadores), which is what Tarea.OperadorId needs.
// Simply pass model.OperadorId directly.
```

- [ ] **Step 5: Update EditTareaAjax**

Same: remove `EnsureCrewOperador` call. Use `model.OperadorId` directly.

- [ ] **Step 6: Update notification code**

Replace all `CrewOperadorId` / `CrewOperadorEmail` / `CrewOperadorNombre` with `OperadorId` / `OperadorEmail` / `OperadorNombre` in the notification blocks.

- [ ] **Step 7: Commit**

```bash
git add EventHub.01.Web/Controllers/EventosController.cs
git commit -m "refactor: EventosController uses Operador instead of CrewOperador"
```

---

## Task 9: Update OperadoresController

**Files:**
- Modify: `EventHub.01.Web\Controllers\OperadoresController.cs`

**Description:** Update `MisTareas` action to use `Operador` instead of `CrewOperador`. Update `RemoverDeEventoAjax`.

- [ ] **Step 1: Update MisTareas action**

```csharp
// OLD:
var crew = context.CrewOperadores.FirstOrDefault(c => c.Email == email);
if (crew == null) ...
var tareas = context.Tareas.Where(t => t.CrewOperadorId == crew.Id)...

// NEW:
var operador = context.Operadores.FirstOrDefault(o => o.Email == email);
if (operador == null) ...
var tareas = context.Tareas.Where(t => t.OperadorId == operador.Id)...
```

Also update the Select projection to use `OperadorId` / `OperadorNombre` / `Operador`.

- [ ] **Step 2: Update RemoverDeEventoAjax**

```csharp
// OLD:
public ActionResult RemoverDeEventoAjax(int crewOperadorId)
{
    var result = _operadorService.RemoverDeEvento(crewOperadorId);

// NEW:
public ActionResult RemoverDeEventoAjax(int operadorId)
{
    var result = _operadorService.RemoverDeEvento(operadorId);
```

- [ ] **Step 3: Commit**

```bash
git add EventHub.01.Web/Controllers/OperadoresController.cs
git commit -m "refactor: OperadoresController uses Operador instead of CrewOperador"
```

---

## Task 10: Update Views

**Files:**
- Modify: `EventHub.01.Web\Views\Eventos\Tareas.cshtml`
- Modify: `EventHub.01.Web\Views\Eventos\Details.cshtml`
- Modify: `EventHub.01.Web\Views\Shared\_Layout.cshtml`

**Description:** Rename all `CrewOperador*` references to `Operador*` in views. Update Details.cshtml crew list type. Update nav link.

- [ ] **Step 1: Update Tareas.cshtml**

Find-and-replace all occurrences:
- `CrewOperadorId` → `OperadorId`
- `CrewOperadorNombre` → `OperadorNombre`
- `formCrewOperadorId` → `formOperadorId`

The `@Html.DropDownList` already uses `ViewBag.Crew` which is now `SelectList` of `OperadorDto`.

- [ ] **Step 2: Update Details.cshtml**

```csharp
// OLD:
var crewList = ViewBag.CrewList as List<EventHub._02.Bussines.DTOs.CrewOperadorDto>;

// NEW:
var crewList = ViewBag.CrewList as List<EventHub._02.Bussines.DTOs.OperadorDto>;
```

- [ ] **Step 3: Update _Layout.cshtml nav link**

```html
<!-- OLD: -->
<a href="@Url.Action("Index", "Crew")" ... data-tooltip="Crew">
    <span class="nav-label">Crew</span>

<!-- NEW: -->
<a href="@Url.Action("Index", "Crew")" ... data-tooltip="Crew">
    <span class="nav-label">Crew</span>
```

The `CrewController.Index()` (no params) still works — it now calls `OperadorService.GetActivos()` and renders `IndexGlobal`. No change needed here if we keep the CrewController as a thin wrapper, OR we can change the link to `Operadores/Index` if we consolidate controllers.

Decision: Keep the `CrewController.Index()` (no params) route working since it's the "Crew" nav item. The controller internally uses OperadorService now.

- [ ] **Step 4: Commit**

```bash
git add EventHub.01.Web/Views/Eventos/Tareas.cshtml EventHub.01.Web/Views/Eventos/Details.cshtml EventHub.01.Web/Views/Shared/_Layout.cshtml
git commit -m "refactor: views use Operador instead of CrewOperador"
```

---

## Task 11: Delete CrewOperador Files

**Files:**
- Delete: `EventHub.03.Data\Entities\CrewOperador.cs`
- Delete: `EventHub.02.Bussines\DTOs\CrewOperadorDto.cs`
- Delete: `EventHub.02.Bussines\Services\ICrewService.cs`
- Delete: `EventHub.02.Bussines\Services\CrewService.cs`

**Description:** Remove all CrewOperador-specific files that are no longer needed.

- [ ] **Step 1: Delete files**

```bash
git rm EventHub.03.Data/Entities/CrewOperador.cs
git rm EventHub.02.Bussines/DTOs/CrewOperadorDto.cs
git rm EventHub.02.Bussines/Services/ICrewService.cs
git rm EventHub.02.Bussines/Services/CrewService.cs
```

- [ ] **Step 2: Commit**

```bash
git commit -m "refactor: delete CrewOperador entity, DTO, and service files"
```

---

## Task 12: Update CrewController + Views (Simplify)

**Files:**
- Modify: `EventHub.01.Web\Controllers\CrewController.cs`
- Modify: `EventHub.01.Web\Views\Crew\IndexGlobal.cshtml`

**Description:** Simplify `CrewController` to only use `OperadorService`. Remove all `CrewService` / `ICrewService` references. The `Index()` (no params) route still serves the global operadores list.

- [ ] **Step 1: Rewrite CrewController**

Remove `ICrewService` dependency. The `Index(int eventoId)` action should use `OperadorService.GetPorEvento()`. The `Index()` (no params) should use `OperadorService.GetActivos()`.

The `CrearCrewAjax` / `ActualizarCrewAjax` / `EliminarCrewAjax` / `ToggleEstadoAjax` actions should use `OperadorService` methods to create/update/delete operadores (setting `EventoId` when creating for a specific event).

`ObtenerCrewEventoAjax` → rename to `ObtenerOperadoresEventoAjax`, use `OperadorService.GetPorEvento()`.

`ObtenerOperadoresAjax` → already uses `OperadorService.GetActivos()`, no change.

`CrearOperadorAjax` → already uses `OperadorService.Create()`, no change.

- [ ] **Step 2: Update IndexGlobal.cshtml**

The view already works with `OperadorDto`. Update the `fetch` URL if action names changed.

- [ ] **Step 3: Commit**

```bash
git add EventHub.01.Web/Controllers/CrewController.cs EventHub.01.Web/Views/Crew/IndexGlobal.cshtml
git commit -m "refactor: CrewController simplified to use OperadorService only"
```

---

## Task 13: Verify Build + Test

**Files:** None (verification only)

- [ ] **Step 1: Build the solution**

```bash
# In Visual Studio or via MSBuild:
msbuild EventHub.v0.slnx /t:Build /p:Configuration=Debug
```

Expected: BUILD SUCCEEDED with 0 errors.

- [ ] **Step 2: Run the migration SQL against the database**

Execute `EventHub.03.Data/Migrations/MergeOperadoresCrew.sql` against the `EventHubv01` database.

- [ ] **Step 3: Manual smoke test**

1. Navigate to Dashboard → verify no crash
2. Navigate to Crew → verify operators list loads
3. Create an event → verify no crash
4. Go to event details → verify crew section shows operators
5. Go to Tareas → verify operator dropdown populates
6. Create a task with an operator assigned → verify no "Invalid Date"
7. Drag task between columns → verify status updates

- [ ] **Step 4: Final commit (if any fixes needed)**

```bash
git add -A
git commit -m "refactor: complete operadores/crew unification"
```
