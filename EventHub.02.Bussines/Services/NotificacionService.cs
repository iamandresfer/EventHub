using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using EventHub._02.Bussines.DTOs;
using EventHub._03.Data;
using EventHub._03.Data.Entities;

namespace EventHub._02.Bussines.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly EventHubContext _context;
        private readonly string _appBaseUrl;

        public NotificacionService()
        {
            _context = new EventHubContext();
            _appBaseUrl = ConfigurationManager.AppSettings["AppBaseUrl"] ?? "https://localhost:44353";
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
            // IMPORTANTE: usamos un contexto nuevo dentro del Task.Run porque DbContext NO es thread-safe.
            // El contexto principal puede quedar disposed antes de que el task secundario complete.
            var notifId = notif.Id;
            try
            {
                var subject = $"EventHub - {tipo}";
                var body = ConstruirBodyEmail(tipo, nombreDestino, mensaje, nombreEvento, tareaTitulo, eventoId);

                System.Threading.Tasks.Task.Run(async () =>
                {
                    using (var asyncContext = new EventHubContext())
                    {
                        var notifAsync = asyncContext.Notificaciones.Find(notifId);
                        try
                        {
                            await emailService.SendGenericEmailAsync(emailDestino, subject, body);

                            if (notifAsync != null)
                            {
                                notifAsync.Enviada = true;
                                notifAsync.FechaEnvio = DateTime.Now;
                                asyncContext.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[NotificacionService] Error enviando email: {ex.Message}");
                            if (notifAsync != null)
                            {
                                notifAsync.Error = ex.Message.Length > 500 ? ex.Message.Substring(0, 500) : ex.Message;
                                asyncContext.SaveChanges();
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificacionService] Error iniciando task de email: {ex.Message}");
                notif.Error = ex.Message;
                _context.SaveChanges();
            }
        }

        private string ConstruirBodyEmail(string tipo, string nombreDestino, string mensaje,
            string nombreEvento, string tareaTitulo, int? eventoId)
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

            var botonVerTarea = eventoId.HasValue
                ? $"<p style='text-align:center; margin:24px 0;'><a href='{_appBaseUrl}/Eventos/Tareas/{eventoId.Value}' style='display:inline-block; padding:12px 24px; background:{color}; color:white; text-decoration:none; border-radius:8px; font-weight:600;'>Ver Tarea →</a></p>"
                : "";

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
                    {botonVerTarea}
                    <hr style='border: none; border-top: 1px solid #dee2e6; margin: 24px 0;'>
                    <p style='color: #adb5bd; font-size: 12px; text-align: center;'>EventHub - Sistema Integral de Gestión de Eventos</p>
                </div>";
        }
    }
}
