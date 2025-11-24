/**
 * Middleware de autenticación para la API de WhatsApp
 * Verifica que las peticiones incluyan una API key válida
 */

/**
 * Middleware que valida la API key en los headers
 * Soporta dos formatos:
 * - X-API-Key: {key}
 * - Authorization: Bearer {key}
 */
function authenticateApiKey(req, res, next) {
    // Obtener la API key de las variables de entorno
    const validApiKey = process.env.WHATSAPP_API_KEY;

    // Si no hay API key configurada, devolver error de configuración
    if (!validApiKey) {
        console.error('❌ WHATSAPP_API_KEY no está configurada en las variables de entorno');
        return res.status(500).json({
            success: false,
            error: 'API key no configurada en el servidor'
        });
    }

    // Intentar obtener la API key del header X-API-Key
    let providedApiKey = req.headers['x-api-key'];

    // Si no está en X-API-Key, intentar con Authorization Bearer
    if (!providedApiKey) {
        const authHeader = req.headers['authorization'];
        if (authHeader && authHeader.startsWith('Bearer ')) {
            providedApiKey = authHeader.substring(7); // Remover 'Bearer '
        }
    }

    // Si no se proporcionó ninguna API key
    if (!providedApiKey) {
        return res.status(401).json({
            success: false,
            error: 'API key requerida. Use el header X-API-Key o Authorization: Bearer {key}'
        });
    }

    // Verificar que la API key sea válida
    if (providedApiKey !== validApiKey) {
        console.warn(`⚠️ Intento de acceso con API key inválida desde ${req.ip}`);
        return res.status(403).json({
            success: false,
            error: 'API key inválida'
        });
    }

    // API key válida, continuar con la petición
    next();
}

/**
 * Middleware opcional que solo valida la API key si está presente
 * Útil para endpoints que pueden ser públicos pero tienen funcionalidad adicional con autenticación
 */
function optionalAuth(req, res, next) {
    const validApiKey = process.env.WHATSAPP_API_KEY;
    const providedApiKey = req.headers['x-api-key'] ||
                          (req.headers['authorization']?.startsWith('Bearer ') ?
                           req.headers['authorization'].substring(7) : null);

    if (providedApiKey) {
        req.isAuthenticated = (providedApiKey === validApiKey);
    } else {
        req.isAuthenticated = false;
    }

    next();
}

module.exports = {
    authenticateApiKey,
    optionalAuth
};
