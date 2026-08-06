-- Crear tabla de operadores reutilizables
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_operadores')
BEGIN
    CREATE TABLE tbl_operadores (
        ope_id INT IDENTITY(1,1) PRIMARY KEY,
        ope_nombre NVARCHAR(150) NOT NULL,
        ope_cedula NVARCHAR(20) NULL,
        ope_email NVARCHAR(200) NOT NULL,
        ope_telefono NVARCHAR(20) NULL,
        ope_rol NVARCHAR(100) NULL,
        ope_estado BIT NOT NULL DEFAULT 1,
        ope_fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
    );

    CREATE INDEX IX_operadores_cedula ON tbl_operadores(ope_cedula);
    CREATE INDEX IX_operadores_estado ON tbl_operadores(ope_estado);

    PRINT 'Tabla tbl_operadores creada.';
END
ELSE
    PRINT 'Tabla tbl_operadores ya existe.';

-- Agregar columna operador_id a tbl_crew_operadores
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_crew_operadores') AND name = 'cro_ope_id')
BEGIN
    ALTER TABLE tbl_crew_operadores ADD cro_ope_id INT NULL;

    ALTER TABLE tbl_crew_operadores
        ADD CONSTRAINT FK_crew_operadores_operador
        FOREIGN KEY (cro_ope_id) REFERENCES tbl_operadores(ope_id);

    CREATE INDEX IX_crew_operadores_ope_id ON tbl_crew_operadores(cro_ope_id);

    PRINT 'Columna cro_ope_id agregada a tbl_crew_operadores.';
END
ELSE
    PRINT 'Columna cro_ope_id ya existe.';
