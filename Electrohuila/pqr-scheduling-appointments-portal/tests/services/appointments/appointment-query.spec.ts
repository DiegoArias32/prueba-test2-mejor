import { describe, it, expect } from 'vitest';
import { setupAppointmentService, service } from './test-helpers';
import type { AppointmentDto } from '@/services/appointments/appointment.types';

/**
 * Tests for querying and retrieving appointments
 */
describe('AppointmentService - Query', () => {
  setupAppointmentService();

  describe('getAppointmentById', () => {
    it('should get appointment by ID', async () => {
      const mockAppointment: AppointmentDto = {
        id: 1,
        clientId: 1,
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        appointmentNumber: 'APT-001',
        status: 'Pending',
        isActive: true,
        isEnabled: true,
        createdAt: '2024-01-15T10:00:00Z',
        updatedAt: '2024-01-15T10:00:00Z',
      };

      (service as any).get.mockResolvedValue(mockAppointment);

      const result = await service.getAppointmentById(1);

      expect((service as any).get).toHaveBeenCalledWith('/appointments/1');
      expect(result).toEqual(mockAppointment);
      expect(result.id).toBe(1);
    });

    it('should throw error if appointment not found', async () => {
      (service as any).get.mockRejectedValue(new Error('Appointment not found'));

      await expect(service.getAppointmentById(999)).rejects.toThrow('Appointment not found');
    });
  });

  describe('getPendingAppointments', () => {
    it('should get all pending appointments', async () => {
      const mockAppointments: AppointmentDto[] = [
        {
          id: 1,
          clientId: 1,
          branchId: 1,
          appointmentTypeId: 1,
          appointmentDate: '2024-01-15',
          appointmentTime: '10:00',
          appointmentNumber: 'APT-001',
          status: 'Pending',
          isActive: true,
          isEnabled: true,
          createdAt: '2024-01-15T10:00:00Z',
          updatedAt: '2024-01-15T10:00:00Z',
        },
        {
          id: 2,
          clientId: 2,
          branchId: 1,
          appointmentTypeId: 1,
          appointmentDate: '2024-01-16',
          appointmentTime: '11:00',
          appointmentNumber: 'APT-002',
          status: 'Pending',
          isActive: true,
          isEnabled: true,
          createdAt: '2024-01-16T11:00:00Z',
          updatedAt: '2024-01-16T11:00:00Z',
        },
      ];

      (service as any).get.mockResolvedValue(mockAppointments);

      const result = await service.getPendingAppointments();

      expect((service as any).get).toHaveBeenCalledWith('/appointments/pending');
      expect(result).toEqual(mockAppointments);
      expect(result).toHaveLength(2);
      expect(result.every((apt) => apt.status === 'Pending')).toBe(true);
    });

    it('should return empty array if no pending appointments', async () => {
      (service as any).get.mockResolvedValue([]);

      const result = await service.getPendingAppointments();

      expect(result).toEqual([]);
      expect(result).toHaveLength(0);
    });
  });

  describe('getCompletedAppointments', () => {
    it('should get all completed appointments', async () => {
      const mockAppointments: AppointmentDto[] = [
        {
          id: 1,
          clientId: 1,
          branchId: 1,
          appointmentTypeId: 1,
          appointmentDate: '2024-01-15',
          appointmentTime: '10:00',
          appointmentNumber: 'APT-001',
          status: 'Completed',
          isActive: true,
          isEnabled: true,
          createdAt: '2024-01-15T10:00:00Z',
          updatedAt: '2024-01-15T15:00:00Z',
        },
      ];

      (service as any).get.mockResolvedValue(mockAppointments);

      const result = await service.getCompletedAppointments();

      expect((service as any).get).toHaveBeenCalledWith('/appointments/completed');
      expect(result).toEqual(mockAppointments);
      expect(result[0].status).toBe('Completed');
    });
  });
});
