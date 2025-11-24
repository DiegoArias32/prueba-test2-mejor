# 📋 Tareas Pendientes - Sistema de Notificaciones

**Proyecto**: PQR Scheduling Appointments
**Fecha**: 2025-11-21
**Estado General**: 40% completado

---

## ✅ Ya Completado

### ✔️ FASE 1: SignalR Hub en Backend (100%)
- [x] Creado `NotificationHub.cs`
- [x] Creado `ISignalRNotificationService` y su implementación
- [x] Configurado SignalR en `Program.cs`
- [x] Configurado CORS para WebSocket
- [x] Integrado con `NotificationService.cs`
- [x] Agregados paquetes NuGet de SignalR
- [x] Documentación en `SIGNALR_IMPLEMENTATION_GUIDE.md`

### ✔️ FASE 2: Entidad Notification (100%)
- [x] Creada entidad `Notification` en Domain
- [x] Creados DTOs (NotificationDto, CreateNotificationDto, NotificationListDto)
- [x] Implementado CQRS (Commands y Queries)
- [x] Creado `INotificationRepository` y su implementación
- [x] Configuración Entity Framework (`NotificationConfiguration.cs`)
- [x] Controlador API `NotificationsController.cs` con 5 endpoints
- [x] Mappings en AutoMapper
- [x] Registro en DependencyInjection

---

## ⏳ Tareas Pendientes

### 🔴 URGENTE: Prerequisitos

#### 1. Ejecutar Migración de Base de Datos
**Ubicación**: Backend API
**Tiempo estimado**: 15 minutos
**Comandos**:
```bash
cd C:\Users\Aprendiz\Downloads\prueba-test\pqr-scheduling-appointments-api

# Crear migración
dotnet ef migrations add AddNotificationEntity \
  --project "src\2. Infrastructure\ElectroHuila.Infrastructure" \
  --startup-project "src\3. Presentation\ElectroHuila.WebApi" \
  --context ApplicationDbContext

# Aplicar a base de datos
dotnet ef database update \
  --project "src\2. Infrastructure\ElectroHuila.Infrastructure" \
  --startup-project "src\3. Presentation\ElectroHuila.WebApi" \
  --context ApplicationDbContext
```

**Tabla que se creará**: `NOTIFICATIONS`
**Columnas**: ID, USER_ID, APPOINTMENT_ID, TYPE, TITLE, MESSAGE, STATUS, SENT_AT, READ_AT, IS_READ, ERROR_MESSAGE, METADATA, CREATED_AT, UPDATED_AT, IS_ACTIVE
**Índices**: 6 índices optimizados para queries frecuentes

---

#### 2. Verificar y Corregir Compilación del Backend
**Ubicación**: Backend API
**Tiempo estimado**: 30-60 minutos

**Problemas Identificados**:
- ⚠️ Errores preexistentes en `NotificationService.cs`
- ⚠️ Posibles referencias faltantes

**Pasos**:
1. Ejecutar `dotnet build` en la raíz de la API
2. Revisar errores de compilación
3. Corregir referencias faltantes
4. Verificar que todos los namespaces estén correctos
5. Compilación exitosa sin errores

---

### 🟡 FASE 3: Integración con APIs Externas (WhatsApp y Gmail)

**Tiempo estimado**: 6-8 horas
**Prioridad**: Alta

#### 3.1 Mejorar API de WhatsApp
**Ubicación**: `C:\Users\Aprendiz\Desktop\ad\ad\mi-whatsapp-api`

**Tareas**:
- [ ] Crear endpoints especializados:
  - `POST /whatsapp/appointment-confirmation`
  - `POST /whatsapp/appointment-reminder`
  - `POST /whatsapp/appointment-cancellation`
  - `GET /whatsapp/status`

