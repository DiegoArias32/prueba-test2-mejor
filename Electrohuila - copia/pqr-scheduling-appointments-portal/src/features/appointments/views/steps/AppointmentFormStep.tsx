/**
 * Step 2: Appointment Form
 * Form to fill appointment details and select time slot
 */

'use client';

import { FC, useState } from 'react';
import {
  FiUser,
  FiCalendar,
  FiAlertCircle,
  FiArrowLeft,
  FiRefreshCw,
  FiCheckCircle
} from 'react-icons/fi';
import { FixedHeader } from '@/shared/layouts';
import { BackNavigation } from '@/shared/components';
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

  // Close warning after 6 seconds
  useState(() => {
    const timer = setTimeout(() => setShowWarning(false), 6000);
    return () => clearTimeout(timer);
  });

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 overflow-x-hidden">
      <FixedHeader />

      {/* Navigation */}
      <div className="max-w-6xl mx-auto px-4 pt-24">
        <BackNavigation backTo="/" backText="Volver" />
      </div>

      {/* Main Content */}
      <main className="max-w-5xl mx-auto px-4 py-8">
        {/* Page Header */}
        <div className="text-center mb-12 print:hidden">
          <h1 className="text-5xl font-bold text-[#203461] mb-4">
            Agendamiento de
            <span className="bg-gradient-to-r from-[#1797D5] to-[#56C2E1] bg-clip-text text-transparent"> Citas</span>
          </h1>
          <h2 className="text-2xl font-semibold text-[#203461] mb-6">ElectroHuila</h2>
          <p className="text-xl text-gray-600 max-w-3xl mx-auto">
            Complete la información para agendar su cita
          </p>
        </div>

        {/* Error Message */}
        {error && (
          <div className="mb-6 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl print:hidden">
            <div className="flex items-center">
              <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
              </svg>
              <span>{error}</span>
            </div>
          </div>
        )}

        {/* Form Card */}
        <div className="bg-white rounded-2xl shadow-xl p-8">
          {/* Progress Indicator */}
          <div className="mb-8">
            <div className="flex items-center justify-center space-x-4">
              {/* Step 1 */}
              <div className="flex items-center">
                <div className={`flex items-center justify-center w-10 h-10 rounded-full border-2 transition-all ${
                  subStep === 'datos'
                    ? 'bg-[#1797D5] border-[#1797D5] text-white'
                    : 'bg-white border-gray-300 text-gray-400'
                }`}>
                  <span className="font-bold">1</span>
                </div>
                <span className={`ml-2 font-medium hidden sm:inline ${
                  subStep === 'datos' ? 'text-[#1797D5]' : 'text-gray-400'
                }`}>
                  Datos de la Cita
                </span>
              </div>

              {/* Connector Line */}
              <div className="w-16 sm:w-24 h-0.5 bg-gray-300"></div>

              {/* Step 2 */}
              <div className="flex items-center">
                <div className={`flex items-center justify-center w-10 h-10 rounded-full border-2 transition-all ${
                  subStep === 'horario'
                    ? 'bg-[#1797D5] border-[#1797D5] text-white'
                    : 'bg-white border-gray-300 text-gray-400'
                }`}>
                  <span className="font-bold">2</span>
                </div>
                <span className={`ml-2 font-medium hidden sm:inline ${
                  subStep === 'horario' ? 'text-[#1797D5]' : 'text-gray-400'
                }`}>
                  Horario
                </span>
              </div>
            </div>
          </div>

          {/* SUB-STEP 1: Datos de la Cita */}
          {subStep === 'datos' && (
            <div>
              <div className="border-2 border-gray-200 rounded-xl p-6 mb-6">
                <h4 className="text-xl font-semibold text-[#1797D5] mb-4 flex items-center">
                  <FiCalendar className="mr-2" /> Información de la Cita
                </h4>

                {/* Warning Message */}
                {showWarning && (
                  <div className="mb-6 p-4 bg-yellow-50 border-l-4 border-yellow-500 rounded-r-lg animate-fade-in">
                    <div className="flex items-start">
                      <FiAlertCircle className="text-yellow-600 text-xl mr-3 mt-0.5 flex-shrink-0" />
                      <div>
                        <p className="text-yellow-800 font-semibold mb-1">Importante</p>
                        <p className="text-yellow-700 text-sm">
                          Por favor seleccione la fecha y sede antes de elegir la hora de su cita.
                        </p>
                      </div>
                    </div>
                  </div>
                )}

                {/* Client Info Card */}
                <div className="mb-6 p-4 bg-blue-50 rounded-lg border-l-4 border-[#1797D5]">
                  <h5 className="font-semibold text-gray-800 mb-2 flex items-center text-sm">
                    <FiUser className="mr-2 text-[#1797D5]" />
                    Cliente: {clientData.fullName} - {clientData.clientNumber}
                  </h5>
                </div>

                <div className="grid md:grid-cols-2 gap-6">
                  {/* Document Type */}
                  <div>
                    <label className="block text-gray-700 font-semibold mb-2">
                      Tipo de Documento
                    </label>
                    <select
                      value={formData.documentType}
                      onChange={(e) => onFormChange('documentType', e.target.value)}
                      className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none"
                    >
                      <option>Cédula de Ciudadanía</option>
                      <option>Cédula de Extranjería</option>
                      <option>Pasaporte</option>
                      <option>NIT</option>
                    </select>
                  </div>

                  {/* Appointment Type */}
                  <div>
                    <label className="block text-gray-700 font-semibold mb-2">
                      Motivo de la Cita
                      <span className="text-red-500 ml-1">*</span>
                    </label>
                    <select
                      value={formData.reason}
                      onChange={(e) => onFormChange('reason', e.target.value)}
                      className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none transition-colors ${
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
                    {validationErrors.reason && (
                      <p className="text-red-500 text-sm mt-1">{validationErrors.reason}</p>
                    )}
                  </div>

                  {/* Branch/Sede */}
                  <div>
                    <label className="block text-gray-700 font-semibold mb-2">
                      Sede
                      <span className="text-red-500 ml-1">*</span>
                    </label>
                    <select
                      value={formData.branch}
                      onChange={(e) => onFormChange('branch', e.target.value)}
                      className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none transition-colors ${
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
                    {validationErrors.branch && (
                      <p className="text-red-500 text-sm mt-1">{validationErrors.branch}</p>
                    )}
                  </div>

                  {/* Date */}
                  <div>
                    <label className="block text-gray-700 font-semibold mb-2">
                      Fecha de Cita
                      <span className="text-red-500 ml-1">*</span>
                    </label>
                    <input
                      type="date"
                      value={formData.appointmentDate}
                      onChange={(e) => onFormChange('appointmentDate', e.target.value)}
                      min={new Date(Date.now() + 86400000).toISOString().split('T')[0]}
                      className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none transition-colors ${
                        validationErrors.appointmentDate
                          ? 'border-red-500 focus:border-red-500'
                          : 'border-gray-300 focus:border-[#1797D5]'
                      }`}
                    />
                    {validationErrors.appointmentDate && (
                      <p className="text-red-500 text-sm mt-1">{validationErrors.appointmentDate}</p>
                    )}
                  </div>
                </div>
              </div>

              {/* Navigation Buttons */}
              <div className="flex flex-col sm:flex-row gap-4">
                <button
                  type="button"
                  onClick={onBack}
                  className="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg font-semibold hover:bg-gray-50 transition-colors flex items-center justify-center"
                >
                  <FiArrowLeft className="mr-2" />
                  Volver
                </button>
                <button
                  type="button"
                  onClick={() => {
                    if (formData.reason && formData.branch && formData.appointmentDate) {
                      setSubStep('horario');
                    }
                  }}
                  disabled={!formData.reason || !formData.branch || !formData.appointmentDate}
                  className="flex-1 px-6 py-3 bg-[#1797D5] text-white rounded-lg font-semibold hover:bg-[#147ab8] transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed flex items-center justify-center"
                >
                  Siguiente
                  <FiArrowLeft className="ml-2 rotate-180" />
                </button>
              </div>
            </div>
          )}

          {/* SUB-STEP 2: Horario */}
          {subStep === 'horario' && (
            <form onSubmit={onSubmit}>
              <div className="border-2 border-gray-200 rounded-xl p-6 mb-6">
                <h4 className="text-xl font-semibold text-[#1797D5] mb-4 flex items-center">
                  <FiCalendar className="mr-2" /> Seleccionar Horario
                </h4>

                {/* Time Selection */}
                <div className="mb-6">
                  <label className="block text-gray-700 font-semibold mb-4">
                    Hora de Cita
                    <span className="text-red-500 ml-1">*</span>
                  </label>

                  {loadingHours ? (
                    <div className="text-center py-8">
                      <div className="animate-spin w-8 h-8 border-4 border-[#1797D5] border-t-transparent rounded-full mx-auto mb-4"></div>
                      <p className="text-gray-600">Cargando horarios disponibles...</p>
                    </div>
                  ) : availableHours.length === 0 ? (
                    <div className="text-center py-8 bg-gray-50 rounded-lg border-2 border-gray-200">
                      <div className="w-16 h-16 mx-auto bg-gray-100 rounded-2xl flex items-center justify-center mb-4">
                        <svg className="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                      </div>
                      <p className="text-gray-600 font-medium">No hay horarios disponibles</p>
                      <p className="text-gray-500 text-sm mt-2">
                        Para la fecha y sede seleccionadas
                      </p>
                    </div>
                  ) : (
                    <div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-6 gap-3">
                      {availableHours.map((hora, index) => (
                        <button
                          key={`hora-${index}-${hora}`}
                          type="button"
                          onClick={() => onFormChange('appointmentTime', hora)}
                          className={`px-4 py-3.5 text-sm font-semibold rounded-xl border-2 transition-all duration-200 ${
                            formData.appointmentTime === hora
                              ? 'bg-emerald-400 text-emerald-900 border-emerald-500 shadow-md'
                              : 'bg-emerald-50 text-emerald-700 border-emerald-300 hover:bg-emerald-100 hover:border-emerald-400'
                          }`}
                        >
                          {hora}
                        </button>
                      ))}
                    </div>
                  )}
                  {validationErrors.appointmentTime && (
                    <p className="text-red-500 text-sm mt-1">{validationErrors.appointmentTime}</p>
                  )}
                </div>

                {/* Observations */}
                <div>
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
                    className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none resize-none"
                  />
                  <p className="text-sm text-gray-500 mt-1 text-right">
                    {formData.observations.length}/300 caracteres
                  </p>
                </div>
              </div>

              {/* Navigation Buttons */}
              <div className="flex flex-col sm:flex-row gap-4">
                <button
                  type="button"
                  onClick={() => setSubStep('datos')}
                  className="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg font-semibold hover:bg-gray-50 transition-colors flex items-center justify-center"
                >
                  <FiArrowLeft className="mr-2" />
                  Anterior
                </button>
                <button
                  type="submit"
                  disabled={isLoading}
                  className="flex-1 px-6 py-3 bg-[#1797D5] text-white rounded-lg font-semibold hover:bg-[#147ab8] transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed flex items-center justify-center"
                >
                  {isLoading ? (
                    <>
                      <FiRefreshCw className="animate-spin mr-2" />
                      Agendando...
                    </>
                  ) : (
                    <>
                      <FiCheckCircle className="mr-2" />
                      Agendar Cita
                    </>
                  )}
                </button>
              </div>
            </form>
          )}
        </div>
      </main>
    </div>
  );
};
