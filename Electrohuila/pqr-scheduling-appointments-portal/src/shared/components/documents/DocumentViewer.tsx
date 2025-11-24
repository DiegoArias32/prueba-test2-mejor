/**
 * Document Viewer Modal Component
 * Modal para visualizar documentos con navegación y acciones
 */

'use client';

import React, { useEffect, useState } from 'react';
import {
  FiX,
  FiDownload,
  FiChevronLeft,
  FiChevronRight,
  FiZoomIn,
  FiZoomOut,
  FiRotateCw,
} from 'react-icons/fi';
import type { AppointmentDocumentDto } from '@/services/documents';
import { formatFileSize, getFileExtension } from '@/shared/utils/fileHelpers';

interface DocumentViewerProps {
  document: AppointmentDocumentDto | null;
  documents?: AppointmentDocumentDto[];
  isOpen: boolean;
  onClose: () => void;
  onDownload?: (document: AppointmentDocumentDto) => void;
  onNavigate?: (document: AppointmentDocumentDto) => void;
}

export const DocumentViewer: React.FC<DocumentViewerProps> = ({
  document,
  documents = [],
  isOpen,
  onClose,
  onDownload,
  onNavigate,
}) => {
  const [zoom, setZoom] = useState(100);
  const [rotation, setRotation] = useState(0);
  const [currentIndex, setCurrentIndex] = useState(0);

  useEffect(() => {
    if (document && documents.length > 0) {
      const index = documents.findIndex((d) => d.id === document.id);
      setCurrentIndex(index >= 0 ? index : 0);
    }
  }, [document, documents]);

  useEffect(() => {
    // Reset zoom and rotation when document changes
    setZoom(100);
    setRotation(0);
  }, [document?.id]);

  useEffect(() => {
    // Handle keyboard navigation
    const handleKeyPress = (e: KeyboardEvent) => {
      if (!isOpen) return;

      switch (e.key) {
        case 'Escape':
          onClose();
          break;
        case 'ArrowLeft':
          handlePrevious();
          break;
        case 'ArrowRight':
          handleNext();
          break;
        case '+':
        case '=':
          handleZoomIn();
          break;
        case '-':
          handleZoomOut();
          break;
      }
    };

    window.addEventListener('keydown', handleKeyPress);
    return () => window.removeEventListener('keydown', handleKeyPress);
  }, [isOpen, currentIndex]);

  const handlePrevious = () => {
    if (documents.length === 0 || currentIndex <= 0) return;
    const prevDoc = documents[currentIndex - 1];
    setCurrentIndex(currentIndex - 1);
    onNavigate?.(prevDoc);
  };

  const handleNext = () => {
    if (documents.length === 0 || currentIndex >= documents.length - 1) return;
    const nextDoc = documents[currentIndex + 1];
    setCurrentIndex(currentIndex + 1);
    onNavigate?.(nextDoc);
  };

  const handleZoomIn = () => {
    setZoom((prev) => Math.min(prev + 25, 300));
  };

  const handleZoomOut = () => {
    setZoom((prev) => Math.max(prev - 25, 25));
  };

  const handleRotate = () => {
    setRotation((prev) => (prev + 90) % 360);
  };

  const handleDownload = () => {
    if (document && onDownload) {
      onDownload(document);
    }
  };

  if (!isOpen || !document) return null;

  const isImage = document.documentType === 'Image';
  const isPDF = document.documentType === 'PDF';
  const extension = getFileExtension(document.documentName);
  const canNavigate = documents.length > 1;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-90">
      {/* Header */}
      <div className="absolute top-0 left-0 right-0 bg-black bg-opacity-50 backdrop-blur-sm z-10">
        <div className="max-w-7xl mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            {/* Document Info */}
            <div className="flex-1 min-w-0 text-white">
              <h2 className="text-lg font-semibold truncate">
                {document.documentName}
              </h2>
              <div className="flex items-center space-x-4 text-sm text-gray-300 mt-1">
                <span>{formatFileSize(document.fileSize)}</span>
                <span>{document.documentType}</span>
                {canNavigate && (
                  <span>
                    {currentIndex + 1} de {documents.length}
                  </span>
                )}
              </div>
            </div>

            {/* Actions */}
            <div className="flex items-center space-x-2 ml-4">
              {isImage && (
                <>
                  <button
                    onClick={handleZoomOut}
                    disabled={zoom <= 25}
                    className="p-2 text-white hover:bg-white hover:bg-opacity-20 rounded-lg transition-colors disabled:opacity-50"
                    title="Alejar (Tecla -)"
                  >
                    <FiZoomOut className="w-5 h-5" />
                  </button>

                  <span className="text-white text-sm min-w-[60px] text-center">
                    {zoom}%
                  </span>

                  <button
                    onClick={handleZoomIn}
                    disabled={zoom >= 300}
                    className="p-2 text-white hover:bg-white hover:bg-opacity-20 rounded-lg transition-colors disabled:opacity-50"
                    title="Acercar (Tecla +)"
                  >
                    <FiZoomIn className="w-5 h-5" />
                  </button>

                  <button
                    onClick={handleRotate}
                    className="p-2 text-white hover:bg-white hover:bg-opacity-20 rounded-lg transition-colors"
                    title="Rotar"
                  >
                    <FiRotateCw className="w-5 h-5" />
                  </button>
                </>
              )}

              {onDownload && (
                <button
                  onClick={handleDownload}
                  className="p-2 text-white hover:bg-white hover:bg-opacity-20 rounded-lg transition-colors"
                  title="Descargar"
                >
                  <FiDownload className="w-5 h-5" />
                </button>
              )}

              <button
                onClick={onClose}
                className="p-2 text-white hover:bg-white hover:bg-opacity-20 rounded-lg transition-colors"
                title="Cerrar (Esc)"
              >
                <FiX className="w-6 h-6" />
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Navigation Buttons */}
      {canNavigate && (
        <>
          <button
            onClick={handlePrevious}
            disabled={currentIndex <= 0}
            className="absolute left-4 top-1/2 -translate-y-1/2 p-3 bg-black bg-opacity-50 hover:bg-opacity-70 text-white rounded-full transition-all disabled:opacity-30 disabled:cursor-not-allowed z-10"
            title="Anterior (←)"
          >
            <FiChevronLeft className="w-6 h-6" />
          </button>

          <button
            onClick={handleNext}
            disabled={currentIndex >= documents.length - 1}
            className="absolute right-4 top-1/2 -translate-y-1/2 p-3 bg-black bg-opacity-50 hover:bg-opacity-70 text-white rounded-full transition-all disabled:opacity-30 disabled:cursor-not-allowed z-10"
            title="Siguiente (→)"
          >
            <FiChevronRight className="w-6 h-6" />
          </button>
        </>
      )}

      {/* Document Content */}
      <div
        className="w-full h-full flex items-center justify-center p-20 overflow-auto"
        onClick={onClose}
      >
        <div
          className="max-w-full max-h-full"
          onClick={(e) => e.stopPropagation()}
        >
          {isImage ? (
            <img
              src={document.filePath}
              alt={document.documentName}
              className="max-w-full max-h-full object-contain transition-transform duration-200"
              style={{
                transform: `scale(${zoom / 100}) rotate(${rotation}deg)`,
              }}
            />
          ) : isPDF ? (
            <iframe
              src={document.filePath}
              className="w-full h-full min-h-[80vh] bg-white rounded-lg"
              title={document.documentName}
            />
          ) : (
            <div className="bg-white rounded-lg p-12 text-center max-w-md">
              <div className="mb-6">
                <div className="w-24 h-24 mx-auto bg-gray-100 rounded-full flex items-center justify-center">
                  <span className="text-4xl">📄</span>
                </div>
              </div>

              <h3 className="text-xl font-semibold text-gray-900 mb-2">
                {document.documentName}
              </h3>

              <p className="text-gray-600 mb-6">
                Este tipo de archivo ({extension}) no se puede visualizar en el navegador.
              </p>

              {document.description && (
                <div className="bg-gray-50 rounded-lg p-4 mb-6 text-left">
                  <p className="text-sm text-gray-700">{document.description}</p>
                </div>
              )}

              <div className="space-y-2 text-sm text-gray-500 mb-6">
                <p>Tipo: {document.documentType}</p>
                <p>Tamaño: {formatFileSize(document.fileSize)}</p>
                <p>Subido por: {document.uploadedBy}</p>
                <p>
                  Fecha: {new Date(document.createdAt).toLocaleDateString('es-ES')}
                </p>
              </div>

              {onDownload && (
                <button
                  onClick={handleDownload}
                  className="w-full flex items-center justify-center space-x-2 px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors"
                >
                  <FiDownload className="w-5 h-5" />
                  <span>Descargar archivo</span>
                </button>
              )}
            </div>
          )}
        </div>
      </div>

      {/* Footer Info */}
      {document.description && (isImage || isPDF) && (
        <div className="absolute bottom-0 left-0 right-0 bg-black bg-opacity-50 backdrop-blur-sm z-10">
          <div className="max-w-7xl mx-auto px-4 py-3">
            <p className="text-sm text-white">{document.description}</p>
          </div>
        </div>
      )}
    </div>
  );
};
