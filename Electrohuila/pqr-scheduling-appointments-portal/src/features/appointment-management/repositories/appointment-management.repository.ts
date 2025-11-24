/**
 * Repositorio de appointment-management
 * Capa de acceso a datos para gestión de citas
 */

import { httpClient } from '@/core/api/http-client';
import { API_ENDPOINTS } from '@/core/config/api.config';
import type { AppointmentDto, ClientDto } from '@/core/types';

export class AppointmentManagementRepository {
  /**
   * Valida un cliente por su número de cliente (público)
   */
  async validateClient(clientNumber: string): Promise<ClientDto> {
    return httpClient.get<ClientDto>(
      API_ENDPOINTS.PUBLIC.CLIENT_VALIDATE(clientNumber),
      { skipAuth: true }
    );
  }

  /**
   * Obtiene todas las citas de un cliente (público)
   */
  async getClientAppointments(clientNumber: string): Promise<AppointmentDto[]> {
    return httpClient.get<AppointmentDto[]>(
      API_ENDPOINTS.PUBLIC.CLIENT_APPOINTMENTS(clientNumber),
      { skipAuth: true }
    );
  }

  /**
   * Cancela una cita (público)
   */
  async cancelAppointment(
    clientNumber: string,
    appointmentId: number,
    cancelReason: string
  ): Promise<void> {
    return httpClient.post(
      API_ENDPOINTS.PUBLIC.CANCEL_APPOINTMENT(clientNumber, appointmentId),
      { cancelReason },
      { skipAuth: true }
    );
  }

  /**
   * Completa una cita (requiere autenticación - admin)
   * FIXED: Cambiado estructura de payload - solo envía 'notes' según API backend
   */
  async completeAppointment(
    appointmentId: number,
    notes: string
  ): Promise<void> {
    return httpClient.patch(
      `${API_ENDPOINTS.APPOINTMENTS.BY_ID(appointmentId)}/complete`,
      { notes }
    );
  }

  /**
   * Consulta una cita por número (público)
   */
  async queryAppointment(
    appointmentNumber: string,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    _clientNumber: string
  ): Promise<AppointmentDto> {
    return httpClient.get<AppointmentDto>(
      API_ENDPOINTS.PUBLIC.APPOINTMENT(appointmentNumber),
      { skipAuth: true }
    );
  }
}

// Instancia singleton
export const appointmentManagementRepository = new AppointmentManagementRepository();
