/**
 * Routes de la API de WhatsApp
 * Endpoints especializados para envío de mensajes con templates
 */

const express = require('express');
const router = express.Router();
const { MessageMedia } = require('whatsapp-web.js');
const axios = require('axios');

const { authenticateApiKey } = require('../middleware/auth');
const { generateMessage } = require('../templates/whatsappTemplates');
const { validatePhoneNumber } = require('../utils/phoneValidator');
const { retryMessageSend } = require('../utils/retryHandler');
const { logger } = require('../utils/logger');
const {
    validateImageRequest,
    validateDocumentRequest,
    validateButtonRequest,
    validateWebhookConfig
} = require('../middleware/validation');
const { webhookManager } = require('../webhooks/handler');

// Aplicar autenticación a todas las routes excepto /status
router.use((req, res, next) => {
    // Permitir /status sin autenticación
    if (req.path === '/status') {
        return next();
    }
    authenticateApiKey(req, res, next);
});

/**
 * Función auxiliar para enviar un mensaje con template
 * @param {string} templateName - Nombre del template
 * @param {object} req - Request de Express
 * @param {object} res - Response de Express
 */
async function sendTemplateMessage(templateName, req, res) {
    const startTime = Date.now();
    let retryCount = 0;

    try {
        const { phoneNumber, data } = req.body;

        // 1. Validar que se proporcionaron los datos
        if (!phoneNumber) {
            return res.status(400).json({
                success: false,
                error: 'phoneNumber es requerido'
            });
        }

        if (!data) {
            return res.status(400).json({
                success: false,
                error: 'data es requerido'
            });
        }

        // 2. Validar teléfono
        const phoneValidation = validatePhoneNumber(phoneNumber);
        if (!phoneValidation.valid) {
            logger.logMessage({
                phoneNumber,
                template: templateName,
                success: false,
                error: phoneValidation.error
            });

            return res.status(400).json({
                success: false,
                error: phoneValidation.error
            });
        }

        // 3. Generar mensaje con template
        const messageResult = generateMessage(templateName, data);
        if (!messageResult.success) {
            logger.logMessage({
                phoneNumber: phoneValidation.normalized,
                phoneFormatted: phoneValidation.formatted,
                template: templateName,
                success: false,
                error: messageResult.error
            });

            return res.status(400).json({
                success: false,
                error: messageResult.error,
                missingFields: messageResult.missingFields
            });
        }

        // 4. Verificar que el cliente de WhatsApp está listo
        if (!global.whatsappClient || !global.whatsappClient.info) {
            logger.logMessage({
                phoneNumber: phoneValidation.normalized,
                phoneFormatted: phoneValidation.formatted,
                template: templateName,
                success: false,
                error: 'Cliente de WhatsApp no está listo'
            });

            return res.status(503).json({
                success: false,
                error: 'Servicio de WhatsApp no disponible. Por favor intenta más tarde.'
            });
        }

        // 5. Enviar mensaje con reintentos
        let sendResult;
        try {
            sendResult = await retryMessageSend(
                async () => {
                    retryCount++;
                    return await global.whatsappClient.sendMessage(
                        phoneValidation.formatted,
                        messageResult.message
                    );
                },
                {
                    maxRetries: 3,
                    initialDelay: 2000,
                    phoneNumber: phoneValidation.display
                }
            );
        } catch (sendError) {
            logger.logMessage({
                phoneNumber: phoneValidation.normalized,
                phoneFormatted: phoneValidation.formatted,
                template: templateName,
                success: false,
                error: sendError.message,
                retries: retryCount - 1,
                messageLength: messageResult.message.length,
                templateData: data
            });

            return res.status(500).json({
                success: false,
                error: `Error enviando mensaje: ${sendError.message}`,
                retries: retryCount - 1
            });
        }

        // 6. Log exitoso
        logger.logMessage({
            phoneNumber: phoneValidation.normalized,
            phoneFormatted: phoneValidation.formatted,
            template: templateName,
            success: true,
            retries: retryCount - 1,
            messageLength: messageResult.message.length,
            templateData: data
        });

        // 7. Responder con éxito
        const duration = Date.now() - startTime;
        return res.json({
            success: true,
            message: 'Mensaje enviado correctamente',
            data: {
                phoneNumber: phoneValidation.display,
                template: templateName,
                messageId: sendResult.id?.id || null,
                timestamp: new Date().toISOString(),
                duration: `${duration}ms`,
                retries: retryCount - 1
            }
        });

    } catch (error) {
        console.error(`❌ Error en sendTemplateMessage: ${error.message}`);

        logger.logMessage({
            phoneNumber: req.body.phoneNumber,
            template: templateName,
            success: false,
            error: error.message,
            retries: retryCount
        });

        return res.status(500).json({
            success: false,
            error: 'Error interno del servidor'
        });
    }
}

