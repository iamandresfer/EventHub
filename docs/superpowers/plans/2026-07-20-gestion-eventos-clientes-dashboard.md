# Gestión de Eventos, Clientes y Dashboard — Plan de Implementación

> **Para workers agente:** Usar subagent-driven-development (recomendado) o executing-plans. Tasks usan checkbox `- [ ]`.

**Objetivo:** Construir el módulo de gestión de eventos y clientes con dashboard, sidebar, entidades EF, servicios, controladores y vistas.

**Arquitectura:** Entidades EF manuales (patrón `Usuario.cs`) → DTOs → Servicios con interfaces → Controladores MVC con `[Authorize]` → Vistas Razor con layout sidebar.

**Tech Stack:** ASP.NET MVC 5.2.9, .NET Framework 4.8.1, Entity Framework 6, Bootstrap 5, SQL Server

## Global Constraints
- Nombres de entidades en inglés (Cliente, Evento, TipoEvento, Venue)
- Data Annotations para mapeo tabla-columna (`[Table]`, `[Column]`)
- Todos los servicios reciben `EventHubContext` por constructor
- No usar emojis como iconos (SVGs inline de Lucide)
- `cursor:pointer` en elementos clickeables y focus states visibles
- Sidebar lateral 240px, responsive con hamburguesa en móvil

---

### Task 1: Entidades EF (Cliente, TipoEvento, Venue, Evento)

**Files:**
- Create: `EventHub.03.Data/Entities/Cliente.cs`
- Create: `EventHub.03.Data/Entities/TipoEvento.cs`
- Create: `EventHub.03.Data/Entities/Venue.cs`
- Create: `EventHub.03.Data/Entities/Evento.cs`
- Modify: `EventHub.03.Data/EventHubContext.cs`

**Interfaces:**
- Produces: `EventHubContext` con DbSets de Cliente, Evento, Venue, TipoEvento

- [ ] **Step 1: Crear Cliente.cs**

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_clientes")]
    public class Cliente
    {
        [Key]
        [Column("cli_id")]
        public int Id { get; set; }

        [Required]
        [Column("cli_razon_social")]
        [MaxLength(150)]
        public string RazonSocial { get; set; }

        [Column("cli_nombre_comercial")]
        [MaxLength(150)]
        public string NombreComercial { get; set; }

        [Required]
        [Column("cli_ruc")]
        [MaxLength(13)]
        public string Ruc { get; set; }

        [Required]
        [Column("cli_email_principal")]
        [MaxLength(100)]
        public string EmailPrincipal { get; set; }

        [Column("cli_telefono_principal")]
        [MaxLength(20)]
        public string TelefonoPrincipal { get; set; }

        [Column("cli_direccion_fiscal")]
        [MaxLength(200)]
        public string DireccionFiscal { get; set; }

        [Column("cli_sitio_web")]
        [MaxLength(100)]
        public string SitioWeb { get; set; }

        [Column("cli_logo_url")]
        [MaxLength(500)]
        public string LogoUrl { get; set; }

        [Column("cli_clasificacion")]
        [MaxLength(20)]
        public string Clasificacion { get; set; }

        [Column("cli_notas_internas")]
        public string NotasInternas { get; set; }

        [Required]
        [Column("cli_fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [Column("cli_estado")]
        public bool Estado { get; set; }

        [Required]
        [Column("cli_usu_id_creador")]
        public int UsuarioCreadorId { get; set; }
    }
}
```

- [ ] **Step 2: Crear TipoEvento.cs**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_tipos_evento")]
    public class TipoEvento
    {
        [Key]
        [Column("tev_id")]
        public int Id { get; set; }

        [Required]
        [Column("tev_nombre")]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Column("tev_descripcion")]
        [MaxLength(200)]
        public string Descripcion { get; set; }
    }
}
```

- [ ] **Step 3: Crear Venue.cs**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_venues")]
    public class Venue
    {
        [Key]
        [Column("ven_id")]
        public int Id { get; set; }

        [Required]
        [Column("ven_nombre")]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [Column("ven_direccion")]
        [MaxLength(200)]
        public string Direccion { get; set; }

        [Required]
        [Column("ven_ciudad")]
        [MaxLength(50)]
        public string Ciudad { get; set; }

        [Column("ven_capacidad_maxima")]
        public int? CapacidadMaxima { get; set; }

        [Column("ven_estado")]
        [MaxLength(20)]
        public string Estado { get; set; }
    }
}
```

- [ ] **Step 4: Crear Evento.cs**

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_eventos")]
    public class Evento
    {
        [Key]
        [Column("eve_id")]
        public int Id { get; set; }

        [Required]
        [Column("eve_codigo")]
        [MaxLength(20)]
        public string Codigo { get; set; }

        [Required]
        [Column("eve_nombre")]
        [MaxLength(150)]
        public string Nombre { get; set; }

        [Required]
        [Column("eve_cli_id")]
        public int ClienteId { get; set; }

        [Required]
        [Column("eve_ven_id")]
        public int VenueId { get; set; }

        [Required]
        [Column("eve_tev_id")]
        public int TipoEventoId { get; set; }

        [Required]
        [Column("eve_fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Column("eve_fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Required]
        [Column("eve_hora_inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [Column("eve_hora_fin")]
        public TimeSpan HoraFin { get; set; }

        [Required]
        [Column("eve_presupuesto_estimado")]
        public decimal PresupuestoEstimado { get; set; }

        [Column("eve_gasto_real")]
        public decimal? GastoReal { get; set; }

        [Required]
        [Column("eve_estado")]
        [MaxLength(20)]
        public string Estado { get; set; }

        [Column("eve_descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Column("eve_fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("eve_fecha_cierre")]
        public DateTime? FechaCierre { get; set; }

        [Required]
        [Column("eve_usu_id_creador")]
        public int UsuarioCreadorId { get; set; }

        [Required]
        [Column("eve_usu_id_productor_general")]
        public int ProductorGeneralId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("VenueId")]
        public virtual Venue Venue { get; set; }

        [ForeignKey("TipoEventoId")]
        public virtual TipoEvento TipoEvento { get; set; }
    }
}
```

- [ ] **Step 5: Modificar EventHubContext.cs**

```csharp
public DbSet<Cliente> Clientes { get; set; }
public DbSet<Evento> Eventos { get; set; }
public DbSet<Venue> Venues { get; set; }
public DbSet<TipoEvento> TiposEvento { get; set; }
```

Colocar después de `public DbSet<Usuario> Usuarios { get; set; }` (línea 15).

- [ ] **Step 6: Compilar para verificar**

Build solution. No debe haber errores.

---

### Task 2: DTOs

**Files:**
- Create: `EventHub.02.Bussines/DTOs/ClienteDto.cs`
- Create: `EventHub.02.Bussines/DTOs/EventoListDto.cs`
- Create: `EventHub.02.Bussines/DTOs/EventoFormDto.cs`
- Create: `EventHub.02.Bussines/DTOs/DashboardDto.cs`

**Interfaces:**
- Produces: DTOs usados por servicios y controladores

