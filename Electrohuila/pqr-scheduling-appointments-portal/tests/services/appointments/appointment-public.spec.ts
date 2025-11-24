import { describe, it, expect } from 'vitest';
import { setupAppointmentService, service } from './test-helpers';
import type { PublicAppointmentRequestDto } from '@/services/appointments/appointment.types';

/**
 * Tests for public appointment operations (no authentication)
 */
describe('AppointmentService - Public', () => {
  setupAppointmentService();

  describe('cancelPublicAppointment', () => {
    it('should cancel public appointment with client number', async () => {
      const clientNumber = '123456789';
      const appointmentId = 1;
      const cancellationReason = 'Cannot attend';

      const expectedResponse = {
        message: 'Appointment cancelled successfully',
      };

      (service as any).publicPatch.mockResolvedValue(expectedResponse);

      const result = await service.cancelPublicAppointment(clientNumber, appointmentId, cancellationReason);

      expect((service as any).publicPatch).toHaveBeenCalledWith(
        '/public/client/123456789/appointment/1/cancel',
        { cancellationReason }
      );
      expect(result.message).toContain('successfully');
    });
  });

  describe('schedulePublicAppointment', () => {
    it('should schedule public appointment without authentication', async () => {
      const appointmentData: PublicAppointmentRequestDto = {
        clientNumber: '123456789',
        branchId: 1,
        appointmentTypeId: 1,
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        observations: 'Public appointment',
      };

      const expectedResponse = {
        appointmentNumber: 'APT-001',
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        status: 'Pending',
        message: 'Appointment scheduled successfully',
      };

      (service as any).publicPost.mockResolvedValue(expectedResponse);

      const result = await service.schedulePublicAppointment(appointmentData);

      expect((service as any).publicPost).toHaveBeenCalledWith('/public/schedule-appointment', appointmentData);
      expect(result.appointmentNumber).toBe('APT-001');
      expect(result.status).toBe('Pending');
    });
  });

  describe('queryPublicAppointment', () => {
    it('should query public appointment with appointment number and client number', async () => {
      const appointmentNumber = 'APT-001';
      const clientNumber = '123456789';

      const expectedResponse = {
        appointmentNumber: 'APT-001',
        status: 'Pending',
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        branch: { name: 'Main Branch' },
        appointmentType: { name: 'Consultation', icon: 'calendar' },
      };

      (service as any).publicGet.mockResolvedValue(expectedResponse);

      const result = await service.queryPublicAppointment(appointmentNumber, clientNumber);

      expect((service as any).publicGet).toHaveBeenCalledWith('/public/appointment/APT-001?clientNumber=123456789');
      expect(result.appointmentNumber).toBe('APT-001');
      expect(result.status).toBe('Pending');
    });

    it('should handle invalid appointment number', async () => {
      const appointmentNumber = 'APT-999';
      const clientNumber = '123456789';

      (service as any).publicGet.mockRejectedValue(new Error('Appointment not found'));

      await expect(service.queryPublicAppointment(appointmentNumber, clientNumber)).rejects.toThrow(
        'Appointment not found'
      );
    });
  });

  describe('verifyAppointmentByQR', () => {
    it('should verify appointment using QR code data', async () => {
      const appointmentNumber = 'APT-001';
      const clientNumber = '123456789';

      const expectedResponse = {
        isValid: true,
        appointmentNumber: 'APT-001',
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        status: 'Pending',
        statusDescription: 'Appointment is pending',
        client: { clientNumber: '123456789', fullName: 'John Doe' },
        branch: { name: 'Main Branch', address: '123 Main St' },
        appointmentType: { name: 'Consultation', icon: 'calendar' },
        createdAt: '2024-01-15T10:00:00Z',
        message: 'Verification successful',
      };

      (service as any).publicGet.mockResolvedValue(expectedResponse);

      const result = await service.verifyAppointmentByQR(appointmentNumber, clientNumber);

      expect((service as any).publicGet).toHaveBeenCalledWith(
        '/public/verify-appointment?number=APT-001&client=123456789'
      );
      expect(result.isValid).toBe(true);
      expect(result.appointmentNumber).toBe('APT-001');
    });

    it('should handle special characters in appointment verification', async () => {
      const appointmentNumber = 'APT-001/TEST';
      const clientNumber = '123-456-789';

      const expectedResponse = {
        isValid: true,
        appointmentNumber: 'APT-001/TEST',
        appointmentDate: '2024-01-15',
        appointmentTime: '10:00',
        status: 'Pending',
        statusDescription: 'Valid',
        client: { clientNumber: '123-456-789', fullName: 'Test User' },
        branch: { name: 'Test Branch', address: 'Test Address' },
        appointmentType: { name: 'Test Type', icon: 'test' },
        createdAt: '2024-01-15T10:00:00Z',
        message: 'Success',
      };

      (service as any).publicGet.mockResolvedValue(expectedResponse);

      await service.verifyAppointmentByQR(appointmentNumber, clientNumber);

      expect((service as any).publicGet).toHaveBeenCalledWith(
        expect.stringContaining('APT-001%2FTEST')
      );
      expect((service as any).publicGet).toHaveBeenCalledWith(
        expect.stringContaining('123-456-789')
      );
    });

    it('should return invalid if appointment verification fails', async () => {
      const appointmentNumber = 'APT-999';
      const clientNumber = '999999999';

      const expectedResponse = {
        isValid: false,
        appointmentNumber: 'APT-999',
        appointmentDate: '',
        appointmentTime: '',
        status: 'Not Found',
        statusDescription: 'Appointment not found',
        client: { clientNumber: '', fullName: '' },
        branch: { name: '', address: '' },
        appointmentType: { name: '', icon: '' },
        createdAt: '',
        message: 'Appointment not found',
      };

      (service as any).publicGet.mockResolvedValue(expectedResponse);

      const result = await service.verifyAppointmentByQR(appointmentNumber, clientNumber);

      expect(result.isValid).toBe(false);
    });
  });
});
