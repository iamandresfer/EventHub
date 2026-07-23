using System;
using System.Collections.Generic;
using System.Linq;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class CrewService : ICrewService
    {
        private readonly EventHubContext _context;

        public CrewService()
        {
            _context = new EventHubContext();
        }

        public List<CrewOperadorDto> ObtenerCrewPorEvento(int eventoId)
        {
            return _context.CrewOperadores
                .Where(c => c.EventoId == eventoId)
                .OrderBy(c => c.Nombre)
                .Select(c => new CrewOperadorDto
                {
                    Id = c.Id,
                    EventoId = c.EventoId,
                    Nombre = c.Nombre,
                    Cedula = c.Cedula,
                    Email = c.Email,
                    Telefono = c.Telefono,
                    Rol = c.Rol,
                    Estado = c.Estado,
                    FechaCreacion = c.FechaCreacion
                })
                .ToList();
        }

        public CrewOperadorDto ObtenerPorId(int id)
        {
            var c = _context.CrewOperadores.Find(id);
            if (c == null) return null;

            return new CrewOperadorDto
            {
                Id = c.Id,
                EventoId = c.EventoId,
                Nombre = c.Nombre,
                Cedula = c.Cedula,
                Email = c.Email,
                Telefono = c.Telefono,
                Rol = c.Rol,
                Estado = c.Estado,
                FechaCreacion = c.FechaCreacion
            };
        }

        public CrewOperadorDto CrearCrew(CrewOperadorFormDto dto)
        {
            var nuevo = new CrewOperador
            {
                EventoId = dto.EventoId,
                Nombre = dto.Nombre,
                Cedula = dto.Cedula,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Rol = dto.Rol,
                Estado = true,
                FechaCreacion = DateTime.Now
            };

            _context.CrewOperadores.Add(nuevo);
            _context.SaveChanges();

            return new CrewOperadorDto
            {
                Id = nuevo.Id,
                EventoId = nuevo.EventoId,
                Nombre = nuevo.Nombre,
                Cedula = nuevo.Cedula,
                Email = nuevo.Email,
                Telefono = nuevo.Telefono,
                Rol = nuevo.Rol,
                Estado = nuevo.Estado,
                FechaCreacion = nuevo.FechaCreacion
            };
        }

        public CrewOperadorDto ActualizarCrew(CrewOperadorFormDto dto)
        {
            var existing = _context.CrewOperadores.Find(dto.Id);
            if (existing == null) return null;

            existing.Nombre = dto.Nombre;
            existing.Cedula = dto.Cedula;
            existing.Email = dto.Email;
            existing.Telefono = dto.Telefono;
            existing.Rol = dto.Rol;

            _context.SaveChanges();

            return new CrewOperadorDto
            {
                Id = existing.Id,
                EventoId = existing.EventoId,
                Nombre = existing.Nombre,
                Cedula = existing.Cedula,
                Email = existing.Email,
                Telefono = existing.Telefono,
                Rol = existing.Rol,
                Estado = existing.Estado,
                FechaCreacion = existing.FechaCreacion
            };
        }

        public bool EliminarCrew(int id)
        {
            var crew = _context.CrewOperadores.Find(id);
            if (crew == null) return false;

            _context.CrewOperadores.Remove(crew);
            _context.SaveChanges();
            return true;
        }

        public bool ToggleEstado(int id)
        {
            var crew = _context.CrewOperadores.Find(id);
            if (crew == null) return false;

            crew.Estado = !crew.Estado;
            _context.SaveChanges();
            return true;
        }
    }
}
