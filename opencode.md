# 📊 EventProduction Hub - Documento Maestro para Agentes IA
## Proyecto Integrador - Sistema Integral de Gestión de Eventos

**Versión:** 1.0  
**Base de Datos:** SQL Server  
**Stack:** C# ASP.NET MVC + SQL Server 2019+  
**Plazo:** Octubre 2026  
**Estado:** Desarrollo MVP  

---

## 🎯 ÍNDICE

1. [Visión del Proyecto](#visión-del-proyecto)
2. [Contexto y Problema](#contexto-y-problema)
3. [Solución Propuesta](#solución-propuesta)
4. [Estructura de Base de Datos](#estructura-de-base-de-datos)
5. [Flujos de Negocio](#flujos-de-negocio)
6. [Rol de Agentes IA](#rol-de-agentes-ia)
7. [Guía Técnica](#guía-técnica)

---

# VISIÓN DEL PROYECTO

## Misión
Desarrollar una plataforma integral que **centralice y automatice la gestión operativa de eventos** medianos en Ecuador, permitiendo visibilidad en tiempo real, control financiero y análisis de datos históricos.

## Público Objetivo
- **Productoras de eventos medianas** (50-200 eventos/año)
- **Ubicación:** Ecuador (Quito, Guayaquil, Cuenca)
- **Tamaño:** Equipo de 5-50 personas
- **Presupuestos:** $5,000 a $500,000+ por evento

## Valor Agregado
✅ **Centralización:** Elimina herramientas dispersas (Excel, WhatsApp, correos)  
✅ **Visibilidad:** Dashboard en tiempo real de estado operativo  
✅ **Control Financiero:** Gestión automática de pagos, presupuestos, alertas  
✅ **Análisis:** Histórico de eventos para mejora continua  
✅ **Escalabilidad:** Multitenancy, soporta múltiples productoras  

---

# CONTEXTO Y PROBLEMA

## Situación Actual (As-Is)
Las productoras de eventos en Ecuador **utilizan herramientas fragmentadas:**

```
Excel           → Cronogramas, presupuestos (desorganizado)
WhatsApp        → Comunicación operativa crítica (sin registro)
Correos         → Confirmaciones dispersas (difícil seguimiento)
Libreta Física  → Notas de campo (pérdida de información)
Llamadas        → Coordinación ad-hoc (sin auditoría)
```

**Impactos negativos:**
- ❌ **Falta de visibilidad:** Productor no sabe estado real hasta preguntar
- ❌ **Errores financieros:** Pagos duplicados, faltantes de registro
- ❌ **Retrasos en cascada:** Cambios en cronograma no comunican bien
- ❌ **Pérdida de información:** Cuando personal se va, se lleva el conocimiento
- ❌ **Decisiones reactivas:** Solo reaccionan ante crisis, no previenen

## Escala del Problema

```
Evento Promedio:
├─ Duración: 1-3 días
├─ Personal: 10-40 personas
├─ Proveedores: 5-15 contratistas
├─ Equipo: 20-100 items de inventario
├─ Presupuesto: $5,000 - $100,000+
└─ Actividades: 20-50 tareas por evento
```

---

# SOLUCIÓN PROPUESTA

## Nombre
**EventProduction Hub (EPH)**

## Descripción
Plataforma web-mobile integral que centraliza:

| Área | Funcionalidad |
|------|-------------|
| **Operativa** | Cronogramas, actividades, asignación de personal |
| **Logística** | Inventario, equipo, asignación a eventos |
| **Financiera** | Pagos, presupuestos, control de gasto |
| **Proveedores** | Directorio, histórico, rating automático |
| **Reportería** | Dashboards, KPIs, análisis post-evento |

## Arquitectura

```
┌─────────────────────────────────┐
│   CAPA PRESENTACIÓN             │
│ ASP.NET MVC + Blazor (Responsive)
└────────────────┬────────────────┘
                 │
┌────────────────▼────────────────┐
│   LÓGICA DE NEGOCIO             │
│ C# Services + Controllers        │
│ Validaciones, Auditoría          │
└────────────────┬────────────────┘
                 │
┌────────────────▼────────────────┐
│   ACCESO A DATOS                │
│ Entity Framework Core + SQL Srv  │
│ Repositories, Migrations         │
└────────────────┬────────────────┘
                 │
┌────────────────▼────────────────┐
│   DATOS                         │
│ SQL Server 2019 (EventHubv01)    │
│ 20+ tablas relacionadas          │
└─────────────────────────────────┘
```

---

# ESTRUCTURA DE BASE DE DATOS

## Base de Datos: EventHubv01

**Configuración:**
- **Motor:** SQL Server 2019+
- **Compatibilidad:** COMPATIBILITY_LEVEL 170
- **Recuperación:** FULL (backups continuos)
- **Collation:** DATABASE_DEFAULT

## Tablas Principales (20+)

### 1. NÚCLEO OPERATIVO

#### `tbl_eventos`
**Propósito:** Registro central de cada evento  
**Campos clave:**
```
eve_id              INT PRIMARY KEY
eve_codigo          VARCHAR UNIQUE (ej: "CONF-2026-001")
eve_nombre          VARCHAR (ej: "Conferencia Tecnológica 2026")
eve_cli_id          FK → Cliente que contrata
eve_ven_id          FK → Venue (lugar)
eve_tev_id          FK → Tipo evento
eve_fecha_inicio    DATE
eve_hora_inicio     TIME
eve_presupuesto_estimado  DECIMAL(18,2)
eve_gasto_real      DECIMAL(18,2)
eve_estado          VARCHAR (Planificación, Ejecución, Cerrado)
eve_usu_id_productor_general  FK → Usuario responsable
```

**Ejemplo datos reales:**
```
eve_id=42
eve_codigo=CONF-2026-001
eve_nombre=Conferencia Tecnológica 2026
eve_cli_id=15 (TechCorp Ecuador)
eve_ven_id=8 (Centro de Convenciones Quito)
eve_presupuesto_estimado=20000.00
eve_estado=Ejecución
eve_usu_id_productor_general=1 (Juan Pérez)
```

#### `tbl_fases_evento`
**Propósito:** Dividir evento en fases (Montaje, Evento, Desmontaje)  
**Relación:** 1 evento → N fases  
**Ejemplo:**
```
fev_id=1, fev_eve_id=42, fev_nombre_fase='Montaje'
fev_id=2, fev_eve_id=42, fev_nombre_fase='Evento Principal'
fev_id=3, fev_eve_id=42, fev_nombre_fase='Desmontaje'
```

#### `tbl_actividades`
**Propósito:** Tareas dentro del evento (cronograma minuto a minuto)  
**Relación:** 1 evento → N actividades  
**Campos clave:**
```
act_id              INT PRIMARY KEY
act_eve_id          FK → Evento
act_fev_id          FK → Fase (opcional)
act_titulo          VARCHAR (ej: "Setup de audio")
act_fecha_hora_estimada_inicio  DATETIME
act_fecha_hora_estimada_fin     DATETIME
act_duracion_estimada_minutos   COMPUTED (fin-inicio)
act_estado          VARCHAR (Pendiente, En curso, Completado, Retrasado)
act_usu_id_responsable  FK → Usuario asignado
act_prioridad       VARCHAR (Alta, Media, Baja)
act_fecha_hora_real_inicio  DATETIME (cuando realmente comenzó)
```

**Ejemplo datos reales:**
```
Actividad 1: Setup de audio (9:00-10:30) → Responsable: Carlos (ID=3)
Actividad 2: Setup iluminación (10:30-12:00) → Responsable: Ana (ID=4)
Actividad 3: Prueba general (12:00-13:00) → Responsable: Carlos (ID=3)
Actividad 4: Apertura evento (14:00-14:15) → Responsable: Juan (ID=1)
```

---

### 2. GESTIÓN DE USUARIOS Y ROLES

#### `tbl_usuarios`
**Propósito:** Registro de usuarios del sistema  
**Campos:**
```
usu_id              INT PRIMARY KEY
usu_email           VARCHAR UNIQUE
usu_nombre          VARCHAR
usu_apellido        VARCHAR
usu_password_hash   VARCHAR (bcrypt)
usu_rol_id          FK → Rol (Productor, Técnico, Admin Financiero)
usu_estado          BIT (Activo=1, Inactivo=0)
usu_fecha_ultimo_login  DATETIME
```

**Roles definidos:**
1. **Productor General** - Acceso total
2. **Productor de Campo** - Logística, cronogramas
3. **Técnico Audio/Luces** - Solo su área
4. **Admin Financiero** - Pagos, presupuestos
5. **Personal Operativo** - Lectura cronograma

#### `tbl_roles`
Define permisos por rol (lectura, escritura, eliminación).

---

### 3. GESTIÓN DE INVENTARIO

#### `tbl_inventario`
**Propósito:** Registro central de equipo y mobiliario  
**Campos clave:**
```
inv_id              INT PRIMARY KEY
inv_nombre          VARCHAR (ej: "Consola Behringer X32")
inv_categoria       VARCHAR (Audio, Video, Iluminación, Mobiliario)
inv_codigo_interno  VARCHAR UNIQUE (SKU: AUDIO-001)
inv_estado_item     VARCHAR (Operativo, Dañado, Mantenimiento)
inv_stock_actual    INT (cantidad disponible)
inv_stock_minimo    INT (alerta cuando baja)
inv_valor_reposicion DECIMAL(18,2)
inv_propietario     VARCHAR (Productora / Proveedor)
inv_pro_id          FK → Proveedor (si pertenece a tercero)
```

**Ejemplo datos:**
```
Consola Audio:      inv_id=1, stock_actual=2, estado=Operativo
Micrófono Shure:    inv_id=2, stock_actual=6, estado=Operativo
Proyector 4K:       inv_id=3, stock_actual=3, estado=Operativo
Pantalla LED:       inv_id=4, stock_actual=10, estado=Operativo
```

#### `tbl_asignacion_inventario`
**Propósito:** Asignar inventario a eventos  
**Relación:** 1 evento puede usar múltiples items de inventario  
**Campos:**
```
asi_id              INT PRIMARY KEY
asi_eve_id          FK → Evento
asi_inv_id          FK → Item inventario
asi_cantidad_asignada INT (cuántos se llevan)
asi_fecha_hora_salida   DATETIME (cuándo se retiran)
asi_fecha_hora_entrada  DATETIME (cuándo se devuelven)
asi_estado_salida   VARCHAR (Bueno, Dañado)
asi_estado_entrada  VARCHAR (Completo, Faltante, Dañado)
asi_es_devolucion_completa BIT (¿se devolvió todo?)
```

**Ejemplo:**
```
Evento 42 necesita:
- 1 Consola de Audio (inv_id=1)
- 4 Micrófonos (inv_id=2)
- 4 Pantallas LED (inv_id=4)

Se registra en tbl_asignacion_inventario:
asi_eve_id=42, asi_inv_id=1, asi_cantidad_asignada=1
asi_eve_id=42, asi_inv_id=2, asi_cantidad_asignada=4
asi_eve_id=42, asi_inv_id=4, asi_cantidad_asignada=4
```

---

### 4. GESTIÓN DE PROVEEDORES Y PAGOS

#### `tbl_proveedores`
**Propósito:** Directorio centralizado de contratistas  
**Campos:**
```
pro_id              INT PRIMARY KEY
pro_nombre          VARCHAR (ej: "SoundTech Ecuaequipos")
pro_categoria       VARCHAR (Audio, Catering, Transporte, etc.)
pro_ruc             CHAR(13) UNIQUE
pro_email           VARCHAR
pro_telefono        VARCHAR
pro_contacto_principal VARCHAR (persona a cargo)
pro_rating_promedio DECIMAL(3,2) (0-5 estrellas, calculado)
pro_numero_eventos_realizados INT
pro_numero_retrasos INT (para calcular confiabilidad)
```

**Ejemplo:**
```
SoundTech Ecuaequipos
├─ Categoría: Audio
├─ Rating: 4.8 ⭐ (8 eventos, 0 retrasos)
├─ Email: contacto@soundtech.com
└─ Teléfono: 0987-654-321
```

#### `tbl_pagos`
**Propósito:** Control de pagos a proveedores  
**Relación:** 1 evento → múltiples pagos  
**Campos:**
```
pag_id              INT PRIMARY KEY
pag_eve_id          FK → Evento
pag_pro_id          FK → Proveedor
pag_concepto        VARCHAR (ej: "Arriendo consola de audio")
pag_monto_contratado DECIMAL(18,2)
pag_monto_pagado    DECIMAL(18,2) (acumulativo)
pag_saldo_pendiente  DECIMAL(18,2) COMPUTED (contratado - pagado)
pag_fecha_vencimiento DATE
pag_estado          VARCHAR (Anticipado, Pendiente, Pagado)
pag_forma_pago      VARCHAR (Efectivo, Transferencia, Cheque)
pag_comprobante     VARCHAR (número de factura/recibo)
```

**Ejemplo datos - Evento 42:**
```
PAGO 1:
├─ Proveedor: SoundTech (Audio)
├─ Concepto: Arriendo consola + técnico
├─ Monto contratado: $3,500
├─ Monto pagado: $1,750 (50% anticipado)
├─ Saldo pendiente: $1,750
└─ Estado: Anticipado ✓

PAGO 2:
├─ Proveedor: LightShow (Iluminación)
├─ Concepto: Instalación de luces especiales
├─ Monto contratado: $4,200
├─ Monto pagado: $0
├─ Saldo pendiente: $4,200
└─ Estado: Pendiente ⏳

PAGO 3:
├─ Proveedor: Catering Excellence
├─ Concepto: Servicio 500 personas
├─ Monto contratado: $5,000
├─ Monto pagado: $0
├─ Saldo pendiente: $5,000
└─ Estado: Pendiente ⏳

RESUMEN EVENTO 42:
Total presupuesto: $20,000
Total pagado: $8,250 (41%)
Total pendiente: $11,750 (59%)
```

---

### 5. CLIENTES Y CONTACTOS

#### `tbl_clientes`
**Propósito:** Registro de organizaciones que contratan eventos  
**Campos:**
```
cli_id              INT PRIMARY KEY
cli_razon_social    VARCHAR (empresa oficial)
cli_ruc             CHAR(13) UNIQUE
cli_email_principal VARCHAR
cli_telefono_principal VARCHAR
cli_sitio_web       VARCHAR
cli_clasificacion   VARCHAR (Corporativo, Evento, Festival, etc.)
cli_estado          BIT (Activo/Inactivo)
```

**Ejemplo:**
```
cli_id=15
cli_razon_social=TechCorp Ecuador S.A.
cli_ruc=1790123456001
cli_email_principal=eventos@techcorp.com
cli_clasificacion=Corporativo
```

#### `tbl_contactos_cliente`
**Propósito:** Múltiples contactos por cliente  
**Ejemplo:**
```
Cliente: TechCorp Ecuador
├─ Contacto 1: Juan Manager (Gerente de Eventos)
│  └─ Email: juan.manager@techcorp.com
├─ Contacto 2: María López (Asistente)
│  └─ Email: maria@techcorp.com
└─ Contacto 3: Carlos Admin (Finanzas)
   └─ Email: carlos.admin@techcorp.com
```

---

### 6. AUDITORÍA Y SEGURIDAD

#### `tbl_bitacora` (Audit Log)
**Propósito:** Registro de TODAS las acciones en el sistema  
**Campos:**
```
bit_id              INT PRIMARY KEY
bit_usu_id          FK → Usuario que hizo la acción
bit_accion          VARCHAR (INSERT, UPDATE, DELETE, LOGIN)
bit_tabla_afectada  VARCHAR (ej: tbl_actividades)
bit_id_registro_afectado INT
bit_detalle         VARCHAR (qué cambió: "Estado: Pendiente → En curso")
bit_ip_origen       VARCHAR (dirección IP)
bit_fecha_hora      DATETIME
```

**Ejemplo bitácora - Evento 42:**
```
17:45 Juan Pérez: Creó Evento "Conferencia Tecnológica"
17:50 Juan Pérez: Agregó 11 Actividades
18:00 Carlos Rodríguez: Confirmó disponibilidad de Audio
18:15 Ana Martínez: Confirmó disponibilidad de Iluminación
18:30 Diego Fernández: Registró Pagos a Proveedores
19:00 Juan Pérez: Cambió Estado a "Ejecución"
```

#### `tbl_intentos_login`
**Propósito:** Seguridad - registrar intentos de acceso  
**Campos:**
```
inl_id              INT PRIMARY KEY
inl_email           VARCHAR
inl_ip_origen       VARCHAR
inl_fecha_hora      DATETIME
inl_fue_exitoso     BIT (1=éxito, 0=fracaso)
```

---

### 7. CHECKLISTS Y VERIFICACIONES

#### `tbl_checklist_desmontaje`
**Propósito:** Verificar que todo se retira después del evento  
**Campos:**
```
cde_id              INT PRIMARY KEY
cde_eve_id          FK → Evento
cde_inv_id          FK → Item inventario
cde_item_descripcion VARCHAR
cde_cantidad_esperada INT
cde_cantidad_encontrada INT
cde_estado_verificacion VARCHAR (Verificado, Faltante, Dañado)
cde_fecha_hora_verificacion DATETIME
cde_usu_id_responsable FK → Quién verificó
```

**Ejemplo:**
```
Evento 42 Desmontaje Checklist:
✓ 1 Consola Audio: Encontrada, Bueno
✓ 4 Micrófonos: 4 encontrados, Buenos
✗ 1 Pantalla LED: NO ENCONTRADA (Faltante)
✓ 12 Reflectores: 12 encontrados, Buenos
```

---

### 8. MÉTRICAS POST-EVENTO

#### `tbl_metricas_evento`
**Propósito:** Análisis de desempeño post-evento  
**Campos:**
```
met_id              INT PRIMARY KEY
met_eve_id          FK → Evento
met_duracion_real_horas DECIMAL(5,2)
met_costo_real      DECIMAL(18,2)
met_numero_actividades INT (total planeadas)
met_numero_actividades_completadas INT
met_porcentaje_cumplimiento  COMPUTED (completadas/total * 100)
met_numero_incidencias INT (problemas ocurridos)
met_calificacion_cliente INT (1-5 estrellas)
met_observaciones   VARCHAR (lecciones aprendidas)
```

**Ejemplo Evento 42:**
```
Duración real: 9.5 horas
Costo real: $19,600 (vs $20,000 presupuestado)
Actividades: 11 planeadas, 11 completadas = 100% ✓
Incidencias: 1 (micrófono sin batería, resuelto)
Calificación cliente: 4.8 ⭐
Observaciones: Evento excelente, considerar proveedor audio para próximos
```

---

## Diagrama de Relaciones Simplificado

```
CLIENTES (tbl_clientes)
    ↓ 1:N
EVENTOS (tbl_eventos)
    ├─ 1:N → FASES (tbl_fases_evento)
    │         ├─ 1:N → ACTIVIDADES (tbl_actividades)
    │         │
    │         └─ 1:N → ACTIVIDADES (asignadas a fases)
    │
    ├─ 1:N → PAGOS (tbl_pagos)
    │         ↓
    │         PROVEEDORES (tbl_proveedores)
    │
    ├─ 1:N → ASIGNACION_INVENTARIO
    │         ├─ FK INVENTARIO (tbl_inventario)
    │         └─ FK USUARIOS (personal asignado)
    │
    └─ 1:N → METRICAS (tbl_metricas_evento)

USUARIOS (tbl_usuarios)
    ├─ 1:N ACTIVIDADES (como responsable)
    ├─ 1:N BITACORA (como auditor)
    └─ 1:N EVENTOS (como productor)

INVENTARIO
    ├─ 1:N ASIGNACION_INVENTARIO
    └─ 1:N FOTOS (tbl_fotos_inventario)
```

---

# FLUJOS DE NEGOCIO

## Flujo 1: Crear un Evento

```
1. USUARIO: Entra al sistema (Login)
   ├─ email + password
   ├─ tbl_intentos_login registra intento
   └─ Sistema autentica vs tbl_usuarios

2. USUARIO: Va a "Crear Evento"
   ├─ Selecciona cliente (tbl_clientes)
   ├─ Ingresa datos evento
   └─ Sistema genera eve_codigo único

3. SISTEMA: Crea registro en tbl_eventos
   ├─ eve_estado = "Planificación"
   ├─ eve_fecha_creacion = NOW()
   ├─ eve_usu_id_creador = ID del usuario
   └─ Registra en tbl_bitacora

4. USUARIO: Agrega FASES (Montaje, Evento, Desmontaje)
   ├─ Sistema crea en tbl_fases_evento
   ├─ Cada fase tiene orden y fechas estimadas
   └─ Registra en tbl_bitacora

5. USUARIO: Agrega ACTIVIDADES (tareas)
   ├─ Selecciona fase
   ├─ Ingresa descripción, hora inicio/fin
   ├─ Asigna responsable (FK tbl_usuarios)
   ├─ Sistema calcula act_duracion_estimada_minutos (COMPUTED)
   └─ Estado inicial: "Pendiente"

6. USUARIO: Asigna INVENTARIO necesario
   ├─ Busca items en tbl_inventario
   ├─ Especifica cantidad
   ├─ Sistema verifica stock disponible
   ├─ Crea registro en tbl_asignacion_inventario
   └─ Decrementa inv_stock_actual

7. USUARIO: Registra PROVEEDORES Y PAGOS
   ├─ Selecciona proveedores de tbl_proveedores
   ├─ Por cada proveedor, crea pago
   │  ├─ Concepto (ej: "Arriendo equipo")
   │  ├─ Monto contratado
   │  ├─ Forma de pago
   │  └─ Fecha vencimiento
   └─ Sistema calcula pag_saldo_pendiente automáticamente
```

---

## Flujo 2: Ejecutar Evento (Día del evento)

```
1. USUARIO (Productor General): Abre Dashboard
   ├─ Ve KPIs en tiempo real
   ├─ Cronograma de hoy
   ├─ Alertas de pagos vencidos
   └─ Status de inventario

2. ACTIVIDAD COMIENZA
   ├─ Responsable abre tbl_actividades
   ├─ Marca como "En curso"
   ├─ Sistema guarda act_fecha_hora_real_inicio = NOW()
   └─ Registra en tbl_bitacora

3. CAMBIOS EN VIVO
   ├─ Si hay problema: agrega nota en act_notas_campo
   ├─ Si se retrasa: cambia act_estado = "Retrasado"
   ├─ Si hay incidencia: reporta en app
   └─ Sistema notifica al productor general

4. ACTIVIDAD TERMINA
   ├─ Responsable marca "Completado"
   ├─ Sistema guarda act_fecha_hora_real_fin = NOW()
   ├─ Dashboard actualiza % completado
   └─ Registra en tbl_bitacora

5. DESMONTAJE
   ├─ Personal retira inventario
   ├─ Por cada item, marca en tbl_asignacion_inventario
   │  ├─ Estado de salida (Bueno, Dañado)
   │  └─ Observaciones
   └─ Sistema verifica tbl_checklist_desmontaje
```

---

## Flujo 3: Cierre Post-Evento

```
1. PRODUCTOR GENERAL: Marca evento como "Cerrado"
   ├─ Sistema calcula tbl_metricas_evento
   │  ├─ Duración real
   │  ├─ Costo real (suma de tbl_pagos)
   │  ├─ % cumplimiento de actividades
   │  └─ Incidencias reportadas
   └─ Registra en tbl_bitacora

2. ADMIN FINANCIERO: Procesa pagos pendientes
   ├─ Revisa tbl_pagos con estado "Pendiente"
   ├─ Registra pagos reales
   ├─ Sistema calcula pag_saldo_pendiente
   └─ Envía alertas de vencimiento

3. INVENTARIO: Verifica devoluciones
   ├─ Compara tbl_checklist_desmontaje vs lo asignado
   ├─ Si faltante: genera reporte
   ├─ Actualiza tbl_inventario (stock_actual)
   └─ Si dañado: cambia inv_estado_item = "Dañado"

4. SISTEMA: Actualiza rating de proveedores
   ├─ Cuenta eventos realizados
   ├─ Cuenta retrasos
   ├─ Calcula pro_rating_promedio en tbl_proveedores
   └─ Usa para próximas selecciones

5. GENERACIÓN DE REPORTES
   ├─ Resumen para cliente
   ├─ Análisis financiero
   ├─ Desempeño de proveedores
   └─ Lecciones aprendidas
```

---

# ROL DE AGENTES IA

## Agente 1: Asistente de Gestión Operativa
**Responsabilidad:** Crear y actualizar cronogramas, actividades, asignaciones

**Tareas:**
- Leer `tbl_eventos` y `tbl_actividades` existentes
- Sugerir cronograma basado en tipo de evento (ej: Corporativo vs Festival)
- Alertar si hay conflictos de horario
- Sugerir asignación de personal basado en especialidad
- Validar disponibilidad de inventario

**Queries típicas:**
```sql
SELECT * FROM tbl_eventos WHERE eve_estado='Planificación'
SELECT * FROM tbl_actividades WHERE act_eve_id=42 ORDER BY act_fecha_hora_estimada_inicio
SELECT * FROM tbl_usuarios WHERE usu_rol_id=3  -- Técnicos
SELECT * FROM tbl_inventario WHERE inv_estado_item='Operativo' AND inv_stock_actual > 0
```

---

## Agente 2: Asistente Financiero
**Responsabilidad:** Gestionar pagos, presupuestos, alertas financieras

**Tareas:**
- Monitorear `tbl_pagos` y alertar sobre vencimientos
- Calcular gasto acumulado vs presupuesto
- Sugerir proveedores basado en rating
- Generar reportes de desviación presupuestaria
- Alertar sobre sobregasto

**Queries típicas:**
```sql
SELECT pag_id, pag_monto_contratado, pag_monto_pagado, 
       pag_saldo_pendiente, pag_fecha_vencimiento
FROM tbl_pagos 
WHERE pag_eve_id=42 
  AND pag_estado!='Pagado'
  AND DATEDIFF(day, GETDATE(), pag_fecha_vencimiento) <= 2

SELECT SUM(pag_monto_contratado) as total_gastado
FROM tbl_pagos 
WHERE pag_eve_id=42
```

---

## Agente 3: Asistente de Inventario
**Responsabilidad:** Control de equipo, asignaciones, devoluciones

**Tareas:**
- Verificar disponibilidad de items
- Alertar sobre stock bajo
- Registrar asignaciones y devoluciones
- Detectar faltantes en desmontaje
- Reportar daños

**Queries típicas:**
```sql
SELECT inv_id, inv_nombre, inv_stock_actual, inv_stock_minimo
FROM tbl_inventario
WHERE inv_stock_actual <= inv_stock_minimo

SELECT * FROM tbl_asignacion_inventario
WHERE asi_eve_id=42 AND asi_fecha_hora_entrada IS NULL
```

---

## Agente 4: Asistente de Auditoría y Reportes
**Responsabilidad:** Auditoría, seguridad, análisis post-evento

**Tareas:**
- Monitorear `tbl_bitacora` para cambios críticos
- Detectar intentos de login fallidos (tbl_intentos_login)
- Generar reportes de desempeño (tbl_metricas_evento)
- Analizar histórico de eventos para mejoras
- Detectar patrones de proveedores problemáticos

**Queries típicas:**
```sql
SELECT * FROM tbl_bitacora 
WHERE bit_fecha_hora >= GETDATE()-1 
ORDER BY bit_fecha_hora DESC

SELECT pro_nombre, pro_numero_eventos_realizados, pro_numero_retrasos,
       CONVERT(decimal(3,2), pro_numero_retrasos*100.0/pro_numero_eventos_realizados) as porcentaje_retrasos
FROM tbl_proveedores
ORDER BY porcentaje_retrasos DESC
```

---

# GUÍA TÉCNICA

## Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Frontend | ASP.NET MVC + Blazor (C#) |
| Backend | C# ASP.NET Web API |
| Base de Datos | SQL Server 2019+ |
| ORM | Entity Framework Core |
| Testing | NUnit, Moq |
| Versionamiento | Git |

## Patrones de Diseño

```
Repository Pattern:
├─ IEventoRepository
├─ IActividadRepository
├─ IPagoRepository
└─ etc.

Service Layer:
├─ EventoService
├─ ActividadService
├─ PagoService
└─ etc.

Controller Layer:
├─ EventosController
├─ ActividadesController
├─ PagosController
└─ etc.
```

## Transacciones Críticas

```csharp
// Crear evento con todas sus relaciones en 1 transacción
using (var transaction = _context.Database.BeginTransaction())
{
    try
    {
        // 1. Crear evento
        var evento = new Evento { ... };
        _context.Eventos.Add(evento);
        
        // 2. Crear fases
        var fases = new List<Fase> { ... };
        _context.Fases.AddRange(fases);
        
        // 3. Crear actividades
        var actividades = new List<Actividad> { ... };
        _context.Actividades.AddRange(actividades);
        
        // 4. Guardar todo o nada
        _context.SaveChanges();
        transaction.Commit();
        
        // 5. Registrar en auditoría
        _bitacoraService.Log(usuarioId, "INSERT", "tbl_eventos", evento.Id);
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}
```

## Validaciones en Negocio

```csharp
// Validar cronograma
- No puede haber actividades solapadas para el mismo responsable
- Duración estimada debe ser positiva (fin > inicio)
- Responsable debe ser usuario activo
- Actividad debe pertenecer a evento existente

// Validar pagos
- Monto pagado NO puede exceder monto contratado
- Fecha vencimiento debe ser >= fecha creación
- Proveedor debe estar registrado y activo

// Validar inventario
- No asignar más stock del disponible
- Inventario dañado no puede ser asignado
- Debe haber registro de entrada al terminar evento
```

---

## Seguridad

### Autenticación
```
1. Usuario ingresa email + password
2. Sistema valida contra tbl_usuarios (password_hash con bcrypt)
3. Si exitoso: genera JWT token con expiración 8h
4. Registra en tbl_intentos_login
5. Si 3 intentos fallidos: bloquea IP por 15 minutos
```

### Autorización
```
Cada endpoint valida:
- Usuario autenticado
- Usuario tiene rol requerido
- Evento pertenece a su tenancy (si aplica)
- Timestamp de token válido
```

### Auditoría
```
TODA acción que modifique datos:
- INSERT tbl_bitacora
  ├─ bit_usu_id = usuario actual
  ├─ bit_accion = INSERT/UPDATE/DELETE
  ├─ bit_tabla_afectada = nombre tabla
  ├─ bit_id_registro_afectado = ID del registro
  ├─ bit_detalle = "Estado: Pendiente → En curso"
  ├─ bit_ip_origen = IP del cliente
  └─ bit_fecha_hora = NOW()
```

---

## Performance

### Índices Críticos
```sql
-- Búsqueda rápida de eventos por estado
CREATE INDEX IDX_eventos_estado ON tbl_eventos(eve_estado)

-- Búsqueda de actividades por evento y fecha
CREATE INDEX IDX_actividades_fecha ON tbl_actividades(act_eve_id, act_fecha_hora_estimada_inicio)

-- Búsqueda de pagos pendientes
CREATE INDEX IDX_pagos_estado ON tbl_pagos(pag_estado, pag_fecha_vencimiento)

-- Búsqueda rápida en inventario
CREATE INDEX IDX_inventario_codigo ON tbl_inventario(inv_codigo_interno)
```

### Query Optimization
```
-- EVITAR:
SELECT * FROM tbl_eventos
SELECT * FROM tbl_actividades WHERE act_eve_id IN (SELECT eve_id FROM tbl_eventos)

-- PREFERIR:
SELECT eve_id, eve_nombre, eve_estado FROM tbl_eventos WHERE eve_estado='Ejecución'
SELECT a.* FROM tbl_actividades a
INNER JOIN tbl_eventos e ON a.act_eve_id = e.eve_id
WHERE e.eve_id = 42
```

---

## Deployment

```
Entorno Local:
├─ Visual Studio Community
├─ SQL Server Developer Edition
├─ .NET 6+
└─ Git

Entorno Producción:
├─ IIS Server (Windows Server 2019+)
├─ SQL Server Enterprise
├─ SSL/TLS obligatorio
└─ Backups automáticos diarios
```

---

## Roadmap Post-MVP

- **SignalR:** Actualizaciones en tiempo real (websockets)
- **Machine Learning:** Predictor de retrasos basado en histórico
- **Mobile App:** App nativa iOS/Android
- **Offline Mode:** Sincronización automática cuando se reconecta
- **Integraciones:** APIs con software contable, sistemas de pago
- **Analytics Avanzado:** Dashboards predictivos

---

## Contacto y Soporte

**Desarrollador:** Andrés Aldeán  
**Email:** andrés@techprod.com  
**Documentación:** https://docs.eventhub.local  
**Base de datos:** EventHubv01 en SQL Server  

---

**Documento versión 1.0 | Generado Junio 2026**