- [ ] **Step 1: ClienteDto.cs**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }

        [Display(Name = "Nombre Comercial")]
        public string NombreComercial { get; set; }

        [Required(ErrorMessage = "El RUC es obligatorio.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El RUC debe tener 13 dígitos.")]
        [Display(Name = "RUC")]
        public string Ruc { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Email no válido.")]
        [Display(Name = "Email Principal")]
        public string EmailPrincipal { get; set; }

        [Display(Name = "Teléfono")]
        public string TelefonoPrincipal { get; set; }

        [Display(Name = "Dirección Fiscal")]
        public string DireccionFiscal { get; set; }

        [Display(Name = "Clasificación")]
        public string Clasificacion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }
    }
}
```

- [ ] **Step 2: EventoListDto.cs**

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class EventoListDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }

        [Display(Name = "Evento")]
        public string Nombre { get; set; }

        [Display(Name = "Cliente")]
        public string ClienteNombre { get; set; }

        [Display(Name = "Lugar")]
        public string VenueNombre { get; set; }

        [Display(Name = "Tipo")]
        public string TipoEvento { get; set; }

        [Display(Name = "Inicio")]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Presupuesto")]
        public decimal PresupuestoEstimado { get; set; }
    }
}
```

- [ ] **Step 3: EventoFormDto.cs**

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class EventoFormDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del evento es obligatorio.")]
        [Display(Name = "Nombre del Evento")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El lugar es obligatorio.")]
        [Display(Name = "Lugar")]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "El tipo de evento es obligatorio.")]
        [Display(Name = "Tipo de Evento")]
        public int TipoEventoId { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        [Display(Name = "Hora Inicio")]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        [Display(Name = "Hora Fin")]
        [DataType(DataType.Time)]
        public TimeSpan HoraFin { get; set; }

        [Required(ErrorMessage = "El presupuesto estimado es obligatorio.")]
        [Display(Name = "Presupuesto Estimado")]
        [DataType(DataType.Currency)]
        public decimal PresupuestoEstimado { get; set; }

        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
    }
}
```

- [ ] **Step 4: DashboardDto.cs**

```csharp
using System.Collections.Generic;
using EventHub._02.Bussines.DTOs;

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
    }
}
```

- [ ] **Step 5: Compilar**

---

### Task 3: Servicios de Negocio

**Files:**
- Create: `EventHub.02.Bussines/Services/IClienteService.cs`
- Create: `EventHub.02.Bussines/Services/ClienteService.cs`
- Create: `EventHub.02.Bussines/Services/IEventoService.cs`
- Create: `EventHub.02.Bussines/Services/EventoService.cs`
- Create: `EventHub.02.Bussines/Services/IVenueService.cs`
- Create: `EventHub.02.Bussines/Services/VenueService.cs`
- Create: `EventHub.02.Bussines/Services/ITipoEventoService.cs`
- Create: `EventHub.02.Bussines/Services/TipoEventoService.cs`

**Interfaces:**
- Consumes: `EventHubContext`, DTOs de Task 2, entidades de Task 1
- Produces: Servicios inyectables para controladores

- [ ] **Step 1: IClienteService.cs**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> GetAllAsync();
        Task<List<ClienteDto>> GetActiveAsync();
        Task<ClienteDto> GetByIdAsync(int id);
        Task<ClienteDto> CreateAsync(ClienteDto dto, int usuarioCreadorId);
        Task<ClienteDto> UpdateAsync(ClienteDto dto);
        Task<bool> DeactivateAsync(int id);
    }
}
```

- [ ] **Step 2: ClienteService.cs**

```csharp
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class ClienteService : IClienteService
    {
        private readonly EventHubContext _context;

        public ClienteService(EventHubContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteDto>> GetAllAsync()
        {
            return await _context.Clientes
                .OrderByDescending(c => c.Id)
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    RazonSocial = c.RazonSocial,
                    NombreComercial = c.NombreComercial,
                    Ruc = c.Ruc,
                    EmailPrincipal = c.EmailPrincipal,
                    TelefonoPrincipal = c.TelefonoPrincipal,
                    DireccionFiscal = c.DireccionFiscal,
                    Clasificacion = c.Clasificacion,
                    Estado = c.Estado
                })
                .ToListAsync();
        }

        public async Task<List<ClienteDto>> GetActiveAsync()
        {
            return await _context.Clientes
                .Where(c => c.Estado)
                .OrderBy(c => c.RazonSocial)
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    RazonSocial = c.RazonSocial,
                    Ruc = c.Ruc,
                    EmailPrincipal = c.EmailPrincipal,
                    Clasificacion = c.Clasificacion,
                    Estado = c.Estado
                })
                .ToListAsync();
        }

        public async Task<ClienteDto> GetByIdAsync(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            if (c == null) return null;

            return new ClienteDto
            {
                Id = c.Id,
                RazonSocial = c.RazonSocial,
                NombreComercial = c.NombreComercial,
                Ruc = c.Ruc,
                EmailPrincipal = c.EmailPrincipal,
                TelefonoPrincipal = c.TelefonoPrincipal,
                DireccionFiscal = c.DireccionFiscal,
                Clasificacion = c.Clasificacion,
                Estado = c.Estado
            };
        }

        public async Task<ClienteDto> CreateAsync(ClienteDto dto, int usuarioCreadorId)
        {
            var entity = new Cliente
            {
                RazonSocial = dto.RazonSocial,
                NombreComercial = dto.NombreComercial,
                Ruc = dto.Ruc,
                EmailPrincipal = dto.EmailPrincipal,
                TelefonoPrincipal = dto.TelefonoPrincipal,
                DireccionFiscal = dto.DireccionFiscal,
                Clasificacion = dto.Clasificacion ?? "Nuevo",
                FechaRegistro = DateTime.Now,
                Estado = true,
                UsuarioCreadorId = usuarioCreadorId
            };

            _context.Clientes.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.Estado = true;
            return dto;
        }

        public async Task<ClienteDto> UpdateAsync(ClienteDto dto)
        {
            var entity = await _context.Clientes.FindAsync(dto.Id);
            if (entity == null) return null;

            entity.RazonSocial = dto.RazonSocial;
            entity.NombreComercial = dto.NombreComercial;
            entity.Ruc = dto.Ruc;
            entity.EmailPrincipal = dto.EmailPrincipal;
            entity.TelefonoPrincipal = dto.TelefonoPrincipal;
            entity.DireccionFiscal = dto.DireccionFiscal;
            entity.Clasificacion = dto.Clasificacion;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await _context.Clientes.FindAsync(id);
            if (entity == null) return false;

            entity.Estado = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
```

