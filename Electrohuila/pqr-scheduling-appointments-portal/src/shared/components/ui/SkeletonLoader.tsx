/**
 * SkeletonLoader Component
 * Skeleton loading screens for better UX
 */

'use client';

import React from 'react';
import { motion } from 'framer-motion';

interface SkeletonLoaderProps {
  type?: 'card' | 'list' | 'form' | 'text';
  count?: number;
}

export const SkeletonLoader: React.FC<SkeletonLoaderProps> = ({
  type = 'card',
  count = 1
}) => {
  const shimmer = {
    initial: { backgroundPosition: '-200% 0' },
    animate: {
      backgroundPosition: '200% 0',
      transition: {
        duration: 2,
        repeat: Infinity,
        ease: 'linear'
      }
    }
  };

  const SkeletonCard = () => (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="bg-white rounded-2xl shadow-lg border border-gray-100 p-6"
    >
      <motion.div
        {...shimmer}
        className="h-6 bg-gradient-to-r from-gray-200 via-gray-300 to-gray-200 rounded mb-4"
        style={{ backgroundSize: '200% 100%' }}
      />
      <motion.div
        {...shimmer}
        className="h-4 bg-gradient-to-r from-gray-200 via-gray-300 to-gray-200 rounded mb-2"
        style={{ backgroundSize: '200% 100%', width: '80%' }}
      />
      <motion.div
        {...shimmer}
        className="h-4 bg-gradient-to-r from-gray-200 via-gray-300 to-gray-200 rounded"
        style={{ backgroundSize: '200% 100%', width: '60%' }}
      />
    </motion.div>
  );

  const SkeletonList = () => (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="space-y-3"
    >
      {[...Array(3)].map((_, i) => (
        <motion.div
          key={i}
          {...shimmer}
          className="h-16 bg-gradient-to-r from-gray-200 via-gray-300 to-gray-200 rounded-xl"
          style={{ backgroundSize: '200% 100%' }}
        />
      ))}
    </motion.div>
  );

  const SkeletonForm = () => (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="space-y-4"
    >
      {[...Array(3)].map((_, i) => (
        <div key={i}>
          <motion.div
            {...shimmer}
            className="h-4 bg-gradient-to-r from-gray-200 via-gray-300 to-gray-200 rounded mb-2"
            style={{ backgroundSize: '200% 100%', width: '30%' }}
          />
          <motion.div
            {...shimmer}
            className="h-12 bg-gradient-to-r from-gray-200 via-gray-300 to-gray-200 rounded-xl"
            style={{ backgroundSize: '200% 100%' }}
          />
        </div>
      ))}
    </motion.div>
  );

  const SkeletonText = () => (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="space-y-2"
    >
      {[...Array(4)].map((_, i) => (
        <motion.div
          key={i}
          {...shimmer}
          className="h-4 bg-gradient-to-r from-gray-200 via-gray-300 to-gray-200 rounded"
          style={{
            backgroundSize: '200% 100%',
            width: `${100 - (i * 10)}%`
          }}
        />
      ))}
    </motion.div>
  );

  const renderSkeleton = () => {
    switch (type) {
      case 'card':
        return <SkeletonCard />;
      case 'list':
        return <SkeletonList />;
      case 'form':
        return <SkeletonForm />;
      case 'text':
        return <SkeletonText />;
      default:
        return <SkeletonCard />;
    }
  };

  return (
    <div className="space-y-4">
      {[...Array(count)].map((_, i) => (
        <div key={i}>{renderSkeleton()}</div>
      ))}
    </div>
  );
};
