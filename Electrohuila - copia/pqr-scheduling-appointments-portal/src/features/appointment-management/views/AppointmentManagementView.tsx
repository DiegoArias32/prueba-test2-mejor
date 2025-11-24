/**
 * Vista de Gestión de Citas
 * Componente UI para consultar y cancelar citas
 */

'use client';

import React from 'react';
import { useAppointmentManagement } from '../viewmodels/useAppointmentManagement';
import { FixedHeader } from '@/shared/layouts';
import { BackNavigation } from '@/shared/components';
import type { AppointmentDto } from '../models/appointment-management.models';

export const AppointmentManagementView: React.FC = () => {
  const {
    clientNumber,
    setClientNumber,
    isAuthenticated,
    client,
    filteredAppointments,
    stats,
    isLoading,
    error,
    successMessage,
    activeTab,
    setActiveTab,
    expandedCard,
    setExpandedCard,
    modalType,
    setModalType,
    selectedAppointment,
    setSelectedAppointment,
    cancelReason,
    setCancelReason,
    validationErrors,
    searchAppointments,
    cancelAppointment,
    closeModal,
    resetForm,
    isPending,
    isCompleted,
    isCancelled,
    getStatusColor,
    getStatusText,
    formatDate,
    formatTime
  } = useAppointmentManagement();

  // Componente de Estadísticas
  const StatsCard = ({ title, count, color, icon }: { title: string; count: number; color: string; icon: React.ReactNode }) => (
    <div className="bg-white rounded-2xl shadow-lg border border-gray-100 p-6 hover:shadow-xl transition-all duration-300">
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm font-medium text-gray-600 mb-1">{title}</p>
          <p className="text-3xl font-bold text-[#203461]">{count}</p>
        </div>
        <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${color}`}>
          {icon}
        </div>
      </div>
    </div>
  );

  // Componente de Tarjeta de Cita
  const AppointmentCard = ({ cita, isExpanded }: { cita: AppointmentDto; isExpanded: boolean }) => (
    <div className="bg-white rounded-2xl shadow-lg border border-gray-100 hover:shadow-xl transition-all duration-300">
      <div className="p-6">
        {/* Header de la tarjeta */}
        <div className="flex justify-between items-start mb-4">
          <div className="flex-1">
            <h4 className="text-lg font-bold text-[#203461] mb-1">{cita.appointmentTypeName}</h4>
            <p className="text-sm text-gray-600">#{cita.appointmentNumber}</p>
          </div>
          <div className="flex items-center space-x-2">
            <span className={`px-3 py-1 rounded-full text-xs font-medium border ${getStatusColor(cita.status)}`}>
              {getStatusText(cita.status)}
            </span>
            {isPending(cita.status) && (
              <button
                onClick={() => {
                  setSelectedAppointment(cita);
                  setModalType('cancel');
                }}
                className="text-red-600 hover:text-red-800 text-sm font-medium px-2 py-1 rounded-lg hover:bg-red-50 transition-colors"
              >
                Cancelar
              </button>
            )}
          </div>
        </div>

        {/* Información básica */}
        <div className="grid grid-cols-2 gap-4 mb-4">
          <div className="flex items-center text-gray-700">
            <svg className="w-4 h-4 text-[#56C2E1] mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            <span className="text-sm font-medium">{formatDate(cita.appointmentDate)}</span>
          </div>
          <div className="flex items-center text-gray-700">
            <svg className="w-4 h-4 text-[#56C2E1] mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <span className="text-sm">{formatTime(cita.appointmentTime)}</span>
          </div>
        </div>

        {/* Información expandible */}
        {isExpanded && (
          <div className="border-t border-gray-100 pt-4 space-y-3">
            <div className="flex items-center text-gray-700">
              <svg className="w-4 h-4 text-[#56C2E1] mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <span className="text-sm">{cita.branchName}</span>
            </div>

            {cita.assignedTechnician && (
              <div className="flex items-center text-gray-700">
                <svg className="w-4 h-4 text-[#56C2E1] mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
                <span className="text-sm">{cita.assignedTechnician}</span>
              </div>
            )}

            {cita.observations && (
              <div className={`rounded-xl p-3 ${
                isCancelled(cita.status) ? 'bg-red-50' :
                isCompleted(cita.status) ? 'bg-gray-50' : 'bg-blue-50'
              }`}>
                <p className={`text-sm ${
                  isCancelled(cita.status) ? 'text-red-700' :
                  isCompleted(cita.status) ? 'text-gray-700' : 'text-blue-700'
                }`}>
                  <span className="font-medium">
                    {isCancelled(cita.status) ? 'Motivo de cancelación:' : 'Observaciones:'}
                  </span> {cita.observations}
                </p>
              </div>
            )}

            {cita.technicianObservations && (
              <div className="bg-green-50 rounded-xl p-3">
                <p className="text-sm text-green-800">
                  <span className="font-medium">Observaciones del técnico:</span> {cita.technicianObservations}
                </p>
              </div>
            )}
          </div>
        )}

        {/* Botón de expandir/colapsar */}
        <button
          onClick={() => setExpandedCard(isExpanded ? null : cita.id)}
          className="w-full mt-4 py-2 text-sm text-[#1797D5] hover:text-[#203461] font-medium flex items-center justify-center transition-colors"
        >
          {isExpanded ? (
            <>
              <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 15l7-7 7 7" />
              </svg>
              Ver menos
            </>
          ) : (
            <>
              <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
              </svg>
              Ver detalles
            </>
          )}
        </button>
      </div>
    </div>
  );

  // Modal de cancelación
  const CancelModal = () => (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-2xl max-w-md w-full p-6">
        <h3 className="text-xl font-bold text-[#203461] mb-4">
          Cancelar Cita
        </h3>
        <p className="text-gray-600 mb-4">
          ¿Está seguro que desea cancelar la cita <strong>#{selectedAppointment?.appointmentNumber}</strong>?
        </p>

        <div className="mb-4">
          <label className="block text-gray-700 font-semibold mb-2">
            Motivo de cancelación
            <span className="text-red-500 ml-1">*</span>
          </label>
          <textarea
            value={cancelReason}
            onChange={(e) => setCancelReason(e.target.value)}
            rows={4}
            placeholder="Escriba el motivo de la cancelación (mínimo 10 caracteres)..."
            className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none resize-none ${
              validationErrors.cancelReason
                ? 'border-red-500 focus:border-red-500'
                : 'border-gray-300 focus:border-[#1797D5]'
            }`}
          />
          {validationErrors.cancelReason && (
            <p className="text-red-500 text-sm mt-1">{validationErrors.cancelReason}</p>
          )}
        </div>

        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm">
            {error}
          </div>
        )}

        <div className="flex gap-3">
          <button
            onClick={closeModal}
            disabled={isLoading}
            className="flex-1 px-4 py-2 border-2 border-gray-300 text-gray-700 rounded-lg font-semibold hover:bg-gray-50 transition-colors disabled:opacity-50"
          >
            Cancelar
          </button>
          <button
            onClick={cancelAppointment}
            disabled={isLoading}
            className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg font-semibold hover:bg-red-700 transition-colors disabled:opacity-50 flex items-center justify-center"
          >
            {isLoading ? (
              <>
                <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Procesando...
              </>
            ) : (
              'Confirmar Cancelación'
            )}
          </button>
        </div>
      </div>
    </div>
  );

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

        {/* Error Alert */}
        {error && !isAuthenticated && (
          <div className="mb-6 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl">
            <div className="flex items-center">
              <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
              </svg>
              {error}
            </div>
          </div>
        )}

        {/* Success Message */}
        {successMessage && (
          <div className="mb-6 bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-xl animate-fade-in">
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
          <div className="max-w-md mx-auto bg-white rounded-2xl shadow-xl p-8">
            <div className="text-center mb-6">
              <div className="inline-flex items-center justify-center w-16 h-16 bg-[#1797D5] rounded-full mb-4">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                </svg>
              </div>
              <h2 className="text-2xl font-bold text-[#203461] mb-2">
                Consultar Citas
              </h2>
              <p className="text-gray-600">
                Ingrese su número de cliente para ver sus citas
              </p>
            </div>

            <div className="mb-6">
              <label className="block text-gray-700 font-semibold mb-2">
                Número de Cliente
                <span className="text-red-500 ml-1">*</span>
              </label>
              <input
                type="text"
                value={clientNumber}
                onChange={(e) => setClientNumber(e.target.value)}
                onKeyPress={(e) => {
                  if (e.key === 'Enter') searchAppointments();
                }}
                placeholder="Ej: 1234567890"
                className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none transition-colors ${
                  validationErrors.clientNumber
                    ? 'border-red-500 focus:border-red-500'
                    : 'border-gray-300 focus:border-[#1797D5]'
                }`}
                disabled={isLoading}
              />
              {validationErrors.clientNumber && (
                <p className="text-red-500 text-sm mt-1">{validationErrors.clientNumber}</p>
              )}
            </div>

            <button
              onClick={searchAppointments}
              disabled={isLoading || !clientNumber}
              className="w-full bg-[#1797D5] text-white py-3 rounded-lg font-semibold hover:bg-[#147ab8] transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed flex items-center justify-center"
            >
              {isLoading ? (
                <>
                  <svg className="animate-spin -ml-1 mr-2 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Buscando...
                </>
              ) : (
                <>
                  <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                  Buscar Citas
                </>
              )}
            </button>
          </div>
        ) : (
          /* Appointments Dashboard */
          <>
            {/* Client Info */}
            {client && (
              <div className="bg-white rounded-2xl shadow-lg p-6 mb-8">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="text-xl font-bold text-[#203461] mb-2">
                      {client.fullName}
                    </h3>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-gray-600">
                      <div>
                        <span className="font-medium">Cliente:</span> {client.clientNumber}
                      </div>
                      <div>
                        <span className="font-medium">Email:</span> {client.email}
                      </div>
                      <div>
                        <span className="font-medium">Teléfono:</span> {client.phone || client.mobile || 'N/A'}
                      </div>
                    </div>
                  </div>
                  <button
                    onClick={resetForm}
                    className="px-4 py-2 text-sm text-gray-600 hover:text-gray-800 hover:bg-gray-100 rounded-lg transition-colors"
                  >
                    Cambiar usuario
                  </button>
                </div>
              </div>
            )}

            {/* Stats Cards */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
              <StatsCard
                title="Total"
                count={stats.total}
                color="bg-blue-100"
                icon={<svg className="w-6 h-6 text-[#1797D5]" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" /></svg>}
              />
              <StatsCard
                title="Pendientes"
                count={stats.pending}
                color="bg-yellow-100"
                icon={<svg className="w-6 h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>}
              />
              <StatsCard
                title="Completadas"
                count={stats.completed}
                color="bg-green-100"
                icon={<svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>}
              />
              <StatsCard
                title="Canceladas"
                count={stats.cancelled}
                color="bg-red-100"
                icon={<svg className="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>}
              />
            </div>

            {/* Tabs */}
            <div className="flex gap-2 mb-6 overflow-x-auto">
              <button
                onClick={() => setActiveTab('pendientes')}
                className={`px-6 py-3 rounded-lg font-semibold transition-colors whitespace-nowrap ${
                  activeTab === 'pendientes'
                    ? 'bg-[#1797D5] text-white'
                    : 'bg-white text-gray-700 hover:bg-gray-100'
                }`}
              >
                Pendientes ({stats.pending})
              </button>
              <button
                onClick={() => setActiveTab('completadas')}
                className={`px-6 py-3 rounded-lg font-semibold transition-colors whitespace-nowrap ${
                  activeTab === 'completadas'
                    ? 'bg-[#1797D5] text-white'
                    : 'bg-white text-gray-700 hover:bg-gray-100'
                }`}
              >
                Completadas ({stats.completed})
              </button>
              <button
                onClick={() => setActiveTab('canceladas')}
                className={`px-6 py-3 rounded-lg font-semibold transition-colors whitespace-nowrap ${
                  activeTab === 'canceladas'
                    ? 'bg-[#1797D5] text-white'
                    : 'bg-white text-gray-700 hover:bg-gray-100'
                }`}
              >
                Canceladas ({stats.cancelled})
              </button>
            </div>

            {/* Appointments List */}
            <div className="space-y-4">
              {activeTab === 'pendientes' && (
                filteredAppointments.pending.length === 0 ? (
                  <div className="text-center py-12 bg-white rounded-2xl">
                    <svg className="w-16 h-16 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                    </svg>
                    <p className="text-gray-600">No hay citas pendientes</p>
                  </div>
                ) : (
                  filteredAppointments.pending.map(cita => (
                    <AppointmentCard
                      key={cita.id}
                      cita={cita}
                      isExpanded={expandedCard === cita.id}
                    />
                  ))
                )
              )}

              {activeTab === 'completadas' && (
                filteredAppointments.completed.length === 0 ? (
                  <div className="text-center py-12 bg-white rounded-2xl">
                    <svg className="w-16 h-16 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <p className="text-gray-600">No hay citas completadas</p>
                  </div>
                ) : (
                  filteredAppointments.completed.map(cita => (
                    <AppointmentCard
                      key={cita.id}
                      cita={cita}
                      isExpanded={expandedCard === cita.id}
                    />
                  ))
                )
              )}

              {activeTab === 'canceladas' && (
                filteredAppointments.cancelled.length === 0 ? (
                  <div className="text-center py-12 bg-white rounded-2xl">
                    <svg className="w-16 h-16 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <p className="text-gray-600">No hay citas canceladas</p>
                  </div>
                ) : (
                  filteredAppointments.cancelled.map(cita => (
                    <AppointmentCard
                      key={cita.id}
                      cita={cita}
                      isExpanded={expandedCard === cita.id}
                    />
                  ))
                )
              )}
            </div>
          </>
        )}
      </div>

      {/* Cancel Modal */}
      {modalType === 'cancel' && <CancelModal />}
    </div>
  );
};
