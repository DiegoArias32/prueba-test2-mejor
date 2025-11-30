# Guía de Testing - Paginación en Festivos

## Test Case 1: Carga Inicial de Página

### Precondiciones:
- App recién iniciada
- Estás en la pantalla de Festivos

### Pasos:
1. Observa la UI al cargar

### Resultados Esperados:
- [ ] Se muestran máximo 15 festivos
- [ ] PageInfo dice "Página 1 de X" (donde X > 1)
- [ ] Botón "Anterior" está DESHABILITADO (gris)
- [ ] Botón "Siguiente" está HABILITADO (azul)
- [ ] Stats cards muestran totales correctos:
  - Total = todos los festivos
  - Nacional = count correcto
  - Local = count correcto
  - Empresa = count correcto

### Console Debug Output Esperado:
```
✅ Loaded XXX holidays from backend
📈 Stats - Total: XXX, National: YYY, Local: ZZZ, Company: WWW
🔍 All filter applied: XXX/XXX | Page 1/67 | Showing 15 items
```

---

## Test Case 2: Navegar a Siguiente Página

### Precondiciones:
- Estás en Página 1
- Botón "Siguiente" está habilitado

### Pasos:
1. Haz click en botón "Siguiente"

### Resultados Esperados:
- [ ] PageInfo cambia a "Página 2 de X"
- [ ] Los 15 items mostrados cambian (son diferentes items)
- [ ] Botón "Anterior" ahora está HABILITADO (azul)
- [ ] Botón "Siguiente" permanece HABILITADO (si no es la última página)

### Verificación Adicional:
- Los items de la página 2 son:
  - Item índice 16 es el primero (después de los 15 de página 1)
  - Máximo 15 items nuevos

### Console Debug Output:
```
🔍 All filter applied: XXX/XXX | Page 2/67 | Showing 15 items
```

---

## Test Case 3: Navegar a Página Anterior

### Precondiciones:
- Estás en Página 2 o posterior
- Botón "Anterior" está habilitado

### Pasos:
1. Haz click en botón "Anterior"

### Resultados Esperados:
- [ ] PageInfo cambia a "Página 1 de X"
- [ ] Los items mostrados vuelven a los originales (mismos que Test Case 1)
- [ ] Botón "Anterior" vuelve a estar DESHABILITADO (gris)
- [ ] Botón "Siguiente" está HABILITADO (azul)

---

## Test Case 4: Última Página

### Precondiciones:
- Estás navegando en la lista

### Pasos:
1. Haz click en "Siguiente" hasta que alcances la última página
   (Ejemplo: Si hay 67 páginas, ve hasta Página 67)

### Resultados Esperados:
- [ ] PageInfo dice "Página 67 de 67"
- [ ] Se muestran menos de 15 items (items restantes)
- [ ] Botón "Siguiente" está DESHABILITADO (gris)
- [ ] Botón "Anterior" está HABILITADO (azul)

### Cálculo Esperado:
```
Si hay 1000 items y PageSize = 15:
- Página 67 = items índice 990-999 (10 items, no 15)
- TotalPages = ceil(1000/15) = ceil(66.67) = 67
```

---

## Test Case 5: Filtro por Tipo - Nacional

### Precondiciones:
- Estás en la pantalla de Festivos
- Estás en cualquier página

### Pasos:
1. Haz click en botón/chip "Nacional"

### Resultados Esperados:
- [ ] PageInfo vuelve a "Página 1 de Y" (donde Y puede ser menor que antes)
- [ ] Los 15 items mostrados ahora son SOLO Nacionales
- [ ] NationalCount en stats muestra el número correcto
- [ ] Stats card de Nacional se ve resaltada
- [ ] Si hay menos de 15 Nacionales totales:
  - [ ] Se muestran todos los disponibles
  - [ ] Botón "Siguiente" está DESHABILITADO

