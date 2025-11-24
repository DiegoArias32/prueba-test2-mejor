# Resumen de Cambios - Sistema de Notificaciones

## Problema Original
- La tabla `NOTIFICATIONS` solo permitía guardar notificaciones para `USERS` (admins)
- Los `CLIENTS` no tienen `UserId`
- No se guardaban notificaciones EMAIL/WHATSAPP en BD (ahora sí se guardan)

## Solución Implementada

### 1. Base de Datos (Oracle)
✅ Ejecutar script SQL: `src/2. Infrastructure/ElectroHuila.Infrastructure/Persistence/Migrations/AddClientIdToNotifications.sql`

```sql
-- USER_ID ahora es nullable
-- Nueva columna CLIENT_ID
-- CHECK constraint: debe tener USER_ID O CLIENT_ID (no ambos, no ninguno)
```

### 2. Código Backend

#### Notification.cs (Domain)
- `UserId` ahora es `int?`
- Nueva propiedad `ClientId` (int?)
- Método `Create()` actualizado para aceptar `userId` O `clientId`

#### NotificationService.cs
**ANTES** (incorrecto):
```csharp
var notification = Notification.Create(
    userId: client.Id,  // ❌ INCORRECTO - client.Id NO es userId
    type: "EMAIL",
    ...
);
```

**AHORA** (correcto):
```csharp
var notification = Notification.Create(
    type: "EMAIL",
    title: "Cita Confirmada",
    message: "...",
    clientId: client.Id,  // ✅ CORRECTO
    appointmentId: appointmentId
);
await _notificationRepository.CreateAsync(notification);
// Luego intenta enviar y actualiza el estado
```

### 3. Nuevo Endpoint API

```http
GET /api/v1/notifications/my-notifications?pageNumber=1&pageSize=20
Authorization: Bearer {JWT_TOKEN}
```

**Retorna**:
- Notificaciones IN_APP del usuario autenticado
- Notificaciones EMAIL/WHATSAPP de clientes con citas en los tipos asignados al usuario
- Datos paginados con metadatos

**Response**:
```json
{
  "items": [...],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Archivos Modificados

### Core (Domain/Application)
1. ✅ `ElectroHuila.Domain/Entities/Notifications/Notification.cs`
2. ✅ `ElectroHuila.Application/Contracts/Repositories/INotificationRepository.cs`
3. ✅ `ElectroHuila.Application/DTOs/Notifications/NotificationDto.cs`
4. ✅ `ElectroHuila.Application/DTOs/Notifications/NotificationListDto.cs`
5. ✅ `ElectroHuila.Application/Common/Mappings/MappingProfile.cs`
6. ✅ `ElectroHuila.Application/Features/Notifications/Queries/GetMyNotifications/GetMyNotificationsQuery.cs` (NUEVO)
7. ✅ `ElectroHuila.Application/Features/Notifications/Queries/GetMyNotifications/GetMyNotificationsQueryHandler.cs` (NUEVO)

### Infrastructure
8. ✅ `ElectroHuila.Infrastructure/Persistence/Configurations/NotificationConfiguration.cs`
9. ✅ `ElectroHuila.Infrastructure/Persistence/Repositories/NotificationRepository.cs`
10. ✅ `ElectroHuila.Infrastructure/Services/NotificationService.cs`
11. ✅ `ElectroHuila.Infrastructure/Persistence/Migrations/AddClientIdToNotifications.sql` (NUEVO)

### Presentation (API)
12. ✅ `ElectroHuila.WebApi/Controllers/V1/NotificationsController.cs`

## Pasos para Implementar

### 1. Base de Datos
```bash
# Conectarse a Oracle
sqlplus ADMIN/password@ORACLE_DB

# Ejecutar migration
@AddClientIdToNotifications.sql

# Verificar
DESC NOTIFICATIONS;
```

### 2. Compilar y Ejecutar
```bash
cd pqr-scheduling-appointments-api
dotnet build
dotnet run --project "src/3. Presentation/ElectroHuila.WebApi"
```

### 3. Probar Endpoint
```bash
# Obtener token
curl -X POST "http://localhost:5000/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'

