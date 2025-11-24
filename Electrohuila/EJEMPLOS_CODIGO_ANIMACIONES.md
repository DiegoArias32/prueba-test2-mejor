# EJEMPLOS DE CÓDIGO - ANIMACIONES IMPLEMENTADAS

## 1. STATS CARD ANIMATION

**Archivo**: `/src/app/gestion-citas/page.tsx`

```typescript
const StatsCard = ({ title, count, color, icon }: StatsCardProps) => (
  <motion.div
    className="bg-white rounded-2xl shadow-lg border border-gray-100 p-6 hover:shadow-xl transition-all duration-300"
    whileHover={{ y: -5, boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)' }}
    whileTap={{ scale: 0.98 }}
    initial={{ opacity: 0, y: 20 }}
    animate={{ opacity: 1, y: 0 }}
    transition={{ duration: 0.4 }}
  >
    <div className="flex items-center justify-between">
      <div>
        <p className="text-sm font-medium text-gray-600 mb-1">{title}</p>
        <motion.p
          className="text-3xl font-bold text-[#203461]"
          initial={{ opacity: 0, scale: 0 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ delay: 0.2, type: "spring", stiffness: 200 }}
        >
          {count}
        </motion.p>
      </div>
      <motion.div
        className={`w-12 h-12 rounded-xl flex items-center justify-center ${color}`}
        whileHover={{ scale: 1.15, rotate: 5 }}
        transition={{ type: "spring", stiffness: 300 }}
      >
        {icon}
      </motion.div>
    </div>
  </motion.div>
);
```

**Efecto**: Card sube 5px en hover, número cuenta hacia arriba con spring, ícono rota en hover

---

## 2. MODAL CON BACKDROP BLUR

**Archivo**: `/src/app/gestion-citas/page.tsx`

```typescript
{modalType && selectedCita && (
  <motion.div
    className="fixed inset-0 bg-black bg-opacity-50 backdrop-blur-sm flex items-center justify-center p-4 z-50"
    initial={{ opacity: 0 }}
    animate={{ opacity: 1 }}
    exit={{ opacity: 0 }}
    transition={{ duration: 0.2 }}
  >
    <motion.div
      className="bg-white rounded-2xl shadow-xl max-w-md w-full"
      initial={{ opacity: 0, scale: 0.95, y: 20 }}
      animate={{ opacity: 1, scale: 1, y: 0 }}
      exit={{ opacity: 0, scale: 0.95, y: 20 }}
      transition={{ duration: 0.3, type: "spring", stiffness: 300 }}
    >
      {/* Modal Content */}
    </motion.div>
  </motion.div>
)}
```

**Efecto**: Fondo se oscurece y desenfoca, modal salta con spring physics

---

## 3. TITLED GRADIENT ANIMADO

**Archivo**: `/src/app/gestion-citas/page.tsx`

```typescript
<motion.div
  className="text-center mb-12"
  initial={{ opacity: 0, y: -30 }}
  animate={{ opacity: 1, y: 0 }}
  transition={{ duration: 0.5 }}
>
  <h1 className="text-5xl font-bold text-[#203461] mb-4">
    Gestión de
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
      Citas
    </motion.span>
  </h1>
</motion.div>
```

**Efecto**: Texto con gradiente que se mueve infinitamente de izquierda a derecha

---

## 4. ALERT CON FADE IN/OUT

**Archivo**: `/src/app/gestion-citas/page.tsx`

```typescript
<motion.div
  initial={{ opacity: 0, y: -20 }}
  animate={{ opacity: error ? 1 : 0, y: error ? 0 : -20 }}
  exit={{ opacity: 0, y: -20 }}
  transition={{ duration: 0.3 }}
  className="mb-6"
>
  {error && (
    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl">
      <div className="flex items-center">
        <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
          {/* Icon SVG */}
        </svg>
        {error}
      </div>
    </div>
  )}
</motion.div>
```

**Efecto**: Alert desliza desde arriba y desaparece hacia arriba

---

## 5. STAGGER LIST ANIMATION

**Archivo**: `/src/app/gestion-citas/page.tsx`

