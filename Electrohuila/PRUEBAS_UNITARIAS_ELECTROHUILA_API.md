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

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.002 segundos
Validar que se pueda cancelar exitosamente una cita con estado "Pendiente".
Verificar que el estado cambie a "Cancelado" y se establezca la razón de cancelación.
Tipo: Prueba Unitaria

Datos de prueba:
appointmentData = {
    'appointmentId': 1,
    'cancellationReason': 'El cliente solicitó cambio de fecha',
    'statusId': 1, // PENDING_STATUS_ID
    'appointmentNumber': 'APT-001',
    'clientId': 1,
    'branchId': 1,
    'appointmentDate': DateTime.UtcNow.AddDays(1),
    'appointmentTime': '10:00 AM'
}

Método ejecutado:
await _handler.Handle(command, CancellationToken.None);
```

---

#### 1.2 CreateAppointmentCommandHandlerTests.cs

**Realizado por**: Team Backend

| Versión | Título                                                        |
| -------- | -------------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Crear cita exitosamente con datos válidos |
| 2        | Prueba Unitaria 02: Manejo de excepciones en base de datos     |

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.0015 segundos
Validar que se pueda crear una cita con información válida del cliente.
Verificar que la cita se guarde correctamente con número de cita asignado.
Tipo: Prueba Unitaria

Datos de prueba:
appointmentDto = {
    'clientId': 1,
    'branchId': 1,
    'appointmentTypeId': 1,
    'appointmentDate': DateTime.UtcNow.AddDays(2),
    'appointmentTime': '14:30 PM',
    'observations': 'Consulta de seguimiento'
}

Método ejecutado:
var result = await _handler.Handle(command, CancellationToken.None);
```

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

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.001 segundos
Validar que se maneje correctamente un token de refresh vacío.
Verificar que retorne error de validación apropiado.
Tipo: Prueba Unitaria

Datos de prueba:
tokenData = {
    'refreshToken': '',
    'userId': 1
}

Método ejecutado:
var result = await _handler.Handle(new RefreshTokenCommand(string.Empty), CancellationToken.None);
```

---

### 2. Pruebas de Domain Layer

#### 2.1 AppointmentTests.cs

**Realizado por**: Team Domain

| Versión | Título                                           |
| -------- | ------------------------------------------------- |
| 1        | Prueba Unitaria 01: Crear cita con datos válidos |
| 2        | Prueba Unitaria 02: Actualizar estado de cita     |
| 3        | Prueba Unitaria 03: Propiedades de auditoría     |

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.0005 segundos
Validar que una entidad Appointment se cree correctamente con todas sus propiedades.
Verificar integridad referencial y valores por defecto.
Tipo: Prueba Unitaria

Datos de prueba:
appointmentEntity = {
    'id': 1,
    'clientId': 1,
    'branchId': 1,
    'appointmentTypeId': 1,
    'appointmentDate': DateTime.Today.AddDays(1),
    'appointmentTime': '10:00 AM',
    'statusId': 2, // CONFIRMED_STATUS_ID
    'appointmentNumber': 'APT-001'
}

Método ejecutado:
var appointment = new Appointment();
// Configuración de propiedades y validaciones
```

---

#### 2.2 EmailTests.cs

**Realizado por**: Team Domain

| Versión | Título                                                     |
| -------- | ----------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Comparación de emails case-insensitive |

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.0003 segundos
Validar que el Value Object Email maneje correctamente mayúsculas y minúsculas.
Verificar que dos emails con diferente case sean considerados iguales.
Tipo: Prueba Unitaria

Datos de prueba:
emailData = {
    'email1': 'Test@Example.com',
    'email2': 'test@example.com',
    'email3': 'TEST@EXAMPLE.COM'
}

