/**
 * @file SettingCard.tsx
 * @description Reusable card component for individual system settings
 * @module features/admin/components
 */

'use client';

import React, { useState, useEffect, useCallback } from 'react';
import { FiSave, FiCheck, FiAlertCircle, FiLoader } from 'react-icons/fi';
import type { SystemSettingDto } from '@/services/system/system-setting.types';
import { validateSettingValue } from '../utils/settingValidators';

type SaveState = 'unchanged' | 'modified' | 'saving' | 'saved' | 'error';

interface SettingCardProps {
  setting: SystemSettingDto;
  onSave: (key: string, value: string) => Promise<void>;
  autoSave?: boolean;
  autoSaveDelay?: number;
}

// Mapa de traducciones para nombres de configuraciones
const SETTING_NAME_TRANSLATIONS: Record<string, string> = {
  'MAX_APPOINTMENTS_PER_DAY': 'MÁXIMO DE CITAS POR DÍA',
  'APPOINTMENT_CANCELLATION_HOURS': 'HORAS DE CANCELACIÓN DE CITA',
  'BUSINESS_HOURS_START': 'HORA DE INICIO DE ATENCIÓN',
  'BUSINESS_HOURS_END': 'HORA DE FIN DE ATENCIÓN',
  'EMAIL_NOTIFICATIONS_ENABLED': 'NOTIFICACIONES POR EMAIL HABILITADAS',
  'SMS_NOTIFICATIONS_ENABLED': 'NOTIFICACIONES POR SMS HABILITADAS',
  'APPOINTMENT_REMINDER_HOURS': 'HORAS DE RECORDATORIO DE CITA',
};

