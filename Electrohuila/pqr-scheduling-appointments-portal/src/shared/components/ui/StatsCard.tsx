/**
 * StatsCard Component
 * Statistics card with animations
 */

'use client';

import React from 'react';
import { motion } from 'framer-motion';

interface StatsCardProps {
  title: string;
  count: number;
  color: string;
  icon: React.ReactNode;
  delay?: number;
}

export const StatsCard: React.FC<StatsCardProps> = ({
  title,
  count,
  color,
  icon,
  delay = 0
}) => {
  return (
    <motion.div
      className="bg-white rounded-2xl shadow-lg border border-gray-100 p-6 overflow-hidden relative"
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay }}
      whileHover={{ y: -5, shadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)' }}
    >
      {/* Background gradient */}
      <div className={`absolute top-0 right-0 w-32 h-32 ${color} opacity-10 rounded-full blur-2xl`} />

      <div className="flex items-center justify-between relative z-10">
        <div>
          <p className="text-sm font-medium text-gray-600 mb-1">{title}</p>
          <motion.p
            className="text-4xl font-bold text-[#203461]"
            initial={{ scale: 0 }}
            animate={{ scale: 1 }}
            transition={{ type: 'spring', stiffness: 200, delay: delay + 0.2 }}
          >
            {count}
          </motion.p>
        </div>

        <motion.div
          className={`w-14 h-14 rounded-xl flex items-center justify-center ${color}`}
          whileHover={{ scale: 1.1, rotate: 10 }}
          transition={{ duration: 0.2 }}
        >
          {icon}
        </motion.div>
      </div>
    </motion.div>
  );
};
