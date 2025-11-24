/**
 * Componente para mostrar badges de estado con colores dinámicos desde la API
 */

'use client';

import { useAppointmentStatuses } from '@/shared/hooks/useCatalogs';
import { getAppointmentStatusColor, getAppointmentStatusIcon, getAppointmentStatusName } from '@/shared/utils/catalogHelpers';

interface StatusBadgeProps {
  statusCode: string;
  className?: string;
  showIcon?: boolean;
}

export function StatusBadge({ statusCode, className = '', showIcon = true }: StatusBadgeProps) {
  const { data: statuses, loading } = useAppointmentStatuses();

  if (loading) {
    return (
      <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm animate-pulse bg-gray-200 ${className}`}>
        Cargando...
      </span>
    );
  }

  const colors = getAppointmentStatusColor(statusCode, statuses);
  const icon = getAppointmentStatusIcon(statusCode, statuses);
  const name = getAppointmentStatusName(statusCode, statuses);

  return (
    <span
      className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-medium ${className}`}
      style={colors.style}
    >
      {showIcon && <i className={`fas fa-${icon}`} />}
      <span>{name}</span>
    </span>
  );
}

/**
 * Hook personalizado para obtener información de un estado específico
 * Útil cuando necesitas los datos del estado pero no quieres renderizar el badge
 */
export function useAppointmentStatus(statusCode: string) {
  const { data: statuses, loading, error } = useAppointmentStatuses();

  const status = statuses.find(s => s.code === statusCode);
  const colors = getAppointmentStatusColor(statusCode, statuses);
  const icon = getAppointmentStatusIcon(statusCode, statuses);
  const name = getAppointmentStatusName(statusCode, statuses);

  return {
    status,
    colors,
    icon,
    name,
    loading,
    error
  };
}
