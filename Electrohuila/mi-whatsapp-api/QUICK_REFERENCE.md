# WhatsApp API v2.0 - Quick Reference Guide

## Server Commands

```bash
# Start server
npm start

# Development mode with auto-reload
npm run dev

# Check status
curl http://localhost:3000/health
```

---

## Authentication

All protected endpoints require API key:

```bash
# Option 1: X-API-Key header
-H "X-API-Key: your-api-key"

# Option 2: Authorization Bearer
-H "Authorization: Bearer your-api-key"
```

---

## Common Operations

### 1. Check WhatsApp Status

```bash
curl http://localhost:3000/whatsapp/status
```

**Response:**
```json
{
  "success": true,
  "status": "ready",
  "whatsappReady": true,
  "clientInfo": {
    "platform": "android",
    "phone": "573001234567"
  }
}
```

---

### 2. Send Appointment Confirmation

```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "data": {
      "nombre": "Juan Pérez",
      "fecha": "2025-11-25",
      "hora": "10:00 AM",
      "servicio": "Instalación de medidor",
      "direccion": "Calle 10 #20-30"
    }
  }'
```

---

### 3. Send Image

```bash
# From URL
curl -X POST http://localhost:3000/whatsapp/send-image \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "image": {
      "type": "url",
      "url": "https://picsum.photos/400/300"
    },
    "caption": "Check out this image!"
  }'
```

---

### 4. Send Document

```bash
# From URL
curl -X POST http://localhost:3000/whatsapp/send-document \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "document": {
      "type": "url",
      "url": "https://example.com/document.pdf",
      "mimetype": "application/pdf",
      "filename": "document.pdf"
    },
    "caption": "Please review this document"
  }'
```

---

### 5. Send Button Message

```bash
curl -X POST http://localhost:3000/whatsapp/send-buttons \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "body": "How can we help you?",
    "buttons": [
      {"body": "Information"},
      {"body": "Support"},
      {"body": "Schedule"}
    ],
    "footer": "Electrohuila - Customer Service"
  }'
```

---

### 6. Register Webhook

```bash
curl -X POST http://localhost:3000/whatsapp/webhooks \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "name": "Production Server",
    "url": "https://your-server.com/webhook",
    "events": ["message_received", "message_status"],
    "secret": "your-webhook-secret"
  }'
```

---

### 7. View Statistics

```bash
# Today's stats
curl http://localhost:3000/whatsapp/stats \
  -H "X-API-Key: your-api-key"

# Specific date
curl "http://localhost:3000/whatsapp/stats?date=2025-11-23" \
  -H "X-API-Key: your-api-key"
```

---

### 8. Test Webhook

```bash
curl -X POST http://localhost:3000/whatsapp/webhooks/WEBHOOK_ID/test \
  -H "X-API-Key: your-api-key"
```

---

## JavaScript Examples

### Send Image from File

```javascript
const fs = require('fs');
const fetch = require('node-fetch');

async function sendImage(phoneNumber, imagePath) {
  const imageBuffer = fs.readFileSync(imagePath);
  const base64Image = imageBuffer.toString('base64');

  const response = await fetch('http://localhost:3000/whatsapp/send-image', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'X-API-Key': process.env.WHATSAPP_API_KEY
    },
    body: JSON.stringify({
      phoneNumber: phoneNumber,
      image: {
        type: 'base64',
        data: base64Image,
        mimetype: 'image/jpeg',
        filename: 'image.jpg'
      },
      caption: 'Your image caption'
    })
  });

  return await response.json();
}

// Usage
sendImage('3001234567', './image.jpg')
  .then(result => console.log(result))
  .catch(error => console.error(error));
```

---

### Handle Webhook Events

```javascript
const express = require('express');
const crypto = require('crypto');

const app = express();
app.use(express.json());

const WEBHOOK_SECRET = 'your-webhook-secret';

app.post('/webhook', (req, res) => {
  // Verify signature
  const signature = req.headers['x-webhook-signature'];
  const payload = JSON.stringify(req.body);

  const expectedSignature = crypto
    .createHmac('sha256', WEBHOOK_SECRET)
    .update(payload)
    .digest('hex');

  if (signature !== expectedSignature) {
    return res.status(401).json({ error: 'Invalid signature' });
  }

  // Process event
  const { event, data } = req.body;

  switch (event) {
    case 'message_received':
      console.log('New message:', data.body);
      // Handle incoming message
      break;

    case 'message_status':
      console.log('Status update:', data.status);
      // Update message status
      break;

    case 'button_clicked':
      console.log('Button clicked:', data.buttonId);
      // Handle button response
      break;
  }

  res.json({ success: true });
});

app.listen(3001);
```

