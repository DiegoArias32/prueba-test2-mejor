/**
 * @file tableUtils.ts
 * @description Utility functions for table operations (search, sort, export)
 */

type SortDirection = 'asc' | 'desc';

/**
 * Filter data based on search term and searchable fields
 */
export const filterData = <T extends object>(
  data: T[],
  searchTerm: string,
  searchableFields: string[]
): T[] => {
  if (!searchTerm) return data;

  const lowerSearch = searchTerm.toLowerCase();
  return data.filter(item =>
    searchableFields.some(field => {
      const value = (item as Record<string, unknown>)[field];
      if (value === null || value === undefined) return false;
      return String(value).toLowerCase().includes(lowerSearch);
    })
  );
};

/**
 * Sort data by column and direction
 */
export const sortData = <T extends object>(
  data: T[],
  column: string,
  direction: SortDirection
): T[] => {
  if (!column) return data;

  return [...data].sort((a, b) => {
    let aVal = (a as Record<string, unknown>)[column];
    let bVal = (b as Record<string, unknown>)[column];

    // Handle null/undefined
    if (aVal === null || aVal === undefined) return direction === 'asc' ? 1 : -1;
    if (bVal === null || bVal === undefined) return direction === 'asc' ? -1 : 1;

    // Handle arrays (like roles)
    if (Array.isArray(aVal)) aVal = aVal.join(', ');
    if (Array.isArray(bVal)) bVal = bVal.join(', ');

    // Handle dates
    if (column.includes('Date') || column === 'createdAt') {
      aVal = new Date(aVal as string | number | Date).getTime();
      bVal = new Date(bVal as string | number | Date).getTime();
    }

    // String comparison
    if (typeof aVal === 'string' && typeof bVal === 'string') {
      return direction === 'asc'
        ? aVal.localeCompare(bVal)
        : bVal.localeCompare(aVal);
    }

    // Number comparison
    return direction === 'asc' ? (aVal as number) - (bVal as number) : (bVal as number) - (aVal as number);
  });
};

/**
 * Get paginated data
 */
export const getPaginatedData = <T>(
  data: T[],
  currentPage: number,
  itemsPerPage: number
): T[] => {
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  return data.slice(startIndex, endIndex);
};

/**
 * Export data to CSV
 */
export const exportToCSV = <T extends object>(
  data: T[],
  filename: string,
  headers: { [key: string]: string },
  onSuccess?: (message: string) => void,
  onWarning?: (message: string) => void
): void => {
  if (data.length === 0) {
    if (onWarning) {
      onWarning('No hay datos para exportar');
    }
    return;
  }

  // Create CSV header
  const headerKeys = Object.keys(headers);
  const csvHeader = Object.values(headers).join(',') + '\n';

  // Create CSV rows
  const csvRows = data.map(item => {
    return headerKeys.map(key => {
      let value = (item as Record<string, unknown>)[key];

      // Handle arrays
      if (Array.isArray(value)) {
        value = value.join('; ');
      }

      // Handle null/undefined
      if (value === null || value === undefined) {
        value = '';
      }

      // Handle booleans
      if (typeof value === 'boolean') {
        value = value ? 'Sí' : 'No';
      }

      // Escape quotes and wrap in quotes if contains comma
      const stringValue = String(value).replace(/"/g, '""');
      const finalValue = stringValue.includes(',') || stringValue.includes('\n') || stringValue.includes('"')
        ? `"${stringValue}"`
        : stringValue;

      return finalValue;
    }).join(',');
  }).join('\n');

  // Combine header and rows
  const csv = csvHeader + csvRows;

  // Create blob and download
  const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' });
  const link = document.createElement('a');
  const url = URL.createObjectURL(blob);

  link.setAttribute('href', url);
  link.setAttribute('download', `${filename}_${new Date().toISOString().split('T')[0]}.csv`);
  link.style.visibility = 'hidden';

  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);

  if (onSuccess) {
    onSuccess(`${data.length} registros exportados a CSV`);
  }
};

/**
 * Format date string
 */
export const formatDate = (dateString: string): string => {
  if (!dateString) return '';
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  });
};

/**
 * Format time string
 */
export const formatTime = (timeString: string): string => {
  if (!timeString) return '';
  if (timeString.includes(':')) {
    const [hours, minutes] = timeString.split(':');
    const hour24 = parseInt(hours);
    const ampm = hour24 >= 12 ? 'PM' : 'AM';
    const hour12 = hour24 % 12 || 12;
    return `${hour12}:${minutes} ${ampm}`;
  }
  return timeString;
};
