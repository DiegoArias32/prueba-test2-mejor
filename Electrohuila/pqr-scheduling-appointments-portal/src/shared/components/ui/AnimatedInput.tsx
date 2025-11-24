/**
 * AnimatedInput Component
 * Input field with modern animations and validation states
 */

'use client';

import React, { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

interface AnimatedInputProps {
  label: string;
  value: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
  onKeyDown?: (e: React.KeyboardEvent<HTMLInputElement>) => void;
  type?: string;
  placeholder?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  maxLength?: number;
  icon?: React.ReactNode;
  className?: string;
}

export const AnimatedInput: React.FC<AnimatedInputProps> = ({
  label,
  value,
  onChange,
  onBlur,
  onKeyDown,
  type = 'text',
  placeholder,
  error,
  required = false,
  disabled = false,
  maxLength,
  icon,
  className = ''
}) => {
  const [isFocused, setIsFocused] = useState(false);

  return (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className={`relative ${className}`}
    >
      <label className="block text-sm font-semibold text-[#203461] mb-2">
        {label} {required && <span className="text-red-500">*</span>}
      </label>

      <div className="relative">
        {icon && (
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            {icon}
          </div>
        )}

        <motion.input
          type={type}
          value={value}
          onChange={(e) => onChange(e.target.value)}
          onBlur={() => {
            setIsFocused(false);
            onBlur?.();
          }}
          onFocus={() => setIsFocused(true)}
          onKeyDown={onKeyDown}
          disabled={disabled}
          maxLength={maxLength}
          placeholder={placeholder}
          whileFocus={{ scale: 1.01 }}
          className={`
            w-full ${icon ? 'pl-10' : 'pl-4'} pr-4 py-3
            border-2 rounded-xl
            focus:ring-2 focus:border-transparent
            transition-all duration-300
            disabled:bg-gray-100 disabled:cursor-not-allowed
            ${error
              ? 'border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:ring-[#1797D5] focus:border-[#1797D5]'
            }
            ${isFocused ? 'shadow-lg' : 'shadow-sm'}
          `}
        />
      </div>

      <AnimatePresence>
        {error && (
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            className="text-red-600 text-sm mt-1 flex items-center"
          >
            <svg className="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
              <path
                fillRule="evenodd"
                d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
                clipRule="evenodd"
              />
            </svg>
            {error}
          </motion.div>
        )}
      </AnimatePresence>

      {maxLength && value && (
        <div className="text-xs text-gray-500 mt-1 text-right">
          {value.length}/{maxLength}
        </div>
      )}
    </motion.div>
  );
};
