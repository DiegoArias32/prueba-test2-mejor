// Admin Panel JavaScript

const API_BASE_URL = window.location.origin;
let API_KEY = localStorage.getItem('whatsapp_api_key') || '';

// Prompt for API key if not set
if (!API_KEY) {
    API_KEY = prompt('Enter your API Key:');
    if (API_KEY) {
        localStorage.setItem('whatsapp_api_key', API_KEY);
    }
}

// API Helper Functions
async function apiRequest(endpoint, options = {}) {
    const headers = {
        'Content-Type': 'application/json',
        'X-API-Key': API_KEY,
        ...options.headers
    };

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            ...options,
            headers
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.error || 'Request failed');
        }

        return data;
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}

// Initialize
document.addEventListener('DOMContentLoaded', function() {
    initializeTabs();
    initializeForms();
    loadStatus();
    loadStatistics();
    setupAutoRefresh();
    updateCurrentTime();
    setInterval(updateCurrentTime, 1000);
});

// Tab Management
function initializeTabs() {
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabContents = document.querySelectorAll('.tab-content');

    tabButtons.forEach(button => {
        button.addEventListener('click', () => {
            const tabId = button.getAttribute('data-tab');

            // Remove active class from all
            tabButtons.forEach(btn => btn.classList.remove('active'));
            tabContents.forEach(content => content.classList.remove('active'));

            // Add active class to selected
            button.classList.add('active');
            document.getElementById(tabId).classList.add('active');

            // Load data for specific tabs
            if (tabId === 'webhooks') {
                loadWebhooks();
            }
        });
    });
}

// Initialize Forms
function initializeForms() {
    // Send Message Form
    document.getElementById('send-message-form').addEventListener('submit', handleSendMessage);
    document.getElementById('msg-template').addEventListener('change', updateTemplateFields);

    // Send Media Form
    document.getElementById('send-media-form').addEventListener('submit', handleSendMedia);
    document.getElementById('media-source').addEventListener('change', toggleMediaSource);

    // Send Buttons Form
    document.getElementById('send-buttons-form').addEventListener('submit', handleSendButtons);
    document.getElementById('add-button').addEventListener('click', addButtonField);

    // Webhook Form
    document.getElementById('add-webhook-btn').addEventListener('click', () => {
        document.getElementById('add-webhook-form').style.display = 'block';
    });
    document.getElementById('cancel-webhook').addEventListener('click', () => {
        document.getElementById('add-webhook-form').style.display = 'none';
    });
    document.getElementById('webhook-form').addEventListener('submit', handleAddWebhook);

    // Load Logs
    document.getElementById('load-logs').addEventListener('click', loadLogs);
    document.getElementById('log-date').valueAsDate = new Date();
}

// Load Status
async function loadStatus() {
    try {
        const healthData = await fetch(`${API_BASE_URL}/health`).then(r => r.json());
        const statusData = await fetch(`${API_BASE_URL}/whatsapp/status`).then(r => r.json());

        const statusBadge = document.getElementById('connection-status');
        const statusText = document.getElementById('status-text');
        const phoneNumber = document.getElementById('phone-number');
        const platform = document.getElementById('platform');
        const uptime = document.getElementById('uptime');

        if (statusData.whatsappReady) {
            statusBadge.textContent = 'Ready';
            statusBadge.className = 'status-badge ready';
            statusText.textContent = 'Connected';
            phoneNumber.textContent = statusData.clientInfo?.phone || '-';
            platform.textContent = statusData.clientInfo?.platform || '-';
            document.getElementById('qr-container').style.display = 'none';
        } else {
            statusBadge.textContent = 'Initializing';
            statusBadge.className = 'status-badge initializing';
            statusText.textContent = 'Initializing...';
            phoneNumber.textContent = '-';
            platform.textContent = '-';

            // Check for QR code
            checkQRCode();
        }

        uptime.textContent = formatUptime(healthData.uptime);

    } catch (error) {
        console.error('Error loading status:', error);
        document.getElementById('connection-status').textContent = 'Error';
        document.getElementById('connection-status').className = 'status-badge disconnected';
    }
}

