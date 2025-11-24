/**
 * Notification Template Types
 * Type definitions for notification template management
 */

/**
 * Notification template entity
 */
export interface NotificationTemplateDto {
  id: number;
  templateCode: string; // APPT_CONFIRMATION, APPT_REMINDER, etc.
  templateName: string;
  subject: string; // Para emails
  bodyTemplate: string; // Texto con placeholders
  templateType: 'Email' | 'SMS' | 'Push';
  placeholders: string; // JSON array de placeholders disponibles
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

/**
 * DTO for creating a new notification template
 */
export interface CreateNotificationTemplateDto {
  templateCode: string;
  templateName: string;
  subject: string;
  bodyTemplate: string;
  templateType: 'Email' | 'SMS' | 'Push';
  placeholders?: string;
}

/**
 * DTO for updating an existing notification template
 */
export interface UpdateNotificationTemplateDto {
  id: number;
  templateCode: string;
  templateName: string;
  subject: string;
  bodyTemplate: string;
  templateType: 'Email' | 'SMS' | 'Push';
  placeholders?: string;
}

/**
 * Available placeholders for templates
 */
export const AVAILABLE_PLACEHOLDERS = [
  '{{CLIENT_NAME}}',
  '{{APPOINTMENT_TYPE}}',
  '{{APPOINTMENT_DATE}}',
  '{{APPOINTMENT_TIME}}',
  '{{BRANCH_NAME}}',
  '{{BRANCH_ADDRESS}}',
  '{{BRANCH_PHONE}}',
  '{{CANCELLATION_REASON}}',
  '{{APPOINTMENT_NUMBER}}'
] as const;

/**
 * Placeholder type
 */
export type PlaceholderType = typeof AVAILABLE_PLACEHOLDERS[number];

/**
 * Placeholder metadata with descriptions
 */
export interface PlaceholderInfo {
  code: PlaceholderType;
  description: string;
  category: 'Cliente' | 'Cita' | 'Sucursal';
  example: string;
}

/**
 * Categorized placeholders
 */
export const PLACEHOLDER_INFO: PlaceholderInfo[] = [
  // Cliente
  {
    code: '{{CLIENT_NAME}}',
    description: 'Nombre completo del cliente',
    category: 'Cliente',
    example: 'Juan Pérez'
  },

  // Cita
  {
    code: '{{APPOINTMENT_TYPE}}',
    description: 'Tipo de cita',
    category: 'Cita',
    example: 'Consulta General'
  },
  {
    code: '{{APPOINTMENT_DATE}}',
    description: 'Fecha de la cita',
    category: 'Cita',
    example: '15/03/2024'
  },
  {
    code: '{{APPOINTMENT_TIME}}',
    description: 'Hora de la cita',
    category: 'Cita',
    example: '10:30 AM'
  },
  {
    code: '{{APPOINTMENT_NUMBER}}',
    description: 'Número de cita',
    category: 'Cita',
    example: 'APT-2024-001'
  },
  {
    code: '{{CANCELLATION_REASON}}',
    description: 'Razón de cancelación',
    category: 'Cita',
    example: 'Conflicto de horario'
  },

  // Sucursal
  {
    code: '{{BRANCH_NAME}}',
    description: 'Nombre de la sucursal',
    category: 'Sucursal',
    example: 'Sede Principal'
  },
  {
    code: '{{BRANCH_ADDRESS}}',
    description: 'Dirección de la sucursal',
    category: 'Sucursal',
    example: 'Calle 123 #45-67'
  },
  {
    code: '{{BRANCH_PHONE}}',
    description: 'Teléfono de la sucursal',
    category: 'Sucursal',
    example: '+57 300 123 4567'
  }
];

/**
 * Sample data for template preview
 */
export const PREVIEW_DATA = {
  CLIENT_NAME: 'Juan Pérez García',
  APPOINTMENT_TYPE: 'Consulta General',
  APPOINTMENT_DATE: '15/03/2024',
  APPOINTMENT_TIME: '10:30 AM',
  BRANCH_NAME: 'Sede Principal',
  BRANCH_ADDRESS: 'Calle 123 #45-67, Bogotá',
  BRANCH_PHONE: '+57 300 123 4567',
  CANCELLATION_REASON: 'Conflicto de horario',
  APPOINTMENT_NUMBER: 'APT-2024-001234'
};

/**
 * Template type options
 */
export const TEMPLATE_TYPE_OPTIONS = [
  { value: 'Email', label: 'Email', icon: '📧', color: 'blue' },
  { value: 'SMS', label: 'SMS', icon: '💬', color: 'green' },
  { value: 'Push', label: 'Push', icon: '🔔', color: 'purple' }
] as const;

/**
 * Predefined template codes
 */
export const PREDEFINED_TEMPLATES = [
  { code: 'APPT_CONFIRMATION', name: 'Confirmación de Cita' },
  { code: 'APPT_REMINDER', name: 'Recordatorio de Cita (24h)' },
  { code: 'APPT_REMINDER_SMS', name: 'Recordatorio SMS' },
  { code: 'APPT_CANCELLATION', name: 'Cancelación de Cita' },
  { code: 'APPT_RESCHEDULED', name: 'Cita Reagendada' },
  { code: 'APPT_COMPLETED', name: 'Cita Completada' },
  { code: 'APPT_NO_SHOW', name: 'Inasistencia a Cita' }
] as const;
