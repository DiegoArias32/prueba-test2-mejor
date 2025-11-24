/**
 * useNotifications Hook
 * Hook para gestionar notificaciones con integración al backend
 */

import React, { useState, useEffect, useCallback } from 'react';
import { NotificationService, NotificationDto } from '@/services/notifications/notification.service';

export interface NotificationHookReturn {
  notifications: NotificationDto[];
  unreadCount: number;
  isLoading: boolean;
  markAsRead: (notificationId: number) => Promise<void>;
  markAllAsRead: () => Promise<void>;
  addNotification: (notification: any) => void;
  refresh: () => Promise<void>;
  loadNotifications: () => Promise<void>;
}

/**
 * Hook to manage notifications with backend integration
 *
 * @example
 * const { notifications, unreadCount, markAsRead } = useNotifications();
 */
export function useNotifications(): NotificationHookReturn {
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  // Create service instance once
  const notificationService = React.useMemo(() => new NotificationService(), []);

  /**
   * Load notifications from backend
   */
  const loadNotifications = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await notificationService.getUserNotifications(1, 20);
      setNotifications(data);
    } catch (error) {
      console.error('Error loading notifications:', error);
    } finally {
      setIsLoading(false);
    }
  }, [notificationService]);

  /**
   * Load unread count from backend
   */
  const loadUnreadCount = useCallback(async () => {
    try {
      const count = await notificationService.getUnreadCount();
      setUnreadCount(count);
    } catch (error) {
      console.error('Error loading unread count:', error);
    }
  }, [notificationService]);

  /**
   * Mark a notification as read and remove it from the panel
   */
  const markAsRead = useCallback(async (notificationId: number) => {
    const success = await notificationService.markAsRead(notificationId);

    if (success) {
      // Remove notification from local state (make it disappear)
      setNotifications(prev => prev.filter(n => n.id !== notificationId));
      setUnreadCount(prev => Math.max(0, prev - 1));
      console.log(`[useNotifications] Notification ${notificationId} removed from panel`);
    } else {
      console.log(`[useNotifications] Failed to mark notification ${notificationId} as read, not removing from panel`);
    }
  }, [notificationService]);

  /**
   * Mark all notifications as read and remove them from the panel
   * Since the backend doesn't have a mark-all endpoint, we mark each notification individually
   * Then we remove them from the local state so they disappear from the panel
   */
  const markAllAsRead = useCallback(async () => {
    try {
      // Get all unread notification IDs
      const unreadNotifications = notifications.filter(n => !n.isRead);

      // Mark each one as read in the backend
      const promises = unreadNotifications.map(n => notificationService.markAsRead(n.id));
      await Promise.all(promises);

      // Remove all notifications from local state (clear the panel)
      setNotifications([]);
      setUnreadCount(0);

      console.log(`[useNotifications] Cleared ${unreadNotifications.length} notifications from panel`);
    } catch (error) {
      console.error('Error clearing notifications:', error);
      throw error;
    }
  }, [notificationService, notifications]);

  /**
   * Add a notification to the local state (for WebSocket updates)
   */
  const addNotification = useCallback((notification: Partial<NotificationDto> & { title: string; message: string }) => {
    const newNotification: NotificationDto = {
      id: notification.id || Date.now(),
      type: notification.type || 'system_notification',
      title: notification.title,
      message: notification.message,
      status: notification.status || 'Sent',
      isRead: false,
      sentAt: notification.timestamp || new Date().toISOString(),
      createdAt: new Date().toISOString(),
      appointmentNumber: notification.appointmentNumber
    };

    setNotifications(prev => [newNotification, ...prev]);
    setUnreadCount(prev => prev + 1);
  }, []);

  /**
   * Refresh notifications and count
   */
  const refresh = useCallback(async () => {
    await Promise.all([
      loadNotifications(),
      loadUnreadCount()
    ]);
  }, [loadNotifications, loadUnreadCount]);

  // Initial load
  useEffect(() => {
    loadNotifications();
    loadUnreadCount();
  }, [loadNotifications, loadUnreadCount]);

  return {
    notifications,
    unreadCount,
    isLoading,
    markAsRead,
    markAllAsRead,
    addNotification,
    refresh,
    loadNotifications
  };
}
