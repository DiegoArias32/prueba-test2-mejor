# Sistema de Notificaciones - Cambios Implementados

## Fecha: 2025-11-23

## Resumen
Se modificó el sistema de notificaciones para soportar notificaciones tanto para **USERS (admins)** como para **CLIENTS**. Anteriormente, solo se podían crear notificaciones para usuarios con UserId.

---

## Cambios Implementados

### 1. Base de Datos - Migración SQL
**Archivo**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Persistence/Migrations/AddClientIdToNotifications.sql`

**Cambios**:
- `USER_ID` ahora es NULLABLE
- Nueva columna `CLIENT_ID NUMBER(10) NULL`
- Foreign Key a tabla `CLIENTS`
- CHECK constraint: `(USER_ID IS NOT NULL AND CLIENT_ID IS NULL) OR (USER_ID IS NULL AND CLIENT_ID IS NOT NULL)`
- Nuevos índices:
  - `IX_NOTIFICATIONS_CLIENT_ID`
  - `IX_NOTIFICATIONS_CLIENT_CREATED`

**Ejecutar**:
```sql
-- Conectarse a Oracle y ejecutar el script
sqlplus ADMIN/password@ORACLE_DB
@AddClientIdToNotifications.sql
```

---

### 2. Entidad Domain - Notification.cs
**Archivo**: `src/1. Core/ElectroHuila.Domain/Entities/Notifications/Notification.cs`

**Cambios**:
- `UserId` ahora es `int?` (nullable)
- Nueva propiedad `ClientId` (int?, nullable)
- Nueva propiedad de navegación `Client`
- Método `Create()` actualizado:
  - Acepta `userId` O `clientId` (no ambos, no ninguno)
  - Validaciones actualizadas

**Uso**:
```csharp
// Para clientes
var notification = Notification.Create(
    type: "EMAIL",
    title: "Cita Confirmada",
    message: "Tu cita ha sido confirmada",
    clientId: 123,
    appointmentId: 456
);

// Para usuarios (admins)
var notification = Notification.Create(
    type: "IN_APP",
    title: "Nueva Cita Asignada",
    message: "Se te ha asignado una nueva cita",
    userId: 789,
    appointmentId: 456
);
```

---

### 3. Entity Framework Configuration - NotificationConfiguration.cs
**Archivo**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Persistence/Configurations/NotificationConfiguration.cs`

**Cambios**:
- `USER_ID` configurado como `.IsRequired(false)`
- Nueva configuración para `CLIENT_ID`
- Nueva relación de navegación con `Client`
- CHECK constraint en EF Core
- Nuevos índices

---

### 4. NotificationService.cs
**Archivo**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Services/NotificationService.cs`

**Cambios en los 3 métodos**:
- `SendAppointmentConfirmationAsync`
- `SendAppointmentReminderAsync`
- `SendAppointmentCancellationAsync`

**Antes** (incorrecto):
```csharp
var emailNotification = Notification.Create(
    userId: client.Id,  // ❌ INCORRECTO
    type: "EMAIL",
    ...
);
```

**Ahora** (correcto):
```csharp
var emailNotification = Notification.Create(
    type: "EMAIL",
    title: "Cita Confirmada",
    message: "...",
    clientId: client.Id,  // ✅ CORRECTO
    appointmentId: appointmentId
);

await _notificationRepository.CreateAsync(emailNotification, cancellationToken);
_logger.LogInformation("EMAIL notification created for client {ClientId}", client.Id);

try
{
    var emailSent = await _gmailService.SendAppointmentConfirmationAsync(...);

    if (emailSent)
        emailNotification.MarkAsSent();
    else
        emailNotification.MarkAsFailed("No se pudo enviar el email");
}
catch (Exception ex)
{
    emailNotification.MarkAsFailed(ex.Message);
}
finally
{
    await _notificationRepository.UpdateAsync(emailNotification, cancellationToken);
}
```

**Logging mejorado**:
- Ahora incluye `ClientId` en los logs
- Logs más detallados sobre el estado de envío

---

### 5. Repository - INotificationRepository.cs & NotificationRepository.cs
**Archivos**:
- `src/1. Core/ElectroHuila.Application/Contracts/Repositories/INotificationRepository.cs`
- `src/2. Infrastructure/ElectroHuila.Infrastructure/Persistence/Repositories/NotificationRepository.cs`

**Nuevo método**:
```csharp
Task<(IEnumerable<Notification> Notifications, int TotalCount)> GetNotificationsForUserByAssignedTypesAsync(
    int userId,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default);