- [ ] **Step 3: IEventoService.cs**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface IEventoService
    {
        Task<List<EventoListDto>> GetAllAsync();
        Task<EventoListDto> GetByIdAsync(int id);
        Task<EventoListDto> CreateAsync(EventoFormDto dto, int usuarioCreadorId);
        Task<EventoListDto> UpdateAsync(int id, EventoFormDto dto);
        Task<bool> UpdateEstadoAsync(int id, string estado);
        Task<DashboardDto> GetDashboardAsync();
    }
}
```

- [ ] **Step 4: EventoService.cs**

```csharp
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class EventoService : IEventoService
    {
        private readonly EventHubContext _context;

        public EventoService(EventHubContext context)
        {
            _context = context;
        }

        public async Task<List<EventoListDto>> GetAllAsync()
        {
            return await _context.Eventos
                .Include(e => e.Cliente)
                .Include(e => e.Venue)
                .Include(e => e.TipoEvento)
                .OrderByDescending(e => e.FechaCreacion)
                .Select(e => new EventoListDto
                {
                    Id = e.Id,
                    Codigo = e.Codigo,
                    Nombre = e.Nombre,
                    ClienteNombre = e.Cliente.RazonSocial,
                    VenueNombre = e.Venue.Nombre,
                    TipoEvento = e.TipoEvento.Nombre,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    Estado = e.Estado,
                    PresupuestoEstimado = e.PresupuestoEstimado
                })
                .ToListAsync();
        }

        public async Task<EventoListDto> GetByIdAsync(int id)
        {
            return await _context.Eventos
                .Include(e => e.Cliente)
                .Include(e => e.Venue)
                .Include(e => e.TipoEvento)
                .Where(e => e.Id == id)
                .Select(e => new EventoListDto
                {
                    Id = e.Id,
                    Codigo = e.Codigo,
                    Nombre = e.Nombre,
                    ClienteNombre = e.Cliente.RazonSocial,
                    VenueNombre = e.Venue.Nombre,
                    TipoEvento = e.TipoEvento.Nombre,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    Estado = e.Estado,
                    PresupuestoEstimado = e.PresupuestoEstimado
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EventoListDto> CreateAsync(EventoFormDto dto, int usuarioCreadorId)
        {
            var codigo = await GenerarCodigoAsync();

            var entity = new Evento
            {
                Codigo = codigo,
                Nombre = dto.Nombre,
                ClienteId = dto.ClienteId,
                VenueId = dto.VenueId,
                TipoEventoId = dto.TipoEventoId,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                PresupuestoEstimado = dto.PresupuestoEstimado,
                Estado = "Planificacion",
                Descripcion = dto.Descripcion,
                FechaCreacion = DateTime.Now,
                UsuarioCreadorId = usuarioCreadorId,
                ProductorGeneralId = usuarioCreadorId
            };

            _context.Eventos.Add(entity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<EventoListDto> UpdateAsync(int id, EventoFormDto dto)
        {
            var entity = await _context.Eventos.FindAsync(id);
            if (entity == null) return null;

            entity.Nombre = dto.Nombre;
            entity.ClienteId = dto.ClienteId;
            entity.VenueId = dto.VenueId;
            entity.TipoEventoId = dto.TipoEventoId;
            entity.FechaInicio = dto.FechaInicio;
            entity.FechaFin = dto.FechaFin;
            entity.HoraInicio = dto.HoraInicio;
            entity.HoraFin = dto.HoraFin;
            entity.PresupuestoEstimado = dto.PresupuestoEstimado;
            entity.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> UpdateEstadoAsync(int id, string estado)
        {
            var entity = await _context.Eventos.FindAsync(id);
            if (entity == null) return false;

            entity.Estado = estado;
            if (estado == "Finalizado" || estado == "Cancelado")
                entity.FechaCierre = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

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
                        ClienteNombre = e.Cliente.RazonSocial,
                        VenueNombre = e.Venue.Nombre,
                        FechaInicio = e.FechaInicio,
                        Estado = e.Estado
                    })
                    .ToListAsync()
            };
        }

        private async Task<string> GenerarCodigoAsync()
        {
            var count = await _context.Eventos.CountAsync();
            return $"EVT-{(count + 1):D5}";
        }
    }
}
```

- [ ] **Step 5: IVenueService.cs**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public interface IVenueService
    {
        Task<List<Venue>> GetAvailableAsync();
    }
}
```

- [ ] **Step 6: VenueService.cs**

```csharp
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class VenueService : IVenueService
    {
        private readonly EventHubContext _context;

        public VenueService(EventHubContext context)
        {
            _context = context;
        }

        public async Task<List<Venue>> GetAvailableAsync()
        {
            return await _context.Venues
                .Where(v => v.Estado == "Disponible")
                .OrderBy(v => v.Nombre)
                .ToListAsync();
        }
    }
}
```

- [ ] **Step 7: ITipoEventoService.cs**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public interface ITipoEventoService
    {
        Task<List<TipoEvento>> GetAllAsync();
    }
}
```

- [ ] **Step 8: TipoEventoService.cs**

```csharp
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class TipoEventoService : ITipoEventoService
    {
        private readonly EventHubContext _context;

        public TipoEventoService(EventHubContext context)
        {
            _context = context;
        }

        public async Task<List<TipoEvento>> GetAllAsync()
        {
            return await _context.TiposEvento
                .OrderBy(t => t.Nombre)
                .ToListAsync();
        }
    }
}
```

- [ ] **Step 9: Compilar**

---

### Task 4: Layout con Sidebar

**Files:**
- Modify: `EventHub.01.Web/Views/Shared/_Layout.cshtml` (reescribir completamente)
- Modify: `EventHub.01.Web/Content/Site.css` (agregar estilos del sidebar)

- [ ] **Step 1: Reescribir _Layout.cshtml**

```html
@using System.Web.Optimization
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - EventProduction Hub</title>
    @Styles.Render("~/Content/css")
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --sidebar-width: 260px;
            --primary: #2563EB;
            --primary-hover: #1d4ed8;
            --primary-light: #eff6ff;
            --accent: #059669;
            --bg: #f8fafc;
            --bg-card: #ffffff;
            --text: #0f172a;
            --text-muted: #64748b;
            --border: #e2e8f0;
            --sidebar-bg: #1e293b;
            --sidebar-text: #cbd5e1;
            --sidebar-hover: #334155;
            --sidebar-active: #2563EB;
        }

        * { margin: 0; padding: 0; box-sizing: border-box; }

        body {
            font-family: 'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: var(--bg);
            color: var(--text);
            min-height: 100vh;
            display: flex;
        }

        .sidebar {
            width: var(--sidebar-width);
            background: var(--sidebar-bg);
            color: var(--sidebar-text);
            min-height: 100vh;
            position: fixed;
            left: 0;
            top: 0;
            display: flex;
            flex-direction: column;
            z-index: 1000;
            transition: transform 0.3s ease;
        }

        .sidebar-header {
            padding: 20px;
            border-bottom: 1px solid rgba(255,255,255,0.1);
        }

        .sidebar-header h2 {
            font-size: 18px;
            font-weight: 700;
            color: #fff;
            margin: 0;
        }

        .sidebar-header p {
            font-size: 12px;
            color: var(--sidebar-text);
            margin: 2px 0 0 0;
        }

        .sidebar-nav {
            flex: 1;
            padding: 12px 0;
            overflow-y: auto;
        }

        .sidebar-nav a {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 10px 20px;
            color: var(--sidebar-text);
            text-decoration: none;
            font-size: 14px;
            font-weight: 500;
            transition: all 0.15s;
        }

        .sidebar-nav a:hover {
            background: var(--sidebar-hover);
            color: #fff;
        }

        .sidebar-nav a.active {
            background: var(--sidebar-active);
            color: #fff;
        }

        .sidebar-nav a svg {
            width: 20px;
            height: 20px;
            flex-shrink: 0;
        }

        .sidebar-nav .nav-label {
            flex: 1;
        }

        .sidebar-footer {
            padding: 16px 20px;
            border-top: 1px solid rgba(255,255,255,0.1);
            font-size: 13px;
        }

        .sidebar-footer .user-name {
            color: #fff;
            font-weight: 600;
        }

        .sidebar-footer a {
            display: flex;
            align-items: center;
            gap: 8px;
            color: var(--sidebar-text);
            text-decoration: none;
            margin-top: 8px;
            font-size: 13px;
            transition: color 0.15s;
        }

        .sidebar-footer a:hover { color: #fff; }

        .main-content {
            margin-left: var(--sidebar-width);
            flex: 1;
            padding: 24px 32px;
            min-height: 100vh;
        }

        .mobile-toggle {
            display: none;
            position: fixed;
            top: 12px;
            left: 12px;
            z-index: 1100;
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: 8px;
            width: 40px;
            height: 40px;
            cursor: pointer;
            align-items: center;
            justify-content: center;
        }

        .sidebar-overlay {
            display: none;
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,0.5);
            z-index: 999;
        }

        @@media (max-width: 768px) {
            .sidebar {
                transform: translateX(-100%);
            }
            .sidebar.open {
                transform: translateX(0);
            }
            .main-content {
                margin-left: 0;
                padding: 20px 16px;
                padding-top: 60px;
            }
            .mobile-toggle {
                display: flex;
            }
            .sidebar-overlay.show {
                display: block;
            }
        }

        .page-header {
            margin-bottom: 24px;
        }

        .page-header h1 {
            font-size: 24px;
            font-weight: 700;
            color: var(--text);
            margin: 0;
        }

        .page-header p {
            font-size: 14px;
            color: var(--text-muted);
            margin: 4px 0 0 0;
        }

        .card {
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.05);
        }

        .table {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
        }

        .table th {
            text-align: left;
            padding: 12px 16px;
            font-weight: 600;
            color: var(--text-muted);
            border-bottom: 2px solid var(--border);
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .table td {
            padding: 12px 16px;
            border-bottom: 1px solid var(--border);
            vertical-align: middle;
        }

        .table tr:hover td {
            background: var(--primary-light);
        }

        .btn {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 10px 20px;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            border: none;
            text-decoration: none;
            transition: all 0.15s;
        }

        .btn-primary {
            background: var(--primary);
            color: #fff;
        }

        .btn-primary:hover {
            background: var(--primary-hover);
        }

        .btn-outline {
            background: transparent;
            border: 1.5px solid var(--border);
            color: var(--text);
        }

        .btn-outline:hover {
            border-color: var(--primary);
            color: var(--primary);
        }

        .btn-sm {
            padding: 6px 12px;
            font-size: 13px;
            gap: 4px;
        }

        .btn-danger {
            background: #dc2626;
            color: #fff;
        }

        .btn-danger:hover {
            background: #b91c1c;
        }

        .badge {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
        }

        .badge-planificacion { background: #dbeafe; color: #1d4ed8; }
        .badge-preproduccion { background: #fef3c7; color: #d97706; }
        .badge-ejecucion { background: #d1fae5; color: #059669; }
        .badge-finalizado { background: #e2e8f0; color: #475569; }
        .badge-cancelado { background: #fee2e2; color: #dc2626; }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 16px;
            margin-bottom: 24px;
        }

        .stat-card {
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: 12px;
            padding: 20px;
        }

        .stat-card .stat-value {
            font-size: 28px;
            font-weight: 700;
            color: var(--primary);
        }

        .stat-card .stat-label {
            font-size: 13px;
            color: var(--text-muted);
            margin-top: 4px;
        }

        .form-control {
            width: 100%;
            padding: 10px 14px;
            background: var(--bg-card);
            border: 1.5px solid var(--border);
            border-radius: 8px;
            font-size: 14px;
            font-family: inherit;
            color: var(--text);
            transition: border-color 0.15s;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
        }

        select.form-control {
            cursor: pointer;
        }

        textarea.form-control {
            min-height: 80px;
            resize: vertical;
        }

        .form-group {
            margin-bottom: 16px;
        }

        .form-group label {
            display: block;
            font-size: 13px;
            font-weight: 600;
            color: var(--text);
            margin-bottom: 4px;
        }

        .field-error {
            color: #dc2626;
            font-size: 12px;
            margin-top: 4px;
            display: block;
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
        }

        @@media (max-width: 600px) {
            .form-row {
                grid-template-columns: 1fr;
            }
        }

        .action-bar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 16px;
            margin-bottom: 20px;
        }

        .search-box {
            display: flex;
            gap: 8px;
            align-items: center;
        }

        .search-box input {
            width: 240px;
        }

        .mt-4 { margin-top: 16px; }
        .mb-4 { margin-bottom: 16px; }
        .text-right { text-align: right; }
        .text-muted { color: var(--text-muted); }
        .w-full { width: 100%; }
        .flex { display: flex; }
        .gap-2 { gap: 8px; }
        .gap-4 { gap: 16px; }
        .items-center { align-items: center; }
        .justify-between { justify-content: space-between; }
        @@media (max-width: 600px) { .action-bar { flex-direction: column; align-items: stretch; } .search-box input { width: 100%; } }
    </style>
</head>
<body>
    <button class="mobile-toggle" id="mobileToggle" aria-label="Abrir menú">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/>
        </svg>
    </button>
    <div class="sidebar-overlay" id="sidebarOverlay"></div>

    <aside class="sidebar" id="sidebar">
        <div class="sidebar-header">
            <h2>EventProduction Hub</h2>
            <p>Gestión de Eventos</p>
        </div>
        <nav class="sidebar-nav">
            <a href="@Url.Action("Index", "Home")" class="@(ViewContext.RouteData.Values["Action"]?.ToString() == "Index" && ViewContext.RouteData.Values["Controller"]?.ToString() == "Home" ? "active" : "")">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/></svg>
                <span class="nav-label">Dashboard</span>
            </a>
            <a href="@Url.Action("Index", "Eventos")" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "Eventos" ? "active" : "")">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
                <span class="nav-label">Eventos</span>
            </a>
            <a href="@Url.Action("Index", "Clientes")" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "Clientes" ? "active" : "")">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
                <span class="nav-label">Clientes</span>
            </a>
        </nav>
        <div class="sidebar-footer">
            <div class="user-name">@User.Identity.Name</div>
            @using (Html.BeginForm("Logout", "Auth", FormMethod.Post, new { style = "margin:0" }))
            {
                @Html.AntiForgeryToken()
                <button type="submit" style="background:none;border:none;cursor:pointer;display:flex;align-items:center;gap:8px;color:var(--sidebar-text);font-size:13px;font-family:inherit;padding:0;margin-top:8px;width:100%;">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/><polyline points="16 17 21 12 16 7"/><line x1="21" y1="12" x2="9" y2="12"/></svg>
                    Cerrar Sesión
                </button>
            }
        </div>
    </aside>

    <main class="main-content">
        @RenderBody()
    </main>

    <script>
        var sidebar = document.getElementById('sidebar');
        var overlay = document.getElementById('sidebarOverlay');
        document.getElementById('mobileToggle').addEventListener('click', function() {
            sidebar.classList.toggle('open');
            overlay.classList.toggle('show');
        });
        overlay.addEventListener('click', function() {
            sidebar.classList.remove('open');
            overlay.classList.remove('show');
        });
    </script>

    @Scripts.Render("~/bundles/jquery")
    @Scripts.Render("~/bundles/bootstrap")
    @RenderSection("scripts", required: false)
</body>
</html>
```

- [ ] **Step 2: Compilar y verificar**

---

### Task 5: Controladores (EventosController, ClientesController)

**Files:**
- Create: `EventHub.01.Web/Controllers/EventosController.cs`
- Create: `EventHub.01.Web/Controllers/ClientesController.cs`
- Modify: `EventHub.01.Web/Controllers/HomeController.cs` (actualizar Index para dashboard)

- [ ] **Step 1: EventosController.cs**

```csharp
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class EventosController : Controller
    {
        private readonly IEventoService _eventoService;
        private readonly IClienteService _clienteService;
        private readonly IVenueService _venueService;
        private readonly ITipoEventoService _tipoEventoService;

        public EventosController()
        {
            var context = new EventHubContext();
            _eventoService = new EventoService(context);
            _clienteService = new ClienteService(context);
            _venueService = new VenueService(context);
            _tipoEventoService = new TipoEventoService(context);
        }

        public async Task<ActionResult> Index(string search, string estado)
        {
            var eventos = await _eventoService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                eventos = eventos.FindAll(e =>
                    e.Nombre.ToLower().Contains(s) ||
                    e.Codigo.ToLower().Contains(s) ||
                    e.ClienteNombre.ToLower().Contains(s));
            }

            if (!string.IsNullOrEmpty(estado))
                eventos = eventos.FindAll(e => e.Estado == estado);

            ViewBag.Search = search;
            ViewBag.Estado = estado;
            return View(eventos);
        }

        public async Task<ActionResult> Details(int id)
        {
            var evento = await _eventoService.GetByIdAsync(id);
            if (evento == null) return HttpNotFound();
            return View(evento);
        }

        public async Task<ActionResult> Create()
        {
            var context = new EventHubContext();
            ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "RazonSocial");
            ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre");
            ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre");
            return View(new EventoFormDto
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(8, 0, 0),
                HoraFin = new TimeSpan(18, 0, 0)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EventoFormDto model)
        {
            var context = new EventHubContext();
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "RazonSocial", model.ClienteId);
                ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre", model.VenueId);
                ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre", model.TipoEventoId);
                return View(model);
            }

            var userId = GetUserId();
            var result = await _eventoService.CreateAsync(model, userId);

            TempData["SuccessMessage"] = "Evento creado exitosamente.";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            var evento = await _eventoService.GetByIdAsync(id);
            if (evento == null) return HttpNotFound();

            var context = new EventHubContext();
            var model = new EventoFormDto
            {
                Id = evento.Id,
                Nombre = evento.Nombre,
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
                PresupuestoEstimado = evento.PresupuestoEstimado
            };

            ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "RazonSocial");
            ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre");
            ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EventoFormDto model)
        {
            if (!ModelState.IsValid)
            {
                var context = new EventHubContext();
                ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "RazonSocial", model.ClienteId);
                ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre", model.VenueId);
                ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre", model.TipoEventoId);
                return View(model);
            }

            var result = await _eventoService.UpdateAsync(id, model);
            if (result == null) return HttpNotFound();

            TempData["SuccessMessage"] = "Evento actualizado exitosamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancelar(int id)
        {
            await _eventoService.UpdateEstadoAsync(id, "Cancelado");
            TempData["SuccessMessage"] = "Evento cancelado.";
            return RedirectToAction("Index");
        }

        private int GetUserId()
        {
            var data = System.Web.Security.FormsAuthentication.Decrypt(
                Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName]?.Value ?? "")?.UserData;
            if (!string.IsNullOrEmpty(data))
            {
                var parts = data.Split('|');
                if (parts.Length > 0 && int.TryParse(parts[0], out var id))
                    return id;
            }
            return 0;
        }
    }
}
```

- [ ] **Step 2: ClientesController.cs**

```csharp
using System.Threading.Tasks;
using System.Web.Mvc;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController()
        {
            var context = new EventHubContext();
            _clienteService = new ClienteService(context);
        }

        public async Task<ActionResult> Index(string search)
        {
            var clientes = await _clienteService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                clientes = clientes.FindAll(c =>
                    c.RazonSocial.ToLower().Contains(s) ||
                    c.Ruc.Contains(s) ||
                    c.EmailPrincipal.ToLower().Contains(s));
            }

            ViewBag.Search = search;
            return View(clientes);
        }

        public ActionResult Create()
        {
            return View(new ClienteDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClienteDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetUserId();
            await _clienteService.CreateAsync(model, userId);

            TempData["SuccessMessage"] = "Cliente creado exitosamente.";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null) return HttpNotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ClienteDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _clienteService.UpdateAsync(model);
            TempData["SuccessMessage"] = "Cliente actualizado exitosamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Desactivar(int id)
        {
            await _clienteService.DeactivateAsync(id);
            TempData["SuccessMessage"] = "Cliente desactivado.";
            return RedirectToAction("Index");
        }

        private int GetUserId()
        {
            var data = System.Web.Security.FormsAuthentication.Decrypt(
                Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName]?.Value ?? "")?.UserData;
            if (!string.IsNullOrEmpty(data))
            {
                var parts = data.Split('|');
                if (parts.Length > 0 && int.TryParse(parts[0], out var id))
                    return id;
            }
            return 0;
        }
    }
}
```

- [ ] **Step 3: Modificar HomeController.cs — Index**

```csharp
using System.Threading.Tasks;
using System.Web.Mvc;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var context = new EventHubContext();
            var eventoService = new EventoService(context);
            var dashboard = await eventoService.GetDashboardAsync();
            return View(dashboard);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
