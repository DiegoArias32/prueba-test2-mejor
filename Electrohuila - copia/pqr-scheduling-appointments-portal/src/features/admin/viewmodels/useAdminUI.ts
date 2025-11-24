/**
 * Hook - Admin UI State
 * Handles UI state, navigation, modals, messages, and pagination
 */

'use client';

import { useState, useCallback, useEffect } from 'react';
import { TabType, ViewType, ModalType, TAB_DISPLAY_NAMES } from '../models/admin.models';
import type {
  AppointmentDto,
  UserDto,
  RolDto,
  BranchDto,
  AppointmentTypeDto,
  AvailableTimeDto
} from '@/services/api';

type AdminItem = AppointmentDto | UserDto | RolDto | BranchDto | AppointmentTypeDto | AvailableTimeDto;

interface FormSetters {
  setAppointmentForm?: React.Dispatch<React.SetStateAction<Partial<AppointmentDto>>>;
  setEmployeeForm?: React.Dispatch<React.SetStateAction<Partial<UserDto>>>;
  setRolForm?: React.Dispatch<React.SetStateAction<Partial<RolDto>>>;
  setBranchForm?: React.Dispatch<React.SetStateAction<Partial<BranchDto>>>;
  setAppointmentTypeForm?: React.Dispatch<React.SetStateAction<Partial<AppointmentTypeDto>>>;
  setAvailableTimeForm?: React.Dispatch<React.SetStateAction<Partial<AvailableTimeDto>>>;
}

interface UseAdminUIReturn {
  activeTab: TabType;
  currentView: ViewType;
  modalType: ModalType;
  selectedItem: AdminItem | null;
  currentPage: number;
  itemsPerPage: number;
  loading: boolean;
  error: string;
  success: string;
  isRefreshing: boolean;
  lastRefresh: Date;
  setActiveTab: (tab: TabType) => void;
  setCurrentView: (view: ViewType) => void;
  setLoading: (loading: boolean) => void;
  openCreateModal: () => void;
  openEditModal: (item: AdminItem, tab: TabType, setters: FormSetters) => void;
  openDeleteModal: (item: AdminItem) => void;
  openActivateModal: (item: AdminItem) => void;
  closeModal: () => void;
  showSuccess: (message: string) => void;
  showError: (message: string) => void;
  getPaginatedData: <T>(data: T[]) => T[];
  getTotalPages: (totalItems: number) => number;
  handlePageChange: (page: number) => void;
  getTabDisplayName: () => string;
  getViewDisplayName: () => string;
  formatDate: (dateString: string) => string;
  formatTime: (timeString: string) => string;
  setLastRefresh: (date: Date) => void;
}

