/**
 * Vista de Agendamiento de Citas
 * Componente UI para el proceso de agendamiento
 */

'use client';

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

  // Render appropriate step
  const renderStep = () => {
    switch (step) {
      case 'client':
        return (
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
        );

      case 'confirmation':
        if (!appointmentConfirmation) return null;
        return (
          <ConfirmationStep
            appointmentData={appointmentConfirmation}
            qrCodeDataURL={qrCodeDataURL}
            onPrint={handlePrint}
            onNewRequest={handleNewRequest}
          />
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
