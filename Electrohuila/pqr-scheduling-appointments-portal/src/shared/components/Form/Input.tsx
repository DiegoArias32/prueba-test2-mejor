/**
 * @file Input.tsx
 * @description Reusable input component with validation and styling
 * @module shared/components/Form
 */

'use client';

import React, { forwardRef } from 'react';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  /** Error state */
  error?: boolean;
  /** Icon to display on the left */
  leftIcon?: React.ReactNode;
  /** Icon to display on the right */
  rightIcon?: React.ReactNode;
  /** Full width input */
  fullWidth?: boolean;
}

/**
 * Input Component
 * Styled input field with optional icons and error state
 *
 * @component
 * @example
 * ```tsx
 * <Input
 *   type="email"
 *   placeholder="Enter your email"
 *   error={!!errors.email}
 *   leftIcon={<FiMail />}
 * />
 * ```
 */
export const Input = forwardRef<HTMLInputElement, InputProps>(
  (
    {
      error = false,
      leftIcon,
      rightIcon,
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
    `;

    const errorClasses = error
      ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
      : 'border-gray-300 focus:border-[#1797D5] focus:ring-2 focus:ring-[#1797D5]/20';

    const widthClasses = fullWidth ? 'w-full' : '';

    const paddingClasses = leftIcon
      ? 'pl-11'
      : rightIcon
      ? 'pr-11'
      : '';

    return (
      <div className={`relative ${fullWidth ? 'w-full' : ''}`}>
        {/* Left Icon */}
        {leftIcon && (
          <div className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">
            {leftIcon}
          </div>
        )}

        {/* Input */}
        <input
          ref={ref}
          className={`
            ${baseClasses}
            ${errorClasses}
            ${widthClasses}
            ${paddingClasses}
            ${className}
          `}
          disabled={disabled}
          {...props}
        />

        {/* Right Icon */}
        {rightIcon && (
          <div className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400">
            {rightIcon}
          </div>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input';
