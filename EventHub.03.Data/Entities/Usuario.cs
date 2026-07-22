using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub._03.Data.Entities
{
    [Table("tbl_usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("usu_id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("usu_alias")]
        public string Alias { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("usu_email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        [Column("usu_pass_hash_bcrypt")]
        public string PasswordHash { get; set; }

        [Column("usu_pass_hash")]
        public byte[] LegacyPasswordHash { get; set; } = new byte[0];

        [Required]
        [Column("usu_pass_salt")]
        public Guid PasswordSalt { get; set; }

        [Column("usu_pass_cambio")]
        public DateTime PasswordCambio { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("usu_nombre")]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("usu_rol")]
        public string Rol { get; set; }

        [MaxLength(20)]
        [Column("usu_telefono")]
        public string Telefono { get; set; }

        [MaxLength(200)]
        [Column("usu_direccion")]
        public string Direccion { get; set; }

        [Column("usu_fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        [Column("usu_ultimo_acceso")]
        public DateTime? UltimoAcceso { get; set; }

        [Column("usu_intentos")]
        public int Intentos { get; set; }

        [Column("usu_bloqueo")]
        public bool Bloqueo { get; set; }

        [Column("usu_bloqueo_fecha")]
        public DateTime? BloqueoFecha { get; set; }

        [Column("usu_estado")]
        public bool Estado { get; set; }

        [Column("usu_id_creador")]
        public int? IdCreador { get; set; }

        [Column("usu_2fa_enabled")]
        public bool Has2FAEnabled { get; set; }

        [MaxLength(10)]
        [Column("usu_otp_code")]
        public string OtpCode { get; set; }

        [Column("usu_otp_expiry")]
        public DateTime? OtpExpiry { get; set; }

        [MaxLength(100)]
        [Column("usu_otp_secret")]
        public string OtpSecret { get; set; }

        [Column("usu_2fa_recovery_codes")]
        public string TwoFARecoveryCodes { get; set; }

        [MaxLength(100)]
        [Column("usu_recovery_email")]
        public string RecoveryEmail { get; set; }

        [MaxLength(200)]
        [Column("usu_password_reset_token")]
        public string PasswordResetToken { get; set; }

        [Column("usu_password_reset_expiry")]
        public DateTime? PasswordResetExpiry { get; set; }

        [ForeignKey("IdCreador")]
        public virtual Usuario Creador { get; set; }
    }
}
