# Implementación de Sincronización de Notificaciones - App MAUI

## Resumen
Se ha implementado la sincronización completa de notificaciones entre la app móvil MAUI y el backend API AWS App Runner.

## Backend URL Configurada
- **Producción:** `https://8i6rrjp9sb.us-east-2.awsapprunner.com`
- **Desarrollo local:** `http://127.0.0.1:5000` (comentado)

## Archivos Creados/Modificados

### 1. Nuevo Servicio API: NotificationApiService.cs
**Ruta:** `pqr-scheduling-appointments-app/Services/Notifications/NotificationApiService.cs`

**Funcionalidades:**
- ✅ `GetMyNotificationsAsync()` - Obtener notificaciones paginadas
- ✅ `GetUnreadCountAsync()` - Obtener conteo de no leídas
- ✅ `MarkAsReadAsync()` - Marcar notificación como leída
- ✅ `MarkAllAsReadAsync()` - Marcar todas como leídas

**Características especiales:**
- Filtra automáticamente type='IN_APP'
- Excluye IDs > 1000000000000
- Maneja errores con fallback a almacenamiento local

### 2. Modelo Actualizado: NotificationDto.cs
**Ruta:** `pqr-scheduling-appointments-app/Models/Notifications/NotificationDto.cs`

**Cambios:**
- ✅ Campos actualizados para coincidir con backend API
- ✅ Atributos `[JsonPropertyName]` para deserialización
- ✅ Propiedades adicionales: UserId, ClientId, AppointmentId, Status, etc.
- ✅ Iconos Font Awesome específicos por tipo de notificación

### 3. Storage Service Mejorado: NotificationStorageService.cs
**Ruta:** `pqr-scheduling-appointments-app/Services/Notifications/NotificationStorageService.cs`

**Métodos nuevos:**
- ✅ `SyncWithBackendAsync()` - Combina notificaciones de API + locales
- ✅ `UpdateNotificationAsync()` - Actualiza notificación existente

**Lógica de sincronización:**
- Combina backend + local sin duplicados
- Preserva estado de lectura local si es más reciente
- Ordena por fecha descendente

### 4. ViewModel Actualizado: NotificationsViewModel.cs
**Ruta:** `pqr-scheduling-appointments-app/ViewModels/NotificationsViewModel.cs`

**Mejoras:**
- ✅ Inyección de dependencias: `INotificationApiService`, `IAuthService`
- ✅ Propiedad `LastSyncTime` para mostrar última sincronización
- ✅ `LoadNotificationsAsync()` - Sincroniza con API primero, fallback a local
- ✅ `MarkAsReadAsync()` - Actualiza en API + local
- ✅ `MarkAllAsReadAsync()` - Usa userId actual para marcar todas

### 5. Configuración Actualizada: ConfigurationService.cs
**Ruta:** `pqr-scheduling-appointments-app/Services/Configuration/ConfigurationService.cs`

**URLs configuradas:**
- ✅ API_BASE_URL: `https://8i6rrjp9sb.us-east-2.awsapprunner.com`
- ✅ SIGNALR_HUB_URL: `https://8i6rrjp9sb.us-east-2.awsapprunner.com/hubs/notifications`

### 6. Dependency Injection: MauiProgram.cs
**Ruta:** `pqr-scheduling-appointments-app/MauiProgram.cs`

**Servicios registrados:**
```csharp
builder.Services.AddSingleton<INotificationApiService, NotificationApiService>();
builder.Services.AddSingleton<NotificationStorageService>();
builder.Services.AddTransient<NotificationsViewModel>();
builder.Services.AddTransient<NotificationsPage>();
```

### 7. Modelo Común: PagedResult.cs
**Ruta:** `pqr-scheduling-appointments-app/Models/Common/PagedResult.cs`

**Funcionalidad:**
- Modelo genérico para respuestas paginadas del API
- Propiedades: Items, TotalCount, PageNumber, PageSize, TotalPages
- Flags: HasPreviousPage, HasNextPage

## Endpoints API Implementados

### 1. Obtener Notificaciones
```
GET /api/v1/notifications/my-notifications?pageNumber=1&pageSize=20
```
- Filtra: type='IN_APP' y ID <= 1000000000000
- Retorna: PagedResult<NotificationDto>

