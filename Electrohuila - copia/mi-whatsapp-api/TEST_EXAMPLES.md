# Ejemplos de Pruebas - WhatsApp API

## Configuración Inicial

### 1. Generar API Key

```bash
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
```

Copia el resultado en tu archivo `.env`:
```env
WHATSAPP_API_KEY=abc123def456...
```

### 2. Iniciar el servidor

```bash
npm start
```

Escanea el QR que aparece en la consola con WhatsApp.

---

## Pruebas con cURL

### Health Check (sin autenticación)

```bash
curl http://localhost:3000/health
```

### Ver información de la API

```bash
curl http://localhost:3000/
```

### Ver estado de WhatsApp (sin autenticación en este endpoint)

```bash
curl http://localhost:3000/whatsapp/status
```

### Ver templates disponibles

```bash
curl -H "X-API-Key: tu-api-key" http://localhost:3000/whatsapp/templates
```

### Enviar confirmación de cita

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key" \
  -d "{\"phoneNumber\": \"3001234567\", \"data\": {\"nombre\": \"Juan Perez\", \"fecha\": \"2025-11-25\", \"hora\": \"10:00 AM\", \"servicio\": \"Instalacion de medidor\"}}"
```

### Enviar recordatorio de cita

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-reminder \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key" \
  -d "{\"phoneNumber\": \"3001234567\", \"data\": {\"nombre\": \"Juan Perez\", \"fecha\": \"2025-11-25\", \"hora\": \"10:00 AM\", \"servicio\": \"Instalacion de medidor\", \"anticipacion\": \"manana a las 10:00 AM\"}}"
```

### Enviar cancelación de cita

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-cancellation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key" \
  -d "{\"phoneNumber\": \"3001234567\", \"data\": {\"nombre\": \"Juan Perez\", \"fecha\": \"2025-11-25\", \"hora\": \"10:00 AM\", \"motivo\": \"Emergencia tecnica\"}}"
```

### Ver estadísticas de hoy

```bash
curl -H "X-API-Key: tu-api-key" http://localhost:3000/whatsapp/stats
```

### Ver estadísticas de una fecha específica

```bash
curl -H "X-API-Key: tu-api-key" "http://localhost:3000/whatsapp/stats?date=2025-11-22"
```

### Ver archivos de log

```bash
curl -H "X-API-Key: tu-api-key" http://localhost:3000/whatsapp/logs
```

---

## Pruebas con PowerShell (Windows)

### Confirmación de cita

```powershell
$headers = @{
    "Content-Type" = "application/json"
    "X-API-Key" = "tu-api-key"
}

$body = @{
    phoneNumber = "3001234567"
    data = @{
        nombre = "Juan Pérez"
        fecha = "2025-11-25"
        hora = "10:00 AM"
        servicio = "Instalación de medidor"
        direccion = "Calle 10 #20-30"
    }
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:3000/whatsapp/appointment-confirmation" -Method POST -Headers $headers -Body $body
```

### Estadísticas

```powershell
$headers = @{
    "X-API-Key" = "tu-api-key"
}

Invoke-RestMethod -Uri "http://localhost:3000/whatsapp/stats" -Method GET -Headers $headers
```

---

## Pruebas con Node.js

### Script de prueba completo

Crear archivo `test.js`:

```javascript
const API_KEY = 'tu-api-key';
const BASE_URL = 'http://localhost:3000';

async function testAPI() {
    console.log('='.repeat(50));
    console.log('PRUEBAS DE API DE WHATSAPP');
    console.log('='.repeat(50) + '\n');

    // 1. Health check
    console.log('1. Health check...');
    const health = await fetch(`${BASE_URL}/health`);
    console.log(await health.json());
    console.log('');

    // 2. Estado de WhatsApp
    console.log('2. Estado de WhatsApp...');
    const status = await fetch(`${BASE_URL}/whatsapp/status`);
    console.log(await status.json());
    console.log('');

    // 3. Templates disponibles
    console.log('3. Templates disponibles...');
    const templates = await fetch(`${BASE_URL}/whatsapp/templates`, {
        headers: { 'X-API-Key': API_KEY }
    });
    console.log(await templates.json());
    console.log('');

    // 4. Enviar confirmación de cita
    console.log('4. Enviando confirmación de cita...');
    const confirmation = await fetch(`${BASE_URL}/whatsapp/appointment-confirmation`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-API-Key': API_KEY
        },
        body: JSON.stringify({
            phoneNumber: '3001234567',
            data: {
                nombre: 'Juan Pérez',
                fecha: '2025-11-25',
                hora: '10:00 AM',
                servicio: 'Instalación de medidor',
                direccion: 'Calle 10 #20-30',
                notas: 'Llevar documento de identidad'
            }
        })
    });
    console.log(await confirmation.json());
    console.log('');

    // 5. Estadísticas
    console.log('5. Estadísticas...');
    const stats = await fetch(`${BASE_URL}/whatsapp/stats`, {
        headers: { 'X-API-Key': API_KEY }
    });
    console.log(await stats.json());
    console.log('');

    console.log('='.repeat(50));
    console.log('PRUEBAS COMPLETADAS');
    console.log('='.repeat(50));
}

testAPI().catch(console.error);
```

Ejecutar:
```bash
node test.js
```

---

## Pruebas de Validación

### Número inválido (muy corto)

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key" \
  -d "{\"phoneNumber\": \"123\", \"data\": {\"nombre\": \"Test\", \"fecha\": \"2025-11-25\", \"hora\": \"10:00 AM\", \"servicio\": \"Test\"}}"
```

**Respuesta esperada:**
```json
{
  "success": false,
  "error": "Número inválido. Debe tener 10 dígitos (actual: 3)"
}
```

### Campos faltantes

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key" \
  -d "{\"phoneNumber\": \"3001234567\", \"data\": {\"nombre\": \"Test\"}}"
```

**Respuesta esperada:**
```json
{
  "success": false,
  "error": "Faltan campos requeridos: fecha, hora, servicio",
  "missingFields": ["fecha", "hora", "servicio"]
}
```

### Sin API Key

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -d "{\"phoneNumber\": \"3001234567\", \"data\": {\"nombre\": \"Test\"}}"
```

**Respuesta esperada:**
```json
{
  "success": false,
  "error": "API key requerida. Use el header X-API-Key o Authorization: Bearer {key}"
}
```

### API Key inválida

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: invalid-key" \
  -d "{\"phoneNumber\": \"3001234567\", \"data\": {\"nombre\": \"Test\"}}"
```

**Respuesta esperada:**
```json
{
  "success": false,
  "error": "API key inválida"
}
```

---

## Pruebas de Integración

### 1. Flujo completo de cita

```javascript
const API_KEY = 'tu-api-key';
const BASE_URL = 'http://localhost:3000';
const PHONE = '3001234567';

async function flujoCompletoCita() {
    // 1. Confirmación
    console.log('Enviando confirmación...');
    await fetch(`${BASE_URL}/whatsapp/appointment-confirmation`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-API-Key': API_KEY
        },
        body: JSON.stringify({
            phoneNumber: PHONE,
            data: {
                nombre: 'María González',
                fecha: '2025-11-25',
                hora: '10:00 AM',
                servicio: 'Revisión de contador',
                direccion: 'Carrera 5 #10-20'
            }
        })
    });

    // Esperar 5 segundos
    await new Promise(resolve => setTimeout(resolve, 5000));

    // 2. Recordatorio
    console.log('Enviando recordatorio...');
    await fetch(`${BASE_URL}/whatsapp/appointment-reminder`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-API-Key': API_KEY
        },
        body: JSON.stringify({
            phoneNumber: PHONE,
            data: {
                nombre: 'María González',
                fecha: '2025-11-25',
                hora: '10:00 AM',
                servicio: 'Revisión de contador',
                anticipacion: 'mañana a las 10:00 AM'
            }
        })
    });

    // Ver estadísticas
    console.log('Consultando estadísticas...');
    const stats = await fetch(`${BASE_URL}/whatsapp/stats`, {
        headers: { 'X-API-Key': API_KEY }
    });
    console.log(await stats.json());
}

