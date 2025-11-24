/**
 * @file TableSearch.tsx
 * @description Search input for filtering table data
 * @module shared/components/Table
 */

'use client';

import React from 'react';
import { FiSearch, FiX } from 'react-icons/fi';

interface TableSearchProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  className?: string;
}

/**
 * TableSearch Component
 * Search input with clear button for filtering table data
 */
export const TableSearch: React.FC<TableSearchProps> = ({
  value,
  onChange,
  placeholder = 'Search...',
  className = ''
}) => {
  const handleClear = () => {
    onChange('');
  };

  return (
    <div className={`relative ${className}`}>
      {/* Search Icon */}
      <div className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">
        <FiSearch className="w-5 h-5" aria-hidden="true" />
      </div>

      {/* Input */}
      <input
        type="text"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="w-full pl-10 pr-10 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none focus:ring-2 focus:ring-[#1797D5]/20 transition-all duration-300"
        aria-label="Search table"
      />

      {/* Clear Button */}
      {value && (
        <button
          type="button"
          onClick={handleClear}
          className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors"
          aria-label="Clear search"
        >
          <FiX className="w-5 h-5" />
        </button>
      )}
    </div>
  );
};
