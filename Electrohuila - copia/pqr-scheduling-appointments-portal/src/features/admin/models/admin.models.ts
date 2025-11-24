/**
 * Admin Feature - Models & DTOs
 * All types and interfaces for the admin panel
 * DO NOT import from @/core/types or @/services/api for admin-specific types
 */

// ============================================
// USER & AUTHENTICATION DTOs
// ============================================

export interface UserDto {
  id: number;
  username: string;
  email: string;
  roles: string[];
  allowedTabs?: string[];
  createdAt?: string;
  updatedAt?: string;
  isActive: boolean;
}

export interface CreateUserDto {
  username: string;
  email: string;
  password: string;
  roleIds: number[];
}

export interface UpdateUserDto {
  id: number;
  username?: string;
  email?: string;
  roleIds?: number[];
  isActive?: boolean;
}

export interface UserTabPermissionDto {
  userId: number;
  username: string;
  email: string;
  allowedTabs: string[];
}

export interface UpdateUserTabsDto {
  userId: number;
  allowedTabs: string[];
}

export interface TabPermissionDto {
  tabId: string;
  tabName: string;
  tabIcon: string;
  isAllowed: boolean;
}

// ============================================
// ROLE DTOs
// ============================================

export interface RolDto {
  id: number;
  name: string;
  code: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
}

export interface UpdateRolDto {
  id: number;
  name?: string;
  code?: string;
  isActive?: boolean;
}

export interface CreateRolDto {
  name: string;
  code: string;
  isActive: boolean;
}

// ============================================
// PERMISSION DTOs
// ============================================

export interface FormPermissionDto {
  formCode: string;
  formName: string;
  canRead: boolean;
  canCreate: boolean;
  canUpdate: boolean;
  canDelete: boolean;
  moduleCode: string;
  moduleName: string;
}

export interface PermissionViewDto {
  id: number;
  canRead: boolean;
  canCreate: boolean;
  canUpdate: boolean;
  canDelete: boolean;
  description: string;
  createdAt: string;
}

export interface CreatePermissionDto {
  canRead: boolean;
  canCreate: boolean;
  canUpdate: boolean;
  canDelete: boolean;
}

export interface RolPermissionSummaryDto {
  rolId: number;
  rolName: string;
  rolCode: string;
  formPermissions: FormPermissionSummaryDto[];
}

export interface FormPermissionSummaryDto {
  formId: number;
  formName: string;
  formCode: string;
  assignedPermission?: PermissionSummaryDto;
  hasPermission: boolean;
}

export interface PermissionSummaryDto {
  id: number;
  canRead: boolean;
  canCreate: boolean;
  canUpdate: boolean;
  canDelete: boolean;
  description: string;
}

export interface RolFormPermissionDetailDto {
  id: number;
  rolId: number;
  rolName: string;
  rolCode: string;
  formId: number;
  formName: string;
  formCode: string;
  permissionId: number;
  permission: PermissionSummaryDto;
  assignedAt: string;
}

export interface AssignPermissionToRolDto {
  rolId: number;
  formId: number;
  permissionId: number;
}

export interface RemovePermissionFromRolDto {
  rolId: number;
  formId: number;
}

export interface UpdateRolFormPermissionDto {
  RolId: number;
  FormId: number;
  PermissionId?: number;
}

export interface ModuleDto {
  id: number;
  name: string;
  code: string;
  forms: FormDto[];
}

export interface FormDto {
  id?: number;
  name: string;
  code: string;
}

export interface UserPermissionsDto {
  forms: { [formCode: string]: FormPermissionDto };
  modules: ModuleDto[];
}

// ============================================
// APPOINTMENT DTOs
// ============================================

