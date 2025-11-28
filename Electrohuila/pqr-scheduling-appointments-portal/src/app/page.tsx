"use client";

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { FixedHeader } from '@/shared/layouts';
import { Footer } from '@/shared/components';
import { Calendar, Sparkles, ArrowRight } from 'lucide-react';

export default function Home() {
  const router = useRouter();
  const [, setKeySequence] = useState('');

  useEffect(() => {
    const handleKeyPress = (event: KeyboardEvent) => {
      if (event.key.match(/^[a-zA-Z0-9]$/)) {
        setKeySequence(prev => {
          const newSequence = (prev + event.key).toLowerCase();

          if (newSequence.includes('admin123')) {
            setTimeout(() => router.push('/login'), 0);
            return '';
          }

          return newSequence.slice(-10);
        });
      }
    };

    document.addEventListener('keydown', handleKeyPress);
    return () => document.removeEventListener('keydown', handleKeyPress);
  }, [router]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-gray-50 overflow-x-hidden">
      <FixedHeader />

      {/* Hero Section */}
      <section className="relative overflow-hidden pt-20">
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
        {[...Array(20)].map((_, i) => (
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

        <div className="relative max-w-6xl mx-auto px-4 py-20">
          {/* Hero Content */}
          <motion.div
            className="text-center mb-16"
            initial={{ opacity: 0, y: 30 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6 }}
          >


            <motion.h1
              className="text-6xl md:text-7xl font-bold text-[#203461] mb-4 leading-tight"
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.3 }}
            >
              Sistema de
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
                Servicios
              </motion.span>
            </motion.h1>

            <motion.h2
              className="text-3xl font-semibold text-[#203461] mb-6"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.6, delay: 0.5 }}
            >
              ElectroHuila
            </motion.h2>
          </motion.div>

          {/* Main Service Card */}
          <motion.div
            className="max-w-2xl mx-auto"
            initial={{ opacity: 0, y: 40 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6, delay: 0.6 }}
          >
            <motion.div
              className="group relative bg-white rounded-3xl shadow-2xl border border-gray-100 overflow-hidden"
              whileHover={{ y: -8 }}
              transition={{ duration: 0.3 }}
            >
              {/* Gradient Border Effect */}
              <div className="absolute inset-0 bg-gradient-to-r from-[#203461] via-[#1797D5] to-[#56C2E1] opacity-0 group-hover:opacity-100 transition-opacity duration-500" />

              <div className="relative bg-white rounded-3xl m-[2px] p-8">
                {/* Icon Container with Animation */}
                <motion.div
                  className="mb-8 relative"
                  whileHover={{ scale: 1.05, rotate: [0, -5, 5, 0] }}
                  transition={{ duration: 0.5 }}
                >
                  <div className="w-24 h-24 mx-auto bg-gradient-to-br from-[#97D4E3] to-[#56C2E1] rounded-2xl flex items-center justify-center shadow-lg">
                    <Calendar className="w-12 h-12 text-[#203461]" strokeWidth={2.5} />
                  </div>

                  {/* Floating Elements with Stagger Animation */}
                  {[
                    { size: 'w-4 h-4', pos: '-top-2 -right-2', delay: 0 },
                    { size: 'w-3 h-3', pos: '-bottom-1 -left-1', delay: 0.2 },
                    { size: 'w-2 h-2', pos: 'top-2 -left-3', delay: 0.4 }
                  ].map((item, i) => (
                    <motion.div
                      key={i}
                      className={`absolute ${item.pos} ${item.size} bg-[#56C2E1] rounded-full`}
                      animate={{
                        y: [0, -10, 0],
                        scale: [1, 1.2, 1],
                        opacity: [0.6, 1, 0.6]
                      }}
                      transition={{
                        duration: 2,
                        repeat: Infinity,
                        delay: item.delay
                      }}
                    />
                  ))}
                </motion.div>

                <div className="text-center">
                  <h3 className="text-3xl font-bold text-[#203461] mb-4">
                    Sistema de Gestión de Citas
                  </h3>

                  <p className="text-gray-600 text-lg mb-8 leading-relaxed">
                    Gestione todos sus servicios eléctricos de forma rápida y segura:
                    <span className="font-semibold text-[#1A6192]"> agende citas, consulte estados de las citas</span>
                  </p>

                  {/* Features Grid */}
                  <motion.div
                    className="grid grid-cols-2 gap-4 mb-8"
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: 0.8 }}
                  >
                    {[
                      { icon: Calendar, text: 'Agendar Citas' },
                      { icon: Calendar, text: 'Consultar Citas' }
                    ].map((feature, index) => (
                      <motion.div
                        key={index}
                        className="flex items-center gap-2 text-[#1A6192] bg-blue-50 rounded-xl p-3"
                        initial={{ opacity: 0, x: -20 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: 0.9 + index * 0.1 }}
                        whileHover={{ scale: 1.05, backgroundColor: '#E0F2FE' }}
                      >
                        <div className="w-2 h-2 bg-[#56C2E1] rounded-full" />
                        <span className="font-medium text-sm">{feature.text}</span>
                      </motion.div>
                    ))}
                  </motion.div>

                  {/* CTA Button */}
                  <Link href="/servicios">
                    <motion.div
                      className="inline-flex items-center justify-center w-full bg-gradient-to-r from-[#203461] to-[#1797D5] text-white px-8 py-4 rounded-xl font-semibold shadow-lg cursor-pointer overflow-hidden relative group"
                      whileHover={{ scale: 1.02 }}
                      whileTap={{ scale: 0.98 }}
                    >
                      {/* Shimmer Effect */}
                      <motion.div
                        className="absolute inset-0 bg-gradient-to-r from-transparent via-white/20 to-transparent"
                        initial={{ x: '-100%' }}
                        animate={{ x: '100%' }}
                        transition={{
                          duration: 2,
                          repeat: Infinity,
                          repeatDelay: 1
                        }}
                      />

                      <span className="relative z-10">Acceder al Sistema</span>

                      <motion.div
                        className="relative z-10 ml-2"
                        animate={{ x: [0, 5, 0] }}
                        transition={{
                          duration: 1.5,
                          repeat: Infinity,
                          ease: "easeInOut"
                        }}
                      >
                        <ArrowRight className="w-5 h-5" />
                      </motion.div>
                    </motion.div>
                  </Link>
                </div>
              </div>
            </motion.div>
          </motion.div>
        </div>
      </section>

      <Footer />
    </div>
  );
}
