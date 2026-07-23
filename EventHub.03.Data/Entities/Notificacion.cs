using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_notificaciones")]
    public class Notificacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("not_id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("not_tipo")]
        public string Tipo { get; set; } // TareaCreada, TareaCompletada, TareaVencida, FechaModificada

        [Required]
        [Column("not_mensaje")]
        public string Mensaje { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("not_email_destino")]
        public string EmailDestino { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("not_nombre_destino")]
        public string NombreDestino { get; set; }

        [Column("not_eve_id")]
        public int? EventoId { get; set; }

        [Column("not_tar_id")]
        public int? TareaId { get; set; }

        [Required]
        [Column("not_leida")]
        public bool Leida { get; set; } = false;

        [Required]
        [Column("not_enviada")]
        public bool Enviada { get; set; } = false;

        [Column("not_fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("not_fecha_envio")]
        public DateTime? FechaEnvio { get; set; }

        [MaxLength(500)]
        [Column("not_error")]
        public string Error { get; set; }

        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }

        [ForeignKey("TareaId")]
        public virtual Tarea Tarea { get; set; }
    }
}
