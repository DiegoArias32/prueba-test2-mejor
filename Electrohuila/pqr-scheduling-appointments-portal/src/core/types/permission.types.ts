/**
 * Tipos y DTOs relacionados con permisos
 */

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
