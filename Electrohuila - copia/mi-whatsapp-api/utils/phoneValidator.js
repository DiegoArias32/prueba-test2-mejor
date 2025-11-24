/**
 * Validador y normalizador de números de teléfono colombianos para WhatsApp
 */

/**
 * Valida y normaliza un número de teléfono colombiano
 * @param {string} phoneNumber - Número de teléfono a validar
 * @returns {object} { valid: boolean, normalized?: string, error?: string, formatted?: string }
 */
function validatePhoneNumber(phoneNumber) {
    // Verificar que se proporcionó un número
    if (!phoneNumber) {
        return {
            valid: false,
            error: 'Número de teléfono requerido'
        };
    }

    // Convertir a string y limpiar espacios y caracteres especiales
    let cleanNumber = String(phoneNumber)
        .replace(/\s+/g, '')  // Remover espacios
        .replace(/[-()]/g, '') // Remover guiones y paréntesis
        .replace(/\+/g, '');   // Remover símbolo +

    // Si el número ya tiene el código de país (57), removerlo temporalmente
    let hasCountryCode = false;
    if (cleanNumber.startsWith('57')) {
        cleanNumber = cleanNumber.substring(2);
        hasCountryCode = true;
    }

    // Validar que contenga solo dígitos
    if (!/^\d+$/.test(cleanNumber)) {
        return {
            valid: false,
            error: 'El número de teléfono debe contener solo dígitos'
        };
    }

    // Validar longitud (debe ser 10 dígitos para Colombia)
    if (cleanNumber.length !== 10) {
        return {
            valid: false,
            error: `Número inválido. Debe tener 10 dígitos (actual: ${cleanNumber.length})`
        };
    }

    // Validar que empiece con 3 (números móviles en Colombia)
    if (!cleanNumber.startsWith('3')) {
        return {
            valid: false,
            error: 'El número debe empezar con 3 (números móviles colombianos)'
        };
    }

    // Validar prefijos móviles comunes en Colombia (300-350)
    const prefix = cleanNumber.substring(0, 3);
    const validPrefixes = [
        '300', '301', '302', '303', '304', '305', '310', '311', '312', '313',
        '314', '315', '316', '317', '318', '319', '320', '321', '322', '323',
        '324', '325', '326', '327', '328', '329', '330', '331', '332', '333',
        '334', '335', '336', '337', '338', '339', '340', '341', '342', '343',
        '344', '345', '346', '347', '348', '349', '350', '351'
    ];

    if (!validPrefixes.includes(prefix)) {
        console.warn(`⚠️ Prefijo ${prefix} no es un prefijo móvil colombiano común, pero se procesará`);
    }

    // Normalizar al formato internacional con código de país
    const normalized = `57${cleanNumber}`;

    // Formato para whatsapp-web.js (añadir @c.us)
    const formatted = `${normalized}@c.us`;

    // Formato legible para humanos
    const display = `+57 ${cleanNumber.substring(0, 3)} ${cleanNumber.substring(3, 6)} ${cleanNumber.substring(6)}`;

    return {
        valid: true,
        normalized,      // 57XXXXXXXXXX
        formatted,       // 57XXXXXXXXXX@c.us (para whatsapp-web.js)
        display,         // +57 XXX XXX XXXX (para mostrar)
        original: phoneNumber
    };
}

/**
 * Valida múltiples números de teléfono
 * @param {string[]} phoneNumbers - Array de números a validar
 * @returns {object} { valid: object[], invalid: object[] }
 */
function validatePhoneNumbers(phoneNumbers) {
    if (!Array.isArray(phoneNumbers)) {
        throw new Error('Se esperaba un array de números de teléfono');
    }

    const valid = [];
    const invalid = [];

    phoneNumbers.forEach(phone => {
        const result = validatePhoneNumber(phone);
        if (result.valid) {
            valid.push(result);
        } else {
            invalid.push({ phone, error: result.error });
        }
    });

    return { valid, invalid };
}

/**
 * Formatea un número ya validado para WhatsApp Web
 * @param {string} phoneNumber - Número validado (puede incluir o no el código de país)
 * @returns {string} Número formateado para whatsapp-web.js (XXXXXXXXXXXX@c.us)
 */
function formatForWhatsApp(phoneNumber) {
    const validation = validatePhoneNumber(phoneNumber);
    if (!validation.valid) {
        throw new Error(validation.error);
    }
    return validation.formatted;
}

module.exports = {
    validatePhoneNumber,
    validatePhoneNumbers,
    formatForWhatsApp
};
