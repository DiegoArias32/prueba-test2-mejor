# Módulo de Documentos Adjuntos para Citas

## Descripción General

Módulo completo para la gestión de documentos adjuntos asociados a citas, implementando arquitectura MVVM con componentes reutilizables, servicios tipados y validaciones robustas.

## Estructura de Archivos Creados

```
pqr-scheduling-appointments-portal/
├── src/
│   ├── services/
│   │   └── documents/
│   │       ├── appointment-document.service.ts    ✅ Servicio API completo
│   │       ├── appointment-document.types.ts      ✅ Definiciones de tipos
│   │       └── index.ts                           ✅ Exports
│   │
│   ├── shared/
│   │   ├── components/
│   │   │   └── documents/
│   │   │       ├── DocumentGallery.tsx            ✅ Galería de documentos
│   │   │       ├── DocumentUploader.tsx           ✅ Uploader básico
│   │   │       ├── DocumentUploaderWithProgress.tsx ✅ Uploader con hook
│   │   │       ├── DocumentViewer.tsx             ✅ Visor modal
│   │   │       ├── DocumentStats.tsx              ✅ Estadísticas visuales
│   │   │       └── index.ts                       ✅ Exports
│   │   │
│   │   ├── hooks/
│   │   │   ├── useDocumentUpload.ts              ✅ Hook para carga
│   │   │   ├── useDocumentManagement.ts          ✅ Hook para gestión
│   │   │   └── index.ts                          ✅ Actualizado
│   │   │
│   │   └── utils/
│   │       └── fileHelpers.ts                    ✅ Ya existía (verificado)
│   │
│   ├── features/
│   │   └── admin/
│   │       └── views/
│   │           └── appointments/
│   │               ├── AppointmentDocumentsView.tsx     ✅ Vista completa
│   │               └── components/
│   │                   └── DocumentsSection.tsx         ✅ Sección integrable
│   │
│   └── app/
│       └── admin/
│           └── appointments/
│               └── [id]/
│                   └── documents/
│                       └── page.tsx                     ✅ Página Next.js
│
└── DOCUMENTS_MODULE.md                                  ✅ Este archivo
```

## Componentes Principales

### 1. DocumentGallery
**Ubicación:** `src/shared/components/documents/DocumentGallery.tsx`

Galería visual de documentos con grid responsive y acciones.

**Props:**
```typescript
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
```

**Características:**
- Grid responsive (1/2/3 columnas)
- Preview de imágenes
- Iconos según tipo de documento
- Badges de tipo con colores
- Hover effects y animaciones
- Loading skeleton
- Estado vacío

### 2. DocumentUploader / DocumentUploaderWithProgress
**Ubicación:** `src/shared/components/documents/DocumentUploader[WithProgress].tsx`

Componente de carga con drag & drop.

**Props:**
```typescript
interface DocumentUploaderProps {
  appointmentId: number;
  onUploadComplete?: () => void;
  onCancel?: () => void;
  maxFiles?: number;
  disabled?: boolean;
  className?: string;
}
```

**Características:**
- Drag & drop interactivo
- Múltiples archivos
- Preview de imágenes
- Barra de progreso por archivo
- Validación automática
- Estados: pending, uploading, success, error
- Límite de archivos configurable

### 3. DocumentViewer
**Ubicación:** `src/shared/components/documents/DocumentViewer.tsx`

Modal fullscreen para visualización de documentos.

**Props:**
```typescript
interface DocumentViewerProps {
  document: AppointmentDocumentDto | null;
  documents?: AppointmentDocumentDto[];
  isOpen: boolean;
  onClose: () => void;
  onDownload?: (document: AppointmentDocumentDto) => void;
  onNavigate?: (document: AppointmentDocumentDto) => void;
}
```

**Características:**
- Fullscreen modal
- Soporte para imágenes (zoom, rotación)
- Preview de PDFs en iframe
- Navegación entre documentos (← →)
- Atajos de teclado
- Download integrado
- Responsive

