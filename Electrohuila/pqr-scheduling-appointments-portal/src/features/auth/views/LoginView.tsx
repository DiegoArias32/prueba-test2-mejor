/**
 * Vista de Login
 * Componente UI para autenticación con animaciones modernas
 */

'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { motion, AnimatePresence } from 'framer-motion';
import { User, Lock, ArrowLeft, AlertCircle, Eye, EyeOff } from 'lucide-react';
import { useAuth } from '../viewmodels/useAuth';
import { AnimatedButton, AnimatedInput, AnimatedAlert } from '@/shared/components/ui';

export const LoginView: React.FC = () => {
  const router = useRouter();
  const {
    isLoading,
    error,
    validationErrors,
    login,
    isAuthenticated,
  } = useAuth();

  const [formData, setFormData] = useState({
    usernameOrEmail: '',
    password: '',
  });

  const [touchedFields, setTouchedFields] = useState<{ [key: string]: boolean }>({});
  const [shouldCheckAuth, setShouldCheckAuth] = useState(true);
  const [showPassword, setShowPassword] = useState(false);

  useEffect(() => {
    const checkAuthTimeout = setTimeout(() => {
      if (shouldCheckAuth && isAuthenticated()) {
        router.push('/admin');
      }
    }, 500);

    return () => clearTimeout(checkAuthTimeout);
  }, [router, shouldCheckAuth, isAuthenticated]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleBlur = (field: string) => {
    setTouchedFields((prev) => ({
      ...prev,
      [field]: true,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setShouldCheckAuth(false);
    await login(formData);
  };

  const getFieldError = (field: string) => {
    return touchedFields[field] && validationErrors[field as keyof typeof validationErrors];
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-gray-50 flex items-center justify-center px-4 overflow-hidden relative">
      {/* Animated Background Blobs */}
      <motion.div
        className="absolute top-20 left-10 w-72 h-72 bg-[#1797D5] rounded-full blur-3xl opacity-20"
        animate={{
          scale: [1, 1.2, 1],
          x: [0, 50, 0],
          y: [0, 30, 0]
        }}
        transition={{
          duration: 8,
          repeat: Infinity,
          ease: "easeInOut"
        }}
      />
      <motion.div
        className="absolute bottom-20 right-10 w-96 h-96 bg-[#56C2E1] rounded-full blur-3xl opacity-20"
        animate={{
          scale: [1, 1.3, 1],
          x: [0, -50, 0],
          y: [0, -30, 0]
        }}
        transition={{
          duration: 10,
          repeat: Infinity,
          ease: "easeInOut"
        }}
      />

      {/* Floating particles */}
      {[...Array(15)].map((_, i) => (
        <motion.div
          key={i}
          className="absolute w-2 h-2 bg-[#56C2E1] rounded-full"
          style={{
            left: `${Math.random() * 100}%`,
            top: `${Math.random() * 100}%`,
          }}
          animate={{
            y: [0, -30, 0],
            opacity: [0.2, 0.5, 0.2]
          }}
          transition={{
            duration: 3 + Math.random() * 2,
            repeat: Infinity,
            delay: Math.random() * 2
          }}
        />
      ))}

      <motion.div
        className="relative w-full max-w-md z-10"
        initial={{ opacity: 0, y: 40 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.6 }}
      >
        {/* Header con Logo */}
        <motion.div
          className="text-center mb-8"
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2 }}
        >
          <Link href="/" className="inline-block mb-6">
            <motion.img
              src="https://www.electrohuila.com.co/wp-content/uploads/2024/07/cropped-logo-nuevo-eh.png.webp"
              alt="ElectroHuila Logo"
              className="h-16 w-auto object-contain mx-auto"
              width="160"
              height="38"
              whileHover={{ scale: 1.05 }}
              transition={{ duration: 0.2 }}
            />
          </Link>

          <motion.h1
            className="text-4xl font-bold text-[#203461] mb-2"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.3 }}
          >
            Iniciar Sesión
          </motion.h1>

          <motion.p
            className="text-gray-600"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.4 }}
          >
            Accede al panel de administración del sistema
          </motion.p>
        </motion.div>

        {/* Login Card */}
        <motion.div
          className="group relative bg-white rounded-3xl shadow-2xl border border-gray-100 overflow-hidden"
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ delay: 0.5, duration: 0.4 }}
          whileHover={{ y: -5 }}
        >
          {/* Gradient Top Border */}
          <div className="bg-gradient-to-r from-[#203461] via-[#1797D5] to-[#56C2E1] h-2" />

          <div className="p-8">
            {/* Error Alert */}
            <AnimatePresence>
              {error && (
                <motion.div
                  initial={{ opacity: 0, y: -10 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, y: -10 }}
                  className="mb-6"
                >
                  <AnimatedAlert
                    type="error"
                    message={error}
                    autoDismiss={false}
                  />
                </motion.div>
              )}
            </AnimatePresence>

            <form onSubmit={handleSubmit} className="space-y-6">
              {/* Username Field */}
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ delay: 0.6 }}
              >
                <AnimatedInput
                  label="Usuario o Email"
                  value={formData.usernameOrEmail}
                  onChange={(value) => setFormData(prev => ({ ...prev, usernameOrEmail: value }))}
                  onBlur={() => handleBlur('usernameOrEmail')}
                  error={getFieldError('usernameOrEmail')}
                  placeholder="Ingresa tu usuario o email"
                  required
                  icon={<User className="h-5 w-5 text-gray-400" />}
                />
              </motion.div>

              {/* Password Field */}
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ delay: 0.7 }}
              >
                <label className="block text-sm font-semibold text-[#203461] mb-2">
                  Contraseña <span className="text-red-500">*</span>
                </label>
                <div className="relative">
                  <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <Lock className="h-5 w-5 text-gray-400" />
                  </div>
                  <input
                    type={showPassword ? 'text' : 'password'}
                    name="password"
                    value={formData.password}
                    onChange={handleInputChange}
                    onBlur={() => handleBlur('password')}
                    className={`w-full pl-10 pr-12 py-3 border-2 rounded-xl focus:ring-2 focus:border-transparent transition-all duration-300 ${
                      getFieldError('password')
                        ? 'border-red-500 focus:ring-red-500'
                        : 'border-gray-300 focus:ring-[#1797D5] focus:border-[#1797D5]'
                    }`}
                    placeholder="Ingresa tu contraseña"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600"
                  >
                    {showPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                  </button>
                </div>
                <AnimatePresence>
                  {getFieldError('password') && (
                    <motion.div
                      initial={{ opacity: 0, y: -10 }}
                      animate={{ opacity: 1, y: 0 }}
                      exit={{ opacity: 0, y: -10 }}
                      className="text-red-600 text-sm mt-1 flex items-center"
                    >
                      <AlertCircle className="w-4 h-4 mr-1" />
                      {getFieldError('password')}
                    </motion.div>
                  )}
                </AnimatePresence>
              </motion.div>

              {/* Submit Button */}
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.8 }}
              >
                <AnimatedButton
                  type="submit"
                  disabled={isLoading}
                  loading={isLoading}
                  fullWidth
                  size="lg"
                >
                  Iniciar Sesión
                </AnimatedButton>
              </motion.div>
            </form>
          </div>
        </motion.div>

        {/* Back to Home Link */}
        <motion.div
          className="text-center mt-6"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 1 }}
        >
          <Link
            href="/"
            className="inline-flex items-center text-[#1A6192] hover:text-[#203461] font-medium transition-colors duration-300 group"
          >
            <motion.div
              whileHover={{ x: -5 }}
              transition={{ duration: 0.2 }}
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
            </motion.div>
            Volver al Inicio
          </Link>
        </motion.div>
      </motion.div>
    </div>
  );
};
