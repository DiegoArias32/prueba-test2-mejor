/**
 * Repository - Verificar Cita
 * Acceso a datos para verificación de citas
 */

import { VerificacionCita } from '../models/verificar-cita.models';
import { apiService } from '@/services/api';

/**
 * Repository para verificación de citas
 */
export class VerificarCitaRepository {
  /**
   * Verifica una cita mediante su número y número de cliente
   */
  async verifyAppointmentByQR(appointmentNumber: string, clientNumber: string): Promise<VerificacionCita> {
    try {
      const data = await apiService.verifyAppointmentByQR(appointmentNumber, clientNumber);
      return data;
    } catch (error) {
      throw error instanceof Error ? error : new Error('Error al verificar la cita');
    }
  }
}

/**
 * Instancia singleton del repository
 */
export const verificarCitaRepository = new VerificarCitaRepository();
