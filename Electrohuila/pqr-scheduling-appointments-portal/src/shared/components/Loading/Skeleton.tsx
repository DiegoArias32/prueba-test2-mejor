/**
 * @file Skeleton.tsx
 * @description Skeleton loader component for content placeholders
 * @module shared/components/Loading
 */

'use client';

import React from 'react';

interface SkeletonProps {
  /** Width of the skeleton */
  width?: string;
  /** Height of the skeleton */
  height?: string;
  /** Shape variant */
  variant?: 'text' | 'circular' | 'rectangular';
  /** Additional CSS classes */
  className?: string;
  /** Animation type */
  animation?: 'pulse' | 'wave';
}

/**
 * Skeleton Component
 * Displays a skeleton loader for content placeholders
 *
 * @component
 * @example
 * ```tsx
 * <Skeleton width="100%" height="20px" variant="text" />
 * ```
 */
export const Skeleton: React.FC<SkeletonProps> = ({
  width = '100%',
  height = '1rem',
  variant = 'text',
  className = '',
  animation = 'pulse'
}) => {
  const baseClasses = 'bg-gray-200';
  const animationClasses = animation === 'pulse' ? 'animate-pulse' : 'animate-wave';

  const variantClasses = {
    text: 'rounded',
    circular: 'rounded-full',
    rectangular: 'rounded-lg'
  };

  return (
    <div
      className={`${baseClasses} ${animationClasses} ${variantClasses[variant]} ${className}`}
      style={{ width, height }}
      role="status"
      aria-label="Loading content"
    >
      <span className="sr-only">Loading...</span>
    </div>
  );
};

/**
 * SkeletonText Component
 * Pre-configured skeleton for text content
 */
export const SkeletonText: React.FC<{ lines?: number; className?: string }> = ({
  lines = 3,
  className = ''
}) => {
  return (
    <div className={`space-y-2 ${className}`}>
      {Array.from({ length: lines }).map((_, index) => (
        <Skeleton
          key={index}
          width={index === lines - 1 ? '80%' : '100%'}
          height="16px"
          variant="text"
        />
      ))}
    </div>
  );
};

/**
 * SkeletonCard Component
 * Pre-configured skeleton for card content
 */
export const SkeletonCard: React.FC<{ className?: string }> = ({ className = '' }) => {
  return (
    <div className={`bg-white rounded-2xl shadow-lg border border-gray-100 p-6 ${className}`}>
      <div className="flex items-start space-x-4 mb-4">
        <Skeleton variant="circular" width="48px" height="48px" />
        <div className="flex-1 space-y-2">
          <Skeleton width="60%" height="20px" />
          <Skeleton width="40%" height="16px" />
        </div>
      </div>
      <SkeletonText lines={3} />
    </div>
  );
};
