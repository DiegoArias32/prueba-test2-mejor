# Gmail API - ElectroHuila

API production-ready para envío de correos electrónicos a través de Gmail con templates HTML profesionales, autenticación por API Key, sistema de logging y reintentos automáticos.

## Características

- **5 Templates HTML Profesionales** - Diseños modernos y responsivos
- **Autenticación con API Key** - Seguridad en todos los endpoints POST
- **Sistema de Logging** - Registro automático de todos los envíos en archivos JSON
- **Reintentos Automáticos** - Backoff exponencial para mayor confiabilidad
- **Estadísticas y Logs** - Endpoints para consultar envíos y métricas
- **CORS Habilitado** - Compatible con frontends de cualquier origen
- **Validación de Emails** - Verificación automática de formatos
- **Compatible con .NET** - Integración directa con backend ElectroHuila

## Estructura del Proyecto

```
mi-api-gmail/
├── config/
│   └── emailSettings.js      # Configuración SMTP
├── middleware/
│   └── auth.js               # Autenticación con API Key
├── routes/
│   └── email.js              # Endpoints de la API
├── templates/
│   └── emailTemplates.js     # Templates HTML
├── utils/
│   ├── logger.js             # Sistema de logging
│   └── retryHandler.js       # Manejo de reintentos
├── logs/                     # Logs de envíos (auto-generado)
├── .env.example              # Variables de entorno ejemplo
├── .gitignore
├── index.js                  # Servidor Express
├── package.json
└── README.md
```

## Instalación

### 1. Instalar Dependencias

```bash
npm install
```

Esto instalará:
- `express@^5.1.0` - Framework web
- `nodemailer@^7.0.10` - Envío de emails
- `dotenv@^16.4.5` - Gestión de variables de entorno
- `cors@^2.8.5` - CORS para peticiones cross-origin

### 2. Configurar Variables de Entorno

Copiar `.env.example` a `.env`:

```bash
cp .env.example .env
```

Editar `.env` con tus credenciales:

```env
PORT=4000

# Obtener App Password de Gmail:
# 1. Ir a https://myaccount.google.com/security
# 2. Activar verificación en 2 pasos
# 3. Ir a "Contraseñas de aplicaciones"
# 4. Generar nueva contraseña para "Correo"
# 5. Copiar la contraseña de 16 caracteres

GMAIL_USER=tu-email@gmail.com
GMAIL_PASSWORD=xxxx xxxx xxxx xxxx

FROM_EMAIL=tu-email@gmail.com
FROM_NAME=Sistema de Citas ElectroHuila

# Generar API Key segura (recomendado: usar generador de UUID)
GMAIL_API_KEY=tu-api-key-super-segura-aqui

NODE_ENV=production
```

### 3. Iniciar el Servidor

**Desarrollo:**
```bash
npm run dev
```

**Producción:**
```bash
npm start
```

El servidor estará disponible en `http://localhost:4000`

## Templates Disponibles

| Template | Uso | Preview |
|----------|-----|---------|
| `appointmentConfirmation` | Confirmar cita agendada | Diseño morado con detalles de la cita |
| `appointmentReminder` | Recordar cita próxima | Diseño naranja con alerta de recordatorio |
| `appointmentCancellation` | Notificar cancelación | Diseño rosa con opción de reagendar |
| `passwordReset` | Recuperar contraseña | Diseño con código de 6 dígitos |
| `welcome` | Dar bienvenida a nuevo usuario | Diseño con features del sistema |

## API Endpoints

### GET - Públicos (sin autenticación)

#### `GET /`
Health check y documentación de endpoints

**Respuesta:**
```json
{
  "status": "online",
  "mensaje": "🚀 Gmail API para ElectroHuila - Production Ready",
  "version": "1.0.0",
  "endpoints": { ... }
}
```

#### `GET /email/status`
Verificar estado de conexión SMTP

**Respuesta:**
```json
{
  "ok": true,
  "status": "Conectado",
  "mensaje": "✅ Servidor SMTP operacional"
}
```

