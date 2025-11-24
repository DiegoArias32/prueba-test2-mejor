# WhatsApp API v2.0 - Electrohuila

API production-ready para envío de mensajes de WhatsApp usando `whatsapp-web.js` con sistema de templates, multimedia, botones interactivos, webhooks y panel de administración.

## Características v2.0

### Core Features
- Sistema de templates predefinidos para citas
- Autenticación con API Key
- Validación de números telefónicos colombianos
- Reintentos automáticos con backoff exponencial
- Sistema de logging en archivos JSON
- Estadísticas de mensajes enviados
- Manejo robusto de errores
- CORS habilitado
- Endpoints RESTful

### New in v2.0
- **Multimedia Support**: Envío de imágenes y documentos (PDF, DOCX, XLSX, etc.)
- **Interactive Buttons**: Botones de respuesta rápida (hasta 3 botones)
- **Webhook System**: Sistema completo de webhooks para eventos en tiempo real
- **Admin Panel**: Panel web de administración con interfaz gráfica
- **Enhanced Validation**: Validación mejorada para media y botones
- **Real-time Status**: Estado de conexión en tiempo real con QR code

## Requisitos

- Node.js 14+
- npm o yarn
- WhatsApp instalado en un dispositivo móvil

## Instalación

1. **Clonar o navegar al directorio del proyecto**

```bash
cd C:\Users\User\Desktop\Electrohuila\mi-whatsapp-api
```

2. **Instalar dependencias**

```bash
npm install
```

3. **Configurar variables de entorno**

Copiar el archivo `.env.example` a `.env`:

```bash
copy .env.example .env
```

Editar el archivo `.env` y configurar:

```env
PORT=3000
WHATSAPP_API_KEY=tu-clave-secreta-aqui
LOG_LEVEL=info
LOG_INCOMING_MESSAGES=false
```

**Generar una API Key segura:**

```bash
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
```

## Uso

### Iniciar el servidor

**Modo producción:**

```bash
npm start
```

**Modo desarrollo (con auto-reload):**

```bash
npm run dev
```

### Primera vez - Vincular WhatsApp

1. Inicia el servidor
2. Aparecerá un código QR en la consola
3. Abre WhatsApp en tu teléfono
4. Ve a: Configuración > Dispositivos vinculados > Vincular dispositivo
5. Escanea el QR de la consola
6. Espera el mensaje "WhatsApp API LISTA"

## Quick Start

```bash
# 1. Install dependencies
npm install

# 2. Configure environment
cp .env.example .env
# Edit .env and set WHATSAPP_API_KEY

# 3. Start server
npm start

# 4. Scan QR code in console with WhatsApp

# 5. Access admin panel
# Open http://localhost:3000/admin
```

## Endpoints Overview

### Public Endpoints (No Auth)
- `GET /` - API information
- `GET /health` - Health check
- `GET /whatsapp/status` - WhatsApp connection status
- `GET /admin` - Admin panel UI

### Protected Endpoints (Require API Key)

#### Template Messages
- `POST /whatsapp/appointment-confirmation` - Send confirmation
- `POST /whatsapp/appointment-reminder` - Send reminder
- `POST /whatsapp/appointment-cancellation` - Send cancellation

#### Multimedia
- `POST /whatsapp/send-image` - Send image (URL or base64)
- `POST /whatsapp/send-document` - Send document (PDF, DOCX, etc.)

#### Interactive
- `POST /whatsapp/send-buttons` - Send message with buttons (max 3)

#### Webhooks
- `POST /whatsapp/webhooks` - Register webhook
- `GET /whatsapp/webhooks` - List webhooks
- `GET /whatsapp/webhooks/:id` - Get webhook details
- `PUT /whatsapp/webhooks/:id` - Update webhook
- `DELETE /whatsapp/webhooks/:id` - Delete webhook
- `POST /whatsapp/webhooks/:id/test` - Test webhook

#### Stats & Logs
- `GET /whatsapp/stats?date=YYYY-MM-DD` - Message statistics
- `GET /whatsapp/logs` - List log files
- `GET /whatsapp/templates` - List available templates

### Authentication

All protected endpoints require:

```
X-API-Key: your-api-key
```

Or alternatively:

```
Authorization: Bearer your-api-key
```

## Detailed Endpoint Documentation

For complete API documentation with examples, see:
- **[API_DOCUMENTATION.md](./API_DOCUMENTATION.md)** - Complete endpoint reference
- **[TESTING_GUIDE.md](./TESTING_GUIDE.md)** - Testing examples with curl, Postman
- **[ADMIN_PANEL.md](./ADMIN_PANEL.md)** - Admin panel user guide