```

- [ ] **Step 4: Compilar**

---

### Task 6: Vistas de Eventos

**Files:**
- Create: `EventHub.01.Web/Views/Eventos/Index.cshtml`
- Create: `EventHub.01.Web/Views/Eventos/Details.cshtml`
- Create: `EventHub.01.Web/Views/Eventos/Create.cshtml`
- Create: `EventHub.01.Web/Views/Eventos/Edit.cshtml`

- [ ] **Step 1: Views/Eventos/Index.cshtml**

```html
@model List<EventHub._02.Bussines.DTOs.EventoListDto>
@{
    ViewBag.Title = "Eventos";
}

<div class="page-header">
    <h1>Eventos</h1>
    <p>Gestión de eventos de producción</p>
</div>

@if (TempData["SuccessMessage"] != null)
{
    <div style="background:#d1fae5;border:1px solid #059669;border-radius:8px;padding:12px 16px;color:#065f46;font-size:14px;margin-bottom:16px;">
        @TempData["SuccessMessage"]
    </div>
}

<div class="action-bar">
    <div class="search-box">
        <form method="get" class="flex gap-2 items-center">
            <input type="text" name="search" class="form-control" placeholder="Buscar eventos..." value="@ViewBag.Search" style="width:240px" />
            <select name="estado" class="form-control" style="width:auto;">
                <option value="">Todos los estados</option>
                <option value="Planificacion" @(ViewBag.Estado == "Planificacion" ? "selected" : "")>Planificación</option>
                <option value="PreProduccion" @(ViewBag.Estado == "PreProduccion" ? "selected" : "")>Pre-Producción</option>
                <option value="Ejecucion" @(ViewBag.Estado == "Ejecucion" ? "selected" : "")>Ejecución</option>
                <option value="Finalizado" @(ViewBag.Estado == "Finalizado" ? "selected" : "")>Finalizado</option>
                <option value="Cancelado" @(ViewBag.Estado == "Cancelado" ? "selected" : "")>Cancelado</option>
            </select>
            <button type="submit" class="btn btn-primary btn-sm">Buscar</button>
        </form>
    </div>
    <a href="@Url.Action("Create")" class="btn btn-primary">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
        Nuevo Evento
    </a>
