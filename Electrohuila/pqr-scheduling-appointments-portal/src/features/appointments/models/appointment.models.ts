/**
 * Modelos del feature de agendamiento de citas
 */

import type { ClientDto } from '@/core/types';

export interface AppointmentFormData {
  documentType: string;
  reason: string;
  branch: string;
  appointmentDate: string;
  appointmentTime: string;
  observations: string;
}

// Alias para compatibilidad
export type DatosFormularioCita = AppointmentFormData;

export interface AppointmentConfirmation {
  ticketNumber: string;
  issueDateTime: string;
  clientData: ClientDto;
  formData: AppointmentFormData;
}

export interface AppointmentScheduleRequest {
  clientNumber: string;
  branchId: number;
  appointmentTypeId: number;
  appointmentDate: string;
  appointmentTime: string;
  observations?: string;
}

export interface AppointmentScheduleResponse {
  appointmentNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  message: string;
}

export type AppointmentStep = 'client' | 'form' | 'confirmation';
