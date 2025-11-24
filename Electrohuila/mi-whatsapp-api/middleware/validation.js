/**
 * Enhanced validation middleware for WhatsApp API
 * Validates media files, buttons, and other request data
 */

const path = require('path');

/**
 * Supported image formats and their MIME types
 */
const SUPPORTED_IMAGE_FORMATS = {
    'image/jpeg': ['.jpg', '.jpeg'],
    'image/png': ['.png'],
    'image/webp': ['.webp'],
    'image/gif': ['.gif']
};

/**
 * Supported document formats and their MIME types
 */
const SUPPORTED_DOCUMENT_FORMATS = {
    'application/pdf': ['.pdf'],
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document': ['.docx'],
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': ['.xlsx'],
    'application/vnd.openxmlformats-officedocument.presentationml.presentation': ['.pptx'],
    'application/msword': ['.doc'],
    'application/vnd.ms-excel': ['.xls'],
    'text/plain': ['.txt'],
    'text/csv': ['.csv']
};

/**
 * Maximum file sizes (in bytes)
 */
const MAX_FILE_SIZES = {
    image: 5 * 1024 * 1024,    // 5MB for images
    document: 100 * 1024 * 1024, // 100MB for documents
    video: 16 * 1024 * 1024      // 16MB for videos
};

/**
 * Validates image data (base64 or URL)
 * @param {object} imageData - Image data object
 * @returns {object} Validation result
 */
function validateImage(imageData) {
    if (!imageData) {
        return {
            valid: false,
            error: 'Image data is required'
        };
    }

    const { type, data, url, mimetype, filename } = imageData;

    // Validate type
    if (!type || !['base64', 'url'].includes(type)) {
        return {
            valid: false,
            error: 'Image type must be "base64" or "url"'
        };
    }

    // Validate base64
    if (type === 'base64') {
        if (!data) {
            return {
                valid: false,
                error: 'Base64 data is required when type is "base64"'
            };
        }

        // Validate mimetype
        if (mimetype && !Object.keys(SUPPORTED_IMAGE_FORMATS).includes(mimetype)) {
            return {
                valid: false,
                error: `Unsupported image format. Supported: ${Object.keys(SUPPORTED_IMAGE_FORMATS).join(', ')}`
            };
        }

        // Estimate base64 size (rough calculation)
        const estimatedSize = (data.length * 3) / 4;
        if (estimatedSize > MAX_FILE_SIZES.image) {
            return {
                valid: false,
                error: `Image size exceeds maximum of ${MAX_FILE_SIZES.image / (1024 * 1024)}MB`
            };
        }
    }

    // Validate URL
    if (type === 'url') {
        if (!url) {
            return {
                valid: false,
                error: 'URL is required when type is "url"'
            };
        }

        // Basic URL validation
        try {
            new URL(url);
        } catch (e) {
            return {
                valid: false,
                error: 'Invalid URL format'
            };
        }
    }

    return {
        valid: true,
        data: imageData
    };
}

/**
 * Validates document data
 * @param {object} documentData - Document data object
 * @returns {object} Validation result
 */
function validateDocument(documentData) {
    if (!documentData) {
        return {
            valid: false,
            error: 'Document data is required'
        };
    }

    const { type, data, url, mimetype, filename } = documentData;

    // Validate type
    if (!type || !['base64', 'url'].includes(type)) {
        return {
            valid: false,
            error: 'Document type must be "base64" or "url"'
        };
    }

    // Validate filename
    if (!filename) {
        return {
            valid: false,
            error: 'Filename is required for documents'
        };
    }

    // Validate base64
    if (type === 'base64') {
        if (!data) {
            return {
                valid: false,
                error: 'Base64 data is required when type is "base64"'
            };
        }

        // Validate mimetype
        if (mimetype && !Object.keys(SUPPORTED_DOCUMENT_FORMATS).includes(mimetype)) {
            return {
                valid: false,
                error: `Unsupported document format. Supported: ${Object.keys(SUPPORTED_DOCUMENT_FORMATS).join(', ')}`
            };
        }

        // Estimate base64 size
        const estimatedSize = (data.length * 3) / 4;
        if (estimatedSize > MAX_FILE_SIZES.document) {
            return {
                valid: false,
                error: `Document size exceeds maximum of ${MAX_FILE_SIZES.document / (1024 * 1024)}MB`
            };
        }
    }

    // Validate URL
    if (type === 'url') {
        if (!url) {
            return {
                valid: false,
                error: 'URL is required when type is "url"'
            };
        }

        try {
            new URL(url);
        } catch (e) {
            return {
                valid: false,
                error: 'Invalid URL format'
            };
        }
    }

    return {
        valid: true,
        data: documentData
    };
}

/**
 * Validates button configuration
 * @param {Array} buttons - Array of button objects
 * @returns {object} Validation result
 */
function validateButtons(buttons) {
    if (!buttons) {
        return {
            valid: false,
            error: 'Buttons array is required'
        };
    }

    if (!Array.isArray(buttons)) {
        return {
            valid: false,
            error: 'Buttons must be an array'
        };
    }

    if (buttons.length === 0) {
        return {
            valid: false,
            error: 'At least one button is required'
        };
    }

    if (buttons.length > 3) {
        return {
            valid: false,
            error: 'Maximum 3 buttons allowed for quick replies'
        };
    }

    // Validate each button
    for (let i = 0; i < buttons.length; i++) {
        const button = buttons[i];

        if (!button.body || typeof button.body !== 'string') {
            return {
                valid: false,
                error: `Button ${i + 1}: body is required and must be a string`
            };
        }

        if (button.body.length > 20) {
            return {
                valid: false,
                error: `Button ${i + 1}: body must be 20 characters or less`
            };
        }

        // Validate button ID (optional, but if provided must be valid)
        if (button.id && typeof button.id !== 'string') {
            return {
                valid: false,
                error: `Button ${i + 1}: id must be a string`
            };
        }
    }

    return {
        valid: true,
        data: buttons
    };
}

