using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_tipos_evento")]
    public class TipoEvento
    {
        [Key]
        [Column("tev_id")]
        public int Id { get; set; }

        [Required]
        [Column("tev_nombre")]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Column("tev_descripcion")]
        [MaxLength(200)]
        public string Descripcion { get; set; }
    }
}
