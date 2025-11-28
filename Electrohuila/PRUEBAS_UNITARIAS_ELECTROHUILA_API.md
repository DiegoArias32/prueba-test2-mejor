# Documento de Pruebas Unitarias - ElectroHuila API

**Realizado por**: Desarrolladores ElectroHuila
**Fecha**: 28 de Noviembre, 2025
**Versión**: 1.0
**Actividad de Proyecto**: Desarrollo y Validación de Sistema de Agendamiento de Citas PQR ElectroHuila

---

## Propósito e Introducción

El propósito del siguiente documento es realizar seguimiento y validación operacional del Sistema de Agendamiento de Citas PQR ElectroHuila mediante la implementación de pruebas unitarias exhaustivas.

Las pruebas unitarias nos permiten asegurar la calidad operacional de nuestro sistema, validando que cada método individual del código funcione correctamente, así como verificar la integración completa de todos los componentes del proyecto ElectroHuila.

Se busca en los **requerimientos funcionales**: detectar y corregir errores relacionados con la funcionalidad del sistema de agendamiento, cancelación y gestión de citas. En los **requerimientos no funcionales**, identificar fallas de rendimiento, seguridad, usabilidad y fiabilidad que puedan afectar la experiencia del usuario final.

Esta orientación operacional nos permite establecer un marco de trabajo robusto para garantizar que el Sistema ElectroHuila cumpla con los estándares de calidad requeridos en un entorno de producción, asegurando la confiabilidad del servicio para los usuarios de ElectroHuila.

---

## Índice de Pruebas Unitarias

### 1. Pruebas de Application Layer

#### 1.1 CancelAppointmentCommandHandlerTests.cs

**Realizado por**: Team Backend

| Versión | Título                                                        |
| -------- | -------------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Cancelar cita pendiente exitosamente       |
| 2        | Prueba Unitaria 02: Cancelar cita confirmada exitosamente      |
| 3        | Prueba Unitaria 03: Error al cancelar cita inexistente         |
| 4        | Prueba Unitaria 04: Error al cancelar cita ya cancelada        |
| 5        | Prueba Unitaria 05: Error al cancelar cita completada          |
| 6        | Prueba Unitaria 06: Validación de actualización de timestamp |
| 7        | Prueba Unitaria 07: Manejo de excepciones en repositorio       |

---

**Detalle de Prueba 01: Cancelar Cita Pendiente**

| **Aspecto**                 | **Detalle**                                                                                                                                                                                                                                                                                                                                                                            |
| --------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-003: El sistema debe permitir la cancelación de citas con estado "Pendiente" o "Confirmada", registrando la razón de cancelación y actualizando el estado a "Cancelada".                                                                                                                                                                                                               |
| **Objetivo**                | Verificar que el sistema permita cancelar exitosamente una cita con estado "Pendiente", cambiando su estado a "Cancelada" y almacenando la razón proporcionada.                                                                                                                                                                                                                             |
| **Tipo de Prueba**          | Prueba Unitaria - Caja Blanca                                                                                                                                                                                                                                                                                                                                                                |
| **Datos de Entrada**        | •`appointmentId`: 1`<br>`• `cancellationReason`: "El cliente solicitó cambio de fecha"`<br>`• Estado inicial de cita: "Pendiente"`<br>`• Cita existente en base de datos                                                                                                                                                                                                      |
| **Procedimiento**           | 1. Arrange: Configurar mock del repositorio con cita en estado "Pendiente"`<br>`2. Configurar comando de cancelación con ID y razón válidos`<br>`3. Act: Ejecutar `Handle(command, CancellationToken.None)<br>`4. Assert: Verificar que el resultado sea exitoso`<br>`5. Verificar que el estado cambió a "Cancelada"`<br>`6. Validar que se guardó la razón de cancelación |
| **Resultados Esperados**    | •`result.IsSuccess` = true`<br>`• `appointment.Status` = "Cancelada"`<br>`• `appointment.CancellationReason` = "El cliente solicitó cambio de fecha"`<br>`• `UpdateAsync()` invocado 1 vez`<br>`• Sin excepciones lanzadas                                                                                                                                             |

---

**Detalle de Prueba 02: Cancelar Cita Confirmada**

