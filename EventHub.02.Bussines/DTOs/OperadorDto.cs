using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class OperadorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string FotoUrl { get; set; }
        public int? EventoId { get; set; }
    }

    public class OperadorFormDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
        public string Nombre { get; set; }

        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string Telefono { get; set; }

        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Rol { get; set; }

        public string FotoUrl { get; set; }
    }

    /// <summary>
    /// DTO enriquecido que incluye los eventos donde el operador está asignado como crew.
    /// </summary>
    public class OperadorConEventosDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<OperadorEventoDto> Eventos { get; set; } = new List<OperadorEventoDto>();
    }

    /// <summary>
    /// Relacion operador → evento (ope_eve_id en tbl_operadores).
    /// </summary>
    public class OperadorEventoDto
    {
        public int EventoId { get; set; }
        public string EventoNombre { get; set; }
        public string EventoCodigo { get; set; }
        public bool Estado { get; set; }
        public string Rol { get; set; }
    }
}
