/**
 * useEnhancedNotifications Hook
 * Enhanced notification hook with sounds, browser notifications, and toast integration
 */

import { useCallback } from 'react';
import { useNotifications, NotificationHookReturn } from './useNotifications';
import { notificationSoundsService } from '@/services/notifications/notification-sounds.service';
import { browserNotificationsService } from '@/services/notifications/browser-notifications.service';
import { notificationPreferencesService } from '@/services/notifications/notification-preferences.service';
import type { NotificationDto } from '@/services/notifications/notification.service';

export interface EnhancedNotificationHookReturn extends NotificationHookReturn {
  // Browser notifications
  requestBrowserPermission: () => Promise<void>;
  browserNotificationsEnabled: boolean;
  browserNotificationsSupported: boolean;

  // Sound settings
  soundsEnabled: boolean;
  toggleSounds: () => void;
  testSound: () => Promise<void>;

  // Browser notification settings
  toggleBrowserNotifications: () => void;

  // Enhanced add notification with automatic browser/sound notifications
  addEnhancedNotification: (notification: any) => void;

  // Backwards compatibility
  clearAll: () => void;
}

/**
 * Enhanced hook for notifications with all UX features
 */
export function useEnhancedNotifications(): EnhancedNotificationHookReturn {
  // Get base notification hook
  const baseHook = useNotifications();

  /**
   * Request browser notification permission
   */
  const requestBrowserPermission = useCallback(async () => {
    await browserNotificationsService.requestPermission();
  }, []);

  /**
   * Toggle sounds on/off
   */
  const toggleSounds = useCallback(() => {
    notificationSoundsService.toggle();
    notificationPreferencesService.toggleSounds();
  }, []);

  /**
   * Toggle browser notifications on/off
   */
  const toggleBrowserNotifications = useCallback(() => {
    browserNotificationsService.toggle();
    notificationPreferencesService.toggleBrowserNotifications();
  }, []);

  /**
   * Test sound playback
   */
  const testSound = useCallback(async () => {
    await notificationSoundsService.test('info');
  }, []);

  /**
   * Get notification sound type based on notification type
   */
  const getSoundType = (notificationType: string): 'success' | 'error' | 'warning' | 'info' => {
    switch (notificationType) {
      case 'appointment':
      case 'appointment_created':
      case 'appointment_assigned':
        return 'info';
      case 'appointment_completed':
        return 'success';
      case 'appointment_cancelled':
        return 'warning';
      case 'alert':
      case 'error':
        return 'error';
      case 'reminder':
        return 'warning';
      default:
        return 'info';
    }
  };

  /**
   * Enhanced add notification with automatic sound and browser notifications
   */
  const addEnhancedNotification = useCallback((notification: any) => {
    const preferences = notificationPreferencesService.getAll();

    // Add to in-app notifications
    baseHook.addNotification(notification);

    // Play sound if enabled
    if (preferences.soundsEnabled) {
      const soundType = getSoundType(notification.type);
      notificationSoundsService.play(soundType);
    }

    // Show browser notification if enabled and app is not focused
    if (preferences.browserNotificationsEnabled) {
      browserNotificationsService.showIfNotFocused({
        title: notification.title || 'Nueva Notificación',
        body: notification.message || '',
        tag: notification.id?.toString() || `notification-${Date.now()}`,
        data: {
          action: 'view',
          appointmentId: notification.appointmentId,
          notificationId: notification.id
        }
      });
    }
  }, [baseHook]);

  return {
    ...baseHook,
    clearAll: baseHook.markAllAsRead, // Alias for backwards compatibility
    requestBrowserPermission,
    browserNotificationsEnabled: browserNotificationsService.isEnabled(),
    browserNotificationsSupported: browserNotificationsService.isSupported(),
    soundsEnabled: notificationSoundsService.isEnabled(),
    toggleSounds,
    toggleBrowserNotifications,
    testSound,
    addEnhancedNotification
  };
}

/**
 * Hook to convert NotificationDto to notification for display
 */
export function useNotificationConverter() {
  const convertToDisplayNotification = useCallback((dto: NotificationDto) => {
    return {
      id: dto.id.toString(),
      type: getNotificationType(dto.type),
      title: dto.title,
      message: dto.message,
      timestamp: new Date(dto.sentAt || dto.createdAt),
      read: dto.isRead,
      appointmentId: dto.appointmentNumber ? parseInt(dto.appointmentNumber) : undefined
    };
  }, []);

  return { convertToDisplayNotification };
}

/**
 * Get notification type for display
 */
function getNotificationType(type: string): 'appointment' | 'reminder' | 'update' | 'alert' {
  if (type.includes('appointment')) return 'appointment';
  if (type.includes('reminder')) return 'reminder';
  if (type.includes('update')) return 'update';
  return 'alert';
}
