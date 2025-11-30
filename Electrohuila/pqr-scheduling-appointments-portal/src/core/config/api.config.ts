/**
 * Configuración de la API y constantes de almacenamiento
 */

/**
 * Configuración general de la API
 */
export const API_CONFIG = {
  BASE_URL: process.env.NEXT_PUBLIC_API_URL || 'https://8papi9muvp.us-east-2.awsapprunner.com/api/v1',
  TIMEOUT: 10000, // 10 segundos - timeout más realista (30s era demasiado alto)
} as const;

/**
 * Claves de almacenamiento en localStorage
 */
export const STORAGE_KEYS = {
  TOKEN: 'token',
  REFRESH_TOKEN: 'refreshToken',
  USER: 'user',
  ROLES: 'roles',
  PERMISSIONS: 'permissions',
  USER_PERMISSIONS: 'userPermissions',
} as const;

/**
 * Endpoints de la API
 */
export const API_ENDPOINTS = {
  // Autenticación
  AUTH: {
    LOGIN: '/auth/login',
    LOGOUT: '/auth/logout',
    REFRESH: '/auth/refresh',
    USER_INFO: '/auth/user-info',
    PERMISSIONS: '/auth/permissions',
  },
  // Citas
  APPOINTMENTS: {
    BASE: '/appointments',
    BY_ID: (id: number | string) => `/appointments/${id}`,
    AVAILABLE_SLOTS: '/appointments/available-slots',
    SCHEDULE: '/appointments/schedule',
  },
  // Endpoints públicos (sin autenticación)
  PUBLIC: {
    BRANCHES: '/public/branches',
    APPOINTMENT_TYPES: '/public/appointment-types',
    AVAILABLE_TIMES: '/public/available-times',
    SCHEDULE_APPOINTMENT: '/public/schedule-appointment',
    CLIENT_VALIDATE: (clientNumber: string) => `/public/client/validate/${clientNumber}`,
    APPOINTMENT: (appointmentNumber: string) => `/public/appointment/${appointmentNumber}`,
    CLIENT_APPOINTMENTS: (clientNumber: string) => `/public/client/${clientNumber}/appointments`,
    CANCEL_APPOINTMENT: (clientNumber: string, appointmentId: number) => `/public/client/${clientNumber}/appointment/${appointmentId}/cancel`,
    VERIFY_APPOINTMENT: '/public/verify-appointment',
  },
} as const;
