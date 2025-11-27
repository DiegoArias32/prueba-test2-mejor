/**
 * @file AdminTypes.ts
 * @description Tipos y entidades compartidas del panel administrativo
 * @module features/admin/domain/entities
 */

/**
 * Tipo de tab del panel administrativo
 */
export type TabType = 'citas' | 'empleados' | 'roles' | 'sedes' | 'tipos-cita' | 'horas-disponibles' | 'permisos' | 'festivos';

/**
 * Tipo de modal
 */
export type ModalType = 'create' | 'edit' | 'delete' | 'activate' | null;

/**
 * Tipo de vista (activos/inactivos)
 */
export type ViewType = 'active' | 'inactive';

/**
 * Datos de formulario de empleado
 */
export type EmployeeFormData = {
  id?: number;
  username?: string;
  email?: string;
  fullName?: string;
  isActive?: boolean;
  roleId?: number;
  password?: string;
  roles?: string[];
};

// Alias para compatibilidad con código legacy
export type EmpleadoFormData = EmployeeFormData;

/**
 * Configuración de tabs del panel
 */
export interface TabConfig {
  id: TabType;
  label: string;
  icon: React.ReactNode;
  requiredPermission?: string;
}
