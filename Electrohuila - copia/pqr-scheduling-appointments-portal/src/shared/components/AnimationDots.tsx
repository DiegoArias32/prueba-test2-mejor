/**
 * Animation Dots - Componente decorativo con puntos animados
 * Migrado desde src/components/animation-dots.tsx
 */

'use client';

export const AnimationDots: React.FC = () => {
  return (
    <>
      <div className="absolute -top-2 -right-2 w-4 h-4 bg-[#56C2E1] rounded-full animate-bounce opacity-60"></div>
      <div className="absolute -bottom-1 -left-1 w-3 h-3 bg-[#1797D5] rounded-full animate-bounce opacity-40"></div>
    </>
  );
};

export default AnimationDots;
