using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class EventoFormDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del evento es obligatorio.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
        [Display(Name = "Nombre del Evento")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente válido.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El lugar es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un lugar válido.")]
        [Display(Name = "Lugar")]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "El tipo de evento es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo de evento válido.")]
        [Display(Name = "Tipo de Evento")]
        public int TipoEventoId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        [Display(Name = "Hora Inicio")]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        [Display(Name = "Hora Fin")]
        [DataType(DataType.Time)]
        public TimeSpan HoraFin { get; set; }

        [Required(ErrorMessage = "El presupuesto estimado es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El presupuesto debe ser mayor a 0.")]
        [Display(Name = "Presupuesto Estimado")]
        [DataType(DataType.Currency)]
        public decimal PresupuestoEstimado { get; set; }

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres.")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Display(Name = "Es Varios Días")]
        public bool EsVariosDias { get; set; }

        [Display(Name = "Foto de Portada")]
        public string CoverPhotoUrl { get; set; }
    }
}
