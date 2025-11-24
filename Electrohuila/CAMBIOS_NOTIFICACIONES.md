# Sistema de Notificaciones en Tiempo Real - ElectroHuila

## Resumen de Cambios Realizados

Se ha activado exitosamente el sistema de notificaciones en tiempo real para el proyecto ElectroHuila. El backend ya tenía SignalR Hub implementado, y ahora el frontend está completamente integrado.

---

## 1. Archivos Creados

### 1.1 NotificationService (Frontend)
**Ruta**: `C:\Users\User\Desktop\Electrohuila\pqr-scheduling-appointments-portal\src\services\notifications\notification.service.ts`

**Funcionalidad**:
- Servicio para gestionar notificaciones de usuario
- Integración con API REST del backend
- Métodos implementados:
  - `getUserNotifications(pageNumber, pageSize)` - Obtener notificaciones paginadas
  - `getUnreadCount()` - Obtener contador de no leídas
  - `markAsRead(notificationId)` - Marcar como leída
  - `markAllAsRead()` - Marcar todas como leídas
  - `getCurrentUserId()` - Obtener ID del usuario actual

**Características**:
- Extiende de `BaseHttpService` para usar métodos HTTP estándar
- Manejo automático de autenticación con JWT
- Singleton exportado: `notificationService`

---

### 1.2 Hook useNotifications (Mejorado)
**Ruta**: `C:\Users\User\Desktop\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\hooks\useNotifications.ts`

**Funcionalidad**:
- Hook personalizado para gestionar notificaciones en componentes React
- Integración completa con backend
- Estado local optimizado con React hooks

**API del Hook**:
```typescript
const {
  notifications,      // NotificationDto[] - Array de notificaciones
  unreadCount,        // number - Contador de no leídas
  isLoading,          // boolean - Estado de carga
  markAsRead,         // (id) => Promise<void> - Marcar como leída
  markAllAsRead,      // () => Promise<void> - Marcar todas como leídas
  addNotification,    // (notification) => void - Agregar (para WebSocket)
  refresh             // () => Promise<void> - Refrescar datos
} = useNotifications();
```

**Características**:
- Carga automática de notificaciones al montar
- Sincronización con backend
- Optimización con useCallback y useMemo
- Manejo de errores con try-catch
- Compatible con actualizaciones de WebSocket

---

## 2. Archivos Modificados

### 2.1 AdminLayout.tsx
**Ruta**: `C:\Users\User\Desktop\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\views\AdminLayout.tsx`

**Cambios Realizados**:

#### A) Imports Agregados (líneas 48-49)
```typescript
import { useWebSocket } from '@/services/websocket.service';
import { websocketService as signalRService } from '@/services/websocket.service';
```

#### B) Activación de WebSocket (línea 114)
```typescript
// ANTES:
const wsConnected = false;

// DESPUÉS:
const { isConnected: wsConnected } = useWebSocket((message) => {
  handleWebSocketMessage(message);
});
```

#### C) Handler de Mensajes WebSocket (líneas 187-241)
Agregado handler completo para procesar mensajes:
```typescript
const handleWebSocketMessage = (message) => {
  switch (message.type) {
    case 'appointment_created':
      addNotification({ ... });
      break;
    case 'appointment_updated':
      addNotification({ ... });
      break;
    case 'appointment_cancelled':
      addNotification({ ... });
      break;
    case 'appointment_reminder':
      addNotification({ ... });
      break;
    default:
      console.log('Mensaje WebSocket no manejado:', message);
  }
};
```

#### D) Auto-Conexión WebSocket (líneas 725-735)
Agregado useEffect para conectar automáticamente:
```typescript
useEffect(() => {
  const token = localStorage.getItem('token');
  if (token && !wsConnected) {
    signalRService.connect(token);
  }

  return () => {
    signalRService.disconnect();
  };
}, [wsConnected]);
```

**Tipos de Mensajes Soportados**:
1. `appointment_created` - Nueva cita creada
2. `appointment_updated` - Cita actualizada
3. `appointment_cancelled` - Cita cancelada
4. `appointment_reminder` - Recordatorio de cita

---

### 2.2 Services Index
**Ruta**: `C:\Users\User\Desktop\Electrohuila\pqr-scheduling-appointments-portal\src\services\index.ts`

**Cambios**:
- Agregado import de `NotificationService`
- Exportado singleton `notificationService`

