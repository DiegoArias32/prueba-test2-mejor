# Admin Panel User Guide

## Overview

The WhatsApp API Admin Panel is a web-based interface for managing and monitoring your WhatsApp messaging service. It provides real-time status updates, testing capabilities, webhook management, and comprehensive logging.

**Access URL:** `http://localhost:3001/admin` (or your configured domain)

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Dashboard Overview](#dashboard-overview)
3. [Connection Status](#connection-status)
4. [Statistics](#statistics)
5. [Send Messages](#send-messages)
6. [Send Media](#send-media)
7. [Interactive Buttons](#interactive-buttons)
8. [Webhook Management](#webhook-management)
9. [Message Logs](#message-logs)
10. [Troubleshooting](#troubleshooting)

---

## Getting Started

### First Time Access

1. Open your browser and navigate to `http://localhost:3001/admin`
2. You'll be prompted to enter your API Key
3. Enter the API Key configured in your `.env` file
4. The key is securely stored in your browser's localStorage
5. You're now ready to use the admin panel!

### Changing API Key

If you need to change the API Key:
1. Open browser developer console (F12)
2. Go to Application/Storage tab
3. Find localStorage
4. Delete the `whatsapp_api_key` entry
5. Refresh the page and enter new key

### Browser Requirements

- Modern browser (Chrome, Firefox, Edge, Safari)
- JavaScript enabled
- LocalStorage enabled
- Internet connection for API calls

---

## Dashboard Overview

The admin panel is divided into several sections:

### Header
- **Connection Status Badge**: Shows real-time WhatsApp connection state
  - Green (Ready): WhatsApp is connected and ready
  - Yellow (Initializing): Starting up or reconnecting
  - Red (Disconnected): Not connected
- **Current Time**: Live clock display

### Main Sections
1. **Connection Status** - WhatsApp client information
2. **Statistics** - Today's message metrics
3. **Tabs** - Different functional areas:
   - Send Message
   - Send Media
   - Send Buttons
   - Webhooks
   - Logs

---

## Connection Status

### Status Information

The status panel displays:
- **Status**: Current connection state (Ready/Initializing/Disconnected)
- **Phone**: Connected WhatsApp phone number
- **Platform**: Device platform (Android/iOS)
- **Uptime**: Server uptime in hours and minutes

### QR Code

If WhatsApp is not connected:
1. A QR code will appear in the status panel
2. Open WhatsApp on your phone
3. Go to: Settings > Linked Devices > Link a Device
4. Scan the displayed QR code
5. Wait for connection confirmation

**Note:** The QR code refreshes automatically every 20 seconds if not scanned.

### Auto-Refresh

The connection status automatically refreshes every 10 seconds to keep information current.

---

## Statistics

### Today's Statistics

Real-time metrics for the current day:

- **Total Messages**: All messages sent today
- **Successful**: Successfully delivered messages
- **Failed**: Messages that failed to send
- **Success Rate**: Percentage of successful deliveries

### Statistics Updates

- Auto-refresh every 30 seconds
- Click any tab to manually refresh
- Stats reset at midnight (server time)

### Understanding Metrics

- High failure rate (>5%) may indicate connectivity issues
- Check logs for specific error details
- Normal success rate: >95%

---

## Send Messages

### Template Messages

Send messages using predefined templates for appointments.

#### Available Templates

1. **Confirmation** - Appointment confirmation
2. **Reminder** - Appointment reminder
3. **Cancellation** - Appointment cancellation

#### How to Send

1. Navigate to "Send Message" tab
2. Enter phone number (10 digits, Colombian format)
   - Example: `3001234567`
3. Select a template from dropdown
4. Fill in required fields (marked with *)
5. Click "Send Message"
6. Wait for confirmation

#### Field Descriptions

**Confirmation Template:**
- Name: Customer's full name
- Date: Appointment date
- Time: Appointment time
- Service: Type of service
- Address: Service location (optional)
- Notes: Additional information (optional)

**Reminder Template:**
- Name: Customer's full name
- Date: Appointment date
- Time: Appointment time
- Service: Type of service
- Address: Service location (optional)
- Advance Notice: When to remind (optional)

**Cancellation Template:**
- Name: Customer's full name
- Date: Original appointment date
- Time: Original appointment time
- Reason: Cancellation reason (optional)
- Reschedule: Check if rescheduling (optional)

#### Success Response

After sending, you'll see:
- Green success message
- Message ID for tracking
- Timestamp of sending
- Duration (response time)
- Retry count (if any)

---

## Send Media

### Sending Images and Documents

Send multimedia files to WhatsApp contacts.

#### Supported Image Formats
- JPEG/JPG
- PNG
- WebP
- GIF

**Maximum Size:** 5MB

#### Supported Document Formats
- PDF
- Word (DOC, DOCX)
- Excel (XLS, XLSX)
- PowerPoint (PPT, PPTX)
- Text (TXT)
- CSV

**Maximum Size:** 100MB

### How to Send Media

1. Navigate to "Send Media" tab
2. Enter phone number
3. Select media type (Image or Document)
4. Choose source:

#### Option A: From URL
1. Select "URL" as source
2. Paste the public URL to your file
3. File will be downloaded and sent

**URL Requirements:**
- Must be publicly accessible
- HTTPS recommended
- No authentication required
- Direct link to file

#### Option B: Upload File
1. Select "Upload File" as source
2. Click "Choose File"
3. Select file from your computer
4. File is converted to base64 and sent

### Adding Caption

- Optional text message with media
- Max 1000 characters for images
- Supports emojis and formatting

### Tips

- Test URLs first in browser
- Optimize images before sending (smaller = faster)
- PDF is best for documents
- Large files take longer to send

---

## Interactive Buttons

### Button Messages

Send messages with clickable quick-reply buttons.

#### Button Specifications
- Minimum: 1 button
- Maximum: 3 buttons
- Max text length: 20 characters per button
- Plain text only (no emojis in buttons)

### How to Send

1. Navigate to "Send Buttons" tab
2. Enter phone number
3. Fill in optional title
4. Enter message body (required)
5. Add optional footer
6. Configure buttons:
   - Button text appears on the button
   - Click "Add Button" for more (max 3)
7. Click "Send Buttons"

### Use Cases

**Customer Service:**
```
Body: "How can we help you today?"
Buttons:
- "Information"
- "Support"
- "Schedule"
Footer: "Electrohuila - Customer Service"
```

**Appointment Confirmation:**
```
Body: "Confirm your appointment for Nov 25, 10:00 AM"
Buttons:
- "Confirm"
- "Reschedule"
- "Cancel"
```

**Survey/Feedback:**
```
Body: "Rate our service"
Buttons:
- "Excellent"
- "Good"
- "Needs Work"
```

### Button Responses

When a user clicks a button:
1. Their response is sent as a regular message
2. You receive the button ID via webhook (if configured)
3. You can automate responses based on button clicks

---

## Webhook Management

### What are Webhooks?

Webhooks allow your system to receive real-time notifications when events occur, enabling automation and integration.

### Event Types

- **message_received**: Incoming messages from users
- **message_status**: Status updates (sent, delivered, read)
- **button_clicked**: User clicks an interactive button
- **qr_updated**: QR code refreshed (for connection)
- **ready**: WhatsApp client connected
- **disconnected**: WhatsApp client disconnected

### Registering a Webhook

1. Navigate to "Webhooks" tab
2. Click "Add Webhook"
3. Fill in the form:
   - **Name**: Descriptive name (e.g., "Production Server")
   - **URL**: Your webhook endpoint (must be HTTPS in production)
   - **Secret**: Optional key for signature verification
   - **Events**: Check events you want to receive
4. Click "Register Webhook"
5. Webhook is now active!

### Webhook Requirements

**URL Requirements:**
- Must be publicly accessible
- HTTPS recommended for production
- Must respond with HTTP 200 within 10 seconds
- Should process requests asynchronously

**Security:**
- Use secret for signature verification
- Validate all incoming data
- Implement rate limiting
- Log all webhook calls

### Managing Webhooks

#### View Webhooks
All registered webhooks are listed with:
- Name and URL
- Active status
- Registered events
- Success/failure counts
- Last triggered timestamp

#### Test Webhook
1. Click "Test" button on any webhook
2. Test payload is sent to your URL
3. Check if your server receives it
4. Verify response is correct

#### Enable/Disable
- Click "Enable" or "Disable" to toggle
- Disabled webhooks don't receive events
- Useful for maintenance

#### Delete Webhook
1. Click "Delete" button
2. Confirm deletion
3. Webhook is permanently removed

### Webhook Payload Example

```json
{
  "event": "message_received",
  "timestamp": "2025-11-23T10:00:00.000Z",
  "data": {
    "id": "message-id-here",
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

If you provided a secret, verify the signature:

```javascript
// Your webhook endpoint
app.post('/webhook', (req, res) => {
  const signature = req.headers['x-webhook-signature'];
  const payload = JSON.stringify(req.body);
  const secret = 'your-webhook-secret';

  const crypto = require('crypto');
  const expectedSignature = crypto
    .createHmac('sha256', secret)
    .update(payload)
    .digest('hex');

  if (signature !== expectedSignature) {
    return res.status(401).json({ error: 'Invalid signature' });
  }

  // Process webhook safely
  res.json({ success: true });
});
```

### Troubleshooting Webhooks

**Webhook not receiving events:**
1. Check URL is correct and accessible
2. Verify selected events
3. Ensure webhook is active
4. Check your server logs
5. Test webhook manually

**High failure count:**
1. Check server is responding
2. Verify response time < 10s
3. Return HTTP 200 status
4. Check server error logs

---

## Message Logs

### Viewing Logs

Access historical message data:

1. Navigate to "Logs" tab
2. Select date using date picker (defaults to today)
3. Click "Load Logs"
4. View summary and details

### Log Information

**Summary Statistics:**
- Total messages for selected date
- Successful deliveries
- Failed attempts
- Success rate percentage

**Individual Entries:**
- Timestamp
- Phone number
- Template used
- Success/failure status
- Error message (if failed)
- Retry count
- Message length

### Date Range

- Logs are stored per day
- Select any historical date
- Logs older than 30 days may be archived
- Default retention: 30 days

### Understanding Logs

**Success Indicators:**
- Green border on log entry
- "Success: true" status
- Message ID present
- Low retry count (0-1)

**Failure Indicators:**
- Red border on log entry
- "Success: false" status
- Error message displayed
- High retry count (2-3)

**Common Errors:**
- "Client not ready": WhatsApp disconnected
- "Timeout exceeded": Network issues
- "Invalid phone": Phone number validation failed
- "Media too large": File size exceeds limit

### Export Logs

Logs are stored as JSON files in the `logs/` directory:
- Location: `logs/whatsapp-YYYY-MM-DD.log`
- Format: JSON lines (one entry per line)
- Can be processed with scripts
- Backup regularly

---

## Troubleshooting

### Common Issues

#### Admin Panel Won't Load

**Problem:** Page shows error or doesn't load

**Solutions:**
1. Check server is running (`http://localhost:3001/health`)
2. Verify port 3001 is not blocked
3. Clear browser cache and reload
4. Try different browser
5. Check server console for errors

#### Invalid API Key

**Problem:** "API key invalid" error on requests

**Solutions:**
1. Verify API key matches `.env` file
2. Clear localStorage and re-enter key
3. Check for extra spaces in key
4. Restart server after changing key
5. Generate new key if compromised

#### QR Code Not Appearing

**Problem:** QR code section not showing

**Solutions:**
1. Wait 10-20 seconds after server start
2. Check WhatsApp client status in console
3. Restart server
4. Clear browser cache
5. Check server logs for Puppeteer errors

#### Messages Not Sending

**Problem:** Messages fail to send

**Solutions:**
1. Check WhatsApp connection status (should be green/ready)
2. Verify phone number format (10 digits)
3. Check message template has all required fields
4. Review logs for specific error
5. Test with different phone number
6. Restart WhatsApp client

#### Media Upload Fails

**Problem:** Image/document won't send

**Solutions:**
1. Check file size (5MB images, 100MB docs)
2. Verify file format is supported
3. Test with smaller file
4. Try URL method instead
5. Check file is not corrupted
6. Ensure good internet connection

#### Webhooks Not Working

**Problem:** Webhook not receiving events

**Solutions:**
1. Test webhook URL in browser
2. Verify URL is publicly accessible
3. Check selected events are correct
4. Enable webhook if disabled
5. Review webhook server logs
6. Test with webhook.site first
7. Verify signature if using secret

#### Statistics Not Updating

**Problem:** Numbers seem stuck or wrong

**Solutions:**
1. Wait for auto-refresh (30 seconds)
2. Manually reload page
3. Check date/time on server
4. Review logs for actual messages
5. Restart server if persistent

### Performance Tips

**For Better Performance:**
1. Keep media files small and optimized
2. Don't send too many messages at once
3. Use webhooks for automation
4. Monitor statistics regularly
5. Clean old logs periodically
6. Use templates for consistency
7. Test in development first

**Browser Performance:**
1. Close unused tabs
2. Clear browser cache regularly
3. Use modern browser
4. Disable unnecessary extensions
5. Keep browser updated

### Getting Help

**Self-Service:**
1. Check server status: `/health` endpoint
2. Review logs in Logs tab
3. Check console output on server
4. Review API documentation
5. Test with curl/Postman

**Contact Support:**
- Provide error messages
- Include relevant log entries
- Describe steps to reproduce
- Note server/browser versions
- Include timestamps

---

## Security Best Practices

### API Key Protection

- Never share your API key
- Don't commit to version control
- Rotate keys periodically
- Use environment variables
- Monitor for unauthorized access

### Webhook Security

- Always use HTTPS in production
- Implement signature verification
- Validate all incoming data
- Rate limit webhook endpoints
- Log all webhook calls
- Monitor for suspicious activity

### Browser Security

- Use HTTPS for admin panel
- Don't access from public computers
- Clear browser data when done
- Use strong admin passwords
- Keep browser updated
- Enable 2FA where possible

### Network Security

- Use firewall to restrict access
- Consider VPN for remote access
- Monitor server logs
- Regular security updates
- Backup configurations

---

## Keyboard Shortcuts

- **Ctrl/Cmd + R**: Reload page and refresh data
- **Tab**: Navigate form fields
- **Enter**: Submit active form
- **Esc**: Close modals/dialogs
- **F5**: Refresh page
- **F12**: Open developer console

---

## Mobile Access

The admin panel is responsive and works on mobile devices:

**Tablet:**
- Full functionality
- Touch-friendly interface
- Landscape recommended

**Phone:**
- All features available
- Some layouts adapt
- Portrait or landscape
- May require scrolling

---

## Updates and Maintenance

### Checking Version

Current version: **v2.0.0**

Check footer of admin panel for version info.

### Update Notifications

Future updates will include:
- In-panel update notifications
- Changelog display
- Feature announcements

### Maintenance Mode

During server maintenance:
- Connection status will show disconnected
- Save work before updates
- Wait for "Ready" status
- Refresh page after updates

---

## Frequently Asked Questions

**Q: Can multiple users access the admin panel simultaneously?**
A: Yes, but each needs their own API key stored in their browser.

**Q: Can I customize the admin panel?**
A: The HTML/CSS files are in `public/admin/` and can be modified.

**Q: Are messages stored in the admin panel?**
A: No, messages are logged to files. The panel reads from those logs.

**Q: Can I send messages to international numbers?**
A: Yes, but phone validation is optimized for Colombian numbers. Adjust validation as needed.

**Q: What happens if I close the admin panel?**
A: The API continues running. The panel is just a UI for management.

**Q: Can I schedule messages?**
A: Not currently. Implement scheduling in your application layer.

**Q: Is there a mobile app?**
A: No, but the admin panel works well on mobile browsers.

---

## Advanced Features

### Browser Developer Tools

Use for debugging:
1. Press F12 to open developer tools
2. Console tab: View JavaScript logs
3. Network tab: Monitor API calls
4. Application tab: Check localStorage

### Custom Integration

The admin panel uses standard REST API calls. You can:
- Build your own UI
- Integrate into existing systems
- Create mobile apps
- Automate with scripts

### API Testing

Use the admin panel to test before integrating:
1. Test phone number formats
2. Verify template rendering
3. Test media uploads
4. Validate webhook payloads
5. Check error handling

---

## Support and Resources

**Documentation:**
- API Documentation: `API_DOCUMENTATION.md`
- Testing Guide: `TESTING_GUIDE.md`
- README: `README.md`

**Endpoints:**
- API Base: `http://localhost:3001/whatsapp`
- Health Check: `http://localhost:3001/health`
- Admin Panel: `http://localhost:3001/admin`

**Log Files:**
- Location: `logs/whatsapp-YYYY-MM-DD.log`
- Format: JSON (one entry per line)
- Retention: 30 days default

---

**Admin Panel Guide Version:** 2.0.0
**Last Updated:** 2025-11-23
**For:** WhatsApp API - Electrohuila
