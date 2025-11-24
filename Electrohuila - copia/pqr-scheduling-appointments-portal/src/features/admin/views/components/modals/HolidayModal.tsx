'use client';

import React, { useState, useEffect } from 'react';
import { FiX, FiSave, FiCalendar, FiTag, FiMapPin } from 'react-icons/fi';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';
import { apiService } from '@/services';
import type { BranchDto } from '@/services';

type HolidayType = 'National' | 'Local' | 'Company';

interface HolidayFormData {
  holidayDate: string;
  holidayName: string;
  holidayType: HolidayType;
  branchId?: number;
}

interface HolidayModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: HolidayFormData) => Promise<void>;
  item?: Partial<HolidayFormData & { id: number }>;
  mode: 'create' | 'edit';
}

export const HolidayModal: React.FC<HolidayModalProps> = ({
  isOpen,
  onClose,
  onSave,
  item,
  mode
}) => {
  const [formData, setFormData] = useState<HolidayFormData>({
    holidayDate: '',
    holidayName: '',
    holidayType: 'National',
    branchId: undefined
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);
  const [branches, setBranches] = useState<BranchDto[]>([]);
  const [loadingBranches, setLoadingBranches] = useState(false);

  // Load branches when modal opens or when type changes to Local
  useEffect(() => {
    const loadBranches = async () => {
      if (!isOpen) return;

      setLoadingBranches(true);
      try {
        const branchesData = await apiService.getBranches();
        setBranches(branchesData.filter(b => b.isActive));
      } catch (error) {
        console.error('Error loading branches:', error);
        setBranches([]);
      } finally {
        setLoadingBranches(false);
      }
    };

    loadBranches();
  }, [isOpen]);

  useEffect(() => {
    if (item && mode === 'edit') {
      setFormData({
        holidayDate: item.holidayDate || '',
        holidayName: item.holidayName || '',
        holidayType: item.holidayType || 'National',
        branchId: item.branchId
      });
    } else {
      setFormData({
        holidayDate: '',
        holidayName: '',
        holidayType: 'National',
        branchId: undefined
      });
    }
    setErrors({});
  }, [item, mode, isOpen]);

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Date validation
    if (!formData.holidayDate || formData.holidayDate.trim() === '') {
      newErrors.holidayDate = 'Fecha es obligatoria';
    } else {
      // Validate date format (YYYY-MM-DD)
      const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
      if (!dateRegex.test(formData.holidayDate)) {
        newErrors.holidayDate = 'Formato de fecha inválido (usar YYYY-MM-DD)';
      } else {
        // Validate it's a valid date
        const date = new Date(formData.holidayDate);
        if (isNaN(date.getTime())) {
          newErrors.holidayDate = 'Fecha inválida';
        }
      }
    }

    // Name validation
    const nameValidation = ValidationUtils.validateRequired(formData.holidayName, 'Nombre');
    if (!nameValidation.isValid) {
      newErrors.holidayName = nameValidation.message;
    } else if (formData.holidayName.trim().length < 3) {
      newErrors.holidayName = 'Nombre debe tener al menos 3 caracteres';
    } else if (formData.holidayName.trim().length > 100) {
      newErrors.holidayName = 'Nombre no puede tener más de 100 caracteres';
    }

    // Holiday type validation
    if (!formData.holidayType) {
      newErrors.holidayType = 'Tipo de festivo es obligatorio';
    } else if (!['National', 'Local', 'Company'].includes(formData.holidayType)) {
      newErrors.holidayType = 'Tipo de festivo inválido';
    }

    // Branch validation for Local holidays
    if (formData.holidayType === 'Local') {
      if (!formData.branchId) {
        newErrors.branchId = 'Sucursal es obligatoria para festivos locales';
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
      // Clean up branchId if not Local type
      const submitData: HolidayFormData = {
        ...formData,
        holidayName: formData.holidayName.trim()
      };

      if (submitData.holidayType !== 'Local') {
        delete submitData.branchId;
      }

      await onSave(submitData);
      onClose();
    } catch (error) {
      // Error handled by parent
      console.error('Error saving holiday:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleTypeChange = (type: HolidayType) => {
    setFormData({
      ...formData,
      holidayType: type,
      branchId: type === 'Local' ? formData.branchId : undefined
    });
    // Clear branch error if switching away from Local
    if (type !== 'Local' && errors.branchId) {
      const newErrors = { ...errors };
      delete newErrors.branchId;
      setErrors(newErrors);
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
                {mode === 'create' ? 'Crear' : 'Editar'} Festivo
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
              {/* Holiday Date */}
              <div>
                <label htmlFor="holidayDate" className="block text-sm font-medium text-gray-700 mb-2">
                  Fecha del Festivo *
                </label>
                <div className="relative">
                  <FiCalendar className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="holidayDate"
                    type="date"
                    value={formData.holidayDate}
                    onChange={(e) => setFormData({ ...formData, holidayDate: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.holidayDate ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Seleccionar fecha"
                  />
                </div>
                {errors.holidayDate && (
                  <p className="mt-1 text-sm text-red-600">{errors.holidayDate}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  Selecciona la fecha del festivo
                </p>
              </div>

              {/* Holiday Name */}
              <div>
                <label htmlFor="holidayName" className="block text-sm font-medium text-gray-700 mb-2">
                  Nombre del Festivo *
                </label>
                <div className="relative">
                  <FiTag className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="holidayName"
                    type="text"
                    value={formData.holidayName}
                    onChange={(e) => setFormData({ ...formData, holidayName: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.holidayName ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ej: Día de la Independencia"
                    maxLength={100}
                  />
                </div>
                {errors.holidayName && (
                  <p className="mt-1 text-sm text-red-600">{errors.holidayName}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  Nombre descriptivo del festivo (3-100 caracteres)
                </p>
              </div>

              {/* Holiday Type */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Tipo de Festivo *
                </label>
                <div className="grid grid-cols-3 gap-3">
                  <button
                    type="button"
                    onClick={() => handleTypeChange('National')}
                    className={`px-4 py-3 rounded-lg border-2 transition-all ${
                      formData.holidayType === 'National'
                        ? 'border-blue-500 bg-blue-50 text-blue-700 font-medium'
                        : 'border-gray-200 bg-white text-gray-700 hover:border-blue-300'
                    }`}
                  >
                    <div className="text-center">
                      <div className="text-sm font-semibold">Nacional</div>
                      <div className="text-xs mt-1">Todo el país</div>
                    </div>
                  </button>
                  <button
                    type="button"
                    onClick={() => handleTypeChange('Local')}
                    className={`px-4 py-3 rounded-lg border-2 transition-all ${
                      formData.holidayType === 'Local'
                        ? 'border-green-500 bg-green-50 text-green-700 font-medium'
                        : 'border-gray-200 bg-white text-gray-700 hover:border-green-300'
                    }`}
                  >
                    <div className="text-center">
                      <div className="text-sm font-semibold">Local</div>
                      <div className="text-xs mt-1">Por sucursal</div>
                    </div>
                  </button>
                  <button
                    type="button"
                    onClick={() => handleTypeChange('Company')}
                    className={`px-4 py-3 rounded-lg border-2 transition-all ${
                      formData.holidayType === 'Company'
                        ? 'border-purple-500 bg-purple-50 text-purple-700 font-medium'
                        : 'border-gray-200 bg-white text-gray-700 hover:border-purple-300'
                    }`}
                  >
                    <div className="text-center">
                      <div className="text-sm font-semibold">Empresa</div>
                      <div className="text-xs mt-1">Toda la empresa</div>
                    </div>
                  </button>
                </div>
                {errors.holidayType && (
                  <p className="mt-1 text-sm text-red-600">{errors.holidayType}</p>
                )}
                <p className="mt-2 text-xs text-gray-500">
                  {formData.holidayType === 'National' && 'Festivo nacional que aplica en todo el país'}
                  {formData.holidayType === 'Local' && 'Festivo local que aplica solo a una sucursal específica'}
                  {formData.holidayType === 'Company' && 'Festivo de empresa que aplica a todas las sucursales'}
                </p>
              </div>

              {/* Branch Selection (only for Local holidays) */}
              {formData.holidayType === 'Local' && (
                <div>
                  <label htmlFor="branchId" className="block text-sm font-medium text-gray-700 mb-2">
                    Sucursal *
                  </label>
                  <div className="relative">
                    <FiMapPin className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                    <select
                      id="branchId"
                      value={formData.branchId || ''}
                      onChange={(e) => setFormData({
                        ...formData,
                        branchId: e.target.value ? Number(e.target.value) : undefined
                      })}
                      className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                        errors.branchId ? 'border-red-500' : 'border-gray-300'
                      }`}
                      disabled={loadingBranches}
                    >
                      <option value="">Seleccionar sucursal</option>
                      {branches.map(branch => (
                        <option key={branch.id} value={branch.id}>
                          {branch.name} - {branch.city}
                        </option>
                      ))}
                    </select>
                  </div>
                  {errors.branchId && (
                    <p className="mt-1 text-sm text-red-600">{errors.branchId}</p>
                  )}
                  {loadingBranches && (
                    <p className="mt-1 text-xs text-gray-500">Cargando sucursales...</p>
                  )}
                  {!loadingBranches && branches.length === 0 && (
                    <p className="mt-1 text-xs text-amber-600">No hay sucursales activas disponibles</p>
                  )}
                  <p className="mt-1 text-xs text-gray-500">
                    Selecciona la sucursal a la que aplica este festivo local
                  </p>
                </div>
              )}

              {/* Info Box */}
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                <div className="flex items-start">
                  <FiCalendar className="w-5 h-5 text-blue-500 mt-0.5 mr-3 flex-shrink-0" />
                  <div className="text-sm text-blue-800">
                    <p className="font-medium mb-1">Información importante:</p>
                    <ul className="list-disc list-inside space-y-1 text-xs">
                      <li>Los festivos afectan la disponibilidad de citas</li>
                      <li>Los festivos nacionales aplican en todas las sucursales</li>
                      <li>Los festivos locales solo aplican en la sucursal seleccionada</li>
                      <li>Los festivos de empresa aplican en todas las sucursales de la empresa</li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>

            {/* Actions */}
            <div className="flex justify-end space-x-4 mt-6 pt-6 border-t border-gray-200">
              <button
                type="button"
                onClick={onClose}
                className="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
                disabled={loading}
              >
                Cancelar
              </button>
              <button
                type="submit"
                disabled={loading}
                className="px-6 py-2 bg-[#1797D5] text-white rounded-lg hover:bg-[#203461] transition-colors flex items-center disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <FiSave className="mr-2" />
                {loading ? 'Guardando...' : mode === 'create' ? 'Crear Festivo' : 'Actualizar Festivo'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
