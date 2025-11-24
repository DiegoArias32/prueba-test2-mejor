# Integración con APIs Externas - Sistema de Notificaciones ElectroHuila

## Resumen de Implementación

Se ha completado la integración con APIs externas para el sistema de notificaciones, permitiendo enviar notificaciones a través de WhatsApp y Gmail.

## Arquitectura de Servicios

```
┌─────────────────────────────────────────────────────────────┐
│                    NotificationService                      │
│  (Orquestador principal de notificaciones)                 │
└─────────────────┬───────────────────────┬───────────────────┘
                  │                       │
         ┌────────▼────────┐     ┌───────▼────────┐
         │ WhatsApp API    │     │   Gmail API    │
         │    Service      │     │    Service     │
         └────────┬────────┘     └───────┬────────┘
                  │                       │
         ┌────────▼────────┐     ┌───────▼────────┐
         │ WhatsApp API    │     │   Gmail API    │
         │  (External)     │     │  (External)    │
         │ localhost:3000  │     │ localhost:4000 │
         └─────────────────┘     └────────────────┘
```

## Archivos Creados

### 1. DTOs para Datos de Notificación
**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/DTOs/ExternalApis/`

- **AppointmentConfirmationData.cs**: Datos para confirmación de citas
  - NumeroCita, NombreCliente, Fecha, Hora, Profesional, Ubicacion, Direccion, TipoCita

- **AppointmentReminderData.cs**: Datos para recordatorios
  - NombreCliente, Fecha, Hora, Ubicacion, Direccion, NumeroCita, HorasAntes

- **AppointmentCancellationData.cs**: Datos para cancelaciones
  - NombreCliente, Fecha, Hora, Motivo, UrlReagendar, Ubicacion, NumeroCita

- **PasswordResetData.cs**: Datos para restablecimiento de contraseña
  - NombreUsuario, ResetToken, ResetUrl, ExpirationHours, Email

- **WelcomeData.cs**: Datos para correos de bienvenida
  - NombreUsuario, Email, AppUrl, TipoCuenta, FechaRegistro

### 2. Interfaces de Servicios
**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Services/ExternalApis/`

- **IWhatsAppApiService.cs**: Interfaz para WhatsApp API
  - SendAppointmentConfirmationAsync
  - SendAppointmentReminderAsync
  - SendAppointmentCancellationAsync
  - CheckStatusAsync

- **IGmailApiService.cs**: Interfaz para Gmail API
  - SendAppointmentConfirmationAsync
  - SendAppointmentReminderAsync
  - SendAppointmentCancellationAsync
  - SendPasswordResetAsync
  - SendWelcomeEmailAsync
  - CheckStatusAsync

### 3. Implementaciones de Servicios
**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Services/ExternalApis/`

- **WhatsAppApiService.cs**: Implementación de WhatsApp API
  - Gestión de HttpClient
  - Logging extensivo
  - Manejo de errores
  - Validación de habilitación del servicio
  - Endpoints:
    - POST `/whatsapp/appointment-confirmation`
    - POST `/whatsapp/appointment-reminder`
    - POST `/whatsapp/appointment-cancellation`
    - GET `/whatsapp/status`

- **GmailApiService.cs**: Implementación de Gmail API
  - Gestión de HttpClient
  - Logging extensivo
  - Manejo de errores
  - Validación de habilitación del servicio
  - Endpoints:
    - POST `/gmail/appointment-confirmation`
    - POST `/gmail/appointment-reminder`
    - POST `/gmail/appointment-cancellation`
    - POST `/gmail/password-reset`
    - POST `/gmail/welcome`
    - GET `/gmail/status`

## Archivos Modificados

### 1. DependencyInjection.cs
**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/DependencyInjection.cs`

