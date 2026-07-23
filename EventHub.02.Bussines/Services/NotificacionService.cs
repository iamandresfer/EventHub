using System;
using System.Collections.Generic;
using System.Linq;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly EventHubContext _context;

        public NotificacionService()
        {
            _context = new EventHubContext();
        }

        public List<NotificacionDto> ObtenerPorEvento(int eventoId)
        {
            return _context.Notificaciones
                .Where(n => n.EventoId == eventoId)
                .OrderByDescending(n => n.FechaCreacion)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    Tipo = n.Tipo,
                    Mensaje = n.Mensaje,
                    EmailDestino = n.EmailDestino,
                    NombreDestino = n.NombreDestino,
                    EventoId = n.EventoId,
                    TareaId = n.TareaId,
                    Leida = n.Leida,
                    Enviada = n.Enviada,
                    FechaCreacion = n.FechaCreacion,
                    FechaEnvio = n.FechaEnvio,
                    EventoNombre = n.Evento != null ? n.Evento.Nombre : null,
                    TareaTitulo = n.Tarea != null ? n.Tarea.Titulo : null
                })
                .ToList();
        }

        public List<NotificacionDto> ObtenerRecientes(int top = 20)
        {
            return _context.Notificaciones
                .OrderByDescending(n => n.FechaCreacion)
                .Take(top)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    Tipo = n.Tipo,
                    Mensaje = n.Mensaje,
                    EmailDestino = n.EmailDestino,
                    NombreDestino = n.NombreDestino,
                    EventoId = n.EventoId,
                    TareaId = n.TareaId,
                    Leida = n.Leida,
                    Enviada = n.Enviada,
                    FechaCreacion = n.FechaCreacion,
                    FechaEnvio = n.FechaEnvio,
                    EventoNombre = n.Evento != null ? n.Evento.Nombre : null,
                    TareaTitulo = n.Tarea != null ? n.Tarea.Titulo : null
                })
                .ToList();
        }

        public bool MarcarComoLeida(int notificacionId)
        {
            var notif = _context.Notificaciones.Find(notificacionId);
            if (notif == null) return false;

            notif.Leida = true;
            _context.SaveChanges();
            return true;
        }

        public int ContarNoLeidas()
        {
            return _context.Notificaciones.Count(n => !n.Leida);
        }

        /// <summary>
        /// Crea una notificación en BD y envía email de forma asíncrona.
        /// </summary>
        public void CrearYEnviar(string tipo, string mensaje, string emailDestino, string nombreDestino,
            int? eventoId, int? tareaId, IEmailService emailService, string nombreEvento = null, string tareaTitulo = null)
        {
            var notif = new Notificacion
            {
                Tipo = tipo,
                Mensaje = mensaje,
                EmailDestino = emailDestino,
                NombreDestino = nombreDestino,
                EventoId = eventoId,
                TareaId = tareaId,
                Leida = false,
                Enviada = false,
                FechaCreacion = DateTime.Now
            };

            _context.Notificaciones.Add(notif);
            _context.SaveChanges();

            // Enviar email de forma asíncrona (fire and forget)
            try
            {
                var subject = $"EventHub - {tipo}";
                var body = ConstruirBodyEmail(tipo, nombreDestino, mensaje, nombreEvento, tareaTitulo);

                System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        await emailService.SendGenericEmailAsync(emailDestino, subject, body);

                        notif.Enviada = true;
                        notif.FechaEnvio = DateTime.Now;
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        notif.Error = ex.Message;
                        _context.SaveChanges();
                    }
                });
            }
            catch (Exception ex)
            {
                notif.Error = ex.Message;
                _context.SaveChanges();
            }
        }

        private string ConstruirBodyEmail(string tipo, string nombreDestino, string mensaje,
            string nombreEvento, string tareaTitulo)
        {
            var icono = tipo switch
            {
                "TareaCreada" => "📋",
                "TareaCompletada" => "✅",
                "TareaVencida" => "⚠️",
                "FechaModificada" => "📅",
                _ => "📌"
            };

            var color = tipo switch
            {
                "TareaCreada" => "#4361ee",
                "TareaCompletada" => "#10b981",
                "TareaVencida" => "#ef4444",
                "FechaModificada" => "#f59e0b",
                _ => "#6c757d"
            };

            return $@"
                <div style='font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, sans-serif; max-width: 500px; margin: 0 auto; padding: 32px;'>
                    <div style='text-align: center; margin-bottom: 24px;'>
                        <span style='font-size: 48px;'>{icono}</span>
                    </div>
                    <h2 style='color: #1a1a2e; text-align: center;'>Hola {nombreDestino}</h2>
                    <div style='background: {color}10; border-left: 4px solid {color}; padding: 16px; border-radius: 0 8px 8px 0; margin: 20px 0;'>
                        <p style='color: #374151; font-size: 15px; margin: 0;'>{mensaje}</p>
                    </div>
                    {(nombreEvento != null ? $"<p style='color: #6c757d; font-size: 14px;'><strong>Evento:</strong> {nombreEvento}</p>" : "")}
                    {(tareaTitulo != null ? $"<p style='color: #6c757d; font-size: 14px;'><strong>Tarea:</strong> {tareaTitulo}</p>" : "")}
                    <hr style='border: none; border-top: 1px solid #dee2e6; margin: 24px 0;'>
                    <p style='color: #adb5bd; font-size: 12px; text-align: center;'>EventProduction Hub - Sistema Integral de Gestión de Eventos</p>
                </div>";
        }
    }
}