### Ejemplo:
```
Total: 1000
Nacional: 300
-> TotalPages = ceil(300/15) = 20 páginas
-> Página 1 muestra items 0-14 (Nacionales)
-> PageInfo: "Página 1 de 20"
```

---

## Test Case 6: Filtro por Tipo - Local

### Precondiciones:
- Estás en filtro "Nacional"

### Pasos:
1. Haz click en botón/chip "Local"

### Resultados Esperados:
- [ ] PageInfo vuelve a "Página 1 de Z" (diferente a Nacional)
- [ ] Los 15 items mostrados ahora son SOLO Locales
- [ ] LocalCount en stats muestra el número correcto
- [ ] Stats card de Local se ve resaltada
- [ ] TotalPages se recalcula para Locales

---

## Test Case 7: Filtro por Tipo - Empresa

### Precondiciones:
- Estás en cualquier filtro

### Pasos:
1. Haz click en botón/chip "Empresa"

### Resultados Esperados:
- [ ] PageInfo vuelve a "Página 1"
- [ ] Los items mostrados ahora son SOLO de Empresa
- [ ] CompanyCount en stats muestra el número correcto
- [ ] Stats card de Empresa se ve resaltada

---

## Test Case 8: Filtro "Todos"

### Precondiciones:
- Estás en un filtro específico (Nacional/Local/Empresa)

### Pasos:
1. Haz click en botón/chip "Todos"

### Resultados Esperados:
- [ ] PageInfo vuelve a "Página 1 de 67" (o el valor original)
- [ ] Los items mostrados vuelven a incluir todos los tipos
- [ ] TotalCount muestra el total combinado
- [ ] Stats card "Todos" se ve resaltada

---

## Test Case 9: Búsqueda con Paginación

### Precondiciones:
- Estás en Página 1 con filtro "Todos"
- El campo de búsqueda está visible

### Pasos:
1. Escribe "Navidad" en el campo de búsqueda
2. Observa los cambios

### Resultados Esperados:
- [ ] PageInfo vuelve a "Página 1 de X"
- [ ] Los items mostrados contienen la palabra "Navidad"
- [ ] X es el número de páginas de resultados (probablemente 1)
- [ ] Si hay menos de 15 resultados:
  - [ ] Se muestran todos
  - [ ] Botón "Siguiente" está DESHABILITADO

### Ejemplo:
```
Búsqueda "Navidad" resulta en 5 items
-> TotalPages = ceil(5/15) = 1
-> PageInfo: "Página 1 de 1"
-> Botón Siguiente DESHABILITADO
-> Botón Anterior DESHABILITADO
```

---

## Test Case 10: Búsqueda + Filtro Combinados

### Precondiciones:
- Estás en la pantalla de Festivos

### Pasos:
1. Haz click en filtro "Local"
2. Escribe "Bogotá" en búsqueda

### Resultados Esperados:
- [ ] PageInfo muestra "Página 1 de Y"
- [ ] Los items son SOLO Locales Y contienen "Bogotá"
- [ ] LocalCount sigue mostrando todos los Locales (no filtrados por búsqueda)
- [ ] Los items mostrados son un subconjunto de los Locales

### Lógica de Filtros:
```
Filtro aplicado EN ESTE ORDEN:
1. Primero: Filtro por tipo (Local)
2. Luego: Búsqueda en nombre/sede/fecha
3. Resultado: Intersección de ambos
```

---

## Test Case 11: Limpiar Búsqueda

### Precondiciones:
- Hay texto en la búsqueda
- Estás en Página 1 de resultados de búsqueda

### Pasos:
1. Haz click en el botón X (cancel) del SearchBar
   OR borra todo el texto manualmente

### Resultados Esperados:
- [ ] PageInfo vuelve a mostrar todas las páginas disponibles
- [ ] Los items mostrados ahora incluyen todos los festivos (sin búsqueda)
- [ ] TotalPages se recalcula sin la búsqueda

---

## Test Case 12: Refrescar Datos

