/**
 * @file HolidaysView.tsx
 * @description Holidays management view with CRUD operations
 */

import React, { useState, useCallback } from 'react';
import { FiEdit, FiCheckCircle, FiPlus, FiDownload, FiCalendar } from 'react-icons/fi';
import { HolidayDto } from '@/services/holidays/holiday.types';
import { HolidayModal } from '../components/modals/HolidayModal';
import { SortableTableHeader, PaginationControls, SearchBar, ViewToggle } from '../components/shared';
import { filterData, sortData, getPaginatedData, exportToCSV } from '../utils/tableUtils';

type SortDirection = 'asc' | 'desc';

interface HolidaysViewProps {
  holidays: HolidayDto[];
  hasPermission: (formCode: string, action: string) => boolean;
  onSaveHoliday: (data: Partial<HolidayDto>) => Promise<void>;
  onDeactivateHoliday: (id: number) => Promise<void>;
  onActivateHoliday: (id: number) => Promise<void>;
  onExportSuccess: (message: string) => void;
  onExportWarning: (message: string) => void;
}

export const HolidaysView: React.FC<HolidaysViewProps> = ({
  holidays,
  hasPermission,
  onSaveHoliday,
  onDeactivateHoliday,
  onActivateHoliday,
  onExportSuccess,
  onExportWarning
}) => {
  const [currentView, setCurrentView] = useState<'active' | 'inactive'>('active');
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortColumn, setSortColumn] = useState<string>('holidayDate');
  const [sortDirection, setSortDirection] = useState<SortDirection>('desc');
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<'create' | 'edit'>('create');
  const [selectedItem, setSelectedItem] = useState<HolidayDto | null>(null);
  const [filterType, setFilterType] = useState<'all' | 'National' | 'Local' | 'Company'>('all');
  const [filterYear, setFilterYear] = useState<number>(new Date().getFullYear());

  const itemsPerPage = 10;

  // Filter by active/inactive status
  const filteredByStatus = holidays.filter(holiday =>
    currentView === 'active' ? holiday.isActive : !holiday.isActive
  );

  // Filter by type
  const filteredByType = filterType === 'all'
    ? filteredByStatus
    : filteredByStatus.filter(h => h.holidayType.toUpperCase() === filterType.toUpperCase());

  // Filter by year
  const filteredByYear = filteredByType.filter(h => {
    const year = new Date(h.holidayDate).getFullYear();
    return year === filterYear;
  });

  // Apply search filter
  let processedData = filterData(filteredByYear, searchTerm, ['holidayName', 'holidayType', 'branchName']);
  processedData = sortData(processedData, sortColumn, sortDirection);
  const paginatedData = getPaginatedData(processedData, currentPage, itemsPerPage);

  const handleSort = (column: string) => {
    if (sortColumn === column) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortColumn(column);
      setSortDirection('asc');
    }
    setCurrentPage(1);
  };

  const handleSearch = useCallback((value: string) => {
    setSearchTerm(value);
    setCurrentPage(1);
  }, []);

  const handleViewChange = (view: 'active' | 'inactive') => {
    setCurrentView(view);
    setCurrentPage(1);
  };

  const handleTypeFilterChange = (type: 'all' | 'National' | 'Local' | 'Company') => {
    setFilterType(type);
    setCurrentPage(1);
  };

  const handleYearChange = (year: number) => {
    setFilterYear(year);
    setCurrentPage(1);
  };

  const handleExport = () => {
    exportToCSV(
      processedData,
      `festivos_${filterYear}`,
      {
        holidayDate: 'Fecha',
        holidayName: 'Nombre',
        holidayType: 'Tipo',
        branchName: 'Sucursal',
        isActive: 'Activo'
      },
      onExportSuccess,
      onExportWarning
    );
  };

  const formatDate = (dateString: string): string => {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('es-CO', {
        day: '2-digit',
        month: 'long',
        year: 'numeric'
      });
    } catch {
      return dateString;
    }
  };

  const getTypeLabel = (type: string): string => {
    const labels: Record<string, string> = {
      'National': 'Nacional',
      'Local': 'Local',
      'Company': 'Empresa'
    };
    return labels[type] || type;
  };

  const getTypeColor = (type: string): string => {
    const colors: Record<string, string> = {
      'National': 'bg-blue-100 text-blue-800',
      'Local': 'bg-green-100 text-green-800',
      'Company': 'bg-purple-100 text-purple-800'
    };
    return colors[type] || 'bg-gray-100 text-gray-800';
  };

  // Generate year options (current year ± 2 years)
  const currentYear = new Date().getFullYear();
  const yearOptions = Array.from({ length: 5 }, (_, i) => currentYear - 2 + i);

  return (
    <div className="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden">
      <div className="p-6">
        {/* Header Section */}
        <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4 mb-6">
          <div className="flex items-center gap-4">
            <ViewToggle currentView={currentView} onViewChange={handleViewChange} />

            {/* Type Filter */}
            <div className="flex gap-2">
              <button
                onClick={() => handleTypeFilterChange('all')}
                className={`px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
                  filterType === 'all'
                    ? 'bg-[#1797D5] text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                Todos
              </button>
              <button
                onClick={() => handleTypeFilterChange('National')}
                className={`px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
                  filterType === 'National'
                    ? 'bg-blue-600 text-white'
                    : 'bg-blue-100 text-blue-700 hover:bg-blue-200'
                }`}
              >
                Nacional
              </button>
              <button
                onClick={() => handleTypeFilterChange('Local')}
                className={`px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
                  filterType === 'Local'
                    ? 'bg-green-600 text-white'
                    : 'bg-green-100 text-green-700 hover:bg-green-200'
                }`}
              >
                Local
              </button>
              <button
                onClick={() => handleTypeFilterChange('Company')}
                className={`px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
                  filterType === 'Company'
                    ? 'bg-purple-600 text-white'
                    : 'bg-purple-100 text-purple-700 hover:bg-purple-200'
                }`}
              >
                Empresa
              </button>
            </div>

            {/* Year Filter */}
            <div className="flex items-center gap-2">
              <FiCalendar className="text-gray-500" />
              <select
                value={filterYear}
                onChange={(e) => handleYearChange(Number(e.target.value))}
                className="px-3 py-1.5 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:outline-none"
              >
                {yearOptions.map(year => (
                  <option key={year} value={year}>{year}</option>
                ))}
              </select>
            </div>
          </div>

          {hasPermission('HOLIDAYS', 'create') && (
            <button
              onClick={() => {
                setModalMode('create');
                setSelectedItem(null);
                setModalOpen(true);
              }}
              className="px-4 py-2 bg-[#1797D5] text-white rounded-lg hover:bg-[#203461] transition-colors flex items-center"
            >
              <FiPlus className="mr-2" />
              Crear Festivo
            </button>
          )}
        </div>

        {/* Search and Export */}
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6">
          <SearchBar
            value={searchTerm}
            onChange={handleSearch}
            placeholder="Buscar por nombre, tipo o sucursal..."
          />
          <button
            onClick={handleExport}
            className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors flex items-center whitespace-nowrap"
          >
            <FiDownload className="mr-2" />
            Exportar CSV
          </button>
        </div>

        {/* Stats Summary */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="bg-blue-50 rounded-lg p-4">
            <div className="text-sm text-blue-600 font-medium">Total</div>
            <div className="text-2xl font-bold text-blue-900">{filteredByStatus.length}</div>
          </div>
          <div className="bg-green-50 rounded-lg p-4">
            <div className="text-sm text-green-600 font-medium">Nacionales</div>
            <div className="text-2xl font-bold text-green-900">
              {filteredByStatus.filter(h => h.holidayType?.toLowerCase() === 'national').length}
            </div>
          </div>
          <div className="bg-purple-50 rounded-lg p-4">
            <div className="text-sm text-purple-600 font-medium">Locales</div>
            <div className="text-2xl font-bold text-purple-900">
              {filteredByStatus.filter(h => h.holidayType?.toLowerCase() === 'local').length}
            </div>
          </div>
          <div className="bg-orange-50 rounded-lg p-4">
            <div className="text-sm text-orange-600 font-medium">Empresa</div>
            <div className="text-2xl font-bold text-orange-900">
              {filteredByStatus.filter(h => h.holidayType?.toLowerCase() === 'company').length}
            </div>
          </div>
        </div>

        {/* Table */}
        <div className="overflow-x-auto">
          <table className="min-w-full bg-white">
            <thead className="bg-gray-50">
              <tr>
                <SortableTableHeader
                  column="holidayDate"
                  label="Fecha"
                  onClick={() => handleSort('holidayDate')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                <SortableTableHeader
                  column="holidayName"
                  label="Nombre"
                  onClick={() => handleSort('holidayName')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                <SortableTableHeader
                  column="holidayType"
                  label="Tipo"
                  onClick={() => handleSort('holidayType')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                <SortableTableHeader
                  column="branchName"
                  label="Sucursal"
                  onClick={() => handleSort('branchName')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                {currentView === 'inactive' && <SortableTableHeader column="isActive" label="Estado" />}
                <SortableTableHeader column="" label="Acciones" />
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {paginatedData.length === 0 ? (
                <tr>
                  <td colSpan={currentView === 'inactive' ? 6 : 5} className="px-6 py-12 text-center">
                    <div className="flex flex-col items-center justify-center text-gray-500">
                      <FiCalendar className="w-12 h-12 mb-2 opacity-50" />
                      <p className="text-lg font-medium">No se encontraron festivos</p>
                      <p className="text-sm">Intenta ajustar los filtros o crea uno nuevo</p>
                    </div>
                  </td>
                </tr>
              ) : (
                paginatedData.map((holiday) => (
                  <tr key={holiday.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {formatDate(holiday.holidayDate)}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-900">
                      {holiday.holidayName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${getTypeColor(holiday.holidayType)}`}>
                        {getTypeLabel(holiday.holidayType)}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {holiday.branchName || '-'}
                    </td>
                    {currentView === 'inactive' && (
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span
                          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                            holiday.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                          }`}
                        >
                          {holiday.isActive ? 'Activo' : 'Inactivo'}
                        </span>
                      </td>
                    )}
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                      {hasPermission('HOLIDAYS', 'update') ? (
                        <button
                          onClick={() => {
                            setModalMode('edit');
                            setSelectedItem(holiday);
                            setModalOpen(true);
                          }}
                          className="text-indigo-600 hover:text-indigo-900 transition-colors"
                        >
                          <FiEdit className="inline mr-1" />
                          Editar
                        </button>
                      ) : (
                        <span className="text-gray-400 cursor-not-allowed text-xs">Sin permiso</span>
                      )}
                      {currentView === 'active' && hasPermission('HOLIDAYS', 'delete') ? (
                        <button
                          onClick={() => onDeactivateHoliday(holiday.id)}
                          className="text-red-600 hover:text-red-900 transition-colors"
                        >
                          Desactivar
                        </button>
                      ) : currentView === 'inactive' && hasPermission('HOLIDAYS', 'update') ? (
                        <button
                          onClick={() => onActivateHoliday(holiday.id)}
                          className="text-green-600 hover:text-green-900 transition-colors font-medium"
                        >
                          <FiCheckCircle className="inline mr-1" />
                          Activar
                        </button>
                      ) : null}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
          <PaginationControls
            currentPage={currentPage}
            totalItems={processedData.length}
            itemsPerPage={itemsPerPage}
            onPageChange={setCurrentPage}
          />
        </div>
      </div>

      <HolidayModal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        onSave={onSaveHoliday}
        item={selectedItem ?? undefined}
        mode={modalMode}
      />
    </div>
  );
};
