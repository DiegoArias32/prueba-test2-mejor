import { describe, it, expect } from 'vitest';
import { setupAppointmentService, service } from './test-helpers';
import type {
  UpdateAppointmentDto,
  PublicAppointmentRequestDto,
} from '@/services/appointments/appointment.types';

/**
 * Tests for complex business logic scenarios and workflows
 */
describe('AppointmentService - Business Logic Scenarios', () => {
  setupAppointmentService();

  it('should handle full appointment lifecycle: create -> complete', async () => {
    // Create appointment
    const newAppointment = {
      clientId: 1,
      branchId: 1,
      appointmentTypeId: 1,
      appointmentDate: '2024-01-15',
      appointmentTime: '10:00',
      appointmentNumber: 'APT-001',
      status: 'Pending' as const,
      isActive: true,
      isEnabled: true,
    };

    const createdAppointment = {
      id: 1,
      ...newAppointment,
      createdAt: '2024-01-15T10:00:00Z',
      updatedAt: '2024-01-15T10:00:00Z',
    };

    (service as any).post.mockResolvedValue(createdAppointment);

    const created = await service.createAppointment(newAppointment);
    expect(created.id).toBe(1);
    expect(created.status).toBe('Pending');

    // Complete appointment
    (service as any).patch.mockResolvedValue({ success: true, message: 'Completed' });

    const completed = await service.completeAppointment(created.id, 'Service completed successfully');
    expect(completed.success).toBe(true);
  });

  it('should handle full appointment lifecycle: create -> cancel', async () => {
    // Create appointment
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

    const createdAppointment = {
      id: 1,
      ...newAppointment,
      createdAt: '2024-01-15T10:00:00Z',
      updatedAt: '2024-01-15T10:00:00Z',
    };

    (service as any).post.mockResolvedValue(createdAppointment);

    const created = await service.createAppointment(newAppointment);
    expect(created.id).toBe(1);
    expect(created.status).toBe('Pending');

    // Cancel appointment
    (service as any).patch.mockResolvedValue({ success: true, message: 'Cancelled' });

    const cancelled = await service.cancelAppointment(created.id, 'Client requested cancellation');
    expect(cancelled.success).toBe(true);
  });

  it('should prevent double-booking by validating availability first', async () => {
    const date = '2024-01-15';
    const time = '10:00';
    const branchId = 1;

    // Check availability first
    (service as any).get.mockResolvedValueOnce({ available: true });

    const availability = await service.validateAvailability(date, time, branchId);
    expect(availability.available).toBe(true);

    // Then create appointment
    const newAppointment = {
      clientId: 1,
      branchId: branchId,
      appointmentTypeId: 1,
      appointmentDate: date,
      appointmentTime: time,
      appointmentNumber: 'APT-003',
      status: 'Pending' as const,
      isActive: true,
      isEnabled: true,
    };

    const createdAppointment = {
      id: 1,
      ...newAppointment,
      createdAt: '2024-01-15T10:00:00Z',
      updatedAt: '2024-01-15T10:00:00Z',
    };

    (service as any).post.mockResolvedValue(createdAppointment);

    const created = await service.createAppointment(newAppointment);
    expect(created.id).toBe(1);
    expect(created.appointmentDate).toBe(date);
    expect(created.appointmentTime).toBe(time);
  });

  it('should handle reschedule scenario: validate -> update', async () => {
    const appointmentId = 1;
    const newDate = '2024-01-16';
    const newTime = '11:00';
    const branchId = 1;

    // Validate new time is available
    (service as any).get.mockResolvedValueOnce({ available: true });

    const availability = await service.validateAvailability(newDate, newTime, branchId);
    expect(availability.available).toBe(true);

    // Update appointment
    const updateData: UpdateAppointmentDto = {
      id: appointmentId,
      appointmentDate: newDate,
      appointmentTime: newTime,
    };

    (service as any).patch.mockResolvedValue({ success: true, message: 'Rescheduled' });

    const updated = await service.updateAppointment(updateData);
    expect(updated.success).toBe(true);
  });

  it('should handle public user journey: schedule -> query -> cancel', async () => {
    const clientNumber = '123456789';

    // Schedule appointment
    const appointmentData: PublicAppointmentRequestDto = {
      clientNumber: clientNumber,
      branchId: 1,
      appointmentTypeId: 1,
      appointmentDate: '2024-01-15',
      appointmentTime: '10:00',
    };

    (service as any).publicPost.mockResolvedValue({
      appointmentNumber: 'APT-001',
      appointmentDate: '2024-01-15',
      appointmentTime: '10:00',
      status: 'Pending',
      message: 'Scheduled',
    });

    const scheduled = await service.schedulePublicAppointment(appointmentData);
    expect(scheduled.appointmentNumber).toBe('APT-001');

    // Query appointment
    (service as any).publicGet.mockResolvedValueOnce({
      appointmentNumber: 'APT-001',
      appointmentDate: '2024-01-15',
      appointmentTime: '10:00',
      status: 'Pending',
      branch: { name: 'Main Branch' },
      appointmentType: { name: 'Consultation', icon: 'calendar' },
    });

    const queried = await service.queryPublicAppointment('APT-001', clientNumber);
    expect(queried.status).toBe('Pending');

    // Cancel appointment
    (service as any).publicPatch.mockResolvedValue({ message: 'Cancelled' });

    const cancelled = await service.cancelPublicAppointment(clientNumber, 1, 'Cannot attend');
    expect(cancelled.message).toBe('Cancelled');
  });
});