/**
 * POST /whatsapp/appointment-confirmation
 * Envía confirmación de cita con imagen del logo y botones interactivos
 * Body: {
 *   phoneNumber,
 *   data: {
 *     numeroCita, nombreCliente, clienteId, fecha, hora,
 *     ubicacion, direccion?, profesional?, tipoCita?,
 *     direccionCliente?, telefono?, observaciones?, qrUrl?
 *   }
 * }
 */
router.post('/appointment-confirmation', async (req, res) => {
    const startTime = Date.now();
    let retryCount = 0;

    try {
        const { phoneNumber, data } = req.body;

        // 1. Validar datos
        if (!phoneNumber || !data) {
            return res.status(400).json({
                success: false,
                error: 'phoneNumber y data son requeridos'
            });
        }

        // 2. Validar teléfono
        const phoneValidation = validatePhoneNumber(phoneNumber);
        if (!phoneValidation.valid) {
            return res.status(400).json({
                success: false,
                error: phoneValidation.error
            });
        }

        // 3. Verificar cliente de WhatsApp
        if (!global.whatsappClient || !global.whatsappClient.info) {
            return res.status(503).json({
                success: false,
                error: 'Servicio de WhatsApp no disponible'
            });
        }

        // 4. Descargar logo de ElectroHuila
        const logoUrl = 'https://www.electrohuila.com.co/wp-content/uploads/2024/07/cropped-logo-nuevo-eh.png.webp';
        let logoMedia;

        try {
            const logoResponse = await axios.get(logoUrl, {
                responseType: 'arraybuffer',
                timeout: 10000
            });
            const logoBase64 = Buffer.from(logoResponse.data).toString('base64');
            logoMedia = new MessageMedia('image/webp', logoBase64, 'electrohuila-logo.webp');
        } catch (logoError) {
            console.warn('No se pudo descargar el logo, enviando sin imagen:', logoError.message);
        }

        // 5. Generar mensaje con template mejorado
        const messageResult = generateMessage('confirmacion_cita', data);
        if (!messageResult.success) {
            return res.status(400).json({
                success: false,
                error: messageResult.error,
                missingFields: messageResult.missingFields
            });
        }

        // 6. Enviar imagen con caption (si se descargó el logo)
        if (logoMedia) {
            await retryMessageSend(
                async () => {
                    retryCount++;
                    return await global.whatsappClient.sendMessage(
                        phoneValidation.formatted,
                        logoMedia,
                        { caption: messageResult.message }
                    );
                },
                {
                    maxRetries: 3,
                    initialDelay: 2000,
                    phoneNumber: phoneValidation.display
                }
            );
        } else {
            // Si no hay logo, enviar mensaje de texto normal
            await retryMessageSend(
                async () => {
                    retryCount++;
                    return await global.whatsappClient.sendMessage(
                        phoneValidation.formatted,
                        messageResult.message
                    );
                },
                {
                    maxRetries: 3,
                    initialDelay: 2000,
                    phoneNumber: phoneValidation.display
                }
            );
        }

        // 7. Esperar 10 minutos y luego enviar mensaje de seguimiento
        // Esto da tiempo al cliente para leer la información antes de solicitar confirmación
        setTimeout(async () => {
            try {
                const confirmationMessage = '┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈\n\n💬 *¿Recibiste la información de tu cita?*\n\nResponde este mensaje para:\n• Confirmar tu asistencia\n• Hacer preguntas\n• Solicitar cambios\n\n📞 *Línea de atención:* 018000 123456\n🌐 *Web:* www.electrohuila.com.co\n\n_ElectroHuila - Energía para tu hogar_ ⚡';

                console.log('📤 Enviando mensaje de seguimiento (después de 10 minutos)...');
                const result = await global.whatsappClient.sendMessage(
                    phoneValidation.formatted,
                    confirmationMessage
                );
                console.log('✅ Mensaje de seguimiento enviado exitosamente:', result.id?.id);
            } catch (msgError) {
                console.error('❌ Error enviando mensaje de seguimiento:', msgError);
            }
        }, 600000); // 10 minutos = 600,000 milisegundos

        // 8. Log exitoso
        logger.logMessage({
            phoneNumber: phoneValidation.normalized,
            phoneFormatted: phoneValidation.formatted,
            template: 'confirmacion_cita_enhanced',
            success: true,
            retries: retryCount - 1,
            messageLength: messageResult.message.length,
            templateData: data
        });

        const duration = Date.now() - startTime;
        return res.json({
            success: true,
            message: 'Confirmación de cita enviada con logo y botones',
            data: {
                phoneNumber: phoneValidation.display,
                template: 'confirmacion_cita_enhanced',
                timestamp: new Date().toISOString(),
                duration: `${duration}ms`,
                retries: retryCount - 1,
                logoSent: !!logoMedia,
                buttonsSent: true
            }
        });

    } catch (error) {
        console.error('Error en appointment-confirmation:', error.message);

        logger.logMessage({
            phoneNumber: req.body.phoneNumber,
            template: 'confirmacion_cita_enhanced',
            success: false,
            error: error.message,
            retries: retryCount
        });

        return res.status(500).json({
            success: false,
            error: 'Error interno del servidor'
        });
    }
});

