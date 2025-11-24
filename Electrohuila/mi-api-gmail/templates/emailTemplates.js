/**
 * Templates de Email HTML Profesionales
 * Diseños modernos y responsivos para diferentes tipos de correos
 */

const templates = {
  /**
   * Template: Confirmación de Cita
   */
  appointmentConfirmation: (data) => `
    <!DOCTYPE html>
    <html lang="es">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>Confirmación de Cita</title>
      <style>
        * {
          margin: 0;
          padding: 0;
          box-sizing: border-box;
        }
        body {
          font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
          background-color: #f5f7fa;
          line-height: 1.6;
          color: #333;
        }
        .container {
          max-width: 600px;
          margin: 0 auto;
          background: white;
          border-radius: 10px;
          box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
          overflow: hidden;
        }
        .header {
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;
          padding: 40px 20px;
          text-align: center;
        }
        .header h1 {
          font-size: 28px;
          margin-bottom: 5px;
        }
        .header p {
          font-size: 14px;
          opacity: 0.9;
        }
        .content {
          padding: 40px 30px;
        }
        .section {
          margin-bottom: 30px;
        }
        .section h2 {
          color: #667eea;
          font-size: 18px;
          margin-bottom: 15px;
          border-bottom: 2px solid #667eea;
          padding-bottom: 10px;
        }
        .info-block {
          background: #f0f4ff;
          padding: 15px;
          border-radius: 8px;
          margin-bottom: 15px;
          border-left: 4px solid #667eea;
        }
        .info-block strong {
          color: #667eea;
          display: block;
          margin-bottom: 5px;
        }
        .info-block span {
          color: #555;
          font-size: 14px;
        }
        .button {
          display: inline-block;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;
          padding: 12px 30px;
          border-radius: 5px;
          text-decoration: none;
          margin-top: 10px;
          font-weight: bold;
        }
        .button:hover {
          opacity: 0.9;
        }
        .footer {
          background: #f5f7fa;
          padding: 20px 30px;
          text-align: center;
          font-size: 12px;
          color: #777;
          border-top: 1px solid #e0e0e0;
        }
        .footer a {
          color: #667eea;
          text-decoration: none;
        }
        .success-badge {
          display: inline-block;
          background: #4caf50;
          color: white;
          padding: 8px 15px;
          border-radius: 20px;
          font-size: 12px;
          font-weight: bold;
          margin-bottom: 20px;
        }
        .divider {
          height: 2px;
          background: linear-gradient(90deg, transparent, #667eea, transparent);
          margin: 30px 0;
        }
      </style>
    </head>
    <body>
      <div class="container">
        <div class="header">
          <h1>✓ Cita Confirmada</h1>
          <p>Tu cita ha sido confirmada exitosamente</p>
        </div>

        <div class="content">
          <div class="success-badge">✓ Estado: Confirmado</div>

          <div class="section">
            <h2>📋 Detalles de tu Cita</h2>
            <div class="info-block">
              <strong>📅 Fecha</strong>
              <span>${data.date || 'Por confirmar'}</span>
            </div>
            <div class="info-block">
              <strong>🕐 Hora</strong>
              <span>${data.time || 'Por confirmar'}</span>
            </div>
            <div class="info-block">
              <strong>👤 Profesional</strong>
              <span>${data.professional || 'Por asignar'}</span>
            </div>
            <div class="info-block">
              <strong>📍 Ubicación</strong>
              <span>${data.location || 'Sede Principal'}</span>
            </div>
          </div>

          <div class="divider"></div>

          <div class="section">
            <h2>📞 ¿Necesitas Cambios?</h2>
            <p style="margin-bottom: 10px;">Si necesitas reprogramar o cancelar tu cita, contáctanos:</p>
            <p style="font-weight: bold; color: #667eea;">📱 +57 (123) 456-7890</p>
            <p style="font-size: 13px; color: #888; margin-top: 10px;">Estamos disponibles de lunes a viernes, 8:00 AM - 5:00 PM</p>
          </div>

          <div class="divider"></div>

          <div class="section">
            <h2>✨ Recomendaciones</h2>
            <ul style="list-style: none; padding: 0;">
              <li style="margin-bottom: 10px;"><strong>✓</strong> Llega 10 minutos antes de tu cita</li>
              <li style="margin-bottom: 10px;"><strong>✓</strong> Trae tu documento de identidad</li>
              <li style="margin-bottom: 10px;"><strong>✓</strong> Si no puedes asistir, avisa con anticipación</li>
              <li><strong>✓</strong> Consulta nuestras medidas de bioseguridad</li>
            </ul>
          </div>
        </div>

        <div class="footer">
          <p>© 2025 Sistema de Citas ElectroHuila. Todos los derechos reservados.</p>
          <p style="margin-top: 10px;">
            <a href="#">Centro de Ayuda</a> | 
            <a href="#">Política de Privacidad</a> | 
            <a href="#">Contacto</a>
          </p>
        </div>
      </div>
    </body>
    </html>
  `,

  /**
   * Template: Recuperación de Contraseña
   */
  passwordReset: (data) => `
    <!DOCTYPE html>
    <html lang="es">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>Recuperar Contraseña</title>
      <style>
        * {
          margin: 0;
          padding: 0;
          box-sizing: border-box;
        }
        body {
          font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          min-height: 100vh;
          display: flex;
          align-items: center;
          justify-content: center;
          padding: 20px;
        }
        .container {
          max-width: 500px;
          background: white;
          border-radius: 12px;
          box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
          overflow: hidden;
        }
        .header {
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;
          padding: 30px 20px;
          text-align: center;
        }
        .header h1 {
          font-size: 24px;
          margin-bottom: 5px;
        }
        .content {
          padding: 40px 30px;
        }
        .alert {
          background: #fff3cd;
          border-left: 4px solid #ffc107;
          padding: 15px;
          border-radius: 5px;
          margin-bottom: 20px;
          font-size: 14px;
          color: #856404;
        }
        .code-box {
          background: #f0f4ff;
          border: 2px dashed #667eea;
          padding: 20px;
          text-align: center;
          border-radius: 8px;
          margin: 25px 0;
        }
        .code-box .label {
          display: block;
          color: #667eea;
          font-weight: bold;
          margin-bottom: 10px;
          font-size: 12px;
          text-transform: uppercase;
        }
        .code-box .code {
          font-size: 32px;
          letter-spacing: 3px;
          color: #333;
          font-weight: bold;
          font-family: 'Courier New', monospace;
        }
        .instructions {
          background: #f5f7fa;
          padding: 20px;
          border-radius: 8px;
          margin: 20px 0;
        }
        .instructions h3 {
          color: #333;
          margin-bottom: 10px;
          font-size: 14px;
        }
        .instructions ol {
          margin-left: 20px;
        }
        .instructions li {
          margin-bottom: 8px;
          font-size: 13px;
          color: #555;
          line-height: 1.5;
        }
        .warning {
          background: #f8d7da;
          border-left: 4px solid #dc3545;
          padding: 15px;
          border-radius: 5px;
          margin: 20px 0;
          font-size: 13px;
          color: #721c24;
        }
        .footer {
          background: #f5f7fa;
          padding: 20px 30px;
          text-align: center;
          font-size: 12px;
          color: #777;
          border-top: 1px solid #e0e0e0;
        }
        .footer a {
          color: #667eea;
          text-decoration: none;
        }
      </style>
    </head>
    <body>
      <div class="container">
        <div class="header">
          <h1>🔐 Recuperar Contraseña</h1>
          <p>Hemos recibido tu solicitud de recuperación</p>
        </div>

        <div class="content">
          <div class="alert">
            ℹ️ Este código expira en 30 minutos. No lo compartas con nadie.
          </div>

          <p style="margin-bottom: 15px;">Hola ${data.name || 'usuario'},</p>
          <p style="color: #555; font-size: 14px; margin-bottom: 20px;">
            Usa el siguiente código para restablecer tu contraseña en nuestro sistema:
          </p>

          <div class="code-box">
            <span class="label">Código de Recuperación</span>
            <div class="code">${data.code || '000000'}</div>
          </div>

          <div class="instructions">
            <h3>📋 ¿Cómo usar este código?</h3>
            <ol>
              <li>Ve a la página de recuperación de contraseña</li>
              <li>Ingresa tu correo electrónico</li>
              <li>Pega este código en el campo indicado</li>
              <li>Crea tu nueva contraseña segura</li>
              <li>¡Listo! Podrás acceder a tu cuenta</li>
            </ol>
          </div>

          <div class="warning">
            ⚠️ Si no solicitaste este cambio, ignora este mensaje. Tu contraseña permanecerá segura.
          </div>

          <p style="font-size: 12px; color: #777; margin-top: 20px;">
            Si tienes problemas, contacta a nuestro equipo de soporte en support@electrohuila.com
          </p>
        </div>

        <div class="footer">
          <p>© 2025 Sistema de Citas ElectroHuila. Todos los derechos reservados.</p>
          <p style="margin-top: 10px;">
            <a href="#">Ayuda</a> | 
            <a href="#">Seguridad</a> | 
            <a href="#">Contacto</a>
          </p>
        </div>
      </div>
    </body>
    </html>
  `,

  /**
   * Template: Bienvenida
   */
  welcome: (data) => `
    <!DOCTYPE html>
    <html lang="es">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>¡Bienvenido!</title>
      <style>
        * {
          margin: 0;
          padding: 0;
          box-sizing: border-box;
        }
        body {
          font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
          background-color: #f5f7fa;
          line-height: 1.6;
          color: #333;
        }
        .container {
          max-width: 600px;
          margin: 0 auto;
          background: white;
          border-radius: 12px;
          box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
          overflow: hidden;
        }
        .header {
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;
          padding: 50px 20px;
          text-align: center;
        }
        .header h1 {
          font-size: 32px;
          margin-bottom: 10px;
        }
        .header p {
          font-size: 16px;
          opacity: 0.95;
        }
        .content {
          padding: 40px 30px;
        }
        .feature {
          display: flex;
          margin-bottom: 25px;
          padding: 20px;
          background: #f0f4ff;
          border-radius: 8px;
          border-left: 4px solid #667eea;
        }
        .feature-icon {
          font-size: 32px;
          margin-right: 15px;
          min-width: 40px;
        }
        .feature-text h3 {
          color: #333;
          margin-bottom: 5px;
          font-size: 16px;
        }
        .feature-text p {
          color: #666;
          font-size: 13px;
          line-height: 1.5;
        }
        .button-container {
          text-align: center;
          margin: 30px 0;
        }
        .button {
          display: inline-block;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;
          padding: 14px 40px;
          border-radius: 25px;
          text-decoration: none;
          font-weight: bold;
          box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
          transition: transform 0.2s;
        }
        .button:hover {
          transform: translateY(-2px);
        }
        .divider {
          height: 2px;
          background: linear-gradient(90deg, transparent, #667eea, transparent);
          margin: 30px 0;
        }
        .footer {
          background: #f5f7fa;
          padding: 20px 30px;
          text-align: center;
          font-size: 12px;
          color: #777;
          border-top: 1px solid #e0e0e0;
        }
        .footer a {
          color: #667eea;
          text-decoration: none;
        }
      </style>
    </head>
    <body>
      <div class="container">
        <div class="header">
          <h1>🎉 ¡Bienvenido!</h1>
          <p>Tu cuenta ha sido creada exitosamente</p>
        </div>

        <div class="content">
          <p style="margin-bottom: 10px;">Hola <strong>${data.name || 'usuario'}</strong>,</p>
          <p style="color: #666; margin-bottom: 30px;">
            Nos alegra mucho tenerte en nuestra comunidad. A continuación, te mostramos las características principales de nuestro sistema de citas:
          </p>

          <div class="feature">
            <div class="feature-icon">📅</div>
            <div class="feature-text">
              <h3>Agenda tus Citas</h3>
              <p>Selecciona el servicio, fecha y hora que mejor se adapte a tu disponibilidad.</p>
            </div>
          </div>

          <div class="feature">
            <div class="feature-icon">🔔</div>
            <div class="feature-text">
              <h3>Notificaciones</h3>
              <p>Recibe recordatorios automáticos antes de tus citas.</p>
            </div>
          </div>

          <div class="feature">
            <div class="feature-icon">⏱️</div>
            <div class="feature-text">
              <h3>Historial</h3>
              <p>Accede a todas tus citas pasadas y futuras en un solo lugar.</p>
            </div>
          </div>

          <div class="feature">
            <div class="feature-icon">💬</div>
            <div class="feature-text">
              <h3>Soporte</h3>
              <p>Nuestro equipo está disponible para ayudarte en cualquier momento.</p>
            </div>
          </div>

          <div class="divider"></div>

          <div class="button-container">
            <a href="${data.dashboardUrl || '#'}" class="button">Ir a Mi Cuenta →</a>
          </div>

          <p style="text-align: center; font-size: 13px; color: #888;">
            Si tienes preguntas, no dudes en contactarnos en support@electrohuila.com
          </p>
        </div>

        <div class="footer">
          <p>© 2025 Sistema de Citas ElectroHuila. Todos los derechos reservados.</p>
          <p style="margin-top: 10px;">
            <a href="#">Centro de Ayuda</a> | 
            <a href="#">Configuración</a> | 
            <a href="#">Contacto</a>
          </p>
        </div>
      </div>
    </body>
    </html>
  `,

  /**
   * Template: Recordatorio de Cita
   */
  appointmentReminder: (data) => `
    <!DOCTYPE html>
    <html lang="es">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>Recordatorio de Cita</title>
      <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); overflow: hidden; }
        .header { background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 40px 20px; text-align: center; }
        .header h1 { font-size: 28px; margin-bottom: 5px; }
        .header p { font-size: 14px; opacity: 0.9; }
        .content { padding: 40px 30px; }
        .section { margin-bottom: 30px; }
        .section h2 { color: #ff9800; font-size: 18px; margin-bottom: 15px; border-bottom: 2px solid #ff9800; padding-bottom: 10px; }
        .info-block { background: #fff8e1; padding: 15px; border-radius: 8px; margin-bottom: 15px; border-left: 4px solid #ff9800; }
        .info-block strong { color: #ff9800; display: block; margin-bottom: 5px; }
        .info-block span { color: #555; font-size: 14px; }
        .reminder-badge { display: inline-block; background: #ff9800; color: white; padding: 8px 15px; border-radius: 20px; font-size: 12px; font-weight: bold; margin-bottom: 20px; }
        .alert-box { background: #fff3e0; border-left: 4px solid #ff9800; padding: 15px; border-radius: 5px; margin: 20px 0; font-size: 14px; color: #e65100; }
        .footer { background: #f5f7fa; padding: 20px 30px; text-align: center; font-size: 12px; color: #777; border-top: 1px solid #e0e0e0; }
        .footer a { color: #ff9800; text-decoration: none; }
        .divider { height: 2px; background: linear-gradient(90deg, transparent, #ff9800, transparent); margin: 30px 0; }
      </style>
    </head>
    <body>
      <div class="container">
        <div class="header">
          <h1>⏰ Recordatorio de Cita</h1>
          <p>Tu cita está próxima</p>
        </div>

        <div class="content">
          <div class="reminder-badge">⏰ Recordatorio</div>

          <div class="alert-box">
            ⚠️ <strong>¡No olvides tu cita!</strong> Te recordamos que tienes una cita programada próximamente.
          </div>

          <p style="margin-bottom: 15px;">Hola <strong>${data.name || 'usuario'}</strong>,</p>
          <p style="color: #555; margin-bottom: 20px;">Este es un recordatorio amistoso de tu cita programada:</p>

          <div class="section">
            <h2>📋 Detalles de tu Cita</h2>
            <div class="info-block">
              <strong>📅 Fecha</strong>
              <span>${data.date || 'Por confirmar'}</span>
            </div>
            <div class="info-block">
              <strong>🕐 Hora</strong>
              <span>${data.time || 'Por confirmar'}</span>
            </div>
            <div class="info-block">
              <strong>📍 Ubicación</strong>
              <span>${data.location || 'Sede Principal'}</span>
            </div>
            ${data.address ? `
            <div class="info-block">
              <strong>🏢 Dirección</strong>
              <span>${data.address}</span>
            </div>
            ` : ''}
            ${data.appointmentNumber ? `
            <div class="info-block">
              <strong>🎫 Número de Cita</strong>
              <span>${data.appointmentNumber}</span>
            </div>
            ` : ''}
          </div>

          <div class="divider"></div>

          <div class="section">
            <h2>✅ Recomendaciones</h2>
            <ul style="list-style: none; padding: 0;">
              <li style="margin-bottom: 10px;"><strong>✓</strong> Llega 10-15 minutos antes</li>
              <li style="margin-bottom: 10px;"><strong>✓</strong> Trae tu documento de identidad</li>
              <li style="margin-bottom: 10px;"><strong>✓</strong> Si no puedes asistir, cancela con anticipación</li>
            </ul>
          </div>

          <div class="divider"></div>

          <div class="section">
            <h2>📞 Contacto</h2>
            <p style="margin-bottom: 10px;">Si necesitas cancelar o reprogramar:</p>
            <p style="font-weight: bold; color: #ff9800;">📱 +57 (123) 456-7890</p>
            <p style="font-size: 13px; color: #888; margin-top: 10px;">Lunes a viernes, 8:00 AM - 5:00 PM</p>
          </div>
        </div>

        <div class="footer">
          <p>© 2025 Sistema de Citas ElectroHuila. Todos los derechos reservados.</p>
          <p style="margin-top: 10px;">
            <a href="#">Centro de Ayuda</a> |
            <a href="#">Política de Privacidad</a> |
            <a href="#">Contacto</a>
          </p>
        </div>
      </div>
    </body>
    </html>
  `,

  /**
   * Template: Cancelación de Cita
   */
  appointmentCancellation: (data) => `
    <!DOCTYPE html>
    <html lang="es">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>Cancelación de Cita</title>
      <style>
        * {
          margin: 0;
          padding: 0;
          box-sizing: border-box;
        }
        body {
          font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
          background-color: #f5f7fa;
          line-height: 1.6;
          color: #333;
        }
        .container {
          max-width: 600px;
          margin: 0 auto;
          background: white;
          border-radius: 10px;
          box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
          overflow: hidden;
        }
        .header {
          background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
          color: white;
          padding: 40px 20px;
          text-align: center;
        }
        .header h1 {
          font-size: 28px;
          margin-bottom: 5px;
        }
        .header p {
          font-size: 14px;
          opacity: 0.9;
        }
        .content {
          padding: 40px 30px;
        }
        .section {
          margin-bottom: 30px;
        }
        .section h2 {
          color: #f5576c;
          font-size: 18px;
          margin-bottom: 15px;
          border-bottom: 2px solid #f5576c;
          padding-bottom: 10px;
        }
        .info-block {
          background: #fff5f5;
          padding: 15px;
          border-radius: 8px;
          margin-bottom: 15px;
          border-left: 4px solid #f5576c;
        }
        .info-block strong {
          color: #f5576c;
          display: block;
          margin-bottom: 5px;
        }
        .info-block span {
          color: #555;
          font-size: 14px;
        }
        .notice-box {
          background: #fff9e6;
          border-left: 4px solid #ffc107;
          padding: 15px;
          border-radius: 5px;
          margin: 20px 0;
          font-size: 14px;
          color: #856404;
        }
        .button {
          display: inline-block;
          background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
          color: white;
          padding: 12px 30px;
          border-radius: 5px;
          text-decoration: none;
          margin-top: 10px;
          font-weight: bold;
        }
        .button:hover {
          opacity: 0.9;
        }
        .footer {
          background: #f5f7fa;
          padding: 20px 30px;
          text-align: center;
          font-size: 12px;
          color: #777;
          border-top: 1px solid #e0e0e0;
        }
        .footer a {
          color: #f5576c;
          text-decoration: none;
        }
        .divider {
          height: 2px;
          background: linear-gradient(90deg, transparent, #f5576c, transparent);
          margin: 30px 0;
        }
        .cancellation-badge {
          display: inline-block;
          background: #f5576c;
          color: white;
          padding: 8px 15px;
          border-radius: 20px;
          font-size: 12px;
          font-weight: bold;
          margin-bottom: 20px;
        }
        .reason-box {
          background: #f0f4ff;
          padding: 15px;
          border-radius: 8px;
          margin: 20px 0;
          border-left: 4px solid #667eea;
        }
        .reason-box strong {
          color: #667eea;
          display: block;
          margin-bottom: 5px;
        }
        .reschedule-section {
          background: linear-gradient(135deg, #e0e0e0 0%, #f5f5f5 100%);
          padding: 20px;
          border-radius: 8px;
          text-align: center;
          margin: 20px 0;
        }
        .reschedule-section h3 {
          color: #333;
          margin-bottom: 15px;
        }
      </style>
    </head>
    <body>
      <div class="container">
        <div class="header">
          <h1>✗ Cita Cancelada</h1>
          <p>Tu cita ha sido cancelada</p>
        </div>

        <div class="content">
          <div class="cancellation-badge">✗ Estado: Cancelado</div>

          <div class="section">
            <h2>📋 Detalles de la Cita Cancelada</h2>
            <div class="info-block">
              <strong>📅 Fecha</strong>
              <span>${data.date || 'Por confirmar'}</span>
            </div>
            <div class="info-block">
              <strong>🕐 Hora</strong>
              <span>${data.time || 'Por confirmar'}</span>
            </div>
            <div class="info-block">
              <strong>👤 Profesional</strong>
              <span>${data.professional || 'Por asignar'}</span>
            </div>
            <div class="info-block">
              <strong>📍 Ubicación</strong>
              <span>${data.location || 'Sede Principal'}</span>
            </div>
          </div>

          ${data.reason ? `
            <div class="reason-box">
              <strong>💬 Motivo de la Cancelación:</strong>
              <span>${data.reason}</span>
            </div>
          ` : ''}

          <div class="divider"></div>

          <div class="reschedule-section">
            <h3>📅 ¿Deseas Agendar una Nueva Cita?</h3>
            <p style="margin-bottom: 15px; font-size: 14px;">No te preocupes, puedes agendar una nueva cita en cualquier momento.</p>
            <a href="${data.schedulingUrl || '#'}" class="button">Agendar Nueva Cita →</a>
          </div>

          <div class="divider"></div>

          <div class="section">
            <h2>📞 ¿Necesitas Ayuda?</h2>
            <p style="margin-bottom: 10px;">Si tienes preguntas sobre esta cancelación o necesitas asistencia:</p>
            <p style="font-weight: bold; color: #f5576c;">📱 +57 (123) 456-7890</p>
            <p style="font-size: 13px; color: #888; margin-top: 10px;">Disponible de lunes a viernes, 8:00 AM - 5:00 PM</p>
          </div>

          <div class="notice-box">
            ⚠️ Por favor, no responda a este correo. Si tiene preguntas, contáctenos directamente usando los datos arriba.
          </div>
        </div>

        <div class="footer">
          <p>© 2025 Sistema de Citas ElectroHuila. Todos los derechos reservados.</p>
          <p style="margin-top: 10px;">
            <a href="#">Centro de Ayuda</a> | 
            <a href="#">Política de Privacidad</a> | 
            <a href="#">Contacto</a>
          </p>
        </div>
      </div>
    </body>
    </html>
  `
};

module.exports = templates;
