USE [master]
GO
/****** Objeto: Database [EventHubv01] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE DATABASE [EventHubv01]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'EventHubv01', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\EventHubv01.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'EventHubv01_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\EventHubv01_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [EventHubv01] SET COMPATIBILITY_LEVEL = 170
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [EventHubv01].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [EventHubv01] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [EventHubv01] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [EventHubv01] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [EventHubv01] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [EventHubv01] SET ARITHABORT OFF 
GO
ALTER DATABASE [EventHubv01] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [EventHubv01] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [EventHubv01] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [EventHubv01] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [EventHubv01] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [EventHubv01] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [EventHubv01] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [EventHubv01] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [EventHubv01] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [EventHubv01] SET  ENABLE_BROKER 
GO
ALTER DATABASE [EventHubv01] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [EventHubv01] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [EventHubv01] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [EventHubv01] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [EventHubv01] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [EventHubv01] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [EventHubv01] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [EventHubv01] SET RECOVERY FULL 
GO
ALTER DATABASE [EventHubv01] SET  MULTI_USER 
GO
ALTER DATABASE [EventHubv01] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [EventHubv01] SET DB_CHAINING OFF 
GO
ALTER DATABASE [EventHubv01] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [EventHubv01] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [EventHubv01] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [EventHubv01] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [EventHubv01] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [EventHubv01] SET QUERY_STORE = ON
GO
ALTER DATABASE [EventHubv01] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [EventHubv01]
GO
/****** Objeto: Table [dbo].[tbl_actividades] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_actividades](
	[act_id] [int] IDENTITY(1,1) NOT NULL,
	[act_eve_id] [int] NOT NULL,
	[act_fev_id] [int] NULL,
	[act_zpl_id] [int] NULL,
	[act_titulo] [nvarchar](150) NOT NULL,
	[act_descripcion] [nvarchar](max) NULL,
	[act_fecha_hora_estimada_inicio] [datetime] NOT NULL,
	[act_fecha_hora_estimada_fin] [datetime] NOT NULL,
	[act_duracion_estimada_minutos]  AS (datediff(minute,[act_fecha_hora_estimada_inicio],[act_fecha_hora_estimada_fin])) PERSISTED,
	[act_fecha_hora_real_inicio] [datetime] NULL,
	[act_fecha_hora_real_fin] [datetime] NULL,
	[act_estado] [nvarchar](20) NOT NULL,
	[act_prioridad] [nvarchar](10) NOT NULL,
	[act_usu_id_responsable] [int] NOT NULL,
	[act_id_dependiente] [int] NULL,
	[act_notas_campo] [nvarchar](max) NULL,
	[act_requiere_conexion] [bit] NOT NULL,
	[act_fecha_creacion] [datetime] NOT NULL,
	[act_fecha_ultima_modificacion] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[act_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_asignacion_inventario] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_asignacion_inventario](
	[asi_id] [int] IDENTITY(1,1) NOT NULL,
	[asi_eve_id] [int] NOT NULL,
	[asi_inv_id] [int] NOT NULL,
	[asi_usu_id_responsable] [int] NOT NULL,
	[asi_cantidad_asignada] [int] NOT NULL,
	[asi_fecha_hora_salida] [datetime] NOT NULL,
	[asi_fecha_hora_entrada] [datetime] NULL,
	[asi_estado_salida] [nvarchar](20) NOT NULL,
	[asi_estado_entrada] [nvarchar](20) NULL,
	[asi_observacion_salida] [nvarchar](max) NULL,
	[asi_observacion_entrada] [nvarchar](max) NULL,
	[asi_es_devolucion_completa] [bit] NOT NULL,
	[asi_fecha_registro] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[asi_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_bitacora] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_bitacora](
	[bit_id] [int] IDENTITY(1,1) NOT NULL,
	[bit_usu_id] [int] NOT NULL,
	[bit_accion] [nvarchar](50) NOT NULL,
	[bit_tabla_afectada] [nvarchar](50) NULL,
	[bit_id_registro_afectado] [int] NULL,
	[bit_detalle] [nvarchar](max) NULL,
	[bit_ip_origen] [nvarchar](45) NULL,
	[bit_fecha_hora] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[bit_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_checklist_desmontaje] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_checklist_desmontaje](
	[cde_id] [int] IDENTITY(1,1) NOT NULL,
	[cde_eve_id] [int] NOT NULL,
	[cde_inv_id] [int] NULL,
	[cde_item_descripcion] [nvarchar](200) NOT NULL,
	[cde_cantidad_esperada] [int] NOT NULL,
	[cde_cantidad_encontrada] [int] NULL,
	[cde_estado_verificacion] [nvarchar](20) NOT NULL,
	[cde_observacion] [nvarchar](max) NULL,
	[cde_fecha_hora_verificacion] [datetime] NULL,
	[cde_usu_id_responsable] [int] NOT NULL,
	[cde_fecha_registro] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cde_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_clientes] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_clientes](
	[cli_id] [int] IDENTITY(1,1) NOT NULL,
	[cli_razon_social] [nvarchar](150) NOT NULL,
	[cli_nombre_comercial] [nvarchar](150) NULL,
	[cli_ruc] [char](13) NOT NULL,
	[cli_email_principal] [nvarchar](100) NOT NULL,
	[cli_telefono_principal] [nvarchar](20) NULL,
	[cli_direccion_fiscal] [nvarchar](200) NULL,
	[cli_sitio_web] [nvarchar](100) NULL,
	[cli_logo_url] [nvarchar](500) NULL,
	[cli_clasificacion] [nvarchar](20) NULL,
	[cli_notas_internas] [nvarchar](max) NULL,
	[cli_fecha_registro] [datetime] NOT NULL,
	[cli_estado] [bit] NOT NULL,
	[cli_usu_id_creador] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cli_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[cli_ruc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_contactos_cliente] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_contactos_cliente](
	[cco_id] [int] IDENTITY(1,1) NOT NULL,
	[cco_cli_id] [int] NOT NULL,
	[cco_nombre_completo] [nvarchar](100) NOT NULL,
	[cco_cargo] [nvarchar](50) NULL,
	[cco_email] [nvarchar](100) NULL,
	[cco_telefono] [nvarchar](20) NULL,
	[cco_es_principal] [bit] NOT NULL,
	[cco_preferencia_contacto] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[cco_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_cuentas_bancarias_cliente] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_cuentas_bancarias_cliente](
	[cbc_id] [int] IDENTITY(1,1) NOT NULL,
	[cbc_cli_id] [int] NOT NULL,
	[cbc_banco] [nvarchar](50) NOT NULL,
	[cbc_tipo_cuenta] [nvarchar](20) NOT NULL,
	[cbc_numero_cuenta] [nvarchar](30) NOT NULL,
	[cbc_titular] [nvarchar](100) NOT NULL,
	[cbc_es_predeterminada] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cbc_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_eventos] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_eventos](
	[eve_id] [int] IDENTITY(1,1) NOT NULL,
	[eve_codigo] [nvarchar](20) NOT NULL,
	[eve_nombre] [nvarchar](150) NOT NULL,
	[eve_cli_id] [int] NOT NULL,
	[eve_ven_id] [int] NOT NULL,
	[eve_tev_id] [int] NOT NULL,
	[eve_fecha_inicio] [date] NOT NULL,
	[eve_fecha_fin] [date] NOT NULL,
	[eve_hora_inicio] [time](7) NOT NULL,
	[eve_hora_fin] [time](7) NOT NULL,
	[eve_presupuesto_estimado] [decimal](18, 2) NOT NULL,
	[eve_gasto_real] [decimal](18, 2) NULL,
	[eve_estado] [nvarchar](20) NOT NULL,
	[eve_descripcion] [nvarchar](max) NULL,
	[eve_notas_internas] [nvarchar](max) NULL,
	[eve_fecha_creacion] [datetime] NOT NULL,
	[eve_fecha_cierre] [datetime] NULL,
	[eve_usu_id_creador] [int] NOT NULL,
	[eve_usu_id_productor_general] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[eve_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[eve_codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_fases_evento] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_fases_evento](
	[fev_id] [int] IDENTITY(1,1) NOT NULL,
	[fev_eve_id] [int] NOT NULL,
	[fev_nombre_fase] [nvarchar](50) NOT NULL,
	[fev_descripcion] [nvarchar](200) NULL,
	[fev_orden] [int] NOT NULL,
	[fev_fecha_inicio_estimada] [datetime] NOT NULL,
	[fev_fecha_fin_estimada] [datetime] NOT NULL,
	[fev_fecha_inicio_real] [datetime] NULL,
	[fev_fecha_fin_real] [datetime] NULL,
	[fev_estado_fase] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[fev_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_fev_orden] UNIQUE NONCLUSTERED 
(
	[fev_eve_id] ASC,
	[fev_orden] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_fotos_inventario] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_fotos_inventario](
	[foi_id] [int] IDENTITY(1,1) NOT NULL,
	[foi_inv_id] [int] NOT NULL,
	[foi_url_foto] [nvarchar](500) NOT NULL,
	[foi_es_principal] [bit] NOT NULL,
	[foi_fecha_subida] [datetime] NOT NULL,
	[foi_usu_id_subio] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[foi_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_intentos_login] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_intentos_login](
	[inl_id] [int] IDENTITY(1,1) NOT NULL,
	[inl_email] [nvarchar](100) NOT NULL,
	[inl_ip_origen] [nvarchar](45) NULL,
	[inl_fecha_hora] [datetime] NOT NULL,
	[inl_fue_exitoso] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[inl_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_inventario] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_inventario](
	[inv_id] [int] IDENTITY(1,1) NOT NULL,
	[inv_nombre] [nvarchar](100) NOT NULL,
	[inv_categoria] [nvarchar](30) NOT NULL,
	[inv_modelo] [nvarchar](50) NULL,
	[inv_marca] [nvarchar](50) NULL,
	[inv_numero_serie] [nvarchar](50) NULL,
	[inv_codigo_interno] [nvarchar](50) NOT NULL,
	[inv_estado_item] [nvarchar](20) NOT NULL,
	[inv_valor_reposicion] [decimal](18, 2) NULL,
	[inv_foto_url] [nvarchar](500) NULL,
	[inv_propietario] [nvarchar](20) NOT NULL,
	[inv_pro_id] [int] NULL,
	[inv_observaciones] [nvarchar](max) NULL,
	[inv_stock_actual] [int] NOT NULL,
	[inv_stock_minimo] [int] NOT NULL,
	[inv_fecha_registro] [datetime] NOT NULL,
	[inv_usu_id_creador] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[inv_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[inv_codigo_interno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_metricas_evento] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_metricas_evento](
	[met_id] [int] IDENTITY(1,1) NOT NULL,
	[met_eve_id] [int] NOT NULL,
	[met_duracion_real_horas] [decimal](5, 2) NULL,
	[met_costo_real] [decimal](18, 2) NULL,
	[met_numero_actividades] [int] NULL,
	[met_numero_actividades_completadas] [int] NULL,
	[met_porcentaje_cumplimiento]  AS (case when [met_numero_actividades]>(0) then ([met_numero_actividades_completadas]*(100.0))/[met_numero_actividades] else (0) end) PERSISTED,
	[met_numero_incidencias] [int] NULL,
	[met_calificacion_cliente] [int] NULL,
	[met_observaciones] [nvarchar](max) NULL,
	[met_fecha_calculo] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[met_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_pagos] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_pagos](
	[pag_id] [int] IDENTITY(1,1) NOT NULL,
	[pag_eve_id] [int] NOT NULL,
	[pag_pro_id] [int] NOT NULL,
	[pag_concepto] [nvarchar](200) NOT NULL,
	[pag_monto_contratado] [decimal](18, 2) NOT NULL,
	[pag_anticipo_pagado] [decimal](18, 2) NOT NULL,
	[pag_saldo_pendiente]  AS ([pag_monto_contratado]-[pag_anticipo_pagado]) PERSISTED,
	[pag_fecha_pago] [date] NOT NULL,
	[pag_estado_pago] [nvarchar](20) NOT NULL,
	[pag_comprobante_url] [nvarchar](500) NULL,
	[pag_notas] [nvarchar](max) NULL,
	[pag_fecha_registro] [datetime] NOT NULL,
	[pag_usu_id_registro] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[pag_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_personal_zona] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_personal_zona](
	[pzo_id] [int] IDENTITY(1,1) NOT NULL,
	[pzo_zev_id] [int] NOT NULL,
	[pzo_usu_id] [int] NOT NULL,
	[pzo_rol_en_zona] [nvarchar](50) NOT NULL,
	[pzo_fecha_hora_llegada] [datetime] NOT NULL,
	[pzo_fecha_hora_salida] [datetime] NULL,
	[pzo_direccion_vivienda] [nvarchar](200) NULL,
	[pzo_estado_asignacion] [nvarchar](20) NOT NULL,
	[pzo_observaciones] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[pzo_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_planos_venue] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_planos_venue](
	[plv_id] [int] IDENTITY(1,1) NOT NULL,
	[plv_ven_id] [int] NOT NULL,
	[plv_org_id] [int] NOT NULL,
	[plv_tipo_plano] [nvarchar](30) NOT NULL,
	[plv_nombre_plano] [nvarchar](100) NOT NULL,
	[plv_url_plano] [nvarchar](500) NOT NULL,
	[plv_fecha_subida] [datetime] NOT NULL,
	[plv_usu_id_subio] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[plv_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_proveedores] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_proveedores](
	[pro_id] [int] IDENTITY(1,1) NOT NULL,
	[pro_razon_social] [nvarchar](150) NOT NULL,
	[pro_ruc] [char](13) NOT NULL,
	[pro_email] [nvarchar](100) NULL,
	[pro_telefono] [nvarchar](20) NULL,
	[pro_direccion] [nvarchar](200) NULL,
	[pro_categoria] [nvarchar](50) NOT NULL,
	[pro_calificacion_promedio] [decimal](3, 2) NULL,
	[pro_notas] [nvarchar](max) NULL,
	[pro_fecha_registro] [datetime] NOT NULL,
	[pro_estado] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[pro_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[pro_ruc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_tipos_evento] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tipos_evento](
	[tev_id] [int] IDENTITY(1,1) NOT NULL,
	[tev_nombre] [nvarchar](50) NOT NULL,
	[tev_descripcion] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[tev_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[tev_nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_usuarios] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_usuarios](
	[usu_id] [int] IDENTITY(1,1) NOT NULL,
	[usu_alias] [nvarchar](50) NOT NULL,
	[usu_email] [nvarchar](100) NOT NULL,
	[usu_pass_hash] [nvarchar](256) NOT NULL,
	[usu_pass_salt] [uniqueidentifier] NOT NULL,
	[usu_pass_cambio] [datetime] NOT NULL,
	[usu_nombre] [nvarchar](100) NOT NULL,
	[usu_rol] [nvarchar](20) NOT NULL,
	[usu_telefono] [nvarchar](20) NULL,
	[usu_direccion] [nvarchar](200) NULL,
	[usu_fecha_registro] [datetime] NOT NULL,
	[usu_ultimo_acceso] [datetime] NULL,
	[usu_intentos] [int] NOT NULL,
	[usu_bloqueo] [bit] NOT NULL,
	[usu_bloqueo_fecha] [datetime] NULL,
	[usu_estado] [bit] NOT NULL,
	[usu_id_creador] [int] NULL,
	[usu_2fa_enabled] [bit] NOT NULL CONSTRAINT [DF_usu_2fa_enabled] DEFAULT (0),
	[usu_otp_code] [nvarchar](10) NULL,
	[usu_otp_expiry] [datetime] NULL,
	[usu_otp_secret] [nvarchar](100) NULL,
	[usu_2fa_recovery_codes] [nvarchar](max) NULL,
	[usu_recovery_email] [nvarchar](100) NULL,
	[usu_password_reset_token] [nvarchar](200) NULL,
	[usu_password_reset_expiry] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[usu_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[usu_email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[usu_alias] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_venue_tipo_evento] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_venue_tipo_evento](
	[vte_ven_id] [int] NOT NULL,
	[vte_tev_id] [int] NOT NULL,
	[vte_nivel_recomendacion] [int] NOT NULL,
	[vte_notas_especificas] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[vte_ven_id] ASC,
	[vte_tev_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_venues] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_venues](
	[ven_id] [int] IDENTITY(1,1) NOT NULL,
	[ven_nombre] [nvarchar](100) NOT NULL,
	[ven_direccion] [nvarchar](200) NOT NULL,
	[ven_ciudad] [nvarchar](50) NOT NULL,
	[ven_latitud] [decimal](10, 8) NULL,
	[ven_longitud] [decimal](11, 8) NULL,
	[ven_capacidad_maxima] [int] NULL,
	[ven_capacidad_con_mesa] [int] NULL,
	[ven_tiene_audio_integrado] [bit] NOT NULL,
	[ven_tiene_iluminacion_integrada] [bit] NOT NULL,
	[ven_tiene_aire_acondicionado] [bit] NOT NULL,
	[ven_tiene_wifi] [bit] NOT NULL,
	[ven_permite_decoracion] [bit] NOT NULL,
	[ven_permite_ingreso_vehiculos] [bit] NOT NULL,
	[ven_horario_carga_descarga] [nvarchar](100) NULL,
	[ven_horario_maximo_evento] [nvarchar](100) NULL,
	[ven_restricciones] [nvarchar](max) NULL,
	[ven_calificacion_promedio] [decimal](3, 2) NULL,
	[ven_contacto_nombre] [nvarchar](100) NULL,
	[ven_contacto_telefono] [nvarchar](20) NULL,
	[ven_estado] [nvarchar](20) NOT NULL,
	[ven_fecha_registro] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ven_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_zonas_evento] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_zonas_evento](
	[zev_id] [int] IDENTITY(1,1) NOT NULL,
	[zev_eve_id] [int] NOT NULL,
	[zev_zpl_id] [int] NOT NULL,
	[zev_nombre_personalizado] [nvarchar](100) NULL,
	[zev_ubicacion_fisica] [nvarchar](200) NULL,
	[zev_capacidad] [int] NULL,
	[zev_horario_apertura] [time](7) NULL,
	[zev_horario_cierre] [time](7) NULL,
	[zev_estado_zona] [nvarchar](20) NOT NULL,
	[zev_fecha_creacion] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[zev_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_zonas_plantilla] Fecha de script: 16/7/2026 17:00:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_zonas_plantilla](
	[zpl_id] [int] IDENTITY(1,1) NOT NULL,
	[zpl_org_id] [int] NOT NULL,
	[zpl_nombre] [nvarchar](100) NOT NULL,
	[zpl_descripcion] [nvarchar](200) NULL,
	[zpl_color_hex] [nvarchar](7) NULL,
	[zpl_es_activa] [bit] NOT NULL,
	[zpl_fecha_creacion] [datetime] NOT NULL,
	[zpl_usu_id_creador] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[zpl_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_act_estado] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_act_estado] ON [dbo].[tbl_actividades]
(
	[act_estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_act_eve_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_act_eve_id] ON [dbo].[tbl_actividades]
(
	[act_eve_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_act_fecha_hora_estimada_inicio] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_act_fecha_hora_estimada_inicio] ON [dbo].[tbl_actividades]
(
	[act_fecha_hora_estimada_inicio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_act_fev_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_act_fev_id] ON [dbo].[tbl_actividades]
(
	[act_fev_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_act_usu_id_responsable] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_act_usu_id_responsable] ON [dbo].[tbl_actividades]
(
	[act_usu_id_responsable] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_asi_es_devolucion_completa] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_asi_es_devolucion_completa] ON [dbo].[tbl_asignacion_inventario]
(
	[asi_es_devolucion_completa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_asi_eve_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_asi_eve_id] ON [dbo].[tbl_asignacion_inventario]
(
	[asi_eve_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_asi_inv_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_asi_inv_id] ON [dbo].[tbl_asignacion_inventario]
(
	[asi_inv_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_bit_fecha_hora] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_bit_fecha_hora] ON [dbo].[tbl_bitacora]
(
	[bit_fecha_hora] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_bit_usu_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_bit_usu_id] ON [dbo].[tbl_bitacora]
(
	[bit_usu_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_eve_cli_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_eve_cli_id] ON [dbo].[tbl_eventos]
(
	[eve_cli_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_eve_estado] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_eve_estado] ON [dbo].[tbl_eventos]
(
	[eve_estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_eve_fecha_inicio] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_eve_fecha_inicio] ON [dbo].[tbl_eventos]
(
	[eve_fecha_inicio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_eve_ven_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_eve_ven_id] ON [dbo].[tbl_eventos]
(
	[eve_ven_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_pag_estado_pago] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_pag_estado_pago] ON [dbo].[tbl_pagos]
(
	[pag_estado_pago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_pag_eve_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_pag_eve_id] ON [dbo].[tbl_pagos]
(
	[pag_eve_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_pag_pro_id] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_pag_pro_id] ON [dbo].[tbl_pagos]
(
	[pag_pro_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_usu_email] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_usu_email] ON [dbo].[tbl_usuarios]
(
	[usu_email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_usu_rol] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_usu_rol] ON [dbo].[tbl_usuarios]
(
	[usu_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_ven_ciudad] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_ven_ciudad] ON [dbo].[tbl_venues]
(
	[ven_ciudad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_ven_estado] Fecha de script: 16/7/2026 17:00:56 ******/
