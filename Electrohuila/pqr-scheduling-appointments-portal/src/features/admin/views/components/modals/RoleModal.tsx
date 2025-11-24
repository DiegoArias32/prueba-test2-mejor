'use client';

import React, { useState, useEffect } from 'react';
import { FiX, FiSave, FiShield, FiCode, FiFileText } from 'react-icons/fi';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface RoleFormData {
  code: string;
  name: string;
  description: string;
}

interface RoleModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: RoleFormData) => Promise<void>;
  item?: Partial<RoleFormData>;
  mode: 'create' | 'edit';
}

export const RoleModal: React.FC<RoleModalProps> = ({
  isOpen,
  onClose,
  onSave,
  item,
  mode
}) => {
  const [formData, setFormData] = useState<RoleFormData>({
    code: '',
    name: '',
    description: ''
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (item && mode === 'edit') {
      setFormData({
        code: item.code || '',
        name: item.name || '',
        description: item.description || ''
      });
    } else {
      setFormData({
        code: '',
        name: '',
        description: ''
      });
    }
    setErrors({});
  }, [item, mode, isOpen]);

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Code validation
    if (!formData.code || formData.code.trim() === '') {
      newErrors.code = 'Código es obligatorio';
    } else {
      // Uppercase validation
      if (formData.code !== formData.code.toUpperCase()) {
        newErrors.code = 'Código debe estar en mayúsculas';
      }
      // No spaces validation
      if (/\s/.test(formData.code)) {
        newErrors.code = 'Código no debe contener espacios';
      }
      // Length validation
      if (formData.code.length < 3) {
        newErrors.code = 'Código debe tener al menos 3 caracteres';
      }
      if (formData.code.length > 20) {
        newErrors.code = 'Código no puede tener más de 20 caracteres';
      }
      // Only letters, numbers and underscore
      if (!/^[A-Z0-9_]+$/.test(formData.code)) {
        newErrors.code = 'Código solo puede contener letras mayúsculas, números y guiones bajos';
      }
    }

    // Name validation
    const nameValidation = ValidationUtils.validateRequired(formData.name, 'Nombre');
    if (!nameValidation.isValid) {
      newErrors.name = nameValidation.message;
    } else if (formData.name.trim().length < 3) {
      newErrors.name = 'Nombre debe tener al menos 3 caracteres';
    } else if (formData.name.trim().length > 100) {
      newErrors.name = 'Nombre no puede tener más de 100 caracteres';
    }

    // Description validation (optional, but if provided must have min length)
    if (formData.description && formData.description.trim().length > 0) {
      if (formData.description.trim().length < 10) {
        newErrors.description = 'Descripción debe tener al menos 10 caracteres';
      }
      if (formData.description.trim().length > 500) {
        newErrors.description = 'Descripción no puede tener más de 500 caracteres';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setLoading(true);
    try {
      await onSave(formData);
      onClose();
    } catch {
      // Error handled by parent
    } finally {
      setLoading(false);
    }
  };

  const handleCodeChange = (value: string) => {
    // Auto-convert to uppercase and remove spaces
    const formatted = value.toUpperCase().replace(/\s/g, '');
    setFormData({ ...formData, code: formatted });
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto animate-fade-in">
      {/* Backdrop */}
      <div
        className="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
        onClick={onClose}
      />

      {/* Modal */}
      <div className="flex min-h-full items-center justify-center p-4">
        <div className="relative bg-white rounded-2xl shadow-2xl w-full max-w-2xl animate-scale-in">
          {/* Header */}
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <div className="flex items-center">
              <FiShield className="w-8 h-8 text-[#1797D5] mr-3" />
              <h2 className="text-2xl font-bold text-gray-900">
                {mode === 'create' ? 'Crear' : 'Editar'} Rol
              </h2>
            </div>
            <button
              onClick={onClose}
              className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
              aria-label="Cerrar"
            >
              <FiX className="w-6 h-6" />
            </button>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit} className="p-6">
            <div className="space-y-4">
              {/* Code */}
              <div>
                <label htmlFor="code" className="block text-sm font-medium text-gray-700 mb-2">
                  Código *
                </label>
                <div className="relative">
                  <FiCode className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="code"
                    type="text"
                    value={formData.code}
                    onChange={(e) => handleCodeChange(e.target.value)}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none uppercase ${
                      errors.code ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ej: ADMIN, EMPLOYEE, SUPERVISOR"
                    disabled={mode === 'edit'} // Code cannot be edited
                  />
                </div>
                {errors.code && (
                  <p className="mt-1 text-sm text-red-600">{errors.code}</p>
                )}
                {mode === 'edit' && (
                  <p className="mt-1 text-xs text-gray-500">
                    El código no se puede modificar después de crear el rol
                  </p>
                )}
                {mode === 'create' && (
                  <p className="mt-1 text-xs text-gray-500">
                    Solo letras mayúsculas, números y guiones bajos. Sin espacios.
                  </p>
                )}
              </div>

              {/* Name */}
              <div>
                <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-2">
                  Nombre *
                </label>
                <div className="relative">
                  <FiShield className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="name"
                    type="text"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.name ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ej: Administrador, Empleado, Supervisor"
                  />
                </div>
                {errors.name && (
                  <p className="mt-1 text-sm text-red-600">{errors.name}</p>
                )}
              </div>

              {/* Description */}
              <div>
                <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-2">
                  Descripción (Opcional)
                </label>
                <div className="relative">
                  <FiFileText className="absolute left-3 top-3 text-gray-400" />
                  <textarea
                    id="description"
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    rows={4}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none resize-none ${
                      errors.description ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Describe las responsabilidades y permisos de este rol..."
                  />
                </div>
                {errors.description && (
                  <p className="mt-1 text-sm text-red-600">{errors.description}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  {formData.description.length}/500 caracteres
                </p>
              </div>

              {/* Info Box */}
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                <div className="flex">
                  <div className="flex-shrink-0">
                    <svg className="h-5 w-5 text-blue-400" viewBox="0 0 20 20" fill="currentColor">
                      <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
                    </svg>
                  </div>
                  <div className="ml-3">
                    <h3 className="text-sm font-medium text-blue-800">
                      Información sobre roles
                    </h3>
                    <div className="mt-2 text-sm text-blue-700">
                      <p>
                        Los roles definen los permisos y accesos que tendrán los usuarios en el sistema.
                        Asegúrate de usar códigos descriptivos y únicos.
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Footer */}
            <div className="flex justify-end space-x-3 mt-6 pt-6 border-t border-gray-200">
              <button
                type="button"
                onClick={onClose}
                className="px-6 py-2 text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors font-medium"
              >
                Cancelar
              </button>
              <button
                type="submit"
                disabled={loading}
                className="px-6 py-2 bg-[#1797D5] text-white rounded-lg hover:bg-[#203461] transition-colors flex items-center disabled:opacity-50 disabled:cursor-not-allowed font-medium"
              >
                <FiSave className="mr-2" />
                {loading ? 'Guardando...' : 'Guardar'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
