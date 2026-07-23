using System.Collections.Generic;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface ICrewService
    {
        List<CrewOperadorDto> ObtenerCrewPorEvento(int eventoId);
        CrewOperadorDto ObtenerPorId(int id);
        CrewOperadorDto CrearCrew(CrewOperadorFormDto dto);
        CrewOperadorDto ActualizarCrew(CrewOperadorFormDto dto);
        bool EliminarCrew(int id);
        bool ToggleEstado(int id);
    }
}
