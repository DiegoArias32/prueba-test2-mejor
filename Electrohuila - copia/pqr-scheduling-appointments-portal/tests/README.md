# 📚 Documentación del Sistema de Testing

**Proyecto**: PQR Scheduling Appointments Portal
**Framework**: Vitest 4.0.12 + @testing-library/react
**Última actualización**: 2025-11-21

---

## 📋 Tabla de Contenido

1. [Introducción](#introducción)
2. [Estructura de Carpetas](#estructura-de-carpetas)
3. [Configuración](#configuración)
4. [Ejecutar Tests](#ejecutar-tests)
5. [Escribir Tests](#escribir-tests)
6. [Helpers y Utilidades](#helpers-y-utilidades)
7. [Buenas Prácticas](#buenas-prácticas)
8. [Ejemplos Completos](#ejemplos-completos)
9. [Troubleshooting](#troubleshooting)

---

## 🎯 Introducción

Este proyecto utiliza **Vitest** como framework de testing, que es una alternativa moderna y rápida a Jest, optimizada para proyectos con Vite y Next.js.

### ¿Por qué Vitest?

- ⚡ **Ultra rápido**: Usa Vite para compilación instantánea
- 🔄 **Hot Module Replacement**: Los tests se re-ejecutan instantáneamente
- 📦 **Compatible con Jest**: API similar, fácil migración
- 🎨 **UI opcional**: Interfaz gráfica con `npm run test:ui`
- 🔍 **Coverage integrado**: Reportes de cobertura con V8

---

## 📁 Estructura de Carpetas

```
tests/
├── services/                        # Tests de servicios (lógica de negocio)
│   ├── appointments/               # Tests del servicio de citas
│   │   ├── test-helpers.ts         # Utilidades compartidas
│   │   ├── appointment-create.spec.ts       # 8 tests - Creación
│   │   ├── appointment-query.spec.ts        # 5 tests - Consultas
│   │   ├── appointment-management.spec.ts   # 8 tests - Gestión
│   │   ├── appointment-public.spec.ts       # 7 tests - Públicos
│   │   ├── appointment-availability.spec.ts # 8 tests - Disponibilidad
│   │   └── appointment-business-logic.spec.ts # 5 tests - Lógica de negocio
│   │
│   └── auth/                       # Tests del servicio de autenticación
│       └── auth.service.spec.ts    # 39 tests - Auth completo
│
├── features/                       # Tests de features (componentes + lógica)
│   └── (pendiente)
│
├── components/                     # Tests de componentes React
│   └── (pendiente)
│
└── README.md                       # Este archivo
```

### Convenciones de Nombres

- **`.spec.ts`**: Tests de servicios/lógica (TypeScript puro)
- **`.spec.tsx`**: Tests de componentes React (TypeScript + JSX)
- **`test-helpers.ts`**: Utilidades compartidas para un módulo
- **Agrupación**: Tests del mismo servicio en una carpeta

---

## ⚙️ Configuración

### 1. `vitest.config.ts` (Raíz del proyecto)

```typescript
import { defineConfig } from 'vitest/config';
import path from 'path';

export default defineConfig({
  test: {
    environment: 'jsdom',                    // Simula navegador
    globals: true,                           // describe, it, expect globales
    setupFiles: ['./vitest.setup.ts'],       // Setup antes de tests
    include: ['tests/**/*.{test,spec}.{js,ts,jsx,tsx}'],  // Busca tests en tests/
    coverage: {
      provider: 'v8',                        // Proveedor de cobertura
      reporter: ['text', 'json', 'html'],    // Formatos de reporte
      include: ['src/**/*.{js,ts,jsx,tsx}'], // Código a medir
      exclude: [
        'node_modules/',
        'vitest.setup.ts',
        'vitest.config.ts',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData',
        '**/.next',
        'tests/',                            // No medir los tests
      ],
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'), // Alias @ para imports
    },
  },
});
```

**Configuraciones clave**:
- `environment: 'jsdom'`: Necesario para tests de componentes React
- `globals: true`: No necesitas importar `describe`, `it`, `expect`
- `include`: Define dónde buscar tests
- `alias`: Permite usar `@/` en lugar de rutas relativas

### 2. `vitest.setup.ts` (Raíz del proyecto)

```typescript
import '@testing-library/jest-dom';
```

Este archivo se ejecuta antes de todos los tests y configura:
- Matchers adicionales de `jest-dom` (`toBeInTheDocument`, `toHaveClass`, etc.)

### 3. `package.json` (Scripts)

```json
{
  "scripts": {
    "test": "vitest",                       // Modo watch (re-ejecuta al cambiar)
    "test:ui": "vitest --ui",              // Interfaz gráfica
    "test:coverage": "vitest --coverage",   // Con reporte de cobertura
    "test:run": "vitest run"               // Una sola ejecución (CI)
  }
}
```

---

## 🚀 Ejecutar Tests

### Comandos Básicos

```bash
# Modo watch (recomendado durante desarrollo)
npm run test

# Ejecutar solo tests que coinciden con un patrón
npm run test auth

# Interfaz gráfica interactiva
npm run test:ui

# Una sola ejecución (para CI/CD)
npm run test:run

# Con reporte de cobertura
npm run test:coverage
```

### Modo Watch

El modo watch es **increíblemente rápido**:
- Detecta cambios en archivos
- Re-ejecuta solo los tests afectados
- Muestra resultados en tiempo real

```bash
npm run test

# Output:
✓ tests/services/auth/auth.service.spec.ts (39 tests) 87ms
✓ tests/services/appointments/appointment-create.spec.ts (8 tests) 45ms
...

Test Files  7 passed (7)
Tests  80 passed (80)
```

### Interfaz Gráfica

```bash
npm run test:ui
```

Abre en el navegador una interfaz donde puedes:
- Ver tests en árbol jerárquico
- Ejecutar tests individuales
- Ver detalles de errores
- Filtrar por nombre

---

## ✍️ Escribir Tests

### Anatomía de un Test

```typescript
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { MyService } from '@/services/my-service';

describe('MyService - Descripción del conjunto', () => {
  let service: MyService;

  // Se ejecuta ANTES de cada test
  beforeEach(() => {
    service = new MyService();
    // Setup común
  });

  // Se ejecuta DESPUÉS de cada test
  afterEach(() => {
    vi.clearAllMocks(); // Limpiar mocks
  });

  // Test individual
  it('should do something specific', () => {
    // Arrange (preparar)
    const input = 'test';

    // Act (ejecutar)
    const result = service.doSomething(input);

    // Assert (verificar)
    expect(result).toBe('expected');
  });
});
```

### Estructura AAA (Arrange-Act-Assert)

```typescript
it('should create a new appointment', () => {
  // ===== ARRANGE ===== (Preparar datos y mocks)
  const appointmentData = {
    clientId: 1,
    branchId: 1,
    appointmentDate: '2025-12-01',
    appointmentTime: '10:00',
  };

  const mockResponse = {
    id: 1,
    ...appointmentData,
    status: 'Pending',
  };

  (service as any).post.mockResolvedValue(mockResponse);

  // ===== ACT ===== (Ejecutar la función)
  const result = await service.createAppointment(appointmentData);

  // ===== ASSERT ===== (Verificar resultados)
  expect(result).toEqual(mockResponse);
  expect((service as any).post).toHaveBeenCalledWith('/appointments', appointmentData);
});
```

### Tipos de Tests

#### 1. Test de Éxito (Happy Path)

```typescript
it('should login user successfully', async () => {
  const credentials = { username: 'user', password: 'pass' };
  const expectedResponse = { token: 'abc', user: { id: 1 } };

  (service as any).post.mockResolvedValue(expectedResponse);

  const result = await service.login(credentials);

  expect(result).toEqual(expectedResponse);
  expect(result.token).toBe('abc');
});
```

#### 2. Test de Error

```typescript
it('should throw error when credentials are invalid', async () => {
  const credentials = { username: 'wrong', password: 'wrong' };

  (service as any).post.mockRejectedValue(new Error('Invalid credentials'));

  await expect(service.login(credentials)).rejects.toThrow('Invalid credentials');
});
```

#### 3. Test Parametrizado (Data-Driven)

```typescript
it.each([
  ['email@test.com', true],
  ['invalid', false],
  ['@test.com', false],
])('should validate email "%s" as %s', (email, expected) => {
  const result = service.validateEmail(email);
  expect(result).toBe(expected);
});
```

---

## 🛠️ Helpers y Utilidades

### Test Helpers

Los helpers son funciones reutilizables que simplifican la configuración de tests.

**Ejemplo**: `tests/services/appointments/test-helpers.ts`

```typescript
import { beforeEach, afterEach, vi } from 'vitest';
import { AppointmentService } from '@/services/appointments/appointment.service';

export let service: AppointmentService;

/**
 * Setup común para tests de AppointmentService
 * Uso: llamar al inicio de cada describe
 */
export function setupAppointmentService() {
  beforeEach(() => {
    service = new AppointmentService();

    // Spy en métodos HTTP
    vi.spyOn(service as any, 'get').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'post').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'patch').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'delete').mockImplementation(vi.fn());
  });

  afterEach(() => {
    vi.clearAllMocks();
  });
}
```

**Uso en tests**:

```typescript
import { describe, it, expect } from 'vitest';
import { setupAppointmentService, service } from './test-helpers';

describe('AppointmentService - Create', () => {
  setupAppointmentService(); // Configura automáticamente

  it('should create appointment', async () => {
    // service ya está disponible y mockeado
    (service as any).post.mockResolvedValue({ id: 1 });
    const result = await service.createAppointment({...});
    expect(result.id).toBe(1);
  });
});
```

### Mocking con Vitest

#### Mock de funciones

```typescript
// Crear mock
const mockFn = vi.fn();

// Mock con valor de retorno
mockFn.mockReturnValue('value');

// Mock con valor asíncrono
mockFn.mockResolvedValue({ data: 'value' });

// Mock que lanza error
mockFn.mockRejectedValue(new Error('Failed'));

// Verificar llamadas
expect(mockFn).toHaveBeenCalled();
expect(mockFn).toHaveBeenCalledWith('arg1', 'arg2');
expect(mockFn).toHaveBeenCalledTimes(2);
```

#### Spy en métodos existentes

```typescript
const spy = vi.spyOn(object, 'method');
spy.mockImplementation(() => 'new value');

// Restaurar implementación original
spy.mockRestore();
```

#### Mock de módulos completos

```typescript
vi.mock('@/services/api', () => ({
  ApiService: vi.fn().mockImplementation(() => ({
    get: vi.fn().mockResolvedValue({ data: 'mocked' }),
  })),
}));
```

### Mock de localStorage

```typescript
beforeEach(() => {
  const localStorageMock = {};

  global.localStorage = {
    getItem: vi.fn((key) => localStorageMock[key] || null),
    setItem: vi.fn((key, value) => {
      localStorageMock[key] = value;
    }),
    removeItem: vi.fn((key) => {
      delete localStorageMock[key];
    }),
    clear: vi.fn(() => {
      localStorageMock = {};
    }),
  } as any;
});
```

---

## 💡 Buenas Prácticas

### 1. Nombres Descriptivos

```typescript
// ❌ Malo
it('should work', () => {});

// ✅ Bueno
it('should create appointment with valid data', () => {});
```

### 2. Un Concepto por Test

```typescript
// ❌ Malo - testea múltiples cosas
it('should create and update appointment', () => {
  const created = await service.create({...});
  const updated = await service.update(created.id, {...});
  expect(created).toBeDefined();
  expect(updated).toBeDefined();
});

// ✅ Bueno - tests separados
it('should create appointment', () => {
  const created = await service.create({...});
  expect(created).toBeDefined();
});

it('should update appointment', () => {
  const updated = await service.update(1, {...});
  expect(updated).toBeDefined();
});
```

### 3. Arrange-Act-Assert Claro

```typescript
it('should validate email format', () => {
  // Arrange
  const validEmail = 'test@example.com';

  // Act
  const result = service.validateEmail(validEmail);

  // Assert
  expect(result).toBe(true);
});
```

### 4. Limpiar Mocks

```typescript
afterEach(() => {
  vi.clearAllMocks();  // Limpia contadores de llamadas
  vi.resetAllMocks();  // Además, resetea implementaciones
});
```

### 5. Evitar Lógica en Tests

```typescript
// ❌ Malo - lógica compleja
it('should calculate total', () => {
  const items = [1, 2, 3];
  let total = 0;
  for (const item of items) {
    total += item;
  }
  expect(service.getTotal(items)).toBe(total);
});

// ✅ Bueno - valor esperado directo
it('should calculate total', () => {
  const items = [1, 2, 3];
  expect(service.getTotal(items)).toBe(6);
});
```

### 6. Tests Independientes

```typescript
// ❌ Malo - tests dependen entre sí
let sharedData;

it('should create data', () => {
  sharedData = service.create({...});
});

it('should use created data', () => {
  expect(sharedData.id).toBe(1); // Falla si el anterior falla
});

// ✅ Bueno - cada test es independiente
it('should create data', () => {
  const data = service.create({...});
  expect(data).toBeDefined();
});

it('should update data', () => {
  const data = service.create({...});
  const updated = service.update(data.id, {...});
  expect(updated).toBeDefined();
});
```

---

## 📖 Ejemplos Completos

### Ejemplo 1: Test de Servicio Simple

```typescript
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { UserService } from '@/services/users/user.service';

describe('UserService - User Management', () => {
  let service: UserService;

  beforeEach(() => {
    service = new UserService();
    vi.spyOn(service as any, 'get').mockImplementation(vi.fn());
    vi.spyOn(service as any, 'post').mockImplementation(vi.fn());
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('getUser', () => {
    it('should get user by id', async () => {
      // Arrange
      const userId = 1;
      const mockUser = { id: 1, name: 'John', email: 'john@test.com' };
      (service as any).get.mockResolvedValue(mockUser);

      // Act
      const result = await service.getUser(userId);

      // Assert
      expect(result).toEqual(mockUser);
      expect((service as any).get).toHaveBeenCalledWith('/users/1');
    });

    it('should throw error if user not found', async () => {
      // Arrange
      const userId = 999;
      (service as any).get.mockRejectedValue(new Error('User not found'));

      // Act & Assert
      await expect(service.getUser(userId)).rejects.toThrow('User not found');
    });
  });

  describe('createUser', () => {
    it('should create user successfully', async () => {
      // Arrange
      const newUser = { name: 'Jane', email: 'jane@test.com', password: 'pass123' };
      const mockResponse = { id: 2, ...newUser };
      (service as any).post.mockResolvedValue(mockResponse);

      // Act
      const result = await service.createUser(newUser);

      // Assert
      expect(result.id).toBe(2);
      expect(result.email).toBe('jane@test.com');
      expect((service as any).post).toHaveBeenCalledWith('/users', newUser);
    });
  });
});
```

### Ejemplo 2: Test de Componente React

```typescript
import { describe, it, expect } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { LoginForm } from '@/features/auth/views/LoginForm';

describe('LoginForm Component', () => {
  it('should render login form', () => {
    render(<LoginForm />);

    expect(screen.getByLabelText(/username/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
  });

  it('should call onSubmit when form is submitted', async () => {
    const mockOnSubmit = vi.fn();
    render(<LoginForm onSubmit={mockOnSubmit} />);

    // Llenar formulario
    fireEvent.change(screen.getByLabelText(/username/i), {
      target: { value: 'testuser' },
    });
    fireEvent.change(screen.getByLabelText(/password/i), {
      target: { value: 'password123' },
    });

    // Enviar formulario
    fireEvent.click(screen.getByRole('button', { name: /login/i }));

    // Verificar
    expect(mockOnSubmit).toHaveBeenCalledWith({
      username: 'testuser',
      password: 'password123',
    });
  });

  it('should show error message on invalid credentials', async () => {
    render(<LoginForm />);

    // Simular error
    const errorMessage = 'Invalid credentials';

    // ... lógica de test
  });
});
```

### Ejemplo 3: Test con Múltiples Escenarios

```typescript
describe('EmailValidator', () => {
  describe('validateEmail', () => {
    it.each([
      ['valid@email.com', true],
      ['user.name+tag@example.co.uk', true],
      ['invalid', false],
      ['@invalid.com', false],
      ['invalid@', false],
      ['invalid@.com', false],
      ['', false],
    ])('should validate "%s" as %s', (email, expected) => {
      const result = validateEmail(email);
      expect(result).toBe(expected);
    });
  });
});
```

---

## 🐛 Troubleshooting

### Problema: Tests no se ejecutan

**Síntoma**: `npm run test` no encuentra tests

**Solución**:
```bash
# Verificar configuración
cat vitest.config.ts

# Asegurar que include apunte a tests/
include: ['tests/**/*.{test,spec}.{js,ts,jsx,tsx}']
```

### Problema: Imports con @ no funcionan

**Síntoma**: `Error: Cannot find module '@/services/...'`

**Solución**:
```typescript
// vitest.config.ts
resolve: {
  alias: {
    '@': path.resolve(__dirname, './src'),
  },
},
```

### Problema: Mocks no se limpian entre tests

**Síntoma**: Test A afecta a Test B

**Solución**:
```typescript
afterEach(() => {
  vi.clearAllMocks();  // Agregar en todos los describe
});
```

### Problema: Tests de React fallan con `document is not defined`

**Síntoma**: `ReferenceError: document is not defined`

**Solución**:
```typescript
// vitest.config.ts
test: {
  environment: 'jsdom',  // ← Asegurar que está configurado
}
```

### Problema: Coverage no incluye archivos

**Síntoma**: Cobertura al 0%

**Solución**:
```typescript
// vitest.config.ts
coverage: {
  include: ['src/**/*.{js,ts,jsx,tsx}'],  // Debe apuntar a src/
  exclude: ['tests/', 'node_modules/'],
}
```

---

## 📊 Estadísticas Actuales del Proyecto

```
📁 tests/
├── 7 archivos de test
├── 80 tests totales
│   ├── 39 tests de auth.service
│   └── 41 tests de appointment.service
│       ├── 8 tests de creación
│       ├── 5 tests de consulta
│       ├── 8 tests de gestión
│       ├── 7 tests públicos
│       ├── 8 tests de disponibilidad
│       └── 5 tests de lógica de negocio
└── 1 helper compartido

Cobertura estimada: ~17%
Tiempo de ejecución: ~3-5 segundos
```

---

## 🎯 Próximos Pasos Recomendados

1. **Agregar tests de componentes React**
   - NotificationBell
   - AdminLayout
   - LoginForm

2. **Agregar tests de hooks**
   - useAuth
   - useNotifications
   - useWebSocket

3. **Agregar tests de utilidades**
   - Funciones de validación
   - Formatters
   - Helpers

4. **Mejorar cobertura de servicios**
   - users.service
   - permissions.service
   - catalogs.service

---

## 📚 Referencias

- **Vitest**: https://vitest.dev/
- **Testing Library**: https://testing-library.com/
- **Testing Best Practices**: https://kentcdodds.com/blog/common-mistakes-with-react-testing-library

---

**Última actualización**: 2025-11-21
**Mantenedor**: Equipo de Desarrollo
**Versión**: 1.0
