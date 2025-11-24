/**
 * Component - ServiceIcon
 * Componente para mostrar el ícono de un servicio
 */

import React from 'react';
import { FiCalendar, FiSearch } from 'react-icons/fi';

/**
 * Props del componente ServiceIcon
 */
interface ServiceIconProps {
  serviceId: string;
  className?: string;
}

/**
 * Componente que muestra el ícono correspondiente a un servicio
 */
export const ServiceIcon: React.FC<ServiceIconProps> = ({ serviceId, className = "w-7 h-7" }) => {
  const icons: { [key: string]: React.ReactElement } = {
    'agendamiento-citas': <FiCalendar className={className} />,
    'gestion-citas': <FiSearch className={className} />
  };

  return icons[serviceId] || <FiCalendar className={className} />;
};
