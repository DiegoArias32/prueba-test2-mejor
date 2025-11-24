/**
 * Dashboard Component
 * Vista principal del panel administrativo con estadísticas y widgets
 */

'use client';

import React from 'react';
import {
  FiCalendar,
  FiUsers,
  FiCheckCircle,
  FiClock,
  FiTrendingUp,
  FiMapPin,
  FiAlertCircle,
  FiBell
} from 'react-icons/fi';

interface DashboardStats {
  totalAppointments: number;
  completedAppointments: number;
  pendingAppointments: number;
  totalEmployees: number;
  totalBranches: number;
  totalAppointmentTypes: number;
  myAppointmentsToday: number;
  myPendingAppointments: number;
  myCompletedAppointments: number;
}

interface DashboardProps {
  stats: DashboardStats;
  recentAppointments: Array<{
    id: number;
    appointmentNumber: string;
    clientName: string;
    appointmentDate: string;
    appointmentTime: string;
    status: string;
    branch: string;
    appointmentType: string;
  }>;
  currentUser: {
    username: string;
  } | null;
  onNavigate: (section: string) => void;
}

export const Dashboard: React.FC<DashboardProps> = ({
  stats,
  recentAppointments,
  currentUser,
  onNavigate
}) => {
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
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'completed':
        return 'bg-green-100 text-green-800';
      case 'scheduled':
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'cancelled':
        return 'bg-red-100 text-red-800';
      case 'no show':
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-blue-100 text-blue-800';
    }
  };

  const statCards = [
    {
      title: 'Total de Citas',
      value: stats.totalAppointments,
      icon: FiCalendar,
      color: 'bg-blue-500',
      bgLight: 'bg-blue-50',
      textColor: 'text-blue-600',
      trend: '+12%',
      onClick: () => onNavigate('citas')
    },
    {
      title: 'Completadas',
      value: stats.completedAppointments,
      icon: FiCheckCircle,
      color: 'bg-green-500',
      bgLight: 'bg-green-50',
      textColor: 'text-green-600',
      trend: '+8%',
      onClick: () => onNavigate('citas')
    },
    {
      title: 'Pendientes',
      value: stats.pendingAppointments,
      icon: FiClock,
      color: 'bg-yellow-500',
      bgLight: 'bg-yellow-50',
      textColor: 'text-yellow-600',
      trend: '+5%',
      onClick: () => onNavigate('citas')
    },
    {
      title: 'Empleados',
      value: stats.totalEmployees,
      icon: FiUsers,
      color: 'bg-purple-500',
      bgLight: 'bg-purple-50',
      textColor: 'text-purple-600',
      trend: '+2',
      onClick: () => onNavigate('empleados')
    },
    {
      title: 'Sedes',
      value: stats.totalBranches,
      icon: FiMapPin,
      color: 'bg-red-500',
      bgLight: 'bg-red-50',
      textColor: 'text-red-600',
      trend: '0',
      onClick: () => onNavigate('sedes')
    },
    {
      title: 'Tipos de Cita',
      value: stats.totalAppointmentTypes,
      icon: FiAlertCircle,
      color: 'bg-indigo-500',
      bgLight: 'bg-indigo-50',
      textColor: 'text-indigo-600',
      trend: '+1',
      onClick: () => onNavigate('tipos-cita')
    }
  ];

  return (
    <div className="space-y-6">
      {/* Welcome Section */}
      <div className="bg-gradient-to-r from-[#1797D5] to-[#56C2E1] rounded-2xl p-8 text-white shadow-xl">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold mb-2">
              Bienvenido, {currentUser?.username || 'Admin'}
            </h1>
            <p className="text-blue-100 text-lg">
              Resumen de tu actividad y citas asignadas para hoy.
            </p>
          </div>
          <div className="hidden md:block">
            <div className="w-24 h-24 bg-white/20 rounded-full flex items-center justify-center backdrop-blur-sm">
              <FiTrendingUp className="w-12 h-12" />
            </div>
          </div>
        </div>

        {/* My Appointments Today */}
        <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4 cursor-pointer hover:bg-white/30 transition-all" onClick={() => onNavigate('my-appointments')}>
            <div className="flex items-center space-x-3">
              <div className="w-12 h-12 bg-orange-500 rounded-lg flex items-center justify-center">
                <FiBell className="w-6 h-6 text-white" />
              </div>
              <div>
                <p className="text-sm text-blue-100">Citas Asignadas Hoy</p>
                <p className="text-2xl font-bold">{stats.myAppointmentsToday}</p>
              </div>
            </div>
          </div>
          <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4 cursor-pointer hover:bg-white/30 transition-all" onClick={() => onNavigate('my-appointments')}>
            <div className="flex items-center space-x-3">
              <div className="w-12 h-12 bg-yellow-500 rounded-lg flex items-center justify-center">
                <FiClock className="w-6 h-6 text-white" />
              </div>
              <div>
                <p className="text-sm text-blue-100">Pendientes de Atender</p>
                <p className="text-2xl font-bold">{stats.myPendingAppointments}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Stats Grid - Only show for admin users */}
      {currentUser?.username === 'admin' && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {statCards.map((card, index) => {
            const Icon = card.icon;
            return (
              <div
                key={index}
                onClick={card.onClick}
                className="bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer group border border-gray-100"
              >
                <div className="flex items-center justify-between mb-4">
                  <div className={`w-14 h-14 ${card.bgLight} rounded-xl flex items-center justify-center group-hover:scale-110 transition-transform duration-300`}>
                  <Icon className={`w-7 h-7 ${card.textColor}`} />
                </div>
                <span className="text-sm font-semibold text-green-600 bg-green-50 px-3 py-1 rounded-full">
                  {card.trend}
                </span>
              </div>
              <h3 className="text-gray-600 text-sm font-medium mb-1">{card.title}</h3>
              <p className="text-3xl font-bold text-gray-900">{card.value}</p>
            </div>
          );
        })}
        </div>
      )}

      {/* Recent Appointments - Show for all users */}
      <div className="bg-white rounded-2xl shadow-lg overflow-hidden border border-gray-100">
        <div className="p-6 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-xl font-bold text-gray-900">Citas Recientes</h2>
            <button
              onClick={() => onNavigate('citas')}
              className="text-[#1797D5] hover:text-[#203461] font-medium text-sm transition-colors"
            >
              Ver Todas →
            </button>
          </div>
        </div>
        <div className="overflow-x-auto">
          {recentAppointments.length === 0 ? (
            <div className="p-12 text-center">
              <FiCalendar className="w-16 h-16 mx-auto text-gray-300 mb-4" />
              <p className="text-gray-500">No hay citas recientes</p>
            </div>
          ) : (
            <table className="w-full">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Cita #
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Cliente
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Fecha y Hora
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Sede
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Tipo
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Estado
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {recentAppointments.slice(0, 5).map((appointment) => (
                  <tr key={appointment.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="text-sm font-mono font-semibold text-gray-900">
                        {appointment.appointmentNumber}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="text-sm text-gray-900">{appointment.clientName}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm">
                        <div className="text-gray-900 font-medium">
                          {formatDate(appointment.appointmentDate)}
                        </div>
                        <div className="text-gray-500">{formatTime(appointment.appointmentTime)}</div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {appointment.branch}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {appointment.appointmentType}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusColor(appointment.status)}`}>
                        {appointment.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
};
