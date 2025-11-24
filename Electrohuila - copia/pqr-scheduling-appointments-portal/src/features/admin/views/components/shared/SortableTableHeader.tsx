/**
 * @file SortableTableHeader.tsx
 * @description Reusable sortable table header component
 */

import React from 'react';
import { FiChevronUp, FiChevronDown } from 'react-icons/fi';

type SortDirection = 'asc' | 'desc';

interface SortableTableHeaderProps {
  column: string;
  label: string;
  onClick?: () => void;
  sortColumn?: string;
  sortDirection?: SortDirection;
}

export const SortableTableHeader: React.FC<SortableTableHeaderProps> = ({
  column,
  label,
  onClick,
  sortColumn = '',
  sortDirection = 'asc'
}) => {
  const isSorted = sortColumn === column;
  const isClickable = onClick !== undefined;

  return (
    <th
      className={`px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider ${
        isClickable ? 'cursor-pointer hover:bg-gray-100 select-none' : ''
      }`}
      onClick={onClick}
    >
      <div className="flex items-center space-x-1">
        <span>{label}</span>
        {isClickable && (
          <div className="flex flex-col">
            <FiChevronUp
              className={`w-3 h-3 -mb-1 ${
                isSorted && sortDirection === 'asc' ? 'text-[#1797D5]' : 'text-gray-300'
              }`}
            />
            <FiChevronDown
              className={`w-3 h-3 ${
                isSorted && sortDirection === 'desc' ? 'text-[#1797D5]' : 'text-gray-300'
              }`}
            />
          </div>
        )}
      </div>
    </th>
  );
};
