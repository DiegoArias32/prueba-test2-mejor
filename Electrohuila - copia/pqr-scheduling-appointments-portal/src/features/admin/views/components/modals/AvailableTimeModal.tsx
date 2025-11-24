'use client';

import React, { useState, useEffect } from 'react';
import { FiX, FiSave, FiClock, FiHome, FiCalendar } from 'react-icons/fi';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface Branch {
  id: string;
  code: string;
  name: string;
  city: string;
}

interface AppointmentType {
  id: string;
  name: string;
  icon: string;
}

interface AvailableTimeFormData {
  time: string;
  branchId: string;
  appointmentTypeId: string | null;
}

interface AvailableTimeModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: AvailableTimeFormData) => Promise<void>;
  item?: Partial<AvailableTimeFormData>;
  mode: 'create' | 'edit';
  branches: Branch[];
  appointmentTypes: AppointmentType[];
}

// Generate time options from 6:00 AM to 6:00 PM in 30-minute intervals
const generateTimeOptions = (): string[] => {
  const times: string[] = [];
  for (let hour = 6; hour <= 18; hour++) {
    for (let minute = 0; minute < 60; minute += 30) {
      const timeString = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`;
      times.push(timeString);
    }
  }
  return times;
};

const TIME_OPTIONS = generateTimeOptions();

export const AvailableTimeModal: React.FC<AvailableTimeModalProps> = ({
  isOpen,
  onClose,
  onSave,
  item,
  mode,
  branches,
  appointmentTypes
}) => {
  const [formData, setFormData] = useState<AvailableTimeFormData>({
    time: '08:00',
    branchId: '',
    appointmentTypeId: null
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (item && mode === 'edit') {
      setFormData({
        time: item.time || '08:00',
        branchId: item.branchId || '',
        appointmentTypeId: item.appointmentTypeId || null
      });
    } else {
      setFormData({
        time: '08:00',
        branchId: branches.length > 0 ? branches[0].id : '',
        appointmentTypeId: null
      });
    }
    setErrors({});
  }, [item, mode, isOpen, branches]);

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Time validation
    if (!formData.time || formData.time.trim() === '') {
      newErrors.time = 'Hora es obligatoria';
    } else {
      // Validate HH:mm format
      const timeRegex = /^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$/;
      if (!timeRegex.test(formData.time)) {
        newErrors.time = 'Formato de hora inválido. Debe ser HH:mm (ej: 08:00)';
      } else {
        // Validate time is within business hours (6:00 - 18:00)
        const [hours, minutes] = formData.time.split(':').map(Number);
        if (hours < 6 || hours > 18 || (hours === 18 && minutes > 0)) {
          newErrors.time = 'La hora debe estar entre 06:00 y 18:00';
        }
      }
    }

    // Branch validation
    const branchValidation = ValidationUtils.validateRequired(formData.branchId, 'Sede');
    if (!branchValidation.isValid) {
      newErrors.branchId = branchValidation.message;
    }

    // AppointmentTypeId is optional (null means "all types")
    // No validation needed

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

  const formatTime12Hour = (time: string): string => {
    const [hours, minutes] = time.split(':').map(Number);
    const period = hours >= 12 ? 'PM' : 'AM';
    const displayHours = hours > 12 ? hours - 12 : hours === 0 ? 12 : hours;
    return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`;
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
              <FiClock className="w-8 h-8 text-[#1797D5] mr-3" />
              <h2 className="text-2xl font-bold text-gray-900">
                {mode === 'create' ? 'Crear' : 'Editar'} Hora Disponible
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
              {/* Time */}
              <div>
                <label htmlFor="time" className="block text-sm font-medium text-gray-700 mb-2">
                  Hora *
                </label>
                <div className="relative">
                  <FiClock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <select
                    id="time"
                    value={formData.time}
                    onChange={(e) => setFormData({ ...formData, time: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.time ? 'border-red-500' : 'border-gray-300'
                    }`}
                  >
                    <option value="">Seleccione una hora</option>
                    {TIME_OPTIONS.map((time) => (
                      <option key={time} value={time}>
                        {time} ({formatTime12Hour(time)})
                      </option>
                    ))}
                  </select>
                </div>
                {errors.time && (
                  <p className="mt-1 text-sm text-red-600">{errors.time}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  Horario de atención: 6:00 AM - 6:00 PM
                </p>
              </div>

              {/* Branch */}
              <div>
                <label htmlFor="branchId" className="block text-sm font-medium text-gray-700 mb-2">
                  Sede *
                </label>
                <div className="relative">
                  <FiHome className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <select
                    id="branchId"
                    value={formData.branchId}
                    onChange={(e) => setFormData({ ...formData, branchId: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.branchId ? 'border-red-500' : 'border-gray-300'
                    }`}
                  >
                    <option value="">Seleccione una sede</option>
                    {branches.map((branch) => (
                      <option key={branch.id} value={branch.id}>
                        {branch.name} - {branch.city} ({branch.code})
                      </option>
                    ))}
                  </select>
                </div>
                {errors.branchId && (
                  <p className="mt-1 text-sm text-red-600">{errors.branchId}</p>
                )}
                {branches.length === 0 && (
                  <p className="mt-1 text-sm text-amber-600">
                    No hay sedes disponibles. Por favor cree una sede primero.
                  </p>
                )}
              </div>

              {/* Appointment Type */}
              <div>
                <label htmlFor="appointmentTypeId" className="block text-sm font-medium text-gray-700 mb-2">
                  Tipo de Cita (Opcional)
                </label>
                <div className="relative">
                  <FiCalendar className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <select
                    id="appointmentTypeId"
                    value={formData.appointmentTypeId || ''}
                    onChange={(e) => setFormData({
                      ...formData,
                      appointmentTypeId: e.target.value === '' ? null : e.target.value
                    })}
                    className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none"
                  >
                    <option value="">Todos los tipos de cita</option>
                    {appointmentTypes.map((type) => (
                      <option key={type.id} value={type.id}>
                        {type.name}
                      </option>
                    ))}
                  </select>
                </div>
                <p className="mt-1 text-xs text-gray-500">
                  Si no selecciona un tipo, esta hora estará disponible para todos los tipos de cita
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
                      Sobre las horas disponibles
                    </h3>
                    <div className="mt-2 text-sm text-blue-700">
                      <ul className="list-disc list-inside space-y-1">
                        <li>Las horas disponibles se mostrarán a los usuarios al agendar citas</li>
                        <li>Puede configurar diferentes horarios para diferentes sedes</li>
                        <li>Si especifica un tipo de cita, la hora solo estará disponible para ese tipo</li>
                        <li>Si deja el tipo de cita vacío, la hora estará disponible para todos los tipos</li>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>

              {/* Preview Box */}
              {formData.time && formData.branchId && (
                <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
                  <h4 className="text-sm font-semibold text-gray-900 mb-2">Vista Previa</h4>
                  <div className="space-y-2 text-sm text-gray-700">
                    <div className="flex items-center">
                      <FiClock className="mr-2 text-gray-500" />
                      <span className="font-medium">Hora:</span>
                      <span className="ml-2">{formData.time} ({formatTime12Hour(formData.time)})</span>
                    </div>
                    <div className="flex items-center">
                      <FiHome className="mr-2 text-gray-500" />
                      <span className="font-medium">Sede:</span>
                      <span className="ml-2">
                        {branches.find(b => b.id === formData.branchId)?.name || 'N/A'}
                      </span>
                    </div>
                    <div className="flex items-center">
                      <FiCalendar className="mr-2 text-gray-500" />
                      <span className="font-medium">Tipo de Cita:</span>
                      <span className="ml-2">
                        {formData.appointmentTypeId
                          ? appointmentTypes.find(t => t.id === formData.appointmentTypeId)?.name || 'N/A'
                          : 'Todos los tipos'}
                      </span>
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
                disabled={loading || branches.length === 0}
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
