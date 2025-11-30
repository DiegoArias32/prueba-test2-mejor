import { useState, useEffect } from 'react';

// Definición de tipos para el tema
export interface ThemeColors {
  // Main colors
  colorPrimary: string;
  colorSecondary: string;
  colorAccent: string;
  colorIntermediate: string;

  // Status colors
  colorSuccess: string;
  colorError: string;
  colorWarning: string;
  colorInfo: string;

  // Background colors
  backgroundPrimary: string;
  backgroundSecondary: string;

  // Text colors
  textPrimary: string;
  textSecondary: string;

  // Scrollbar colors
  scrollbarGradientStart: string;
  scrollbarGradientEnd: string;
  scrollbarHoverStart: string;
  scrollbarHoverEnd: string;
}

export interface ThemeSettings extends ThemeColors {
  id: number;
  name: string;
  description?: string;
  isDefaultTheme: boolean;
  isActive: boolean;
}

// Tema por defecto (fallback)
const DEFAULT_THEME: ThemeColors = {
  colorPrimary: '#203461',
  colorSecondary: '#1797D5',
  colorAccent: '#56C2E1',
  colorIntermediate: '#1A6192',
  colorSuccess: '#22C55E',
  colorError: '#EF4444',
  colorWarning: '#F59E0B',
  colorInfo: '#3B82F6',
  backgroundPrimary: '#FFFFFF',
  backgroundSecondary: '#F9FAFB',
  textPrimary: '#111827',
  textSecondary: '#6B7280',
  scrollbarGradientStart: '#1797D5',
  scrollbarGradientEnd: '#56C2E1',
  scrollbarHoverStart: '#203461',
  scrollbarHoverEnd: '#1797D5',
};

/**
 * Hook personalizado para obtener y aplicar el tema desde el backend
 * Obtiene los colores de la API y los aplica como variables CSS
 */
export const useTheme = () => {
  const [theme, setTheme] = useState<ThemeColors>(DEFAULT_THEME);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchTheme = async () => {
      try {
        setIsLoading(true);

        // Intentar obtener desde localStorage primero (cache)
        const cachedTheme = localStorage.getItem('app-theme');
        if (cachedTheme) {
          const parsed = JSON.parse(cachedTheme);
          applyTheme(parsed);
          setTheme(parsed);
        }

        // Obtener tema desde la API - OPCIONAL, si falla usa el tema por defecto
        const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://8papi9muvp.us-east-2.awsapprunner.com/api/v1'}/theme/active`);

        if (!response.ok) {
          // No lanzar error, simplemente usar el tema por defecto
          console.warn('No se pudo cargar el tema desde el backend, usando tema por defecto');
          applyTheme(DEFAULT_THEME);
          setTheme(DEFAULT_THEME);
          setIsLoading(false);
          return;
        }

        const result = await response.json();

        if (result.data) {
          const themeData: ThemeColors = {
            colorPrimary: result.data.colorPrimary,
            colorSecondary: result.data.colorSecondary,
            colorAccent: result.data.colorAccent,
            colorIntermediate: result.data.colorIntermediate,
            colorSuccess: result.data.colorSuccess,
            colorError: result.data.colorError,
            colorWarning: result.data.colorWarning,
            colorInfo: result.data.colorInfo,
            backgroundPrimary: result.data.backgroundPrimary,
            backgroundSecondary: result.data.backgroundSecondary,
            textPrimary: result.data.textPrimary,
            textSecondary: result.data.textSecondary,
            scrollbarGradientStart: result.data.scrollbarGradientStart,
            scrollbarGradientEnd: result.data.scrollbarGradientEnd,
            scrollbarHoverStart: result.data.scrollbarHoverStart,
            scrollbarHoverEnd: result.data.scrollbarHoverEnd,
          };

          setTheme(themeData);
          applyTheme(themeData);

          // Guardar en localStorage para cache
          localStorage.setItem('app-theme', JSON.stringify(themeData));
        }
      } catch (err) {
        console.warn('Error al cargar tema desde backend, usando tema por defecto:', err);
        // En caso de error, aplicar tema por defecto SIN mostrar error al usuario
        applyTheme(DEFAULT_THEME);
        setTheme(DEFAULT_THEME);
      } finally {
        setIsLoading(false);
      }
    };

    fetchTheme();
  }, []);

  return { theme, isLoading };
};

/**
 * Aplica el tema configurando variables CSS en el root
 */
const applyTheme = (theme: ThemeColors) => {
  const root = document.documentElement;

  // Colores principales
  root.style.setProperty('--color-primary', theme.colorPrimary);
  root.style.setProperty('--color-secondary', theme.colorSecondary);
  root.style.setProperty('--color-accent', theme.colorAccent);
  root.style.setProperty('--color-intermediate', theme.colorIntermediate);

  // Colores de estado
  root.style.setProperty('--color-success', theme.colorSuccess);
  root.style.setProperty('--color-error', theme.colorError);
  root.style.setProperty('--color-warning', theme.colorWarning);
  root.style.setProperty('--color-info', theme.colorInfo);

  // Colores de fondo
  root.style.setProperty('--bg-primary', theme.backgroundPrimary);
  root.style.setProperty('--bg-secondary', theme.backgroundSecondary);

  // Colores de texto
  root.style.setProperty('--text-primary', theme.textPrimary);
  root.style.setProperty('--text-secondary', theme.textSecondary);

  // Colores de scrollbar
  root.style.setProperty('--scrollbar-gradient-start', theme.scrollbarGradientStart);
  root.style.setProperty('--scrollbar-gradient-end', theme.scrollbarGradientEnd);
  root.style.setProperty('--scrollbar-hover-start', theme.scrollbarHoverStart);
  root.style.setProperty('--scrollbar-hover-end', theme.scrollbarHoverEnd);
};
