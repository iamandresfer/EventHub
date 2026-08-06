using System.Collections.Generic;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface IGastoService
    {
        List<GastoDto> ObtenerPorEvento(int eventoId);
        GastoDto ObtenerPorId(int id);
        GastoDto Crear(GastoFormDto dto, string usuario);
        bool Actualizar(int id, GastoFormDto dto);
        bool Eliminar(int id);
        Dictionary<string, decimal> ObtenerResumenPorCategoria(int eventoId);
    }
}
