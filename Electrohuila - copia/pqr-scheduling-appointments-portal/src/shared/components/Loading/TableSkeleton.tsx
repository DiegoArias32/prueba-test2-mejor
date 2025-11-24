/**
 * @file TableSkeleton.tsx
 * @description Skeleton loader specifically designed for tables
 * @module shared/components/Loading
 */

'use client';

import React from 'react';
import { Skeleton } from './Skeleton';

interface TableSkeletonProps {
  /** Number of rows to display */
  rows?: number;
  /** Number of columns to display */
  columns?: number;
  /** Show table header */
  showHeader?: boolean;
  /** Additional CSS classes */
  className?: string;
}

/**
 * TableSkeleton Component
 * Displays a skeleton loader for table content
 *
 * @component
 * @example
 * ```tsx
 * <TableSkeleton rows={5} columns={4} showHeader />
 * ```
 */
export const TableSkeleton: React.FC<TableSkeletonProps> = ({
  rows = 5,
  columns = 4,
  showHeader = true,
  className = ''
}) => {
  return (
    <div className={`w-full ${className}`} role="status" aria-label="Loading table">
      <div className="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden">
        {/* Table Header Skeleton */}
        {showHeader && (
          <div className="bg-gray-50 border-b border-gray-200 p-4">
            <div className="grid gap-4" style={{ gridTemplateColumns: `repeat(${columns}, 1fr)` }}>
              {Array.from({ length: columns }).map((_, colIndex) => (
                <Skeleton key={`header-${colIndex}`} height="20px" width="80%" />
              ))}
            </div>
          </div>
        )}

        {/* Table Body Skeleton */}
        <div className="divide-y divide-gray-200">
          {Array.from({ length: rows }).map((_, rowIndex) => (
            <div
              key={`row-${rowIndex}`}
              className="p-4 hover:bg-gray-50 transition-colors"
            >
              <div className="grid gap-4" style={{ gridTemplateColumns: `repeat(${columns}, 1fr)` }}>
                {Array.from({ length: columns }).map((_, colIndex) => (
                  <Skeleton
                    key={`cell-${rowIndex}-${colIndex}`}
                    height="16px"
                    width={colIndex === 0 ? '90%' : '70%'}
                  />
                ))}
              </div>
            </div>
          ))}
        </div>
      </div>
      <span className="sr-only">Loading table data...</span>
    </div>
  );
};

/**
 * TableSkeletonCompact Component
 * More compact version for smaller tables or mobile views
 */
export const TableSkeletonCompact: React.FC<{ rows?: number; className?: string }> = ({
  rows = 3,
  className = ''
}) => {
  return (
    <div className={`w-full space-y-3 ${className}`}>
      {Array.from({ length: rows }).map((_, index) => (
        <div
          key={`compact-row-${index}`}
          className="bg-white rounded-xl shadow border border-gray-100 p-4"
        >
          <div className="flex items-center space-x-3 mb-3">
            <Skeleton variant="circular" width="40px" height="40px" />
            <div className="flex-1 space-y-2">
              <Skeleton width="60%" height="18px" />
              <Skeleton width="40%" height="14px" />
            </div>
          </div>
          <div className="space-y-2">
            <Skeleton width="100%" height="12px" />
            <Skeleton width="80%" height="12px" />
          </div>
        </div>
      ))}
    </div>
  );
};
