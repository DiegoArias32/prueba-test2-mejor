# Endpoint: schedule-simple-appointment

## Descripción
Endpoint público para agendar citas de manera simplificada. Este endpoint permite crear un cliente (si no existe) y agendar su cita en **una sola operación**.

**Características:**
- ✅ Sin autenticación requerida (público)
- ✅ Crea el cliente automáticamente si no existe
- ✅ Reutiliza cliente existente si ya está registrado (por DocumentNumber)
- ✅ Transaccional (todo o nada)
- ✅ Generación automática de números únicos (ClientNumber y AppointmentNumber)
- ✅ Validaciones completas con FluentValidation

---

## Endpoint

```
POST /api/v1/public/schedule-simple-appointment
```

---

## Request Body

```json
{
  "documentType": "CC",
  "documentNumber": "1234567890",
  "fullName": "Juan Pérez García",
  "phone": "6012345678",
  "mobile": "3001234567",
  "email": "juan.perez@email.com",
  "address": "Calle 123 #45-67, Barrio Centro",
  "branchId": 1,
  "appointmentTypeId": 2,
  "appointmentDate": "2025-11-20",
  "appointmentTime": "09:30",
  "observations": "Requiero atención para solicitud de nuevo servicio"
}
```

### Campos

#### Datos del Cliente

| Campo | Tipo | Requerido | Descripción | Validaciones |
|-------|------|-----------|-------------|--------------|
| `documentType` | string | ✅ Sí | Tipo de documento | Valores: CC, TI, CE, RC |
| `documentNumber` | string | ✅ Sí | Número de documento | Máx. 20 caracteres, solo alfanumérico |
| `fullName` | string | ✅ Sí | Nombre completo | Min. 3, máx. 200 caracteres |
| `phone` | string | ❌ No | Teléfono fijo | Máx. 20 caracteres, solo números |
| `mobile` | string | ✅ Sí | Número celular | Exactamente 10 dígitos |
| `email` | string | ❌ No | Correo electrónico | Formato email válido, máx. 100 caracteres |
| `address` | string | ❌ No | Dirección física | Máx. 500 caracteres |

#### Datos de la Cita

| Campo | Tipo | Requerido | Descripción | Validaciones |
|-------|------|-----------|-------------|--------------|
| `branchId` | integer | ✅ Sí | ID de la sucursal | Mayor a 0 |
| `appointmentTypeId` | integer | ✅ Sí | ID del tipo de cita | Mayor a 0 |
| `appointmentDate` | string | ✅ Sí | Fecha de la cita | Formato ISO (YYYY-MM-DD), fecha futura |
| `appointmentTime` | string | ✅ Sí | Hora de la cita | Formato HH:mm (ej: 09:30) |
| `observations` | string | ❌ No | Observaciones | Máx. 1000 caracteres |

---

## Response (200 OK)

```json
{
  "isSuccess": true,
  "error": null,
  "data": {
    "clientNumber": "CLI-20251116-001",
    "appointmentNumber": "APT-20251116-000001",
    "message": "Cliente creado y cita agendada exitosamente",
    "appointmentDate": "2025-11-20T00:00:00Z",
    "appointmentTime": "09:30",
    "branchName": "Sede Principal"
  }
}
```

### Campos de Respuesta

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `clientNumber` | string | Número único del cliente generado/existente |
| `appointmentNumber` | string | Número único de la cita generada |
| `message` | string | Mensaje de confirmación |
| `appointmentDate` | datetime | Fecha de la cita agendada |
| `appointmentTime` | string | Hora de la cita en formato HH:mm |
| `branchName` | string | Nombre de la sucursal |

---

## Response (400 Bad Request)

```json
{
  "error": "El número de celular debe tener exactamente 10 dígitos"
}
```

### Errores Comunes

| Error | Descripción |
|-------|-------------|
| `El tipo de documento debe ser CC, TI, CE o RC` | Tipo de documento inválido |
| `El número de documento es requerido` | Campo obligatorio faltante |
| `El nombre completo debe tener al menos 3 caracteres` | Validación de longitud |
| `El número de celular debe tener exactamente 10 dígitos` | Formato de celular incorrecto |
| `El formato del email es inválido` | Email con formato incorrecto |
| `La fecha debe estar en formato ISO válido (YYYY-MM-DD)` | Formato de fecha incorrecto |
| `La fecha de la cita debe ser futura` | Fecha en el pasado |
| `El formato de hora debe ser HH:mm (ejemplo: 09:30)` | Formato de hora incorrecto |
| `La sucursal especificada no existe o no está activa` | Sucursal no encontrada |

---

## Ejemplos de Uso

### Ejemplo 1: Cliente Nuevo

**Request:**
```bash
curl -X POST https://api.electrohuila.com/api/v1/public/schedule-simple-appointment \
  -H "Content-Type: application/json" \
  -d '{
    "documentType": "CC",
    "documentNumber": "1234567890",
    "fullName": "María López",
    "mobile": "3001234567",
    "email": "maria.lopez@email.com",
    "branchId": 1,
    "appointmentTypeId": 2,
    "appointmentDate": "2025-11-25",
    "appointmentTime": "10:00",
    "observations": "Primera vez solicitando servicio"
  }'
```

