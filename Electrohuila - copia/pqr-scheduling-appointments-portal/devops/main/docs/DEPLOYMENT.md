# Guía de Despliegue - Ambiente Producción

## 📋 Información del Ambiente

- **Rama:** `main`
- **Puerto:** 80 (HTTP) y 443 (HTTPS)
- **URL:** https://pqr.electrohuila.com.co (o tu dominio de producción)
- **API Backend:** https://api.electrohuila.com.co

## ⚠️ IMPORTANTE: Consideraciones de Producción

### Antes de Desplegar a Producción

- [ ] **Código validado en Staging** - Todo funciona perfectamente
- [ ] **Tests completos ejecutados** - Unit, Integration, E2E
- [ ] **Performance testing aprobado** - Carga y velocidad óptimas
- [ ] **Security scan realizado** - Sin vulnerabilidades críticas
- [ ] **Documentación actualizada** - README, CHANGELOG
- [ ] **Plan de rollback preparado** - Cómo volver atrás si falla
- [ ] **Monitoreo configurado** - Alertas y logs
- [ ] **Backup realizado** - De la versión anterior
- [ ] **Comunicación a usuarios** - Si hay cambios importantes
- [ ] **Ventana de mantenimiento** - Si es necesario downtime

---

## 🚀 Comandos de Despliegue

### Despliegue Completo (Recomendado)

```bash
# 1. Asegurarse de estar en la rama main
git checkout main
git pull origin main

# 2. Verificar que todo esté listo
git log --oneline -5
git status

# 3. Navegar al directorio de producción
cd devops/main

# 4. Construir e iniciar el contenedor
docker-compose up -d --build

# 5. Verificar logs inmediatamente
docker-compose logs -f --tail=100
```

### Despliegue con Versionado (Mejor Práctica)

```bash
# 1. Crear tag de versión
git tag -a v1.0.0 -m "Production release v1.0.0"
git push origin v1.0.0

# 2. Verificar tag
git describe --tags

# 3. Desplegar
cd devops/main
docker-compose up -d --build

# 4. Etiquetar la imagen Docker con versión
docker tag pqr-frontend-prod:latest pqr-frontend-prod:v1.0.0
```

---

## 🔐 Configuración de SSL/HTTPS (Producción Real)

### Opción 1: Nginx como Reverse Proxy (Recomendado)

```bash
# Instalar Nginx
sudo apt-get update
sudo apt-get install nginx

# Configurar Nginx (/etc/nginx/sites-available/pqr)
server {
    listen 80;
    server_name pqr.electrohuila.com.co;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name pqr.electrohuila.com.co;

    ssl_certificate /etc/letsencrypt/live/pqr.electrohuila.com.co/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/pqr.electrohuila.com.co/privkey.pem;

    location / {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}

# Habilitar sitio
sudo ln -s /etc/nginx/sites-available/pqr /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### Opción 2: Let's Encrypt (SSL Gratuito)

```bash
# Instalar Certbot
sudo apt-get install certbot python3-certbot-nginx

# Obtener certificado
sudo certbot --nginx -d pqr.electrohuila.com.co

# Auto-renovación (ya configurado por defecto)
sudo certbot renew --dry-run
```

---

## 📊 Monitoreo Post-Despliegue

### Verificar que el Contenedor Está Corriendo

```bash
# Ver estado del contenedor
docker ps | grep pqr-frontend-prod

# Verificar health check
docker inspect pqr-frontend-prod --format='{{.State.Health.Status}}'

# Ver logs en tiempo real
docker logs -f pqr-frontend-prod

# Ver últimos errores
docker logs pqr-frontend-prod 2>&1 | grep -i error
```

### Probar Endpoints Críticos

```bash
# Health check
curl -I https://pqr.electrohuila.com.co

# Time to first byte
curl -w "@curl-format.txt" -o /dev/null -s https://pqr.electrohuila.com.co

# Verificar integración con backend
curl https://api.electrohuila.com.co/health
```

### Monitoreo de Recursos

```bash
# Uso de recursos del contenedor
docker stats pqr-frontend-prod --no-stream

# Logs con filtro de tiempo
docker logs --since 10m pqr-frontend-prod

# Espacio en disco
df -h
docker system df
```

---

## 🔄 Rollback (Volver Atrás)

### Rollback Rápido

```bash
# 1. Detener contenedor actual
cd devops/main
docker-compose down

# 2. Volver al commit/tag anterior
git checkout v0.9.0  # o el commit anterior

# 3. Redesplegar
docker-compose up -d --build

