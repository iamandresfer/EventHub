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
                    Categoria = t.Categoria,
                    FechaLimite = t.FechaLimite,
                    AsignadoAId = t.AsignadoAId,
                    AsignadoANombre = t.AsignadoA != null ? t.AsignadoA.Nombre : null,
                    AsignadoAEmail = t.AsignadoA != null ? t.AsignadoA.Email : null,
                    OperadorId = t.OperadorId,
                    OperadorNombre = t.Operador != null ? t.Operador.Nombre : null,
                    OperadorEmail = t.Operador != null ? t.Operador.Email : null,
                    CreadoPorId = t.CreadoPorId,
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
                Categoria = dto.Categoria,
                FechaLimite = dto.FechaLimite,
                AsignadoAId = dto.AsignadoAId,
                OperadorId = dto.OperadorId,
                Orden = maxOrden + 1
            };

            _context.Tareas.Add(nuevaTarea);
            _context.SaveChanges();

            if (nuevaTarea.AsignadoAId.HasValue)
                _context.Entry(nuevaTarea).Reference(t => t.AsignadoA).Load();
            if (nuevaTarea.OperadorId.HasValue)
                _context.Entry(nuevaTarea).Reference(t => t.Operador).Load();

            return new TareaDto
            {
                Id = nuevaTarea.Id,
                EventoId = nuevaTarea.EventoId,
                Titulo = nuevaTarea.Titulo,
                Descripcion = nuevaTarea.Descripcion,
                Estado = nuevaTarea.Estado,
                Categoria = nuevaTarea.Categoria,
                FechaLimite = nuevaTarea.FechaLimite,
                AsignadoAId = nuevaTarea.AsignadoAId,
                AsignadoANombre = nuevaTarea.AsignadoA?.Nombre,
                AsignadoAEmail = nuevaTarea.AsignadoA?.Email,
                OperadorId = nuevaTarea.OperadorId,
                OperadorNombre = nuevaTarea.Operador?.Nombre,
                OperadorEmail = nuevaTarea.Operador?.Email,
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

        public TareaDto ObtenerPorId(int id)
        {
            var t = _context.Tareas.Find(id);
            if (t == null) return null;

            return new TareaDto
            {
                Id = t.Id,
                EventoId = t.EventoId,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                Estado = t.Estado,
                Categoria = t.Categoria,
                FechaLimite = t.FechaLimite,
                AsignadoAId = t.AsignadoAId,
                AsignadoANombre = t.AsignadoA?.Nombre,
                AsignadoAEmail = t.AsignadoA?.Email,
                OperadorId = t.OperadorId,
                OperadorNombre = t.Operador?.Nombre,
                OperadorEmail = t.Operador?.Email,
                CreadoPorId = t.CreadoPorId,
                Orden = t.Orden
            };
        }

        public bool ActualizarFechaLimite(int tareaId, DateTime? nuevaFecha)
        {
            var tarea = _context.Tareas.Find(tareaId);
            if (tarea == null) return false;

            tarea.FechaLimite = nuevaFecha;
            _context.SaveChanges();
            return true;
        }
    }
}
