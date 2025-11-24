/**
 * User Types
 * All DTOs related to user management
 */

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
