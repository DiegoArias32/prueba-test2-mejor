/**
 * @file VerificarCitaPage.tsx
 * @description Página principal para verificación de citas por QR (Arquitectura MVVM)
 * @module features/verificar-cita/presentation/pages
 */

'use client';

import React, { useEffect } from 'react';
import { useSearchParams } from 'next/navigation';
import { useVerificarCita } from '../viewmodels/useVerificarCita';
import { verificarCitaRepository } from '../repositories/verificar-cita.repository';
import {
  VerificacionHeader,
  VerificacionLoading,
  VerificacionError,
  VerificacionDetalle
} from './components';
import { FixedHeader } from '@/shared/layouts';
import { Footer } from '@/shared/components';

/**
 * Componente interno que usa useSearchParams
 * Maneja la lógica de verificación de citas por QR
 * @component
 * @returns {JSX.Element} Contenido de la página de verificación
 */
export const VerificarCitaContent: React.FC = () => {
  const searchParams = useSearchParams();

  // ViewModel - maneja la lógica de negocio y estado
  const {
    verificacion,
    loading,
    error,
    verificarCita
  } = useVerificarCita(verificarCitaRepository);

  // Efecto para verificar la cita al cargar
  useEffect(() => {
    const numero = searchParams.get('numero');
    const cliente = searchParams.get('cliente');

    if (!numero || !cliente) {
      // El ViewModel manejará el error
      return;
    }

    verificarCita(numero, cliente);
  }, [searchParams, verificarCita]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 overflow-x-hidden">
      {/* Header */}
      <FixedHeader />

      {/* Main Content */}
      <main className="max-w-4xl mx-auto px-4 py-8 pt-24">
        {/* Page Header */}
        <VerificacionHeader />

        {/* Loading State */}
        {loading && <VerificacionLoading />}

        {/* Error State */}
        {error && !loading && <VerificacionError error={error} />}

        {/* Success State */}
        {verificacion && !loading && <VerificacionDetalle verificacion={verificacion} />}
      </main>

      {/* Footer */}
      <Footer />
    </div>
  );
};
