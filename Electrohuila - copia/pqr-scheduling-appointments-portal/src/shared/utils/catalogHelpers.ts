/**
 * Utilidades para trabajar con catálogos dinámicos del sistema
 */

import { AppointmentStatusDto } from '@/services/api';

/**
 * Convierte un color hexadecimal a una clase de Tailwind
 * Si el color ya existe en Tailwind, lo usa; sino, usa style inline
 */
export function getColorClass(hexColor?: string, prefix: 'bg' | 'text' | 'border' = 'bg'): string {
  if (!hexColor) return `${prefix}-gray-500`;

  // Mapeo de colores comunes de Tailwind
  const tailwindColors: { [key: string]: string } = {
    '#EF4444': `${prefix}-red-500`,
    '#F59E0B': `${prefix}-amber-500`,
    '#EAB308': `${prefix}-yellow-500`,
    '#84CC16': `${prefix}-lime-500`,
    '#22C55E': `${prefix}-green-500`,
    '#10B981': `${prefix}-emerald-500`,
    '#14B8A6': `${prefix}-teal-500`,
    '#06B6D4': `${prefix}-cyan-500`,
    '#0EA5E9': `${prefix}-sky-500`,
    '#3B82F6': `${prefix}-blue-500`,
    '#6366F1': `${prefix}-indigo-500`,
    '#8B5CF6': `${prefix}-violet-500`,
    '#A855F7': `${prefix}-purple-500`,
    '#D946EF': `${prefix}-fuchsia-500`,
    '#EC4899': `${prefix}-pink-500`,
    '#F43F5E': `${prefix}-rose-500`,
    '#6B7280': `${prefix}-gray-500`,
  };

  return tailwindColors[hexColor.toUpperCase()] || `${prefix}-gray-500`;
}

/**
 * Obtiene el estilo inline para colores personalizados
 */
export function getColorStyle(hexColor?: string, property: 'backgroundColor' | 'color' | 'borderColor' = 'backgroundColor'): React.CSSProperties {
  if (!hexColor) return {};
  return { [property]: hexColor };
}

/**
 * Obtiene el color de un estado de cita de forma dinámica
 */
export function getAppointmentStatusColor(
  statusCode: string,
  statuses: AppointmentStatusDto[]
): { bg: string; text: string; style: React.CSSProperties } {
  const status = statuses.find(s => s.code === statusCode);

  if (!status) {
    return {
      bg: 'bg-gray-500',
      text: 'text-white',
      style: {}
    };
  }

  return {
    bg: getColorClass(status.colorPrimary, 'bg'),
    text: getColorClass(status.colorText, 'text'),
    style: {
      backgroundColor: status.colorPrimary,
      color: status.colorText || '#FFFFFF'
    }
  };
}

/**
 * Obtiene el ícono de FontAwesome para un estado
 */
export function getAppointmentStatusIcon(
  statusCode: string,
  statuses: AppointmentStatusDto[]
): string {
  const status = statuses.find(s => s.code === statusCode);
  return status?.iconName || 'calendar';
}

/**
 * Obtiene el nombre legible de un estado
 */
export function getAppointmentStatusName(
  statusCode: string,
  statuses: AppointmentStatusDto[]
): string {
  const status = statuses.find(s => s.code === statusCode);
  return status?.name || statusCode;
}
