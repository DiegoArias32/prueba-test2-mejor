# Ejemplos de Uso - Plantillas de Notificación

## Ejemplos Rápidos

### 1. Crear una Plantilla de Confirmación (Email)

```typescript
import { notificationTemplateService } from '@/services';

const createConfirmationTemplate = async () => {
  const template = await notificationTemplateService.createNotificationTemplate({
    templateCode: 'APPT_CONFIRMATION',
    templateName: 'Confirmación de Cita',
    templateType: 'Email',
    subject: 'Confirmación de tu cita - {{APPOINTMENT_DATE}}',
    bodyTemplate: `
Estimado/a {{CLIENT_NAME}},

Tu cita ha sido confirmada exitosamente.

📋 Detalles de la cita:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
• Tipo de cita: {{APPOINTMENT_TYPE}}
• Fecha: {{APPOINTMENT_DATE}}
• Hora: {{APPOINTMENT_TIME}}
• Número de confirmación: {{APPOINTMENT_NUMBER}}

📍 Ubicación:
{{BRANCH_NAME}}
{{BRANCH_ADDRESS}}
📞 Teléfono: {{BRANCH_PHONE}}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

⚠️ Importante:
• Por favor llega 10 minutos antes de tu cita
• Si necesitas cancelar o reagendar, contáctanos con al menos 24 horas de anticipación

¡Te esperamos!

Saludos,
El equipo de atención
    `
  });

  console.log('Plantilla creada:', template);
};
```

### 2. Crear una Plantilla de Recordatorio (SMS)

```typescript
import { notificationTemplateService } from '@/services';

const createReminderSMS = async () => {
  const template = await notificationTemplateService.createNotificationTemplate({
    templateCode: 'APPT_REMINDER_SMS',
    templateName: 'Recordatorio SMS',
    templateType: 'SMS',
    subject: '', // No se usa en SMS
    bodyTemplate: '🔔 Recordatorio: Tienes cita mañana {{APPOINTMENT_DATE}} a las {{APPOINTMENT_TIME}} en {{BRANCH_NAME}}. Cita #{{APPOINTMENT_NUMBER}}'
  });

  console.log('SMS template creado:', template);
};
```

### 3. Crear una Plantilla Push

```typescript
import { notificationTemplateService } from '@/services';

const createPushNotification = async () => {
  const template = await notificationTemplateService.createNotificationTemplate({
    templateCode: 'APPT_REMINDER_PUSH',
    templateName: 'Recordatorio Push',
    templateType: 'Push',
    subject: 'Recordatorio de Cita',
    bodyTemplate: 'Tu cita de {{APPOINTMENT_TYPE}} es mañana a las {{APPOINTMENT_TIME}}. No olvides asistir!'
  });

  console.log('Push notification creada:', template);
};
```

### 4. Actualizar una Plantilla

```typescript
import { notificationTemplateService } from '@/services';

const updateTemplate = async (templateId: number) => {
  const template = await notificationTemplateService.updateNotificationTemplate({
    id: templateId,
    templateCode: 'APPT_CONFIRMATION',
    templateName: 'Confirmación de Cita Actualizada',
    templateType: 'Email',
    subject: '✅ Confirmación de tu cita - {{APPOINTMENT_DATE}}',
    bodyTemplate: `
Hola {{CLIENT_NAME}},

¡Excelente noticia! Tu cita está confirmada.

Detalles:
• {{APPOINTMENT_TYPE}}
• {{APPOINTMENT_DATE}} a las {{APPOINTMENT_TIME}}
• {{BRANCH_NAME}}

¡Nos vemos pronto!
    `
  });

  console.log('Plantilla actualizada:', template);
};
```

### 5. Obtener Todas las Plantillas

```typescript
import { notificationTemplateService } from '@/services';

const getAllTemplates = async () => {
  const templates = await notificationTemplateService.getNotificationTemplates();

  console.log('Total de plantillas:', templates.length);

  // Filtrar por tipo
  const emailTemplates = templates.filter(t => t.templateType === 'Email');
  const smsTemplates = templates.filter(t => t.templateType === 'SMS');
  const pushTemplates = templates.filter(t => t.templateType === 'Push');

  console.log('Emails:', emailTemplates.length);
  console.log('SMS:', smsTemplates.length);
  console.log('Push:', pushTemplates.length);
};
```

### 6. Obtener Plantilla por Código

