const express = require("express");
const router = express.Router();
const nodemailer = require("nodemailer");
const emailSettings = require("../config/emailSettings");
const templates = require("../templates/emailTemplates");
const authMiddleware = require("../middleware/auth");
const logger = require("../utils/logger");
const { retryWithBackoff } = require("../utils/retryHandler");

/**
 * Crear transportador SMTP reutilizable
 */
const transporter = nodemailer.createTransport(emailSettings.smtp);

/**
 * Verificar conexión al iniciar
 */
transporter.verify((error, success) => {
  if (error) {
    console.error('❌ Error de conexión SMTP:', error.message);
  } else {
    console.log('✅ Servidor SMTP conectado y listo para enviar correos');
  }
});

/**
 * POST /email/send
 * Enviar correo electrónico
 *
 * Body esperado:
 * {
 *   "to": "destinatario@example.com",
 *   "subject": "Asunto del correo",
 *   "text": "Contenido del correo",
 *   "html": "<h1>Contenido HTML opcional</h1>",
 *   "template": "appointmentConfirmation", // opcional
 *   "templateData": { ... } // datos para el template
 * }
 */
router.post("/send", authMiddleware, async (req, res) => {
  try {
    const { to, subject, text, html, template, templateData } = req.body;

    // Validar campos requeridos
    if (!to || !subject || (!text && !html && !template)) {
      return res.status(400).json({
        ok: false,
        error: "Campos requeridos faltantes: to, subject, y (text o html o template)"
      });
    }

    // Validar formato de email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(to)) {
      return res.status(400).json({
        ok: false,
        error: "Formato de email inválido"
      });
    }

    const mailOptions = {
      from: `"${emailSettings.from.name}" <${emailSettings.from.email}>`,
      to,
      subject
    };

    // Si se proporciona un template, generarlo
    if (template && templates[template]) {
      mailOptions.html = templates[template](templateData || {});
    } else {
      // Si no, usar text o html proporcionados
      if (text) mailOptions.text = text;
      if (html) mailOptions.html = html;
    }

    // Enviar correo con reintentos
    const info = await retryWithBackoff(
      () => transporter.sendMail(mailOptions),
      3,
      1000
    );

    console.log('📧 Correo enviado:', info.response);

    // Registrar en logs
    await logger.logEmail({
      email: to,
      template: template || 'custom',
      subject,
      success: true,
      messageId: info.messageId
    });

    res.json({
      ok: true,
      mensaje: "✅ Correo enviado exitosamente",
      messageId: info.messageId
    });

  } catch (error) {
    console.error('❌ Error al enviar correo:', error);

    // Registrar error en logs
    await logger.logEmail({
      email: req.body.to,
      template: req.body.template || 'custom',
      subject: req.body.subject,
      success: false,
      error: error.message
    });

    const status = error.responseCode || 500;
    const message = error.message || "Error desconocido al enviar el correo";

    res.status(status).json({
      ok: false,
      error: message,
      type: error.code
    });
  }
});

/**
 * POST /email/appointment-confirmation
 * Enviar confirmación de cita con diseño profesional
 */
router.post("/appointment-confirmation", authMiddleware, async (req, res) => {
  try {
    const { to, name, date, time, professional, location } = req.body;

    if (!to) {
      return res.status(400).json({
        ok: false,
        error: "El email del destinatario es requerido"
      });
    }

    const mailOptions = {
      from: `"${emailSettings.from.name}" <${emailSettings.from.email}>`,
      to,
      subject: "✓ Tu cita ha sido confirmada",
      html: templates.appointmentConfirmation({
        date: date || "Por confirmar",
        time: time || "Por confirmar",
        professional: professional || "Por asignar",
        location: location || "Sede Principal"
      })
    };

    const info = await retryWithBackoff(
      () => transporter.sendMail(mailOptions),
      3,
      1000
    );

    // Registrar en logs
    await logger.logEmail({
      email: to,
      template: 'appointmentConfirmation',
      subject: "✓ Tu cita ha sido confirmada",
      success: true,
      messageId: info.messageId
    });

    res.json({
      ok: true,
      mensaje: "✅ Confirmación de cita enviada",
      messageId: info.messageId
    });

  } catch (error) {
    console.error('❌ Error:', error);

    // Registrar error en logs
    await logger.logEmail({
      email: req.body.to,
      template: 'appointmentConfirmation',
      subject: "✓ Tu cita ha sido confirmada",
      success: false,
      error: error.message
    });

    res.status(500).json({
      ok: false,
      error: error.message
    });
  }
});

