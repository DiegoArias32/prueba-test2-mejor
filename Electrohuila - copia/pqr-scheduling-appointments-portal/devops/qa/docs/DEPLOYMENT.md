# Guía de Despliegue - Ambiente QA

## 📋 Información del Ambiente

- **Rama:** `qa`
- **Puerto:** 3001 (mapeado al 3000 interno)
- **URL:** http://localhost:3001
- **API Backend:** http://qa-api.electrohuila.local:8080

## 🚀 Comandos de Despliegue

### Despliegue Completo

```bash
# Navegar al directorio del proyecto
cd "c:\Users\XJuan\Desktop\ADSO - 2901817\PQR\pqr-scheduling-appointments-portal"

# Cambiar a rama QA
git checkout qa
git pull origin qa

# Construir e iniciar el contenedor
cd devops/qa
docker-compose up -d --build

# Ver logs
docker-compose logs -f

# Detener
docker-compose down
```

### Actualización Rápida (Hot Deploy)

```bash
# Después de hacer merge a la rama qa
git checkout qa
git pull origin qa

# Reconstruir sin caché
cd devops/qa
docker-compose build --no-cache
docker-compose up -d

# Verificar logs
docker-compose logs -f --tail=100
```

### Rollback a Versión Anterior

```bash
# Ver commits recientes
git log --oneline -10

# Revertir a commit específico
git checkout qa
git reset --hard <commit-hash>

# Reconstruir
cd devops/qa
docker-compose up -d --build
```

## 🔧 Configuración

Editar `devops/qa/properties/.env.qa`:

```env
NEXT_PUBLIC_API_URL=http://qa-api.electrohuila.local:8080
NEXT_PUBLIC_ENV=qa
NODE_ENV=production
```

## 🧪 Testing Post-Despliegue

```bash
# Verificar que el contenedor está corriendo
docker ps | grep pqr-frontend-qa

# Verificar health check
docker inspect pqr-frontend-qa --format='{{.State.Health.Status}}'

# Probar endpoint
curl http://localhost:3001

# Ver logs en tiempo real
docker logs -f pqr-frontend-qa
```

## 📊 Monitoreo

```bash
# Ver uso de recursos
docker stats pqr-frontend-qa

# Ver logs con timestamps
docker logs -f --timestamps pqr-frontend-qa

# Filtrar logs por error
docker logs pqr-frontend-qa 2>&1 | grep -i error
```

## 🐛 Troubleshooting

### El contenedor se detiene inmediatamente

```bash
# Ver logs de error
docker logs pqr-frontend-qa

# Verificar el build
docker-compose build --no-cache
docker-compose up
```

### Cambios no se reflejan

```bash
# Limpiar todo y reconstruir
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

### Error de conectividad con backend

```bash
# Verificar que el backend QA esté activo
curl http://qa-api.electrohuila.local:8080/health

# Verificar DNS
ping qa-api.electrohuila.local

# Verificar variables de entorno
docker exec pqr-frontend-qa env | grep API_URL
```

## 📝 Checklist de Despliegue

- [ ] Pull de la última versión de la rama `qa`
- [ ] Verificar que las variables de entorno estén correctas
- [ ] Construir imagen sin caché si hay cambios importantes
- [ ] Verificar que el contenedor inicie correctamente
- [ ] Probar funcionalidades críticas
- [ ] Verificar integración con backend QA
- [ ] Documentar cualquier issue encontrado
