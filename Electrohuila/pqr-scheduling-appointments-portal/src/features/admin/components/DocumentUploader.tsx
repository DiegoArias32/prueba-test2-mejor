'use client';

/**
 * DocumentUploader Component
 * Componente para subir documentos con drag & drop
 */

import React, { useState, useRef, useCallback } from 'react';
import { Upload, X, FileText, AlertCircle } from 'lucide-react';
import {
  validateFiles,
  formatFileSize,
  getDocumentIcon,
  getDocumentTypeFromFile,
  FILE_VALIDATION_CONFIG,
} from '@/shared/utils/fileHelpers';

interface FileWithPreview {
  file: File;
  preview?: string;
  description?: string;
  id: string;
}

interface DocumentUploaderProps {
  appointmentId: number;
  onUpload: (files: File[], descriptions: string[]) => Promise<void>;
  onCancel?: () => void;
  maxFiles?: number;
  maxFileSizeMB?: number;
}

export function DocumentUploader({
  appointmentId,
  onUpload,
  onCancel,
  maxFiles = FILE_VALIDATION_CONFIG.MAX_FILES_PER_UPLOAD,
  maxFileSizeMB = FILE_VALIDATION_CONFIG.MAX_FILE_SIZE_MB,
}: DocumentUploaderProps) {
  const [selectedFiles, setSelectedFiles] = useState<FileWithPreview[]>([]);
  const [isDragging, setIsDragging] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Manejar selección de archivos
  const handleFileSelect = useCallback(
    (files: FileList | null) => {
      if (!files || files.length === 0) return;

      const fileArray = Array.from(files);

      // Validar archivos
      const validation = validateFiles(fileArray);

      if (!validation.valid) {
        setErrors(validation.errors);
        return;
      }

      // Verificar límite de archivos
      if (selectedFiles.length + fileArray.length > maxFiles) {
        setErrors([`Solo puedes subir un máximo de ${maxFiles} archivos`]);
        return;
      }

      // Crear previews para imágenes
      const newFiles: FileWithPreview[] = fileArray.map((file) => ({
        file,
        description: '',
        id: Math.random().toString(36).substring(7),
      }));

      // Generar previews para imágenes
      newFiles.forEach((fileWithPreview) => {
        if (fileWithPreview.file.type.startsWith('image/')) {
          const reader = new FileReader();
          reader.onload = (e) => {
            setSelectedFiles((prev) =>
              prev.map((f) =>
                f.id === fileWithPreview.id
                  ? { ...f, preview: e.target?.result as string }
                  : f
              )
            );
          };
          reader.readAsDataURL(fileWithPreview.file);
        }
      });

      setSelectedFiles((prev) => [...prev, ...newFiles]);
      setErrors([]);
    },
    [selectedFiles, maxFiles]
  );

  // Manejar drag & drop
  const handleDragEnter = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(true);
  }, []);

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

      const files = e.dataTransfer.files;
      handleFileSelect(files);
    },
    [handleFileSelect]
  );

  // Manejar click en zona de drop
  const handleClick = () => {
    fileInputRef.current?.click();
  };

  // Manejar cambio en input de archivo
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    handleFileSelect(e.target.files);
  };

  // Remover archivo
  const removeFile = (id: string) => {
    setSelectedFiles((prev) => prev.filter((f) => f.id !== id));
    setErrors([]);
  };

  // Actualizar descripción
  const updateDescription = (id: string, description: string) => {
    setSelectedFiles((prev) =>
      prev.map((f) => (f.id === id ? { ...f, description } : f))
    );
  };

  // Subir archivos
  const handleUpload = async () => {
    if (selectedFiles.length === 0) return;

    try {
      setIsUploading(true);
      setUploadProgress(0);
      setErrors([]);

      const files = selectedFiles.map((f) => f.file);
      const descriptions = selectedFiles.map((f) => f.description || '');

      // Simular progreso
      const progressInterval = setInterval(() => {
        setUploadProgress((prev) => {
          if (prev >= 90) {
            clearInterval(progressInterval);
            return 90;
          }
          return prev + 10;
        });
      }, 200);

      await onUpload(files, descriptions);

      clearInterval(progressInterval);
      setUploadProgress(100);

      // Limpiar después de subir
      setTimeout(() => {
        setSelectedFiles([]);
        setUploadProgress(0);
        setIsUploading(false);
      }, 500);
    } catch (error) {
      setErrors([
        error instanceof Error ? error.message : 'Error al subir archivos',
      ]);
      setIsUploading(false);
      setUploadProgress(0);
    }
  };

  return (
    <div className="space-y-4">
      {/* Zona de Drag & Drop */}
      <div
        className={`
          relative border-2 border-dashed rounded-lg p-8 text-center transition-colors cursor-pointer
          ${
            isDragging
              ? 'border-blue-500 bg-blue-50'
              : 'border-gray-300 hover:border-gray-400'
          }
          ${isUploading ? 'opacity-50 pointer-events-none' : ''}
        `}
        onDragEnter={handleDragEnter}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        onClick={handleClick}
      >
        <input
          ref={fileInputRef}
          type="file"
          multiple
          accept={FILE_VALIDATION_CONFIG.ALLOWED_EXTENSIONS.join(',')}
          onChange={handleInputChange}
          className="hidden"
        />

        <Upload className="w-12 h-12 mx-auto mb-4 text-gray-400" />

        <p className="text-lg font-medium text-gray-700 mb-2">
          Arrastra archivos aquí o haz click para seleccionar
        </p>

        <p className="text-sm text-gray-500">
          Formatos aceptados: JPG, PNG, PDF, DOC, DOCX, XLS, XLSX
        </p>
        <p className="text-sm text-gray-500">
          Tamaño máximo: {maxFileSizeMB} MB por archivo | Máximo {maxFiles}{' '}
          archivos
        </p>
      </div>

      {/* Errores */}
      {errors.length > 0 && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4">
          <div className="flex items-start">
            <AlertCircle className="w-5 h-5 text-red-600 mt-0.5 mr-3 flex-shrink-0" />
            <div className="flex-1">
              <h4 className="text-sm font-medium text-red-800 mb-1">
                Errores de validación
              </h4>
              <ul className="text-sm text-red-700 list-disc list-inside space-y-1">
                {errors.map((error, index) => (
                  <li key={index}>{error}</li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      )}

      {/* Lista de archivos seleccionados */}
      {selectedFiles.length > 0 && (
        <div className="space-y-3">
          <h4 className="font-medium text-gray-900">
            Archivos seleccionados ({selectedFiles.length})
          </h4>

          <div className="space-y-2">
            {selectedFiles.map((fileWithPreview) => {
              const documentType = getDocumentTypeFromFile(
                fileWithPreview.file
              );
              const icon = getDocumentIcon(documentType);

              return (
                <div
                  key={fileWithPreview.id}
                  className="bg-white border border-gray-200 rounded-lg p-4"
                >
                  <div className="flex items-start gap-3">
                    {/* Preview o icono */}
                    <div className="flex-shrink-0 w-16 h-16 bg-gray-100 rounded flex items-center justify-center overflow-hidden">
                      {fileWithPreview.preview ? (
                        <img
                          src={fileWithPreview.preview}
                          alt={fileWithPreview.file.name}
                          className="w-full h-full object-cover"
                        />
                      ) : (
                        <span className="text-2xl">{icon}</span>
                      )}
                    </div>

                    {/* Información del archivo */}
                    <div className="flex-1 min-w-0">
                      <div className="flex items-start justify-between gap-2 mb-2">
                        <div className="flex-1 min-w-0">
                          <p className="font-medium text-gray-900 truncate">
                            {fileWithPreview.file.name}
                          </p>
                          <p className="text-sm text-gray-500">
                            {formatFileSize(fileWithPreview.file.size)}
                          </p>
                        </div>

                        <button
                          onClick={() => removeFile(fileWithPreview.id)}
                          className="text-gray-400 hover:text-red-600 transition-colors"
                          disabled={isUploading}
                        >
                          <X className="w-5 h-5" />
                        </button>
                      </div>

                      {/* Campo de descripción */}
                      <input
                        type="text"
                        placeholder="Descripción opcional"
                        value={fileWithPreview.description}
                        onChange={(e) =>
                          updateDescription(fileWithPreview.id, e.target.value)
                        }
                        disabled={isUploading}
                        className="w-full px-3 py-2 text-sm border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-50 disabled:text-gray-500"
                      />
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Barra de progreso */}
      {isUploading && (
        <div className="space-y-2">
          <div className="flex items-center justify-between text-sm">
            <span className="text-gray-700">Subiendo archivos...</span>
            <span className="font-medium text-blue-600">{uploadProgress}%</span>
          </div>
          <div className="w-full bg-gray-200 rounded-full h-2 overflow-hidden">
            <div
              className="bg-blue-600 h-full transition-all duration-300 ease-out"
              style={{ width: `${uploadProgress}%` }}
            />
          </div>
        </div>
      )}

      {/* Botones */}
      <div className="flex items-center justify-end gap-3 pt-4 border-t">
        {onCancel && (
          <button
            onClick={onCancel}
            disabled={isUploading}
            className="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Cancelar
          </button>
        )}

        <button
          onClick={handleUpload}
          disabled={selectedFiles.length === 0 || isUploading}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
        >
          <Upload className="w-4 h-4" />
          {isUploading ? 'Subiendo...' : `Subir ${selectedFiles.length} archivo(s)`}
        </button>
      </div>
    </div>
  );
}