### Precondiciones:
- Estás en cualquier página y filtro
- Has hecho cambios en backend (agregado/eliminado festivos)

### Pasos:
1. Haz click en botón Refresh (ícono de recarga)

### Resultados Esperados:
- [ ] Overlay de "Cargando..." aparece
- [ ] Se recarga la data del backend
- [ ] Vuelves a Página 1
- [ ] Los items y conteos se actualizan
- [ ] Overlay desaparece

### Console Debug Output:
```
🔄 Cache cleared - forcing fresh data from backend
✅ Loaded XXX holidays from backend
📈 Stats - Total: XXX, ...
```

---

## Test Case 13: Cache de 5 Minutos

### Precondiciones:
- Estás en la pantalla de Festivos
- Ya cargaste los datos una vez

### Pasos:
1. Navega a otra pantalla
2. Vuelve a Festivos en menos de 5 minutos
3. Observa los logs de consola

### Resultados Esperados:
- [ ] Los datos cargan instantáneamente
- [ ] En consola ves:
  ```
  ⚡ Using CACHED data (age: XXs)
  ```
- [ ] No hay delay de carga desde backend

### Pasos Alternos (después de 5+ minutos):
1. Navega a otra pantalla
2. Espera 5 minutos
3. Vuelve a Festivos

### Resultados Esperados:
- [ ] Los datos se recargan del backend (cache expirado)
- [ ] En consola ves:
  ```
  ✅ Loaded XXX holidays from backend
  ```

---

## Test Case 14: Eliminar Festivo y Actualizar Paginación

### Precondiciones:
- Estás viendo la lista de Festivos
- Hay al menos 2 festivos en la página actual

### Pasos:
1. Haz click en "Ver detalles" de un festivo
2. (Si hay botón de eliminar) Elimina el festivo
3. Confirma la eliminación
4. Observa la página

### Resultados Esperados:
- [ ] Se muestra mensaje de éxito
- [ ] Los datos se recargan automáticamente
- [ ] Vuelves a Página 1
- [ ] TotalCount disminuye en 1
- [ ] PageInfo se recalcula

### Nota:
- Actualmente la feature de eliminar muestra alerta, no elimina realmente

---

## Test Case 15: Edge Case - Una Página Exacta

### Precondiciones:
- Backend tiene exactamente 15 festivos

### Pasos:
1. Carga la pantalla de Festivos

### Resultados Esperados:
- [ ] Se muestran los 15 festivos
- [ ] PageInfo muestra "Página 1 de 1"
- [ ] Botón "Anterior" está DESHABILITADO (gris)
- [ ] Botón "Siguiente" está DESHABILITADO (gris)
- [ ] No hay necesidad de paginar

---

## Test Case 16: Edge Case - Cero Festivos

### Precondiciones:
- Backend retorna 0 festivos (hipotético)

### Pasos:
1. Carga la pantalla

### Resultados Esperados:
- [ ] Se muestra "No hay festivos"
- [ ] PageInfo muestra "Página 1 de 1"
- [ ] Ambos botones están DESHABILITADOS
- [ ] Stats muestran 0 en todo

---

## Test Case 17: Edge Case - Búsqueda sin Resultados

### Precondiciones:
- Estás en la pantalla

### Pasos:
1. Escribe "XXXYYYZZZWWW" (texto que no existe)

### Resultados Esperados:
- [ ] Se muestra "No hay festivos"
- [ ] PageInfo muestra "Página 1 de 1"
- [ ] Ambos botones están DESHABILITADOS
- [ ] Stats aún muestran los totales reales (no filtrados)

---

## Test Case 18: Performance - Scroll Suave

### Precondiciones:
- Estás en la pantalla con 15 items mostrados

### Pasos:
1. Intenta hacer scroll en la lista
2. Intenta hacer click rápidamente en filtros
3. Intenta cambiar de página rápidamente

### Resultados Esperados:
- [ ] El scroll es suave, sin lag
- [ ] Los filtros responden rápidamente
- [ ] No hay delay en cambiar de página
- [ ] No se congela la app