---

## Python Examples

### Send Appointment Confirmation

```python
import requests

def send_confirmation(phone, data):
    response = requests.post(
        'http://localhost:3000/whatsapp/appointment-confirmation',
        headers={
            'Content-Type': 'application/json',
            'X-API-Key': 'your-api-key'
        },
        json={
            'phoneNumber': phone,
            'data': data
        }
    )
    return response.json()

# Usage
result = send_confirmation('3001234567', {
    'nombre': 'Juan Pérez',
    'fecha': '2025-11-25',
    'hora': '10:00 AM',
    'servicio': 'Instalación de medidor'
})
print(result)
```

---

### Send Image from URL

```python
import requests

def send_image(phone, image_url, caption=''):
    response = requests.post(
        'http://localhost:3000/whatsapp/send-image',
        headers={
            'Content-Type': 'application/json',
            'X-API-Key': 'your-api-key'
        },
        json={
            'phoneNumber': phone,
            'image': {
                'type': 'url',
                'url': image_url
            },
            'caption': caption
        }
    )
    return response.json()

# Usage
result = send_image(
    '3001234567',
    'https://example.com/image.jpg',
    'Check this out!'
)
print(result)
```

---

## Admin Panel

**Access:** `http://localhost:3000/admin`

### First Time Setup
1. Open admin panel in browser
2. Enter your API key when prompted
3. Key is saved in browser localStorage
4. Start using the panel!

### Features Quick Access
- **Connection Status:** Top of page - shows WhatsApp status
- **Statistics:** Second section - today's message stats
- **Send Message:** Tab 1 - test template messages
- **Send Media:** Tab 2 - send images/documents
- **Send Buttons:** Tab 3 - create button messages
- **Webhooks:** Tab 4 - manage webhooks
- **Logs:** Tab 5 - view message history

---

## Common Issues

### WhatsApp Not Connected
```bash
# Check status
curl http://localhost:3000/whatsapp/status

# If initializing, wait for QR code in console or admin panel
# Scan with WhatsApp: Settings > Linked Devices > Link Device
```

### API Key Invalid
```bash
# Verify API key in .env file
cat .env | grep WHATSAPP_API_KEY

# Regenerate key
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"

# Update .env and restart server
```

### Message Not Sending
```bash
# 1. Check WhatsApp status
curl http://localhost:3000/whatsapp/status

# 2. Check logs
curl http://localhost:3000/whatsapp/logs \
  -H "X-API-Key: your-api-key"

# 3. Check statistics
curl http://localhost:3000/whatsapp/stats \
  -H "X-API-Key: your-api-key"

# 4. Verify phone number format (10 digits, starts with 3)
```

### Webhook Not Working
```bash
# 1. Test webhook manually
curl -X POST http://localhost:3000/whatsapp/webhooks/WEBHOOK_ID/test \
  -H "X-API-Key: your-api-key"

# 2. Check webhook status
curl http://localhost:3000/whatsapp/webhooks \
  -H "X-API-Key: your-api-key"

# 3. Verify URL is accessible
curl https://your-webhook-url.com

# 4. Check webhook server logs
```

---

## Environment Variables

```env
# Required
PORT=3000
WHATSAPP_API_KEY=your-secure-api-key-here

# Optional
LOG_LEVEL=info
LOG_INCOMING_MESSAGES=false
NODE_ENV=production
```

---

## File Locations

```
Configuration:
- .env                          - Environment variables
- webhooks/webhooks.json        - Webhook configurations

Logs:
- logs/whatsapp-YYYY-MM-DD.log  - Daily message logs

Admin Panel:
- public/admin/index.html       - Admin UI
- Access: http://localhost:3000/admin

Documentation:
- README.md                     - Main documentation
- API_DOCUMENTATION.md          - Complete API reference
- ADMIN_PANEL.md                - Admin panel guide
- QUICK_REFERENCE.md            - This file
```

