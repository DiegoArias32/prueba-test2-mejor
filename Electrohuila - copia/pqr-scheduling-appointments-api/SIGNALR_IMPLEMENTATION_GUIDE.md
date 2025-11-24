# Guía de Implementación de SignalR - Sistema de Notificaciones en Tiempo Real

## Resumen de Implementación

Se ha implementado exitosamente un sistema completo de notificaciones en tiempo real usando SignalR en la API de ElectroHuila.

---

## Archivos Creados/Modificados

### 1. Archivos Creados

#### Backend - Hub de SignalR
- **Ubicación**: `C:\Users\Aprendiz\Downloads\prueba-test\pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\Hubs\NotificationHub.cs`
- **Descripción**: Hub principal de SignalR que maneja las conexiones WebSocket y grupos de usuarios
- **Características**:
  - Envío de notificaciones a usuarios específicos
  - Envío de notificaciones a grupos
  - Unión automática de usuarios a grupos personales y de roles
  - Logging completo de conexiones/desconexiones
  - Autenticación JWT requerida

#### Backend - Interfaz del Servicio
- **Ubicación**: `C:\Users\Aprendiz\Downloads\prueba-test\pqr-scheduling-appointments-api\src\1. Core\ElectroHuila.Application\Contracts\Notifications\ISignalRNotificationService.cs`
- **Descripción**: Contrato del servicio de notificaciones SignalR
- **Métodos**:
  - `SendNotificationToUserAsync()` - Enviar a usuario específico
  - `SendNotificationToRoleAsync()` - Enviar a todos los usuarios de un rol
  - `BroadcastNotificationAsync()` - Broadcast a todos
  - `SendNotificationToGroupAsync()` - Enviar a grupo específico

#### Backend - Implementación del Servicio
- **Ubicación**: `C:\Users\Aprendiz\Downloads\prueba-test\pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\Services\SignalRNotificationService.cs`
- **Descripción**: Implementación del servicio que envuelve el IHubContext para enviar notificaciones
- **Características**:
  - Manejo robusto de errores (no lanza excepciones para no romper flujo principal)
  - Logging detallado
  - Validación de parámetros

### 2. Archivos Modificados

#### Backend - DependencyInjection.cs
- **Ubicación**: `C:\Users\Aprendiz\Downloads\prueba-test\pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\DependencyInjection.cs`
- **Cambios**: Registrado `ISignalRNotificationService` como servicio Scoped

#### Backend - Program.cs
- **Ubicación**: `C:\Users\Aprendiz\Downloads\prueba-test\pqr-scheduling-appointments-api\src\3. Presentation\ElectroHuila.WebApi\Program.cs`
- **Cambios**:
  - Agregado `builder.Services.AddSignalR()`
  - Configurado CORS para soportar SignalR (requiere `AllowCredentials`)
  - Mapeado el hub en `/hubs/notifications`

#### Backend - NotificationService.cs
- **Ubicación**: `C:\Users\Aprendiz\Downloads\prueba-test\pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\Services\NotificationService.cs`
- **Cambios**: Integrado SignalR en los métodos de notificación existentes:
  - `SendAppointmentConfirmationAsync()` - Envía notificación en tiempo real cuando se confirma una cita
  - `SendAppointmentReminderAsync()` - Envía recordatorio en tiempo real
  - `SendAppointmentCancellationAsync()` - Envía notificación de cancelación en tiempo real

#### Backend - Paquetes NuGet
- **Archivo**: `ElectroHuila.WebApi.csproj` y `ElectroHuila.Infrastructure.csproj`
- **Paquetes agregados**:
  - `Microsoft.AspNetCore.SignalR` (v1.2.0) en WebApi
  - `Microsoft.AspNetCore.SignalR.Core` (v1.2.0) en Infrastructure

---

## Configuración del Endpoint

### Endpoint del Hub
```
ws://localhost:5000/hubs/notifications
wss://localhost:5000/hubs/notifications (HTTPS)
```

### Configuración CORS
Los orígenes permitidos están configurados en `Program.cs`:
- `http://localhost:3000` (React dev)
- `http://localhost:3001` (React staging)
- `http://localhost:4200` (Angular)
- Versiones HTTPS de los anteriores

**IMPORTANTE**: Si tu frontend usa un puerto diferente, debes agregarlo a la configuración de CORS en `Program.cs`.

---

## Instrucciones para el Frontend

### 1. Instalar Cliente de SignalR

#### Para React/Next.js/Vue:
```bash
npm install @microsoft/signalr
```

#### Para Angular:
```bash
npm install @microsoft/signalr
```

### 2. Código de Ejemplo para Conectar (React/TypeScript)

