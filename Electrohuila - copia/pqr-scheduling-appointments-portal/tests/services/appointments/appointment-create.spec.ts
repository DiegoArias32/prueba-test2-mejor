import { describe, it, expect } from 'vitest';
import { setupAppointmentService, service } from './test-helpers';
import type {
  AppointmentDto,
  CreateAppointmentV1Dto,
} from '@/services/appointments/appointment.types';

/**
 * Tests for appointment creation and scheduling
 */
describe('AppointmentService - Create & Schedule', () => {
  setupAppointmentService();

  describe('createAppointment', () => {
    it('should create a new appointment successfully', async () => {
      const newAppointment = {
        clientId: 1,
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        appointmentNumber: 'APT-001',
        observations: 'Test appointment',
        status: 'Pending' as const,
        isActive: true,
        isEnabled: true,
      };

      const expectedResponse: AppointmentDto = {
        id: 1,
        ...newAppointment,
        appointmentNumber: 'APT-001',
        isEnabled: true,
        createdAt: '2024-01-15T10:00:00Z',
        updatedAt: '2024-01-15T10:00:00Z',
      };

      (service as any).post.mockResolvedValue(expectedResponse);

      const result = await service.createAppointment(newAppointment);

      expect((service as any).post).toHaveBeenCalledWith('/appointments', newAppointment);
      expect(result).toEqual(expectedResponse);
      expect(result.id).toBe(1);
      expect(result.status).toBe('Pending');
    });

    it('should handle API errors when creating appointment', async () => {
      const newAppointment = {
        clientId: 1,
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        appointmentNumber: 'APT-002',
        status: 'Pending' as const,
        isActive: true,
        isEnabled: true,
      };

      (service as any).post.mockRejectedValue(new Error('API Error: Unable to create appointment'));

      await expect(service.createAppointment(newAppointment)).rejects.toThrow('API Error');
    });
  });

  describe('scheduleAppointmentV1', () => {
    it('should schedule appointment with validations', async () => {
      const appointmentData: CreateAppointmentV1Dto = {
        clientId: 1,
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        notes: 'Scheduled with validation',
      };

      const expectedResponse: AppointmentDto = {
        id: 1,
        clientId: appointmentData.clientId,
        branchId: appointmentData.branchId,
        appointmentTypeId: appointmentData.appointmentTypeId,
        appointmentDate: appointmentData.appointmentDate,
        appointmentTime: appointmentData.appointmentTime || '',
        appointmentNumber: 'APT-001',
        status: 'Pending',
        isActive: true,
        isEnabled: true,
        observations: appointmentData.notes,
        createdAt: '2024-01-15T10:00:00Z',
        updatedAt: '2024-01-15T10:00:00Z',
      };

      (service as any).post.mockResolvedValue(expectedResponse);

      const result = await service.scheduleAppointmentV1(appointmentData);

      expect((service as any).post).toHaveBeenCalledWith('/appointments/schedule', appointmentData);
      expect(result).toEqual(expectedResponse);
      expect(result.id).toBe(1);
    });

    it('should handle validation errors from API', async () => {
      const appointmentData: CreateAppointmentV1Dto = {
        clientId: 1,
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
      };

      (service as any).post.mockRejectedValue(new Error('Time slot already taken'));

      await expect(service.scheduleAppointmentV1(appointmentData)).rejects.toThrow('Time slot already taken');
    });
  });

  describe('scheduleAppointmentForNewUser', () => {
    it('should schedule appointment for new user with simple registration', async () => {
      const appointmentData = {
        userData: {
          fullName: 'Juan Pérez',
          documentType: 'CC',
          documentNumber: '123456789',
          email: 'juan@example.com',
          phone: '3001234567',
          mobile: '3001234567',
          address: 'Calle 123',
        },
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        observations: 'First appointment',
      };

      const expectedResponse = {
        requestNumber: 'REQ-001',
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        status: 'Pending',
        message: 'Appointment scheduled successfully',
      };

      (service as any).publicPost.mockResolvedValue(expectedResponse);

      const result = await service.scheduleAppointmentForNewUser(appointmentData);

      expect((service as any).publicPost).toHaveBeenCalledWith(
        '/public/schedule-simple-appointment',
        expect.objectContaining({
          DocumentNumber: '123456789',
          FullName: 'Juan Pérez',
          Email: 'juan@example.com',
          BranchId: 1,
          AppointmentTypeId: 1,
        })
      );
      expect(result.requestNumber).toBe('REQ-001');
      expect(result.status).toBe('Pending');
    });

    it('should throw error if required fields are missing', async () => {
      const incompleteData = {
        userData: {
          fullName: '',
          documentType: 'CC',
          documentNumber: '',
          email: '',
          phone: '',
          mobile: '',
          address: '',
        },
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
      };

      await expect(service.scheduleAppointmentForNewUser(incompleteData)).rejects.toThrow(
        'Faltan datos obligatorios: documento, nombre o email'
      );
    });

    it('should throw error if branch or appointment type is missing', async () => {
      const incompleteData = {
        userData: {
          fullName: 'Juan Pérez',
          documentType: 'CC',
          documentNumber: '123456789',
          email: 'juan@example.com',
          phone: '3001234567',
          mobile: '3001234567',
          address: 'Calle 123',
        },
        branchId: 0,
        appointmentTypeId: 0,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
      };

      await expect(service.scheduleAppointmentForNewUser(incompleteData)).rejects.toThrow(
        'Faltan IDs de sede o tipo de cita'
      );
    });

    it('should throw error if date or time is missing', async () => {
      const incompleteData = {
        userData: {
          fullName: 'Juan Pérez',
          documentType: 'CC',
          documentNumber: '123456789',
          email: 'juan@example.com',
          phone: '3001234567',
          mobile: '3001234567',
          address: 'Calle 123',
        },
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '',
        appointmentTime: '',
      };

      await expect(service.scheduleAppointmentForNewUser(incompleteData)).rejects.toThrow(
        'Faltan fecha u hora de la cita'
      );
    });
  });
});
