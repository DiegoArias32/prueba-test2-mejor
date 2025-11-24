# API de Gestión de Citas (Appointments)

## Tabla de Contenidos

- [Introducción](#introducción)
- [Autenticación](#autenticación)
- [Endpoints](#endpoints)
  - [Consultas (Queries)](#consultas-queries)
  - [Comandos (Commands)](#comandos-commands)
- [Modelos de Datos](#modelos-de-datos)
- [Códigos de Estado](#códigos-de-estado)
- [Ejemplos de Uso](#ejemplos-de-uso)

---

## Introducción

La API de Gestión de Citas proporciona endpoints completos para la creación, consulta, actualización y cancelación de citas en el sistema ElectroHuila.

**Base URL**: `http://localhost:5000/api/v1/appointments`

**Versión**: v1.0.0

---

## Autenticación

Todos los endpoints requieren autenticación mediante **JWT Bearer Token**.

### Cómo autenticarse

1. Obtén un token JWT mediante el endpoint `/api/v1/auth/login`
2. Incluye el token en el header `Authorization` de cada petición:

```http
Authorization: Bearer {tu-token-jwt}
```

### Ejemplo de Login

```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }'
```

**Respuesta:**

```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiration": "2024-11-22T10:00:00Z",
    "user": {
      "id": 1,
      "username": "admin",
      "roles": ["Administrator"]
    }
  }
}
```

---

## Endpoints

### Consultas (Queries)

#### 1. Obtener Cita por ID

Obtiene los detalles completos de una cita específica.

**Endpoint:** `GET /api/v1/appointments/{id}`

**Parámetros:**
- `id` (path, required): ID de la cita

**Ejemplo de Petición:**

```bash
curl -X GET http://localhost:5000/api/v1/appointments/1 \
  -H "Authorization: Bearer {token}"
```

**Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "appointmentNumber": "APT-20241121-a1b2c3d4",
    "appointmentDate": "2024-11-25T00:00:00Z",
    "appointmentTime": "10:00 AM",
    "status": "Pending",
    "statusId": 1,
    "notes": "Primera cita del cliente",
    "cancellationReason": null,
    "completedDate": null,
    "clientId": 1,
    "clientName": "Juan Pérez",
    "clientEmail": "juan.perez@example.com",
    "clientPhone": "3001234567",
    "branchId": 1,
    "branchName": "Sucursal Centro",
    "appointmentTypeId": 1,
    "appointmentTypeName": "Atención de PQR",
    "isActive": true,
    "createdAt": "2024-11-21T08:30:00Z",
    "updatedAt": "2024-11-21T08:30:00Z"
  }
}
```

---

#### 2. Obtener Cita por Número

Busca una cita usando su número único de identificación.

**Endpoint:** `GET /api/v1/appointments/number/{appointmentNumber}`

**Parámetros:**
- `appointmentNumber` (path, required): Número único de la cita

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/number/APT-20241121-a1b2c3d4" \
  -H "Authorization: Bearer {token}"
```

---

#### 3. Obtener Citas por Cliente

Obtiene todas las citas de un cliente específico.

**Endpoint:** `GET /api/v1/appointments/client/{clientNumber}`

**Parámetros:**
- `clientNumber` (path, required): Número del cliente

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/client/CLI-20241001-123456" \
  -H "Authorization: Bearer {token}"
```

**Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "appointmentNumber": "APT-20241121-a1b2c3d4",
      "appointmentDate": "2024-11-25T00:00:00Z",
      "appointmentTime": "10:00 AM",
      "status": "Pending",
      "branchName": "Sucursal Centro",
      "appointmentTypeName": "Atención de PQR"
    },
    {
      "id": 2,
      "appointmentNumber": "APT-20241120-b2c3d4e5",
      "appointmentDate": "2024-11-22T00:00:00Z",
      "appointmentTime": "02:00 PM",
      "status": "Completed",
      "branchName": "Sucursal Norte",
      "appointmentTypeName": "Proyecto Nuevo"
    }
  ]
}
```

---

#### 4. Obtener Citas por Fecha

Lista todas las citas programadas para una fecha específica.

**Endpoint:** `GET /api/v1/appointments/date/{date}`

**Parámetros:**
- `date` (path, required): Fecha en formato ISO 8601 (YYYY-MM-DD)

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/date/2024-11-25" \
  -H "Authorization: Bearer {token}"
```

---

#### 5. Obtener Citas por Sucursal

Lista todas las citas de una sucursal específica.

**Endpoint:** `GET /api/v1/appointments/branch/{branchId}`

**Parámetros:**
- `branchId` (path, required): ID de la sucursal

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/branch/1" \
  -H "Authorization: Bearer {token}"
```

---

#### 6. Obtener Citas por Estado

Filtra citas por su estado actual.

**Endpoint:** `GET /api/v1/appointments/status/{status}`

**Parámetros:**
- `status` (path, required): Estado de la cita (Pending, Confirmed, InProgress, Completed, Cancelled)

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/status/Pending" \
  -H "Authorization: Bearer {token}"
```

---

#### 7. Obtener Citas Pendientes

Obtiene todas las citas que aún no han sido completadas o canceladas.

**Endpoint:** `GET /api/v1/appointments/pending`

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/pending" \
  -H "Authorization: Bearer {token}"
```

---

#### 8. Obtener Citas Completadas

Obtiene todas las citas que han sido completadas.

**Endpoint:** `GET /api/v1/appointments/completed`

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/completed" \
  -H "Authorization: Bearer {token}"
```

---

#### 9. Obtener Mis Citas Asignadas

Obtiene las citas asignadas al usuario actual basándose en sus tipos de cita asignados.

**Endpoint:** `GET /api/v1/appointments/my-assigned`

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/my-assigned" \
  -H "Authorization: Bearer {token}"
```

---

#### 10. Validar Disponibilidad

Valida si existe disponibilidad para una fecha, hora y sucursal específica.

**Endpoint:** `GET /api/v1/appointments/availability`

**Parámetros Query:**
- `date` (query, required): Fecha en formato ISO 8601
- `time` (query, required): Hora en formato TimeSpan (HH:mm:ss)
- `branchId` (query, required): ID de la sucursal

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/availability?date=2024-11-25&time=10:00:00&branchId=1" \
  -H "Authorization: Bearer {token}"
```

**Respuesta Exitosa (200 OK):**

```json
{
  "disponible": true
}
```

---

#### 11. Obtener Horarios Disponibles

Obtiene una lista de horarios disponibles para una fecha y sucursal.

**Endpoint:** `GET /api/v1/appointments/available-times`

**Parámetros Query:**
- `date` (query, required): Fecha en formato ISO 8601
- `branchId` (query, required): ID de la sucursal

**Ejemplo de Petición:**

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/available-times?date=2024-11-25&branchId=1" \
  -H "Authorization: Bearer {token}"
```

**Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "data": [
    "08:00 AM",
    "08:30 AM",
    "09:00 AM",
    "09:30 AM",
    "10:00 AM",
    "02:00 PM",
    "02:30 PM",
    "03:00 PM"
  ]
}
```

---

### Comandos (Commands)

#### 1. Crear Cita

Crea una nueva cita en el sistema.

**Endpoint:** `POST /api/v1/appointments`

**Body (JSON):**

```json
{
  "clientId": 1,
  "branchId": 1,
  "appointmentTypeId": 1,
  "appointmentDate": "2024-11-25",
  "appointmentTime": "10:00 AM",
  "notes": "Primera cita del cliente"
}
```

**Ejemplo de Petición:**

```bash
curl -X POST http://localhost:5000/api/v1/appointments \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": 1,
    "branchId": 1,
    "appointmentTypeId": 1,
    "appointmentDate": "2024-11-25",
    "appointmentTime": "10:00 AM",
    "notes": "Primera cita del cliente"
  }'
```

**Respuesta Exitosa (201 Created):**

```json
{
  "success": true,
  "data": {
    "id": 15,
    "appointmentNumber": "APT-20241121-f5e4d3c2",
    "appointmentDate": "2024-11-25T00:00:00Z",
    "appointmentTime": "10:00 AM",
    "status": "Pending",
    "statusId": 1,
    "notes": "Primera cita del cliente",
    "clientId": 1,
    "branchId": 1,
    "appointmentTypeId": 1,
    "isActive": true,
    "createdAt": "2024-11-21T09:15:00Z",
    "updatedAt": "2024-11-21T09:15:00Z"
  }
}
```

**Errores Posibles:**

- **400 Bad Request**: Validación fallida

```json
{
  "success": false,
  "error": "No se pueden agendar citas en días festivos. Día de la Independencia"
}
```

- **400 Bad Request**: Fecha en el pasado

```json
{
  "success": false,
  "error": "No se pueden agendar citas en fechas pasadas"
}
```

- **400 Bad Request**: Día domingo

```json
{
  "success": false,
  "error": "No se pueden agendar citas los domingos"
}
```

---

#### 2. Agendar Cita (Alternativo)

Agenda una cita con validaciones adicionales.

**Endpoint:** `POST /api/v1/appointments/schedule`

**Body (JSON):** (Mismo formato que crear cita)

```json
{
  "clientId": 1,
  "branchId": 1,
  "appointmentTypeId": 1,
  "appointmentDate": "2024-11-25",
  "appointmentTime": "10:00 AM",
  "notes": "Cita para atención de PQR"
}
```

---

#### 3. Actualizar Cita

Actualiza los datos de una cita existente.

**Endpoint:** `PATCH /api/v1/appointments/{id}`

**Parámetros:**
- `id` (path, required): ID de la cita a actualizar

**Body (JSON):**

```json
{
  "appointmentDate": "2024-11-26",
  "appointmentTime": "02:00 PM",
  "notes": "Cita reprogramada por solicitud del cliente",
  "branchId": 2,
  "appointmentTypeId": 1
}
```

**Ejemplo de Petición:**

```bash
curl -X PATCH http://localhost:5000/api/v1/appointments/15 \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "appointmentDate": "2024-11-26",
    "appointmentTime": "02:00 PM",
    "notes": "Cita reprogramada por solicitud del cliente"
  }'
```

**Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": 15,
    "appointmentNumber": "APT-20241121-f5e4d3c2",
    "appointmentDate": "2024-11-26T00:00:00Z",
    "appointmentTime": "02:00 PM",
    "status": "Pending",
    "notes": "Cita reprogramada por solicitud del cliente",
    "updatedAt": "2024-11-21T09:30:00Z"
  }
}
```

---

#### 4. Cancelar Cita

Cancela una cita existente proporcionando un motivo.

**Endpoint:** `PATCH /api/v1/appointments/cancel/{appointmentId}`

**Parámetros:**
- `appointmentId` (path, required): ID de la cita a cancelar

**Body (JSON):**

```json
{
  "reason": "El cliente solicitó cambio de fecha por emergencia médica"
}
```

**Ejemplo de Petición:**

```bash
curl -X PATCH http://localhost:5000/api/v1/appointments/cancel/15 \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "reason": "El cliente solicitó cambio de fecha por emergencia médica"
  }'
```

**Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "message": "Cita cancelada exitosamente"
}
```

**Errores Posibles:**

- **404 Not Found**: Cita no encontrada

```json
{
  "success": false,
  "error": "Appointment not found"
}
```

- **400 Bad Request**: Cita ya cancelada

```json
{
  "success": false,
  "error": "Appointment is already cancelled"
}
```

- **400 Bad Request**: Cita ya completada

```json
{
  "success": false,
  "error": "Cannot cancel a completed appointment"
}
```

---

#### 5. Completar Cita

Marca una cita como completada agregando notas finales.

**Endpoint:** `PATCH /api/v1/appointments/complete/{appointmentId}`

**Parámetros:**
- `appointmentId` (path, required): ID de la cita a completar

**Body (JSON):**

```json
{
  "notes": "Servicio prestado satisfactoriamente. Cliente recibió respuesta a su PQR."
}
```

**Ejemplo de Petición:**

```bash
curl -X PATCH http://localhost:5000/api/v1/appointments/complete/15 \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "notes": "Servicio prestado satisfactoriamente. Cliente recibió respuesta a su PQR."
  }'
```

**Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "message": "Cita completada exitosamente"
}
```

---

#### 6. Eliminar Lógicamente Cita

Realiza una eliminación lógica de la cita (la marca como inactiva sin eliminarla físicamente).

**Endpoint:** `PATCH /api/v1/appointments/delete-logical/{id}`

**Parámetros:**
- `id` (path, required): ID de la cita a desactivar

**Ejemplo de Petición:**

```bash
curl -X PATCH http://localhost:5000/api/v1/appointments/delete-logical/15 \
  -H "Authorization: Bearer {token}"
```

**Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": 15,
    "isActive": false,
    "updatedAt": "2024-11-21T09:45:00Z"
  }
}
```

---

## Modelos de Datos

### AppointmentDto

```typescript
{
  id: number;                     // ID único de la cita
  appointmentNumber: string;      // Número único generado automáticamente
  appointmentDate: string;        // Fecha de la cita (ISO 8601)
  appointmentTime: string;        // Hora de la cita (formato 12h con AM/PM)
  status: string;                 // Estado actual (Pending, Confirmed, InProgress, Completed, Cancelled)
  statusId: number;               // ID del estado
  notes: string | null;           // Notas adicionales
  cancellationReason: string | null; // Motivo de cancelación (si aplica)
  completedDate: string | null;   // Fecha de completación (si aplica)
  clientId: number;               // ID del cliente
  clientName: string;             // Nombre completo del cliente
  clientEmail: string;            // Email del cliente
  clientPhone: string;            // Teléfono del cliente
  branchId: number;               // ID de la sucursal
  branchName: string;             // Nombre de la sucursal
  appointmentTypeId: number;      // ID del tipo de cita
  appointmentTypeName: string;    // Nombre del tipo de cita
  isActive: boolean;              // Indica si está activa
  createdAt: string;              // Fecha de creación
  updatedAt: string;              // Fecha de última actualización
}
```

### CreateAppointmentDto

```typescript
{
  clientId: number;               // ID del cliente (requerido)
  branchId: number;               // ID de la sucursal (requerido)
  appointmentTypeId: number;      // ID del tipo de cita (requerido)
  appointmentDate: string;        // Fecha de la cita YYYY-MM-DD (requerido)
  appointmentTime: string;        // Hora de la cita HH:mm AM/PM (requerido)
  notes?: string;                 // Notas adicionales (opcional)
}
```

### UpdateAppointmentDto

```typescript
{
  appointmentDate?: string;       // Nueva fecha de la cita
  appointmentTime?: string;       // Nueva hora de la cita
  notes?: string;                 // Notas actualizadas
  branchId?: number;              // Nueva sucursal
  appointmentTypeId?: number;     // Nuevo tipo de cita
}
```

### CancelAppointmentDto

```typescript
{
  reason: string;                 // Motivo de cancelación (requerido)
}
```

### CompleteAppointmentDto

```typescript
{
  notes: string;                  // Notas finales del servicio (requerido)
}
```

---

## Códigos de Estado

| Código | Descripción | Uso |
|--------|-------------|-----|
| 200 OK | Petición exitosa | GET, PATCH exitosos |
| 201 Created | Recurso creado exitosamente | POST exitoso |
| 400 Bad Request | Datos inválidos o reglas de negocio violadas | Validaciones fallidas |
| 401 Unauthorized | Token inválido o expirado | Autenticación fallida |
| 403 Forbidden | Usuario sin permisos | Autorización fallida |
| 404 Not Found | Recurso no encontrado | Cita no existe |
| 500 Internal Server Error | Error del servidor | Errores inesperados |

---

## Ejemplos de Uso

### Flujo Completo: Agendar y Gestionar una Cita

#### 1. Autenticarse

```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}' \
  | jq -r '.data.token')
