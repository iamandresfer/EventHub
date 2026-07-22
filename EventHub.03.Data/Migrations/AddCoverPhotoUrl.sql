-- Migración: Agregar columna eve_cover_photo_url a la tabla tbl_eventos
-- Fecha: 2026-07-21

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_eventos') AND name = 'eve_cover_photo_url')
BEGIN
    ALTER TABLE tbl_eventos
    ADD eve_cover_photo_url NVARCHAR(500) NULL;
    
    PRINT 'Columna eve_cover_photo_url agregada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La columna eve_cover_photo_url ya existe.';
END
GO