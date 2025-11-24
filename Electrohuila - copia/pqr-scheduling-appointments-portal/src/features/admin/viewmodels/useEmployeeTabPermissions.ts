/**
 * Hook - Employee Tab Permissions
 * Handles employee tab permissions management
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { UserDto } from '../models/admin.models';

interface UseEmployeeTabPermissionsReturn {
  selectedEmployeeId: number | null;
  employeeTabPermissions: { [tabId: string]: boolean };
  isEditingTabs: boolean;
  originalTabPermissions: { [tabId: string]: boolean };
  loading: boolean;
  error: string | null;
  setSelectedEmployeeId: React.Dispatch<React.SetStateAction<number | null>>;
  loadEmployeeTabPermissions: (employeeId: number, employees: UserDto[]) => Promise<void>;
  handleEmployeeChange: (employeeId: string, employees: UserDto[]) => void;
  handleToggleTabPermission: (tabId: string) => void;
  handleStartEditingTabs: () => void;
  handleCancelEditingTabs: () => void;
  handleSaveTabPermissions: () => Promise<void>;
  getActiveTabsCount: () => number;
}

const AVAILABLE_TABS = ['citas', 'sedes', 'empleados', 'tipos-cita', 'horas-disponibles', 'roles'];

export const useEmployeeTabPermissions = (repository: AdminRepository): UseEmployeeTabPermissionsReturn => {
  const [selectedEmployeeId, setSelectedEmployeeId] = useState<number | null>(null);
  const [employeeTabPermissions, setEmployeeTabPermissions] = useState<{ [tabId: string]: boolean }>({});
  const [isEditingTabs, setIsEditingTabs] = useState(false);
  const [originalTabPermissions, setOriginalTabPermissions] = useState<{ [tabId: string]: boolean }>({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadEmployeeTabPermissions = useCallback(async (employeeId: number, employees: UserDto[]) => {
    setLoading(true);
    setError(null);
    try {
      const employee = employees.find((emp: UserDto) => emp.id === employeeId);
      if (employee && employee.allowedTabs) {
        const allowedTabsArray = Array.isArray(employee.allowedTabs)
          ? employee.allowedTabs
          : (typeof employee.allowedTabs === 'string'
              ? (employee.allowedTabs as string).split(',').map((tab: string) => tab.trim())
              : []);

        const permissions: { [tabId: string]: boolean } = {};
        AVAILABLE_TABS.forEach(tabId => {
          permissions[tabId] = allowedTabsArray.includes(tabId);
        });

        setEmployeeTabPermissions(permissions);
        setOriginalTabPermissions({ ...permissions });
      } else {
        const permissions: { [tabId: string]: boolean } = {};
        AVAILABLE_TABS.forEach(tabId => {
          permissions[tabId] = false;
        });
        setEmployeeTabPermissions(permissions);
        setOriginalTabPermissions({ ...permissions });
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading employee tab permissions');
    } finally {
      setLoading(false);
    }
  }, []);

  const handleEmployeeChange = useCallback((employeeId: string, employees: UserDto[]) => {
    const id = employeeId ? parseInt(employeeId) : null;
    setSelectedEmployeeId(id);
    setIsEditingTabs(false);

    if (id) {
      loadEmployeeTabPermissions(id, employees);
    } else {
      setEmployeeTabPermissions({});
      setOriginalTabPermissions({});
    }
  }, [loadEmployeeTabPermissions]);

  const handleToggleTabPermission = useCallback((tabId: string) => {
    if (!isEditingTabs) return;

    setEmployeeTabPermissions(prev => ({
      ...prev,
      [tabId]: !prev[tabId]
    }));
  }, [isEditingTabs]);

  const handleStartEditingTabs = useCallback(() => {
    setIsEditingTabs(true);
  }, []);

  const handleCancelEditingTabs = useCallback(() => {
    setEmployeeTabPermissions({ ...originalTabPermissions });
    setIsEditingTabs(false);
  }, [originalTabPermissions]);

  const handleSaveTabPermissions = useCallback(async () => {
    if (!selectedEmployeeId) return;

    setLoading(true);
    setError(null);
    try {
      const allowedTabsArray = Object.entries(employeeTabPermissions)
        .filter(([, allowed]) => allowed)
        .map(([tabId]) => tabId);

      const updateData = {
        userId: selectedEmployeeId,
        allowedTabs: allowedTabsArray
      };

      const result = await repository.updateUserTabs(updateData);

      if (result.success) {
        setOriginalTabPermissions({ ...employeeTabPermissions });
        setIsEditingTabs(false);
      } else {
        throw new Error(result.message || 'Error updating employee tabs');
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error saving tab permissions');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [selectedEmployeeId, employeeTabPermissions, repository]);

  const getActiveTabsCount = useCallback(() => {
    return Object.values(employeeTabPermissions).filter(allowed => allowed).length;
  }, [employeeTabPermissions]);

  return {
    selectedEmployeeId,
    employeeTabPermissions,
    isEditingTabs,
    originalTabPermissions,
    loading,
    error,
    setSelectedEmployeeId,
    loadEmployeeTabPermissions,
    handleEmployeeChange,
    handleToggleTabPermission,
    handleStartEditingTabs,
    handleCancelEditingTabs,
    handleSaveTabPermissions,
    getActiveTabsCount
  };
};
