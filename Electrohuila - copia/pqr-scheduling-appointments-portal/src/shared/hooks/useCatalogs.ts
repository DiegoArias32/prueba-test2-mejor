/**
 * Custom hooks para consumir catálogos del sistema desde la API
 * Incluyen cache, loading states y manejo de errores
 */

'use client';

import { useState, useEffect } from 'react';
import { apiService, AppointmentStatusDto, DocumentTypeDto, ProjectTypeDto, NewAccountStatusDto, PropertyTypeDto, ServiceUseTypeDto } from '@/services/api';

// ========== HOOK GENÉRICO PARA CATÁLOGOS ==========
function useCatalog<T>(
  fetchFunction: () => Promise<T[]>,
  cacheKey: string,
  cacheDuration: number = 5 * 60 * 1000 // 5 minutos por defecto
) {
  const [data, setData] = useState<T[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [hasFetched, setHasFetched] = useState(false);

  useEffect(() => {
    // Evitar múltiples fetches
    if (hasFetched) return;

    const fetchData = async () => {
      try {
        // Verificar cache en localStorage
        const cachedData = localStorage.getItem(cacheKey);
        const cacheTimestamp = localStorage.getItem(`${cacheKey}_timestamp`);

        if (cachedData && cacheTimestamp) {
          const age = Date.now() - parseInt(cacheTimestamp, 10);
          if (age < cacheDuration) {
            // Cache válido
            setData(JSON.parse(cachedData));
            setLoading(false);
            setHasFetched(true);
            return;
          }
        }

        // Fetch desde API
        setLoading(true);
        const result = await fetchFunction();
        setData(result);

        // Guardar en cache
        localStorage.setItem(cacheKey, JSON.stringify(result));
        localStorage.setItem(`${cacheKey}_timestamp`, Date.now().toString());

        setError(null);
        setHasFetched(true);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error al cargar datos');
        console.error(`Error fetching ${cacheKey}:`, err);
        setHasFetched(true); // Marcar como fetched incluso en error para evitar reintentos infinitos
      } finally {
        setLoading(false);
      }
    };

    fetchData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [cacheKey, cacheDuration, hasFetched]);

  const invalidateCache = () => {
    localStorage.removeItem(cacheKey);
    localStorage.removeItem(`${cacheKey}_timestamp`);
  };

  return { data, loading, error, invalidateCache };
}

// ========== HOOKS ESPECÍFICOS ==========

/**
 * Hook para obtener todos los estados de citas desde la API
 */
export function useAppointmentStatuses() {
  return useCatalog<AppointmentStatusDto>(
    () => apiService.getAppointmentStatuses(),
    'catalog_appointment_statuses'
  );
}

/**
 * Hook para obtener todos los tipos de documento
 */
export function useDocumentTypes() {
  return useCatalog<DocumentTypeDto>(
    () => apiService.getDocumentTypes(),
    'catalog_document_types'
  );
}

/**
 * Hook para obtener todos los tipos de proyecto
 */
export function useProjectTypes() {
  return useCatalog<ProjectTypeDto>(
    () => apiService.getProjectTypes(),
    'catalog_project_types'
  );
}

/**
 * Hook para obtener todos los estados de cuentas nuevas
 */
export function useNewAccountStatuses() {
  return useCatalog<NewAccountStatusDto>(
    () => apiService.getNewAccountStatuses(),
    'catalog_new_account_statuses'
  );
}

/**
 * Hook para obtener todos los tipos de propiedad
 */
export function usePropertyTypes() {
  return useCatalog<PropertyTypeDto>(
    () => apiService.getPropertyTypes(),
    'catalog_property_types'
  );
}

/**
 * Hook para obtener todos los tipos de uso de servicio
 */
export function useServiceUseTypes() {
  return useCatalog<ServiceUseTypeDto>(
    () => apiService.getServiceUseTypes(),
    'catalog_service_use_types'
  );
}

/**
 * Hook para invalidar todos los caches de catálogos
 */
export function useInvalidateAllCatalogs() {
  const invalidateAll = () => {
    const catalogKeys = [
      'catalog_appointment_statuses',
      'catalog_document_types',
      'catalog_project_types',
      'catalog_new_account_statuses',
      'catalog_property_types',
      'catalog_service_use_types',
    ];

    catalogKeys.forEach((key) => {
      localStorage.removeItem(key);
      localStorage.removeItem(`${key}_timestamp`);
    });
  };

  return { invalidateAll };
}
