/**
 * Assignment Service
 * Handles user assignment to appointment types
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  UserAssignmentDto,
  CreateAssignmentDto,
  BulkAssignmentDto,
  AppointmentDetailDto,
} from './assignment.types';

export class AssignmentService extends BaseHttpService {
  // ===== USER ASSIGNMENT MANAGEMENT =====

  /**
   * Get assignments for a specific user
   */
  async getUserAssignments(userId: number): Promise<UserAssignmentDto[]> {
    return this.get<UserAssignmentDto[]>(`/user-assignments/users/${userId}/assignments`);
  }

  /**
   * Get all assignments (admin)
   */
  async getAllAssignments(): Promise<UserAssignmentDto[]> {
    return this.get<UserAssignmentDto[]>('/user-assignments/assignments');
  }

  /**
   * Assign an appointment type to a user
   */
  async assignUserToAppointmentType(dto: CreateAssignmentDto): Promise<UserAssignmentDto> {
    return this.post<UserAssignmentDto>(`/user-assignments/users/${dto.userId}/assign-appointment-type`, dto);
  }

  /**
   * Remove an assignment
   */
  async removeAssignment(assignmentId: number): Promise<{ success: boolean; message: string }> {
    return this.delete<{ success: boolean; message: string }>(`/user-assignments/assignments/${assignmentId}`);
  }

  /**
   * Assign multiple appointment types to a user
   */
  async bulkAssignUser(dto: BulkAssignmentDto): Promise<UserAssignmentDto[]> {
    return this.post<UserAssignmentDto[]>(`/user-assignments/users/${dto.userId}/bulk-assign`, dto);
  }

  /**
   * Get appointments assigned to current user (with full data)
   * This endpoint returns appointments filtered by the user's assigned appointment types
   */
  async getMyAssignedAppointments(): Promise<AppointmentDetailDto[]> {
    return this.get<AppointmentDetailDto[]>('/appointments/my-assigned');
  }
}
