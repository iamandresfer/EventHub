using System;

namespace EventHub._02.Bussines.DTOs
{
    public class NotificacionDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Mensaje { get; set; }
        public string EmailDestino { get; set; }
        public string NombreDestino { get; set; }
        public int? EventoId { get; set; }
        public int? TareaId { get; set; }
        public bool Leida { get; set; }
        public bool Enviada { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string EventoNombre { get; set; }
        public string TareaTitulo { get; set; }
    }
}
