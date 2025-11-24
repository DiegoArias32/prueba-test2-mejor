/**
 * @file ViewToggle.tsx
 * @description Reusable active/inactive view toggle component
 */

import React from 'react';
import { FiCheckCircle, FiX } from 'react-icons/fi';

interface ViewToggleProps {
  currentView: 'active' | 'inactive';
  onViewChange: (view: 'active' | 'inactive') => void;
}

export const ViewToggle: React.FC<ViewToggleProps> = ({ currentView, onViewChange }) => {
  return (
    <div className="flex bg-gray-100 rounded-lg p-1">
      <button
        onClick={() => onViewChange('active')}
        className={`px-4 py-2 rounded-md text-sm font-medium transition-all duration-200 ${
          currentView === 'active'
            ? 'bg-white text-[#1797D5] shadow-sm'
            : 'text-gray-600 hover:text-gray-900'
        }`}
      >
        <FiCheckCircle className="inline mr-1" />
        Activos
      </button>
      <button
        onClick={() => onViewChange('inactive')}
        className={`px-4 py-2 rounded-md text-sm font-medium transition-all duration-200 ${
          currentView === 'inactive'
            ? 'bg-white text-red-600 shadow-sm'
            : 'text-gray-600 hover:text-gray-900'
        }`}
      >
        <FiX className="inline mr-1" />
        Inactivos
      </button>
    </div>
  );
};
