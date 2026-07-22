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
        Task<bool> ToggleEstadoAsync(int id);
        Task<bool> DeactivateAsync(int id);
    }
}
