/**
 * Document Gallery Component
 * Galería visual de documentos adjuntos con preview y acciones
 */

'use client';

import React, { useState } from 'react';
import { FiDownload, FiTrash2, FiEdit2, FiFileText, FiImage, FiFile } from 'react-icons/fi';
import type { AppointmentDocumentDto } from '@/services/documents';
import { formatFileSize, getDocumentTypeFromExtension, getFileExtension } from '@/shared/utils/fileHelpers';

interface DocumentGalleryProps {
  documents: AppointmentDocumentDto[];
  onView: (document: AppointmentDocumentDto) => void;
  onDownload: (document: AppointmentDocumentDto) => void;
  onDelete: (document: AppointmentDocumentDto) => void;
  onEditDescription?: (document: AppointmentDocumentDto) => void;
  isLoading?: boolean;
  showActions?: boolean;
  className?: string;
}

export const DocumentGallery: React.FC<DocumentGalleryProps> = ({
  documents,
  onView,
  onDownload,
  onDelete,
  onEditDescription,
  isLoading = false,
  showActions = true,
  className = '',
}) => {
  const [selectedDocument, setSelectedDocument] = useState<number | null>(null);

  const getDocumentIcon = (docType: string) => {
    switch (docType.toLowerCase()) {
      case 'image':
        return <FiImage className="w-12 h-12 text-purple-500" />;
      case 'pdf':
        return <FiFileText className="w-12 h-12 text-red-500" />;
      case 'word':
        return <FiFileText className="w-12 h-12 text-blue-500" />;
      case 'excel':
        return <FiFileText className="w-12 h-12 text-green-500" />;
      default:
        return <FiFile className="w-12 h-12 text-gray-500" />;
    }
  };

  const handleDocumentClick = (document: AppointmentDocumentDto) => {
    onView(document);
  };

  const handleAction = (
    e: React.MouseEvent,
    action: () => void
  ) => {
    e.stopPropagation();
    action();
  };

  if (isLoading) {
    return (
      <div className={`grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 ${className}`}>
        {[...Array(3)].map((_, index) => (
          <div
            key={index}
            className="border border-gray-200 rounded-lg p-4 animate-pulse"
          >
            <div className="flex items-center space-x-3 mb-3">
              <div className="w-12 h-12 bg-gray-200 rounded"></div>
              <div className="flex-1">
                <div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
                <div className="h-3 bg-gray-200 rounded w-1/2"></div>
              </div>
            </div>
            <div className="h-3 bg-gray-200 rounded w-full mb-2"></div>
            <div className="h-8 bg-gray-200 rounded w-full"></div>
          </div>
        ))}
      </div>
    );
  }

  if (documents.length === 0) {
    return (
      <div className={`text-center py-12 ${className}`}>
        <FiFile className="w-16 h-16 mx-auto text-gray-300 mb-4" />
        <p className="text-gray-500">No hay documentos adjuntos</p>
        <p className="text-sm text-gray-400 mt-2">
          Los documentos que subas aparecerán aquí
        </p>
      </div>
    );
  }

  return (
    <div className={`grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 ${className}`}>
      {documents.map((document) => {
        const extension = getFileExtension(document.documentName);
        const docType = getDocumentTypeFromExtension(extension);
        const isImage = docType === 'Image';
        const isSelected = selectedDocument === document.id;

        return (
          <div
            key={document.id}
            className={`
              border rounded-lg overflow-hidden transition-all duration-200 cursor-pointer
              ${isSelected ? 'ring-2 ring-blue-500 border-blue-500' : 'border-gray-200 hover:border-blue-300'}
              hover:shadow-md
            `}
            onClick={() => handleDocumentClick(document)}
            onMouseEnter={() => setSelectedDocument(document.id)}
            onMouseLeave={() => setSelectedDocument(null)}
          >
            {/* Preview Section */}
            <div className="bg-gray-50 p-4 flex items-center justify-center h-40">
              {isImage ? (
                <img
                  src={document.filePath}
                  alt={document.documentName}
                  className="max-w-full max-h-full object-contain"
                  onError={(e) => {
                    const target = e.target as HTMLImageElement;
                    target.style.display = 'none';
                    target.nextElementSibling?.classList.remove('hidden');
                  }}
                />
              ) : null}
              <div className={isImage ? 'hidden' : ''}>
                {getDocumentIcon(document.documentType)}
              </div>
            </div>

            {/* Info Section */}
            <div className="p-4">
              <div className="flex items-start justify-between mb-2">
                <div className="flex-1 min-w-0">
                  <h4 className="font-medium text-gray-900 truncate text-sm">
                    {document.documentName}
                  </h4>
                  <p className="text-xs text-gray-500 mt-1">
                    {formatFileSize(document.fileSize)}
                  </p>
                </div>
                <span
                  className={`
                    px-2 py-1 text-xs rounded-full ml-2 flex-shrink-0
                    ${
                      document.documentType === 'Image'
                        ? 'bg-purple-100 text-purple-700'
                        : document.documentType === 'PDF'
                        ? 'bg-red-100 text-red-700'
                        : document.documentType === 'Word'
                        ? 'bg-blue-100 text-blue-700'
                        : document.documentType === 'Excel'
                        ? 'bg-green-100 text-green-700'
                        : 'bg-gray-100 text-gray-700'
                    }
                  `}
                >
                  {document.documentType}
                </span>
              </div>

              {document.description && (
                <p className="text-xs text-gray-600 mb-3 line-clamp-2">
                  {document.description}
                </p>
              )}

              <div className="text-xs text-gray-400 mb-3">
                <p>Subido por: {document.uploadedBy}</p>
                <p>{new Date(document.createdAt).toLocaleDateString('es-ES')}</p>
              </div>

              {/* Actions */}
              {showActions && (
                <div className="flex items-center space-x-2 pt-3 border-t border-gray-100">
                  <button
                    onClick={(e) => handleAction(e, () => onDownload(document))}
                    className="flex-1 flex items-center justify-center space-x-1 px-3 py-2 text-xs font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md transition-colors"
                    title="Descargar"
                  >
                    <FiDownload className="w-3 h-3" />
                    <span>Descargar</span>
                  </button>

                  {onEditDescription && (
                    <button
                      onClick={(e) => handleAction(e, () => onEditDescription(document))}
                      className="flex items-center justify-center p-2 text-gray-600 hover:text-blue-600 hover:bg-blue-50 rounded-md transition-colors"
                      title="Editar descripción"
                    >
                      <FiEdit2 className="w-3 h-3" />
                    </button>
                  )}

                  <button
                    onClick={(e) => handleAction(e, () => onDelete(document))}
                    className="flex items-center justify-center p-2 text-gray-600 hover:text-red-600 hover:bg-red-50 rounded-md transition-colors"
                    title="Eliminar"
                  >
                    <FiTrash2 className="w-3 h-3" />
                  </button>
                </div>
              )}
            </div>
          </div>
        );
      })}
    </div>
  );
};
