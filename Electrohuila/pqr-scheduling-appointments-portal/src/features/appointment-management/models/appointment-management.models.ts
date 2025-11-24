/**
 * Modelos del feature appointment-management
 * Gestión de citas (consultar, cancelar)
 */

import type { AppointmentDto, ClientDto } from '@/core/types';

/**
 * Datos de autenticación del cliente
 */
export interface ClientAuthData {
  clientNumber: string;
  isAuthenticated: boolean;
}

/**
 * Request para cancelar cita
 */
export interface CancelAppointmentRequest {
  clientNumber: string;
  appointmentId: number;
  cancelReason: string;
}

/**
 * Datos para completar cita (solo admin)
 * FIXED: Actualizado para coincidir con estructura de API backend
 */
export interface CompleteAppointmentData {
  appointmentId: number;
  notes: string;
}

/**
 * Tipo de pestaña activa
 */
export type AppointmentTab = 'pendientes' | 'completadas' | 'canceladas';

/**
 * Tipo de modal activo
 */
export type ModalType = 'cancel' | 'complete' | null;

/**
 * Filtros de búsqueda de citas
 */
export interface AppointmentFilters {
  status?: 'scheduled' | 'completed' | 'cancelled';
  dateFrom?: string;
  dateTo?: string;
}

/**
 * Estadísticas de citas del cliente
 */
export interface AppointmentStats {
  total: number;
  pending: number;
  completed: number;
  cancelled: number;
}

/**
 * Estado de validación de campos
 */
export interface ValidationState {
  errors: { [key: string]: string };
  touchedFields: { [key: string]: boolean };
}

// Re-exportar tipos necesarios
export type { AppointmentDto, ClientDto };
