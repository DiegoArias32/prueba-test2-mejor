/**
 * Utility for parsing backend error messages
 * Handles error codes in format: CODE|User message
 */

export interface ParsedError {
  code: string;
  message: string;
  isExpectedValidation?: boolean;
}

/**
 * Known error codes that represent expected validation errors
 * These should not trigger console errors as they are part of normal business logic
 */
export const EXPECTED_VALIDATION_CODES = [
  'HOLIDAY_NOT_AVAILABLE',
  'SUNDAY_NOT_AVAILABLE',
  'PAST_DATE_NOT_AVAILABLE',
  'NO_HOURS_AVAILABLE',
  'DUPLICATE_APPOINTMENT',
  'CLIENT_NOT_FOUND',
  'OUTSIDE_BUSINESS_HOURS'
] as const;

export type ExpectedValidationCode = typeof EXPECTED_VALIDATION_CODES[number];

/**
 * Parses error message from backend in format: CODE|User message
 * Falls back to generic error message if format doesn't match
 */
export function parseErrorMessage(errorMessage: string | null | undefined): ParsedError {
  if (!errorMessage || typeof errorMessage !== 'string') {
    return {
      code: 'UNKNOWN_ERROR',
      message: 'Ocurrió un error inesperado',
      isExpectedValidation: false,
    };
  }

  if (errorMessage.includes('|')) {
    const [code, ...messageParts] = errorMessage.split('|');
    const message = messageParts.join('|').trim();
    const trimmedCode = code.trim();
    return {
      code: trimmedCode,
      message: message || 'Ocurrió un error',
      isExpectedValidation: EXPECTED_VALIDATION_CODES.includes(trimmedCode as ExpectedValidationCode),
    };
  }

  return {
    code: 'UNKNOWN_ERROR',
    message: errorMessage.trim(),
    isExpectedValidation: false,
  };
}

/**
 * Checks if an error code represents an expected validation error
 */
export function isExpectedValidationError(errorCode: string): boolean {
  return EXPECTED_VALIDATION_CODES.includes(errorCode as ExpectedValidationCode);
}

/**
 * Gets helpful hint based on error code
 */
export function getErrorHint(errorCode: string): string | null {
  const hints: Record<string, string> = {
    'SUNDAY_NOT_AVAILABLE': 'Por favor seleccione un día entre lunes y sábado',
    'HOLIDAY_NOT_AVAILABLE': 'Por favor seleccione otra fecha',
    'PAST_DATE_NOT_AVAILABLE': 'Seleccione una fecha desde hoy en adelante',
  };

  return hints[errorCode] || null;
}

/**
 * Gets icon for error code
 */
export function getErrorIcon(errorCode: string): string {
  const icons: Record<string, string> = {
    'SUNDAY_NOT_AVAILABLE': '📅',
    'HOLIDAY_NOT_AVAILABLE': '🎉',
    'PAST_DATE_NOT_AVAILABLE': '⏰',
  };

  return icons[errorCode] || '⚠️';
}

/**
 * Gets color class for error code
 */
export function getErrorColorClass(errorCode: string): string {
  const colors: Record<string, string> = {
    'SUNDAY_NOT_AVAILABLE': 'text-amber-600',
    'HOLIDAY_NOT_AVAILABLE': 'text-red-600',
    'PAST_DATE_NOT_AVAILABLE': 'text-gray-600',
  };

  return colors[errorCode] || 'text-gray-600';
}

/**
 * Gets background color class for error code
 */
export function getErrorBgClass(errorCode: string): string {
  const bgColors: Record<string, string> = {
    'SUNDAY_NOT_AVAILABLE': 'bg-amber-50',
    'HOLIDAY_NOT_AVAILABLE': 'bg-red-50',
    'PAST_DATE_NOT_AVAILABLE': 'bg-gray-50',
  };

  return bgColors[errorCode] || 'bg-gray-50';
}

/**
 * Gets border color class for error code
 */
export function getErrorBorderClass(errorCode: string): string {
  const borders: Record<string, string> = {
    'SUNDAY_NOT_AVAILABLE': 'border-amber-200',
    'HOLIDAY_NOT_AVAILABLE': 'border-red-200',
    'PAST_DATE_NOT_AVAILABLE': 'border-gray-200',
  };

  return borders[errorCode] || 'border-gray-200';
}