**Cambios realizados**:
- Agregado using para `ElectroHuila.Infrastructure.Services.ExternalApis`
- Registrado HttpClient para WhatsApp API con:
  - Base URL configurable (default: http://localhost:3000)
  - Timeout configurable (default: 30 segundos)
  - API Key en headers (si está configurada)
- Registrado HttpClient para Gmail API con:
  - Base URL configurable (default: http://localhost:4000)
  - Timeout configurable (default: 30 segundos)

### 2. appsettings.json
**Ubicación**: `src/3. Presentation/ElectroHuila.WebApi/appsettings.json`

**Configuración agregada**:

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

## Características Implementadas

### 1. Inyección de Dependencias
- HttpClient configurado con patrón HttpClientFactory
- Timeout configurable por servicio
- Base URL dinámica desde configuración
- API Key opcional para WhatsApp

### 2. Logging Extensivo
- LogInformation para operaciones exitosas
- LogWarning para advertencias (API deshabilitada, errores HTTP)
- LogError para excepciones (HTTP, timeout, genéricas)
- LogDebug para chequeos de estado

### 3. Manejo de Errores
- Try-catch en todos los métodos
- HttpRequestException: errores de red
- TaskCanceledException: timeouts
- Exception: errores genéricos
- Retorno de bool para indicar éxito/fallo

### 4. Configuración Flexible
- Habilitación/deshabilitación por servicio
- URLs configurables
- Timeouts configurables
- Reintentos configurables (preparado para implementación futura)

### 5. Inmutabilidad y Type Safety
- DTOs implementados como records
- Propiedades required para datos obligatorios
- Propiedades opcionales con nullable types

## Endpoints de APIs Externas

### WhatsApp API (localhost:3000)
```
POST /whatsapp/appointment-confirmation
POST /whatsapp/appointment-reminder
POST /whatsapp/appointment-cancellation
GET  /whatsapp/status
```

### Gmail API (localhost:4000)
```
POST /gmail/appointment-confirmation
POST /gmail/appointment-reminder
POST /gmail/appointment-cancellation
POST /gmail/password-reset
POST /gmail/welcome
GET  /gmail/status
```

## Formato de Payloads

Todos los endpoints POST reciben un payload con esta estructura:

```json
{
  "phoneNumber": "+573001234567",  // Para WhatsApp
  "email": "user@example.com",     // Para Gmail
  "data": {
    // DTO específico según el tipo de notificación
  }
}
```

## Próximos Pasos

1. **Integrar con NotificationService**: Modificar NotificationService para usar estos servicios
2. **Implementar Retry Logic**: Usar Polly para reintentos automáticos
3. **Agregar Circuit Breaker**: Protección contra APIs caídas
4. **Implementar Caching**: Cachear estados de APIs
5. **Agregar Métricas**: Tracking de éxito/fallo de envíos
6. **Crear Health Checks**: Monitorear estado de APIs externas

## Resultado de Compilación

✅ **Compilación exitosa**
- 0 errores
- 2 advertencias (pre-existentes, no relacionadas con esta implementación)
- Tiempo de compilación: 2.63 segundos

## Uso de los Servicios

### Ejemplo de inyección en un servicio:

```csharp
public class MiServicio
{
    private readonly IWhatsAppApiService _whatsappService;
    private readonly IGmailApiService _gmailService;

    public MiServicio(
        IWhatsAppApiService whatsappService,
        IGmailApiService gmailService)
    {
        _whatsappService = whatsappService;
        _gmailService = gmailService;
    }

    public async Task EnviarConfirmacion(...)
    {
        var data = new AppointmentConfirmationData
        {
            NumeroCita = "APPT-001",
            NombreCliente = "Juan Pérez",
            Fecha = "2025-11-25",
            Hora = "10:30",
            Profesional = "Dr. García",
            Ubicacion = "Sede Principal"
        };

        // Enviar por WhatsApp
        var whatsappSuccess = await _whatsappService
            .SendAppointmentConfirmationAsync("+573001234567", data);

        // Enviar por Email
        var emailSuccess = await _gmailService
            .SendAppointmentConfirmationAsync("juan@example.com", data);
    }
}
```

## Consideraciones de Seguridad

1. **API Keys**: Almacenar en variables de entorno o Azure Key Vault en producción
2. **HTTPS**: Las APIs externas deben usar HTTPS en producción
3. **Validación**: Validar números de teléfono y emails antes de enviar
4. **Rate Limiting**: Implementar límites de tasa para evitar abuso
5. **Datos Sensibles**: No logear información sensible del cliente

## Configuración de Producción

Para producción, actualizar en `appsettings.Production.json`:

```json
{
  "ExternalApis": {
    "WhatsApp": {
      "BaseUrl": "https://api.whatsapp.production.com",
      "Enabled": true,
      "ApiKey": "${WHATSAPP_API_KEY}",
      "RetryAttempts": 3,
      "TimeoutSeconds": 30
    },
    "Gmail": {
      "BaseUrl": "https://api.gmail.production.com",
      "Enabled": true,
      "RetryAttempts": 3,
      "TimeoutSeconds": 30
    }
  }
}
```
