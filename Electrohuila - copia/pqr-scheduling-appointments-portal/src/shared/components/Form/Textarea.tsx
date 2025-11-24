/**
 * @file Textarea.tsx
 * @description Reusable textarea component with character count
 * @module shared/components/Form
 */

'use client';

import React, { forwardRef } from 'react';

interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  /** Error state */
  error?: boolean;
  /** Show character count */
  showCount?: boolean;
  /** Full width textarea */
  fullWidth?: boolean;
}

/**
 * Textarea Component
 * Styled textarea with optional character counter
 *
 * @component
 * @example
 * ```tsx
 * <Textarea
 *   placeholder="Enter your message"
 *   maxLength={500}
 *   showCount
 *   error={!!errors.message}
 * />
 * ```
 */
export const Textarea = forwardRef<HTMLTextAreaElement, TextareaProps>(
  (
    {
      error = false,
      showCount = false,
      fullWidth = true,
      className = '',
      disabled,
      maxLength,
      value,
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
      resize-none
    `;

    const errorClasses = error
      ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
      : 'border-gray-300 focus:border-[#1797D5] focus:ring-2 focus:ring-[#1797D5]/20';

    const widthClasses = fullWidth ? 'w-full' : '';

    const currentLength = typeof value === 'string' ? value.length : 0;

    return (
      <div className={fullWidth ? 'w-full' : ''}>
        <textarea
          ref={ref}
          className={`
            ${baseClasses}
            ${errorClasses}
            ${widthClasses}
            ${className}
          `}
          disabled={disabled}
          maxLength={maxLength}
          value={value}
          {...props}
        />

        {/* Character Count */}
        {showCount && maxLength && (
          <div className="flex justify-end mt-1">
            <span
              className={`text-xs ${
                currentLength >= maxLength * 0.9
                  ? 'text-orange-600'
                  : 'text-gray-500'
              }`}
            >
              {currentLength}/{maxLength}
            </span>
          </div>
        )}
      </div>
    );
  }
);

Textarea.displayName = 'Textarea';
