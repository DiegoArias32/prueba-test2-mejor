# Testing Guide - WhatsApp API v2.0

Complete guide for testing all API endpoints and features.

## Table of Contents
1. [Setup](#setup)
2. [Test Image Sending](#test-image-sending)
3. [Test Document Sending](#test-document-sending)
4. [Test Interactive Buttons](#test-interactive-buttons)
5. [Test Webhooks](#test-webhooks)
6. [Test Admin Panel](#test-admin-panel)
7. [Troubleshooting](#troubleshooting)

---

## Setup

### Prerequisites
```bash
# Ensure server is running
npm start

# Verify status
curl http://localhost:3000/health
```

### Environment Variables
```env
PORT=3000
WHATSAPP_API_KEY=your-secure-api-key-here
LOG_LEVEL=info
LOG_INCOMING_MESSAGES=true
```

### Get Your API Key
```bash
# Generate a secure API key
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
```

---

## Test Image Sending

### Test 1: Send Image from URL

**cURL:**
```bash
curl -X POST http://localhost:3000/whatsapp/send-image \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "image": {
      "type": "url",
      "url": "https://picsum.photos/400/300",
      "filename": "test-image.jpg"
    },
    "caption": "Test image from URL"
  }'
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Imagen enviada correctamente",
  "data": {
    "phoneNumber": "+57 300 123 4567",
    "messageId": "...",
    "timestamp": "...",
    "duration": "..."
  }
}
```

### Test 2: Send Image from Base64

**JavaScript (Node.js):**
```javascript
const fs = require('fs');
const axios = require('axios');

async function testBase64Image() {
  // Read image file
  const imageBuffer = fs.readFileSync('./test-image.jpg');
  const base64Image = imageBuffer.toString('base64');

  const response = await axios.post('http://localhost:3000/whatsapp/send-image', {
    phoneNumber: '3001234567',
    image: {
      type: 'base64',
      data: base64Image,
      mimetype: 'image/jpeg',
      filename: 'test.jpg'
    },
    caption: 'Test image from base64'
  }, {
    headers: {
      'X-API-Key': 'your-api-key'
    }
  });

  console.log(response.data);
}

testBase64Image();
```

### Test 3: Send Different Image Formats

**PNG:**
```bash
curl -X POST http://localhost:3000/whatsapp/send-image \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "image": {
      "type": "url",
      "url": "https://picsum.photos/400/300.png"
    }
  }'
```

**WebP:**
```bash
curl -X POST http://localhost:3000/whatsapp/send-image \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "image": {
      "type": "url",
      "url": "https://picsum.photos/400/300.webp"
    }
  }'
```

### Test 4: Test Image Validation

**Invalid Format (should fail):**
```bash
curl -X POST http://localhost:3000/whatsapp/send-image \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "image": {
      "type": "url",
      "url": "https://example.com/file.pdf"
    }
  }'
```

**Missing Image Data (should fail):**
```bash
curl -X POST http://localhost:3000/whatsapp/send-image \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567"
  }'
```

---

## Test Document Sending

### Test 1: Send PDF from URL

```bash
curl -X POST http://localhost:3000/whatsapp/send-document \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "document": {
      "type": "url",
      "url": "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
      "mimetype": "application/pdf",
      "filename": "test-document.pdf"
    },
    "caption": "Test PDF document"
  }'
```

### Test 2: Send XLSX from Base64

**Python Example:**
```python
import requests
import base64

def send_excel_document():
    # Read Excel file
    with open('test-data.xlsx', 'rb') as f:
        file_data = f.read()
        base64_data = base64.b64encode(file_data).decode('utf-8')

    response = requests.post(
        'http://localhost:3000/whatsapp/send-document',
        headers={
            'Content-Type': 'application/json',
            'X-API-Key': 'your-api-key'
        },
        json={
            'phoneNumber': '3001234567',
            'document': {
                'type': 'base64',
                'data': base64_data,
                'mimetype': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
                'filename': 'data.xlsx'
            },
            'caption': 'Monthly report'
        }
    )

    print(response.json())

send_excel_document()
```

### Test 3: Send Multiple Document Types

**DOCX:**
```bash
curl -X POST http://localhost:3000/whatsapp/send-document \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "document": {
      "type": "url",
      "url": "https://example.com/document.docx",
      "mimetype": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
      "filename": "report.docx"
    }
  }'
```

**Text File:**
```bash
curl -X POST http://localhost:3000/whatsapp/send-document \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "document": {
      "type": "url",
      "url": "https://example.com/readme.txt",
      "mimetype": "text/plain",
      "filename": "readme.txt"
    }
  }'
```

---

## Test Interactive Buttons

### Test 1: Simple Quick Reply Buttons

```bash
curl -X POST http://localhost:3000/whatsapp/send-buttons \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "body": "¿Cómo podemos ayudarte hoy?",
    "buttons": [
      { "body": "Información" },
      { "body": "Soporte" },
      { "body": "Cancelar" }
    ],
    "footer": "Electrohuila - Atención al Cliente"
  }'
```

### Test 2: Buttons with Title

```bash
curl -X POST http://localhost:3000/whatsapp/send-buttons \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "title": "Confirmar Cita",
    "body": "Su cita está programada para mañana a las 10:00 AM. ¿Desea confirmar?",
    "buttons": [
      { "body": "Confirmar", "id": "confirm" },
      { "body": "Cancelar", "id": "cancel" }
    ]
  }'
```

### Test 3: Maximum Buttons (3)

```bash
curl -X POST http://localhost:3000/whatsapp/send-buttons \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "body": "Seleccione un servicio:",
    "buttons": [
      { "body": "Instalación" },
      { "body": "Reparación" },
      { "body": "Consulta" }
    ]
  }'
```

### Test 4: Button Validation Tests

**Too Many Buttons (should fail):**
```bash
curl -X POST http://localhost:3000/whatsapp/send-buttons \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "body": "Test",
    "buttons": [
      { "body": "Button 1" },
      { "body": "Button 2" },
      { "body": "Button 3" },
      { "body": "Button 4" }
    ]
  }'
```

**Button Text Too Long (should fail):**
```bash
curl -X POST http://localhost:3000/whatsapp/send-buttons \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "3001234567",
    "body": "Test",
    "buttons": [
      { "body": "This is a very long button text that exceeds twenty characters" }
    ]
  }'
```

---

## Test Webhooks

### Test 1: Register Webhook

```bash
curl -X POST http://localhost:3000/whatsapp/webhooks \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "name": "Test Webhook",
    "url": "https://webhook.site/your-unique-id",
    "secret": "my-secret-key",
    "events": ["message_received", "message_status"]
  }'
```

**Note:** Use https://webhook.site to get a free webhook URL for testing.

### Test 2: List All Webhooks

```bash
curl -X GET http://localhost:3000/whatsapp/webhooks \
  -H "X-API-Key: your-api-key"
```

### Test 3: Test Webhook

```bash
curl -X POST http://localhost:3000/whatsapp/webhooks/WEBHOOK_ID/test \
  -H "X-API-Key: your-api-key"
```

### Test 4: Update Webhook

```bash
curl -X PUT http://localhost:3000/whatsapp/webhooks/WEBHOOK_ID \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "active": false
  }'
```

### Test 5: Delete Webhook

```bash
curl -X DELETE http://localhost:3000/whatsapp/webhooks/WEBHOOK_ID \
  -H "X-API-Key: your-api-key"
```

### Test 6: Webhook Receiver (Express)

Create a test webhook receiver:

```javascript
const express = require('express');
const crypto = require('crypto');

const app = express();
app.use(express.json());

app.post('/webhook', (req, res) => {
  console.log('Webhook received:', req.body);

  // Verify signature if secret is used
  const signature = req.headers['x-webhook-signature'];
  if (signature) {
    const secret = 'my-secret-key';
    const payload = JSON.stringify(req.body);
    const expectedSignature = crypto
      .createHmac('sha256', secret)
      .update(payload)
      .digest('hex');

    if (signature === expectedSignature) {
      console.log('Signature verified!');
    } else {
      console.log('Invalid signature!');
      return res.status(401).json({ error: 'Invalid signature' });
    }
  }

  res.json({ success: true });
});

app.listen(3001, () => {
  console.log('Webhook test server running on port 3001');
});
```

---

## Test Admin Panel

### Test 1: Access Admin Panel

1. Open browser: `http://localhost:3000/admin`
2. Enter your API Key when prompted
3. Verify connection status is displayed

### Test 2: View Statistics

1. Navigate to admin panel
2. Check "Today's Statistics" section
3. Verify message counts are accurate
4. Compare with API stats endpoint

### Test 3: Send Test Message

1. Go to "Send Message" tab
2. Enter phone number
3. Select template
4. Fill in template fields
5. Click "Send Message"
6. Verify success message

### Test 4: Send Media via Admin Panel

1. Go to "Send Media" tab
2. Enter phone number
3. Select "Image" or "Document"
4. Choose "URL" and enter: `https://picsum.photos/400/300`
5. Add caption (optional)
6. Click "Send Media"
7. Verify message sent

### Test 5: Create Button Message

1. Go to "Send Buttons" tab
2. Enter phone number
3. Enter message body
4. Add 2-3 buttons
5. Click "Send Buttons"
6. Verify button message sent

### Test 6: Manage Webhooks

1. Go to "Webhooks" tab
2. Click "Add Webhook"
3. Fill in webhook details
4. Click "Register Webhook"
5. Test webhook with "Test" button
6. Disable/enable webhook
7. Delete webhook

### Test 7: View Logs

1. Go to "Logs" tab
2. Select today's date
3. Click "Load Logs"
4. Verify log summary appears

---

## Troubleshooting

### Issue: Images Not Sending

**Check:**
```bash
# Verify image format
curl -I https://your-image-url.jpg

# Test with known working image
curl -X POST http://localhost:3000/whatsapp/send-image \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "phoneNumber": "YOUR_NUMBER",
    "image": {
      "type": "url",
      "url": "https://picsum.photos/200/300"
    }
  }'
```

### Issue: Buttons Not Appearing

**Verify:**
- WhatsApp version is up to date
- Button text is ≤ 20 characters
- Maximum 3 buttons
- Body text is provided

### Issue: Webhooks Not Firing

**Debug:**
```bash
# Check webhook list
curl http://localhost:3000/whatsapp/webhooks \
  -H "X-API-Key: your-api-key"

# Test webhook
curl -X POST http://localhost:3000/whatsapp/webhooks/WEBHOOK_ID/test \
  -H "X-API-Key: your-api-key"

# Check server logs
# Look for webhook trigger messages in console
```

### Issue: Admin Panel Not Loading

**Solutions:**
1. Clear browser cache
2. Check browser console for errors
3. Verify API key is correct
4. Try incognito/private mode
5. Check server is running: `curl http://localhost:3000/health`

### Issue: WhatsApp Not Ready

**Steps:**
1. Check status: `curl http://localhost:3000/whatsapp/status`
2. Look for QR code in console
3. Scan QR with WhatsApp
4. Wait for "WhatsApp API LISTA" message
5. Check admin panel for QR code

---

## Performance Testing

### Test Bulk Sending

```javascript
async function testBulkSending() {
  const numbers = ['3001234567', '3009876543', '3005551234'];

  for (const number of numbers) {
    try {
      const response = await axios.post(
        'http://localhost:3000/whatsapp/send-image',
        {
          phoneNumber: number,
          image: {
            type: 'url',
            url: 'https://picsum.photos/400/300'
          },
          caption: 'Bulk test message'
        },
        {
          headers: { 'X-API-Key': 'your-api-key' }
        }
      );

      console.log(`Sent to ${number}:`, response.data);

      // Wait 3 seconds between messages
      await new Promise(resolve => setTimeout(resolve, 3000));

    } catch (error) {
      console.error(`Failed for ${number}:`, error.message);
    }
  }
}

testBulkSending();
```

### Monitor Statistics

```bash
# Get today's stats
curl http://localhost:3000/whatsapp/stats \
  -H "X-API-Key: your-api-key"

# Get specific date
curl "http://localhost:3000/whatsapp/stats?date=2025-11-23" \
  -H "X-API-Key: your-api-key"
```

---

## Test Checklist

- [ ] Send image from URL
- [ ] Send image from base64
- [ ] Send PDF document
- [ ] Send XLSX document
- [ ] Send button message (2 buttons)
- [ ] Send button message (3 buttons)
- [ ] Register webhook
- [ ] Test webhook
- [ ] Update webhook
- [ ] Delete webhook
- [ ] Access admin panel
- [ ] Send message via admin panel
- [ ] Send media via admin panel
- [ ] Create buttons via admin panel
- [ ] Manage webhooks via admin panel
- [ ] View logs via admin panel
- [ ] Test validation errors
- [ ] Test authentication
- [ ] Check statistics

---

## Complete Test Script

```bash
#!/bin/bash

API_KEY="your-api-key"
PHONE="3001234567"
BASE_URL="http://localhost:3000"

echo "Testing WhatsApp API v2.0..."

# Test 1: Send Image
echo "Test 1: Send Image"
curl -s -X POST $BASE_URL/whatsapp/send-image \
  -H "Content-Type: application/json" \
  -H "X-API-Key: $API_KEY" \
  -d "{\"phoneNumber\":\"$PHONE\",\"image\":{\"type\":\"url\",\"url\":\"https://picsum.photos/400/300\"}}" \
  | jq

sleep 5

# Test 2: Send Buttons
echo "Test 2: Send Buttons"
curl -s -X POST $BASE_URL/whatsapp/send-buttons \
  -H "Content-Type: application/json" \
  -H "X-API-Key: $API_KEY" \
  -d "{\"phoneNumber\":\"$PHONE\",\"body\":\"Test buttons\",\"buttons\":[{\"body\":\"Yes\"},{\"body\":\"No\"}]}" \
  | jq

sleep 5

# Test 3: Check Stats
echo "Test 3: Check Stats"
curl -s $BASE_URL/whatsapp/stats \
  -H "X-API-Key: $API_KEY" \
  | jq

echo "All tests completed!"
```

---

**Version:** 2.0.0
**Last Updated:** 2025-11-23
