-- Migration: Add Categoria, FotoUrl columns
-- Date: 2026-07-24

-- Add Categoria to tbl_tareas
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tareas') AND name = 'tar_categoria')
BEGIN
    ALTER TABLE tbl_tareas ADD tar_categoria NVARCHAR(50) NULL;
END
GO

-- Add FotoUrl to tbl_operadores
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_operadores') AND name = 'ope_foto_url')
BEGIN
    ALTER TABLE tbl_operadores ADD ope_foto_url NVARCHAR(500) NULL;
END
GO

-- Add FotoUrl to tbl_crew_operadores
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_crew_operadores') AND name = 'cro_foto_url')
BEGIN
    ALTER TABLE tbl_crew_operadores ADD cro_foto_url NVARCHAR(500) NULL;
END
GO