/**
 * POST /whatsapp/appointment-reminder
 * Envía recordatorio de cita
 * Body: { phoneNumber, data: { nombre, fecha, hora, servicio, direccion?, anticipacion? } }
 */
router.post('/appointment-reminder', async (req, res) => {
    await sendTemplateMessage('recordatorio_cita', req, res);
});

/**
 * POST /whatsapp/appointment-cancellation
 * Envía cancelación de cita
 * Body: { phoneNumber, data: { nombre, fecha, hora, motivo?, reprogramar? } }
 */
router.post('/appointment-cancellation', async (req, res) => {
    await sendTemplateMessage('cancelacion_cita', req, res);
});

/**
 * GET /whatsapp/status
 * Obtiene el estado actual del servicio de WhatsApp
 */
router.get('/status', (req, res) => {
    try {
        const client = global.whatsappClient;

        if (!client) {
            return res.status(503).json({
                success: false,
                status: 'disconnected',
                error: 'Cliente de WhatsApp no inicializado'
            });
        }

        const isReady = client.info ? true : false;

        res.json({
            success: true,
            status: isReady ? 'ready' : 'initializing',
            whatsappReady: isReady,
            clientInfo: isReady ? {
                platform: client.info.platform,
                phone: client.info.wid?.user || null
            } : null,
            timestamp: new Date().toISOString()
        });

    } catch (error) {
        res.status(500).json({
            success: false,
            status: 'error',
            error: error.message
        });
    }
});

/**
 * GET /whatsapp/stats
 * Obtiene estadísticas de mensajes enviados
 * Query params: ?date=YYYY-MM-DD (opcional, default: hoy)
 */
router.get('/stats', (req, res) => {
    try {
        const date = req.query.date || null;

        // Validar formato de fecha si se proporciona
        if (date && !/^\d{4}-\d{2}-\d{2}$/.test(date)) {
            return res.status(400).json({
                success: false,
                error: 'Formato de fecha inválido. Use YYYY-MM-DD'
            });
        }

        const stats = logger.getStats(date);

        res.json({
            success: true,
            stats
        });

    } catch (error) {
        console.error(`❌ Error obteniendo estadísticas: ${error.message}`);
        res.status(500).json({
            success: false,
            error: 'Error obteniendo estadísticas'
        });
    }
});

/**
 * GET /whatsapp/logs
 * Obtiene los archivos de log disponibles
 */
