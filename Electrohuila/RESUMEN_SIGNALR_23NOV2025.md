# Resumen de Trabajo - SignalR y Notificaciones en Tiempo Real
**Fecha:** 23 de Noviembre de 2025

## 🎯 Objetivo Principal
Implementar y corregir la funcionalidad de notificaciones en tiempo real usando SignalR entre el backend .NET y el portal Next.js.

---

## 🔧 Problemas Identificados y Solucionados

### 1. **Error: "The connection was stopped during negotiation"**
**Causa:** React Strict Mode en desarrollo monta componentes dos veces, causando que la primera conexión se aborte.

**Solución:** 
- Agregado cleanup adecuado en el hook `useWebSocket`
- La conexión ahora se desconecta correctamente al desmontar el componente
- Error es cosmético en desarrollo, no afecta funcionalidad

**Archivo modificado:**
```typescript
// pqr-scheduling-appointments-portal/src/services/websocket.service.ts
return () => {
  mounted = false;
  unsubscribe();
  clearInterval(statusInterval);
  // Disconnect on cleanup to avoid abort errors in React Strict Mode
  websocketService.disconnect().catch(() => {
    // Ignore cleanup errors
  });
};
```

---

### 2. **Error: "401 Unauthorized" en conexiones SignalR**
**Causa:** SignalR WebSocket no puede enviar headers personalizados después del handshake inicial HTTP. El JWT debe pasarse como query parameter, pero el middleware no estaba configurado para leerlo.

**Solución:**
- Configurado `JwtBearerEvents.OnMessageReceived` para extraer el token del query string
- Solo se aplica a rutas `/hubs/*` por seguridad

**Archivo modificado:**
```csharp
// ElectroHuila.Infrastructure/DependencyInjection.cs
options.Events = new JwtBearerEvents
{
    OnMessageReceived = context =>
    {
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;
        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
        {
            context.Token = accessToken;
        }
        return Task.CompletedTask;
    }
};
```

---

### 3. **Error: "Ping method does not exist"**
**Causa:** El cliente SignalR intentaba validar la conexión llamando al método `Ping()` que no existía en el hub.

**Solución:**
- Agregado método `Ping()` al `NotificationHub`

**Archivo modificado:**
```csharp
// ElectroHuila.Infrastructure/Hubs/NotificationHub.cs
public Task<string> Ping()
{
    _logger.LogDebug("Ping received from connection {ConnectionId}", Context.ConnectionId);
    return Task.FromResult("pong");
}
```

---

### 4. **Errores poco descriptivos en SignalR**
**Causa:** SignalR no tenía habilitados los errores detallados.

**Solución:**
- Habilitado `EnableDetailedErrors` en configuración de SignalR

**Archivo modificado:**
```csharp
// ElectroHuila.WebApi/Program.cs
builder.Services.AddSignalR(options => 
{ 
    options.EnableDetailedErrors = true; 
});
```

---

## ✅ Estado Final

### Conexión SignalR Funcionando ✓
```
[2025-11-23T16:03:34.438Z] Information: WebSocket connected to ws://localhost:5000/hubs/notifications
[2025-11-23T16:03:34.439Z] Information: Using HubProtocol 'json'.
```

### Características Implementadas:
1. ✅ **Autenticación JWT** - Token pasado correctamente via query string
2. ✅ **WebSocket Connection** - Conexión estable establecida
3. ✅ **Protocol Negotiation** - Protocolo JSON negociado exitosamente
4. ✅ **User Groups** - Usuarios se unen automáticamente a sus grupos personales (`user_{id}`)
5. ✅ **Role Groups** - Usuarios se unen automáticamente a grupos de roles (`role_{roleName}`)
6. ✅ **Ping/Pong** - Validación de conexión funcionando

---

## 📊 Arquitectura de Grupos en SignalR

### Grupos Automáticos al Conectar:
Cuando un usuario se conecta, automáticamente se une a:

1. **Grupo Personal:** `user_{userId}`
   - Ejemplo: `user_1`
   - Para notificaciones individuales

2. **Grupos de Roles:** `role_{roleName}`
   - Ejemplo: `role_Super Administrator`
   - Para notificaciones masivas por rol

### Métodos Disponibles en NotificationHub:

```csharp
// Enviar a un usuario específico
Task SendNotificationToUser(string userId, object notification)

// Enviar a un grupo/rol
Task SendNotificationToGroup(string groupName, object notification)

// Unirse/salir de grupos personalizados
Task JoinUserGroup(string userId)
Task LeaveUserGroup(string userId)
Task JoinRoleGroup(string roleName)
Task LeaveRoleGroup(string roleName)

// Validación de conexión
Task<string> Ping()
```

---

