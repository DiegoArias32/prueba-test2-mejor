/**
 * Componente de ruta protegida - Migrado a arquitectura MVVM
 */

'use client';

import React, { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { authRepository } from '@/features/auth/repositories/auth.repository';

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRole?: string;
  requiredPermission?: string;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
  requiredRole,
  requiredPermission,
}) => {
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthorized, setIsAuthorized] = useState(false);
  const router = useRouter();

  useEffect(() => {
    const checkAuth = async () => {
      try {
        // Verificar si está autenticado
        if (!authRepository.isAuthenticated()) {
          router.push('/login');
          return;
        }

        // Verificar rol si es requerido
        if (requiredRole) {
          const roles = authRepository.getStoredRoles();
          if (!roles.includes(requiredRole)) {
            router.push('/unauthorized');
            return;
          }
        }

        // Verificar permiso si es requerido
        if (requiredPermission) {
          const permissions = authRepository.getStoredPermissions();
          const hasPermission = Object.keys(permissions.forms).includes(requiredPermission);
          if (!hasPermission) {
            router.push('/unauthorized');
            return;
          }
        }

        setIsAuthorized(true);
      } catch (error) {
        console.error('Authorization error:', error);
        router.push('/login');
      } finally {
        setIsLoading(false);
      }
    };

    checkAuth();
  }, [router, requiredRole, requiredPermission]);

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#1797D5] mx-auto mb-4"></div>
          <p className="text-gray-600">Verificando autenticación...</p>
        </div>
      </div>
    );
  }

  if (!isAuthorized) {
    return null;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
