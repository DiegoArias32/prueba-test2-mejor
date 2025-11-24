/**
 * Step 1: Client Number Validation
 * Validates client number and retrieves client data
 */

'use client';

import { FC } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { FiUser, FiRefreshCw, FiUsers, FiCalendar, FiAlertCircle, FiHelpCircle, FiPhone, FiUserPlus } from 'react-icons/fi';
import { FixedHeader } from '@/shared/layouts';
import { BackNavigation } from '@/shared/components';
import { AnimatedCard } from '@/shared/components/ui/AnimatedCard';
import { AnimatedButton } from '@/shared/components/ui/AnimatedButton';
import { AnimatedInput } from '@/shared/components/ui/AnimatedInput';
import { AnimatedAlert } from '@/shared/components/ui/AnimatedAlert';
import { NewClientForm, NewClientData } from '../components/NewClientForm';

interface ClientValidationStepProps {
  clientNumber: string;
  setClientNumber: (value: string) => void;
  validateClient: () => Promise<void>;
  isLoading: boolean;
  error: string | null;
  isNewClient: boolean;
  onNewClientClick: () => void;
  onBackToClientNumber: () => void;
  onNewClientDataChange: (data: NewClientData, isValid: boolean) => void;
  onContinueAsNewClient: () => void;
  isNewClientFormValid: boolean;
}

