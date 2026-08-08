using System.Collections.Generic;

namespace EventHub._02.Bussines.DTOs
{
    public class DashboardDto
    {
        public int TotalEventos { get; set; }
        public int EventosActivos { get; set; }
        public int EventosPlanificacion { get; set; }
        public int EventosEjecucion { get; set; }
        public int EventosFinalizados { get; set; }
        public int TotalClientes { get; set; }
        public int ClientesActivos { get; set; }
        public int TotalOperadores { get; set; }
        public int OperadoresActivos { get; set; }
        public decimal TotalPresupuestoEstimado { get; set; }
        public decimal TotalGastado { get; set; }
        public decimal TotalRecaudado { get; set; }
        public decimal? EjecucionGasto { get; set; }
        public List<EventoListDto> ProximosEventos { get; set; }
        public List<EventoListDto> EventosFinalizadosRecientes { get; set; } = new List<EventoListDto>();
        public List<System.DateTime> EventosActividad { get; set; } = new List<System.DateTime>();
        public List<TareaHoyDto> TareasHoy { get; set; } = new List<TareaHoyDto>();
    }
}
