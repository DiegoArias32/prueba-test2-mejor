/**
 * Hook - Roles Management
 * Handles all role CRUD operations
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { RolDto, UpdateRolDto } from '../models/admin.models';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface UseRolesReturn {
  roles: RolDto[];
  loading: boolean;
  error: string | null;
  formData: Partial<RolDto>;
  formErrors: FormErrors;
  touchedFields: { [key: string]: boolean };
  expandedRoles: Set<number>;
  setFormData: React.Dispatch<React.SetStateAction<Partial<RolDto>>>;
  fetchRoles: (activeOnly?: boolean) => Promise<void>;
  createRole: (data: Partial<RolDto>) => Promise<void>;
  updateRole: (id: number, data: UpdateRolDto) => Promise<void>;
  deleteRole: (id: number, logical?: boolean) => Promise<void>;
  activateRole: (id: number) => Promise<void>;
  toggleRoleAccordion: (roleId: number) => void;
  validateField: (fieldName: string, value: string | number | boolean | null | undefined) => string;
  validateForm: (data: Partial<RolDto>) => FormErrors;
  clearFormErrors: () => void;
  handleFieldChange: (fieldName: string, value: string | number | boolean | null | undefined) => void;
  resetForm: () => void;
}

export const useRoles = (repository: AdminRepository): UseRolesReturn => {
  const [roles, setRoles] = useState<RolDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<Partial<RolDto>>({});
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [touchedFields, setTouchedFields] = useState<{ [key: string]: boolean }>({});
  const [expandedRoles, setExpandedRoles] = useState<Set<number>>(new Set());

  const fetchRoles = useCallback(async (activeOnly = true) => {
    setLoading(true);
    setError(null);
    try {
      const data = await repository.getRoles();
      setRoles(activeOnly ? data.filter(r => r.isActive) : data.filter(r => !r.isActive));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading roles');
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const createRole = useCallback(async (data: Partial<RolDto>) => {
    setLoading(true);
    setError(null);
    try {
      const newRole = await repository.createRol({
        name: data.name || '',
        code: data.code || data.name?.toUpperCase().replace(/\s/g, '_') || 'NEW_ROLE',
        isActive: true
      });
      setRoles(prev => [...prev, newRole]);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error creating role');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const updateRole = useCallback(async (id: number, data: UpdateRolDto) => {
    setLoading(true);
    setError(null);
    try {
      await repository.updateRol({
        id,
        name: data.name,
        code: data.code
      });
      setRoles(prev => prev.map(r => r.id === id ? { ...r, ...data } : r));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error updating role');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const deleteRole = useCallback(async (id: number, logical = true) => {
    setLoading(true);
    setError(null);
    try {
      if (logical) {
        await repository.deleteLogicalRol(id);
      } else {
        await repository.deleteRol(id);
      }
      setRoles(prev => prev.filter(r => r.id !== id));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error deleting role');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const activateRole = useCallback(async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      await repository.activateRol(id);
      setRoles(prev => prev.filter(r => r.id !== id));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error activating role');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const toggleRoleAccordion = useCallback((roleId: number) => {
    setExpandedRoles(prev => {
      const newSet = new Set(prev);
      if (newSet.has(roleId)) {
        newSet.delete(roleId);
      } else {
        newSet.add(roleId);
      }
      return newSet;
    });
  }, []);

  const validateField = useCallback((fieldName: string, value: string | number | boolean | null | undefined): string => {
    const strValue = String(value || '');
    switch (fieldName) {
      case 'name':
        const nameValidation = ValidationUtils.validateName(strValue, 'Role name');
        if (!nameValidation.isValid) return nameValidation.message;
        break;
      case 'code':
        if (!strValue || strValue.trim() === '') return 'Code is required';
        if (!/^[A-Z_]+$/.test(strValue)) return 'Code must contain only uppercase letters and underscores';
        if (strValue.length > 50) return 'Code cannot exceed 50 characters';
        break;
    }
    return '';
  }, []);

  const validateForm = useCallback((data: Partial<RolDto>): FormErrors => {
    const errors: FormErrors = {};
    Object.keys(data).forEach((field: string) => {
      const error = validateField(field, data[field as keyof RolDto] as string | number | boolean | null | undefined);
      if (error) {
        errors[field] = error;
      }
    });
    return errors;
  }, [validateField]);

  const clearFormErrors = useCallback(() => {
    setFormErrors({});
    setTouchedFields({});
  }, []);

  const handleFieldChange = useCallback((fieldName: string, value: string | number | boolean | null | undefined) => {
    setTouchedFields(prev => ({
      ...prev,
      [fieldName]: true
    }));

    if (touchedFields[fieldName]) {
      const error = validateField(fieldName, value);
      setFormErrors(prev => ({
        ...prev,
        [fieldName]: error
      }));
    }
  }, [touchedFields, validateField]);

  const resetForm = useCallback(() => {
    setFormData({});
    clearFormErrors();
  }, [clearFormErrors]);

  return {
    roles,
    loading,
    error,
    formData,
    formErrors,
    touchedFields,
    expandedRoles,
    setFormData,
    fetchRoles,
    createRole,
    updateRole,
    deleteRole,
    activateRole,
    toggleRoleAccordion,
    validateField,
    validateForm,
    clearFormErrors,
    handleFieldChange,
    resetForm
  };
};
