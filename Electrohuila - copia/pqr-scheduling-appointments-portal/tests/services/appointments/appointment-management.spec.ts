import { describe, it, expect } from 'vitest';
import { setupAppointmentService, service } from './test-helpers';
import type { UpdateAppointmentDto } from '@/services/appointments/appointment.types';

/**
 * Tests for appointment management (update, cancel, complete, delete)
 */
describe('AppointmentService - Management', () => {
  setupAppointmentService();

  describe('updateAppointment', () => {
    it('should update an appointment successfully', async () => {
      const updateData: UpdateAppointmentDto = {
        id: 1,
        appointmentDate: '2024-01-16',
        appointmentTime: '11:00',
        observations: 'Updated appointment',
      };

      const expectedResponse = {
        success: true,
        message: 'Appointment updated successfully',
      };

      (service as any).patch.mockResolvedValue(expectedResponse);

      const result = await service.updateAppointment(updateData);

      expect((service as any).patch).toHaveBeenCalledWith('/appointments/1', updateData);
      expect(result.success).toBe(true);
      expect(result.message).toContain('successfully');
    });

    it('should handle update errors', async () => {
      const updateData: UpdateAppointmentDto = {
        id: 1,
        appointmentDate: '2024-01-16',
      };

      (service as any).patch.mockRejectedValue(new Error('Update failed'));

      await expect(service.updateAppointment(updateData)).rejects.toThrow('Update failed');
    });
  });

  describe('cancelAppointment', () => {
    it('should cancel an appointment with reason', async () => {
      const appointmentId = 1;
      const reason = 'Client requested cancellation';

      const expectedResponse = {
        success: true,
        message: 'Appointment cancelled successfully',
      };

      (service as any).patch.mockResolvedValue(expectedResponse);

      const result = await service.cancelAppointment(appointmentId, reason);

      expect((service as any).patch).toHaveBeenCalledWith('/appointments/cancel/1', { reason });
      expect(result.success).toBe(true);
      expect(result.message).toContain('cancelled');
    });

    it('should handle cancellation errors', async () => {
      (service as any).patch.mockRejectedValue(new Error('Cannot cancel completed appointment'));

      await expect(service.cancelAppointment(1, 'Test reason')).rejects.toThrow('Cannot cancel completed appointment');
    });
  });

  describe('completeAppointment', () => {
    it('should complete an appointment with notes', async () => {
      const appointmentId = 1;
      const notes = 'Service completed successfully. Customer satisfied.';

      const expectedResponse = {
        success: true,
        message: 'Appointment completed',
      };

      (service as any).patch.mockResolvedValue(expectedResponse);

      const result = await service.completeAppointment(appointmentId, notes);

      expect((service as any).patch).toHaveBeenCalledWith('/appointments/complete/1', { notes });
      expect(result.success).toBe(true);
    });

    it('should allow completing appointment with empty notes', async () => {
      const appointmentId = 1;
      const notes = '';

      const expectedResponse = {
        success: true,
        message: 'Appointment completed',
      };

      (service as any).patch.mockResolvedValue(expectedResponse);

      const result = await service.completeAppointment(appointmentId, notes);

      expect((service as any).patch).toHaveBeenCalledWith('/appointments/complete/1', { notes: '' });
      expect(result.success).toBe(true);
    });
  });

  describe('deleteAppointment', () => {
    it('should delete appointment (hard delete)', async () => {
      const appointmentId = 1;

      const expectedResponse = {
        success: true,
        message: 'Appointment deleted successfully',
      };

      (service as any).delete.mockResolvedValue(expectedResponse);

      const result = await service.deleteAppointment(appointmentId);

      expect((service as any).delete).toHaveBeenCalledWith('/appointments/1');
      expect(result.success).toBe(true);
    });

    it('should handle delete errors', async () => {
      (service as any).delete.mockRejectedValue(new Error('Cannot delete completed appointment'));

      await expect(service.deleteAppointment(1)).rejects.toThrow('Cannot delete completed appointment');
    });
  });
});
