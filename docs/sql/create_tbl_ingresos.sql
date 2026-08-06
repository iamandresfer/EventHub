-- =============================================
-- EventHub Income Management (tbl_ingresos)
-- Run this script on EventHubv01
-- =============================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tbl_ingresos')
BEGIN
    CREATE TABLE [dbo].[tbl_ingresos] (
        [ing_id] INT IDENTITY(1,1) NOT NULL,
        [ing_eve_id] INT NOT NULL,
        [ing_tipo] NVARCHAR(50) NOT NULL,
        [ing_concepto] NVARCHAR(200) NOT NULL,
        [ing_monto] DECIMAL(18,2) NOT NULL,
        [ing_fecha] DATETIME NOT NULL,
        [ing_cliente] NVARCHAR(200) NULL,
        [ing_notas] NVARCHAR(MAX) NULL,
        [ing_creado_por] NVARCHAR(100) NULL,
        [ing_fecha_creacion] DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT [PK_tbl_ingresos] PRIMARY KEY CLUSTERED ([ing_id] ASC),
        CONSTRAINT [FK_tbl_ingresos_tbl_eventos] FOREIGN KEY ([ing_eve_id])
            REFERENCES [dbo].[tbl_eventos] ([eve_id])
    );

    CREATE NONCLUSTERED INDEX [IX_tbl_ingresos_EventoId]
        ON [dbo].[tbl_ingresos] ([ing_eve_id] ASC);

    PRINT 'Table tbl_ingresos created successfully.';
END
ELSE
BEGIN
    PRINT 'Table tbl_ingresos already exists.';
END
GO