```typescript
import { notificationTemplateService } from '@/services';

const getTemplateByCode = async (code: string) => {
  try {
    const template = await notificationTemplateService.getNotificationTemplateByCode(code);
    console.log('Plantilla encontrada:', template);
    return template;
  } catch (error) {
    console.error('Plantilla no encontrada:', code);
    return null;
  }
};

// Uso
getTemplateByCode('APPT_CONFIRMATION');
```

### 7. Validar Placeholders

```typescript
import { notificationTemplateService, AVAILABLE_PLACEHOLDERS } from '@/services';

const validateTemplateContent = (content: string) => {
  const invalidPlaceholders = notificationTemplateService.validatePlaceholders(
    content,
    AVAILABLE_PLACEHOLDERS
  );

  if (invalidPlaceholders.length > 0) {
    console.warn('⚠️ Placeholders inválidos encontrados:', invalidPlaceholders);
    return false;
  }

  console.log('✅ Todos los placeholders son válidos');
  return true;
};

// Ejemplo
const template = 'Hola {{CLIENT_NAME}}, tu cita es el {{INVALID_DATE}}';
validateTemplateContent(template);
// Output: ⚠️ Placeholders inválidos encontrados: ['{{INVALID_DATE}}']
```

### 8. Reemplazar Placeholders con Datos Reales

```typescript
import { notificationTemplateService } from '@/services';

const generateMessage = (template: string, appointmentData: any) => {
  const data = {
    CLIENT_NAME: appointmentData.clientName,
    APPOINTMENT_TYPE: appointmentData.type,
    APPOINTMENT_DATE: appointmentData.date,
    APPOINTMENT_TIME: appointmentData.time,
    APPOINTMENT_NUMBER: appointmentData.number,
    BRANCH_NAME: appointmentData.branch.name,
    BRANCH_ADDRESS: appointmentData.branch.address,
    BRANCH_PHONE: appointmentData.branch.phone,
    CANCELLATION_REASON: appointmentData.cancellationReason || ''
  };

  return notificationTemplateService.replacePlaceholders(template, data);
};

// Ejemplo de uso
const appointmentData = {
  clientName: 'María González',
  type: 'Consulta General',
  date: '20/03/2024',
  time: '14:30',
  number: 'APT-2024-5678',
  branch: {
    name: 'Sede Norte',
    address: 'Av. Principal 123, Piso 2',
    phone: '+57 300 555 1234'
  }
};

const message = generateMessage(
  'Hola {{CLIENT_NAME}}, tu cita de {{APPOINTMENT_TYPE}} es el {{APPOINTMENT_DATE}} a las {{APPOINTMENT_TIME}}.',
  appointmentData
);

console.log(message);
// Output: "Hola María González, tu cita de Consulta General es el 20/03/2024 a las 14:30."
```

### 9. Extraer Placeholders de un Template

```typescript
import { notificationTemplateService } from '@/services';

const analyzeTemplate = (template: string) => {
  const placeholders = notificationTemplateService.extractPlaceholders(template);
  const count = notificationTemplateService.getPlaceholderCount(template);

  console.log('Placeholders únicos:', placeholders);
  console.log('Total de placeholders:', count);

  return {
    placeholders,
    count
  };
};

// Ejemplo
const template = 'Hola {{CLIENT_NAME}}, tu cita {{APPOINTMENT_NUMBER}} de {{APPOINTMENT_TYPE}} es el {{APPOINTMENT_DATE}}. {{CLIENT_NAME}} recuerda llegar a tiempo.';

const analysis = analyzeTemplate(template);
// Output:
// Placeholders únicos: ['{{CLIENT_NAME}}', '{{APPOINTMENT_NUMBER}}', '{{APPOINTMENT_TYPE}}', '{{APPOINTMENT_DATE}}']
// Total de placeholders: 5
```

### 10. Enviar Notificación con Plantilla

