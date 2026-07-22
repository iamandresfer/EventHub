using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_tareas")]
    public class Tarea
    {
        [Key]
        [Column("tar_id")]
        public int Id { get; set; }

        [Required]
        [Column("tar_eve_id")]
        public int EventoId { get; set; }

        [Required]
        [Column("tar_titulo")]
        [MaxLength(200)]
        public string Titulo { get; set; }

        [Column("tar_descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Column("tar_estado")]
        [MaxLength(20)]
        public string Estado { get; set; } // "Pendiente", "EnProgreso", "Completado"

        [Column("tar_fecha_limite")]
        public DateTime? FechaLimite { get; set; }

        [Column("tar_usu_id_asignado")]
        public int? AsignadoAId { get; set; }

        [Required]
        [Column("tar_orden")]
        public int Orden { get; set; }

        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }

        [ForeignKey("AsignadoAId")]
        public virtual Usuario AsignadoA { get; set; }
    }
}
