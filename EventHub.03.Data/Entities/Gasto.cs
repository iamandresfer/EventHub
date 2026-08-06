using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_gastos")]
    public class Gasto
    {
        [Key]
        [Column("gas_id")]
        public int Id { get; set; }

        [Required]
        [Column("gas_eve_id")]
        public int EventoId { get; set; }

        [Required]
        [Column("gas_categoria")]
        [MaxLength(50)]
        public string Categoria { get; set; }

        [Required]
        [Column("gas_concepto")]
        [MaxLength(200)]
        public string Concepto { get; set; }

        [Required]
        [Column("gas_monto")]
        public decimal Monto { get; set; }

        [Column("gas_fecha")]
        public DateTime Fecha { get; set; }

        [Column("gas_proveedor")]
        [MaxLength(200)]
        public string Proveedor { get; set; }

        [Column("gas_notas")]
        public string Notas { get; set; }

        [Column("gas_creado_por")]
        [MaxLength(100)]
        public string CreadoPor { get; set; }

        [Column("gas_fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }
    }
}
