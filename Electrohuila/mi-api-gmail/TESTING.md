# Guía de Pruebas - Gmail API

## Pre-requisitos

1. Tener la API iniciada: `npm start`
2. Tener el archivo `.env` configurado correctamente
3. Tener una API Key válida (del archivo `.env`)

## Pruebas Básicas

### 1. Verificar que el servidor está corriendo

```bash
curl http://localhost:4000
```

**Resultado esperado:**
```json
{
  "status": "online",
  "mensaje": "🚀 Gmail API para ElectroHuila - Production Ready",
  "version": "1.0.0"
}
```

### 2. Verificar conexión SMTP

```bash
curl http://localhost:4000/email/status
```

**Resultado esperado:**
```json
{
  "ok": true,
  "status": "Conectado",
  "mensaje": "✅ Servidor SMTP operacional"
}
```

### 3. Ver templates disponibles

```bash
curl http://localhost:4000/email/templates
```

**Resultado esperado:**
```json
{
  "ok": true,
  "templates": [
    "appointmentConfirmation",
    "appointmentReminder",
    "appointmentCancellation",
    "passwordReset",
    "welcome"
  ]
}
```

## Pruebas de Envío (requieren API Key)

**IMPORTANTE:** Reemplazar `TU_API_KEY_AQUI` con tu API Key del archivo `.env`

### 4. Enviar Confirmación de Cita

```bash
curl -X POST http://localhost:4000/email/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: TU_API_KEY_AQUI" \
  -d '{
    "to": "tu-email@example.com",
    "name": "Juan Pérez",
    "date": "2025-11-25",
    "time": "10:00 AM",
    "professional": "Dr. María González",
    "location": "Sede Principal - Piso 3"
  }'
```

**Resultado esperado:**
```json
{
  "ok": true,
  "mensaje": "✅ Confirmación de cita enviada",
  "messageId": "<abc123@gmail.com>"
}
```

### 5. Enviar Recordatorio de Cita

```bash
curl -X POST http://localhost:4000/email/appointment-reminder \
  -H "Content-Type: application/json" \
  -H "X-API-Key: TU_API_KEY_AQUI" \
  -d '{
    "to": "tu-email@example.com",
    "name": "Ana López",
    "date": "2025-11-26",
    "time": "3:00 PM",
    "location": "Sede Norte",
    "address": "Calle 123 #45-67",
    "appointmentNumber": "APT-2025-001234"
  }'
```

### 6. Enviar Cancelación de Cita

```bash
curl -X POST http://localhost:4000/email/appointment-cancellation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: TU_API_KEY_AQUI" \
  -d '{
    "to": "tu-email@example.com",
    "name": "Carlos Ruiz",
    "date": "2025-11-27",
    "time": "11:30 AM",
    "professional": "Dra. Laura Martínez",
    "location": "Sede Sur",
    "reason": "Solicitud del paciente",
    "schedulingUrl": "https://electrohuila.com/citas"
  }'
```

### 7. Enviar Recuperación de Contraseña

```bash
curl -X POST http://localhost:4000/email/password-reset \
  -H "Content-Type: application/json" \
  -H "X-API-Key: TU_API_KEY_AQUI" \
  -d '{
    "to": "tu-email@example.com",
    "name": "María Rodríguez",
    "code": "123456"
  }'
```

### 8. Enviar Email de Bienvenida

```bash
curl -X POST http://localhost:4000/email/welcome \
  -H "Content-Type: application/json" \
  -H "X-API-Key: TU_API_KEY_AQUI" \
  -d '{
    "to": "tu-email@example.com",
    "name": "Pedro Sánchez",
    "dashboardUrl": "https://electrohuila.com/dashboard"
  }'
```

## Pruebas de Logging y Estadísticas

### 9. Ver estadísticas del día

```bash
curl http://localhost:4000/email/stats
```

**Resultado esperado:**
```json
{
  "ok": true,
  "date": "2025-11-23",
  "stats": {
    "total": 5,
    "success": 5,
    "failed": 0,
    "byTemplate": {
      "appointmentConfirmation": { "total": 1, "success": 1, "failed": 0 },
      "appointmentReminder": { "total": 1, "success": 1, "failed": 0 },
      "appointmentCancellation": { "total": 1, "success": 1, "failed": 0 },
      "passwordReset": { "total": 1, "success": 1, "failed": 0 },
      "welcome": { "total": 1, "success": 1, "failed": 0 }
    }
  }
}
```

