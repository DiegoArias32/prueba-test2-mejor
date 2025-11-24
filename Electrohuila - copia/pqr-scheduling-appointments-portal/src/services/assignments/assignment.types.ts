/**
 * Types for user assignment to appointment types
 */

export interface UserAssignmentDto {
  id: number;
  userId: number;
  username: string;
  email: string;
  appointmentTypeId: number;
  appointmentTypeCode: string;
  appointmentTypeName: string;
  appointmentTypeDescription?: string;
  appointmentTypeIcon?: string;
  appointmentTypeColor?: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
}

export interface CreateAssignmentDto {
  userId: number;
  appointmentTypeId: number;
}

export interface BulkAssignmentDto {
  userId: number;
  appointmentTypeIds: number[];
}

// Summary DTOs for appointments with full data
export interface ClientSummaryDto {
  id: number;
  clientNumber?: string;
  fullName: string;
  email?: string;
  phoneNumber?: string;
  documentNumber: string;
}

export interface BranchSummaryDto {
  id: number;
  code: string;
  name: string;
  address?: string;
  city?: string;
}

export interface AppointmentTypeSummaryDto {
  id: number;
  code: string;
  name: string;
  description?: string;
  iconName?: string;
  colorPrimary?: string;
  colorSecondary?: string;
}

export interface AppointmentDetailDto {
  id: number;
  appointmentNumber: string;
  appointmentDate: string;
  appointmentTime?: string;
  status: string;
  notes?: string;
  completedDate?: string;
  cancellationReason?: string;
  client: ClientSummaryDto;
  branch: BranchSummaryDto;
  appointmentType: AppointmentTypeSummaryDto;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
}