| **Aspecto**                 | **Detalle**                                                                                                                                                       |
| --------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-003: El sistema debe permitir la cancelación de citas confirmadas antes de su fecha programada.                                                                     |
| **Objetivo**                | Validar que citas con estado "Confirmada" puedan ser canceladas correctamente.                                                                                          |
| **Tipo de Prueba**          | Prueba Unitaria - Caja Blanca                                                                                                                                           |
| **Datos de Entrada**        | •`appointmentId`: 2`<br>`• `cancellationReason`: "Emergencia familiar"`<br>`• Estado inicial: "Confirmada"`<br>`• Fecha de cita: Mañana 14:00            |
| **Procedimiento**           | 1. Configurar cita confirmada en mock repository`<br>`2. Crear comando de cancelación`<br>`3. Ejecutar handler`<br>`4. Verificar cambio de estado y persistencia |
| **Resultados Esperados**    | • Cancelación exitosa`<br>`• Estado = "Cancelada"`<br>`• Razón registrada correctamente`<br>`• Repositorio actualizado                                      |

---

**Detalle de Prueba 03: Error al Cancelar Cita Inexistente**

| **Aspecto**                 | **Detalle**                                                                                                                                               |
| --------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RNF-002: El sistema debe manejar errores de forma controlada y retornar mensajes descriptivos.                                                                  |
| **Objetivo**                | Verificar que el sistema maneje correctamente el intento de cancelar una cita que no existe.                                                                    |
| **Tipo de Prueba**          | Prueba Unitaria - Manejo de Errores                                                                                                                             |
| **Datos de Entrada**        | •`appointmentId`: 999 (inexistente)`<br>`• `cancellationReason`: "Cualquier razón"                                                                     |
| **Procedimiento**           | 1. Configurar repositorio para retornar null`<br>`2. Crear comando con ID inexistente`<br>`3. Ejecutar handler`<br>`4. Verificar respuesta de error       |
| **Resultados Esperados**    | •`result.IsFailure` = true`<br>`• Mensaje de error: "Cita no encontrada"`<br>`• `UpdateAsync()` NO invocado`<br>`• Sin excepciones no controladas |

---

**Detalle de Prueba 04: Error al Cancelar Cita Ya Cancelada**

| **Aspecto**                 | **Detalle**                                                                                                                                     |
| --------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RNF-003: El sistema debe prevenir operaciones duplicadas sobre entidades ya procesadas.                                                               |
| **Objetivo**                | Validar que no se permita cancelar una cita que ya tiene estado "Cancelada".                                                                          |
| **Tipo de Prueba**          | Prueba Unitaria - Validación de Reglas de Negocio                                                                                                    |
| **Datos de Entrada**        | •`appointmentId`: 3`<br>`• Estado actual: "Cancelada"`<br>`• Razón anterior: "Cancelación previa"                                          |
| **Procedimiento**           | 1. Configurar cita con estado "Cancelada"`<br>`2. Intentar cancelar nuevamente`<br>`3. Verificar rechazo de la operación                         |
| **Resultados Esperados**    | •`result.IsFailure` = true`<br>`• Mensaje: "La cita ya está cancelada"`<br>`• Estado sin cambios`<br>`• Base de datos sin modificaciones |

---

#### 1.2 CreateAppointmentCommandHandlerTests.cs

**Realizado por**: Team Backend

| Versión | Título                                                        |
| -------- | -------------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Crear cita exitosamente con datos válidos |
| 2        | Prueba Unitaria 02: Manejo de excepciones en base de datos     |

---

**Detalle de Prueba 01: Crear Cita con Datos Válidos**

| **Aspecto**                 | **Detalle**                                                                                                                                                                                                                                                                                                                                                                 |
| --------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-001: El sistema debe permitir la creación de citas validando disponibilidad de horarios y asignando un número único de cita.                                                                                                                                                                                                                                                |
| **Objetivo**                | Verificar que el sistema cree exitosamente una cita cuando se proporcionan todos los datos válidos requeridos.                                                                                                                                                                                                                                                                   |
| **Tipo de Prueba**          | Prueba Unitaria - Caja Blanca                                                                                                                                                                                                                                                                                                                                                     |
| **Datos de Entrada**        | •`clientId`: 1`<br>`• `branchId`: 1`<br>`• `appointmentTypeId`: 1`<br>`• `appointmentDate`: DateTime.UtcNow.AddDays(2)`<br>`• `appointmentTime`: "14:30"`<br>`• `observations`: "Consulta de seguimiento"                                                                                                                                             |
| **Procedimiento**           | 1. Arrange: Configurar mocks de repositorios (Appointment, Holiday, Branch, UserAssignment, SystemSetting)`<br>`2. Configurar mapper para mapear entidad a DTO`<br>`3. Configurar servicios de notificación (SignalR, Notification)`<br>`4. Act: Ejecutar `Handle(command, CancellationToken.None)<br>`5. Assert: Verificar resultado exitoso y número de cita asignado |
| **Resultados Esperados**    | •`result.IsSuccess` = true`<br>`• `result.Data` contiene AppointmentDto`<br>`• `appointmentNumber` generado (formato: "APT-XXXXXX")`<br>`• `AddAsync()` invocado en repositorio`<br>`• Notificaciones enviadas correctamente                                                                                                                                 |