CREATE NONCLUSTERED INDEX [IX_ven_estado] ON [dbo].[tbl_venues]
(
	[ven_estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbl_actividades] ADD  DEFAULT ('Pendiente') FOR [act_estado]
GO
ALTER TABLE [dbo].[tbl_actividades] ADD  DEFAULT ('Media') FOR [act_prioridad]
GO
ALTER TABLE [dbo].[tbl_actividades] ADD  DEFAULT ((0)) FOR [act_requiere_conexion]
GO
ALTER TABLE [dbo].[tbl_actividades] ADD  DEFAULT (getdate()) FOR [act_fecha_creacion]
GO
ALTER TABLE [dbo].[tbl_actividades] ADD  DEFAULT (getdate()) FOR [act_fecha_ultima_modificacion]
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario] ADD  DEFAULT (getdate()) FOR [asi_fecha_hora_salida]
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario] ADD  DEFAULT ('Bueno') FOR [asi_estado_salida]
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario] ADD  DEFAULT ((0)) FOR [asi_es_devolucion_completa]
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario] ADD  DEFAULT (getdate()) FOR [asi_fecha_registro]
GO
ALTER TABLE [dbo].[tbl_bitacora] ADD  DEFAULT (getdate()) FOR [bit_fecha_hora]
GO
ALTER TABLE [dbo].[tbl_checklist_desmontaje] ADD  DEFAULT ('Pendiente') FOR [cde_estado_verificacion]
GO
ALTER TABLE [dbo].[tbl_checklist_desmontaje] ADD  DEFAULT (getdate()) FOR [cde_fecha_registro]
GO
ALTER TABLE [dbo].[tbl_clientes] ADD  DEFAULT ('Regular') FOR [cli_clasificacion]
GO
ALTER TABLE [dbo].[tbl_clientes] ADD  DEFAULT (getdate()) FOR [cli_fecha_registro]
GO
ALTER TABLE [dbo].[tbl_clientes] ADD  DEFAULT ((1)) FOR [cli_estado]
GO
ALTER TABLE [dbo].[tbl_contactos_cliente] ADD  DEFAULT ((0)) FOR [cco_es_principal]
GO
ALTER TABLE [dbo].[tbl_contactos_cliente] ADD  DEFAULT ('Email') FOR [cco_preferencia_contacto]
GO
ALTER TABLE [dbo].[tbl_cuentas_bancarias_cliente] ADD  DEFAULT ((0)) FOR [cbc_es_predeterminada]
GO
ALTER TABLE [dbo].[tbl_eventos] ADD  DEFAULT ((0)) FOR [eve_gasto_real]
GO
ALTER TABLE [dbo].[tbl_eventos] ADD  DEFAULT ('Planificacion') FOR [eve_estado]
GO
ALTER TABLE [dbo].[tbl_eventos] ADD  DEFAULT (getdate()) FOR [eve_fecha_creacion]
GO
ALTER TABLE [dbo].[tbl_fases_evento] ADD  DEFAULT ('Pendiente') FOR [fev_estado_fase]
GO
ALTER TABLE [dbo].[tbl_fotos_inventario] ADD  DEFAULT ((0)) FOR [foi_es_principal]
GO
ALTER TABLE [dbo].[tbl_fotos_inventario] ADD  DEFAULT (getdate()) FOR [foi_fecha_subida]
GO
ALTER TABLE [dbo].[tbl_intentos_login] ADD  DEFAULT (getdate()) FOR [inl_fecha_hora]
GO
ALTER TABLE [dbo].[tbl_intentos_login] ADD  DEFAULT ((0)) FOR [inl_fue_exitoso]
GO
ALTER TABLE [dbo].[tbl_inventario] ADD  DEFAULT ('Operativo') FOR [inv_estado_item]
GO
ALTER TABLE [dbo].[tbl_inventario] ADD  DEFAULT ((1)) FOR [inv_stock_actual]
GO
ALTER TABLE [dbo].[tbl_inventario] ADD  DEFAULT ((0)) FOR [inv_stock_minimo]
GO
ALTER TABLE [dbo].[tbl_inventario] ADD  DEFAULT (getdate()) FOR [inv_fecha_registro]
GO
ALTER TABLE [dbo].[tbl_metricas_evento] ADD  DEFAULT ((0)) FOR [met_numero_incidencias]
GO
ALTER TABLE [dbo].[tbl_metricas_evento] ADD  DEFAULT (getdate()) FOR [met_fecha_calculo]
GO
ALTER TABLE [dbo].[tbl_pagos] ADD  DEFAULT ((0)) FOR [pag_anticipo_pagado]
GO
ALTER TABLE [dbo].[tbl_pagos] ADD  DEFAULT ('Pendiente') FOR [pag_estado_pago]
GO
ALTER TABLE [dbo].[tbl_pagos] ADD  DEFAULT (getdate()) FOR [pag_fecha_registro]
GO
ALTER TABLE [dbo].[tbl_personal_zona] ADD  DEFAULT ('Confirmado') FOR [pzo_estado_asignacion]
GO
ALTER TABLE [dbo].[tbl_planos_venue] ADD  DEFAULT (getdate()) FOR [plv_fecha_subida]
GO
ALTER TABLE [dbo].[tbl_proveedores] ADD  DEFAULT ((0)) FOR [pro_calificacion_promedio]
GO
ALTER TABLE [dbo].[tbl_proveedores] ADD  DEFAULT (getdate()) FOR [pro_fecha_registro]
GO
ALTER TABLE [dbo].[tbl_proveedores] ADD  DEFAULT ((1)) FOR [pro_estado]
GO
ALTER TABLE [dbo].[tbl_usuarios] ADD  DEFAULT (newid()) FOR [usu_pass_salt]
GO
ALTER TABLE [dbo].[tbl_usuarios] ADD  DEFAULT (getdate()) FOR [usu_pass_cambio]
GO
ALTER TABLE [dbo].[tbl_usuarios] ADD  DEFAULT (getdate()) FOR [usu_fecha_registro]
GO
ALTER TABLE [dbo].[tbl_usuarios] ADD  DEFAULT ((0)) FOR [usu_intentos]
GO
ALTER TABLE [dbo].[tbl_usuarios] ADD  DEFAULT ((0)) FOR [usu_bloqueo]
GO
ALTER TABLE [dbo].[tbl_usuarios] ADD  DEFAULT ((1)) FOR [usu_estado]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT ((0)) FOR [ven_tiene_audio_integrado]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT ((0)) FOR [ven_tiene_iluminacion_integrada]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT ((0)) FOR [ven_tiene_aire_acondicionado]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT ((0)) FOR [ven_tiene_wifi]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT ((1)) FOR [ven_permite_decoracion]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT ((1)) FOR [ven_permite_ingreso_vehiculos]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT ((0)) FOR [ven_calificacion_promedio]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT ('Disponible') FOR [ven_estado]
GO
ALTER TABLE [dbo].[tbl_venues] ADD  DEFAULT (getdate()) FOR [ven_fecha_registro]
GO
ALTER TABLE [dbo].[tbl_zonas_evento] ADD  DEFAULT ('Activa') FOR [zev_estado_zona]
GO
ALTER TABLE [dbo].[tbl_zonas_evento] ADD  DEFAULT (getdate()) FOR [zev_fecha_creacion]
GO
ALTER TABLE [dbo].[tbl_zonas_plantilla] ADD  DEFAULT ('#CCCCCC') FOR [zpl_color_hex]
GO
ALTER TABLE [dbo].[tbl_zonas_plantilla] ADD  DEFAULT ((1)) FOR [zpl_es_activa]
GO
ALTER TABLE [dbo].[tbl_zonas_plantilla] ADD  DEFAULT (getdate()) FOR [zpl_fecha_creacion]
GO
ALTER TABLE [dbo].[tbl_actividades]  WITH CHECK ADD  CONSTRAINT [FK_act_eve_id] FOREIGN KEY([act_eve_id])
REFERENCES [dbo].[tbl_eventos] ([eve_id])
GO
ALTER TABLE [dbo].[tbl_actividades] CHECK CONSTRAINT [FK_act_eve_id]
GO
ALTER TABLE [dbo].[tbl_actividades]  WITH CHECK ADD  CONSTRAINT [FK_act_fev_id] FOREIGN KEY([act_fev_id])
REFERENCES [dbo].[tbl_fases_evento] ([fev_id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[tbl_actividades] CHECK CONSTRAINT [FK_act_fev_id]
GO
ALTER TABLE [dbo].[tbl_actividades]  WITH CHECK ADD  CONSTRAINT [FK_act_id_dependiente] FOREIGN KEY([act_id_dependiente])
REFERENCES [dbo].[tbl_actividades] ([act_id])
GO
ALTER TABLE [dbo].[tbl_actividades] CHECK CONSTRAINT [FK_act_id_dependiente]
GO
ALTER TABLE [dbo].[tbl_actividades]  WITH CHECK ADD  CONSTRAINT [FK_act_usu_id_responsable] FOREIGN KEY([act_usu_id_responsable])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_actividades] CHECK CONSTRAINT [FK_act_usu_id_responsable]
GO
ALTER TABLE [dbo].[tbl_actividades]  WITH CHECK ADD  CONSTRAINT [FK_act_zpl_id] FOREIGN KEY([act_zpl_id])
REFERENCES [dbo].[tbl_zonas_plantilla] ([zpl_id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[tbl_actividades] CHECK CONSTRAINT [FK_act_zpl_id]
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario]  WITH CHECK ADD FOREIGN KEY([asi_eve_id])
REFERENCES [dbo].[tbl_eventos] ([eve_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario]  WITH CHECK ADD FOREIGN KEY([asi_inv_id])
REFERENCES [dbo].[tbl_inventario] ([inv_id])
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario]  WITH CHECK ADD FOREIGN KEY([asi_usu_id_responsable])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_bitacora]  WITH CHECK ADD FOREIGN KEY([bit_usu_id])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_checklist_desmontaje]  WITH CHECK ADD FOREIGN KEY([cde_eve_id])
REFERENCES [dbo].[tbl_eventos] ([eve_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_checklist_desmontaje]  WITH CHECK ADD FOREIGN KEY([cde_inv_id])
REFERENCES [dbo].[tbl_inventario] ([inv_id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[tbl_checklist_desmontaje]  WITH CHECK ADD FOREIGN KEY([cde_usu_id_responsable])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_clientes]  WITH CHECK ADD FOREIGN KEY([cli_usu_id_creador])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_contactos_cliente]  WITH CHECK ADD FOREIGN KEY([cco_cli_id])
REFERENCES [dbo].[tbl_clientes] ([cli_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_cuentas_bancarias_cliente]  WITH CHECK ADD FOREIGN KEY([cbc_cli_id])
REFERENCES [dbo].[tbl_clientes] ([cli_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_eventos]  WITH CHECK ADD FOREIGN KEY([eve_cli_id])
REFERENCES [dbo].[tbl_clientes] ([cli_id])
GO
ALTER TABLE [dbo].[tbl_eventos]  WITH CHECK ADD FOREIGN KEY([eve_tev_id])
REFERENCES [dbo].[tbl_tipos_evento] ([tev_id])
GO
ALTER TABLE [dbo].[tbl_eventos]  WITH CHECK ADD FOREIGN KEY([eve_usu_id_creador])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_eventos]  WITH CHECK ADD FOREIGN KEY([eve_usu_id_productor_general])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_eventos]  WITH CHECK ADD FOREIGN KEY([eve_ven_id])
REFERENCES [dbo].[tbl_venues] ([ven_id])
GO
ALTER TABLE [dbo].[tbl_fases_evento]  WITH CHECK ADD FOREIGN KEY([fev_eve_id])
REFERENCES [dbo].[tbl_eventos] ([eve_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_fotos_inventario]  WITH CHECK ADD FOREIGN KEY([foi_inv_id])
REFERENCES [dbo].[tbl_inventario] ([inv_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_fotos_inventario]  WITH CHECK ADD FOREIGN KEY([foi_usu_id_subio])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_inventario]  WITH CHECK ADD FOREIGN KEY([inv_pro_id])
REFERENCES [dbo].[tbl_proveedores] ([pro_id])
GO
ALTER TABLE [dbo].[tbl_inventario]  WITH CHECK ADD FOREIGN KEY([inv_usu_id_creador])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_metricas_evento]  WITH CHECK ADD FOREIGN KEY([met_eve_id])
REFERENCES [dbo].[tbl_eventos] ([eve_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_pagos]  WITH CHECK ADD FOREIGN KEY([pag_eve_id])
REFERENCES [dbo].[tbl_eventos] ([eve_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_pagos]  WITH CHECK ADD FOREIGN KEY([pag_pro_id])
REFERENCES [dbo].[tbl_proveedores] ([pro_id])
GO
ALTER TABLE [dbo].[tbl_pagos]  WITH CHECK ADD FOREIGN KEY([pag_usu_id_registro])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_personal_zona]  WITH CHECK ADD FOREIGN KEY([pzo_usu_id])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_personal_zona]  WITH CHECK ADD FOREIGN KEY([pzo_zev_id])
REFERENCES [dbo].[tbl_zonas_evento] ([zev_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_planos_venue]  WITH CHECK ADD FOREIGN KEY([plv_usu_id_subio])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_planos_venue]  WITH CHECK ADD FOREIGN KEY([plv_ven_id])
REFERENCES [dbo].[tbl_venues] ([ven_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_usuarios]  WITH CHECK ADD FOREIGN KEY([usu_id_creador])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_venue_tipo_evento]  WITH CHECK ADD FOREIGN KEY([vte_tev_id])
REFERENCES [dbo].[tbl_tipos_evento] ([tev_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_venue_tipo_evento]  WITH CHECK ADD FOREIGN KEY([vte_ven_id])
REFERENCES [dbo].[tbl_venues] ([ven_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_zonas_evento]  WITH CHECK ADD FOREIGN KEY([zev_eve_id])
REFERENCES [dbo].[tbl_eventos] ([eve_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_zonas_evento]  WITH CHECK ADD FOREIGN KEY([zev_zpl_id])
REFERENCES [dbo].[tbl_zonas_plantilla] ([zpl_id])
GO
ALTER TABLE [dbo].[tbl_zonas_plantilla]  WITH CHECK ADD FOREIGN KEY([zpl_usu_id_creador])
REFERENCES [dbo].[tbl_usuarios] ([usu_id])
GO
ALTER TABLE [dbo].[tbl_actividades]  WITH CHECK ADD CHECK  (([act_estado]='Retrasada' OR [act_estado]='Cancelada' OR [act_estado]='Completada' OR [act_estado]='EnProgreso' OR [act_estado]='Pendiente'))
GO
ALTER TABLE [dbo].[tbl_actividades]  WITH CHECK ADD CHECK  (([act_prioridad]='Baja' OR [act_prioridad]='Media' OR [act_prioridad]='Alta'))
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario]  WITH CHECK ADD CHECK  (([asi_cantidad_asignada]>(0)))
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario]  WITH CHECK ADD CHECK  (([asi_estado_salida]='Dañado' OR [asi_estado_salida]='Regular' OR [asi_estado_salida]='Bueno'))
GO
ALTER TABLE [dbo].[tbl_asignacion_inventario]  WITH CHECK ADD CHECK  (([asi_estado_entrada]='Agotado' OR [asi_estado_entrada]='Faltante' OR [asi_estado_entrada]='Dañado' OR [asi_estado_entrada]='Regular' OR [asi_estado_entrada]='Bueno'))
GO
ALTER TABLE [dbo].[tbl_checklist_desmontaje]  WITH CHECK ADD CHECK  (([cde_cantidad_esperada]>(0)))
GO
ALTER TABLE [dbo].[tbl_checklist_desmontaje]  WITH CHECK ADD CHECK  (([cde_estado_verificacion]='ConNovedad' OR [cde_estado_verificacion]='Verificado' OR [cde_estado_verificacion]='Pendiente'))
GO
ALTER TABLE [dbo].[tbl_clientes]  WITH CHECK ADD CHECK  (([cli_clasificacion]='Estrategico' OR [cli_clasificacion]='VIP' OR [cli_clasificacion]='Regular' OR [cli_clasificacion]='Nuevo'))
GO
ALTER TABLE [dbo].[tbl_contactos_cliente]  WITH CHECK ADD CHECK  (([cco_preferencia_contacto]='SMS' OR [cco_preferencia_contacto]='Llamada' OR [cco_preferencia_contacto]='WhatsApp' OR [cco_preferencia_contacto]='Email'))
GO
ALTER TABLE [dbo].[tbl_cuentas_bancarias_cliente]  WITH CHECK ADD CHECK  (([cbc_tipo_cuenta]='Empresarial' OR [cbc_tipo_cuenta]='Ahorros' OR [cbc_tipo_cuenta]='Corriente'))
GO
ALTER TABLE [dbo].[tbl_eventos]  WITH CHECK ADD CHECK  (([eve_estado]='Cancelado' OR [eve_estado]='Finalizado' OR [eve_estado]='Ejecucion' OR [eve_estado]='PreProduccion' OR [eve_estado]='Planificacion'))
GO
ALTER TABLE [dbo].[tbl_eventos]  WITH CHECK ADD CHECK  (([eve_presupuesto_estimado]>(0)))
GO
ALTER TABLE [dbo].[tbl_fases_evento]  WITH CHECK ADD CHECK  (([fev_estado_fase]='Retrasada' OR [fev_estado_fase]='Completada' OR [fev_estado_fase]='EnProgreso' OR [fev_estado_fase]='Pendiente'))
GO
ALTER TABLE [dbo].[tbl_inventario]  WITH CHECK ADD CHECK  (([inv_categoria]='Otro' OR [inv_categoria]='Herramienta' OR [inv_categoria]='Decoracion' OR [inv_categoria]='Mobiliario' OR [inv_categoria]='Iluminacion' OR [inv_categoria]='Video' OR [inv_categoria]='Audio'))
GO
ALTER TABLE [dbo].[tbl_inventario]  WITH CHECK ADD CHECK  (([inv_estado_item]='DadoDeBaja' OR [inv_estado_item]='EnMantenimiento' OR [inv_estado_item]='Dañado' OR [inv_estado_item]='Operativo'))
GO
ALTER TABLE [dbo].[tbl_inventario]  WITH CHECK ADD CHECK  (([inv_propietario]='Cliente' OR [inv_propietario]='Proveedor' OR [inv_propietario]='Propio'))
GO
ALTER TABLE [dbo].[tbl_inventario]  WITH CHECK ADD CHECK  (([inv_stock_actual]>=(0)))
GO
ALTER TABLE [dbo].[tbl_metricas_evento]  WITH CHECK ADD CHECK  (([met_calificacion_cliente]>=(1) AND [met_calificacion_cliente]<=(5)))
GO
ALTER TABLE [dbo].[tbl_pagos]  WITH CHECK ADD CHECK  (([pag_anticipo_pagado]>=(0)))
GO
ALTER TABLE [dbo].[tbl_pagos]  WITH CHECK ADD CHECK  (([pag_estado_pago]='Cancelado' OR [pag_estado_pago]='Pagado' OR [pag_estado_pago]='Parcial' OR [pag_estado_pago]='Pendiente'))
GO
ALTER TABLE [dbo].[tbl_pagos]  WITH CHECK ADD CHECK  (([pag_monto_contratado]>(0)))
GO
ALTER TABLE [dbo].[tbl_personal_zona]  WITH CHECK ADD CHECK  (([pzo_estado_asignacion]='Cancelado' OR [pzo_estado_asignacion]='Reemplazo' OR [pzo_estado_asignacion]='Pendiente' OR [pzo_estado_asignacion]='Confirmado'))
GO
ALTER TABLE [dbo].[tbl_personal_zona]  WITH CHECK ADD CHECK  (([pzo_rol_en_zona]='Logistica' OR [pzo_rol_en_zona]='Tecnico' OR [pzo_rol_en_zona]='Seguridad' OR [pzo_rol_en_zona]='Staff' OR [pzo_rol_en_zona]='Asistente' OR [pzo_rol_en_zona]='Coordinador'))
GO
ALTER TABLE [dbo].[tbl_planos_venue]  WITH CHECK ADD CHECK  (([plv_tipo_plano]='Decoracion' OR [plv_tipo_plano]='Escenario' OR [plv_tipo_plano]='Evacuacion' OR [plv_tipo_plano]='Sonido' OR [plv_tipo_plano]='Luces' OR [plv_tipo_plano]='General'))
GO
ALTER TABLE [dbo].[tbl_proveedores]  WITH CHECK ADD CHECK  (([pro_categoria]='Otro' OR [pro_categoria]='Seguridad' OR [pro_categoria]='Transporte' OR [pro_categoria]='Catering' OR [pro_categoria]='Decoracion' OR [pro_categoria]='Mobiliario' OR [pro_categoria]='Iluminacion' OR [pro_categoria]='Video' OR [pro_categoria]='Audio'))
GO
ALTER TABLE [dbo].[tbl_proveedores]  WITH CHECK ADD CHECK  (([pro_calificacion_promedio]>=(0) AND [pro_calificacion_promedio]<=(5)))
GO
ALTER TABLE [dbo].[tbl_usuarios]  WITH CHECK ADD CHECK  (([usu_rol]='Almacen' OR [usu_rol]='Tecnico' OR [usu_rol]='ProductorCampo' OR [usu_rol]='ProductorGeneral' OR [usu_rol]='Administrador'))
GO
ALTER TABLE [dbo].[tbl_venue_tipo_evento]  WITH CHECK ADD CHECK  (([vte_nivel_recomendacion]>=(1) AND [vte_nivel_recomendacion]<=(5)))
GO
ALTER TABLE [dbo].[tbl_venues]  WITH CHECK ADD CHECK  (([ven_calificacion_promedio]>=(0) AND [ven_calificacion_promedio]<=(5)))
GO
ALTER TABLE [dbo].[tbl_venues]  WITH CHECK ADD CHECK  (([ven_estado]='NoDisponible' OR [ven_estado]='EnMantenimiento' OR [ven_estado]='Disponible'))
GO
ALTER TABLE [dbo].[tbl_zonas_evento]  WITH CHECK ADD CHECK  (([zev_estado_zona]='Preparacion' OR [zev_estado_zona]='Cerrada' OR [zev_estado_zona]='Activa'))
GO
USE [master]
GO
ALTER DATABASE [EventHubv01] SET  READ_WRITE 
GO
