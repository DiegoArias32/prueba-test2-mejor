/**
 * Notification Sounds Service
 * Manages sound effects for different notification types
 */

export type NotificationSoundType = 'success' | 'warning' | 'error' | 'info' | 'default';

interface SoundConfig {
  frequency: number;
  duration: number;
  volume: number;
}

// Sound configurations for different notification types
const SOUND_CONFIGS: Record<NotificationSoundType, SoundConfig> = {
  success: { frequency: 800, duration: 150, volume: 0.3 },
  info: { frequency: 600, duration: 100, volume: 0.25 },
  warning: { frequency: 500, duration: 200, volume: 0.3 },
  error: { frequency: 400, duration: 250, volume: 0.35 },
  default: { frequency: 650, duration: 120, volume: 0.25 }
};

class NotificationSoundsService {
  private audioContext: AudioContext | null = null;
  private enabled: boolean = true;

  constructor() {
    // Initialize AudioContext lazily
    this.loadPreferences();
  }

  /**
   * Initialize AudioContext (must be called after user interaction)
   */
  private initAudioContext(): AudioContext {
    if (!this.audioContext) {
      this.audioContext = new (window.AudioContext || (window as any).webkitAudioContext)();
    }
    return this.audioContext;
  }

  /**
   * Load user preferences from localStorage
   */
  private loadPreferences(): void {
    if (typeof window === 'undefined') return;

    const saved = localStorage.getItem('notification_sounds_enabled');
    if (saved !== null) {
      this.enabled = saved === 'true';
    }
  }

  /**
   * Save user preferences to localStorage
   */
  private savePreferences(): void {
    if (typeof window === 'undefined') return;

    localStorage.setItem('notification_sounds_enabled', this.enabled.toString());
  }

  /**
   * Play notification sound
   */
  async play(type: NotificationSoundType = 'default'): Promise<void> {
    if (!this.enabled || typeof window === 'undefined') return;

    try {
      const config = SOUND_CONFIGS[type] || SOUND_CONFIGS.default;
      const audioContext = this.initAudioContext();

      // Resume audio context if suspended (browser autoplay policy)
      if (audioContext.state === 'suspended') {
        await audioContext.resume();
      }

      // Create oscillator for beep sound
      const oscillator = audioContext.createOscillator();
      const gainNode = audioContext.createGain();

      oscillator.connect(gainNode);
      gainNode.connect(audioContext.destination);

      oscillator.type = 'sine';
      oscillator.frequency.value = config.frequency;

      // Envelope for smooth sound
      const now = audioContext.currentTime;
      gainNode.gain.setValueAtTime(0, now);
      gainNode.gain.linearRampToValueAtTime(config.volume, now + 0.01);
      gainNode.gain.exponentialRampToValueAtTime(0.01, now + config.duration / 1000);

      oscillator.start(now);
      oscillator.stop(now + config.duration / 1000);

    } catch (error) {
      console.error('[NotificationSounds] Error playing sound:', error);
    }
  }

  /**
   * Play success sound
   */
  async playSuccess(): Promise<void> {
    await this.play('success');
  }

  /**
   * Play error sound
   */
  async playError(): Promise<void> {
    await this.play('error');
  }

  /**
   * Play warning sound
   */
  async playWarning(): Promise<void> {
    await this.play('warning');
  }

  /**
   * Play info sound
   */
  async playInfo(): Promise<void> {
    await this.play('info');
  }

  /**
   * Enable notification sounds
   */
  enable(): void {
    this.enabled = true;
    this.savePreferences();
  }

  /**
   * Disable notification sounds
   */
  disable(): void {
    this.enabled = false;
    this.savePreferences();
  }

  /**
   * Toggle notification sounds
   */
  toggle(): boolean {
    this.enabled = !this.enabled;
    this.savePreferences();
    return this.enabled;
  }

  /**
   * Check if sounds are enabled
   */
  isEnabled(): boolean {
    return this.enabled;
  }

  /**
   * Test sound playback
   */
  async test(type: NotificationSoundType = 'default'): Promise<void> {
    const wasEnabled = this.enabled;
    this.enabled = true;
    await this.play(type);
    this.enabled = wasEnabled;
  }

  /**
   * Cleanup resources
   */
  dispose(): void {
    if (this.audioContext) {
      this.audioContext.close();
      this.audioContext = null;
    }
  }
}

// Singleton instance
export const notificationSoundsService = new NotificationSoundsService();
