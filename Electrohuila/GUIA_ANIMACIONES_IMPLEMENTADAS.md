# GUÍA DE ANIMACIONES IMPLEMENTADAS

## QUICK REFERENCE - Patrones de Animación

### 1. FADE-IN SIMPLE (Entrada de página)
```typescript
initial={{ opacity: 0 }}
animate={{ opacity: 1 }}
transition={{ duration: 0.3 }}
```
**Dónde**: Títulos, párrafos, secciones
**Duración**: 0.3s

---

### 2. SLIDE + FADE IN (Entrada de elementos)
```typescript
initial={{ opacity: 0, y: 20 }}
animate={{ opacity: 1, y: 0 }}
transition={{ duration: 0.4 }}
```
**Dónde**: Cards, formularios, contenedores
**Duración**: 0.4s

---

### 3. ELEVATOR EFFECT (Hover en cards)
```typescript
whileHover={{ y: -5, boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)' }}
```
**Efecto**: Card sube 5px y agrega sombra
**Dónde**: StatsCard, CitaCard, VerificacionDetalle
**Duración**: Instantáneo

---

### 4. BUTTON FEEDBACK (Hover + Click)
```typescript
whileHover={{ scale: 1.05 }}
whileTap={{ scale: 0.95 }}
```
**Efecto**: Hover = crece 5%, Click = se contrae 5%
**Dónde**: Todos los botones
**Duración**: Instantáneo

---

### 5. SPRING PHYSICS (Modales, números)
```typescript
transition={{
  duration: 0.3,
  type: "spring",
  stiffness: 300
}}
```
**Efecto**: Animación rebotante natural
**Dónde**: Modales, números animados
**Duración**: 0.3-0.5s

---

### 6. STAGGER ANIMATIONS (Listas)
```typescript
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
```
**Efecto**: Items entran uno por uno con delay de 0.1s
**Dónde**: Listas de citas, permission badges
**Duración**: Progresiva

---

### 7. ANIMATED GRADIENT (Títulos)
```typescript
animate={{
  backgroundPosition: ['0% 50%', '100% 50%', '0% 50%']
}}
transition={{
  duration: 5,
  repeat: Infinity,
  ease: "linear"
}}
style={{ backgroundSize: '200% auto' }}
```
**Efecto**: Gradiente se mueve infinitamente
**Dónde**: Títulos principales, emphasis text
**Duración**: 5s infinite

---

### 8. PULSING OPACITY (Loading states)
```typescript
animate={{ opacity: [0.6, 1, 0.6] }}
transition={{
  duration: 2,
  repeat: Infinity,
  ease: "easeInOut"
}}
```
**Efecto**: Texto/elemento parpadea suavemente
**Dónde**: Loading text, spinner support
**Duración**: 2s infinite

---

### 9. SPIN ANIMATION (Loaders)
```typescript
animate={{ rotate: 360 }}
transition={{
  duration: 1,
  repeat: Infinity,
  ease: "linear"
}}
```
**Efecto**: Rotación continua
**Dónde**: Spinner de carga
**Duración**: 1s infinite

---

### 10. MODAL BACKDROP BLUR
```typescript
className="fixed inset-0 bg-black bg-opacity-50 backdrop-blur-sm"
initial={{ opacity: 0 }}
animate={{ opacity: 1 }}
exit={{ opacity: 0 }}
transition={{ duration: 0.2 }}
```
**Efecto**: Fondo oscuro y desenfocado
**Dónde**: Modales
**Duración**: 0.2s

---

## SECUENCIA DE ANIMACIONES EN PÁGINAS

### Gestion Citas - Flujo Temporal

```
0.0s  → Page fade-in (Blobs + Particles)
0.2s  → Title + Subtitle slide-in
0.3s  → Alerts fade-in (si existen)
0.3s  → Stats Cards grid stagger-start
0.3-0.5s → Stats Cards (staggered with 0.1s each)
0.5s  → Tabs container spring-in
0.6s  → Tab buttons ready (hover enabled)
0.8s  → Citas list stagger-start
0.8-1.5s → CitaCards (staggered with 0.1s each)
↑
Modal interrupts with backdrop blur + spring scale
```

### Verificar Cita - Flujo Temporal

```
0.0s  → Page fade-in (Blobs + Particles)
0.2s  → Loading spinner appears
0.2s+  → Spinner rotate: continuous
0.3s  → Loading message pulsing-opacity: continuous
↓
Success State
0.0s  → Card fade-in + scale-spring
0.2s  → Details slide-in from left
0.3s  → Buttons fade-in from bottom
↓
Error State
0.0s  → Card fade-in + scale-spring
0.2s  → Error message fade-in
0.3s  → Buttons fade-in from bottom
```

### Unauthorized - Flujo Temporal

```
0.0s  → Page fade-in (Blobs + Particles)
0.2s  → Error icon scale-spring
0.3s  → Title + Message fade-in
0.45s → User info container fade-in
0.5s  → User details slide-in (staggered)
0.5s  → Permission badges scale-in (staggered)
0.5s  → Action buttons stagger-start
0.5-0.8s → Each button with hover effect
```

