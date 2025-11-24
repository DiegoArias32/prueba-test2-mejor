/**
 * Tipos y DTOs relacionados con autenticación
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
  accessToken: string;      // Backend devuelve AccessToken
  refreshToken: string;      // Backend devuelve RefreshToken
  expiresAt: string;         // Backend devuelve ExpiresAt
  user: UserDto;             // Backend devuelve User
  roles: string[];           // Backend devuelve Roles
  permissions: string[];     // Backend devuelve Permissions
  userPermissions?: UserPermissionsDto; // Opcional, puede no venir del backend
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