// Check for QR Code
async function checkQRCode() {
    try {
        const response = await fetch(`${API_BASE_URL}/admin/qr`);
        if (response.ok) {
            const data = await response.json();
            if (data.qr) {
                document.getElementById('qr-code').src = data.qr;
                document.getElementById('qr-container').style.display = 'block';
            }
        }
    } catch (error) {
        console.error('Error loading QR:', error);
    }
}

// Load Statistics
async function loadStatistics() {
    try {
        const data = await apiRequest('/whatsapp/stats');
        const stats = data.stats;

        document.getElementById('total-messages').textContent = stats.totalMessages;
        document.getElementById('successful-messages').textContent = stats.successful;
        document.getElementById('failed-messages').textContent = stats.failed;
        document.getElementById('success-rate').textContent = stats.successRate;

    } catch (error) {
        console.error('Error loading statistics:', error);
    }
}

// Handle Send Message
async function handleSendMessage(e) {
    e.preventDefault();

    const resultBox = document.getElementById('message-result');
    resultBox.style.display = 'none';

    const phoneNumber = document.getElementById('msg-phone').value;
    const template = document.getElementById('msg-template').value;

    // Collect template data
    const data = {};
    const fields = document.querySelectorAll('#template-fields input, #template-fields textarea');
    fields.forEach(field => {
        data[field.name] = field.value;
    });

    try {
        // Map Spanish template names to English API endpoints
        const templateMap = {
            'confirmacion_cita': 'confirmation',
            'recordatorio_cita': 'reminder',
            'cancelacion_cita': 'cancellation'
        };
        
        const endpoint = `/whatsapp/appointment-${templateMap[template]}`;
        const result = await apiRequest(endpoint, {
            method: 'POST',
            body: JSON.stringify({ phoneNumber, data })
        });

        showResult(resultBox, 'success', `Message sent successfully! ID: ${result.data.messageId}`);
        e.target.reset();
        loadStatistics();

    } catch (error) {
        showResult(resultBox, 'error', `Error: ${error.message}`);
    }
}

// Update Template Fields
function updateTemplateFields() {
    const template = document.getElementById('msg-template').value;
    const container = document.getElementById('template-fields');

    if (!template) {
        container.innerHTML = '';
        return;
    }

    const fields = {
        confirmacion_cita: [
            { name: 'nombre', label: 'Name', type: 'text', required: true },
            { name: 'fecha', label: 'Date', type: 'date', required: true },
            { name: 'hora', label: 'Time', type: 'text', required: true },
            { name: 'servicio', label: 'Service', type: 'text', required: true },
            { name: 'direccion', label: 'Address', type: 'text', required: false },
            { name: 'notas', label: 'Notes', type: 'textarea', required: false }
        ],
        recordatorio_cita: [
            { name: 'nombre', label: 'Name', type: 'text', required: true },
            { name: 'fecha', label: 'Date', type: 'date', required: true },
            { name: 'hora', label: 'Time', type: 'text', required: true },
            { name: 'servicio', label: 'Service', type: 'text', required: true },
            { name: 'direccion', label: 'Address', type: 'text', required: false },
            { name: 'anticipacion', label: 'Advance Notice', type: 'text', required: false }
        ],
        cancelacion_cita: [
            { name: 'nombre', label: 'Name', type: 'text', required: true },
            { name: 'fecha', label: 'Date', type: 'date', required: true },
            { name: 'hora', label: 'Time', type: 'text', required: true },
            { name: 'motivo', label: 'Reason', type: 'text', required: false },
            { name: 'reprogramar', label: 'Reschedule', type: 'checkbox', required: false }
        ]
    };

    const templateFields = fields[template] || [];
    container.innerHTML = templateFields.map(field => {
        if (field.type === 'textarea') {
            return `
                <div class="form-group">
                    <label for="${field.name}">${field.label}${field.required ? ' *' : ''}</label>
                    <textarea name="${field.name}" ${field.required ? 'required' : ''} rows="3"></textarea>
                </div>
            `;
        } else if (field.type === 'checkbox') {
            return `
                <div class="form-group">
                    <label>
                        <input type="checkbox" name="${field.name}">
                        ${field.label}
                    </label>
                </div>
            `;
        } else {
            return `
                <div class="form-group">
                    <label for="${field.name}">${field.label}${field.required ? ' *' : ''}</label>
                    <input type="${field.type}" name="${field.name}" ${field.required ? 'required' : ''}>
                </div>
            `;
        }
    }).join('');
}

