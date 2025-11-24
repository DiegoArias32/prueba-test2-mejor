/**
 * Hook personalizado para manejar validaciones en tiempo real
 */

'use client';

import { useState } from 'react';
import { ValidationUtils, ValidationResult, FormErrors } from '../utils/validation.utils';

export const useFormValidation = <T extends Record<string, unknown>>(
  initialData: T,
  validationRules: { [K in keyof T]?: (value: T[K]) => ValidationResult }
) => {
  const [data, setData] = useState<T>(initialData);
  const [errors, setErrors] = useState<FormErrors>({});
  const [touched, setTouched] = useState<{ [key: string]: boolean }>({});

  const validateField = (field: keyof T, value: unknown) => {
    if (validationRules[field]) {
      const validation = validationRules[field]!(value);
      setErrors((prev) => ({
        ...prev,
        [field]: validation.isValid ? '' : validation.message,
      }));
    }
  };

  const handleChange = (field: keyof T, value: unknown) => {
    setData((prev) => ({
      ...prev,
      [field]: value,
    }));

    if (touched[field as string]) {
      validateField(field, value);
    }
  };

  const handleBlur = (field: keyof T) => {
    setTouched((prev) => ({
      ...prev,
      [field as string]: true,
    }));
    validateField(field, data[field]);
  };

  const validateAll = () => {
    const newErrors = ValidationUtils.validateForm(data, validationRules as { [key: string]: (value: unknown) => ValidationResult });
    setErrors(newErrors);

    // Marcar todos los campos como touched
    const allTouched = Object.keys(data).reduce((acc, key) => {
      acc[key] = true;
      return acc;
    }, {} as { [key: string]: boolean });
    setTouched(allTouched);

    return !ValidationUtils.hasErrors(newErrors);
  };

  const resetForm = () => {
    setData(initialData);
    setErrors({});
    setTouched({});
  };

  const setFieldError = (field: keyof T, message: string) => {
    setErrors((prev) => ({
      ...prev,
      [field]: message,
    }));
  };

  const clearFieldError = (field: keyof T) => {
    setErrors((prev) => {
      const newErrors = { ...prev };
      delete newErrors[field as string];
      return newErrors;
    });
  };

  return {
    data,
    setData,
    errors,
    touched,
    handleChange,
    handleBlur,
    validateAll,
    validateField,
    resetForm,
    setFieldError,
    clearFieldError,
    hasErrors: ValidationUtils.hasErrors(errors),
  };
};