#### `GET /email/templates`
Listar templates disponibles

**Respuesta:**
```json
{
  "ok": true,
  "templates": ["appointmentConfirmation", "appointmentReminder", ...],
  "description": { ... }
}
```

#### `GET /email/stats?date=2025-11-23`
Obtener estadísticas de envíos del día

**Query params:**
- `date` (opcional): Fecha en formato `YYYY-MM-DD`. Por defecto: hoy

**Respuesta:**
```json
{
  "ok": true,
  "date": "2025-11-23",
  "stats": {
    "total": 150,
    "success": 148,
    "failed": 2,
    "byTemplate": {
      "appointmentConfirmation": { "total": 80, "success": 80, "failed": 0 },
      "appointmentReminder": { "total": 50, "success": 49, "failed": 1 },
      "passwordReset": { "total": 20, "success": 19, "failed": 1 }
    }
  }
}
```

#### `GET /email/logs?date=2025-11-23&limit=50`
Obtener logs de envíos

**Query params:**
- `date` (opcional): Fecha en formato `YYYY-MM-DD`
- `limit` (opcional): Número de logs. Por defecto: 100

**Respuesta:**
```json
{
  "ok": true,
  "date": "2025-11-23",
  "count": 50,
  "logs": [
    {
      "timestamp": "2025-11-23T14:30:00.000Z",
      "email": "usuario@example.com",
      "template": "appointmentConfirmation",
      "subject": "✓ Tu cita ha sido confirmada",
      "success": true,
      "messageId": "<abc123@gmail.com>"
    }
  ]
}
```

### POST - Requieren Autenticación

**Todas las peticiones POST requieren incluir el header:**

```
X-API-Key: tu-api-key-aqui
```

O alternativamente:

```
Authorization: Bearer tu-api-key-aqui
```

#### `POST /email/send`
Enviar email personalizado

**Body:**
```json
{
  "to": "destinatario@example.com",
  "subject": "Asunto del correo",
  "text": "Contenido en texto plano",
  "html": "<h1>Contenido HTML opcional</h1>",
  "template": "appointmentConfirmation",
  "templateData": {
    "name": "Juan Pérez",
    "date": "2025-11-25",
    "time": "10:00 AM"
  }
}
```

**Respuesta:**
```json
{
  "ok": true,
  "mensaje": "✅ Correo enviado exitosamente",
  "messageId": "<abc123@gmail.com>"
}
```

#### `POST /email/appointment-confirmation`
Enviar confirmación de cita

**Body:**
```json
{
  "to": "paciente@example.com",
  "name": "Juan Pérez",
  "date": "2025-11-25",
  "time": "10:00 AM",
  "professional": "Dr. María González",
  "location": "Sede Principal - Piso 3"
}
```

#### `POST /email/appointment-reminder`
Enviar recordatorio de cita próxima

**Body:**
```json
{
  "to": "paciente@example.com",
  "name": "Juan Pérez",
  "date": "2025-11-25",
  "time": "10:00 AM",
  "location": "Sede Principal",
  "address": "Calle 123 #45-67",
  "appointmentNumber": "APT-2025-001234"
}
```

#### `POST /email/appointment-cancellation`
Enviar notificación de cancelación

**Body:**
```json
{
  "to": "paciente@example.com",
  "name": "Juan Pérez",
  "date": "2025-11-25",
  "time": "10:00 AM",
  "professional": "Dr. María González",
  "location": "Sede Principal",
  "reason": "Solicitud del paciente",
  "schedulingUrl": "https://electrohuila.com/citas"
}
```

#### `POST /email/password-reset`
Enviar código de recuperación de contraseña

**Body:**
```json
{
  "to": "usuario@example.com",
  "name": "Juan Pérez",
  "code": "123456"
}
```

#### `POST /email/welcome`
Enviar email de bienvenida

**Body:**
```json
{
  "to": "nuevousuario@example.com",
  "name": "Juan Pérez",
  "dashboardUrl": "https://electrohuila.com/dashboard"
}
```