```

#### 2. Verificar Disponibilidad

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/available-times?date=2024-11-25&branchId=1" \
  -H "Authorization: Bearer $TOKEN"
```

#### 3. Crear la Cita

```bash
APPOINTMENT_ID=$(curl -s -X POST http://localhost:5000/api/v1/appointments \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": 1,
    "branchId": 1,
    "appointmentTypeId": 1,
    "appointmentDate": "2024-11-25",
    "appointmentTime": "10:00 AM",
    "notes": "Primera cita del cliente"
  }' \
  | jq -r '.data.id')
```

#### 4. Consultar la Cita

```bash
curl -X GET "http://localhost:5000/api/v1/appointments/$APPOINTMENT_ID" \
  -H "Authorization: Bearer $TOKEN"
```

#### 5. Completar la Cita

```bash
curl -X PATCH "http://localhost:5000/api/v1/appointments/complete/$APPOINTMENT_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "notes": "Servicio completado satisfactoriamente"
  }'
```

---

## Testing con Postman

### Colección de Postman

Se puede importar esta colección para probar todos los endpoints:

1. Crea una nueva colección en Postman
2. Configura una variable de entorno `baseUrl` = `http://localhost:5000`
3. Configura una variable de entorno `token` con el token JWT obtenido del login
4. Importa los siguientes requests:

**Variables de Entorno:**
```json
{
  "baseUrl": "http://localhost:5000",
  "token": "{{token_from_login}}"
}
```

**Headers Globales:**
```
Authorization: Bearer {{token}}
Content-Type: application/json
```

---

## Swagger UI

La documentación interactiva está disponible en:

**URL**: [http://localhost:5000/swagger](http://localhost:5000/swagger)

Desde Swagger UI puedes:
- Ver todos los endpoints disponibles
- Probar los endpoints directamente desde el navegador
- Ver los modelos de datos con ejemplos
- Autenticarte usando el botón "Authorize"

---

## Soporte

Para reportar problemas o solicitar nuevas características:

- **Email**: soporte@electrohuila.com
- **Issue Tracker**: [GitHub Issues](https://github.com/electrohuila/pqr-api/issues)
- **Documentación**: [Wiki del Proyecto](https://github.com/electrohuila/pqr-api/wiki)

---

## Versionamiento

Esta API usa **versionamiento semántico** (SemVer):

- **MAJOR**: Cambios incompatibles en la API
- **MINOR**: Nuevas funcionalidades compatibles con versiones anteriores
- **PATCH**: Correcciones de bugs compatibles

**Versión Actual**: v1.0.0

---

## Changelog

### v1.0.0 (2024-11-21)

#### ✨ Nuevas Características
- Endpoints completos para gestión de citas
- Autenticación JWT
- Validación de disponibilidad
- Soporte multi-sucursal
- Eliminación lógica de citas

#### 🐛 Correcciones
- Validación de fechas festivas
- Prevención de citas en domingos
- Validación de citas en fechas pasadas

---

**Última Actualización**: 2024-11-21
**Mantenedor**: Equipo de Desarrollo ElectroHuila