```typescript
import * as signalR from "@microsoft/signalr";

// Configuración de la conexión
const createNotificationConnection = (accessToken: string) => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/hubs/notifications", {
      accessTokenFactory: () => accessToken, // JWT Token
      transport: signalR.HttpTransportType.WebSockets, // Usar WebSockets
      skipNegotiation: true, // Opcional: omitir negociación si solo usas WebSockets
    })
    .withAutomaticReconnect() // Reconexión automática
    .configureLogging(signalR.LogLevel.Information)
    .build();

  return connection;
};

// Hook personalizado para React
export const useNotifications = (accessToken: string | null) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [notifications, setNotifications] = useState<any[]>([]);

  useEffect(() => {
    if (!accessToken) return;

    const conn = createNotificationConnection(accessToken);

    // Registrar handler para recibir notificaciones
    conn.on("ReceiveNotification", (notification) => {
      console.log("Notification received:", notification);
      setNotifications((prev) => [...prev, notification]);

      // Mostrar notificación (puedes usar react-toastify, etc.)
      showToast(notification.title, notification.message, notification.type);
    });

    // Conectar
    conn.start()
      .then(() => {
        console.log("Connected to SignalR Hub");
        setConnection(conn);

        // Opcional: Unirse a grupos adicionales
        // conn.invoke("JoinRoleGroup", "Admin");
      })
      .catch((error) => {
        console.error("Error connecting to SignalR:", error);
      });

    // Cleanup al desmontar
    return () => {
      conn.stop();
    };
  }, [accessToken]);

  return { connection, notifications };
};
```

### 3. Código de Ejemplo para Angular (TypeScript)

```typescript
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private connection: signalR.HubConnection;
  public notifications$ = new Subject<any>();

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/notifications', {
        accessTokenFactory: () => this.getAccessToken(),
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.registerHandlers();
  }

  private registerHandlers(): void {
    this.connection.on('ReceiveNotification', (notification) => {
      console.log('Notification received:', notification);
      this.notifications$.next(notification);
    });
  }

  public async startConnection(): Promise<void> {
    try {
      await this.connection.start();
      console.log('SignalR Connected');
    } catch (err) {
      console.error('Error connecting to SignalR:', err);
    }
  }

  public async stopConnection(): Promise<void> {
    try {
      await this.connection.stop();
      console.log('SignalR Disconnected');
    } catch (err) {
      console.error('Error disconnecting:', err);
    }
  }

  private getAccessToken(): string {
    // Obtener token del localStorage o servicio de autenticación
    return localStorage.getItem('access_token') || '';
  }
}
```

### 4. Uso en Componente (React)

```typescript
import React from 'react';
import { useNotifications } from './hooks/useNotifications';
import { useAuth } from './context/AuthContext'; // Tu contexto de auth

const NotificationComponent: React.FC = () => {
  const { accessToken } = useAuth();
  const { notifications, connection } = useNotifications(accessToken);

  return (
    <div>
      <h2>Notificaciones</h2>
      {notifications.map((notif, index) => (
        <div key={index} className="notification">
          <h3>{notif.title}</h3>
          <p>{notif.message}</p>
          <small>{new Date(notif.timestamp).toLocaleString()}</small>
        </div>
      ))}
    </div>
  );
};
```

---

## Formato de Notificaciones

Las notificaciones enviadas desde el backend tienen el siguiente formato:

```typescript
interface Notification {
  type: string;              // "appointment_confirmed", "appointment_reminder", "appointment_cancelled"
  title: string;             // Título de la notificación
  message: string;           // Mensaje descriptivo
  data: {
    appointmentId: number;
    appointmentNumber: string;
    appointmentDate: string; // ISO 8601 format
    branchName: string;
    clientName: string;
    cancellationReason?: string; // Solo para cancelaciones
  };
  timestamp: string;         // ISO 8601 format (UTC)
}
```

### Ejemplos de Notificaciones

#### Cita Confirmada
```json
{
  "type": "appointment_confirmed",
  "title": "Cita Confirmada",
  "message": "Tu cita ha sido confirmada para el 25/11/2025 a las 10:30",
  "data": {
    "appointmentId": 123,
    "appointmentNumber": "APT-2025-000123",
    "appointmentDate": "2025-11-25T10:30:00Z",
    "branchName": "Sucursal Centro",
    "clientName": "Juan Pérez"
  },
  "timestamp": "2025-11-21T15:30:00Z"
}
```

#### Recordatorio de Cita
```json
{
  "type": "appointment_reminder",
  "title": "Recordatorio de Cita",
  "message": "Recuerda tu cita programada para el 25/11/2025 a las 10:30",
  "data": {
    "appointmentId": 123,
    "appointmentNumber": "APT-2025-000123",
    "appointmentDate": "2025-11-25T10:30:00Z",
    "branchName": "Sucursal Centro",
    "branchAddress": "Calle 50 #25-30",
    "clientName": "Juan Pérez"
  },
  "timestamp": "2025-11-24T10:30:00Z"
}
```

