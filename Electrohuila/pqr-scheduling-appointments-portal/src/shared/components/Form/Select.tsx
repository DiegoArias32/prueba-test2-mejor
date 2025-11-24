/**
 * @file Select.tsx
 * @description Reusable select dropdown component
 * @module shared/components/Form
 */

'use client';

import React, { forwardRef } from 'react';

interface SelectOption {
  value: string | number;
  label: string;
  disabled?: boolean;
}

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  /** Options for the select */
  options: SelectOption[];
  /** Error state */
  error?: boolean;
  /** Placeholder text */
  placeholder?: string;
  /** Full width select */
  fullWidth?: boolean;
}

/**
 * Select Component
 * Styled dropdown select with options
 *
 * @component
 * @example
 * ```tsx
 * <Select
 *   options={[
 *     { value: '1', label: 'Option 1' },
 *     { value: '2', label: 'Option 2' }
 *   ]}
 *   placeholder="Select an option"
 *   error={!!errors.field}
 * />
 * ```
 */
export const Select = forwardRef<HTMLSelectElement, SelectProps>(
  (
    {
      options,
      error = false,
      placeholder,
      fullWidth = true,
      className = '',
      disabled,
      ...props
    },
    ref
  ) => {
    const baseClasses = `
      px-4 py-3
      border-2 rounded-lg
      focus:outline-none
      transition-all duration-300
      disabled:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-60
      appearance-none
      bg-white
    `;

    const errorClasses = error
      ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
      : 'border-gray-300 focus:border-[#1797D5] focus:ring-2 focus:ring-[#1797D5]/20';

    const widthClasses = fullWidth ? 'w-full' : '';

    return (
      <div className={`relative ${fullWidth ? 'w-full' : ''}`}>
        <select
          ref={ref}
          className={`
            ${baseClasses}
            ${errorClasses}
            ${widthClasses}
            ${className}
          `}
          disabled={disabled}
          {...props}
        >
          {placeholder && (
            <option value="" disabled>
              {placeholder}
            </option>
          )}
          {options.map((option) => (
            <option
              key={option.value}
              value={option.value}
              disabled={option.disabled}
            >
              {option.label}
            </option>
          ))}
        </select>

        {/* Dropdown Arrow Icon */}
        <div className="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none text-gray-400">
          <svg
            className="w-5 h-5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              d="M19 9l-7 7-7-7"
            />
          </svg>
        </div>
      </div>
    );
  }
);

Select.displayName = 'Select';