### 2. Conteo No Leídas
```
GET /api/v1/notifications/unread-count
```
- Retorna: { count: number }

### 3. Marcar Como Leída
```
PATCH /api/v1/notifications/{notificationId}/mark-read
```
- Retorna: NotificationDto actualizada

### 4. Marcar Todas Como Leídas
```
PATCH /api/v1/notifications/user/{userId}/mark-all-read
```
- Retorna: { success: bool, updatedCount: number }

## Flujo de Sincronización

### Carga Inicial
1. Usuario abre pantalla de notificaciones
2. `LoadNotificationsAsync()` se ejecuta
3. Intenta obtener datos del API backend
4. Si éxito: sincroniza con almacenamiento local
5. Si falla: carga solo desde almacenamiento local
6. Actualiza UI con notificaciones combinadas

### Marcar Como Leída
1. Usuario toca notificación
2. `MarkAsReadAsync()` se ejecuta
3. Envía PATCH al backend API
4. Actualiza almacenamiento local (siempre)
5. Actualiza conteo de no leídas desde API
6. Si API falla: actualiza solo localmente

### Marcar Todas Como Leídas
1. Usuario toca "Marcar todas como leídas"
2. Obtiene userId del AuthService
3. Envía PATCH al backend con userId
4. Actualiza almacenamiento local (siempre)
5. Actualiza UI
6. Si API falla: funciona en modo offline

## Características Clave

### ✅ Sincronización Bidireccional
- Backend → App: Descarga notificaciones del servidor
- App → Backend: Marca como leídas en el servidor

### ✅ Modo Offline
- Funciona sin conexión usando almacenamiento local
- Sincroniza automáticamente cuando hay conexión

### ✅ Filtrado Inteligente
- Solo notificaciones IN_APP (no EMAIL, SMS, WHATSAPP)
- Excluye IDs temporales > 1000000000000

### ✅ Preservación de Estado
- Si notificación local está leída y backend no, preserva lectura local
- Evita duplicados al combinar backend + local

### ✅ Manejo de Errores
- Try-catch en todos los métodos de API
- Fallback automático a almacenamiento local
- Logs detallados en consola

## Testing

### Pruebas Recomendadas

1. **Conectividad:**
   - ✅ Probar con internet conectado
   - ✅ Probar sin internet (modo offline)
   - ✅ Probar con internet intermitente

2. **Funcionalidad:**
   - ✅ Cargar notificaciones
   - ✅ Marcar individual como leída
   - ✅ Marcar todas como leídas
   - ✅ Refrescar lista

3. **Sincronización:**
   - ✅ Verificar que notificaciones aparecen desde API
   - ✅ Verificar que estado de lectura se sincroniza
   - ✅ Verificar conteo de no leídas

## Próximos Pasos (Opcional)

1. **Notificaciones Push:**
   - Integrar con Firebase Cloud Messaging (FCM)
   - Mostrar notificaciones en tiempo real

2. **SignalR en Tiempo Real:**
   - Conectar a hub de notificaciones
   - Actualizar lista automáticamente

3. **Paginación Infinita:**
   - Implementar scroll infinito
   - Cargar más notificaciones al llegar al final

4. **Cache Inteligente:**
   - Implementar tiempo de expiración de cache
   - Sincronizar solo notificaciones nuevas

## Notas Importantes

- ⚠️ La app ahora apunta a producción por defecto
- ⚠️ Para desarrollo local, cambiar API_BASE_URL en ConfigurationService
- ⚠️ Requiere autenticación JWT válida
- ⚠️ El userId se obtiene automáticamente del AuthService

## Logs de Consola

El sistema genera logs detallados:
```
🚀 LoadNotificationsAsync STARTED - Syncing with backend API
📡 Fetching notifications from backend API...
✅ Fetched 15 notifications from API
🔄 Syncing 15 backend notifications with local storage
📱 Found 3 local notifications
✅ Sync completed: 18 total notifications stored
✅ Unread count: 5
📊 Total Notifications: 18, Unread: 5
```

Usa estos logs para debugging y monitoreo.