- [ ] Crear sistema de templates:
```javascript
// templates/whatsappTemplates.js
const templates = {
  confirmacion_cita: (data) => `
    ✅ *Cita Confirmada*

    📅 Fecha: ${data.fecha}
    🕐 Hora: ${data.hora}
    👤 Profesional: ${data.profesional}
    📍 Ubicación: ${data.ubicacion}

    Número de cita: ${data.numeroCita}
  `,

  recordatorio_cita: (data) => `
    ⏰ *Recordatorio de Cita*

    Hola ${data.nombreCliente},

    Te recordamos que tienes una cita programada:
    📅 ${data.fecha}
    🕐 ${data.hora}
    📍 ${data.ubicacion}

    ¡Te esperamos!
  `,

  cancelacion_cita: (data) => `
    ❌ *Cita Cancelada*

    Tu cita programada para el ${data.fecha} a las ${data.hora} ha sido cancelada.

    Motivo: ${data.motivo}

    Para reagendar, visita: ${data.urlReagendar}
  `
};
```

- [ ] Agregar validación de números telefónicos
- [ ] Implementar sistema de reintentos
- [ ] Agregar autenticación con API Key
- [ ] Logging de mensajes enviados

**Archivos a crear**:
- `templates/whatsappTemplates.js`
- `middleware/auth.js`
- `routes/whatsapp.js` (actualizado)
- `.env` (configuración actualizada)

---

#### 3.2 Crear Servicios de Integración en Backend .NET

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Services/ExternalApis/`

**Tareas**:

##### A. Interface WhatsApp
```csharp
// IWhatsAppApiService.cs
public interface IWhatsAppApiService
{
    Task<bool> SendAppointmentConfirmationAsync(string phoneNumber, AppointmentConfirmationData data, CancellationToken cancellationToken = default);
    Task<bool> SendAppointmentReminderAsync(string phoneNumber, AppointmentReminderData data, CancellationToken cancellationToken = default);
    Task<bool> SendAppointmentCancellationAsync(string phoneNumber, AppointmentCancellationData data, CancellationToken cancellationToken = default);
    Task<bool> CheckStatusAsync(CancellationToken cancellationToken = default);
}
```

##### B. Implementación WhatsApp
```csharp
// WhatsAppApiService.cs
public class WhatsAppApiService : IWhatsAppApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WhatsAppApiService> _logger;

    // Constructor con IHttpClientFactory
    // Métodos implementados con manejo de errores
    // Logging completo
}
```

##### C. Interface Gmail
```csharp
// IGmailApiService.cs
public interface IGmailApiService
{
    Task<bool> SendAppointmentConfirmationAsync(string email, AppointmentConfirmationData data, CancellationToken cancellationToken = default);
    Task<bool> SendAppointmentReminderAsync(string email, AppointmentReminderData data, CancellationToken cancellationToken = default);
    Task<bool> SendAppointmentCancellationAsync(string email, AppointmentCancellationData data, CancellationToken cancellationToken = default);
    Task<bool> SendPasswordResetAsync(string email, PasswordResetData data, CancellationToken cancellationToken = default);
    Task<bool> SendWelcomeEmailAsync(string email, WelcomeData data, CancellationToken cancellationToken = default);
    Task<bool> CheckStatusAsync(CancellationToken cancellationToken = default);
}
```

##### D. Implementación Gmail
```csharp
// GmailApiService.cs
public class GmailApiService : IGmailApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GmailApiService> _logger;

    // Constructor con IHttpClientFactory
    // Métodos implementados
}
```

**Archivos a crear**:
- [ ] `Services/ExternalApis/IWhatsAppApiService.cs`
- [ ] `Services/ExternalApis/WhatsAppApiService.cs`
- [ ] `Services/ExternalApis/IGmailApiService.cs`
- [ ] `Services/ExternalApis/GmailApiService.cs`
- [ ] `DTOs/ExternalApis/AppointmentConfirmationData.cs`
- [ ] `DTOs/ExternalApis/AppointmentReminderData.cs`
- [ ] `DTOs/ExternalApis/AppointmentCancellationData.cs`

---

#### 3.3 Configurar HttpClient en DependencyInjection

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/DependencyInjection.cs`