---

## API Endpoints Summary

### Public (No Auth)
```
GET  /                      - API info
GET  /health                - Health check
GET  /whatsapp/status       - WhatsApp status
GET  /admin                 - Admin panel
GET  /admin/qr              - QR code data
```

### Templates (Auth Required)
```
POST /whatsapp/appointment-confirmation   - Send confirmation
POST /whatsapp/appointment-reminder       - Send reminder
POST /whatsapp/appointment-cancellation   - Send cancellation
```

### Media (Auth Required)
```
POST /whatsapp/send-image       - Send image
POST /whatsapp/send-document    - Send document
```

### Interactive (Auth Required)
```
POST /whatsapp/send-buttons     - Send buttons
```

### Webhooks (Auth Required)
```
POST   /whatsapp/webhooks           - Register
GET    /whatsapp/webhooks           - List all
GET    /whatsapp/webhooks/:id       - Get one
PUT    /whatsapp/webhooks/:id       - Update
DELETE /whatsapp/webhooks/:id       - Delete
POST   /whatsapp/webhooks/:id/test  - Test
```

### Stats (Auth Required)
```
GET /whatsapp/stats         - Statistics
GET /whatsapp/logs          - Log files
GET /whatsapp/templates     - Available templates
```

---

## Supported Formats

### Images
- JPEG/JPG
- PNG
- WebP
- GIF
- Max size: 5MB

### Documents
- PDF
- Word (DOC, DOCX)
- Excel (XLS, XLSX)
- PowerPoint (PPT, PPTX)
- Text (TXT)
- CSV
- Max size: 100MB

### Buttons
- Quick replies: 1-3 buttons
- Max 20 characters per button
- Plain text only

---

## Rate Limits

**Recommendations:**
- Max 10 messages/second
- Max 1000 messages/hour per number
- 2-3 seconds between messages to same number

**Retry Logic:**
- Auto-retry on failure (max 3 attempts)
- Exponential backoff (2s, 4s, 8s)
- All retries logged

---

## Security Checklist

- [ ] Strong API key set in .env
- [ ] .env file not in version control
- [ ] Webhook secrets configured
- [ ] HTTPS enabled for production
- [ ] CORS origins configured
- [ ] Firewall rules set
- [ ] Regular security updates
- [ ] Logs monitored
- [ ] Admin panel access controlled

---

## Performance Tips

1. **Optimize Media:**
   - Compress images before sending
   - Keep files as small as possible
   - Use appropriate formats

2. **Batch Operations:**
   - Don't send too many at once
   - Implement queue system for bulk
   - Respect rate limits

3. **Webhooks:**
   - Process async in webhook handler
   - Return 200 quickly
   - Use queue for heavy processing

4. **Monitoring:**
   - Check stats regularly
   - Monitor error rates
   - Review logs daily
   - Track webhook failures

---

## Quick Testing

### 1. Test Server Health
```bash
curl http://localhost:3000/health
```

### 2. Test WhatsApp Connection
```bash
curl http://localhost:3000/whatsapp/status
```

### 3. Test Authentication
```bash
curl http://localhost:3000/whatsapp/templates \
  -H "X-API-Key: your-api-key"
```

### 4. Test Message Send
```bash
curl -X POST http://localhost:3000/whatsapp/appointment-confirmation \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{"phoneNumber":"3001234567","data":{"nombre":"Test","fecha":"2025-11-25","hora":"10:00","servicio":"Test"}}'
```

### 5. View Results
- Check admin panel: `http://localhost:3000/admin`
- Check statistics
- Review logs
- Verify WhatsApp message received

---

## Contact & Support

**Documentation:**
- [README.md](./README.md) - Getting started
- [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) - API reference
- [ADMIN_PANEL.md](./ADMIN_PANEL.md) - Admin guide
- [TESTING_GUIDE.md](./TESTING_GUIDE.md) - Testing examples

**Debugging:**
- Server logs (console output)
- Message logs (logs/ directory)
- Admin panel (http://localhost:3000/admin)
- Health endpoint (/health)

---

**Quick Reference Version:** 1.0
**Last Updated:** 2025-11-23
**For:** WhatsApp API v2.0 - Electrohuila