```

**Funcionalidad**:
- Obtiene notificaciones IN_APP del usuario (UserId)
- Obtiene notificaciones EMAIL/WHATSAPP de clientes con citas en los tipos asignados al usuario
- Usa `UserAppointmentTypeAssignments` para filtrar
- Retorna datos paginados con total count

**Query SQL generado**:
```sql
SELECT n.*, u.*, c.*, a.*
FROM NOTIFICATIONS n
LEFT JOIN USERS u ON n.USER_ID = u.ID
LEFT JOIN CLIENTS c ON n.CLIENT_ID = c.ID
LEFT JOIN APPOINTMENTS a ON n.APPOINTMENT_ID = a.ID
WHERE n.IS_ACTIVE = 1
  AND (
    n.USER_ID = @userId  -- Notificaciones directas
    OR (
      n.CLIENT_ID IS NOT NULL
      AND n.APPOINTMENT_ID IS NOT NULL
      AND a.APPOINTMENT_TYPE_ID IN (
        SELECT APPOINTMENT_TYPE_ID
        FROM USER_APPOINTMENT_TYPE_ASSIGNMENTS
        WHERE USER_ID = @userId AND IS_ACTIVE = 1
      )
    )
  )
ORDER BY n.CREATED_AT DESC
OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY
```

---

### 6. DTOs Actualizados

#### NotificationDto.cs
```csharp
public int? UserId { get; set; }
public string? UserName { get; set; }
public int? ClientId { get; set; }
public string? ClientName { get; set; }
```

#### NotificationListDto.cs
```csharp
public int? UserId { get; set; }
public string? UserName { get; set; }
public int? ClientId { get; set; }
public string? ClientName { get; set; }
```

---

### 7. AutoMapper - MappingProfile.cs
**Archivo**: `src/1. Core/ElectroHuila.Application/Common/Mappings/MappingProfile.cs`

**Mappings actualizados**:
```csharp
CreateMap<Notification, NotificationDto>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.FullName : null))
    .ForMember(dest => dest.AppointmentNumber, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentNumber : null));

