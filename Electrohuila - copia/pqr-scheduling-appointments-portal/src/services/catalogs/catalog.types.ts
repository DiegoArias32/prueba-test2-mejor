/**
 * Catalog Types
 * All DTOs related to catalogs (branches, appointment types, statuses, etc.)
 */

export interface BranchDto {
  id: number;
  name: string;
  code: string;
  address: string;
  phone?: string;
  city: string;
  state: string;
  isMain: boolean;
  colorPrimary?: string;
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
  code: string;
  description?: string;
  icon?: string;
  iconName?: string;
  colorPrimary?: string;
  colorSecondary?: string;
  estimatedTimeMinutes: number;
  requiresDocumentation: boolean;
  displayOrder: number;
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

export interface AppointmentStatusDto {
  id: number;
  code: string;
  name: string;
  description?: string;
  colorPrimary?: string;
  colorSecondary?: string;
  colorText?: string;
  iconName?: string;
  displayOrder: number;
  allowCancellation: boolean;
  isFinalState: boolean;
  isActive: boolean;
}

export interface DocumentTypeDto {
  id: number;
  code: string;
  name: string;
  abbreviation?: string;
  minLength?: number;
  maxLength?: number;
  requiresVerification: boolean;
  displayOrder: number;
  isActive: boolean;
}

export interface ProjectTypeDto {
  id: number;
  code: string;
  name: string;
  description?: string;
  iconName?: string;
  colorPrimary?: string;
  displayOrder: number;
  isActive: boolean;
}

export interface NewAccountStatusDto {
  id: number;
  code: string;
  name: string;
  colorPrimary?: string;
  iconName?: string;
  displayOrder: number;
  isActive: boolean;
}

export interface PropertyTypeDto {
  id: number;
  code: string;
  name: string;
  iconName?: string;
  displayOrder: number;
  isActive: boolean;
}

export interface ServiceUseTypeDto {
  id: number;
  code: string;
  name: string;
  displayOrder: number;
  isActive: boolean;
}

export interface PublicClientDto {
  clientNumber: string;
  fullName: string;
  email: string;
  documentNumber: string;
  phone: string;
  mobile: string;
  id: number;
}

export interface CreateClientV1Dto {
  documentType: string;
  documentNumber: string;
  fullName: string;
  email: string;
  phone: string;
  mobile: string;
  address: string;
}

export interface NewAccountRequestDto {
  documentType?: string;
  documentNumber: string;
  fullName: string;
  phone?: string;
  mobile: string;
  email: string;
  propertyType?: string;
  address: string;
  neighborhood?: string;
  municipality?: string;
  stratum?: number;
  squareMeters?: number;
  apartmentNumber?: string;
  serviceUse?: string;
  installationType?: string;
  requiredLoad?: string;
  connectionType?: string;
  hasTransformer?: string;
  networkDistance?: string;
  branchId: number;
  appointmentDate: string;
  appointmentTime: string;
  observations?: string;
}

export interface NewAccountQueryResponseDto {
  requestNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  fullName: string;
  address: string;
  municipality: string;
  branchName: string;
  installationType: string;
  serviceUse: string;
  creationDate: string;
  observations?: string;
}

export interface ProjectNewsDto {
  documentType: string;
  documentNumber: string;
  fullName: string;
  phone?: string;
  mobile: string;
  email: string;
  projectName: string;
  sector: string;
  municipality: string;
  descriptionProject: string;
  branchId: number;
  appointmentDate: string;
}