router.get('/logs', (req, res) => {
    try {
        const logFiles = logger.listLogFiles();

        res.json({
            success: true,
            logs: logFiles
        });

    } catch (error) {
        console.error(`❌ Error listando logs: ${error.message}`);
        res.status(500).json({
            success: false,
            error: 'Error listando logs'
        });
    }
});

/**
 * GET /whatsapp/templates
 * Lista los templates disponibles
 */
router.get('/templates', (req, res) => {
    const { getAvailableTemplates } = require('../templates/whatsappTemplates');

    try {
        const templates = getAvailableTemplates();

        res.json({
            success: true,
            templates
        });

    } catch (error) {
        console.error(`❌ Error listando templates: ${error.message}`);
        res.status(500).json({
            success: false,
            error: 'Error listando templates'
        });
    }
});

// ============================================================================
// MEDIA ENDPOINTS - Images and Documents
// ============================================================================

/**
 * POST /whatsapp/send-image
 * Sends an image with optional caption
 * Body: {
 *   phoneNumber: string,
 *   image: {
 *     type: 'base64' | 'url',
 *     data?: string (base64),
 *     url?: string,
 *     mimetype?: string,
 *     filename?: string
 *   },
 *   caption?: string
 * }
 */
router.post('/send-image', validateImageRequest, async (req, res) => {
    const startTime = Date.now();

    try {
        const { phoneNumber, image, caption } = req.body;

        // Validate phone number
        const phoneValidation = validatePhoneNumber(phoneNumber);
        if (!phoneValidation.valid) {
            return res.status(400).json({
                success: false,
                error: phoneValidation.error
            });
        }

        // Check WhatsApp client
        if (!global.whatsappClient || !global.whatsappClient.info) {
            return res.status(503).json({
                success: false,
                error: 'Servicio de WhatsApp no disponible'
            });
        }

        let media;

        // Process image based on type
        if (image.type === 'base64') {
            media = new MessageMedia(
                image.mimetype || 'image/jpeg',
                image.data,
                image.filename || 'image.jpg'
            );
        } else if (image.type === 'url') {
            // Download image from URL
            const response = await axios.get(image.url, {
                responseType: 'arraybuffer',
                timeout: 30000
            });

            const base64Data = Buffer.from(response.data).toString('base64');
            const mimetype = response.headers['content-type'] || 'image/jpeg';
            const filename = image.filename || 'image.jpg';

            media = new MessageMedia(mimetype, base64Data, filename);
        }

        // Send image with retry
        const sendResult = await retryMessageSend(
            async () => {
                return await global.whatsappClient.sendMessage(
                    phoneValidation.formatted,
                    media,
                    { caption: caption || '' }
                );
            },
            {
                maxRetries: 3,
                initialDelay: 2000,
                phoneNumber: phoneValidation.display
            }
        );

        // Log successful send
        logger.logMessage({
            phoneNumber: phoneValidation.normalized,
            phoneFormatted: phoneValidation.formatted,
            template: 'image',
            success: true,
            messageLength: caption?.length || 0,
            templateData: { hasCaption: !!caption }
        });

        const duration = Date.now() - startTime;
        res.json({
            success: true,
            message: 'Imagen enviada correctamente',
            data: {
                phoneNumber: phoneValidation.display,
                messageId: sendResult.id?.id || null,
                timestamp: new Date().toISOString(),
                duration: `${duration}ms`
            }
        });

    } catch (error) {
        console.error('Error sending image:', error.message);

        logger.logMessage({
            phoneNumber: req.body.phoneNumber,
            template: 'image',
            success: false,
            error: error.message
        });

        res.status(500).json({
            success: false,
            error: `Error enviando imagen: ${error.message}`
        });
    }
});

/**
 * POST /whatsapp/send-document
 * Sends a document with optional caption
 * Body: {
 *   phoneNumber: string,
 *   document: {
 *     type: 'base64' | 'url',
 *     data?: string (base64),
 *     url?: string,
 *     mimetype: string,
 *     filename: string
 *   },
 *   caption?: string
 * }
 */