# Obtener notificaciones
curl -X GET "http://localhost:5000/api/v1/notifications/my-notifications?pageNumber=1&pageSize=20" \
  -H "Authorization: Bearer {TOKEN}"
```

## IMPORTANTE: Investigar Email/WhatsApp

**El código ahora SÍ guarda las notificaciones en BD**, pero debes verificar:

### 1. Revisar IGmailApiService
```bash
# Buscar implementación
find . -name "*GmailApiService*.cs" -type f
```

**Preguntas**:
- ¿Realmente envía emails o solo crea templates?
- ¿Hay configuración SMTP?
- ¿Están las credenciales configuradas?

### 2. Revisar IWhatsAppApiService
```bash
# Buscar implementación
find . -name "*WhatsAppApiService*.cs" -type f
```

**Preguntas**:
- ¿Hace llamada HTTP a API de WhatsApp?
- ¿Está la URL configurada?
- ¿Está el token configurado?

### 3. Revisar appsettings.json
```json
{
  "ExternalApis": {
    "WhatsApp": {
      "Enabled": true,  // ¿Está en true?
      "BaseUrl": "...",
      "ApiKey": "..."
    },
    "Gmail": {
      "SmtpServer": "smtp.gmail.com",
      "SmtpPort": 587,
      "Username": "...",
      "Password": "..."
    }
  }
}
```

### 4. Revisar Logs
Buscar en logs de aplicación:
- `"Email confirmation sent successfully"`
- `"WhatsApp confirmation sent successfully"`
- `"Failed to send email"`
- `"Error sending email"`

## Testing

### Test 1: Crear Cita y Verificar Notificaciones
1. Crear una cita para un cliente
2. Verificar en BD que se crearon registros en `NOTIFICATIONS`:
```sql
SELECT ID, TYPE, CLIENT_ID, USER_ID, STATUS, TITLE, SENT_AT, CREATED_AT
FROM NOTIFICATIONS
WHERE APPOINTMENT_ID = {appointmentId}
ORDER BY CREATED_AT DESC;
```

### Test 2: Endpoint my-notifications
1. Login como usuario admin
2. Llamar `GET /api/v1/notifications/my-notifications`
3. Verificar que retorna notificaciones propias + de clientes asignados

### Test 3: Verificar Estado de Envío
```sql
-- Ver notificaciones fallidas
SELECT ID, TYPE, CLIENT_ID, TITLE, ERROR_MESSAGE, CREATED_AT
FROM NOTIFICATIONS
WHERE STATUS = 'FAILED'
ORDER BY CREATED_AT DESC;

-- Ver notificaciones enviadas
SELECT ID, TYPE, CLIENT_ID, TITLE, SENT_AT
FROM NOTIFICATIONS
WHERE STATUS = 'SENT'
ORDER BY SENT_AT DESC;
```

## Rollback

Si necesitas revertir cambios en BD:
```sql
-- Ver script completo en NOTIFICATION_SYSTEM_CHANGES.md sección "Rollback"
ALTER TABLE NOTIFICATIONS DROP CONSTRAINT CHK_NOTIFICATION_RECIPIENT;
ALTER TABLE NOTIFICATIONS DROP CONSTRAINT FK_NOTIFICATIONS_CLIENT;
DROP INDEX IX_NOTIFICATIONS_CLIENT_ID;
DROP INDEX IX_NOTIFICATIONS_CLIENT_CREATED;
ALTER TABLE NOTIFICATIONS DROP COLUMN CLIENT_ID;
ALTER TABLE NOTIFICATIONS MODIFY USER_ID NUMBER(10) NOT NULL;
COMMIT;
```

## Documentación Completa

Ver `NOTIFICATION_SYSTEM_CHANGES.md` para:
- Explicación detallada de cada cambio
- Ejemplos de código
- Queries SQL completos
- Mejores prácticas
- Troubleshooting

## Contacto
Para dudas o problemas, contactar al equipo de desarrollo.
