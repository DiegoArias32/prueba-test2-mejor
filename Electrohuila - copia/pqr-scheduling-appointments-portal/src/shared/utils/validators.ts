/**
 * Shared Validation Utilities
 * Consolidates validation logic used across the application
 *
 * Note: For formatters, use @/shared/utils/formatters instead
 */

export const validators = {
  /**
   * Validate client number
   */
  clientNumber: (value: string): string => {
    if (!value.trim()) return 'Número de cliente es obligatorio';
    if (!/^\d+$/.test(value)) return 'El número de cliente debe contener solo números';
    if (value.length < 3) return 'El número de cliente debe tener al menos 3 dígitos';
    return '';
  },

  /**
   * Validate cancellation reason
   */
  cancellationReason: (value: string): string => {
    if (!value.trim()) return 'El motivo de cancelación es obligatorio';
    if (value.trim().length < 10) return 'El motivo debe tener al menos 10 caracteres';
    if (value.length > 500) return 'El motivo no puede tener más de 500 caracteres';
    return '';
  },

  /**
   * Validate technician name
   */
  technicianName: (value: string): string => {
    if (!value.trim()) return 'El nombre del técnico es obligatorio';
    if (!/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/.test(value)) return 'El nombre del técnico debe contener solo letras';
    if (value.trim().length < 2) return 'El nombre del técnico debe tener al menos 2 caracteres';
    if (value.length > 100) return 'El nombre del técnico no puede tener más de 100 caracteres';
    return '';
  },

  /**
   * Validate email
   */
  email: (value: string): string => {
    if (!value.trim()) return 'El correo electrónico es obligatorio';
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(value)) return 'El correo electrónico no es válido';
    return '';
  },

  /**
   * Validate phone number
   */
  phone: (value: string): string => {
    if (!value.trim()) return '';
    if (!/^\d{7,10}$/.test(value)) return 'El teléfono debe tener entre 7 y 10 dígitos';
    return '';
  },

  /**
   * Validate required field
   */
  required: (value: string, fieldName: string = 'Este campo'): string => {
    if (!value || !value.trim()) return `${fieldName} es obligatorio`;
    return '';
  },

  /**
   * Validate text length
   */
  length: (value: string, min: number, max: number, fieldName: string = 'Este campo'): string => {
    if (value.length < min) return `${fieldName} debe tener al menos ${min} caracteres`;
    if (value.length > max) return `${fieldName} no puede tener más de ${max} caracteres`;
    return '';
  },
};

/**
 * Status checking utilities
 */
export const statusCheckers = {
  isPending: (status: string): boolean => {
    if (!status) return false;
    const s = status.toLowerCase();
    return s === 'scheduled' || s === 'pendiente' || s === 'programada';
  },

  isCompleted: (status: string): boolean => {
    if (!status) return false;
    const s = status.toLowerCase();
    return s === 'completed' || s === 'completada';
  },

  isCancelled: (status: string): boolean => {
    if (!status) return false;
    const s = status.toLowerCase();
    return s === 'cancelled' || s === 'cancelada';
  },
};
