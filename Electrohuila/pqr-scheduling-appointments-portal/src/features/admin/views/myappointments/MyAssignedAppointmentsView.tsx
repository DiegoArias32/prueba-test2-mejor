/**
 * MyAssignedAppointmentsView Component
 * Vista mejorada de citas asignadas al empleado actual
 * Utiliza el nuevo endpoint que retorna datos completos con JOINs
 */

'use client';

import React, { useState, useEffect } from 'react';
import {
  FiCalendar,
  FiClock,
  FiMapPin,
  FiUser,
  FiFileText,
  FiCheckCircle,
  FiXCircle,
  FiAlertCircle,
  FiRefreshCw,
  FiFilter,
  FiTag,
  FiLoader,
} from 'react-icons/fi';
import { apiService, type AppointmentDetailDto } from '@/services';

export const MyAssignedAppointmentsView: React.FC = () => {
  const [appointments, setAppointments] = useState<AppointmentDetailDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filterStatus, setFilterStatus] = useState<'all' | 'pending' | 'completed' | 'cancelled'>('all');
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadAppointments();
  }, []);

  const loadAppointments = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await apiService.getMyAssignedAppointments();
      setAppointments(data);
    } catch (err) {
      setError('Error al cargar citas: ' + (err instanceof Error ? err.message : 'Error desconocido'));
      console.error('Error loading appointments:', err);
    } finally {
      setLoading(false);
    }
  };

  const formatTime = (timeString?: string) => {
    if (!timeString) return 'Sin hora';
    if (timeString.includes(':')) {
      const [hours, minutes] = timeString.split(':');
      const hour24 = parseInt(hours);
      const ampm = hour24 >= 12 ? 'PM' : 'AM';
      const hour12 = hour24 % 12 || 12;
      return `${hour12}:${minutes} ${ampm}`;
    }
    return timeString;
  };

  const formatDate = (dateString: string) => {
    if (!dateString) return 'Sin fecha';
    return new Date(dateString).toLocaleDateString('es-ES', {
      weekday: 'short',
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  };

  const getStatusIcon = (status: string) => {
    const statusLower = status.toLowerCase();
    if (statusLower.includes('complet')) return <FiCheckCircle className="w-5 h-5 text-green-500" />;
    if (statusLower.includes('cancel')) return <FiXCircle className="w-5 h-5 text-red-500" />;
    if (statusLower.includes('pend') || statusLower.includes('confirm')) return <FiClock className="w-5 h-5 text-yellow-500" />;
    return <FiAlertCircle className="w-5 h-5 text-blue-500" />;
  };

  const getStatusColor = (status: string) => {
    const statusLower = status.toLowerCase();
    if (statusLower.includes('complet')) return 'bg-green-100 text-green-800 border-green-200';
    if (statusLower.includes('cancel')) return 'bg-red-100 text-red-800 border-red-200';
    if (statusLower.includes('pend') || statusLower.includes('confirm')) return 'bg-yellow-100 text-yellow-800 border-yellow-200';
    return 'bg-blue-100 text-blue-800 border-blue-200';
  };

  const filteredAppointments = appointments
    .filter(apt => {
      if (filterStatus === 'all') return true;
      const statusLower = apt.status.toLowerCase();
      if (filterStatus === 'pending') return statusLower.includes('pend') || statusLower.includes('confirm');
      if (filterStatus === 'completed') return statusLower.includes('complet');
      if (filterStatus === 'cancelled') return statusLower.includes('cancel');
      return false;
    })
    .filter(apt => {
      if (!searchTerm) return true;
      const search = searchTerm.toLowerCase();
      return (
        apt.appointmentNumber.toLowerCase().includes(search) ||
        apt.client.fullName.toLowerCase().includes(search) ||
        apt.client.clientNumber?.toLowerCase().includes(search) ||
        apt.client.documentNumber.toLowerCase().includes(search)
      );
    });

  if (loading) {
    return (
      <div className="flex justify-center items-center py-12">
        <FiLoader className="w-8 h-8 animate-spin text-blue-600" />
        <span className="ml-3 text-gray-600">Cargando mis citas...</span>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Mis Citas Asignadas</h2>
          <p className="text-gray-600 mt-1">
            Citas filtradas según tus tipos asignados ({appointments.length} total)
          </p>
        </div>
        <button
          onClick={loadAppointments}
          disabled={loading}
          className="inline-flex items-center px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 disabled:opacity-50"
        >
          <FiRefreshCw className={`w-4 h-4 mr-2 ${loading ? 'animate-spin' : ''}`} />
          Actualizar
        </button>
      </div>

      {/* Error Message */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-center">
          <FiAlertCircle className="w-5 h-5 text-red-600 mr-3" />
          <p className="text-red-800">{error}</p>
        </div>
      )}

      {/* Filters */}
      <div className="bg-white rounded-lg shadow p-4 space-y-4">
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Buscar
            </label>
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Buscar por número, cliente..."
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <div className="sm:w-48">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Filtrar por Estado
            </label>
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value as any)}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="all">Todos</option>
              <option value="pending">Pendiente</option>
              <option value="completed">Completado</option>
              <option value="cancelled">Cancelado</option>
            </select>
          </div>
        </div>
      </div>

      {/* Appointments List */}
      {filteredAppointments.length === 0 ? (
        <div className="bg-white rounded-lg shadow p-12 text-center">
          <FiCalendar className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 mb-2">
            {appointments.length === 0 ? 'No tienes citas asignadas' : 'No hay citas que coincidan con el filtro'}
          </h3>
          <p className="text-gray-600">
            {appointments.length === 0
              ? 'Contacta a tu administrador para asignarte tipos de cita'
              : 'Intenta cambiar los filtros de búsqueda'}
          </p>
        </div>
      ) : (
        <div className="space-y-4">
          {filteredAppointments.map((apt) => (
            <div key={apt.id} className="bg-white rounded-lg shadow border border-gray-200 p-6 hover:shadow-lg transition-shadow">
              <div className="flex items-start justify-between mb-4">
                <div className="flex items-center space-x-3">
                  <div className="bg-blue-100 rounded-full p-2">
                    <FiCalendar className="w-5 h-5 text-blue-600" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900">{apt.appointmentNumber}</h3>
                    <p className="text-sm text-gray-600">{formatDate(apt.appointmentDate)}</p>
                  </div>
                </div>
                <div className={`inline-flex items-center px-3 py-1 rounded-full border ${getStatusColor(apt.status)}`}>
                  {getStatusIcon(apt.status)}
                  <span className="ml-2 text-sm font-medium">{apt.status}</span>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {/* Cliente */}
                <div className="flex items-start space-x-3">
                  <FiUser className="w-5 h-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm text-gray-600">Cliente</p>
                    <p className="font-medium text-gray-900">{apt.client.fullName}</p>
                    {apt.client.clientNumber && (
                      <p className="text-sm text-gray-600">Nro: {apt.client.clientNumber}</p>
                    )}
                    {!apt.client.clientNumber && (
                      <p className="text-sm text-gray-600">Doc: {apt.client.documentNumber}</p>
                    )}
                  </div>
                </div>

                {/* Hora */}
                <div className="flex items-start space-x-3">
                  <FiClock className="w-5 h-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm text-gray-600">Hora</p>
                    <p className="font-medium text-gray-900">{formatTime(apt.appointmentTime)}</p>
                  </div>
                </div>

                {/* Sede */}
                <div className="flex items-start space-x-3">
                  <FiMapPin className="w-5 h-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm text-gray-600">Sede</p>
                    <p className="font-medium text-gray-900">{apt.branch.name}</p>
                    {apt.branch.city && (
                      <p className="text-sm text-gray-600">{apt.branch.city}</p>
                    )}
                  </div>
                </div>

                {/* Tipo de Cita */}
                <div className="flex items-start space-x-3">
                  <FiTag className="w-5 h-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm text-gray-600">Tipo de Cita</p>
                    <div className="flex items-center space-x-2">
                      <div
                        className="w-3 h-3 rounded-full"
                        style={{ backgroundColor: apt.appointmentType.colorPrimary || '#3B82F6' }}
                      />
                      <p className="font-medium text-gray-900">{apt.appointmentType.name}</p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Notas */}
              {apt.notes && (
                <div className="mt-4 pt-4 border-t border-gray-200">
                  <div className="flex items-start space-x-3">
                    <FiFileText className="w-5 h-5 text-gray-400 mt-0.5" />
                    <div className="flex-1">
                      <p className="text-sm text-gray-600">Notas</p>
                      <p className="text-gray-900">{apt.notes}</p>
                    </div>
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
