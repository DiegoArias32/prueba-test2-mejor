/**
 * @file FormField.tsx
 * @description Wrapper component for form fields with label, error, and helper text
 * @module shared/components/Form
 */

'use client';

import React from 'react';

interface FormFieldProps {
  /** Label text */
  label: string;
  /** Field ID for accessibility */
  htmlFor: string;
  /** Whether field is required */
  required?: boolean;
  /** Error message */
  error?: string;
  /** Helper text */
  helperText?: string;
  /** Additional CSS classes */
  className?: string;
  /** Child input element */
  children: React.ReactNode;
}

/**
 * FormField Component
 * Provides consistent styling and structure for form fields
 *
 * @component
 * @example
 * ```tsx
 * <FormField label="Email" htmlFor="email" required error={errors.email}>
 *   <Input id="email" type="email" />
 * </FormField>
 * ```
 */
export const FormField: React.FC<FormFieldProps> = ({
  label,
  htmlFor,
  required = false,
  error,
  helperText,
  className = '',
  children
}) => {
  return (
    <div className={`space-y-2 ${className}`}>
      {/* Label */}
      <label
        htmlFor={htmlFor}
        className="block text-sm font-semibold text-gray-700"
      >
        {label}
        {required && <span className="text-red-500 ml-1">*</span>}
      </label>

      {/* Input/Select/Textarea */}
      {children}

      {/* Error Message */}
      {error && (
        <div className="flex items-center text-red-600 text-sm" role="alert">
          <svg
            className="w-4 h-4 mr-1 flex-shrink-0"
            fill="currentColor"
            viewBox="0 0 20 20"
            aria-hidden="true"
          >
            <path
              fillRule="evenodd"
              d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
              clipRule="evenodd"
            />
          </svg>
          <span>{error}</span>
        </div>
      )}

      {/* Helper Text */}
      {helperText && !error && (
        <p className="text-sm text-gray-500">{helperText}</p>
      )}
    </div>
  );
};
