# Frontend - ElectroHuila PQR

Sistema de agendamiento de citas y gestión de PQR para ElectroHuila.

## 🏗️ Arquitectura

Este proyecto utiliza **MVVM Feature-based Architecture** con Next.js 15, React 19, TypeScript y Tailwind CSS 4.

```
src/
├── features/          # Features organizados por funcionalidad (MVVM)
│   ├── auth/         # ✅ Autenticación
│   ├── appointments/ # ✅ Agendamiento de citas
│   ├── appointment-management/ # ✅ Gestión de citas
│   └── ...
├── shared/           # Componentes y utilidades compartidas
│   ├── components/
│   ├── layouts/
│   ├── hooks/
│   └── utils/
├── core/             # Infraestructura central
│   ├── api/         # HTTP Client
│   ├── config/      # Configuración
│   └── types/       # Tipos globales
└── app/              # Next.js App Router (solo rutas)
```

### Patrón MVVM por Feature

Cada feature sigue esta estructura:

```
features/nombre-feature/
├── models/          # DTOs, Interfaces, Types
├── repositories/    # Acceso a datos (API calls)
├── viewmodels/      # Custom Hooks (lógica de negocio)
├── views/           # Componentes React UI
└── index.ts         # Exportaciones públicas
```

## 🚀 Inicio Rápido

### Configuración del Backend

Este frontend se conecta al backend .NET API. Configura la URL del backend:

1. **Crea un archivo `.env.local`** en la raíz del proyecto:

```bash
cp .env.example .env.local
```

2. **Edita `.env.local`** con la URL de tu backend:

```bash
# Para desarrollo local
NEXT_PUBLIC_API_URL=http://localhost:5000/api/v1

# Para producción
# NEXT_PUBLIC_API_URL=https://tu-backend.com/api/v1
```

### Instalación

```bash
npm install
```

### Desarrollo

```bash
npm run dev
```

