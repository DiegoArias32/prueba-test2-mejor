/**
 * Repositorio de citas (appointments)
 * Capa de acceso a datos para operaciones de agendamiento
 */

import { httpClient } from '@/core/api/http-client';
import { API_ENDPOINTS } from '@/core/config/api.config';
import { BranchDto, AppointmentTypeDto, ClientDto, AppointmentDto } from '@/core/types';


export class AppointmentRepository {
  /**
   * Valida un cliente público por su número
   */
  async validatePublicClient(clientNumber: string): Promise<ClientDto> {
    return httpClient.get<ClientDto>(
      API_ENDPOINTS.PUBLIC.CLIENT_VALIDATE(clientNumber),
      { skipAuth: true }
    );
  }



  /**
   * Obtiene sedes públicas (sin autenticación)
   */
  async getPublicBranches(): Promise<BranchDto[]> {
    return httpClient.get<BranchDto[]>(API_ENDPOINTS.PUBLIC.BRANCHES, { skipAuth: true });
  }

  /**
   * Obtiene tipos de cita públicos (sin autenticación)
   */
  async getPublicAppointmentTypes(): Promise<AppointmentTypeDto[]> {
    return httpClient.get<AppointmentTypeDto[]>(
      API_ENDPOINTS.PUBLIC.APPOINTMENT_TYPES,
      { skipAuth: true }
    );
  }

  /**
   * Obtiene horas disponibles para una fecha y sede
   */
  async getPublicAvailableTimes(date: string, branchId: number): Promise<string[]> {
    return httpClient.get<string[]>(
      `${API_ENDPOINTS.PUBLIC.AVAILABLE_TIMES}?date=${date}&branchId=${branchId}`,
      { skipAuth: true }
    );
  }

  /**
   * Agenda una cita pública
   */
  async schedulePublicAppointment(appointmentData: {
    clientNumber: string;
    branchId: number;
    appointmentTypeId: number;
    appointmentDate: string;
    appointmentTime: string;
    observations?: string;
  }): Promise<{
    appointmentNumber: string;
    appointmentDate: string;
    appointmentTime: string;
    status: string;
    message: string;
  }> {
    return httpClient.post(
      API_ENDPOINTS.PUBLIC.SCHEDULE_APPOINTMENT,
      appointmentData,
      { skipAuth: true }
    );
  }

  /**
   * Consulta una cita pública
   */
  async queryPublicAppointment(
    appointmentNumber: string,
    clientNumber: string
  ): Promise<{
    appointmentNumber: string;
    appointmentDate: string;
    appointmentTime: string;
    status: string;
    observations?: string;
    completedDate?: string;
    branch: { name: string };
    appointmentType: { name: string; icon: string };
  }> {
    return httpClient.get(
      `${API_ENDPOINTS.PUBLIC.APPOINTMENT(appointmentNumber)}?clientNumber=${clientNumber}`,
      { skipAuth: true }
    );
  }

  /**
   * Obtiene citas de un cliente
   */
  async getPublicClientAppointments(clientNumber: string): Promise<AppointmentDto[]> {
    return httpClient.get(
      API_ENDPOINTS.PUBLIC.CLIENT_APPOINTMENTS(clientNumber),
      { skipAuth: true }
    );
  }

  /**
   * Cancela una cita pública
   */
  async cancelPublicAppointment(
    clientNumber: string,
    appointmentId: number,
    cancellationReason: string
  ): Promise<{ message: string }> {
    return httpClient.patch(
      API_ENDPOINTS.PUBLIC.CANCEL_APPOINTMENT(clientNumber, appointmentId),
      { cancellationReason },
      { skipAuth: true }
    );
  }

  /**
   * Verifica una cita por código QR
   */
  async verifyAppointmentByQR(
    appointmentNumber: string,
    clientNumber: string
  ): Promise<{
    isValid: boolean;
    appointmentNumber: string;
    appointmentDate: string;
    appointmentTime: string;
    status: string;
    statusDescription: string;
    client: { clientNumber: string; fullName: string };
    branch: { name: string; address: string };
    appointmentType: { name: string; icon: string };
    createdAt: string;
    observations?: string;
    message: string;
  }> {
    return httpClient.get(
      `${API_ENDPOINTS.PUBLIC.VERIFY_APPOINTMENT}?number=${encodeURIComponent(appointmentNumber)}&client=${encodeURIComponent(clientNumber)}`,
      { skipAuth: true }
    );
  }

  /**
   * Agenda cita para usuario nuevo (registro simple)
   */
  async scheduleAppointmentForNewUser(appointmentData: {
    userData: {
      documentType: string;
      documentNumber: string;
      fullName: string;
      phone: string;
      mobile: string;
      email: string;
      address: string;
    };
    branchId: number;
    appointmentTypeId: number;
    appointmentDate: string;
    appointmentTime: string;
    observations?: string;
  }): Promise<{
    requestNumber: string;
    appointmentDate: string;
    appointmentTime: string;
    status: string;
    message: string;
  }> {
    const requestBody = {
      DocumentType: appointmentData.userData.documentType,
      DocumentNumber: appointmentData.userData.documentNumber,
      FullName: appointmentData.userData.fullName,
      Phone: appointmentData.userData.phone,
      Mobile: appointmentData.userData.mobile,
      Email: appointmentData.userData.email,
      Address: appointmentData.userData.address,
      BranchId: appointmentData.branchId,
      AppointmentTypeId: appointmentData.appointmentTypeId,
      AppointmentDate: appointmentData.appointmentDate,
      AppointmentTime: appointmentData.appointmentTime,
      Observations: appointmentData.observations || '',
    };

    return httpClient.post(
      '/api/v1/public/schedule-simple-appointment',
      requestBody,
      { skipAuth: true }
    );
  }

  /**
   * Formatea hora para mostrar (24h a 12h con AM/PM)
   */
  formatTimeForDisplay(time: string): string {
    if (!time || typeof time !== 'string') {
      return '';
    }

    const cleanTime = time.trim();
    if (!cleanTime.includes(':')) {
      return time;
    }

    const parts = cleanTime.split(':');
    if (parts.length < 2) {
      return time;
    }

    const [hoursStr, minutesStr] = parts;
    const hours = parseInt(hoursStr, 10);
    const minutes = parseInt(minutesStr, 10);

    if (
      isNaN(hours) ||
      isNaN(minutes) ||
      hours < 0 ||
      hours > 23 ||
      minutes < 0 ||
      minutes > 59
    ) {
      return time;
    }

    const ampm = hours >= 12 ? 'PM' : 'AM';
    const hour12 = hours % 12 || 12;
    const formattedMinutes = minutes.toString().padStart(2, '0');

    return `${hour12}:${formattedMinutes} ${ampm}`;
  }
}

// Instancia singleton del repositorio
export const appointmentRepository = new AppointmentRepository();
