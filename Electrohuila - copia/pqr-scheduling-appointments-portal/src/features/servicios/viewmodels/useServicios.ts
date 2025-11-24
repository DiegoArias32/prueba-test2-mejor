/**
 * ViewModel - Servicios
 * Lógica de negocio para la página de servicios
 * Siguiendo el patrón MVVM - usa Repository en lugar de acceso directo a datos
 */

import { useState, useCallback, useEffect, useRef } from 'react';
import { Service } from '../models/servicios.models';
import { serviciosRepository } from '../repositories/servicios.repository';

/**
 * Estado del ViewModel de servicios
 */
export interface ServiciosViewModelState {
  selectedService: string;
  isOpen: boolean;
  services: Service[];
  selectedServiceData: Service | undefined;
}

/**
 * Acciones disponibles en el ViewModel
 */
export interface ServiciosViewModelActions {
  handleServiceSelect: (service: Service) => void;
  toggleDropdown: () => void;
  setIsOpen: (isOpen: boolean) => void;
  dropdownRef: React.RefObject<HTMLDivElement | null>;
}

/**
 * Hook personalizado que implementa el ViewModel para servicios
 */
export const useServicios = (): ServiciosViewModelState & ServiciosViewModelActions => {
  const [selectedService, setSelectedService] = useState<string>('');
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [services, setServices] = useState<Service[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  /**
   * Carga los servicios desde el repository
   */
  useEffect(() => {
    const loadServices = async () => {
      try {
        setLoading(true);
        const data = await serviciosRepository.getAllServices();
        setServices(data);
      } catch (error) {
        console.error('Error loading services:', error);
        setServices([]);
      } finally {
        setLoading(false);
      }
    };

    loadServices();
  }, []);

  /**
   * Maneja la selección de un servicio
   */
  const handleServiceSelect = useCallback((service: Service) => {
    setSelectedService(service.name);
    setIsOpen(false);
  }, []);

  /**
   * Alterna el estado del dropdown
   */
  const toggleDropdown = useCallback(() => {
    setIsOpen(prev => !prev);
  }, []);

  /**
   * Datos del servicio seleccionado (calculado desde el estado)
   */
  const selectedServiceData = services.find(s => s.name === selectedService);

  // Cerrar dropdown cuando se hace clic fuera o se presiona Escape
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    const handleEscapeKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    document.addEventListener('keydown', handleEscapeKey);

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
      document.removeEventListener('keydown', handleEscapeKey);
    };
  }, []);

  return {
    // Estado
    selectedService,
    isOpen,
    services,
    selectedServiceData,
    // Acciones
    handleServiceSelect,
    toggleDropdown,
    setIsOpen,
    dropdownRef
  };
};