## Estructura del Proyecto

```
mi-whatsapp-api/
├── index.js                    # Archivo principal del servidor
├── package.json                # Dependencias y scripts
├── .env                        # Variables de entorno (no incluir en git)
├── .env.example                # Ejemplo de variables de entorno
├── README.md                   # Esta documentación
├── API_DOCUMENTATION.md        # Documentación completa de API
├── TESTING_GUIDE.md            # Guía de pruebas
├── ADMIN_PANEL.md              # Guía del panel de administración
├── middleware/
│   ├── auth.js                 # Middleware de autenticación
│   └── validation.js           # Validación de media y botones
├── routes/
│   └── whatsapp.js             # Rutas de la API
├── templates/
│   └── whatsappTemplates.js    # Templates de mensajes
├── utils/
│   ├── phoneValidator.js       # Validador de teléfonos
│   ├── retryHandler.js         # Sistema de reintentos
│   └── logger.js               # Sistema de logging
├── webhooks/
│   ├── handler.js              # Gestor de webhooks
│   └── webhooks.json           # Configuración de webhooks (auto-creado)
├── public/
│   └── admin/
│       ├── index.html          # Panel de administración
│       ├── app.js              # Lógica del panel
│       └── styles.css          # Estilos del panel
└── logs/                       # Archivos de log (auto-creado)
    └── whatsapp-YYYY-MM-DD.log
```

## Validación de Teléfonos

El sistema valida automáticamente números colombianos:

- Deben tener 10 dígitos
- Deben empezar con 3 (móviles)
- Se normalizan al formato internacional: 57XXXXXXXXXX
- Se formatean para WhatsApp: 57XXXXXXXXXX@c.us

**Formatos aceptados:**

- `3001234567`
- `573001234567`
- `+57 300 123 4567`
- `300-123-4567`

## Sistema de Reintentos

Los mensajes se envían con reintentos automáticos:

- Máximo 3 reintentos por defecto
- Backoff exponencial (2s, 4s, 8s...)
- Logging de cada intento
- Jitter aleatorio para evitar thundering herd

## Logging

Cada mensaje enviado se registra en:
`logs/whatsapp-YYYY-MM-DD.log`

**Formato del log:**

```json
{
  "timestamp": "2025-11-22T10:00:00.000Z",
  "phoneNumber": "573001234567",
  "phoneFormatted": "573001234567@c.us",
  "template": "confirmacion_cita",
  "success": true,
  "error": null,
  "retries": 0,
  "messageLength": 234,
  "data": { ... }
}
```

## Seguridad

- API Key obligatoria para todos los endpoints (excepto `/health` y `/`)
- CORS habilitado (configurar según necesidad)
- No se exponen errores internos en producción
- Logging de intentos de acceso no autorizados

## Ejemplos de Uso

### cURL

**Confirmación de cita:**

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "data": {
      "nombre": "Juan Pérez",
      "fecha": "2025-11-25",
      "hora": "10:00 AM",
      "servicio": "Instalación de medidor"
    }
  }'
```

### JavaScript (fetch)

```javascript
const response = await fetch('http://localhost:3000/whatsapp/appointment-confirmation', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'X-API-Key': 'tu-api-key'
  },
  body: JSON.stringify({
    phoneNumber: '3001234567',
    data: {
      nombre: 'Juan Pérez',
      fecha: '2025-11-25',
      hora: '10:00 AM',
      servicio: 'Instalación de medidor'
    }
  })
});

const result = await response.json();
console.log(result);
```

### Python (requests)

```python
import requests

response = requests.post(
    'http://localhost:3000/whatsapp/appointment-confirmation',
    headers={
        'Content-Type': 'application/json',
        'X-API-Key': 'tu-api-key'
    },
    json={
        'phoneNumber': '3001234567',
        'data': {
            'nombre': 'Juan Pérez',
            'fecha': '2025-11-25',
            'hora': '10:00 AM',
            'servicio': 'Instalación de medidor'
        }
    }
)

