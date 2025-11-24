/**
 * Permission Service
 * Handles permission management, role-form assignments, and authorization
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  PermissionViewDto,
  CreatePermissionDto,
  RolPermissionSummaryDto,
  RolFormPermissionDetailDto,
  AssignPermissionToRolDto,
  RemovePermissionFromRolDto,
  UpdateRolFormPermissionDto,
} from './permission.types';

export class PermissionService extends BaseHttpService {
  /**
   * Get all permissions
   */
  async getAllPermissions(): Promise<PermissionViewDto[]> {
    return this.get<PermissionViewDto[]>('/permissions');
  }

  /**
   * Get summary of permissions for all roles
   */
  async getAllRolPermissionsSummary(): Promise<RolPermissionSummaryDto[]> {
    return this.get<RolPermissionSummaryDto[]>('/permissions/roles-permissions-summary');
  }

  /**
   * Get permission summary for a specific role
   */
  async getRolPermissionsSummary(rolId: number): Promise<RolPermissionSummaryDto> {
    return this.get<RolPermissionSummaryDto>(`/permissions/rol/${rolId}/summary`);
  }

  /**
   * Get role-form permission assignments
   */
  async getRolFormPermissions(rolId?: number, formId?: number): Promise<RolFormPermissionDetailDto[]> {
    const params = new URLSearchParams();
    if (rolId) params.append('rolId', rolId.toString());
    if (formId) params.append('formId', formId.toString());

    const endpoint = `/permissions/assignments?${params.toString()}`;
    return this.get<RolFormPermissionDetailDto[]>(endpoint);
  }

  /**
   * Assign permission to role
   */
  async assignPermissionToRol(assignment: AssignPermissionToRolDto): Promise<{ success: boolean; message: string }> {
    return this.post<{ success: boolean; message: string }>('/permissions/assign-to-rol', assignment);
  }

  /**
   * Remove permission from role
   */
  async removePermissionFromRol(removal: RemovePermissionFromRolDto): Promise<{ success: boolean; message: string }> {
    return this.post<{ success: boolean; message: string }>('/permissions/remove-from-rol', removal);
  }

  /**
   * Update role-form permission
   */
  async updateRolFormPermission(update: UpdateRolFormPermissionDto): Promise<{ success: boolean; message: string }> {
    return this.put<{ success: boolean; message: string }>('/permissions/update', update);
  }

  /**
   * Create new permission
   */
  async createPermission(permission: CreatePermissionDto): Promise<PermissionViewDto> {
    return this.post<PermissionViewDto>('/permissions', permission);
  }
}