export interface AppointmentDto {
  id: number;
  appointmentNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  observations?: string;
  completedDate?: string;
  assignedTechnician?: string;
  technicianObservations?: string;
  clientId: number;
  branchId: number;
  appointmentTypeId: number;
  clientName?: string;
  clientNumber?: string;
  clientDocument?: string;
  branchName?: string;
  branchAddress?: string;
  appointmentTypeName?: string;
  appointmentTypeIcon?: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateAppointmentDto {
  id: number;
  appointmentNumber?: string;
  appointmentDate?: string;
  appointmentTime?: string;
  status?: string;
  observations?: string;
  completedDate?: string;
  assignedTechnician?: string;
  technicianObservations?: string;
  clientId?: number;
  branchId?: number;
  appointmentTypeId?: number;
}

// ============================================
// CLIENT DTOs
// ============================================

export interface ClientDto {
  id: number;
  clientNumber: string;
  documentType: string;
  documentNumber: string;
  fullName: string;
  email: string;
  phone?: string;
  mobile?: string;
  address?: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateClientDto {
  id: number;
  clientNumber?: string;
  documentType?: string;
  documentNumber?: string;
  fullName?: string;
  email?: string;
  phone?: string;
  mobile?: string;
  address?: string;
}

// ============================================
// BRANCH DTOs
// ============================================

export interface BranchDto {
  id: number;
  name: string;
  code: string;
  address: string;
  phone?: string;
  city: string;
  state: string;
  isMain: boolean;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateBranchDto {
  id: number;
  name?: string;
  code?: string;
  address?: string;
  phone?: string;
  city?: string;
  state?: string;
  isMain?: boolean;
  isActive?: boolean;
}

// ============================================
// APPOINTMENT TYPE DTOs
// ============================================

export interface AppointmentTypeDto {
  id: number;
  name: string;
  code: string;
  description?: string;
  icon?: string;
  iconName?: string;
  colorPrimary?: string;
  colorSecondary?: string;
  estimatedTimeMinutes: number;
  requiresDocumentation: boolean;
  displayOrder: number;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateAppointmentTypeDto {
  id: number;
  name?: string;
  description?: string;
  icon?: string;
  estimatedTimeMinutes?: number;
  requiresDocumentation?: boolean;
  isActive?: boolean;
}

// ============================================
// AVAILABLE TIME DTOs
// ============================================

export interface AvailableTimeDto {
  id: number;
  time: string;
  branchId: number;
  appointmentTypeId?: number;
  branchName?: string;
  appointmentTypeName?: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateAvailableTimeDto {
  id: number;
  time?: string;
  branchId?: number;
  appointmentTypeId?: number;
  isActive?: boolean;
}

// ============================================
// DOMAIN ENTITIES (from AdminTypes.ts)
// ============================================

export type TabType = 'citas' | 'empleados' | 'roles' | 'sedes' | 'tipos-cita' | 'horas-disponibles' | 'permisos' | 'festivos' | 'plantillas' | 'settings';

export type ModalType = 'create' | 'edit' | 'delete' | 'activate' | null;

export type ViewType = 'active' | 'inactive';

export interface EmployeeFormData {
  id?: number;
  username?: string;
  email?: string;
  fullName?: string;
  isActive?: boolean;
  roleId?: number;
  password?: string;
  roles?: string[];
}

export type EmpleadoFormData = EmployeeFormData;

export interface TabConfig {
  id: TabType;
  label: string;
  icon: React.ReactNode;
  requiredPermission?: string;
}

// ============================================
// STATE INTERFACES
// ============================================

export interface PaginationState {
  currentPage: number;
  itemsPerPage: number;
}

export interface UserPermissionsState {
  currentUser: UserDto | null;
  userRoles: string[];
  userPermissions: { [formCode: string]: FormPermissionDto };
  hasInitialPermissions: boolean;
  permissionUpdateKey: number;
}

export interface AdminDataState {
  appointments: AppointmentDto[];
  employees: UserDto[];
  roles: RolDto[];
  branches: BranchDto[];
  appointmentTypes: AppointmentTypeDto[];
  availableTimes: AvailableTimeDto[];
  permissions: PermissionViewDto[];
  rolPermissionsSummary: RolPermissionSummaryDto[];
  rolFormPermissions: RolFormPermissionDetailDto[];
}

export interface AdminFormsState {
  appointmentForm: Partial<AppointmentDto>;
  employeeForm: Partial<UserDto> & { password?: string; roles?: string[] };
  rolForm: Partial<RolDto>;
  branchForm: Partial<BranchDto>;
  appointmentTypeForm: Partial<AppointmentTypeDto>;
  availableTimeForm: Partial<AvailableTimeDto>;
}

export interface AdminUIState {
  loading: boolean;
  error: string;
  success: string;
  isLoggingOut: boolean;
  isRefreshing: boolean;
  lastRefresh: Date;
  autoRefreshEnabled: boolean;
  refreshInterval: NodeJS.Timeout | null;
}

export interface AdminModalState {
  modalType: 'create' | 'edit' | 'delete' | 'activate' | null;
  selectedItem: AppointmentDto | UserDto | RolDto | BranchDto | AppointmentTypeDto | AvailableTimeDto | null;
}

export interface EmployeeTabPermissionsState {
  selectedEmployeeId: number | null;
  employeeTabPermissions: { [tabId: string]: boolean };
  isEditingTabs: boolean;
  originalTabPermissions: { [tabId: string]: boolean };
}

export interface PermissionsAccordionState {
  expandedRoles: Set<number>;
}

// ============================================
// DASHBOARD DTOs
// ============================================

export interface DashboardStatsDto {
  totalAppointments: number;
  completedAppointments: number;
  pendingAppointments: number;
  totalEmployees: number;
  totalBranches: number;
  totalAppointmentTypes: number;
  myAppointmentsToday: number;
  myPendingAppointments: number;
}

// ============================================
// HOLIDAY DTOs
// ============================================

export interface HolidayDto {
  id: number;
  holidayDate: string; // ISO date (YYYY-MM-DD)
  holidayName: string;
  holidayType: 'National' | 'Local' | 'Company';
  branchId?: number; // Solo para tipo Local
  branchName?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

// ============================================
// CONSTANTS
// ============================================

export const TAB_FORM_CODE_MAP: { [key: string]: string } = {
  'citas': 'APPOINTMENTS',
  'empleados': 'USERS',
  'roles': 'ROLES',
  'sedes': 'BRANCHES',
  'tipos-cita': 'APPOINTMENT_TYPES',
  'horas-disponibles': 'AVAILABLE_TIMES',
  'permisos': 'PERMISSIONS',
  'festivos': 'HOLIDAYS',
  'plantillas': 'NOTIFICATIONS',
  'settings': 'SETTINGS'
};

export const TAB_DISPLAY_NAMES: { [key: string]: string } = {
  'citas': 'Citas',
  'empleados': 'Empleados',
  'roles': 'Roles',
  'sedes': 'Sedes',
  'tipos-cita': 'Tipos de Cita',
  'horas-disponibles': 'Horas Disponibles',
  'permisos': 'Permisos',
  'festivos': 'Festivos',
  'plantillas': 'Plantillas',
  'settings': 'Configuración'
};
