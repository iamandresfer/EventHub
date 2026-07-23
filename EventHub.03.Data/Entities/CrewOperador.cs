using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_crew_operadores")]
    public class CrewOperador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("cro_id")]
        public int Id { get; set; }

        [Required]
        [Column("cro_eve_id")]
        public int EventoId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("cro_nombre")]
        public string Nombre { get; set; }

        [MaxLength(20)]
        [Column("cro_cedula")]
        public string Cedula { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("cro_email")]
        public string Email { get; set; }

        [MaxLength(20)]
        [Column("cro_telefono")]
        public string Telefono { get; set; }

        [MaxLength(100)]
        [Column("cro_rol")]
        public string Rol { get; set; } // Ej: "DJ", "Sonidista", "Iluminación"

        [Required]
        [Column("cro_estado")]
        public bool Estado { get; set; } = true;

        [Column("cro_fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [MaxLength(50)]
        [Column("cro_num_cuenta")]
        public string NumeroCuenta { get; set; } // Proyección futura: pagos

        [MaxLength(100)]
        [Column("cro_banco")]
        public string Banco { get; set; } // Proyección futura: pagos

        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }
    }
}
