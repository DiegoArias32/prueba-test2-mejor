/**
 * Appointment Service
 * Handles all appointment-related operations including scheduling, updates, and availability
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  AppointmentDto,
  UpdateAppointmentDto,
  AvailableTimeDto,
  UpdateAvailableTimeDto,
  CreateAppointmentV1Dto,
  PublicAppointmentRequestDto,
  PublicAppointmentResponseDto,
  PublicAppointmentQueryDto,
  AppointmentQRVerificationDto,
  SimpleAppointmentRequestDto,
  SimpleAppointmentResponseDto,
} from './appointment.types';

export class AppointmentService extends BaseHttpService {
  /**
   * Get all appointments (including inactive)
   * TODO: Este endpoint no existe en la API backend - usar /appointments/pending como alternativa
   */
  async getAppointments(): Promise<AppointmentDto[]> {
    // return this.get<AppointmentDto[]>('/appointments/all-including-inactive');
    console.warn('⚠️ Endpoint /appointments/all-including-inactive no existe. Usando /appointments/pending como fallback');
    return this.get<AppointmentDto[]>('/appointments/pending');
  }

  /**
   * Get appointment by ID
   */
  async getAppointmentById(id: number): Promise<AppointmentDto> {
    return this.get<AppointmentDto>(`/appointments/${id}`);
  }

  /**
   * Create new appointment
   */
  async createAppointment(
    appointment: Omit<AppointmentDto, 'id' | 'createdAt' | 'updatedAt'>
  ): Promise<AppointmentDto> {
    return this.post<AppointmentDto>('/appointments', appointment);
  }

  /**
   * Update appointment
   */
  async updateAppointment(id: number, appointment: UpdateAppointmentDto): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/appointments/${id}`, appointment);
  }

  /**
   * Delete appointment (hard delete)
   */
  async deleteAppointment(id: number): Promise<{ success: boolean; message: string }> {
    return this.delete<{ success: boolean; message: string }>(`/appointments/${id}`);
  }

  /**
   * Delete appointment (logical delete)
   * TODO: Este endpoint no existe en la API backend - necesita ser implementado
   */
  async deleteLogicalAppointment(id: number): Promise<{ success: boolean; message: string }> {
    throw new Error('⚠️ Endpoint /appointments/delete-logical/{id} no implementado en la API backend');
    // return this.patch<{ success: boolean; message: string }>(`/appointments/delete-logical/${id}`);
  }

  /**
   * Get pending appointments
   */
  async getPendingAppointments(): Promise<AppointmentDto[]> {
    return this.get<AppointmentDto[]>('/appointments/pending');
  }

  /**
   * Get completed appointments
   */
  async getCompletedAppointments(): Promise<AppointmentDto[]> {
    return this.get<AppointmentDto[]>('/appointments/completed');
  }

  /**
   * Cancel appointment
   * FIXED: Cambiado campo 'observations' por 'reason' según API backend
   */
  async cancelAppointment(
    appointmentId: number,
    reason: string
  ): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/appointments/cancel/${appointmentId}`, {
      reason,
    });
  }

  /**
   * Complete appointment
   * FIXED: Cambiado campos 'assignedTechnician, technicianObservations' por 'notes' según API backend
   */
  async completeAppointment(
    appointmentId: number,
    notes: string
  ): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/appointments/complete/${appointmentId}`, {
      notes,
    });
  }

  /**
   * Get all appointments including inactive
   */
  async getAllAppointmentsIncludingInactive(): Promise<AppointmentDto[]> {
    return this.get<AppointmentDto[]>('/appointments/all-including-inactive');
  }

  // ===== AVAILABLE TIMES =====

  /**
   * Get all available times
   */
  async getAllAvailableTimes(): Promise<AvailableTimeDto[]> {
    return this.get<AvailableTimeDto[]>('/availabletimes/all-including-inactive');
  }

  /**
   * Get available time by ID
   */
  async getAvailableTimeById(id: number): Promise<AvailableTimeDto> {
    return this.get<AvailableTimeDto>(`/availabletimes/${id}`);
  }

  /**
   * Create available time
   */
  async createAvailableTime(
    availableTime: Omit<AvailableTimeDto, 'id' | 'createdAt' | 'updatedAt'>
  ): Promise<AvailableTimeDto> {
    return this.post<AvailableTimeDto>('/availabletimes', availableTime);
  }

  /**
   * Update available time
   * FIXED: Cambiado de /availabletimes/update a /availabletimes/{id} con PATCH
   */
  async updateAvailableTime(availableTime: UpdateAvailableTimeDto): Promise<{ success: boolean; message: string }> {
    if (!availableTime.id) {
      throw new Error('AvailableTime ID is required for update');
    }
    return this.patch<{ success: boolean; message: string }>(`/availabletimes/${availableTime.id}`, availableTime);
  }

  /**
   * Delete available time (hard delete)
   */
  async deleteAvailableTime(id: number): Promise<{ success: boolean; message: string }> {
    return this.delete<{ success: boolean; message: string }>(`/availabletimes/${id}`);
  }

  /**
   * Delete available time (logical delete / deactivate)
   * Uses dedicated deactivate endpoint with PATCH method
   */
  async deleteLogicalAvailableTime(id: number): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/availabletimes/${id}/deactivate`, {});
  }

  /**
   * Activate available time (reactivate a deactivated time slot)
   * Uses dedicated activate endpoint with PATCH method
   */
  async activateAvailableTime(id: number): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/availabletimes/${id}/activate`, {});
  }

  /**
   * Get available times by branch
   * FIXED: Cambiado de /branches/{id} a /branch/{id} (singular)
   */
  async getAvailableTimesByBranch(branchId: number): Promise<AvailableTimeDto[]> {
    return this.get<AvailableTimeDto[]>(`/availabletimes/branch/${branchId}`);
  }

  /**
   * Get available times by appointment type
   * FIXED: Cambiado de /appointmenttypes/{id} a /appointment-type/{id} (singular + guión)
   */
  async getAvailableTimesByAppointmentType(appointmentTypeId: number): Promise<AvailableTimeDto[]> {
    return this.get<AvailableTimeDto[]>(`/availabletimes/appointment-type/${appointmentTypeId}`);
  }

  /**
   * Get all available times including inactive
   */
  async getAllAvailableTimesIncludingInactive(): Promise<AvailableTimeDto[]> {
    return this.get<AvailableTimeDto[]>('/availabletimes/all-including-inactive');
  }

  /**
   * Get available hours for a specific date and branch
   */
  async getAvailableHours(date: string, branchId: number): Promise<string[]> {
    try {
      const response = await this.get<string[]>(
        `/appointments/available-times?date=${date}&branchId=${branchId}`
      );
      return response || [];
    } catch {
      try {
        const horasConfiguradas = await this.getAvailableTimesByBranch(branchId);

        if (horasConfiguradas.length === 0) {
          return [];
        }

        const horasDisponibles = horasConfiguradas
          .filter((h) => h.isActive)
          .map((h) => h.time)
          .sort();

        const horasLibres = await this.filterOccupiedHours(horasDisponibles, date, branchId);
        return horasLibres;
      } catch {
        return [];
      }
    }
  }

  /**
   * Filter occupied hours from available hours
   */
  private async filterOccupiedHours(horasDisponibles: string[], date: string, branchId: number): Promise<string[]> {
    try {
      const todasCitas = await this.getAppointments();
      const citasDelDia = todasCitas.filter(
        (cita) =>
          cita.appointmentDate.split('T')[0] === date &&
          cita.branchId === branchId &&
          cita.isActive &&
          cita.status !== 'Cancelled'
      );

      const horasOcupadas = citasDelDia.map((cita) => cita.appointmentTime);
      return horasDisponibles.filter((hora) => !horasOcupadas.includes(hora));
    } catch {
      return horasDisponibles;
    }
  }

  /**
   * Validate if a specific time is available
   */
  async validateAvailability(date: string, time: string, branchId: number): Promise<{ available: boolean }> {
    try {
      return await this.get<{ available: boolean }>(
        `/appointments/availability?date=${date}&time=${time}&branchId=${branchId}`
      );
    } catch {
      const horasDisponibles = await this.getAvailableHours(date, branchId);
      return { available: horasDisponibles.includes(time) };
    }
  }

  // ===== TIME FORMATTING UTILITIES =====

  /**
   * Convert time from 24h format to 12h format with AM/PM
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

    if (isNaN(hours) || isNaN(minutes) || hours < 0 || hours > 23 || minutes < 0 || minutes > 59) {
      return time;
    }

    const ampm = hours >= 12 ? 'PM' : 'AM';
    const hour12 = hours % 12 || 12;
    const formattedMinutes = minutes.toString().padStart(2, '0');

    return `${hour12}:${formattedMinutes} ${ampm}`;
  }

  /**
   * Convert time from 12h format to 24h format
   */
  convertTo24HourFormat(time12h: string): string {
    const [time, modifier] = time12h.split(' ');
    let [hours] = time.split(':');
    const minutes = time.split(':')[1];

    if (hours === '12') {
      hours = '00';
    }

    if (modifier === 'PM') {
      hours = (parseInt(hours, 10) + 12).toString();
    }

    return `${hours.padStart(2, '0')}:${minutes}`;
  }

  // ===== V1 API METHODS =====

  /**
   * Create appointment (V1 API)
   */
  async createAppointmentV1(dto: CreateAppointmentV1Dto): Promise<AppointmentDto> {
    return this.post<AppointmentDto>('/appointments', dto);
  }

  /**
   * Schedule appointment (V1 API with validations)
   */
  async scheduleAppointmentV1(dto: CreateAppointmentV1Dto): Promise<AppointmentDto> {
    return this.post<AppointmentDto>('/appointments/schedule', dto);
  }

  /**
   * Get available times (V1 API)
   */
  async getAvailableTimesV1(date: string, branchId: number): Promise<AvailableTimeDto[]> {
    return this.get<AvailableTimeDto[]>(`/appointments/available-times?date=${date}&branchId=${branchId}`);
  }

  /**
   * Validate availability (V1 API)
   */
  async validateAvailabilityV1(date: string, time: string, branchId: number): Promise<{ disponible: boolean }> {
    return this.get<{ disponible: boolean }>(
      `/appointments/availability?date=${date}&time=${time}&branchId=${branchId}`
    );
  }

  // ===== PUBLIC ENDPOINTS (NO AUTHENTICATION) =====

  /**
   * Schedule public appointment
   */
  async schedulePublicAppointment(appointmentData: PublicAppointmentRequestDto): Promise<PublicAppointmentResponseDto> {
    return this.publicPost<PublicAppointmentResponseDto>('/public/schedule-appointment', appointmentData);
  }

  /**
   * Query public appointment
   */
  async queryPublicAppointment(appointmentNumber: string, clientNumber: string): Promise<PublicAppointmentQueryDto> {
    return this.publicGet<PublicAppointmentQueryDto>(
      `/public/appointment/${appointmentNumber}?clientNumber=${clientNumber}`
    );
  }

  /**
   * Get public client appointments
   * FIXED: Cambiado de /clients/{number} a /client/{number} (singular)
   */
  async getPublicClientAppointments(clientNumber: string): Promise<AppointmentDto[]> {
    return this.publicGet<AppointmentDto[]>(`/public/client/${clientNumber}/appointments`);
  }

  /**
   * Cancel public appointment
   * FIXED: Cambiado de /clients/{number} a /client/{number} (singular)
   * FIXED: Cambiado campo 'cancellationReason' por 'Reason' (PascalCase) según CancelAppointmentDto del backend
   */
  async cancelPublicAppointment(
    clientNumber: string,
    appointmentId: number,
    cancellationReason: string
  ): Promise<{ message: string }> {
    return this.publicPatch<{ message: string }>(
      `/public/client/${clientNumber}/appointment/${appointmentId}/cancel`,
      { Reason: cancellationReason }
    );
  }

  /**
   * Verify appointment by QR code
   */
  async verifyAppointmentByQR(appointmentNumber: string, clientNumber: string): Promise<AppointmentQRVerificationDto> {
    return this.publicGet<AppointmentQRVerificationDto>(
      `/public/verify-appointment?number=${encodeURIComponent(appointmentNumber)}&client=${encodeURIComponent(clientNumber)}`
    );
  }

  /**
   * Verify appointment by QR (V1 API)
   * FIXED: Cambiado de /appointments/verify-qr a /public/verify-appointment
   */
  async verifyAppointmentByQRV1(appointmentNumber: string, clientNumber: string): Promise<AppointmentDto> {
    return this.publicGet<AppointmentDto>(
      `/public/verify-appointment?number=${appointmentNumber}&client=${clientNumber}`
    );
  }

  /**
   * Get public available times
   */
  async getPublicAvailableTimes(date: string, branchId: number): Promise<string[]> {
    return this.publicGet<string[]>(`/public/available-times?date=${date}&branchId=${branchId}`);
  }

  /**
   * Schedule appointment for new user (simple registration)
   */
  async scheduleAppointmentForNewUser(appointmentData: {
    userData: {
      fullName: string;
      documentType: string;
      documentNumber: string;
      email: string;
      phone: string;
      mobile: string;
      address: string;
    };
    branchId: number;
    appointmentTypeId: number;
    appointmentDate: string;
    appointmentTime: string;
    observations?: string;
  }): Promise<SimpleAppointmentResponseDto> {
    const requestData: SimpleAppointmentRequestDto = {
      DocumentType: appointmentData.userData.documentType || '',
      DocumentNumber: appointmentData.userData.documentNumber || '',
      FullName: appointmentData.userData.fullName || '',
      Phone: appointmentData.userData.phone || '',
      Mobile: appointmentData.userData.mobile || '',
      Email: appointmentData.userData.email || '',
      Address: appointmentData.userData.address || '',
      BranchId: Number(appointmentData.branchId) || 0,
      AppointmentTypeId: Number(appointmentData.appointmentTypeId) || 0,
      AppointmentDate: appointmentData.appointmentDate || '',
      AppointmentTime: appointmentData.appointmentTime?.includes(':')
        ? appointmentData.appointmentTime
        : appointmentData.appointmentTime || '',
      Observations: appointmentData.observations || '',
    };

    if (!requestData.DocumentNumber || !requestData.FullName || !requestData.Email) {
      throw new Error('Faltan datos obligatorios: documento, nombre o email');
    }

    if (!requestData.BranchId || !requestData.AppointmentTypeId) {
      throw new Error('Faltan IDs de sede o tipo de cita');
    }

    if (!requestData.AppointmentDate || !requestData.AppointmentTime) {
      throw new Error('Faltan fecha u hora de la cita');
    }

    return this.publicPost<SimpleAppointmentResponseDto>('/public/schedule-simple-appointment', requestData);
  }
}
