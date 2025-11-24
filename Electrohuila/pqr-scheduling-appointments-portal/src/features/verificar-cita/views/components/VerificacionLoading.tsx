/**
 * @file VerificacionLoading.tsx
 * @description Componente de estado de carga para verificación de citas
 * @module features/verificar-cita/presentation/components
 */

import React from 'react';
import { motion } from 'framer-motion';

/**
 * Componente que muestra el estado de carga durante la verificación
 * @component
 * @returns {JSX.Element} UI de carga
 */
export const VerificacionLoading: React.FC = () => {
  return (
    <motion.div
      className="flex justify-center items-center py-16"
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.3 }}
    >
      <motion.div
        className="bg-white rounded-2xl shadow-xl p-8 text-center"
        initial={{ opacity: 0, scale: 0.9 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ delay: 0.2, type: "spring", stiffness: 200 }}
      >
        <motion.div
          className="w-16 h-16 border-4 border-[#56C2E1] border-t-transparent rounded-full mx-auto mb-4"
          animate={{ rotate: 360 }}
          transition={{
            duration: 1,
            repeat: Infinity,
            ease: "linear"
          }}
        />
        <motion.p
          className="text-gray-600 text-lg"
          animate={{ opacity: [0.6, 1, 0.6] }}
          transition={{
            duration: 2,
            repeat: Infinity,
            ease: "easeInOut"
          }}
        >
          Verificando cita...
        </motion.p>
      </motion.div>
    </motion.div>
  );
};