---

**Detalle de Prueba 02: Manejo de Excepciones**

| **Aspecto**                 | **Detalle**                                                                                                                                             |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RNF-002: El sistema debe manejar errores de persistencia sin afectar la estabilidad del sistema.                                                              |
| **Objetivo**                | Validar que el handler maneje correctamente excepciones de base de datos durante la creación de citas.                                                       |
| **Tipo de Prueba**          | Prueba Unitaria - Manejo de Excepciones                                                                                                                       |
| **Datos de Entrada**        | • Datos válidos de cita`<br>`• Repositorio configurado para lanzar DbUpdateException                                                                     |
| **Procedimiento**           | 1. Configurar mock para lanzar excepción en AddAsync()`<br>`2. Ejecutar comando de creación`<br>`3. Verificar manejo de error                           |
| **Resultados Esperados**    | •`result.IsFailure` = true`<br>`• Mensaje de error descriptivo`<br>`• Sistema estable sin propagación de excepción`<br>`• Log de error generado |

---

#### 1.3 RefreshTokenCommandHandlerTests.cs

**Realizado por**: Team Security

| Versión | Título                                           |
| -------- | ------------------------------------------------- |
| 1        | Prueba Unitaria 01: Token refresh no implementado |
| 2        | Prueba Unitaria 02: Validación con token vacío  |
| 3        | Prueba Unitaria 03: Validación con token null    |
| 4        | Prueba Unitaria 04: Token inválido formato       |
| 5        | Prueba Unitaria 05: Token expirado                |

---

**Detalle de Prueba 01: Token Refresh No Implementado**

| **Aspecto**                 | **Detalle**                                                                                                           |
| --------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-005: El sistema debe notificar cuando funcionalidades de autenticación no estén implementadas.                         |
| **Objetivo**                | Verificar que el handler retorne un error controlado indicando que la funcionalidad de refresh token no está implementada. |
| **Tipo de Prueba**          | Prueba Unitaria - Validación de Estado                                                                                     |
| **Datos de Entrada**        | •`refreshToken`: "valid-token-format"                                                                                    |
| **Procedimiento**           | 1. Crear comando con token válido`<br>`2. Ejecutar handler`<br>`3. Verificar respuesta de no implementado              |
| **Resultados Esperados**    | •`result.IsFailure` = true`<br>`• Mensaje: "Refresh token no implementado"`<br>`• `result.Data` = null           |

---

**Detalle de Prueba 02: Validación Token Vacío**

| **Aspecto**                 | **Detalle**                                                                                                   |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RNF-004: El sistema debe validar todos los parámetros de entrada antes de procesarlos.                             |
| **Objetivo**                | Validar que el sistema rechace tokens de refresh vacíos.                                                           |
| **Tipo de Prueba**          | Prueba Unitaria - Validación de Entrada                                                                            |
| **Datos de Entrada**        | •`refreshToken`: "" (string vacío)                                                                              |
| **Procedimiento**           | 1. Crear comando con string vacío`<br>`2. Ejecutar Handle()`<br>`3. Verificar error de validación             |
| **Resultados Esperados**    | •`result.IsFailure` = true`<br>`• Validación de campo requerido fallida`<br>`• Sin acceso a base de datos |

---

**Detalle de Prueba 03: Validación Token Null**

| **Aspecto**                 | **Detalle**                                                                                                           |
| --------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RNF-004: Validación de datos nulos en parámetros críticos de seguridad.                                                  |
| **Objetivo**                | Verificar manejo de valores null en refresh token.                                                                          |
| **Tipo de Prueba**          | Prueba Unitaria - Validación de Nulos                                                                                      |
| **Datos de Entrada**        | •`refreshToken`: null                                                                                                    |
| **Procedimiento**           | 1. Intentar crear comando con null`<br>`2. Capturar excepción o error de validación`<br>`3. Verificar manejo correcto |
| **Resultados Esperados**    | • Excepción de validación o`<br>`• `result.IsFailure` = true`<br>`• Mensaje descriptivo del error                |

---

### 2. Pruebas de Domain Layer

#### 2.1 AppointmentTests.cs

**Realizado por**: Team Domain

| Versión | Título                                           |
| -------- | ------------------------------------------------- |
| 1        | Prueba Unitaria 01: Crear cita con datos válidos |
| 2        | Prueba Unitaria 02: Actualizar estado de cita     |
| 3        | Prueba Unitaria 03: Propiedades de auditoría     |

---

**Detalle de Prueba 01: Crear Cita con Datos Válidos**

