-- ============================================
-- Migración: Crew Operadores, Notificaciones y Adjuntos de Tareas
-- Fecha: 2026-07-22
-- ============================================

-- 1. Tabla de Crew Operadores (personal operativo por evento)
CREATE TABLE tbl_crew_operadores (
    cro_id          INT IDENTITY(1,1) PRIMARY KEY,
    cro_eve_id      INT NOT NULL,
    cro_nombre      NVARCHAR(150) NOT NULL,
    cro_cedula      NVARCHAR(20) NULL,
    cro_email       NVARCHAR(200) NOT NULL,
    cro_telefono    NVARCHAR(20) NULL,
    cro_rol         NVARCHAR(100) NULL,          -- Ej: "DJ", "Sonidista", "Iluminación"
    cro_estado      BIT NOT NULL DEFAULT 1,
    cro_fecha_creacion DATETIME2 NOT NULL DEFAULT GETDATE(),
    cro_num_cuenta  NVARCHAR(50) NULL,           -- Proyección futura: pagos
    cro_banco       NVARCHAR(100) NULL,          -- Proyección futura: pagos
    
    CONSTRAINT FK_crew_evento FOREIGN KEY (cro_eve_id) REFERENCES tbl_eventos(eve_id)
);

CREATE INDEX IX_crew_evento ON tbl_crew_operadores(cro_eve_id);

-- 2. Tabla de Notificaciones
CREATE TABLE tbl_notificaciones (
    not_id              INT IDENTITY(1,1) PRIMARY KEY,
    not_tipo            NVARCHAR(50) NOT NULL,    -- TareaCreada, TareaCompletada, TareaVencida, FechaModificada
    not_mensaje         NVARCHAR(MAX) NOT NULL,
    not_email_destino   NVARCHAR(200) NOT NULL,
    not_nombre_destino  NVARCHAR(150) NOT NULL,
    not_eve_id          INT NULL,
    not_tar_id          INT NULL,
    not_leida           BIT NOT NULL DEFAULT 0,
    not_enviada         BIT NOT NULL DEFAULT 0,
    not_fecha_creacion  DATETIME2 NOT NULL DEFAULT GETDATE(),
    not_fecha_envio     DATETIME2 NULL,
    not_error           NVARCHAR(500) NULL,
    
    CONSTRAINT FK_not_evento FOREIGN KEY (not_eve_id) REFERENCES tbl_eventos(eve_id),
    CONSTRAINT FK_not_tarea FOREIGN KEY (not_tar_id) REFERENCES tbl_tareas(tar_id)
);

CREATE INDEX IX_notificaciones_evento ON tbl_notificaciones(not_eve_id);
CREATE INDEX IX_notificaciones_fecha ON tbl_notificaciones(not_fecha_creacion);

-- 3. Tabla de Adjuntos de Tareas (imágenes)
CREATE TABLE tbl_tarea_adjuntos (
    tad_id          INT IDENTITY(1,1) PRIMARY KEY,
    tad_tar_id      INT NOT NULL,
    tad_nombre      NVARCHAR(200) NOT NULL,       -- Nombre original del archivo
    tad_ruta        NVARCHAR(500) NOT NULL,        -- Ruta relativa en disco
    tad_tipo        NVARCHAR(50) NULL,             -- image/jpeg, image/png, etc.
    tad_tamanio     INT NULL,                      -- Tamaño en bytes
    tad_fecha_creacion DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_adjunto_tarea FOREIGN KEY (tad_tar_id) REFERENCES tbl_tareas(tar_id)
);

CREATE INDEX IX_adjuntos_tarea ON tbl_tarea_adjuntos(tad_tar_id);

-- 4. Alter tbl_tareas: agregar campo creado_por y cambiar FK a crew
ALTER TABLE tbl_tareas ADD tar_creado_por_id INT NULL;
ALTER TABLE tbl_tareas ADD tar_crew_operador_id INT NULL;

-- La columna tar_usu_id_asignado se mantiene por compatibilidad,
-- pero ahora usaremos tar_crew_operador_id para asignar a crew members
-- FK al crew operador
ALTER TABLE tbl_tareas ADD CONSTRAINT FK_tarea_crew 
    FOREIGN KEY (tar_crew_operador_id) REFERENCES tbl_crew_operadores(cro_id);
