/**
 * Services Index
 * Central export point for all services
 * Provides backward compatibility with the old monolithic API service
 */

import { AuthService } from './auth/auth.service';
import { AppointmentService } from './appointments/appointment.service';
import { UserService } from './users/user.service';
import { CatalogService } from './catalogs/catalog.service';
import { PermissionService } from './permissions/permission.service';
import { HolidayService } from './holidays/holiday.service';
import { NotificationService } from './notifications/notification.service';
import { SystemSettingService } from './system/system-setting.service';
import { appointmentDocumentService } from './documents/appointment-document.service';
import { AssignmentService } from './assignments/assignment.service';

// ===== SERVICE INSTANCES =====

export const authService = new AuthService();
export const appointmentService = new AppointmentService();
export const userService = new UserService();
export const catalogService = new CatalogService();
export const permissionService = new PermissionService();
export const holidayService = new HolidayService();
export const notificationService = new NotificationService();
export const systemSettingService = new SystemSettingService();
export const assignmentService = new AssignmentService();
export { appointmentDocumentService };

// ===== UNIFIED API SERVICE (BACKWARD COMPATIBILITY) =====

/**
 * Unified API Service
 * Provides a single point of access to all domain services
 * This maintains backward compatibility with the old monolithic ApiService
 */
export const apiService = {
  // Auth methods
  login: authService.login.bind(authService),
  refreshToken: authService.refreshToken.bind(authService),
  logout: authService.logout.bind(authService),
  isAuthenticated: authService.isAuthenticated.bind(authService),
  getCurrentUser: authService.getCurrentUser.bind(authService),
  getCurrentUserRoles: authService.getCurrentUserRoles.bind(authService),
  getCurrentUserPermissions: authService.getCurrentUserPermissions.bind(authService),
  getCurrentUserPermissionsDetailed: authService.getCurrentUserPermissionsDetailed.bind(authService),
  hasFormPermission: authService.hasFormPermission.bind(authService),
  getCurrentUserModules: authService.getCurrentUserModules.bind(authService),
  getCurrentUserFromServer: authService.getCurrentUserFromServer.bind(authService),
  getCurrentUserRolesFromServer: authService.getCurrentUserRolesFromServer.bind(authService),
  getCurrentUserPermissionsFromServer: authService.getCurrentUserPermissionsFromServer.bind(authService),

  // User methods
  getUsers: userService.getUsers.bind(userService),
  getUserById: userService.getUserById.bind(userService),
  createUser: userService.createUser.bind(userService),
  updateUser: userService.updateUser.bind(userService),
  deleteUser: userService.deleteUser.bind(userService),
  deleteLogicalUser: userService.deleteLogicalUser.bind(userService),
  activateUser: userService.activateUser.bind(userService),

  // Role methods
  getRoles: userService.getRoles.bind(userService),
  getRolById: userService.getRolById.bind(userService),
  createRol: userService.createRol.bind(userService),
  updateRol: userService.updateRol.bind(userService),
  deleteRol: userService.deleteRol.bind(userService),
  deleteLogicalRol: userService.deleteLogicalRol.bind(userService),
  activateRol: userService.activateRol.bind(userService),
  getRolByCode: userService.getRolByCode.bind(userService),
  getRolesByUserId: userService.getRolesByUserId.bind(userService),
  getAllRolesIncludingInactive: userService.getAllRolesIncludingInactive.bind(userService),

  // Tab permission methods
  getUserTabsPermissions: userService.getUserTabsPermissions.bind(userService),
  getUserTabsPermission: userService.getUserTabsPermission.bind(userService),
  updateUserTabs: userService.updateUserTabs.bind(userService),
  getAvailableTabs: userService.getAvailableTabs.bind(userService),

  // Appointment methods
  getAppointments: appointmentService.getAppointments.bind(appointmentService),
  getAppointmentById: appointmentService.getAppointmentById.bind(appointmentService),
  createAppointment: appointmentService.createAppointment.bind(appointmentService),
  updateAppointment: appointmentService.updateAppointment.bind(appointmentService),
  deleteAppointment: appointmentService.deleteAppointment.bind(appointmentService),
  deleteLogicalAppointment: appointmentService.deleteLogicalAppointment.bind(appointmentService),
  getPendingAppointments: appointmentService.getPendingAppointments.bind(appointmentService),
  getCompletedAppointments: appointmentService.getCompletedAppointments.bind(appointmentService),
  cancelAppointment: appointmentService.cancelAppointment.bind(appointmentService),
  completeAppointment: appointmentService.completeAppointment.bind(appointmentService),
  getAllAppointmentsIncludingInactive: appointmentService.getAllAppointmentsIncludingInactive.bind(appointmentService),

  // Available time methods
  getAllAvailableTimes: appointmentService.getAllAvailableTimes.bind(appointmentService),
  getAvailableTimeById: appointmentService.getAvailableTimeById.bind(appointmentService),
  createAvailableTime: appointmentService.createAvailableTime.bind(appointmentService),
  updateAvailableTime: appointmentService.updateAvailableTime.bind(appointmentService),
  deleteAvailableTime: appointmentService.deleteAvailableTime.bind(appointmentService),
  deleteLogicalAvailableTime: appointmentService.deleteLogicalAvailableTime.bind(appointmentService),
  activateAvailableTime: appointmentService.activateAvailableTime.bind(appointmentService),
  getAvailableTimesByBranch: appointmentService.getAvailableTimesByBranch.bind(appointmentService),
  getAvailableTimesByAppointmentType: appointmentService.getAvailableTimesByAppointmentType.bind(appointmentService),
  getAllAvailableTimesIncludingInactive: appointmentService.getAllAvailableTimesIncludingInactive.bind(appointmentService),
  getAvailableHours: appointmentService.getAvailableHours.bind(appointmentService),
  validateAvailability: appointmentService.validateAvailability.bind(appointmentService),

  // Time formatting methods
  formatTimeForDisplay: appointmentService.formatTimeForDisplay.bind(appointmentService),
  convertTo24HourFormat: appointmentService.convertTo24HourFormat.bind(appointmentService),

  // V1 Appointment methods
  createAppointmentV1: appointmentService.createAppointmentV1.bind(appointmentService),
  scheduleAppointmentV1: appointmentService.scheduleAppointmentV1.bind(appointmentService),
  getAvailableTimesV1: appointmentService.getAvailableTimesV1.bind(appointmentService),
  validateAvailabilityV1: appointmentService.validateAvailabilityV1.bind(appointmentService),
  verifyAppointmentByQRV1: appointmentService.verifyAppointmentByQRV1.bind(appointmentService),

  // Public appointment methods
  schedulePublicAppointment: appointmentService.schedulePublicAppointment.bind(appointmentService),
  queryPublicAppointment: appointmentService.queryPublicAppointment.bind(appointmentService),
  getPublicClientAppointments: appointmentService.getPublicClientAppointments.bind(appointmentService),
  cancelPublicAppointment: appointmentService.cancelPublicAppointment.bind(appointmentService),
  verifyAppointmentByQR: appointmentService.verifyAppointmentByQR.bind(appointmentService),
  getPublicAvailableTimes: appointmentService.getPublicAvailableTimes.bind(appointmentService),
  scheduleAppointmentForNewUser: appointmentService.scheduleAppointmentForNewUser.bind(appointmentService),
  getPublicClientAppointmentsByNumber: appointmentService.getPublicClientAppointments.bind(appointmentService),

  // Client methods
  getClients: catalogService.getClients.bind(catalogService),
  getClientById: catalogService.getClientById.bind(catalogService),
  createClient: catalogService.createClient.bind(catalogService),
  updateClient: catalogService.updateClient.bind(catalogService),
  deleteClient: catalogService.deleteClient.bind(catalogService),
  deleteLogicalClient: catalogService.deleteLogicalClient.bind(catalogService),
  activateClient: catalogService.activateClient.bind(catalogService),
  getClientByNumber: catalogService.getClientByNumber.bind(catalogService),
  getAllClientsIncludingInactive: catalogService.getAllClientsIncludingInactive.bind(catalogService),
  validatePublicClient: catalogService.validatePublicClient.bind(catalogService),
  validatePublicClientByNumber: catalogService.validatePublicClientByNumber.bind(catalogService),
  createClientV1: catalogService.createClientV1.bind(catalogService),

  // Branch methods
  getBranches: catalogService.getBranches.bind(catalogService),
  getBranchById: catalogService.getBranchById.bind(catalogService),
  createBranch: catalogService.createBranch.bind(catalogService),
  updateBranch: catalogService.updateBranch.bind(catalogService),
  deleteBranch: catalogService.deleteBranch.bind(catalogService),
  deleteLogicalBranch: catalogService.deleteLogicalBranch.bind(catalogService),
  activateBranch: catalogService.activateBranch.bind(catalogService),
  getActiveBranches: catalogService.getActiveBranches.bind(catalogService),
  getAllBranchesIncludingInactive: catalogService.getAllBranchesIncludingInactive.bind(catalogService),
  getPublicBranches: catalogService.getPublicBranches.bind(catalogService),
  getBranchesV1: catalogService.getBranchesV1.bind(catalogService),

  // Appointment type methods
  getAppointmentTypes: catalogService.getAppointmentTypes.bind(catalogService),
  getAppointmentTypeById: catalogService.getAppointmentTypeById.bind(catalogService),
  createAppointmentType: catalogService.createAppointmentType.bind(catalogService),
  updateAppointmentType: catalogService.updateAppointmentType.bind(catalogService),
  deleteAppointmentType: catalogService.deleteAppointmentType.bind(catalogService),
  deleteLogicalAppointmentType: catalogService.deleteLogicalAppointmentType.bind(catalogService),
  activateAppointmentType: catalogService.activateAppointmentType.bind(catalogService),
  getActiveAppointmentTypes: catalogService.getActiveAppointmentTypes.bind(catalogService),
  getAllAppointmentTypesIncludingInactive: catalogService.getAllAppointmentTypesIncludingInactive.bind(catalogService),
  getPublicAppointmentTypes: catalogService.getPublicAppointmentTypes.bind(catalogService),
  getAppointmentTypesV1: catalogService.getAppointmentTypesV1.bind(catalogService),

  // Catalog methods - Statuses
  getAppointmentStatuses: catalogService.getAppointmentStatuses.bind(catalogService),
  getAppointmentStatusById: catalogService.getAppointmentStatusById.bind(catalogService),
  getAppointmentStatusByCode: catalogService.getAppointmentStatusByCode.bind(catalogService),

  // Catalog methods - Document types
  getDocumentTypes: catalogService.getDocumentTypes.bind(catalogService),
  getDocumentTypeById: catalogService.getDocumentTypeById.bind(catalogService),
  getDocumentTypeByCode: catalogService.getDocumentTypeByCode.bind(catalogService),
  getDocumentTypesV1: catalogService.getDocumentTypesV1.bind(catalogService),

  // Catalog methods - Project types
  getProjectTypes: catalogService.getProjectTypes.bind(catalogService),
  getProjectTypeById: catalogService.getProjectTypeById.bind(catalogService),
  getProjectTypeByCode: catalogService.getProjectTypeByCode.bind(catalogService),

  // Catalog methods - New account statuses
  getNewAccountStatuses: catalogService.getNewAccountStatuses.bind(catalogService),
  getNewAccountStatusById: catalogService.getNewAccountStatusById.bind(catalogService),
  getNewAccountStatusByCode: catalogService.getNewAccountStatusByCode.bind(catalogService),

  // Catalog methods - Property types
  getPropertyTypes: catalogService.getPropertyTypes.bind(catalogService),
  getPropertyTypeById: catalogService.getPropertyTypeById.bind(catalogService),
  getPropertyTypeByCode: catalogService.getPropertyTypeByCode.bind(catalogService),

  // Catalog methods - Service use types
  getServiceUseTypes: catalogService.getServiceUseTypes.bind(catalogService),
  getServiceUseTypeById: catalogService.getServiceUseTypeById.bind(catalogService),
  getServiceUseTypeByCode: catalogService.getServiceUseTypeByCode.bind(catalogService),

  // New account request methods
  createNewAccountRequest: catalogService.createNewAccountRequest.bind(catalogService),
  requestNewAccountPublic: catalogService.requestNewAccountPublic.bind(catalogService),
  queryNewAccountRequestPublic: catalogService.queryNewAccountRequestPublic.bind(catalogService),

  // Project news methods
  createProjectNewsV1: catalogService.createProjectNewsV1.bind(catalogService),

  // Permission methods
  getAllPermissions: permissionService.getAllPermissions.bind(permissionService),
  getAllRolPermissionsSummary: permissionService.getAllRolPermissionsSummary.bind(permissionService),
  getRolPermissionsSummary: permissionService.getRolPermissionsSummary.bind(permissionService),
  getRolFormPermissions: permissionService.getRolFormPermissions.bind(permissionService),
  assignPermissionToRol: permissionService.assignPermissionToRol.bind(permissionService),
  removePermissionFromRol: permissionService.removePermissionFromRol.bind(permissionService),
  updateRolFormPermission: permissionService.updateRolFormPermission.bind(permissionService),
  createPermission: permissionService.createPermission.bind(permissionService),

  // Holiday methods
  getHolidays: holidayService.getHolidays.bind(holidayService),
  getHolidayById: holidayService.getHolidayById.bind(holidayService),
  getHolidaysInRange: holidayService.getHolidaysInRange.bind(holidayService),
  checkIfHoliday: holidayService.checkIfHoliday.bind(holidayService),
  createNationalHoliday: holidayService.createNationalHoliday.bind(holidayService),
  createLocalHoliday: holidayService.createLocalHoliday.bind(holidayService),
  createCompanyHoliday: holidayService.createCompanyHoliday.bind(holidayService),
  updateHoliday: holidayService.updateHoliday.bind(holidayService),
  deleteHoliday: holidayService.deleteHoliday.bind(holidayService),
  activateHoliday: holidayService.activateHoliday.bind(holidayService),
  deactivateHoliday: holidayService.deactivateHoliday.bind(holidayService),
  getHolidaysByType: holidayService.getHolidaysByType.bind(holidayService),
  getHolidaysByYear: holidayService.getHolidaysByYear.bind(holidayService),
  getHolidaysByMonthYear: holidayService.getHolidaysByMonthYear.bind(holidayService),
  getActiveHolidays: holidayService.getActiveHolidays.bind(holidayService),
  formatHolidayDateForDisplay: holidayService.formatDateForDisplay.bind(holidayService),
  formatHolidayDateForAPI: holidayService.formatDateForAPI.bind(holidayService),

  // System setting methods
  getSystemSettings: systemSettingService.getSystemSettings.bind(systemSettingService),
  getSystemSettingByKey: systemSettingService.getSystemSettingByKey.bind(systemSettingService),
  createSystemSetting: systemSettingService.createSystemSetting.bind(systemSettingService),
  updateSystemSetting: systemSettingService.updateSystemSetting.bind(systemSettingService),
  updateSystemSettingValue: systemSettingService.updateSystemSettingValue.bind(systemSettingService),
  getActiveSystemSettings: systemSettingService.getActiveSystemSettings.bind(systemSettingService),
  getSystemSettingsByType: systemSettingService.getSystemSettingsByType.bind(systemSettingService),
  parseSettingValue: systemSettingService.parseSettingValue.bind(systemSettingService),
  stringifySettingValue: systemSettingService.stringifySettingValue.bind(systemSettingService),

  // Appointment document methods
  getDocumentById: appointmentDocumentService.getDocumentById.bind(appointmentDocumentService),
  getDocumentsByAppointment: appointmentDocumentService.getDocumentsByAppointment.bind(appointmentDocumentService),
  getDocumentStats: appointmentDocumentService.getDocumentStats.bind(appointmentDocumentService),
  uploadDocument: appointmentDocumentService.uploadDocument.bind(appointmentDocumentService),
  uploadMultipleDocuments: appointmentDocumentService.uploadMultipleDocuments.bind(appointmentDocumentService),
  updateDocumentDescription: appointmentDocumentService.updateDocumentDescription.bind(appointmentDocumentService),
  deleteDocument: appointmentDocumentService.deleteDocument.bind(appointmentDocumentService),
  downloadDocument: appointmentDocumentService.downloadDocument.bind(appointmentDocumentService),
  getDocumentUrl: appointmentDocumentService.getDocumentUrl.bind(appointmentDocumentService),

  // User assignment methods
  getUserAssignments: assignmentService.getUserAssignments.bind(assignmentService),
  getAllAssignments: assignmentService.getAllAssignments.bind(assignmentService),
  assignUserToAppointmentType: assignmentService.assignUserToAppointmentType.bind(assignmentService),
  removeAssignment: assignmentService.removeAssignment.bind(assignmentService),
  bulkAssignUser: assignmentService.bulkAssignUser.bind(assignmentService),
  getMyAssignedAppointments: assignmentService.getMyAssignedAppointments.bind(assignmentService),
};

// ===== TYPE EXPORTS =====

// Auth types
export * from './auth/auth.types';

// Appointment types
export * from './appointments/appointment.types';

// User types
export * from './users/user.types';

// Catalog types
export * from './catalogs/catalog.types';

// Permission types
export * from './permissions/permission.types';

// Holiday types
export * from './holidays/holiday.types';

// System setting types
export * from './system/system-setting.types';

// Document types
export * from './documents/appointment-document.types';

// Assignment types
export * from './assignments/assignment.types';

// ===== DEFAULT EXPORT (for backward compatibility) =====

export default apiService;