// Handle Send Media
async function handleSendMedia(e) {
    e.preventDefault();

    const resultBox = document.getElementById('media-result');
    resultBox.style.display = 'none';

    const phoneNumber = document.getElementById('media-phone').value;
    const mediaType = document.getElementById('media-type').value;
    const mediaSource = document.getElementById('media-source').value;
    const caption = document.getElementById('media-caption').value;

    try {
        let mediaData;

        if (mediaSource === 'url') {
            const url = document.getElementById('media-url').value;
            mediaData = {
                type: 'url',
                url: url,
                filename: url.split('/').pop()
            };
        } else {
            const file = document.getElementById('media-file').files[0];
            if (!file) {
                throw new Error('Please select a file');
            }

            const base64 = await fileToBase64(file);
            mediaData = {
                type: 'base64',
                data: base64.split(',')[1],
                mimetype: file.type,
                filename: file.name
            };
        }

        const endpoint = mediaType === 'image' ? '/whatsapp/send-image' : '/whatsapp/send-document';
        const bodyKey = mediaType === 'image' ? 'image' : 'document';

        const result = await apiRequest(endpoint, {
            method: 'POST',
            body: JSON.stringify({
                phoneNumber,
                [bodyKey]: mediaData,
                caption
            })
        });

        showResult(resultBox, 'success', `${mediaType} sent successfully! ID: ${result.data.messageId}`);
        e.target.reset();
        loadStatistics();

    } catch (error) {
        showResult(resultBox, 'error', `Error: ${error.message}`);
    }
}

// Toggle Media Source
function toggleMediaSource() {
    const source = document.getElementById('media-source').value;
    document.getElementById('url-input').style.display = source === 'url' ? 'block' : 'none';
    document.getElementById('file-input').style.display = source === 'file' ? 'block' : 'none';
}

// Handle Send Buttons
async function handleSendButtons(e) {
    e.preventDefault();

    const resultBox = document.getElementById('buttons-result');
    resultBox.style.display = 'none';

    const phoneNumber = document.getElementById('btn-phone').value;
    const title = document.getElementById('btn-title').value;
    const body = document.getElementById('btn-body').value;
    const footer = document.getElementById('btn-footer').value;

    const buttonInputs = document.querySelectorAll('.btn-text');
    const buttons = Array.from(buttonInputs)
        .map((input, index) => ({
            body: input.value,
            id: `btn_${index + 1}`
        }))
        .filter(btn => btn.body.trim() !== '');

    if (buttons.length === 0) {
        showResult(resultBox, 'error', 'Please add at least one button');
        return;
    }

    try {
        const result = await apiRequest('/whatsapp/send-buttons', {
            method: 'POST',
            body: JSON.stringify({
                phoneNumber,
                title,
                body,
                footer,
                buttons
            })
        });

        showResult(resultBox, 'success', `Buttons sent successfully! ID: ${result.data.messageId}`);
        e.target.reset();
        document.getElementById('buttons-container').innerHTML = `
            <div class="button-input">
                <input type="text" class="btn-text" placeholder="Button 1 text" required>
            </div>
        `;
        loadStatistics();

    } catch (error) {
        showResult(resultBox, 'error', `Error: ${error.message}`);
    }
}

// Add Button Field
function addButtonField() {
    const container = document.getElementById('buttons-container');
    const count = container.querySelectorAll('.button-input').length;

    if (count >= 3) {
        alert('Maximum 3 buttons allowed');
        return;
    }

    const div = document.createElement('div');
    div.className = 'button-input';
    div.innerHTML = `<input type="text" class="btn-text" placeholder="Button ${count + 1} text">`;
    container.appendChild(div);
}

// Handle Add Webhook
async function handleAddWebhook(e) {
    e.preventDefault();

    const name = document.getElementById('webhook-name').value;
    const url = document.getElementById('webhook-url').value;
    const secret = document.getElementById('webhook-secret').value;

    const eventCheckboxes = document.querySelectorAll('input[name="webhook-events"]:checked');
    const events = Array.from(eventCheckboxes).map(cb => cb.value);

    try {
        await apiRequest('/whatsapp/webhooks', {
            method: 'POST',
            body: JSON.stringify({ name, url, secret, events })
        });

        alert('Webhook registered successfully!');
        e.target.reset();
        document.getElementById('add-webhook-form').style.display = 'none';
        loadWebhooks();

    } catch (error) {
        alert(`Error: ${error.message}`);
    }
}

