/**
 * System Setting Service
 * Handles all system configuration operations
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  SystemSettingDto,
  CreateSystemSettingDto,
  UpdateSystemSettingDto,
  UpdateSystemSettingValueDto,
} from './system-setting.types';

export class SystemSettingService extends BaseHttpService {
  /**
   * Get all system settings
   * @returns Promise with array of system settings
   */
  async getSystemSettings(): Promise<SystemSettingDto[]> {
    return this.get<SystemSettingDto[]>('/systemsettings');
  }

  /**
   * Get system setting by key
   * @param key - Setting key (e.g., 'MAX_APPOINTMENTS_PER_DAY')
   * @returns Promise with system setting data
   */
  async getSystemSettingByKey(key: string): Promise<SystemSettingDto> {
    return this.get<SystemSettingDto>(`/systemsettings/${key}`);
  }

  /**
   * Create new system setting
   * @param dto - Create system setting data
   * @returns Promise with created system setting
   */
  async createSystemSetting(dto: CreateSystemSettingDto): Promise<SystemSettingDto> {
    return this.post<SystemSettingDto>('/systemsettings', dto);
  }

  /**
   * Update system setting (full update)
   * @param dto - Update system setting data
   * @returns Promise with updated system setting
   */
  async updateSystemSetting(dto: UpdateSystemSettingDto): Promise<SystemSettingDto> {
    return this.put<SystemSettingDto>(`/systemsettings/${dto.id}`, dto);
  }

  /**
   * Update system setting value only (PATCH)
   * @param key - Setting key
   * @param value - New setting value
   * @returns Promise with void
   */
  async updateSystemSettingValue(key: string, value: string): Promise<void> {
    const dto: UpdateSystemSettingValueDto = {
      settingKey: key,
      settingValue: value,
    };
    return this.patch<void>('/systemsettings/value', dto);
  }

  /**
   * Get active system settings only
   * @returns Promise with array of active system settings
   */
  async getActiveSystemSettings(): Promise<SystemSettingDto[]> {
    const allSettings = await this.getSystemSettings();
    return allSettings.filter(s => s.isActive);
  }

  /**
   * Get settings by type
   * @param type - Setting type (String, Number, Boolean, Time, Json)
   * @returns Promise with filtered system settings
   */
  async getSystemSettingsByType(type: SystemSettingDto['settingType']): Promise<SystemSettingDto[]> {
    const allSettings = await this.getSystemSettings();
    return allSettings.filter(s => s.settingType === type);
  }

  /**
   * Parse setting value based on type
   * @param setting - System setting DTO
   * @returns Parsed value
   */
  parseSettingValue(setting: SystemSettingDto): string | number | boolean | object | null {
    try {
      switch (setting.settingType) {
        case 'Number':
          return Number(setting.settingValue);

        case 'Boolean':
          return setting.settingValue.toLowerCase() === 'true';

        case 'Json':
          return JSON.parse(setting.settingValue);

        case 'Time':
        case 'String':
        default:
          return setting.settingValue;
      }
    } catch (error) {
      console.error(`Error parsing setting ${setting.settingKey}:`, error);
      return setting.settingValue;
    }
  }

  /**
   * Convert value to string for API
   * @param value - Value to convert
   * @param type - Setting type
   * @returns String representation
   */
  stringifySettingValue(value: any, type: SystemSettingDto['settingType']): string {
    if (type === 'Json') {
      return JSON.stringify(value);
    }
    return String(value);
  }
}
