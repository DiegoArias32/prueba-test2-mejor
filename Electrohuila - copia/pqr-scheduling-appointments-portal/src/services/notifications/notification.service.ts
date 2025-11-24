/**
 * Notification Service
 * Servicio para gestionar notificaciones de usuario en tiempo real
 */

import { BaseHttpService } from '../base/base-http.service';

export interface NotificationDto {
  id: number;
  type: string;
  title: string;
  message: string;
  status: string;
  isRead: boolean;
  sentAt: string | null;
  createdAt: string;
  appointmentNumber?: string;
}

export class NotificationService extends BaseHttpService {
  /**
   * Get user notifications with pagination
   * Uses /my-notifications endpoint which filters by assigned appointment types
   * Only returns IN_APP notifications (notifications for admins/users)
   * Filters out EMAIL/WHATSAPP notifications (which are for clients)
   * Only shows unread notifications (isRead = false)
   */
  async getUserNotifications(pageNumber: number = 1, pageSize: number = 20): Promise<NotificationDto[]> {
    try {
      const response = await this.get<{ items: NotificationDto[], totalCount: number, pageNumber: number, pageSize: number }>(`/notifications/my-notifications?pageNumber=${pageNumber}&pageSize=${pageSize}`);

      // The backend returns PagedList structure directly: { items: [...], totalCount: ..., pageNumber: ..., pageSize: ... }
      if (response && response.items && Array.isArray(response.items)) {
        // Filter to show only IN_APP notifications (for admins) that are unread
        // EMAIL/WHATSAPP notifications are for clients and should not appear in admin panel
        // Already read notifications should not appear (they were dismissed)
        const unreadInAppNotifications = response.items.filter(n =>
          n.type === 'IN_APP' && !n.isRead
        );
        console.log(`[NotificationService] Loaded ${response.items.length} total notifications, filtered to ${unreadInAppNotifications.length} unread IN_APP notifications`);
        return unreadInAppNotifications;
      }

      console.warn('Unexpected notification response structure:', response);
      return [];
    } catch (error) {
      console.error('Error loading notifications:', error);
      return [];
    }
  }

  /**
   * Get unread notification count
   */
  async getUnreadCount(): Promise<number> {
    const response = await this.get<{ unreadCount: number }>('/notifications/unread-count');
    return response.unreadCount;
  }

  /**
   * Mark notification as read
   * Only works for IN_APP notifications (notifications with USER_ID)
   * EMAIL/WHATSAPP notifications (with CLIENT_ID) cannot be marked as read by admins
   * Returns true if successful, false otherwise
   */
  async markAsRead(notificationId: number): Promise<boolean> {
    // Skip if notificationId is a timestamp (> 1000000000000)
    // These are temporary UI-only notifications from SignalR, not from DB
    if (notificationId > 1000000000000) {
      console.log(`[NotificationService] Skipping mark as read for temporary notification: ${notificationId}`);
      return false;
    }

    try {
      await this.patch(`/notifications/${notificationId}/mark-read`, {});
      console.log(`[NotificationService] Notification ${notificationId} marked as read successfully`);
      return true;
    } catch (error: any) {
      // Silently ignore 400 errors - notification might be EMAIL/WHATSAPP for a client (not IN_APP for admin)
      if (error?.message?.includes('400')) {
        console.log(`[NotificationService] Cannot mark notification ${notificationId} as read - probably EMAIL/WHATSAPP notification for client`);
      } else {
        console.log(`[NotificationService] Error marking notification ${notificationId} as read:`, error);
      }
      return false;
    }
  }

  /**
   * Mark all notifications as read
   */
  async markAllAsRead(): Promise<void> {
    const userId = this.getCurrentUserId();
    await this.patch(`/notifications/user/${userId}/mark-all-read`, {});
  }

  /**
   * Get current user ID from localStorage
   */
  private getCurrentUserId(): number {
    if (typeof window === 'undefined') {
      throw new Error('Cannot get user ID on server side');
    }

    const user = localStorage.getItem('user');
    if (!user) {
      throw new Error('User not found in localStorage');
    }

    try {
      const userData = JSON.parse(user);
      if (!userData.id) {
        throw new Error('User ID not found in user data');
      }
      return userData.id;
    } catch {
      throw new Error('Invalid user data in localStorage');
    }
  }
}

// Singleton instance
export const notificationService = new NotificationService();
