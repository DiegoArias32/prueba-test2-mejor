/**
 * Reusable Cancel Appointment Modal Component
 */

'use client';

import React, { useState, useEffect } from 'react';
import { AppointmentDto } from '@/services/api';
import { apiService } from '@/services';

interface CancelAppointmentModalProps {
  appointment: AppointmentDto;
  onConfirm: (appointmentId: number, reason: string) => Promise<boolean>;
  onClose: () => void;
}

export const CancelAppointmentModal: React.FC<CancelAppointmentModalProps> = ({
  appointment,
  onConfirm,
  onClose
}) => {
  const [reason, setReason] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [cancellationHours, setCancellationHours] = useState(24); // default 24 hours
  const [canCancel, setCanCancel] = useState(true);

  // Load cancellation hours from system settings
  useEffect(() => {
    const loadCancellationSettings = async () => {
      try {
        const settings = await apiService.getSystemSettings();
        const cancellationSetting = settings.find(s => s.settingKey === 'APPOINTMENT_CANCELLATION_HOURS');
        const hours = cancellationSetting ? parseInt(cancellationSetting.settingValue) : 24;
        setCancellationHours(hours);

        // Check if appointment can be cancelled (must be X hours before appointment)
        const appointmentDate = new Date(appointment.appointmentDate);
        const now = new Date();
        const hoursUntilAppointment = (appointmentDate.getTime() - now.getTime()) / (1000 * 60 * 60);
        
        if (hoursUntilAppointment < hours) {
          setCanCancel(false);
          setError(`No se puede cancelar. Debe cancelar con al menos ${hours} horas de anticipación.`);
        }
      } catch (error) {
        console.error('Error loading cancellation settings:', error);
        // Keep defaults if error
      }
    };

    loadCancellationSettings();
  }, [appointment.appointmentDate]);

  const validateReason = (value: string): string => {
    if (!value.trim()) return 'El motivo de cancelación es obligatorio';
    if (value.trim().length < 10) return 'El motivo debe tener al menos 10 caracteres';
    if (value.length > 500) return 'El motivo no puede tener más de 500 caracteres';
    return '';
  };

  const handleSubmit = async () => {
    if (!canCancel) {
      return;
    }

    const validationError = validateReason(reason);
    if (validationError) {
      setError(validationError);
      return;
    }

    setIsSubmitting(true);
    setError('');

    const success = await onConfirm(appointment.id, reason);

    if (success) {
      onClose();
    } else {
      setError('Error al cancelar la cita');
    }

    setIsSubmitting(false);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-2xl shadow-xl max-w-md w-full">
        {/* Header */}
        <div className="px-6 py-4 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-semibold text-[#203461]">Cancelar Cita</h3>
            <button
              onClick={onClose}
              className="text-gray-400 hover:text-gray-600"
              disabled={isSubmitting}
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>

        {/* Content */}
        <div className="px-6 py-4">
          <p className="text-gray-600 mb-4">
            ¿Está seguro de que desea cancelar la cita <strong>{appointment.appointmentNumber}</strong>?
          </p>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Motivo de la cancelación *
            </label>
            <textarea
              value={reason}
              onChange={(e) => {
                setReason(e.target.value);
                if (error && canCancel) setError('');
              }}
              rows={3}
              maxLength={500}
              disabled={isSubmitting || !canCancel}
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:border-transparent transition-all duration-300 ${
                error ? 'border-red-500 focus:ring-red-500' : 'border-gray-300 focus:ring-[#1797D5]'
              } ${!canCancel ? 'bg-gray-100' : ''}`}
              placeholder={canCancel ? "Explique el motivo de la cancelación (mínimo 10 caracteres)..." : "No se puede cancelar esta cita"}
            />
            <div className="flex justify-between items-center mt-1">
              {error && (
                <div className={`text-sm flex items-center ${canCancel ? 'text-red-600' : 'text-orange-600'}`}>
                  <svg className="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                  </svg>
                  {error}
                </div>
              )}
              {canCancel && (
                <span className="text-xs text-gray-500 ml-auto">{reason.length}/500</span>
              )}
            </div>
          </div>
          
          {/* Show cancellation policy */}
          <div className="mb-4 p-3 bg-blue-50 rounded-lg">
            <div className="flex items-center">
              <svg className="w-5 h-5 text-blue-600 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
              </svg>
              <p className="text-sm text-blue-700">
                Política de cancelación: Debe cancelar con al menos {cancellationHours} horas de anticipación
              </p>
            </div>
          </div>
          <div className="flex justify-end space-x-4">
            <button
              onClick={onClose}
              disabled={isSubmitting}
              className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 disabled:opacity-50"
            >
              Cancelar
            </button>
            <button
              onClick={handleSubmit}
              disabled={!canCancel || !reason || reason.trim().length === 0 || isSubmitting}
              className={`px-4 py-2 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed ${
                canCancel 
                  ? 'bg-red-600 text-white hover:bg-red-700' 
                  : 'bg-gray-400 text-white cursor-not-allowed'
              }`}
            >
              {isSubmitting ? 'Cancelando...' : canCancel ? 'Confirmar Cancelación' : 'No se puede cancelar'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
