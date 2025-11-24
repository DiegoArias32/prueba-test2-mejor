/**
 * Document Stats Component
 * Muestra estadísticas visuales de los documentos de una cita
 */

'use client';

import React from 'react';
import { FiFile, FiImage, FiFileText, FiHardDrive } from 'react-icons/fi';
import type { AppointmentDocumentsStatsDto } from '@/services/documents';
import { formatFileSize } from '@/shared/utils/fileHelpers';

interface DocumentStatsProps {
  stats: AppointmentDocumentsStatsDto;
  isLoading?: boolean;
  className?: string;
}

export const DocumentStats: React.FC<DocumentStatsProps> = ({
  stats,
  isLoading = false,
  className = '',
}) => {
  if (isLoading) {
    return (
      <div className={`grid grid-cols-2 md:grid-cols-4 gap-4 ${className}`}>
        {[...Array(4)].map((_, index) => (
          <div
            key={index}
            className="bg-white rounded-lg shadow p-4 animate-pulse"
          >
            <div className="flex items-center space-x-3">
              <div className="w-10 h-10 bg-gray-200 rounded-lg"></div>
              <div className="flex-1">
                <div className="h-4 bg-gray-200 rounded w-12 mb-2"></div>
                <div className="h-3 bg-gray-200 rounded w-20"></div>
              </div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  const statItems = [
    {
      label: 'Total',
      value: stats.totalDocuments,
      icon: FiFile,
      color: 'text-blue-600',
      bgColor: 'bg-blue-100',
    },
    {
      label: 'Imágenes',
      value: stats.imageCount,
      icon: FiImage,
      color: 'text-purple-600',
      bgColor: 'bg-purple-100',
    },
    {
      label: 'PDFs',
      value: stats.pdfCount,
      icon: FiFileText,
      color: 'text-red-600',
      bgColor: 'bg-red-100',
    },
    {
      label: 'Tamaño',
      value: formatFileSize(stats.totalSizeBytes),
      icon: FiHardDrive,
      color: 'text-green-600',
      bgColor: 'bg-green-100',
    },
  ];

  return (
    <div className={`grid grid-cols-2 md:grid-cols-4 gap-4 ${className}`}>
      {statItems.map((item, index) => {
        const Icon = item.icon;

        return (
          <div
            key={index}
            className="bg-white rounded-lg shadow hover:shadow-md transition-shadow p-4"
          >
            <div className="flex items-center space-x-3">
              <div className={`${item.bgColor} ${item.color} p-2.5 rounded-lg`}>
                <Icon className="w-5 h-5" />
              </div>
              <div>
                <div className="text-2xl font-bold text-gray-900">
                  {typeof item.value === 'number'
                    ? item.value.toLocaleString()
                    : item.value}
                </div>
                <div className="text-sm text-gray-600">{item.label}</div>
              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
};
