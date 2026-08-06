using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_operadores")]
    public class Operador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ope_id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("ope_nombre")]
        public string Nombre { get; set; }

        [MaxLength(20)]
        [Column("ope_cedula")]
        public string Cedula { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("ope_email")]
        public string Email { get; set; }

        [MaxLength(20)]
        [Column("ope_telefono")]
        public string Telefono { get; set; }

        [MaxLength(100)]
        [Column("ope_rol")]
        public string Rol { get; set; }

        [Column("ope_estado")]
        public bool Estado { get; set; } = true;

        [Column("ope_fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [MaxLength(500)]
        [Column("ope_foto_url")]
        public string FotoUrl { get; set; }

        [Column("ope_eve_id")]
        public int? EventoId { get; set; }

        [MaxLength(50)]
        [Column("ope_num_cuenta")]
        public string NumeroCuenta { get; set; }

        [MaxLength(100)]
        [Column("ope_banco")]
        public string Banco { get; set; }

        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }
    }
}