| **Aspecto**                 | **Detalle**                                                                                                                                                                                                                                                       |
| --------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-001: La entidad Appointment debe mantener integridad referencial con Client, Branch, AppointmentType y AppointmentStatus.                                                                                                                                            |
| **Objetivo**                | Validar que la entidad Appointment se instancie correctamente con todas sus propiedades y relaciones.                                                                                                                                                                   |
| **Tipo de Prueba**          | Prueba Unitaria - Entidad de Dominio                                                                                                                                                                                                                                    |
| **Datos de Entrada**        | •`id`: 1`<br>`• `clientId`: 1`<br>`• `branchId`: 1`<br>`• `appointmentTypeId`: 1`<br>`• `appointmentDate`: DateTime.Today.AddDays(1)`<br>`• `appointmentTime`: "10:00"`<br>`• `statusId`: 2`<br>`• `appointmentNumber`: "APT-001" |
| **Procedimiento**           | 1. Crear instancia de Appointment`<br>`2. Asignar propiedades`<br>`3. Verificar valores asignados`<br>`4. Validar integridad referencial                                                                                                                          |
| **Resultados Esperados**    | • Todas las propiedades asignadas correctamente`<br>`• `CreatedAt` establecido automáticamente`<br>`• `IsActive` = true por defecto`<br>`• Relaciones navegables (Client, Branch, etc.)                                                                  |

---

**Detalle de Prueba 02: Actualizar Estado de Cita**

| **Aspecto**                 | **Detalle**                                                                                                                     |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-004: La entidad debe permitir cambios de estado manteniendo trazabilidad.                                                          |
| **Objetivo**                | Verificar que el estado de una cita pueda actualizarse correctamente.                                                                 |
| **Tipo de Prueba**          | Prueba Unitaria - Mutación de Estado                                                                                                 |
| **Datos de Entrada**        | • Cita existente con `statusId`: 1 (Pendiente)`<br>`• Nuevo `statusId`: 2 (Confirmada)                                        |
| **Procedimiento**           | 1. Crear cita con estado inicial`<br>`2. Cambiar statusId`<br>`3. Verificar actualización`<br>`4. Validar UpdatedAt modificado |
| **Resultados Esperados**    | •`statusId` actualizado correctamente`<br>`• `UpdatedAt` modificado`<br>`• Estado anterior no afecta nueva asignación     |

---

**Detalle de Prueba 03: Propiedades de Auditoría**

| **Aspecto**                 | **Detalle**                                                                                                             |
| --------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RNF-005: Todas las entidades deben mantener campos de auditoría (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy).                |
| **Objetivo**                | Validar que las propiedades de auditoría se establezcan automáticamente.                                                    |
| **Tipo de Prueba**          | Prueba Unitaria - Auditoría                                                                                                  |
| **Datos de Entrada**        | • Nueva instancia de Appointment                                                                                             |
| **Procedimiento**           | 1. Crear nueva cita`<br>`2. Verificar CreatedAt automático`<br>`3. Actualizar entidad`<br>`4. Validar UpdatedAt        |
| **Resultados Esperados**    | •`CreatedAt` ≈ DateTime.UtcNow`<br>`• `UpdatedAt` null inicialmente`<br>`• Campos listos para tracking de cambios |

---

#### 2.2 EmailTests.cs

**Realizado por**: Team Domain

| Versión | Título                                                     |
| -------- | ----------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Comparación de emails case-insensitive |

---

**Detalle de Prueba 01: Comparación Case-Insensitive**

| **Aspecto**                 | **Detalle**                                                                                                                                                                                          |
| --------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-006: El sistema debe tratar emails como case-insensitive para evitar duplicados.                                                                                                                        |
| **Objetivo**                | Verificar que el Value Object Email maneje correctamente la comparación sin distinguir mayúsculas/minúsculas.                                                                                           |
| **Tipo de Prueba**          | Prueba Unitaria - Value Object                                                                                                                                                                             |
| **Datos de Entrada**        | •`email1`: "Test@Example.com"`<br>`• `email2`: "test@example.com"`<br>`• `email3`: "TEST@EXAMPLE.COM"                                                                                         |
| **Procedimiento**           | 1. Crear tres instancias de Email con diferentes casos`<br>`2. Comparar email1 con email2`<br>`3. Comparar email2 con email3`<br>`4. Verificar igualdad en todos los casos                           |
| **Resultados Esperados**    | •`email1 == email2` = true`<br>`• `email2 == email3` = true`<br>`• `email1 == email3` = true`<br>`• `GetHashCode()` igual para todos`<br>`• Normalización a minúsculas internamente |

---

### 3. Pruebas de Infrastructure Layer

#### 3.1 AppointmentRepositoryTests.cs

**Realizado por**: Team Infrastructure

