/**
 * Repository - Admin
 * Data access layer for admin panel
 * Follows MVVM pattern - only data operations, no business logic
 */

import { apiService } from '@/services/api';
import type {
  AppointmentDto,
  UpdateAppointmentDto,
  UserDto,
  CreateUserDto,
  UpdateUserDto,
  UpdateUserTabsDto,
  RolDto,
  CreateRolDto,
  UpdateRolDto,
  BranchDto,
  UpdateBranchDto,
  AppointmentTypeDto,
  UpdateAppointmentTypeDto,
  AvailableTimeDto,
  UpdateAvailableTimeDto,
  FormPermissionDto,
  PermissionViewDto,
  RolPermissionSummaryDto,
  UpdateRolFormPermissionDto,
  DashboardStatsDto,
  HolidayDto
} from '../models/admin.models';

/**
 * Admin repository for data access
 */
export class AdminRepository {
  // ============================================
  // AUTHENTICATION & USER SESSION
  // ============================================

  isAuthenticated(): boolean {
    return apiService.isAuthenticated();
  }

  async getCurrentUserFromServer(): Promise<UserDto | null> {
    return await apiService.getCurrentUserFromServer();
  }

  async getCurrentUserRolesFromServer(): Promise<string[] | null> {
    return await apiService.getCurrentUserRolesFromServer();
  }

  async getCurrentUserPermissionsFromServer(): Promise<{ forms: { [formCode: string]: FormPermissionDto } } | null> {
    return await apiService.getCurrentUserPermissionsFromServer();
  }

  async logout(): Promise<void> {
    return await apiService.logout();
  }

  // ============================================
  // APPOINTMENTS
  // ============================================

  async getAllAppointmentsIncludingInactive(): Promise<AppointmentDto[]> {
    return await apiService.getAllAppointmentsIncludingInactive();
  }

  async updateAppointment(data: UpdateAppointmentDto): Promise<{ success: boolean; message: string }> {
    return await apiService.updateAppointment(data);
  }

  // ============================================
  // USERS (EMPLOYEES)
  // ============================================

  async getUsers(): Promise<UserDto[]> {
    return await apiService.getUsers();
  }

  async createUser(data: CreateUserDto): Promise<UserDto> {
    return await apiService.createUser(data);
  }

  async updateUser(data: UpdateUserDto): Promise<UserDto> {
    return await apiService.updateUser(data);
  }

  async deleteLogicalUser(id: number): Promise<boolean> {
    return await apiService.deleteLogicalUser(id);
  }

  async activateUser(id: number): Promise<UserDto> {
    return await apiService.updateUser({ id, isActive: true } as UpdateUserDto);
  }

  async deleteUser(id: number): Promise<boolean> {
    return await apiService.deleteUser(id);
  }

  async updateUserTabs(data: UpdateUserTabsDto): Promise<{ success: boolean; message: string }> {
    return await apiService.updateUserTabs(data);
  }

  // ============================================
  // ROLES
  // ============================================

  async getRoles(): Promise<RolDto[]> {
    return await apiService.getRoles();
  }

  async createRol(data: CreateRolDto): Promise<RolDto> {
    return await apiService.createRol(data);
  }

  async updateRol(data: UpdateRolDto): Promise<{ success: boolean; message: string }> {
    return await apiService.updateRol(data);
  }

  async deleteLogicalRol(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.deleteLogicalRol(id);
  }

  async activateRol(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.updateRol({ id, isActive: true } as UpdateRolDto);
  }

  async deleteRol(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.deleteRol(id);
  }

  // ============================================
  // BRANCHES
  // ============================================

  async getBranches(): Promise<BranchDto[]> {
    return await apiService.getBranches();
  }

  async getAllBranchesIncludingInactive(): Promise<BranchDto[]> {
    return await apiService.getAllBranchesIncludingInactive();
  }

  async createBranch(data: Omit<BranchDto, 'id'>): Promise<BranchDto> {
    return await apiService.createBranch(data);
  }

  async updateBranch(data: UpdateBranchDto): Promise<{ success: boolean; message: string }> {
    return await apiService.updateBranch(data);
  }

  async deleteLogicalBranch(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.deleteLogicalBranch(id);
  }

  async activateBranch(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.updateBranch({ id, isActive: true } as UpdateBranchDto);
  }

  async deleteBranch(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.deleteBranch(id);
  }

