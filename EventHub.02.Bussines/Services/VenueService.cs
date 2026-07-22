using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;
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
                .Where(v => v.Estado == "Disponible" || v.Estado == "Activo" || v.Estado == null)
                .OrderBy(v => v.Nombre)
                .ToListAsync();
        }

        public async Task<VenueDto> CreateAsync(VenueDto dto)
        {
            var entity = new Venue
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                CapacidadMaxima = dto.CapacidadMaxima,
                Estado = "Disponible"
            };

            _context.Venues.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.Estado = entity.Estado;
            return dto;
        }
    }
}
