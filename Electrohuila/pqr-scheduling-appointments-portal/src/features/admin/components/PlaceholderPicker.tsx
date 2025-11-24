/**
 * PlaceholderPicker Component
 * Displays available placeholders grouped by category
 * Allows clicking to insert placeholder into template
 */

'use client';

import React, { useState } from 'react';
import { FiCopy, FiInfo } from 'react-icons/fi';
import type { PlaceholderInfo } from '@/services/notifications/notification-template.types';
import { PLACEHOLDER_INFO } from '@/services/notifications/notification-template.types';

interface PlaceholderPickerProps {
  onInsert: (placeholder: string) => void;
  compact?: boolean;
}

export const PlaceholderPicker: React.FC<PlaceholderPickerProps> = ({
  onInsert,
  compact = false
}) => {
  const [selectedCategory, setSelectedCategory] = useState<'all' | 'Cliente' | 'Cita' | 'Sucursal'>('all');
  const [copiedPlaceholder, setCopiedPlaceholder] = useState<string | null>(null);

  const categories = ['all', 'Cliente', 'Cita', 'Sucursal'] as const;

  const filteredPlaceholders = selectedCategory === 'all'
    ? PLACEHOLDER_INFO
    : PLACEHOLDER_INFO.filter(p => p.category === selectedCategory);

  const handleInsert = (placeholder: string) => {
    onInsert(placeholder);
    setCopiedPlaceholder(placeholder);
    setTimeout(() => setCopiedPlaceholder(null), 1500);
  };

  const getCategoryIcon = (category: string) => {
    switch (category) {
      case 'Cliente': return '👤';
      case 'Cita': return '📅';
      case 'Sucursal': return '🏢';
      default: return '📋';
    }
  };

  const getCategoryColor = (category: string) => {
    switch (category) {
      case 'Cliente': return 'blue';
      case 'Cita': return 'green';
      case 'Sucursal': return 'purple';
      default: return 'gray';
    }
  };

  if (compact) {
    return (
      <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
        {filteredPlaceholders.map((placeholder) => (
          <button
            key={placeholder.code}
            type="button"
            onClick={() => handleInsert(placeholder.code)}
            className="px-3 py-2 text-xs font-mono bg-gray-50 hover:bg-blue-50 border border-gray-200 hover:border-blue-300 rounded text-left transition-all group relative"
            title={placeholder.description}
          >
            <span className="text-gray-700 group-hover:text-blue-600">
              {placeholder.code}
            </span>
            {copiedPlaceholder === placeholder.code && (
              <span className="absolute -top-8 left-1/2 transform -translate-x-1/2 px-2 py-1 bg-green-600 text-white text-xs rounded whitespace-nowrap">
                Insertado!
              </span>
            )}
          </button>
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Category Filters */}
      <div className="flex flex-wrap gap-2">
        {categories.map((category) => {
          const isActive = selectedCategory === category;
          const color = category === 'all' ? 'gray' : getCategoryColor(category);

          return (
            <button
              key={category}
              type="button"
              onClick={() => setSelectedCategory(category)}
              className={`px-4 py-2 rounded-lg text-sm font-medium transition-all ${
                isActive
                  ? `bg-${color}-100 text-${color}-700 border-${color}-300 border-2`
                  : 'bg-white text-gray-600 border-gray-200 border hover:border-gray-300'
              }`}
            >
              <span className="mr-2">{getCategoryIcon(category)}</span>
              {category === 'all' ? 'Todos' : category}
            </button>
          );
        })}
      </div>

      {/* Placeholders Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
        {filteredPlaceholders.map((placeholder) => {
          const color = getCategoryColor(placeholder.category);
          const isCopied = copiedPlaceholder === placeholder.code;

          return (
            <div
              key={placeholder.code}
              className={`bg-white border-2 rounded-lg p-4 transition-all hover:shadow-md ${
                isCopied
                  ? 'border-green-400 bg-green-50'
                  : `border-${color}-200 hover:border-${color}-400`
              }`}
            >
              <div className="flex items-start justify-between mb-2">
                <div className="flex-1">
                  <div className="flex items-center space-x-2 mb-1">
                    <span className="text-lg">{getCategoryIcon(placeholder.category)}</span>
                    <code className={`text-sm font-bold ${
                      isCopied ? 'text-green-700' : `text-${color}-700`
                    }`}>
                      {placeholder.code}
                    </code>
                  </div>
                  <p className="text-xs text-gray-600 mb-2">
                    {placeholder.description}
                  </p>
                  <div className="flex items-center space-x-1 text-xs text-gray-500">
                    <FiInfo className="w-3 h-3" />
                    <span>Ejemplo: {placeholder.example}</span>
                  </div>
                </div>
                <button
                  type="button"
                  onClick={() => handleInsert(placeholder.code)}
                  className={`ml-2 p-2 rounded-lg transition-all ${
                    isCopied
                      ? 'bg-green-100 text-green-700'
                      : `bg-${color}-100 hover:bg-${color}-200 text-${color}-700`
                  }`}
                  title="Insertar placeholder"
                >
                  <FiCopy className="w-4 h-4" />
                </button>
              </div>
              {isCopied && (
                <div className="mt-2 text-xs text-green-600 font-medium flex items-center">
                  <span className="w-2 h-2 bg-green-500 rounded-full mr-2 animate-pulse"></span>
                  Insertado en el template
                </div>
              )}
            </div>
          );
        })}
      </div>

      {filteredPlaceholders.length === 0 && (
        <div className="text-center py-8 text-gray-500">
          No hay placeholders en esta categoría
        </div>
      )}
    </div>
  );
};
