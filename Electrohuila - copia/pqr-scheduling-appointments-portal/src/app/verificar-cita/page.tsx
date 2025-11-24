/**
 * @file page.tsx
 * @description Página de verificación de citas por QR migrada a MVVM
 * @module app/verificar-cita
 */

'use client';

import { Suspense } from 'react';
import { VerificarCitaContent } from '@/features/verificar-cita';

/**
 * Componente principal de la página de verificación
 * Incluye Suspense para manejar la carga inicial
 * @component
 * @returns {JSX.Element} Página de verificación con Suspense
 */
export default function VerificarCitaPage() {
  return (
    <Suspense fallback={
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 flex items-center justify-center">
        <div className="bg-white rounded-2xl shadow-xl p-8 text-center">
          <div className="animate-spin w-16 h-16 border-4 border-[#56C2E1] border-t-transparent rounded-full mx-auto mb-4"></div>
          <p className="text-gray-600 text-lg">Cargando verificación...</p>
        </div>
      </div>
    }>
      <VerificarCitaContent />
    </Suspense>
  );
}