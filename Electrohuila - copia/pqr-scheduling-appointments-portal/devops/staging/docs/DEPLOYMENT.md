# Guía de Despliegue - Ambiente Staging

## 📋 Información del Ambiente

- **Rama:** `staging`
- **Puerto:** 3002 (mapeado al 3000 interno)
- **URL:** http://localhost:3002
- **API Backend:** http://staging-api.electrohuila.local:8080

## 🚀 Comandos de Despliegue

### Despliegue Completo

```bash
# Navegar al directorio del proyecto
cd "c:\Users\XJuan\Desktop\ADSO - 2901817\PQR\pqr-scheduling-appointments-portal"

# Cambiar a rama Staging
git checkout staging
git pull origin staging

# Construir e iniciar el contenedor
cd devops/staging
docker-compose up -d --build

# Ver logs
docker-compose logs -f

# Detener
docker-compose down
```

### Despliegue con Tags (Versionado)

```bash
# Crear tag de versión
git tag -a v1.0.0-staging -m "Release v1.0.0 to staging"
git push origin v1.0.0-staging

# Checkout a tag específico
git checkout v1.0.0-staging

# Construir con tag
cd devops/staging
docker-compose build
docker tag pqr-frontend-staging:latest pqr-frontend-staging:v1.0.0
docker-compose up -d
```

### Rollback Seguro

```bash
# Listar tags disponibles
git tag -l

# Volver a versión anterior
git checkout staging
git reset --hard v0.9.0-staging

# Reconstruir
cd devops/staging
docker-compose up -d --build
```

## 🔧 Configuración

Editar `devops/staging/properties/.env.staging`:

```env
NEXT_PUBLIC_API_URL=http://staging-api.electrohuila.local:8080
NEXT_PUBLIC_ENV=staging
NODE_ENV=production
```

## 🧪 Testing y Validación

```bash
# Smoke tests básicos
curl -I http://localhost:3002
curl http://localhost:3002/api/health

# Verificar variables de entorno
docker exec pqr-frontend-staging env

# Verificar integración con backend
docker exec pqr-frontend-staging wget -q -O- http://staging-api.electrohuila.local:8080/health
```

## 📊 Monitoreo y Logs

```bash
# Logs en tiempo real
docker-compose logs -f

# Últimos 100 logs
docker-compose logs --tail=100

# Filtrar por severidad
docker logs pqr-frontend-staging 2>&1 | grep -E "ERROR|WARN"

# Estadísticas de uso
docker stats pqr-frontend-staging --no-stream
```

## 🔒 Seguridad

```bash
# Escanear vulnerabilidades en la imagen
docker scan pqr-frontend-staging:latest

# Verificar puertos expuestos
docker port pqr-frontend-staging

# Verificar permisos del usuario
docker exec pqr-frontend-staging whoami
```

## 🐛 Troubleshooting

### Performance Issues

```bash
# Ver uso de recursos
docker stats pqr-frontend-staging

# Aumentar límites de memoria (en docker-compose.yml)
services:
  pqr-frontend-staging:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '1.0'
```

### Conexión con Backend Falla

```bash
# Verificar conectividad
docker exec pqr-frontend-staging ping -c 3 staging-api.electrohuila.local

# Verificar DNS
docker exec pqr-frontend-staging nslookup staging-api.electrohuila.local

# Test directo de API
docker exec pqr-frontend-staging wget -q -O- http://staging-api.electrohuila.local:8080/actuator/health
```

### Limpiar y Reiniciar

```bash
# Full cleanup
docker-compose down -v
docker rmi pqr-frontend-staging:latest
docker system prune -f

# Reconstruir desde cero
docker-compose build --no-cache --pull
docker-compose up -d
```

## 📋 Checklist Pre-Producción

- [ ] Tests de QA aprobados
- [ ] Performance testing realizado
- [ ] Security scan sin vulnerabilidades críticas
- [ ] Validación con usuarios beta
- [ ] Documentación actualizada
- [ ] Plan de rollback preparado
- [ ] Monitoreo configurado
- [ ] Logs centralizados activos
- [ ] Alertas configuradas
- [ ] Backup de versión anterior disponible

## 🔄 Proceso de Promoción a Producción

1. Validar todos los tests en staging
2. Crear tag de producción: `git tag -a v1.0.0 -m "Production release v1.0.0"`
3. Merge a rama `main`: `git checkout main && git merge staging`
4. Push: `git push origin main --tags`
5. Desplegar en producción usando la carpeta `devops/main/`