| Versión | Título                                             |
| -------- | --------------------------------------------------- |
| 1        | Prueba Unitaria 01: Obtener cita por ID existente   |
| 2        | Prueba Unitaria 02: Obtener cita por ID inexistente |
| 3        | Prueba Unitaria 03: Obtener todas las citas activas |
| 4        | Prueba Unitaria 04: Filtrar citas por sucursal      |
| 5        | Prueba Unitaria 05: Filtrar citas por fecha         |
| 6        | Prueba Unitaria 06: Obtener citas por cliente       |
| 7        | Prueba Unitaria 07: Agregar nueva cita              |
| 8        | Prueba Unitaria 08: Actualizar cita existente       |
| 9        | Prueba Unitaria 09: Eliminar cita (soft delete)     |

---

**Detalle de Prueba 01: Obtener Cita por ID Existente**

| **Aspecto**                 | **Detalle**                                                                                                                                                                                                                                      |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Requerimiento Funcional** | RF-007: El repositorio debe recuperar citas por ID incluyendo todas sus relaciones (Branch, Client, AppointmentType, Status).                                                                                                                          |
| **Objetivo**                | Verificar que el método GetByIdAsync retorne correctamente una cita con todas sus entidades relacionadas cargadas.                                                                                                                                    |
| **Tipo de Prueba**          | Prueba de Integración con EF In-Memory                                                                                                                                                                                                                |
| **Datos de Entrada**        | •`appointmentId`: 1`<br>`• Cita en BD con:`<br>`&nbsp;&nbsp;- appointmentNumber: "APT-001"`<br>`&nbsp;&nbsp;- clientId: 1`<br>`&nbsp;&nbsp;- branchId: 1`<br>`&nbsp;&nbsp;- appointmentTypeId: 1`<br>`&nbsp;&nbsp;- isActive: true     |
| **Procedimiento**           | 1. Configurar DbContext con In-Memory Database`<br>`2. Seed de datos: crear Branch, Client, AppointmentType, Status`<br>`3. Crear Appointment con relaciones`<br>`4. Ejecutar `GetByIdAsync(1)<br>`5. Verificar entidad retornada y relaciones |
| **Resultados Esperados**    | • Resultado != null`<br>`• `appointment.Id` = 1`<br>`• `appointment.Client` cargado`<br>`• `appointment.Branch` cargado`<br>`• `appointment.AppointmentType` cargado`<br>`• `appointment.Status` cargado                     |

---

**Detalle de Prueba 02: Obtener Cita por ID Inexistente**

| **Aspecto**                 | **Detalle**                                                                               |
| --------------------------------- | ----------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RNF-006: El repositorio debe retornar null cuando no se encuentre un registro.                  |
| **Objetivo**                | Validar el comportamiento cuando se busca una cita que no existe.                               |
| **Tipo de Prueba**          | Prueba de Integración - Caso Negativo                                                          |
| **Datos de Entrada**        | •`appointmentId`: 999 (no existe en BD)                                                      |
| **Procedimiento**           | 1. Configurar BD vacía`<br>`2. Ejecutar GetByIdAsync(999)`<br>`3. Verificar resultado null |
| **Resultados Esperados**    | • Resultado = null`<br>`• Sin excepciones lanzadas                                          |

---

**Detalle de Prueba 03: Obtener Todas las Citas Activas**

| **Aspecto**                 | **Detalle**                                                                                                  |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| **Requerimiento Funcional** | RF-008: El sistema debe listar solo citas activas excluyendo las eliminadas lógicamente.                          |
| **Objetivo**                | Verificar que GetAllAsync retorne únicamente citas con IsActive = true.                                           |
| **Tipo de Prueba**          | Prueba de Integración - Filtrado                                                                                  |
| **Datos de Entrada**        | • 3 citas activas (IsActive = true)`<br>`• 2 citas inactivas (IsActive = false)                                |
| **Procedimiento**           | 1. Seed de 5 citas (3 activas, 2 inactivas)`<br>`2. Ejecutar GetAllAsync()`<br>`3. Verificar cantidad y estado |
| **Resultados Esperados**    | • Count = 3`<br>`• Todas con IsActive = true`<br>`• Citas inactivas excluidas                               |

---

**Detalle de Prueba 07: Agregar Nueva Cita**

