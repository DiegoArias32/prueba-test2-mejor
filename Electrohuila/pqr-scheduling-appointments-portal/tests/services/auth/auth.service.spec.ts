import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { AuthService } from '@/services/auth/auth.service';
import type {
  LoginDto,
  LoginResponseDto,
  UserDto,
  UserPermissionsDto,
  FormPermissionDto,
} from '@/services/auth/auth.types';

/**
 * Unit tests for AuthService
 * Tests authentication, token management, user session, and permissions
 */
describe('AuthService - Authentication & Authorization', () => {
  let service: AuthService;
  let localStorageMock: { [key: string]: string };

  beforeEach(() => {
    // Mock localStorage
    localStorageMock = {};

    global.localStorage = {
      getItem: vi.fn((key: string) => localStorageMock[key] || null),
      setItem: vi.fn((key: string, value: string) => {
        localStorageMock[key] = value;
      }),
      removeItem: vi.fn((key: string) => {
        delete localStorageMock[key];
      }),
      clear: vi.fn(() => {
        localStorageMock = {};
      }),
      length: 0,
      key: vi.fn(),
    } as Storage;

    // Mock window
    Object.defineProperty(global, 'window', {
      value: { localStorage: global.localStorage },
      writable: true,
    });

    service = new AuthService();

    // Spy on HTTP methods
    vi.spyOn(service as any, 'post').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'get').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'clearAuthData').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'getAuthToken').mockImplementation(vi.fn());
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  // ===== LOGIN TESTS =====

  describe('login', () => {
    it('should login user successfully with valid credentials', async () => {
      const loginData: LoginDto = {
        Username: 'testuser',
        Password: 'password123',
        RememberMe: true,
      };

      const expectedResponse: LoginResponseDto = {
        accessToken: 'mock-access-token',
        refreshToken: 'mock-refresh-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        user: {
          id: 1,
          username: 'testuser',
          email: 'test@example.com',
          roles: ['User'],
          isActive: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
        roles: ['User'],
        permissions: ['read', 'create'],
      };

      (service as any).post.mockResolvedValue(expectedResponse);

      const result = await service.login(loginData);

      expect(result).toEqual(expectedResponse);
      expect((service as any).post).toHaveBeenCalledWith('/auth/login', {
        LoginDto: loginData,
      });
    });

    it('should throw error when login fails with invalid credentials', async () => {
      const loginData: LoginDto = {
        Username: 'wronguser',
        Password: 'wrongpass',
      };

      const error = new Error('Invalid credentials');
      (service as any).post.mockRejectedValue(error);

      await expect(service.login(loginData)).rejects.toThrow('Invalid credentials');
    });
  });

  // ===== REFRESH TOKEN TESTS =====

  describe('refreshToken', () => {
    it('should refresh token successfully', async () => {
      const refreshToken = 'mock-refresh-token';
      const expectedResponse: LoginResponseDto = {
        accessToken: 'new-access-token',
        refreshToken: 'new-refresh-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        user: {
          id: 1,
          username: 'testuser',
          email: 'test@example.com',
          roles: ['User'],
          isActive: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
        roles: ['User'],
        permissions: ['read', 'create'],
      };

      (service as any).post.mockResolvedValue(expectedResponse);

      const result = await service.refreshToken(refreshToken);

      expect(result).toEqual(expectedResponse);
      expect((service as any).post).toHaveBeenCalledWith('/auth/refresh', { refreshToken });
    });

    it('should throw error when refresh token is invalid', async () => {
      const refreshToken = 'invalid-token';
      const error = new Error('Invalid refresh token');
      (service as any).post.mockRejectedValue(error);

      await expect(service.refreshToken(refreshToken)).rejects.toThrow('Invalid refresh token');
    });
  });

  // ===== LOGOUT TESTS =====

  describe('logout', () => {
    it('should logout user successfully with valid refresh token', async () => {
      localStorageMock['refreshToken'] = 'mock-refresh-token';
      (service as any).post.mockResolvedValue(undefined);

      await service.logout();

      expect((service as any).post).toHaveBeenCalledWith('/auth/logout', {
        refreshToken: 'mock-refresh-token',
      });
      expect((service as any).clearAuthData).toHaveBeenCalled();
    });

    it('should clear auth data even if logout API call fails', async () => {
      localStorageMock['refreshToken'] = 'mock-refresh-token';
      const error = new Error('API Error');
      (service as any).post.mockRejectedValue(error);

      await service.logout();

      expect((service as any).clearAuthData).toHaveBeenCalled();
    });

    it('should clear auth data when no refresh token exists', async () => {
      (service as any).post.mockResolvedValue(undefined);

      await service.logout();

      expect((service as any).post).not.toHaveBeenCalled();
      expect((service as any).clearAuthData).toHaveBeenCalled();
    });
  });

  // ===== IS AUTHENTICATED TESTS =====

  describe('isAuthenticated', () => {
    it('should return true when token is valid and not expired', () => {
      const futureTimestamp = Math.floor(Date.now() / 1000) + 3600; // 1 hour from now
      const payload = { exp: futureTimestamp };
      const token = `header.${btoa(JSON.stringify(payload))}.signature`;

      (service as any).getAuthToken.mockReturnValue(token);

      const result = service.isAuthenticated();

      expect(result).toBe(true);
    });

    it('should return false when token is expired', () => {
      const pastTimestamp = Math.floor(Date.now() / 1000) - 3600; // 1 hour ago
      const payload = { exp: pastTimestamp };
      const token = `header.${btoa(JSON.stringify(payload))}.signature`;

      (service as any).getAuthToken.mockReturnValue(token);

      const result = service.isAuthenticated();

      expect(result).toBe(false);
    });

    it('should return false when token does not exist', () => {
      (service as any).getAuthToken.mockReturnValue(null);

      const result = service.isAuthenticated();

      expect(result).toBe(false);
    });

    it('should return false when token is malformed', () => {
      (service as any).getAuthToken.mockReturnValue('invalid.token');

      const result = service.isAuthenticated();

      expect(result).toBe(false);
    });
  });

  // ===== GET CURRENT USER TESTS =====

  describe('getCurrentUser', () => {
    it('should return user from localStorage', () => {
      const mockUser: UserDto = {
        id: 1,
        username: 'testuser',
        email: 'test@example.com',
        roles: ['User'],
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };

      localStorageMock['user'] = JSON.stringify(mockUser);

      const result = service.getCurrentUser();

      expect(result).toEqual(mockUser);
    });

    it('should return null when user is not in localStorage', () => {
      const result = service.getCurrentUser();

      expect(result).toBeNull();
    });
  });

  // ===== GET CURRENT USER ROLES TESTS =====

  describe('getCurrentUserRoles', () => {
    it('should return roles from localStorage', () => {
      const mockRoles = ['Admin', 'User'];
      localStorageMock['roles'] = JSON.stringify(mockRoles);

      const result = service.getCurrentUserRoles();

      expect(result).toEqual(mockRoles);
    });

    it('should return empty array when roles are not in localStorage', () => {
      const result = service.getCurrentUserRoles();

      expect(result).toEqual([]);
    });
  });

  // ===== GET CURRENT USER PERMISSIONS TESTS =====

  describe('getCurrentUserPermissions', () => {
    it('should return permissions from localStorage', () => {
      const mockPermissions = ['read', 'create', 'update'];
      localStorageMock['permissions'] = JSON.stringify(mockPermissions);

      const result = service.getCurrentUserPermissions();

      expect(result).toEqual(mockPermissions);
    });

    it('should return empty array when permissions are not in localStorage', () => {
      const result = service.getCurrentUserPermissions();

      expect(result).toEqual([]);
    });
  });

  // ===== GET CURRENT USER PERMISSIONS DETAILED TESTS =====

  describe('getCurrentUserPermissionsDetailed', () => {
    it('should return detailed permissions from localStorage', () => {
      const mockPermissions: UserPermissionsDto = {
        forms: {
          'FORM_APPOINTMENTS': {
            formCode: 'FORM_APPOINTMENTS',
            formName: 'Appointments',
            canRead: true,
            canCreate: true,
            canUpdate: true,
            canDelete: false,
            moduleCode: 'MOD_APPOINTMENTS',
            moduleName: 'Appointments Module',
          },
        },
        modules: [],
      };

      localStorageMock['userPermissions'] = JSON.stringify(mockPermissions);

      const result = service.getCurrentUserPermissionsDetailed();

      expect(result).toEqual(mockPermissions);
    });

    it('should return default structure when permissions are not in localStorage', () => {
      const result = service.getCurrentUserPermissionsDetailed();

      expect(result).toEqual({ forms: {}, modules: [] });
    });
  });

  // ===== HAS FORM PERMISSION TESTS =====

  describe('hasFormPermission', () => {
    const setupPermissions = () => {
      const mockPermissions: UserPermissionsDto = {
        forms: {
          'FORM_APPOINTMENTS': {
            formCode: 'FORM_APPOINTMENTS',
            formName: 'Appointments',
            canRead: true,
            canCreate: true,
            canUpdate: false,
            canDelete: false,
            moduleCode: 'MOD_APPOINTMENTS',
            moduleName: 'Appointments Module',
          },
        },
        modules: [],
      };

      localStorageMock['userPermissions'] = JSON.stringify(mockPermissions);
    };

    it('should return true when user has read permission on form', () => {
      setupPermissions();

      const result = service.hasFormPermission('FORM_APPOINTMENTS', 'read');

      expect(result).toBe(true);
    });

    it('should return true when user has create permission on form', () => {
      setupPermissions();

      const result = service.hasFormPermission('FORM_APPOINTMENTS', 'create');

      expect(result).toBe(true);
    });

    it('should return false when user does not have update permission on form', () => {
      setupPermissions();

      const result = service.hasFormPermission('FORM_APPOINTMENTS', 'update');

      expect(result).toBe(false);
    });

    it('should return false when user does not have delete permission on form', () => {
      setupPermissions();

      const result = service.hasFormPermission('FORM_APPOINTMENTS', 'delete');

      expect(result).toBe(false);
    });

    it('should return false when form does not exist in permissions', () => {
      setupPermissions();

      const result = service.hasFormPermission('NON_EXISTENT_FORM', 'read');

      expect(result).toBe(false);
    });
  });

  // ===== GET CURRENT USER MODULES TESTS =====

  describe('getCurrentUserModules', () => {
    it('should return user modules from permissions', () => {
      const mockPermissions: UserPermissionsDto = {
        forms: {},
        modules: [
          {
            id: 1,
            name: 'Appointments Module',
            code: 'MOD_APPOINTMENTS',
            forms: [{ id: 1, name: 'Appointments', code: 'FORM_APPOINTMENTS' }],
          },
        ],
      };

      localStorageMock['userPermissions'] = JSON.stringify(mockPermissions);

      const result = service.getCurrentUserModules();

      expect(result).toEqual(mockPermissions.modules);
    });

    it('should return empty array when no modules exist', () => {
      const result = service.getCurrentUserModules();

      expect(result).toEqual([]);
    });
  });

  // ===== GET CURRENT USER FROM SERVER TESTS =====

  describe('getCurrentUserFromServer', () => {
    it('should fetch user from server and store in localStorage', async () => {
      const mockServerResponse = {
        id: 1,
        username: 'testuser',
        email: 'test@example.com',
        roles: ['User', 'Admin'],
      };

      const expectedUser: UserDto = {
        id: 1,
        username: 'testuser',
        email: 'test@example.com',
        roles: ['User', 'Admin'],
        isActive: true,
        createdAt: expect.any(String),
        updatedAt: expect.any(String),
      };

      (service as any).get.mockResolvedValue(mockServerResponse);

      const result = await service.getCurrentUserFromServer();

      expect(result).toMatchObject({
        id: expectedUser.id,
        username: expectedUser.username,
        email: expectedUser.email,
        roles: expectedUser.roles,
        isActive: expectedUser.isActive,
      });
      expect((service as any).get).toHaveBeenCalledWith('/auth/user-info');
      expect(localStorage.setItem).toHaveBeenCalledWith('user', expect.any(String));
    });

    it('should handle string id from server response', async () => {
      const mockServerResponse = {
        id: '123',
        username: 'testuser',
        email: 'test@example.com',
        roles: ['User'],
      };

      (service as any).get.mockResolvedValue(mockServerResponse);

      const result = await service.getCurrentUserFromServer();

      expect(result?.id).toBe(123);
    });

    it('should return null when server request fails', async () => {
      (service as any).get.mockRejectedValue(new Error('Network error'));

      const result = await service.getCurrentUserFromServer();

      expect(result).toBeNull();
    });
  });

  // ===== GET CURRENT USER ROLES FROM SERVER TESTS =====

  describe('getCurrentUserRolesFromServer', () => {
    it('should fetch roles from server and store in localStorage', async () => {
      const mockServerResponse = {
        id: 1,
        username: 'testuser',
        email: 'test@example.com',
        roles: ['Admin', 'User'],
      };

      (service as any).get.mockResolvedValue(mockServerResponse);

      const result = await service.getCurrentUserRolesFromServer();

      expect(result).toEqual(['Admin', 'User']);
      expect(localStorage.setItem).toHaveBeenCalledWith('roles', JSON.stringify(['Admin', 'User']));
    });

    it('should return empty array when user has empty roles', async () => {
      const mockServerResponse = {
        id: 1,
        username: 'testuser',
        email: 'test@example.com',
        roles: [],
      };

      (service as any).get.mockResolvedValue(mockServerResponse);

      const result = await service.getCurrentUserRolesFromServer();

      expect(result).toEqual([]);
    });

    it('should return null when server request fails', async () => {
      (service as any).get.mockRejectedValue(new Error('Network error'));

      const result = await service.getCurrentUserRolesFromServer();

      expect(result).toBeNull();
    });
  });

  // ===== GET CURRENT USER PERMISSIONS FROM SERVER TESTS =====

  describe('getCurrentUserPermissionsFromServer', () => {
    it('should fetch permissions from server and store in localStorage (array format)', async () => {
      const mockServerResponse = [
        {
          formCode: 'FORM_APPOINTMENTS',
          formName: 'Appointments',
          permissions: [
            {
              canRead: true,
              canCreate: true,
              canUpdate: false,
              canDelete: false,
            },
          ],
        },
      ];

      (service as any).get.mockResolvedValue(mockServerResponse);

      const result = await service.getCurrentUserPermissionsFromServer();

      expect(result).toBeDefined();
      expect(result?.forms['FORM_APPOINTMENTS']).toEqual({
        formCode: 'FORM_APPOINTMENTS',
        formName: 'Appointments',
        canRead: true,
        canCreate: true,
        canUpdate: false,
        canDelete: false,
        moduleCode: '',
        moduleName: '',
      });
      expect(localStorage.setItem).toHaveBeenCalledWith('userPermissions', expect.any(String));
      expect(localStorage.setItem).toHaveBeenCalledWith('permissions', expect.any(String));
    });

    it('should fetch permissions from server and store in localStorage (object format)', async () => {
      const mockServerResponse = {
        forms: [
          {
            formCode: 'FORM_USERS',
            formName: 'Users',
            permissions: [
              {
                canRead: true,
                canCreate: false,
                canUpdate: true,
                canDelete: true,
              },
            ],
          },
        ],
      };

      (service as any).get.mockResolvedValue(mockServerResponse);

      const result = await service.getCurrentUserPermissionsFromServer();

      expect(result).toBeDefined();
      expect(result?.forms['FORM_USERS']).toEqual({
        formCode: 'FORM_USERS',
        formName: 'Users',
        canRead: true,
        canCreate: false,
        canUpdate: true,
        canDelete: true,
        moduleCode: '',
        moduleName: '',
      });
    });

    it('should combine multiple permissions using OR logic', async () => {
      const mockServerResponse = [
        {
          formCode: 'FORM_TEST',
          formName: 'Test Form',
          permissions: [
            {
              canRead: true,
              canCreate: false,
              canUpdate: false,
              canDelete: false,
            },
            {
              canRead: false,
              canCreate: true,
              canUpdate: false,
              canDelete: false,
            },
          ],
        },
      ];

      (service as any).get.mockResolvedValue(mockServerResponse);

      const result = await service.getCurrentUserPermissionsFromServer();

      expect(result?.forms['FORM_TEST']).toEqual({
        formCode: 'FORM_TEST',
        formName: 'Test Form',
        canRead: true,
        canCreate: true,
        canUpdate: false,
        canDelete: false,
        moduleCode: '',
        moduleName: '',
      });
    });

    it('should return null when server request fails', async () => {
      (service as any).get.mockRejectedValue(new Error('Network error'));

      const result = await service.getCurrentUserPermissionsFromServer();

      expect(result).toBeNull();
    });
  });

  // ===== BUSINESS LOGIC SCENARIOS =====

  describe('Business Logic Scenarios', () => {
    it('should complete full login flow with all user data', async () => {
      const loginData: LoginDto = {
        Username: 'admin',
        Password: 'admin123',
        RememberMe: true,
      };

      const loginResponse: LoginResponseDto = {
        accessToken: 'access-token',
        refreshToken: 'refresh-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        user: {
          id: 1,
          username: 'admin',
          email: 'admin@example.com',
          roles: ['Admin'],
          isActive: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
        roles: ['Admin'],
        permissions: ['read', 'create', 'update', 'delete'],
        userPermissions: {
          forms: {
            'FORM_APPOINTMENTS': {
              formCode: 'FORM_APPOINTMENTS',
              formName: 'Appointments',
              canRead: true,
              canCreate: true,
              canUpdate: true,
              canDelete: true,
              moduleCode: 'MOD_APPOINTMENTS',
              moduleName: 'Appointments Module',
            },
          },
          modules: [],
        },
      };

      (service as any).post.mockResolvedValue(loginResponse);

      const result = await service.login(loginData);

      expect(result).toEqual(loginResponse);
      expect(result.user.username).toBe('admin');
      expect(result.roles).toContain('Admin');
      expect(result.permissions).toContain('delete');
    });

    it('should handle token refresh before expiration', async () => {
      const oldRefreshToken = 'old-refresh-token';
      const newLoginResponse: LoginResponseDto = {
        accessToken: 'new-access-token',
        refreshToken: 'new-refresh-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        user: {
          id: 1,
          username: 'testuser',
          email: 'test@example.com',
          roles: ['User'],
          isActive: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
        roles: ['User'],
        permissions: ['read'],
      };

      (service as any).post.mockResolvedValue(newLoginResponse);

      const result = await service.refreshToken(oldRefreshToken);

      expect(result.accessToken).toBe('new-access-token');
      expect(result.refreshToken).toBe('new-refresh-token');
      expect(result.accessToken).not.toBe(oldRefreshToken);
    });

    it('should handle permission check for restricted form access', () => {
      const mockPermissions: UserPermissionsDto = {
        forms: {
          'FORM_APPOINTMENTS': {
            formCode: 'FORM_APPOINTMENTS',
            formName: 'Appointments',
            canRead: true,
            canCreate: false,
            canUpdate: false,
            canDelete: false,
            moduleCode: 'MOD_APPOINTMENTS',
            moduleName: 'Appointments Module',
          },
        },
        modules: [],
      };

      localStorageMock['userPermissions'] = JSON.stringify(mockPermissions);

      expect(service.hasFormPermission('FORM_APPOINTMENTS', 'read')).toBe(true);
      expect(service.hasFormPermission('FORM_APPOINTMENTS', 'create')).toBe(false);
      expect(service.hasFormPermission('FORM_APPOINTMENTS', 'update')).toBe(false);
      expect(service.hasFormPermission('FORM_APPOINTMENTS', 'delete')).toBe(false);
    });
  });
});
