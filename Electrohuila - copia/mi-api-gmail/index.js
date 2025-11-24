require('dotenv').config();
const express = require("express");
const cors = require("cors");
const emailRoutes = require("./routes/email");

const app = express();
const PORT = process.env.PORT || 4000;

/**
 * Middleware
 */
app.use(express.json({ limit: '10mb' }));
app.use(express.urlencoded({ limit: '10mb', extended: true }));

/**
 * CORS - Permitir solicitudes desde otros orígenes
 */
app.use(cors({
  origin: '*',
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'],
  allowedHeaders: ['Origin', 'X-Requested-With', 'Content-Type', 'Accept', 'X-API-Key', 'Authorization']
}));

/**
 * Logging de peticiones
 */
app.use((req, res, next) => {
  console.log(`📨 ${req.method} ${req.path}`);
  next();
});

/**
 * Rutas
 */

// Ruta raíz - Health check
app.get("/", (req, res) => {
  res.json({
    status: "online",
    mensaje: "🚀 Gmail API para ElectroHuila - Production Ready",
    version: "1.0.0",
    endpoints: {
      // GET endpoints (público)
      health: "GET /",
      status: "GET /email/status",
      templates: "GET /email/templates",
      stats: "GET /email/stats?date=YYYY-MM-DD",
      logs: "GET /email/logs?date=YYYY-MM-DD&limit=100",
      // POST endpoints (requieren API Key)
      send: "POST /email/send",
      appointmentConfirmation: "POST /email/appointment-confirmation",
      appointmentReminder: "POST /email/appointment-reminder",
      appointmentCancellation: "POST /email/appointment-cancellation",
      passwordReset: "POST /email/password-reset",
      welcome: "POST /email/welcome"
    },
    authentication: {
      header: "X-API-Key",
      alternativeHeader: "Authorization: Bearer {key}"
    }
  });
});

// Rutas de email
app.use("/email", emailRoutes);

/**
 * Manejo de rutas no encontradas (404)
 */
app.use((req, res) => {
  res.status(404).json({
    ok: false,
    error: "Ruta no encontrada",
    path: req.path,
    method: req.method
  });
});

/**
 * Manejo global de errores
 */
app.use((err, req, res, next) => {
  console.error('❌ Error del servidor:', err);
  res.status(500).json({
    ok: false,
    error: "Error interno del servidor",
    message: process.env.NODE_ENV === 'development' ? err.message : undefined
  });
});

/**
 * Iniciar servidor
 */
app.listen(PORT, () => {
  console.log(`\n${'='.repeat(50)}`);
  console.log(`🚀 Mi API Gmail iniciada`);
  console.log(`📍 Puerto: ${PORT}`);
  console.log(`🔗 URL: http://localhost:${PORT}`);
  console.log(`${'='.repeat(50)}\n`);
});

/**
 * Manejo de errores no capturados
 */
process.on('unhandledRejection', (reason, promise) => {
  console.error('❌ Promise rechazada no manejada:', reason);
});
