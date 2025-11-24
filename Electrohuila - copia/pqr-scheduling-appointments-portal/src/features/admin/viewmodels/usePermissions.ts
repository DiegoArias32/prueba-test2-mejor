/**
 * Hook - Permissions Management
 * Handles permissions and role permission operations
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { PermissionViewDto, RolPermissionSummaryDto, UpdateRolFormPermissionDto } from '../models/admin.models';

interface UsePermissionsReturn {
  permissions: PermissionViewDto[];
  rolPermissionsSummary: RolPermissionSummaryDto[];
  loading: boolean;
  error: string | null;
  fetchPermissions: () => Promise<void>;
  fetchRolPermissionsSummary: () => Promise<void>;
  updateRolPermission: (rolId: number, formId: number, permissionId?: number) => Promise<void>;
}

export const usePermissions = (repository: AdminRepository): UsePermissionsReturn => {
  const [permissions, setPermissions] = useState<PermissionViewDto[]>([]);
  const [rolPermissionsSummary, setRolPermissionsSummary] = useState<RolPermissionSummaryDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchPermissions = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await repository.getAllPermissions();
      setPermissions(data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading permissions');
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const fetchRolPermissionsSummary = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await repository.getAllRolPermissionsSummary();
      setRolPermissionsSummary(data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading role permissions summary');
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const updateRolPermission = useCallback(async (rolId: number, formId: number, permissionId?: number) => {
    setLoading(true);
    setError(null);
    try {
      const updateData: UpdateRolFormPermissionDto = {
        RolId: rolId,
        FormId: formId,
        PermissionId: permissionId
      };

      const result = await repository.updateRolFormPermission(updateData);

      if (!result.success) {
        throw new Error('Could not update permission');
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error updating role permission');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  return {
    permissions,
    rolPermissionsSummary,
    loading,
    error,
    fetchPermissions,
    fetchRolPermissionsSummary,
    updateRolPermission
  };
};
