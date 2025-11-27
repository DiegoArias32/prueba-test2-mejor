/**
 * Models - Verificar Cita
 * DTOs e interfaces para verificación de citas por QR
 */

export interface VerificacionCliente {
  clientNumber: string;
  fullName: string;
}

export interface VerificacionSede {
  name: string;
  address: string;
}

export interface VerificacionTipoCita {
  name: string;
  icon: string;
}

export interface VerificacionCita {
  isValid: boolean;
  appointmentNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  statusDescription: string;
  client?: VerificacionCliente;
  branch?: VerificacionSede;
  appointmentType?: VerificacionTipoCita;
  createdAt: string;
  observations?: string;
  message: string;
}