**Código a agregar**:
```csharp
// Registrar HttpClients con Named Clients
services.AddHttpClient<IWhatsAppApiService, WhatsAppApiService>(client =>
{
    var baseUrl = configuration["ExternalApis:WhatsApp:BaseUrl"] ?? "http://localhost:3000";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

services.AddHttpClient<IGmailApiService, GmailApiService>(client =>
{
    var baseUrl = configuration["ExternalApis:Gmail:BaseUrl"] ?? "http://localhost:4000";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

---

#### 3.4 Actualizar appsettings.json

**Ubicación**: `src/3. Presentation/ElectroHuila.WebApi/appsettings.json`

**Configuración a agregar**:
```json
{
  "ExternalApis": {
    "WhatsApp": {
      "BaseUrl": "http://localhost:3000",
      "Enabled": true,
      "ApiKey": "your-secure-api-key-here",
      "RetryAttempts": 3,
      "TimeoutSeconds": 30
    },
    "Gmail": {
      "BaseUrl": "http://localhost:4000",
      "Enabled": true,
      "RetryAttempts": 3,
      "TimeoutSeconds": 30
    }
  },
  "Notifications": {
    "EnableEmail": true,
    "EnableSms": false,
    "EnableWhatsApp": true,
    "EnableInApp": true,
    "EnableSignalR": true
  }
}
```

---

#### 3.5 Actualizar NotificationService para Usar APIs Reales

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Services/NotificationService.cs`

**Cambios necesarios**:

1. **Inyectar servicios**:
```csharp
private readonly IWhatsAppApiService _whatsAppService;
private readonly IGmailApiService _gmailService;
private readonly INotificationRepository _notificationRepository;
private readonly IConfiguration _configuration;
```

2. **Actualizar método SendAppointmentConfirmationAsync**:
```csharp
public async Task SendAppointmentConfirmationAsync(int appointmentId, CancellationToken cancellationToken = default)
{
    // 1. Obtener datos de la cita
    var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

    // 2. Crear notificaciones en DB (para cada canal)
    var emailNotification = Notification.Create(
        userId: appointment.ClientId,
        appointmentId: appointmentId,
        type: "EMAIL",
        title: "Cita Confirmada",
        message: $"Tu cita para el {appointment.AppointmentDate} ha sido confirmada"
    );
    await _notificationRepository.CreateAsync(emailNotification, cancellationToken);

    // 3. Enviar por Email
    try
    {
        var emailSent = await _gmailService.SendAppointmentConfirmationAsync(
            appointment.Client.Email,
            new AppointmentConfirmationData { /* datos */ },
            cancellationToken
        );

        if (emailSent)
        {
            emailNotification.MarkAsSent();
        }
        else
        {
            emailNotification.MarkAsFailed("No se pudo enviar el email");
        }
        await _notificationRepository.UpdateAsync(emailNotification, cancellationToken);
    }
    catch (Exception ex)
    {
        emailNotification.MarkAsFailed(ex.Message);
        await _notificationRepository.UpdateAsync(emailNotification, cancellationToken);
        _logger.LogError(ex, "Error enviando email de confirmación");
    }

    // 4. Enviar por WhatsApp
    if (_configuration["ExternalApis:WhatsApp:Enabled"] == "true")
    {
        var whatsappNotification = Notification.Create(/* ... */);
        await _notificationRepository.CreateAsync(whatsappNotification, cancellationToken);

        try
        {
            var whatsappSent = await _whatsAppService.SendAppointmentConfirmationAsync(
                appointment.Client.Phone,
                new AppointmentConfirmationData { /* datos */ },
                cancellationToken
            );

            if (whatsappSent)
            {
                whatsappNotification.MarkAsSent();
            }
            else
            {
                whatsappNotification.MarkAsFailed("No se pudo enviar WhatsApp");
            }
            await _notificationRepository.UpdateAsync(whatsappNotification, cancellationToken);
        }
        catch (Exception ex)
        {
            whatsappNotification.MarkAsFailed(ex.Message);
            await _notificationRepository.UpdateAsync(whatsappNotification, cancellationToken);
            _logger.LogError(ex, "Error enviando WhatsApp de confirmación");
        }
    }

    // 5. Enviar por SignalR (ya implementado)
    await _signalRService.SendNotificationToUserAsync(/* ... */);

    // 6. Crear notificación IN_APP
    var inAppNotification = Notification.Create(
        userId: appointment.ClientId,
        appointmentId: appointmentId,
        type: "IN_APP",
        title: "Cita Confirmada",
        message: $"Tu cita para el {appointment.AppointmentDate} ha sido confirmada"
    );
    inAppNotification.MarkAsSent(); // IN_APP se marca como enviada inmediatamente
    await _notificationRepository.CreateAsync(inAppNotification, cancellationToken);
}
```

