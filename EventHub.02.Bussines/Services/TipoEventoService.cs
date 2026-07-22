using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;
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

        public async Task<TipoEventoDto> CreateAsync(TipoEventoDto dto)
        {
            var entity = new TipoEvento
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            _context.TiposEvento.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }
    }
}