| **Aspecto**                 | **Detalle**                                                                                                                                       |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-009: El repositorio debe persistir nuevas citas asignando ID automáticamente.                                                                       |
| **Objetivo**                | Validar que AddAsync guarde correctamente una nueva cita en la base de datos.                                                                           |
| **Tipo de Prueba**          | Prueba de Integración - Escritura                                                                                                                      |
| **Datos de Entrada**        | • Nueva Appointment sin ID`<br>`• clientId: 1`<br>`• branchId: 1`<br>`• appointmentDate: Tomorrow`<br>`• appointmentTime: "09:00"          |
| **Procedimiento**           | 1. Crear entidad Appointment nueva`<br>`2. Ejecutar AddAsync(appointment)`<br>`3. SaveChangesAsync()`<br>`4. Verificar ID asignado y persistencia |
| **Resultados Esperados**    | •`appointment.Id` > 0`<br>`• Registro existe en BD`<br>`• CreatedAt establecido`<br>`• SaveChangesAsync ejecutado exitosamente              |

---

#### 3.2 BranchRepositoryTests.cs

**Realizado por**: Team Infrastructure

| Versión | Título                                                    |
| -------- | ---------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Obtener sucursal activa por ID         |
| 2        | Prueba Unitaria 02: Filtrar sucursales inactivas           |
| 3        | Prueba Unitaria 03: Obtener todas las sucursales ordenadas |

---

**Detalle de Prueba 01: Obtener Sucursal Activa por ID**

| **Aspecto**                 | **Detalle**                                                                                                                          |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| **Requerimiento Funcional** | RF-010: El repositorio debe recuperar sucursales activas por ID con su información completa.                                              |
| **Objetivo**                | Verificar que GetByIdAsync retorne correctamente una sucursal activa.                                                                      |
| **Tipo de Prueba**          | Prueba de Integración con EF In-Memory                                                                                                    |
| **Datos de Entrada**        | •`branchId`: 1`<br>`• Branch: { name: "Sucursal Centro", code: "SC01", isActive: true }                                              |
| **Procedimiento**           | 1. Seed sucursal en BD In-Memory`<br>`2. Ejecutar GetByIdAsync(1)`<br>`3. Verificar datos retornados                                   |
| **Resultados Esperados**    | • Resultado != null`<br>`• `branch.Name` = "Sucursal Centro"`<br>`• `branch.Code` = "SC01"`<br>`• `branch.IsActive` = true |

---

**Detalle de Prueba 02: Filtrar Sucursales Inactivas**

| **Aspecto**                 | **Detalle**                                                                                                        |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| **Requerimiento Funcional** | RF-011: El sistema debe excluir sucursales inactivas de listados generales.                                              |
| **Objetivo**                | Validar que GetAllActiveAsync solo retorne sucursales con IsActive = true.                                               |
| **Tipo de Prueba**          | Prueba de Integración - Filtrado                                                                                        |
| **Datos de Entrada**        | • 3 sucursales activas`<br>`• 2 sucursales inactivas                                                                 |
| **Procedimiento**           | 1. Seed 5 sucursales (3 activas, 2 inactivas)`<br>`2. Ejecutar GetAllActiveAsync()`<br>`3. Verificar conteo y estado |
| **Resultados Esperados**    | • Count = 3`<br>`• Todas IsActive = true`<br>`• Sucursales inactivas excluidas                                    |

---

**Detalle de Prueba 03: Obtener Todas las Sucursales Ordenadas**

| **Aspecto**                 | **Detalle**                                                                                                       |
| --------------------------------- | ----------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-012: Las sucursales deben listarse ordenadas alfabéticamente por nombre.                                            |
| **Objetivo**                | Verificar que el listado de sucursales se retorne en orden alfabético ascendente.                                      |
| **Tipo de Prueba**          | Prueba de Integración - Ordenamiento                                                                                   |
| **Datos de Entrada**        | • Sucursal "Zeta"`<br>`• Sucursal "Alpha"`<br>`• Sucursal "Beta"`<br>`• Todas activas                         |
| **Procedimiento**           | 1. Seed sucursales en orden aleatorio`<br>`2. Ejecutar GetAllActiveAsync()`<br>`3. Verificar orden alfabético      |
| **Resultados Esperados**    | • Posición 0: "Alpha"`<br>`• Posición 1: "Beta"`<br>`• Posición 2: "Zeta"`<br>`• Orden ascendente por Name |

---

#### 3.3 JwtTokenGeneratorTests.cs

**Realizado por**: Team Security

| Versión | Título                                                 |
| -------- | ------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Generar token JWT válido           |
| 2        | Prueba Unitaria 02: Incluir claims del usuario en token |

---

**Detalle de Prueba 01: Generar Token JWT Válido**

