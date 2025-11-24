/**
 * @file VerificacionHeader.tsx
 * @description Componente de encabezado para la página de verificación
 * @module features/verificar-cita/presentation/components
 */

import React from 'react';

/**
 * Componente que muestra el encabezado de la página de verificación
 * @component
 * @returns {JSX.Element} UI del encabezado
 */
export const VerificacionHeader: React.FC = () => {
  return (
    <div className="text-center mb-12">
      <div className="inline-flex items-center px-4 py-2 bg-white/70 rounded-full text-[#1A6192] text-sm font-medium mb-6 shadow-sm">
        <span className="w-2 h-2 bg-[#56C2E1] rounded-full mr-2 animate-pulse"></span>
        Verificación de Cita
      </div>
      <h1 className="text-5xl font-bold text-[#203461] mb-4">
        Verificar
        <span className="bg-gradient-to-r from-[#1797D5] to-[#56C2E1] bg-clip-text text-transparent"> Cita</span>
      </h1>
      <h2 className="text-2xl font-semibold text-[#1A6192] mb-6">ElectroHuila</h2>
      <p className="text-xl text-gray-600 max-w-3xl mx-auto">
        Verificación automática de la autenticidad de su cita
      </p>
    </div>
  );
};
