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
                    ClienteNombre = e.Cliente.Nombre,
                    VenueNombre = e.Venue.Nombre,
                    TipoEvento = e.TipoEvento.Nombre,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    Estado = e.Estado,
                    PresupuestoEstimado = e.PresupuestoEstimado,
                    GastoReal = e.GastoReal,
                    TotalIngresos = e.TotalIngresos,
                    CoverPhotoUrl = e.CoverPhotoUrl,
                    ClienteId = e.ClienteId,
                    VenueId = e.VenueId,
                    TipoEventoId = e.TipoEventoId,
                    HoraInicio = e.HoraInicio,
                    HoraFin = e.HoraFin,
                    Descripcion = e.Descripcion
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
                    ClienteNombre = e.Cliente.Nombre,
                    VenueNombre = e.Venue.Nombre,
                    TipoEvento = e.TipoEvento.Nombre,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    Estado = e.Estado,
                    PresupuestoEstimado = e.PresupuestoEstimado,
                    GastoReal = e.GastoReal,
                    TotalIngresos = e.TotalIngresos,
                    CoverPhotoUrl = e.CoverPhotoUrl,
                    ClienteId = e.ClienteId,
                    VenueId = e.VenueId,
                    TipoEventoId = e.TipoEventoId,
                    HoraInicio = e.HoraInicio,
                    HoraFin = e.HoraFin,
                    Descripcion = e.Descripcion
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
                FechaFin = dto.FechaFin ?? dto.FechaInicio,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                PresupuestoEstimado = dto.PresupuestoEstimado,
                Estado = "Planificacion",
                Descripcion = dto.Descripcion,
                FechaCreacion = DateTime.Now,
                UsuarioCreadorId = usuarioCreadorId,
                ProductorGeneralId = usuarioCreadorId,
                CoverPhotoUrl = dto.CoverPhotoUrl
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
            entity.FechaFin = dto.FechaFin ?? dto.FechaInicio;
            entity.HoraInicio = dto.HoraInicio;
            entity.HoraFin = dto.HoraFin;
            entity.PresupuestoEstimado = dto.PresupuestoEstimado;
            entity.Descripcion = dto.Descripcion;
            if (!string.IsNullOrEmpty(dto.CoverPhotoUrl))
                entity.CoverPhotoUrl = dto.CoverPhotoUrl;

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

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Eventos.FindAsync(id);
            if (entity == null) return false;

            _context.Eventos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var now = DateTime.Now;
            var inicioActividad = now.AddMonths(-12);

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
                    .ToListAsync(),
                EventosActividad = await _context.Eventos
                    .Where(e => e.FechaInicio >= inicioActividad && e.FechaInicio <= now)
                    .Select(e => e.FechaInicio)
                    .ToListAsync()
            };
        }

        private async Task<string> GenerarCodigoAsync()
        {
            var codes = await _context.Eventos.Select(e => e.Codigo).ToListAsync();
            var maxNum = 0;
            foreach (var code in codes)
            {
                var match = System.Text.RegularExpressions.Regex.Match(code ?? "", @"(\d+)\s*$");
                if (match.Success && int.TryParse(match.Value, out var n) && n > maxNum)
                    maxNum = n;
            }
            return $"EVT-{maxNum + 1:D5}";
        }
    }
}