| **Aspecto**                 | **Detalle**                                                                                                                                                                                                                                       |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-013: El sistema debe generar tokens JWT con formato estándar (Header.Payload.Signature).                                                                                                                                                            |
| **Objetivo**                | Verificar que el generador de JWT cree tokens con formato válido y no vacíos.                                                                                                                                                                         |
| **Tipo de Prueba**          | Prueba Unitaria - Seguridad                                                                                                                                                                                                                             |
| **Datos de Entrada**        | • User: { id: 1, username: "testuser", email: "test@electrohuila.com", fullName: "Usuario Prueba", isActive: true }`<br>`• Roles: ["Admin"]`<br>`• Permissions: ["Read", "Write"]                                                                |
| **Procedimiento**           | 1. Configurar JwtSettings (Secret, Issuer, Audience, ExpirationMinutes)`<br>`2. Crear instancia de JwtTokenGenerator`<br>`3. Ejecutar GenerateToken(user, roles, permissions)`<br>`4. Verificar formato del token (3 partes separadas por puntos) |
| **Resultados Esperados**    | • Token != null`<br>`• Token != string.Empty`<br>`• `token.Split('.').Length` = 3`<br>`• Cada parte contiene caracteres Base64Url                                                                                                           |

---

**Detalle de Prueba 02: Incluir Claims del Usuario**

| **Aspecto**                 | **Detalle**                                                                                                                                                                                                        |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Requerimiento Funcional** | RF-014: Los tokens JWT deben incluir claims de identificación del usuario (UserId, Email, Username).                                                                                                                    |
| **Objetivo**                | Validar que el token generado contenga los claims correctos del usuario.                                                                                                                                                 |
| **Tipo de Prueba**          | Prueba Unitaria - Validación de Claims                                                                                                                                                                                  |
| **Datos de Entrada**        | • User con datos completos`<br>`• Roles: ["Admin", "User"]`<br>`• Permissions: ["Appointments.Create"]                                                                                                            |
| **Procedimiento**           | 1. Generar token`<br>`2. Decodificar payload del token`<br>`3. Extraer claims`<br>`4. Verificar presencia de UserId, Email, Username                                                                               |
| **Resultados Esperados**    | • Claim "sub" (UserId) presente`<br>`• Claim "email" presente`<br>`• Claim "unique_name" (Username) presente`<br>`• Claims de roles y permisos incluidos`<br>`• Expiración (exp) configurada correctamente |

---

### 4. Pruebas de Integration Tests

#### 4.1 HealthCheckTests.cs

**Realizado por**: Team DevOps

| Versión | Título                                                  |
| -------- | -------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Health check endpoint responde OK    |
| 2        | Prueba Unitaria 02: Ping endpoint responde correctamente |

---

**Detalle de Prueba 01: Health Check Endpoint Responde OK**

| **Aspecto**                 | **Detalle**                                                                                                                                                      |
| --------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-015: El sistema debe exponer un endpoint de health check que retorne el estado de salud del sistema.                                                                |
| **Objetivo**                | Verificar que el endpoint /health responda correctamente con estado 200 OK y contenido "Healthy".                                                                      |
| **Tipo de Prueba**          | Prueba de Integración - Health Check                                                                                                                                  |
| **Datos de Entrada**        | • Request: GET /api/v1/setup/health`<br>`• Headers: { "Content-Type": "application/json" }                                                                         |
| **Procedimiento**           | 1. Configurar WebApplicationFactory`<br>`2. Crear HttpClient de prueba`<br>`3. Ejecutar GET /api/v1/setup/health`<br>`4. Verificar StatusCode y contenido        |
| **Resultados Esperados**    | •`response.StatusCode` = 200 (OK)`<br>`• `response.IsSuccessStatusCode` = true`<br>`• Contenido incluye "Healthy"`<br>`• Tiempo de respuesta < 1 segundo |

---

**Detalle de Prueba 02: Ping Endpoint Responde Correctamente**

| **Aspecto**                 | **Detalle**                                                                                    |
| --------------------------------- | ---------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-016: El sistema debe proporcionar un endpoint de ping para verificación básica de conectividad. |
| **Objetivo**                | Validar que el endpoint /ping retorne "pong" correctamente.                                          |
| **Tipo de Prueba**          | Prueba de Integración - Conectividad                                                                |
| **Datos de Entrada**        | • Request: GET /api/v1/setup/ping                                                                   |
| **Procedimiento**           | 1. Crear cliente HTTP de prueba`<br>`2. Ejecutar GET /ping`<br>`3. Verificar respuesta "pong"    |
| **Resultados Esperados**    | • StatusCode = 200`<br>`• Contenido = "pong"`<br>`• Respuesta inmediata (< 100ms)             |

---

### 5. Pruebas E2E

#### 5.1 SetupControllerTests.cs

**Realizado por**: Team QA

| Versión | Título                                                                  |
| -------- | ------------------------------------------------------------------------ |
| 1        | Prueba Unitaria 01: Health endpoint retorna estado del sistema           |
| 2        | Prueba Unitaria 02: Ping endpoint responde con 'pong'                    |
| 3        | Prueba Unitaria 03: Info endpoint retorna información de la aplicación |

