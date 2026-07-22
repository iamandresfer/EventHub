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
        Task<bool> DeleteAsync(int id);
        Task<DashboardDto> GetDashboardAsync();
    }
}
