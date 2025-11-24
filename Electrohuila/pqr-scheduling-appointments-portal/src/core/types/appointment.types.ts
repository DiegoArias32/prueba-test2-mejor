/**
 * Tipos y DTOs relacionados con citas
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

export interface ClientDto {
  id: number;
  clientNumber: string;
  documentType: string;
  documentNumber: string;
  fullName: string;
  email: string;
  phone?: string;
  mobile?: string;
  address?: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateClientDto {
  id: number;
  clientNumber?: string;
  documentType?: string;
  documentNumber?: string;
  fullName?: string;
  email?: string;
  phone?: string;
  mobile?: string;
  address?: string;
}

export interface BranchDto {
  id: number;
  name: string;
  code: string;
  address: string;
  phone?: string;
  city: string;
  state: string;
  isMain: boolean;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateBranchDto {
  id: number;
  name?: string;
  code?: string;
  address?: string;
  phone?: string;
  city?: string;
  state?: string;
  isMain?: boolean;
}

export interface AppointmentTypeDto {
  id: number;
  name: string;
  description?: string;
  icon?: string;
  estimatedTimeMinutes: number;
  requiresDocumentation: boolean;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isEnabled: boolean;
}

export interface UpdateAppointmentTypeDto {
  id: number;
  name?: string;
  description?: string;
  icon?: string;
  estimatedTimeMinutes?: number;
  requiresDocumentation?: boolean;
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
