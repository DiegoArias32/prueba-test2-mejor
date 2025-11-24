/**
 * Appointment Types
 * All DTOs related to appointments and scheduling
 */

export interface AppointmentDto {
  id: number;
  appointmentNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  observations?: string;
  completedDate?: string;
  assignedTechnician?: string;
  technicianObservations?: string;
  clientId: number;
  branchId: number;
  appointmentTypeId: number;
  clientName?: string;
  clientNumber?: string;
  clientDocument?: string;
  branchName?: string;
  branchAddress?: string;
  appointmentTypeName?: string;
  appointmentTypeIcon?: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateAppointmentDto {
  id: number;
  appointmentNumber?: string;
  appointmentDate?: string;
  appointmentTime?: string;
  status?: string;
  observations?: string;
  completedDate?: string;
  assignedTechnician?: string;
  technicianObservations?: string;
  clientId?: number;
  branchId?: number;
  appointmentTypeId?: number;
}

export interface AvailableTimeDto {
  id: number;
  time: string;
  branchId: number;
  appointmentTypeId?: number;
  branchName?: string;
  appointmentTypeName?: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateAvailableTimeDto {
  id: number;
  time?: string;
  branchId?: number;
  appointmentTypeId?: number;
}

export interface CreateAppointmentV1Dto {
  appointmentDate: string;
  appointmentTime?: string;
  notes?: string;
  clientId: number;
  branchId: number;
  appointmentTypeId: number;
}

export interface PublicAppointmentRequestDto {
  clientNumber: string;
  branchId: number;
  appointmentTypeId: number;
  appointmentDate: string;
  appointmentTime: string;
  observations?: string;
}

export interface PublicAppointmentResponseDto {
  appointmentNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  message: string;
}

export interface PublicAppointmentQueryDto {
  appointmentNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  observations?: string;
  completedDate?: string;
  branch: { name: string };
  appointmentType: { name: string; icon: string };
}

export interface AppointmentQRVerificationDto {
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
}

export interface SimpleAppointmentRequestDto {
  DocumentType: string;
  DocumentNumber: string;
  FullName: string;
  Phone: string;
  Mobile: string;
  Email: string;
  Address: string;
  BranchId: number;
  AppointmentTypeId: number;
  AppointmentDate: string;
  AppointmentTime: string;
  Observations?: string;
}

export interface SimpleAppointmentResponseDto {
  requestNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  message: string;
}