### Comparación:
```
ANTES (sin paginación):
- Con 100 items: lag noticeable
- Con filtro: lag muy notable

DESPUÉS (con paginación):
- Con 15 items: suave y rápido
- Con filtro: instantáneo
```

---

## Test Case 19: Presiona Siguiente en Última Página

### Precondiciones:
- Estás en la última página
- Botón "Siguiente" está DESHABILITADO

### Pasos:
1. Intenta hacer click en botón "Siguiente"
   (aunque debería estar deshabilitado visualmente)

### Resultados Esperados:
- [ ] No pasa nada (el botón no responde)
- [ ] CurrentPage sigue siendo el mismo
- [ ] Aún ves los mismos items

---

## Test Case 20: Presiona Anterior en Primera Página

### Precondiciones:
- Estás en página 1
- Botón "Anterior" está DESHABILITADO

### Pasos:
1. Intenta hacer click en botón "Anterior"
   (aunque debería estar deshabilitado)

### Resultados Esperados:
- [ ] No pasa nada
- [ ] CurrentPage sigue siendo 1
- [ ] Aún ves los mismos items

---

## Checklist de Testing Completo

Ejecuta todos estos tests antes de deployar:

- [ ] Test Case 1: Carga Inicial
- [ ] Test Case 2: Siguiente Página
- [ ] Test Case 3: Página Anterior
- [ ] Test Case 4: Última Página
- [ ] Test Case 5: Filtro Nacional
- [ ] Test Case 6: Filtro Local
- [ ] Test Case 7: Filtro Empresa
- [ ] Test Case 8: Filtro Todos
- [ ] Test Case 9: Búsqueda
- [ ] Test Case 10: Búsqueda + Filtro
- [ ] Test Case 11: Limpiar Búsqueda
- [ ] Test Case 12: Refrescar
- [ ] Test Case 13: Cache
- [ ] Test Case 14: Eliminar
- [ ] Test Case 15: 1 página exacta
- [ ] Test Case 16: 0 festivos
- [ ] Test Case 17: Búsqueda sin resultados
- [ ] Test Case 18: Performance
- [ ] Test Case 19: Siguiente en última
- [ ] Test Case 20: Anterior en primera

---

## Logs Esperados en Debug

### Al Cargar:
```
⚡ Using CACHED data (age: 0.5s)
🔍 All filter applied: 1000/1000 | Page 1/67 | Showing 15 items
```

### Al Filtrar:
```
🔍 National filter applied: 300/1000 | Page 1/20 | Showing 15 items
```

### Al Buscar:
```
🔍 National filter applied: 5/1000 | Page 1/1 | Showing 5 items
```

### Al Paginar:
```
🔍 National filter applied: 300/1000 | Page 2/20 | Showing 15 items
```

### Al Refrescar:
```
🔄 Cache cleared - forcing fresh data from backend
✅ Loaded 1000 holidays from backend
📈 Stats - Total: 1000, National: 300, Local: 400, Company: 300
🔍 All filter applied: 1000/1000 | Page 1/67 | Showing 15 items
```

---

## Notas Importantes para Testing

1. **Color de Botones:**
   - DESHABILITADO: #D0D5DD (gris claro)
   - HABILITADO: #203461 (azul oscuro)

2. **Iconos:**
   - Anterior: &#xf053; (flecha izquierda)
   - Siguiente: &#xf054; (flecha derecha)

3. **Máximo Items por Página:**
   - Modificable en ViewModel: `private int _pageSize = 15;`
   - Cambiar a 20 si necesitas

4. **Debug Console:**
   - Solo visible cuando ejecutas en DEBUG
   - Release no muestra los logs

5. **Performance Típico:**
   - Cambio de página: instantáneo (< 100ms)
   - Filtro: instantáneo (< 50ms)
   - Búsqueda: < 200ms dependiendo de la entrada

