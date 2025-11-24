/**
 * Formulario de Registro de Cliente Nuevo
 * Captura información básica para crear un cliente durante el agendamiento
 */

'use client';

import { FC, useState, useEffect } from 'react';
import { FiUser, FiMail, FiPhone, FiMapPin, FiFileText } from 'react-icons/fi';

export interface NewClientData {
  documentType: string;
  documentNumber: string;
  fullName: string;
  phone: string;
  mobile: string;
  email: string;
  address: string;
}

interface NewClientFormProps {
  onDataChange: (data: NewClientData, isValid: boolean) => void;
  initialData?: Partial<NewClientData>;
}

const DOCUMENT_TYPES = [
  { value: 'CC', label: 'Cédula de Ciudadanía' },
  { value: 'TI', label: 'Tarjeta de Identidad' },
  { value: 'CE', label: 'Cédula de Extranjería' },
  { value: 'RC', label: 'Registro Civil' },
  { value: 'NIT', label: 'NIT' },
];

export const NewClientForm: FC<NewClientFormProps> = ({ onDataChange, initialData }) => {
  const [formData, setFormData] = useState<NewClientData>({
    documentType: initialData?.documentType || 'CC',
    documentNumber: initialData?.documentNumber || '',
    fullName: initialData?.fullName || '',
    phone: initialData?.phone || '',
    mobile: initialData?.mobile || '',
    email: initialData?.email || '',
    address: initialData?.address || '',
  });

  const [errors, setErrors] = useState<Partial<Record<keyof NewClientData, string>>>({});
  const [touched, setTouched] = useState<Partial<Record<keyof NewClientData, boolean>>>({});

  // Validaciones
  const validateEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const validateMobile = (mobile: string): boolean => {
    const mobileRegex = /^\d{10}$/;
    return mobileRegex.test(mobile);
  };

  const validateDocumentNumber = (docNumber: string): boolean => {
    return /^\d{1,15}$/.test(docNumber);
  };

  const validateFullName = (name: string): boolean => {
    return name.trim().length >= 3 && name.trim().length <= 200;
  };

  // Validar campo individual
  const validateField = (field: keyof NewClientData, value: string): string => {
    switch (field) {
      case 'documentNumber':
        if (!value.trim()) return 'Número de documento es requerido';
        if (!validateDocumentNumber(value)) return 'Número de documento inválido (solo números, máx 15 dígitos)';
        break;
      case 'fullName':
        if (!value.trim()) return 'Nombre completo es requerido';
        if (!validateFullName(value)) return 'Nombre debe tener entre 3 y 200 caracteres';
        break;
      case 'email':
        if (!value.trim()) return 'Email es requerido';
        if (!validateEmail(value)) return 'Email inválido';
        break;
      case 'mobile':
        if (!value.trim()) return 'Celular es requerido';
        if (!validateMobile(value)) return 'Celular debe tener 10 dígitos';
        break;
    }
    return '';
  };

  // Validar todos los campos requeridos
  const validateForm = (): boolean => {
    const newErrors: Partial<Record<keyof NewClientData, string>> = {};

    newErrors.documentNumber = validateField('documentNumber', formData.documentNumber);
    newErrors.fullName = validateField('fullName', formData.fullName);
    newErrors.email = validateField('email', formData.email);
    newErrors.mobile = validateField('mobile', formData.mobile);

    // Filtrar errores vacíos
    const filteredErrors = Object.fromEntries(
      Object.entries(newErrors).filter(([, v]) => v !== '')
    );

    setErrors(filteredErrors);
    return Object.keys(filteredErrors).length === 0;
  };

  // Manejar cambio de campo
  const handleChange = (field: keyof NewClientData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    setTouched(prev => ({ ...prev, [field]: true }));
  };

  // Manejar blur (validar campo)
  const handleBlur = (field: keyof NewClientData) => {
    setTouched(prev => ({ ...prev, [field]: true }));
    const error = validateField(field, formData[field]);
    setErrors(prev => ({
      ...prev,
      [field]: error,
    }));
  };

  // Notificar cambios al padre
  useEffect(() => {
    const isValid = validateForm();
    onDataChange(formData, isValid);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [formData]);

  return (
    <div className="bg-blue-50 border border-blue-200 rounded-lg p-6 mt-4 animate-fadeIn">
      <h3 className="text-lg font-semibold text-gray-800 mb-4 flex items-center">
        <FiUser className="mr-2 text-[#1797D5]" />
        Datos del Solicitante
      </h3>
      <p className="text-sm text-gray-600 mb-6">
        Complete la siguiente información para continuar con el agendamiento
      </p>

      <div className="space-y-4">
        {/* Tipo de documento */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            Tipo de documento
            <span className="text-red-500 ml-1">*</span>
          </label>
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <FiFileText className="text-gray-400" />
            </div>
            <select
              value={formData.documentType}
              onChange={(e) => handleChange('documentType', e.target.value)}
              className="w-full pl-10 pr-4 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none transition-colors bg-white"
            >
              {DOCUMENT_TYPES.map((type) => (
                <option key={type.value} value={type.value}>
                  {type.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* Número de documento */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            Número de documento
            <span className="text-red-500 ml-1">*</span>
          </label>
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <FiFileText className="text-gray-400" />
            </div>
            <input
              type="text"
              value={formData.documentNumber}
              onChange={(e) => handleChange('documentNumber', e.target.value.replace(/\D/g, ''))}
              onBlur={() => handleBlur('documentNumber')}
              placeholder="Ej: 1234567890"
              maxLength={15}
              className={`w-full pl-10 pr-4 py-3 border-2 rounded-lg focus:outline-none transition-colors ${
                touched.documentNumber && errors.documentNumber
                  ? 'border-red-500 focus:border-red-500'
                  : 'border-gray-300 focus:border-[#1797D5]'
              }`}
            />
          </div>
          {touched.documentNumber && errors.documentNumber && (
            <p className="text-red-500 text-sm mt-1">{errors.documentNumber}</p>
          )}
        </div>

        {/* Nombre completo */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            Nombre completo
            <span className="text-red-500 ml-1">*</span>
          </label>
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <FiUser className="text-gray-400" />
            </div>
            <input
              type="text"
              value={formData.fullName}
              onChange={(e) => handleChange('fullName', e.target.value)}
              onBlur={() => handleBlur('fullName')}
              placeholder="Ej: Juan Pérez García"
              maxLength={200}
              className={`w-full pl-10 pr-4 py-3 border-2 rounded-lg focus:outline-none transition-colors ${
                touched.fullName && errors.fullName
                  ? 'border-red-500 focus:border-red-500'
                  : 'border-gray-300 focus:border-[#1797D5]'
              }`}
            />
          </div>
          {touched.fullName && errors.fullName && (
            <p className="text-red-500 text-sm mt-1">{errors.fullName}</p>
          )}
        </div>

        {/* Email */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            Email
            <span className="text-red-500 ml-1">*</span>
          </label>
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <FiMail className="text-gray-400" />
            </div>
            <input
              type="email"
              value={formData.email}
              onChange={(e) => handleChange('email', e.target.value.toLowerCase())}
              onBlur={() => handleBlur('email')}
              placeholder="ejemplo@correo.com"
              className={`w-full pl-10 pr-4 py-3 border-2 rounded-lg focus:outline-none transition-colors ${
                touched.email && errors.email
                  ? 'border-red-500 focus:border-red-500'
                  : 'border-gray-300 focus:border-[#1797D5]'
              }`}
            />
          </div>
          {touched.email && errors.email && (
            <p className="text-red-500 text-sm mt-1">{errors.email}</p>
          )}
        </div>

        {/* Celular */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            Celular
            <span className="text-red-500 ml-1">*</span>
          </label>
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <FiPhone className="text-gray-400" />
            </div>
            <input
              type="tel"
              value={formData.mobile}
              onChange={(e) => handleChange('mobile', e.target.value.replace(/\D/g, ''))}
              onBlur={() => handleBlur('mobile')}
              placeholder="3001234567"
              maxLength={10}
              className={`w-full pl-10 pr-4 py-3 border-2 rounded-lg focus:outline-none transition-colors ${
                touched.mobile && errors.mobile
                  ? 'border-red-500 focus:border-red-500'
                  : 'border-gray-300 focus:border-[#1797D5]'
              }`}
            />
          </div>
          {touched.mobile && errors.mobile && (
            <p className="text-red-500 text-sm mt-1">{errors.mobile}</p>
          )}
        </div>

        {/* Teléfono fijo (opcional) */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            Teléfono fijo
            <span className="text-gray-400 text-sm ml-1">(opcional)</span>
          </label>
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <FiPhone className="text-gray-400" />
            </div>
            <input
              type="tel"
              value={formData.phone}
              onChange={(e) => handleChange('phone', e.target.value)}
              placeholder="Ej: 6012345678"
              maxLength={20}
              className="w-full pl-10 pr-4 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none transition-colors"
            />
          </div>
        </div>

        {/* Dirección (opcional) */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">
            Dirección
            <span className="text-gray-400 text-sm ml-1">(opcional)</span>
          </label>
          <div className="relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <FiMapPin className="text-gray-400" />
            </div>
            <input
              type="text"
              value={formData.address}
              onChange={(e) => handleChange('address', e.target.value)}
              placeholder="Ej: Calle 123 #45-67"
              maxLength={500}
              className="w-full pl-10 pr-4 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none transition-colors"
            />
          </div>
        </div>
      </div>

      <div className="mt-4 p-3 bg-blue-100 rounded-lg">
        <p className="text-sm text-gray-700">
          <strong>Nota:</strong> Los campos marcados con <span className="text-red-500">*</span> son obligatorios
        </p>
      </div>
    </div>
  );
};
