/**
 * Appointment Documents View
 * Vista completa para gestión de documentos adjuntos de una cita
 */

'use client';

import React, { useCallback, useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import {
  FiArrowLeft,
  FiUpload,
  FiRefreshCw,
  FiAlertCircle,
  FiCheckCircle,
} from 'react-icons/fi';
import {
  DocumentGallery,
  DocumentUploader,
  DocumentViewer,
  DocumentStats,
} from '@/shared/components/documents';
import { ConfirmModal } from '@/shared/components';
import { appointmentDocumentService } from '@/services/documents';
import type {
  AppointmentDocumentDto,
  AppointmentDocumentsStatsDto,
  DocumentUploadProgress,
} from '@/services/documents';
import { Toast } from '@/shared/components/Toast';

interface AppointmentDocumentsViewProps {
  appointmentId: number;
  appointmentTitle?: string;
  showBackButton?: boolean;
  onBack?: () => void;
}

export const AppointmentDocumentsView: React.FC<AppointmentDocumentsViewProps> = ({
  appointmentId,
  appointmentTitle = 'Cita',
  showBackButton = true,
  onBack,
}) => {
  const router = useRouter();

  // State
  const [documents, setDocuments] = useState<AppointmentDocumentDto[]>([]);
  const [stats, setStats] = useState<AppointmentDocumentsStatsDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState<DocumentUploadProgress[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [showUploader, setShowUploader] = useState(false);

  // Viewer state
  const [viewerDocument, setViewerDocument] = useState<AppointmentDocumentDto | null>(
    null
  );
  const [isViewerOpen, setIsViewerOpen] = useState(false);

  // Delete confirmation
  const [deleteDocument, setDeleteDocument] = useState<AppointmentDocumentDto | null>(
    null
  );
  const [isDeleting, setIsDeleting] = useState(false);

  // Edit description
  const [editDocument, setEditDocument] = useState<AppointmentDocumentDto | null>(
    null
  );
  const [editDescription, setEditDescription] = useState('');
  const [isEditing, setIsEditing] = useState(false);

  // Toast
  const [toast, setToast] = useState<{
    message: string;
    type: 'success' | 'error' | 'info';
  } | null>(null);

  // Load documents
  const loadDocuments = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const [documentsData, statsData] = await Promise.all([
        appointmentDocumentService.getDocumentsByAppointment(appointmentId),
        appointmentDocumentService.getDocumentStats(appointmentId),
      ]);

      setDocuments(documentsData);
      setStats(statsData);
    } catch (err) {
      console.error('Error loading documents:', err);
      setError(
        err instanceof Error
          ? err.message
          : 'Error al cargar los documentos'
      );
    } finally {
      setIsLoading(false);
    }
  }, [appointmentId]);

  useEffect(() => {
    loadDocuments();
  }, [loadDocuments]);

  // Upload handlers
  const handleUploadStart = () => {
    setIsUploading(true);
    setUploadProgress([]);
  };

  const handleUploadProgress = (progress: DocumentUploadProgress[]) => {
    setUploadProgress(progress);
  };

  const handleUploadComplete = async () => {
    setIsUploading(false);
    setShowUploader(false);
    setToast({
      message: 'Documentos subidos exitosamente',
      type: 'success',
    });
    await loadDocuments();
  };

  // Upload files
  const uploadFiles = async (files: File[]) => {
    try {
      handleUploadStart();

      const progressArray: DocumentUploadProgress[] = files.map((file) => ({
        fileName: file.name,
        progress: 0,
        status: 'pending',
      }));

      setUploadProgress(progressArray);

      // Upload each file
      for (let i = 0; i < files.length; i++) {
        const file = files[i];

        // Update status to uploading
        setUploadProgress((prev) =>
          prev.map((p) =>
            p.fileName === file.name
              ? { ...p, status: 'uploading', progress: 0 }
              : p
          )
        );

        try {
          await appointmentDocumentService.uploadDocument({
            file,
            appointmentId,
          });

          // Update status to success
          setUploadProgress((prev) =>
            prev.map((p) =>
              p.fileName === file.name
                ? { ...p, status: 'success', progress: 100 }
                : p
            )
          );
        } catch (error) {
          // Update status to error
          setUploadProgress((prev) =>
            prev.map((p) =>
              p.fileName === file.name
                ? {
                    ...p,
                    status: 'error',
                    progress: 0,
                    error:
                      error instanceof Error
                        ? error.message
                        : 'Error al subir',
                  }
                : p
            )
          );
        }
      }

      // Check if all uploads were successful
      const hasErrors = uploadProgress.some((p) => p.status === 'error');
      if (!hasErrors) {
        handleUploadComplete();
      } else {
        setToast({
          message: 'Algunos archivos no se pudieron subir',
          type: 'error',
        });
        setIsUploading(false);
      }
    } catch (error) {
      console.error('Upload error:', error);
      setToast({
        message:
          error instanceof Error
            ? error.message
            : 'Error al subir los documentos',
        type: 'error',
      });
      setIsUploading(false);
    }
  };

  // View document
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

  // Download document
  const handleDownloadDocument = (document: AppointmentDocumentDto) => {
    try {
      appointmentDocumentService.downloadDocument(
        document.filePath,
        document.documentName
      );
      setToast({
        message: 'Descargando documento...',
        type: 'info',
      });
    } catch (error) {
      console.error('Download error:', error);
      setToast({
        message: 'Error al descargar el documento',
        type: 'error',
      });
    }
  };

  // Delete document
  const handleDeleteDocument = (document: AppointmentDocumentDto) => {
    setDeleteDocument(document);
  };

  const confirmDelete = async () => {
    if (!deleteDocument) return;

    try {
      setIsDeleting(true);
      await appointmentDocumentService.deleteDocument(deleteDocument.id);

      setToast({
        message: 'Documento eliminado exitosamente',
        type: 'success',
      });

      setDeleteDocument(null);
      await loadDocuments();
    } catch (error) {
      console.error('Delete error:', error);
      setToast({
        message:
          error instanceof Error
            ? error.message
            : 'Error al eliminar el documento',
        type: 'error',
      });
    } finally {
      setIsDeleting(false);
    }
  };

  // Edit description
  const handleEditDescription = (document: AppointmentDocumentDto) => {
    setEditDocument(document);
    setEditDescription(document.description || '');
  };

  const saveDescription = async () => {
    if (!editDocument) return;

    try {
      setIsEditing(true);
      await appointmentDocumentService.updateDocumentDescription(
        editDocument.id,
        editDescription
      );

      setToast({
        message: 'Descripción actualizada exitosamente',
        type: 'success',
      });

      setEditDocument(null);
      setEditDescription('');
      await loadDocuments();
    } catch (error) {
      console.error('Update error:', error);
      setToast({
        message:
          error instanceof Error
            ? error.message
            : 'Error al actualizar la descripción',
        type: 'error',
      });
    } finally {
      setIsEditing(false);
    }
  };

  const handleBack = () => {
    if (onBack) {
      onBack();
    } else {
      router.back();
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center justify-between mb-4">
            <div className="flex items-center space-x-4">
              {showBackButton && (
                <button
                  onClick={handleBack}
                  className="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="Volver"
                >
                  <FiArrowLeft className="w-5 h-5" />
                </button>
              )}
              <div>
                <h1 className="text-2xl font-bold text-gray-900">
                  Documentos Adjuntos
                </h1>
                <p className="text-sm text-gray-600 mt-1">{appointmentTitle}</p>
              </div>
            </div>

            <div className="flex items-center space-x-3">
              <button
                onClick={loadDocuments}
                disabled={isLoading}
                className="flex items-center space-x-2 px-4 py-2 text-gray-700 bg-white hover:bg-gray-50 border border-gray-300 rounded-lg transition-colors disabled:opacity-50"
              >
                <FiRefreshCw
                  className={`w-4 h-4 ${isLoading ? 'animate-spin' : ''}`}
                />
                <span>Actualizar</span>
              </button>

              <button
                onClick={() => setShowUploader(!showUploader)}
                disabled={isUploading}
                className="flex items-center space-x-2 px-4 py-2 text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors disabled:opacity-50"
              >
                <FiUpload className="w-4 h-4" />
                <span>{showUploader ? 'Ocultar' : 'Subir'} Documentos</span>
              </button>
            </div>
          </div>

          {/* Stats */}
          {stats && !isLoading && (
            <DocumentStats stats={stats} className="mb-6" />
          )}
        </div>

        {/* Error Message */}
        {error && (
          <div className="mb-6 bg-red-50 border border-red-200 rounded-lg p-4">
            <div className="flex items-start">
              <FiAlertCircle className="w-5 h-5 text-red-500 mt-0.5 mr-3 flex-shrink-0" />
              <div className="flex-1">
                <h3 className="text-sm font-medium text-red-900">Error</h3>
                <p className="text-sm text-red-700 mt-1">{error}</p>
                <button
                  onClick={loadDocuments}
                  className="mt-3 text-sm text-red-700 hover:text-red-900 font-medium"
                >
                  Intentar de nuevo
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Uploader */}
        {showUploader && (
          <div className="mb-6 bg-white rounded-lg shadow-sm p-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">
              Subir Nuevos Documentos
            </h2>
            <DocumentUploader
              appointmentId={appointmentId}
              onUploadStart={handleUploadStart}
              onUploadProgress={handleUploadProgress}
              onUploadComplete={handleUploadComplete}
              disabled={isUploading}
            />
          </div>
        )}

        {/* Documents Gallery */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            Documentos ({documents.length})
          </h2>

          <DocumentGallery
            documents={documents}
            onView={handleViewDocument}
            onDownload={handleDownloadDocument}
            onDelete={handleDeleteDocument}
            onEditDescription={handleEditDescription}
            isLoading={isLoading}
            showActions={true}
          />
        </div>
      </div>

      {/* Document Viewer */}
      <DocumentViewer
        document={viewerDocument}
        documents={documents}
        isOpen={isViewerOpen}
        onClose={handleCloseViewer}
        onDownload={handleDownloadDocument}
        onNavigate={handleNavigateDocument}
      />

      {/* Delete Confirmation Modal */}
      <ConfirmModal
        isOpen={!!deleteDocument}
        onClose={() => setDeleteDocument(null)}
        onConfirm={confirmDelete}
        title="Eliminar Documento"
        message={`¿Está seguro de que desea eliminar el documento "${deleteDocument?.documentName}"? Esta acción no se puede deshacer.`}
        confirmText="Eliminar"
        cancelText="Cancelar"
        isLoading={isDeleting}
        type="danger"
      />

      {/* Edit Description Modal */}
      {editDocument && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
            <div className="p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">
                Editar Descripción
              </h3>

              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Documento
                </label>
                <p className="text-sm text-gray-600">{editDocument.documentName}</p>
              </div>

              <div className="mb-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Descripción
                </label>
                <textarea
                  value={editDescription}
                  onChange={(e) => setEditDescription(e.target.value)}
                  rows={4}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Ingrese una descripción para el documento..."
                />
              </div>

              <div className="flex items-center space-x-3">
                <button
                  onClick={saveDescription}
                  disabled={isEditing}
                  className="flex-1 px-4 py-2 text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors disabled:opacity-50"
                >
                  {isEditing ? 'Guardando...' : 'Guardar'}
                </button>
                <button
                  onClick={() => {
                    setEditDocument(null);
                    setEditDescription('');
                  }}
                  disabled={isEditing}
                  className="flex-1 px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors disabled:opacity-50"
                >
                  Cancelar
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Toast Notifications */}
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </div>
  );
};
