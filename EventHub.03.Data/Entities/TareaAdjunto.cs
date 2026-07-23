using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_tarea_adjuntos")]
    public class TareaAdjunto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("tad_id")]
        public int Id { get; set; }

        [Required]
        [Column("tad_tar_id")]
        public int TareaId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("tad_nombre")]
        public string Nombre { get; set; } // Nombre original del archivo

        [Required]
        [MaxLength(500)]
        [Column("tad_ruta")]
        public string Ruta { get; set; } // Ruta relativa en disco

        [MaxLength(50)]
        [Column("tad_tipo")]
        public string Tipo { get; set; } // image/jpeg, image/png, etc.

        [Column("tad_tamanio")]
        public int? Tamanio { get; set; } // Tamaño en bytes

        [Column("tad_fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [ForeignKey("TareaId")]
        public virtual Tarea Tarea { get; set; }
    }
}
