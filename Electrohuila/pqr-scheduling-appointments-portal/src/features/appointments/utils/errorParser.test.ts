/**
 * Tests for error parser utility
 */

import {
  parseErrorMessage,
  getErrorHint,
  getErrorIcon,
  getErrorColorClass,
  getErrorBgClass,
  getErrorBorderClass,
} from './errorParser';

describe('Error Parser Utility', () => {
  describe('parseErrorMessage', () => {
    it('should parse error message with pipe separator', () => {
      const result = parseErrorMessage('SUNDAY_NOT_AVAILABLE|Los domingos no se atienden citas');
      expect(result.code).toBe('SUNDAY_NOT_AVAILABLE');
      expect(result.message).toBe('Los domingos no se atienden citas');
    });

    it('should parse holiday error message', () => {
      const result = parseErrorMessage('HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es Navidad');
      expect(result.code).toBe('HOLIDAY_NOT_AVAILABLE');
      expect(result.message).toBe('No se puede agendar porque es Navidad');
    });

    it('should parse past date error message', () => {
      const result = parseErrorMessage('PAST_DATE_NOT_AVAILABLE|No se pueden agendar citas en fechas pasadas');
      expect(result.code).toBe('PAST_DATE_NOT_AVAILABLE');
      expect(result.message).toBe('No se pueden agendar citas en fechas pasadas');
    });

    it('should handle message without pipe separator', () => {
      const result = parseErrorMessage('Generic error message');
      expect(result.code).toBe('UNKNOWN_ERROR');
      expect(result.message).toBe('Generic error message');
    });

    it('should handle null or undefined input', () => {
      const resultNull = parseErrorMessage(null);
      const resultUndefined = parseErrorMessage(undefined);

      expect(resultNull.code).toBe('UNKNOWN_ERROR');
      expect(resultNull.message).toBe('Ocurrió un error inesperado');

      expect(resultUndefined.code).toBe('UNKNOWN_ERROR');
      expect(resultUndefined.message).toBe('Ocurrió un error inesperado');
    });

    it('should handle empty string', () => {
      const result = parseErrorMessage('');
      expect(result.code).toBe('UNKNOWN_ERROR');
      expect(result.message).toBe('Ocurrió un error inesperado');
    });

    it('should handle message with multiple pipes', () => {
      const result = parseErrorMessage('ERROR_CODE|This is a message|with multiple|pipes');
      expect(result.code).toBe('ERROR_CODE');
      expect(result.message).toBe('This is a message|with multiple|pipes');
    });
  });

  describe('getErrorHint', () => {
    it('should return Sunday hint', () => {
      const hint = getErrorHint('SUNDAY_NOT_AVAILABLE');
      expect(hint).toBe('Por favor seleccione un día entre lunes y sábado');
    });

    it('should return Holiday hint', () => {
      const hint = getErrorHint('HOLIDAY_NOT_AVAILABLE');
      expect(hint).toBe('Por favor seleccione otra fecha');
    });

    it('should return Past date hint', () => {
      const hint = getErrorHint('PAST_DATE_NOT_AVAILABLE');
      expect(hint).toBe('Seleccione una fecha desde hoy en adelante');
    });

    it('should return null for unknown code', () => {
      const hint = getErrorHint('UNKNOWN_ERROR');
      expect(hint).toBeNull();
    });
  });

  describe('getErrorIcon', () => {
    it('should return Sunday icon', () => {
      const icon = getErrorIcon('SUNDAY_NOT_AVAILABLE');
      expect(icon).toBe('📅');
    });

    it('should return Holiday icon', () => {
      const icon = getErrorIcon('HOLIDAY_NOT_AVAILABLE');
      expect(icon).toBe('🎉');
    });

    it('should return Past date icon', () => {
      const icon = getErrorIcon('PAST_DATE_NOT_AVAILABLE');
      expect(icon).toBe('⏰');
    });

    it('should return default icon for unknown code', () => {
      const icon = getErrorIcon('UNKNOWN_ERROR');
      expect(icon).toBe('⚠️');
    });
  });

  describe('getErrorColorClass', () => {
    it('should return Sunday color class', () => {
      const color = getErrorColorClass('SUNDAY_NOT_AVAILABLE');
      expect(color).toBe('text-amber-600');
    });

    it('should return Holiday color class', () => {
      const color = getErrorColorClass('HOLIDAY_NOT_AVAILABLE');
      expect(color).toBe('text-red-600');
    });

    it('should return Past date color class', () => {
      const color = getErrorColorClass('PAST_DATE_NOT_AVAILABLE');
      expect(color).toBe('text-gray-600');
    });

    it('should return default color for unknown code', () => {
      const color = getErrorColorClass('UNKNOWN_ERROR');
      expect(color).toBe('text-gray-600');
    });
  });

  describe('getErrorBgClass', () => {
    it('should return Sunday background class', () => {
      const bg = getErrorBgClass('SUNDAY_NOT_AVAILABLE');
      expect(bg).toBe('bg-amber-50');
    });

    it('should return Holiday background class', () => {
      const bg = getErrorBgClass('HOLIDAY_NOT_AVAILABLE');
      expect(bg).toBe('bg-red-50');
    });

    it('should return Past date background class', () => {
      const bg = getErrorBgClass('PAST_DATE_NOT_AVAILABLE');
      expect(bg).toBe('bg-gray-50');
    });

    it('should return default background for unknown code', () => {
      const bg = getErrorBgClass('UNKNOWN_ERROR');
      expect(bg).toBe('bg-gray-50');
    });
  });

  describe('getErrorBorderClass', () => {
    it('should return Sunday border class', () => {
      const border = getErrorBorderClass('SUNDAY_NOT_AVAILABLE');
      expect(border).toBe('border-amber-200');
    });

    it('should return Holiday border class', () => {
      const border = getErrorBorderClass('HOLIDAY_NOT_AVAILABLE');
      expect(border).toBe('border-red-200');
    });

    it('should return Past date border class', () => {
      const border = getErrorBorderClass('PAST_DATE_NOT_AVAILABLE');
      expect(border).toBe('border-gray-200');
    });

    it('should return default border for unknown code', () => {
      const border = getErrorBorderClass('UNKNOWN_ERROR');
      expect(border).toBe('border-gray-200');
    });
  });
});
