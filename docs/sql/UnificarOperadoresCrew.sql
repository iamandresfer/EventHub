-- Migration: Unificar operadores + crew en una sola tabla tbl_crew_operadores
-- Date: 2026-08-08
-- Reversa de MergeOperadoresCrew: la tabla viva (tbl_operadores) pasa a llamarse
-- tbl_crew_operadores y la legacy (cro_*) se elimina (sus 3 filas ya estaban
-- duplicadas en tbl_operadores: Andres=1, Fer=4, test=5).
-- Backups de seguridad.

SELECT * INTO bk_operadores_20260808 FROM tbl_operadores;
SELECT * INTO bk_crew_operadores_20260808 FROM tbl_crew_operadores;
SELECT * INTO bk_tareas_20260808 FROM tbl_tareas;
GO

-- Eliminar tabla legacy y sus FKs
ALTER TABLE tbl_crew_operadores DROP CONSTRAINT FK_crew_evento;
ALTER TABLE tbl_crew_operadores DROP CONSTRAINT FK_crew_operadores_operador;
DROP TABLE tbl_crew_operadores;
GO

-- Renombrar la tabla viva
EXEC sp_rename 'tbl_operadores', 'tbl_crew_operadores';
GO

-- Recrear FKs con nombres consistentes con el nombre de la tabla
ALTER TABLE tbl_tareas DROP CONSTRAINT FK_tarea_operador;
ALTER TABLE tbl_tareas WITH CHECK ADD CONSTRAINT FK_tarea_crew_operador FOREIGN KEY (tar_crew_operador_id) REFERENCES tbl_crew_operadores(ope_id);
ALTER TABLE tbl_crew_operadores DROP CONSTRAINT FK_operador_evento;
ALTER TABLE tbl_crew_operadores WITH CHECK ADD CONSTRAINT FK_crew_operador_evento FOREIGN KEY (ope_eve_id) REFERENCES tbl_eventos(eve_id);
GO
