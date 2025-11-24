/**
 * Admin Feature - MVVM Architecture
 * Punto de entrada público del módulo
 *
 * ARQUITECTURA: Feature-based MVVM
 * ✅ Models - Tipos y entidades de dominio
 * ✅ Repositories - Acceso a datos (API)
 * ✅ ViewModels - Lógica de negocio (hooks)
 * ✅ Views - Componentes UI (ahora en views/)
 */

// Models y Tipos (Domain Layer)
// All types are exported from models/admin.models (includes domain types)
export * from './models/admin.models';

// Repositories (Data Access Layer)
export { adminRepository } from './repositories/admin.repository';

// ViewModels (Business Logic Layer)
export * from './viewmodels/useAdmin';

// Views (Presentation Layer)
export * from './views/AdminView';
