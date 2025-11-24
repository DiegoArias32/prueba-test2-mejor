/**
 * Middleware de autenticación con API Key
 * Similar a WhatsApp API
 */

const authMiddleware = (req, res, next) => {
  const apiKey = req.headers['x-api-key'] || req.headers['authorization']?.replace('Bearer ', '');

  // Obtener API key del entorno
  const validApiKey = process.env.GMAIL_API_KEY || 'your-secure-api-key-here';

  if (!apiKey) {
    return res.status(401).json({
      ok: false,
      error: 'API Key requerida. Incluye el header X-API-Key o Authorization: Bearer {key}'
    });
  }

  if (apiKey !== validApiKey) {
    return res.status(403).json({
      ok: false,
      error: 'API Key inválida'
    });
  }

  next();
};

module.exports = authMiddleware;
