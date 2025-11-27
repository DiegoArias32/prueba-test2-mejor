/**
 * Hook - User Permissions
 * Handles current user authentication and permissions
 */

'use client';

import { useState, useCallback, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { AdminRepository } from '../repositories/admin.repository';
import { UserDto, FormPermissionDto, TabType, TAB_FORM_CODE_MAP } from '../models/admin.models';

interface UseUserPermissionsReturn {
  currentUser: UserDto | null;
  userRoles: string[];
  userPermissions: { [formCode: string]: FormPermissionDto };
  hasInitialPermissions: boolean;
  permissionUpdateKey: number;
  loading: boolean;
  error: string | null;
  hasPermission: (formCode: string, permission: 'read' | 'create' | 'update' | 'delete') => boolean;
  getFormCodeForTab: (tab: TabType) => string | null;
  getAvailableTabs: (permissions?: { [formCode: string]: FormPermissionDto }) => TabType[];
  reloadUserPermissions: () => Promise<{ [formCode: string]: FormPermissionDto } | null>;
}

export const useUserPermissions = (repository: AdminRepository): UseUserPermissionsReturn => {
  const router = useRouter();
  const [currentUser, setCurrentUser] = useState<UserDto | null>(null);
  const [userRoles, setUserRoles] = useState<string[]>([]);
  const [userPermissions, setUserPermissions] = useState<{ [formCode: string]: FormPermissionDto }>({});
  const [hasInitialPermissions, setHasInitialPermissions] = useState(false);
  const [permissionUpdateKey, setPermissionUpdateKey] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const getFormCodeForTab = useCallback((tab: TabType): string | null => {
    return TAB_FORM_CODE_MAP[tab] || null;
  }, []);

  const hasPermission = useCallback((formCode: string, permission: 'read' | 'create' | 'update' | 'delete'): boolean => {
    const formPermission = userPermissions[formCode];
    if (!formPermission) return false;

    switch (permission) {
      case 'read': return formPermission.canRead;
      case 'create': return formPermission.canCreate;
      case 'update': return formPermission.canUpdate;
      case 'delete': return formPermission.canDelete;
      default: return false;
    }
  }, [userPermissions]);

  const getAvailableTabs = useCallback((permissions?: { [formCode: string]: FormPermissionDto }): TabType[] => {
    const permsToUse = permissions || userPermissions;

    const allTabs: TabType[] = [
      'citas',
      'empleados',
      'roles',
      'sedes',
      'tipos-cita',
      'horas-disponibles',
      'permisos',
      'festivos',
      'settings'
    ];

    const available = allTabs.filter(tab => {
      const formCode = getFormCodeForTab(tab);
      if (!formCode) return true;

      const formPermission = permsToUse[formCode];
      const hasFormAccess = formPermission ? formPermission.canRead : false;

      let userAllowedTabs: string[] = [];
      if (typeof window !== 'undefined') {
        const currentUser = JSON.parse(localStorage.getItem('user') || '{}');
        userAllowedTabs = currentUser.allowedTabs || [];
      }
      const hasTabAccess = userAllowedTabs.includes(tab);

      return hasFormAccess || hasTabAccess;
    });
    return available;
  }, [userPermissions, getFormCodeForTab]);

  const reloadUserPermissions = useCallback(async (): Promise<{ [formCode: string]: FormPermissionDto } | null> => {
    setLoading(true);
    setError(null);
    try {
      const updatedUser = await repository.getCurrentUserFromServer();
      const updatedRoles = await repository.getCurrentUserRolesFromServer();
      const updatedPermissions = await repository.getCurrentUserPermissionsFromServer();

      if (updatedUser && updatedRoles && updatedPermissions) {
        setCurrentUser(updatedUser);
        setUserRoles(updatedRoles);
        setUserPermissions(updatedPermissions.forms);
        setPermissionUpdateKey(prev => prev + 1);
        setHasInitialPermissions(true);
        return updatedPermissions.forms;
      }
      return null;
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error reloading permissions');
      return null;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  useEffect(() => {
    const initializeUser = async () => {
      if (!repository.isAuthenticated()) {
        router.push('/login');
        return;
      }

      setLoading(true);
      try {
        const user = await repository.getCurrentUserFromServer();
        const roles = await repository.getCurrentUserRolesFromServer();
        const permissions = await repository.getCurrentUserPermissionsFromServer();

        if (!user || !permissions) {
          router.push('/login');
          return;
        }

        setCurrentUser(user);
        setUserRoles(roles || []);
        setUserPermissions(permissions.forms);
        setPermissionUpdateKey(prev => prev + 1);
        setHasInitialPermissions(true);
      } catch (err: unknown) {
        setError(err instanceof Error ? err.message : 'Error initializing user');
        router.push('/login');
      } finally {
        setLoading(false);
      }
    };

    initializeUser();
  }, [router, repository]);

  return {
    currentUser,
    userRoles,
    userPermissions,
    hasInitialPermissions,
    permissionUpdateKey,
    loading,
    error,
    hasPermission,
    getFormCodeForTab,
    getAvailableTabs,
    reloadUserPermissions
  };
};