</div>

<div class="card" style="padding:0;overflow:hidden;">
    <table class="table">
        <thead>
            <tr>
                <th>Código</th>
                <th>Evento</th>
                <th>Cliente</th>
                <th>Lugar</th>
                <th>Fecha Inicio</th>
                <th>Estado</th>
                <th>Presupuesto</th>
                <th style="text-align:right;">Acciones</th>
            </tr>
        </thead>
        <tbody>
            @if (Model.Count == 0)
            {
                <tr><td colspan="8" style="text-align:center;padding:40px;color:var(--text-muted);">No hay eventos registrados</td></tr>
            }
            @foreach (var item in Model)
            {
                <tr>
                    <td style="font-weight:600;font-size:13px;">@item.Codigo</td>
                    <td><a href="@Url.Action("Details", new { id = item.Id })" style="color:var(--primary);text-decoration:none;font-weight:600;">@item.Nombre</a></td>
                    <td>@item.ClienteNombre</td>
                    <td>@item.VenueNombre</td>
                    <td>@item.FechaInicio.ToString("dd/MM/yyyy")</td>
                    <td><span class="badge badge-@item.Estado.ToLower()">@item.Estado</span></td>
                    <td>@item.PresupuestoEstimado.ToString("C2")</td>
                    <td style="text-align:right;">
                        <a href="@Url.Action("Details", new { id = item.Id })" class="btn btn-outline btn-sm">Ver</a>
                        @if (item.Estado != "Finalizado" && item.Estado != "Cancelado")
                        {
                            <a href="@Url.Action("Edit", new { id = item.Id })" class="btn btn-outline btn-sm">Editar</a>
                        }
                    </td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

- [ ] **Step 2: Views/Eventos/Details.cshtml**

```html
@model EventHub._02.Bussines.DTOs.EventoListDto
@{
    ViewBag.Title = Model.Nombre;
}

<div class="page-header">
    <h1>@Model.Nombre</h1>
    <p>Código: @Model.Codigo</p>
</div>

<div class="card mb-4">
    <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px;">
        <div>
            <div class="text-muted" style="font-size:12px;font-weight:600;text-transform:uppercase;">Cliente</div>
            <div style="font-weight:600;">@Model.ClienteNombre</div>
        </div>
        <div>
            <div class="text-muted" style="font-size:12px;font-weight:600;text-transform:uppercase;">Lugar</div>
            <div style="font-weight:600;">@Model.VenueNombre</div>
        </div>
        <div>
            <div class="text-muted" style="font-size:12px;font-weight:600;text-transform:uppercase;">Tipo de Evento</div>
            <div style="font-weight:600;">@Model.TipoEvento</div>
        </div>
        <div>
            <div class="text-muted" style="font-size:12px;font-weight:600;text-transform:uppercase;">Estado</div>
            <span class="badge badge-@Model.Estado.ToLower()">@Model.Estado</span>
        </div>
        <div>
            <div class="text-muted" style="font-size:12px;font-weight:600;text-transform:uppercase;">Fecha Inicio</div>
            <div style="font-weight:600;">@Model.FechaInicio.ToString("dd/MM/yyyy")</div>
        </div>
        <div>
            <div class="text-muted" style="font-size:12px;font-weight:600;text-transform:uppercase;">Fecha Fin</div>
            <div style="font-weight:600;">@Model.FechaFin.ToString("dd/MM/yyyy")</div>
        </div>
        <div>
            <div class="text-muted" style="font-size:12px;font-weight:600;text-transform:uppercase;">Presupuesto</div>
            <div style="font-weight:600;">@Model.PresupuestoEstimado.ToString("C2")</div>
        </div>
    </div>
</div>

<div style="display:flex;gap:8px;">
    <a href="@Url.Action("Index")" class="btn btn-outline">Volver</a>
    @if (Model.Estado != "Finalizado" && Model.Estado != "Cancelado")
    {
        <a href="@Url.Action("Edit", new { id = Model.Id })" class="btn btn-primary">Editar</a>
    }
</div>
```

- [ ] **Step 3: Views/Eventos/Create.cshtml**

```html
@model EventHub._02.Bussines.DTOs.EventoFormDto
@{
    ViewBag.Title = "Nuevo Evento";
}

<div class="page-header">
    <h1>Nuevo Evento</h1>
    <p>Ingrese los datos del evento</p>
</div>

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()

    <div class="card">
        <div class="form-group">
            @Html.LabelFor(m => m.Nombre)
            @Html.TextBoxFor(m => m.Nombre, new { @class = "form-control", placeholder = "Nombre del evento" })
            @Html.ValidationMessageFor(m => m.Nombre, "", new { @class = "field-error" })
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.ClienteId)
                @Html.DropDownListFor(m => m.ClienteId, (SelectList)ViewBag.Clientes, "-- Seleccione --", new { @class = "form-control" })
                @Html.ValidationMessageFor(m => m.ClienteId, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.TipoEventoId)
                @Html.DropDownListFor(m => m.TipoEventoId, (SelectList)ViewBag.TiposEvento, "-- Seleccione --", new { @class = "form-control" })
                @Html.ValidationMessageFor(m => m.TipoEventoId, "", new { @class = "field-error" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.VenueId)
            @Html.DropDownListFor(m => m.VenueId, (SelectList)ViewBag.Venues, "-- Seleccione --", new { @class = "form-control" })
            @Html.ValidationMessageFor(m => m.VenueId, "", new { @class = "field-error" })
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.FechaInicio)
                @Html.TextBoxFor(m => m.FechaInicio, "{0:yyyy-MM-dd}", new { @class = "form-control", type = "date" })
                @Html.ValidationMessageFor(m => m.FechaInicio, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.FechaFin)
                @Html.TextBoxFor(m => m.FechaFin, "{0:yyyy-MM-dd}", new { @class = "form-control", type = "date" })
                @Html.ValidationMessageFor(m => m.FechaFin, "", new { @class = "field-error" })
            </div>
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.HoraInicio)
                @Html.TextBoxFor(m => m.HoraInicio, "{0:HH:mm}", new { @class = "form-control", type = "time" })
                @Html.ValidationMessageFor(m => m.HoraInicio, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.HoraFin)
                @Html.TextBoxFor(m => m.HoraFin, "{0:HH:mm}", new { @class = "form-control", type = "time" })
                @Html.ValidationMessageFor(m => m.HoraFin, "", new { @class = "field-error" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.PresupuestoEstimado)
            @Html.TextBoxFor(m => m.PresupuestoEstimado, "{0:F2}", new { @class = "form-control", type = "number", step = "0.01", min = "0" })
            @Html.ValidationMessageFor(m => m.PresupuestoEstimado, "", new { @class = "field-error" })
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.Descripcion)
            @Html.TextAreaFor(m => m.Descripcion, new { @class = "form-control", placeholder = "Descripción del evento..." })
        </div>
    </div>

    <div style="display:flex;gap:8px;margin-top:16px;">
        <button type="submit" class="btn btn-primary">Crear Evento</button>
        <a href="@Url.Action("Index")" class="btn btn-outline">Cancelar</a>
    </div>
}

@section scripts {
    @Scripts.Render("~/bundles/jqueryval")
}
```

- [ ] **Step 4: Views/Eventos/Edit.cshtml**

```html
@model EventHub._02.Bussines.DTOs.EventoFormDto
@{
    ViewBag.Title = "Editar Evento";
}

<div class="page-header">
    <h1>Editar Evento</h1>
    <p>Modifique los datos del evento</p>
</div>

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
    @Html.HiddenFor(m => m.Id)

    <div class="card">
        <div class="form-group">
            @Html.LabelFor(m => m.Nombre)
            @Html.TextBoxFor(m => m.Nombre, new { @class = "form-control" })
            @Html.ValidationMessageFor(m => m.Nombre, "", new { @class = "field-error" })
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.ClienteId)
                @Html.DropDownListFor(m => m.ClienteId, (SelectList)ViewBag.Clientes, "-- Seleccione --", new { @class = "form-control" })
                @Html.ValidationMessageFor(m => m.ClienteId, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.TipoEventoId)
                @Html.DropDownListFor(m => m.TipoEventoId, (SelectList)ViewBag.TiposEvento, "-- Seleccione --", new { @class = "form-control" })
                @Html.ValidationMessageFor(m => m.TipoEventoId, "", new { @class = "field-error" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.VenueId)
            @Html.DropDownListFor(m => m.VenueId, (SelectList)ViewBag.Venues, "-- Seleccione --", new { @class = "form-control" })
            @Html.ValidationMessageFor(m => m.VenueId, "", new { @class = "field-error" })
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.FechaInicio)
                @Html.TextBoxFor(m => m.FechaInicio, "{0:yyyy-MM-dd}", new { @class = "form-control", type = "date" })
                @Html.ValidationMessageFor(m => m.FechaInicio, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.FechaFin)
                @Html.TextBoxFor(m => m.FechaFin, "{0:yyyy-MM-dd}", new { @class = "form-control", type = "date" })
                @Html.ValidationMessageFor(m => m.FechaFin, "", new { @class = "field-error" })
            </div>
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.HoraInicio)
                @Html.TextBoxFor(m => m.HoraInicio, "{0:HH:mm}", new { @class = "form-control", type = "time" })
                @Html.ValidationMessageFor(m => m.HoraInicio, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.HoraFin)
                @Html.TextBoxFor(m => m.HoraFin, "{0:HH:mm}", new { @class = "form-control", type = "time" })
                @Html.ValidationMessageFor(m => m.HoraFin, "", new { @class = "field-error" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.PresupuestoEstimado)
            @Html.TextBoxFor(m => m.PresupuestoEstimado, "{0:F2}", new { @class = "form-control", type = "number", step = "0.01" })
            @Html.ValidationMessageFor(m => m.PresupuestoEstimado, "", new { @class = "field-error" })
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.Descripcion)
            @Html.TextAreaFor(m => m.Descripcion, new { @class = "form-control" })
        </div>
    </div>

    <div style="display:flex;gap:8px;margin-top:16px;">
        <button type="submit" class="btn btn-primary">Guardar Cambios</button>
        <a href="@Url.Action("Index")" class="btn btn-outline">Cancelar</a>
    </div>
}
```

---

### Task 7: Vistas de Clientes + Dashboard

**Files:**
- Create: `EventHub.01.Web/Views/Clientes/Index.cshtml`
- Create: `EventHub.01.Web/Views/Clientes/Create.cshtml`
- Create: `EventHub.01.Web/Views/Clientes/Edit.cshtml`
- Modify: `EventHub.01.Web/Views/Home/Index.cshtml`

- [ ] **Step 1: Views/Clientes/Index.cshtml**

```html
@model List<EventHub._02.Bussines.DTOs.ClienteDto>
@{
    ViewBag.Title = "Clientes";
}

<div class="page-header">
    <h1>Clientes</h1>
    <p>Gestión de clientes</p>
</div>

@if (TempData["SuccessMessage"] != null)
{
    <div style="background:#d1fae5;border:1px solid #059669;border-radius:8px;padding:12px 16px;color:#065f46;font-size:14px;margin-bottom:16px;">
        @TempData["SuccessMessage"]
    </div>
}

<div class="action-bar">
    <div class="search-box">
        <form method="get" class="flex gap-2 items-center">
            <input type="text" name="search" class="form-control" placeholder="Buscar clientes..." value="@ViewBag.Search" style="width:240px" />
            <button type="submit" class="btn btn-primary btn-sm">Buscar</button>
        </form>
    </div>
    <a href="@Url.Action("Create")" class="btn btn-primary">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
        Nuevo Cliente
    </a>
</div>

<div class="card" style="padding:0;overflow:hidden;">
    <table class="table">
        <thead>
            <tr>
                <th>Razón Social</th>
                <th>RUC</th>
                <th>Email</th>
                <th>Teléfono</th>
                <th>Clasificación</th>
                <th>Estado</th>
                <th style="text-align:right;">Acciones</th>
            </tr>
        </thead>
        <tbody>
            @if (Model.Count == 0)
            {
                <tr><td colspan="7" style="text-align:center;padding:40px;color:var(--text-muted);">No hay clientes registrados</td></tr>
            }
            @foreach (var item in Model)
            {
                <tr>
                    <td style="font-weight:600;">@item.RazonSocial</td>
                    <td>@item.Ruc</td>
                    <td>@item.EmailPrincipal</td>
                    <td>@item.TelefonoPrincipal</td>
                    <td><span class="badge" style="background:#dbeafe;color:#1d4ed8;">@item.Clasificacion</span></td>
                    <td>@(item.Estado ? "Activo" : "Inactivo")</td>
                    <td style="text-align:right;">
                        <a href="@Url.Action("Edit", new { id = item.Id })" class="btn btn-outline btn-sm">Editar</a>
                    </td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

- [ ] **Step 2: Views/Clientes/Create.cshtml**

```html
@model EventHub._02.Bussines.DTOs.ClienteDto
@{
    ViewBag.Title = "Nuevo Cliente";
}

<div class="page-header">
    <h1>Nuevo Cliente</h1>
    <p>Ingrese los datos del cliente</p>
</div>

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()

    <div class="card" style="max-width:600px;">
        <div class="form-group">
            @Html.LabelFor(m => m.RazonSocial)
            @Html.TextBoxFor(m => m.RazonSocial, new { @class = "form-control", placeholder = "Razón Social" })
            @Html.ValidationMessageFor(m => m.RazonSocial, "", new { @class = "field-error" })
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.NombreComercial)
            @Html.TextBoxFor(m => m.NombreComercial, new { @class = "form-control", placeholder = "Nombre Comercial" })
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.Ruc)
                @Html.TextBoxFor(m => m.Ruc, new { @class = "form-control", placeholder = "0000000000000", maxlength = "13" })
                @Html.ValidationMessageFor(m => m.Ruc, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.Clasificacion)
                @Html.DropDownListFor(m => m.Clasificacion, new SelectList(new[] { "Nuevo", "Regular", "VIP", "Estrategico" }), new { @class = "form-control" })
            </div>
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.EmailPrincipal)
                @Html.TextBoxFor(m => m.EmailPrincipal, new { @class = "form-control", placeholder = "email@cliente.com", type = "email" })
                @Html.ValidationMessageFor(m => m.EmailPrincipal, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.TelefonoPrincipal)
                @Html.TextBoxFor(m => m.TelefonoPrincipal, new { @class = "form-control", placeholder = "Teléfono" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.DireccionFiscal)
            @Html.TextBoxFor(m => m.DireccionFiscal, new { @class = "form-control", placeholder = "Dirección Fiscal" })
        </div>
    </div>

    <div style="display:flex;gap:8px;margin-top:16px;">
        <button type="submit" class="btn btn-primary">Crear Cliente</button>
        <a href="@Url.Action("Index")" class="btn btn-outline">Cancelar</a>
    </div>
}

