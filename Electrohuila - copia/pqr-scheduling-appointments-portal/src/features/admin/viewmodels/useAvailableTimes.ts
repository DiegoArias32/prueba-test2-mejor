/**
 * Hook - Available Times Management
 * Handles all available time CRUD operations
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { AvailableTimeDto, UpdateAvailableTimeDto } from '../models/admin.models';
import { FormErrors } from '@/shared/utils/validation.utils';

interface UseAvailableTimesReturn {
  availableTimes: AvailableTimeDto[];
  loading: boolean;
  error: string | null;
  formData: Partial<AvailableTimeDto>;
  formErrors: FormErrors;
  touchedFields: { [key: string]: boolean };
  setFormData: React.Dispatch<React.SetStateAction<Partial<AvailableTimeDto>>>;
  fetchAvailableTimes: (activeOnly?: boolean) => Promise<void>;
  createAvailableTime: (data: Partial<AvailableTimeDto>) => Promise<void>;
  updateAvailableTime: (id: number, data: UpdateAvailableTimeDto) => Promise<void>;
  deleteAvailableTime: (id: number, logical?: boolean) => Promise<void>;
  activateAvailableTime: (id: number) => Promise<void>;
  validateField: (fieldName: string, value: string | number | boolean | null | undefined) => string;
  validateForm: (data: Partial<AvailableTimeDto>) => FormErrors;
  clearFormErrors: () => void;
  handleFieldChange: (fieldName: string, value: string | number | boolean | null | undefined) => void;
  resetForm: () => void;
}

export const useAvailableTimes = (repository: AdminRepository): UseAvailableTimesReturn => {
  const [availableTimes, setAvailableTimes] = useState<AvailableTimeDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<Partial<AvailableTimeDto>>({});
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [touchedFields, setTouchedFields] = useState<{ [key: string]: boolean }>({});

  const fetchAvailableTimes = useCallback(async (activeOnly = true) => {
    setLoading(true);
    setError(null);
    try {
      const data = activeOnly
        ? await repository.getAllAvailableTimes()
        : await repository.getAllAvailableTimesIncludingInactive();
      setAvailableTimes(activeOnly ? data.filter(t => t.isActive) : data.filter(t => !t.isActive));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading available times');
      setAvailableTimes([]);
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const createAvailableTime = useCallback(async (data: Partial<AvailableTimeDto>) => {
    setLoading(true);
    setError(null);
    try {
      const newTimeData = {
        time: data.time ? String(data.time) : '',
        branchId: typeof data.branchId === 'string' ? parseInt(data.branchId) : Number(data.branchId),
        appointmentTypeId: data.appointmentTypeId !== undefined && data.appointmentTypeId !== null
          ? (typeof data.appointmentTypeId === 'string' ? parseInt(data.appointmentTypeId) : Number(data.appointmentTypeId))
          : undefined,
        isActive: true,
        isEnabled: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };

      const newTime = await repository.createAvailableTime(newTimeData);
      setAvailableTimes(prev => [...prev, newTime]);
    } catch (err: unknown) {
      let errorMessage = 'Error creating available time';
      if (err && typeof err === 'object') {
        if ('response' in err && err.response && typeof err.response === 'object' && 'data' in err.response) {
          const serverData = err.response.data as Record<string, unknown>;
          const serverError = (typeof serverData?.message === 'string' ? serverData.message : null) ||
                             (typeof serverData?.error === 'string' ? serverData.error : null) ||
                             (err && typeof err === 'object' && 'message' in err && typeof err.message === 'string' ? err.message : null);
          if (serverError) {
            errorMessage = `Error creating available time: ${serverError}`;
          }
        } else if ('message' in err && typeof err.message === 'string') {
          errorMessage = `Error creating available time: ${err.message}`;
        }
      }
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const updateAvailableTime = useCallback(async (id: number, data: UpdateAvailableTimeDto) => {
    setLoading(true);
    setError(null);
    try {
      const updateData: UpdateAvailableTimeDto = {
        id,
        time: data.time,
        branchId: data.branchId ? (typeof data.branchId === 'string' ? parseInt(data.branchId) : Number(data.branchId)) : undefined,
        appointmentTypeId: data.appointmentTypeId ? (typeof data.appointmentTypeId === 'string' ? parseInt(data.appointmentTypeId) : Number(data.appointmentTypeId)) : undefined
      };

      await repository.updateAvailableTime(updateData);
      setAvailableTimes(prev => prev.map(t => t.id === id ? { ...t, ...data } : t));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error updating available time');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const deleteAvailableTime = useCallback(async (id: number, logical = true) => {
    setLoading(true);
    setError(null);
    try {
      if (logical) {
        await repository.deleteLogicalAvailableTime(id);
      } else {
        await repository.deleteAvailableTime(id);
      }
      setAvailableTimes(prev => prev.filter(t => t.id !== id));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error deleting available time');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const activateAvailableTime = useCallback(async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      await repository.activateAvailableTime(id);
      setAvailableTimes(prev => prev.filter(t => t.id !== id));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error activating available time');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const validateField = useCallback((fieldName: string, value: string | number | boolean | null | undefined): string => {
    switch (fieldName) {
      case 'time':
        const strValue = String(value || '');
        if (!strValue || strValue.trim() === '') return 'Time is required';
        if (!/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/.test(strValue)) return 'Invalid time format (HH:MM)';
        break;
    }
    return '';
  }, []);

  const validateForm = useCallback((data: Partial<AvailableTimeDto>): FormErrors => {
    const errors: FormErrors = {};
    Object.keys(data).forEach((field: string) => {
      const fieldValue = data[field as keyof AvailableTimeDto];
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
    availableTimes,
    loading,
    error,
    formData,
    formErrors,
    touchedFields,
    setFormData,
    fetchAvailableTimes,
    createAvailableTime,
    updateAvailableTime,
    deleteAvailableTime,
    activateAvailableTime,
    validateField,
    validateForm,
    clearFormErrors,
    handleFieldChange,
    resetForm
  };
};