### 4. DocumentStats
**Ubicación:** `src/shared/components/documents/DocumentStats.tsx`

Muestra estadísticas visuales de documentos.

**Props:**
```typescript
interface DocumentStatsProps {
  stats: AppointmentDocumentsStatsDto;
  isLoading?: boolean;
  className?: string;
}
```

**Estadísticas:**
- Total de documentos
- Total de imágenes
- Total de PDFs
- Tamaño total (formateado)

### 5. DocumentsSection
**Ubicación:** `src/features/admin/views/appointments/components/DocumentsSection.tsx`

Sección lista para integrar en detalles de cita.

**Props:**
```typescript
interface DocumentsSectionProps {
  appointmentId: number;
  showUploadButton?: boolean;
  onUploadClick?: () => void;
  className?: string;
}
```

### 6. AppointmentDocumentsView
**Ubicación:** `src/features/admin/views/appointments/AppointmentDocumentsView.tsx`

Vista completa standalone para gestión de documentos.

**Props:**
```typescript
interface AppointmentDocumentsViewProps {
  appointmentId: number;
  appointmentTitle?: string;
  showBackButton?: boolean;
  onBack?: () => void;
}
```

## Servicios API

### AppointmentDocumentService
**Ubicación:** `src/services/documents/appointment-document.service.ts`

**Métodos:**
```typescript
class AppointmentDocumentService {
  // GET
  getDocumentById(id: number): Promise<AppointmentDocumentDto>
  getDocumentsByAppointment(appointmentId: number): Promise<AppointmentDocumentDto[]>
  getDocumentStats(appointmentId: number): Promise<AppointmentDocumentsStatsDto>

  // POST
  uploadDocument(params: UploadDocumentParams): Promise<AppointmentDocumentDto>
  uploadMultipleDocuments(files: File[], appointmentId: number): Promise<AppointmentDocumentDto[]>

  // PATCH
  updateDocumentDescription(id: number, description: string): Promise<AppointmentDocumentDto>

  // DELETE
  deleteDocument(id: number): Promise<void>

  // UTILS
  downloadDocument(filePath: string, documentName: string): void
  getDocumentUrl(filePath: string): string
}
```

**Endpoints API:**
```
GET    /api/v1/appointmentdocuments/{id}
GET    /api/v1/appointmentdocuments/appointment/{appointmentId}
GET    /api/v1/appointmentdocuments/appointment/{appointmentId}/stats
POST   /api/v1/appointmentdocuments
PATCH  /api/v1/appointmentdocuments/{id}
DELETE /api/v1/appointmentdocuments/{id}
```

## Hooks Personalizados

### useDocumentUpload
**Ubicación:** `src/shared/hooks/useDocumentUpload.ts`

Hook para manejar la lógica de carga de documentos.

**Uso:**
```typescript
const {
  uploadFiles,
  uploadSingleFile,
  isUploading,
  uploadProgress,
  errors,
  resetProgress,
} = useDocumentUpload({
  appointmentId: 123,
  onSuccess: () => console.log('Success!'),
  onError: (error) => console.error(error),
});

// Subir múltiples archivos
await uploadFiles([file1, file2, file3]);

// Subir un archivo con descripción
await uploadSingleFile(file, 'Descripción del documento');
```

### useDocumentManagement
**Ubicación:** `src/shared/hooks/useDocumentManagement.ts`

Hook para gestión completa de documentos.

**Uso:**
```typescript
const {
  documents,
  stats,
  isLoading,
  error,
  loadDocuments,
  getDocument,
  updateDescription,
  deleteDocument,
  downloadDocument,
  getDocumentUrl,
  refresh,
} = useDocumentManagement({
  appointmentId: 123,
  autoLoad: true,
});
```

## Utilidades de Archivos

### fileHelpers
**Ubicación:** `src/shared/utils/fileHelpers.ts`

