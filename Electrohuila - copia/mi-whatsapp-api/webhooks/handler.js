/**
 * Webhook Handler System
 * Manages webhooks for incoming messages, status updates, and events
 */

const axios = require('axios');
const crypto = require('crypto');
const fs = require('fs');
const path = require('path');

class WebhookManager {
    constructor() {
        this.webhooks = new Map();
        this.configFile = path.join(process.cwd(), 'webhooks', 'webhooks.json');
        this.loadWebhooks();
    }

    /**
     * Load webhooks from configuration file
     */
    loadWebhooks() {
        try {
            if (fs.existsSync(this.configFile)) {
                const data = fs.readFileSync(this.configFile, 'utf8');
                const webhooksArray = JSON.parse(data);

                webhooksArray.forEach(webhook => {
                    this.webhooks.set(webhook.id, webhook);
                });

                console.log(`Loaded ${this.webhooks.size} webhook(s) from configuration`);
            }
        } catch (error) {
            console.error('Error loading webhooks:', error.message);
        }
    }

    /**
     * Save webhooks to configuration file
     */
    saveWebhooks() {
        try {
            const webhooksDir = path.dirname(this.configFile);
            if (!fs.existsSync(webhooksDir)) {
                fs.mkdirSync(webhooksDir, { recursive: true });
            }

            const webhooksArray = Array.from(this.webhooks.values());
            fs.writeFileSync(this.configFile, JSON.stringify(webhooksArray, null, 2), 'utf8');
            console.log('Webhooks saved to configuration');
        } catch (error) {
            console.error('Error saving webhooks:', error.message);
        }
    }

    /**
     * Register a new webhook
     * @param {object} config - Webhook configuration
     * @returns {object} Registration result
     */
    registerWebhook(config) {
        const { url, events, secret, name } = config;

        // Generate unique ID
        const id = crypto.randomBytes(16).toString('hex');

        const webhook = {
            id,
            name: name || `Webhook ${id.substring(0, 8)}`,
            url,
            events: events || ['message_received', 'message_status'],
            secret: secret || null,
            active: true,
            createdAt: new Date().toISOString(),
            lastTriggered: null,
            successCount: 0,
            failureCount: 0
        };

        this.webhooks.set(id, webhook);
        this.saveWebhooks();

        return {
            success: true,
            webhook: {
                id: webhook.id,
                name: webhook.name,
                url: webhook.url,
                events: webhook.events,
                active: webhook.active
            }
        };
    }

    /**
     * Update an existing webhook
     * @param {string} id - Webhook ID
     * @param {object} updates - Fields to update
     * @returns {object} Update result
     */
    updateWebhook(id, updates) {
        const webhook = this.webhooks.get(id);

        if (!webhook) {
            return {
                success: false,
                error: 'Webhook not found'
            };
        }

        // Update allowed fields
        const allowedFields = ['url', 'events', 'secret', 'name', 'active'];
        allowedFields.forEach(field => {
            if (updates[field] !== undefined) {
                webhook[field] = updates[field];
            }
        });

        webhook.updatedAt = new Date().toISOString();
        this.webhooks.set(id, webhook);
        this.saveWebhooks();

        return {
            success: true,
            webhook: {
                id: webhook.id,
                name: webhook.name,
                url: webhook.url,
                events: webhook.events,
                active: webhook.active
            }
        };
    }

    /**
     * Delete a webhook
     * @param {string} id - Webhook ID
     * @returns {object} Deletion result
     */
    deleteWebhook(id) {
        const webhook = this.webhooks.get(id);

        if (!webhook) {
            return {
                success: false,
                error: 'Webhook not found'
            };
        }

        this.webhooks.delete(id);
        this.saveWebhooks();

        return {
            success: true,
            message: 'Webhook deleted successfully'
        };
    }

    /**
     * Get all webhooks
     * @returns {Array} List of webhooks
     */
    getWebhooks() {
        return Array.from(this.webhooks.values()).map(webhook => ({
            id: webhook.id,
            name: webhook.name,
            url: webhook.url,
            events: webhook.events,
            active: webhook.active,
            createdAt: webhook.createdAt,
            lastTriggered: webhook.lastTriggered,
            successCount: webhook.successCount,
            failureCount: webhook.failureCount
        }));
    }

    /**
     * Get a specific webhook
     * @param {string} id - Webhook ID
     * @returns {object|null} Webhook or null
     */
    getWebhook(id) {
        return this.webhooks.get(id) || null;
    }

    /**
     * Generate signature for webhook payload
     * @param {string} payload - JSON payload
     * @param {string} secret - Webhook secret
     * @returns {string} HMAC signature
     */
    generateSignature(payload, secret) {
        return crypto
            .createHmac('sha256', secret)
            .update(payload)
            .digest('hex');
    }

