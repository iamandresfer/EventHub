using System.Collections.Generic;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface IIngresoService
    {
        List<IngresoDto> ObtenerPorEvento(int eventoId);
        IngresoDto ObtenerPorId(int id);
        IngresoDto Crear(IngresoFormDto dto, string usuario);
        bool Actualizar(int id, IngresoFormDto dto);
        bool Eliminar(int id);
        Dictionary<string, decimal> ObtenerResumenPorTipo(int eventoId);
    }
}
