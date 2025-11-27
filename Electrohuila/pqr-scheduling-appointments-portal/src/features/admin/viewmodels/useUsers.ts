/**
 * Hook - Users Management
 * Handles all user/employee CRUD operations
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { UserDto, UpdateUserDto, CreateUserDto } from '../models/admin.models';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface UseUsersReturn {
  users: UserDto[];
  loading: boolean;
  error: string | null;
  formData: Partial<UserDto> & { roles?: string[]; password?: string };
  formErrors: FormErrors;
  touchedFields: { [key: string]: boolean };
  setFormData: React.Dispatch<React.SetStateAction<Partial<UserDto> & { roles?: string[]; password?: string }>>;
  fetchUsers: (activeOnly?: boolean) => Promise<void>;
  createUser: (data: CreateUserDto) => Promise<void>;
  updateUser: (id: number, data: UpdateUserDto) => Promise<void>;
  deleteUser: (id: number, logical?: boolean) => Promise<void>;
  activateUser: (id: number) => Promise<void>;
  validateField: (fieldName: string, value: string | number | boolean | null | undefined) => string;
  validateForm: (data: Partial<UserDto>) => FormErrors;
  clearFormErrors: () => void;
  handleFieldChange: (fieldName: string, value: string | number | boolean | null | undefined) => void;
  resetForm: () => void;
}

export const useUsers = (repository: AdminRepository): UseUsersReturn => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<Partial<UserDto> & { roles?: string[]; password?: string }>({});
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [touchedFields, setTouchedFields] = useState<{ [key: string]: boolean }>({});

  const fetchUsers = useCallback(async (activeOnly = true) => {
    setLoading(true);
    setError(null);
    try {
      // Always fetch all users (including inactive) to have complete data
      const data = await repository.getUsers();
      // Store all users - UsersView will filter them based on currentView
      setUsers(data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading users');
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const createUser = useCallback(async (data: CreateUserDto) => {
    setLoading(true);
    setError(null);
    try {
      const roleIds = (data.roleIds || []).filter((id: number) => id !== undefined);

      if (roleIds.length === 0) {
        throw new Error('At least one role must be selected');
      }

      if (!data.username || !data.email || !data.password) {
        throw new Error('All fields are required');
      }

      const newUser = await repository.createUser({
        username: data.username,
        email: data.email,
        password: data.password,
        roleIds: roleIds
      });

      setUsers(prev => [...prev, newUser]);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error creating user');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const updateUser = useCallback(async (id: number, data: UpdateUserDto) => {
    setLoading(true);
    setError(null);
    try {
      const roleIds = (data.roleIds || []).filter((id: number) => id !== undefined);

      if (roleIds.length === 0) {
        throw new Error('At least one role must be selected');
      }

      if (!data.username || !data.email) {
        throw new Error('Username and email are required');
      }

      await repository.updateUser({
        id,
        username: data.username,
        email: data.email,
        roleIds: roleIds
      });

      setUsers(prev => prev.map(u => u.id === id ? { ...u, ...data } : u));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error updating user');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const deleteUser = useCallback(async (id: number, logical = true) => {
    setLoading(true);
    setError(null);
    try {
      if (logical) {
        await repository.deleteLogicalUser(id);
        // Update local state to mark as inactive instead of removing
        setUsers(prev => prev.map(u =>
          u.id === id ? { ...u, isActive: false } : u
        ));
      } else {
        await repository.deleteUser(id);
        // Physical delete - remove from list
        setUsers(prev => prev.filter(u => u.id !== id));
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error deleting user');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const activateUser = useCallback(async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      await repository.activateUser(id);
      // Update local state to mark as active instead of removing
      setUsers(prev => prev.map(u =>
        u.id === id ? { ...u, isActive: true } : u
      ));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error activating user');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const validateField = useCallback((fieldName: string, value: string | number | boolean | null | undefined): string => {
    const strValue = String(value || '');
    switch (fieldName) {
      case 'username':
        const usernameValidation = ValidationUtils.validateRequired(strValue, 'Username');
        if (!usernameValidation.isValid) return usernameValidation.message;
        if (strValue.length < 3) return 'Username must be at least 3 characters';
        if (strValue.length > 50) return 'Username cannot exceed 50 characters';
        break;
      case 'email':
        const emailValidation = ValidationUtils.validateEmail(strValue);
        if (!emailValidation.isValid) return emailValidation.message;
        break;
      case 'password':
        if (!strValue || strValue.trim() === '') return 'Password is required';
        if (strValue.length < 6) return 'Password must be at least 6 characters';
        if (strValue.length > 100) return 'Password cannot exceed 100 characters';
        break;
    }
    return '';
  }, []);

  const validateForm = useCallback((data: Partial<UserDto>): FormErrors => {
    const errors: FormErrors = {};
    Object.keys(data).forEach((field: string) => {
      const error = validateField(field, data[field as keyof UserDto] as string | number | boolean | null | undefined);
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
    users,
    loading,
    error,
    formData,
    formErrors,
    touchedFields,
    setFormData,
    fetchUsers,
    createUser,
    updateUser,
    deleteUser,
    activateUser,
    validateField,
    validateForm,
    clearFormErrors,
    handleFieldChange,
    resetForm
  };
};