router.post('/send-document', validateDocumentRequest, async (req, res) => {
    const startTime = Date.now();

    try {
        const { phoneNumber, document, caption } = req.body;

        // Validate phone number
        const phoneValidation = validatePhoneNumber(phoneNumber);
        if (!phoneValidation.valid) {
            return res.status(400).json({
                success: false,
                error: phoneValidation.error
            });
        }

        // Check WhatsApp client
        if (!global.whatsappClient || !global.whatsappClient.info) {
            return res.status(503).json({
                success: false,
                error: 'Servicio de WhatsApp no disponible'
            });
        }

        let media;

        // Process document based on type
        if (document.type === 'base64') {
            media = new MessageMedia(
                document.mimetype,
                document.data,
                document.filename
            );
        } else if (document.type === 'url') {
            // Download document from URL
            const response = await axios.get(document.url, {
                responseType: 'arraybuffer',
                timeout: 60000 // 60 seconds for large files
            });

            const base64Data = Buffer.from(response.data).toString('base64');
            const mimetype = document.mimetype || response.headers['content-type'];

            media = new MessageMedia(mimetype, base64Data, document.filename);
        }

        // Send document with retry
        const sendResult = await retryMessageSend(
            async () => {
                return await global.whatsappClient.sendMessage(
                    phoneValidation.formatted,
                    media,
                    { caption: caption || '' }
                );
            },
            {
                maxRetries: 3,
                initialDelay: 2000,
                phoneNumber: phoneValidation.display
            }
        );

        // Log successful send
        logger.logMessage({
            phoneNumber: phoneValidation.normalized,
            phoneFormatted: phoneValidation.formatted,
            template: 'document',
            success: true,
            messageLength: caption?.length || 0,
            templateData: { filename: document.filename }
        });

        const duration = Date.now() - startTime;
        res.json({
            success: true,
            message: 'Documento enviado correctamente',
            data: {
                phoneNumber: phoneValidation.display,
                messageId: sendResult.id?.id || null,
                filename: document.filename,
                timestamp: new Date().toISOString(),
                duration: `${duration}ms`
            }
        });

    } catch (error) {
        console.error('Error sending document:', error.message);

        logger.logMessage({
            phoneNumber: req.body.phoneNumber,
            template: 'document',
            success: false,
            error: error.message
        });

        res.status(500).json({
            success: false,
            error: `Error enviando documento: ${error.message}`
        });
    }
});

// ============================================================================
// INTERACTIVE BUTTONS ENDPOINTS
// ============================================================================

/**
 * POST /whatsapp/send-buttons
 * Sends a message with interactive buttons
 * Body: {
 *   phoneNumber: string,
 *   body: string,
 *   buttons: [
 *     { body: string, id?: string }
 *   ],
 *   footer?: string,
 *   title?: string
 * }
 */
router.post('/send-buttons', validateButtonRequest, async (req, res) => {
    const startTime = Date.now();

    try {
        const { phoneNumber, body, buttons, footer, title } = req.body;

        if (!body) {
            return res.status(400).json({
                success: false,
                error: 'Body text is required'
            });
        }

        // Validate phone number
        const phoneValidation = validatePhoneNumber(phoneNumber);
        if (!phoneValidation.valid) {
            return res.status(400).json({
                success: false,
                error: phoneValidation.error
            });
        }

        // Check WhatsApp client
        if (!global.whatsappClient || !global.whatsappClient.info) {
            return res.status(503).json({
                success: false,
                error: 'Servicio de WhatsApp no disponible'
            });
        }

        // Format buttons for whatsapp-web.js
        const formattedButtons = buttons.map((btn, index) => ({
            body: btn.body,
            id: btn.id || `btn_${index + 1}`
        }));

        // Create button message object
        const buttonMessage = {
            body: body,
            buttons: formattedButtons,
            footer: footer || '',
            title: title || ''
        };

        // Send button message with retry
        const sendResult = await retryMessageSend(
            async () => {
                return await global.whatsappClient.sendMessage(
                    phoneValidation.formatted,
                    buttonMessage
                );
            },
            {
                maxRetries: 3,
                initialDelay: 2000,
                phoneNumber: phoneValidation.display
            }
        );

        // Log successful send
        logger.logMessage({
            phoneNumber: phoneValidation.normalized,
            phoneFormatted: phoneValidation.formatted,
            template: 'buttons',
            success: true,
            messageLength: body.length,
            templateData: { buttonCount: buttons.length }
        });

        const duration = Date.now() - startTime;
        res.json({
            success: true,
            message: 'Mensaje con botones enviado correctamente',
            data: {
                phoneNumber: phoneValidation.display,
                messageId: sendResult.id?.id || null,
                buttonCount: buttons.length,
                timestamp: new Date().toISOString(),
                duration: `${duration}ms`
            }
        });

    } catch (error) {
        console.error('Error sending buttons:', error.message);

        logger.logMessage({
            phoneNumber: req.body.phoneNumber,
            template: 'buttons',
            success: false,
            error: error.message
        });

        res.status(500).json({
            success: false,
            error: `Error enviando botones: ${error.message}`
        });
    }
});