## Ejemplos de Uso

### cURL

**Sin autenticación (GET):**
```bash
# Verificar estado
curl http://localhost:4000/email/status

# Ver estadísticas
curl http://localhost:4000/email/stats?date=2025-11-23
```

**Con autenticación (POST):**
```bash
# Enviar confirmación de cita
curl -X POST http://localhost:4000/email/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key-aqui" \
  -d '{
    "to": "paciente@example.com",
    "name": "Juan Pérez",
    "date": "2025-11-25",
    "time": "10:00 AM",
    "professional": "Dr. María González",
    "location": "Sede Principal"
  }'

# Enviar recordatorio
curl -X POST http://localhost:4000/email/appointment-reminder \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key-aqui" \
  -d '{
    "to": "paciente@example.com",
    "name": "Juan Pérez",
    "date": "2025-11-25",
    "time": "10:00 AM",
    "location": "Sede Principal"
  }'
```

### JavaScript (Fetch)

```javascript
// Función helper para enviar emails
async function enviarEmail(endpoint, data) {
  const response = await fetch(`http://localhost:4000/email/${endpoint}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'X-API-Key': 'tu-api-key-aqui'
    },
    body: JSON.stringify(data)
  });

  return await response.json();
}

// Enviar confirmación
const resultado = await enviarEmail('appointment-confirmation', {
  to: 'paciente@example.com',
  name: 'Juan Pérez',
  date: '2025-11-25',
  time: '10:00 AM',
  professional: 'Dr. María González',
  location: 'Sede Principal'
});

