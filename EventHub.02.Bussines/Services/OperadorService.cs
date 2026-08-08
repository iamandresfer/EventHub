using System;
using System.Collections.Generic;
using System.Linq;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class OperadorService : IOperadorService
    {
        private readonly EventHubContext _context;

        public OperadorService()
        {
            _context = new EventHubContext();
        }

        public List<OperadorDto> GetAll()
        {
            return _context.Operadores
                .OrderBy(o => o.Nombre)
                .Select(o => new OperadorDto
                {
                    Id = o.Id,
                    Nombre = o.Nombre,
                    Cedula = o.Cedula,
                    Email = o.Email,
                    Telefono = o.Telefono,
                    Rol = o.Rol,
                    Estado = o.Estado,
                    FechaCreacion = o.FechaCreacion,
                    FotoUrl = o.FotoUrl,
                    EventoId = o.EventoId
                })
                .ToList();
        }

        public List<OperadorDto> GetActivos()
        {
            return _context.Operadores
                .Where(o => o.Estado)
                .OrderBy(o => o.Nombre)
                .Select(o => new OperadorDto
                {
                    Id = o.Id,
                    Nombre = o.Nombre,
                    Cedula = o.Cedula,
                    Email = o.Email,
                    Telefono = o.Telefono,
                    Rol = o.Rol,
                    Estado = o.Estado,
                    FechaCreacion = o.FechaCreacion,
                    FotoUrl = o.FotoUrl,
                    EventoId = o.EventoId
                })
                .ToList();
        }

        public List<OperadorDto> GetPorEvento(int eventoId)
        {
            return _context.Operadores
                .Where(o => o.EventoId == eventoId && o.Estado)
                .OrderBy(o => o.Nombre)
                .Select(o => new OperadorDto
                {
                    Id = o.Id,
                    Nombre = o.Nombre,
                    Cedula = o.Cedula,
                    Email = o.Email,
                    Telefono = o.Telefono,
                    Rol = o.Rol,
                    Estado = o.Estado,
                    FechaCreacion = o.FechaCreacion,
                    FotoUrl = o.FotoUrl,
                    EventoId = o.EventoId
                })
                .ToList();
        }

        public OperadorDto GetById(int id)
        {
            var o = _context.Operadores.Find(id);
            if (o == null) return null;

            return new OperadorDto
            {
                Id = o.Id,
                Nombre = o.Nombre,
                Cedula = o.Cedula,
                Email = o.Email,
                Telefono = o.Telefono,
                Rol = o.Rol,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion,
                FotoUrl = o.FotoUrl,
                EventoId = o.EventoId
            };
        }

        public OperadorDto GetByCedula(string cedula)
        {
            var o = _context.Operadores.FirstOrDefault(x => x.Cedula == cedula);
            if (o == null) return null;

            return new OperadorDto
            {
                Id = o.Id,
                Nombre = o.Nombre,
                Cedula = o.Cedula,
                Email = o.Email,
                Telefono = o.Telefono,
                Rol = o.Rol,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion,
                FotoUrl = o.FotoUrl,
                EventoId = o.EventoId
            };
        }

        public List<OperadorConEventosDto> GetConEventos()
        {
            // EF6 no traduce listas dentro del query: primero se proyecta una forma
            // plana a SQL y luego se materializa la lista Eventos en memoria.
            var plano = _context.Operadores
                .OrderBy(o => o.Nombre)
                .Select(o => new
                {
                    o.Id,
                    o.Nombre,
                    o.Cedula,
                    o.Email,
                    o.Telefono,
                    o.Rol,
                    o.Estado,
                    o.FechaCreacion,
                    o.FotoUrl,
                    EventoId = o.EventoId,
                    EventoNombre = o.Evento != null ? o.Evento.Nombre : null,
                    EventoCodigo = o.Evento != null ? o.Evento.Codigo : null
                })
                .ToList();

            return plano
                .Select(o => new OperadorConEventosDto
                {
                    Id = o.Id,
                    Nombre = o.Nombre,
                    Cedula = o.Cedula,
                    Email = o.Email,
                    Telefono = o.Telefono,
                    Rol = o.Rol,
                    Estado = o.Estado,
                    FechaCreacion = o.FechaCreacion,
                    FotoUrl = o.FotoUrl,
                    Eventos = o.EventoId.HasValue
                        ? new List<OperadorEventoDto>
                        {
                            new OperadorEventoDto
                            {
                                EventoId = o.EventoId.Value,
                                EventoNombre = o.EventoNombre,
                                EventoCodigo = o.EventoCodigo,
                                Estado = o.Estado,
                                Rol = o.Rol
                            }
                        }
                        : new List<OperadorEventoDto>()
                })
                .ToList();
        }

        public bool RemoverDeEvento(int operadorId)
        {
            var operador = _context.Operadores.Find(operadorId);
            if (operador == null) return false;

            operador.EventoId = null;
            _context.SaveChanges();
            return true;
        }

        public OperadorDto Create(OperadorFormDto dto)
        {
            var nuevo = new Operador
            {
                Nombre = dto.Nombre,
                Cedula = dto.Cedula,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Rol = dto.Rol,
                Estado = true,
                FechaCreacion = DateTime.Now,
                FotoUrl = dto.FotoUrl
            };

            _context.Operadores.Add(nuevo);
            _context.SaveChanges();

            return new OperadorDto
            {
                Id = nuevo.Id,
                Nombre = nuevo.Nombre,
                Cedula = nuevo.Cedula,
                Email = nuevo.Email,
                Telefono = nuevo.Telefono,
                Rol = nuevo.Rol,
                Estado = nuevo.Estado,
                FechaCreacion = nuevo.FechaCreacion,
                FotoUrl = nuevo.FotoUrl,
                EventoId = nuevo.EventoId
            };
        }

        public OperadorDto Update(OperadorFormDto dto)
        {
            var existing = _context.Operadores.Find(dto.Id);
            if (existing == null) return null;

            existing.Nombre = dto.Nombre;
            existing.Cedula = dto.Cedula;
            existing.Email = dto.Email;
            existing.Telefono = dto.Telefono;
            existing.Rol = dto.Rol;
            if (!string.IsNullOrEmpty(dto.FotoUrl))
                existing.FotoUrl = dto.FotoUrl;

            _context.SaveChanges();

            return new OperadorDto
            {
                Id = existing.Id,
                Nombre = existing.Nombre,
                Cedula = existing.Cedula,
                Email = existing.Email,
                Telefono = existing.Telefono,
                Rol = existing.Rol,
                Estado = existing.Estado,
                FechaCreacion = existing.FechaCreacion,
                FotoUrl = existing.FotoUrl,
                EventoId = existing.EventoId
            };
        }

        public bool Delete(int id)
        {
            var o = _context.Operadores.Find(id);
            if (o == null) return false;

            _context.Operadores.Remove(o);
            _context.SaveChanges();
            return true;
        }

        public bool ToggleEstado(int id)
        {
            var o = _context.Operadores.Find(id);
            if (o == null) return false;

            o.Estado = !o.Estado;
            _context.SaveChanges();
            return true;
        }
    }
}