print(response.json())
```

## Manejo de Errores

### Error 400 - Bad Request

```json
{
  "success": false,
  "error": "Faltan campos requeridos: nombre, fecha",
  "missingFields": ["nombre", "fecha"]
}
```

### Error 401 - Unauthorized

```json
{
  "success": false,
  "error": "API key requerida. Use el header X-API-Key o Authorization: Bearer {key}"
}
```

### Error 403 - Forbidden

```json
{
  "success": false,
  "error": "API key inválida"
}
```

### Error 503 - Service Unavailable

```json
{
  "success": false,
  "error": "Servicio de WhatsApp no disponible. Por favor intenta más tarde."
}
```

## Solución de Problemas

### El QR no aparece

- Verifica que Puppeteer se instaló correctamente
- Intenta instalar las dependencias de Chrome: `npm install puppeteer`

### WhatsApp se desconecta

- Revisa que el teléfono esté conectado a internet
- Verifica que WhatsApp esté activo en el teléfono
- Reinicia el servidor si es necesario

### Los mensajes no se envían

1. Verifica que WhatsApp está listo: `GET /whatsapp/status`
2. Revisa los logs en `logs/whatsapp-YYYY-MM-DD.log`
3. Verifica que el número es válido (10 dígitos, empieza con 3)
4. Revisa las estadísticas: `GET /whatsapp/stats`

### Error "Cannot find module"

```bash
npm install
```

## Mantenimiento

### Limpiar logs antiguos

Los logs se acumulan por fecha. Para limpiar manualmente:

```bash
# Eliminar logs más antiguos que 30 días (en Windows PowerShell)
Get-ChildItem logs\whatsapp-*.log | Where-Object {$_.LastWriteTime -lt (Get-Date).AddDays(-30)} | Remove-Item
```

## Admin Panel

Access the web-based admin panel at: `http://localhost:3000/admin`

### Features:
- Real-time connection status with QR code display
- Send test messages (templates, images, documents, buttons)
- Webhook management (register, test, enable/disable)
- Message statistics and history
- Interactive UI with auto-refresh
- Mobile-responsive design

See **[ADMIN_PANEL.md](./ADMIN_PANEL.md)** for complete user guide.

## New Features in v2.0

### 1. Image Support
```bash
curl -X POST http://localhost:3000/whatsapp/send-image \
  -H "X-API-Key: your-key" \
  -H "Content-Type: application/json" \
  -d '{
    "phoneNumber": "3001234567",
    "image": {
      "type": "url",
      "url": "https://example.com/image.jpg"
    },
    "caption": "Check this out!"
  }'
```

### 2. Document Support
```bash
curl -X POST http://localhost:3000/whatsapp/send-document \
  -H "X-API-Key: your-key" \
  -H "Content-Type: application/json" \
  -d '{
    "phoneNumber": "3001234567",
    "document": {
      "type": "url",
      "url": "https://example.com/document.pdf",
      "mimetype": "application/pdf",
      "filename": "document.pdf"
    }
  }'
```

### 3. Interactive Buttons
```bash
curl -X POST http://localhost:3000/whatsapp/send-buttons \
  -H "X-API-Key: your-key" \
  -H "Content-Type: application/json" \
  -d '{
    "phoneNumber": "3001234567",
    "body": "Choose an option:",
    "buttons": [
      {"body": "Option 1"},
      {"body": "Option 2"},
      {"body": "Option 3"}
    ]
  }'
```

### 4. Webhooks
```bash
# Register webhook
curl -X POST http://localhost:3000/whatsapp/webhooks \
  -H "X-API-Key: your-key" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "My Webhook",
    "url": "https://your-server.com/webhook",
    "events": ["message_received", "message_status"],
    "secret": "your-secret"
  }'
```

## Migration from v1.0

No breaking changes! All v1.0 endpoints continue to work.

**Update steps:**
```bash
# Pull latest code
git pull

# Install new dependencies
npm install

# Restart server
npm start
```

## Próximas Mejoras

- [x] Soporte para multimedia (imágenes, PDFs)
- [x] Webhooks para notificaciones
- [x] Dashboard web para estadísticas
- [x] Botones interactivos
- [ ] Rate limiting avanzado
- [ ] Queue de mensajes con prioridad
- [ ] Tests automatizados
- [ ] Docker container
- [ ] Soporte para listas y menús
- [ ] Mensajes programados

## Licencia

ISC

## Soporte

Para reportar problemas o solicitar nuevas funcionalidades, contacta al equipo de desarrollo de Electrohuila.

---

**Versión:** 2.0.0
**Última actualización:** 2025-11-23

## Change Log

### v2.0.0 (2025-11-23)
- Added image support (JPEG, PNG, WebP, GIF)
- Added document support (PDF, DOCX, XLSX, TXT, CSV)
- Added interactive button messages (quick replies)
- Implemented complete webhook system
- Created web-based admin panel
- Enhanced validation for media and buttons
- Real-time connection status with QR display
- Webhook event notifications (message_received, message_status, etc.)
- Admin UI for testing and management
- Improved error handling and logging

### v1.0.0 (2025-11-22)
- Initial release
- Template-based messages
- Authentication with API Key
- Phone validation
- Retry logic with exponential backoff
- JSON logging system
- Statistics endpoint
