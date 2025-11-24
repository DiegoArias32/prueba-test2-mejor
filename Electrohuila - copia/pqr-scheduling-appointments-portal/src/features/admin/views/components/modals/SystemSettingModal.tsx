/**
 * @file SystemSettingModal.tsx
 * @description Modal for creating new system settings
 * @module features/admin/views/components/modals
 */

'use client';

import React, { useState } from 'react';
import { FiX } from 'react-icons/fi';
import type { CreateSystemSettingDto } from '@/services/system/system-setting.types';
import { validateSettingKey, validateSettingValue } from '@/features/admin/utils/settingValidators';

interface SystemSettingModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (dto: CreateSystemSettingDto) => Promise<void>;
}

export const SystemSettingModal: React.FC<SystemSettingModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
}) => {
  const [formData, setFormData] = useState<CreateSystemSettingDto>({
    settingKey: '',
    settingValue: '',
    settingType: 'String',
    description: '',
    isEncrypted: false,
  });

  const [errors, setErrors] = useState<Partial<Record<keyof CreateSystemSettingDto, string>>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  if (!isOpen) return null;

  // Handle input change
  const handleChange = (
    field: keyof CreateSystemSettingDto,
    value: string | boolean
  ) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    // Clear error for this field
    setErrors((prev) => ({ ...prev, [field]: undefined }));
  };

  // Validate form
  const validateForm = (): boolean => {
    const newErrors: Partial<Record<keyof CreateSystemSettingDto, string>> = {};

    // Validate key
    const keyValidation = validateSettingKey(formData.settingKey);
    if (!keyValidation.isValid) {
      newErrors.settingKey = keyValidation.error;
    }

    // Validate value
    const valueValidation = validateSettingValue(
      formData.settingKey,
      formData.settingValue,
      formData.settingType as any
    );
    if (!valueValidation.isValid) {
      newErrors.settingValue = valueValidation.error;
    }

    // Validate description
    if (!formData.description || formData.description.trim() === '') {
      newErrors.description = 'La descripción es requerida';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle submit
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);

    try {
      await onSubmit(formData);
      // Reset form
      setFormData({
        settingKey: '',
        settingValue: '',
        settingType: 'String',
        description: '',
        isEncrypted: false,
      });
      setErrors({});
      onClose();
    } catch (error) {
      console.error('Error creating setting:', error);
      setErrors({ settingKey: 'Error al crear la configuración' });
    } finally {
      setIsSubmitting(false);
    }
  };

  // Handle close
  const handleClose = () => {
    if (!isSubmitting) {
      setFormData({
        settingKey: '',
        settingValue: '',
        settingType: 'String',
        description: '',
        isEncrypted: false,
      });
      setErrors({});
      onClose();
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <h2 className="text-xl font-semibold text-gray-900">
            Nueva Configuración del Sistema
          </h2>
          <button
            onClick={handleClose}
            disabled={isSubmitting}
            className="text-gray-400 hover:text-gray-600 disabled:opacity-50"
          >
            <FiX size={24} />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          {/* Setting Key */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Clave de Configuración *
            </label>
            <input
              type="text"
              value={formData.settingKey}
              onChange={(e) => handleChange('settingKey', e.target.value.toUpperCase())}
              placeholder="MAX_APPOINTMENTS_PER_DAY"
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
                errors.settingKey
                  ? 'border-red-300 focus:ring-red-500'
                  : 'border-gray-300 focus:ring-blue-500'
              }`}
            />
            {errors.settingKey && (
              <p className="mt-1 text-sm text-red-600">{errors.settingKey}</p>
            )}
            <p className="mt-1 text-xs text-gray-500">
              Solo mayúsculas, números, guiones bajos y guiones
            </p>
          </div>

          {/* Setting Type */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Tipo de Valor *
            </label>
            <select
              value={formData.settingType}
              onChange={(e) => handleChange('settingType', e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="String">String (Texto)</option>
              <option value="Number">Number (Número)</option>
              <option value="Boolean">Boolean (Verdadero/Falso)</option>
              <option value="Time">Time (Hora)</option>
              <option value="Json">Json (Objeto JSON)</option>
            </select>
          </div>

          {/* Setting Value */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Valor *
            </label>
            {formData.settingType === 'Boolean' ? (
              <select
                value={formData.settingValue}
                onChange={(e) => handleChange('settingValue', e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Seleccionar...</option>
                <option value="true">True (Verdadero)</option>
                <option value="false">False (Falso)</option>
              </select>
            ) : formData.settingType === 'Time' ? (
              <input
                type="time"
                value={formData.settingValue}
                onChange={(e) => handleChange('settingValue', e.target.value)}
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
                  errors.settingValue
                    ? 'border-red-300 focus:ring-red-500'
                    : 'border-gray-300 focus:ring-blue-500'
                }`}
              />
            ) : formData.settingType === 'Number' ? (
              <input
                type="number"
                value={formData.settingValue}
                onChange={(e) => handleChange('settingValue', e.target.value)}
                placeholder="50"
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
                  errors.settingValue
                    ? 'border-red-300 focus:ring-red-500'
                    : 'border-gray-300 focus:ring-blue-500'
                }`}
              />
            ) : formData.settingType === 'Json' ? (
              <textarea
                value={formData.settingValue}
                onChange={(e) => handleChange('settingValue', e.target.value)}
                placeholder='{"key": "value"}'
                rows={4}
                className={`w-full px-3 py-2 border rounded-md font-mono text-sm focus:outline-none focus:ring-2 ${
                  errors.settingValue
                    ? 'border-red-300 focus:ring-red-500'
                    : 'border-gray-300 focus:ring-blue-500'
                }`}
              />
            ) : (
              <input
                type="text"
                value={formData.settingValue}
                onChange={(e) => handleChange('settingValue', e.target.value)}
                placeholder="Valor de configuración"
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
                  errors.settingValue
                    ? 'border-red-300 focus:ring-red-500'
                    : 'border-gray-300 focus:ring-blue-500'
                }`}
              />
            )}
            {errors.settingValue && (
              <p className="mt-1 text-sm text-red-600">{errors.settingValue}</p>
            )}
          </div>

          {/* Description */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Descripción *
            </label>
            <textarea
              value={formData.description}
              onChange={(e) => handleChange('description', e.target.value)}
              placeholder="Descripción de la configuración..."
              rows={3}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
                errors.description
                  ? 'border-red-300 focus:ring-red-500'
                  : 'border-gray-300 focus:ring-blue-500'
              }`}
            />
            {errors.description && (
              <p className="mt-1 text-sm text-red-600">{errors.description}</p>
            )}
          </div>

          {/* Is Encrypted */}
          <div className="flex items-center">
            <input
              type="checkbox"
              id="isEncrypted"
              checked={formData.isEncrypted}
              onChange={(e) => handleChange('isEncrypted', e.target.checked)}
              className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            />
            <label htmlFor="isEncrypted" className="ml-2 text-sm text-gray-700">
              Valor encriptado (para información sensible)
            </label>
          </div>

          {/* Actions */}
          <div className="flex justify-end space-x-3 pt-4 border-t border-gray-200">
            <button
              type="button"
              onClick={handleClose}
              disabled={isSubmitting}
              className="px-4 py-2 text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200 disabled:opacity-50"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {isSubmitting ? 'Creando...' : 'Crear Configuración'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
