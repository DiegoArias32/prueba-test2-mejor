/**
 * LEGACY API SERVICE WRAPPER
 *
 * This file maintains backward compatibility with the old monolithic API service.
 * All functionality has been moved to domain-specific services under:
 * - src/services/auth/
 * - src/services/appointments/
 * - src/services/users/
 * - src/services/catalogs/
 * - src/services/permissions/
 *
 * This wrapper re-exports everything from the new modular structure.
 *
 * @deprecated Use individual domain services from '@/services' instead
 */

// Re-export everything from the new modular services
export { apiService } from './index';
export * from './index';

// Default export for backward compatibility
export { apiService as default } from './index';
