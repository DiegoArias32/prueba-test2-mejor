/**
 * Step 1: Client Number Validation
 * Validates client number and retrieves client data
 */

'use client';

import { FC } from 'react';
import { FiUser, FiRefreshCw, FiUsers, FiCalendar, FiAlertCircle, FiHelpCircle, FiPhone, FiUserPlus } from 'react-icons/fi';
import { FixedHeader } from '@/shared/layouts';
import { BackNavigation } from '@/shared/components';
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
      <div className="bg-gradient-to-br from-gray-50 to-blue-50 pt-24 pb-16 px-4">
        <div className="max-w-4xl mx-auto">
          <BackNavigation backTo="/servicios" />
        </div>
        <div className="max-w-md mx-auto">
          <div className="bg-white rounded-2xl shadow-xl p-8">
            {/* Header */}
            <div className="text-center mb-8">
              <div className="inline-flex items-center justify-center w-16 h-16 bg-[#1797D5] rounded-full mb-4">
                <FiCalendar className="text-white text-3xl" />
              </div>
              <h1 className="text-3xl font-bold text-gray-800 mb-2">
                Agendamiento de Citas
              </h1>
              <p className="text-gray-600">
                Ingrese su número de cliente para continuar
              </p>
            </div>

            {/* Error Message con opción para registro */}
            {error && !isNewClient && (
              <div className="mb-6">
                <div className="p-4 bg-yellow-50 border-l-4 border-yellow-500 rounded-r-lg">
                  <div className="flex items-start">
                    <FiAlertCircle className="text-yellow-600 text-xl mr-3 mt-0.5 flex-shrink-0" />
                    <div className="flex-1">
                      <p className="text-yellow-800 font-semibold mb-2">
                        No se encontró ningún cliente con ese número
                      </p>
                      <button
                        onClick={onNewClientClick}
                        className="inline-flex items-center text-[#1797D5] font-semibold hover:underline transition-colors"
                      >
                        <FiUserPlus className="mr-2" />
                        ¿No tienes número de cliente? Regístrate aquí →
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Client Number Input */}
            {!isNewClient && (
              <>
                <div className="mb-6">
                  <label className="block text-gray-700 font-semibold mb-2">
                    Número de Cliente
                    <span className="text-red-500 ml-1">*</span>
                  </label>
                  <div className="relative">
                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                      <FiUser className="text-gray-400" />
                    </div>
                    <input
                      type="text"
                      value={clientNumber}
                      onChange={(e) => setClientNumber(e.target.value)}
                      onKeyPress={handleKeyPress}
                      placeholder="Ej: 1234567890"
                      className="w-full pl-10 pr-4 py-3 border-2 border-gray-300 rounded-lg focus:border-[#1797D5] focus:outline-none transition-colors"
                      disabled={isLoading}
                    />
                  </div>
                </div>

                {/* Submit Button */}
                <button
                  onClick={validateClient}
                  disabled={isLoading || !clientNumber}
                  className="w-full bg-[#1797D5] text-white py-3 rounded-lg font-semibold hover:bg-[#147ab8] transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed flex items-center justify-center"
                >
                  {isLoading ? (
                    <>
                      <FiRefreshCw className="animate-spin mr-2" />
                      Validando...
                    </>
                  ) : (
                    <>
                      <FiUsers className="mr-2" />
                      Buscar Cliente
                    </>
                  )}
                </button>
              </>
            )}

            {/* Formulario de cliente nuevo */}
            {isNewClient && (
              <>
                <NewClientForm onDataChange={onNewClientDataChange} />

                <div className="mt-6 space-y-3">
                  <button
                    onClick={onContinueAsNewClient}
                    disabled={!isNewClientFormValid}
                    className="w-full bg-[#1797D5] text-white py-3 rounded-lg font-semibold hover:bg-[#147ab8] transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed flex items-center justify-center"
                  >
                    <FiCalendar className="mr-2" />
                    Continuar con agendamiento
                  </button>

                  <button
                    onClick={onBackToClientNumber}
                    className="w-full text-gray-600 hover:text-gray-800 font-semibold flex items-center justify-center transition-colors py-2"
                  >
                    ← Volver a ingresar número de cliente
                  </button>
                </div>
              </>
            )}

            {/* Help Section */}
            <div className="mt-8 p-4 bg-blue-50 rounded-lg">
              <div className="flex items-start">
                <FiHelpCircle className="text-[#1797D5] text-xl mr-3 mt-0.5 flex-shrink-0" />
                <div>
                  <h3 className="font-semibold text-gray-800 mb-1">
                    ¿No conoce su número de cliente?
                  </h3>
                  <p className="text-sm text-gray-600 mb-2">
                    Puede encontrarlo en su factura de energía o comunicarse con nosotros.
                  </p>
                  <div className="flex items-center text-sm text-[#1797D5]">
                    <FiPhone className="mr-1" />
                    <span>Línea de atención: 018000 111 888</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};
