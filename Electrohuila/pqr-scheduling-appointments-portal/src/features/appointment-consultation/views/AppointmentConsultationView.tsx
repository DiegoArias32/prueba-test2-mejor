/**
 * Appointment Consultation View
 * Refactored to follow MVVM pattern
 */

"use client";

import React, { useState } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { AppointmentDto } from '@/services/api';
import { ValidatedInput, BackNavigation, AppointmentCard, StatsCard, CancelAppointmentModal } from '@/shared/components';
import { FixedHeader } from '@/shared/layouts';
import { useAppointmentConsultation } from '../viewmodels/useAppointmentConsultation';

export default function AppointmentConsultationView() {
  const {
    clientNumber,
    isAuthenticated,
    appointments,
    client,
    loading,
    error,
    successMessage,
    activeTab,
    expandedCard,
    setClientNumber,
    setActiveTab,
    setExpandedCard,
    searchAppointments,
    cancelAppointment,
    resetForm,
    getFilteredAppointments,
  } = useAppointmentConsultation();

  const [selectedAppointment, setSelectedAppointment] = useState<AppointmentDto | null>(null);

  const pendingAppointments = getFilteredAppointments('pending');
  const completedAppointments = getFilteredAppointments('completed');
  const cancelledAppointments = getFilteredAppointments('cancelled');

  const handleCancelClick = (appointment: AppointmentDto) => {
    setSelectedAppointment(appointment);
  };

  const handleCancelConfirm = async (appointmentId: number, reason: string) => {
    const success = await cancelAppointment(appointmentId, reason);
    if (success) {
      setSelectedAppointment(null);
    }
    return success;
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50">
      <FixedHeader />

      <div className="max-w-6xl mx-auto px-4 py-8 pt-24">
        <BackNavigation backTo="/servicios" />

        {/* Page Header */}
        <div className="text-center mb-12">
          <h1 className="text-5xl font-bold text-[#203461] mb-4">
            Gestión de
            <span className="bg-gradient-to-r from-[#1797D5] to-[#56C2E1] bg-clip-text text-transparent"> Citas</span>
          </h1>
          <p className="text-xl text-[#1A6192] max-w-2xl mx-auto">
            Consulte el estado de sus citas para reclamos por facturación y reportes por daños
          </p>
        </div>

        {/* Alerts */}
        {error && (
          <div className="mb-6 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl">
            <div className="flex items-center">
              <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
              </svg>
              {error}
            </div>
          </div>
        )}

        {successMessage && (
          <div className="mb-6 bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-xl">
            <div className="flex items-center">
              <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
              </svg>
              {successMessage}
            </div>
          </div>
        )}

        {!isAuthenticated ? (
          /* Login Form */
          <div className="max-w-md mx-auto">
            <div className="bg-white rounded-3xl shadow-xl border border-gray-100 p-8">
              <div className="text-center mb-8">
                <div className="w-20 h-20 mx-auto bg-gradient-to-br from-[#97D4E3] to-[#56C2E1] rounded-2xl flex items-center justify-center shadow-lg mb-6">
                  <svg className="w-10 h-10 text-[#203461]" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth="2">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                </div>
                <h2 className="text-2xl font-bold text-[#203461] mb-2">Acceso al Sistema</h2>
                <p className="text-gray-600">Ingrese su número de cliente para consultar sus citas</p>
              </div>

              <div className="space-y-6">
                <ValidatedInput
                  label="Número de Cliente"
                  value={clientNumber}
                  onChange={(value) => setClientNumber(value.replace(/\D/g, ''))}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter' && !loading && clientNumber.trim()) {
                      e.preventDefault();
                      searchAppointments();
                    }
                  }}
                  type="text"
                  placeholder="Ej: 100001"
                  required
                  maxLength={15}
                />

                <button
                  onClick={searchAppointments}
                  disabled={loading}
                  className="w-full bg-gradient-to-r from-[#203461] to-[#1797D5] text-white px-8 py-4 rounded-xl font-semibold hover:from-[#1A6192] hover:to-[#56C2E1] transition-all duration-300 shadow-lg hover:shadow-xl transform hover:-translate-y-1 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none"
                >
                  {loading ? (
                    <div className="flex items-center justify-center">
                      <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                      Consultando...
                    </div>
                  ) : (
                    'Consultar Citas'
                  )}
                </button>
              </div>
            </div>
          </div>
        ) : (
          /* Dashboard & Appointments */
          <div className="space-y-8">
            {/* User Info */}
            <div className="bg-white rounded-2xl shadow-lg border border-gray-100 p-6">
              <div className="flex items-center justify-between">
                <div>
                  <h2 className="text-xl font-bold text-[#203461]">
                    Cliente: {client?.fullName} ({client?.clientNumber})
                  </h2>
                  <p className="text-gray-600">Total de citas: {appointments.length}</p>
                </div>
                <button
                  onClick={resetForm}
                  className="flex items-center space-x-2 text-[#1A6192] hover:text-[#203461] font-medium transition-colors"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                  </svg>
                  <span>Cambiar Usuario</span>
                </button>
              </div>
            </div>

            {/* Stats Dashboard */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <StatsCard
                title="Citas Pendientes"
                count={pendingAppointments.length}
                color="bg-yellow-100"
                icon={
                  <svg className="w-6 h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                }
              />
              <StatsCard
                title="Citas Completadas"
                count={completedAppointments.length}
                color="bg-green-100"
                icon={
                  <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                }
              />
              <StatsCard
                title="Citas Canceladas"
                count={cancelledAppointments.length}
                color="bg-red-100"
                icon={
                  <svg className="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                }
              />
            </div>

            {/* Tabs */}
            {appointments.length > 0 && (
              <div className="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden">
                <div className="border-b border-gray-200">
                  <nav className="flex">
                    {[
                      { id: 'pendientes' as const, label: 'Pendientes', count: pendingAppointments.length, color: 'yellow' },
                      { id: 'completadas' as const, label: 'Completadas', count: completedAppointments.length, color: 'green' },
                      { id: 'canceladas' as const, label: 'Canceladas', count: cancelledAppointments.length, color: 'red' }
                    ].map(tab => (
                      <button
                        key={tab.id}
                        onClick={() => setActiveTab(tab.id)}
                        className={`flex-1 py-4 px-6 text-sm font-medium text-center border-b-2 transition-colors ${
                          activeTab === tab.id
                            ? `border-${tab.color}-500 text-${tab.color}-600 bg-${tab.color}-50`
                            : 'border-transparent text-gray-500 hover:text-gray-700 hover:bg-gray-50'
                        }`}
                      >
                        <div className="flex items-center justify-center space-x-2">
                          <span className={`w-2 h-2 bg-${tab.color}-400 rounded-full`}></span>
                          <span>{tab.label} ({tab.count})</span>
                        </div>
                      </button>
                    ))}
                  </nav>
                </div>

                <div className="p-6">
                  {activeTab === 'pendientes' && (
                    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                      {pendingAppointments.map(apt => (
                        <AppointmentCard
                          key={apt.id}
                          appointment={apt}
                          isExpanded={expandedCard === apt.id}
                          onToggleExpand={setExpandedCard}
                          onCancel={handleCancelClick}
                          showCancelButton
                        />
                      ))}
                    </div>
                  )}

                  {activeTab === 'completadas' && (
                    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                      {completedAppointments.map(apt => (
                        <AppointmentCard
                          key={apt.id}
                          appointment={apt}
                          isExpanded={expandedCard === apt.id}
                          onToggleExpand={setExpandedCard}
                        />
                      ))}
                    </div>
                  )}

                  {activeTab === 'canceladas' && (
                    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                      {cancelledAppointments.map(apt => (
                        <AppointmentCard
                          key={apt.id}
                          appointment={apt}
                          isExpanded={expandedCard === apt.id}
                          onToggleExpand={setExpandedCard}
                        />
                      ))}
                    </div>
                  )}
                </div>
              </div>
            )}
          </div>
        )}
      </div>

      {/* Cancel Modal */}
      {selectedAppointment && (
        <CancelAppointmentModal
          appointment={selectedAppointment}
          onConfirm={handleCancelConfirm}
          onClose={() => setSelectedAppointment(null)}
        />
      )}

      {/* Footer */}
      <footer className="bg-gradient-to-r from-[#203461] to-[#1A6192] text-white py-12 mt-20">
        <div className="max-w-7xl mx-auto px-4">
          <div className="flex items-center space-x-3 mb-8">
            <Image
              src="https://www.electrohuila.com.co/wp-content/uploads/2024/07/cropped-logo-nuevo-eh.png.webp"
              alt="ElectroHuila Logo"
              className="h-10 w-auto object-contain filter brightness-0 invert"
              width={100}
              height={24}
            />
          </div>

          <div className="grid md:grid-cols-4 gap-6 text-sm">
            <div className="bg-white/10 rounded-xl p-4 backdrop-blur-sm">
              <div className="font-semibold mb-2">📞 Atención al Cliente</div>
              <div className="text-white/90">(608) 8664600</div>
            </div>
            <div className="bg-white/10 rounded-xl p-4 backdrop-blur-sm">
              <div className="font-semibold mb-2">🛠 Mesa de Ayuda</div>
              <div className="text-white/90">(608) 8664646</div>
            </div>
            <div className="bg-white/10 rounded-xl p-4 backdrop-blur-sm">
              <div className="font-semibold mb-2">📞 Línea Gratuita</div>
              <div className="text-white/90">018000952115</div>
            </div>
            <div className="bg-white/10 rounded-xl p-4 backdrop-blur-sm">
              <div className="font-semibold mb-2">📋 Transparencia</div>
              <div className="text-white/90">Línea PQR</div>
            </div>
          </div>

          <div className="mt-8 pt-8 border-t border-white/20 text-center text-white/70 text-sm">
            © {new Date().getFullYear()} ElectroHuila S.A. E.S.P. - Todos los derechos reservados
          </div>
        </div>
      </footer>
    </div>
  );
}
