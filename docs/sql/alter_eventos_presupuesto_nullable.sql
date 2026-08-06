-- =============================================
-- EventHub: make event budget optional + total income column
-- Run this script on EventHubv01 AFTER create_tbl_ingresos.sql
-- =============================================

-- 1. Drop the unnamed CHECK constraint on eve_presupuesto_estimado (> 0)
IF EXISTS (SELECT * FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID('dbo.tbl_eventos'))
BEGIN
    DECLARE @chkName NVARCHAR(200);
    SELECT @chkName = cc.name
    FROM sys.check_constraints cc
    JOIN sys.columns c
        ON c.object_id = cc.parent_object_id
       AND c.column_id = cc.parent_column_id
    WHERE cc.parent_object_id = OBJECT_ID('dbo.tbl_eventos')
      AND c.name = 'eve_presupuesto_estimado';

    IF @chkName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE [dbo].[tbl_eventos] DROP CONSTRAINT [' + @chkName + ']');
        PRINT 'Dropped CHECK constraint: ' + @chkName;
    END
    ELSE
    BEGIN
        PRINT 'No CHECK constraint found on eve_presupuesto_estimado.';
    END
END
GO

-- 2. Make eve_presupuesto_estimado nullable (budget is now optional)
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'tbl_eventos' AND COLUMN_NAME = 'eve_presupuesto_estimado'
      AND IS_NULLABLE = 'NO'
)
BEGIN
    ALTER TABLE [dbo].[tbl_eventos] ALTER COLUMN [eve_presupuesto_estimado] DECIMAL(18,2) NULL;
    PRINT 'Column eve_presupuesto_estimado is now NULLABLE.';
END
ELSE
BEGIN
    PRINT 'Column eve_presupuesto_estimado is already NULLABLE (or missing).';
END
GO

-- 3. Add denormalized total income column (mirrors eve_gasto_real)
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'tbl_eventos' AND COLUMN_NAME = 'eve_total_ingresos'
)
BEGIN
    ALTER TABLE [dbo].[tbl_eventos]
        ADD [eve_total_ingresos] DECIMAL(18,2) NULL
        CONSTRAINT [DF_tbl_eventos_eve_total_ingresos] DEFAULT (0);
    PRINT 'Column eve_total_ingresos added.';
END
ELSE
BEGIN
    PRINT 'Column eve_total_ingresos already exists.';
END
GO