export const useAdminUI = (): UseAdminUIReturn => {
  const [activeTab, setActiveTab] = useState<TabType>('citas');
  const [currentView, setCurrentView] = useState<ViewType>('active');
  const [modalType, setModalType] = useState<ModalType>(null);
  const [selectedItem, setSelectedItem] = useState<AdminItem | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [lastRefresh, setLastRefresh] = useState<Date>(new Date());

  const showSuccess = useCallback((message: string) => {
    setSuccess(message);
    setTimeout(() => setSuccess(''), 3000);
  }, []);

  const showError = useCallback((message: string) => {
    setError(message);
    setTimeout(() => setError(''), 5000);
  }, []);

  const getPaginatedData = useCallback(<T,>(data: T[]): T[] => {
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return data.slice(startIndex, endIndex);
  }, [currentPage, itemsPerPage]);

  const getTotalPages = useCallback((totalItems: number) => {
    return Math.ceil(totalItems / itemsPerPage);
  }, [itemsPerPage]);

  const handlePageChange = useCallback((page: number) => {
    setCurrentPage(page);
    if (typeof window !== 'undefined') {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }, []);

  const openCreateModal = useCallback(() => {
    setSelectedItem(null);
    setModalType('create');
  }, []);

  const openEditModal = useCallback((item: AdminItem, tab: TabType, setters: FormSetters) => {
    setSelectedItem(item);
    setModalType('edit');

    switch (tab) {
      case 'citas':
        if ('appointmentNumber' in item && setters.setAppointmentForm) {
          setters.setAppointmentForm({
            appointmentNumber: item.appointmentNumber,
            appointmentDate: item.appointmentDate?.split('T')[0],
            appointmentTime: item.appointmentTime,
            status: item.status,
            observations: item.observations,
            clientId: item.clientId,
            branchId: item.branchId,
            appointmentTypeId: item.appointmentTypeId
          });
        }
        break;
      case 'empleados':
        if ('username' in item && setters.setEmployeeForm) {
          setters.setEmployeeForm({
            username: item.username,
            email: item.email,
            roles: item.roles
          });
        }
        break;
      case 'roles':
        if ('code' in item && setters.setRolForm) {
          setters.setRolForm({
            name: item.name,
            code: item.code
          });
        }
        break;
      case 'sedes':
        if ('address' in item && setters.setBranchForm) {
          setters.setBranchForm({
            name: item.name,
            code: item.code,
            address: item.address,
            phone: item.phone,
            city: item.city,
            state: item.state,
            isMain: item.isMain
          });
        }
        break;
      case 'tipos-cita':
        if ('estimatedTimeMinutes' in item && setters.setAppointmentTypeForm) {
          setters.setAppointmentTypeForm({
            name: item.name,
            description: item.description,
            icon: item.icon,
            estimatedTimeMinutes: item.estimatedTimeMinutes,
            requiresDocumentation: item.requiresDocumentation
          });
        }
        break;
      case 'horas-disponibles':
        if ('time' in item && setters.setAvailableTimeForm) {
          setters.setAvailableTimeForm({
            time: item.time,
            branchId: item.branchId,
            appointmentTypeId: item.appointmentTypeId
          });
        }
        break;
    }
  }, []);

  const openDeleteModal = useCallback((item: AdminItem) => {
    setSelectedItem(item);
    setModalType('delete');
  }, []);

  const openActivateModal = useCallback((item: AdminItem) => {
    setSelectedItem(item);
    setModalType('activate');
  }, []);

  const closeModal = useCallback(() => {
    setModalType(null);
    setSelectedItem(null);
  }, []);

  const getTabDisplayName = useCallback(() => {
    return TAB_DISPLAY_NAMES[activeTab] || '';
  }, [activeTab]);

  const getViewDisplayName = useCallback(() => {
    if (activeTab === 'citas') {
      return currentView === 'active' ? 'Attended' : 'Not Attended';
    }
    if (activeTab === 'permisos') {
      return currentView === 'active' ? 'By Role' : 'By Employee';
    }
    return currentView === 'active' ? 'Active' : 'Inactive';
  }, [activeTab, currentView]);

  const formatDate = useCallback((dateString: string) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('en-US');
  }, []);

  const formatTime = useCallback((timeString: string) => {
    if (!timeString) return '';
    if (timeString.includes(':')) {
      const [hours, minutes] = timeString.split(':');
      const hour24 = parseInt(hours);
      const ampm = hour24 >= 12 ? 'PM' : 'AM';
      const hour12 = hour24 % 12 || 12;
      return `${hour12}:${minutes} ${ampm}`;
    }
    return timeString;
  }, []);

  useEffect(() => {
    setCurrentPage(1);
  }, [activeTab, currentView]);

  return {
    activeTab,
    currentView,
    modalType,
    selectedItem,
    currentPage,
    itemsPerPage,
    loading,
    error,
    success,
    isRefreshing: false, // Not used in this hook, kept for compatibility
    lastRefresh,
    setActiveTab,
    setCurrentView,
    setLoading,
    openCreateModal,
    openEditModal,
    openDeleteModal,
    openActivateModal,
    closeModal,
    showSuccess,
    showError,
    getPaginatedData,
    getTotalPages,
    handlePageChange,
    getTabDisplayName,
    getViewDisplayName,
    formatDate,
    formatTime,
    setLastRefresh
  };
};
