/**
 * MyAppointments Component
 * Vista de citas asignadas al empleado actual con notificaciones en tiempo real
 */

'use client';

import React, { useState } from 'react';
import {
  FiCalendar,
  FiClock,
  FiMapPin,
  FiUser,
  FiFileText,
  FiCheckCircle,
  FiXCircle,
  FiAlertCircle,
  FiBell,
  FiFilter
} from 'react-icons/fi';

interface AppointmentItem {
  id: number;
  appointmentNumber: string;
  clientId: number;
  clientNumber?: string;
  clientName?: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string | null;
  observations?: string;
  branchId: number;
  branchName?: string;
  appointmentTypeId: number;
  appointmentTypeName?: string;
  createdAt: string;
  assignedAt?: string;
}

interface MyAppointmentsProps {
  appointments: AppointmentItem[];
  loading: boolean;
  onMarkCompleted: (id: number) => void;
  onMarkNoShow: (id: number) => void;
  onRefresh: () => void;
  lastRefresh: Date;
}

export const MyAppointments: React.FC<MyAppointmentsProps> = ({
  appointments,
  loading,
  onMarkCompleted,
  onMarkNoShow,
  onRefresh,
  lastRefresh
}) => {
  const [filterStatus, setFilterStatus] = useState<'all' | 'scheduled' | 'completed' | 'no-show'>('all');
  const [searchTerm, setSearchTerm] = useState('');
  
  // Pagination states
  const [todayPage, setTodayPage] = useState(1);
  const [upcomingPage, setUpcomingPage] = useState(1);
  const [pastPage, setPastPage] = useState(1);
  const itemsPerPage = 5;

  const formatTime = (timeString: string) => {
    if (!timeString) return '';
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
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('en-US', {
      weekday: 'short',
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  };

  const getStatusIcon = (status: string | null | undefined) => {
    const statusLower = status?.toLowerCase() || '';
    switch (statusLower) {
      case 'completed':
        return <FiCheckCircle className="w-5 h-5 text-green-500" />;
      case 'scheduled':
      case 'pending':
        return <FiClock className="w-5 h-5 text-yellow-500" />;
      case 'cancelled':
      case 'no show':
        return <FiXCircle className="w-5 h-5 text-red-500" />;
      default:
        return <FiAlertCircle className="w-5 h-5 text-blue-500" />;
    }
  };

  const getStatusColor = (status: string | null | undefined) => {
    const statusLower = status?.toLowerCase() || '';
    switch (statusLower) {
      case 'completed':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'scheduled':
      case 'pending':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'cancelled':
        return 'bg-red-100 text-red-800 border-red-200';
      case 'no show':
        return 'bg-gray-100 text-gray-800 border-gray-200';
      default:
        return 'bg-blue-100 text-blue-800 border-blue-200';
    }
  };

  const isToday = (dateString: string) => {
    const today = new Date();
    const appointmentDate = new Date(dateString);
    return (
      appointmentDate.getDate() === today.getDate() &&
      appointmentDate.getMonth() === today.getMonth() &&
      appointmentDate.getFullYear() === today.getFullYear()
    );
  };

  const filteredAppointments = appointments
    .filter(apt => {
      if (filterStatus === 'all') return true;
      if (filterStatus === 'no-show') return apt.status?.toLowerCase() === 'no show';
      return apt.status?.toLowerCase() === filterStatus;
    })
    .filter(apt => {
      if (!searchTerm) return true;
      const search = searchTerm.toLowerCase();
      return (
        apt.appointmentNumber?.toLowerCase().includes(search) ||
        apt.clientName?.toLowerCase().includes(search) ||
        apt.clientNumber?.toLowerCase().includes(search)
      );
    })
    .sort((a, b) => {
      const dateA = new Date(a.appointmentDate + ' ' + a.appointmentTime);
      const dateB = new Date(b.appointmentDate + ' ' + b.appointmentTime);
      return dateA.getTime() - dateB.getTime();
    });

  const todayAppointments = filteredAppointments.filter(apt => isToday(apt.appointmentDate));
  const upcomingAppointments = filteredAppointments.filter(apt => !isToday(apt.appointmentDate) && new Date(apt.appointmentDate) > new Date());
  const pastAppointments = filteredAppointments.filter(apt => new Date(apt.appointmentDate) < new Date() && !isToday(apt.appointmentDate));

  // Pagination helpers
  const paginate = (items: AppointmentItem[], page: number) => {
    const startIndex = (page - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return items.slice(startIndex, endIndex);
  };

  const getTotalPages = (totalItems: number) => Math.ceil(totalItems / itemsPerPage);

  // Paginated data
  const paginatedTodayAppointments = paginate(todayAppointments, todayPage);
  const paginatedUpcomingAppointments = paginate(upcomingAppointments, upcomingPage);
  const paginatedPastAppointments = paginate(pastAppointments, pastPage);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-orange-500 to-yellow-500 rounded-2xl p-8 text-white shadow-xl">
        <div className="flex items-center justify-between">
          <div>
            <div className="flex items-center space-x-3 mb-2">
              <FiBell className="w-8 h-8" />
              <h1 className="text-3xl font-bold">Mis Citas</h1>
            </div>
            <p className="text-orange-100 text-lg">
              Citas asignadas a ti • {appointments.length} total
            </p>
          </div>
          <button
            onClick={onRefresh}
            disabled={loading}
            className="px-6 py-3 bg-white/20 hover:bg-white/30 rounded-xl backdrop-blur-sm transition-all disabled:opacity-50"
          >
            <div className="flex items-center space-x-2">
              <FiAlertCircle className={`w-5 h-5 ${loading ? 'animate-spin' : ''}`} />
              <span className="font-medium">Actualizar</span>
            </div>
          </button>
        </div>

        {/* Last Refresh */}
        <div className="mt-4 text-sm text-orange-100">
          Última actualización: {lastRefresh.toLocaleTimeString()}
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-2xl p-6 shadow-lg border border-gray-100">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              <FiFilter className="inline w-4 h-4 mr-1" />
              Filtrar por Estado
            </label>
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value as 'all' | 'scheduled' | 'completed' | 'no-show')}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-transparent"
            >
              <option value="all">Todas las Citas</option>
              <option value="scheduled">Programada</option>
              <option value="completed">Completada</option>
              <option value="no-show">No Asistió</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Buscar
            </label>
            <input
              type="text"
              placeholder="Buscar por cita #, cliente..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-transparent"
            />
          </div>
        </div>
      </div>

      {/* Today's Appointments */}
      {todayAppointments.length > 0 && (
        <div className="space-y-4">
          <h2 className="text-2xl font-bold text-gray-900 flex items-center">
            <FiCalendar className="w-6 h-6 mr-2 text-orange-500" />
            Citas de Hoy ({todayAppointments.length})
          </h2>
          <div className="grid grid-cols-1 gap-4">
            {paginatedTodayAppointments.map((appointment) => (
              <AppointmentCard
                key={appointment.id}
                appointment={appointment}
                onMarkCompleted={onMarkCompleted}
                onMarkNoShow={onMarkNoShow}
                formatTime={formatTime}
                formatDate={formatDate}
                getStatusIcon={getStatusIcon}
                getStatusColor={getStatusColor}
                isHighlighted={true}
              />
            ))}
          </div>
          <PaginationControls
            currentPage={todayPage}
            totalPages={getTotalPages(todayAppointments.length)}
            totalItems={todayAppointments.length}
            itemsPerPage={itemsPerPage}
            onPageChange={setTodayPage}
          />
        </div>
      )}

      {/* Upcoming Appointments */}
      {upcomingAppointments.length > 0 && (
        <div className="space-y-4">
          <h2 className="text-2xl font-bold text-gray-900 flex items-center">
            <FiClock className="w-6 h-6 mr-2 text-blue-500" />
            Próximas Citas ({upcomingAppointments.length})
          </h2>
          <div className="grid grid-cols-1 gap-4">
            {paginatedUpcomingAppointments.map((appointment) => (
              <AppointmentCard
                key={appointment.id}
                appointment={appointment}
                onMarkCompleted={onMarkCompleted}
                onMarkNoShow={onMarkNoShow}
                formatTime={formatTime}
                formatDate={formatDate}
                getStatusIcon={getStatusIcon}
                getStatusColor={getStatusColor}
              />
            ))}
          </div>
          <PaginationControls
            currentPage={upcomingPage}
            totalPages={getTotalPages(upcomingAppointments.length)}
            totalItems={upcomingAppointments.length}
            itemsPerPage={itemsPerPage}
            onPageChange={setUpcomingPage}
          />
        </div>
      )}

      {/* Past Appointments */}
      {pastAppointments.length > 0 && (
        <div className="space-y-4">
          <h2 className="text-2xl font-bold text-gray-900 flex items-center">
            <FiCheckCircle className="w-6 h-6 mr-2 text-gray-400" />
            Citas Pasadas ({pastAppointments.length})
          </h2>
          <div className="grid grid-cols-1 gap-4">
            {paginatedPastAppointments.map((appointment) => (
              <AppointmentCard
                key={appointment.id}
                appointment={appointment}
                onMarkCompleted={onMarkCompleted}
                onMarkNoShow={onMarkNoShow}
                formatTime={formatTime}
                formatDate={formatDate}
                getStatusIcon={getStatusIcon}
                getStatusColor={getStatusColor}
                isPast={true}
              />
            ))}
          </div>
          <PaginationControls
            currentPage={pastPage}
            totalPages={getTotalPages(pastAppointments.length)}
            totalItems={pastAppointments.length}
            itemsPerPage={itemsPerPage}
            onPageChange={setPastPage}
          />
        </div>
      )}

      {/* Empty State */}
      {filteredAppointments.length === 0 && (
        <div className="bg-white rounded-2xl p-12 text-center shadow-lg border border-gray-100">
          <FiCalendar className="w-20 h-20 mx-auto text-gray-300 mb-4" />
          <h3 className="text-xl font-semibold text-gray-900 mb-2">No se encontraron citas</h3>
          <p className="text-gray-500">
            {searchTerm || filterStatus !== 'all'
              ? 'Intenta ajustar tus filtros'
              : 'Aún no tienes citas asignadas'}
          </p>
        </div>
      )}
    </div>
  );
};

