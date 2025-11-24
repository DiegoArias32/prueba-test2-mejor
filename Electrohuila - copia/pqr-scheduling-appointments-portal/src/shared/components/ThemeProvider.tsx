'use client';

import { useEffect } from 'react';
import { useTheme } from '../hooks/useTheme';

interface ThemeProviderProps {
  children: React.ReactNode;
}

/**
 * Componente proveedor de tema que aplica los colores dinámicamente
 * Debe envolverse alrededor de toda la aplicación en el layout raíz
 */
export const ThemeProvider: React.FC<ThemeProviderProps> = ({ children }) => {
  const { isLoading, error, theme } = useTheme();

  // Log theme for debugging
  console.debug('Current theme:', theme);

  useEffect(() => {
    if (error) {
      console.warn('Error loading theme, using default theme:', error);
    }
  }, [error]);

  // Mostrar indicador de carga mínimo mientras se obtiene el tema
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          <p className="mt-4 text-gray-600">Cargando tema...</p>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};
