using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_ingresos")]
    public class Ingreso
    {
        [Key]
        [Column("ing_id")]
        public int Id { get; set; }

        [Required]
        [Column("ing_eve_id")]
        public int EventoId { get; set; }

        [Required]
        [Column("ing_tipo")]
        [MaxLength(50)]
        public string Tipo { get; set; }

        [Required]
        [Column("ing_concepto")]
        [MaxLength(200)]
        public string Concepto { get; set; }

        [Required]
        [Column("ing_monto")]
        public decimal Monto { get; set; }

        [Column("ing_fecha")]
        public DateTime Fecha { get; set; }

        [Column("ing_cliente")]
        [MaxLength(200)]
        public string Cliente { get; set; }

        [Column("ing_notas")]
        public string Notas { get; set; }

        [Column("ing_creado_por")]
        [MaxLength(100)]
        public string CreadoPor { get; set; }

        [Column("ing_fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }
    }
}
