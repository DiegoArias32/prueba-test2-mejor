/**
 * File Helpers
 * Utilidades para manejo de archivos y validaciones
 */

import type { DocumentType } from '@/services/documents';

// Configuración de validación
export const FILE_VALIDATION_CONFIG = {
  MAX_FILE_SIZE_MB: 10,
  MAX_FILES_PER_UPLOAD: 5,
  ALLOWED_FILE_TYPES: [
    'image/jpeg',
    'image/jpg',
    'image/png',
    'image/gif',
    'image/webp',
    'application/pdf',
    'application/msword',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  ] as const,
  ALLOWED_EXTENSIONS: [
    '.jpg',
    '.jpeg',
    '.png',
    '.gif',
    '.webp',
    '.pdf',
    '.doc',
    '.docx',
    '.xls',
    '.xlsx',
  ] as const,
};

/**
 * Formatea el tamaño de un archivo en bytes a formato legible
 */
export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 Bytes';

  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(2))} ${sizes[i]}`;
}

/**
 * Obtiene la extensión de un archivo
 */
export function getFileExtension(filename: string): string {
  const lastDot = filename.lastIndexOf('.');
  if (lastDot === -1) return '';
  return filename.substring(lastDot).toLowerCase();
}

/**
 * Determina el tipo de documento basándose en la extensión
 */
export function getDocumentTypeFromExtension(extension: string): DocumentType {
  const ext = extension.toLowerCase();

  if (['.jpg', '.jpeg', '.png', '.gif', '.webp'].includes(ext)) {
    return 'Image';
  }

  if (ext === '.pdf') {
    return 'PDF';
  }

  if (['.doc', '.docx'].includes(ext)) {
    return 'Word';
  }

  if (['.xls', '.xlsx'].includes(ext)) {
    return 'Excel';
  }

  return 'Other';
}

/**
 * Determina el tipo de documento basándose en el archivo
 */
export function getDocumentTypeFromFile(file: File): DocumentType {
  const extension = getFileExtension(file.name);
  return getDocumentTypeFromExtension(extension);
}

/**
 * Valida que el tipo de archivo sea permitido
 */
export function validateFileType(file: File): boolean {
  // Validar por MIME type
  if (FILE_VALIDATION_CONFIG.ALLOWED_FILE_TYPES.includes(file.type as any)) {
    return true;
  }

  // Validar por extensión (fallback)
  const extension = getFileExtension(file.name);
  return FILE_VALIDATION_CONFIG.ALLOWED_EXTENSIONS.includes(extension as any);
}

/**
 * Valida que el tamaño del archivo no exceda el límite
 */
export function validateFileSize(file: File, maxMB?: number): boolean {
  const maxBytes = (maxMB || FILE_VALIDATION_CONFIG.MAX_FILE_SIZE_MB) * 1024 * 1024;
  return file.size <= maxBytes;
}

/**
 * Valida un archivo completamente
 */
export function validateFile(
  file: File
): { valid: boolean; error?: string } {
  if (!validateFileType(file)) {
    return {
      valid: false,
      error: `Tipo de archivo no permitido. Formatos aceptados: ${FILE_VALIDATION_CONFIG.ALLOWED_EXTENSIONS.join(', ')}`,
    };
  }

  if (!validateFileSize(file)) {
    return {
      valid: false,
      error: `El archivo excede el tamaño máximo permitido de ${FILE_VALIDATION_CONFIG.MAX_FILE_SIZE_MB} MB`,
    };
  }

  return { valid: true };
}

/**
 * Valida múltiples archivos
 */
export function validateFiles(
  files: File[]
): { valid: boolean; errors: string[] } {
  const errors: string[] = [];

  if (files.length > FILE_VALIDATION_CONFIG.MAX_FILES_PER_UPLOAD) {
    errors.push(
      `Solo puedes subir un máximo de ${FILE_VALIDATION_CONFIG.MAX_FILES_PER_UPLOAD} archivos a la vez`
    );
  }

  files.forEach((file, index) => {
    const validation = validateFile(file);
    if (!validation.valid) {
      errors.push(`Archivo "${file.name}": ${validation.error}`);
    }
  });

  return {
    valid: errors.length === 0,
    errors,
  };
}

/**
 * Genera un thumbnail para una imagen
 */
export function generateThumbnail(
  file: File,
  maxWidth = 200,
  maxHeight = 200
): Promise<string> {
  return new Promise((resolve, reject) => {
    if (!file.type.startsWith('image/')) {
      reject(new Error('El archivo no es una imagen'));
      return;
    }

    const reader = new FileReader();

    reader.onload = (e) => {
      const img = new Image();

      img.onload = () => {
        const canvas = document.createElement('canvas');
        let width = img.width;
        let height = img.height;

        // Calcular dimensiones manteniendo aspect ratio
        if (width > height) {
          if (width > maxWidth) {
            height = (height * maxWidth) / width;
            width = maxWidth;
          }
        } else {
          if (height > maxHeight) {
            width = (width * maxHeight) / height;
            height = maxHeight;
          }
        }

        canvas.width = width;
        canvas.height = height;

        const ctx = canvas.getContext('2d');
        if (!ctx) {
          reject(new Error('No se pudo crear el contexto del canvas'));
          return;
        }

        ctx.drawImage(img, 0, 0, width, height);

        // Convertir a base64
        const thumbnail = canvas.toDataURL(file.type);
        resolve(thumbnail);
      };

      img.onerror = () => {
        reject(new Error('Error al cargar la imagen'));
      };

      img.src = e.target?.result as string;
    };

    reader.onerror = () => {
      reject(new Error('Error al leer el archivo'));
    };

    reader.readAsDataURL(file);
  });
}

/**
 * Convierte un archivo a base64
 */
export function fileToBase64(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();

    reader.onload = () => {
      resolve(reader.result as string);
    };

    reader.onerror = () => {
      reject(new Error('Error al leer el archivo'));
    };

    reader.readAsDataURL(file);
  });
}

/**
 * Obtiene el icono correspondiente al tipo de documento
 */
export function getDocumentIcon(documentType: DocumentType): string {
  switch (documentType) {
    case 'Image':
      return '🖼️';
    case 'PDF':
      return '📄';
    case 'Word':
      return '📝';
    case 'Excel':
      return '📊';
    case 'Other':
      return '📎';
    default:
      return '📎';
  }
}

/**
 * Obtiene el color correspondiente al tipo de documento
 */
export function getDocumentColor(documentType: DocumentType): string {
  switch (documentType) {
    case 'Image':
      return 'text-purple-600 bg-purple-100';
    case 'PDF':
      return 'text-red-600 bg-red-100';
    case 'Word':
      return 'text-blue-600 bg-blue-100';
    case 'Excel':
      return 'text-green-600 bg-green-100';
    case 'Other':
      return 'text-gray-600 bg-gray-100';
    default:
      return 'text-gray-600 bg-gray-100';
  }
}

/**
 * Verifica si un archivo es una imagen
 */
export function isImageFile(file: File | string): boolean {
  if (typeof file === 'string') {
    const ext = getFileExtension(file);
    return ['.jpg', '.jpeg', '.png', '.gif', '.webp'].includes(ext);
  }
  return file.type.startsWith('image/');
}

/**
 * Verifica si un tipo de documento es imagen
 */
export function isImageDocumentType(documentType: DocumentType): boolean {
  return documentType === 'Image';
}
