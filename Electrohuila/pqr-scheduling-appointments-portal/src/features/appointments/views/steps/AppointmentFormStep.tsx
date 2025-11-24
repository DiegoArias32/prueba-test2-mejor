/**
 * Step 2: Appointment Form
 * Form to fill appointment details and select time slot
 */

'use client';

import { FC, useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import {
  FiUser,
  FiCalendar,
  FiAlertCircle,
  FiArrowLeft,
  FiRefreshCw,
  FiCheckCircle,
  FiClock,
  FiMapPin
} from 'react-icons/fi';
import { FixedHeader } from '@/shared/layouts';
import { BackNavigation } from '@/shared/components';
import { AnimatedCard } from '@/shared/components/ui/AnimatedCard';
import { AnimatedButton } from '@/shared/components/ui/AnimatedButton';
import { AnimatedAlert } from '@/shared/components/ui/AnimatedAlert';
import { LoadingSpinner } from '@/shared/components/ui/LoadingSpinner';
import { BranchDto, AppointmentTypeDto } from '@/core/types/appointment.types';

interface ClientData {
  fullName: string;
  clientNumber: string;
}

interface FormData {
  documentType: string;
  reason: string;
  branch: string;
  appointmentDate: string;
  appointmentTime: string;
  observations: string;
}

interface AppointmentFormStepProps {
  clientData: ClientData;
  formData: FormData;
  branches: BranchDto[];
  appointmentTypes: AppointmentTypeDto[];
  availableHours: string[];
  loadingHours: boolean;
  isLoading: boolean;
  error: string | null;
  validationErrors: Record<string, string>;
  onFormChange: (field: string, value: string) => void;
  onSubmit: (e: React.FormEvent) => Promise<void>;
  onBack: () => void;
}

export const AppointmentFormStep: FC<AppointmentFormStepProps> = ({
  clientData,
  formData,
  branches,
  appointmentTypes,
  availableHours,
  loadingHours,
  isLoading,
  error,
  validationErrors,
  onFormChange,
  onSubmit,
  onBack
}) => {
  const [subStep, setSubStep] = useState<'datos' | 'horario'>('datos');
  const [showWarning, setShowWarning] = useState(true);

  useState(() => {
    const timer = setTimeout(() => setShowWarning(false), 6000);
    return () => clearTimeout(timer);
  });

  return (
    <div className="min-h-screen relative">
      <FixedHeader />

      {/* Navigation */}
      <div className="max-w-6xl mx-auto px-4 pt-24 relative z-10">
        <BackNavigation backTo="/" backText="Volver" />
      </div>

      {/* Main Content */}
      <main className="max-w-3xl mx-auto px-4 py-8 relative z-10">
        {/* Page Header */}
        <motion.div
          className="text-center mb-12 print:hidden"
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6 }}
        >
          <motion.h1
            className="text-5xl font-bold text-[#203461] mb-4"
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

          <motion.h2
            className="text-2xl font-semibold text-[#203461] mb-6"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 0.6, delay: 0.2 }}
          >
            ElectroHuila
          </motion.h2>

          <motion.p
            className="text-xl text-gray-600 max-w-3xl mx-auto"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6, delay: 0.3 }}
          >
            Complete la información para agendar su cita
          </motion.p>
        </motion.div>

        {/* Error Message */}
        <AnimatePresence>
          {error && (
            <motion.div
              initial={{ opacity: 0, y: -20 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -20 }}
              className="mb-6"
            >
              <AnimatedAlert
                type="error"
                message={error}
                onClose={() => {}}
              />
            </motion.div>
          )}
        </AnimatePresence>

        {/* Form Card */}
        <AnimatedCard delay={0.3} hover={false}>
          <div className="p-8">
            {/* Progress Indicator */}
            <motion.div
              className="mb-8"
              initial={{ opacity: 0, y: -10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.4 }}
            >
              <div className="flex items-center justify-center space-x-4">
                {/* Step 1 */}
                <motion.div
                  className="flex items-center"
                  whileHover={{ scale: 1.05 }}
                >
                  <motion.div
                    className={`flex items-center justify-center w-10 h-10 rounded-full border-2 transition-all ${
                      subStep === 'datos'
                        ? 'bg-[#1797D5] border-[#1797D5] text-white'
                        : 'bg-white border-gray-300 text-gray-400'
                    }`}
                    animate={subStep === 'datos' ? { scale: [1, 1.1, 1] } : {}}
                    transition={{ duration: 0.5 }}
                  >
                    <span className="font-bold">1</span>
                  </motion.div>
                  <span className={`ml-2 font-medium hidden sm:inline ${
                    subStep === 'datos' ? 'text-[#1797D5]' : 'text-gray-400'
                  }`}>
                    Datos de la Cita
                  </span>
                </motion.div>

                {/* Connector Line */}
                <motion.div
                  className="w-16 sm:w-24 h-0.5 bg-gray-300"
                  animate={subStep === 'horario' ? { scaleX: [0, 1] } : {}}
                  transition={{ duration: 0.5 }}
                />

                {/* Step 2 */}
                <motion.div
                  className="flex items-center"
                  whileHover={{ scale: 1.05 }}
                >
                  <motion.div
                    className={`flex items-center justify-center w-10 h-10 rounded-full border-2 transition-all ${
                      subStep === 'horario'
                        ? 'bg-[#1797D5] border-[#1797D5] text-white'
                        : 'bg-white border-gray-300 text-gray-400'
                    }`}
                    animate={subStep === 'horario' ? { scale: [1, 1.1, 1] } : {}}
                    transition={{ duration: 0.5 }}
                  >
                    <span className="font-bold">2</span>
                  </motion.div>
                  <span className={`ml-2 font-medium hidden sm:inline ${
                    subStep === 'horario' ? 'text-[#1797D5]' : 'text-gray-400'
                  }`}>
                    Horario
                  </span>
                </motion.div>
              </div>
            </motion.div>

            {/* SUB-STEP 1: Datos de la Cita */}
            <AnimatePresence mode="wait">
              {subStep === 'datos' && (
                <motion.div
                  key="datos"
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  transition={{ duration: 0.3 }}
                >
                  <div className="border-2 border-gray-200 rounded-xl p-6 mb-6">
                    <h4 className="text-xl font-semibold text-[#1797D5] mb-4 flex items-center">
                      <FiCalendar className="mr-2" /> Información de la Cita
                    </h4>

                    {/* Warning Message */}
                    <AnimatePresence>
                      {showWarning && (
                        <motion.div
                          initial={{ opacity: 0, height: 0 }}
                          animate={{ opacity: 1, height: 'auto' }}
                          exit={{ opacity: 0, height: 0 }}
                          className="mb-6 overflow-hidden"
                        >
                          <AnimatedAlert
                            type="warning"
                            message="Por favor seleccione la fecha y sede antes de elegir la hora de su cita."
                            autoDismiss={true}
                            duration={6000}
                          />
                        </motion.div>
                      )}
                    </AnimatePresence>

                    {/* Client Info Card */}
                    <motion.div
                      className="mb-6 p-4 bg-gradient-to-r from-blue-50 to-cyan-50 rounded-xl border-l-4 border-[#1797D5]"
                      whileHover={{ scale: 1.02 }}
                      transition={{ duration: 0.2 }}
                    >
                      <h5 className="font-semibold text-gray-800 mb-2 flex items-center text-sm">
                        <FiUser className="mr-2 text-[#1797D5]" />
                        Cliente: {clientData.fullName} - {clientData.clientNumber}
                      </h5>
                    </motion.div>

                    <div className="grid md:grid-cols-2 gap-6">
                      {/* Document Type */}
                      <motion.div
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: 0.1 }}
                      >
                        <label className="block text-gray-700 font-semibold mb-2">
                          Tipo de Documento
                        </label>
                        <select
                          value={formData.documentType}
                          onChange={(e) => onFormChange('documentType', e.target.value)}
                          className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none transition-all"
                        >
                          <option>Cédula de Ciudadanía</option>
                          <option>Cédula de Extranjería</option>
                          <option>Pasaporte</option>
                          <option>NIT</option>
                        </select>
                      </motion.div>

                      {/* Appointment Type */}
                      <motion.div
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: 0.2 }}
                      >
                        <label className="block text-gray-700 font-semibold mb-2">
                          Motivo de la Cita
                          <span className="text-red-500 ml-1">*</span>
                        </label>
                        <select
                          value={formData.reason}
                          onChange={(e) => onFormChange('reason', e.target.value)}
                          className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none transition-all ${
                            validationErrors.reason
                              ? 'border-red-500 focus:border-red-500'
                              : 'border-gray-300 focus:border-[#1797D5]'
                          }`}
                        >
                          <option value="">Seleccione un motivo</option>
                          {appointmentTypes.map(tipo => (
                            <option key={tipo.id} value={`${tipo.icon || ''} ${tipo.name}`}>
                              {tipo.icon || ''} {tipo.name}
                            </option>
                          ))}
                        </select>
                        <AnimatePresence>
                          {validationErrors.reason && (
                            <motion.p
                              initial={{ opacity: 0, y: -5 }}
                              animate={{ opacity: 1, y: 0 }}
                              exit={{ opacity: 0, y: -5 }}
                              className="text-red-500 text-sm mt-1"
                            >
                              {validationErrors.reason}
                            </motion.p>
                          )}
                        </AnimatePresence>
                      </motion.div>

                      {/* Branch/Sede */}
                      <motion.div
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: 0.3 }}
                      >
                        <label className="block text-gray-700 font-semibold mb-2">
                          Sede
                          <span className="text-red-500 ml-1">*</span>
                        </label>
                        <select
                          value={formData.branch}
                          onChange={(e) => onFormChange('branch', e.target.value)}
                          className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none transition-all ${
                            validationErrors.branch
                              ? 'border-red-500 focus:border-red-500'
                              : 'border-gray-300 focus:border-[#1797D5]'
                          }`}
                        >
                          <option value="">Seleccione una sede</option>
                          {branches.map(branch => (
                            <option key={branch.id} value={branch.name}>
                              {branch.name} - {branch.city}
                            </option>
                          ))}
                        </select>
                        <AnimatePresence>
                          {validationErrors.branch && (
                            <motion.p
                              initial={{ opacity: 0, y: -5 }}
                              animate={{ opacity: 1, y: 0 }}
                              exit={{ opacity: 0, y: -5 }}
                              className="text-red-500 text-sm mt-1"
                            >
                              {validationErrors.branch}
                            </motion.p>
                          )}
                        </AnimatePresence>
                      </motion.div>

                      {/* Date */}
                      <motion.div
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: 0.4 }}
                      >
                        <label className="block text-gray-700 font-semibold mb-2">
                          Fecha de Cita
                          <span className="text-red-500 ml-1">*</span>
                        </label>
                        <input
                          type="date"
                          value={formData.appointmentDate}
                          onChange={(e) => onFormChange('appointmentDate', e.target.value)}
                          min={new Date(Date.now() + 86400000).toISOString().split('T')[0]}
                          className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none transition-all ${
                            validationErrors.appointmentDate
                              ? 'border-red-500 focus:border-red-500'
                              : 'border-gray-300 focus:border-[#1797D5]'
                          }`}
                        />
                        <AnimatePresence>
                          {validationErrors.appointmentDate && (
                            <motion.p
                              initial={{ opacity: 0, y: -5 }}
                              animate={{ opacity: 1, y: 0 }}
                              exit={{ opacity: 0, y: -5 }}
                              className="text-red-500 text-sm mt-1"
                            >
                              {validationErrors.appointmentDate}
                            </motion.p>
                          )}
                        </AnimatePresence>
                      </motion.div>
                    </div>
                  </div>

                  {/* Navigation Buttons */}
                  <div className="flex flex-col sm:flex-row gap-4">
                    <AnimatedButton
                      onClick={onBack}
                      variant="secondary"
                      size="lg"
                      icon={<FiArrowLeft />}
                      iconPosition="left"
                      className="flex-1"
                    >
                      Volver
                    </AnimatedButton>
                    <AnimatedButton
                      onClick={() => {
                        if (formData.reason && formData.branch && formData.appointmentDate) {
                          setSubStep('horario');
                        }
                      }}
                      disabled={!formData.reason || !formData.branch || !formData.appointmentDate}
                      size="lg"
                      icon={<FiArrowLeft className="rotate-180" />}
                      className="flex-1"
                    >
                      Siguiente
                    </AnimatedButton>
                  </div>
                </motion.div>
              )}

              {/* SUB-STEP 2: Horario */}
              {subStep === 'horario' && (
                <motion.form
                  key="horario"
                  onSubmit={onSubmit}
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: 20 }}
                  transition={{ duration: 0.3 }}
                >
                  <div className="border-2 border-gray-200 rounded-xl p-6 mb-6">
                    <h4 className="text-xl font-semibold text-[#1797D5] mb-4 flex items-center">
                      <FiClock className="mr-2" /> Seleccionar Horario
                    </h4>

                    {/* Time Selection */}
                    <div className="mb-6">
                      <label className="block text-gray-700 font-semibold mb-4">
                        Hora de Cita
                        <span className="text-red-500 ml-1">*</span>
                      </label>

                      {loadingHours ? (
                        <div className="text-center py-8">
                          <LoadingSpinner size="lg" text="Cargando horarios disponibles..." />
                        </div>
                      ) : availableHours.length === 0 ? (
                        <motion.div
                          initial={{ opacity: 0, scale: 0.9 }}
                          animate={{ opacity: 1, scale: 1 }}
                          className="text-center py-8 bg-gray-50 rounded-lg border-2 border-gray-200"
                        >
                          <div className="w-16 h-16 mx-auto bg-gray-100 rounded-2xl flex items-center justify-center mb-4">
                            <FiClock className="w-8 h-8 text-gray-400" />
                          </div>
                          <p className="text-gray-600 font-medium">No hay horarios disponibles</p>
                          <p className="text-gray-500 text-sm mt-2">
                            Para la fecha y sede seleccionadas
                          </p>
                        </motion.div>
                      ) : (
                        <motion.div
                          className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-6 gap-3"
                          initial={{ opacity: 0 }}
                          animate={{ opacity: 1 }}
                          transition={{ staggerChildren: 0.05 }}
                        >
                          {availableHours.map((hora, index) => (
                            <motion.button
                              key={`hora-${index}-${hora}`}
                              type="button"
                              onClick={() => onFormChange('appointmentTime', hora)}
                              initial={{ opacity: 0, scale: 0.8 }}
                              animate={{ opacity: 1, scale: 1 }}
                              transition={{ delay: index * 0.03 }}
                              whileHover={{ scale: 1.05, y: -3 }}
                              whileTap={{ scale: 0.95 }}
                              className={`px-4 py-3.5 text-sm font-semibold rounded-xl border-2 transition-all duration-200 ${
                                formData.appointmentTime === hora
                                  ? 'bg-emerald-400 text-emerald-900 border-emerald-500 shadow-lg'
                                  : 'bg-emerald-50 text-emerald-700 border-emerald-300 hover:bg-emerald-100 hover:border-emerald-400'
                              }`}
                            >
                              {hora}
                            </motion.button>
                          ))}
                        </motion.div>
                      )}
                      <AnimatePresence>
                        {validationErrors.appointmentTime && (
                          <motion.p
                            initial={{ opacity: 0, y: -5 }}
                            animate={{ opacity: 1, y: 0 }}
                            exit={{ opacity: 0, y: -5 }}
                            className="text-red-500 text-sm mt-2"
                          >
                            {validationErrors.appointmentTime}
                          </motion.p>
                        )}
                      </AnimatePresence>
                    </div>

                    {/* Observations */}
                    <motion.div
                      initial={{ opacity: 0, y: 10 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ delay: 0.3 }}
                    >
                      <label className="block text-gray-700 font-semibold mb-2">
                        Observaciones
                      </label>
                      <textarea
                        value={formData.observations}
                        onChange={(e) => {
                          if (e.target.value.length <= 300) {
                            onFormChange('observations', e.target.value);
                          }
                        }}
                        rows={4}
                        maxLength={300}
                        placeholder="Escriba aquí cualquier información adicional sobre su cita..."
                        className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none resize-none transition-all"
                      />
                      <p className="text-sm text-gray-500 mt-1 text-right">
                        {formData.observations.length}/300 caracteres
                      </p>
                    </motion.div>
                  </div>

                  {/* Navigation Buttons */}
                  <div className="flex flex-col sm:flex-row gap-4">
                    <AnimatedButton
                      onClick={() => setSubStep('datos')}
                      variant="secondary"
                      size="lg"
                      icon={<FiArrowLeft />}
                      iconPosition="left"
                      className="flex-1"
                    >
                      Anterior
                    </AnimatedButton>
                    <AnimatedButton
                      type="submit"
                      disabled={isLoading}
                      loading={isLoading}
                      size="lg"
                      icon={<FiCheckCircle />}
                      className="flex-1"
                    >
                      Agendar Cita
                    </AnimatedButton>
                  </div>
                </motion.form>
              )}
            </AnimatePresence>
          </div>
        </AnimatedCard>
      </main>
    </div>
  );
};