```typescript
import { notificationTemplateService } from '@/services';

// Función helper completa para enviar notificaciones
const sendNotification = async (
  templateCode: string,
  appointmentData: any,
  recipient: { email?: string; phone?: string; deviceToken?: string }
) => {
  try {
    // 1. Obtener la plantilla
    const template = await notificationTemplateService.getNotificationTemplateByCode(templateCode);

    if (!template.isActive) {
      throw new Error('La plantilla no está activa');
    }

    // 2. Preparar datos para reemplazo
    const data = {
      CLIENT_NAME: appointmentData.clientName,
      APPOINTMENT_TYPE: appointmentData.type,
      APPOINTMENT_DATE: appointmentData.date,
      APPOINTMENT_TIME: appointmentData.time,
      APPOINTMENT_NUMBER: appointmentData.number,
      BRANCH_NAME: appointmentData.branch.name,
      BRANCH_ADDRESS: appointmentData.branch.address,
      BRANCH_PHONE: appointmentData.branch.phone,
      CANCELLATION_REASON: appointmentData.cancellationReason || ''
    };

    // 3. Reemplazar placeholders
    const subject = notificationTemplateService.replacePlaceholders(template.subject, data);
    const body = notificationTemplateService.replacePlaceholders(template.bodyTemplate, data);

    // 4. Enviar según el tipo
    let result;
    switch (template.templateType) {
      case 'Email':
        if (!recipient.email) throw new Error('Email requerido');
        result = await sendEmail(recipient.email, subject, body);
        break;

      case 'SMS':
        if (!recipient.phone) throw new Error('Teléfono requerido');
        result = await sendSMS(recipient.phone, body);
        break;

      case 'Push':
        if (!recipient.deviceToken) throw new Error('Device token requerido');
        result = await sendPushNotification(recipient.deviceToken, subject, body);
        break;
    }

    console.log('✅ Notificación enviada exitosamente:', result);
    return result;

  } catch (error) {
    console.error('❌ Error enviando notificación:', error);
    throw error;
  }
};

// Funciones mock (implementar con tu servicio real)
const sendEmail = async (to: string, subject: string, body: string) => {
  console.log('Sending email to:', to);
  console.log('Subject:', subject);
  console.log('Body:', body);
  return { status: 'sent', type: 'email' };
};

const sendSMS = async (to: string, message: string) => {
  console.log('Sending SMS to:', to);
  console.log('Message:', message);
  return { status: 'sent', type: 'sms' };
};

const sendPushNotification = async (token: string, title: string, body: string) => {
  console.log('Sending push to:', token);
  console.log('Title:', title);
  console.log('Body:', body);
  return { status: 'sent', type: 'push' };
};

// Uso
sendNotification('APPT_CONFIRMATION', appointmentData, {
  email: 'maria.gonzalez@email.com'
});
```

## Flujo Completo de Trabajo

### Escenario: Sistema de Recordatorios Automáticos

