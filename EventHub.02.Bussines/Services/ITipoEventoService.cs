using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public interface ITipoEventoService
    {
        Task<List<TipoEvento>> GetAllAsync();
        Task<TipoEventoDto> CreateAsync(TipoEventoDto dto);
    }
}
