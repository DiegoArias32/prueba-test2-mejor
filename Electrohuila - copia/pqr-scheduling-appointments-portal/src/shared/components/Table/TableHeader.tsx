/**
 * @file TableHeader.tsx
 * @description Table header with sorting functionality
 * @module shared/components/Table
 */

'use client';

import React from 'react';
import { Column } from './Table';

interface TableHeaderProps<T> {
  columns: Column<T>[];
  sortColumn: string | null;
  sortDirection: 'asc' | 'desc';
  onSort: (columnKey: string) => void;
}

/**
 * TableHeader Component
 * Renders table header with sorting controls
 */
export function TableHeader<T>({
  columns,
  sortColumn,
  sortDirection,
  onSort
}: TableHeaderProps<T>) {
  return (
    <tr className="bg-gray-50 border-b border-gray-200">
      {columns.map((column) => (
        <th
          key={column.key}
          className={`
            px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider
            ${column.sortable ? 'cursor-pointer select-none hover:bg-gray-100' : ''}
            ${column.align === 'center' ? 'text-center' : ''}
            ${column.align === 'right' ? 'text-right' : ''}
          `}
          style={{ width: column.width }}
          onClick={() => column.sortable && onSort(column.key)}
        >
          <div className="flex items-center space-x-2">
            <span>{column.label}</span>
            {column.sortable && (
              <div className="flex flex-col">
                {sortColumn === column.key ? (
                  sortDirection === 'asc' ? (
                    <svg
                      className="w-4 h-4 text-[#1797D5]"
                      fill="currentColor"
                      viewBox="0 0 20 20"
                      aria-label="Sorted ascending"
                    >
                      <path
                        fillRule="evenodd"
                        d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z"
                        clipRule="evenodd"
                      />
                    </svg>
                  ) : (
                    <svg
                      className="w-4 h-4 text-[#1797D5]"
                      fill="currentColor"
                      viewBox="0 0 20 20"
                      aria-label="Sorted descending"
                    >
                      <path
                        fillRule="evenodd"
                        d="M14.707 10.293a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 111.414-1.414L9 12.586V5a1 1 0 012 0v7.586l2.293-2.293a1 1 0 011.414 0z"
                        clipRule="evenodd"
                      />
                    </svg>
                  )
                ) : (
                  <svg
                    className="w-4 h-4 text-gray-400"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                    aria-label="Sortable"
                  >
                    <path d="M5 12a1 1 0 102 0V6.414l1.293 1.293a1 1 0 001.414-1.414l-3-3a1 1 0 00-1.414 0l-3 3a1 1 0 001.414 1.414L5 6.414V12zM15 8a1 1 0 10-2 0v5.586l-1.293-1.293a1 1 0 00-1.414 1.414l3 3a1 1 0 001.414 0l3-3a1 1 0 00-1.414-1.414L15 13.586V8z" />
                  </svg>
                )}
              </div>
            )}
          </div>
        </th>
      ))}
    </tr>
  );
}