## 🧪 Pruebas Realizadas

### 1. Test de Endpoint de Notificaciones
```bash
POST http://localhost:5000/api/v1/Notifications?database=oracle
Authorization: Bearer {JWT_TOKEN}

Body:
{
  "userId": 1,
  "appointmentId": 1,
  "type": "INFO",
  "title": "Prueba SignalR",
  "message": "Mensaje de prueba desde Copilot",
  "metadata": "{\"test\": true}"
}
```

**Resultado:** 
- ✅ API recibe correctamente la solicitud
- ✅ JWT autenticación funciona
- ❌ Error en Oracle (problema separado de configuración EF Core con booleanos)

---

## 🐛 Problemas Pendientes

### 1. Error de Oracle con Booleanos
**Error:** `ORA-00904: "FALSE": identificador no válido`

**Causa:** EF Core está enviando literal `FALSE` para campos booleanos, pero Oracle usa `NUMBER(1)` con valores `0/1`.

**Impacto:** No afecta la funcionalidad de SignalR, pero impide guardar notificaciones en BD.

**Solución Sugerida:** Configurar value converter en EF Core:
```csharp
builder.Property(n => n.IsRead)
    .HasConversion<int>()
    .HasDefaultValue(0);
```

---

## 📝 Archivos Modificados

1. **pqr-scheduling-appointments-portal/src/services/websocket.service.ts**
   - Agregado cleanup de conexión en useEffect

2. **pqr-scheduling-appointments-api/src/2. Infrastructure/ElectroHuila.Infrastructure/DependencyInjection.cs**
   - Configurado OnMessageReceived para JWT desde query string

3. **pqr-scheduling-appointments-api/src/2. Infrastructure/ElectroHuila.Infrastructure/Hubs/NotificationHub.cs**
   - Agregado método Ping()

4. **pqr-scheduling-appointments-api/src/3. Presentation/ElectroHuila.WebApi/Program.cs**
   - Habilitado EnableDetailedErrors en SignalR

---

## 🚀 Próximos Pasos Recomendados

1. **Corregir problema de Oracle con booleanos**
   - Configurar value converter en NotificationConfiguration
   - Probar inserción de notificaciones

2. **Probar notificación end-to-end**
   - Crear notificación desde API
   - Verificar recepción en tiempo real en portal
   - Validar grupos de usuarios y roles

3. **Implementar UI de notificaciones**
   - Toast/snackbar para mostrar notificaciones
   - Badge contador de no leídas
   - Centro de notificaciones

4. **Testing en producción**
   - Deshabilitar React Strict Mode para eliminar errores cosméticos
   - Probar con múltiples usuarios simultáneos
   - Validar reconexión automática

---

## 📚 Documentación Técnica

### Stack Tecnológico:
- **Backend:** ASP.NET Core 9.0 + SignalR
- **Frontend:** Next.js + @microsoft/signalr
- **Autenticación:** JWT (HS256)
- **Base de datos:** Oracle
- **WebSocket:** ws://localhost:5000/hubs/notifications

### Configuración de Ambiente:
```bash
# Backend
PORT: 5000
JWT_SECRET: (configurado en appsettings.json)

# Frontend  
NEXT_PUBLIC_API_URL: http://localhost:5000/api/v1
PORT: 3000
```

### Logs de Conexión Exitosa:
```
[2025-11-23T16:03:34.438Z] Information: WebSocket connected
[2025-11-23T16:03:34.439Z] Information: Using HubProtocol 'json'
```

---

## 💡 Notas Importantes

1. **React Strict Mode:** Los errores "connection stopped during negotiation" son esperados en desarrollo y no afectan funcionalidad.

2. **JWT en Query String:** Es seguro para SignalR porque:
   - Solo se usa para `/hubs/*` endpoints
   - WebSocket no puede enviar headers después del handshake
   - Es el patrón estándar recomendado por Microsoft

3. **Grupos Automáticos:** Los usuarios se unen automáticamente a sus grupos al conectar, no requiere llamadas explícitas del cliente.

---

## 🎓 Aprendizajes Clave

1. **SignalR y WebSockets:** WebSockets no soportan headers personalizados después del handshake inicial, por eso JWT debe ir en query string.

2. **React Strict Mode:** En desarrollo, React monta componentes dos veces para detectar efectos secundarios, causando conexiones duplicadas/abortadas.

3. **Arquitectura de Grupos:** SignalR usa grupos internos para enrutamiento eficiente de mensajes a múltiples conexiones.

4. **OnMessageReceived:** Evento crucial para autenticación custom en SignalR, permite interceptar tokens antes de validación.

---

**Resumen preparado por:** GitHub Copilot
**Fecha:** 23 de Noviembre de 2025
