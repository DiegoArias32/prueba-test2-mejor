/**
 * ViewModel de appointment-management
 * Lógica de negocio para gestión de citas
 */

'use client';

import { useState, useCallback } from 'react';
import { appointmentManagementRepository } from '../repositories/appointment-management.repository';
import { ValidationUtils } from '@/shared/utils/validation.utils';
import type {
  AppointmentDto,
  ClientDto,
  AppointmentTab,
  ModalType,
  AppointmentStats
} from '../models/appointment-management.models';
import type { LoadingState } from '@/core/types';

export const useAppointmentManagement = () => {
  // Estados de autenticación y datos
  const [clientNumber, setClientNumber] = useState<string>('');
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [client, setClient] = useState<ClientDto | null>(null);
  const [appointments, setAppointments] = useState<AppointmentDto[]>([]);

  // Estados de UI
  const [loadingState, setLoadingState] = useState<LoadingState>('idle');
  const [error, setError] = useState<string>('');
  const [successMessage, setSuccessMessage] = useState<string>('');
  const [activeTab, setActiveTab] = useState<AppointmentTab>('pendientes');
  const [expandedCard, setExpandedCard] = useState<number | null>(null);

  // Estados de modal
  const [modalType, setModalType] = useState<ModalType>(null);
  const [selectedAppointment, setSelectedAppointment] = useState<AppointmentDto | null>(null);
  const [cancelReason, setCancelReason] = useState<string>('');
  const [completionData, setCompletionData] = useState({
    notes: ''
  });

  // Estados de validación
  const [validationErrors, setValidationErrors] = useState<{ [key: string]: string }>({});
  const [touchedFields, setTouchedFields] = useState<{ [key: string]: boolean }>({});

  /**
   * Valida un campo específico
   */
  const validateField = useCallback((field: string, value: string): string => {
    switch (field) {
      case 'clientNumber':
        const result = ValidationUtils.validateIdentificationNumber(value);
        return result.isValid ? '' : result.message;

      case 'cancelReason':
        if (!value.trim()) {
          return 'El motivo de cancelación es obligatorio';
        }
        if (value.trim().length < 10) {
          return 'El motivo debe tener al menos 10 caracteres';
        }
        if (value.length > 500) {
          return 'El motivo no puede tener más de 500 caracteres';
        }
        return '';

      case 'notes':
        if (!value.trim()) {
          return 'Las notas de completado son obligatorias';
        }
        if (value.trim().length < 10) {
          return 'Las notas deben tener al menos 10 caracteres';
        }
        if (value.length > 1000) {
          return 'Las notas no pueden tener más de 1000 caracteres';
        }
        return '';

      default:
        return '';
    }
  }, []);

  /**
   * Maneja el cambio de campo con validación
   */
  const handleFieldChange = useCallback((field: string, value: string) => {
    setTouchedFields(prev => ({ ...prev, [field]: true }));

    if (touchedFields[field]) {
      const errorMessage = validateField(field, value);
      setValidationErrors(prev => ({ ...prev, [field]: errorMessage }));
    }
  }, [touchedFields, validateField]);

  /**
   * Limpia errores de validación
   */
  const clearValidationErrors = useCallback(() => {
    setValidationErrors({});
    setTouchedFields({});
  }, []);

  /**
   * Busca las citas de un cliente
   */
  const searchAppointments = useCallback(async () => {
    const clientError = validateField('clientNumber', clientNumber);
    if (clientError) {
      setValidationErrors({ clientNumber: clientError });
      setTouchedFields({ clientNumber: true });
      setError(clientError);
      return;
    }

    setLoadingState('loading');
    setError('');
    clearValidationErrors();

    try {
      const clientData = await appointmentManagementRepository.validateClient(clientNumber);
      setClient(clientData);

      const clientAppointments = await appointmentManagementRepository.getClientAppointments(clientNumber);

      if (clientAppointments.length === 0) {
        setError('No se encontraron citas para este código de usuario');
        setIsAuthenticated(false);
      } else {
        setAppointments(clientAppointments);
        setIsAuthenticated(true);
        setLoadingState('success');
      }
    } catch {
      setError('Código de usuario no encontrado o sin citas registradas');
      setIsAuthenticated(false);
      setLoadingState('error');
    }
  }, [clientNumber, validateField, clearValidationErrors]);

  /**
   * Cancela una cita
   */
  const cancelAppointment = useCallback(async () => {
    const cancelError = validateField('cancelReason', cancelReason);
    if (cancelError) {
      setValidationErrors(prev => ({ ...prev, cancelReason: cancelError }));
      setTouchedFields(prev => ({ ...prev, cancelReason: true }));
      setError(cancelError);
      return;
    }

    if (!selectedAppointment || !client) {
      setError('Error: Cita o cliente no seleccionado');
      return;
    }

    setLoadingState('loading');
    try {
      await appointmentManagementRepository.cancelAppointment(
        client.clientNumber,
        selectedAppointment.id,
        cancelReason.trim()
      );

      // Esperar un momento para que el backend procese
      await new Promise(resolve => setTimeout(resolve, 1000));

      // Recargar citas
      const updatedAppointments = await appointmentManagementRepository.getClientAppointments(client.clientNumber);
      setAppointments(updatedAppointments);

      // Cambiar a tab de canceladas
      setActiveTab('canceladas');

      // Mostrar mensaje de éxito
      setSuccessMessage(`Cita ${selectedAppointment.appointmentNumber} cancelada exitosamente`);
      setTimeout(() => setSuccessMessage(''), 5000);

      // Cerrar modal
      closeModal();
      setLoadingState('success');
    } catch (err) {
      setError('Error al cancelar la cita: ' + (err instanceof Error ? err.message : 'Error desconocido'));
      setLoadingState('error');
    }
  }, [cancelReason, selectedAppointment, client, validateField]);

  /**
   * Completa una cita (solo admin)
   * FIXED: Actualizado para enviar solo 'notes' según nueva estructura de API
   */
  const completeAppointment = useCallback(async () => {
    const notesError = validateField('notes', completionData.notes);

    if (notesError) {
      setValidationErrors({ notes: notesError });
      setTouchedFields({ notes: true });
      setError(notesError);
      return;
    }

    if (!selectedAppointment) {
      setError('Error: Cita no seleccionada');
      return;
    }

    setLoadingState('loading');
    try {
      await appointmentManagementRepository.completeAppointment(
        selectedAppointment.id,
        completionData.notes
      );

      // Recargar citas
      if (client) {
        const updatedAppointments = await appointmentManagementRepository.getClientAppointments(client.clientNumber);
        setAppointments(updatedAppointments);
      }

      setSuccessMessage(`Cita ${selectedAppointment.appointmentNumber} completada exitosamente`);
      setTimeout(() => setSuccessMessage(''), 5000);

      closeModal();
      setLoadingState('success');
    } catch (err) {
      setError('Error al completar la cita: ' + (err instanceof Error ? err.message : 'Error desconocido'));
      setLoadingState('error');
    }
  }, [completionData, selectedAppointment, client, validateField, closeModal]);

  /**
   * Cierra el modal
   */
  const closeModal = useCallback(() => {
    setModalType(null);
    setSelectedAppointment(null);
    setCancelReason('');
    setCompletionData({ notes: '' });
    clearValidationErrors();
    setError('');
  }, [clearValidationErrors]);

  /**
   * Resetea el formulario
   */
  const resetForm = useCallback(() => {
    setIsAuthenticated(false);
    setClientNumber('');
    setAppointments([]);
    setError('');
    setClient(null);
    clearValidationErrors();
    setActiveTab('pendientes');
  }, [clearValidationErrors]);

  /**
   * Helpers para verificar el estado de la cita
   */
  const isPending = (status: string): boolean => {
    const s = status.toLowerCase();
    return s === 'scheduled' || s === 'pendiente' || s === 'programada';
  };

  const isCompleted = (status: string): boolean => {
    const s = status.toLowerCase();
    return s === 'completed' || s === 'completada';
  };

  const isCancelled = (status: string): boolean => {
    const s = status.toLowerCase();
    return s === 'cancelled' || s === 'cancelada';
  };

  /**
   * Obtiene el color del estado
   */
  const getStatusColor = (status: string): string => {
    if (isPending(status)) return 'bg-yellow-100 text-yellow-800 border-yellow-200';
    if (isCompleted(status)) return 'bg-green-100 text-green-800 border-green-200';
    if (isCancelled(status)) return 'bg-red-100 text-red-800 border-red-200';
    return 'bg-gray-100 text-gray-800 border-gray-200';
  };

  /**
   * Formatea el texto del estado
   */
  const getStatusText = (status: string): string => {
    return status.charAt(0).toUpperCase() + status.slice(1).toLowerCase();
  };

  /**
   * Formatea la fecha
   */
  const formatDate = (dateString: string): string => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('es-CO');
  };

  /**
   * Formatea la hora
   */
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

  /**
   * Filtra las citas por estado
   */
  const filteredAppointments = {
    pending: appointments.filter(apt => isPending(apt.status)),
    completed: appointments.filter(apt => isCompleted(apt.status)),
    cancelled: appointments.filter(apt => isCancelled(apt.status))
  };

  /**
   * Estadísticas de citas
   */
  const stats: AppointmentStats = {
    total: appointments.length,
    pending: filteredAppointments.pending.length,
    completed: filteredAppointments.completed.length,
    cancelled: filteredAppointments.cancelled.length
  };

  return {
    // Estado
    clientNumber,
    setClientNumber,
    isAuthenticated,
    client,
    appointments,
    filteredAppointments,
    stats,
    loadingState,
    isLoading: loadingState === 'loading',
    error,
    successMessage,

    // UI State
    activeTab,
    setActiveTab,
    expandedCard,
    setExpandedCard,

    // Modal State
    modalType,
    setModalType,
    selectedAppointment,
    setSelectedAppointment,
    cancelReason,
    setCancelReason,
    completionData,
    setCompletionData,

    // Validación
    validationErrors,
    touchedFields,
    handleFieldChange,

    // Métodos
    searchAppointments,
    cancelAppointment,
    completeAppointment,
    closeModal,
    resetForm,

    // Helpers
    isPending,
    isCompleted,
    isCancelled,
    getStatusColor,
    getStatusText,
    formatDate,
    formatTime
  };
};
