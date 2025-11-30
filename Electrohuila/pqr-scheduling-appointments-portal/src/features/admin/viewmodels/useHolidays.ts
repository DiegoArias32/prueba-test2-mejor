/**
 * Hook - Holidays Management
 * Handles all holiday CRUD operations
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { HolidayDto } from '../models/admin.models';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface UseHolidaysReturn {
  holidays: HolidayDto[];
  loading: boolean;
  error: string | null;
  formData: Partial<HolidayDto>;
  formErrors: FormErrors;
  touchedFields: { [key: string]: boolean };
  setFormData: React.Dispatch<React.SetStateAction<Partial<HolidayDto>>>;
  fetchHolidays: () => Promise<void>;
  createHoliday: (data: Partial<HolidayDto>) => Promise<void>;
  updateHoliday: (id: number, data: Partial<HolidayDto>) => Promise<void>;
  deleteHoliday: (id: number) => Promise<void>;
  activateHoliday: (id: number) => Promise<void>;
  validateField: (fieldName: string, value: string | number | boolean | null | undefined) => string;
  validateForm: (data: Partial<HolidayDto>) => FormErrors;
  clearFormErrors: () => void;
  handleFieldChange: (fieldName: string, value: string | number | boolean | null | undefined) => void;
  resetForm: () => void;
}

export const useHolidays = (repository: AdminRepository): UseHolidaysReturn => {
  const [holidays, setHolidays] = useState<HolidayDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<Partial<HolidayDto>>({});
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [touchedFields, setTouchedFields] = useState<{ [key: string]: boolean }>({});

  const fetchHolidays = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await repository.getHolidays();
      // La validación defensiva se mantiene pero ahora debería siempre pasar
      setHolidays(Array.isArray(data) ? data : []);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading holidays');
      setHolidays([]); // Asegurar array vacío en caso de error
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const createHoliday = useCallback(async (data: Partial<HolidayDto>) => {
    setLoading(true);
    setError(null);
    try {
      let newHoliday: HolidayDto;

      if (data.holidayType === 'National') {
        newHoliday = await repository.createNationalHoliday({
          holidayDate: data.holidayDate!,
          holidayName: data.holidayName!
        });
      } else if (data.holidayType === 'Local') {
        newHoliday = await repository.createLocalHoliday({
          holidayDate: data.holidayDate!,
          holidayName: data.holidayName!,
          branchId: data.branchId!
        });
      } else {
        // Company
        newHoliday = await repository.createCompanyHoliday({
          holidayDate: data.holidayDate!,
          holidayName: data.holidayName!
        });
      }

      setHolidays(prev => [...prev, newHoliday]);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error creating holiday');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const updateHoliday = useCallback(async (id: number, data: Partial<HolidayDto>) => {
    setLoading(true);
    setError(null);
    try {
      const updatedHoliday = await repository.updateHoliday({
        id,
        holidayDate: data.holidayDate!,
        holidayName: data.holidayName!,
        holidayType: data.holidayType!,
        branchId: data.branchId
      });
      setHolidays(prev => prev.map(h => h.id === id ? updatedHoliday : h));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error updating holiday');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const deleteHoliday = useCallback(async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      await repository.deleteHoliday(id);
      setHolidays(prev => prev.filter(h => h.id !== id));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error deleting holiday');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const activateHoliday = useCallback(async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      await repository.activateHoliday(id);
      setHolidays(prev => prev.filter(h => h.id !== id));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error activating holiday');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const validateField = useCallback((fieldName: string, value: string | number | boolean | null | undefined): string => {
    const strValue = String(value || '');
    switch (fieldName) {
      case 'holidayName':
        const nameValidation = ValidationUtils.validateRequired(strValue, 'Holiday name');
        if (!nameValidation.isValid) return nameValidation.message;
        if (strValue.trim().length < 3) return 'Holiday name must be at least 3 characters';
        if (strValue.trim().length > 100) return 'Holiday name cannot exceed 100 characters';
        break;
      case 'holidayDate':
        if (!strValue || strValue.trim() === '') return 'Holiday date is required';
        // Validate date format (YYYY-MM-DD)
        const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
        if (!dateRegex.test(strValue)) return 'Invalid date format (use YYYY-MM-DD)';
        // Validate it's a valid date
        const date = new Date(strValue);
        if (isNaN(date.getTime())) return 'Invalid date';
        break;
      case 'holidayType':
        if (!strValue || !['National', 'Local', 'Company'].includes(strValue)) {
          return 'Holiday type is required';
        }
        break;
      case 'branchId':
        // Only validate if holidayType is Local
        if (formData.holidayType === 'Local' && (!value || value === 0)) {
          return 'Branch is required for local holidays';
        }
        break;
    }
    return '';
  }, [formData.holidayType]);

  const validateForm = useCallback((data: Partial<HolidayDto>): FormErrors => {
    const errors: FormErrors = {};
    Object.keys(data).forEach((field: string) => {
      const fieldValue = data[field as keyof HolidayDto];
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
    holidays,
    loading,
    error,
    formData,
    formErrors,
    touchedFields,
    setFormData,
    fetchHolidays,
    createHoliday,
    updateHoliday,
    deleteHoliday,
    activateHoliday,
    validateField,
    validateForm,
    clearFormErrors,
    handleFieldChange,
    resetForm
  };
};