export const ClientValidationStep: FC<ClientValidationStepProps> = ({
  clientNumber,
  setClientNumber,
  validateClient,
  isLoading,
  error,
  isNewClient,
  onNewClientClick,
  onBackToClientNumber,
  onNewClientDataChange,
  onContinueAsNewClient,
  isNewClientFormValid
}) => {
  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter' && !isNewClient) {
      validateClient();
    }
  };

  return (
    <>
      <FixedHeader className="w-full" />

      {/* Content */}
      <div className="min-h-screen relative pt-24 pb-16 px-4">
        <div className="max-w-4xl mx-auto relative z-10">
          <BackNavigation backTo="/servicios" />
        </div>

        <div className="max-w-md mx-auto relative z-10">
          {/* Page Header */}
          <motion.div
            className="text-center mb-8"
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6 }}
          >
            <motion.div
              className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-br from-[#97D4E3] to-[#56C2E1] rounded-2xl mb-4 shadow-lg relative group"
              whileHover={{ scale: 1.05, rotate: [0, -5, 5, 0] }}
              transition={{ duration: 0.5 }}
            >
              {/* Glow effect on hover */}
              <motion.div
                className="absolute inset-0 bg-gradient-to-br from-[#97D4E3] to-[#56C2E1] rounded-2xl blur-lg opacity-0 group-hover:opacity-50 transition-opacity duration-300"
                style={{ pointerEvents: 'none' }}
              />
              <FiCalendar className="text-[#203461] text-4xl relative z-10" />
            </motion.div>

            <motion.h1
              className="text-4xl font-bold text-[#203461] mb-2"
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.1 }}
            >
              Agendamiento de
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

            <motion.p
              className="text-lg text-gray-600"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.6, delay: 0.2 }}
            >
              Ingrese su número de cliente para continuar
            </motion.p>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.6, delay: 0.3 }}
          >
            <AnimatedCard delay={0.2} hover={false}>
              <div className="p-8">
                {/* Error Message with Registration Option */}
                <AnimatePresence>
                  {error && !isNewClient && (
                    <motion.div
                      initial={{ opacity: 0, y: -10 }}
                      animate={{ opacity: 1, y: 0 }}
                      exit={{ opacity: 0, y: -10 }}
                    >
                      <AnimatedAlert
                        type="warning"
                        message={error}
                        onClose={() => {}}
                      />
                    </motion.div>
                  )}
                </AnimatePresence>

                {error && !isNewClient && (
                  <motion.div
                    initial={{ opacity: 0, y: -10 }}
                    animate={{ opacity: 1, y: 0 }}
                    className="mb-6"
                  >
                    <motion.button
                      onClick={onNewClientClick}
                      whileHover={{ scale: 1.02, y: -2 }}
                      whileTap={{ scale: 0.98 }}
                      className="w-full p-4 bg-gradient-to-r from-blue-50 to-cyan-50 border-2 border-[#1797D5] rounded-xl flex items-center justify-center gap-2 text-[#1797D5] font-semibold hover:border-[#56C2E1] hover:shadow-lg transition-all duration-300"
                    >
                      <motion.div
                        animate={{ rotate: [0, 10, 0] }}
                        transition={{ duration: 2, repeat: Infinity }}
                      >
                        <FiUserPlus className="text-xl" />
                      </motion.div>
                      ¿No tienes número de cliente? Regístrate aquí
                    </motion.button>
                  </motion.div>
                )}

                {/* Client Number Input */}
                {!isNewClient && (
                  <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: 0.3, staggerChildren: 0.1 }}
                  >
                    <AnimatedInput
                      label="Número de Cliente"
                      value={clientNumber}
                      onChange={setClientNumber}
                      onKeyDown={handleKeyPress}
                      type="text"
                      placeholder="Ej: 1234567890"
                      required
                      disabled={isLoading}
                      icon={<FiUser className="text-gray-400" />}
                      className="mb-6"
                    />

                    <motion.div
                      whileHover={{ scale: 1.01 }}
                    >
                      <AnimatedButton
                        onClick={validateClient}
                        disabled={isLoading || !clientNumber}
                        loading={isLoading}
                        fullWidth
                        size="lg"
                        icon={<FiUsers />}
                      >
                        Buscar Cliente
                      </AnimatedButton>
                    </motion.div>
                  </motion.div>
                )}

                {/* New Client Form */}
                {isNewClient && (
                  <motion.div
                    initial={{ opacity: 0, x: 20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ duration: 0.4 }}
                  >
                    <NewClientForm onDataChange={onNewClientDataChange} />

                    <div className="mt-6 space-y-3">
                      <motion.div
                        whileHover={{ scale: 1.01 }}
                      >
                        <AnimatedButton
                          onClick={onContinueAsNewClient}
                          disabled={!isNewClientFormValid}
                          fullWidth
                          size="lg"
                          icon={<FiCalendar />}
                        >
                          Continuar con agendamiento
                        </AnimatedButton>
                      </motion.div>

                      <motion.button
                        onClick={onBackToClientNumber}
                        whileHover={{ x: -5, color: '#1797D5' }}
                        className="w-full text-gray-600 hover:text-gray-800 font-semibold flex items-center justify-center transition-colors py-2"
                      >
                        ← Volver a ingresar número de cliente
                      </motion.button>
                    </div>
                  </motion.div>
                )}
              </div>
            </AnimatedCard>
          </motion.div>

          {/* Help Section */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.5 }}
          >
            <motion.div
              whileHover={{ y: -5, shadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)' }}
              transition={{ duration: 0.3 }}
              className="mt-6"
            >
              <AnimatedCard delay={0.6} hover={false}>
                <div className="p-6 bg-gradient-to-r from-blue-50 to-cyan-50">
                  <div className="flex items-start gap-3">
                    <motion.div
                      whileHover={{ rotate: 360 }}
                      transition={{ duration: 0.5 }}
                    >
                      <FiHelpCircle className="text-[#1797D5] text-2xl flex-shrink-0 mt-1" />
                    </motion.div>
                    <div>
                      <motion.h3
                        className="font-semibold text-[#203461] mb-2"
                        initial={{ opacity: 0, x: -10 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: 0.7 }}
                      >
                        ¿No conoce su número de cliente?
                      </motion.h3>

                      <motion.p
                        className="text-sm text-gray-600 mb-3"
                        initial={{ opacity: 0, x: -10 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: 0.8 }}
                      >
                        Puede encontrarlo en su factura de energía o comunicarse con nosotros.
                      </motion.p>

                      <motion.div
                        className="flex items-center gap-2 text-sm text-[#1797D5] font-medium"
                        whileHover={{ x: 5 }}
                        initial={{ opacity: 0, x: -10 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: 0.9 }}
                      >
                        <FiPhone />
                        <span>Línea de atención: 018000 111 888</span>
                      </motion.div>
                    </div>
                  </div>
                </div>
              </AnimatedCard>
            </motion.div>
          </motion.div>
        </div>
      </div>
    </>
  );
};
