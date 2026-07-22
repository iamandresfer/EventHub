using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_eventos")]
    public class Evento
    {
        [Key]
        [Column("eve_id")]
        public int Id { get; set; }

        [Required]
        [Column("eve_codigo")]
        [MaxLength(20)]
        public string Codigo { get; set; }

        [Required]
        [Column("eve_nombre")]
        [MaxLength(150)]
        public string Nombre { get; set; }

        [Required]
        [Column("eve_cli_id")]
        public int ClienteId { get; set; }

        [Required]
        [Column("eve_ven_id")]
        public int VenueId { get; set; }

        [Required]
        [Column("eve_tev_id")]
        public int TipoEventoId { get; set; }

        [Required]
        [Column("eve_fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Column("eve_fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Required]
        [Column("eve_hora_inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [Column("eve_hora_fin")]
        public TimeSpan HoraFin { get; set; }

        [Required]
        [Column("eve_presupuesto_estimado")]
        public decimal PresupuestoEstimado { get; set; }

        [Column("eve_gasto_real")]
        public decimal? GastoReal { get; set; }

        [Required]
        [Column("eve_estado")]
        [MaxLength(20)]
        public string Estado { get; set; }

        [Column("eve_descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Column("eve_fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("eve_fecha_cierre")]
        public DateTime? FechaCierre { get; set; }

        [Required]
        [Column("eve_usu_id_creador")]
        public int UsuarioCreadorId { get; set; }

        [Required]
        [Column("eve_usu_id_productor_general")]
        public int ProductorGeneralId { get; set; }

        [Column("eve_cover_photo_url")]
        [MaxLength(500)]
        public string CoverPhotoUrl { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("VenueId")]
        public virtual Venue Venue { get; set; }

        [ForeignKey("TipoEventoId")]
        public virtual TipoEvento TipoEvento { get; set; }

        public virtual ICollection<Tarea> Tareas { get; set; }
    }
}
