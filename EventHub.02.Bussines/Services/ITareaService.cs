using System;
using System.Collections.Generic;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface ITareaService
    {
        List<TareaDto> ObtenerTareasPorEvento(int eventoId);
        TareaDto ObtenerPorId(int id);
        TareaDto CrearTarea(TareaFormDto dto, int creadoPorId = 0);
        TareaDto ActualizarTarea(TareaFormDto dto);
        bool ActualizarEstado(int tareaId, string nuevoEstado, int nuevoOrden);
        bool EliminarTarea(int id);
        bool ActualizarOrden(int tareaId, int nuevoOrden, string estado);
        bool ActualizarFechaLimite(int tareaId, DateTime? nuevaFecha);
    }
}
