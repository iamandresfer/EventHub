-- ============================================
-- Migración: Índices de rendimiento
-- Fecha: 2026-07-22
-- ============================================

-- 1. Índices para búsquedas de clientes
CREATE INDEX IX_clientes_nombre ON tbl_clientes(cli_nombre_comercial);
CREATE INDEX IX_clientes_ruc ON tbl_clientes(cli_ruc);
CREATE INDEX IX_clientes_email ON tbl_clientes(cli_email_principal);
CREATE INDEX IX_clientes_estado ON tbl_clientes(cli_estado);

-- 2. Índices para búsquedas de eventos
CREATE INDEX IX_eventos_nombre ON tbl_eventos(eve_nombre);
CREATE INDEX IX_eventos_codigo ON tbl_eventos(eve_codigo);
CREATE INDEX IX_eventos_estado ON tbl_eventos(eve_estado);
CREATE INDEX IX_eventos_fecha ON tbl_eventos(eve_fecha_inicio);

-- 3. Índices para tareas (kanban)
CREATE INDEX IX_tareas_estado ON tbl_tareas(tar_estado);
CREATE INDEX IX_tareas_evento ON tbl_tareas(tar_eve_id);
CREATE INDEX IX_tareas_asignado ON tbl_tareas(tar_usu_id_asignado);
CREATE INDEX IX_tareas_crew ON tbl_tareas(tar_crew_operador_id);
