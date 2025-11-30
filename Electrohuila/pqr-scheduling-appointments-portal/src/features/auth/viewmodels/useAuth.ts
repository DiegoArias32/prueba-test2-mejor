/**
 * ViewModel de autenticación (Custom Hook)
 * Gestiona la lógica de negocio de autenticación
 */

'use client';

import { useState, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import { authRepository } from '../repositories/auth.repository';
import { LoginCredentials } from '../models/auth.models';
import type { LoadingState } from '@/core/types';

interface ValidationErrors {
  usernameOrEmail?: string;
  password?: string;
}

export const useAuth = () => {
  const router = useRouter();
  const [loadingState, setLoadingState] = useState<LoadingState>('idle');
  const [error, setError] = useState<string>('');
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});

  /**
   * Valida un campo individual
   */
  const validateField = useCallback((field: string, value: string): string => {
    switch (field) {
      case 'usernameOrEmail':
        if (!value.trim()) {
          return 'Usuario o email es obligatorio';
        }
        if (value.length < 3) {
          return 'Usuario o email debe tener al menos 3 caracteres';
        }
        if (value.includes('@')) {
          const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
          if (!emailRegex.test(value)) {
            return 'El formato del email no es válido';
          }
        }
        return '';
      case 'password':
        if (!value.trim()) {
          return 'Contraseña es obligatoria';
        }
        if (value.length < 3) {
          return 'Contraseña debe tener al menos 3 caracteres';
        }
        return '';
      default:
        return '';
    }
  }, []);

  /**
   * Valida las credenciales de login
   */
  const validateCredentials = useCallback(
    (credentials: LoginCredentials): ValidationErrors => {
      const errors: ValidationErrors = {};

      const usernameError = validateField('usernameOrEmail', credentials.usernameOrEmail);
      if (usernameError) errors.usernameOrEmail = usernameError;

      const passwordError = validateField('password', credentials.password);
      if (passwordError) errors.password = passwordError;

      return errors;
    },
    [validateField]
  );

  /**
   * Inicia sesión
   */
  const login = useCallback(
    async (credentials: LoginCredentials) => {
      setLoadingState('loading');
      setError('');
      setValidationErrors({});

      try {
        // Validar credenciales
        const errors = validateCredentials(credentials);
        if (Object.keys(errors).length > 0) {
          setValidationErrors(errors);
          setError('Por favor corrija los errores en el formulario.');
          setLoadingState('error');
          return;
        }

        // Intentar login
        const response = await authRepository.login({
          Username: credentials.usernameOrEmail,
          Password: credentials.password,
        });

        // Guardar datos de autenticación
        authRepository.saveAuthData(response);

        setLoadingState('success');

        // Cargar permisos granulares EN PARALELO SIN BLOQUEAR (fire and forget)
        // Esto permite que la redirección sea inmediata (en ~500ms)
        // Los permisos se cargarán en background mientras el usuario navega
        authRepository.getCurrentUserPermissions()
          .then((userPermissions) => {
            if (typeof window !== 'undefined') {
              localStorage.setItem('userPermissions', JSON.stringify(userPermissions));
            }
          })
          .catch((error) => {
            // Silenciar errores - los permisos se pueden cargar después
            console.debug('Permisos cargados en background después del login');
          });

        // Redirigir al admin INMEDIATAMENTE (no esperar permisos)
        router.push('/admin');
      } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : 'Error al iniciar sesión. Verifica tus credenciales.';
        setError(errorMessage);
        setLoadingState('error');
      }
    },
    [router, validateCredentials]
  );

  /**
   * Cierra sesión
   */
  const logout = useCallback(async () => {
    setLoadingState('loading');
    try {
      await authRepository.logout();
      setLoadingState('success');
      router.push('/login');
    } catch (err: unknown) {
      // Log error to console for debugging
      if (err instanceof Error) {
        console.error('Error al cerrar sesión:', err.message);
      }
      // Aún así limpiar datos locales
      authRepository.clearAuthData();
      setLoadingState('error');
      router.push('/login');
    }
  }, [router]);

  /**
   * Verifica si el usuario está autenticado
   */
  const isAuthenticated = useCallback(() => {
    return authRepository.isAuthenticated();
  }, []);

  /**
   * Obtiene el usuario actual
   */
  const getCurrentUser = useCallback(() => {
    return authRepository.getStoredUser();
  }, []);

  /**
   * Verifica permiso en un formulario
   */
  const hasPermission = useCallback(
    (formCode: string, permission: 'read' | 'create' | 'update' | 'delete') => {
      return authRepository.hasFormPermission(formCode, permission);
    },
    []
  );

  return {
    // Estado
    loadingState,
    isLoading: loadingState === 'loading',
    error,
    validationErrors,

    // Métodos
    login,
    logout,
    validateField,
    isAuthenticated,
    getCurrentUser,
    hasPermission,
  };
};
