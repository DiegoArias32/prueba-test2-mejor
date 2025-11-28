/**
 * User Service
 * Handles user management, roles, and tab permissions
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  UserDto,
  CreateUserDto,
  UpdateUserDto,
  UserTabPermissionDto,
  UpdateUserTabsDto,
  TabPermissionDto,
  RolDto,
  UpdateRolDto,
} from './user.types';

export class UserService extends BaseHttpService {
  // ===== USER MANAGEMENT =====

  /**
   * Get all users
   */
  async getUsers(): Promise<UserDto[]> {
    return this.get<UserDto[]>('/users');
  }

  /**
   * Get user by ID
   */
  async getUserById(id: number): Promise<UserDto> {
    return this.get<UserDto>(`/users/${id}`);
  }

  /**
   * Create new user
   */
  async createUser(userData: CreateUserDto): Promise<UserDto> {
    // Backend expects data wrapped in a UserDto property
    return this.post<UserDto>('/users', { userDto: userData });
  }

  /**
   * Update user
   */
  async updateUser(updateData: UpdateUserDto): Promise<UserDto> {
    const { id, ...data } = updateData;
    return this.put<UserDto>(`/users/${id}/update-with-roles`, { id, ...data });
  }

  /**
   * Delete user (hard delete)
   */
  async deleteUser(id: number): Promise<boolean> {
    return this.delete<boolean>(`/users/${id}`);
  }

  /**
   * Delete user (logical delete / deactivate)
   * Uses dedicated deactivate endpoint with PATCH method
   */
  async deleteLogicalUser(id: number): Promise<boolean> {
    const result = await this.patch<{ success: boolean; message: string }>(`/users/${id}/deactivate`, {});
    return true;
  }

  /**
   * Activate user (reactivate a deactivated user)
   * Uses dedicated activate endpoint with PATCH method
   */
  async activateUser(id: number): Promise<boolean> {
    const result = await this.patch<{ success: boolean; message: string }>(`/users/${id}/activate`, {});
    return true;
  }

  // ===== ROLE MANAGEMENT =====

  /**
   * Get all roles
   */
  async getRoles(): Promise<RolDto[]> {
    return this.get<RolDto[]>('/roles');
  }

  /**
   * Get role by ID
   */
  async getRolById(id: number): Promise<RolDto> {
    return this.get<RolDto>(`/roles/${id}`);
  }

  /**
   * Create new role
   */
  async createRol(rol: Omit<RolDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<RolDto> {
    return this.post<RolDto>('/roles', rol);
  }

  /**
   * Update role
   */
  async updateRol(rol: UpdateRolDto): Promise<{ success: boolean; message: string }> {
    const { id, ...data } = rol;
    return this.put<{ success: boolean; message: string }>(`/roles/${id}`, data);
  }

  /**
   * Delete role (hard delete)
   */
  async deleteRol(id: number): Promise<{ success: boolean; message: string }> {
    return this.delete<{ success: boolean; message: string }>(`/roles/${id}`);
  }

  /**
   * Delete role (logical delete / deactivate)
   * Uses dedicated deactivate endpoint with PATCH method
   */
  async deleteLogicalRol(id: number): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/roles/${id}/deactivate`, {});
  }

  /**
   * Activate role (reactivate a deactivated role)
   * Uses dedicated activate endpoint with PATCH method
   */
  async activateRol(id: number): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/roles/${id}/activate`, {});
  }

  /**
   * Get role by code
   */
  async getRolByCode(code: string): Promise<RolDto> {
    return this.get<RolDto>(`/roles/by-code/${code}`);
  }

  /**
   * Get roles by user ID
   */
  async getRolesByUserId(userId: number): Promise<RolDto[]> {
    return this.get<RolDto[]>(`/roles/by-user/${userId}`);
  }

  /**
   * Get all roles including inactive
   * TODO: Este endpoint no existe en la API backend - usar /roles como alternativa
   */
  async getAllRolesIncludingInactive(): Promise<RolDto[]> {
    // return this.get<RolDto[]>('/roles/all-including-inactive');
    console.warn('⚠️ Endpoint /roles/all-including-inactive no existe. Usando /roles como fallback');
    return this.get<RolDto[]>('/roles');
  }

  // ===== TAB PERMISSIONS MANAGEMENT =====

  /**
   * Get all users with their tab permissions
   */
  async getUserTabsPermissions(): Promise<UserTabPermissionDto[]> {
    return this.get<UserTabPermissionDto[]>('/permissions/user-tabs');
  }

  /**
   * Get tab permissions for a specific user
   */
  async getUserTabsPermission(userId: number): Promise<UserTabPermissionDto> {
    return this.get<UserTabPermissionDto>(`/permissions/user-tabs/${userId}`);
  }

  /**
   * Update user tab permissions
   */
  async updateUserTabs(data: UpdateUserTabsDto): Promise<{ success: boolean; message: string }> {
    return this.put<{ success: boolean; message: string }>('/permissions/user-tabs', data);
  }

  /**
   * Get all available tabs in the system
   */
  async getAvailableTabs(): Promise<TabPermissionDto[]> {
    return this.get<TabPermissionDto[]>('/permissions/available-tabs');
  }
}
