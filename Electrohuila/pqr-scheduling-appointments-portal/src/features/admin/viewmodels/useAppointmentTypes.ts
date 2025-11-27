/**
 * Hook - Appointment Types Management
 * Handles all appointment type CRUD operations
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { AppointmentTypeDto, UpdateAppointmentTypeDto } from '../models/admin.models';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface UseAppointmentTypesReturn {
  appointmentTypes: AppointmentTypeDto[];
  loading: boolean;
  error: string | null;
  formData: Partial<AppointmentTypeDto>;
  formErrors: FormErrors;
  touchedFields: { [key: string]: boolean };
  setFormData: React.Dispatch<React.SetStateAction<Partial<AppointmentTypeDto>>>;
  fetchAppointmentTypes: (activeOnly?: boolean) => Promise<void>;
  createAppointmentType: (data: Partial<AppointmentTypeDto>) => Promise<void>;
  updateAppointmentType: (id: number, data: UpdateAppointmentTypeDto) => Promise<void>;
  deleteAppointmentType: (id: number, logical?: boolean) => Promise<void>;
  activateAppointmentType: (id: number) => Promise<void>;
  validateField: (fieldName: string, value: string | number | boolean | null | undefined) => string;
  validateForm: (data: Partial<AppointmentTypeDto>) => FormErrors;
  clearFormErrors: () => void;
  handleFieldChange: (fieldName: string, value: string | number | boolean | null | undefined) => void;
  resetForm: () => void;
}

export const useAppointmentTypes = (repository: AdminRepository): UseAppointmentTypesReturn => {
  const [appointmentTypes, setAppointmentTypes] = useState<AppointmentTypeDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<Partial<AppointmentTypeDto>>({});
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [touchedFields, setTouchedFields] = useState<{ [key: string]: boolean }>({});

  const fetchAppointmentTypes = useCallback(async (activeOnly = true) => {
    setLoading(true);
    setError(null);
    try {
      // Always fetch all appointment types (including inactive) so we can display both tabs
      const data = await repository.getAllAppointmentTypesIncludingInactive();
      // Store all types - TypesView will filter them based on currentView
      setAppointmentTypes(data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading appointment types');
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const createAppointmentType = useCallback(async (data: Partial<AppointmentTypeDto>) => {
    setLoading(true);
    setError(null);
    try {
      const newType = await repository.createAppointmentType({
        name: data.name || '',
        code: data.code || data.name?.toUpperCase().replace(/\s/g, '_') || 'NEW_TYPE',
        description: data.description || '',
        estimatedTimeMinutes: data.estimatedTimeMinutes || 30,
        requiresDocumentation: data.requiresDocumentation || false,
        displayOrder: data.displayOrder || 0,
        isActive: true,
        isEnabled: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      });
      setAppointmentTypes(prev => [...prev, newType]);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error creating appointment type');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const updateAppointmentType = useCallback(async (id: number, data: UpdateAppointmentTypeDto) => {
    setLoading(true);
    setError(null);
    try {
      await repository.updateAppointmentType({
        ...data,
        id
      });
      setAppointmentTypes(prev => prev.map(t => t.id === id ? { ...t, ...data } : t));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error updating appointment type');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const deleteAppointmentType = useCallback(async (id: number, logical = true) => {
    setLoading(true);
    setError(null);
    try {
      if (logical) {
        await repository.deleteLogicalAppointmentType(id);
        // Update local state to mark as inactive instead of removing
        setAppointmentTypes(prev => prev.map(t =>
          t.id === id ? { ...t, isActive: false } : t
        ));
      } else {
        await repository.deleteAppointmentType(id);
        // Physical delete - remove from list
        setAppointmentTypes(prev => prev.filter(t => t.id !== id));
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error deleting appointment type');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const activateAppointmentType = useCallback(async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      await repository.activateAppointmentType(id);
      // Update local state to mark as active instead of removing
      setAppointmentTypes(prev => prev.map(t =>
        t.id === id ? { ...t, isActive: true } : t
      ));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error activating appointment type');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const validateField = useCallback((fieldName: string, value: string | number | boolean | null | undefined): string => {
    switch (fieldName) {
      case 'name':
        const nameValidation = ValidationUtils.validateName(String(value || ''), 'Appointment type name');
        if (!nameValidation.isValid) return nameValidation.message;
        break;
      case 'description':
        if (value && typeof value === 'string' && value.length > 500) return 'Description cannot exceed 500 characters';
        break;
      case 'estimatedTimeMinutes':
        const numValue = Number(value);
        if (!numValue || numValue <= 0) return 'Estimated time must be greater than 0';
        if (numValue > 480) return 'Estimated time cannot exceed 8 hours (480 minutes)';
        break;
    }
    return '';
  }, []);

  const validateForm = useCallback((data: Partial<AppointmentTypeDto>): FormErrors => {
    const errors: FormErrors = {};
    Object.keys(data).forEach((field: string) => {
      const fieldValue = data[field as keyof AppointmentTypeDto];
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
    appointmentTypes,
    loading,
    error,
    formData,
    formErrors,
    touchedFields,
    setFormData,
    fetchAppointmentTypes,
    createAppointmentType,
    updateAppointmentType,
    deleteAppointmentType,
    activateAppointmentType,
    validateField,
    validateForm,
    clearFormErrors,
    handleFieldChange,
    resetForm
  };
};