// Pagination Controls Component
interface PaginationControlsProps {
  currentPage: number;
  totalPages: number;
  totalItems: number;
  itemsPerPage: number;
  onPageChange: (page: number) => void;
}

const PaginationControls: React.FC<PaginationControlsProps> = ({
  currentPage,
  totalPages,
  totalItems,
  itemsPerPage,
  onPageChange
}) => {
  if (totalPages <= 1) return null;

  const startIndex = (currentPage - 1) * itemsPerPage + 1;
  const endIndex = Math.min(currentPage * itemsPerPage, totalItems);

  return (
    <div className="flex items-center justify-between mt-6 px-4 py-3 bg-gray-50 rounded-lg">
      <div className="text-sm text-gray-700">
        Mostrando {startIndex} - {endIndex} de {totalItems} citas
      </div>

      <div className="flex items-center space-x-2">
        <button
          onClick={() => onPageChange(currentPage - 1)}
          disabled={currentPage === 1}
          className="px-3 py-1 text-sm bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          Anterior
        </button>

        {Array.from({ length: Math.min(totalPages, 5) }, (_, i) => {
          let page;
          if (totalPages <= 5) {
            page = i + 1;
          } else if (currentPage <= 3) {
            page = i + 1;
          } else if (currentPage >= totalPages - 2) {
            page = totalPages - 4 + i;
          } else {
            page = currentPage - 2 + i;
          }

          return (
            <button
              key={page}
              onClick={() => onPageChange(page)}
              className={`px-3 py-1 text-sm rounded-md transition-colors ${
                page === currentPage
                  ? 'bg-orange-500 text-white'
                  : 'bg-white border border-gray-300 hover:bg-gray-50'
              }`}
            >
              {page}
            </button>
          );
        })}

        <button
          onClick={() => onPageChange(currentPage + 1)}
          disabled={currentPage === totalPages}
          className="px-3 py-1 text-sm bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          Siguiente
        </button>
      </div>
    </div>
  );
};

