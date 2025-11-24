/**
 * Reusable Appointment Card Component
 */

'use client';

import React from 'react';
import { AppointmentDto } from '@/services/api';
import { StatusBadge } from './StatusBadge';

interface AppointmentCardProps {
  appointment: AppointmentDto;
  isExpanded: boolean;
  onToggleExpand: (id: number) => void;
  onCancel?: (appointment: AppointmentDto) => void;
  showCancelButton?: boolean;
}

const formatDate = (dateString: string): string => {
  if (!dateString) return '';
  return new Date(dateString).toLocaleDateString('es-CO');
};

const formatTime = (timeString: string): string => {
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

const isStatusCancelled = (status: string): boolean => {
  if (!status) return false;
  const s = status.toLowerCase();
  return s === 'cancelled' || s === 'cancelada';
};

const isStatusCompleted = (status: string): boolean => {
  if (!status) return false;
  const s = status.toLowerCase();
  return s === 'completed' || s === 'completada';
};

const isStatusPending = (status: string): boolean => {
  if (!status) return false;
  const s = status.toLowerCase();
  return s === 'scheduled' || s === 'pendiente' || s === 'programada';
};

export const AppointmentCard: React.FC<AppointmentCardProps> = ({
  appointment,
  isExpanded,
  onToggleExpand,
  onCancel,
  showCancelButton = false
}) => {
  return (
    <div className="bg-white rounded-2xl shadow-lg border border-gray-100 hover:shadow-xl transition-all duration-300">
      <div className="p-6">
        {/* Header */}
        <div className="flex justify-between items-start mb-4">
          <div className="flex-1">
            <h4 className="text-lg font-bold text-[#203461] mb-1">{appointment.appointmentTypeName}</h4>
            <p className="text-sm text-gray-600">#{appointment.appointmentNumber}</p>
          </div>
          <div className="flex items-center space-x-2">
            <StatusBadge statusCode={appointment.status} showIcon={false} className="text-xs" />
            {showCancelButton && isStatusPending(appointment.status) && onCancel && (
              <button
                onClick={() => onCancel(appointment)}
                className="text-red-600 hover:text-red-800 text-sm font-medium px-2 py-1 rounded-lg hover:bg-red-50 transition-colors"
              >
                Cancelar
              </button>
            )}
          </div>
        </div>

        {/* Basic Info */}
        <div className="grid grid-cols-2 gap-4 mb-4">
          <div className="flex items-center text-gray-700">
            <svg className="w-4 h-4 text-[#56C2E1] mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3a1 1 0 011-1h6a1 1 0 011 1v4h.5a2 2 0 012 2v6a2 2 0 01-2 2H7a2 2 0 01-2-2V9a2 2 0 012-2H8z" />
            </svg>
            <span className="text-sm font-medium">{formatDate(appointment.appointmentDate)}</span>
          </div>
          <div className="flex items-center text-gray-700">
            <svg className="w-4 h-4 text-[#56C2E1] mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <span className="text-sm">{formatTime(appointment.appointmentTime)}</span>
          </div>
        </div>

        {/* Expandable Details */}
        {isExpanded && (
          <div className="border-t border-gray-100 pt-4 space-y-3">
            <div className="flex items-center text-gray-700">
              <svg className="w-4 h-4 text-[#56C2E1] mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <span className="text-sm">{appointment.branchName}</span>
            </div>

            {appointment.assignedTechnician && (
              <div className="flex items-center text-gray-700">
                <svg className="w-4 h-4 text-[#56C2E1] mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
                <span className="text-sm">{appointment.assignedTechnician}</span>
              </div>
            )}

            {appointment.observations && (
              <div className={`rounded-xl p-3 ${
                isStatusCancelled(appointment.status) ? 'bg-red-50' :
                isStatusCompleted(appointment.status) ? 'bg-gray-50' : 'bg-blue-50'
              }`}>
                <p className={`text-sm ${
                  isStatusCancelled(appointment.status) ? 'text-red-700' :
                  isStatusCompleted(appointment.status) ? 'text-gray-700' : 'text-blue-700'
                }`}>
                  <span className="font-medium">
                    {isStatusCancelled(appointment.status) ? 'Motivo de cancelación:' : 'Observaciones:'}
                  </span> {appointment.observations}
                </p>
              </div>
            )}

            {appointment.technicianObservations && (
              <div className="bg-green-50 rounded-xl p-3">
                <p className="text-sm text-green-800">
                  <span className="font-medium">Observaciones del técnico:</span> {appointment.technicianObservations}
                </p>
              </div>
            )}
          </div>
        )}

        {/* Toggle Button */}
        <button
          onClick={() => onToggleExpand(appointment.id)}
          className="w-full mt-4 py-2 text-sm text-[#1797D5] hover:text-[#203461] font-medium flex items-center justify-center transition-colors"
        >
          {isExpanded ? (
            <>
              <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 15l7-7 7 7" />
              </svg>
              Ver menos
            </>
          ) : (
            <>
              <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
              </svg>
              Ver detalles
            </>
          )}
        </button>
      </div>
    </div>
  );
};
