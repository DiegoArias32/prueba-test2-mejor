'use client';

import React, { useState, useEffect } from 'react';
import { FiX, FiSave, FiMapPin, FiPhone, FiCode, FiHome } from 'react-icons/fi';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface BranchFormData {
  code: string;
  name: string;
  address: string;
  city: string;
  phone: string;
  isMain: boolean;
}

interface BranchModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: BranchFormData) => Promise<void>;
  item?: Partial<BranchFormData>;
  mode: 'create' | 'edit';
}

export const BranchModal: React.FC<BranchModalProps> = ({
  isOpen,
  onClose,
  onSave,
  item,
  mode
}) => {
  const [formData, setFormData] = useState<BranchFormData>({
    code: '',
    name: '',
    address: '',
    city: '',
    phone: '',
    isMain: false
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (item && mode === 'edit') {
      setFormData({
        code: item.code || '',
        name: item.name || '',
        address: item.address || '',
        city: item.city || '',
        phone: item.phone || '',
        isMain: item.isMain === true
      });
    } else {
      setFormData({
        code: '',
        name: '',
        address: '',
        city: '',
        phone: '',
        isMain: false
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
      if (formData.code.trim().length < 3) {
        newErrors.code = 'Código debe tener al menos 3 caracteres';
      }
      if (formData.code.trim().length > 10) {
        newErrors.code = 'Código no puede tener más de 10 caracteres';
      }
      // Only alphanumeric and hyphens
      if (!/^[A-Za-z0-9-]+$/.test(formData.code)) {
        newErrors.code = 'Código solo puede contener letras, números y guiones';
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

    // Address validation
    const addressValidation = ValidationUtils.validateAddress(formData.address);
    if (!addressValidation.isValid) {
      newErrors.address = addressValidation.message;
    }

    // City validation
    const cityValidation = ValidationUtils.validateRequired(formData.city, 'Ciudad');
    if (!cityValidation.isValid) {
      newErrors.city = cityValidation.message;
    } else if (formData.city.trim().length < 3) {
      newErrors.city = 'Ciudad debe tener al menos 3 caracteres';
    } else if (formData.city.trim().length > 100) {
      newErrors.city = 'Ciudad no puede tener más de 100 caracteres';
    }

    // Phone validation
    const phoneValidation = ValidationUtils.validatePhone(formData.phone, true);
    if (!phoneValidation.isValid) {
      newErrors.phone = phoneValidation.message;
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
              <FiHome className="w-8 h-8 text-[#1797D5] mr-3" />
              <h2 className="text-2xl font-bold text-gray-900">
                {mode === 'create' ? 'Crear' : 'Editar'} Sede
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
                    onChange={(e) => setFormData({ ...formData, code: e.target.value.toUpperCase() })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none uppercase ${
                      errors.code ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ej: HQ, NEIVA-01, GARZON"
                    maxLength={10}
                  />
                </div>
                {errors.code && (
                  <p className="mt-1 text-sm text-red-600">{errors.code}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  Código único para identificar la sede (3-10 caracteres)
                </p>
              </div>

              {/* Name */}
              <div>
                <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-2">
                  Nombre *
                </label>
                <div className="relative">
                  <FiHome className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="name"
                    type="text"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.name ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ej: Sede Principal, Sede Garzón"
                  />
                </div>
                {errors.name && (
                  <p className="mt-1 text-sm text-red-600">{errors.name}</p>
                )}
              </div>

              {/* City */}
              <div>
                <label htmlFor="city" className="block text-sm font-medium text-gray-700 mb-2">
                  Ciudad *
                </label>
                <div className="relative">
                  <FiMapPin className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="city"
                    type="text"
                    value={formData.city}
                    onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.city ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ej: Neiva, Garzón, Pitalito"
                  />
                </div>
                {errors.city && (
                  <p className="mt-1 text-sm text-red-600">{errors.city}</p>
                )}
              </div>

              {/* Address */}
              <div>
                <label htmlFor="address" className="block text-sm font-medium text-gray-700 mb-2">
                  Dirección *
                </label>
                <div className="relative">
                  <FiMapPin className="absolute left-3 top-3 text-gray-400" />
                  <textarea
                    id="address"
                    value={formData.address}
                    onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                    rows={3}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none resize-none ${
                      errors.address ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ingrese la dirección completa de la sede"
                  />
                </div>
                {errors.address && (
                  <p className="mt-1 text-sm text-red-600">{errors.address}</p>
                )}
              </div>

              {/* Phone */}
              <div>
                <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-2">
                  Teléfono *
                </label>
                <div className="relative">
                  <FiPhone className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="phone"
                    type="tel"
                    value={formData.phone}
                    onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.phone ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Solo números"
                  />
                </div>
                {errors.phone && (
                  <p className="mt-1 text-sm text-red-600">{errors.phone}</p>
                )}
              </div>

              {/* Is Main */}
              <div className="flex items-start">
                <div className="flex items-center h-5">
                  <input
                    id="isMain"
                    type="checkbox"
                    checked={formData.isMain}
                    onChange={(e) => setFormData({ ...formData, isMain: e.target.checked })}
                    className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                  />
                </div>
                <div className="ml-3 text-sm">
                  <label htmlFor="isMain" className="font-medium text-gray-700 cursor-pointer">
                    Sede Principal
                  </label>
                  <p className="text-gray-500">
                    Marque esta opción si esta es la sede principal de la empresa
                  </p>
                </div>
              </div>

              {/* Info Box */}
              {formData.isMain && (
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
                  <div className="flex">
                    <div className="flex-shrink-0">
                      <svg className="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                        <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                      </svg>
                    </div>
                    <div className="ml-3">
                      <h3 className="text-sm font-medium text-yellow-800">
                        Advertencia
                      </h3>
                      <div className="mt-2 text-sm text-yellow-700">
                        <p>
                          Si marca esta sede como principal, todas las demás sedes serán automáticamente marcadas como no principales.
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
