/**
 * NotificationTemplatesView Component
 * Main view for managing notification templates
 */

'use client';

import React, { useState, useEffect, useMemo } from 'react';
import {
  FiPlus,
  FiEdit2,
  FiTrash2,
  FiSearch,
  FiFilter,
  FiDownload,
  FiEye,
  FiMail,
  FiMessageSquare,
  FiBell,
  FiToggleLeft,
  FiToggleRight,
  FiAlertCircle,
  FiX
} from 'react-icons/fi';
import { NotificationTemplateModal } from '../../components/NotificationTemplateModal';
import { TemplatePreview } from '../../components/TemplatePreview';
import type { NotificationTemplateDto, CreateNotificationTemplateDto } from '@/services/notifications/notification-template.types';
import { NotificationTemplateService } from '@/services/notifications/notification-template.service';
import { TEMPLATE_TYPE_OPTIONS } from '@/services/notifications/notification-template.types';

interface NotificationTemplatesViewProps {
  hasPermission: (formCode: string, action: string) => boolean;
  onExportSuccess?: (message: string) => void;
  onExportWarning?: (message: string) => void;
}

const templateService = new NotificationTemplateService();

export const NotificationTemplatesView: React.FC<NotificationTemplatesViewProps> = ({
  hasPermission,
  onExportSuccess,
  onExportWarning
}) => {
  // Data state
  const [templates, setTemplates] = useState<NotificationTemplateDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // UI state
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedType, setSelectedType] = useState<'all' | 'Email' | 'SMS' | 'Push'>('all');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedTemplate, setSelectedTemplate] = useState<NotificationTemplateDto | null>(null);
  const [modalMode, setModalMode] = useState<'create' | 'edit'>('create');
  const [previewTemplate, setPreviewTemplate] = useState<NotificationTemplateDto | null>(null);

  // Permissions
  const canCreate = hasPermission('notification-templates', 'create');
  const canUpdate = hasPermission('notification-templates', 'update');
  const canDelete = hasPermission('notification-templates', 'delete');

  // Load templates
  const loadTemplates = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await templateService.getNotificationTemplates();
      setTemplates(data);
    } catch (err) {
      console.error('Error loading templates:', err);
      setError('Error al cargar las plantillas. Por favor intenta de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadTemplates();
  }, []);

  // Filtered templates
  const filteredTemplates = useMemo(() => {
    return templates.filter(template => {
      const matchesSearch =
        template.templateName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        template.templateCode.toLowerCase().includes(searchTerm.toLowerCase());

      const matchesType = selectedType === 'all' || template.templateType === selectedType;

      return matchesSearch && matchesType;
    });
  }, [templates, searchTerm, selectedType]);

  // Stats
  const stats = useMemo(() => {
    return {
      total: templates.length,
      active: templates.filter(t => t.isActive).length,
      email: templates.filter(t => t.templateType?.toLowerCase() === 'email').length,
      sms: templates.filter(t => t.templateType?.toLowerCase() === 'sms').length,
      push: templates.filter(t => t.templateType?.toLowerCase() === 'push').length
    };
  }, [templates]);

  // Handlers
  const handleCreate = () => {
    setSelectedTemplate(null);
    setModalMode('create');
    setIsModalOpen(true);
  };

  const handleEdit = (template: NotificationTemplateDto) => {
    setSelectedTemplate(template);
    setModalMode('edit');
    setIsModalOpen(true);
  };

  const handleSave = async (data: Partial<NotificationTemplateDto> | CreateNotificationTemplateDto) => {
    try {
      if (modalMode === 'edit' && 'id' in data) {
        await templateService.updateNotificationTemplate(data as NotificationTemplateDto);
      } else {
        await templateService.createNotificationTemplate(data as CreateNotificationTemplateDto);
      }
      await loadTemplates();
      setIsModalOpen(false);
    } catch (err) {
      console.error('Error saving template:', err);
      throw err;
    }
  };

  const handleDelete = async (template: NotificationTemplateDto) => {
    if (!confirm(`¿Estás seguro de eliminar la plantilla "${template.templateName}"?`)) {
      return;
    }

    try {
      await templateService.deleteNotificationTemplate(template.id);
      await loadTemplates();
    } catch (err) {
      console.error('Error deleting template:', err);
      setError('Error al eliminar la plantilla');
    }
  };

  const handleToggleActive = async (template: NotificationTemplateDto) => {
    try {
      if (template.isActive) {
        await templateService.deleteNotificationTemplate(template.id);
      } else {
        await templateService.activateNotificationTemplate(template.id);
      }
      await loadTemplates();
    } catch (err) {
      console.error('Error toggling template:', err);
      setError('Error al cambiar el estado de la plantilla');
    }
  };

  const handleExport = () => {
    if (filteredTemplates.length === 0) {
      onExportWarning?.('No hay plantillas para exportar');
      return;
    }

    const dataStr = JSON.stringify(filteredTemplates, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });
    const url = URL.createObjectURL(dataBlob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `notification-templates-${new Date().toISOString().split('T')[0]}.json`;
    link.click();
    URL.revokeObjectURL(url);

    onExportSuccess?.('Plantillas exportadas exitosamente');
  };

  const getTypeIcon = (type: 'Email' | 'SMS' | 'Push') => {
    switch (type) {
      case 'Email':
        return <FiMail className="w-4 h-4" />;
      case 'SMS':
        return <FiMessageSquare className="w-4 h-4" />;
      case 'Push':
        return <FiBell className="w-4 h-4" />;
    }
  };

  const getTypeBadgeColor = (type: 'Email' | 'SMS' | 'Push') => {
    switch (type) {
      case 'Email':
        return 'bg-blue-100 text-blue-700 border-blue-300';
      case 'SMS':
        return 'bg-green-100 text-green-700 border-green-300';
      case 'Push':
        return 'bg-purple-100 text-purple-700 border-purple-300';
    }
  };

  return (
    <div className="space-y-6">
      {/* Stats Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4">
        <div className="bg-white rounded-lg shadow-sm p-4 border border-gray-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Total</p>
              <p className="text-2xl font-bold text-gray-900">{stats.total}</p>
            </div>
            <div className="p-3 bg-gray-100 rounded-lg">
              <FiBell className="w-6 h-6 text-gray-600" />
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow-sm p-4 border border-gray-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Activas</p>
              <p className="text-2xl font-bold text-green-600">{stats.active}</p>
            </div>
            <div className="p-3 bg-green-100 rounded-lg">
              <FiToggleRight className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow-sm p-4 border border-blue-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-blue-600">Email</p>
              <p className="text-2xl font-bold text-blue-700">{stats.email}</p>
            </div>
            <div className="p-3 bg-blue-100 rounded-lg">
              <FiMail className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow-sm p-4 border border-green-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-green-600">SMS</p>
              <p className="text-2xl font-bold text-green-700">{stats.sms}</p>
            </div>
            <div className="p-3 bg-green-100 rounded-lg">
              <FiMessageSquare className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow-sm p-4 border border-purple-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-purple-600">Push</p>
              <p className="text-2xl font-bold text-purple-700">{stats.push}</p>
            </div>
            <div className="p-3 bg-purple-100 rounded-lg">
              <FiBell className="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      {/* Filters and Actions */}
      <div className="bg-white rounded-lg shadow-sm p-4 border border-gray-200">
        <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between space-y-4 lg:space-y-0">
          {/* Search */}
          <div className="flex-1 max-w-md">
            <div className="relative">
              <FiSearch className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
              <input
                type="text"
                placeholder="Buscar por nombre o código..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* Type Filter */}
          <div className="flex items-center space-x-2">
            <FiFilter className="w-5 h-5 text-gray-400" />
            <select
              value={selectedType}
              onChange={(e) => setSelectedType(e.target.value as typeof selectedType)}
              className="px-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="all">Todos los tipos</option>
              {TEMPLATE_TYPE_OPTIONS.map(option => (
                <option key={option.value} value={option.value}>
                  {option.icon} {option.label}
                </option>
              ))}
            </select>
          </div>

          {/* Actions */}
          <div className="flex items-center space-x-3">
            <button
              onClick={handleExport}
              className="px-4 py-2.5 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 transition-colors flex items-center space-x-2"
            >
              <FiDownload className="w-4 h-4" />
              <span>Exportar</span>
            </button>

            {canCreate && (
              <button
                onClick={handleCreate}
                className="px-4 py-2.5 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg text-sm font-medium hover:from-blue-700 hover:to-purple-700 transition-all flex items-center space-x-2"
              >
                <FiPlus className="w-4 h-4" />
                <span>Nueva Plantilla</span>
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Error Message */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-center space-x-2 text-red-800">
          <FiAlertCircle className="w-5 h-5 flex-shrink-0" />
          <span>{error}</span>
        </div>
      )}

      {/* Templates Table */}
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          </div>
        ) : filteredTemplates.length === 0 ? (
          <div className="text-center py-12 text-gray-500">
            <FiBell className="w-12 h-12 mx-auto mb-4 text-gray-400" />
            <p className="text-lg font-medium">No se encontraron plantillas</p>
            <p className="text-sm mt-1">
              {searchTerm || selectedType !== 'all'
                ? 'Intenta ajustar los filtros de búsqueda'
                : 'Crea tu primera plantilla de notificación'
              }
            </p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Código
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Nombre
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Tipo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Asunto
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Estado
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Acciones
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {filteredTemplates.map((template) => (
                  <tr key={template.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <code className="text-sm font-mono font-semibold text-gray-900">
                        {template.templateCode}
                      </code>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm font-medium text-gray-900">
                        {template.templateName}
                      </div>
                      <div className="text-xs text-gray-500 mt-0.5">
                        Creado: {new Date(template.createdAt).toLocaleDateString()}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex items-center space-x-1.5 px-2.5 py-1 rounded-full text-xs font-semibold border ${getTypeBadgeColor(template.templateType)}`}>
                        {getTypeIcon(template.templateType)}
                        <span>{template.templateType}</span>
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm text-gray-600 max-w-xs truncate">
                        {template.templateType === 'Email'
                          ? template.subject || '(Sin asunto)'
                          : '—'
                        }
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {template.isActive ? (
                        <span className="inline-flex items-center space-x-1 px-2.5 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-700 border border-green-300">
                          <FiToggleRight className="w-3 h-3" />
                          <span>Activa</span>
                        </span>
                      ) : (
                        <span className="inline-flex items-center space-x-1 px-2.5 py-1 rounded-full text-xs font-semibold bg-gray-100 text-gray-700 border border-gray-300">
                          <FiToggleLeft className="w-3 h-3" />
                          <span>Inactiva</span>
                        </span>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex items-center justify-end space-x-2">
                        <button
                          onClick={() => setPreviewTemplate(template)}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                          title="Vista previa"
                        >
                          <FiEye className="w-4 h-4" />
                        </button>

                        {canUpdate && (
                          <button
                            onClick={() => handleEdit(template)}
                            className="p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
                            title="Editar"
                          >
                            <FiEdit2 className="w-4 h-4" />
                          </button>
                        )}

                        {canUpdate && (
                          <button
                            onClick={() => handleToggleActive(template)}
                            className={`p-2 rounded-lg transition-colors ${
                              template.isActive
                                ? 'text-orange-600 hover:bg-orange-50'
                                : 'text-green-600 hover:bg-green-50'
                            }`}
                            title={template.isActive ? 'Desactivar' : 'Activar'}
                          >
                            {template.isActive ? (
                              <FiToggleLeft className="w-4 h-4" />
                            ) : (
                              <FiToggleRight className="w-4 h-4" />
                            )}
                          </button>
                        )}

                        {canDelete && (
                          <button
                            onClick={() => handleDelete(template)}
                            className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                            title="Eliminar"
                          >
                            <FiTrash2 className="w-4 h-4" />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Template Modal */}
      <NotificationTemplateModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSave={handleSave}
        template={selectedTemplate}
        mode={modalMode}
      />

      {/* Preview Modal */}
      {previewTemplate && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-2xl w-full max-w-3xl max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 p-6 flex items-center justify-between">
              <div>
                <h3 className="text-xl font-bold text-gray-900">Vista Previa</h3>
                <p className="text-sm text-gray-600 mt-1">{previewTemplate.templateName}</p>
              </div>
              <button
                onClick={() => setPreviewTemplate(null)}
                className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <FiX className="w-6 h-6 text-gray-500" />
              </button>
            </div>
            <div className="p-6">
              <TemplatePreview
                templateType={previewTemplate.templateType}
                subject={previewTemplate.subject}
                bodyTemplate={previewTemplate.bodyTemplate}
                templateName={previewTemplate.templateName}
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
