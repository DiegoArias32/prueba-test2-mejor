/**
 * Sistema de templates para mensajes de WhatsApp
 * Cada template define los campos requeridos y genera el mensaje formateado
 */

const templates = {
    confirmacion_cita: {
        requiredFields: ['nombreCliente', 'fecha', 'hora', 'ubicacion'],
        generate: (data) => {
            return `✅ *CONFIRMACIÓN DE CITA - ELECTROHUILA*

Hola *${data.nombreCliente}*,

Tu cita ha sido confirmada exitosamente:

━━━━━━━━━━━━━━━━━━━━━━━━
📋 *DATOS DEL CLIENTE*
━━━━━━━━━━━━━━━━━━━━━━━━
👤 *Nombre:* ${data.nombreCliente}
🆔 *Cliente:* ${data.clienteId || data.numeroCita || 'N/A'}
${data.telefono ? `📱 *Teléfono:* ${data.telefono}\n` : ''}${data.direccionCliente ? `🏠 *Dirección:* ${data.direccionCliente}\n` : ''}
━━━━━━━━━━━━━━━━━━━━━━━━
📅 *DETALLES DE LA CITA*
━━━━━━━━━━━━━━━━━━━━━━━━
🎫 *Número de Cita:* ${data.numeroCita || 'N/A'}
📅 *Fecha:* ${data.fecha}
🕐 *Hora:* ${data.hora}
${data.tipoCita ? `📝 *Motivo:* ${data.tipoCita}\n` : ''}${data.profesional ? `👤 *Atendido por:* ${data.profesional}\n` : ''}
━━━━━━━━━━━━━━━━━━━━━━━━
📍 *UBICACIÓN*
━━━━━━━━━━━━━━━━━━━━━━━━
📍 *Sede:* ${data.ubicacion}
${data.direccion ? `🗺️ *Dirección:* ${data.direccion}\n` : ''}
${data.observaciones ? `━━━━━━━━━━━━━━━━━━━━━━━━\n📝 *OBSERVACIONES*\n━━━━━━━━━━━━━━━━━━━━━━━━\n${data.observaciones}\n\n` : ''}⏰ *Por favor llega 10 minutos antes de tu cita.*

${data.qrUrl ? '🔍 *Escanea el código QR que te enviamos para verificar tu cita.*\n\n' : ''}Si necesitas reprogramar o cancelar, contáctanos con anticipación.

¡Te esperamos! 👷‍♂️⚡

_ElectroHuila - Energía para tu hogar_`;
        }
    },

    recordatorio_cita: {
        requiredFields: ['nombreCliente', 'fecha', 'hora', 'ubicacion'],
        generate: (data) => {
            return `🔔 *RECORDATORIO DE CITA - ELECTROHUILA*

Hola *${data.nombreCliente}*,

Te recordamos tu cita programada:

🎫 *Número de Cita:* ${data.numeroCita || 'N/A'}
📅 *Fecha:* ${data.fecha}
🕐 *Hora:* ${data.hora}
📍 *Ubicación:* ${data.ubicacion}
${data.direccion ? `🗺️ *Dirección:* ${data.direccion}\n` : ''}

${data.anticipacion ? `⏰ Tu cita es ${data.anticipacion}\n` : ''}
Por favor confirma tu asistencia respondiendo este mensaje.

Si necesitas cancelar o reprogramar, avísanos lo antes posible.

¡Te esperamos! 👷‍♂️`;
        }
    },

    cancelacion_cita: {
        requiredFields: ['nombreCliente', 'fecha', 'hora'],
        generate: (data) => {
            return `❌ *CANCELACIÓN DE CITA - ELECTROHUILA*

Hola *${data.nombreCliente}*,

Lamentamos informarte que tu cita ha sido cancelada:

🎫 *Número de Cita:* ${data.numeroCita || 'N/A'}
📅 *Fecha:* ${data.fecha}
🕐 *Hora:* ${data.hora}
${data.motivo ? `📋 *Motivo:* ${data.motivo}` : ''}

${data.reprogramar ? '🔄 Por favor contáctanos para reprogramar tu cita.\n' : ''}
Disculpa las molestias ocasionadas.

Si tienes alguna duda, estamos a tu disposición. 📞`;
        }
    },

    cita_completada: {
        requiredFields: ['nombreCliente', 'fecha', 'hora', 'ubicacion'],
        generate: (data) => {
            return `✅ *CITA COMPLETADA - ELECTROHUILA*

Hola *${data.nombreCliente}*,

¡Gracias por asistir a tu cita! 🎉

━━━━━━━━━━━━━━━━━━━━━━━━
📋 *DETALLES DEL SERVICIO*
━━━━━━━━━━━━━━━━━━━━━━━━
🎫 *Número de Cita:* ${data.numeroCita || 'N/A'}
📅 *Fecha:* ${data.fecha}
🕐 *Hora:* ${data.hora}
📍 *Sede:* ${data.ubicacion}
${data.tipoCita ? `📝 *Servicio:* ${data.tipoCita}\n` : ''}${data.observaciones ? `📝 *Observaciones:* ${data.observaciones}\n` : ''}
━━━━━━━━━━━━━━━━━━━━━━━━

✅ Tu servicio ha sido completado exitosamente.

Si tienes alguna consulta sobre el servicio realizado o necesitas asistencia adicional, no dudes en contactarnos. 📞

¡Gracias por confiar en ElectroHuila! 👷‍♂️⚡

_ElectroHuila - Energía para tu hogar_`;
        }
    }
};

/**
 * Valida que los datos requeridos para un template estén presentes
 * @param {string} templateName - Nombre del template
 * @param {object} data - Datos a validar
 * @returns {object} { valid: boolean, missingFields: string[] }
 */
function validateTemplateData(templateName, data) {
    const template = templates[templateName];

    if (!template) {
        return {
            valid: false,
            error: `Template '${templateName}' no encontrado. Templates disponibles: ${Object.keys(templates).join(', ')}`
        };
    }

    const missingFields = template.requiredFields.filter(field => !data[field]);

    if (missingFields.length > 0) {
        return {
            valid: false,
            missingFields,
            error: `Faltan campos requeridos: ${missingFields.join(', ')}`
        };
    }

    return { valid: true };
}

/**
 * Genera un mensaje a partir de un template
 * @param {string} templateName - Nombre del template a usar
 * @param {object} data - Datos para el template
 * @returns {object} { success: boolean, message?: string, error?: string }
 */
function generateMessage(templateName, data) {
    // Validar que el template existe
    const template = templates[templateName];
    if (!template) {
        return {
            success: false,
            error: `Template '${templateName}' no encontrado. Templates disponibles: ${Object.keys(templates).join(', ')}`
        };
    }

    // Validar datos requeridos
    const validation = validateTemplateData(templateName, data);
    if (!validation.valid) {
        return {
            success: false,
            error: validation.error,
            missingFields: validation.missingFields
        };
    }

    // Generar mensaje
    try {
        const message = template.generate(data);
        return {
            success: true,
            message
        };
    } catch (error) {
        return {
            success: false,
            error: `Error generando mensaje: ${error.message}`
        };
    }
}

/**
 * Obtiene la lista de templates disponibles
 * @returns {string[]} Lista de nombres de templates
 */
function getAvailableTemplates() {
    return Object.keys(templates).map(name => ({
        name,
        requiredFields: templates[name].requiredFields
    }));
}

module.exports = {
    generateMessage,
    validateTemplateData,
    getAvailableTemplates
};
