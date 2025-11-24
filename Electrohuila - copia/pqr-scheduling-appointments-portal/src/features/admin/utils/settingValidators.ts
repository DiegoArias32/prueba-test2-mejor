/**
 * Setting Validators
 * Type-specific validation functions for system settings
 */

import type { ValidationResult } from '@/services/system/system-setting.types';

/**
 * Validate number value
 * @param value - Value to validate
 * @param min - Minimum value (optional)
 * @param max - Maximum value (optional)
 * @returns Validation result
 */
export function validateNumber(
  value: string,
  min?: number,
  max?: number
): ValidationResult {
  if (!value || value.trim() === '') {
    return { isValid: false, error: 'El valor es requerido' };
  }

  const numValue = Number(value);

  if (isNaN(numValue)) {
    return { isValid: false, error: 'Debe ser un número válido' };
  }

  if (min !== undefined && numValue < min) {
    return { isValid: false, error: `El valor mínimo es ${min}` };
  }

  if (max !== undefined && numValue > max) {
    return { isValid: false, error: `El valor máximo es ${max}` };
  }

  return { isValid: true };
}

/**
 * Validate boolean value
 * @param value - Value to validate
 * @returns Validation result
 */
export function validateBoolean(value: string): ValidationResult {
  const lowerValue = value.toLowerCase();

  if (lowerValue !== 'true' && lowerValue !== 'false') {
    return { isValid: false, error: 'Debe ser "true" o "false"' };
  }

  return { isValid: true };
}

/**
 * Validate time value (HH:mm format)
 * @param value - Value to validate
 * @returns Validation result
 */
export function validateTime(value: string): ValidationResult {
  if (!value || value.trim() === '') {
    return { isValid: false, error: 'La hora es requerida' };
  }

  // Regex for HH:mm format (24-hour)
  const timeRegex = /^([01]\d|2[0-3]):([0-5]\d)$/;

  if (!timeRegex.test(value)) {
    return { isValid: false, error: 'Formato inválido. Use HH:mm (ej: 08:00)' };
  }

  return { isValid: true };
}

/**
 * Validate JSON value
 * @param value - Value to validate
 * @returns Validation result
 */
export function validateJson(value: string): ValidationResult {
  if (!value || value.trim() === '') {
    return { isValid: false, error: 'El JSON es requerido' };
  }

  try {
    JSON.parse(value);
    return { isValid: true };
  } catch (error) {
    return { isValid: false, error: 'JSON inválido' };
  }
}

/**
 * Validate string value
 * @param value - Value to validate
 * @param minLength - Minimum length (optional)
 * @param maxLength - Maximum length (optional)
 * @returns Validation result
 */
export function validateString(
  value: string,
  minLength?: number,
  maxLength?: number
): ValidationResult {
  if (!value || value.trim() === '') {
    return { isValid: false, error: 'El valor es requerido' };
  }

  if (minLength !== undefined && value.length < minLength) {
    return { isValid: false, error: `Longitud mínima: ${minLength} caracteres` };
  }

  if (maxLength !== undefined && value.length > maxLength) {
    return { isValid: false, error: `Longitud máxima: ${maxLength} caracteres` };
  }

  return { isValid: true };
}

/**
 * Validate setting key format
 * @param key - Key to validate
 * @returns Validation result
 */
export function validateSettingKey(key: string): ValidationResult {
  if (!key || key.trim() === '') {
    return { isValid: false, error: 'La clave es requerida' };
  }

  // Allow alphanumeric, underscores, and hyphens
  const keyRegex = /^[A-Z0-9_-]+$/;

  if (!keyRegex.test(key)) {
    return {
      isValid: false,
      error: 'Solo mayúsculas, números, guiones bajos y guiones',
    };
  }

  return { isValid: true };
}

/**
 * Get validation constraints for specific setting keys
 * @param key - Setting key
 * @returns Validation constraints
 */
export function getSettingConstraints(key: string): {
  min?: number;
  max?: number;
  required?: boolean;
} {
  const constraints: Record<string, { min?: number; max?: number; required?: boolean }> = {
    MAX_APPOINTMENTS_PER_DAY: { min: 1, max: 500, required: true },
    APPOINTMENT_CANCELLATION_HOURS: { min: 0, max: 72, required: true },
    MAX_RESCHEDULE_ATTEMPTS: { min: 0, max: 10, required: true },
    APPOINTMENT_REMINDER_HOURS: { min: 0, max: 168, required: true }, // 7 days max
  };

  return constraints[key] || {};
}

/**
 * Validate setting value based on key and type
 * @param key - Setting key
 * @param value - Setting value
 * @param type - Setting type
 * @returns Validation result
 */
export function validateSettingValue(
  key: string,
  value: string,
  type: 'String' | 'Number' | 'Boolean' | 'Time' | 'Json'
): ValidationResult {
  const constraints = getSettingConstraints(key);

  switch (type) {
    case 'Number':
      return validateNumber(value, constraints.min, constraints.max);

    case 'Boolean':
      return validateBoolean(value);

    case 'Time':
      return validateTime(value);

    case 'Json':
      return validateJson(value);

    case 'String':
    default:
      return validateString(value);
  }
}
