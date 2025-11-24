/**
 * Notification Preferences Service
 * Manages user preferences for all notification features
 */

export interface NotificationPreferences {
  soundsEnabled: boolean;
  browserNotificationsEnabled: boolean;
  toastNotificationsEnabled: boolean;
  soundVolume: number;
  toastDuration: number;
  toastPosition: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left';
  showInAppNotifications: boolean;
}

const DEFAULT_PREFERENCES: NotificationPreferences = {
  soundsEnabled: true,
  browserNotificationsEnabled: true,
  toastNotificationsEnabled: true,
  soundVolume: 0.5,
  toastDuration: 5000,
  toastPosition: 'top-right',
  showInAppNotifications: true
};

const STORAGE_KEY = 'notification_preferences';

class NotificationPreferencesService {
  private preferences: NotificationPreferences;
  private listeners: Set<(prefs: NotificationPreferences) => void> = new Set();

  constructor() {
    this.preferences = this.load();
  }

  /**
   * Load preferences from localStorage
   */
  private load(): NotificationPreferences {
    if (typeof window === 'undefined') {
      return { ...DEFAULT_PREFERENCES };
    }

    try {
      const saved = localStorage.getItem(STORAGE_KEY);
      if (saved) {
        const parsed = JSON.parse(saved);
        return { ...DEFAULT_PREFERENCES, ...parsed };
      }
    } catch (error) {
      console.error('[NotificationPreferences] Error loading preferences:', error);
    }

    return { ...DEFAULT_PREFERENCES };
  }

  /**
   * Save preferences to localStorage
   */
  private save(): void {
    if (typeof window === 'undefined') return;

    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(this.preferences));
      this.notifyListeners();
    } catch (error) {
      console.error('[NotificationPreferences] Error saving preferences:', error);
    }
  }

  /**
   * Notify all listeners of preference changes
   */
  private notifyListeners(): void {
    this.listeners.forEach(listener => {
      try {
        listener(this.getAll());
      } catch (error) {
        console.error('[NotificationPreferences] Listener error:', error);
      }
    });
  }

  /**
   * Get all preferences
   */
  getAll(): NotificationPreferences {
    return { ...this.preferences };
  }

  /**
   * Get a specific preference
   */
  get<K extends keyof NotificationPreferences>(key: K): NotificationPreferences[K] {
    return this.preferences[key];
  }

  /**
   * Set a specific preference
   */
  set<K extends keyof NotificationPreferences>(key: K, value: NotificationPreferences[K]): void {
    this.preferences[key] = value;
    this.save();
  }

  /**
   * Update multiple preferences
   */
  update(updates: Partial<NotificationPreferences>): void {
    this.preferences = { ...this.preferences, ...updates };
    this.save();
  }

  /**
   * Reset to default preferences
   */
  reset(): void {
    this.preferences = { ...DEFAULT_PREFERENCES };
    this.save();
  }

  /**
   * Subscribe to preference changes
   */
  subscribe(listener: (prefs: NotificationPreferences) => void): () => void {
    this.listeners.add(listener);

    // Return unsubscribe function
    return () => {
      this.listeners.delete(listener);
    };
  }

  /**
   * Toggle sounds
   */
  toggleSounds(): boolean {
    this.preferences.soundsEnabled = !this.preferences.soundsEnabled;
    this.save();
    return this.preferences.soundsEnabled;
  }

  /**
   * Toggle browser notifications
   */
  toggleBrowserNotifications(): boolean {
    this.preferences.browserNotificationsEnabled = !this.preferences.browserNotificationsEnabled;
    this.save();
    return this.preferences.browserNotificationsEnabled;
  }

  /**
   * Toggle toast notifications
   */
  toggleToastNotifications(): boolean {
    this.preferences.toastNotificationsEnabled = !this.preferences.toastNotificationsEnabled;
    this.save();
    return this.preferences.toastNotificationsEnabled;
  }

  /**
   * Set sound volume (0-1)
   */
  setSoundVolume(volume: number): void {
    this.preferences.soundVolume = Math.max(0, Math.min(1, volume));
    this.save();
  }

  /**
   * Set toast duration in milliseconds
   */
  setToastDuration(duration: number): void {
    this.preferences.toastDuration = Math.max(1000, Math.min(30000, duration));
    this.save();
  }

  /**
   * Set toast position
   */
  setToastPosition(position: NotificationPreferences['toastPosition']): void {
    this.preferences.toastPosition = position;
    this.save();
  }

  /**
   * Export preferences as JSON
   */
  export(): string {
    return JSON.stringify(this.preferences, null, 2);
  }

  /**
   * Import preferences from JSON
   */
  import(json: string): boolean {
    try {
      const parsed = JSON.parse(json);
      this.preferences = { ...DEFAULT_PREFERENCES, ...parsed };
      this.save();
      return true;
    } catch (error) {
      console.error('[NotificationPreferences] Error importing preferences:', error);
      return false;
    }
  }
}

// Singleton instance
export const notificationPreferencesService = new NotificationPreferencesService();
