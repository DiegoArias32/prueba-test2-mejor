/**
 * @file app/admin/page.tsx
 * @description Página de administración - Next.js Route
 * @module app/admin
 *
 * ARQUITECTURA: Feature-based MVVM
 * Este archivo solo define la ruta de Next.js.
 * La lógica y UI están en features/admin/
 */

import { AdminView } from '@/features/admin';

/**
 * Página de administración
 * Simplemente renderiza el AdminView del feature
 */
export default function AdminPage() {
  return <AdminView />;
}