export const SettingCard: React.FC<SettingCardProps> = ({
  setting,
  onSave,
  autoSave = true,
  autoSaveDelay = 1000,
}) => {
  const [value, setValue] = useState(setting.settingValue);
  const [saveState, setSaveState] = useState<SaveState>('unchanged');
  const [validationError, setValidationError] = useState<string | null>(null);
  const [autoSaveTimeout, setAutoSaveTimeout] = useState<NodeJS.Timeout | null>(null);

  // Update local value when setting changes
  useEffect(() => {
    setValue(setting.settingValue);
    setSaveState('unchanged');
  }, [setting.settingValue]);

  // Validate value
  const validate = useCallback((val: string): boolean => {
    const result = validateSettingValue(setting.settingKey, val, setting.settingType);
    if (!result.isValid) {
      setValidationError(result.error || 'Valor inválido');
      return false;
    }
    setValidationError(null);
    return true;
  }, [setting.settingKey, setting.settingType]);

  // Handle value change
  const handleChange = (newValue: string) => {
    setValue(newValue);
    setSaveState('modified');

    // Validate on change
    validate(newValue);

    // Clear existing timeout
    if (autoSaveTimeout) {
      clearTimeout(autoSaveTimeout);
    }

    // Set up auto-save if enabled
    if (autoSave && newValue !== setting.settingValue) {
      const timeout = setTimeout(() => {
        handleSave(newValue);
      }, autoSaveDelay);
      setAutoSaveTimeout(timeout);
    }
  };

  // Handle manual save
  const handleSave = async (valueToSave?: string) => {
    const finalValue = valueToSave || value;

    // Validate before saving
    if (!validate(finalValue)) {
      setSaveState('error');
      return;
    }

    // Don't save if unchanged
    if (finalValue === setting.settingValue) {
      setSaveState('unchanged');
      return;
    }

    setSaveState('saving');

    try {
      await onSave(setting.settingKey, finalValue);
      setSaveState('saved');

      // Reset to unchanged after 2 seconds
      setTimeout(() => {
        if (saveState === 'saved') {
          setSaveState('unchanged');
        }
      }, 2000);
    } catch (error) {
      console.error('Error saving setting:', error);
      setSaveState('error');
      setValidationError('Error al guardar la configuración');
    }
  };

  // Cleanup timeout on unmount
  useEffect(() => {
    return () => {
      if (autoSaveTimeout) {
        clearTimeout(autoSaveTimeout);
      }
    };
  }, [autoSaveTimeout]);

  // Render input based on setting type
  const renderInput = () => {
    switch (setting.settingType) {
      case 'Boolean':
        return (
          <div className="flex items-center">
            <button
              type="button"
              onClick={() => handleChange(value === 'true' ? 'false' : 'true')}
              className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors ${
                value === 'true' ? 'bg-blue-600' : 'bg-gray-300'
              }`}
            >
              <span
                className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
                  value === 'true' ? 'translate-x-6' : 'translate-x-1'
                }`}
              />
            </button>
            <span className="ml-3 text-sm font-medium text-gray-700">
              {value === 'true' ? 'Habilitado' : 'Deshabilitado'}
            </span>
          </div>
        );

      case 'Number':
        return (
          <input
            type="number"
            value={value}
            onChange={(e) => handleChange(e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
              validationError
                ? 'border-red-300 focus:ring-red-500'
                : 'border-gray-300 focus:ring-blue-500'
            }`}
          />
        );

      case 'Time':
        return (
          <input
            type="time"
            value={value}
            onChange={(e) => handleChange(e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
              validationError
                ? 'border-red-300 focus:ring-red-500'
                : 'border-gray-300 focus:ring-blue-500'
            }`}
          />
        );

      case 'Json':
        return (
          <textarea
            value={value}
            onChange={(e) => handleChange(e.target.value)}
            rows={4}
            className={`w-full px-3 py-2 border rounded-md font-mono text-sm focus:outline-none focus:ring-2 ${
              validationError
                ? 'border-red-300 focus:ring-red-500'
                : 'border-gray-300 focus:ring-blue-500'
            }`}
          />
        );

      case 'String':
      default:
        return (
          <input
            type="text"
            value={value}
            onChange={(e) => handleChange(e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
              validationError
                ? 'border-red-300 focus:ring-red-500'
                : 'border-gray-300 focus:ring-blue-500'
            }`}
          />
        );
    }
  };

  // Render save indicator
  const renderSaveIndicator = () => {
    switch (saveState) {
      case 'saving':
        return (
          <div className="flex items-center text-blue-600">
            <FiLoader className="animate-spin mr-2" />
            <span className="text-sm">Guardando...</span>
          </div>
        );

      case 'saved':
        return (
          <div className="flex items-center text-green-600">
            <FiCheck className="mr-2" />
            <span className="text-sm">Guardado</span>
          </div>
        );

      case 'error':
        return (
          <div className="flex items-center text-red-600">
            <FiAlertCircle className="mr-2" />
            <span className="text-sm">Error</span>
          </div>
        );

      case 'modified':
        if (!autoSave) {
          return (
            <button
              onClick={() => handleSave()}
              disabled={!!validationError}
              className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <FiSave className="mr-2" />
              Guardar
            </button>
          );
        }
        return (
          <div className="flex items-center text-yellow-600">
            <span className="text-sm">Auto-guardando...</span>
          </div>
        );

      case 'unchanged':
      default:
        return null;
    }
  };

  return (
    <div className="bg-white border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow">
      {/* Header */}
      <div className="flex items-start justify-between mb-3">
        <div className="flex-1">
          <h4 className="text-sm font-semibold text-gray-900">
            {SETTING_NAME_TRANSLATIONS[setting.settingKey] || setting.settingKey.replace(/_/g, ' ')}
          </h4>
          <p className="text-xs text-gray-500 mt-1">{setting.description}</p>
        </div>
        <span className="ml-4 px-2 py-1 bg-gray-100 text-gray-600 text-xs rounded">
          {setting.settingType}
        </span>
      </div>

      {/* Input */}
      <div className="mb-3">
        {renderInput()}
      </div>

      {/* Validation Error */}
      {validationError && (
        <div className="mb-3 flex items-start text-red-600 text-sm">
          <FiAlertCircle className="mt-0.5 mr-2 flex-shrink-0" />
          <span>{validationError}</span>
        </div>
      )}

      {/* Footer */}
      <div className="flex items-center justify-between">
        <span className="text-xs text-gray-400">
          Actualizado: {new Date(setting.updatedAt).toLocaleString('es-ES')}
        </span>
        {renderSaveIndicator()}
      </div>
    </div>
  );
};