/**
 * POST /email/appointment-reminder
 * Enviar recordatorio de cita
 */
router.post("/appointment-reminder", authMiddleware, async (req, res) => {
  try {
    const { to, name, date, time, location, address, appointmentNumber } = req.body;

    if (!to) {
      return res.status(400).json({
        ok: false,
        error: "El email del destinatario es requerido"
      });
    }

    const mailOptions = {
      from: `"${emailSettings.from.name}" <${emailSettings.from.email}>`,
      to,
      subject: "⏰ Recordatorio: Tu cita está próxima",
      html: templates.appointmentReminder({
        name: name || "usuario",
        date: date || "Por confirmar",
        time: time || "Por confirmar",
        location: location || "Sede Principal",
        address: address || "",
        appointmentNumber: appointmentNumber || ""
      })
    };

    const info = await retryWithBackoff(
      () => transporter.sendMail(mailOptions),
      3,
      1000
    );

    // Registrar en logs
    await logger.logEmail({
      email: to,
      template: 'appointmentReminder',
      subject: "⏰ Recordatorio: Tu cita está próxima",
      success: true,
      messageId: info.messageId
    });

    res.json({
      ok: true,
      mensaje: "✅ Recordatorio de cita enviado",
      messageId: info.messageId
    });

  } catch (error) {
    console.error('❌ Error:', error);

    // Registrar error en logs
    await logger.logEmail({
      email: req.body.to,
      template: 'appointmentReminder',
      subject: "⏰ Recordatorio: Tu cita está próxima",
      success: false,
      error: error.message
    });

    res.status(500).json({
      ok: false,
      error: error.message
    });
  }
});

/**
 * POST /email/password-reset
 * Enviar código de recuperación de contraseña
 */
router.post("/password-reset", authMiddleware, async (req, res) => {
  try {
    const { to, name, code } = req.body;

    if (!to || !code) {
      return res.status(400).json({
        ok: false,
        error: "El email y el código son requeridos"
      });
    }

    const mailOptions = {
      from: `"${emailSettings.from.name}" <${emailSettings.from.email}>`,
      to,
      subject: "🔐 Recupera tu contraseña",
      html: templates.passwordReset({
        name: name || "usuario",
        code: code
      })
    };

    const info = await retryWithBackoff(
      () => transporter.sendMail(mailOptions),
      3,
      1000
    );

    // Registrar en logs
    await logger.logEmail({
      email: to,
      template: 'passwordReset',
      subject: "🔐 Recupera tu contraseña",
      success: true,
      messageId: info.messageId
    });

    res.json({
      ok: true,
      mensaje: "✅ Email de recuperación enviado",
      messageId: info.messageId
    });

  } catch (error) {
    console.error('❌ Error:', error);

    // Registrar error en logs
    await logger.logEmail({
      email: req.body.to,
      template: 'passwordReset',
      subject: "🔐 Recupera tu contraseña",
      success: false,
      error: error.message
    });

    res.status(500).json({
      ok: false,
      error: error.message
    });
  }
});

/**
 * POST /email/welcome
 * Enviar email de bienvenida
 */
router.post("/welcome", authMiddleware, async (req, res) => {
  try {
    const { to, name, dashboardUrl } = req.body;

    if (!to) {
      return res.status(400).json({
        ok: false,
        error: "El email del destinatario es requerido"
      });
    }

    const mailOptions = {
      from: `"${emailSettings.from.name}" <${emailSettings.from.email}>`,
      to,
      subject: "🎉 ¡Bienvenido a nuestro sistema!",
      html: templates.welcome({
        name: name || "usuario",
        dashboardUrl: dashboardUrl || "https://localhost:3000"
      })
    };

    const info = await retryWithBackoff(
      () => transporter.sendMail(mailOptions),
      3,
      1000
    );

    // Registrar en logs
    await logger.logEmail({
      email: to,
      template: 'welcome',
      subject: "🎉 ¡Bienvenido a nuestro sistema!",
      success: true,
      messageId: info.messageId
    });

    res.json({
      ok: true,
      mensaje: "✅ Email de bienvenida enviado",
      messageId: info.messageId
    });

  } catch (error) {
    console.error('❌ Error:', error);

    // Registrar error en logs
    await logger.logEmail({
      email: req.body.to,
      template: 'welcome',
      subject: "🎉 ¡Bienvenido a nuestro sistema!",
      success: false,
      error: error.message
    });

    res.status(500).json({
      ok: false,
      error: error.message
    });
  }
});