---

**Detalle de Prueba 01: Health Endpoint Retorna Estado del Sistema**

| **Aspecto**                 | **Detalle**                                                                                                                                                                       |
| --------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-017: El endpoint de health debe retornar información completa del estado del sistema incluyendo dependencias.                                                                       |
| **Objetivo**                | Verificar que el endpoint de salud retorne el estado completo del sistema en formato JSON.                                                                                              |
| **Tipo de Prueba**          | Prueba End-to-End - API                                                                                                                                                                 |
| **Datos de Entrada**        | • Request: GET /api/v1/setup/health`<br>`• Sin autenticación requerida                                                                                                             |
| **Procedimiento**           | 1. Iniciar aplicación de prueba (WebApplicationFactory)`<br>`2. Ejecutar petición GET al endpoint`<br>`3. Deserializar respuesta JSON`<br>`4. Verificar estructura y valores    |
| **Resultados Esperados**    | • StatusCode = 200`<br>`• JSON válido retornado`<br>`• Campo "status" = "Healthy"`<br>`• Incluye información de base de datos`<br>`• Timestamp de verificación presente |

---

**Detalle de Prueba 02: Ping Endpoint Responde 'pong'**

| **Aspecto**                 | **Detalle**                                                                                                   |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-018: El sistema debe responder instantáneamente a solicitudes de ping para monitoreo.                           |
| **Objetivo**                | Validar respuesta rápida del endpoint de ping para verificación de disponibilidad.                                |
| **Tipo de Prueba**          | Prueba End-to-End - Disponibilidad                                                                                  |
| **Datos de Entrada**        | • Request: GET /api/v1/setup/ping                                                                                  |
| **Procedimiento**           | 1. Ejecutar petición GET`<br>`2. Medir tiempo de respuesta`<br>`3. Verificar contenido exacto                  |
| **Resultados Esperados**    | • StatusCode = 200`<br>`• Contenido exacto = "pong"`<br>`• Tiempo < 50ms`<br>`• Content-Type = text/plain |

---

**Detalle de Prueba 03: Info Endpoint Retorna Información de Aplicación**

| **Aspecto**                 | **Detalle**                                                                                                                                                                              |
| --------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Requerimiento Funcional** | RF-019: El sistema debe exponer información de versión y configuración para operaciones DevOps.                                                                                             |
| **Objetivo**                | Verificar que el endpoint de información retorne datos correctos de la aplicación.                                                                                                           |
| **Tipo de Prueba**          | Prueba End-to-End - Metadata                                                                                                                                                                   |
| **Datos de Entrada**        | • Request: GET /api/v1/setup/info                                                                                                                                                             |
| **Procedimiento**           | 1. Ejecutar GET al endpoint info`<br>`2. Parsear respuesta JSON`<br>`3. Validar campos esperados                                                                                           |
| **Resultados Esperados**    | • StatusCode = 200`<br>`• Campo "applicationName" = "ElectroHuila API"`<br>`• Campo "version" presente y válido`<br>`• Campo "environment" presente`<br>`• Formato JSON correcto |

---

## Resumen Estadístico de Pruebas

| Categoría           | Archivos     | Métodos de Prueba | Cobertura Estimada |
| -------------------- | ------------ | ------------------ | ------------------ |
| Application Layer    | 3            | 25+                | 85%                |
| Domain Layer         | 2            | 4                  | 90%                |
| Infrastructure Layer | 7            | 35+                | 80%                |
| Integration Tests    | 1            | 2                  | 95%                |
| E2E Tests            | 1            | 3                  | 90%                |
| **TOTAL**      | **14** | **69+**      | **83%**      |

---

## Tecnologías y Frameworks Utilizados

- **xUnit 2.9.0**: Framework principal de testing
- **Moq 4.20.70**: Framework de mocking
- **FluentAssertions 6.12.0**: Assertions mejoradas
- **Entity Framework In-Memory**: Base de datos en memoria para pruebas
- **Microsoft.AspNetCore.Mvc.Testing 9.0.0**: Testing de APIs
- **Coverlet.Collector 6.0.2**: Colección de cobertura de código

---

## Patrones de Prueba Implementados

1. **Arrange-Act-Assert (AAA)**: Estructura estándar en todas las pruebas
2. **Repository Pattern Mocking**: Simulación de acceso a datos
3. **Builder Pattern**: Para construcción de objetos de prueba
4. **Test Fixtures**: Reutilización de configuración de pruebas
5. **In-Memory Database**: Para pruebas de integración sin dependencias externas

---

**Nota**: Este documento refleja el estado actual de las pruebas unitarias del proyecto ElectroHuila API v1.0 al 28 de Noviembre de 2025.
