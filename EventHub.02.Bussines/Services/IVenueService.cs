using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public interface IVenueService
    {
        Task<List<Venue>> GetAvailableAsync();
        Task<VenueDto> CreateAsync(VenueDto dto);
    }
}
