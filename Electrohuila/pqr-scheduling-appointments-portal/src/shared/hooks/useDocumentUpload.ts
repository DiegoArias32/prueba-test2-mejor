/**
 * useDocumentUpload Hook
 * Custom hook para manejar la lógica de carga de documentos
 */

import { useState, useCallback } from 'react';
import { appointmentDocumentService } from '@/services/documents';
import type { DocumentUploadProgress } from '@/services/documents';
import { validateFiles } from '@/shared/utils/fileHelpers';

interface UseDocumentUploadOptions {
  appointmentId: number;
  onSuccess?: () => void;
  onError?: (error: Error) => void;
}

export const useDocumentUpload = ({
  appointmentId,
  onSuccess,
  onError,
}: UseDocumentUploadOptions) => {
  const [isUploading, setIsUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState<DocumentUploadProgress[]>([]);
  const [errors, setErrors] = useState<string[]>([]);

  const uploadFiles = useCallback(
    async (files: File[]) => {
      // Validate files
      const validation = validateFiles(files);
      if (!validation.valid) {
        setErrors(validation.errors);
        return;
      }

      try {
        setIsUploading(true);
        setErrors([]);

        // Initialize progress tracking
        const progressArray: DocumentUploadProgress[] = files.map((file) => ({
          fileName: file.name,
          progress: 0,
          status: 'pending',
        }));

        setUploadProgress(progressArray);

        // Upload files sequentially with progress updates
        const results = [];
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
            // Simulate progress (in a real implementation, you'd use XMLHttpRequest or fetch with progress)
            const progressInterval = setInterval(() => {
              setUploadProgress((prev) =>
                prev.map((p) =>
                  p.fileName === file.name && p.progress < 90
                    ? { ...p, progress: p.progress + 10 }
                    : p
                )
              );
            }, 100);

            const result = await appointmentDocumentService.uploadDocument({
              file,
              appointmentId,
            });

            clearInterval(progressInterval);
            results.push(result);

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
            const errorMessage =
              error instanceof Error ? error.message : 'Error al subir';

            setUploadProgress((prev) =>
              prev.map((p) =>
                p.fileName === file.name
                  ? {
                      ...p,
                      status: 'error',
                      progress: 0,
                      error: errorMessage,
                    }
                  : p
              )
            );

            // Don't throw, continue with next file
            console.error(`Error uploading ${file.name}:`, error);
          }
        }

        // Check if all uploads were successful
        const hasErrors = uploadProgress.some((p) => p.status === 'error');
        if (!hasErrors && onSuccess) {
          onSuccess();
        }

        return results;
      } catch (error) {
        const err = error instanceof Error ? error : new Error('Error desconocido');
        setErrors([err.message]);
        if (onError) {
          onError(err);
        }
      } finally {
        setIsUploading(false);
      }
    },
    [appointmentId, onSuccess, onError]
  );

  const uploadSingleFile = useCallback(
    async (file: File, description?: string) => {
      try {
        setIsUploading(true);
        setErrors([]);

        setUploadProgress([
          {
            fileName: file.name,
            progress: 0,
            status: 'uploading',
          },
        ]);

        const result = await appointmentDocumentService.uploadDocument({
          file,
          appointmentId,
          description,
        });

        setUploadProgress([
          {
            fileName: file.name,
            progress: 100,
            status: 'success',
          },
        ]);

        if (onSuccess) {
          onSuccess();
        }

        return result;
      } catch (error) {
        const err = error instanceof Error ? error : new Error('Error desconocido');
        setErrors([err.message]);

        setUploadProgress([
          {
            fileName: file.name,
            progress: 0,
            status: 'error',
            error: err.message,
          },
        ]);

        if (onError) {
          onError(err);
        }
      } finally {
        setIsUploading(false);
      }
    },
    [appointmentId, onSuccess, onError]
  );

  const resetProgress = useCallback(() => {
    setUploadProgress([]);
    setErrors([]);
  }, []);

  return {
    uploadFiles,
    uploadSingleFile,
    isUploading,
    uploadProgress,
    errors,
    resetProgress,
  };
};
