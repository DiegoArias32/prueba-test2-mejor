/**
 * ViewModel for Appointment Consultation
 * Handles business logic for consulting and managing appointments
 */

import { useState, useCallback } from 'react';
import { AppointmentDto, ClientDto } from '@/services/api';

interface ValidationErrors {
  [key: string]: string;
}

type TabType = 'pendientes' | 'completadas' | 'canceladas';

export const useAppointmentConsultation = () => {
  // State
  const [clientNumber, setClientNumber] = useState('');
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [appointments, setAppointments] = useState<AppointmentDto[]>([]);
  const [client, setClient] = useState<ClientDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [activeTab, setActiveTab] = useState<TabType>('pendientes');
  const [expandedCard, setExpandedCard] = useState<number | null>(null);
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});

  const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api/v1';

  // Validation
  const validateClientNumber = useCallback((value: string): string => {
    if (!value.trim()) return 'Número de cliente es obligatorio';
    if (!/^\d+$/.test(value)) return 'El número de cliente debe contener solo números';
    if (value.length < 3) return 'El número de cliente debe tener al menos 3 dígitos';
    return '';
  }, []);

  // Search appointments
  const searchAppointments = useCallback(async () => {
    const error = validateClientNumber(clientNumber);
    if (error) {
      setValidationErrors({ clientNumber: error });
      setError(error);
      return;
    }

    setLoading(true);
    setError('');
    setValidationErrors({});

    try {
      // Validate client exists
      const validateUrl = `${API_BASE_URL}/public/client/validate/${clientNumber}`;
      const validateResponse = await fetch(validateUrl, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
      });

      if (!validateResponse.ok) {
        throw new Error('Cliente no encontrado');
      }

      const clientData = await validateResponse.json();
      setClient(clientData);

      // Get appointments
      const appointmentsUrl = `${API_BASE_URL}/public/client/${clientNumber}/appointments`;
      const appointmentsResponse = await fetch(appointmentsUrl, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
      });

      if (!appointmentsResponse.ok) {
        throw new Error('Error al obtener citas');
      }

      const appointmentsData = await appointmentsResponse.json();

      if (appointmentsData.length === 0) {
        setError('No se encontraron citas para este número de cliente');
        setIsAuthenticated(false);
      } else {
        setAppointments(appointmentsData);
        setIsAuthenticated(true);
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
      setError('Número de cliente no encontrado o sin citas registradas. Verifica el número e intenta nuevamente.');
      setIsAuthenticated(false);
    } finally {
      setLoading(false);
    }
  }, [clientNumber, validateClientNumber, API_BASE_URL]);

  // Cancel appointment
  const cancelAppointment = useCallback(async (appointmentId: number, reason: string) => {
    if (!client) {
      setError('Error: Cliente no encontrado');
      return false;
    }

    try {
      const cancelUrl = `${API_BASE_URL}/public/client/${client.clientNumber}/appointment/${appointmentId}/cancel`;
      const cancelResponse = await fetch(cancelUrl, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ cancellationReason: reason.trim() })
      });

      if (!cancelResponse.ok) {
        throw new Error('Error al cancelar la cita');
      }

      // Refresh appointments
      await new Promise(resolve => setTimeout(resolve, 1000));
      const appointmentsUrl = `${API_BASE_URL}/public/client/${client.clientNumber}/appointments`;
      const appointmentsResponse = await fetch(appointmentsUrl, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
      });

      if (appointmentsResponse.ok) {
        const updatedAppointments = await appointmentsResponse.json();
        setAppointments(updatedAppointments);
      }

      setActiveTab('canceladas');
      setSuccessMessage('Cita cancelada exitosamente');
      setTimeout(() => setSuccessMessage(''), 5000);

      return true;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
      setError('Error al cancelar la cita: ' + errorMessage);
      return false;
    }
  }, [client, API_BASE_URL]);

  // Reset form
  const resetForm = useCallback(() => {
    setIsAuthenticated(false);
    setClientNumber('');
    setAppointments([]);
    setClient(null);
    setError('');
    setValidationErrors({});
  }, []);

  // Filter appointments by status
  const getFilteredAppointments = useCallback((status: 'pending' | 'completed' | 'cancelled') => {
    return appointments.filter(apt => {
      if (!apt.status) return false;
      const s = apt.status.toLowerCase();

      switch (status) {
        case 'pending':
          return s === 'scheduled' || s === 'pendiente' || s === 'programada';
        case 'completed':
          return s === 'completed' || s === 'completada';
        case 'cancelled':
          return s === 'cancelled' || s === 'cancelada';
        default:
          return false;
      }
    });
  }, [appointments]);

  return {
    // State
    clientNumber,
    isAuthenticated,
    appointments,
    client,
    loading,
    error,
    successMessage,
    activeTab,
    expandedCard,
    validationErrors,

    // Actions
    setClientNumber,
    setActiveTab,
    setExpandedCard,
    setError,
    searchAppointments,
    cancelAppointment,
    resetForm,
    getFilteredAppointments,
  };
};
