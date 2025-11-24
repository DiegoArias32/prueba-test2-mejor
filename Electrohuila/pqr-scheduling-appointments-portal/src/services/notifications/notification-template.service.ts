/**
 * Notification Template Service
 * Handles notification template CRUD operations
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  NotificationTemplateDto,
  CreateNotificationTemplateDto,
  UpdateNotificationTemplateDto
} from './notification-template.types';

export class NotificationTemplateService extends BaseHttpService {
  /**
   * Get all notification templates
   */
  async getNotificationTemplates(): Promise<NotificationTemplateDto[]> {
    return this.get<NotificationTemplateDto[]>('/notificationtemplates');
  }

  /**
   * Get notification template by ID
   */
  async getNotificationTemplateById(id: number): Promise<NotificationTemplateDto> {
    return this.get<NotificationTemplateDto>(`/notificationtemplates/${id}`);
  }

  /**
   * Get notification template by code
   */
  async getNotificationTemplateByCode(code: string): Promise<NotificationTemplateDto> {
    return this.get<NotificationTemplateDto>(`/notificationtemplates/by-code/${code}`);
  }

  /**
   * Create a new notification template
   */
  async createNotificationTemplate(
    dto: CreateNotificationTemplateDto
  ): Promise<NotificationTemplateDto> {
    return this.post<NotificationTemplateDto>('/notificationtemplates', dto);
  }

  /**
   * Update an existing notification template
   */
  async updateNotificationTemplate(
    dto: UpdateNotificationTemplateDto
  ): Promise<NotificationTemplateDto> {
    return this.put<NotificationTemplateDto>(`/notificationtemplates/${dto.id}`, dto);
  }

  /**
   * Delete (logical) a notification template
   */
  async deleteNotificationTemplate(id: number): Promise<void> {
    return this.delete<void>(`/notificationtemplates/${id}`);
  }

  /**
   * Activate a notification template
   */
  async activateNotificationTemplate(id: number): Promise<NotificationTemplateDto> {
    return this.patch<NotificationTemplateDto>(`/notificationtemplates/${id}/activate`, {});
  }

  /**
   * Validate template placeholders
   * Returns array of invalid placeholders found in the template
   */
  validatePlaceholders(template: string, validPlaceholders: readonly string[]): string[] {
    const placeholderRegex = /\{\{([A-Z_]+)\}\}/g;
    const foundPlaceholders = template.match(placeholderRegex) || [];
    const invalidPlaceholders = foundPlaceholders.filter(
      placeholder => !validPlaceholders.includes(placeholder)
    );
    return [...new Set(invalidPlaceholders)]; // Remove duplicates
  }

  /**
   * Replace placeholders in template with actual values
   */
  replacePlaceholders(template: string, data: Record<string, string>): string {
    let result = template;
    Object.keys(data).forEach(key => {
      const placeholder = `{{${key}}}`;
      const value = data[key];
      result = result.replace(new RegExp(placeholder, 'g'), value);
    });
    return result;
  }

  /**
   * Get placeholder count in template
   */
  getPlaceholderCount(template: string): number {
    const placeholderRegex = /\{\{[A-Z_]+\}\}/g;
    const matches = template.match(placeholderRegex);
    return matches ? matches.length : 0;
  }

  /**
   * Extract unique placeholders from template
   */
  extractPlaceholders(template: string): string[] {
    const placeholderRegex = /\{\{[A-Z_]+\}\}/g;
    const matches = template.match(placeholderRegex) || [];
    return [...new Set(matches)];
  }
}
