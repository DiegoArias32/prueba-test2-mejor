/**
 * ViewModel para agendamiento de citas
 */

'use client';

import { useState, useEffect, useCallback } from 'react';
import { appointmentRepository } from '../repositories/appointment.repository';
import type { ClientDto, BranchDto, AppointmentTypeDto, LoadingState } from '@/core/types';
import type {
  AppointmentFormData,
  AppointmentConfirmation,
  AppointmentStep,
} from '../models/appointment.models';
import type { NewClientData } from '../views/components/NewClientForm';

export const useAppointmentScheduling = () => {
  // Estados de UI
  const [step, setStep] = useState<AppointmentStep>('client');
  const [loadingState, setLoadingState] = useState<LoadingState>('idle');
  const [loadingHours, setLoadingHours] = useState(false);
  const [error, setError] = useState('');
  const [validationErrors, setValidationErrors] = useState<{ [key: string]: string }>({});

  // Datos del cliente
  const [clientNumber, setClientNumber] = useState('');
  const [clientData, setClientData] = useState<ClientDto | null>(null);
  const [isNewClient, setIsNewClient] = useState(false);
  const [newClientData, setNewClientData] = useState<NewClientData | null>(null);
  const [isNewClientFormValid, setIsNewClientFormValid] = useState(false);

  // Datos del formulario
  const [formData, setFormData] = useState<AppointmentFormData>({
    documentType: 'Cédula de Ciudadanía',
    reason: '',
    branch: '',
    appointmentDate: '',
    appointmentTime: '',
    observations: '',
  });

  // Datos de confirmación
  const [appointmentConfirmation, setAppointmentConfirmation] =
    useState<AppointmentConfirmation | null>(null);
  const [qrCodeDataURL, setQrCodeDataURL] = useState<string>('');

  // Datos de catálogos
  const [branches, setBranches] = useState<BranchDto[]>([]);
  const [appointmentTypes, setAppointmentTypes] = useState<AppointmentTypeDto[]>([]);
  const [availableHours, setAvailableHours] = useState<string[]>([]);

  /**
   * Carga datos iniciales (sedes, tipos de cita)
   */
  const loadInitialData = useCallback(async () => {
    setLoadingState('loading');
    try {
      const [branchesData, typesData] = await Promise.all([
        appointmentRepository.getPublicBranches(),
        appointmentRepository.getPublicAppointmentTypes(),
      ]);

      setBranches(branchesData);
      setAppointmentTypes(typesData);

      // Establecer valores por defecto
      if (branchesData.length > 0) {
        const mainBranch = branchesData.find((s) => s.isMain) || branchesData[0];
        setFormData((prev) => ({
          ...prev,
          branch: mainBranch.name,
        }));
      }

      if (typesData.length > 0) {
        setFormData((prev) => ({
          ...prev,
          reason: `${typesData[0].icon} ${typesData[0].name}`,
        }));
      }

      // Fecha por defecto (mañana)
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      setFormData((prev) => ({
        ...prev,
        appointmentDate: tomorrow.toISOString().split('T')[0],
      }));

      setLoadingState('success');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar datos iniciales');
      setLoadingState('error');
    }
  }, []);

  /**
   * Valida un cliente por número
   */
  const validateClient = useCallback(async () => {
    if (!clientNumber.trim()) {
      setError('Por favor ingrese el número de cliente');
      return;
    }

    setLoadingState('loading');
    setError('');

    try {
      const client = await appointmentRepository.validatePublicClient(clientNumber.trim());
      setClientData(client);
      setIsNewClient(false);
      setStep('form');
      setLoadingState('success');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Cliente no encontrado');
      setLoadingState('error');
      // No cambiamos isNewClient aquí, el usuario debe hacer click en "Regístrate"
    }
  }, [clientNumber]);

  /**
   * Maneja el click en "Registrarse como nuevo cliente"
   */
  const handleNewClientClick = useCallback(() => {
    setIsNewClient(true);
    setError('');
    setClientData(null);
  }, []);

  /**
   * Vuelve al formulario de número de cliente
   */
  const handleBackToClientNumber = useCallback(() => {
    setIsNewClient(false);
    setNewClientData(null);
    setIsNewClientFormValid(false);
    setError('');
  }, []);

  /**
   * Maneja cambios en los datos del nuevo cliente
   */
  const handleNewClientDataChange = useCallback((data: NewClientData, isValid: boolean) => {
    setNewClientData(data);
    setIsNewClientFormValid(isValid);
  }, []);

  /**
   * Continúa al formulario de cita como cliente nuevo
   */
  const handleContinueAsNewClient = useCallback(() => {
    if (isNewClientFormValid && newClientData) {
      setStep('form');
    }
  }, [isNewClientFormValid, newClientData]);

  /**
   * Carga horas disponibles para una fecha y sede
   */
  const loadAvailableHours = useCallback(async () => {
    if (!formData.appointmentDate || !formData.branch) return;

    setLoadingHours(true);
    try {
      const selectedBranch = branches.find((s) => s.name === formData.branch);
      if (!selectedBranch) {
        setAvailableHours([]);
        return;
      }

      const horas = await appointmentRepository.getPublicAvailableTimes(
        formData.appointmentDate,
        selectedBranch.id
      );

      setAvailableHours(horas || []);

      // Limpiar hora seleccionada si ya no está disponible
      if (formData.appointmentTime && !horas.includes(formData.appointmentTime)) {
        setFormData((prev) => ({
          ...prev,
          appointmentTime: '',
        }));
      }
    } catch (err) {
      console.error('Error al cargar horas:', err);
      setAvailableHours([]);
    } finally {
      setLoadingHours(false);
    }
  }, [formData.appointmentDate, formData.branch, formData.appointmentTime, branches]);

  /**
   * Agenda la cita
   */
  const scheduleAppointment = useCallback(async () => {
    setLoadingState('loading');
    setError('');

    try {
      const selectedBranch = branches.find((s) => s.name === formData.branch);
      const selectedType = appointmentTypes.find((t) => `${t.icon} ${t.name}` === formData.reason);

      if (!selectedBranch || !selectedType) {
        throw new Error('Datos inválidos');
      }

      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      let response: any;
      let clientNumberForQR: string;

      if (isNewClient && newClientData) {
        // Flujo para cliente nuevo
        if (!isNewClientFormValid) {
          throw new Error('Por favor complete todos los campos requeridos del formulario');
        }

        const simpleResponse = await appointmentRepository.scheduleAppointmentForNewUser({
          userData: newClientData,
          branchId: selectedBranch.id,
          appointmentTypeId: selectedType.id,
          appointmentDate: formData.appointmentDate,
          appointmentTime: formData.appointmentTime,
          observations: formData.observations,
        });

        // Crear objeto de respuesta compatible
        response = {
          appointmentNumber: simpleResponse.requestNumber,
          appointmentDate: simpleResponse.appointmentDate,
          appointmentTime: simpleResponse.appointmentTime,
          status: simpleResponse.status,
          message: simpleResponse.message,
        };

        // Para cliente nuevo, usar el número de documento como identificador temporal
        clientNumberForQR = newClientData.documentNumber;

        // Crear objeto clientData temporal para la confirmación
        const tempClientData: ClientDto = {
          id: 0,
          clientNumber: simpleResponse.requestNumber, // Usar el número de solicitud
          documentType: newClientData.documentType,
          documentNumber: newClientData.documentNumber,
          fullName: newClientData.fullName,
          email: newClientData.email,
          phone: newClientData.phone,
          mobile: newClientData.mobile,
          address: newClientData.address,
          createdAt: new Date().toISOString(),
          isActive: true,
          isEnabled: true,
        };

        // Generar QR Code
        const qrData = `CITA:${response.appointmentNumber}|CLIENTE:${clientNumberForQR}`;
        const QRCode = (await import('qrcode')).default;
        const qrUrl = await QRCode.toDataURL(qrData, { width: 300 });

        setAppointmentConfirmation({
          ticketNumber: response.appointmentNumber,
          issueDateTime: new Date().toISOString(),
          clientData: tempClientData,
          formData,
        });

        setQrCodeDataURL(qrUrl);
      } else if (clientData) {
        // Flujo para cliente existente
        response = await appointmentRepository.schedulePublicAppointment({
          clientNumber: clientData.clientNumber,
          branchId: selectedBranch.id,
          appointmentTypeId: selectedType.id,
          appointmentDate: formData.appointmentDate,
          appointmentTime: formData.appointmentTime,
          observations: formData.observations,
        });

        clientNumberForQR = clientData.clientNumber;

        // Generar QR Code
        const qrData = `CITA:${response.appointmentNumber}|CLIENTE:${clientNumberForQR}`;
        const QRCode = (await import('qrcode')).default;
        const qrUrl = await QRCode.toDataURL(qrData, { width: 300 });

        setAppointmentConfirmation({
          ticketNumber: response.appointmentNumber,
          issueDateTime: new Date().toISOString(),
          clientData,
          formData,
        });

        setQrCodeDataURL(qrUrl);
      } else {
        throw new Error('No hay datos de cliente disponibles');
      }

      setStep('confirmation');
      setLoadingState('success');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al agendar cita');
      setLoadingState('error');
    }
  }, [clientData, isNewClient, newClientData, isNewClientFormValid, branches, appointmentTypes, formData]);

  /**
   * Reinicia el flujo
   */
  const resetFlow = useCallback(() => {
    setStep('client');
    setClientNumber('');
    setClientData(null);
    setIsNewClient(false);
    setNewClientData(null);
    setIsNewClientFormValid(false);
    setAppointmentConfirmation(null);
    setQrCodeDataURL('');
    setError('');
    setValidationErrors({});
    setFormData({
      documentType: 'Cédula de Ciudadanía',
      reason: '',
      branch: '',
      appointmentDate: '',
      appointmentTime: '',
      observations: '',
    });
  }, []);

  /**
   * Actualiza un campo del formulario
   */
  const updateFormField = useCallback((field: keyof AppointmentFormData, value: string) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));
  }, []);

  // Cargar datos iniciales al montar
  useEffect(() => {
    loadInitialData();
  }, [loadInitialData]);

  // Cargar horas disponibles cuando cambian fecha o sede
  useEffect(() => {
    if (formData.appointmentDate && formData.branch && step === 'form') {
      loadAvailableHours();
    }
  }, [formData.appointmentDate, formData.branch, step, loadAvailableHours]);

  return {
    // Estados
    step,
    loadingState,
    isLoading: loadingState === 'loading',
    loadingHours,
    error,
    validationErrors,

    // Datos
    clientNumber,
    clientData,
    formData,
    appointmentConfirmation,
    qrCodeDataURL,
    branches,
    appointmentTypes,
    availableHours,

    // Nuevo cliente
    isNewClient,
    newClientData,
    isNewClientFormValid,

    // Métodos
    setClientNumber,
    setStep,
    validateClient,
    loadAvailableHours,
    scheduleAppointment,
    resetFlow,
    updateFormField,
    setFormData,
    handleNewClientClick,
    handleBackToClientNumber,
    handleNewClientDataChange,
    handleContinueAsNewClient,
  };
};
