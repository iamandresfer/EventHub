using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_clientes")]
    public class Cliente
    {
        [Key]
        [Column("cli_id")]
        public int Id { get; set; }

        [Required]
        [Column("cli_razon_social")]
        [MaxLength(150)]
        public string Nombre { get; set; }

        [Column("cli_nombre_comercial")]
        [MaxLength(150)]
        public string NombreComercial { get; set; }

        [Required]
        [Column("cli_ruc")]
        [MaxLength(13)]
        public string Ruc { get; set; }

        [Required]
        [Column("cli_email_principal")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Column("cli_telefono_principal")]
        [MaxLength(20)]
        public string Telefono { get; set; }

        [Column("cli_direccion_fiscal")]
        [MaxLength(200)]
        public string Direccion { get; set; }

        [Column("cli_contacto")]
        [MaxLength(150)]
        public string Contacto { get; set; }

        [Column("cli_sitio_web")]
        [MaxLength(100)]
        public string SitioWeb { get; set; }

        [Column("cli_logo_url")]
        [MaxLength(500)]
        public string LogoUrl { get; set; }

        [Column("cli_clasificacion")]
        [MaxLength(20)]
        public string Clasificacion { get; set; }

        [Column("cli_notas_internas")]
        public string NotasInternas { get; set; }

        [Required]
        [Column("cli_fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [Column("cli_estado")]
        public bool Estado { get; set; }

        [Required]
        [Column("cli_usu_id_creador")]
        public int UsuarioCreadorId { get; set; }
    }
}
