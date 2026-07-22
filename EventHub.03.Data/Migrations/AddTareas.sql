CREATE TABLE [dbo].[tbl_tareas] (
    [tar_id] INT IDENTITY(1,1) NOT NULL,
    [tar_eve_id] INT NOT NULL,
    [tar_titulo] NVARCHAR(200) NOT NULL,
    [tar_descripcion] NVARCHAR(MAX) NULL,
    [tar_estado] NVARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    [tar_fecha_limite] DATETIME NULL,
    [tar_usu_id_asignado] INT NULL,
    [tar_orden] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_tbl_tareas] PRIMARY KEY CLUSTERED ([tar_id] ASC),
    CONSTRAINT [FK_tbl_tareas_tbl_eventos] FOREIGN KEY ([tar_eve_id]) REFERENCES [dbo].[tbl_eventos] ([eve_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_tareas_tbl_usuarios] FOREIGN KEY ([tar_usu_id_asignado]) REFERENCES [dbo].[tbl_usuarios] ([usu_id])
);
GO

CREATE NONCLUSTERED INDEX [IX_tbl_tareas_tar_eve_id] ON [dbo].[tbl_tareas]([tar_eve_id] ASC);
GO