  // ============================================
  // APPOINTMENT TYPES
  // ============================================

  async getAppointmentTypes(): Promise<AppointmentTypeDto[]> {
    return await apiService.getAppointmentTypes();
  }

  async getAllAppointmentTypesIncludingInactive(): Promise<AppointmentTypeDto[]> {
    return await apiService.getAllAppointmentTypesIncludingInactive();
  }

  async createAppointmentType(data: Omit<AppointmentTypeDto, 'id'>): Promise<AppointmentTypeDto> {
    return await apiService.createAppointmentType(data);
  }

  async updateAppointmentType(data: UpdateAppointmentTypeDto): Promise<{ success: boolean; message: string }> {
    return await apiService.updateAppointmentType(data);
  }

  async deleteLogicalAppointmentType(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.deleteLogicalAppointmentType(id);
  }

  async activateAppointmentType(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.updateAppointmentType({ id, isActive: true } as UpdateAppointmentTypeDto);
  }

  async deleteAppointmentType(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.deleteAppointmentType(id);
  }

  // ============================================
  // AVAILABLE TIMES
  // ============================================

  async getAllAvailableTimes(): Promise<AvailableTimeDto[]> {
    return await apiService.getAllAvailableTimes();
  }

  async getAllAvailableTimesIncludingInactive(): Promise<AvailableTimeDto[]> {
    return await apiService.getAllAvailableTimesIncludingInactive();
  }

  async createAvailableTime(data: Omit<AvailableTimeDto, 'id'>): Promise<AvailableTimeDto> {
    return await apiService.createAvailableTime(data);
  }

  async updateAvailableTime(data: UpdateAvailableTimeDto): Promise<{ success: boolean; message: string }> {
    return await apiService.updateAvailableTime(data);
  }

  async deleteLogicalAvailableTime(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.deleteLogicalAvailableTime(id);
  }

  async activateAvailableTime(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.updateAvailableTime({ id, isActive: true } as UpdateAvailableTimeDto);
  }

  async deleteAvailableTime(id: number): Promise<{ success: boolean; message: string }> {
    return await apiService.deleteAvailableTime(id);
  }

  // ============================================
  // PERMISSIONS
  // ============================================

  async getAllPermissions(): Promise<PermissionViewDto[]> {
    return await apiService.getAllPermissions();
  }

  async getAllRolPermissionsSummary(): Promise<RolPermissionSummaryDto[]> {
    const backendData = await apiService.getAllRolPermissionsSummary();

    // Transform backend format to frontend format
    return backendData.map(rolPerm => ({
      rolId: rolPerm.rolId,
      rolName: rolPerm.rolName,
      rolCode: rolPerm.rolCode,
      formPermissions: (rolPerm as any).permissions?.map((perm: any) => ({
        formId: perm.formId,
        formName: perm.formName,
        formCode: perm.formCode,
        assignedPermission: {
          canCreate: perm.canCreate,
          canRead: perm.canRead,
          canUpdate: perm.canUpdate,
          canDelete: perm.canDelete
        },
        hasPermission: perm.canCreate || perm.canRead || perm.canUpdate || perm.canDelete
      })) || []
    }));
  }

  async updateRolFormPermission(data: UpdateRolFormPermissionDto): Promise<{ success: boolean; message: string }> {
    return await apiService.updateRolFormPermission(data);
  }

  // ============================================
  // DASHBOARD STATS
  // ============================================

