/**
 * useDocumentManagement Hook
 * Custom hook para manejar la gestión completa de documentos de una cita
 */

import { useState, useEffect, useCallback } from 'react';
import { appointmentDocumentService } from '@/services/documents';
import type {
  AppointmentDocumentDto,
  AppointmentDocumentsStatsDto,
} from '@/services/documents';

interface UseDocumentManagementOptions {
  appointmentId: number;
  autoLoad?: boolean;
}

export const useDocumentManagement = ({
  appointmentId,
  autoLoad = true,
}: UseDocumentManagementOptions) => {
  const [documents, setDocuments] = useState<AppointmentDocumentDto[]>([]);
  const [stats, setStats] = useState<AppointmentDocumentsStatsDto | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load documents and stats
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
      const errorMessage =
        err instanceof Error ? err.message : 'Error al cargar documentos';
      setError(errorMessage);
      console.error('Error loading documents:', err);
    } finally {
      setIsLoading(false);
    }
  }, [appointmentId]);

  // Load on mount if autoLoad is true
  useEffect(() => {
    if (autoLoad) {
      loadDocuments();
    }
  }, [autoLoad, loadDocuments]);

  // Get single document
  const getDocument = useCallback(
    async (documentId: number) => {
      try {
        setError(null);
        const document = await appointmentDocumentService.getDocumentById(documentId);
        return document;
      } catch (err) {
        const errorMessage =
          err instanceof Error ? err.message : 'Error al cargar documento';
        setError(errorMessage);
        throw err;
      }
    },
    []
  );

  // Update document description
  const updateDescription = useCallback(
    async (documentId: number, description: string) => {
      try {
        setError(null);
        const updatedDocument =
          await appointmentDocumentService.updateDocumentDescription(
            documentId,
            description
          );

        // Update local state
        setDocuments((prev) =>
          prev.map((doc) => (doc.id === documentId ? updatedDocument : doc))
        );

        return updatedDocument;
      } catch (err) {
        const errorMessage =
          err instanceof Error ? err.message : 'Error al actualizar documento';
        setError(errorMessage);
        throw err;
      }
    },
    []
  );

  // Delete document
  const deleteDocument = useCallback(
    async (documentId: number) => {
      try {
        setError(null);
        await appointmentDocumentService.deleteDocument(documentId);

        // Update local state
        setDocuments((prev) => prev.filter((doc) => doc.id !== documentId));

        // Reload stats
        const statsData = await appointmentDocumentService.getDocumentStats(
          appointmentId
        );
        setStats(statsData);
      } catch (err) {
        const errorMessage =
          err instanceof Error ? err.message : 'Error al eliminar documento';
        setError(errorMessage);
        throw err;
      }
    },
    [appointmentId]
  );

  // Download document
  const downloadDocument = useCallback((document: AppointmentDocumentDto) => {
    try {
      appointmentDocumentService.downloadDocument(
        document.filePath,
        document.documentName
      );
    } catch (err) {
      const errorMessage =
        err instanceof Error ? err.message : 'Error al descargar documento';
      setError(errorMessage);
      throw err;
    }
  }, []);

  // Get document URL
  const getDocumentUrl = useCallback((filePath: string) => {
    return appointmentDocumentService.getDocumentUrl(filePath);
  }, []);

  // Refresh data
  const refresh = useCallback(() => {
    loadDocuments();
  }, [loadDocuments]);

  return {
    // Data
    documents,
    stats,
    isLoading,
    error,

    // Actions
    loadDocuments,
    getDocument,
    updateDescription,
    deleteDocument,
    downloadDocument,
    getDocumentUrl,
    refresh,
  };
};
