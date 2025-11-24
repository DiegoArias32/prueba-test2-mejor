/**
 * @file VerificacionError.tsx
 * @description Componente de estado de error para verificación de citas
 * @module features/verificar-cita/presentation/components
 */

import React from 'react';
import Link from 'next/link';

/**
 * Props del componente VerificacionError
 * @interface VerificacionErrorProps
 * @property {string} error - Mensaje de error a mostrar
 */
interface VerificacionErrorProps {
  error: string;
}

/**
 * Componente que muestra el estado de error durante la verificación
 * @component
 * @param {VerificacionErrorProps} props - Props del componente
 * @returns {JSX.Element} UI de error
 */
export const VerificacionError: React.FC<VerificacionErrorProps> = ({ error }) => {
  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-2xl shadow-xl border border-red-200 overflow-hidden">
        <div className="bg-red-50 px-6 py-4 border-b border-red-200">
          <div className="flex items-center">
            <div className="w-12 h-12 bg-red-100 rounded-xl flex items-center justify-center mr-4">
              <svg className="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.082 16.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </div>
            <div>
              <h3 className="text-lg font-semibold text-red-800">Error de Verificación</h3>
              <p className="text-red-700">{error}</p>
            </div>
          </div>
        </div>
        <div className="p-6 text-center">
          <p className="text-gray-600 mb-6">
            No se pudo verificar la cita. Verifique que el código QR sea válido o contacte al servicio al cliente.
          </p>
          <div className="flex justify-center space-x-4">
            <Link
              href="/agendamiento-citas"
              className="bg-gradient-to-r from-[#203461] to-[#1797D5] text-white px-6 py-3 rounded-xl font-semibold hover:from-[#1A6192] hover:to-[#56C2E1] transition-all duration-300"
            >
              Agendar Nueva Cita
            </Link>
            <a
              href="tel:6088664600"
              className="bg-gray-500 text-white px-6 py-3 rounded-xl font-semibold hover:bg-gray-600 transition-all duration-300"
            >
              Llamar Soporte
            </a>
          </div>
        </div>
      </div>
    </div>
  );
};
