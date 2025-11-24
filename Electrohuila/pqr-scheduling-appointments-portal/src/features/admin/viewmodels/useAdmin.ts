/**
 * ViewModel - Admin Orchestrator
 * Orchestrates all focused admin hooks into a single interface
 * This hook is a lightweight orchestrator that delegates to focused, single-responsibility hooks
 */

'use client';

import { AdminRepository } from '../repositories/admin.repository';
import { useUsers } from './useUsers';
import { useRoles } from './useRoles';
import { usePermissions } from './usePermissions';
import { useAdminAppointments } from './useAdminAppointments';
import { useBranches } from './useBranches';
import { useAppointmentTypes } from './useAppointmentTypes';
import { useAvailableTimes } from './useAvailableTimes';
import { useHolidays } from './useHolidays';
import { useDashboardStats } from './useDashboardStats';
import { useUserPermissions } from './useUserPermissions';
import { useEmployeeTabPermissions } from './useEmployeeTabPermissions';
import { useAdminUI } from './useAdminUI';

/**
 * Main orchestrator hook for admin functionality
 * Usage: const admin = useAdmin(repository, userId);
 * Access sub-hooks: admin.users.fetchUsers(), admin.roles.createRole(), etc.
 */
export const useAdmin = (repository: AdminRepository, userId?: number) => {
  const users = useUsers(repository);
  const roles = useRoles(repository);
  const permissions = usePermissions(repository);
  const appointments = useAdminAppointments(repository, userId);
  const branches = useBranches(repository);
  const appointmentTypes = useAppointmentTypes(repository);
  const availableTimes = useAvailableTimes(repository);
  const holidays = useHolidays(repository);
  const dashboardStats = useDashboardStats(repository, userId);
  const userPermissions = useUserPermissions(repository);
  const employeeTabPermissions = useEmployeeTabPermissions(repository);
  const ui = useAdminUI();

  return {
    users,
    roles,
    permissions,
    appointments,
    branches,
    appointmentTypes,
    availableTimes,
    holidays,
    dashboardStats,
    userPermissions,
    employeeTabPermissions,
    ui,
    repository
  };
};

export type AdminViewModel = ReturnType<typeof useAdmin>;