@section scripts {
    @Scripts.Render("~/bundles/jqueryval")
}
```

- [ ] **Step 3: Views/Clientes/Edit.cshtml**

```html
@model EventHub._02.Bussines.DTOs.ClienteDto
@{
    ViewBag.Title = "Editar Cliente";
}

<div class="page-header">
    <h1>Editar Cliente</h1>
    <p>Modifique los datos del cliente</p>
</div>

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
    @Html.HiddenFor(m => m.Id)

    <div class="card" style="max-width:600px;">
        <div class="form-group">
            @Html.LabelFor(m => m.RazonSocial)
            @Html.TextBoxFor(m => m.RazonSocial, new { @class = "form-control" })
            @Html.ValidationMessageFor(m => m.RazonSocial, "", new { @class = "field-error" })
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.NombreComercial)
            @Html.TextBoxFor(m => m.NombreComercial, new { @class = "form-control" })
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.Ruc)
                @Html.TextBoxFor(m => m.Ruc, new { @class = "form-control", maxlength = "13" })
                @Html.ValidationMessageFor(m => m.Ruc, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.Clasificacion)
                @Html.DropDownListFor(m => m.Clasificacion, new SelectList(new[] { "Nuevo", "Regular", "VIP", "Estrategico" }), new { @class = "form-control" })
            </div>
        </div>

        <div class="form-row">
            <div class="form-group">
                @Html.LabelFor(m => m.EmailPrincipal)
                @Html.TextBoxFor(m => m.EmailPrincipal, new { @class = "form-control", type = "email" })
                @Html.ValidationMessageFor(m => m.EmailPrincipal, "", new { @class = "field-error" })
            </div>
            <div class="form-group">
                @Html.LabelFor(m => m.TelefonoPrincipal)
                @Html.TextBoxFor(m => m.TelefonoPrincipal, new { @class = "form-control" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(m => m.DireccionFiscal)
            @Html.TextBoxFor(m => m.DireccionFiscal, new { @class = "form-control" })
        </div>
    </div>

    <div style="display:flex;gap:8px;margin-top:16px;">
        <button type="submit" class="btn btn-primary">Guardar Cambios</button>
        <a href="@Url.Action("Index")" class="btn btn-outline">Cancelar</a>
    </div>
}

@section scripts {
    @Scripts.Render("~/bundles/jqueryval")
}
```

- [ ] **Step 4: Views/Home/Index.cshtml (Dashboard)**

```html
@model EventHub._02.Bussines.DTOs.DashboardDto
@{
    ViewBag.Title = "Dashboard";
}

<div class="page-header">
    <h1>Dashboard</h1>
    <p>Resumen general del sistema</p>
</div>

<div class="stats-grid">
    <div class="stat-card">
        <div class="stat-value">@Model.TotalEventos</div>
        <div class="stat-label">Total Eventos</div>
    </div>
    <div class="stat-card">
        <div class="stat-value">@Model.EventosActivos</div>
        <div class="stat-label">Eventos Activos</div>
    </div>
    <div class="stat-card">
        <div class="stat-value">@Model.EventosPlanificacion</div>
        <div class="stat-label">En Planificación</div>
    </div>
    <div class="stat-card">
        <div class="stat-value">@Model.EventosEjecucion</div>
        <div class="stat-label">En Ejecución</div>
    </div>
    <div class="stat-card">
        <div class="stat-value">@Model.TotalClientes</div>
        <div class="stat-label">Total Clientes</div>
    </div>
    <div class="stat-card">
        <div class="stat-value">@Model.ClientesActivos</div>
        <div class="stat-label">Clientes Activos</div>
    </div>
</div>

@if (Model.ProximosEventos.Count > 0)
{
    <h3 style="font-size:18px;font-weight:700;margin-bottom:12px;">Próximos Eventos</h3>
    <div class="card" style="padding:0;overflow:hidden;">
        <table class="table">
            <thead>
                <tr>
                    <th>Código</th>
                    <th>Evento</th>
                    <th>Cliente</th>
                    <th>Lugar</th>
                    <th>Fecha</th>
                    <th>Estado</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var e in Model.ProximosEventos)
                {
                    <tr>
                        <td style="font-weight:600;font-size:13px;">@e.Codigo</td>
                        <td><a href="@Url.Action("Details", "Eventos", new { id = e.Id })" style="color:var(--primary);text-decoration:none;font-weight:600;">@e.Nombre</a></td>
                        <td>@e.ClienteNombre</td>
                        <td>@e.VenueNombre</td>
                        <td>@e.FechaInicio.ToString("dd/MM/yyyy")</td>
                        <td><span class="badge badge-@e.Estado.ToLower()">@e.Estado</span></td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
}
```

- [ ] **Step 5: Compilar y probar**

---

### Task 8: Agregar rutas y vistas a proyecto

**Files:**
- Modify: `EventHub.01.Web/EventHub.01.Web.csproj` (asegurar que las vistas nuevas están incluidas)

Nota: Si el proyecto usa archivos `<Content Include="...">` en el csproj, las vistas nuevas deben agregarse. Si no (proyecto SDK-style o con wildcards), no es necesario.

- [ ] **Step 1: Verificar que el proyecto compila**

```bash
dotnet build EventHub.01.Web
```

O build desde Visual Studio. Si faltan vistas, el csproj debe incluirlas.

- [ ] **Step 2: Verificar rutas en RouteConfig.cs**

```csharp
// Default route ya está: {controller}/{action}/{id}
// Home/Index es default
routes.MapRoute(
    name: "Default",
    url: "{controller}/{action}/{id}",
    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
);
```
