/**
 * @file BranchesView.tsx
 * @description Branches management view with CRUD operations
 */

import React, { useState, useCallback } from 'react';
import { FiEdit, FiCheckCircle, FiPlus, FiDownload } from 'react-icons/fi';
import { BranchDto } from '@/services/api';
import { BranchModal } from '../components/modals';
import { SortableTableHeader, PaginationControls, SearchBar, ViewToggle } from '../components/shared';
import { filterData, sortData, getPaginatedData, exportToCSV } from '../utils/tableUtils';

type SortDirection = 'asc' | 'desc';

interface BranchesViewProps {
  branches: BranchDto[];
  hasPermission: (formCode: string, action: string) => boolean;
  onSaveBranch: (data: Partial<BranchDto>) => Promise<void>;
  onDeactivateBranch: (id: number) => Promise<void>;
  onActivateBranch: (id: number) => Promise<void>;
  onExportSuccess: (message: string) => void;
  onExportWarning: (message: string) => void;
}

export const BranchesView: React.FC<BranchesViewProps> = ({
  branches,
  hasPermission,
  onSaveBranch,
  onDeactivateBranch,
  onActivateBranch,
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
  const [selectedItem, setSelectedItem] = useState<BranchDto | null>(null);

  const itemsPerPage = 10;

  const filteredByStatus = branches.filter(branch =>
    currentView === 'active' ? branch.isActive : !branch.isActive
  );

  let processedData = filterData(filteredByStatus, searchTerm, ['code', 'name', 'city', 'address']);
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
      'sedes',
      {
        code: 'Código',
        name: 'Nombre',
        address: 'Dirección',
        city: 'Ciudad',
        phone: 'Teléfono',
        isMain: 'Principal',
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
          {hasPermission('BRANCHES', 'create') && (
            <button
              onClick={() => {
                setModalMode('create');
                setSelectedItem(null);
                setModalOpen(true);
              }}
              className="px-4 py-2 bg-[#1797D5] text-white rounded-lg hover:bg-[#203461] transition-colors flex items-center"
            >
              <FiPlus className="mr-2" />
              Crear Sede
            </button>
          )}
        </div>

        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6">
          <SearchBar
            value={searchTerm}
            onChange={handleSearch}
            placeholder="Buscar por código, nombre, ciudad o dirección..."
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
                  column="city"
                  label="Ciudad"
                  onClick={() => handleSort('city')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                <SortableTableHeader
                  column="isMain"
                  label="Principal"
                  onClick={() => handleSort('isMain')}
                  sortColumn={sortColumn}
                  sortDirection={sortDirection}
                />
                {currentView === 'inactive' && <SortableTableHeader column="isActive" label="Estado" />}
                <SortableTableHeader column="" label="Acciones" />
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {paginatedData.map((branch) => (
                <tr key={branch.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{branch.code}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{branch.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{branch.city}</td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${
                        branch.isMain ? 'bg-blue-100 text-blue-800' : 'bg-gray-100 text-gray-800'
                      }`}
                    >
                      {branch.isMain ? 'Sí' : 'No'}
                    </span>
                  </td>
                  {currentView === 'inactive' && (
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          branch.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                        }`}
                      >
                        {branch.isActive ? 'Activo' : 'Inactivo'}
                      </span>
                    </td>
                  )}
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                    {hasPermission('BRANCHES', 'update') ? (
                      <button
                        onClick={() => {
                          setModalMode('edit');
                          setSelectedItem(branch);
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
                    {currentView === 'active' && hasPermission('BRANCHES', 'delete') ? (
                      <button
                        onClick={() => onDeactivateBranch(branch.id)}
                        className="text-red-600 hover:text-red-900 transition-colors"
                      >
                        Desactivar
                      </button>
                    ) : currentView === 'inactive' && hasPermission('BRANCHES', 'update') ? (
                      <button
                        onClick={() => onActivateBranch(branch.id)}
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

      <BranchModal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        onSave={onSaveBranch}
        item={selectedItem ?? undefined}
        mode={modalMode}
      />
    </div>
  );
};
