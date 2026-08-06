-- Migration: Unify tbl_operadores + tbl_crew_operadores
-- Date: 2026-08-04

-- 1. Add event FK to tbl_operadores
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_operadores') AND name = 'ope_eve_id')
BEGIN
    ALTER TABLE tbl_operadores ADD ope_eve_id INT NULL;
    ALTER TABLE tbl_operadores ADD CONSTRAINT FK_operador_evento
        FOREIGN KEY (ope_eve_id) REFERENCES tbl_eventos(eve_id);
    CREATE INDEX IX_operador_evento ON tbl_operadores(ope_eve_id);
END
GO

-- 2. Add missing columns from CrewOperador to Operador
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_operadores') AND name = 'ope_num_cuenta')
    ALTER TABLE tbl_operadores ADD ope_num_cuenta NVARCHAR(50) NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_operadores') AND name = 'ope_banco')
    ALTER TABLE tbl_operadores ADD ope_banco NVARCHAR(100) NULL;
GO

-- 3. Migrate data: for each CrewOperador with a linked Operador, update the Operador with event assignment
UPDATE o
SET o.ope_eve_id = c.cro_eve_id,
    o.ope_num_cuenta = c.cro_num_cuenta,
    o.ope_banco = c.cro_banco
FROM tbl_operadores o
INNER JOIN tbl_crew_operadores c ON o.ope_id = c.cro_ope_id
WHERE c.cro_ope_id IS NOT NULL;
GO

-- 4. For CrewOperadores without a linked Operador, create a new Operador record
INSERT INTO tbl_operadores (ope_nombre, ope_cedula, ope_email, ope_telefono, ope_rol, ope_estado, ope_fecha_creacion, ope_foto_url, ope_eve_id, ope_num_cuenta, ope_banco)
SELECT c.cro_nombre, c.cro_cedula, c.cro_email, c.cro_telefono, c.cro_rol, c.cro_estado, c.cro_fecha_creacion, c.cro_foto_url, c.cro_eve_id, c.cro_num_cuenta, c.cro_banco
FROM tbl_crew_operadores c
WHERE c.cro_ope_id IS NULL;
GO

-- 5. Drop old FK from tbl_tareas to tbl_crew_operadores
DECLARE @fkName NVARCHAR(200);
SELECT @fkName = name FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('tbl_tareas')
  AND referenced_object_id = OBJECT_ID('tbl_crew_operadores');
IF @fkName IS NOT NULL
    EXEC('ALTER TABLE tbl_tareas DROP CONSTRAINT ' + @fkName);
GO

-- 6. Update tbl_tareas references: CrewOperador.Id -> the linked Operador.Id
UPDATE t
SET t.tar_crew_operador_id = o.ope_id
FROM tbl_tareas t
INNER JOIN tbl_crew_operadores c ON t.tar_crew_operador_id = c.cro_id
INNER JOIN tbl_operadores o ON c.cro_ope_id = o.ope_id
WHERE c.cro_ope_id IS NOT NULL;
GO

-- 7. Add new FK from tbl_tareas to tbl_operadores
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_tarea_operador')
BEGIN
    ALTER TABLE tbl_tareas ADD CONSTRAINT FK_tarea_operador
        FOREIGN KEY (tar_crew_operador_id) REFERENCES tbl_operadores(ope_id);
END
GO

PRINT 'Migration complete. Verify data before dropping tbl_crew_operadores.';
GO
