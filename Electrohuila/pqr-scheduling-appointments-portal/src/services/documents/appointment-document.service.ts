/**
 * Appointment Documents Service
 * Servicio para gestión de documentos adjuntos de citas
 */

import { BaseHttpService } from '../base/base-http.service';
import type {
  AppointmentDocumentDto,
  CreateAppointmentDocumentDto,
  UpdateDocumentDescriptionDto,
  AppointmentDocumentsStatsDto,
  UploadDocumentParams,
} from './appointment-document.types';

class AppointmentDocumentService extends BaseHttpService {
  private readonly basePath = '/appointmentdocuments';

  /**
   * Obtener documento por ID
   */
  async getDocumentById(id: number): Promise<AppointmentDocumentDto> {
    return this.get<AppointmentDocumentDto>(`${this.basePath}/${id}`);
  }

  /**
   * Obtener todos los documentos de una cita
   */
  async getDocumentsByAppointment(appointmentId: number): Promise<AppointmentDocumentDto[]> {
    return this.get<AppointmentDocumentDto[]>(`${this.basePath}/appointment/${appointmentId}`);
  }

  /**
   * Obtener estadísticas de documentos de una cita
   */
  async getDocumentStats(appointmentId: number): Promise<AppointmentDocumentsStatsDto> {
    return this.get<AppointmentDocumentsStatsDto>(
      `${this.basePath}/appointment/${appointmentId}/stats`
    );
  }

  /**
   * Subir documento
   * Utiliza FormData para envío multipart
   */
  async uploadDocument(params: UploadDocumentParams): Promise<AppointmentDocumentDto> {
    const { file, appointmentId, description } = params;
    const formData = new FormData();

    formData.append('file', file);
    formData.append('appointmentId', appointmentId.toString());

    if (description) {
      formData.append('description', description);
    }

    const url = `${this.baseUrl}${this.basePath}`;
    const token = this.getAuthToken();

    const response = await fetch(url, {
      method: 'POST',
      headers: {
        ...(token && { 'Authorization': `Bearer ${token}` }),
        // NO incluir 'Content-Type' - el navegador lo agrega automáticamente con el boundary
      },
      body: formData,
    });

    if (response.status === 401) {
      this.clearAuthData();
      if (typeof window !== 'undefined') {
        window.location.href = '/login';
      }
      throw new Error('Sesión expirada. Por favor inicia sesión nuevamente.');
    }

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({
        message: 'Error al subir el documento'
      }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return await response.json();
  }

  /**
   * Subir múltiples documentos
   */
  async uploadMultipleDocuments(
    files: File[],
    appointmentId: number,
    descriptions?: string[]
  ): Promise<AppointmentDocumentDto[]> {
    const uploadPromises = files.map((file, index) =>
      this.uploadDocument({
        file,
        appointmentId,
        description: descriptions?.[index],
      })
    );

    return Promise.all(uploadPromises);
  }

  /**
   * Actualizar descripción de un documento
   */
  async updateDocumentDescription(
    id: number,
    description: string
  ): Promise<AppointmentDocumentDto> {
    const data: UpdateDocumentDescriptionDto = { description };
    return this.patch<AppointmentDocumentDto>(`${this.basePath}/${id}`, data);
  }

  /**
   * Eliminar documento (soft delete)
   */
  async deleteDocument(id: number): Promise<void> {
    await this.delete<void>(`${this.basePath}/${id}`);
  }

  /**
   * Descargar documento
   * Abre el archivo en una nueva pestaña o descarga según el tipo
   */
  downloadDocument(filePath: string, documentName: string): void {
    if (typeof window === 'undefined') return;

    // Si es una URL completa, abrirla directamente
    if (filePath.startsWith('http://') || filePath.startsWith('https://')) {
      const link = document.createElement('a');
      link.href = filePath;
      link.target = '_blank';
      link.download = documentName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } else {
      // Si es una ruta relativa, construir la URL completa
      const fullUrl = `${this.baseUrl}/files/${filePath}`;
      const link = document.createElement('a');
      link.href = fullUrl;
      link.target = '_blank';
      link.download = documentName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }

  /**
   * Obtener URL completa del documento
   */
  getDocumentUrl(filePath: string): string {
    if (filePath.startsWith('http://') || filePath.startsWith('https://')) {
      return filePath;
    }
    return `${this.baseUrl}/files/${filePath}`;
  }
}

// Exportar instancia única del servicio
export const appointmentDocumentService = new AppointmentDocumentService();