// ============================================================================
// WEBHOOK ENDPOINTS
// ============================================================================

/**
 * POST /whatsapp/webhooks
 * Register a new webhook
 * Body: {
 *   url: string,
 *   events: string[],
 *   secret?: string,
 *   name?: string
 * }
 */
router.post('/webhooks', async (req, res) => {
    try {
        const validation = validateWebhookConfig(req.body);
        if (!validation.valid) {
            return res.status(400).json({
                success: false,
                error: validation.error
            });
        }

        const result = webhookManager.registerWebhook(req.body);
        res.json(result);

    } catch (error) {
        console.error('Error registering webhook:', error.message);
        res.status(500).json({
            success: false,
            error: 'Error registrando webhook'
        });
    }
});

/**
 * GET /whatsapp/webhooks
 * Get all registered webhooks
 */
router.get('/webhooks', (req, res) => {
    try {
        const webhooks = webhookManager.getWebhooks();
        res.json({
            success: true,
            webhooks
        });
    } catch (error) {
        console.error('Error getting webhooks:', error.message);
        res.status(500).json({
            success: false,
            error: 'Error obteniendo webhooks'
        });
    }
});

/**
 * GET /whatsapp/webhooks/:id
 * Get a specific webhook
 */
router.get('/webhooks/:id', (req, res) => {
    try {
        const webhook = webhookManager.getWebhook(req.params.id);

        if (!webhook) {
            return res.status(404).json({
                success: false,
                error: 'Webhook no encontrado'
            });
        }

        res.json({
            success: true,
            webhook
        });
    } catch (error) {
        console.error('Error getting webhook:', error.message);
        res.status(500).json({
            success: false,
            error: 'Error obteniendo webhook'
        });
    }
});

/**
 * PUT /whatsapp/webhooks/:id
 * Update a webhook
 * Body: { url?, events?, secret?, name?, active? }
 */
router.put('/webhooks/:id', async (req, res) => {
    try {
        const result = webhookManager.updateWebhook(req.params.id, req.body);

        if (!result.success) {
            return res.status(404).json(result);
        }

        res.json(result);
    } catch (error) {
        console.error('Error updating webhook:', error.message);
        res.status(500).json({
            success: false,
            error: 'Error actualizando webhook'
        });
    }
});

/**
 * DELETE /whatsapp/webhooks/:id
 * Delete a webhook
 */
router.delete('/webhooks/:id', async (req, res) => {
    try {
        const result = webhookManager.deleteWebhook(req.params.id);

        if (!result.success) {
            return res.status(404).json(result);
        }

        res.json(result);
    } catch (error) {
        console.error('Error deleting webhook:', error.message);
        res.status(500).json({
            success: false,
            error: 'Error eliminando webhook'
        });
    }
});

/**
 * POST /whatsapp/webhooks/:id/test
 * Test a webhook
 */
router.post('/webhooks/:id/test', async (req, res) => {
    try {
        const result = await webhookManager.testWebhook(req.params.id);

        if (!result.success) {
            return res.status(result.error === 'Webhook not found' ? 404 : 500).json(result);
        }

        res.json({
            success: true,
            message: 'Webhook test enviado',
            result
        });
    } catch (error) {
        console.error('Error testing webhook:', error.message);
        res.status(500).json({
            success: false,
            error: 'Error probando webhook'
        });
    }
});

module.exports = router;
