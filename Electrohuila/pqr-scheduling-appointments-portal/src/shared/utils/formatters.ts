/**
 * @file formatters.ts
 * @description Shared formatting utilities for the application
 * Centralized location for all date, time, and other formatters
 */

/**
 * Formatea una hora en formato 12 horas con AM/PM
 * @param {string} time - Hora en formato 24 horas (HH:MM)
 * @returns {string} Hora formateada en formato 12 horas
 */
export const formatTimeForDisplay = (time: string): string => {
  if (!time || typeof time !== 'string') {
    return '';
  }

  const cleanTime = time.trim();
  if (!cleanTime.includes(':')) {
    return time;
  }

  const parts = cleanTime.split(':');
  if (parts.length < 2) {
    return time;
  }

  const [hoursStr, minutesStr] = parts;
  const hours = parseInt(hoursStr, 10);
  const minutes = parseInt(minutesStr, 10);

  if (isNaN(hours) || isNaN(minutes) || hours < 0 || hours > 23 || minutes < 0 || minutes > 59) {
    return time;
  }

  const ampm = hours >= 12 ? 'PM' : 'AM';
  const hour12 = hours % 12 || 12;
  const formattedMinutes = minutes.toString().padStart(2, '0');

  return `${hour12}:${formattedMinutes} ${ampm}`;
};

/**
 * Alias for formatTimeForDisplay - formats time to 12-hour format
 * @param {string} timeString - Time in 24-hour format
 * @returns {string} Time formatted in 12-hour format
 */
export const formatTime = (timeString: string): string => {
  return formatTimeForDisplay(timeString);
};

/**
 * Obtiene la fecha y hora actual formateada
 * @returns {string} Fecha y hora formateada
 */
export const getCurrentDateTime = (): string => {
  const now = new Date();
  const date = now.toLocaleDateString('es-CO');
  const time = now.toLocaleTimeString('es-CO', { hour12: false });
  return `${date} ${time}`;
};

/**
 * Formatea una fecha en formato legible (Colombian locale)
 * @param {string} dateString - Fecha a formatear
 * @returns {string} Fecha formateada
 */
export const formatDate = (dateString: string): string => {
  if (!dateString) return '';
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  } catch {
    return dateString;
  }
};

/**
 * Formatea una fecha en formato corto (Colombian locale)
 * @param {string} dateString - Fecha a formatear
 * @returns {string} Fecha formateada (DD/MM/YYYY)
 */
export const formatDateShort = (dateString: string): string => {
  if (!dateString) return '';
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-CO');
  } catch {
    return dateString;
  }
};

/**
 * Collection of formatters for easy import
 */
export const formatters = {
  /**
   * Format date to Colombian locale (long format)
   */
  date: formatDate,

  /**
   * Format date to Colombian locale (short format)
   */
  dateShort: formatDateShort,

  /**
   * Format time to 12-hour format
   */
  time: formatTime,

  /**
   * Get current date and time formatted
   */
  currentDateTime: getCurrentDateTime,
};
