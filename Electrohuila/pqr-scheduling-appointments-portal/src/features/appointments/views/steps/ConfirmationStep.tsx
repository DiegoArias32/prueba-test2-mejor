/**
 * Step 3: Confirmation
 * Shows appointment confirmation with QR code
 */

'use client';

import { FC, useState, useEffect } from 'react';
import Link from 'next/link';
import { motion, AnimatePresence } from 'framer-motion';
import { FiCheckCircle, FiUser, FiCalendar, FiAlertCircle, FiPrinter, FiPlus, FiClock, FiMapPin } from 'react-icons/fi';
import { FixedHeader } from '@/shared/layouts';
import { AnimatedCard } from '@/shared/components/ui/AnimatedCard';
import { AnimatedButton } from '@/shared/components/ui/AnimatedButton';
import { AnimatedAlert } from '@/shared/components/ui/AnimatedAlert';

interface ClientData {
  fullName: string;
  clientNumber: string;
  email: string;
  phone?: string;
  mobile?: string;
}

interface FormData {
  reason: string;
  branch: string;
  appointmentDate: string;
  appointmentTime: string;
  observations: string;
}

interface AppointmentConfirmation {
  ticketNumber: string;
  issueDateTime: string;
  clientData: ClientData;
  formData: FormData;
}

interface ConfirmationStepProps {
  appointmentData: AppointmentConfirmation;
  qrCodeDataURL: string | null;
  onPrint: () => void;
  onNewRequest: () => void;
}

