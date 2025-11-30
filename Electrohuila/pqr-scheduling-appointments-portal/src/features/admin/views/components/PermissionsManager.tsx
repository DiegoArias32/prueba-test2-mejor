/**
 * PermissionsManager Component
 * Gestión completa de permisos rol-formulario con acordeones
 */

'use client';

import React, { useState, useEffect } from 'react';
import { FiChevronDown, FiChevronUp, FiCheck, FiX, FiLock, FiUnlock, FiSave } from 'react-icons/fi';
import type { RolDto, FormPermissionDto, RolPermissionSummaryDto } from '@/services/api';

interface PermissionsManagerProps {
  roles: RolDto[];
  permissions: FormPermissionDto[];
  rolPermissions: RolPermissionSummaryDto[];
  onUpdatePermission: (rolId: number, formCode: string, permissionData: PermissionUpdate) => Promise<void>;
  loading?: boolean;
}

export interface PermissionUpdate {
  canCreate: boolean;
  canRead: boolean;
  canUpdate: boolean;
  canDelete: boolean;
}

interface RoleAccordionState {
  [rolId: number]: boolean;
}

export const PermissionsManager: React.FC<PermissionsManagerProps> = ({
  roles,
  permissions,
  rolPermissions,
  onUpdatePermission,
  loading = false
}) => {
  const [expandedRoles, setExpandedRoles] = useState<RoleAccordionState>({});
  const [localPermissions, setLocalPermissions] = useState<{ [key: string]: PermissionUpdate }>({});
  const [savingStates, setSavingStates] = useState<{ [key: string]: boolean }>({});
  const [changedPermissions, setChangedPermissions] = useState<Set<string>>(new Set());

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 5; // Show 5 forms per page when expanded

  // Initialize local permissions from server data
  useEffect(() => {
    const initialPerms: { [key: string]: PermissionUpdate } = {};

    rolPermissions.forEach(rolPerm => {
      rolPerm.formPermissions?.forEach(form => {
        const key = `${rolPerm.rolId}-${form.formCode}`;
        initialPerms[key] = {
          canCreate: form.assignedPermission?.canCreate || false,
          canRead: form.assignedPermission?.canRead || false,
          canUpdate: form.assignedPermission?.canUpdate || false,
          canDelete: form.assignedPermission?.canDelete || false
        };
      });
    });

    setLocalPermissions(initialPerms);
  }, [rolPermissions]);

  // Toggle role accordion
  const toggleRole = (rolId: number) => {
    setExpandedRoles(prev => ({
      ...prev,
      [rolId]: !prev[rolId]
    }));
    // Reset pagination when expanding a role
    setCurrentPage(1);
  };

  // Get permission key
  const getPermKey = (rolId: number, formCode: string) => {
    return `${rolId}-${formCode}`;
  };

  // Get local permission or default
  const getLocalPermission = (rolId: number, formCode: string): PermissionUpdate => {
    const key = getPermKey(rolId, formCode);
    return localPermissions[key] || {
      canCreate: false,
      canRead: false,
      canUpdate: false,
      canDelete: false
    };
  };

  // Toggle individual permission
  const togglePermission = (rolId: number, formCode: string, permissionType: keyof PermissionUpdate) => {
    const key = getPermKey(rolId, formCode);
    const currentPerm = getLocalPermission(rolId, formCode);

    const updatedPerm = {
      ...currentPerm,
      [permissionType]: !currentPerm[permissionType]
    };

    setLocalPermissions(prev => ({
      ...prev,
      [key]: updatedPerm
    }));

    setChangedPermissions(prev => new Set(prev).add(key));
  };

  // Save permissions for a specific role-form combination
  const savePermissions = async (rolId: number, formCode: string) => {
    const key = getPermKey(rolId, formCode);
    const permission = getLocalPermission(rolId, formCode);

    setSavingStates(prev => ({ ...prev, [key]: true }));

    try {
      await onUpdatePermission(rolId, formCode, permission);
      setChangedPermissions(prev => {
        const newSet = new Set(prev);
        newSet.delete(key);
        return newSet;
      });
    } catch (error) {
      console.error('Error saving permissions:', error);
    } finally {
      setSavingStates(prev => ({ ...prev, [key]: false }));
    }
  };

  // Lista de códigos de formularios permitidos (solo los que existen en el menú lateral)
  const ALLOWED_FORM_CODES = [
    'appointments', 'APPOINTMENTS',
    'appointment-types', 'APPOINTMENT_TYPES',
    'available-times', 'AVAILABLE_TIMES',
    'users', 'USERS',
    'roles', 'ROLES',
    'permissions', 'PERMISSIONS',
    'branches', 'BRANCHES',
    'holidays', 'HOLIDAYS',
    'settings', 'SETTINGS'
  ];

  // Get unique form codes from rolPermissions (filtered by allowed forms)
  const getFormCodes = () => {
    const formCodesSet = new Set<string>();
    // Extract form codes from all role permissions
    rolPermissions.forEach(rolPerm => {
      rolPerm.formPermissions?.forEach(form => {
        // Solo agregar si está en la lista de formularios permitidos
        if (ALLOWED_FORM_CODES.includes(form.formCode)) {
          formCodesSet.add(form.formCode);
        }
      });
    });
    return Array.from(formCodesSet).sort();
  };

  // Get form display name
  const getFormDisplayName = (formCode: string) => {
    const formNames: { [key: string]: string } = {
      // Formularios válidos del sistema (kebab-case)
      'appointments': 'Citas',
      'appointment-types': 'Tipos de Cita',
      'available-times': 'Horarios Disponibles',
      'branches': 'Sucursales',
      'clients': 'Clientes',
      'new-accounts': 'Nuevas Cuentas',
      'project-news': 'Proyectos Nuevos',
      'users': 'Usuarios',
      'roles': 'Roles',
      'permissions': 'Permisos',
      'forms': 'Formularios',
      'modules': 'Módulos',

      // Formularios válidos del sistema (UPPER_CASE)
      'APPOINTMENTS': 'Citas',
      'APPOINTMENT_TYPES': 'Tipos de Cita',
      'AVAILABLE_TIMES': 'Horarios Disponibles',
      'BRANCHES': 'Sucursales',
      'CLIENTS': 'Clientes',
      'NEW_ACCOUNTS': 'Nuevas Cuentas',
      'PROJECT_NEWS': 'Proyectos Nuevos',
      'USERS': 'Usuarios',
      'ROLES': 'Roles',
      'PERMISSIONS': 'Permisos',
      'FORMS': 'Formularios',
      'MODULES': 'Módulos',

      // Formularios adicionales (kebab-case)
      'settings': 'Configuración',
      'theme-settings': 'Configuración de Tema',
      'documents': 'Documentos',
      'holidays': 'Festivos',
      'notifications': 'Notificaciones',

      // Formularios adicionales (UPPER_CASE)
      'SETTINGS': 'Configuración',
      'THEME_SETTINGS': 'Configuración de Tema',
      'DOCUMENTS': 'Documentos',
      'HOLIDAYS': 'Festivos',
      'NOTIFICATIONS': 'Notificaciones'
    };
    return formNames[formCode] || formCode;
  };

  // Count enabled permissions
  const countEnabledPermissions = (rolId: number, formCode: string): number => {
    const perm = getLocalPermission(rolId, formCode);
    return [perm.canCreate, perm.canRead, perm.canUpdate, perm.canDelete].filter(Boolean).length;
  };

  const formCodes = getFormCodes();

  if (loading) {
    return (
      <div className="flex items-center justify-center p-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#1797D5]"></div>
      </div>
    );
  }

  if (roles.length === 0) {
    return (
      <div className="text-center p-12">
        <FiLock className="w-16 h-16 mx-auto text-gray-300 mb-4" />
        <p className="text-gray-500">No hay roles disponibles</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="bg-blue-50 border border-blue-200 rounded-xl p-4">
        <p className="text-sm text-blue-800">
          <strong>Gestión de Permisos:</strong> Configura qué acciones puede realizar cada rol en cada módulo del sistema.
          Los cambios se guardan individualmente por módulo.
        </p>
      </div>

      {/* Roles Accordion */}
      {roles.filter(r => r.isActive).map(role => {
        const isExpanded = expandedRoles[role.id];

        return (
          <div key={role.id} className="bg-white border border-gray-200 rounded-xl overflow-hidden shadow-sm hover:shadow-md transition-shadow">
            {/* Role Header */}
            <button
              onClick={() => toggleRole(role.id)}
              className="w-full px-6 py-4 flex items-center justify-between hover:bg-gray-50 transition-colors"
            >
              <div className="flex items-center space-x-4">
                <div className={`w-12 h-12 rounded-lg flex items-center justify-center ${
                  isExpanded ? 'bg-[#1797D5] text-white' : 'bg-gray-100 text-gray-600'
                }`}>
                  <FiLock className="w-6 h-6" />
                </div>
                <div className="text-left">
                  <h3 className="text-lg font-semibold text-gray-900">{role.name}</h3>
                  <p className="text-sm text-gray-500">Código: {role.code}</p>
                </div>
              </div>
              <div className="flex items-center space-x-4">
                <span className="text-sm text-gray-500">
                  {formCodes.reduce((acc, formCode) => acc + countEnabledPermissions(role.id, formCode), 0)} permisos activos
                </span>
                {isExpanded ? (
                  <FiChevronUp className="w-5 h-5 text-gray-400" />
                ) : (
                  <FiChevronDown className="w-5 h-5 text-gray-400" />
                )}
              </div>
            </button>

            {/* Role Content */}
            {isExpanded && (
              <div className="border-t border-gray-200 bg-gray-50 p-6">
                {/* Pagination Controls */}
                {formCodes.length > itemsPerPage && (
                  <div className="flex items-center justify-between mb-4 bg-white p-3 rounded-lg border border-gray-200">
                    <span className="text-sm text-gray-600">
                      Mostrando {((currentPage - 1) * itemsPerPage) + 1}-{Math.min(currentPage * itemsPerPage, formCodes.length)} de {formCodes.length} formularios
                    </span>
                    <div className="flex space-x-2">
                      <button
                        onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                        disabled={currentPage === 1}
                        className="px-3 py-1 text-sm border rounded hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                        Anterior
                      </button>
                      <span className="px-3 py-1 text-sm">
                        Página {currentPage} de {Math.ceil(formCodes.length / itemsPerPage)}
                      </span>
                      <button
                        onClick={() => setCurrentPage(prev => Math.min(Math.ceil(formCodes.length / itemsPerPage), prev + 1))}
                        disabled={currentPage === Math.ceil(formCodes.length / itemsPerPage)}
                        className="px-3 py-1 text-sm border rounded hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                        Siguiente
                      </button>
                    </div>
                  </div>
                )}

                <div className="space-y-4">
                  {formCodes.slice((currentPage - 1) * itemsPerPage, currentPage * itemsPerPage).map(formCode => {
                    const key = getPermKey(role.id, formCode);
                    const perm = getLocalPermission(role.id, formCode);
                    const hasChanges = changedPermissions.has(key);
                    const isSaving = savingStates[key];
                    const enabledCount = countEnabledPermissions(role.id, formCode);

                    return (
                      <div key={formCode} className="bg-white border border-gray-200 rounded-lg p-4">
                        {/* Form Header */}
                        <div className="flex items-center justify-between mb-4">
                          <div className="flex items-center space-x-3">
                            <div className={`w-10 h-10 rounded-lg flex items-center justify-center ${
                              enabledCount > 0 ? 'bg-green-100 text-green-600' : 'bg-gray-100 text-gray-400'
                            }`}>
                              {enabledCount > 0 ? <FiUnlock className="w-5 h-5" /> : <FiLock className="w-5 h-5" />}
                            </div>
                            <div>
                              <h4 className="font-semibold text-gray-900">{getFormDisplayName(formCode)}</h4>
                              <p className="text-xs text-gray-500">{enabledCount}/4 permisos activos</p>
                            </div>
                          </div>
                          {hasChanges && (
                            <button
                              onClick={() => savePermissions(role.id, formCode)}
                              disabled={isSaving}
                              className="px-4 py-2 bg-[#1797D5] text-white rounded-lg hover:bg-[#203461] transition-colors flex items-center disabled:opacity-50"
                            >
                              {isSaving ? (
                                <>
                                  <div className="animate-spin rounded-full h-4 w-4 border-2 border-white border-t-transparent mr-2"></div>
                                  Guardando...
                                </>
                              ) : (
                                <>
                                  <FiSave className="mr-2" />
                                  Guardar Cambios
                                </>
                              )}
                            </button>
                          )}
                        </div>

                        {/* Permission Toggles */}
                        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                          {/* Create */}
                          <button
                            onClick={() => togglePermission(role.id, formCode, 'canCreate')}
                            className={`p-3 rounded-lg border-2 transition-all ${
                              perm.canCreate
                                ? 'bg-green-50 border-green-500 text-green-700'
                                : 'bg-gray-50 border-gray-300 text-gray-500 hover:border-gray-400'
                            }`}
                          >
                            <div className="flex items-center justify-center mb-1">
                              {perm.canCreate ? (
                                <FiCheck className="w-5 h-5" />
                              ) : (
                                <FiX className="w-5 h-5" />
                              )}
                            </div>
                            <p className="text-xs font-medium">Crear</p>
                          </button>

                          {/* Read */}
                          <button
                            onClick={() => togglePermission(role.id, formCode, 'canRead')}
                            className={`p-3 rounded-lg border-2 transition-all ${
                              perm.canRead
                                ? 'bg-blue-50 border-blue-500 text-blue-700'
                                : 'bg-gray-50 border-gray-300 text-gray-500 hover:border-gray-400'
                            }`}
                          >
                            <div className="flex items-center justify-center mb-1">
                              {perm.canRead ? (
                                <FiCheck className="w-5 h-5" />
                              ) : (
                                <FiX className="w-5 h-5" />
                              )}
                            </div>
                            <p className="text-xs font-medium">Leer</p>
                          </button>

                          {/* Update */}
                          <button
                            onClick={() => togglePermission(role.id, formCode, 'canUpdate')}
                            className={`p-3 rounded-lg border-2 transition-all ${
                              perm.canUpdate
                                ? 'bg-yellow-50 border-yellow-500 text-yellow-700'
                                : 'bg-gray-50 border-gray-300 text-gray-500 hover:border-gray-400'
                            }`}
                          >
                            <div className="flex items-center justify-center mb-1">
                              {perm.canUpdate ? (
                                <FiCheck className="w-5 h-5" />
                              ) : (
                                <FiX className="w-5 h-5" />
                              )}
                            </div>
                            <p className="text-xs font-medium">Actualizar</p>
                          </button>

                          {/* Delete */}
                          <button
                            onClick={() => togglePermission(role.id, formCode, 'canDelete')}
                            className={`p-3 rounded-lg border-2 transition-all ${
                              perm.canDelete
                                ? 'bg-red-50 border-red-500 text-red-700'
                                : 'bg-gray-50 border-gray-300 text-gray-500 hover:border-gray-400'
                            }`}
                          >
                            <div className="flex items-center justify-center mb-1">
                              {perm.canDelete ? (
                                <FiCheck className="w-5 h-5" />
                              ) : (
                                <FiX className="w-5 h-5" />
                              )}
                            </div>
                            <p className="text-xs font-medium">Eliminar</p>
                          </button>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
};