```typescript
import { notificationTemplateService } from '@/services';

class NotificationSystem {
  /**
   * Envía recordatorio 24 horas antes de la cita
   */
  async sendReminderNotifications(appointments: any[]) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    for (const appointment of appointments) {
      try {
        // Determinar canal de notificación preferido
        const channel = appointment.client.preferredChannel || 'Email';

        // Seleccionar código de plantilla según canal
        const templateCode = channel === 'SMS'
          ? 'APPT_REMINDER_SMS'
          : 'APPT_REMINDER';

        // Preparar datos
        const data = {
          CLIENT_NAME: appointment.client.name,
          APPOINTMENT_TYPE: appointment.type.name,
          APPOINTMENT_DATE: this.formatDate(appointment.date),
          APPOINTMENT_TIME: this.formatTime(appointment.time),
          APPOINTMENT_NUMBER: appointment.number,
          BRANCH_NAME: appointment.branch.name,
          BRANCH_ADDRESS: appointment.branch.address,
          BRANCH_PHONE: appointment.branch.phone
        };

        // Obtener plantilla
        const template = await notificationTemplateService.getNotificationTemplateByCode(templateCode);

        // Generar mensaje
        const message = notificationTemplateService.replacePlaceholders(
          template.bodyTemplate,
          data
        );

        // Enviar notificación
        if (channel === 'Email') {
          await this.sendEmail(
            appointment.client.email,
            notificationTemplateService.replacePlaceholders(template.subject, data),
            message
          );
        } else if (channel === 'SMS') {
          await this.sendSMS(appointment.client.phone, message);
        }

        console.log(`✅ Recordatorio enviado a ${appointment.client.name}`);

      } catch (error) {
        console.error(`❌ Error enviando recordatorio a ${appointment.client.name}:`, error);
      }
    }
  }

  /**
   * Envía confirmación inmediata después de agendar
   */
  async sendConfirmation(appointment: any) {
    const template = await notificationTemplateService.getNotificationTemplateByCode('APPT_CONFIRMATION');

    const data = {
      CLIENT_NAME: appointment.client.name,
      APPOINTMENT_TYPE: appointment.type.name,
      APPOINTMENT_DATE: this.formatDate(appointment.date),
      APPOINTMENT_TIME: this.formatTime(appointment.time),
      APPOINTMENT_NUMBER: appointment.number,
      BRANCH_NAME: appointment.branch.name,
      BRANCH_ADDRESS: appointment.branch.address,
      BRANCH_PHONE: appointment.branch.phone
    };

    const subject = notificationTemplateService.replacePlaceholders(template.subject, data);
    const body = notificationTemplateService.replacePlaceholders(template.bodyTemplate, data);

    await this.sendEmail(appointment.client.email, subject, body);
  }

  /**
   * Envía notificación de cancelación
   */
  async sendCancellationNotice(appointment: any, reason: string) {
    const template = await notificationTemplateService.getNotificationTemplateByCode('APPT_CANCELLATION');

    const data = {
      CLIENT_NAME: appointment.client.name,
      APPOINTMENT_TYPE: appointment.type.name,
      APPOINTMENT_DATE: this.formatDate(appointment.date),
      APPOINTMENT_TIME: this.formatTime(appointment.time),
      APPOINTMENT_NUMBER: appointment.number,
      BRANCH_NAME: appointment.branch.name,
      BRANCH_ADDRESS: appointment.branch.address,
      BRANCH_PHONE: appointment.branch.phone,
      CANCELLATION_REASON: reason
    };

    const subject = notificationTemplateService.replacePlaceholders(template.subject, data);
    const body = notificationTemplateService.replacePlaceholders(template.bodyTemplate, data);

    // Enviar por email y SMS
    await Promise.all([
      this.sendEmail(appointment.client.email, subject, body),
      this.sendSMS(appointment.client.phone, body.substring(0, 160))
    ]);
  }

  // Helper methods
  private formatDate(date: Date): string {
    return date.toLocaleDateString('es-ES', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  private formatTime(time: string): string {
    return time;
  }

  private async sendEmail(to: string, subject: string, body: string) {
    // Implementación real
  }

  private async sendSMS(to: string, message: string) {
    // Implementación real
  }
}

// Uso
const notificationSystem = new NotificationSystem();

// Enviar recordatorios diarios
setInterval(async () => {
  const tomorrowAppointments = await getTomorrowAppointments();
  await notificationSystem.sendReminderNotifications(tomorrowAppointments);
}, 24 * 60 * 60 * 1000); // Cada 24 horas
```

## Tips y Mejores Prácticas

### 1. Manejo de Errores

```typescript
const safeTemplateOperation = async (operation: () => Promise<any>) => {
  try {
    return await operation();
  } catch (error) {
    if (error.message.includes('404')) {
      console.error('Plantilla no encontrada');
    } else if (error.message.includes('400')) {
      console.error('Datos inválidos');
    } else {
      console.error('Error inesperado:', error);
    }
    throw error;
  }
};
```

### 2. Cache de Plantillas

```typescript
class TemplateCache {
  private cache: Map<string, any> = new Map();
  private ttl = 5 * 60 * 1000; // 5 minutos

  async getTemplate(code: string) {
    const cached = this.cache.get(code);
    if (cached && Date.now() - cached.timestamp < this.ttl) {
      return cached.template;
    }

    const template = await notificationTemplateService.getNotificationTemplateByCode(code);
    this.cache.set(code, {
      template,
      timestamp: Date.now()
    });

    return template;
  }

  clear() {
    this.cache.clear();
  }
}
```

### 3. Validación Antes de Guardar

```typescript
const validateBeforeSave = (template: any) => {
  const errors: string[] = [];

  if (!template.templateCode) {
    errors.push('Código requerido');
  }

  if (template.templateType === 'Email' && !template.subject) {
    errors.push('Subject requerido para emails');
  }

  const invalid = notificationTemplateService.validatePlaceholders(
    template.bodyTemplate,
    AVAILABLE_PLACEHOLDERS
  );

  if (invalid.length > 0) {
    errors.push(`Placeholders inválidos: ${invalid.join(', ')}`);
  }

  return {
    isValid: errors.length === 0,
    errors
  };
};
```

---

**Última actualización:** 2024-03-15