export const ConfirmationStep: FC<ConfirmationStepProps> = ({
  appointmentData,
  qrCodeDataURL,
  onPrint,
  onNewRequest
}) => {
  const [showWarning, setShowWarning] = useState(true);
  const [showConfetti, setShowConfetti] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => setShowWarning(false), 7000);
    const confettiTimer = setTimeout(() => setShowConfetti(false), 3000);
    return () => {
      clearTimeout(timer);
      clearTimeout(confettiTimer);
    };
  }, []);

  return (
    <>
      <FixedHeader className="w-full print:hidden" />

      <div className="min-h-screen relative overflow-hidden pt-32 pb-8 px-4 print:pt-4 print:min-h-0">
        {/* Celebration Confetti Effect */}
        <AnimatePresence>
          {showConfetti && (
            <>
              {[...Array(30)].map((_, i) => (
                <motion.div
                  key={i}
                  className="absolute w-3 h-3 rounded-full"
                  style={{
                    left: `${50 + (Math.random() - 0.5) * 30}%`,
                    top: '20%',
                    backgroundColor: ['#1797D5', '#56C2E1', '#97D4E3', '#FFD700'][Math.floor(Math.random() * 4)]
                  }}
                  initial={{ y: 0, opacity: 1, scale: 0 }}
                  animate={{
                    y: [0, Math.random() * 300 + 200],
                    x: [(Math.random() - 0.5) * 200],
                    opacity: [1, 0],
                    scale: [0, 1, 0.5],
                    rotate: [0, Math.random() * 360]
                  }}
                  transition={{
                    duration: 2 + Math.random(),
                    ease: "easeOut"
                  }}
                />
              ))}
            </>
          )}
        </AnimatePresence>

        <div className="max-w-3xl mx-auto relative z-10 print:max-w-full">
          <AnimatedCard delay={0.2} hover={false}>
            <div className="p-8 print:shadow-none print:p-4 print:rounded-none">
              {/* Success Header */}
              <motion.div
                className="text-center mb-6 print:mb-4"
                initial={{ opacity: 0, scale: 0.8 }}
                animate={{ opacity: 1, scale: 1 }}
                transition={{ duration: 0.5 }}
              >
                <motion.div
                  className="inline-flex items-center justify-center w-24 h-24 bg-gradient-to-br from-green-100 to-emerald-100 rounded-full mb-4 print:w-16 print:h-16 print:mb-2 relative group"
                  animate={{
                    scale: [1, 1.1, 1],
                    rotate: [0, 10, -10, 0]
                  }}
                  transition={{
                    duration: 0.6,
                    repeat: 2
                  }}
                >
                  {/* Glow effect */}
                  <motion.div
                    className="absolute inset-0 bg-gradient-to-br from-green-100 to-emerald-100 rounded-full blur-lg opacity-0 group-hover:opacity-60 transition-opacity duration-300"
                    style={{ pointerEvents: 'none' }}
                  />
                  <FiCheckCircle className="text-green-600 text-6xl print:text-4xl relative z-10" />
                </motion.div>

                <motion.h1
                  className="text-4xl font-bold text-[#203461] mb-2 print:text-2xl print:mb-1"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.3 }}
                >
                  Cita Agendada
                  <motion.span
                    className="bg-gradient-to-r from-green-500 to-emerald-500 bg-clip-text text-transparent block"
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
                    Exitosamente
                  </motion.span>
                </motion.h1>

                <motion.p
                  className="text-lg text-gray-600 print:text-sm"
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  transition={{ delay: 0.5 }}
                >
                  Su cita ha sido registrada correctamente
                </motion.p>
              </motion.div>

              {/* Warning Message */}
              <AnimatePresence>
                {showWarning && (
                  <motion.div
                    initial={{ opacity: 0, y: -10 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, height: 0 }}
                    className="mb-6 print:hidden"
                  >
                    <AnimatedAlert
                      type="info"
                      message="Presente este comprobante el día de su cita. Puede imprimirlo o mostrar el código QR desde su dispositivo móvil."
                      autoDismiss={true}
                      duration={7000}
                    />
                  </motion.div>
                )}
              </AnimatePresence>

              {/* Ticket/Receipt */}
              <motion.div
                className="border-2 border-dashed border-gray-300 rounded-lg p-6 mb-6 print:p-3 print:mb-3 print:border print:rounded"
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.7 }}
              >
                {/* Ticket Number */}
                <motion.div
                  className="text-center mb-6 pb-6 border-b-2 border-dashed border-gray-200 print:mb-3 print:pb-3 print:border-b"
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  transition={{ delay: 0.8 }}
                >
                  <p className="text-gray-600 text-sm mb-1 print:text-xs">Número de Cita</p>
                  <motion.p
                    className="text-5xl font-bold text-[#1797D5] print:text-3xl"
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    transition={{ type: "spring", stiffness: 200, delay: 0.9 }}
                  >
                    {appointmentData.ticketNumber}
                  </motion.p>
                  <p className="text-gray-500 text-xs mt-2 print:mt-1">
                    Fecha de emisión: {appointmentData.issueDateTime}
                  </p>
                </motion.div>

                {/* Client Info */}
                <motion.div
                  className="mb-6 pb-6 border-b-2 border-dashed border-gray-200 print:mb-3 print:pb-3 print:border-b"
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 1 }}
                >
                  <h3 className="font-semibold text-[#203461] mb-3 flex items-center print:text-sm print:mb-2">
                    <FiUser className="mr-2 text-[#1797D5]" />
                    Datos del Cliente
                  </h3>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm print:text-xs print:gap-2">
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 1.1 }}
                    >
                      <span className="text-gray-600">Nombre:</span>
                      <p className="font-semibold">{appointmentData.clientData.fullName}</p>
                    </motion.div>
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 1.2 }}
                    >
                      <span className="text-gray-600">Cliente:</span>
                      <p className="font-semibold">{appointmentData.clientData.clientNumber}</p>
                    </motion.div>
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 1.3 }}
                    >
                      <span className="text-gray-600">Dirección:</span>
                      <p className="font-semibold">{appointmentData.clientData.email}</p>
                    </motion.div>
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 1.4 }}
                    >
                      <span className="text-gray-600">Teléfono:</span>
                      <p className="font-semibold">
                        {appointmentData.clientData.phone || appointmentData.clientData.mobile || 'N/A'}
                      </p>
                    </motion.div>
                  </div>
                </motion.div>

                {/* Appointment Details */}
                <motion.div
                  className="mb-6 pb-6 border-b-2 border-dashed border-gray-200 print:mb-3 print:pb-3 print:border-b"
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 1.5 }}
                >
                  <h3 className="font-semibold text-[#203461] mb-3 flex items-center print:text-sm print:mb-2">
                    <FiCalendar className="mr-2 text-[#1797D5]" />
                    Detalles de la Cita
                  </h3>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm print:text-xs print:gap-2">
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 1.6 }}
                    >
                      <span className="text-gray-600">Motivo:</span>
                      <p className="font-semibold">{appointmentData.formData.reason}</p>
                    </motion.div>
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 1.7 }}
                    >
                      <span className="text-gray-600 flex items-center gap-1">
                        <FiMapPin className="inline" />
                        Sede:
                      </span>
                      <p className="font-semibold">{appointmentData.formData.branch}</p>
                    </motion.div>
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 1.8 }}
                    >
                      <span className="text-gray-600">Fecha:</span>
                      <p className="font-semibold">
                        {new Date(appointmentData.formData.appointmentDate).toLocaleDateString('es-CO', {
                          weekday: 'long',
                          year: 'numeric',
                          month: 'long',
                          day: 'numeric'
                        })}
                      </p>
                    </motion.div>
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 1.9 }}
                    >
                      <span className="text-gray-600 flex items-center gap-1">
                        <FiClock className="inline" />
                        Hora:
                      </span>
                      <p className="font-semibold">{appointmentData.formData.appointmentTime}</p>
                    </motion.div>
                  </div>
                  {appointmentData.formData.observations && (
                    <motion.div
                      className="mt-3 print:mt-2"
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 2 }}
                    >
                      <span className="text-gray-600">Observaciones:</span>
                      <p className="font-semibold mt-1">{appointmentData.formData.observations}</p>
                    </motion.div>
                  )}
                </motion.div>

                {/* QR Code */}
                {qrCodeDataURL && (
                  <motion.div
                    className="text-center"
                    initial={{ opacity: 0, scale: 0.8 }}
                    animate={{ opacity: 1, scale: 1 }}
                    transition={{ delay: 2.1, type: "spring" }}
                  >
                    <p className="text-gray-600 text-sm mb-3 print:text-xs print:mb-2">
                      Código QR para verificación
                    </p>
                    <motion.div
                      className="inline-block p-4 bg-white border-2 border-gray-200 rounded-lg print:p-2 print:border"
                      whileHover={{ scale: 1.05, rotate: [0, -2, 2, 0] }}
                      transition={{ duration: 0.3 }}
                    >
                      <img
                        src={qrCodeDataURL}
                        alt="QR Code"
                        className="w-48 h-48 mx-auto print:w-32 print:h-32"
                      />
                    </motion.div>
                    <p className="text-gray-500 text-xs mt-2 print:mt-1">
                      Escanee este código al momento de su cita
                    </p>
                  </motion.div>
                )}
              </motion.div>

              {/* Action Buttons */}
              <motion.div
                className="flex flex-col sm:flex-row gap-4 print:hidden mt-8 justify-center"
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 2.2, staggerChildren: 0.1 }}
              >
                <motion.div
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                >
                  <AnimatedButton
                    onClick={onPrint}
                    variant="primary"
                    size="lg"
                    icon={<FiPrinter />}
                    className="w-full sm:w-auto relative overflow-hidden group"
                  >
                    {/* Shimmer effect */}
                    <motion.div
                      className="absolute inset-0 bg-gradient-to-r from-transparent via-white/20 to-transparent"
                      initial={{ x: '-100%' }}
                      animate={{ x: '100%' }}
                      transition={{
                        duration: 2,
                        repeat: Infinity,
                        repeatDelay: 1
                      }}
                      style={{ pointerEvents: 'none' }}
                    />
                    <span className="relative z-10">Imprimir Comprobante</span>
                  </AnimatedButton>
                </motion.div>

                <motion.div
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                >
                  <AnimatedButton
                    onClick={onNewRequest}
                    variant="secondary"
                    size="lg"
                    icon={<FiPlus />}
                    className="w-full sm:w-auto"
                  >
                    Nueva Cita
                  </AnimatedButton>
                </motion.div>
              </motion.div>

              {/* Info Footer */}
              <motion.div
                className="mt-8 p-6 bg-gradient-to-r from-blue-50 to-cyan-50 rounded-2xl text-center print:hidden border border-blue-100 print:border-none"
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 2.4 }}
                whileHover={{ y: -4, boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)' }}
              >
                <motion.p
                  className="text-sm text-gray-600 mb-3"
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  transition={{ delay: 2.5 }}
                >
                  Para consultar o cancelar su cita, visite:
                </motion.p>

                <Link href="/gestion-citas">
                  <motion.div
                    className="text-[#1797D5] hover:text-[#203461] font-semibold transition-colors inline-flex items-center gap-2 cursor-pointer"
                    whileHover={{ x: 5 }}
                  >
                    <FiCalendar className="text-lg" />
                    <span>Gestión de Citas</span>
                  </motion.div>
                </Link>
              </motion.div>
            </div>
          </AnimatedCard>
        </div>
      </div>
    </>
  );
};
