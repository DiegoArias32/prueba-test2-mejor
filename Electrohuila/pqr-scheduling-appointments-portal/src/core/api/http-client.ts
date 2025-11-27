/**
 * Cliente HTTP base para todas las peticiones a la API
 * Implementa manejo de autenticación, errores y reintentos
 */

import { API_CONFIG, STORAGE_KEYS } from '../config/api.config';

export interface HttpClientConfig extends RequestInit {
  skipAuth?: boolean;
  retries?: number;
}

export class HttpClient {
  private baseURL: string;
  private timeout: number;

  constructor(baseURL: string = API_CONFIG.BASE_URL, timeout: number = API_CONFIG.TIMEOUT) {
    this.baseURL = baseURL;
    this.timeout = timeout;
  }

  /**
   * Obtiene el token de autenticación
   */
  private getAuthToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem(STORAGE_KEYS.TOKEN);
  }

  /**
   * Limpia los datos de autenticación
   */
  private clearAuthData(): void {
    if (typeof window === 'undefined') return;

    Object.values(STORAGE_KEYS).forEach(key => {
      localStorage.removeItem(key);
    });
  }

  /**
   * Realiza una petición HTTP con manejo de errores y autenticación
   */
  async request<T>(endpoint: string, config: HttpClientConfig = {}): Promise<T> {
    const { skipAuth = false, retries = 0, ...requestConfig } = config;
    const url = `${this.baseURL}${endpoint}`;
    const token = this.getAuthToken();

    const headers: HeadersInit = {
      'Content-Type': 'application/json',
      ...(!skipAuth && token && { 'Authorization': `Bearer ${token}` }),
      ...requestConfig.headers,
    };

    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), this.timeout);

    try {
      const response = await fetch(url, {
        ...requestConfig,
        headers,
        signal: controller.signal,
      });

      clearTimeout(timeoutId);

      // Manejo de autenticación
      if (response.status === 401) {
        this.clearAuthData();
        if (typeof window !== 'undefined') {
          window.location.href = '/login';
        }
        throw new Error('Sesión expirada. Por favor inicia sesión nuevamente.');
      }

      // Manejo de errores - Parse respuesta incluso cuando hay error
      if (!response.ok) {
        let errorMessage = `HTTP error! status: ${response.status}`;

        try {
          const contentType = response.headers.get('content-type');
          let errorData: any;

          // Parse JSON si el content-type es application/json
          if (contentType && contentType.includes('application/json')) {
            errorData = await response.json();
          } else {
            // Si no es JSON, intenta leer como texto
            const text = await response.text();
            errorData = text;
          }

          // Extrae mensaje de error de múltiples posibles campos
          if (typeof errorData === 'object' && errorData !== null) {
            errorMessage =
              errorData.error ||        // Campo 'error' (formato personalizado: HOLIDAY_NOT_AVAILABLE|mensaje)
              errorData.detail ||       // Campo 'detail' (ProblemDetails de ASP.NET Core)
              errorData.message ||      // Campo 'message'
              errorData.title ||        // Campo 'title'
              (typeof errorData === 'string' ? errorData : null) ||
              `HTTP error! status: ${response.status}`;  // Fallback al mensaje por defecto con status
          } else if (typeof errorData === 'string' && errorData.trim()) {
            // Si es texto plano, usarlo como mensaje de error
            errorMessage = errorData;
          }
        } catch {
          // Si falla el parsing, usar el mensaje por defecto
          // (errorMessage ya tiene el valor por defecto)
        }

        throw new Error(errorMessage);
      }

      return await response.json();
    } catch (error) {
      clearTimeout(timeoutId);

      // Reintentar en caso de error de red
      if (retries > 0 && error instanceof TypeError) {
        return this.request<T>(endpoint, { ...config, retries: retries - 1 });
      }

      throw error;
    }
  }

  /**
   * Métodos HTTP convenientes
   */
  async get<T>(endpoint: string, config?: HttpClientConfig): Promise<T> {
    return this.request<T>(endpoint, { ...config, method: 'GET' });
  }

  async post<T>(endpoint: string, data?: unknown, config?: HttpClientConfig): Promise<T> {
    return this.request<T>(endpoint, {
      ...config,
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async put<T>(endpoint: string, data?: unknown, config?: HttpClientConfig): Promise<T> {
    return this.request<T>(endpoint, {
      ...config,
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async patch<T>(endpoint: string, data?: unknown, config?: HttpClientConfig): Promise<T> {
    return this.request<T>(endpoint, {
      ...config,
      method: 'PATCH',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async delete<T>(endpoint: string, config?: HttpClientConfig): Promise<T> {
    return this.request<T>(endpoint, { ...config, method: 'DELETE' });
  }
}

// Instancia global del cliente HTTP
export const httpClient = new HttpClient();
