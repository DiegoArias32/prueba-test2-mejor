/**
 * Authentication Service
 * Handles authentication, login, logout, and user session management
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  LoginDto,
  LoginResponseDto,
  UserDto,
  UserPermissionsDto,
  FormPermissionDto,
} from './auth.types';

export class AuthService extends BaseHttpService {
  /**
   * Login user with credentials
   */
  async login(loginData: LoginDto): Promise<LoginResponseDto> {
    const payload = {
      LoginDto: loginData,
    };
    return this.post<LoginResponseDto>('/auth/login', payload);
  }

  /**
   * Refresh authentication token
   */
  async refreshToken(refreshToken: string): Promise<LoginResponseDto> {
    return this.post<LoginResponseDto>('/auth/refresh', { refreshToken });
  }

  /**
   * Logout user
   */
  async logout(): Promise<void> {
    const refreshToken = typeof window !== 'undefined' ? localStorage.getItem('refreshToken') : null;
    try {
      if (refreshToken) {
        await this.post<void>('/auth/logout', { refreshToken });
      }
    } catch {
      // Error during logout
    } finally {
      this.clearAuthData();
    }
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    const token = this.getAuthToken();
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);
      return payload.exp > currentTime;
    } catch {
      return false;
    }
  }

  /**
   * Get current user from localStorage
   */
  getCurrentUser(): UserDto | null {
    if (typeof window === 'undefined') return null;
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  /**
   * Get current user roles from localStorage
   */
  getCurrentUserRoles(): string[] {
    if (typeof window === 'undefined') return [];
    const rolesStr = localStorage.getItem('roles');
    return rolesStr ? JSON.parse(rolesStr) : [];
  }

  /**
   * Get current user permissions from localStorage
   */
  getCurrentUserPermissions(): string[] {
    if (typeof window === 'undefined') return [];
    const permissionsStr = localStorage.getItem('permissions');
    return permissionsStr ? JSON.parse(permissionsStr) : [];
  }

  /**
   * Get current user detailed permissions from localStorage
   */
  getCurrentUserPermissionsDetailed(): UserPermissionsDto {
    if (typeof window === 'undefined') return { forms: {}, modules: [] };
    const userPermissionsStr = localStorage.getItem('userPermissions');
    return userPermissionsStr ? JSON.parse(userPermissionsStr) : { forms: {}, modules: [] };
  }

  /**
   * Check if user has specific permission on a form
   */
  hasFormPermission(formCode: string, permission: 'read' | 'create' | 'update' | 'delete'): boolean {
    const userPermissions = this.getCurrentUserPermissionsDetailed();
    const formPermission = userPermissions.forms[formCode];
    if (!formPermission) return false;

    switch (permission) {
      case 'read':
        return formPermission.canRead;
      case 'create':
        return formPermission.canCreate;
      case 'update':
        return formPermission.canUpdate;
      case 'delete':
        return formPermission.canDelete;
      default:
        return false;
    }
  }

  /**
   * Get current user modules from localStorage
   */
  getCurrentUserModules() {
    const userPermissions = this.getCurrentUserPermissionsDetailed();
    return userPermissions.modules;
  }

  /**
   * Get current user from server
   */
  async getCurrentUserFromServer(): Promise<UserDto | null> {
    try {
      interface UserInfoResponse {
        id: string | number;
        username: string;
        email: string;
        roles?: string[];
      }

      const userInfo = await this.get<UserInfoResponse>('/auth/user-info');

      const user: UserDto = {
        id: typeof userInfo.id === 'string' ? parseInt(userInfo.id) : userInfo.id,
        username: userInfo.username,
        email: userInfo.email,
        roles: userInfo.roles || [],
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };

      if (typeof window !== 'undefined') {
        localStorage.setItem('user', JSON.stringify(user));
      }

      return user;
    } catch {
      return null;
    }
  }

  /**
   * Get current user roles from server
   */
  async getCurrentUserRolesFromServer(): Promise<string[] | null> {
    try {
      const user = await this.getCurrentUserFromServer();
      if (!user || !user.roles) return null;

      const roles = user.roles;

      if (typeof window !== 'undefined') {
        localStorage.setItem('roles', JSON.stringify(roles));
      }

      return roles;
    } catch {
      return null;
    }
  }

  /**
   * Get current user permissions from server
   */
  async getCurrentUserPermissionsFromServer(): Promise<UserPermissionsDto | null> {
    try {
      interface PermissionItem {
        canRead: boolean;
        canCreate: boolean;
        canUpdate: boolean;
        canDelete: boolean;
      }

      interface FormItem {
        formCode: string;
        formName: string;
        permissions?: PermissionItem[];
      }

      interface PermissionsResponse {
        forms?: FormItem[];
      }

      const response = await this.get<PermissionsResponse | FormItem[]>('/auth/permissions');

      const transformedForms: { [formCode: string]: FormPermissionDto } = {};
      let formsArray: FormItem[] = [];

      if (Array.isArray(response)) {
        formsArray = response;
      } else if (response.forms && Array.isArray(response.forms)) {
        formsArray = response.forms;
      }

      formsArray.forEach((form) => {
        const combinedPermissions = {
          canRead: false,
          canCreate: false,
          canUpdate: false,
          canDelete: false,
        };

        if (form.permissions && Array.isArray(form.permissions)) {
          form.permissions.forEach((perm) => {
            combinedPermissions.canRead = combinedPermissions.canRead || perm.canRead;
            combinedPermissions.canCreate = combinedPermissions.canCreate || perm.canCreate;
            combinedPermissions.canUpdate = combinedPermissions.canUpdate || perm.canUpdate;
            combinedPermissions.canDelete = combinedPermissions.canDelete || perm.canDelete;
          });
        }

        transformedForms[form.formCode] = {
          formCode: form.formCode,
          formName: form.formName,
          ...combinedPermissions,
          moduleCode: '',
          moduleName: '',
        };
      });

      const permissions: UserPermissionsDto = {
        forms: transformedForms,
        modules: [],
      };

      if (typeof window !== 'undefined') {
        localStorage.setItem('userPermissions', JSON.stringify(permissions));

        const simplePermissions = Object.keys(permissions.forms);
        localStorage.setItem('permissions', JSON.stringify(simplePermissions));
      }

      return permissions;
    } catch {
      return null;
    }
  }
}
