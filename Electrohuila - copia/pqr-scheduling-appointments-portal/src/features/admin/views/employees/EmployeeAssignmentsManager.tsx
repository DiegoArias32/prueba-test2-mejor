/**
 * EmployeeAssignmentsManager Component
 * Gestiona las asignaciones de empleados a tipos de cita
 */

'use client';

import React, { useState, useEffect } from 'react';
import {
  FiUser,
  FiPlus,
  FiX,
  FiChevronDown,
  FiChevronUp,
  FiTag,
  FiAlertCircle,
  FiCheck,
  FiLoader,
} from 'react-icons/fi';
import { apiService } from '@/services';
import type { UserDto } from '@/services/users/user.types';
import type { AppointmentTypeDto } from '@/services/catalogs/catalog.types';
import type { UserAssignmentDto, CreateAssignmentDto, BulkAssignmentDto } from '@/services/assignments/assignment.types';

export const EmployeeAssignmentsManager: React.FC = () => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [appointmentTypes, setAppointmentTypes] = useState<AppointmentTypeDto[]>([]);
  const [assignments, setAssignments] = useState<UserAssignmentDto[]>([]);
  const [expandedUserId, setExpandedUserId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  // Estado para agregar asignaciones
  const [addingToUserId, setAddingToUserId] = useState<number | null>(null);
  const [selectedTypes, setSelectedTypes] = useState<number[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      const [usersData, typesData, assignmentsData] = await Promise.all([
        apiService.getUsers(),
        apiService.getAppointmentTypes(),
        apiService.getAllAssignments(),
      ]);

      setUsers(usersData);
      setAppointmentTypes(typesData);
      setAssignments(assignmentsData);
    } catch (err) {
      setError('Error al cargar datos: ' + (err instanceof Error ? err.message : 'Error desconocido'));
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const getUserAssignments = (userId: number): UserAssignmentDto[] => {
    return assignments.filter(a => a.userId === userId && a.isActive);
  };

  const getAvailableTypesForUser = (userId: number): AppointmentTypeDto[] => {
    const userAssignments = getUserAssignments(userId);
    const assignedTypeIds = userAssignments.map(a => a.appointmentTypeId);
    return appointmentTypes.filter(type => !assignedTypeIds.includes(type.id));
  };

  const handleAddAssignment = async (userId: number) => {
    if (selectedTypes.length === 0) {
      setError('Selecciona al menos un tipo de cita');
      return;
    }

    try {
      setSaving(true);
      setError(null);

      if (selectedTypes.length === 1) {
        // Asignación simple
        const dto: CreateAssignmentDto = {
          userId,
          appointmentTypeId: selectedTypes[0],
        };
        await apiService.assignUserToAppointmentType(dto);
      } else {
        // Asignación múltiple
        const dto: BulkAssignmentDto = {
          userId,
          appointmentTypeIds: selectedTypes,
        };
        await apiService.bulkAssignUser(dto);
      }

      setSuccessMessage('Asignaciones creadas exitosamente');
      setSelectedTypes([]);
      setAddingToUserId(null);
      await loadData();

      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err) {
      setError('Error al crear asignación: ' + (err instanceof Error ? err.message : 'Error desconocido'));
    } finally {
      setSaving(false);
    }
  };

  const handleRemoveAssignment = async (assignmentId: number) => {
    if (!confirm('¿Estás seguro de eliminar esta asignación?')) return;

    try {
      setSaving(true);
      setError(null);

      await apiService.removeAssignment(assignmentId);
      setSuccessMessage('Asignación eliminada exitosamente');
      await loadData();

      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err) {
      setError('Error al eliminar asignación: ' + (err instanceof Error ? err.message : 'Error desconocido'));
    } finally {
      setSaving(false);
    }
  };

  const toggleUserExpanded = (userId: number) => {
    setExpandedUserId(expandedUserId === userId ? null : userId);
    setAddingToUserId(null);
    setSelectedTypes([]);
  };

  const toggleTypeSelection = (typeId: number) => {
    setSelectedTypes(prev =>
      prev.includes(typeId)
        ? prev.filter(id => id !== typeId)
        : [...prev, typeId]
    );
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-12">
        <FiLoader className="w-8 h-8 animate-spin text-blue-600" />
        <span className="ml-3 text-gray-600">Cargando...</span>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Asignación de Empleados a Tipos de Cita</h2>
        <p className="text-gray-600 mt-1">
          Gestiona qué tipos de cita puede ver y atender cada empleado
        </p>
      </div>

      {/* Success Message */}
      {successMessage && (
        <div className="bg-green-50 border border-green-200 rounded-lg p-4 flex items-center">
          <FiCheck className="w-5 h-5 text-green-600 mr-3" />
          <p className="text-green-800">{successMessage}</p>
        </div>
      )}

      {/* Error Message */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-center">
          <FiAlertCircle className="w-5 h-5 text-red-600 mr-3" />
          <p className="text-red-800">{error}</p>
        </div>
      )}

      {/* Employees List */}
      <div className="space-y-4">
        {users.map(user => {
          const userAssignments = getUserAssignments(user.id);
          const isExpanded = expandedUserId === user.id;
          const isAdding = addingToUserId === user.id;
          const availableTypes = getAvailableTypesForUser(user.id);

          return (
            <div key={user.id} className="bg-white rounded-lg shadow border border-gray-200">
              {/* User Header */}
              <div
                className="p-4 cursor-pointer hover:bg-gray-50 transition-colors flex items-center justify-between"
                onClick={() => toggleUserExpanded(user.id)}
              >
                <div className="flex items-center space-x-4">
                  <div className="bg-blue-100 rounded-full p-2">
                    <FiUser className="w-5 h-5 text-blue-600" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900">{user.username}</h3>
                    <p className="text-sm text-gray-600">{user.email}</p>
                  </div>
                </div>

                <div className="flex items-center space-x-4">
                  <span className="text-sm text-gray-600">
                    {userAssignments.length} tipo{userAssignments.length !== 1 ? 's' : ''} asignado{userAssignments.length !== 1 ? 's' : ''}
                  </span>
                  {isExpanded ? (
                    <FiChevronUp className="w-5 h-5 text-gray-400" />
                  ) : (
                    <FiChevronDown className="w-5 h-5 text-gray-400" />
                  )}
                </div>
              </div>

              {/* Expanded Content */}
              {isExpanded && (
                <div className="border-t border-gray-200 p-4 space-y-4">
                  {/* Current Assignments */}
                  {userAssignments.length > 0 ? (
                    <div>
                      <h4 className="text-sm font-semibold text-gray-700 mb-3">Tipos de Cita Asignados:</h4>
                      <div className="flex flex-wrap gap-2">
                        {userAssignments.map(assignment => (
                          <div
                            key={assignment.id}
                            className="inline-flex items-center bg-blue-50 border border-blue-200 rounded-full px-3 py-1.5 text-sm group"
                          >
                            <FiTag
                              className="w-4 h-4 mr-2"
                              style={{ color: assignment.appointmentTypeColor || '#3B82F6' }}
                            />
                            <span className="text-gray-800">{assignment.appointmentTypeName}</span>
                            <button
                              onClick={() => handleRemoveAssignment(assignment.id)}
                              disabled={saving}
                              className="ml-2 text-red-500 hover:text-red-700 disabled:opacity-50"
                            >
                              <FiX className="w-4 h-4" />
                            </button>
                          </div>
                        ))}
                      </div>
                    </div>
                  ) : (
                    <p className="text-sm text-gray-500 italic">No tiene tipos de cita asignados</p>
                  )}

                  {/* Add Button */}
                  {!isAdding && availableTypes.length > 0 && (
                    <button
                      onClick={() => setAddingToUserId(user.id)}
                      className="inline-flex items-center px-4 py-2 border border-blue-600 rounded-lg text-blue-600 hover:bg-blue-50 transition-colors text-sm font-medium"
                    >
                      <FiPlus className="w-4 h-4 mr-2" />
                      Agregar Tipo de Cita
                    </button>
                  )}

                  {/* Add Form */}
                  {isAdding && (
                    <div className="bg-gray-50 rounded-lg p-4 space-y-4">
                      <h4 className="text-sm font-semibold text-gray-700">Seleccionar Tipos de Cita:</h4>
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-2">
                        {availableTypes.map(type => (
                          <label
                            key={type.id}
                            className="flex items-center space-x-3 p-3 bg-white border border-gray-200 rounded-lg cursor-pointer hover:bg-gray-50 transition-colors"
                          >
                            <input
                              type="checkbox"
                              checked={selectedTypes.includes(type.id)}
                              onChange={() => toggleTypeSelection(type.id)}
                              className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
                            />
                            <div className="flex items-center space-x-2 flex-1">
                              <FiTag
                                className="w-4 h-4"
                                style={{ color: type.colorPrimary || '#3B82F6' }}
                              />
                              <span className="text-sm text-gray-800">{type.name}</span>
                            </div>
                          </label>
                        ))}
                      </div>

                      <div className="flex space-x-3">
                        <button
                          onClick={() => handleAddAssignment(user.id)}
                          disabled={saving || selectedTypes.length === 0}
                          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed text-sm font-medium"
                        >
                          {saving ? 'Guardando...' : `Asignar ${selectedTypes.length} tipo${selectedTypes.length !== 1 ? 's' : ''}`}
                        </button>
                        <button
                          onClick={() => {
                            setAddingToUserId(null);
                            setSelectedTypes([]);
                          }}
                          disabled={saving}
                          className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 disabled:opacity-50 text-sm font-medium"
                        >
                          Cancelar
                        </button>
                      </div>
                    </div>
                  )}

                  {availableTypes.length === 0 && !isAdding && (
                    <p className="text-sm text-gray-500 italic">
                      Todos los tipos de cita han sido asignados a este usuario
                    </p>
                  )}
                </div>
              )}
            </div>
          );
        })}
      </div>

      {users.length === 0 && (
        <div className="text-center py-12">
          <FiUser className="w-12 h-12 text-gray-400 mx-auto mb-3" />
          <p className="text-gray-600">No hay empleados registrados</p>
        </div>
      )}
    </div>
  );
};