flujoCompletoCita().catch(console.error);
```

---

## Monitoreo en Tiempo Real

### Ver logs en tiempo real (PowerShell)

```powershell
Get-Content "C:\Users\User\Desktop\Electrohuila\mi-whatsapp-api\logs\whatsapp-$(Get-Date -Format 'yyyy-MM-dd').log" -Wait -Tail 10
```

### Ver logs en tiempo real (CMD)

```cmd
powershell -Command "Get-Content 'C:\Users\User\Desktop\Electrohuila\mi-whatsapp-api\logs\whatsapp-$(Get-Date -Format ''yyyy-MM-dd'').log' -Wait -Tail 10"
```

---

## Verificación de Reintentos

Para probar el sistema de reintentos, detén temporalmente WhatsApp en tu teléfono:

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key" \
  -d "{\"phoneNumber\": \"3001234567\", \"data\": {\"nombre\": \"Test Reintentos\", \"fecha\": \"2025-11-25\", \"hora\": \"10:00 AM\", \"servicio\": \"Test\"}}"
```

Observa en la consola los reintentos con backoff exponencial.

---

## Checklist de Pruebas

- [ ] Health check responde correctamente
- [ ] Estado de WhatsApp muestra "ready"
- [ ] Templates lista 3 templates
- [ ] Confirmación de cita se envía correctamente
- [ ] Recordatorio de cita se envía correctamente
- [ ] Cancelación de cita se envía correctamente
- [ ] Validación de teléfono rechaza números inválidos
- [ ] Validación de campos rechaza datos incompletos
- [ ] Autenticación rechaza peticiones sin API key
- [ ] Autenticación rechaza API keys inválidas
- [ ] Estadísticas muestran datos correctos
- [ ] Logs se generan correctamente
- [ ] Sistema de reintentos funciona
- [ ] CORS permite peticiones desde navegador