Método ejecutado:
var email1 = new Email("Test@Example.com");
var email2 = new Email("test@example.com");
// Comparaciones de igualdad
```

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

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.035 segundos
Validar que el repositorio retorne correctamente una cita por su ID.
Verificar que incluya las entidades relacionadas (Branch, Client, AppointmentType).
Tipo: Prueba de Integración con EF In-Memory

Datos de prueba:
appointmentData = {
    'id': 1,
    'appointmentNumber': 'APT-001',
    'clientId': 1,
    'branchId': 1,
    'appointmentTypeId': 1,
    'appointmentDate': DateTime.Today,
    'appointmentTime': '09:00 AM',
    'statusId': 1,
    'isActive': true
}

Método ejecutado:
var result = await _repository.GetByIdAsync(1);
```

---

#### 3.2 BranchRepositoryTests.cs

**Realizado por**: Team Infrastructure

| Versión | Título                                                    |
| -------- | ---------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Obtener sucursal activa por ID         |
| 2        | Prueba Unitaria 02: Filtrar sucursales inactivas           |
| 3        | Prueba Unitaria 03: Obtener todas las sucursales ordenadas |

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.025 segundos
Validar que el repositorio retorne solo sucursales activas ordenadas alfabéticamente.
Verificar que se excluyan sucursales con IsActive = false.
Tipo: Prueba de Integración con EF In-Memory

Datos de prueba:
branchesData = [
    { 'name': 'Sucursal Zeta', 'code': 'SZ01', 'isActive': true },
    { 'name': 'Sucursal Alpha', 'code': 'SA01', 'isActive': true },
    { 'name': 'Sucursal Beta', 'code': 'SB01', 'isActive': true },
    { 'name': 'Sucursal Inactive', 'code': 'SI01', 'isActive': false }
]

Método ejecutado:
var result = await _repository.GetAllActiveAsync();
```

---

#### 3.3 JwtTokenGeneratorTests.cs

**Realizado por**: Team Security

| Versión | Título                                                 |
| -------- | ------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Generar token JWT válido           |
| 2        | Prueba Unitaria 02: Incluir claims del usuario en token |

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.008 segundos
Validar que el generador de JWT cree un token con formato válido (3 partes separadas por puntos).
Verificar que el token no sea null ni vacío.
Tipo: Prueba Unitaria

Datos de prueba:
userData = {
    'id': 1,
    'username': 'testuser',
    'email': 'test@electrohuila.com',
    'fullName': 'Usuario de Prueba',
    'isActive': true
}

Método ejecutado:
var token = _jwtGenerator.GenerateToken(user);
var parts = token.Split('.');
```

---

### 4. Pruebas de Integration Tests

#### 4.1 HealthCheckTests.cs

**Realizado por**: Team DevOps

| Versión | Título                                                  |
| -------- | -------------------------------------------------------- |
| 1        | Prueba Unitaria 01: Health check endpoint responde OK    |
| 2        | Prueba Unitaria 02: Ping endpoint responde correctamente |

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.150 segundos
Validar que el endpoint de health check del sistema responda correctamente.
Verificar que el estado del sistema sea "Healthy".
Tipo: Prueba de Integración

Datos de prueba:
requestData = {
    'endpoint': '/api/v1/setup/health',
    'method': 'GET',
    'headers': { 'Content-Type': 'application/json' }
}

Método ejecutado:
var response = await _client.GetAsync("/api/v1/setup/health");
```

---

### 5. Pruebas E2E

#### 5.1 SetupControllerTests.cs

**Realizado por**: Team QA

| Versión | Título                                                                  |
| -------- | ------------------------------------------------------------------------ |
| 1        | Prueba Unitaria 01: Health endpoint retorna estado del sistema           |
| 2        | Prueba Unitaria 02: Ping endpoint responde con 'pong'                    |
| 3        | Prueba Unitaria 03: Info endpoint retorna información de la aplicación |

**Detalle de Prueba - Ejemplo:**

```
Duración de la prueba: 0.200 segundos
Validar que el endpoint de información retorne correctamente los datos de la aplicación.
Verificar que incluya nombre de la aplicación y versión.
Tipo: Prueba End-to-End

Datos de prueba:
requestData = {
    'endpoint': '/api/v1/setup/info',
    'method': 'GET',
    'expectedContent': ['ElectroHuila', 'version']
}

Método ejecutado:
var response = await _client.GetAsync("/api/v1/setup/info");
var content = await response.Content.ReadAsStringAsync();
```

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