console.log(resultado);
// { ok: true, mensaje: "✅ Confirmación de cita enviada", messageId: "..." }
```

### C# (.NET)

```csharp
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class GmailApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GmailApiClient(string apiKey, string baseUrl = "http://localhost:4000")
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _apiKey = apiKey;
    }

    public async Task<bool> EnviarConfirmacionCita(
        string email,
        string nombre,
        string fecha,
        string hora,
        string profesional,
        string ubicacion)
    {
        var data = new
        {
            to = email,
            name = nombre,
            date = fecha,
            time = hora,
            professional = profesional,
            location = ubicacion
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "/email/appointment-confirmation")
        {
            Content = content
        };
        request.Headers.Add("X-API-Key", _apiKey);

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EnviarRecordatorioCita(
        string email,
        string nombre,
        string fecha,
        string hora,
        string ubicacion,
        string direccion = null,
        string numeroCita = null)
    {
        var data = new
        {
            to = email,
            name = nombre,
            date = fecha,
            time = hora,
            location = ubicacion,
            address = direccion,
            appointmentNumber = numeroCita
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "/email/appointment-reminder")
        {
            Content = content
        };
        request.Headers.Add("X-API-Key", _apiKey);

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}

// Uso
var gmailApi = new GmailApiClient("tu-api-key-aqui");

await gmailApi.EnviarConfirmacionCita(
    email: "paciente@example.com",
    nombre: "Juan Pérez",
    fecha: "2025-11-25",
    hora: "10:00 AM",
    profesional: "Dr. María González",
    ubicacion: "Sede Principal"
);
```

## Sistema de Logging

Los logs se guardan automáticamente en la carpeta `logs/`:

```
logs/
├── gmail-2025-11-23.log
├── gmail-2025-11-24.log
└── gmail-2025-11-25.log
```

Cada línea del log es un JSON:

```json
{"timestamp":"2025-11-23T14:30:00.000Z","email":"usuario@example.com","template":"appointmentConfirmation","subject":"✓ Tu cita ha sido confirmada","success":true,"error":null,"messageId":"<abc123@gmail.com>"}
```

## Reintentos Automáticos

Todos los envíos implementan **backoff exponencial**:

- **Intento 1**: Inmediato
- **Intento 2**: Espera 1 segundo
- **Intento 3**: Espera 2 segundos

Si los 3 intentos fallan, se retorna error y se registra en los logs.

## Troubleshooting

### Error: "Invalid login: 535-5.7.8 Username and Password not accepted"

**Solución:**
1. Verificar que `GMAIL_USER` sea correcto
2. No usar tu contraseña de Gmail normal
3. Generar **App Password**:
   - https://myaccount.google.com/security
   - Activar verificación en 2 pasos
   - Ir a "Contraseñas de aplicaciones"
   - Generar nueva contraseña para "Correo"
   - Usar esa contraseña en `GMAIL_PASSWORD`

### Error: "API Key requerida"

**Solución:**
Asegúrate de incluir el header en todas las peticiones POST:

```
X-API-Key: tu-api-key-aqui
```

### Error: "SMTP connection timeout"

**Solución:**
1. Verificar conexión a internet
2. Verificar firewall (puerto 587 saliente)
3. Verificar que `SMTP_HOST=smtp.gmail.com` esté correcto

### Los emails no llegan

**Verificar:**
1. Revisar carpeta de SPAM
2. Verificar que `FROM_EMAIL` sea el mismo que `GMAIL_USER`
3. Ver logs con `GET /email/logs` para identificar errores
4. Verificar que el email destino sea válido

### Error: "Template not found"

**Solución:**
Verificar que el nombre del template esté escrito correctamente:
- `appointmentConfirmation` ✅
- `appointmentreminder` ❌ (falta mayúscula)
- `appointment-confirmation` ❌ (usa guion)

## Integración con Backend .NET

### 1. Crear servicio en C#

```csharp
// Services/EmailService.cs
public interface IEmailService
{
    Task<bool> EnviarConfirmacionCita(CitaDto cita);
    Task<bool> EnviarRecordatorioCita(CitaDto cita);
    Task<bool> EnviarCancelacionCita(CitaDto cita, string motivo);
}

public class EmailService : IEmailService
{
    private readonly GmailApiClient _gmailApi;

    public EmailService(IConfiguration config)
    {
        var apiKey = config["GmailApi:ApiKey"];
        var baseUrl = config["GmailApi:BaseUrl"];
        _gmailApi = new GmailApiClient(apiKey, baseUrl);
    }

    public async Task<bool> EnviarConfirmacionCita(CitaDto cita)
    {
        return await _gmailApi.EnviarConfirmacionCita(
            email: cita.PacienteEmail,
            nombre: cita.PacienteNombre,
            fecha: cita.Fecha.ToString("dd/MM/yyyy"),
            hora: cita.Hora.ToString("hh:mm tt"),
            profesional: cita.ProfesionalNombre,
            ubicacion: cita.Ubicacion
        );
    }

    // Implementar EnviarRecordatorioCita y EnviarCancelacionCita...
}
```

### 2. Configurar en appsettings.json

```json
{
  "GmailApi": {
    "BaseUrl": "http://localhost:4000",
    "ApiKey": "tu-api-key-super-segura"
  }
}
```

### 3. Registrar servicio

```csharp
// Program.cs
builder.Services.AddScoped<IEmailService, EmailService>();
```

### 4. Usar en controladores

```csharp
[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly IEmailService _emailService;

    public CitasController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearCita([FromBody] CitaDto cita)
    {
        // ... guardar cita en BD

        // Enviar email de confirmación
        await _emailService.EnviarConfirmacionCita(cita);

        return Ok(cita);
    }
}
```

## Seguridad

1. **API Key**: Cambiar en producción a una clave segura (UUID recomendado)
2. **CORS**: Configurar orígenes permitidos en producción
3. **Rate Limiting**: Considerar implementar rate limiting
4. **HTTPS**: Usar HTTPS en producción
5. **Variables de Entorno**: Nunca commitear el archivo `.env`

## Licencia

ISC - ElectroHuila Team

## Soporte

Para soporte o preguntas:
- Email: support@electrohuila.com
- Documentación: Este README

---

**Versión:** 1.0.0
**Última actualización:** 2025-11-23
**Estado:** Production Ready ✅