    /**
     * Trigger webhooks for a specific event
     * @param {string} eventType - Type of event
     * @param {object} data - Event data
     */
    async triggerWebhooks(eventType, data) {
        const webhooksToTrigger = Array.from(this.webhooks.values())
            .filter(webhook => webhook.active && webhook.events.includes(eventType));

        if (webhooksToTrigger.length === 0) {
            return;
        }

        console.log(`Triggering ${webhooksToTrigger.length} webhook(s) for event: ${eventType}`);

        const promises = webhooksToTrigger.map(webhook =>
            this.sendWebhook(webhook, eventType, data)
        );

        await Promise.allSettled(promises);
    }

    /**
     * Send webhook HTTP request
     * @param {object} webhook - Webhook configuration
     * @param {string} eventType - Event type
     * @param {object} data - Event data
     */
    async sendWebhook(webhook, eventType, data) {
        try {
            const payload = {
                event: eventType,
                timestamp: new Date().toISOString(),
                data
            };

            const payloadString = JSON.stringify(payload);
            const headers = {
                'Content-Type': 'application/json',
                'User-Agent': 'WhatsApp-API-Webhook/1.0'
            };

            // Add signature if secret is configured
            if (webhook.secret) {
                headers['X-Webhook-Signature'] = this.generateSignature(payloadString, webhook.secret);
            }

            const response = await axios.post(webhook.url, payload, {
                headers,
                timeout: 10000, // 10 second timeout
                validateStatus: (status) => status >= 200 && status < 300
            });

            // Update webhook stats
            webhook.lastTriggered = new Date().toISOString();
            webhook.successCount++;
            this.webhooks.set(webhook.id, webhook);
            this.saveWebhooks();

            console.log(`Webhook ${webhook.id} triggered successfully for ${eventType}`);

            return {
                success: true,
                webhookId: webhook.id,
                status: response.status
            };

        } catch (error) {
            // Update failure count
            webhook.failureCount++;
            this.webhooks.set(webhook.id, webhook);
            this.saveWebhooks();

            console.error(`Error triggering webhook ${webhook.id}:`, error.message);

            return {
                success: false,
                webhookId: webhook.id,
                error: error.message
            };
        }
    }

    /**
     * Test a webhook by sending a test event
     * @param {string} id - Webhook ID
     * @returns {object} Test result
     */
    async testWebhook(id) {
        const webhook = this.webhooks.get(id);

        if (!webhook) {
            return {
                success: false,
                error: 'Webhook not found'
            };
        }

        const testData = {
            test: true,
            message: 'This is a test webhook event',
            timestamp: new Date().toISOString()
        };

        const result = await this.sendWebhook(webhook, 'test', testData);

        return result;
    }
}

// Create global webhook manager instance
const webhookManager = new WebhookManager();

/**
 * Setup webhook listeners for WhatsApp client
 * @param {object} client - WhatsApp client instance
 */
function setupWebhookListeners(client) {
    // Message received event
    client.on('message', async (message) => {
        try {
            const messageData = {
                id: message.id.id,
                from: message.from,
                to: message.to,
                body: message.body,
                type: message.type,
                timestamp: message.timestamp,
                hasMedia: message.hasMedia,
                isForwarded: message.isForwarded,
                fromMe: message.fromMe
            };

            await webhookManager.triggerWebhooks('message_received', messageData);
        } catch (error) {
            console.error('Error processing message webhook:', error.message);
        }
    });

    // Message ack (status) event
    client.on('message_ack', async (message, ack) => {
        try {
            const ackStates = {
                1: 'sent',
                2: 'received',
                3: 'read',
                4: 'played'
            };

            const statusData = {
                messageId: message.id.id,
                to: message.to,
                status: ackStates[ack] || 'unknown',
                timestamp: new Date().toISOString()
            };

            await webhookManager.triggerWebhooks('message_status', statusData);
        } catch (error) {
            console.error('Error processing message_ack webhook:', error.message);
        }
    });

    // QR code updated event
    client.on('qr', async (qr) => {
        try {
            await webhookManager.triggerWebhooks('qr_updated', { qr });
        } catch (error) {
            console.error('Error processing qr webhook:', error.message);
        }
    });

    // Client ready event
    client.on('ready', async () => {
        try {
            const readyData = {
                phone: client.info.wid?.user || null,
                platform: client.info.platform
            };
            await webhookManager.triggerWebhooks('ready', readyData);
        } catch (error) {
            console.error('Error processing ready webhook:', error.message);
        }
    });

    // Client disconnected event
    client.on('disconnected', async (reason) => {
        try {
            await webhookManager.triggerWebhooks('disconnected', { reason });
        } catch (error) {
            console.error('Error processing disconnected webhook:', error.message);
        }
    });

    console.log('Webhook listeners configured');
}

module.exports = {
    WebhookManager,
    webhookManager,
    setupWebhookListeners
};