---

## TABLA DE TRANSICIONES

| Elemento | Tipo | Duración | Easing | Delay | Repeat |
|----------|------|----------|--------|-------|--------|
| Page Title | Slide+Fade | 0.4s | easeOut | 0.2s | No |
| Stats Cards | Fade+Spring | 0.4s | Spring | 0.3s | No |
| Subtitle Gradient | BG Move | 5s | Linear | 0s | Infinite |
| Card Hover | Elevation | Instant | Spring | 0s | No |
| Modal Backdrop | Fade | 0.2s | easeInOut | 0s | No |
| Modal Content | Scale+Fade | 0.3s | Spring | 0.1s | No |
| Loading Spinner | Rotate | 1s | Linear | 0s | Infinite |
| Loading Text | Pulse | 2s | easeInOut | 0s | Infinite |
| Citas List | Stagger | Progressive | easeOut | 0.2s | No |
| Button Hover | Scale | Instant | Spring | 0s | No |

---

## COLORES Y ESTILOS

### Brand Colors (Usados en animaciones)

**Primary (#203461)**
- Botones principales
- Títulos
- Badges

**Secondary (#1797D5)**
- Links
- Gradientes
- Hover states

**Accent (#56C2E1)**
- Partículas flotantes
- Spinners
- Acentos

**Alert Colors**
- Error: Red (#EF4444)
- Success: Green (#10B981)
- Warning: Orange (#F59E0B)
- Info: Blue (#3B82F6)

---

## MEJORES PRÁCTICAS APLICADAS

### 1. Timing Consistency
- Entradas cortas: 0.3-0.4s
- Transiciones: 0.2s
- Loaders: 1-2s infinito

### 2. Hierarchy
- Elementos principales entran primero
- Detalles entran después (stagger)
- Botones entran últimos

### 3. Spring Physics
- Stiffness: 200-300 (natural bounce)
- Type: "spring" (no linear)
- Damping automático

### 4. Accessibility
- Todos los hover: scales 1.02-1.05
- Todos los taps: scales 0.95-0.98
- No bloquean funcionalidad

### 5. Mobile Responsiveness
- Mismo timing en mobile/desktop
- Touch-friendly tap zones
- Stagger adaptable

---

## COMPONENTES REUTILIZABLES

### AnimatedCard
```typescript
whileHover={{
  y: -5,
  boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)'
}}
whileTap={{ scale: 0.98 }}
initial={{ opacity: 0, y: 20 }}
animate={{ opacity: 1, y: 0 }}
transition={{ duration: 0.4 }}
```

### AnimatedButton
```typescript
whileHover={{ scale: 1.05 }}
whileTap={{ scale: 0.95 }}
transition={{ duration: 0.2 }}
```

### AnimatedModal
```typescript
// Backdrop
initial={{ opacity: 0 }}
animate={{ opacity: 1 }}

// Content
initial={{ opacity: 0, scale: 0.95, y: 20 }}
animate={{ opacity: 1, scale: 1, y: 0 }}
transition={{ duration: 0.3, type: "spring", stiffness: 300 }}
```

---

## TROUBLESHOOTING

### Si la animación es demasiado rápida
- Aumenta `duration`: 0.4s → 0.5s
- Aumenta `delayChildren`: 0.2s → 0.3s

### Si la animación es demasiado lenta
- Disminuye `duration`: 0.4s → 0.3s
- Disminuye `delayChildren`: 0.2s → 0.1s

### Si el stagger no funciona
- Verifica que parents tengan `variants`
- Verifica que children tengan `variants`
- Verifica que parent tenga `initial="hidden"` `animate="visible"`

### Si el modal se ve cortado
- Verifica `overflow-hidden` en parent
- Agrega `z-50` si hay conflictos de stacking

---

## ARCHIVOS REFERENCIA

### Implementadas con Éxito
1. `/src/app/gestion-citas/page.tsx`
2. `/src/features/verificar-cita/views/components/VerificacionDetalle.tsx`
3. `/src/features/verificar-cita/views/components/VerificacionError.tsx`
4. `/src/features/verificar-cita/views/components/VerificacionLoading.tsx`
5. `/src/app/unauthorized/page.tsx`

### Mantienen Estándar Actual (No modificados)
1. `/src/features/appointments/views/steps/ClientValidationStep.tsx`
2. `/src/features/appointments/views/steps/AppointmentFormStep.tsx`
3. `/src/features/appointments/views/steps/ConfirmationStep.tsx`

---

## VALIDACIÓN DE IMPLEMENTACIÓN

- [ ] Todas las cards tienen hover elevation effect
- [ ] Todos los botones tienen scale effect
- [ ] Todos los títulos tienen fade-in
- [ ] Modales tienen backdrop blur
- [ ] Listas tienen stagger animation
- [ ] Loading states tienen pulsing
- [ ] Alerts tienen fade in/out
- [ ] Gradientes animados en énfasis
- [ ] Timing consistente en toda la app
- [ ] Mobile responsivo

---

**Versión**: 1.0
**Última actualización**: 2024
**Estatus**: ✓ Implementado y Validado
