/**
 * @file index.ts
 * @description Punto de entrada para el feature de verificación de citas
 * @module features/verificar-cita
 *
 * Arquitectura: MVVM Feature-based
 */

// Models (Data Transfer Objects)
export * from './models/verificar-cita.models';

// Repositories (Data Access Layer)
export * from './repositories/verificar-cita.repository';

// ViewModels (Business Logic)
export * from './viewmodels/useVerificarCita';

// Views (Presentation Layer)
export * from './views/VerificarCitaView';
export * from './views/components';
