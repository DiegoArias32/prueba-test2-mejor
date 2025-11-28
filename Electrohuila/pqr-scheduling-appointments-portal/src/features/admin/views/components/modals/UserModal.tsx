'use client';

import React, { useState, useEffect } from 'react';
import { FiX, FiSave, FiUser, FiMail, FiLock } from 'react-icons/fi';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

interface Role {
  id: string;
  code: string;
  name: string;
}

interface UserFormData {
  username: string;
  email: string;
  password?: string;
  roleIds: string[];
}

interface UserItem extends Partial<UserFormData> {
  roles?: Role[];
}

interface UserModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: UserFormData) => Promise<void>;
  item?: UserItem;
  mode: 'create' | 'edit';
  roles: Role[];
}

export const UserModal: React.FC<UserModalProps> = ({
  isOpen,
  onClose,
  onSave,
  item,
  mode,
  roles
}) => {
  const [formData, setFormData] = useState<UserFormData>({
    username: '',
    email: '',
    password: '',
    roleIds: []
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (item && mode === 'edit') {
      const roleIds = item.roleIds || [];
      setFormData({
        username: item.username || '',
        email: item.email || '',
        roleIds: roleIds
      });
    } else {
      setFormData({
        username: '',
        email: '',
        password: '',
        roleIds: []
      });
    }
    setErrors({});
  }, [item, mode, isOpen]);

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Username validation
    if (!formData.username || formData.username.trim().length < 3) {
      newErrors.username = formData.username.trim() === ''
        ? 'Nombre de usuario es obligatorio'
        : 'Nombre de usuario debe tener al menos 3 caracteres';
    }

    // Email validation
    const emailValidation = ValidationUtils.validateEmail(formData.email);
    if (!emailValidation.isValid) {
      newErrors.email = emailValidation.message;
    }

    // Password validation (only on create)
    if (mode === 'create') {
      if (!formData.password || formData.password.length < 6) {
        newErrors.password = formData.password === ''
          ? 'Contraseña es obligatoria'
          : 'Contraseña debe tener al menos 6 caracteres';
      }
    }

    // Roles validation
    if (formData.roleIds.length === 0) {
      newErrors.roleIds = 'Debe seleccionar al menos un rol';
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

  const handleRoleToggle = (roleId: string) => {
    setFormData(prev => ({
      ...prev,
      roleIds: prev.roleIds.includes(roleId)
        ? prev.roleIds.filter(id => id !== roleId)
        : [...prev.roleIds, roleId]
    }));
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
        <div className="relative bg-white rounded-2xl shadow-2xl w-full max-w-3xl animate-scale-in">
          {/* Header */}
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <div className="flex items-center">
              <FiUser className="w-8 h-8 text-[#1797D5] mr-3" />
              <h2 className="text-2xl font-bold text-gray-900">
                {mode === 'create' ? 'Crear' : 'Editar'} Empleado
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
            <div className="space-y-4 max-h-[60vh] overflow-y-auto pr-2">
              {/* Username */}
              <div>
                <label htmlFor="username" className="block text-sm font-medium text-gray-700 mb-2">
                  Nombre de Usuario *
                </label>
                <div className="relative">
                  <FiUser className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="username"
                    type="text"
                    value={formData.username}
                    onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.username ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Ingrese nombre de usuario"
                  />
                </div>
                {errors.username && (
                  <p className="mt-1 text-sm text-red-600">{errors.username}</p>
                )}
              </div>

              {/* Email */}
              <div>
                <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
                  Correo Electrónico *
                </label>
                <div className="relative">
                  <FiMail className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                  <input
                    id="email"
                    type="email"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                      errors.email ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="ejemplo@electrohuila.com"
                  />
                </div>
                {errors.email && (
                  <p className="mt-1 text-sm text-red-600">{errors.email}</p>
                )}
              </div>

              {/* Password (only on create) */}
              {mode === 'create' && (
                <div>
                  <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-2">
                    Contraseña *
                  </label>
                  <div className="relative">
                    <FiLock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                    <input
                      id="password"
                      type="password"
                      value={formData.password}
                      onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                      className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none ${
                        errors.password ? 'border-red-500' : 'border-gray-300'
                      }`}
                      placeholder="Mínimo 6 caracteres"
                    />
                  </div>
                  {errors.password && (
                    <p className="mt-1 text-sm text-red-600">{errors.password}</p>
                  )}
                </div>
              )}

              {/* Roles */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Roles *
                </label>
                <div className="border border-gray-300 rounded-lg p-4 space-y-2 max-h-48 overflow-y-auto">
                  {roles.length === 0 ? (
                    <p className="text-sm text-gray-500 text-center py-4">
                      No hay roles disponibles
                    </p>
                  ) : (
                    roles.map((role) => (
                      <label
                        key={role.id}
                        className="flex items-center space-x-3 p-2 hover:bg-gray-50 rounded cursor-pointer"
                      >
                        <input
                          type="checkbox"
                          checked={formData.roleIds.includes(role.id)}
                          onChange={() => handleRoleToggle(role.id)}
                          className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                        />
                        <div className="flex-1">
                          <span className="text-sm font-medium text-gray-900">{role.name}</span>
                          <span className="text-xs text-gray-500 ml-2">({role.code})</span>
                        </div>
                      </label>
                    ))
                  )}
                </div>
                {errors.roleIds && (
                  <p className="mt-1 text-sm text-red-600">{errors.roleIds}</p>
                )}
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
