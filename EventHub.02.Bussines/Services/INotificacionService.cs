using System.Collections.Generic;
using EventHub._02.Bussines.DTOs;

namespace EventHub._02.Bussines.Services
{
    public interface INotificacionService
    {
        List<NotificacionDto> ObtenerPorEvento(int eventoId);
        List<NotificacionDto> ObtenerRecientes(int top = 20);
        bool MarcarComoLeida(int notificacionId);
        int ContarNoLeidas();
        void CrearYEnviar(string tipo, string mensaje, string emailDestino, string nombreDestino,
            int? eventoId, int? tareaId, IEmailService emailService, string nombreEvento = null, string tareaTitulo = null);
    }
}
