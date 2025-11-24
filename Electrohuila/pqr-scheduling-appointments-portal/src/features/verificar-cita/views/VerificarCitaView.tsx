/**
 * @file VerificarCitaPage.tsx
 * @description Página principal para verificación de citas por QR (Arquitectura MVVM)
 * @module features/verificar-cita/presentation/pages
 */

'use client';

import React, { useEffect } from 'react';
import { useSearchParams } from 'next/navigation';
import { motion } from 'framer-motion';
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
    <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-gray-50 overflow-hidden relative">
      {/* Animated Background Blobs */}
      <motion.div
        className="absolute top-20 left-10 w-72 h-72 bg-[#1797D5] rounded-full blur-3xl opacity-20 pointer-events-none"
        animate={{
          scale: [1, 1.2, 1],
          x: [0, 50, 0],
          y: [0, 30, 0]
        }}
        transition={{
          duration: 8,
          repeat: Infinity,
          ease: "easeInOut"
        }}
      />
      <motion.div
        className="absolute bottom-20 right-10 w-96 h-96 bg-[#56C2E1] rounded-full blur-3xl opacity-20 pointer-events-none"
        animate={{
          scale: [1, 1.3, 1],
          x: [0, -50, 0],
          y: [0, -30, 0]
        }}
        transition={{
          duration: 10,
          repeat: Infinity,
          ease: "easeInOut"
        }}
      />

      {/* Floating particles */}
      {[...Array(20)].map((_, i) => (
        <motion.div
          key={i}
          className="absolute w-2 h-2 bg-[#56C2E1] rounded-full pointer-events-none"
          style={{
            left: `${Math.random() * 100}%`,
            top: `${Math.random() * 100}%`,
          }}
          animate={{
            y: [0, -30, 0],
            opacity: [0.2, 0.5, 0.2]
          }}
          transition={{
            duration: 3 + Math.random() * 2,
            repeat: Infinity,
            delay: Math.random() * 2
          }}
        />
      ))}

      {/* Header */}
      <FixedHeader />

      {/* Main Content */}
      <main className="max-w-4xl mx-auto px-4 py-8 pt-24 relative z-10">
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