CreateMap<Notification, NotificationListDto>()
    .ForMember(dest => dest.MessagePreview, opt => opt.MapFrom(src =>
        src.Message.Length > 100 ? src.Message.Substring(0, 100) + "..." : src.Message))
    .ForMember(dest => dest.AppointmentNumber, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentNumber : null))
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.FullName : null));
```

---

### 8. CQRS - GetMyNotificationsQuery

**Archivos creados**:
- `src/1. Core/ElectroHuila.Application/Features/Notifications/Queries/GetMyNotifications/GetMyNotificationsQuery.cs`
- `src/1. Core/ElectroHuila.Application/Features/Notifications/Queries/GetMyNotifications/GetMyNotificationsQueryHandler.cs`

**Query**:
```csharp
public record GetMyNotificationsQuery(
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedList<NotificationListDto>>>;
```

**Handler**:
- Obtiene el UserId del `ICurrentUserService`
- Llama al repositorio `GetNotificationsForUserByAssignedTypesAsync`
- Retorna `PagedList<NotificationListDto>` con metadatos de paginación

---

### 9. API Endpoint - NotificationsController.cs

**Archivo**: `src/3. Presentation/ElectroHuila.WebApi/Controllers/V1/NotificationsController.cs`

**Nuevo endpoint**:
```http
GET /api/v1/notifications/my-notifications?pageNumber=1&pageSize=20
```

**Detalles**:
- Requiere autenticación JWT
- Obtiene el userId del token automáticamente
- Retorna notificaciones IN_APP + EMAIL/WHATSAPP de clientes con citas asignadas
- Paginación incluida

**Response**:
```json
{
  "items": [
    {
      "id": 1,
      "type": "EMAIL",
      "title": "Cita Confirmada",
      "messagePreview": "Tu cita para el 23/11/2025 a las 10:00 ha sido confirmada",
      "status": "SENT",
      "isRead": false,
      "sentAt": "2025-11-23T08:00:00Z",
      "createdAt": "2025-11-23T08:00:00Z",
      "appointmentNumber": "CITA-001",
      "clientName": "Juan Perez",
      "clientId": 123,
      "userId": null,
      "userName": null
    },
    {
      "id": 2,
      "type": "IN_APP",
      "title": "Nueva Cita Asignada",
      "messagePreview": "Se te ha asignado una nueva cita para gestionar",
      "status": "SENT",
      "isRead": false,
      "sentAt": "2025-11-23T09:00:00Z",
      "createdAt": "2025-11-23T09:00:00Z",
      "appointmentNumber": "CITA-002",
      "clientName": null,
      "clientId": null,
      "userId": 789,
      "userName": "admin_user"
    }
  ],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

## IMPORTANTE: Investigación Email/WhatsApp

### Estado Actual
El código **SÍ** guarda las notificaciones EMAIL/WHATSAPP en BD, pero necesitas verificar:

### 1. Verificar IGmailApiService
**Archivo**: Buscar la implementación de `IGmailApiService`

**Preguntas**:
- ¿El método `SendAppointmentConfirmationAsync` realmente envía el email?
- ¿Hay configuración SMTP en `appsettings.json`?
- ¿Las credenciales de Gmail están configuradas?

**Buscar**:
```bash
grep -r "IGmailApiService" --include="*.cs"
grep -r "GmailApiService" --include="*.cs"
```

### 2. Verificar IWhatsAppApiService
**Archivo**: Buscar la implementación de `IWhatsAppApiService`

**Preguntas**:
- ¿El método `SendAppointmentConfirmationAsync` hace la llamada HTTP a la API de WhatsApp?
- ¿La URL de la API está configurada en `appsettings.json`?
- ¿El token de autenticación está configurado?

**Buscar**:
```bash
grep -r "IWhatsAppApiService" --include="*.cs"
grep -r "WhatsAppApiService" --include="*.cs"
```

### 3. Revisar appsettings.json
```json
{
  "ExternalApis": {
    "WhatsApp": {
      "Enabled": true,  // ¿Está en true?
      "BaseUrl": "https://api.whatsapp.com/...",
      "ApiKey": "..."
    },
    "Gmail": {
      "Enabled": true,
      "SmtpServer": "smtp.gmail.com",
      "SmtpPort": 587,
      "Username": "...",
      "Password": "...",
      "FromEmail": "...",
      "FromName": "ElectroHuila"
    }
  }
}
```

### 4. Revisar Logs
Buscar en los logs:
```
"Email confirmation sent successfully"
"WhatsApp confirmation sent successfully"
"Failed to send email"
"Failed to send WhatsApp"
"Error sending email"
"Error sending WhatsApp"
```

---

## Testing

### 1. Verificar Migration
```sql
-- Conectarse a Oracle
sqlplus ADMIN/password@ORACLE_DB

-- Verificar estructura
DESC NOTIFICATIONS;

-- Verificar constraint
SELECT constraint_name, constraint_type, search_condition
FROM user_constraints
WHERE table_name = 'NOTIFICATIONS'
AND constraint_name = 'CHK_NOTIFICATION_RECIPIENT';

-- Verificar índices
SELECT index_name, column_name
FROM user_ind_columns
WHERE table_name = 'NOTIFICATIONS'
ORDER BY index_name, column_position;
```

### 2. Test Crear Notificación para Cliente
```csharp
var notification = Notification.Create(
    type: "EMAIL",
    title: "Test Email",
    message: "Mensaje de prueba",
    clientId: 1,  // ID de cliente existente
    appointmentId: 1
);

await _notificationRepository.CreateAsync(notification);
```

### 3. Test Crear Notificación para Usuario
```csharp
var notification = Notification.Create(
    type: "IN_APP",
    title: "Test IN_APP",
    message: "Mensaje de prueba",
    userId: 1,  // ID de usuario existente
    appointmentId: 1
);

await _notificationRepository.CreateAsync(notification);
```

### 4. Test Validaciones
```csharp
// Debe lanzar exception: "Debe proporcionar userId o clientId"
var notification1 = Notification.Create(
    type: "EMAIL",
    title: "Test",
    message: "Test"
);

// Debe lanzar exception: "No puede proporcionar userId y clientId al mismo tiempo"
var notification2 = Notification.Create(
    type: "EMAIL",
    title: "Test",
    message: "Test",
    userId: 1,
    clientId: 1
);
```

### 5. Test Endpoint
```bash
# Obtener token JWT
TOKEN="eyJhbGciOiJIUzI1NiIs..."

# Llamar al endpoint
curl -X GET "http://localhost:5000/api/v1/notifications/my-notifications?pageNumber=1&pageSize=20" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
```

---

## Rollback (Si es necesario)

### 1. Rollback Base de Datos
```sql
-- Eliminar constraint
ALTER TABLE NOTIFICATIONS DROP CONSTRAINT CHK_NOTIFICATION_RECIPIENT;

-- Eliminar índices
DROP INDEX IX_NOTIFICATIONS_CLIENT_ID;
DROP INDEX IX_NOTIFICATIONS_CLIENT_CREATED;

-- Eliminar Foreign Key
ALTER TABLE NOTIFICATIONS DROP CONSTRAINT FK_NOTIFICATIONS_CLIENT;

-- Eliminar columna
ALTER TABLE NOTIFICATIONS DROP COLUMN CLIENT_ID;

-- Hacer USER_ID NOT NULL de nuevo
ALTER TABLE NOTIFICATIONS MODIFY USER_ID NUMBER(10) NOT NULL;

COMMIT;
```

---

## Próximos Pasos

1. **URGENTE**: Investigar por qué no llegan emails/WhatsApp
   - Revisar implementación de `GmailApiService`
   - Revisar implementación de `WhatsAppApiService`
   - Verificar configuración en `appsettings.json`
   - Revisar logs de aplicación

2. **Opcional**: Crear notificaciones IN_APP para admins cuando:
   - Se crea una nueva cita
   - Se cancela una cita
   - Se modifica una cita

3. **Opcional**: Dashboard de notificaciones
   - Métricas de envío (SENT vs FAILED)
   - Tasa de apertura de notificaciones IN_APP
   - Estadísticas por tipo de notificación

4. **Opcional**: Retry automático para notificaciones FAILED
   - Background job que reintenta enviar notificaciones fallidas
   - Límite de reintentos
   - Backoff exponencial

---

## Notas Adicionales

### Diferencias: USERS vs CLIENTS
- **USERS**: Admins/Empleados que gestionan citas, tienen login, reciben notificaciones IN_APP
- **CLIENTS**: Clientes externos que agendan citas, NO tienen login, reciben EMAIL/WHATSAPP

### Tipos de Notificación por Destinatario
- **USERS (UserId)**:
  - `IN_APP` ✅
  - `EMAIL` ✅ (opcional)
  - `SMS` ✅ (opcional)
  - `WHATSAPP` ✅ (opcional)

- **CLIENTS (ClientId)**:
  - `EMAIL` ✅
  - `WHATSAPP` ✅
  - `SMS` ✅ (si se implementa)
  - `IN_APP` ❌ (NO tienen portal web)

### Performance
- Los índices creados (`IX_NOTIFICATIONS_CLIENT_ID`, `IX_NOTIFICATIONS_CLIENT_CREATED`) mejoran el performance de las queries
- El query de `GetNotificationsForUserByAssignedTypesAsync` usa una subconsulta optimizada con `Contains`
- La paginación evita cargar todas las notificaciones en memoria

---

## Archivos Modificados/Creados

### Modificados
1. `Notification.cs` - Entidad domain
2. `NotificationConfiguration.cs` - EF Core configuration
3. `NotificationService.cs` - Service layer
4. `INotificationRepository.cs` - Repository interface
5. `NotificationRepository.cs` - Repository implementation
6. `NotificationDto.cs` - DTO
7. `NotificationListDto.cs` - DTO
8. `MappingProfile.cs` - AutoMapper
9. `NotificationsController.cs` - API Controller

### Creados
1. `AddClientIdToNotifications.sql` - Migration script
2. `GetMyNotificationsQuery.cs` - CQRS Query
3. `GetMyNotificationsQueryHandler.cs` - CQRS Handler

---

## Contacto
Para dudas o problemas con esta implementación, contactar al equipo de desarrollo.
