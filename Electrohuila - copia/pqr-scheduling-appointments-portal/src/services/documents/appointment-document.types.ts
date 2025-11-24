/**
 * Appointment Documents - Type Definitions
 * Tipos para gestión de documentos adjuntos de citas
 */

export type DocumentType = 'Image' | 'PDF' | 'Word' | 'Excel' | 'Other';

export interface AppointmentDocumentDto {
  id: number;
  appointmentId: number;
  documentName: string;
  documentType: DocumentType;
  filePath: string; // URL o path del archivo
  fileSize: number; // En bytes
  uploadedBy: string; // Username del que subió
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAppointmentDocumentDto {
  appointmentId: number;
  documentName: string;
  documentType: string;
  filePath: string;
  fileSize: number;
  description?: string;
}

export interface UpdateDocumentDescriptionDto {
  description: string;
}

export interface AppointmentDocumentsStatsDto {
  totalDocuments: number;
  totalSizeBytes: number;
  imageCount: number;
  pdfCount: number;
  otherCount: number;
}

export interface UploadDocumentParams {
  file: File;
  appointmentId: number;
  description?: string;
}

export interface DocumentValidationError {
  field: string;
  message: string;
}

export interface DocumentUploadProgress {
  documentId?: string;
  fileName: string;
  progress: number; // 0-100
  status: 'pending' | 'uploading' | 'success' | 'error';
  error?: string;
}
