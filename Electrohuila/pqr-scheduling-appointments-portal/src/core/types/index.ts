/**
 * Exportación centralizada de todos los tipos
 */

export * from './auth.types';
export * from './appointment.types';
export * from './permission.types';

/**
 * Tipos genéricos de respuesta
 */
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Tipos de estado de carga
 */
export type LoadingState = 'idle' | 'loading' | 'success' | 'error';

/**
 * Tipo genérico de error
 */
export interface ApiError {
  message: string;
  statusCode?: number;
  errors?: Record<string, string[]>;
}
