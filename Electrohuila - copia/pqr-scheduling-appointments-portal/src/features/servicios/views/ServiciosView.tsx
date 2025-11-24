/**
 * View - Servicios
 * Vista principal para la página de servicios (Arquitectura MVVM)
 */

'use client';

import Link from 'next/link';
import { FiInfo, FiChevronDown, FiChevronRight } from 'react-icons/fi';
import { FixedHeader } from '@/shared/layouts';
import { BackNavigation, Footer } from '@/shared/components';
import { useServicios } from '../viewmodels/useServicios';
import { ServiceIcon } from './components';

/**
 * Componente principal de la vista de servicios
 */
export const ServiciosView: React.FC = () => {
  // ViewModel - maneja la lógica de negocio y estado
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
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 overflow-x-hidden">
      {/* Header */}
      <FixedHeader />

      {/* Main Content */}
      <main className="max-w-4xl mx-auto px-4 py-12 pt-24">
        {/* Back Navigation */}
        <BackNavigation backTo="/" />

        {/* Page Header */}
        <div className="text-center mb-12">
          <h1 className="text-4xl md:text-5xl font-bold text-[#203461] mb-4">
            Sistema de
            <span className="bg-gradient-to-r from-[#1797D5] to-[#56C2E1] bg-clip-text text-transparent"> Citas</span>
          </h1>
          <h2 className="text-2xl font-semibold text-[#203461] mb-8">ElectroHuila</h2>
        </div>

        {/* Service Selection Card */}
        <div className="bg-white rounded-2xl shadow-xl border border-gray-100 p-8 mb-8 mx-auto max-w-md">
          <div className="text-center mb-8">
            <div className="w-20 h-20 mx-auto bg-gradient-to-br from-[#97D4E3] to-[#56C2E1] rounded-2xl flex items-center justify-center shadow-lg mb-4">
              <FiInfo className="w-10 h-10 text-[#203461]" />
            </div>
            <h3 className="text-2xl font-bold text-[#203461] mb-2">Seleccione el Tipo de cita</h3>
            <p className="text-gray-600">Elija el tipo de atención o cita que mejor se ajuste a su necesidad para continuar con el proceso.</p>
          </div>

          {/* Info Alert y Select en un contenedor más angosto */}
          <div>
            {/* Info Alert */}
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 mb-6 w-full flex items-center overflow-x-auto">
              <FiInfo className="w-5 h-5 text-blue-500 mr-2 flex-shrink-0" />
              <span className="text-blue-700 font-medium text-sm truncate">
                Seleccione el tipo de servicio que requiere
              </span>
            </div>

            {/* Dropdown */}
            <div className="relative mb-6 w-full" ref={dropdownRef}>
              <label className="block text-sm font-semibold text-[#203461] mb-2">
                Tipo de Servicio <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <button
                  onClick={toggleDropdown}
                  className="w-full bg-white border-2 border-gray-300 rounded-lg px-4 py-3 text-left focus:outline-none focus:border-[#1797D5] transition-colors duration-200 hover:border-[#56C2E1]"
                >
                  <div className="flex items-center justify-between">
                    <span className={`${selectedService ? 'text-gray-900' : 'text-gray-500'}`}>
                      {selectedService || 'Seleccione un servicio...'}
                    </span>
                    <FiChevronDown className={`w-5 h-5 text-gray-400 transform transition-transform duration-200 ${isOpen ? 'rotate-180' : ''}`} />
                  </div>
                </button>

                {/* Dropdown Menu */}
                {isOpen && (
                  <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg">
                    <div className="py-1">
                      <div className="px-4 py-2 bg-[#1797D5] text-white font-medium">
                        Seleccione un servicio...
                      </div>
                      {services.map((service) => (
                        <button
                          key={service.id}
                          onClick={() => handleServiceSelect(service)}
                          className="w-full px-4 py-3 text-left hover:bg-gray-50 focus:bg-gray-50 focus:outline-none transition-colors duration-150 flex items-center space-x-3"
                        >
                          <span className="text-lg"><ServiceIcon serviceId={service.id} /></span>
                          <span className="text-gray-900 font-medium">{service.name}</span>
                        </button>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* Continue Button */}
          {selectedService && selectedServiceData && (
            <div className="text-center">
              <Link
                href={selectedServiceData.href}
                className="inline-flex items-center justify-center bg-gradient-to-r from-[#203461] to-[#1797D5] text-white px-8 py-3 rounded-xl font-semibold hover:from-[#1A6192] hover:to-[#56C2E1] transition-all duration-300 shadow-lg hover:shadow-xl transform hover:-translate-y-1"
              >
                <span className="mr-2"><ServiceIcon serviceId={selectedServiceData.id} /></span>
                <span>{selectedService}</span>
                <FiChevronRight className="w-5 h-5 ml-2" />
              </Link>
            </div>
          )}
        </div>
      </main>

      {/* Footer */}
      <Footer />
    </div>
  );
};
