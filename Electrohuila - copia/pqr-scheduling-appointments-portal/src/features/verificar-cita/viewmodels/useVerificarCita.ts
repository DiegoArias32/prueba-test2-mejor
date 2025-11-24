/**
 * @file VerificarCitaViewModel.ts
 * @description ViewModel para la verificación de citas por QR
 * @module features/verificar-cita/presentation/viewmodels
 */

import { useState, useCallback } from 'react';
import { VerificacionCita } from '../models/verificar-cita.models';
import { VerificarCitaRepository } from '../repositories/verificar-cita.repository';

/**
 * Estado del ViewModel de verificación de cita
 * @interface VerificarCitaViewModelState
 * @property {VerificacionCita | null} verificacion - Datos de la verificación
 * @property {boolean} loading - Indica si está cargando
 * @property {string} error - Mensaje de error si existe
 */
export interface VerificarCitaViewModelState {
  verificacion: VerificacionCita | null;
  loading: boolean;
  error: string;
}

/**
 * Acciones disponibles en el ViewModel de verificación de cita
 * @interface VerificarCitaViewModelActions
 * @property {Function} verificarCita - Verifica una cita por QR
 */
export interface VerificarCitaViewModelActions {
  verificarCita: (appointmentNumber: string, clientNumber: string) => Promise<void>;
}

/**
 * Hook personalizado que implementa el ViewModel para verificación de citas
 * Maneja la lógica de negocio y el estado de la verificación de citas por QR
 * @param {VerificarCitaRepository} repository - Repository de verificación de citas
 * @returns {VerificarCitaViewModelState & VerificarCitaViewModelActions} Estado y acciones del ViewModel
 */
export const useVerificarCitaViewModel = (
  repository: VerificarCitaRepository
): VerificarCitaViewModelState & VerificarCitaViewModelActions => {
  const [verificacion, setVerificacion] = useState<VerificacionCita | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string>('');

  /**
   * Verifica una cita mediante su número y número de cliente
   * @param {string} appointmentNumber - Número de la cita
   * @param {string} clientNumber - Número del cliente
   */
  const verificarCita = useCallback(async (appointmentNumber: string, clientNumber: string) => {
    try {
      setLoading(true);
      setError('');

      const data = await repository.verifyAppointmentByQR(appointmentNumber, clientNumber);
      setVerificacion(data);
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Error al verificar la cita';
      setError(errorMessage);
      setVerificacion(null);
    } finally {
      setLoading(false);
    }
  }, [repository]);

  return {
    // Estado
    verificacion,
    loading,
    error,
    // Acciones
    verificarCita
  };
};

// Alias para compatibilidad
export const useVerificarCita = useVerificarCitaViewModel;
