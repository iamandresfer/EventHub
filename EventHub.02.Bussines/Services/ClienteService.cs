using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class ClienteService : IClienteService
    {
        private readonly EventHubContext _context;

        public ClienteService(EventHubContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteDto>> GetAllAsync()
        {
            return await _context.Clientes
                .OrderByDescending(c => c.Id)
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Ruc = c.Ruc,
                    Email = c.Email,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Contacto = c.Contacto,
                    Clasificacion = c.Clasificacion,
                    Estado = c.Estado
                })
                .ToListAsync();
        }

        public async Task<List<ClienteDto>> GetActiveAsync()
        {
            return await _context.Clientes
                .Where(c => c.Estado)
                .OrderBy(c => c.Nombre)
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Ruc = c.Ruc,
                    Email = c.Email,
                    Contacto = c.Contacto,
                    Clasificacion = c.Clasificacion,
                    Estado = c.Estado
                })
                .ToListAsync();
        }

        public async Task<ClienteDto> GetByIdAsync(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            if (c == null) return null;

            return new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Ruc = c.Ruc,
                Email = c.Email,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                Contacto = c.Contacto,
                Clasificacion = c.Clasificacion,
                Estado = c.Estado
            };
        }

        public async Task<ClienteDto> CreateAsync(ClienteDto dto, int usuarioCreadorId)
        {
            var entity = new Cliente
            {
                Nombre = dto.Nombre,
                Ruc = dto.Ruc,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Contacto = dto.Contacto,
                Clasificacion = dto.Clasificacion ?? "Nuevo",
                FechaRegistro = DateTime.Now,
                Estado = true,
                UsuarioCreadorId = usuarioCreadorId
            };

            _context.Clientes.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.Estado = true;
            return dto;
        }

        public async Task<ClienteDto> UpdateAsync(ClienteDto dto)
        {
            var entity = await _context.Clientes.FindAsync(dto.Id);
            if (entity == null) return null;

            entity.Nombre = dto.Nombre;
            entity.Ruc = dto.Ruc;
            entity.Email = dto.Email;
            entity.Telefono = dto.Telefono;
            entity.Direccion = dto.Direccion;
            entity.Contacto = dto.Contacto;
            entity.Clasificacion = dto.Clasificacion;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> ToggleEstadoAsync(int id)
        {
            var entity = await _context.Clientes.FindAsync(id);
            if (entity == null) return false;

            entity.Estado = !entity.Estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await _context.Clientes.FindAsync(id);
            if (entity == null) return false;

            entity.Estado = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
