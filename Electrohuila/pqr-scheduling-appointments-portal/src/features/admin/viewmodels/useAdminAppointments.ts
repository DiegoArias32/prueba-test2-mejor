/**
 * Hook - Admin Appointments Management
 * Handles appointment CRUD operations from admin panel
 */

'use client';

import { useState, useCallback } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { AppointmentDto, UpdateAppointmentDto } from '../models/admin.models';

interface UseAdminAppointmentsReturn {
  appointments: AppointmentDto[];
  loading: boolean;
  error: string | null;
  formData: Partial<AppointmentDto>;
  setFormData: React.Dispatch<React.SetStateAction<Partial<AppointmentDto>>>;
  fetchAppointments: (status?: 'attended' | 'not-attended' | 'all') => Promise<void>;
  updateAppointment: (id: number, data: UpdateAppointmentDto) => Promise<void>;
  toggleAttendance: (appointment: AppointmentDto, attended: boolean) => Promise<void>;
  resetForm: () => void;
}

export const useAdminAppointments = (repository: AdminRepository, userId?: number): UseAdminAppointmentsReturn => {
  const [appointments, setAppointments] = useState<AppointmentDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<Partial<AppointmentDto>>({});

  const fetchAppointments = useCallback(async (status: 'attended' | 'not-attended' | 'all' = 'all') => {
    setLoading(true);
    setError(null);
    try {
      // Si hay userId, usar citas asignadas; si no, todas las citas
      const data = userId
        ? await repository.getMyAssignedAppointments(userId)
        : await repository.getAllAppointmentsIncludingInactive();

      if (status === 'attended') {
        setAppointments(data.filter(c => c.status === 'Completed'));
      } else if (status === 'not-attended') {
        setAppointments(data.filter(c => c.status === 'No Show' || c.status === 'Cancelled'));
      } else {
        setAppointments(data);
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading appointments');
    } finally {
      setLoading(false);
    }
  }, [repository, userId]);

  const updateAppointment = useCallback(async (id: number, data: UpdateAppointmentDto) => {
    setLoading(true);
    setError(null);
    try {
      await repository.updateAppointment(id, data);
      setAppointments(prev => prev.map(a => a.id === id ? { ...a, ...data } : a));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error updating appointment');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const toggleAttendance = useCallback(async (appointment: AppointmentDto, attended: boolean) => {
    setLoading(true);
    setError(null);
    try {
      // StatusIds: 1=PENDING, 2=CONFIRMED, 3=NO_SHOW, 4=COMPLETED, 5=CANCELLED
      const statusId = attended ? 4 : 3; // 4=COMPLETED, 3=NO_SHOW
      const statusName = attended ? 'Completada' : 'No Asistida';

      const updateData: UpdateAppointmentDto = {
        appointmentDate: appointment.appointmentDate,
        appointmentTime: appointment.appointmentTime,
        statusId: statusId,
        notes: appointment.observations,
        branchId: appointment.branchId,
        appointmentTypeId: appointment.appointmentTypeId
      };

      await repository.updateAppointment(appointment.id, updateData);
      setAppointments(prev => prev.map(a => a.id === appointment.id ? { ...a, status: statusName, statusId: statusId } : a));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error toggling attendance');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [repository]);

  const resetForm = useCallback(() => {
    setFormData({});
  }, []);

  return {
    appointments,
    loading,
    error,
    formData,
    setFormData,
    fetchAppointments,
    updateAppointment,
    toggleAttendance,
    resetForm
  };
};
