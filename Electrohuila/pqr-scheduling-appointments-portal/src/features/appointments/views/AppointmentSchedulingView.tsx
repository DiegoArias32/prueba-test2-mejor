/**
 * Vista de Agendamiento de Citas
 * Componente UI para el proceso de agendamiento con animaciones mejoradas
 */

'use client';

import { motion } from 'framer-motion';
import { useAppointmentScheduling } from '../viewmodels/useAppointmentScheduling';
import { ClientValidationStep } from './steps/ClientValidationStep';
import { AppointmentFormStep } from './steps/AppointmentFormStep';
import { ConfirmationStep } from './steps/ConfirmationStep';
import { Footer } from '@/shared/components';

export const AppointmentSchedulingView: React.FC = () => {
  const {
    step,
    isLoading,
    loadingHours,
    error,
    clientNumber,
    setClientNumber,
    clientData,
    appointmentConfirmation,
    qrCodeDataURL,
    branches,
    appointmentTypes,
    availableHours,
    formData,
    validationErrors,
    isNewClient,
    isNewClientFormValid,
    validateClient,
    scheduleAppointment,
    resetFlow,
    updateFormField,
    setStep,
    handleNewClientClick,
    handleBackToClientNumber,
    handleNewClientDataChange,
    handleContinueAsNewClient
  } = useAppointmentScheduling();

  // Handler functions
  const handleFormChange = (field: string, value: string) => {
    updateFormField(field as keyof typeof formData, value);
  };

  const handleFormSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await scheduleAppointment();
  };

  const handleBackToClient = () => {
    setStep('client');
  };

  const handleNewRequest = () => {
    resetFlow();
  };

  const handlePrint = () => {
    window.print();
  };

  // Render appropriate step with animations
  const renderStep = () => {
    switch (step) {
      case 'client':
        return (
          <motion.div
            key="client-step"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 20 }}
            transition={{ duration: 0.4 }}
          >
            <ClientValidationStep
              clientNumber={clientNumber}
              setClientNumber={setClientNumber}
              validateClient={validateClient}
              isLoading={isLoading}
              error={error}
              isNewClient={isNewClient}
              onNewClientClick={handleNewClientClick}
              onBackToClientNumber={handleBackToClientNumber}
              onNewClientDataChange={handleNewClientDataChange}
              onContinueAsNewClient={handleContinueAsNewClient}
              isNewClientFormValid={isNewClientFormValid}
            />
          </motion.div>
        );

      case 'form':
        // Validar que haya clientData O que sea cliente nuevo con datos válidos
        if (!clientData && !isNewClient) return null;

        // Crear objeto clientData temporal si es cliente nuevo
        const displayClientData = clientData || {
          fullName: 'Cliente Nuevo',
          clientNumber: 'Pendiente de asignación'
        };

        return (
          <motion.div
            key="form-step"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 20 }}
            transition={{ duration: 0.4 }}
          >
            <AppointmentFormStep
              clientData={displayClientData}
              formData={formData}
              branches={branches}
              appointmentTypes={appointmentTypes}
              availableHours={availableHours}
              loadingHours={loadingHours}
              isLoading={isLoading}
              error={error}
              validationErrors={validationErrors}
              onFormChange={handleFormChange}
              onSubmit={handleFormSubmit}
              onBack={handleBackToClient}
            />
          </motion.div>
        );

      case 'confirmation':
        if (!appointmentConfirmation) return null;
        return (
          <motion.div
            key="confirmation-step"
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            exit={{ opacity: 0, scale: 0.95 }}
            transition={{ duration: 0.4 }}
          >
            <ConfirmationStep
              appointmentData={appointmentConfirmation}
              qrCodeDataURL={qrCodeDataURL}
              onPrint={handlePrint}
              onNewRequest={handleNewRequest}
            />
          </motion.div>
        );

      default:
        return null;
    }
  };

  return (
    <>
      {renderStep()}
      <Footer />
    </>
  );
};
