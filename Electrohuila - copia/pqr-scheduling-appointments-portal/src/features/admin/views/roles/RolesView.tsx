/**
 * @file RolesView.tsx
 * @description Roles management view with CRUD operations
 */

import React, { useState, useCallback } from 'react';
import { FiEdit, FiCheckCircle, FiPlus, FiDownload } from 'react-icons/fi';
import { RolDto } from '@/services/api';
import { RoleModal } from '../components/modals';
import { SortableTableHeader, PaginationControls, SearchBar, ViewToggle } from '../components/shared';
import { filterData, sortData, getPaginatedData, exportToCSV } from '../utils/tableUtils';

type SortDirection = 'asc' | 'desc';

interface RolesViewProps {
  roles: RolDto[];
  hasPermission: (formCode: string, action: string) => boolean;
  onSaveRole: (data: Partial<RolDto>) => Promise<void>;
  onDeactivateRole: (id: number) => Promise<void>;
  onActivateRole: (id: number) => Promise<void>;
  onExportSuccess: (message: string) => void;
  onExportWarning: (message: string) => void;
}

export const RolesView: React.FC<RolesViewProps> = ({
  roles,
  hasPermission,
  onSaveRole,
  onDeactivateRole,
  onActivateRole,
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
  const [selectedItem, setSelectedItem] = useState<RolDto | null>(null);

  const itemsPerPage = 10;

  // Filter by active/inactive status
  const filteredByStatus = roles.filter(role =>
    currentView === 'active' ? role.isActive : !role.isActive
  );

  // Apply search and sort
  let processedData = filterData(
    filteredByStatus,
    searchTerm,
    ['code', 'name', 'description']
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

  const handleExport = () => {
    exportToCSV(
      processedData,
      'roles',
      {
        code: 'Código',
        name: 'Nombre',
        description: 'Descripción',
        createdAt: 'Fecha de Creación',
        isActive: 'Activo'
      },
      onExportSuccess,
      onExportWarning
    );
  };

  return (
    <div className="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden">
      <div className="p-6">
        {/* Header with Toggle and Create Button */}
        <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4 mb-6">
          <ViewToggle currentView={currentView} onViewChange={handleViewChange} />

          {hasPermission('ROLES', 'create') && (
            <button
              onClick={() => {
                setModalMode('create');
                setSelectedItem(null);
                setModalOpen(true);
              }}
              className="px-4 py-2 bg-[#1797D5] text-white rounded-lg hover:bg-[#203461] transition-colors flex items-center"
            >
              <FiPlus className="mr-2" />
              Crear Rol
            </button>
          )}
        </div>

        {/* Search and Export Bar */}
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6">
          <SearchBar
            value={searchTerm}
            onChange={handleSearch}
            placeholder="Buscar por código, nombre o descripción..."
          />
          <button
            onClick={handleExport}
            className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors flex items-center whitespace-nowrap"
          >
            <FiDownload className="mr-2" />
            Exportar CSV
          </button>
        </div>

        {/* Table */}
        <div className="overflow-x-auto">
          <table className="min-w-full bg-white">
            <thead className="bg-gray-50">
              <tr>
                <SortableTableHeader
                  column="code"
                  label="Código"
                  onClick={() => handleSort('code')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                <SortableTableHeader
                  column="name"
                  label="Nombre"
                  onClick={() => handleSort('name')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                <SortableTableHeader
                  column="createdAt"
                  label="Fecha de Creación"
                  onClick={() => handleSort('createdAt')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                {currentView === 'inactive' && (
                  <SortableTableHeader column="isActive" label="Estado" />
                )}
                <SortableTableHeader column="" label="Acciones" />
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {paginatedData.map((role) => (
                <tr key={role.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                      {role.code}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {role.name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {new Date(role.createdAt).toLocaleDateString('en-US', {
                      year: 'numeric',
                      month: 'short',
                      day: 'numeric'
                    })}
                  </td>
                  {currentView === 'inactive' && (
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                        Inactivo
                      </span>
                    </td>
                  )}
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-3">
                    {hasPermission('ROLES', 'update') ? (
                      <button
                        onClick={() => {
                          setModalMode('edit');
                          setSelectedItem(role);
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
                    {currentView === 'active' && hasPermission('ROLES', 'delete') ? (
                      <button
                        onClick={() => onDeactivateRole(role.id)}
                        className="text-red-600 hover:text-red-900 transition-colors"
                      >
                        Desactivar
                      </button>
                    ) : currentView === 'inactive' && hasPermission('ROLES', 'update') ? (
                      <button
                        onClick={() => onActivateRole(role.id)}
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

      {/* Modal */}
      <RoleModal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        onSave={onSaveRole}
        item={selectedItem ?? undefined}
        mode={modalMode}
      />
    </div>
  );
};