**Métodos a actualizar**:
- [ ] `SendAppointmentConfirmationAsync`
- [ ] `SendAppointmentReminderAsync`
- [ ] `SendAppointmentCancellationAsync`

---

### 🟡 FASE 4: Activar WebSocket en Frontend

**Tiempo estimado**: 2-3 horas
**Prioridad**: Alta
**Ubicación**: `C:\Users\Aprendiz\Downloads\prueba-test\pqr-scheduling-appointments-portal`

#### 4.1 Activar WebSocket en AdminLayout

**Archivo**: `src/features/admin/views/AdminLayout.tsx`

**Cambios**:

1. **Reemplazar línea 112** (actualmente `const wsConnected = false;`):
```typescript
// ANTES:
const wsConnected = false;

// DESPUÉS:
const { isConnected: wsConnected, lastMessage } = useWebSocket((message) => {
  handleWebSocketMessage(message);
});
```

2. **Agregar handler de mensajes**:
```typescript
const handleWebSocketMessage = (message: WebSocketMessage) => {
  switch (message.type) {
    case 'appointment_created':
      addNotification({
        id: Date.now(),
        type: 'appointment',
        title: 'Nueva Cita Creada',
        message: `Cita #${message.data.appointmentNumber} ha sido creada`,
        timestamp: message.timestamp,
        read: false,
        appointmentId: message.data.id,
      });
      // Actualizar lista de citas si está visible
      refetchAppointments?.();
      break;

    case 'appointment_updated':
      addNotification({
        id: Date.now(),
        type: 'appointment',
        title: 'Cita Actualizada',
        message: `Cita #${message.data.appointmentNumber} ha sido actualizada`,
        timestamp: message.timestamp,
        read: false,
        appointmentId: message.data.id,
      });
      break;

    case 'appointment_cancelled':
      addNotification({
        id: Date.now(),
        type: 'appointment',
        title: 'Cita Cancelada',
        message: `Cita #${message.data.appointmentNumber} ha sido cancelada`,
        timestamp: message.timestamp,
        read: false,
        appointmentId: message.data.id,
      });
      break;

    case 'appointment_reminder':
      addNotification({
        id: Date.now(),
        type: 'reminder',
        title: 'Recordatorio de Cita',
        message: message.data.message,
        timestamp: message.timestamp,
        read: false,
        appointmentId: message.data.appointmentId,
      });
      break;

    default:
      console.log('Mensaje WebSocket no manejado:', message);
  }
};
```

3. **Conectar al iniciar sesión**:
```typescript
useEffect(() => {
  const token = localStorage.getItem('token');
  if (token && !wsConnected) {
    signalRService.connect(token);
  }

  return () => {
    // Cleanup: desconectar al desmontar
    signalRService.disconnect();
  };
}, [wsConnected]);
```

**Archivo a modificar**:
- [ ] `src/features/admin/views/AdminLayout.tsx`

---

#### 4.2 Actualizar NotificationBell para Usar Notificaciones Reales

**Archivo**: `src/features/admin/views/components/NotificationBell.tsx`

**Cambios**:

1. **Conectar con API de notificaciones**:
```typescript
import { useNotifications } from '@/features/admin/hooks/useNotifications';

// En el componente
const {
  notifications,
  unreadCount,
  markAsRead,
  clearAll,
  isLoading
} = useNotifications();
```

2. **Actualizar el hook useNotifications**:

**Archivo**: `src/features/admin/hooks/useNotifications.ts` (crear si no existe)

```typescript
import { useState, useEffect } from 'react';
import { NotificationService } from '@/services/notifications/notification.service';

