/**
 * Índice general de exportaciones - Nueva Arquitectura MVVM
 *
 * Este archivo facilita las importaciones centralizadas
 */

// === CORE ===
export * from './core/types';
export { httpClient } from './core/api/http-client';
export { API_CONFIG, API_ENDPOINTS, STORAGE_KEYS } from './core/config/api.config';

// === FEATURES ===
export * from './features/auth';
export * from './features/appointments';

// === SHARED ===
export * from './shared';
