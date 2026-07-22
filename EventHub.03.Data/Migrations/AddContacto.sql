-- Migración: Agregar columna cli_contacto a la tabla tbl_clientes
-- Fecha: 2026-07-21

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_clientes') AND name = 'cli_contacto')
BEGIN
    ALTER TABLE tbl_clientes
    ADD cli_contacto NVARCHAR(150) NULL;
    
    PRINT 'Columna cli_contacto agregada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La columna cli_contacto ya existe.';
END
GO