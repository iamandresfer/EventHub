using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class EventoListDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }

        [Display(Name = "Evento")]
        public string Nombre { get; set; }

        [Display(Name = "Cliente")]
        public string ClienteNombre { get; set; }

        [Display(Name = "Lugar")]
        public string VenueNombre { get; set; }

        [Display(Name = "Tipo")]
        public string TipoEvento { get; set; }

        [Display(Name = "Inicio")]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Presupuesto")]
        public decimal? PresupuestoEstimado { get; set; }

        [Display(Name = "Gasto Real")]
        public decimal? GastoReal { get; set; }

        [Display(Name = "Total Ingresos")]
        public decimal? TotalIngresos { get; set; }

        [Display(Name = "Foto de Portada")]
        public string CoverPhotoUrl { get; set; }

        public int ClienteId { get; set; }
        public int VenueId { get; set; }
        public int TipoEventoId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Descripcion { get; set; }
    }
}
