/**
 * View - Servicios
 * Vista principal para la página de servicios con animaciones modernas
 */

'use client';

import Link from 'next/link';
import { motion, AnimatePresence } from 'framer-motion';
import { Info, ChevronDown, ChevronRight, CheckCircle2 } from 'lucide-react';
import { FixedHeader } from '@/shared/layouts';
import { BackNavigation, Footer } from '@/shared/components';
import { useServicios } from '../viewmodels/useServicios';
import { ServiceIcon } from './components';

export const ServiciosView: React.FC = () => {
  const {
    selectedService,
    isOpen,
    services,
    selectedServiceData,
    handleServiceSelect,
    toggleDropdown,
    dropdownRef
  } = useServicios();

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-gray-50 overflow-hidden relative">
      {/* Animated Background Blobs */}
      <motion.div
        className="absolute top-20 left-10 w-72 h-72 bg-[#1797D5] rounded-full blur-3xl opacity-20"
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
        className="absolute bottom-20 right-10 w-96 h-96 bg-[#56C2E1] rounded-full blur-3xl opacity-20"
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
      <main className="max-w-4xl mx-auto px-4 py-12 pt-24 relative z-10">
        {/* Back Navigation */}
        <motion.div
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.5 }}
        >
          <BackNavigation backTo="/" />
        </motion.div>

        {/* Page Header */}
        <motion.div
          className="text-center mb-12"
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6, delay: 0.2 }}
        >
          <motion.h1
            className="text-4xl md:text-5xl font-bold text-[#203461] mb-4"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.3 }}
          >
            Sistema de
            <motion.span
              className="bg-gradient-to-r from-[#1797D5] to-[#56C2E1] bg-clip-text text-transparent block"
              animate={{
                backgroundPosition: ['0% 50%', '100% 50%', '0% 50%']
              }}
              transition={{
                duration: 5,
                repeat: Infinity,
                ease: "linear"
              }}
              style={{ backgroundSize: '200% auto' }}
            >
              Citas
            </motion.span>
          </motion.h1>

          <motion.h2
            className="text-2xl font-semibold text-[#203461] mb-8"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.4 }}
          >
            ElectroHuila
          </motion.h2>
        </motion.div>

        {/* Service Selection Card */}
        <motion.div
          className="bg-white rounded-3xl shadow-2xl border border-gray-100 p-8 mb-8 mx-auto max-w-md relative z-50"
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.6, delay: 0.5 }}
          whileHover={{ y: -5 }}
        >
          {/* Icon and Title */}
          <motion.div
            className="text-center mb-8"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.6 }}
          >
            <motion.div
              className="w-24 h-24 mx-auto bg-gradient-to-br from-[#97D4E3] to-[#56C2E1] rounded-2xl flex items-center justify-center shadow-lg mb-6"
              whileHover={{ scale: 1.1, rotate: [0, -5, 5, 0] }}
              transition={{ duration: 0.5 }}
            >
              <Info className="w-12 h-12 text-[#203461]" strokeWidth={2.5} />
            </motion.div>

            <h3 className="text-3xl font-bold text-[#203461] mb-3">
              Seleccione el Tipo de cita
            </h3>

            <p className="text-gray-600 leading-relaxed">
              Elija el tipo de atención o cita que mejor se ajuste a su necesidad para continuar con el proceso.
            </p>
          </motion.div>

          {/* Info Alert */}
          <motion.div
            className="bg-blue-50 border border-blue-200 rounded-xl p-4 mb-6 flex items-center"
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.7 }}
            whileHover={{ scale: 1.02 }}
          >
            <Info className="w-5 h-5 text-blue-500 mr-3 flex-shrink-0" />
            <span className="text-blue-700 font-medium text-sm">
              Seleccione el tipo de servicio que requiere
            </span>
          </motion.div>

          {/* Dropdown */}
          <motion.div
            className="relative mb-6 z-50"
            ref={dropdownRef}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.8 }}
          >
            <label className="block text-sm font-semibold text-[#203461] mb-2">
              Tipo de Servicio <span className="text-red-500">*</span>
            </label>

            <motion.button
              onClick={toggleDropdown}
              whileHover={{ scale: 1.01 }}
              whileTap={{ scale: 0.99 }}
              className="w-full bg-white border-2 border-gray-300 rounded-xl px-4 py-3 text-left focus:outline-none focus:border-[#1797D5] transition-all duration-200 hover:border-[#56C2E1] hover:shadow-lg"
            >
              <div className="flex items-center justify-between">
                <span className={`${selectedService ? 'text-gray-900 font-medium' : 'text-gray-500'}`}>
                  {selectedService || 'Seleccione un servicio...'}
                </span>
                <motion.div
                  animate={{ rotate: isOpen ? 180 : 0 }}
                  transition={{ duration: 0.3 }}
                >
                  <ChevronDown className="w-5 h-5 text-gray-400" />
                </motion.div>
              </div>
            </motion.button>

            {/* Dropdown Menu */}
            <AnimatePresence>
              {isOpen && (
                <motion.div
                  initial={{ opacity: 0, y: -10, scale: 0.95 }}
                  animate={{ opacity: 1, y: 0, scale: 1 }}
                  exit={{ opacity: 0, y: -10, scale: 0.95 }}
                  transition={{ duration: 0.2 }}
                  className="absolute z-50 w-full mt-2 bg-white border border-gray-200 rounded-xl shadow-2xl overflow-hidden"
                >
                  <div className="py-1">
                    <div className="px-4 py-3 bg-gradient-to-r from-[#1797D5] to-[#56C2E1] text-white font-semibold">
                      Seleccione un servicio...
                    </div>

                    {services.map((service, index) => (
                      <motion.button
                        key={service.id}
                        onClick={() => handleServiceSelect(service)}
                        initial={{ opacity: 0, x: -20 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: index * 0.1 }}
                        whileHover={{ backgroundColor: '#F3F4F6', x: 5 }}
                        className="w-full px-4 py-4 text-left focus:outline-none transition-all duration-150 flex items-center gap-3 border-b border-gray-100 last:border-b-0"
                      >
                        <motion.span
                          className="text-2xl"
                          whileHover={{ scale: 1.2, rotate: 10 }}
                          transition={{ duration: 0.2 }}
                        >
                          <ServiceIcon serviceId={service.id} />
                        </motion.span>
                        <span className="text-gray-900 font-medium flex-1">{service.name}</span>
                        {selectedService === service.name && (
                          <motion.div
                            initial={{ scale: 0 }}
                            animate={{ scale: 1 }}
                            transition={{ type: "spring", stiffness: 500 }}
                          >
                            <CheckCircle2 className="w-5 h-5 text-green-500" />
                          </motion.div>
                        )}
                      </motion.button>
                    ))}
                  </div>
                </motion.div>
              )}
            </AnimatePresence>
          </motion.div>

          {/* Continue Button */}
          <AnimatePresence>
            {selectedService && selectedServiceData && (
              <motion.div
                className="text-center"
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: 20 }}
                transition={{ duration: 0.3 }}
              >
                <Link href={selectedServiceData.href}>
                  <motion.div
                    className="inline-flex items-center justify-center bg-gradient-to-r from-[#203461] to-[#1797D5] text-white px-8 py-4 rounded-xl font-semibold shadow-lg cursor-pointer overflow-hidden relative group w-full"
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                  >
                    {/* Shimmer Effect */}
                    <motion.div
                      className="absolute inset-0 bg-gradient-to-r from-transparent via-white/20 to-transparent"
                      initial={{ x: '-100%' }}
                      animate={{ x: '100%' }}
                      transition={{
                        duration: 2,
                        repeat: Infinity,
                        repeatDelay: 1
                      }}
                    />

                    <motion.span
                      className="relative z-10 mr-2"
                      animate={{ rotate: [0, 360] }}
                      transition={{
                        duration: 2,
                        repeat: Infinity,
                        ease: "linear"
                      }}
                    >
                      <ServiceIcon serviceId={selectedServiceData.id} />
                    </motion.span>

                    <span className="relative z-10">{selectedService}</span>

                    <motion.div
                      className="relative z-10 ml-2"
                      animate={{ x: [0, 5, 0] }}
                      transition={{
                        duration: 1.5,
                        repeat: Infinity,
                        ease: "easeInOut"
                      }}
                    >
                      <ChevronRight className="w-5 h-5" />
                    </motion.div>
                  </motion.div>
                </Link>
              </motion.div>
            )}
          </AnimatePresence>
        </motion.div>

        {/* Spacer para mantener la posición del footer */}
        <div className="h-24"></div>
      </main>

      {/* Footer */}
      <div className="relative z-10">
        <Footer />
      </div>
    </div>
  );
};
