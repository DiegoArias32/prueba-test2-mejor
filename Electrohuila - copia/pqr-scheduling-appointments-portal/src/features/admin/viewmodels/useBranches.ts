/**
 * Hook - Branches Management
 * Handles all branch CRUD operations
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { BranchDto, UpdateBranchDto } from '../models/admin.models';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface UseBranchesReturn {
  branches: BranchDto[];
  loading: boolean;
  error: string | null;
  formData: Partial<BranchDto>;
  formErrors: FormErrors;
  touchedFields: { [key: string]: boolean };
  setFormData: React.Dispatch<React.SetStateAction<Partial<BranchDto>>>;
  fetchBranches: (activeOnly?: boolean) => Promise<void>;
  createBranch: (data: Partial<BranchDto>) => Promise<void>;
  updateBranch: (id: number, data: UpdateBranchDto) => Promise<void>;
  deleteBranch: (id: number, logical?: boolean) => Promise<void>;
  activateBranch: (id: number) => Promise<void>;
  validateField: (fieldName: string, value: string | number | boolean | null | undefined) => string;
  validateForm: (data: Partial<BranchDto>) => FormErrors;
  clearFormErrors: () => void;
  handleFieldChange: (fieldName: string, value: string | number | boolean | null | undefined) => void;
  resetForm: () => void;
}

export const useBranches = (repository: AdminRepository): UseBranchesReturn => {
  const [branches, setBranches] = useState<BranchDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<Partial<BranchDto>>({});
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [touchedFields, setTouchedFields] = useState<{ [key: string]: boolean }>({});

  const fetchBranches = useCallback(async (activeOnly = true) => {
    setLoading(true);
    setError(null);
    try {
      const data = activeOnly
        ? await repository.getBranches()
        : await repository.getAllBranchesIncludingInactive();
      setBranches(activeOnly ? data.filter(b => b.isActive) : data.filter(b => !b.isActive));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading branches');
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const createBranch = useCallback(async (data: Partial<BranchDto>) => {
    setLoading(true);
    setError(null);
    try {
      const newBranch = await repository.createBranch({
        name: data.name || '',
        code: data.code || '',
        address: data.address || '',
        city: data.city || '',
        state: data.state || '',
        phone: data.phone || '',
        isMain: false,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        isActive: true,
        isEnabled: true
      });
      setBranches(prev => [...prev, newBranch]);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error creating branch');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const updateBranch = useCallback(async (id: number, data: UpdateBranchDto) => {
    setLoading(true);
    setError(null);
    try {
      await repository.updateBranch({
        ...data,
        id
      });
      setBranches(prev => prev.map(b => b.id === id ? { ...b, ...data } : b));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error updating branch');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const deleteBranch = useCallback(async (id: number, logical = true) => {
    setLoading(true);
    setError(null);
    try {
      if (logical) {
        await repository.deleteLogicalBranch(id);
      } else {
        await repository.deleteBranch(id);
      }
      setBranches(prev => prev.filter(b => b.id !== id));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error deleting branch');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const activateBranch = useCallback(async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      await repository.activateBranch(id);
      setBranches(prev => prev.filter(b => b.id !== id));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error activating branch');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const validateField = useCallback((fieldName: string, value: string | number | boolean | null | undefined): string => {
    const strValue = String(value || '');
    switch (fieldName) {
      case 'name':
        const nameValidation = ValidationUtils.validateName(strValue, 'Branch name');
        if (!nameValidation.isValid) return nameValidation.message;
        break;
      case 'code':
        if (!strValue || strValue.trim() === '') return 'Code is required';
        if (strValue.length > 20) return 'Code cannot exceed 20 characters';
        break;
      case 'address':
        const addressValidation = ValidationUtils.validateAddress(strValue);
        if (!addressValidation.isValid) return addressValidation.message;
        break;
      case 'city':
        const cityValidation = ValidationUtils.validateName(strValue, 'City');
        if (!cityValidation.isValid) return cityValidation.message;
        break;
      case 'state':
        const stateValidation = ValidationUtils.validateName(strValue, 'State/Department');
        if (!stateValidation.isValid) return stateValidation.message;
        break;
      case 'phone':
        const phoneValidation = ValidationUtils.validatePhone(strValue, false);
        if (!phoneValidation.isValid) return phoneValidation.message;
        break;
    }
    return '';
  }, []);

  const validateForm = useCallback((data: Partial<BranchDto>): FormErrors => {
    const errors: FormErrors = {};
    Object.keys(data).forEach((field: string) => {
      const fieldValue = data[field as keyof BranchDto];
      const error = validateField(field, fieldValue as string | number | boolean | null | undefined);
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
    branches,
    loading,
    error,
    formData,
    formErrors,
    touchedFields,
    setFormData,
    fetchBranches,
    createBranch,
    updateBranch,
    deleteBranch,
    activateBranch,
    validateField,
    validateForm,
    clearFormErrors,
    handleFieldChange,
    resetForm
  };
};
