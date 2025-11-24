/**
 * System Setting Types
 * Type definitions for system configuration settings
 */

/**
 * System Setting DTO (from API)
 */
export interface SystemSettingDto {
  id: number;
  settingKey: string;
  settingValue: string;
  settingType: 'String' | 'Number' | 'Boolean' | 'Time' | 'Json';
  description: string;
  isEncrypted: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

/**
 * Create System Setting DTO
 */
export interface CreateSystemSettingDto {
  settingKey: string;
  settingValue: string;
  settingType: string;
  description: string;
  isEncrypted?: boolean;
}

/**
 * Update System Setting DTO (full update)
 */
export interface UpdateSystemSettingDto {
  id: number;
  settingKey: string;
  settingValue: string;
  settingType: string;
  description: string;
}

/**
 * Update System Setting Value DTO (value-only update)
 */
export interface UpdateSystemSettingValueDto {
  settingKey: string;
  settingValue: string;
}

/**
 * Setting categories for organization
 */
export type SettingCategory = 'appointments' | 'notifications' | 'businessHours' | 'general';

/**
 * Setting category configuration
 */
export interface SettingCategoryConfig {
  id: SettingCategory;
  label: string;
  description: string;
  icon: string;
  settings: string[];
}

/**
 * Parsed setting value (typed)
 */
export type ParsedSettingValue = string | number | boolean | object | null;

/**
 * Setting validation result
 */
export interface ValidationResult {
  isValid: boolean;
  error?: string;
}

/**
 * Setting with parsed value
 */
export interface ParsedSystemSetting extends Omit<SystemSettingDto, 'settingValue'> {
  settingValue: string;
  parsedValue: ParsedSettingValue;
}
