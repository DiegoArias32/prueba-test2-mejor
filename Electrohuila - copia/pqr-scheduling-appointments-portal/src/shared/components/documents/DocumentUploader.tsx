/**
 * Document Uploader Component
 * Componente de subida de documentos con drag & drop y múltiples archivos
 */

'use client';

import React, { useCallback, useState, useRef } from 'react';
import { FiUpload, FiX, FiFile, FiCheck, FiAlertCircle } from 'react-icons/fi';
import {
  validateFiles,
  formatFileSize,
  FILE_VALIDATION_CONFIG,
  getDocumentTypeFromFile,
} from '@/shared/utils/fileHelpers';
import type { DocumentUploadProgress } from '@/services/documents';

interface DocumentUploaderProps {
  appointmentId: number;
  onUploadComplete?: () => void;
  onUploadProgress?: (progress: DocumentUploadProgress[]) => void;
  onUploadStart?: () => void;
  maxFiles?: number;
  disabled?: boolean;
  className?: string;
}

interface FileWithPreview extends File {
  preview?: string;
  id: string;
}

export const DocumentUploader: React.FC<DocumentUploaderProps> = ({
  appointmentId,
  onUploadComplete,
  onUploadProgress,
  onUploadStart,
  maxFiles = FILE_VALIDATION_CONFIG.MAX_FILES_PER_UPLOAD,
  disabled = false,
  className = '',
}) => {
  const [files, setFiles] = useState<FileWithPreview[]>([]);
  const [isDragging, setIsDragging] = useState(false);
  const [uploadProgress, setUploadProgress] = useState<DocumentUploadProgress[]>([]);
  const [isUploading, setIsUploading] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleDragEnter = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (!disabled) {
      setIsDragging(true);
    }
  }, [disabled]);

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  }, []);

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
  }, []);

  const handleDrop = useCallback(
    (e: React.DragEvent) => {
      e.preventDefault();
      e.stopPropagation();
      setIsDragging(false);

      if (disabled) return;

      const droppedFiles = Array.from(e.dataTransfer.files);
      handleFiles(droppedFiles);
    },
    [disabled]
  );

  const handleFileInputChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const selectedFiles = e.target.files ? Array.from(e.target.files) : [];
      handleFiles(selectedFiles);
      // Reset input
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
    },
    []
  );

  const handleFiles = useCallback(
    (newFiles: File[]) => {
      const totalFiles = files.length + newFiles.length;

      if (totalFiles > maxFiles) {
        setErrors([`Solo puedes subir un máximo de ${maxFiles} archivos`]);
        return;
      }

      const validation = validateFiles(newFiles);
      if (!validation.valid) {
        setErrors(validation.errors);
        return;
      }

      setErrors([]);

      const filesWithId: FileWithPreview[] = newFiles.map((file) => {
        const fileWithId = file as FileWithPreview;
        fileWithId.id = `${Date.now()}-${Math.random()}`;

        // Generate preview for images
        if (file.type.startsWith('image/')) {
          const reader = new FileReader();
          reader.onloadend = () => {
            fileWithId.preview = reader.result as string;
            setFiles((prev) => [...prev]);
          };
          reader.readAsDataURL(file);
        }

        return fileWithId;
      });

      setFiles((prev) => [...prev, ...filesWithId]);
    },
    [files, maxFiles]
  );

  const removeFile = useCallback((fileId: string) => {
    setFiles((prev) => prev.filter((f) => f.id !== fileId));
    setErrors([]);
  }, []);

  const handleBrowseClick = () => {
    fileInputRef.current?.click();
  };

  const getProgressForFile = (fileName: string): DocumentUploadProgress | undefined => {
    return uploadProgress.find((p) => p.fileName === fileName);
  };

  const getFileStatusIcon = (fileName: string) => {
    const progress = getProgressForFile(fileName);
    if (!progress) return null;

    switch (progress.status) {
      case 'success':
        return <FiCheck className="w-5 h-5 text-green-500" />;
      case 'error':
        return <FiAlertCircle className="w-5 h-5 text-red-500" />;
      case 'uploading':
        return (
          <div className="w-5 h-5 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
        );
      default:
        return null;
    }
  };

  const clearAll = () => {
    setFiles([]);
    setErrors([]);
    setUploadProgress([]);
  };

  return (
    <div className={`space-y-4 ${className}`}>
      {/* Drop Zone */}
      <div
        onDragEnter={handleDragEnter}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        className={`
          relative border-2 border-dashed rounded-lg p-8 text-center transition-all duration-200
          ${isDragging ? 'border-blue-500 bg-blue-50' : 'border-gray-300 bg-white'}
          ${disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer hover:border-blue-400'}
        `}
        onClick={!disabled ? handleBrowseClick : undefined}
      >
        <input
          ref={fileInputRef}
          type="file"
          multiple
          accept={FILE_VALIDATION_CONFIG.ALLOWED_EXTENSIONS.join(',')}
          onChange={handleFileInputChange}
          className="hidden"
          disabled={disabled}
        />

        <FiUpload className={`w-12 h-12 mx-auto mb-4 ${isDragging ? 'text-blue-500' : 'text-gray-400'}`} />

        <h3 className="text-lg font-medium text-gray-900 mb-2">
          {isDragging ? 'Suelta los archivos aquí' : 'Arrastra archivos aquí'}
        </h3>

        <p className="text-sm text-gray-500 mb-4">
          o{' '}
          <span className="text-blue-600 hover:text-blue-700 font-medium">
            busca en tu computadora
          </span>
        </p>

        <div className="text-xs text-gray-400 space-y-1">
          <p>Formatos permitidos: JPG, PNG, GIF, PDF, DOC, DOCX, XLS, XLSX</p>
          <p>Tamaño máximo: {FILE_VALIDATION_CONFIG.MAX_FILE_SIZE_MB} MB por archivo</p>
          <p>Máximo {maxFiles} archivos por carga</p>
        </div>
      </div>

      {/* Errors */}
      {errors.length > 0 && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4">
          <div className="flex items-start">
            <FiAlertCircle className="w-5 h-5 text-red-500 mt-0.5 mr-2 flex-shrink-0" />
            <div className="flex-1">
              <h4 className="text-sm font-medium text-red-900 mb-1">
                Error de validación
              </h4>
              <ul className="text-sm text-red-700 space-y-1">
                {errors.map((error, index) => (
                  <li key={index}>{error}</li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      )}

      {/* Files List */}
      {files.length > 0 && (
        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <h4 className="text-sm font-medium text-gray-900">
              Archivos seleccionados ({files.length})
            </h4>
            {!isUploading && (
              <button
                onClick={clearAll}
                className="text-xs text-gray-500 hover:text-gray-700"
              >
                Limpiar todo
              </button>
            )}
          </div>

          <div className="space-y-2">
            {files.map((file) => {
              const progress = getProgressForFile(file.name);
              const docType = getDocumentTypeFromFile(file);

              return (
                <div
                  key={file.id}
                  className="flex items-center space-x-3 p-3 bg-gray-50 rounded-lg border border-gray-200"
                >
                  {/* Preview or Icon */}
                  <div className="flex-shrink-0">
                    {file.preview ? (
                      <img
                        src={file.preview}
                        alt={file.name}
                        className="w-12 h-12 object-cover rounded"
                      />
                    ) : (
                      <div className="w-12 h-12 bg-gray-200 rounded flex items-center justify-center">
                        <FiFile className="w-6 h-6 text-gray-400" />
                      </div>
                    )}
                  </div>

                  {/* File Info */}
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center justify-between mb-1">
                      <p className="text-sm font-medium text-gray-900 truncate">
                        {file.name}
                      </p>
                      <span
                        className={`
                          px-2 py-0.5 text-xs rounded-full ml-2 flex-shrink-0
                          ${
                            docType === 'Image'
                              ? 'bg-purple-100 text-purple-700'
                              : docType === 'PDF'
                              ? 'bg-red-100 text-red-700'
                              : docType === 'Word'
                              ? 'bg-blue-100 text-blue-700'
                              : docType === 'Excel'
                              ? 'bg-green-100 text-green-700'
                              : 'bg-gray-100 text-gray-700'
                          }
                        `}
                      >
                        {docType}
                      </span>
                    </div>
                    <p className="text-xs text-gray-500">{formatFileSize(file.size)}</p>

                    {/* Progress Bar */}
                    {progress && progress.status === 'uploading' && (
                      <div className="mt-2">
                        <div className="w-full bg-gray-200 rounded-full h-1.5">
                          <div
                            className="bg-blue-600 h-1.5 rounded-full transition-all duration-300"
                            style={{ width: `${progress.progress}%` }}
                          />
                        </div>
                        <p className="text-xs text-gray-500 mt-1">
                          {progress.progress}%
                        </p>
                      </div>
                    )}

                    {/* Error Message */}
                    {progress && progress.status === 'error' && (
                      <p className="text-xs text-red-600 mt-1">{progress.error}</p>
                    )}
                  </div>

                  {/* Status Icon or Remove Button */}
                  <div className="flex-shrink-0">
                    {getFileStatusIcon(file.name) || (
                      <button
                        onClick={() => removeFile(file.id)}
                        disabled={isUploading}
                        className="p-1 text-gray-400 hover:text-red-600 disabled:opacity-50"
                        title="Eliminar"
                      >
                        <FiX className="w-5 h-5" />
                      </button>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
};
