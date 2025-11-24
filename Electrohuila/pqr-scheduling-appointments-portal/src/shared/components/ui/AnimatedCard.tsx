/**
 * AnimatedCard Component
 * Card component with modern animations and hover effects
 */

'use client';

import React from 'react';
import { motion } from 'framer-motion';

interface AnimatedCardProps {
  children: React.ReactNode;
  className?: string;
  delay?: number;
  hover?: boolean;
  gradient?: boolean;
  onClick?: () => void;
}

export const AnimatedCard: React.FC<AnimatedCardProps> = ({
  children,
  className = '',
  delay = 0,
  hover = true,
  gradient = false,
  onClick
}) => {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay }}
      whileHover={hover ? { y: -8, transition: { duration: 0.2 } } : {}}
      onClick={onClick}
      className={`relative group ${className}`}
    >
      {/* Gradient border effect */}
      {gradient && (
        <div className="absolute inset-0 bg-gradient-to-r from-[#203461] via-[#1797D5] to-[#56C2E1] rounded-3xl opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
      )}

      <div className={`relative bg-white ${gradient ? 'rounded-3xl m-[1px]' : 'rounded-2xl'} shadow-xl border border-gray-100 hover:shadow-2xl transition-shadow duration-300`}>
        {children}
      </div>
    </motion.div>
  );
};
