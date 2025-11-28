/**
 * Holiday Service
 * Handles all holiday-related operations
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  HolidayDto,
  CreateNationalHolidayDto,
  CreateLocalHolidayDto,
  CreateCompanyHolidayDto,
  UpdateHolidayDto,
  CheckHolidayResponseDto,
} from './holiday.types';

export class HolidayService extends BaseHttpService {
  /**
   * Get all holidays
   * @returns Promise with array of holidays
   */
  async getHolidays(): Promise<HolidayDto[]> {
    return this.get<HolidayDto[]>('/holidays');
  }

  /**
   * Get holiday by ID
   * @param id - Holiday ID
   * @returns Promise with holiday data
   */
  async getHolidayById(id: number): Promise<HolidayDto> {
    return this.get<HolidayDto>(`/holidays/${id}`);
  }

  /**
   * Get holidays in date range
   * @param startDate - Start date (ISO format: YYYY-MM-DD)
   * @param endDate - End date (ISO format: YYYY-MM-DD)
   * @returns Promise with array of holidays in range
   */
  async getHolidaysInRange(startDate: string, endDate: string): Promise<HolidayDto[]> {
    return this.get<HolidayDto[]>(`/holidays/range?startDate=${startDate}&endDate=${endDate}`);
  }

  /**
   * Check if a date is a holiday
   * @param date - Date to check (ISO format: YYYY-MM-DD)
   * @returns Promise with check result
   */
  async checkIfHoliday(date: string): Promise<CheckHolidayResponseDto> {
    return this.get<CheckHolidayResponseDto>(`/holidays/check?date=${date}`);
  }

  /**
   * Create national holiday
   * @param dto - National holiday data
   * @returns Promise with created holiday
   */
  async createNationalHoliday(dto: CreateNationalHolidayDto): Promise<HolidayDto> {
    return this.post<HolidayDto>('/holidays/national', dto);
  }

  /**
   * Create local holiday (branch-specific)
   * @param dto - Local holiday data
   * @returns Promise with created holiday
   */
  async createLocalHoliday(dto: CreateLocalHolidayDto): Promise<HolidayDto> {
    return this.post<HolidayDto>('/holidays/local', dto);
  }

  /**
   * Create company holiday
   * @param dto - Company holiday data
   * @returns Promise with created holiday
   */
  async createCompanyHoliday(dto: CreateCompanyHolidayDto): Promise<HolidayDto> {
    return this.post<HolidayDto>('/holidays/company', dto);
  }

  /**
   * Update holiday
   * @param dto - Update holiday data
   * @returns Promise with updated holiday
   */
  async updateHoliday(dto: UpdateHolidayDto): Promise<HolidayDto> {
    return this.put<HolidayDto>(`/holidays/${dto.id}`, dto);
  }

  /**
   * Delete holiday (logical delete)
   * @param id - Holiday ID
   * @returns Promise with void
   */
  async deleteHoliday(id: number): Promise<void> {
    return this.delete<void>(`/holidays/${id}`);
  }

  /**
   * Activate holiday
   * @param id - Holiday ID
   * @returns Promise with success response
   */
  async activateHoliday(id: number): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/holidays/${id}/activate`, {});
  }

  /**
   * Deactivate holiday
   * @param id - Holiday ID
   * @returns Promise with success response
   */
  async deactivateHoliday(id: number): Promise<{ success: boolean; message: string }> {
    return this.patch<{ success: boolean; message: string }>(`/holidays/${id}/deactivate`, {});
  }

  /**
   * Get holidays by type
   * @param type - Holiday type (National, Local, Company)
   * @returns Promise with filtered holidays
   */
  async getHolidaysByType(type: 'National' | 'Local' | 'Company'): Promise<HolidayDto[]> {
    const allHolidays = await this.getHolidays();
    return allHolidays.filter(h => h.holidayType === type);
  }

  /**
   * Get holidays by year
   * @param year - Year to filter
   * @returns Promise with filtered holidays
   */
  async getHolidaysByYear(year: number): Promise<HolidayDto[]> {
    const allHolidays = await this.getHolidays();
    return allHolidays.filter(h => {
      const holidayYear = new Date(h.holidayDate).getFullYear();
      return holidayYear === year;
    });
  }

  /**
   * Get holidays by month and year
   * @param year - Year to filter
   * @param month - Month to filter (1-12)
   * @returns Promise with filtered holidays
   */
  async getHolidaysByMonthYear(year: number, month: number): Promise<HolidayDto[]> {
    const allHolidays = await this.getHolidays();
    return allHolidays.filter(h => {
      const date = new Date(h.holidayDate);
      return date.getFullYear() === year && (date.getMonth() + 1) === month;
    });
  }

  /**
   * Get active holidays
   * @returns Promise with array of active holidays
   */
  async getActiveHolidays(): Promise<HolidayDto[]> {
    const allHolidays = await this.getHolidays();
    return allHolidays.filter(h => h.isActive);
  }

  /**
   * Format date for display (DD/MM/YYYY)
   * @param isoDate - ISO date string
   * @returns Formatted date string
   */
  formatDateForDisplay(isoDate: string): string {
    try {
      const date = new Date(isoDate);
      const day = date.getDate().toString().padStart(2, '0');
      const month = (date.getMonth() + 1).toString().padStart(2, '0');
      const year = date.getFullYear();
      return `${day}/${month}/${year}`;
    } catch {
      return isoDate;
    }
  }

  /**
   * Format date for API (YYYY-MM-DD)
   * @param date - JavaScript Date object
   * @returns ISO date string
   */
  formatDateForAPI(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
