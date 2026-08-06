using System;
using System.Collections.Generic;
using System.Linq;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class GastoService : IGastoService
    {
        private readonly EventHubContext _context;

        public GastoService()
        {
            _context = new EventHubContext();
        }

        public List<GastoDto> ObtenerPorEvento(int eventoId)
        {
            return _context.Gastos
                .Where(g => g.EventoId == eventoId)
                .OrderByDescending(g => g.Fecha).ThenByDescending(g => g.FechaCreacion)
                .Select(g => new GastoDto
                {
                    Id = g.Id,
                    EventoId = g.EventoId,
                    Categoria = g.Categoria,
                    Concepto = g.Concepto,
                    Monto = g.Monto,
                    Fecha = g.Fecha,
                    Proveedor = g.Proveedor,
                    Notas = g.Notas,
                    CreadoPor = g.CreadoPor,
                    FechaCreacion = g.FechaCreacion
                })
                .ToList();
        }

        public GastoDto ObtenerPorId(int id)
        {
            return _context.Gastos
                .Where(g => g.Id == id)
                .Select(g => new GastoDto
                {
                    Id = g.Id,
                    EventoId = g.EventoId,
                    Categoria = g.Categoria,
                    Concepto = g.Concepto,
                    Monto = g.Monto,
                    Fecha = g.Fecha,
                    Proveedor = g.Proveedor,
                    Notas = g.Notas,
                    CreadoPor = g.CreadoPor,
                    FechaCreacion = g.FechaCreacion
                })
                .FirstOrDefault();
        }

        public GastoDto Crear(GastoFormDto dto, string usuario)
        {
            var gasto = new Gasto
            {
                EventoId = dto.EventoId,
                Categoria = dto.Categoria,
                Concepto = dto.Concepto,
                Monto = dto.Monto,
                Fecha = dto.Fecha,
                Proveedor = dto.Proveedor,
                Notas = dto.Notas,
                CreadoPor = usuario,
                FechaCreacion = DateTime.Now
            };

            _context.Gastos.Add(gasto);
            _context.SaveChanges();

            // Update Evento totals
            ActualizarTotalesEvento(dto.EventoId);

            return new GastoDto
            {
                Id = gasto.Id,
                EventoId = gasto.EventoId,
                Categoria = gasto.Categoria,
                Concepto = gasto.Concepto,
                Monto = gasto.Monto,
                Fecha = gasto.Fecha,
                Proveedor = gasto.Proveedor,
                Notas = gasto.Notas,
                CreadoPor = gasto.CreadoPor,
                FechaCreacion = gasto.FechaCreacion
            };
        }

        public bool Actualizar(int id, GastoFormDto dto)
        {
            var gasto = _context.Gastos.Find(id);
            if (gasto == null) return false;

            gasto.Categoria = dto.Categoria;
            gasto.Concepto = dto.Concepto;
            gasto.Monto = dto.Monto;
            gasto.Fecha = dto.Fecha;
            gasto.Proveedor = dto.Proveedor;
            gasto.Notas = dto.Notas;

            _context.SaveChanges();
            ActualizarTotalesEvento(dto.EventoId);
            return true;
        }

        public bool Eliminar(int id)
        {
            var gasto = _context.Gastos.Find(id);
            if (gasto == null) return false;

            var eventoId = gasto.EventoId;
            _context.Gastos.Remove(gasto);
            _context.SaveChanges();
            ActualizarTotalesEvento(eventoId);
            return true;
        }

        public Dictionary<string, decimal> ObtenerResumenPorCategoria(int eventoId)
        {
            return _context.Gastos
                .Where(g => g.EventoId == eventoId)
                .GroupBy(g => g.Categoria)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Monto));
        }

        private void ActualizarTotalesEvento(int eventoId)
        {
            var totalGastado = _context.Gastos
                .Where(g => g.EventoId == eventoId)
                .Sum(g => (decimal?)g.Monto) ?? 0;

            var evento = _context.Eventos.Find(eventoId);
            if (evento != null)
            {
                evento.GastoReal = totalGastado;
                _context.SaveChanges();
            }
        }
    }
}
