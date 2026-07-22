using System.Collections.Generic;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface ITareaService
    {
        List<TareaDto> ObtenerTareasPorEvento(int eventoId);
        TareaDto CrearTarea(TareaFormDto dto);
        bool ActualizarEstado(int tareaId, string nuevoEstado, int nuevoOrden);
        bool EliminarTarea(int id);
        bool ActualizarOrden(int tareaId, int nuevoOrden, string estado);
    }
}
