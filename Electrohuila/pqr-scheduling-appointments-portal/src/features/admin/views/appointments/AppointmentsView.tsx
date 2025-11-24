/**
 * @file AppointmentsView.tsx
 * @description Appointments management view with attendance tracking
 */

import React, { useState } from 'react';
import { FiCheckCircle, FiX, FiAlertCircle, FiClock } from 'react-icons/fi';
import { AppointmentDto } from '@/services/api';
import { StatusBadge } from '@/shared/components/StatusBadge';
import { PaginationControls } from '../components/shared';
import { getPaginatedData, formatDate, formatTime } from '../utils/tableUtils';

interface AppointmentsViewProps {
  appointments: AppointmentDto[];
  loading: boolean;
  onToggleAttendance: (appointment: AppointmentDto, attended: boolean) => Promise<void>;
}

export const AppointmentsView: React.FC<AppointmentsViewProps> = ({
  appointments,
  loading,
  onToggleAttendance
}) => {
  const [currentView, setCurrentView] = useState<'pending' | 'completed' | 'cancelled'>('completed');
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  // Filter appointments based on currentView
  const filteredAppointments = appointments.filter(apt => {
    const status = apt.status?.toUpperCase();
    
    if (currentView === 'completed') {
      // "Completadas" tab - show only COMPLETED appointments
      return status === 'COMPLETED' || status === 'COMPLETADA';
    } else if (currentView === 'cancelled') {
      // "No Asistidas" tab - show CANCELLED appointments only
      return status === 'CANCELLED' || status === 'CANCELADA' || status === 'CANCELED';
    } else {
      // "Pendientes" tab - show PENDING and CONFIRMED appointments only
      return status === 'PENDING' || status === 'PENDIENTE' || 
             status === 'CONFIRMED' || status === 'CONFIRMADA';
    }
  });

  const paginatedData = getPaginatedData(filteredAppointments, currentPage, itemsPerPage);

  return (
    <div className="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden">
      <div className="p-6">
        {/* Header with View Toggle */}
        <div className="flex justify-between items-center mb-6">
          <div className="flex bg-gray-100 rounded-lg p-1">
            <button
              onClick={() => {
                setCurrentView('completed');
                setCurrentPage(1);
              }}
              className={`px-4 py-2 rounded-md text-sm font-medium transition-all duration-200 ${
                currentView === 'completed'
                  ? 'bg-white text-[#1797D5] shadow-sm'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              <FiCheckCircle className="inline mr-1" />
              Completadas
            </button>
            <button
              onClick={() => {
                setCurrentView('cancelled');
                setCurrentPage(1);
              }}
              className={`px-4 py-2 rounded-md text-sm font-medium transition-all duration-200 ${
                currentView === 'cancelled'
                  ? 'bg-white text-red-600 shadow-sm'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              <FiX className="inline mr-1" />
              No Asistidas
            </button>
            <button
              onClick={() => {
                setCurrentView('pending');
                setCurrentPage(1);
              }}
              className={`px-4 py-2 rounded-md text-sm font-medium transition-all duration-200 ${
                currentView === 'pending'
                  ? 'bg-white text-yellow-600 shadow-sm'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              <FiClock className="inline mr-1" />
              Pendientes
            </button>
          </div>
        </div>

        {/* Info Banner */}
        <div className="bg-blue-50 border border-blue-200 rounded-xl p-4 mb-6">
          <div className="flex items-center">
            <FiAlertCircle className="w-5 h-5 text-blue-500 mr-2" />
            <span className="text-blue-800 font-medium">
              {currentView === 'pending'
                ? 'Mostrando citas pendientes y confirmadas. Use los botones de acción para cambiar el estado.'
                : currentView === 'completed'
                ? 'Mostrando citas completadas (asistidas). Estas citas ya fueron finalizadas.'
                : 'Mostrando citas no asistidas (canceladas). Estas citas ya fueron canceladas.'}
            </span>
          </div>
        </div>

        {/* Loading */}
        {loading && (
          <div className="text-center py-12">
            <div className="inline-flex items-center">
              <svg
                className="animate-spin -ml-1 mr-3 h-5 w-5 text-[#1797D5]"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  className="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  strokeWidth="4"
                ></circle>
                <path
                  className="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                ></path>
              </svg>
              Cargando datos...
            </div>
          </div>
        )}

        {/* Appointments Table */}
        {!loading && (
          <div className="overflow-x-auto">
            <table className="min-w-full bg-white">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Número
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Cliente
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Fecha
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Hora
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Estado
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Acciones
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {paginatedData.map((appointment) => (
                  <tr key={appointment.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {appointment.appointmentNumber}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {appointment.clientName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {formatDate(appointment.appointmentDate)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {formatTime(appointment.appointmentTime)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <StatusBadge
                        statusCode={appointment.status}
                        showIcon={false}
                        className="text-xs"
                      />
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                      {currentView === 'completed' ? (
                        <span className="text-gray-400 text-xs">
                          Cita finalizada
                        </span>
                      ) : currentView === 'cancelled' ? (
                        <span className="text-gray-400 text-xs">
                          Cita cancelada
                        </span>
                      ) : (
                        <div className="flex gap-2">
                          <button
                            onClick={() => onToggleAttendance(appointment, true)}
                            className="text-green-600 hover:text-green-900 transition-colors font-medium"
                          >
                            <FiCheckCircle className="inline mr-1" />
                            Marcar Asistida
                          </button>
                          <button
                            onClick={() => onToggleAttendance(appointment, false)}
                            className="text-red-600 hover:text-red-900 transition-colors"
                          >
                            <FiX className="inline mr-1" />
                            Marcar No Asistida
                          </button>
                        </div>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            <PaginationControls
              currentPage={currentPage}
              totalItems={filteredAppointments.length}
              itemsPerPage={itemsPerPage}
              onPageChange={setCurrentPage}
            />
          </div>
        )}
      </div>
    </div>
  );
};