Abrir [http://localhost:3000](http://localhost:3000) en el navegador.

**Importante:** Asegúrate de que el backend .NET API esté corriendo antes de iniciar el frontend.

### Build

```bash
npm run build
```

### Linting

```bash
npm run lint
```

## 📚 Documentación

- **[ARQUITECTURA.md](./ARQUITECTURA.md)** - Documentación completa de la arquitectura

## 🛠️ Stack Tecnológico

- **Framework:** Next.js 15.3.2 (App Router)
- **UI Library:** React 19
- **Lenguaje:** TypeScript 5
- **Estilos:** Tailwind CSS 4.1.6
- **Iconos:** React Icons, Font Awesome 7
- **QR:** qrcode 1.5.4

## 📦 Features Implementados

- ✅ **auth** - Sistema de autenticación completo
- ✅ **appointments** - Agendamiento de citas públicas
- ✅ **appointment-management** - Gestión de citas (admin)
- 🔄 **cuentas-nuevas** - Solicitud de cuentas nuevas
- 🔄 **proyecto-nuevo** - Solicitud de proyecto nuevo
- 🔄 **verificar-cita** - Verificación de citas
- 🔄 **servicios** - Listado de servicios
- 🔄 **admin** - Panel administrativo

## 🎯 Convenciones

### Nombrado de Archivos

- **Models**: `{domain}.models.ts`
- **ViewModels**: `use{Feature}.ts`
- **Views**: `{Feature}View.tsx`
- **Repositories**: `{domain}.repository.ts`

### Imports

Siempre usar alias `@/` para imports:

```typescript
// ✅ Correcto
import { useAuth } from '@/features/auth';
import { ValidationUtils } from '@/shared/utils/validation.utils';

// ❌ Evitar
import { useAuth } from '../../../features/auth';
```

## 📝 Crear Nuevo Feature

1. Crear estructura de carpetas:
```bash
mkdir -p features/mi-feature/{models,repositories,viewmodels,views}
```

2. Implementar siguiendo el patrón MVVM
3. Exportar desde `index.ts`
4. Usar en página de Next.js

Ver [ARQUITECTURA.md](./ARQUITECTURA.md) para más detalles.

## 🔌 Conexión con Backend .NET

### Endpoints Configurados

El frontend se conecta con los siguientes endpoints del backend:

#### Autenticación
- `POST /api/v1/auth/login` - Login de usuario
- `POST /api/v1/auth/refresh-token` - Renovar token
- `POST /api/v1/auth/logout` - Cerrar sesión
- `GET /api/v1/auth/user-info` - Info del usuario autenticado
- `GET /api/v1/auth/permissions` - Permisos del usuario

#### Citas (Appointments)
- `POST /api/v1/appointments` - Crear cita
- `POST /api/v1/appointments/schedule` - Agendar cita (con validaciones)
- `GET /api/v1/appointments/available-times` - Obtener horarios disponibles
- `GET /api/v1/appointments/availability` - Validar disponibilidad
- `GET /api/v1/appointments/verify-qr` - Verificar cita por QR (público)

#### Cuentas Nuevas (New Accounts)
- `POST /api/v1/newaccounts` - Crear solicitud de cuenta nueva (público)
- `GET /api/v1/newaccounts/{id}` - Obtener solicitud por ID
- `GET /api/v1/newaccounts/by-request/{requestNumber}` - Obtener por número de solicitud

#### Proyectos (Project News)
- `POST /api/v1/projectnews` - Crear solicitud de proyecto (público)
- `GET /api/v1/projectnews/{id}` - Obtener proyecto por ID
- `GET /api/v1/projectnews/by-document/{documentNumber}` - Obtener por documento

#### Catálogos
- `GET /api/v1/branches` - Obtener sucursales
- `GET /api/v1/appointmenttypes` - Obtener tipos de cita
- `GET /api/v1/documenttypes` - Obtener tipos de documento
- `GET /api/v1/clients` - Obtener clientes

### Estructura de DTOs

Los formularios envían datos según los DTOs del backend:

**CreateAppointmentDto:**
```typescript
{
  appointmentDate: string;      // DateTime
  appointmentTime?: string;     // string nullable
  notes?: string;               // string nullable
  clientId: number;             // int
  branchId: number;             // int
  appointmentTypeId: number;    // int
}
```

**RequestNewAccountDto:**
```typescript
{
  documentTypeId?: number;      // int nullable
  documentNumber?: string;      // string nullable
  fullName: string;             // string
  phone?: string;               // string nullable
  mobile?: string;              // string nullable
  email?: string;               // string nullable
  address?: string;             // string nullable
  branchId?: number;            // int nullable
  appointmentDate?: string;     // DateTime nullable
  appointmentTime?: string;     // string nullable
  observations?: string;        // string nullable
}
```

**CreateProjectNewDto:**
```typescript
{
  documentType: string;         // string
  documentNumber: string;       // string
  fullName: string;             // string
  phone?: string;               // string nullable
  mobile: string;               // string
  email: string;                // string
  projectName: string;          // string
  sector: string;               // string
  municipality: string;         // string
  descriptionProject: string;   // string
  branchId: number;             // int
  appointmentDate: string;      // DateTime
}
```

## 🔐 Autenticación

El sistema usa JWT tokens con refresh tokens. Los permisos se gestionan por formulario (FORM_CODE).

```typescript
import { useAuth } from '@/features/auth';

const { login, logout, isAuthenticated, hasPermission } = useAuth();
```

## 🎨 Tailwind CSS

Tailwind 4.x está configurado con PostCSS. Los colores corporativos son:

- Azul primario: `#1797D5`
- Azul oscuro: `#203461`
- Azul claro: `#56C2E1`
- Naranja: `#FF7A00`

## 📄 Licencia

Propiedad de ElectroHuila S.A. E.S.P.

---

**Última actualización:** 2025-01-09
