/**
 * @file FormActions.tsx
 * @description Reusable form action buttons (submit, cancel, etc.)
 * @module shared/components/Form
 */

'use client';

import React from 'react';
import { Spinner } from '../Loading';

interface FormActionsProps {
  /** Submit button text */
  submitText?: string;
  /** Cancel button text */
  cancelText?: string;
  /** Loading state */
  loading?: boolean;
  /** Disable submit button */
  disabled?: boolean;
  /** Cancel button handler */
  onCancel?: () => void;
  /** Show cancel button */
  showCancel?: boolean;
  /** Layout direction */
  layout?: 'horizontal' | 'vertical';
  /** Additional CSS classes */
  className?: string;
}

/**
 * FormActions Component
 * Provides consistent styling for form action buttons
 *
 * @component
 * @example
 * ```tsx
 * <FormActions
 *   submitText="Save"
 *   cancelText="Cancel"
 *   loading={isSubmitting}
 *   onCancel={() => router.back()}
 *   showCancel
 * />
 * ```
 */
export const FormActions: React.FC<FormActionsProps> = ({
  submitText = 'Submit',
  cancelText = 'Cancel',
  loading = false,
  disabled = false,
  onCancel,
  showCancel = false,
  layout = 'horizontal',
  className = ''
}) => {
  const containerClasses =
    layout === 'horizontal'
      ? 'flex justify-end space-x-4'
      : 'flex flex-col space-y-3';

  return (
    <div className={`${containerClasses} ${className}`}>
      {/* Cancel Button */}
      {showCancel && onCancel && (
        <button
          type="button"
          onClick={onCancel}
          disabled={loading}
          className="px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg font-semibold hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {cancelText}
        </button>
      )}

      {/* Submit Button */}
      <button
        type="submit"
        disabled={disabled || loading}
        className="flex items-center justify-center px-6 py-3 bg-gradient-to-r from-[#203461] to-[#1797D5] text-white rounded-lg font-semibold hover:from-[#1A6192] hover:to-[#56C2E1] transition-all duration-300 shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:shadow-lg"
      >
        {loading ? (
          <>
            <Spinner size="sm" color="white" className="mr-2" />
            <span>Processing...</span>
          </>
        ) : (
          submitText
        )}
      </button>
    </div>
  );
};

/**
 * FormActionsSecondary Component
 * Alternative styling for secondary actions
 */
export const FormActionsSecondary: React.FC<FormActionsProps> = ({
  submitText = 'Continue',
  cancelText = 'Back',
  loading = false,
  disabled = false,
  onCancel,
  showCancel = true,
  className = ''
}) => {
  return (
    <div className={`flex justify-between pt-4 ${className}`}>
      {/* Back/Cancel Button */}
      {showCancel && onCancel && (
        <button
          type="button"
          onClick={onCancel}
          disabled={loading}
          className="px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg font-semibold hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {cancelText}
        </button>
      )}

      {/* Submit Button */}
      <button
        type="submit"
        disabled={disabled || loading}
        className="flex items-center justify-center px-6 py-3 bg-[#1797D5] text-white rounded-lg font-semibold hover:bg-[#147ab8] transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {loading ? (
          <>
            <Spinner size="sm" color="white" className="mr-2" />
            <span>Processing...</span>
          </>
        ) : (
          <>
            {submitText}
            {!showCancel && ' →'}
          </>
        )}
      </button>
    </div>
  );
};
