/**
 * Models - Servicios
 * DTOs e interfaces para la página de servicios
 */

/**
 * Entidad que representa un servicio disponible
 */
export interface Service {
  id: string;
  name: string;
  href: string;
  description: string;
}

/**
 * Lista de servicios disponibles
 */
export const SERVICES: Service[] = [
  {
    id: 'agendamiento-citas',
    name: 'Agendamiento de Cita PQR',
    href: '/agendamiento-citas',
    description: 'Programe citas para reclamos, reportes de daños, cuentas nuevas y proyectos de factibilidad'
  },
  {
    id: 'gestion-citas',
    name: 'Consultar citas generales',
    href: '/gestion-citas',
    description: 'Verifique el estado y detalles de sus citas existentes'
  }
];
