/**
 * useToastNotifications Hook
 * Enhanced hook for managing toast notifications with preferences
 */

import { useState, useCallback, useRef } from 'react';
import type { Toast, ToastType, ToastAction } from '@/shared/components/Toast';
import { notificationPreferencesService } from '@/services/notifications/notification-preferences.service';
import { notificationSoundsService } from '@/services/notifications/notification-sounds.service';

export interface ToastNotificationHook {
  toasts: Toast[];
  addToast: (toast: Omit<Toast, 'id'>) => string;
  removeToast: (id: string) => void;
  clearAll: () => void;
  success: (title: string, message: string, options?: ToastOptions) => string;
  error: (title: string, message: string, options?: ToastOptions) => string;
  warning: (title: string, message: string, options?: ToastOptions) => string;
  info: (title: string, message: string, options?: ToastOptions) => string;
}

export interface ToastOptions {
  duration?: number;
  actions?: ToastAction[];
  onView?: () => void;
  playSound?: boolean;
}

export const useToastNotifications = (): ToastNotificationHook => {
  const [toasts, setToasts] = useState<Toast[]>([]);
  const toastIdCounter = useRef(0);

  /**
   * Generate unique toast ID
   */
  const generateId = useCallback((): string => {
    toastIdCounter.current += 1;
    return `toast-${Date.now()}-${toastIdCounter.current}`;
  }, []);

  /**
   * Add a toast notification
   */
  const addToast = useCallback((toast: Omit<Toast, 'id'>): string => {
    const preferences = notificationPreferencesService.getAll();

    // Check if toast notifications are enabled
    if (!preferences.toastNotificationsEnabled) {
      return '';
    }

    const id = generateId();
    const duration = toast.duration ?? preferences.toastDuration;

    const newToast: Toast = {
      ...toast,
      id,
      duration
    };

    setToasts(prev => [...prev, newToast]);

    // Play sound if enabled
    if (preferences.soundsEnabled) {
      notificationSoundsService.play(toast.type as any);
    }

    return id;
  }, [generateId]);

  /**
   * Remove a toast notification
   */
  const removeToast = useCallback((id: string): void => {
    setToasts(prev => prev.filter(toast => toast.id !== id));
  }, []);

  /**
   * Clear all toasts
   */
  const clearAll = useCallback((): void => {
    setToasts([]);
  }, []);

  /**
   * Show success toast
   */
  const success = useCallback((
    title: string,
    message: string,
    options: ToastOptions = {}
  ): string => {
    return addToast({
      type: 'success',
      title,
      message,
      duration: options.duration,
      actions: options.actions,
      onView: options.onView
    });
  }, [addToast]);

  /**
   * Show error toast
   */
  const error = useCallback((
    title: string,
    message: string,
    options: ToastOptions = {}
  ): string => {
    return addToast({
      type: 'error',
      title,
      message,
      duration: options.duration,
      actions: options.actions,
      onView: options.onView
    });
  }, [addToast]);

  /**
   * Show warning toast
   */
  const warning = useCallback((
    title: string,
    message: string,
    options: ToastOptions = {}
  ): string => {
    return addToast({
      type: 'warning',
      title,
      message,
      duration: options.duration,
      actions: options.actions,
      onView: options.onView
    });
  }, [addToast]);

  /**
   * Show info toast
   */
  const info = useCallback((
    title: string,
    message: string,
    options: ToastOptions = {}
  ): string => {
    return addToast({
      type: 'info',
      title,
      message,
      duration: options.duration,
      actions: options.actions,
      onView: options.onView
    });
  }, [addToast]);

  return {
    toasts,
    addToast,
    removeToast,
    clearAll,
    success,
    error,
    warning,
    info
  };
};