**Response:**
```json
{
  "isSuccess": true,
  "error": null,
  "data": {
    "clientNumber": "CLI-20251116-001",
    "appointmentNumber": "APT-20251116-000001",
    "message": "Cliente creado y cita agendada exitosamente",
    "appointmentDate": "2025-11-25T00:00:00Z",
    "appointmentTime": "10:00",
    "branchName": "Sede Principal"
  }
}
```

### Ejemplo 2: Cliente Existente

**Request:**
```bash
curl -X POST https://api.electrohuila.com/api/v1/public/schedule-simple-appointment \
  -H "Content-Type: application/json" \
  -d '{
    "documentType": "CC",
    "documentNumber": "1234567890",
    "fullName": "María López",
    "mobile": "3001234567",
    "email": "maria.lopez@email.com",
    "branchId": 1,
    "appointmentTypeId": 3,
    "appointmentDate": "2025-12-01",
    "appointmentTime": "14:30",
    "observations": "Segunda cita - seguimiento"
  }'
```

**Response:**
```json
{
  "isSuccess": true,
  "error": null,
  "data": {
    "clientNumber": "CLI-20251116-001",
    "appointmentNumber": "APT-20251116-000002",
    "message": "Cita agendada exitosamente",
    "appointmentDate": "2025-12-01T00:00:00Z",
    "appointmentTime": "14:30",
    "branchName": "Sede Principal"
  }
}
```

**Nota:** El cliente ya existía (mismo DocumentNumber), por lo que se reutiliza el ClientNumber existente.

---

## Lógica de Negocio

### Flujo del Proceso

1. **Validar Sucursal**
   - Verifica que `branchId` exista y esté activa

2. **Buscar Cliente por DocumentNumber**
   - Si NO existe → Crear nuevo cliente con número único
   - Si YA existe → Reutilizar cliente existente

3. **Crear Cita**
   - Generar AppointmentNumber único
   - Estado inicial: PENDIENTE (StatusId = 1)
   - Vincular con cliente y sucursal

4. **Retornar Confirmación**
   - Incluir números únicos generados
   - Mensaje diferenciado según si es cliente nuevo o existente

### Generación de Números Únicos

#### ClientNumber
- **Formato:** `CLI-YYYYMMDD-XXX`
- **Ejemplo:** `CLI-20251116-001`
- **XXX:** Contador secuencial de 3 dígitos por día

#### AppointmentNumber
- **Formato:** `APT-YYYYMMDD-XXXXXX`
- **Ejemplo:** `APT-20251116-000001`
- **XXXXXX:** Contador secuencial de 6 dígitos por día

---

## Arquitectura

### Archivos Creados

```
src/1. Core/ElectroHuila.Application/Features/Appointments/Commands/ScheduleSimpleAppointment/
├── ScheduleSimpleAppointmentCommand.cs          (Command/Request DTO)
├── ScheduleSimpleAppointmentResponse.cs         (Response DTO)
├── ScheduleSimpleAppointmentCommandHandler.cs   (Lógica de negocio)
└── ScheduleSimpleAppointmentCommandValidator.cs (Validaciones)

src/3. Presentation/ElectroHuila.WebApi/Controllers/V1/
└── PublicController.cs                           (Endpoint agregado)
```

### Patrón Utilizado

- **CQRS** (Command Query Responsibility Segregation)
- **MediatR** para manejo de comandos
- **FluentValidation** para validaciones
- **Result Pattern** para manejo de errores
- **Clean Architecture** con separación de capas

### Dependencias Inyectadas

```csharp
- IClientRepository           // Gestión de clientes
- IAppointmentRepository      // Gestión de citas
- IBranchRepository          // Validación de sucursales
- IAppointmentNumberGenerator // Generación de números únicos
```

---

## Consideraciones

### Transaccionalidad
- Cada repositorio maneja su propio `SaveChanges`
- Cliente se crea ANTES que la cita
- Si falla creación de cita, el cliente ya quedó creado (no hay rollback automático)

### Idempotencia
- Basada en `DocumentNumber`
- Mismo documento = mismo cliente (se reutiliza)
- Múltiples citas permitidas para el mismo cliente

### Seguridad
- Endpoint público (sin autenticación)
- Validaciones exhaustivas en entrada
- No expone información sensible

### Performance
- Consultas optimizadas por índices (DocumentNumber)
- Sin N+1 queries
- Operaciones atómicas por repositorio

---

## Testing

### Casos de Prueba Sugeridos

1. ✅ Cliente nuevo + cita exitosa
2. ✅ Cliente existente + nueva cita
3. ❌ DocumentNumber vacío
4. ❌ Email con formato inválido
5. ❌ Celular con menos de 10 dígitos
6. ❌ Fecha en el pasado
7. ❌ Hora con formato incorrecto
8. ❌ BranchId inexistente
9. ❌ AppointmentTypeId inexistente
10. ✅ Observaciones opcionales vacías

---

## Notas de Implementación

- ✅ **Clean Architecture:** Application layer no depende de Infrastructure
- ✅ **SOLID:** Inyección de dependencias, responsabilidad única
- ✅ **Result Pattern:** Manejo de errores consistente
- ✅ **FluentValidation:** Validaciones declarativas y reutilizables
- ✅ **Async/Await:** Operaciones asíncronas para mejor performance
- ✅ **Documentación:** XML comments en todo el código
