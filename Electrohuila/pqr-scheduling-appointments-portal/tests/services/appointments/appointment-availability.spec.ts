import { describe, it, expect } from 'vitest';
import { setupAppointmentService, service } from './test-helpers';

/**
 * Tests for appointment availability validation
 */
describe('AppointmentService - Availability', () => {
  setupAppointmentService();

  describe('getAvailableHours', () => {
    it('should get available hours for a specific date and branch', async () => {
      const date = '2024-01-15';
      const branchId = 1;
      const mockAvailableHours = ['08:00', '09:00', '10:00', '11:00', '14:00', '15:00'];

      (service as any).get.mockResolvedValue(mockAvailableHours);

      const result = await service.getAvailableHours(date, branchId);

      expect((service as any).get).toHaveBeenCalledWith('/appointments/available-times?date=2024-01-15&branchId=1');
      expect(result).toEqual(mockAvailableHours);
      expect(result).toHaveLength(6);
    });

    it('should return empty array if no hours available', async () => {
      const date = '2024-01-15';
      const branchId = 1;

      (service as any).get.mockResolvedValue([]);

      const result = await service.getAvailableHours(date, branchId);

      expect(result).toEqual([]);
    });

    it('should fallback to manual filtering if API fails', async () => {
      const date = '2024-01-15';
      const branchId = 1;

      // First call (getAvailableHours) fails
      (service as any).get.mockRejectedValueOnce(new Error('API Error'));

      // Second call (getAvailableTimesByBranch) succeeds
      (service as any).get.mockResolvedValueOnce([
        { id: 1, time: '08:00', branchId: 1, isActive: true },
        { id: 2, time: '09:00', branchId: 1, isActive: true },
        { id: 3, time: '10:00', branchId: 1, isActive: false },
      ]);

      // Third call (getAppointments) for filtering
      (service as any).get.mockResolvedValueOnce([
        {
          id: 1,
          appointmentDate: '2024-01-15T00:00:00',
          appointmentTime: '08:00',
          branchId: 1,
          isActive: true,
          status: 'Pending',
        },
      ]);

      const result = await service.getAvailableHours(date, branchId);

      // Should return only active hours that are not occupied
      expect(result).toContain('09:00');
      expect(result).not.toContain('08:00'); // Occupied
      expect(result).not.toContain('10:00'); // Not active
    });

    it('should return empty array if fallback also fails', async () => {
      const date = '2024-01-15';
      const branchId = 1;

      // All calls fail
      (service as any).get.mockRejectedValue(new Error('API Error'));

      const result = await service.getAvailableHours(date, branchId);

      expect(result).toEqual([]);
    });
  });

  describe('validateAvailability', () => {
    it('should validate if a time slot is available', async () => {
      const date = '2024-01-15';
      const time = '10:00';
      const branchId = 1;

      (service as any).get.mockResolvedValue({ available: true });

      const result = await service.validateAvailability(date, time, branchId);

      expect((service as any).get).toHaveBeenCalledWith('/appointments/availability?date=2024-01-15&time=10:00&branchId=1');
      expect(result.available).toBe(true);
    });

    it('should return false if time slot is occupied', async () => {
      const date = '2024-01-15';
      const time = '10:00';
      const branchId = 1;

      (service as any).get.mockResolvedValue({ available: false });

      const result = await service.validateAvailability(date, time, branchId);

      expect(result.available).toBe(false);
    });

    it('should fallback to getAvailableHours if API fails', async () => {
      const date = '2024-01-15';
      const time = '10:00';
      const branchId = 1;

      // First call (validateAvailability) fails
      (service as any).get.mockRejectedValueOnce(new Error('API Error'));

      // Second call (getAvailableHours via fallback) succeeds
      (service as any).get.mockResolvedValueOnce(['09:00', '10:00', '11:00']);

      const result = await service.validateAvailability(date, time, branchId);

      expect(result.available).toBe(true);
    });

    it('should return false if time not in available hours list', async () => {
      const date = '2024-01-15';
      const time = '10:00';
      const branchId = 1;

      // First call fails
      (service as any).get.mockRejectedValueOnce(new Error('API Error'));

      // Second call returns hours without the requested time
      (service as any).get.mockResolvedValueOnce(['09:00', '11:00']);

      const result = await service.validateAvailability(date, time, branchId);

      expect(result.available).toBe(false);
    });
  });
});
