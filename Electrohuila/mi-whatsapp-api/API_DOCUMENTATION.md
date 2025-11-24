# WhatsApp API v2.0 - Complete Documentation

## Table of Contents
1. [Overview](#overview)
2. [New Features](#new-features)
3. [Authentication](#authentication)
4. [Image Support](#image-support)
5. [Document Support](#document-support)
6. [Interactive Buttons](#interactive-buttons)
7. [Webhooks](#webhooks)
8. [Admin Panel](#admin-panel)
9. [Error Handling](#error-handling)
10. [Rate Limits](#rate-limits)

---

## Overview

WhatsApp API v2.0 is a production-ready API for sending WhatsApp messages with support for:
- Text messages with templates
- Images with captions
- Documents (PDF, DOCX, XLSX, etc.)
- Interactive buttons (quick replies and call-to-action)
- Webhooks for real-time events
- Web-based admin panel

**Base URL:** `http://localhost:3001` (or your configured port)

---

## New Features

### Version 2.0 Additions
- Image sending (base64 and URL)
- Document sending with multiple formats
- Interactive button messages
- Webhook system for events
- Admin panel for management
- Enhanced validation
- Better error handling

---

## Authentication

All protected endpoints require authentication using an API Key.

### Methods
1. **X-API-Key Header** (Recommended)
```bash
X-API-Key: your-api-key-here
```

2. **Authorization Bearer**
```bash
Authorization: Bearer your-api-key-here
```

### Public Endpoints (No Auth Required)
- `GET /health`
- `GET /`
- `GET /whatsapp/status`
- `GET /admin` (Admin panel)

---

## Image Support

### POST /whatsapp/send-image

Send images with optional captions.

#### Supported Formats
- JPEG/JPG
- PNG
- WebP
- GIF

#### Maximum Size
- 5MB per image

#### Request Body

**Option 1: Base64**
```json
{
  "phoneNumber": "3001234567",
  "image": {
    "type": "base64",
    "data": "base64-encoded-image-data-here",
    "mimetype": "image/jpeg",
    "filename": "image.jpg"
  },
  "caption": "Optional caption text"
}
```

**Option 2: URL**
```json
{
  "phoneNumber": "3001234567",
  "image": {
    "type": "url",
    "url": "https://example.com/image.jpg",
    "filename": "image.jpg"
  },
  "caption": "Optional caption text"
}
```

#### Response
```json
{
  "success": true,
  "message": "Imagen enviada correctamente",
  "data": {
    "phoneNumber": "+57 300 123 4567",
    "messageId": "ABC123...",
    "timestamp": "2025-11-23T10:00:00.000Z",
    "duration": "1234ms"
  }
}
```

#### cURL Example
```bash
curl -X POST http://localhost:3001/whatsapp/send-image \
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

## Document Support

### POST /whatsapp/send-document

Send documents with optional captions.

#### Supported Formats
- PDF (.pdf)
- Word (.doc, .docx)
- Excel (.xls, .xlsx)
- PowerPoint (.ppt, .pptx)
- Text (.txt)
- CSV (.csv)

#### Maximum Size
- 100MB per document

#### Request Body

**Option 1: Base64**
```json
{
  "phoneNumber": "3001234567",
  "document": {
    "type": "base64",
    "data": "base64-encoded-document-data-here",
    "mimetype": "application/pdf",
    "filename": "document.pdf"
  },
  "caption": "Please review this document"
}
```

**Option 2: URL**
```json
{
  "phoneNumber": "3001234567",
  "document": {
    "type": "url",
    "url": "https://example.com/document.pdf",
    "mimetype": "application/pdf",
    "filename": "document.pdf"
  },
  "caption": "Please review this document"
}
```

#### Response
```json
{
  "success": true,
  "message": "Documento enviado correctamente",
  "data": {
    "phoneNumber": "+57 300 123 4567",
    "messageId": "ABC123...",
    "filename": "document.pdf",
    "timestamp": "2025-11-23T10:00:00.000Z",
    "duration": "2345ms"
  }
}
```

#### JavaScript Example
```javascript
const fs = require('fs');

// Read file and convert to base64
const fileBuffer = fs.readFileSync('path/to/document.pdf');
const base64Data = fileBuffer.toString('base64');

const response = await fetch('http://localhost:3001/whatsapp/send-document', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'X-API-Key': 'your-api-key'
  },
  body: JSON.stringify({
    phoneNumber: '3001234567',
    document: {
      type: 'base64',
      data: base64Data,
      mimetype: 'application/pdf',
      filename: 'document.pdf'
    },
    caption: 'Here is your document'
  })
});
```

---

## Interactive Buttons

### POST /whatsapp/send-buttons

Send messages with interactive buttons.

#### Button Types

**1. Quick Reply Buttons**
- Maximum 3 buttons
- Simple text buttons
- 20 characters max per button

#### Request Body
```json
{
  "phoneNumber": "3001234567",
  "title": "Optional Title",
  "body": "Please select an option:",
  "footer": "Optional footer text",
  "buttons": [
    { "body": "Option 1", "id": "opt1" },
    { "body": "Option 2", "id": "opt2" },
    { "body": "Option 3", "id": "opt3" }
  ]
}
```

#### Response
```json
{
  "success": true,
  "message": "Mensaje con botones enviado correctamente",
  "data": {
    "phoneNumber": "+57 300 123 4567",
    "messageId": "ABC123...",
    "buttonCount": 3,
    "timestamp": "2025-11-23T10:00:00.000Z",
    "duration": "1456ms"
  }
}
```

#### Python Example
```python
import requests

response = requests.post(
    'http://localhost:3001/whatsapp/send-buttons',
    headers={
        'Content-Type': 'application/json',
        'X-API-Key': 'your-api-key'
    },
    json={
        'phoneNumber': '3001234567',
        'body': '¿Cómo podemos ayudarte?',
        'buttons': [
            {'body': 'Información'},
            {'body': 'Soporte'},
            {'body': 'Cancelar'}
        ],
        'footer': 'Electrohuila - Servicio al Cliente'
    }
)

print(response.json())
```

---

## Webhooks

Webhooks allow you to receive real-time notifications about WhatsApp events.

### Event Types
- `message_received` - When a message is received
- `message_status` - When message status changes (sent, delivered, read)
- `button_clicked` - When a user clicks a button
- `qr_updated` - When QR code is updated
- `ready` - When WhatsApp client is ready
- `disconnected` - When client disconnects

### Register Webhook

**POST /whatsapp/webhooks**

```json
{
  "name": "My Webhook",
  "url": "https://your-server.com/webhook",
  "secret": "optional-secret-for-signature",
  "events": ["message_received", "message_status"]
}
```

#### Response
```json
{
  "success": true,
  "webhook": {
    "id": "abc123...",
    "name": "My Webhook",
    "url": "https://your-server.com/webhook",
    "events": ["message_received", "message_status"],
    "active": true
  }
}
```

### List Webhooks

**GET /whatsapp/webhooks**

```json
{
  "success": true,
  "webhooks": [
    {
      "id": "abc123",
      "name": "My Webhook",
      "url": "https://your-server.com/webhook",
      "events": ["message_received"],
      "active": true,
      "successCount": 45,
      "failureCount": 2,
      "lastTriggered": "2025-11-23T10:00:00.000Z"
    }
  ]
}
```

### Update Webhook

**PUT /whatsapp/webhooks/:id**

```json
{
  "active": false,
  "events": ["message_received", "message_status", "button_clicked"]
}
```

### Delete Webhook

**DELETE /whatsapp/webhooks/:id**

### Test Webhook

**POST /whatsapp/webhooks/:id/test**

Sends a test event to verify webhook is working.

### Webhook Payload

When an event occurs, your webhook will receive a POST request:

```json
{
  "event": "message_received",
  "timestamp": "2025-11-23T10:00:00.000Z",
  "data": {
    "id": "message-id",
    "from": "573001234567@c.us",
    "to": "573009876543@c.us",
    "body": "Hello!",
    "type": "chat",
    "timestamp": 1700740800,
    "hasMedia": false,
    "isForwarded": false,
    "fromMe": false
  }
}
```

### Webhook Security

If you provide a secret, we'll include a signature header:

```
X-Webhook-Signature: sha256-hash-of-payload
```

Verify the signature:
```javascript
const crypto = require('crypto');

function verifyWebhook(payload, signature, secret) {
  const hash = crypto
    .createHmac('sha256', secret)
    .update(JSON.stringify(payload))
    .digest('hex');

  return hash === signature;
}
```

---

## Admin Panel

Access the web-based admin panel at: `http://localhost:3001/admin`

### Features
1. **Connection Status**
   - Real-time WhatsApp connection status
   - QR code display for pairing
   - Phone number and platform info

2. **Statistics Dashboard**
   - Today's message statistics
   - Success/failure rates
   - Message counts by template

3. **Send Test Messages**
   - Test template messages
   - Send images and documents
   - Create button messages

4. **Webhook Management**
   - Register new webhooks
   - View webhook statistics
   - Test webhooks
   - Enable/disable webhooks

5. **View Logs**
   - Filter logs by date
   - View message history
   - Success/failure details

### First Time Setup
1. Navigate to `http://localhost:3001/admin`
2. Enter your API Key when prompted
3. Key is saved in browser localStorage
4. Start managing your WhatsApp API

---

## Error Handling

### Error Response Format
```json
{
  "success": false,
  "error": "Error message here",
  "missingFields": ["field1", "field2"]
}
```

### Common Error Codes

**400 - Bad Request**
- Missing required fields
- Invalid phone number format
- Invalid image/document format
- File size exceeds limit
- Invalid button configuration

**401 - Unauthorized**
- Missing API key
- Invalid API key

**403 - Forbidden**
- API key doesn't match

**404 - Not Found**
- Endpoint doesn't exist
- Webhook not found

**500 - Internal Server Error**
- Server error
- WhatsApp client error

**503 - Service Unavailable**
- WhatsApp client not ready
- Client disconnected

### Validation Errors

**Image Validation**
```json
{
  "success": false,
  "error": "Image size exceeds maximum of 5MB"
}
```

**Button Validation**
```json
{
  "success": false,
  "error": "Maximum 3 buttons allowed for quick replies"
}
```

**Phone Validation**
```json
{
  "success": false,
  "error": "El número debe tener 10 dígitos"
}
```

---

## Rate Limits

### Recommendations
- Max 10 messages per second
- Max 1000 messages per hour per number
- Allow 2-3 seconds between messages to same number

### Retry Logic
- Automatic retry on failure (max 3 attempts)
- Exponential backoff (2s, 4s, 8s)
- All retries are logged

### Best Practices
1. Use message queues for bulk sending
2. Implement rate limiting on your side
3. Monitor statistics regularly
4. Handle webhook errors gracefully
5. Keep media files optimized (smaller is better)

---

## Complete Examples

### Send Image from File
```javascript
const fs = require('fs');
const fetch = require('node-fetch');

async function sendImage(phoneNumber, imagePath, caption) {
  const imageBuffer = fs.readFileSync(imagePath);
  const base64Image = imageBuffer.toString('base64');

  const response = await fetch('http://localhost:3001/whatsapp/send-image', {
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
      caption: caption
    })
  });

  return await response.json();
}

// Usage
sendImage('3001234567', './image.jpg', 'Hello!')
  .then(result => console.log(result))
  .catch(error => console.error(error));
```

### Complete Webhook Server (Express)
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
      console.log('New message from:', data.from);
      console.log('Message:', data.body);
      // Process incoming message
      break;

    case 'message_status':
      console.log('Message status:', data.status);
      // Update message status in database
      break;

    case 'button_clicked':
      console.log('Button clicked:', data.buttonId);
      // Handle button response
      break;
  }

  res.json({ success: true });
});

app.listen(3001, () => {
  console.log('Webhook server running on port 3001');
});
```

---

## Migration from v1.0

If upgrading from v1.0:

1. **No Breaking Changes** - All v1.0 endpoints still work
2. **New Dependencies** - Run `npm install` to get new packages
3. **Environment Variables** - No changes needed
4. **Admin Panel** - Access at `/admin`
5. **New Features** - Available immediately

### Update Steps
```bash
# Pull latest code
git pull

# Install dependencies
npm install

# Restart server
npm start

# Access admin panel
# Open http://localhost:3001/admin
```

---

## Support

For issues or questions:
- Check logs: `GET /whatsapp/logs`
- View stats: `GET /whatsapp/stats`
- Admin panel: `/admin`
- Server logs: Console output

---

**Version:** 2.0.0
**Last Updated:** 2025-11-23
