USE [EventHubv01]
GO

-- =============================================
-- Script: ALTER_usuario_security.sql
-- Propósito: Agregar campos de seguridad a tbl_usuarios
--   - OTP por email para 2FA
--   - Recuperación de contraseña
--   - Códigos de recuperación 2FA
--   - Cambio a bcrypt (NVARCHAR en lugar de VARBINARY)
-- Fecha: 2026-07-16
-- =============================================

-- 1. Agregar columna para hash bcrypt (NVARCHAR)
ALTER TABLE [dbo].[tbl_usuarios]
ADD [usu_pass_hash_bcrypt] [nvarchar](256) NULL
GO

-- 2. Migrar datos existentes: copiar hash antiguo como respaldo
--    (ejecutar después de que los usuarios tengan hash bcrypt)
-- UPDATE [dbo].[tbl_usuarios] SET usu_pass_hash_bcrypt = CONVERT(NVARCHAR(256), usu_pass_hash) WHERE usu_pass_hash IS NOT NULL
-- GO

-- 3. Campos para OTP / 2FA
ALTER TABLE [dbo].[tbl_usuarios]
ADD
    [usu_2fa_enabled] [bit] NOT NULL CONSTRAINT [DF_usu_2fa_enabled] DEFAULT (0),
    [usu_otp_code] [nvarchar](10) NULL,
    [usu_otp_expiry] [datetime] NULL,
    [usu_otp_secret] [nvarchar](100) NULL
GO

-- 4. Códigos de recuperación para 2FA (JSON array)
ALTER TABLE [dbo].[tbl_usuarios]
ADD [usu_2fa_recovery_codes] [nvarchar](max) NULL
GO

-- 5. Email de respaldo para recuperación
ALTER TABLE [dbo].[tbl_usuarios]
ADD [usu_recovery_email] [nvarchar](100) NULL
GO

-- 6. Campos para reset de contraseña
ALTER TABLE [dbo].[tbl_usuarios]
ADD
    [usu_password_reset_token] [nvarchar](200) NULL,
    [usu_password_reset_expiry] [datetime] NULL
GO

-- 7. Actualizar DEFAULT de usu_pass_hash (opcional, para nuevos registros)
--    Los nuevos registros usarán usu_pass_hash_bcrypt

-- 8. Eventualmente, cuando todos los usuarios migren:
--    DROP COLUMN usu_pass_hash
--    DROP COLUMN usu_pass_salt
--    RENAME COLUMN usu_pass_hash_bcrypt TO usu_pass_hash

PRINT '✅ Campos de seguridad agregados a tbl_usuarios correctamente.'
GO
