-- =============================================
-- EventHub Budget Management (tbl_gastos)
-- Run this script on EventHubDB
-- =============================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tbl_gastos')
BEGIN
    CREATE TABLE [dbo].[tbl_gastos] (
        [gas_id] INT IDENTITY(1,1) NOT NULL,
        [gas_eve_id] INT NOT NULL,
        [gas_categoria] NVARCHAR(50) NOT NULL,
        [gas_concepto] NVARCHAR(200) NOT NULL,
        [gas_monto] DECIMAL(18,2) NOT NULL,
        [gas_fecha] DATETIME NOT NULL,
        [gas_proveedor] NVARCHAR(200) NULL,
        [gas_notas] NVARCHAR(MAX) NULL,
        [gas_creado_por] NVARCHAR(100) NULL,
        [gas_fecha_creacion] DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT [PK_tbl_gastos] PRIMARY KEY CLUSTERED ([gas_id] ASC),
        CONSTRAINT [FK_tbl_gastos_tbl_eventos] FOREIGN KEY ([gas_eve_id])
            REFERENCES [dbo].[tbl_eventos] ([eve_id])
    );

    CREATE NONCLUSTERED INDEX [IX_tbl_gastos_EventoId]
        ON [dbo].[tbl_gastos] ([gas_eve_id] ASC);

    PRINT 'Table tbl_gastos created successfully.';
END
ELSE
BEGIN
    PRINT 'Table tbl_gastos already exists.';
END
GO
