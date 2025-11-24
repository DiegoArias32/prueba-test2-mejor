/**
 * Authentication Types
 * All DTOs related to authentication, login, and user sessions
 */

export interface LoginDto {
  Username: string;
  Password: string;
  RememberMe?: boolean;
}

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

export interface LoginResponseDto {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserDto;
  roles: string[];
  permissions: string[];
  userPermissions?: UserPermissionsDto;
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
}
