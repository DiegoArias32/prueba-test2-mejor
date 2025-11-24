/**
 * Modelos del feature de autenticación
 */

export interface LoginCredentials {
  usernameOrEmail: string;
  password: string;
}

export interface AuthUser {
  id: number;
  username: string;
  email: string;
  roles: string[];
  allowedTabs?: string[];
}

export interface AuthState {
  user: AuthUser | null;
  token: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  permissions: Record<string, unknown>;
}

export interface FormPermission {
  formCode: string;
  formName: string;
  canRead: boolean;
  canCreate: boolean;
  canUpdate: boolean;
  canDelete: boolean;
  moduleCode: string;
  moduleName: string;
}
