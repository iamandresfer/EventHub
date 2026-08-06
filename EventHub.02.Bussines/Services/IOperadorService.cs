using System.Collections.Generic;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface IOperadorService
    {
        List<OperadorDto> GetAll();
        List<OperadorDto> GetActivos();
        List<OperadorDto> GetPorEvento(int eventoId);
        OperadorDto GetById(int id);
        OperadorDto GetByCedula(string cedula);
        List<OperadorConEventosDto> GetConEventos();
        bool RemoverDeEvento(int operadorId);
        OperadorDto Create(OperadorFormDto dto);
        OperadorDto Update(OperadorFormDto dto);
        bool Delete(int id);
        bool ToggleEstado(int id);
    }
}
