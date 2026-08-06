using System;
using System.Collections.Generic;
using System.Linq;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class IngresoService : IIngresoService
    {
        private readonly EventHubContext _context;

        public IngresoService()
        {
            _context = new EventHubContext();
        }

        public List<IngresoDto> ObtenerPorEvento(int eventoId)
        {
            return _context.Ingresos
                .Where(i => i.EventoId == eventoId)
                .OrderByDescending(i => i.Fecha).ThenByDescending(i => i.FechaCreacion)
                .Select(i => new IngresoDto
                {
                    Id = i.Id,
                    EventoId = i.EventoId,
                    Tipo = i.Tipo,
                    Concepto = i.Concepto,
                    Monto = i.Monto,
                    Fecha = i.Fecha,
                    Cliente = i.Cliente,
                    Notas = i.Notas,
                    CreadoPor = i.CreadoPor,
                    FechaCreacion = i.FechaCreacion
                })
                .ToList();
        }

        public IngresoDto ObtenerPorId(int id)
        {
            return _context.Ingresos
                .Where(i => i.Id == id)
                .Select(i => new IngresoDto
                {
                    Id = i.Id,
                    EventoId = i.EventoId,
                    Tipo = i.Tipo,
                    Concepto = i.Concepto,
                    Monto = i.Monto,
                    Fecha = i.Fecha,
                    Cliente = i.Cliente,
                    Notas = i.Notas,
                    CreadoPor = i.CreadoPor,
                    FechaCreacion = i.FechaCreacion
                })
                .FirstOrDefault();
        }

        public IngresoDto Crear(IngresoFormDto dto, string usuario)
        {
            var ingreso = new Ingreso
            {
                EventoId = dto.EventoId,
                Tipo = dto.Tipo,
                Concepto = dto.Concepto,
                Monto = dto.Monto,
                Fecha = dto.Fecha,
                Cliente = dto.Cliente,
                Notas = dto.Notas,
                CreadoPor = usuario,
                FechaCreacion = DateTime.Now
            };

            _context.Ingresos.Add(ingreso);
            _context.SaveChanges();

            ActualizarTotalesEvento(dto.EventoId);

            return new IngresoDto
            {
                Id = ingreso.Id,
                EventoId = ingreso.EventoId,
                Tipo = ingreso.Tipo,
                Concepto = ingreso.Concepto,
                Monto = ingreso.Monto,
                Fecha = ingreso.Fecha,
                Cliente = ingreso.Cliente,
                Notas = ingreso.Notas,
                CreadoPor = ingreso.CreadoPor,
                FechaCreacion = ingreso.FechaCreacion
            };
        }

        public bool Actualizar(int id, IngresoFormDto dto)
        {
            var ingreso = _context.Ingresos.Find(id);
            if (ingreso == null) return false;

            ingreso.Tipo = dto.Tipo;
            ingreso.Concepto = dto.Concepto;
            ingreso.Monto = dto.Monto;
            ingreso.Fecha = dto.Fecha;
            ingreso.Cliente = dto.Cliente;
            ingreso.Notas = dto.Notas;

            _context.SaveChanges();
            ActualizarTotalesEvento(dto.EventoId);
            return true;
        }

        public bool Eliminar(int id)
        {
            var ingreso = _context.Ingresos.Find(id);
            if (ingreso == null) return false;

            var eventoId = ingreso.EventoId;
            _context.Ingresos.Remove(ingreso);
            _context.SaveChanges();
            ActualizarTotalesEvento(eventoId);
            return true;
        }

        public Dictionary<string, decimal> ObtenerResumenPorTipo(int eventoId)
        {
            return _context.Ingresos
                .Where(i => i.EventoId == eventoId)
                .GroupBy(i => i.Tipo)
                .ToDictionary(i => i.Key, i => i.Sum(x => x.Monto));
        }

        private void ActualizarTotalesEvento(int eventoId)
        {
            var totalIngresos = _context.Ingresos
                .Where(i => i.EventoId == eventoId)
                .Sum(i => (decimal?)i.Monto) ?? 0;

            var evento = _context.Eventos.Find(eventoId);
            if (evento != null)
            {
                evento.TotalIngresos = totalIngresos;
                _context.SaveChanges();
            }
        }
    }
}
