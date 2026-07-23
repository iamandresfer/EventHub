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
    }
}
