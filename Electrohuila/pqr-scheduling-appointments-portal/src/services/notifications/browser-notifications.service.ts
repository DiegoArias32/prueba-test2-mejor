/**
 * Browser Notifications Service
 * Manages native OS notifications using Web Notifications API
 */

export interface BrowserNotificationOptions {
  title: string;
  body: string;
  icon?: string;
  badge?: string;
  tag?: string;
  data?: any;
  requireInteraction?: boolean;
  silent?: boolean;
}

export type NotificationPermissionStatus = 'granted' | 'denied' | 'default';

class BrowserNotificationsService {
  private enabled: boolean = true;
  private permission: NotificationPermissionStatus = 'default';

  constructor() {
    this.loadPreferences();
    this.checkPermission();
  }

  /**
   * Load user preferences from localStorage
   */
  private loadPreferences(): void {
    if (typeof window === 'undefined') return;

    const saved = localStorage.getItem('browser_notifications_enabled');
    if (saved !== null) {
      this.enabled = saved === 'true';
    }
  }

  /**
   * Save user preferences to localStorage
   */
  private savePreferences(): void {
    if (typeof window === 'undefined') return;

    localStorage.setItem('browser_notifications_enabled', this.enabled.toString());
  }

  /**
   * Check current notification permission
   */
  private checkPermission(): void {
    if (typeof window === 'undefined' || !('Notification' in window)) {
      this.permission = 'denied';
      return;
    }

    this.permission = Notification.permission as NotificationPermissionStatus;
  }

  /**
   * Request notification permission
   */
  async requestPermission(): Promise<NotificationPermissionStatus> {
    if (typeof window === 'undefined' || !('Notification' in window)) {
      return 'denied';
    }

    try {
      const permission = await Notification.requestPermission();
      this.permission = permission as NotificationPermissionStatus;

      if (permission === 'granted') {
        this.enabled = true;
        this.savePreferences();
      }

      return this.permission;
    } catch (error) {
      console.error('[BrowserNotifications] Error requesting permission:', error);
      return 'denied';
    }
  }

  /**
   * Show browser notification
   */
  async show(options: BrowserNotificationOptions): Promise<Notification | null> {
    if (!this.enabled || typeof window === 'undefined' || !('Notification' in window)) {
      return null;
    }

    // Check permission
    if (this.permission !== 'granted') {
      const newPermission = await this.requestPermission();
      if (newPermission !== 'granted') {
        return null;
      }
    }

    try {
      const notification = new Notification(options.title, {
        body: options.body,
        icon: options.icon || '/images/logo.png',
        badge: options.badge || '/images/logo.png',
        tag: options.tag,
        data: options.data,
        requireInteraction: options.requireInteraction || false,
        silent: options.silent || false,
      });

      // Handle notification click
      notification.onclick = () => {
        window.focus();
        notification.close();

        // Handle custom data action
        if (options.data?.action) {
          this.handleNotificationAction(options.data.action, options.data);
        }
      };

      return notification;
    } catch (error) {
      console.error('[BrowserNotifications] Error showing notification:', error);
      return null;
    }
  }

  /**
   * Handle notification action
   */
  private handleNotificationAction(action: string, data: any): void {
    // Emit custom event for notification action
    const event = new CustomEvent('notification-action', {
      detail: { action, data }
    });
    window.dispatchEvent(event);
  }

  /**
   * Show notification only if app is not focused
   */
  async showIfNotFocused(options: BrowserNotificationOptions): Promise<Notification | null> {
    if (typeof window === 'undefined') return null;

    // Only show if document is not visible/focused
    if (document.hidden || !document.hasFocus()) {
      return await this.show(options);
    }

    return null;
  }

  /**
   * Enable browser notifications
   */
  enable(): void {
    this.enabled = true;
    this.savePreferences();
  }

  /**
   * Disable browser notifications
   */
  disable(): void {
    this.enabled = false;
    this.savePreferences();
  }

  /**
   * Toggle browser notifications
   */
  toggle(): boolean {
    this.enabled = !this.enabled;
    this.savePreferences();
    return this.enabled;
  }

  /**
   * Check if browser notifications are enabled
   */
  isEnabled(): boolean {
    return this.enabled;
  }

  /**
   * Get current permission status
   */
  getPermission(): NotificationPermissionStatus {
    return this.permission;
  }

  /**
   * Check if browser supports notifications
   */
  isSupported(): boolean {
    return typeof window !== 'undefined' && 'Notification' in window;
  }

  /**
   * Test browser notification
   */
  async test(): Promise<void> {
    await this.show({
      title: 'Notificación de Prueba',
      body: 'Las notificaciones del navegador están funcionando correctamente.',
      tag: 'test-notification'
    });
  }
}

// Singleton instance
export const browserNotificationsService = new BrowserNotificationsService();
