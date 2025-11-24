/**
 * @file TableRow.tsx
 * @description Table row component with custom rendering
 * @module shared/components/Table
 */

'use client';

import React from 'react';
import { Column } from './Table';

interface TableRowProps<T> {
  row: T;
  columns: Column<T>[];
  index: number;
  onClick?: () => void;
}

/**
 * TableRow Component
 * Renders a single table row with cell data
 */
export function TableRow<T extends Record<string, unknown>>({
  row,
  columns,
  index,
  onClick
}: TableRowProps<T>) {
  return (
    <tr
      className={`
        hover:bg-gray-50 transition-colors
        ${onClick ? 'cursor-pointer' : ''}
      `}
      onClick={onClick}
    >
      {columns.map((column) => {
        const value = row[column.key];
        const content = column.render ? column.render(value, row, index) : value;

        return (
          <td
            key={column.key}
            className={`
              px-6 py-4 text-sm text-gray-900
              ${column.align === 'center' ? 'text-center' : ''}
              ${column.align === 'right' ? 'text-right' : ''}
            `}
          >
            {content}
          </td>
        );
      })}
    </tr>
  );
}
