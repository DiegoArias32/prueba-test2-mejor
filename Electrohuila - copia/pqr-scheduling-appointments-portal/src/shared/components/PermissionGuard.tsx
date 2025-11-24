/**
 * Componente guardian de permisos - Migrado a arquitectura MVVM
 */

'use client';

import { ReactNode } from 'react';
import { authRepository } from '@/features/auth/repositories/auth.repository';

interface PermissionGuardProps {
  children: ReactNode;
  formCode: string;
  permission: 'read' | 'create' | 'update' | 'delete';
  fallback?: ReactNode;
}

/**
 * Componente que renderiza contenido solo si el usuario tiene el permiso específico
 *
 * @example
 * <PermissionGuard formCode="CITAS" permission="create">
 *   <button onClick={createCita}>Crear Cita</button>
 * </PermissionGuard>
 */
export const PermissionGuard: React.FC<PermissionGuardProps> = ({
  children,
  formCode,
  permission,
  fallback = null,
}) => {
  const hasPermission = authRepository.hasFormPermission(formCode, permission);

  return hasPermission ? <>{children}</> : <>{fallback}</>;
};

export default PermissionGuard;
