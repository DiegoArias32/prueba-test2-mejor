/**
 * @file Spinner.tsx
 * @description Loading spinner component with customizable size and color
 * @module shared/components/Loading
 */

'use client';

import React from 'react';

interface SpinnerProps {
  /** Size of the spinner */
  size?: 'sm' | 'md' | 'lg' | 'xl';
  /** Color variant */
  color?: 'primary' | 'secondary' | 'white' | 'gray';
  /** Additional CSS classes */
  className?: string;
  /** Label for screen readers */
  label?: string;
}

/**
 * Spinner Component
 * Displays an animated loading spinner
 *
 * @component
 * @example
 * ```tsx
 * <Spinner size="md" color="primary" />
 * ```
 */
export const Spinner: React.FC<SpinnerProps> = ({
  size = 'md',
  color = 'primary',
  className = '',
  label = 'Loading...'
}) => {
  const sizeClasses = {
    sm: 'w-4 h-4 border-2',
    md: 'w-8 h-8 border-4',
    lg: 'w-12 h-12 border-4',
    xl: 'w-16 h-16 border-4'
  };

  const colorClasses = {
    primary: 'border-[#1797D5] border-t-transparent',
    secondary: 'border-[#56C2E1] border-t-transparent',
    white: 'border-white border-t-transparent',
    gray: 'border-gray-400 border-t-transparent'
  };

  return (
    <div
      className={`animate-spin rounded-full ${sizeClasses[size]} ${colorClasses[color]} ${className}`}
      role="status"
      aria-label={label}
    >
      <span className="sr-only">{label}</span>
    </div>
  );
};
