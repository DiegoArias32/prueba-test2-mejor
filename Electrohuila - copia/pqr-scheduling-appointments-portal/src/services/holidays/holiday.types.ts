/**
 * Holiday Types
 * Type definitions for holiday management
 */

/**
 * Holiday type enum
 */
export type HolidayType = 'National' | 'Local' | 'Company';

/**
 * Holiday DTO - Main holiday data structure
 */
export interface HolidayDto {
  id: number;
  holidayDate: string; // ISO date (YYYY-MM-DD)
  holidayName: string;
  holidayType: HolidayType;
  branchId?: number; // Solo para tipo Local
  branchName?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

/**
 * Create National Holiday DTO
 */
export interface CreateNationalHolidayDto {
  holidayDate: string;
  holidayName: string;
}

/**
 * Create Local Holiday DTO (por sucursal)
 */
export interface CreateLocalHolidayDto {
  holidayDate: string;
  holidayName: string;
  branchId: number;
}

/**
 * Create Company Holiday DTO
 */
export interface CreateCompanyHolidayDto {
  holidayDate: string;
  holidayName: string;
}

/**
 * Update Holiday DTO
 */
export interface UpdateHolidayDto {
  id: number;
  holidayDate: string;
  holidayName: string;
  holidayType: HolidayType;
  branchId?: number; // Solo para tipo Local
}

/**
 * Check Holiday Response DTO
 */
export interface CheckHolidayResponseDto {
  isHoliday: boolean;
  holidayName?: string;
}

/**
 * Holiday filter options
 */
export interface HolidayFilterOptions {
  type?: HolidayType;
  year?: number;
  month?: number;
  branchId?: number;
  isActive?: boolean;
}
