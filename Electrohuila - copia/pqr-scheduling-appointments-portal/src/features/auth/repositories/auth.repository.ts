/**
 * Repositorio de autenticación
 * Capa de acceso a datos para operaciones de autenticación
 */

import { httpClient } from '@/core/api/http-client';
import { LoginDto, LoginResponseDto, UserDto, UserPermissionsDto, FormPermissionDto } from '@/core/types';

// Constantes de almacenamiento
const STORAGE_KEYS = {
  TOKEN: 'token',
  REFRESH_TOKEN: 'refreshToken',
  USER: 'user',
  ROLES: 'roles',
  PERMISSIONS: 'permissions',
  USER_PERMISSIONS: 'userPermissions',
} as const;

export class AuthRepository {
  /**
   * Inicia sesión con credenciales
   */
  async login(credentials: LoginDto): Promise<LoginResponseDto> {
    // Backend espera LoginCommand { LoginDto: { ... } }
    const payload = {
      LoginDto: credentials
    };
    return httpClient.post<LoginResponseDto>('/auth/login', payload, {
      skipAuth: true,
    });
  }

  /**
   * Refresca el token de autenticación
   */
  async refreshToken(refreshToken: string): Promise<LoginResponseDto> {
    return httpClient.post<LoginResponseDto>(
      '/auth/refresh',
      { refreshToken },
      { skipAuth: true }
    );
  }

  /**
   * Cierra sesión
   */
  async logout(): Promise<void> {
    const refreshToken = this.getRefreshToken();
    try {
      if (refreshToken) {
        await httpClient.post<void>('/auth/logout', { refreshToken });
      }
    } finally {
      this.clearAuthData();
    }
  }

  /**
   * Obtiene información del usuario actual
   */
  async getCurrentUser(): Promise<UserDto> {
    return httpClient.get<UserDto>('/auth/user-info');
  }

  /**
   * Obtiene permisos del usuario actual
   */
  async getCurrentUserPermissions(): Promise<UserPermissionsDto> {
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

    const response = await httpClient.get<PermissionsResponse | FormItem[]>('/auth/permissions');

    // El backend devuelve directamente un array: [ {...}, {...} ]
    // Necesitamos transformarlo a { forms: { "CODE": {...} }, modules: [] }
    const transformedForms: { [formCode: string]: FormPermissionDto } = {};

    let formsArray: FormItem[] = [];

    // La respuesta puede venir como array directo o dentro de response.forms
    if (Array.isArray(response)) {
      formsArray = response;
    } else if (response.forms && Array.isArray(response.forms)) {
      formsArray = response.forms;
    }

    formsArray.forEach((form) => {
      // Combinar todos los permisos de este formulario
      const combinedPermissions = {
        canRead: false,
        canCreate: false,
        canUpdate: false,
        canDelete: false
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
        moduleCode: '', // No viene del backend
        moduleName: ''  // No viene del backend
      };
    });

    return {
      forms: transformedForms,
      modules: [] // El backend no devuelve módulos en este endpoint
    };
  }

  /**
   * Guarda datos de autenticación en localStorage
   */
  saveAuthData(data: LoginResponseDto): void {
    if (typeof window === 'undefined') return;

    localStorage.setItem(STORAGE_KEYS.TOKEN, data.accessToken);
    localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, data.refreshToken);
    localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(data.user));
    localStorage.setItem(STORAGE_KEYS.ROLES, JSON.stringify(data.roles));
    localStorage.setItem(STORAGE_KEYS.PERMISSIONS, JSON.stringify(data.permissions));

    if (data.userPermissions) {
      localStorage.setItem(STORAGE_KEYS.USER_PERMISSIONS, JSON.stringify(data.userPermissions));
    }
  }

  /**
   * Limpia datos de autenticación
   */
  clearAuthData(): void {
    if (typeof window === 'undefined') return;

    Object.values(STORAGE_KEYS).forEach((key) => {
      localStorage.removeItem(key);
    });
  }

  /**
   * Obtiene el token almacenado
   */
  getToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem(STORAGE_KEYS.TOKEN);
  }

  /**
   * Obtiene el refresh token almacenado
   */
  getRefreshToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
  }

  /**
   * Obtiene el usuario almacenado
   */
  getStoredUser(): UserDto | null {
    if (typeof window === 'undefined') return null;
    const userStr = localStorage.getItem(STORAGE_KEYS.USER);
    return userStr ? JSON.parse(userStr) : null;
  }

  /**
   * Obtiene los roles almacenados
   */
  getStoredRoles(): string[] {
    if (typeof window === 'undefined') return [];
    const rolesStr = localStorage.getItem(STORAGE_KEYS.ROLES);
    return rolesStr ? JSON.parse(rolesStr) : [];
  }

  /**
   * Obtiene permisos almacenados
   */
  getStoredPermissions(): UserPermissionsDto {
    if (typeof window === 'undefined')
      return { forms: {}, modules: [] };

    const permissionsStr = localStorage.getItem(STORAGE_KEYS.USER_PERMISSIONS);

    if (!permissionsStr) {
      return { forms: {}, modules: [] };
    }

    try {
      return JSON.parse(permissionsStr);
    } catch (error) {
      console.error('Error parseando permisos:', error);
      return { forms: {}, modules: [] };
    }
  }

  /**
   * Verifica si el usuario está autenticado
   */
  isAuthenticated(): boolean {
    const token = this.getToken();
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
   * Verifica permiso específico en un formulario
   */
  hasFormPermission(
    formCode: string,
    permission: 'read' | 'create' | 'update' | 'delete'
  ): boolean {
    const permissions = this.getStoredPermissions();
    const formPermission = permissions.forms[formCode];
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
}

// Instancia singleton del repositorio
export const authRepository = new AuthRepository();
