using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub._02.Bussines.DTOs
{
    public class CrewOperadorDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class CrewOperadorFormDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El evento es obligatorio")]
        public int EventoId { get; set; }

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
    }
}
