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
        public List<EventoListDto> ProximosEventos { get; set; }
    }
}
