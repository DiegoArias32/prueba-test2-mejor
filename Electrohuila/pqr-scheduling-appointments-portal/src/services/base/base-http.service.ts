/**
 * Base HTTP Service
 * Provides common HTTP methods and error handling for all domain services
 */

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://8papi9muvp.us-east-2.awsapprunner.com/api/v1';

export class BaseHttpService {
  protected baseUrl: string;

  constructor() {
    this.baseUrl = API_BASE_URL;
  }

  /**
   * Get authentication token from localStorage
   */
  protected getAuthToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem('token');
  }

  /**
   * Clear all authentication data from localStorage
   */
  protected clearAuthData(): void {
    if (typeof window === 'undefined') return;

    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    localStorage.removeItem('roles');
    localStorage.removeItem('permissions');
    localStorage.removeItem('empleado_logueado');
    localStorage.removeItem('empleado_username');
  }

  /**
   * Generic request method with error handling
   */
  protected async request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;
    const token = this.getAuthToken();

    const config: RequestInit = {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }),
        ...options.headers,
      },
      ...options,
    };

    try {
      const response = await fetch(url, config);

      if (response.status === 401) {
        // Token expired or invalid
        this.clearAuthData();
        if (typeof window !== 'undefined') {
          window.location.href = '/login';
        }
        throw new Error('Sesión expirada. Por favor inicia sesión nuevamente.');
      }

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({ message: 'Error desconocido' }));
        // Backend can return errors in multiple formats:
        // 1. { error: "message" } - from ApiController BadRequest
        // 2. { error: "...", errors: {...} } - from ValidationException
        // 3. { message: "..." } - from other sources
        const errorMessage = errorData.error || errorData.message || `HTTP error! status: ${response.status}`;
        throw new Error(errorMessage);
      }

      // Handle empty responses (e.g., 200 OK with no body from logical delete operations)
      const contentType = response.headers.get('content-type');
      const contentLength = response.headers.get('content-length');

      // If no content, return appropriate response based on HTTP method
      if (contentLength === '0' || !contentType?.includes('application/json')) {
        // Para GET requests, retornar array vacío si se espera un array
        // Para otros métodos (DELETE, POST, PUT, PATCH), retornar objeto success
        const method = options.method || 'GET';
        if (method === 'GET') {
          return [] as T;
        }
        return { success: true, message: 'Operation completed successfully' } as T;
      }

      return await response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Public request method (no authentication required)
   */
  protected async publicRequest<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;

    const config: RequestInit = {
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        ...options.headers,
      },
      ...options,
    };

    try {
      const response = await fetch(url, config);

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({ message: 'Error desconocido' }));
        // Backend can return errors in multiple formats:
        // 1. { error: "message" } - from ApiController BadRequest
        // 2. { error: "...", errors: {...} } - from ValidationException
        // 3. { message: "..." } - from other sources
        const errorMessage = errorData.error || errorData.message || `HTTP error! status: ${response.status}`;
        throw new Error(errorMessage);
      }

      // Handle empty responses
      const contentType = response.headers.get('content-type');
      const contentLength = response.headers.get('content-length');

      // If no content, return appropriate response based on HTTP method
      if (contentLength === '0' || !contentType?.includes('application/json')) {
        // Para GET requests, retornar array vacío si se espera un array
        // Para otros métodos (DELETE, POST, PUT, PATCH), retornar objeto success
        const method = options.method || 'GET';
        if (method === 'GET') {
          return [] as T;
        }
        return { success: true, message: 'Operation completed successfully' } as T;
      }

      return await response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * GET request
   */
  protected async get<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: 'GET' });
  }

  /**
   * POST request
   */
  protected async post<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * PUT request
   */
  protected async put<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * PATCH request
   */
  protected async patch<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'PATCH',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * DELETE request
   */
  protected async delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: 'DELETE' });
  }

  /**
   * Public GET request (no authentication)
   */
  protected async publicGet<T>(endpoint: string): Promise<T> {
    return this.publicRequest<T>(endpoint, { method: 'GET' });
  }

  /**
   * Public POST request (no authentication)
   */
  protected async publicPost<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.publicRequest<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * Public PATCH request (no authentication)
   */
  protected async publicPatch<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.publicRequest<T>(endpoint, {
      method: 'PATCH',
      body: data ? JSON.stringify(data) : undefined,
    });
  }
}
