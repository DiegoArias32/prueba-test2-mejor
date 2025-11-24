/**
 * Catalog Service
 * Handles all catalog operations: branches, appointment types, clients, statuses, document types, etc.
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  BranchDto,
  UpdateBranchDto,
  AppointmentTypeDto,
  UpdateAppointmentTypeDto,
  ClientDto,
  UpdateClientDto,
  AppointmentStatusDto,
  DocumentTypeDto,
  ProjectTypeDto,
  NewAccountStatusDto,
  PropertyTypeDto,
  ServiceUseTypeDto,
  PublicClientDto,
  CreateClientV1Dto,
  NewAccountRequestDto,
  NewAccountQueryResponseDto,
  ProjectNewsDto,
} from './catalog.types';

export class CatalogService extends BaseHttpService {
  // ===== BRANCH MANAGEMENT =====

  /**
   * Get all branches
   */
  async getBranches(): Promise<BranchDto[]> {
    return this.get<BranchDto[]>('/branches');
  }

  /**
   * Get branch by ID
   */
  async getBranchById(id: number): Promise<BranchDto> {
    return this.get<BranchDto>(`/branches/${id}`);
  }

  /**
   * Create new branch
   */
  async createBranch(branch: Omit<BranchDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<BranchDto> {
    return this.post<BranchDto>('/branches', branch);
  }

  /**
   * Update branch
   * FIXED: Cambiado de /branches/update a /branches/{id} con PATCH
   */
  async updateBranch(branch: UpdateBranchDto): Promise<{ success: boolean; message: string }> {
    if (!branch.id) {
      throw new Error('Branch ID is required for update');
    }
    return this.patch<{ success: boolean; message: string }>(`/branches/${branch.id}`, branch);
  }

  /**
   * Delete branch (hard delete)
   */
  async deleteBranch(id: number): Promise<{ success: boolean; message: string }> {
    return this.delete<{ success: boolean; message: string }>(`/branches/${id}`);
  }

  /**
   * Delete branch (logical delete)
   * TODO: Este endpoint no existe en la API backend - necesita ser implementado
   */
  async deleteLogicalBranch(id: number): Promise<{ success: boolean; message: string }> {
    throw new Error('⚠️ Endpoint /branches/delete-logical/{id} no implementado en la API backend');
    // return this.patch<{ success: boolean; message: string }>(`/branches/delete-logical/${id}`);
  }

  /**
   * Get active branches only
   */
  async getActiveBranches(): Promise<BranchDto[]> {
    return this.get<BranchDto[]>('/branches/active');
  }

  /**
   * Get all branches including inactive
   * TODO: Este endpoint no existe en la API backend - usar /branches/active como alternativa
   */
  async getAllBranchesIncludingInactive(): Promise<BranchDto[]> {
    // return this.get<BranchDto[]>('/branches/all-including-inactive');
    console.warn('⚠️ Endpoint /branches/all-including-inactive no existe. Usando /branches/active como fallback');
    return this.get<BranchDto[]>('/branches/active');
  }

  /**
   * Get public branches (no authentication)
   */
  async getPublicBranches(): Promise<BranchDto[]> {
    return this.publicGet<BranchDto[]>('/public/branches');
  }

  /**
   * Get branches (V1 API)
   */
  async getBranchesV1(): Promise<BranchDto[]> {
    return this.get<BranchDto[]>('/branches');
  }

  // ===== APPOINTMENT TYPE MANAGEMENT =====

  /**
   * Get all appointment types
   */
  async getAppointmentTypes(): Promise<AppointmentTypeDto[]> {
    return this.get<AppointmentTypeDto[]>('/appointmenttypes');
  }

  /**
   * Get appointment type by ID
   */
  async getAppointmentTypeById(id: number): Promise<AppointmentTypeDto> {
    return this.get<AppointmentTypeDto>(`/appointmenttypes/${id}`);
  }

  /**
   * Create new appointment type
   */
  async createAppointmentType(
    appointmentType: Omit<AppointmentTypeDto, 'id' | 'createdAt' | 'updatedAt'>
  ): Promise<AppointmentTypeDto> {
    return this.post<AppointmentTypeDto>('/appointmenttypes', appointmentType);
  }

  /**
   * Update appointment type
   * FIXED: Cambiado de /appointmenttypes/update a /appointmenttypes/{id} con PATCH
   */
  async updateAppointmentType(
    appointmentType: UpdateAppointmentTypeDto
  ): Promise<{ success: boolean; message: string }> {
    if (!appointmentType.id) {
      throw new Error('AppointmentType ID is required for update');
    }
    return this.patch<{ success: boolean; message: string }>(`/appointmenttypes/${appointmentType.id}`, appointmentType);
  }

  /**
   * Delete appointment type (hard delete)
   */
  async deleteAppointmentType(id: number): Promise<{ success: boolean; message: string }> {
    return this.delete<{ success: boolean; message: string }>(`/appointmenttypes/${id}`);
  }

  /**
   * Delete appointment type (logical delete)
   * TODO: Este endpoint no existe en la API backend - necesita ser implementado
   */
  async deleteLogicalAppointmentType(id: number): Promise<{ success: boolean; message: string }> {
    throw new Error('⚠️ Endpoint /appointmenttypes/delete-logical/{id} no implementado en la API backend');
    // return this.patch<{ success: boolean; message: string }>(`/appointmenttypes/delete-logical/${id}`);
  }

  /**
   * Get active appointment types only
   */
  async getActiveAppointmentTypes(): Promise<AppointmentTypeDto[]> {
    return this.get<AppointmentTypeDto[]>('/appointmenttypes/active');
  }

  /**
   * Get all appointment types including inactive
   * TODO: Este endpoint no existe en la API backend - usar /appointmenttypes/active como alternativa
   */
  async getAllAppointmentTypesIncludingInactive(): Promise<AppointmentTypeDto[]> {
    // return this.get<AppointmentTypeDto[]>('/appointmenttypes/all-including-inactive');
    console.warn('⚠️ Endpoint /appointmenttypes/all-including-inactive no existe. Usando /appointmenttypes/active como fallback');
    return this.get<AppointmentTypeDto[]>('/appointmenttypes/active');
  }

  /**
   * Get public appointment types (no authentication)
   */
  async getPublicAppointmentTypes(): Promise<AppointmentTypeDto[]> {
    return this.publicGet<AppointmentTypeDto[]>('/public/appointment-types');
  }

  /**
   * Get appointment types (V1 API)
   */
  async getAppointmentTypesV1(): Promise<AppointmentTypeDto[]> {
    return this.get<AppointmentTypeDto[]>('/appointmenttypes');
  }

  // ===== CLIENT MANAGEMENT =====

  /**
   * Get all clients
   */
  async getClients(): Promise<ClientDto[]> {
    return this.get<ClientDto[]>('/clients');
  }

  /**
   * Get client by ID
   */
  async getClientById(id: number): Promise<ClientDto> {
    return this.get<ClientDto>(`/clients/${id}`);
  }

  /**
   * Create new client
   */
  async createClient(client: Omit<ClientDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<ClientDto> {
    return this.post<ClientDto>('/clients', client);
  }

  /**
   * Update client
   * FIXED: Cambiado de /clients/update a /clients/{id} con PATCH
   */
  async updateClient(client: UpdateClientDto): Promise<{ success: boolean; message: string }> {
    if (!client.id) {
      throw new Error('Client ID is required for update');
    }
    return this.patch<{ success: boolean; message: string }>(`/clients/${client.id}`, client);
  }

  /**
   * Delete client (hard delete)
   */
  async deleteClient(id: number): Promise<{ success: boolean; message: string }> {
    return this.delete<{ success: boolean; message: string }>(`/clients/${id}`);
  }

  /**
   * Delete client (logical delete)
   * TODO: Este endpoint no existe en la API backend - necesita ser implementado
   */
  async deleteLogicalClient(id: number): Promise<{ success: boolean; message: string }> {
    throw new Error('⚠️ Endpoint /clients/delete-logical/{id} no implementado en la API backend');
    // return this.patch<{ success: boolean; message: string }>(`/clients/delete-logical/${id}`);
  }

  /**
   * Get client by client number
   */
  async getClientByNumber(clientNumber: string): Promise<ClientDto> {
    return this.get<ClientDto>(`/clients/number/${clientNumber}`);
  }

  /**
   * Get all clients including inactive
   * TODO: Este endpoint no existe en la API backend - usar /clients como alternativa
   */
  async getAllClientsIncludingInactive(): Promise<ClientDto[]> {
    // return this.get<ClientDto[]>('/clients/all-including-inactive');
    console.warn('⚠️ Endpoint /clients/all-including-inactive no existe. Usando /clients como fallback');
    return this.get<ClientDto[]>('/clients');
  }

  /**
   * Validate public client
   */
  async validatePublicClient(clientNumber: string): Promise<ClientDto> {
    return this.publicGet<ClientDto>(`/public/client/validate/${clientNumber}`);
  }

  /**
   * Validate public client by number
   */
  async validatePublicClientByNumber(clientNumber: string): Promise<PublicClientDto> {
    return this.publicGet<PublicClientDto>(`/public/client/validate/${encodeURIComponent(clientNumber)}`);
  }

  /**
   * Create client (V1 API)
   */
  async createClientV1(dto: CreateClientV1Dto): Promise<ClientDto> {
    return this.post<ClientDto>('/clients', dto);
  }

  // ===== APPOINTMENT STATUS CATALOG =====

  /**
   * Get all appointment statuses
   */
  async getAppointmentStatuses(): Promise<AppointmentStatusDto[]> {
    return this.get<AppointmentStatusDto[]>('/appointmentstatuses');
  }

  /**
   * Get appointment status by ID
   */
  async getAppointmentStatusById(id: number): Promise<AppointmentStatusDto> {
    return this.get<AppointmentStatusDto>(`/appointmentstatuses/${id}`);
  }

  /**
   * Get appointment status by code
   */
  async getAppointmentStatusByCode(code: string): Promise<AppointmentStatusDto> {
    return this.get<AppointmentStatusDto>(`/appointmentstatuses/code/${code}`);
  }

  // ===== DOCUMENT TYPE CATALOG =====

  /**
   * Get all document types
   */
  async getDocumentTypes(): Promise<DocumentTypeDto[]> {
    return this.get<DocumentTypeDto[]>('/documenttypes');
  }

  /**
   * Get document type by ID
   */
  async getDocumentTypeById(id: number): Promise<DocumentTypeDto> {
    return this.get<DocumentTypeDto>(`/documenttypes/${id}`);
  }

  /**
   * Get document type by code
   */
  async getDocumentTypeByCode(code: string): Promise<DocumentTypeDto> {
    return this.get<DocumentTypeDto>(`/documenttypes/code/${code}`);
  }

  /**
   * Get document types (V1 API)
   */
  async getDocumentTypesV1(): Promise<DocumentTypeDto[]> {
    return this.get<DocumentTypeDto[]>('/documenttypes');
  }

  // ===== PROJECT TYPE CATALOG =====

  /**
   * Get all project types
   */
  async getProjectTypes(): Promise<ProjectTypeDto[]> {
    return this.get<ProjectTypeDto[]>('/projecttypes');
  }

  /**
   * Get project type by ID
   */
  async getProjectTypeById(id: number): Promise<ProjectTypeDto> {
    return this.get<ProjectTypeDto>(`/projecttypes/${id}`);
  }

  /**
   * Get project type by code
   */
  async getProjectTypeByCode(code: string): Promise<ProjectTypeDto> {
    return this.get<ProjectTypeDto>(`/projecttypes/code/${code}`);
  }

  // ===== NEW ACCOUNT STATUS CATALOG =====

  /**
   * Get all new account statuses
   */
  async getNewAccountStatuses(): Promise<NewAccountStatusDto[]> {
    return this.get<NewAccountStatusDto[]>('/newaccountstatuses');
  }

  /**
   * Get new account status by ID
   */
  async getNewAccountStatusById(id: number): Promise<NewAccountStatusDto> {
    return this.get<NewAccountStatusDto>(`/newaccountstatuses/${id}`);
  }

  /**
   * Get new account status by code
   */
  async getNewAccountStatusByCode(code: string): Promise<NewAccountStatusDto> {
    return this.get<NewAccountStatusDto>(`/newaccountstatuses/code/${code}`);
  }

  // ===== PROPERTY TYPE CATALOG =====

  /**
   * Get all property types
   */
  async getPropertyTypes(): Promise<PropertyTypeDto[]> {
    return this.get<PropertyTypeDto[]>('/propertytypes');
  }

  /**
   * Get property type by ID
   */
  async getPropertyTypeById(id: number): Promise<PropertyTypeDto> {
    return this.get<PropertyTypeDto>(`/propertytypes/${id}`);
  }

  /**
   * Get property type by code
   */
  async getPropertyTypeByCode(code: string): Promise<PropertyTypeDto> {
    return this.get<PropertyTypeDto>(`/propertytypes/code/${code}`);
  }

  // ===== SERVICE USE TYPE CATALOG =====

  /**
   * Get all service use types
   */
  async getServiceUseTypes(): Promise<ServiceUseTypeDto[]> {
    return this.get<ServiceUseTypeDto[]>('/serviceusetypes');
  }

  /**
   * Get service use type by ID
   */
  async getServiceUseTypeById(id: number): Promise<ServiceUseTypeDto> {
    return this.get<ServiceUseTypeDto>(`/serviceusetypes/${id}`);
  }

  /**
   * Get service use type by code
   */
  async getServiceUseTypeByCode(code: string): Promise<ServiceUseTypeDto> {
    return this.get<ServiceUseTypeDto>(`/serviceusetypes/code/${code}`);
  }

  // ===== NEW ACCOUNT REQUESTS (PUBLIC) =====

  /**
   * Create new account request
   */
  async createNewAccountRequest(dto: {
    documentTypeId?: number;
    documentNumber?: string;
    fullName: string;
    phone?: string;
    mobile?: string;
    email?: string;
    address?: string;
    branchId?: number;
    appointmentDate?: string;
    appointmentTime?: string;
    observations?: string;
  }): Promise<{ message: string; data?: unknown }> {
    return this.publicPost<{ message: string; data?: unknown }>('/newaccounts', dto);
  }

  /**
   * Request new account (public)
   */
  async requestNewAccountPublic(requestData: NewAccountRequestDto): Promise<{
    requestNumber: string;
    appointmentDate: string;
    appointmentTime: string;
    status: string;
    message: string;
  }> {
    return this.publicPost<{
      requestNumber: string;
      appointmentDate: string;
      appointmentTime: string;
      status: string;
      message: string;
    }>('/public/request-new-account', requestData);
  }

  /**
   * Query new account request (public)
   */
  async queryNewAccountRequestPublic(
    requestNumber: string,
    documentNumber: string
  ): Promise<NewAccountQueryResponseDto> {
    return this.publicGet<NewAccountQueryResponseDto>(
      `/public/query-new-account-request?requestNumber=${requestNumber}&documentNumber=${documentNumber}`
    );
  }

  // ===== PROJECT NEWS (PUBLIC) =====

  /**
   * Create project news (V1 API)
   */
  async createProjectNewsV1(dto: ProjectNewsDto): Promise<{ message: string; data?: unknown }> {
    return this.publicPost<{ message: string; data?: unknown }>('/projectnews', dto);
  }
}
