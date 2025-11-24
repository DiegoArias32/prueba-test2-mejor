/**
 * Repository - Servicios
 * Capa de acceso a datos para servicios
 * Siguiendo el patrón Repository de MVVM
 */

import { Service, SERVICES } from '../models/servicios.models';

/**
 * Repository para gestión de servicios
 * Abstrae el acceso a datos de servicios (actualmente en memoria, pero podría ser API)
 */
export class ServiciosRepository {
  /**
   * Obtiene todos los servicios disponibles
   * @returns Promise con lista de servicios
   */
  async getAllServices(): Promise<Service[]> {
    // Simular delay de API para mantener consistencia con otros repositories
    return Promise.resolve(SERVICES);
  }

  /**
   * Obtiene un servicio por su ID
   * @param id - ID del servicio
   * @returns Promise con el servicio o undefined si no existe
   */
  async getServiceById(id: string): Promise<Service | undefined> {
    return Promise.resolve(SERVICES.find(s => s.id === id));
  }

  /**
   * Obtiene un servicio por su nombre
   * @param name - Nombre del servicio
   * @returns Promise con el servicio o undefined si no existe
   */
  async getServiceByName(name: string): Promise<Service | undefined> {
    return Promise.resolve(SERVICES.find(s => s.name === name));
  }

  /**
   * Busca servicios por texto (nombre o descripción)
   * @param searchTerm - Término de búsqueda
   * @returns Promise con lista de servicios que coinciden
   */
  async searchServices(searchTerm: string): Promise<Service[]> {
    const term = searchTerm.toLowerCase();
    const filtered = SERVICES.filter(
      s => s.name.toLowerCase().includes(term) ||
           s.description.toLowerCase().includes(term)
    );
    return Promise.resolve(filtered);
  }
}

/**
 * Instancia singleton del repository
 * Exportada para uso en ViewModels
 */
export const serviciosRepository = new ServiciosRepository();
