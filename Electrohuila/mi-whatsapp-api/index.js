/**
 * WhatsApp API - Production Ready
 * API para envío de mensajes de WhatsApp usando templates
 */

const { Client, LocalAuth } = require('whatsapp-web.js');
const qrcode = require('qrcode-terminal');
const QRCode = require('qrcode');
const express = require('express');
const cors = require('cors');
const path = require('path');
require('dotenv').config();

const whatsappRoutes = require('./routes/whatsapp');
const { setupWebhookListeners } = require('./webhooks/handler');

const app = express();
const PORT = process.env.PORT || 3001;

// Cliente de WhatsApp (global para que las routes lo usen)
const client = new Client({
    authStrategy: new LocalAuth(),
    puppeteer: {
        headless: true,
        args: [
            '--no-sandbox',
            '--disable-setuid-sandbox',
            '--disable-dev-shm-usage',
            '--disable-accelerated-2d-canvas',
            '--no-first-run',
            '--no-zygote',
            '--disable-gpu'
        ]
    }
});

// Store current QR code for admin panel
let currentQRCode = null;

// Event handlers del cliente de WhatsApp
client.on('qr', async (qr) => {
    console.log('\n' + '='.repeat(50));
    console.log('📱 ESCANEA ESTE QR CON TU WHATSAPP');
    console.log('='.repeat(50) + '\n');
    qrcode.generate(qr, { small: true });
    console.log('\n' + '='.repeat(50));
    console.log('Esperando escaneo del código QR...');
    console.log('='.repeat(50) + '\n');

    // Generate QR code as data URL for admin panel
    try {
        currentQRCode = await QRCode.toDataURL(qr);
        global.currentQRCode = currentQRCode;
    } catch (err) {
        console.error('Error generating QR code:', err);
    }
});

client.on('ready', () => {
    console.log('\n' + '='.repeat(50));
    console.log('✅ WhatsApp API LISTA');
    console.log('='.repeat(50));
    console.log(`📞 Conectado como: ${client.info.wid.user}`);
    console.log(`📱 Plataforma: ${client.info.platform}`);
    console.log('='.repeat(50) + '\n');

    // Clear QR code when ready
    currentQRCode = null;
    global.currentQRCode = null;
});

client.on('authenticated', () => {
    console.log('🔐 Autenticación exitosa');
});

client.on('auth_failure', msg => {
    console.error('❌ Fallo de autenticación:', msg);
});

client.on('disconnected', (reason) => {
    console.log('⚠️ Cliente desconectado:', reason);
});

client.on('message', async msg => {
    // Opcional: Log de mensajes recibidos
    if (process.env.LOG_INCOMING_MESSAGES === 'true') {
        console.log(`📨 Mensaje recibido de ${msg.from}: ${msg.body.substring(0, 50)}...`);
    }
});

// Inicializar cliente
console.log('🚀 Inicializando cliente de WhatsApp...');
client.initialize();

// Setup webhook listeners
setupWebhookListeners(client);

// Exportar cliente para que lo usen las routes
global.whatsappClient = client;

// Middleware
app.use(cors());
app.use(express.json({ limit: '50mb' })); // Increase limit for base64 images
app.use(express.urlencoded({ extended: true, limit: '50mb' }));

// Serve static files for admin panel
app.use('/admin', express.static(path.join(__dirname, 'public', 'admin')));

// Middleware para logging de requests
app.use((req, res, next) => {
    const timestamp = new Date().toISOString();
    console.log(`[${timestamp}] ${req.method} ${req.path}`);
    next();
});

// Health check (sin autenticación)
app.get('/health', (req, res) => {
    res.json({
        status: 'ok',
        whatsappReady: client.info ? true : false,
        timestamp: new Date().toISOString(),
        uptime: process.uptime()
    });
});

// Root endpoint
app.get('/', (req, res) => {
    res.json({
        name: 'WhatsApp API - Electrohuila',
        version: '2.0.0',
        status: 'running',
        endpoints: {
            health: 'GET /health',
            admin: 'GET /admin',
            status: 'GET /whatsapp/status',
            templates: 'GET /whatsapp/templates',
            stats: 'GET /whatsapp/stats?date=YYYY-MM-DD',
            logs: 'GET /whatsapp/logs',
            confirmation: 'POST /whatsapp/appointment-confirmation',
            reminder: 'POST /whatsapp/appointment-reminder',
            cancellation: 'POST /whatsapp/appointment-cancellation',
            sendImage: 'POST /whatsapp/send-image',
            sendDocument: 'POST /whatsapp/send-document',
            sendButtons: 'POST /whatsapp/send-buttons',
            webhooks: 'GET/POST /whatsapp/webhooks'
        },
        documentation: 'Ver README.md para más información'
    });
});

// QR Code endpoint for admin panel
app.get('/admin/qr', (req, res) => {
    if (global.currentQRCode) {
        res.json({ qr: global.currentQRCode });
    } else {
        res.status(404).json({ error: 'No QR code available' });
    }
});

// Routes de WhatsApp
app.use('/whatsapp', whatsappRoutes);

// Endpoint legacy (mantener compatibilidad)
// Este endpoint quedará obsoleto, usar los nuevos endpoints con templates
app.post('/send', async (req, res) => {
    console.warn('⚠️ Endpoint /send está obsoleto. Usa /whatsapp/appointment-* en su lugar');

    const { telefono, mensaje } = req.body;

    if (!telefono || !mensaje) {
        return res.status(400).json({
            ok: false,
            error: 'telefono y mensaje son requeridos'
        });
    }

    try {
        if (!client.info) {
            return res.status(503).json({
                ok: false,
                error: 'WhatsApp no está listo'
            });
        }

        await client.sendMessage(telefono + '@c.us', mensaje);
        res.json({ ok: true, enviado: true });
    } catch (error) {
        res.status(500).json({ ok: false, error: error.message });
    }
});

// Manejo de errores 404
app.use((req, res) => {
    res.status(404).json({
        success: false,
        error: 'Endpoint no encontrado',
        path: req.path
    });
});

// Manejo de errores global
app.use((err, req, res, next) => {
    console.error('❌ Error no manejado:', err);
    res.status(500).json({
        success: false,
        error: 'Error interno del servidor'
    });
});

// Iniciar servidor
app.listen(PORT, () => {
    console.log('\n' + '='.repeat(50));
    console.log('🚀 WHATSAPP API CORRIENDO');
    console.log('='.repeat(50));
    console.log(`📡 Puerto: ${PORT}`);
    console.log(`🔑 API Key configurada: ${process.env.WHATSAPP_API_KEY ? 'Sí' : 'No (¡CONFIGÚRALA!)'}`);
    console.log(`📋 Endpoints disponibles en: http://localhost:${PORT}/`);
    console.log('='.repeat(50) + '\n');
});

// Manejo de señales de terminación
process.on('SIGINT', async () => {
    console.log('\n⚠️ Cerrando servidor...');
    await client.destroy();
    process.exit(0);
});

process.on('SIGTERM', async () => {
    console.log('\n⚠️ Cerrando servidor...');
    await client.destroy();
    process.exit(0);
});
