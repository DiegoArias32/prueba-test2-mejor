/**
 * @file TimesView.tsx
 * @description Available times management view with CRUD operations
 */

import React, { useState, useCallback } from 'react';
import { FiEdit, FiCheckCircle, FiPlus, FiDownload } from 'react-icons/fi';
import { AvailableTimeDto, BranchDto, AppointmentTypeDto } from '@/services/api';
import { AvailableTimeModal } from '../components/modals';
import { SortableTableHeader, PaginationControls, SearchBar, ViewToggle } from '../components/shared';
import { filterData, sortData, getPaginatedData, exportToCSV, formatTime } from '../utils/tableUtils';

type SortDirection = 'asc' | 'desc';

interface TimesViewProps {
  availableTimes: AvailableTimeDto[];
  branches: BranchDto[];
  appointmentTypes: AppointmentTypeDto[];
  hasPermission: (formCode: string, action: string) => boolean;
  onSaveTime: (data: Partial<AvailableTimeDto>) => Promise<void>;
  onDeactivateTime: (id: number) => Promise<void>;
  onActivateTime: (id: number) => Promise<void>;
  onExportSuccess: (message: string) => void;
  onExportWarning: (message: string) => void;
}

export const TimesView: React.FC<TimesViewProps> = ({
  availableTimes,
  branches,
  appointmentTypes,
  hasPermission,
  onSaveTime,
  onDeactivateTime,
  onActivateTime,
  onExportSuccess,
  onExportWarning
}) => {
  const [currentView, setCurrentView] = useState<'active' | 'inactive'>('active');
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortColumn, setSortColumn] = useState<string>('');
  const [sortDirection, setSortDirection] = useState<SortDirection>('asc');
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<'create' | 'edit'>('create');
  const [selectedItem, setSelectedItem] = useState<AvailableTimeDto | null>(null);

  const itemsPerPage = 10;

  const filteredByStatus = availableTimes.filter(time =>
    currentView === 'active' ? time.isActive : !time.isActive
  );

  let processedData = filterData(
    filteredByStatus,
    searchTerm,
    ['time', 'branchName', 'appointmentTypeName']
  );
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

  const handleSaveTime = async (formData: { id?: number; time: string; branchId: string; appointmentTypeId: string | null }) => {
    const converted: Partial<AvailableTimeDto> = {
      time: formData.time,
      branchId: parseInt(formData.branchId),
      appointmentTypeId: formData.appointmentTypeId && formData.appointmentTypeId !== '' 
        ? parseInt(formData.appointmentTypeId) 
        : undefined
    };
    
    // If editing, include the id
    if (formData.id) {
      converted.id = formData.id;
    }
    
    await onSaveTime(converted);
  };

  const handleExport = () => {
    exportToCSV(
      processedData,
      'horas-disponibles',
      {
        time: 'Hora',
        branchName: 'Sede',
        appointmentTypeName: 'Tipo de Cita',
        isActive: 'Activo'
      },
      onExportSuccess,
      onExportWarning
    );
  };

  return (
    <div className="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden">
      <div className="p-6">
        <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4 mb-6">
          <ViewToggle currentView={currentView} onViewChange={handleViewChange} />
          {hasPermission('AVAILABLE_TIMES', 'create') && (
            <button
              onClick={() => {
                setModalMode('create');
                setSelectedItem(null);
                setModalOpen(true);
              }}
              className="px-4 py-2 bg-[#1797D5] text-white rounded-lg hover:bg-[#203461] transition-colors flex items-center"
            >
              <FiPlus className="mr-2" />
              Crear Hora Disponible
            </button>
          )}
        </div>

        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6">
          <SearchBar
            value={searchTerm}
            onChange={handleSearch}
            placeholder="Buscar por hora, sede o tipo de cita..."
          />
          <button
            onClick={handleExport}
            className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors flex items-center whitespace-nowrap"
          >
            <FiDownload className="mr-2" />
            Exportar CSV
          </button>
        </div>

        <div className="overflow-x-auto">
          <table className="min-w-full bg-white">
            <thead className="bg-gray-50">
              <tr>
                <SortableTableHeader
                  column="time"
                  label="Hora"
                  onClick={() => handleSort('time')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                <SortableTableHeader
                  column="branchName"
                  label="Sede"
                  onClick={() => handleSort('branchName')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                <SortableTableHeader
                  column="appointmentTypeName"
                  label="Tipo de Cita"
                  onClick={() => handleSort('appointmentTypeName')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                {currentView === 'inactive' && <SortableTableHeader column="isActive" label="Estado" />}
                <SortableTableHeader column="" label="Acciones" />
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {paginatedData.map((time) => (
                <tr key={time.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {formatTime(time.time)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{time.branchName}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {time.appointmentTypeName || 'Todos'}
                  </td>
                  {currentView === 'inactive' && (
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          time.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                        }`}
                      >
                        {time.isActive ? 'Activo' : 'Inactivo'}
                      </span>
                    </td>
                  )}
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                    {hasPermission('AVAILABLE_TIMES', 'update') ? (
                      <button
                        onClick={() => {
                          setModalMode('edit');
                          setSelectedItem(time);
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
                    {currentView === 'active' && hasPermission('AVAILABLE_TIMES', 'delete') ? (
                      <button
                        onClick={() => onDeactivateTime(time.id)}
                        className="text-red-600 hover:text-red-900 transition-colors"
                      >
                        Desactivar
                      </button>
                    ) : currentView === 'inactive' && hasPermission('AVAILABLE_TIMES', 'update') ? (
                      <button
                        onClick={() => onActivateTime(time.id)}
                        className="text-green-600 hover:text-green-900 transition-colors font-medium"
                      >
                        <FiCheckCircle className="inline mr-1" />
                        Activar
                      </button>
                    ) : null}
                  </td>
                </tr>
              ))}
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

      <AvailableTimeModal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        onSave={handleSaveTime}
        item={selectedItem ? { 
          id: selectedItem.id, 
          time: selectedItem.time, 
          branchId: String(selectedItem.branchId), 
          appointmentTypeId: selectedItem.appointmentTypeId ? String(selectedItem.appointmentTypeId) : null 
        } : undefined}
        mode={modalMode}
        branches={branches.map(b => ({ id: String(b.id), code: b.code, name: b.name, city: b.city }))}
        appointmentTypes={appointmentTypes.map(at => ({ id: String(at.id), name: at.name, icon: at.icon || '' }))}
      />
    </div>
  );
};