**Configuración:**
```typescript
FILE_VALIDATION_CONFIG = {
  MAX_FILE_SIZE_MB: 10,
  MAX_FILES_PER_UPLOAD: 5,
  ALLOWED_FILE_TYPES: ['image/jpeg', 'image/png', 'application/pdf', ...],
  ALLOWED_EXTENSIONS: ['.jpg', '.png', '.pdf', '.doc', ...],
}
```

**Funciones principales:**
```typescript
// Formateo
formatFileSize(bytes: number): string
getFileExtension(filename: string): string

// Tipo de documento
getDocumentTypeFromExtension(extension: string): DocumentType
getDocumentTypeFromFile(file: File): DocumentType

// Validación
validateFileType(file: File): boolean
validateFileSize(file: File, maxMB?: number): boolean
validateFile(file: File): { valid: boolean; error?: string }
validateFiles(files: File[]): { valid: boolean; errors: string[] }

// Preview
generateThumbnail(file: File, maxWidth?: number, maxHeight?: number): Promise<string>
fileToBase64(file: File): Promise<string>

// UI Helpers
getDocumentIcon(documentType: DocumentType): string
getDocumentColor(documentType: DocumentType): string
isImageFile(file: File | string): boolean
```

## Tipos de Datos

### AppointmentDocumentDto
```typescript
interface AppointmentDocumentDto {
  id: number;
  appointmentId: number;
  documentName: string;
  documentType: DocumentType;
  filePath: string;
  fileSize: number;
  uploadedBy: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
```

### DocumentType
```typescript
type DocumentType = 'Image' | 'PDF' | 'Word' | 'Excel' | 'Other';
```

### AppointmentDocumentsStatsDto
```typescript
interface AppointmentDocumentsStatsDto {
  totalDocuments: number;
  totalSizeBytes: number;
  imageCount: number;
  pdfCount: number;
  otherCount: number;
}
```

## Validaciones

### Tipos de Archivo Permitidos
- **Imágenes:** JPG, JPEG, PNG, GIF, WEBP
- **Documentos:** PDF, DOC, DOCX
- **Hojas de cálculo:** XLS, XLSX

### Límites
- **Tamaño máximo por archivo:** 10 MB
- **Archivos por carga:** 5 máximo
- **Total:** Configurable según servidor

## Integración en Proyecto

### 1. Uso Standalone (Vista Completa)

```typescript
import { AppointmentDocumentsView } from '@/features/admin/views/appointments/AppointmentDocumentsView';

export default function MyPage() {
  return (
    <AppointmentDocumentsView
      appointmentId={123}
      appointmentTitle="Cita #123"
      showBackButton={true}
    />
  );
}
```

### 2. Sección Integrada

```typescript
import { DocumentsSection } from '@/features/admin/views/appointments/components/DocumentsSection';

export default function AppointmentDetails({ appointment }) {
  return (
    <div>
      <h1>Detalles de Cita</h1>
      {/* Otros detalles */}

      <DocumentsSection
        appointmentId={appointment.id}
        showUploadButton={true}
        onUploadClick={() => setShowUploadModal(true)}
      />
    </div>
  );
}
```

### 3. Componentes Individuales

```typescript
import {
  DocumentGallery,
  DocumentViewer,
  DocumentUploaderWithProgress,
  DocumentStats,
} from '@/shared/components/documents';
import { useDocumentManagement } from '@/shared/hooks';

export default function CustomView() {
  const { documents, stats, downloadDocument } = useDocumentManagement({
    appointmentId: 123,
  });

  return (
    <>
      <DocumentStats stats={stats} />
      <DocumentGallery
        documents={documents}
        onDownload={downloadDocument}
        {...}
      />
    </>
  );
}
```

## Rutas Configuradas

### Página de Documentos
```
/admin/appointments/[id]/documents
```

Ejemplo:
```
/admin/appointments/123/documents
```

## Características Implementadas