// AppointmentCard sub-component
interface AppointmentCardProps {
  appointment: AppointmentItem;
  onMarkCompleted: (id: number) => void;
  onMarkNoShow: (id: number) => void;
  formatTime: (time: string) => string;
  formatDate: (date: string) => string;
  getStatusIcon: (status: string | null | undefined) => React.ReactNode;
  getStatusColor: (status: string | null | undefined) => string;
  isHighlighted?: boolean;
  isPast?: boolean;
}

const AppointmentCard: React.FC<AppointmentCardProps> = ({
  appointment,
  onMarkCompleted,
  onMarkNoShow,
  formatTime,
  formatDate,
  getStatusIcon,
  getStatusColor,
  isHighlighted = false,
  isPast = false
}) => {
  const canMarkStatus = appointment.status?.toLowerCase() === 'scheduled' || appointment.status?.toLowerCase() === 'pending';

  return (
    <div className={`
      bg-white rounded-xl p-6 shadow-lg hover:shadow-xl transition-all duration-300
      border-2
      ${isHighlighted ? 'border-orange-400 ring-4 ring-orange-100' : 'border-gray-200'}
      ${isPast ? 'opacity-75' : ''}
    `}>
      <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between space-y-4 lg:space-y-0">
        {/* Left Section */}
        <div className="flex-1 space-y-3">
          <div className="flex items-center space-x-3">
            <span className="text-lg font-mono font-bold text-gray-900">
              {appointment.appointmentNumber}
            </span>
            <span className={`px-3 py-1 rounded-full text-xs font-semibold border ${getStatusColor(appointment.status)}`}>
              <span className="flex items-center space-x-1">
                {getStatusIcon(appointment.status)}
                <span>{appointment.status || 'Sin estado'}</span>
              </span>
            </span>
            {isHighlighted && (
              <span className="px-3 py-1 bg-orange-500 text-white rounded-full text-xs font-bold animate-pulse">
                HOY
              </span>
            )}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <div className="flex items-center space-x-2 text-gray-700">
              <FiUser className="w-5 h-5 text-gray-400" />
              <div>
                <p className="text-sm font-medium">{appointment.clientName || 'Cliente Desconocido'}</p>
                <p className="text-xs text-gray-500">{appointment.clientNumber || 'Sin número de cliente'}</p>
              </div>
            </div>

            <div className="flex items-center space-x-2 text-gray-700">
              <FiCalendar className="w-5 h-5 text-gray-400" />
              <div>
                <p className="text-sm font-medium">{formatDate(appointment.appointmentDate)}</p>
                <p className="text-xs text-gray-500">{formatTime(appointment.appointmentTime)}</p>
              </div>
            </div>

            <div className="flex items-center space-x-2 text-gray-700">
              <FiMapPin className="w-5 h-5 text-gray-400" />
              <span className="text-sm">{appointment.branchName || 'Sede Desconocida'}</span>
            </div>

            <div className="flex items-center space-x-2 text-gray-700">
              <FiFileText className="w-5 h-5 text-gray-400" />
              <span className="text-sm">{appointment.appointmentTypeName || 'Tipo Desconocido'}</span>
            </div>
          </div>

          {appointment.observations && (
            <div className="mt-2 p-3 bg-gray-50 rounded-lg">
              <p className="text-sm text-gray-600">
                <span className="font-semibold">Notas:</span> {appointment.observations}
              </p>
            </div>
          )}
        </div>

        {/* Right Section - Actions */}
        {canMarkStatus && !isPast && (
          <div className="flex flex-col space-y-2 lg:ml-6">
            <button
              onClick={() => onMarkCompleted(appointment.id)}
              className="px-6 py-3 bg-green-500 hover:bg-green-600 text-white rounded-lg font-medium transition-all flex items-center justify-center space-x-2 shadow-md hover:shadow-lg"
            >
              <FiCheckCircle className="w-5 h-5" />
              <span>Marcar como Completada</span>
            </button>
            <button
              onClick={() => onMarkNoShow(appointment.id)}
              className="px-6 py-3 bg-gray-500 hover:bg-gray-600 text-white rounded-lg font-medium transition-all flex items-center justify-center space-x-2 shadow-md hover:shadow-lg"
            >
              <FiXCircle className="w-5 h-5" />
              <span>Marcar como No Asistió</span>
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