/**
 * Validates call-to-action buttons (URL or phone)
 * @param {Array} buttons - Array of CTA button objects
 * @returns {object} Validation result
 */
function validateCtaButtons(buttons) {
    if (!buttons) {
        return {
            valid: false,
            error: 'Buttons array is required'
        };
    }

    if (!Array.isArray(buttons)) {
        return {
            valid: false,
            error: 'Buttons must be an array'
        };
    }

    if (buttons.length === 0) {
        return {
            valid: false,
            error: 'At least one button is required'
        };
    }

    if (buttons.length > 2) {
        return {
            valid: false,
            error: 'Maximum 2 call-to-action buttons allowed'
        };
    }

    // Validate each button
    for (let i = 0; i < buttons.length; i++) {
        const button = buttons[i];

        if (!button.type || !['url', 'call'].includes(button.type)) {
            return {
                valid: false,
                error: `Button ${i + 1}: type must be "url" or "call"`
            };
        }

        if (!button.displayText || typeof button.displayText !== 'string') {
            return {
                valid: false,
                error: `Button ${i + 1}: displayText is required`
            };
        }

        if (button.type === 'url') {
            if (!button.url) {
                return {
                    valid: false,
                    error: `Button ${i + 1}: url is required for URL buttons`
                };
            }
            try {
                new URL(button.url);
            } catch (e) {
                return {
                    valid: false,
                    error: `Button ${i + 1}: invalid URL format`
                };
            }
        }

        if (button.type === 'call') {
            if (!button.phoneNumber) {
                return {
                    valid: false,
                    error: `Button ${i + 1}: phoneNumber is required for call buttons`
                };
            }
        }
    }

    return {
        valid: true,
        data: buttons
    };
}

/**
 * Validates webhook configuration
 * @param {object} webhookData - Webhook configuration
 * @returns {object} Validation result
 */
function validateWebhookConfig(webhookData) {
    if (!webhookData) {
        return {
            valid: false,
            error: 'Webhook configuration is required'
        };
    }

    const { url, events, secret } = webhookData;

    // Validate URL
    if (!url) {
        return {
            valid: false,
            error: 'Webhook URL is required'
        };
    }

    try {
        const parsedUrl = new URL(url);
        if (!['http:', 'https:'].includes(parsedUrl.protocol)) {
            return {
                valid: false,
                error: 'Webhook URL must use HTTP or HTTPS protocol'
            };
        }
    } catch (e) {
        return {
            valid: false,
            error: 'Invalid webhook URL format'
        };
    }

    // Validate events
    if (events && !Array.isArray(events)) {
        return {
            valid: false,
            error: 'Events must be an array'
        };
    }

    const validEvents = ['message_received', 'message_status', 'button_clicked', 'qr_updated', 'ready', 'disconnected'];
    if (events) {
        for (const event of events) {
            if (!validEvents.includes(event)) {
                return {
                    valid: false,
                    error: `Invalid event type: ${event}. Valid events: ${validEvents.join(', ')}`
                };
            }
        }
    }

    // Validate secret (optional but recommended)
    if (secret && typeof secret !== 'string') {
        return {
            valid: false,
            error: 'Secret must be a string'
        };
    }

    return {
        valid: true,
        data: webhookData
    };
}

/**
 * Express middleware to validate image requests
 */
function validateImageRequest(req, res, next) {
    const { image, caption } = req.body;

    if (!image) {
        return res.status(400).json({
            success: false,
            error: 'Image data is required'
        });
    }

    const validation = validateImage(image);
    if (!validation.valid) {
        return res.status(400).json({
            success: false,
            error: validation.error
        });
    }

    // Validate caption (optional)
    if (caption && typeof caption !== 'string') {
        return res.status(400).json({
            success: false,
            error: 'Caption must be a string'
        });
    }

    if (caption && caption.length > 1000) {
        return res.status(400).json({
            success: false,
            error: 'Caption must be 1000 characters or less'
        });
    }

    next();
}

/**
 * Express middleware to validate document requests
 */
function validateDocumentRequest(req, res, next) {
    const { document, caption } = req.body;

    if (!document) {
        return res.status(400).json({
            success: false,
            error: 'Document data is required'
        });
    }

    const validation = validateDocument(document);
    if (!validation.valid) {
        return res.status(400).json({
            success: false,
            error: validation.error
        });
    }

    // Validate caption (optional)
    if (caption && typeof caption !== 'string') {
        return res.status(400).json({
            success: false,
            error: 'Caption must be a string'
        });
    }

    next();
}

/**
 * Express middleware to validate button requests
 */
function validateButtonRequest(req, res, next) {
    const { buttons, buttonType } = req.body;

    if (!buttons) {
        return res.status(400).json({
            success: false,
            error: 'Buttons are required'
        });
    }

    let validation;
    if (buttonType === 'cta' || buttonType === 'call-to-action') {
        validation = validateCtaButtons(buttons);
    } else {
        validation = validateButtons(buttons);
    }

    if (!validation.valid) {
        return res.status(400).json({
            success: false,
            error: validation.error
        });
    }

    next();
}

module.exports = {
    validateImage,
    validateDocument,
    validateButtons,
    validateCtaButtons,
    validateWebhookConfig,
    validateImageRequest,
    validateDocumentRequest,
    validateButtonRequest,
    SUPPORTED_IMAGE_FORMATS,
    SUPPORTED_DOCUMENT_FORMATS,
    MAX_FILE_SIZES
};