```typescript
{activeTab === 'pendientes' && (
  <motion.div
    initial={{ opacity: 0 }}
    animate={{ opacity: 1 }}
    transition={{ duration: 0.3 }}
  >
    {citasPendientes.length > 0 ? (
      <motion.div
        className="grid gap-4 md:grid-cols-2 lg:grid-cols-3"
        variants={{
          hidden: { opacity: 0 },
          visible: {
            opacity: 1,
            transition: {
              staggerChildren: 0.1,
              delayChildren: 0.2
            }
          }
        }}
        initial="hidden"
        animate="visible"
      >
        {citasPendientes.map((cita) => (
          <motion.div
            key={cita.id}
            variants={{
              hidden: { opacity: 0, y: 20 },
              visible: { opacity: 1, y: 0 }
            }}
          >
            <CitaCard
              cita={cita}
              isExpanded={expandedCard === cita.id}
            />
          </motion.div>
        ))}
      </motion.div>
    ) : (
      // Empty state
    )}
  </motion.div>
)}
```

**Efecto**: Cada card entra con delay de 0.1s, comenzando 0.2s después de la página cargar

---

## 6. BUTTON CON FEEDBACK MÚLTIPLE

**Archivo**: `/src/app/gestion-citas/page.tsx`

```typescript
<motion.div
  className="flex justify-end space-x-4"
  initial={{ opacity: 0, y: 10 }}
  animate={{ opacity: 1, y: 0 }}
  transition={{ delay: 0.3 }}
>
  <motion.button
    onClick={() => setModalType(null)}
    className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50"
    whileHover={{ scale: 1.05 }}
    whileTap={{ scale: 0.95 }}
  >
    Cancelar
  </motion.button>
  <motion.button
    onClick={handleCancelCita}
    disabled={!cancelReason || cancelReason.trim().length === 0}
    className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-50"
    whileHover={{ scale: 1.05 }}
    whileTap={{ scale: 0.95 }}
  >
    Confirmar Cancelación
  </motion.button>
</motion.div>
```

**Efecto**: Botones escalan al hacer hover y al hacer click

---

## 7. LOADING SPINNER MEJORADO

**Archivo**: `/src/features/verificar-cita/views/components/VerificacionLoading.tsx`

```typescript
<motion.div
  className="flex justify-center items-center py-16"
  initial={{ opacity: 0 }}
  animate={{ opacity: 1 }}
  transition={{ duration: 0.3 }}
>
  <motion.div
    className="bg-white rounded-2xl shadow-xl p-8 text-center"
    initial={{ opacity: 0, scale: 0.9 }}
    animate={{ opacity: 1, scale: 1 }}
    transition={{ delay: 0.2, type: "spring", stiffness: 200 }}
  >
    <motion.div
      className="w-16 h-16 border-4 border-[#56C2E1] border-t-transparent rounded-full mx-auto mb-4"
      animate={{ rotate: 360 }}
      transition={{
        duration: 1,
        repeat: Infinity,
        ease: "linear"
      }}
    />
    <motion.p
      className="text-gray-600 text-lg"
      animate={{ opacity: [0.6, 1, 0.6] }}
      transition={{
        duration: 2,
        repeat: Infinity,
        ease: "easeInOut"
      }}
    >
      Verificando cita...
    </motion.p>
  </motion.div>
</motion.div>
```

**Efecto**: Spinner rota continuamente, texto parpadea suavemente

---

## 8. CARD AVEC HOVER Y SHADOW ENHANCEMENT

**Archivo**: `/src/features/verificar-cita/views/components/VerificacionDetalle.tsx`

```typescript
<motion.div
  className="max-w-3xl mx-auto"
  initial={{ opacity: 0, y: 20 }}
  animate={{ opacity: 1, y: 0 }}
  transition={{ duration: 0.4 }}
>
  <motion.div
    className="bg-white rounded-2xl shadow-xl border border-gray-100 overflow-hidden"
    whileHover={{ boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)' }}
    transition={{ duration: 0.3 }}
  >
    {/* Content */}
  </motion.div>
</motion.div>
```

**Efecto**: Card se eleva visualmente en hover mediante cambio de sombra

---

## 9. BADGES CON SCALE ANIMATION

**Archivo**: `/src/app/unauthorized/page.tsx`

