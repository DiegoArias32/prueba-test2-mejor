'use client';

import React, { useState, useEffect } from 'react';
import { FiX, FiSave, FiCalendar, FiFileText, FiClock, FiFile } from 'react-icons/fi';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface AppointmentTypeFormData {
  name: string;
  description: string;
  icon: string;
  durationMinutes: number;
  requiresDocumentation: boolean;
}

interface AppointmentTypeModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: AppointmentTypeFormData) => Promise<void>;
  item?: Partial<AppointmentTypeFormData>;
  mode: 'create' | 'edit';
}

const SUGGESTED_ICONS = [
  { value: 'FiCalendar', label: 'Calendario', description: 'Para citas generales' },
  { value: 'FiFileText', label: 'Documento', description: 'Para trámites documentales' },
  { value: 'FiUser', label: 'Usuario', description: 'Para atención personalizada' },
  { value: 'FiClipboard', label: 'Portapapeles', description: 'Para consultas y reclamos' },
  { value: 'FiTool', label: 'Herramienta', description: 'Para servicios técnicos' },
  { value: 'FiZap', label: 'Rayo', description: 'Para servicios eléctricos' },
  { value: 'FiInfo', label: 'Información', description: 'Para información general' },
  { value: 'FiPhone', label: 'Teléfono', description: 'Para soporte telefónico' },
];

export const AppointmentTypeModal: React.FC<AppointmentTypeModalProps> = ({
  isOpen,
  onClose,
  onSave,
  item,
  mode
}) => {
  const [formData, setFormData] = useState<AppointmentTypeFormData>({
    name: '',
    description: '',
    icon: 'FiCalendar',
    durationMinutes: 30,
    requiresDocumentation: false
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (item && mode === 'edit') {
      setFormData({
        name: item.name || '',
        description: item.description || '',
        icon: item.icon || 'FiCalendar',
        durationMinutes: item.durationMinutes || 30,
        requiresDocumentation: item.requiresDocumentation || false
      });
    } else {
      setFormData({
        name: '',
        description: '',
        icon: 'FiCalendar',
        durationMinutes: 30,
        requiresDocumentation: false
      });
    }
    setErrors({});
  }, [item, mode, isOpen]);

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

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

    // Icon validation
    const iconValidation = ValidationUtils.validateRequired(formData.icon, 'Icono');
    if (!iconValidation.isValid) {
      newErrors.icon = iconValidation.message;
    }

    // Duration validation
    if (!formData.durationMinutes) {
      newErrors.durationMinutes = 'Duración es obligatoria';
    } else {
      const duration = Number(formData.durationMinutes);
      if (isNaN(duration)) {
        newErrors.durationMinutes = 'Duración debe ser un número válido';
      } else if (duration < 15) {
        newErrors.durationMinutes = 'Duración mínima es 15 minutos';
      } else if (duration > 480) {
        newErrors.durationMinutes = 'Duración máxima es 480 minutos (8 horas)';
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

  const handleDurationChange = (value: string) => {
    const numValue = parseInt(value);
    if (!isNaN(numValue) || value === '') {
      setFormData({ ...formData, durationMinutes: numValue || 0 });
    }
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
              <FiCalendar className="w-8 h-8 text-[#1797D5] mr-3" />
              <h2 className="text-2xl font-bold text-gray-900">
                {mode === 'create' ? 'Crear' : 'Editar'} Tipo de Cita
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
              {/* Name */}
              <div>
                <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-2">
                  Nombre *
                </label>
                <div className="relative">
                  <FiCalendar className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="name"
                    type="text"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.name ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ej: Atención al Cliente, Reclamos, Nuevas Instalaciones"
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
                    placeholder="Describe para qué sirve este tipo de cita y qué pueden esperar los usuarios..."
                  />
                </div>
                {errors.description && (
                  <p className="mt-1 text-sm text-red-600">{errors.description}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  {formData.description.length}/500 caracteres
                </p>
              </div>

              {/* Icon */}
              <div>
                <label htmlFor="icon" className="block text-sm font-medium text-gray-700 mb-2">
                  Icono *
                </label>
                <select
                  id="icon"
                  value={formData.icon}
                  onChange={(e) => setFormData({ ...formData, icon: e.target.value })}
                  className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                    errors.icon ? 'border-red-500' : 'border-gray-300'
                  }`}
                >
                  <option value="">Seleccione un icono</option>
                  {SUGGESTED_ICONS.map((icon) => (
                    <option key={icon.value} value={icon.value}>
                      {icon.label} - {icon.description}
                    </option>
                  ))}
                </select>
                {errors.icon && (
                  <p className="mt-1 text-sm text-red-600">{errors.icon}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  Este icono se mostrará junto al tipo de cita en la interfaz
                </p>
              </div>

              {/* Duration Minutes */}
              <div>
                <label htmlFor="durationMinutes" className="block text-sm font-medium text-gray-700 mb-2">
                  Duración (minutos) *
                </label>
                <div className="relative">
                  <FiClock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="durationMinutes"
                    type="number"
                    min="15"
                    max="480"
                    step="15"
                    value={formData.durationMinutes}
                    onChange={(e) => handleDurationChange(e.target.value)}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.durationMinutes ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="30"
                  />
                </div>
                {errors.durationMinutes && (
                  <p className="mt-1 text-sm text-red-600">{errors.durationMinutes}</p>
                )}
                <div className="mt-2 flex flex-wrap gap-2">
                  {[15, 30, 45, 60, 90, 120].map((minutes) => (
                    <button
                      key={minutes}
                      type="button"
                      onClick={() => setFormData({ ...formData, durationMinutes: minutes })}
                      className={`px-3 py-1 text-sm rounded-lg border transition-colors ${
                        formData.durationMinutes === minutes
                          ? 'bg-blue-500 text-white border-blue-500'
                          : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
                      }`}
                    >
                      {minutes} min
                    </button>
                  ))}
                </div>
                <p className="mt-2 text-xs text-gray-500">
                  Duración estimada de la cita (mínimo 15 minutos, máximo 480 minutos)
                </p>
              </div>

              {/* Requires Documentation */}
              <div className="flex items-start">
                <div className="flex items-center h-5">
                  <input
                    id="requiresDocumentation"
                    type="checkbox"
                    checked={formData.requiresDocumentation}
                    onChange={(e) => setFormData({ ...formData, requiresDocumentation: e.target.checked })}
                    className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                  />
                </div>
                <div className="ml-3 text-sm">
                  <label htmlFor="requiresDocumentation" className="font-medium text-gray-700 cursor-pointer flex items-center">
                    <FiFile className="mr-2" />
                    Requiere Documentación
                  </label>
                  <p className="text-gray-500">
                    Marque esta opción si los usuarios deben traer o presentar documentos para esta cita
                  </p>
                </div>
              </div>

              {/* Info Box */}
              {formData.requiresDocumentation && (
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                  <div className="flex">
                    <div className="flex-shrink-0">
                      <svg className="h-5 w-5 text-blue-400" viewBox="0 0 20 20" fill="currentColor">
                        <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
                      </svg>
                    </div>
                    <div className="ml-3">
                      <h3 className="text-sm font-medium text-blue-800">
                        Documentación requerida
                      </h3>
                      <div className="mt-2 text-sm text-blue-700">
                        <p>
                          Se informará a los usuarios que deben traer la documentación necesaria cuando agenden este tipo de cita.
                          Asegúrese de especificar qué documentos son necesarios en la descripción.
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              )}
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