#### Cita Cancelada
```json
{
  "type": "appointment_cancelled",
  "title": "Cita Cancelada",
  "message": "Tu cita del 25/11/2025 a las 10:30 ha sido cancelada",
  "data": {
    "appointmentId": 123,
    "appointmentNumber": "APT-2025-000123",
    "appointmentDate": "2025-11-25T10:30:00Z",
    "branchName": "Sucursal Centro",
    "clientName": "Juan Pérez",
    "cancellationReason": "Cliente solicitó cancelación"
  },
  "timestamp": "2025-11-21T15:30:00Z"
}
```

---

## Métodos Disponibles del Hub

El Hub expone los siguientes métodos que pueden invocarse desde el cliente:

### 1. SendNotificationToUser
```typescript
connection.invoke("SendNotificationToUser", userId, notification);
```

### 2. SendNotificationToGroup
```typescript
connection.invoke("SendNotificationToGroup", groupName, notification);
```

### 3. JoinUserGroup
```typescript
connection.invoke("JoinUserGroup", userId);
```

### 4. LeaveUserGroup
```typescript
connection.invoke("LeaveUserGroup", userId);
```

### 5. JoinRoleGroup
```typescript
connection.invoke("JoinRoleGroup", "Admin");
```

### 6. LeaveRoleGroup
```typescript
connection.invoke("LeaveRoleGroup", "Admin");
```

**NOTA**: La conexión automática une al usuario a su grupo personal (`user_{userId}`) y a todos sus grupos de roles (`role_{roleName}`).

---

## Testing

### 1. Probar la Conexión

Usa la consola del navegador para probar:

```javascript
const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/hubs/notifications", {
    accessTokenFactory: () => "TU_JWT_TOKEN_AQUI"
  })
  .build();

connection.on("ReceiveNotification", (notification) => {
  console.log("Received:", notification);
});

connection.start().then(() => {
  console.log("Connected!");
}).catch(err => console.error(err));
```

### 2. Probar desde Postman/Thunder Client

No es posible probar SignalR directamente desde Postman. Usa herramientas como:
- **SignalR Client Test Tool**: https://github.com/SignalR/SignalR-Client-Cpp
- **WebSocket King Chrome Extension**
- **Código JavaScript en consola del navegador** (recomendado)

### 3. Verificar Logs del Servidor

El servidor loggea todas las conexiones y notificaciones:
```
Client connected to NotificationHub. ConnectionId: xxx, UserId: 123
User 123 automatically joined group user_123
User 123 automatically joined role group role_Admin
```

---

## Resolución de Problemas

### Problema: CORS Error
**Solución**: Verifica que tu origen esté en la lista de CORS en `Program.cs`. Agrega tu URL:
```csharp
.WithOrigins("http://localhost:TU_PUERTO")
```

### Problema: 401 Unauthorized
**Solución**: Asegúrate de enviar el token JWT correcto:
```typescript
accessTokenFactory: () => localStorage.getItem('access_token')
```

### Problema: No recibo notificaciones
**Verificar**:
1. Conexión establecida: `connection.state === signalR.HubConnectionState.Connected`
2. Handler registrado: `connection.on("ReceiveNotification", ...)`
3. Usuario autenticado correctamente
4. El userId en el token JWT coincide con el userId al que se envía la notificación

### Problema: WebSocket connection failed
**Solución**: Verifica que el servidor esté corriendo y el endpoint sea correcto. Si usas HTTPS, usa `wss://` en lugar de `ws://`.

---

## Seguridad

### Autenticación
- El Hub requiere autenticación JWT (`[Authorize]` attribute)
- El token se envía automáticamente en cada request WebSocket
- El userId se extrae del `ClaimTypes.NameIdentifier` del token

### Autorización
- Los usuarios solo reciben notificaciones de sus propios grupos
- Los grupos se forman automáticamente: `user_{userId}`, `role_{roleName}`
- No es posible enviar notificaciones a otros usuarios sin los permisos adecuados

---

## Arquitectura

```
Frontend (React/Angular)
    |
    | WebSocket Connection
    |
SignalR Hub (/hubs/notifications)
    |
    | IHubContext<NotificationHub>
    |
SignalRNotificationService
    |
NotificationService (Email + SMS + SignalR)
    |
Appointment Commands/Queries (MediatR)
```

---

## Próximos Pasos Recomendados

1. **Implementar UI de notificaciones**: Toast notifications, badge counters, etc.
2. **Persistir notificaciones**: Guardar en BD para historial
3. **Notificaciones push**: Integrar con Firebase Cloud Messaging
4. **Filtros de notificaciones**: Permitir al usuario configurar qué notificaciones recibir
5. **Grupos personalizados**: Crear grupos dinámicos (por sucursal, por tipo de servicio, etc.)

---

## Contacto y Soporte

Para preguntas o problemas, revisar los logs del servidor en la consola donde corre la API.

**Estado de la Implementación**: ✅ Completado y compilado exitosamente

**Fecha**: 2025-11-21