export function useNotifications() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const notificationService = new NotificationService();

  // Cargar notificaciones al montar
  useEffect(() => {
    loadNotifications();
    loadUnreadCount();
  }, []);

  const loadNotifications = async () => {
    setIsLoading(true);
    try {
      const data = await notificationService.getUserNotifications(1, 20);
      setNotifications(data);
    } catch (error) {
      console.error('Error cargando notificaciones:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const loadUnreadCount = async () => {
    try {
      const count = await notificationService.getUnreadCount();
      setUnreadCount(count);
    } catch (error) {
      console.error('Error cargando contador:', error);
    }
  };

  const markAsRead = async (notificationId: number) => {
    try {
      await notificationService.markAsRead(notificationId);
      setNotifications(prev =>
        prev.map(n => n.id === notificationId ? { ...n, isRead: true } : n)
      );
      setUnreadCount(prev => Math.max(0, prev - 1));
    } catch (error) {
      console.error('Error marcando como leída:', error);
    }
  };

  const clearAll = async () => {
    // Implementar si es necesario
  };

  const addNotification = (notification: Notification) => {
    setNotifications(prev => [notification, ...prev]);
    setUnreadCount(prev => prev + 1);
  };

  return {
    notifications,
    unreadCount,
    markAsRead,
    clearAll,
    addNotification,
    isLoading
  };
}
```

**Archivos a crear/modificar**:
- [ ] `src/services/notifications/notification.service.ts` (crear)
- [ ] `src/features/admin/hooks/useNotifications.ts` (crear o actualizar)
- [ ] `src/features/admin/views/components/NotificationBell.tsx` (actualizar)

---

#### 4.3 Crear NotificationService en Frontend

**Archivo**: `src/services/notifications/notification.service.ts`

```typescript
import { BaseHttpService } from '../base/base-http.service';

export interface NotificationDto {
  id: number;
  type: string;
  title: string;
  message: string;
  status: string;
  isRead: boolean;
  sentAt: string | null;
  createdAt: string;
  appointmentNumber?: string;
}

export class NotificationService extends BaseHttpService {
  async getUserNotifications(pageNumber: number = 1, pageSize: number = 20): Promise<NotificationDto[]> {
    const userId = this.getCurrentUserId(); // Obtener del localStorage o contexto
    return this.get<NotificationDto[]>(`/notifications/user/${userId}?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  async getUnreadCount(): Promise<number> {
    const response = await this.get<{ unreadCount: number }>('/notifications/unread-count');
    return response.unreadCount;
  }

  async markAsRead(notificationId: number): Promise<void> {
    await this.patch(`/notifications/${notificationId}/mark-read`, {});
  }

  private getCurrentUserId(): number {
    // Implementar lógica para obtener userId
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    return user.id;
  }
}
```

**Archivos a crear**:
- [ ] `src/services/notifications/notification.service.ts`
- [ ] `src/services/notifications/notification.types.ts`

---

### 🟢 FASE 5: Testing y Validación (Opcional pero Recomendado)

**Tiempo estimado**: 4-6 horas
**Prioridad**: Media

#### 5.1 Tests Backend

**Tests a crear**:
- [ ] Tests unitarios para `NotificationHub`
- [ ] Tests unitarios para `SignalRNotificationService`
- [ ] Tests unitarios para `WhatsAppApiService`
- [ ] Tests unitarios para `GmailApiService`
- [ ] Tests de integración para endpoints de `/api/v1/notifications`
- [ ] Tests para `CreateNotificationCommandHandler`
- [ ] Tests para `GetUserNotificationsQueryHandler`

**Ubicación**: `tests/ElectroHuila.Application.UnitTests/Features/Notifications/`

---

#### 5.2 Tests Frontend

**Tests a crear**:
- [ ] Tests para `useWebSocket` hook
- [ ] Tests para `useNotifications` hook
- [ ] Tests para `NotificationBell` component
- [ ] Tests para `NotificationService`
- [ ] Tests de integración SignalR

**Ubicación**: `tests/services/notifications/`

---

### 🟢 FASE 6: Mejoras y Optimizaciones (Opcional)

**Tiempo estimado**: 3-4 horas
**Prioridad**: Baja

#### 6.1 Mejoras de UX en Frontend
- [ ] Sonidos de notificación
- [ ] Notificaciones del navegador (Web Notifications API)
- [ ] Animaciones al recibir notificaciones
- [ ] Indicador visual de estado de conexión WebSocket
- [ ] Toast notifications automáticos

#### 6.2 Mejoras de Backend
- [ ] Rate limiting para envío de notificaciones
- [ ] Sistema de prioridades de notificaciones
- [ ] Queue system para envío masivo (Azure Service Bus o RabbitMQ)
- [ ] Métricas y analytics de notificaciones
- [ ] Dashboard de monitoreo de notificaciones

#### 6.3 Mejoras de API WhatsApp
- [ ] Soporte para envío de imágenes
- [ ] Soporte para envío de documentos
- [ ] Soporte para botones interactivos
- [ ] Webhooks para recibir respuestas
- [ ] Panel de administración

---

## 📊 Resumen de Progreso

### Estado General: 40%

| Fase | Estado | Progreso | Tiempo Restante |
|------|--------|----------|-----------------|
| ✅ Fase 1: SignalR Hub | Completado | 100% | - |
| ✅ Fase 2: Entidad Notification | Completado | 100% | - |
| ⏳ Prerequisitos (Migración, Compilación) | Pendiente | 0% | 1 hora |
| ⏳ Fase 3: APIs Externas | Pendiente | 0% | 6-8 horas |
| ⏳ Fase 4: Activar WebSocket Frontend | Pendiente | 0% | 2-3 horas |
| 🟢 Fase 5: Testing | Pendiente | 0% | 4-6 horas |
| 🟢 Fase 6: Mejoras | Pendiente | 0% | 3-4 horas |
| **TOTAL** | - | **40%** | **16-23 horas** |

---

## 🎯 Recomendaciones

### Orden de Ejecución Recomendado:

1. **Prerequisitos** (URGENTE - 1 hora)
   - Ejecutar migración
   - Verificar compilación
   - Corregir errores

2. **Fase 4** antes que Fase 3 (2-3 horas)
   - Activar WebSocket en frontend
   - Probar con datos de prueba
   - Validar que la conexión funciona

3. **Fase 3** (6-8 horas)
   - Integrar WhatsApp
   - Integrar Gmail
   - Actualizar NotificationService

4. **Pruebas de integración completa** (1-2 horas)
   - Crear cita → Email + WhatsApp + SignalR + IN_APP
   - Verificar que se guarden en DB
   - Verificar que aparezcan en frontend

5. **Fase 5** (opcional - 4-6 horas)
   - Tests automatizados

6. **Fase 6** (opcional - 3-4 horas)
   - Mejoras de UX y optimizaciones

---

## 📝 Notas Importantes

- **Credenciales**: Actualizar en producción las credenciales de Gmail en `.env`
- **API Keys**: Generar API keys seguras para WhatsApp API
- **CORS**: Asegurar que los orígenes estén configurados correctamente
- **SSL/TLS**: En producción usar HTTPS para WebSocket (wss://)
- **Monitoring**: Considerar implementar Application Insights o similar
- **Logs**: Todos los servicios tienen logging, revisar periódicamente
- **Rate Limits**: Gmail tiene límites de envío diario, considerar esto

---

## 🔗 Referencias

### Documentación Creada:
- `SIGNALR_IMPLEMENTATION_GUIDE.md` - Guía de implementación de SignalR

### APIs Externas:
- WhatsApp API: `http://localhost:3000`
- Gmail API: `http://localhost:4000`
- SignalR Hub: `ws://localhost:5000/hubs/notifications`

### Endpoints API Backend:
```
GET    /api/v1/notifications/user/{userId}
GET    /api/v1/notifications/unread-count
GET    /api/v1/notifications/user/{userId}/unread-count
PATCH  /api/v1/notifications/{id}/mark-read
POST   /api/v1/notifications
```

---

**Última actualización**: 2025-11-21
**Autor**: Sistema de Notificaciones - Equipo de Desarrollo
