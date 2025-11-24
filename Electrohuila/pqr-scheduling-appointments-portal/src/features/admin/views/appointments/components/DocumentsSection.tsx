/**
 * Documents Section Component
 * Sección de documentos para agregar a la vista de detalles de citas
 */

'use client';

import React, { useState } from 'react';
import { FiFile, FiPlus, FiEye } from 'react-icons/fi';
import { useDocumentManagement } from '@/shared/hooks';
import {
  DocumentGallery,
  DocumentViewer,
  DocumentStats,
} from '@/shared/components/documents';
import type { AppointmentDocumentDto } from '@/services/documents';

interface DocumentsSectionProps {
  appointmentId: number;
  showUploadButton?: boolean;
  onUploadClick?: () => void;
  className?: string;
}

export const DocumentsSection: React.FC<DocumentsSectionProps> = ({
  appointmentId,
  showUploadButton = true,
  onUploadClick,
  className = '',
}) => {
  const [viewerDocument, setViewerDocument] = useState<AppointmentDocumentDto | null>(
    null
  );
  const [isViewerOpen, setIsViewerOpen] = useState(false);

  const {
    documents,
    stats,
    isLoading,
    error,
    deleteDocument,
    downloadDocument,
    updateDescription,
    refresh,
  } = useDocumentManagement({
    appointmentId,
    autoLoad: true,
  });

  const handleViewDocument = (document: AppointmentDocumentDto) => {
    setViewerDocument(document);
    setIsViewerOpen(true);
  };

  const handleCloseViewer = () => {
    setIsViewerOpen(false);
    setViewerDocument(null);
  };

  const handleNavigateDocument = (document: AppointmentDocumentDto) => {
    setViewerDocument(document);
  };

  const handleDeleteDocument = async (document: AppointmentDocumentDto) => {
    if (confirm(`¿Está seguro de que desea eliminar "${document.documentName}"?`)) {
      try {
        await deleteDocument(document.id);
      } catch (error) {
        console.error('Error deleting document:', error);
      }
    }
  };

  const handleEditDescription = async (document: AppointmentDocumentDto) => {
    const newDescription = prompt(
      'Ingrese la nueva descripción:',
      document.description || ''
    );

    if (newDescription !== null) {
      try {
        await updateDescription(document.id, newDescription);
      } catch (error) {
        console.error('Error updating description:', error);
      }
    }
  };

  if (error) {
    return (
      <div className={`bg-red-50 border border-red-200 rounded-lg p-4 ${className}`}>
        <div className="flex items-center">
          <FiFile className="w-5 h-5 text-red-500 mr-2" />
          <div>
            <h3 className="text-sm font-medium text-red-900">Error</h3>
            <p className="text-sm text-red-700 mt-1">{error}</p>
            <button
              onClick={refresh}
              className="mt-2 text-sm text-red-700 hover:text-red-900 font-medium"
            >
              Intentar de nuevo
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className={className}>
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center space-x-2">
          <FiFile className="w-5 h-5 text-gray-600" />
          <h3 className="text-lg font-semibold text-gray-900">
            Documentos Adjuntos
          </h3>
          {!isLoading && documents.length > 0 && (
            <span className="px-2 py-0.5 text-xs bg-blue-100 text-blue-700 rounded-full">
              {documents.length}
            </span>
          )}
        </div>

        <div className="flex items-center space-x-2">
          {documents.length > 0 && (
            <button
              onClick={() => {
                const win = window.open(
                  `/admin/appointments/${appointmentId}/documents`,
                  '_blank'
                );
                if (win) win.focus();
              }}
              className="flex items-center space-x-1 px-3 py-1.5 text-sm text-gray-700 hover:text-blue-600 hover:bg-blue-50 rounded-md transition-colors"
            >
              <FiEye className="w-4 h-4" />
              <span>Ver todo</span>
            </button>
          )}

          {showUploadButton && onUploadClick && (
            <button
              onClick={onUploadClick}
              className="flex items-center space-x-1 px-3 py-1.5 text-sm text-white bg-blue-600 hover:bg-blue-700 rounded-md transition-colors"
            >
              <FiPlus className="w-4 h-4" />
              <span>Subir</span>
            </button>
          )}
        </div>
      </div>

      {/* Stats */}
      {stats && !isLoading && documents.length > 0 && (
        <div className="mb-4">
          <DocumentStats stats={stats} />
        </div>
      )}

      {/* Gallery */}
      <DocumentGallery
        documents={documents}
        onView={handleViewDocument}
        onDownload={downloadDocument}
        onDelete={handleDeleteDocument}
        onEditDescription={handleEditDescription}
        isLoading={isLoading}
        showActions={true}
      />

      {/* Viewer */}
      <DocumentViewer
        document={viewerDocument}
        documents={documents}
        isOpen={isViewerOpen}
        onClose={handleCloseViewer}
        onDownload={downloadDocument}
        onNavigate={handleNavigateDocument}
      />
    </div>
  );
};