// Load Webhooks
async function loadWebhooks() {
    try {
        const data = await apiRequest('/whatsapp/webhooks');
        const container = document.getElementById('webhooks-list');

        if (data.webhooks.length === 0) {
            container.innerHTML = '<div class="empty-state"><div class="empty-state-icon">📡</div><p>No webhooks configured</p></div>';
            return;
        }

        container.innerHTML = data.webhooks.map(webhook => `
            <div class="webhook-item">
                <div class="webhook-header">
                    <div class="webhook-name">${webhook.name}</div>
                    <span class="status-badge ${webhook.active ? 'ready' : 'disconnected'}">
                        ${webhook.active ? 'Active' : 'Inactive'}
                    </span>
                </div>
                <div class="webhook-url">${webhook.url}</div>
                <div class="webhook-events">
                    ${webhook.events.map(event => `<span class="event-badge">${event}</span>`).join('')}
                </div>
                <div class="webhook-stats">
                    Success: ${webhook.successCount} | Failed: ${webhook.failureCount}
                    ${webhook.lastTriggered ? ` | Last: ${new Date(webhook.lastTriggered).toLocaleString()}` : ''}
                </div>
                <div class="webhook-actions">
                    <button class="btn btn-secondary" onclick="testWebhook('${webhook.id}')">Test</button>
                    <button class="btn btn-secondary" onclick="toggleWebhook('${webhook.id}', ${!webhook.active})">
                        ${webhook.active ? 'Disable' : 'Enable'}
                    </button>
                    <button class="btn btn-danger" onclick="deleteWebhook('${webhook.id}')">Delete</button>
                </div>
            </div>
        `).join('');

    } catch (error) {
        console.error('Error loading webhooks:', error);
    }
}

// Test Webhook
async function testWebhook(id) {
    try {
        await apiRequest(`/whatsapp/webhooks/${id}/test`, { method: 'POST' });
        alert('Test webhook sent!');
        loadWebhooks();
    } catch (error) {
        alert(`Error: ${error.message}`);
    }
}

// Toggle Webhook
async function toggleWebhook(id, active) {
    try {
        await apiRequest(`/whatsapp/webhooks/${id}`, {
            method: 'PUT',
            body: JSON.stringify({ active })
        });
        loadWebhooks();
    } catch (error) {
        alert(`Error: ${error.message}`);
    }
}

// Delete Webhook
async function deleteWebhook(id) {
    if (!confirm('Are you sure you want to delete this webhook?')) {
        return;
    }

    try {
        await apiRequest(`/whatsapp/webhooks/${id}`, { method: 'DELETE' });
        loadWebhooks();
    } catch (error) {
        alert(`Error: ${error.message}`);
    }
}

// Load Logs
async function loadLogs() {
    const date = document.getElementById('log-date').value;
    const container = document.getElementById('logs-container');

    try {
        const data = await apiRequest(`/whatsapp/stats?date=${date}`);
        const stats = data.stats;

        if (stats.totalMessages === 0) {
            container.innerHTML = '<div class="empty-state"><div class="empty-state-icon">📋</div><p>No logs for this date</p></div>';
            return;
        }

        // Show summary
        container.innerHTML = `
            <div style="margin-bottom: 20px; padding: 15px; background: var(--light-bg); border-radius: 6px;">
                <h4>Summary for ${date}</h4>
                <p>Total: ${stats.totalMessages} | Success: ${stats.successful} | Failed: ${stats.failed} | Rate: ${stats.successRate}</p>
            </div>
        `;

    } catch (error) {
        container.innerHTML = `<div class="result-box error">Error loading logs: ${error.message}</div>`;
    }
}

// Helper Functions
function showResult(element, type, message) {
    element.className = `result-box ${type}`;
    element.textContent = message;
    element.style.display = 'block';
}

function fileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}

function formatUptime(seconds) {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    return `${hours}h ${minutes}m`;
}

function updateCurrentTime() {
    document.getElementById('current-time').textContent = new Date().toLocaleString();
}

function setupAutoRefresh() {
    setInterval(loadStatus, 10000); // Every 10 seconds
    setInterval(loadStatistics, 30000); // Every 30 seconds
}