```typescript
import { NotificationService } from './notifications/notification.service';
// ...
export const notificationService = new NotificationService();
```

---

## 3. Arquitectura del Sistema

### Flujo de Notificaciones en Tiempo Real

```
Backend SignalR Hub (Puerto 5000)
         |
         | WebSocket Connection
         ↓
websocketService (Singleton)
         |
         | addEventListener
         ↓
useWebSocket Hook
         |
         | onMessage callback
         ↓
handleWebSocketMessage (AdminLayout)
         |
         | addNotification
         ↓
useNotifications Hook (Estado Local)
         |
         ↓
NotificationBell Component (UI)
```

### Persistencia de Notificaciones

```
Backend API (/api/v1/notifications)
         ↑
         | HTTP GET/PATCH
         |
NotificationService (HTTP Client)
         ↑
         | Métodos async
         |
useNotifications Hook
         ↑
         | markAsRead, refresh, etc.
         |
NotificationBell Component
```

---

## 4. Configuración del Backend

El backend debe tener configurado:

1. **SignalR Hub** en `/notifications`
2. **Endpoints REST**:
   - `GET /notifications/user/{userId}?pageNumber={n}&pageSize={p}`
   - `GET /notifications/unread-count`
   - `PATCH /notifications/{id}/mark-read`
   - `PATCH /notifications/user/{userId}/mark-all-read`

3. **Eventos SignalR**:
   - `ReceiveNotification` - Evento principal para recibir notificaciones
   - `Connected` - Confirmación de conexión
   - `Pong` - Respuesta a ping (keep-alive)

---

## 5. Tipos de Datos

### NotificationDto (Backend)
```typescript
interface NotificationDto {
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
```

### WebSocketMessage (Frontend)
```typescript
interface WebSocketMessage {
  type: WebSocketEventType;
  data: {
    appointmentNumber?: string;
    id?: number;
    message?: string;
    appointmentId?: number;
  };
  timestamp: string;
}
```

---

## 6. Características Implementadas

### ✅ WebSocket en Tiempo Real
- Conexión automática al cargar AdminLayout
- Reconexión automática con backoff exponencial
- Soporte para múltiples transportes (WebSockets, SSE, Long Polling)
- Desconexión limpia al desmontar componente

### ✅ Gestión de Notificaciones
- Carga paginada desde backend
- Contador de no leídas en tiempo real
- Marcar como leída (individual o todas)
- Agregar notificaciones desde WebSocket

### ✅ UI/UX
- Indicador visual de conexión WebSocket ("Tiempo Real" vs "Desconectado")
- Badge animado con contador de no leídas
- Notificaciones clasificadas por tipo con iconos y colores

### ✅ Performance
- Optimización con React hooks (useCallback, useMemo)
- Actualizaciones de estado funcionales para evitar re-renders
- Singleton para servicios (evita múltiples instancias)

---

## 7. Próximos Pasos Recomendados

1. **Testing**:
   - Probar conexión WebSocket con backend real
   - Verificar eventos de notificación
   - Validar persistencia de estado

2. **Mejoras Futuras**:
   - Agregar sonido/vibración para notificaciones importantes
   - Implementar filtros de notificaciones
   - Agregar búsqueda de notificaciones
   - Persistir preferencias de notificación

3. **Monitoreo**:
   - Agregar logging de eventos WebSocket
   - Métricas de latencia de notificaciones
   - Dashboard de estado de conexiones

---

## 8. Notas Técnicas

### Compatibilidad
- **Next.js**: ✅ Compatible (SSR-safe con checks de `typeof window`)
- **React 18**: ✅ Compatible con hooks modernos
- **SignalR**: ✅ Usa @microsoft/signalr package

### Seguridad
- JWT Token en header de WebSocket
- Validación de usuario en backend
- Sanitización de mensajes WebSocket

### Dependencias
```json
{
  "@microsoft/signalr": "^8.x.x",
  "react": "^18.x.x",
  "next": "^14.x.x"
}
```

---

## 9. Comandos de Verificación

```bash
# NO ejecutar build todavía (según instrucciones)
# Solo validación de tipos
npm run lint
```

---

## Contacto y Soporte

Para cualquier problema con las notificaciones:
1. Verificar logs del navegador (Console)
2. Verificar estado de conexión SignalR
3. Validar endpoints del backend
4. Revisar autenticación JWT

---

**Fecha de Implementación**: 2025-11-22
**Estado**: ✅ Completado - Pendiente de Testing con Backend Real