/**
 * POST /email/appointment-cancellation
 * Enviar cancelación de cita con diseño profesional
 */
router.post("/appointment-cancellation", authMiddleware, async (req, res) => {
  try {
    const { to, name, date, time, professional, location, reason, schedulingUrl } = req.body;

    if (!to) {
      return res.status(400).json({
        ok: false,
        error: "El email del destinatario es requerido"
      });
    }

    const mailOptions = {
      from: `"${emailSettings.from.name}" <${emailSettings.from.email}>`,
      to,
      subject: "✗ Tu cita ha sido cancelada",
      html: templates.appointmentCancellation({
        date: date || "Por confirmar",
        time: time || "Por confirmar",
        professional: professional || "Por asignar",
        location: location || "Sede Principal",
        reason: reason || null,
        schedulingUrl: schedulingUrl || "https://localhost:3000/citas"
      })
    };

    const info = await retryWithBackoff(
      () => transporter.sendMail(mailOptions),
      3,
      1000
    );

    // Registrar en logs
    await logger.logEmail({
      email: to,
      template: 'appointmentCancellation',
      subject: "✗ Tu cita ha sido cancelada",
      success: true,
      messageId: info.messageId
    });

    res.json({
      ok: true,
      mensaje: "✅ Notificación de cancelación enviada",
      messageId: info.messageId
    });

  } catch (error) {
    console.error('❌ Error:', error);

    // Registrar error en logs
    await logger.logEmail({
      email: req.body.to,
      template: 'appointmentCancellation',
      subject: "✗ Tu cita ha sido cancelada",
      success: false,
      error: error.message
    });

    res.status(500).json({
      ok: false,
      error: error.message
    });
  }
});

/**
 * GET /email/status
 * Verificar estado de la conexión SMTP
 */
router.get("/status", async (req, res) => {
  try {
    await transporter.verify();
    res.json({
      ok: true,
      status: "Conectado",
      mensaje: "✅ Servidor SMTP operacional"
    });
  } catch (error) {
    res.status(500).json({
      ok: false,
      status: "Desconectado",
      error: error.message
    });
  }
});

/**
 * GET /email/templates
 * Listar templates disponibles
 */
router.get("/templates", (req, res) => {
  res.json({
    ok: true,
    templates: Object.keys(templates),
    description: {
      appointmentConfirmation: "Confirmación de cita",
      appointmentReminder: "Recordatorio de cita",
      appointmentCancellation: "Cancelación de cita",
      passwordReset: "Recuperación de contraseña",
      welcome: "Email de bienvenida"
    }
  });
});

/**
 * GET /email/stats
 * Obtener estadísticas de envíos
 */
router.get("/stats", async (req, res) => {
  try {
    const date = req.query.date || null;
    const stats = await logger.getStats(date);

    res.json({
      ok: true,
      date: date || new Date().toISOString().split('T')[0],
      stats
    });
  } catch (error) {
    console.error('❌ Error obteniendo estadísticas:', error);
    res.status(500).json({
      ok: false,
      error: error.message
    });
  }
});

/**
 * GET /email/logs
 * Obtener logs de envíos
 */
router.get("/logs", async (req, res) => {
  try {
    const date = req.query.date || null;
    const limit = parseInt(req.query.limit) || 100;
    const logs = await logger.getLogs(date, limit);

    res.json({
      ok: true,
      date: date || new Date().toISOString().split('T')[0],
      count: logs.length,
      logs
    });
  } catch (error) {
    console.error('❌ Error obteniendo logs:', error);
    res.status(500).json({
      ok: false,
      error: error.message
    });
  }
});

module.exports = router;
