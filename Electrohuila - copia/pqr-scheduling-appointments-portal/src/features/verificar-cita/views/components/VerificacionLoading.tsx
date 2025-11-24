/**
 * @file VerificacionLoading.tsx
 * @description Componente de estado de carga para verificación de citas
 * @module features/verificar-cita/presentation/components
 */

import React from 'react';

/**
 * Componente que muestra el estado de carga durante la verificación
 * @component
 * @returns {JSX.Element} UI de carga
 */
export const VerificacionLoading: React.FC = () => {
  return (
    <div className="flex justify-center items-center py-16">
      <div className="bg-white rounded-2xl shadow-xl p-8 text-center">
        <div className="animate-spin w-16 h-16 border-4 border-[#56C2E1] border-t-transparent rounded-full mx-auto mb-4"></div>
        <p className="text-gray-600 text-lg">Verificando cita...</p>
      </div>
    </div>
  );
};