# 4. Verificar
docker-compose logs -f --tail=50
```

### Rollback con Imagen Anterior

```bash
# 1. Ver imágenes disponibles
docker images | grep pqr-frontend-prod

# 2. Usar imagen anterior
docker-compose down
docker tag pqr-frontend-prod:v0.9.0 pqr-frontend-prod:latest
docker-compose up -d

# 3. Verificar
curl -I https://pqr.electrohuila.com.co
```

---

## 🐛 Troubleshooting en Producción

### El Sitio No Responde

```bash
# 1. Verificar que el contenedor esté corriendo
docker ps -a | grep pqr-frontend-prod

# 2. Si está detenido, ver logs
docker logs pqr-frontend-prod --tail=100

# 3. Verificar puertos
sudo netstat -tlnp | grep :80
sudo netstat -tlnp | grep :443

# 4. Verificar Nginx (si lo usas)
sudo nginx -t
sudo systemctl status nginx

# 5. Reiniciar servicios
docker-compose restart
sudo systemctl restart nginx
```

### Performance Degradado

```bash
# Ver uso de recursos
docker stats pqr-frontend-prod

# Ver procesos dentro del contenedor
docker top pqr-frontend-prod

# Aumentar recursos (en docker-compose.yml)
deploy:
  resources:
    limits:
      memory: 2G
      cpus: '2.0'
```

### Errores 502 Bad Gateway

```bash
# 1. Verificar que el contenedor responda
curl http://localhost:3000

# 2. Ver logs de Nginx
sudo tail -f /var/log/nginx/error.log

# 3. Verificar configuración de proxy
sudo nginx -t

# 4. Reiniciar Nginx
sudo systemctl restart nginx
```

---

## 🔒 Seguridad en Producción

### Hardening Básico

```bash
# Firewall - Solo permitir 80, 443, 22
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable

# Actualizar sistema
sudo apt-get update
sudo apt-get upgrade

# Escanear vulnerabilidades en imagen
docker scan pqr-frontend-prod:latest
```

### Variables de Entorno Sensibles

```bash
# NUNCA commitear .env.prod con datos reales
# Usar secretos de Docker o variables de entorno del servidor

# Ejemplo con Docker secrets:
echo "secret-api-key" | docker secret create api_key -
```

---

## 📈 Métricas y Logs

### Logs Centralizados (Opcional)

```bash
# Configurar logging en docker-compose.yml
logging:
  driver: "syslog"
  options:
    syslog-address: "tcp://logserver:514"
```

### Monitoreo con Prometheus/Grafana (Avanzado)

```bash
# Exponer métricas de Next.js
# Agregar en next.config.ts:
experimental: {
  instrumentationHook: true
}
```

---

## 📋 Checklist de Despliegue a Producción

### Pre-Despliegue
- [ ] Backup de base de datos (si aplica)
- [ ] Backup de configuración actual
- [ ] Comunicación enviada a usuarios
- [ ] Ventana de mantenimiento agendada
- [ ] Plan de rollback documentado
- [ ] Equipo de guardia notificado

### Durante Despliegue
- [ ] Despliegue ejecutado exitosamente
- [ ] Contenedor iniciado sin errores
- [ ] Health checks pasando
- [ ] Logs sin errores críticos
- [ ] Endpoints respondiendo

### Post-Despliegue
- [ ] Tests de humo ejecutados
- [ ] Funcionalidades críticas validadas
- [ ] Performance aceptable
- [ ] Logs monitoreados (primeras 2 horas)
- [ ] No hay reportes de errores de usuarios
- [ ] Documentación actualizada
- [ ] CHANGELOG actualizado

---

## 🆘 Contactos de Emergencia

En caso de problemas en producción:

1. **Tech Lead:** [Nombre y contacto]
2. **DevOps Lead:** [Nombre y contacto]
3. **On-Call Engineer:** [Nombre y contacto]

## 📞 Escalamiento

- **Incidente Menor:** Reportar en Slack #prod-incidents
- **Incidente Mayor:** Llamar a On-Call + Crear ticket de emergencia
- **Incidente Crítico:** Activar protocolo de crisis

---

## 📚 Recursos Adicionales

- Monitoreo: https://monitor.electrohuila.com.co
- Logs: https://logs.electrohuila.com.co
- Status Page: https://status.electrohuila.com.co
- Runbooks: https://docs.electrohuila.com.co/runbooks

---

**Recuerda:** En producción, más vale prevenir que lamentar. Siempre valida en staging primero.