```typescript
<motion.div
  className="flex flex-wrap gap-2"
  initial={{ opacity: 0 }}
  animate={{ opacity: 1 }}
  transition={{ delay: 0.65, staggerChildren: 0.05 }}
>
  {Object.entries(permissionsDetailed.forms).map(([formCode, perms], index) => (
    <motion.div
      key={formCode}
      className="bg-gray-100 rounded-lg px-3 py-1 text-xs text-gray-700 border border-gray-200"
      initial={{ opacity: 0, scale: 0.8 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ delay: 0.65 + index * 0.05 }}
      whileHover={{ scale: 1.05 }}
    >
      <span className="font-semibold text-[#1797D5]">{formCode}:</span>
      {/* Permissions indicators */}
    </motion.div>
  ))}
</motion.div>
```

**Efecto**: Badges entran escalonadamente y escalan en hover

---

## 10. EMPTY STATE ANIMADO

**Archivo**: `/src/app/gestion-citas/page.tsx`

```typescript
{citas.length === 0 && (
  <motion.div
    className="text-center py-12"
    initial={{ opacity: 0, y: 20 }}
    animate={{ opacity: 1, y: 0 }}
    transition={{ delay: 0.4 }}
  >
    <motion.div
      className="w-20 h-20 mx-auto bg-gray-100 rounded-2xl flex items-center justify-center mb-6"
      whileHover={{ scale: 1.1, rotate: 5 }}
      transition={{ type: "spring", stiffness: 300 }}
    >
      <svg className="w-10 h-10 text-gray-400" {...svgProps} />
    </motion.div>
    <motion.h3
      className="text-xl font-semibold text-gray-600 mb-2"
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ delay: 0.5 }}
    >
      No hay citas registradas
    </motion.h3>
    <motion.p
      className="text-gray-500 mb-4"
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ delay: 0.6 }}
    >
      No se encontraron citas para este usuario.
    </motion.p>
    <motion.div
      whileHover={{ scale: 1.05 }}
      whileTap={{ scale: 0.95 }}
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ delay: 0.7 }}
    >
      <Link href="/agendamiento-citas" className="...">
        Agendar Nueva Cita
      </Link>
    </motion.div>
  </motion.div>
)}
```

**Efecto**: Ícono rota en hover, texto entra progresivamente, botón con feedback

---

## PATRÓN GENERAL DE IMPLEMENTACIÓN

### Paso 1: Importar Motion
```typescript
import { motion } from 'framer-motion';
```

### Paso 2: Reemplazar div por motion.div
```typescript
// Antes
<div className="...">

// Después
<motion.div
  className="..."
  initial={{ opacity: 0, y: 20 }}
  animate={{ opacity: 1, y: 0 }}
  transition={{ duration: 0.4 }}
>
```

### Paso 3: Agregar Hover Effects
```typescript
<motion.div
  whileHover={{ scale: 1.05, y: -5 }}
  whileTap={{ scale: 0.95 }}
>
```

### Paso 4: Para Listas, usar Variants
```typescript
<motion.div
  variants={{ hidden: { opacity: 0 }, visible: { opacity: 1 } }}
  initial="hidden"
  animate="visible"
  transition={{ staggerChildren: 0.1 }}
>
  {items.map((item, i) => (
    <motion.div key={i} variants={{ hidden: { opacity: 0 }, visible: { opacity: 1 } }}>
      {item}
    </motion.div>
  ))}
</motion.div>
```

---

## TESTING DE ANIMACIONES

### En desarrollo:
```bash
npm run dev
# Visitar cada página y verificar que las animaciones funcionen smoothly
```

### Verificar performance:
```typescript
// En Chrome DevTools -> Performance tab
// Grabar y verificar que animations no causen jank
// Frame rate debe ser 60fps o mayor
```

### Verificar en mobile:
```bash
# Usar device emulation en DevTools
# Verificar que animations no sean demasiado rápidas
# Verificar que taps funcionen correctamente
```

---

## NOTAS IMPORTANTES

1. **Always import motion**: Sin el import, los components no funcionarán
2. **Use className**: Las clases Tailwind funcionan con motion.div
3. **Spring es mejor**: Para UI elements, usa `type: "spring"` en lugar de durations fijas
4. **Stagger children**: Para listas, siempre usa el patrón de variants
5. **Exit animations**: Usa `exit` prop cuando hay AnimatePresence parent

---

**Última actualización**: 2024
**Versión**: 1.0