### ✅ Features Clave
- [x] Drag & drop upload
- [x] Preview de imágenes y PDFs
- [x] Progress bar individual por archivo
- [x] Validación de tipos y tamaños
- [x] Galería responsive (1/2/3 columnas)
- [x] Estadísticas visuales
- [x] Modal viewer con zoom y rotación
- [x] Navegación entre documentos con teclado
- [x] Download de archivos
- [x] Edición de descripciones
- [x] Soft delete
- [x] Loading states
- [x] Error handling
- [x] Toast notifications
- [x] Confirm modals

### 🎨 UI/UX
- [x] Diseño responsive
- [x] Animaciones suaves
- [x] Estados de carga (skeletons)
- [x] Estados vacíos
- [x] Iconos y badges por tipo
- [x] Hover effects
- [x] Atajos de teclado
- [x] Accesibilidad básica

### 🏗️ Arquitectura
- [x] MVVM pattern
- [x] Componentes reutilizables
- [x] Servicios tipados con TypeScript
- [x] Custom hooks
- [x] Separación de responsabilidades
- [x] Error boundaries
- [x] Exports centralizados

## Testing

### Archivos a Probar
```bash
# Servicios
npm test src/services/documents/appointment-document.service.test.ts

# Componentes
npm test src/shared/components/documents/DocumentGallery.test.tsx
npm test src/shared/components/documents/DocumentUploader.test.tsx
npm test src/shared/components/documents/DocumentViewer.test.tsx

# Hooks
npm test src/shared/hooks/useDocumentUpload.test.ts
npm test src/shared/hooks/useDocumentManagement.test.ts

# Utils
npm test src/shared/utils/fileHelpers.test.ts
```

## Troubleshooting

### Problema: Los archivos no se suben
**Solución:**
1. Verificar que el backend esté configurado para multipart/form-data
2. Revisar el tamaño máximo permitido en el servidor
3. Verificar autenticación (token JWT)

### Problema: Las imágenes no se muestran
**Solución:**
1. Verificar que `filePath` sea una URL completa o relativa válida
2. Configurar CORS en el backend si las imágenes están en otro dominio
3. Revisar que el servicio `getDocumentUrl()` construya la URL correctamente

### Problema: Error 401 Unauthorized
**Solución:**
1. Verificar que el token esté en localStorage
2. Revisar que el servicio incluya el header Authorization
3. Renovar el token si expiró

## Próximas Mejoras Sugeridas

### Features Adicionales
- [ ] Búsqueda y filtrado de documentos
- [ ] Ordenamiento (por fecha, tamaño, tipo)
- [ ] Paginación para grandes cantidades
- [ ] Edición de metadatos avanzados
- [ ] Tags personalizados
- [ ] Vista de lista vs. grid
- [ ] Bulk operations (selección múltiple)
- [ ] Compartir documentos
- [ ] Historial de versiones
- [ ] Compresión automática de imágenes

### Mejoras Técnicas
- [ ] Tests unitarios completos
- [ ] Tests E2E con Playwright
- [ ] Optimización de bundle size
- [ ] Lazy loading de componentes
- [ ] Virtual scrolling para listas largas
- [ ] Service Worker para uploads offline
- [ ] IndexedDB para cache local
- [ ] WebSockets para progreso real-time

## Dependencias

### Requeridas
```json
{
  "react": "^19.0.0",
  "react-dom": "^19.0.0",
  "next": "15.3.2",
  "react-icons": "^5.5.0"
}
```

### Opcionales (para mejoras)
```json
{
  "@testing-library/react": "^14.0.0",
  "@testing-library/jest-dom": "^6.0.0",
  "jest": "^29.0.0"
}
```

## Soporte

Para reportar bugs o solicitar features, contactar al equipo de desarrollo.

## Licencia

Uso interno del proyecto PQR Scheduling Appointments Portal.

---

**Última actualización:** 2025-01-16
**Autor:** Claude Code Assistant
**Versión:** 1.0.0
