using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_venues")]
    public class Venue
    {
        [Key]
        [Column("ven_id")]
        public int Id { get; set; }

        [Required]
        [Column("ven_nombre")]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [Column("ven_direccion")]
        [MaxLength(200)]
        public string Direccion { get; set; }

        [Required]
        [Column("ven_ciudad")]
        [MaxLength(50)]
        public string Ciudad { get; set; }

        [Column("ven_capacidad_maxima")]
        public int? CapacidadMaxima { get; set; }

        [Column("ven_estado")]
        [MaxLength(20)]
        public string Estado { get; set; }
    }
}
