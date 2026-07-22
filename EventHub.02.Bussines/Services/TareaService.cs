using System;
using System.Collections.Generic;
using System.Linq;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class TareaService : ITareaService
    {
        private readonly EventHubContext _context;

        public TareaService()
        {
            _context = new EventHubContext();
        }

        public List<TareaDto> ObtenerTareasPorEvento(int eventoId)
        {
            return _context.Tareas
                .Where(t => t.EventoId == eventoId)
                .OrderBy(t => t.Estado).ThenBy(t => t.Orden)
                .Select(t => new TareaDto
                {
                    Id = t.Id,
                    EventoId = t.EventoId,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Estado = t.Estado,
                    FechaLimite = t.FechaLimite,
                    AsignadoAId = t.AsignadoAId,
                    AsignadoANombre = t.AsignadoA != null ? t.AsignadoA.Nombre : null,
                    Orden = t.Orden
                })
                .ToList();
        }

        public TareaDto CrearTarea(TareaFormDto dto)
        {
            var maxOrden = _context.Tareas
                .Where(t => t.EventoId == dto.EventoId && t.Estado == dto.Estado)
                .Select(t => (int?)t.Orden)
                .Max() ?? 0;

            var nuevaTarea = new Tarea
            {
                EventoId = dto.EventoId,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Estado = dto.Estado,
                FechaLimite = dto.FechaLimite,
                AsignadoAId = dto.AsignadoAId,
                Orden = maxOrden + 1
            };

            _context.Tareas.Add(nuevaTarea);
            _context.SaveChanges();

            return new TareaDto
            {
                Id = nuevaTarea.Id,
                EventoId = nuevaTarea.EventoId,
                Titulo = nuevaTarea.Titulo,
                Descripcion = nuevaTarea.Descripcion,
                Estado = nuevaTarea.Estado,
                FechaLimite = nuevaTarea.FechaLimite,
                AsignadoAId = nuevaTarea.AsignadoAId,
                Orden = nuevaTarea.Orden
            };
        }

        public bool ActualizarEstado(int tareaId, string nuevoEstado, int nuevoOrden)
        {
            var tarea = _context.Tareas.Find(tareaId);
            if (tarea == null) return false;

            tarea.Estado = nuevoEstado;
            tarea.Orden = nuevoOrden;
            
            _context.SaveChanges();
            return true;
        }
        
        public bool ActualizarOrden(int tareaId, int nuevoOrden, string estado)
        {
            var tarea = _context.Tareas.Find(tareaId);
            if (tarea == null) return false;

            tarea.Orden = nuevoOrden;
            tarea.Estado = estado;
            
            _context.SaveChanges();
            return true;
        }

        public bool EliminarTarea(int id)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null) return false;

            _context.Tareas.Remove(tarea);
            _context.SaveChanges();
            return true;
        }
    }
}
