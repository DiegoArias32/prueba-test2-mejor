/**
 * Configuración de SMTP para Gmail
 * Utiliza variables de entorno para mayor seguridad
 */

const emailSettings = {
  smtp: {
    host: process.env.SMTP_HOST || "smtp.gmail.com",
    port: parseInt(process.env.SMTP_PORT || "587"),
    secure: process.env.SMTP_SECURE === "true" ? true : false,
    auth: {
      user: process.env.GMAIL_USER || "ariasdiego5709@gmail.com",
      pass: process.env.GMAIL_PASSWORD || "klty ndqe excg zuij"
    }
  },
  from: {
    email: process.env.FROM_EMAIL || "ariasdiego5709@gmail.com",
    name: process.env.FROM_NAME || "Sistema de Citas ElectroHuila"
  },
  templates: {
    confirmationEmail: "confirmación de correo",
    recoveryEmail: "recuperación de contraseña"
  }
};

// Validar configuración requerida
const validateConfig = () => {
  const required = ['smtp.auth.user', 'smtp.auth.pass', 'from.email'];
  const errors = [];
  
  if (!emailSettings.smtp.auth.user) errors.push('GMAIL_USER no configurado');
  if (!emailSettings.smtp.auth.pass) errors.push('GMAIL_PASSWORD no configurado');
  if (!emailSettings.from.email) errors.push('FROM_EMAIL no configurado');
  
  if (errors.length > 0) {
    console.warn('⚠️  Advertencias de configuración:', errors);
  }
  
  return emailSettings;
};

module.exports = validateConfig();
