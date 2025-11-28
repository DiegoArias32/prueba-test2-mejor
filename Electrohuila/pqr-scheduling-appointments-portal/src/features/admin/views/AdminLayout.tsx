/**
 * @file AdminLayout.tsx
 * @description Main admin layout with routing and view management
 * @module features/admin/views
 *
 * ARCHITECTURE: Modular layout with lazy-loaded sub-views
 * - Manages sidebar navigation and view routing
 * - Handles WebSocket connections at layout level
 * - Delegates CRUD operations to sub-views
 */

'use client';

import React, { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { FiUser, FiAlertCircle, FiCheckCircle, FiRefreshCw } from 'react-icons/fi';
import Link from 'next/link';

// Components
import {
  Sidebar,
  Dashboard,
  type PermissionUpdate
} from './components';
import { EnhancedNotificationBell } from './components/EnhancedNotificationBell';
import { useEnhancedNotifications } from '../hooks/useEnhancedNotifications';
import { WebSocketStatus } from '@/shared/components/WebSocketStatus/WebSocketStatus';

// Sub-views
import { UsersView } from './users/UsersView';
import { RolesView } from './roles/RolesView';
import { AppointmentsView } from './appointments/AppointmentsView';
import { BranchesView } from './branches/BranchesView';
import { TypesView } from './types/TypesView';
import { TimesView } from './times/TimesView';
import { PermissionsView } from './permissions/PermissionsView';
import { HolidaysView } from './holidays/HolidaysView';
import { SystemSettingsView } from './settings/SystemSettingsView';
import { MyAssignedAppointmentsView } from './myappointments/MyAssignedAppointmentsView';
import { EmployeeAssignmentsManager } from './employees/EmployeeAssignmentsManager';

// Services & Utils
import { ToastContainer } from '@/shared/components/Toast/ToastContainer';
import { useToastNotifications } from '@/shared/hooks/useToastNotifications';
import { useToast } from '@/shared/components/Toast';
import { useConfirm } from '@/shared/hooks/useConfirm';
import { adminRepository } from '../repositories/admin.repository';
import { useAdmin } from '../viewmodels/useAdmin';
import { useWebSocket } from '@/services/websocket.service';
import { websocketService as signalRService } from '@/services/websocket.service';
import type { TabType } from '@/features/admin';
import type { RolPermissionSummaryDto, UserDto, RolDto, BranchDto, AppointmentTypeDto, AvailableTimeDto, AppointmentDto, HolidayDto } from '@/services/api';

type SectionType = 'dashboard' | 'my-appointments' | TabType;

export const AdminLayout: React.FC = () => {
  const router = useRouter();

  // Get current user first to pass to admin hook
  const [currentUserId, setCurrentUserId] = React.useState<number | undefined>(undefined);

  // Admin ViewModel Hook - pass userId for appointment filtering
  const admin = useAdmin(adminRepository, currentUserId);

  // Destructure from nested hooks
  const currentUser = admin.userPermissions.currentUser;

  // Update currentUserId when user loads
  React.useEffect(() => {
    if (currentUser?.id && currentUserId !== currentUser.id) {
      setCurrentUserId(currentUser.id);
    }
  }, [currentUser?.id, currentUserId]);
  const hasPermission = admin.userPermissions.hasPermission;
  const getAvailableTabs = admin.userPermissions.getAvailableTabs;
  const handleToggleAppointmentAttendance = admin.appointments.toggleAttendance;
  const employees = admin.users.users;
  const users = admin.users.users;
  const roles = admin.roles.roles;
  const branches = admin.branches.branches;
  const appointmentTypes = admin.appointmentTypes.appointmentTypes;
  const availableTimes = admin.availableTimes.availableTimes;
  const appointments = admin.appointments.appointments;
  const holidays = admin.holidays.holidays;
  const permissions = admin.permissions.permissions;
  const error = admin.ui.error;
  const success = admin.ui.success;
  const loading = admin.ui.loading;

  // Section Management
  const [activeSection, setActiveSection] = useState<SectionType>('dashboard');
  const [isRefreshing, setIsRefreshing] = useState(false);
  const [employeesTab, setEmployeesTab] = useState<'list' | 'assignments'>('list');

  // Dashboard & My Appointments Data
  const [myAppointments, setMyAppointments] = useState<AppointmentDto[]>([]);
  const [rolPermissions, setRolPermissions] = useState<RolPermissionSummaryDto[]>([]);

  // Helper function to refresh entity data after activate/deactivate
  const refreshEntityData = async (entityType: 'appointmentTypes' | 'branches' | 'users' | 'roles' | 'availableTimes' | 'clients') => {
    switch (entityType) {
      case 'appointmentTypes':
        await admin.appointmentTypes.fetchAppointmentTypes();
        break;
      case 'branches':
        await admin.branches.fetchBranches();
        break;
      case 'users':
        await admin.users.fetchUsers();
        break;
      case 'roles':
        await admin.roles.fetchRoles();
        break;
      case 'availableTimes':
        await admin.availableTimes.fetchAvailableTimes();
        break;
      case 'clients':
        await admin.clients.fetchClients();
        break;
    }
  };

  // Enhanced Notifications with sounds and browser notifications
  const {
    notifications,
    addEnhancedNotification,
    markAsRead,
    clearAll,
    loadNotifications
  } = useEnhancedNotifications();

  // WebSocket - Activado para notificaciones en tiempo real
  const { isConnected: wsConnected } = useWebSocket((message) => {
    handleWebSocketMessage(message);
  });

  // Toast Notifications System
  const {
    toasts,
    removeToast,
    success: toastSuccess,
    error: toastError,
    warning: toastWarning,
    info: toastInfo
  } = useToastNotifications();

  // Confirm Modal
  const { confirm, ConfirmDialog } = useConfirm();

  // Load Role Permissions
  const loadRolPermissions = React.useCallback(async () => {
    try {
      const perms = await adminRepository.getAllRolPermissionsSummary();
      setRolPermissions(perms);
    } catch {
      // Silent error handling
    }
  }, []);

  // Load My Appointments
  const loadMyAppointments = React.useCallback(async () => {
    if (!currentUser?.id) return;

    try {
      const appointments = await adminRepository.getMyAssignedAppointments(currentUser.id);
      const mappedAppointments = appointments.map(apt => ({
        ...apt,
        clientName: apt.clientName || 'Unknown',
        clientNumber: apt.clientNumber || '',
        branchName: apt.branchName || 'Unknown',
        appointmentTypeName: apt.appointmentTypeName || 'Unknown'
      }));
      setMyAppointments(mappedAppointments);

      // Create notifications for today's appointments
      const newAppointments = mappedAppointments.filter(apt => {
        const appointmentDate = new Date(apt.appointmentDate);
        const today = new Date();
        return appointmentDate.toDateString() === today.toDateString() &&
               (apt.status === 'Scheduled' || apt.status === 'Pending');
      });

      // Note: We don't check if notification exists to avoid depending on notifications array
      // The notification system will handle duplicates if needed
      newAppointments.forEach(apt => {
        addEnhancedNotification({
          type: 'appointment',
          title: 'New Appointment Assigned',
          message: `Appointment ${apt.appointmentNumber} for ${apt.clientName}`,
          appointmentId: apt.id
        });
      });
    } catch {
      // Silent error handling
    }
  }, [currentUser?.id, addEnhancedNotification]);

  // Handle WebSocket Messages - Enhanced with sounds, browser notifications, and toasts
  const handleWebSocketMessage = (message: { type: string; data: { appointmentNumber?: string; id?: number; message?: string; appointmentId?: number }; timestamp: string }) => {
    switch (message.type) {
      case 'appointment_created':
        // Add notification to panel immediately
        addEnhancedNotification({
          id: Date.now(),
          type: 'appointment',
          title: 'Nueva Cita Creada',
          message: `Cita #${message.data.appointmentNumber} ha sido creada`,
          timestamp: new Date(message.timestamp),
          appointmentId: message.data.appointmentId
        });

        // Show toast notification
        toastInfo(
          'Nueva Cita Creada',
          `Cita #${message.data.appointmentNumber} ha sido creada`,
          {
            onView: () => {
              setActiveSection('citas');
            }
          }
        );

        // Reload dashboard stats to update counters
        admin.dashboardStats.fetchStats();
        break;

      case 'appointment_updated':
        // Show toast notification
        toastInfo(
          'Cita Actualizada',
          `Cita #${message.data.appointmentNumber} ha sido actualizada`,
          {
            onView: () => {
              setActiveSection('citas');
            }
          }
        );

        // Reload notifications from BD after a small delay
        setTimeout(() => {
          loadNotifications();
        }, 1000);

        // Reload dashboard stats
        admin.dashboardStats.fetchStats();
        break;

      case 'appointment_cancelled':
        // Show toast notification
        toastWarning(
          'Cita Cancelada',
          `Cita #${message.data.appointmentNumber} ha sido cancelada`,
          {
            onView: () => {
              setActiveSection('citas');
            }
          }
        );

        // Reload notifications from BD after a small delay
        setTimeout(() => {
          loadNotifications();
        }, 1000);

        // Reload dashboard stats
        admin.dashboardStats.fetchStats();
        break;

      case 'appointment_updated_old':
        // Add to in-app notifications
        addEnhancedNotification({
          id: Date.now().toString(),
          type: 'update',
          title: 'Cita Actualizada',
          message: `Cita #${message.data.appointmentNumber} ha sido actualizada`,
          timestamp: new Date(message.timestamp),
          read: false,
          appointmentId: message.data.id,
        });

        // Show toast notification
        toastInfo(
          'Cita Actualizada',
          `Cita #${message.data.appointmentNumber} ha sido actualizada`,
          {
            onView: () => {
              setActiveSection('citas');
            }
          }
        );

        // Reload dashboard stats to update counters
        admin.dashboardStats.fetchStats();
        break;

      case 'appointment_cancelled':
        // Add to in-app notifications
        addEnhancedNotification({
          id: Date.now().toString(),
          type: 'alert',
          title: 'Cita Cancelada',
          message: `Cita #${message.data.appointmentNumber} ha sido cancelada`,
          timestamp: new Date(message.timestamp),
          read: false,
          appointmentId: message.data.id,
        });

        // Show toast notification
        toastWarning(
          'Cita Cancelada',
          `Cita #${message.data.appointmentNumber} ha sido cancelada`,
          {
            onView: () => {
              setActiveSection('citas');
            }
          }
        );

        // Reload dashboard stats to update counters
        admin.dashboardStats.fetchStats();
        break;

      case 'appointment_reminder':
        // Add to in-app notifications
        addEnhancedNotification({
          id: Date.now().toString(),
          type: 'reminder',
          title: 'Recordatorio de Cita',
          message: message.data.message || 'Tienes una cita próxima',
          timestamp: new Date(message.timestamp),
          read: false,
          appointmentId: message.data.appointmentId,
        });

        // Show toast notification
        toastWarning(
          'Recordatorio de Cita',
          message.data.message || 'Tienes una cita próxima',
          {
            onView: () => {
              if (message.data.appointmentId) {
                setActiveSection('my-appointments');
              }
            }
          }
        );
        break;

      default:
        console.log('Mensaje WebSocket no manejado:', message);
    }
  };

  // Handle Mark Completed
  const handleMarkCompleted = async (id: number) => {
    const appointment = myAppointments.find(a => a.id === id);
    if (appointment) {
      await handleToggleAppointmentAttendance(appointment, true);
      await loadMyAppointments();
      await admin.dashboardStats.fetchStats();
      toastSuccess('Appointment Completed', `Appointment ${appointment.appointmentNumber} marked as completed`);
    }
  };

  // Handle Mark No Show
  const handleMarkNoShow = async (id: number) => {
    const appointment = myAppointments.find(a => a.id === id);
    if (appointment) {
      await handleToggleAppointmentAttendance(appointment, false);
      await loadMyAppointments();
      await admin.dashboardStats.fetchStats();
      toastWarning('Appointment No Show', `Appointment ${appointment.appointmentNumber} marked as no show`);
    }
  };

  // Handle Update Permission
  const handleUpdatePermission = async (rolId: number, formCode: string, permissionData: PermissionUpdate) => {
    try {
      // Find the form ID from formCode by searching across ALL roles
      let formId: number | undefined;
      
      // Search in all rolPermissions to find the formId
      for (const rolPerm of rolPermissions) {
        const formPerm = rolPerm.formPermissions?.find(fp => fp.formCode === formCode);
        if (formPerm) {
          formId = formPerm.formId;
          break;
        }
      }
      
      if (!formId) {
        throw new Error(`Formulario ${formCode} no encontrado en el sistema`);
      }

      // Send the permission flags directly to the backend
      // The backend will find or create the appropriate Permission record
      await adminRepository.updateRolFormPermission({
        RolId: rolId,
        FormId: formId,
        CanInsert: permissionData.canCreate,
        CanUpdate: permissionData.canUpdate,
        CanDelete: permissionData.canDelete,
        CanView: permissionData.canRead
      });

      toastSuccess('Permiso Actualizado', 'Los permisos se actualizaron exitosamente');
      await loadRolPermissions();
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'No se pudo actualizar el permiso';
      toastError('Error', errorMessage);
      throw err;
    }
  };

  // Refresh Data Handler
  const handleRefreshData = async () => {
    setIsRefreshing(true);
    try {
      if (activeSection === 'dashboard') {
        await admin.dashboardStats.fetchStats();
      } else if (activeSection === 'my-appointments') {
        await loadMyAppointments();
      } else {
        // TODO: Implement section-specific refresh logic
        console.warn('Refresh for section not implemented:', activeSection);
      }
    } catch {
      // Silent error handling
    } finally {
      setIsRefreshing(false);
    }
  };

  // CRUD Handlers - Users
  const handleSaveUser = async (data: Partial<UserDto> & { password?: string; roleIds?: number[] }) => {
    try {
      if (data.id) {
        await adminRepository.updateUser({
          id: data.id,
          username: data.username,
          email: data.email,
          isActive: data.isActive
        });
        await refreshEntityData('users');
        toastSuccess('Usuario Actualizado', 'El usuario se actualizó exitosamente');
      } else {
        if (!data.username || !data.email || !data.password || !data.roleIds) {
          throw new Error('Missing required fields for creating user');
        }
        await adminRepository.createUser({
          username: data.username,
          email: data.email,
          password: data.password,
          roleIds: data.roleIds
        });
        await refreshEntityData('users');
        toastSuccess('Usuario Creado', 'El usuario se creó exitosamente');
      }
    } catch (err) {
      toastError('Error', 'No se pudo guardar el usuario');
      throw err;
    }
  };

  const handleDeactivateUser = async (id: number) => {
    const user = employees.find(e => e.id === id);
    const confirmed = await confirm({
      title: 'Desactivar Usuario',
      message: `¿Estás seguro de desactivar a ${user?.username}?`,
      confirmText: 'Desactivar',
      cancelText: 'Cancelar',
      type: 'danger'
    });
    if (!confirmed) return;
    try {
      await adminRepository.deleteLogicalUser(id);
      await refreshEntityData('users');
      toastSuccess('Usuario Desactivado', `El usuario fue desactivado exitosamente`);
    } catch {
      toastError('Error', 'No se pudo desactivar el usuario');
    }
  };

  const handleActivateUser = async (id: number) => {
    const user = employees.find(e => e.id === id);
    const confirmed = await confirm({
      title: 'Activar Usuario',
      message: `¿Estás seguro de activar a ${user?.username}?`,
      confirmText: 'Activar',
      cancelText: 'Cancelar',
      type: 'info'
    });
    if (!confirmed) return;
    try {
      await adminRepository.activateUser(id);
      await refreshEntityData('users');
      toastSuccess('Usuario Activado', `El usuario fue activado exitosamente`);
    } catch {
      toastError('Error', 'No se pudo activar el usuario');
    }
  };

  // CRUD Handlers - Roles
  const handleSaveRole = async (data: Partial<RolDto>) => {
    try {
      if (data.id) {
        await adminRepository.updateRol({
          id: data.id,
          name: data.name,
          code: data.code,
          isActive: data.isActive
        });
        await refreshEntityData('roles');
        toastSuccess('Rol Actualizado', 'El rol se actualizó exitosamente');
      } else {
        if (!data.name || !data.code) {
          throw new Error('Missing required fields for creating role');
        }
        await adminRepository.createRol({
          name: data.name,
          code: data.code,
          isActive: true
        });
        await refreshEntityData('roles');
        toastSuccess('Rol Creado', 'El rol se creó exitosamente');
      }
    } catch (err) {
      toastError('Error', 'No se pudo guardar el rol');
      throw err;
    }
  };

  const handleDeactivateRole = async (id: number) => {
    const role = roles.find(r => r.id === id);
    const confirmed = await confirm({
      title: 'Desactivar Rol',
      message: `¿Estás seguro de desactivar el rol ${role?.name}?`,
      confirmText: 'Desactivar',
      cancelText: 'Cancelar',
      type: 'danger'
    });
    if (!confirmed) return;
    try {
      await adminRepository.deleteLogicalRol(id);
      await refreshEntityData('roles');
      toastSuccess('Rol Desactivado', `El rol fue desactivado exitosamente`);
    } catch {
      toastError('Error', 'No se pudo desactivar el rol');
    }
  };

  const handleActivateRole = async (id: number) => {
    const role = roles.find(r => r.id === id);
    const confirmed = await confirm({
      title: 'Activar Rol',
      message: `¿Estás seguro de activar el rol ${role?.name}?`,
      confirmText: 'Activar',
      cancelText: 'Cancelar',
      type: 'info'
    });
    if (!confirmed) return;
    try {
      await adminRepository.activateRol(id);
      await refreshEntityData('roles');
      toastSuccess('Rol Activado', `El rol fue activado exitosamente`);
    } catch {
      toastError('Error', 'No se pudo activar el rol');
    }
  };

  // CRUD Handlers - Branches
  const handleSaveBranch = async (data: Partial<BranchDto>) => {
    try {
      if (data.id) {
        await adminRepository.updateBranch({
          id: data.id,
          name: data.name,
          code: data.code,
          address: data.address,
          phone: data.phone,
          city: data.city,
          state: data.state,
          isMain: data.isMain,
          isActive: data.isActive
        });
        await refreshEntityData('branches');
        toastSuccess('Sede Actualizada', 'La sede se actualizó exitosamente');
      } else {
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const { id: _id, createdAt: _createdAt, updatedAt: _updatedAt, isEnabled: _isEnabled, ...createData } = data;
        await adminRepository.createBranch(createData as Omit<BranchDto, 'id'>);
        await refreshEntityData('branches');
        toastSuccess('Sede Creada', 'La sede se creó exitosamente');
      }
    } catch (err) {
      toastError('Error', 'No se pudo guardar la sede');
      throw err;
    }
  };

  const handleDeactivateBranch = async (id: number) => {
    const branch = branches.find(b => b.id === id);
    const confirmed = await confirm({
      title: 'Desactivar Sede',
      message: `¿Estás seguro de desactivar la sede ${branch?.name}?`,
      confirmText: 'Desactivar',
      cancelText: 'Cancelar',
      type: 'danger'
    });
    if (!confirmed) return;
    try {
      await adminRepository.deleteLogicalBranch(id);
      await refreshEntityData('branches');
      toastSuccess('Sede Desactivada', `La sede fue desactivada exitosamente`);
    } catch {
      toastError('Error', 'No se pudo desactivar la sede');
    }
  };

  const handleActivateBranch = async (id: number) => {
    const branch = branches.find(b => b.id === id);
    const confirmed = await confirm({
      title: 'Activar Sede',
      message: `¿Estás seguro de activar la sede ${branch?.name}?`,
      confirmText: 'Activar',
      cancelText: 'Cancelar',
      type: 'info'
    });
    if (!confirmed) return;
    try {
      await adminRepository.activateBranch(id);
      await refreshEntityData('branches');
      toastSuccess('Sede Activada', `La sede fue activada exitosamente`);
    } catch {
      toastError('Error', 'No se pudo activar la sede');
    }
  };

  // CRUD Handlers - Appointment Types
  const handleSaveAppointmentType = async (data: Partial<AppointmentTypeDto>) => {
    try {
      if (data.id) {
        await adminRepository.updateAppointmentType({
          id: data.id,
          name: data.name,
          description: data.description,
          icon: data.icon,
          estimatedTimeMinutes: data.estimatedTimeMinutes,
          requiresDocumentation: data.requiresDocumentation,
          isActive: data.isActive
        });
        await refreshEntityData('appointmentTypes');
        toastSuccess('Tipo de Cita Actualizado', 'El tipo de cita se actualizó exitosamente');
      } else {
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const { id: _id, createdAt: _createdAt, updatedAt: _updatedAt, isEnabled: _isEnabled, ...createData } = data;
        await adminRepository.createAppointmentType(createData as Omit<AppointmentTypeDto, 'id'>);
        await refreshEntityData('appointmentTypes');
        toastSuccess('Tipo de Cita Creado', 'El tipo de cita se creó exitosamente');
      }
    } catch (err) {
      toastError('Error', 'No se pudo guardar el tipo de cita');
      throw err;
    }
  };

  const handleDeactivateAppointmentType = async (id: number) => {
    const type = appointmentTypes.find(t => t.id === id);
    const confirmed = await confirm({
      title: 'Desactivar Tipo de Cita',
      message: `¿Estás seguro de desactivar el tipo de cita ${type?.name}?`,
      confirmText: 'Desactivar',
      cancelText: 'Cancelar',
      type: 'danger'
    });
    if (!confirmed) return;
    try {
      await adminRepository.deleteLogicalAppointmentType(id);
      await refreshEntityData('appointmentTypes');
      toastSuccess('Tipo de Cita Desactivado', `El tipo de cita fue desactivado exitosamente`);
    } catch {
      toastError('Error', 'No se pudo desactivar el tipo de cita');
    }
  };

  const handleActivateAppointmentType = async (id: number) => {
    const type = appointmentTypes.find(t => t.id === id);
    const confirmed = await confirm({
      title: 'Activar Tipo de Cita',
      message: `¿Estás seguro de activar el tipo de cita ${type?.name}?`,
      confirmText: 'Activar',
      cancelText: 'Cancelar',
      type: 'info'
    });
    if (!confirmed) return;
    try {
      await adminRepository.activateAppointmentType(id);
      await refreshEntityData('appointmentTypes');
      toastSuccess('Tipo de Cita Activado', `El tipo de cita fue activado exitosamente`);
    } catch (error) {
      console.error('❌ Error activating appointment type:', error);
      toastError('Error', 'No se pudo activar el tipo de cita');
    }
  };

  // CRUD Handlers - Available Times
  const handleSaveAvailableTime = async (data: Partial<AvailableTimeDto>) => {
    try {
      if (data.id) {
        await adminRepository.updateAvailableTime({
          id: data.id,
          time: data.time,
          branchId: data.branchId,
          appointmentTypeId: data.appointmentTypeId,
          isActive: data.isActive
        });
        await refreshEntityData('availableTimes');
        toastSuccess('Hora Disponible Actualizada', 'La hora disponible se actualizó exitosamente');
      } else {
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const { id: _id, createdAt: _createdAt, updatedAt: _updatedAt, isEnabled: _isEnabled, branchName: _branchName, appointmentTypeName: _appointmentTypeName, ...createData } = data;
        await adminRepository.createAvailableTime(createData as Omit<AvailableTimeDto, 'id'>);
        await refreshEntityData('availableTimes');
        toastSuccess('Hora Disponible Creada', 'La hora disponible se creó exitosamente');
      }
    } catch (err) {
      toastError('Error', 'No se pudo guardar la hora disponible');
      throw err;
    }
  };

  const handleDeactivateAvailableTime = async (id: number) => {
    const confirmed = await confirm({
      title: 'Desactivar Hora Disponible',
      message: `¿Estás seguro de desactivar esta hora disponible?`,
      confirmText: 'Desactivar',
      cancelText: 'Cancelar',
      type: 'danger'
    });
    if (!confirmed) return;
    try {
      await adminRepository.deleteLogicalAvailableTime(id);
      await refreshEntityData('availableTimes');
      toastSuccess('Hora Desactivada', `La hora fue desactivada exitosamente`);
    } catch {
      toastError('Error', 'No se pudo desactivar la hora disponible');
    }
  };

  const handleActivateAvailableTime = async (id: number) => {
    const confirmed = await confirm({
      title: 'Activar Hora Disponible',
      message: `¿Estás seguro de activar esta hora disponible?`,
      confirmText: 'Activar',
      cancelText: 'Cancelar',
      type: 'info'
    });
    if (!confirmed) return;
    try {
      await adminRepository.activateAvailableTime(id);
      await refreshEntityData('availableTimes');
      toastSuccess('Hora Activada', `La hora fue activada exitosamente`);
    } catch {
      toastError('Error', 'No se pudo activar la hora disponible');
    }
  };

  // CRUD Handlers - Holidays
  const handleSaveHoliday = async (data: Partial<HolidayDto>) => {
    try {
      if (data.id) {
        await admin.holidays.updateHoliday(data.id, data);
        toastSuccess('Festivo Actualizado', 'El festivo se actualizó exitosamente');
      } else {
        await admin.holidays.createHoliday(data);
        toastSuccess('Festivo Creado', 'El festivo se creó exitosamente');
      }
      await admin.holidays.fetchHolidays();
    } catch (err) {
      toastError('Error', 'No se pudo guardar el festivo');
      throw err;
    }
  };

  const handleDeactivateHoliday = async (id: number) => {
    const holiday = holidays.find(h => h.id === id);
    const confirmed = await confirm({
      title: 'Desactivar Festivo',
      message: `¿Estás seguro de desactivar el festivo "${holiday?.holidayName}"?`,
      confirmText: 'Desactivar',
      cancelText: 'Cancelar',
      type: 'danger'
    });
    if (!confirmed) return;
    try {
      await admin.holidays.deleteHoliday(id);
      await admin.holidays.fetchHolidays();
      toastSuccess('Festivo Desactivado', `El festivo fue desactivado exitosamente`);
    } catch {
      toastError('Error', 'No se pudo desactivar el festivo');
    }
  };

  const handleActivateHoliday = async (id: number) => {
    const holiday = holidays.find(h => h.id === id);
    const confirmed = await confirm({
      title: 'Activar Festivo',
      message: `¿Estás seguro de activar el festivo "${holiday?.holidayName}"?`,
      confirmText: 'Activar',
      cancelText: 'Cancelar',
      type: 'info'
    });
    if (!confirmed) return;
    try {
      await admin.holidays.activateHoliday(id);
      await admin.holidays.fetchHolidays();
      toastSuccess('Festivo Activado', `El festivo fue activado exitosamente`);
    } catch (error) {
      console.error('Error activating holiday:', error);
      toastError('Error', 'No se pudo activar el festivo');
    }
  };

  // Real-time updates with polling
  // Only poll when WebSocket is NOT connected (WebSocket provides real-time updates)
  useEffect(() => {
    if (!currentUser?.id) return;

    // Initial load
    loadMyAppointments();
    admin.dashboardStats.fetchStats();

    // Only poll if WebSocket is NOT connected
    if (!wsConnected) {
      const interval = setInterval(() => {
        loadMyAppointments();
        admin.dashboardStats.fetchStats();
      }, 120000); // Reduced from 60000 to 120000 (2 minutes)

      return () => {
        clearInterval(interval);
      };
    }
  }, [currentUser?.id, wsConnected]); // Added wsConnected dependency

  // Note: myCompletedAppointments is now calculated in the hook stats
  // This useEffect is no longer needed as the dashboard stats are managed by useDashboardStats hook

  // Auto-connect WebSocket once on mount
  // Empty dependency array prevents infinite loop - SignalR service handles reconnection automatically
  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      signalRService.connect(token);
    }

    // Disconnect only on component unmount (when user logs out or leaves admin panel)
    return () => {
      signalRService.disconnect();
    };
  }, []); // Empty dependencies - only run once on mount to avoid infinite reconnection loop

  // Sync section with tab
  useEffect(() => {
    if (activeSection !== 'dashboard' && activeSection !== 'my-appointments') {
      // TODO: Implement setActiveTab functionality
    }
  }, [activeSection]);

  // Load data based on active section - using refs to prevent infinite loops
  const loadedSectionsRef = React.useRef<Set<string>>(new Set());

  useEffect(() => {
    // Load data based on section
    const loadSectionData = async () => {
      try {
        switch (activeSection) {
          case 'dashboard':
            // Always load appointments for dashboard when currentUserId is available
            // This ensures filtered appointments are loaded for operators
            if (appointments.length === 0 && currentUserId !== undefined) {
              await admin.appointments.fetchAppointments();
              loadedSectionsRef.current.add('dashboard');
            }
            // Skip marking as loaded if userId not yet available - will retry when userId loads
            break;
          case 'festivos':
            if (holidays.length === 0) {
              await admin.holidays.fetchHolidays();
              loadedSectionsRef.current.add('festivos');
            }
            break;
          case 'citas':
            // Always load appointments for citas section since this is the main appointments view
            if (appointments.length === 0) {
              await admin.appointments.fetchAppointments();
              loadedSectionsRef.current.add('citas');
            }
            break;
          case 'empleados':
            if (users.length === 0 || roles.length === 0) {
              await Promise.all([
                admin.users.fetchUsers(),
                roles.length === 0 ? admin.roles.fetchRoles() : Promise.resolve()
              ]);
              loadedSectionsRef.current.add('empleados');
            }
            break;
          case 'roles':
            if (roles.length === 0) {
              await admin.roles.fetchRoles();
              loadedSectionsRef.current.add('roles');
            }
            break;
          case 'sedes':
            if (branches.length === 0) {
              await admin.branches.fetchBranches();
              loadedSectionsRef.current.add('sedes');
            }
            break;
          case 'tipos-cita':
            if (appointmentTypes.length === 0) {
              await admin.appointmentTypes.fetchAppointmentTypes();
              loadedSectionsRef.current.add('tipos-cita');
            }
            break;
          case 'horas-disponibles':
            // Load available times, branches, and appointment types for the form
            if (availableTimes.length === 0 || branches.length === 0 || appointmentTypes.length === 0) {
              await Promise.all([
                availableTimes.length === 0 ? admin.availableTimes.fetchAvailableTimes() : Promise.resolve(),
                branches.length === 0 ? admin.branches.fetchBranches() : Promise.resolve(),
                appointmentTypes.length === 0 ? admin.appointmentTypes.fetchAppointmentTypes() : Promise.resolve()
              ]);
              loadedSectionsRef.current.add('horas-disponibles');
            }
            break;
          case 'permisos':
            if (permissions.length === 0 || roles.length === 0 || rolPermissions.length === 0) {
              await Promise.all([
                admin.permissions.fetchPermissions(),
                admin.roles.fetchRoles(),
                loadRolPermissions()
              ]);
              loadedSectionsRef.current.add('permisos');
            }
            break;
        }
      } catch (error) {
        // Silent error handling - errors already handled by individual hooks
        console.error('Error loading section data:', error);
      }
    };

    loadSectionData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeSection, currentUserId]); // Depend on activeSection and currentUserId

  // Helper functions
  const hasPermissionWrapper = (formCode: string, action: string): boolean => {
    const validActions = ['read', 'create', 'update', 'delete'];
    if (!validActions.includes(action)) {
      return false;
    }
    return hasPermission(formCode, action as 'read' | 'create' | 'update' | 'delete');
  };

  const getTabsForSidebar = () => {
    const tabs = getAvailableTabs();
    const tabNames: Record<TabType, string> = {
      'citas': 'Citas',
      'empleados': 'Empleados',
      'roles': 'Roles',
      'sedes': 'Sedes',
      'tipos-cita': 'Tipos de Cita',
      'horas-disponibles': 'Horas Disponibles',
      'permisos': 'Permisos',
      'festivos': 'Festivos',
      'settings': 'Configuración'
    };
    return tabs.map(tab => ({ id: tab, name: tabNames[tab] || tab }));
  };

  const getPageTitle = () => {
    switch (activeSection) {
      case 'dashboard': return 'Panel Principal';
      case 'my-appointments': return 'Mis Citas';
      case 'citas': return 'Citas';
      case 'empleados': return 'Empleados';
      case 'roles': return 'Roles';
      case 'sedes': return 'Sedes';
      case 'tipos-cita': return 'Tipos de Cita';
      case 'horas-disponibles': return 'Horas Disponibles';
      case 'permisos': return 'Permisos';
      case 'festivos': return 'Festivos';
      case 'settings': return 'Configuración del Sistema';
      default: return '';
    }
  };

  const getBreadcrumb = () => {
    return `Panel Administrativo / ${getPageTitle()}`;
  };

  // Loading state
  if (!currentUser) {
    return (
      <div className="flex items-center justify-center h-screen bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#1797D5] mx-auto mb-4"></div>
          <p className="text-gray-600">Cargando...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex h-screen bg-gray-50 overflow-hidden">
      {/* Sidebar */}
      <Sidebar
        activeTab={activeSection}
        onTabChange={(tab) => {
          setActiveSection(tab);
        }}
        availableTabs={getTabsForSidebar()}
        currentUser={currentUser}
        onLogout={async () => {
          await adminRepository.logout();
          router.push('/login');
        }}
        notificationCount={notifications.filter(n => !n.read).length}
      />

      {/* Main Content */}
      <div className="flex-1 ml-64 flex flex-col overflow-hidden">
        {/* Header */}
        <header className="bg-white border-b border-gray-200 px-8 py-4 flex items-center justify-between sticky top-0 z-30 shadow-sm">
          <div>
            <h2 className="text-2xl font-bold text-gray-900">{getPageTitle()}</h2>
            <p className="text-sm text-gray-500">{getBreadcrumb()}</p>
          </div>
          <div className="flex items-center space-x-4">
            {/* Enhanced WebSocket Status Indicator */}
            <WebSocketStatus
              status={wsConnected ? 'connected' : 'disconnected'}
              showLabel={true}
            />

            <button
              onClick={handleRefreshData}
              disabled={isRefreshing}
              className="px-4 py-2 text-sm font-medium text-[#1797D5] hover:bg-blue-50 rounded-lg transition-colors flex items-center"
            >
              <FiRefreshCw className={`w-4 h-4 mr-2 ${isRefreshing ? 'animate-spin' : ''}`} />
              {isRefreshing ? 'Actualizando...' : 'Actualizar'}
            </button>

            {/* Enhanced Notification Bell with animations and settings */}
            <EnhancedNotificationBell
              notifications={notifications}
              onMarkAsRead={markAsRead}
              onClearAll={clearAll}
              onNotificationClick={(notification) => {
                if (notification.appointmentId) {
                  setActiveSection('my-appointments');
                }
              }}
            />
            <div className="flex items-center space-x-2 px-3 py-2 bg-gray-50 rounded-lg">
              <FiUser className="w-4 h-4 text-gray-600" />
              <span className="text-sm font-medium text-gray-700">
                {currentUser?.username || 'User'}
              </span>
            </div>
            <Link
              href="/"
              className="px-4 py-2 text-sm text-[#1797D5] hover:bg-blue-50 rounded-lg transition-colors"
            >
              Volver al Inicio
            </Link>
          </div>
        </header>

        {/* Messages */}
        {error && (
          <div className="mx-8 mt-4 p-4 bg-red-50 border border-red-200 rounded-lg text-red-800 flex items-center">
            <FiAlertCircle className="w-5 h-5 mr-2 flex-shrink-0" />
            {error}
          </div>
        )}
        {success && (
          <div className="mx-8 mt-4 p-4 bg-green-50 border border-green-200 rounded-lg text-green-800 flex items-center">
            <FiCheckCircle className="w-5 h-5 mr-2 flex-shrink-0" />
            {success}
          </div>
        )}

        {/* Main Content Area */}
        <main className="flex-1 overflow-auto p-8">
          {activeSection === 'dashboard' && (
            <Dashboard
              stats={admin.dashboardStats.stats}
              recentAppointments={appointments.slice(0, 10).map(apt => ({
                id: apt.id,
                appointmentNumber: apt.appointmentNumber || '',
                clientName: apt.clientName || 'Unknown',
                appointmentDate: apt.appointmentDate || '',
                appointmentTime: apt.appointmentTime || '',
                status: apt.status || '',
                branch: apt.branchName || '',
                appointmentType: apt.appointmentTypeName || ''
              }))}
              currentUser={currentUser}
              onNavigate={(section) => setActiveSection(section as SectionType)}
            />
          )}

          {activeSection === 'my-appointments' && (
            <MyAssignedAppointmentsView
              currentUserId={currentUser?.id || 0}
              onMarkCompleted={handleMarkCompleted}
              onMarkNoShow={handleMarkNoShow}
            />
          )}

          {activeSection === 'citas' && (
            <AppointmentsView
              appointments={appointments}
              loading={loading}
              onToggleAttendance={handleToggleAppointmentAttendance}
            />
          )}

          {activeSection === 'empleados' && (
            <div className="space-y-6">
              {/* Tabs para Empleados */}
              <div className="bg-white rounded-lg shadow-sm border border-gray-200">
                <div className="border-b border-gray-200">
                  <nav className="flex space-x-4 px-6" aria-label="Tabs">
                    <button
                      onClick={() => setEmployeesTab('list')}
                      className={`py-4 px-1 border-b-2 font-medium text-sm transition-colors ${
                        employeesTab === 'list'
                          ? 'border-[#1797D5] text-[#1797D5]'
                          : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                      }`}
                    >
                      Lista de Empleados
                    </button>
                    <button
                      onClick={() => setEmployeesTab('assignments')}
                      className={`py-4 px-1 border-b-2 font-medium text-sm transition-colors ${
                        employeesTab === 'assignments'
                          ? 'border-[#1797D5] text-[#1797D5]'
                          : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                      }`}
                    >
                      Asignación de Tipos de Cita
                    </button>
                  </nav>
                </div>

                <div className="p-6">
                  {employeesTab === 'list' ? (
                    <UsersView
                      employees={employees}
                      roles={roles}
                      hasPermission={hasPermissionWrapper}
                      onSaveUser={handleSaveUser}
                      onDeactivateUser={handleDeactivateUser}
                      onActivateUser={handleActivateUser}
                      onExportSuccess={(msg) => toastSuccess('Exportado', msg)}
                      onExportWarning={(msg) => toastWarning('Sin Datos', msg)}
                    />
                  ) : (
                    <EmployeeAssignmentsManager
                      users={users}
                      appointmentTypes={appointmentTypes}
                      onSuccess={(msg) => toastSuccess('Éxito', msg)}
                      onError={(msg) => toastError('Error', msg)}
                    />
                  )}
                </div>
              </div>
            </div>
          )}

          {activeSection === 'roles' && (
            <RolesView
              roles={roles}
              hasPermission={hasPermissionWrapper}
              onSaveRole={handleSaveRole}
              onDeactivateRole={handleDeactivateRole}
              onActivateRole={handleActivateRole}
              onExportSuccess={(msg) => toastSuccess('Exportado', msg)}
              onExportWarning={(msg) => toastWarning('Sin Datos', msg)}
            />
          )}

          {activeSection === 'sedes' && (
            <BranchesView
              branches={branches}
              hasPermission={hasPermissionWrapper}
              onSaveBranch={handleSaveBranch}
              onDeactivateBranch={handleDeactivateBranch}
              onActivateBranch={handleActivateBranch}
              onExportSuccess={(msg) => toastSuccess('Exportado', msg)}
              onExportWarning={(msg) => toastWarning('Sin Datos', msg)}
            />
          )}

          {activeSection === 'tipos-cita' && (
            <TypesView
              appointmentTypes={appointmentTypes}
              hasPermission={hasPermissionWrapper}
              onSaveType={handleSaveAppointmentType}
              onDeactivateType={handleDeactivateAppointmentType}
              onActivateType={handleActivateAppointmentType}
              onExportSuccess={(msg) => toastSuccess('Exportado', msg)}
              onExportWarning={(msg) => toastWarning('Sin Datos', msg)}
            />
          )}

          {activeSection === 'horas-disponibles' && (
            <TimesView
              availableTimes={availableTimes}
              branches={branches}
              appointmentTypes={appointmentTypes}
              hasPermission={hasPermissionWrapper}
              onSaveTime={handleSaveAvailableTime}
              onDeactivateTime={handleDeactivateAvailableTime}
              onActivateTime={handleActivateAvailableTime}
              onExportSuccess={(msg) => toastSuccess('Exportado', msg)}
              onExportWarning={(msg) => toastWarning('Sin Datos', msg)}
            />
          )}

          {activeSection === 'permisos' && (
            <PermissionsView
              roles={roles}
              permissions={permissions}
              rolPermissions={rolPermissions}
              loading={loading}
              onUpdatePermission={handleUpdatePermission}
            />
          )}

          {activeSection === 'festivos' && (
            <HolidaysView
              holidays={holidays}
              hasPermission={hasPermissionWrapper}
              onSaveHoliday={handleSaveHoliday}
              onDeactivateHoliday={handleDeactivateHoliday}
              onActivateHoliday={handleActivateHoliday}
              onExportSuccess={(msg) => toastSuccess('Exportado', msg)}
              onExportWarning={(msg) => toastWarning('Sin Datos', msg)}
            />
          )}


          {activeSection === 'settings' && (
            <SystemSettingsView />
          )}
        </main>

        {/* Enhanced Toast Container with configurable position */}
        <ToastContainer
          toasts={toasts}
          position="top-right"
          onClose={removeToast}
        />

        {/* Confirm Dialog */}
        <ConfirmDialog />
      </div>
    </div>
  );
};