### 10. Ver logs del día

```bash
curl http://localhost:4000/email/logs?limit=10
```

**Resultado esperado:**
```json
{
  "ok": true,
  "date": "2025-11-23",
  "count": 5,
  "logs": [
    {
      "timestamp": "2025-11-23T15:30:00.000Z",
      "email": "tu-email@example.com",
      "template": "welcome",
      "subject": "🎉 ¡Bienvenido a nuestro sistema!",
      "success": true,
      "messageId": "<xyz789@gmail.com>"
    }
  ]
}
```

## Pruebas de Errores

### 11. Probar sin API Key (debe fallar)

```bash
curl -X POST http://localhost:4000/email/appointment-confirmation \
  -H "Content-Type: application/json" \
  -d '{
    "to": "test@example.com"
  }'
```

**Resultado esperado:**
```json
{
  "ok": false,
  "error": "API Key requerida. Incluye el header X-API-Key o Authorization: Bearer {key}"
}
```

### 12. Probar con API Key inválida (debe fallar)

```bash
curl -X POST http://localhost:4000/email/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: clave-incorrecta" \
  -d '{
    "to": "test@example.com"
  }'
```

**Resultado esperado:**
```json
{
  "ok": false,
  "error": "API Key inválida"
}
```

### 13. Probar sin campo requerido (debe fallar)

```bash
curl -X POST http://localhost:4000/email/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: TU_API_KEY_AQUI" \
  -d '{
    "name": "Juan"
  }'
```

**Resultado esperado:**
```json
{
  "ok": false,
  "error": "El email del destinatario es requerido"
}
```

## Script de Prueba Completo (Bash)

Crear archivo `test.sh`:

```bash
#!/bin/bash

API_KEY="TU_API_KEY_AQUI"
EMAIL="tu-email@example.com"
BASE_URL="http://localhost:4000"

echo "=== Iniciando pruebas de Gmail API ==="
echo ""

echo "1. Health Check..."
curl -s $BASE_URL | jq
echo ""

echo "2. SMTP Status..."
curl -s $BASE_URL/email/status | jq
echo ""

echo "3. Templates..."
curl -s $BASE_URL/email/templates | jq
echo ""

echo "4. Confirmación de Cita..."
curl -s -X POST $BASE_URL/email/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: $API_KEY" \
  -d "{
    \"to\": \"$EMAIL\",
    \"name\": \"Test User\",
    \"date\": \"2025-11-25\",
    \"time\": \"10:00 AM\",
    \"professional\": \"Dr. Test\",
    \"location\": \"Sede Test\"
  }" | jq
echo ""

echo "5. Estadísticas..."
curl -s $BASE_URL/email/stats | jq
echo ""

echo "6. Logs (últimos 5)..."
curl -s "$BASE_URL/email/logs?limit=5" | jq
echo ""

echo "=== Pruebas completadas ==="
```

Ejecutar:
```bash
chmod +x test.sh
./test.sh
```

## Verificación Manual

1. **Revisar bandeja de entrada** del email configurado en `to`
2. **Verificar carpeta SPAM** si no llega
3. **Revisar logs** en `logs/gmail-YYYY-MM-DD.log`
4. **Verificar consola** del servidor para ver mensajes de error

## Checklist de Validación

- [ ] Servidor inicia sin errores
- [ ] Health check responde OK
- [ ] SMTP status = "Conectado"
- [ ] 5 templates listados
- [ ] Confirmación de cita enviada y recibida
- [ ] Recordatorio de cita enviado y recibido
- [ ] Cancelación enviada y recibida
- [ ] Password reset enviado y recibido
- [ ] Welcome email enviado y recibido
- [ ] Estadísticas muestran los 5 envíos
- [ ] Logs contienen los 5 registros
- [ ] Error sin API Key funciona
- [ ] Error con API Key inválida funciona
- [ ] Logs se guardan en `logs/gmail-YYYY-MM-DD.log`

## Notas

- Cambiar `TU_API_KEY_AQUI` por tu API Key real
- Cambiar `tu-email@example.com` por un email válido para pruebas
- Si usas Gmail para pruebas, verifica que no haya rate limiting
- Los reintentos son automáticos, si falla verás 3 intentos en la consola