  async getDashboardStats(userId?: number): Promise<DashboardStatsDto> {
    try {
      // For non-admin users, use their assigned appointments
      const appointmentsPromise = userId
        ? this.getMyAssignedAppointments(userId)
        : this.getAllAppointmentsIncludingInactive();

      const [appointments, employees, branches, appointmentTypes] = await Promise.all([
        appointmentsPromise,
        this.getUsers(),
        this.getBranches(),
        this.getAppointmentTypes()
      ]);

      // Get today's date in YYYY-MM-DD format (local timezone)
      const today = new Date().toLocaleDateString('en-CA'); // en-CA gives YYYY-MM-DD format

      // Check for pending status values (database uses English uppercase)
      const isPendingStatus = (status: string) => {
        const pendingStatuses = ['PENDING', 'Pending', 'CONFIRMED', 'Confirmed'];
        return pendingStatuses.includes(status);
      };

      return {
        totalAppointments: appointments.length,
        completedAppointments: appointments.filter(a => a.status === 'COMPLETED' || a.status === 'Completed').length,
        pendingAppointments: appointments.filter(a => isPendingStatus(a.status)).length,
        totalEmployees: employees.filter(e => e.isActive).length,
        totalBranches: branches.filter(b => b.isActive).length,
        totalAppointmentTypes: appointmentTypes.filter(t => t.isActive).length,
        // Count pending/confirmed appointments for today only
        myAppointmentsToday: appointments.filter(a =>
          a.appointmentDate?.startsWith(today) &&
          isPendingStatus(a.status)
        ).length,
        // Count pending/confirmed appointments for today (same as myAppointmentsToday)
        myPendingAppointments: appointments.filter(a =>
          a.appointmentDate?.startsWith(today) &&
          isPendingStatus(a.status)
        ).length
      };
    } catch (error) {
      console.error('Error in getDashboardStats:', error);
      return {
        totalAppointments: 0,
        completedAppointments: 0,
        pendingAppointments: 0,
        totalEmployees: 0,
        totalBranches: 0,
        totalAppointmentTypes: 0,
        myAppointmentsToday: 0,
        myPendingAppointments: 0
      };
    }
  }

  // ============================================
  // ASSIGNED APPOINTMENTS (Employee View)
  // ============================================

  /**
   * Get appointments assigned to current employee based on their assigned appointment types
   * Uses backend endpoint: GET /api/v1/appointments/my-assigned
   * @param _userId - User ID (not needed, backend uses JWT token)
   */
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  async getMyAssignedAppointments(_userId: number): Promise<AppointmentDto[]> {
    const detailedAppointments = await apiService.getMyAssignedAppointments();

    // Map AppointmentDetailDto[] to AppointmentDto[] (flatten nested structure)
    return detailedAppointments.map(apt => ({
      id: apt.id,
      appointmentNumber: apt.appointmentNumber,
      appointmentDate: apt.appointmentDate,
      appointmentTime: apt.appointmentTime || '',
      status: apt.status,
      observations: apt.notes,
      completedDate: apt.completedDate,
      assignedTechnician: undefined,
      technicianObservations: undefined,
      clientId: apt.client.id,
      branchId: apt.branch.id,
      appointmentTypeId: apt.appointmentType.id,
      clientName: apt.client.fullName,
      clientNumber: apt.client.clientNumber || undefined,
      clientDocument: apt.client.documentNumber,
      branchName: apt.branch.name,
      branchAddress: apt.branch.address || undefined,
      appointmentTypeName: apt.appointmentType.name,
      appointmentTypeIcon: apt.appointmentType.iconName || undefined,
      createdAt: apt.createdAt,
      updatedAt: apt.updatedAt,
      isActive: apt.isActive
    }));
  }

  // ============================================
  // HOLIDAYS
  // ============================================

  async getHolidays(): Promise<HolidayDto[]> {
    return await apiService.getHolidays();
  }

  async getHolidayById(id: number): Promise<HolidayDto> {
    return await apiService.getHolidayById(id);
  }

  async createNationalHoliday(data: { holidayDate: string; holidayName: string }): Promise<HolidayDto> {
    return await apiService.createNationalHoliday(data);
  }

  async createLocalHoliday(data: { holidayDate: string; holidayName: string; branchId: number }): Promise<HolidayDto> {
    return await apiService.createLocalHoliday(data);
  }

  async createCompanyHoliday(data: { holidayDate: string; holidayName: string }): Promise<HolidayDto> {
    return await apiService.createCompanyHoliday(data);
  }

  async updateHoliday(data: { id: number; holidayDate: string; holidayName: string; holidayType: 'National' | 'Local' | 'Company'; branchId?: number }): Promise<HolidayDto> {
    return await apiService.updateHoliday(data);
  }

  async deleteHoliday(id: number): Promise<void> {
    return await apiService.deleteHoliday(id);
  }

  async activateHoliday(id: number): Promise<HolidayDto> {
    // Since there's no direct activate endpoint, we need to update the holiday
    // This assumes the backend sets isActive to true when updating
    const holiday = await apiService.getHolidayById(id);
    return await apiService.updateHoliday({
      id: holiday.id,
      holidayDate: holiday.holidayDate,
      holidayName: holiday.holidayName,
      holidayType: holiday.holidayType,
      branchId: holiday.branchId
    });
  }
}

/**
 * Singleton instance
 */
export const adminRepository = new AdminRepository();
