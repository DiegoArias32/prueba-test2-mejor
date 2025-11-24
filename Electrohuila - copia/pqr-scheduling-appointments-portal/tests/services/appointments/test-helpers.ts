import { beforeEach, afterEach, vi } from 'vitest';
import { AppointmentService } from '@/services/appointments/appointment.service';

/**
 * Shared test helpers and setup for AppointmentService tests
 */

export let service: AppointmentService;

/**
 * Setup service with mocked HTTP methods
 */
export function setupAppointmentService() {
  beforeEach(() => {
    service = new AppointmentService();

    // Spy on HTTP methods
    vi.spyOn(service as any, 'get').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'post').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'patch').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'delete').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'publicGet').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'publicPost').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'publicPatch').mockImplementation(vi.fn());
  });

  afterEach(() => {
    vi.clearAllMocks();
  });
}

export { AppointmentService };
