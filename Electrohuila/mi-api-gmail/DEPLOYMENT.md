# Guía de Despliegue - Gmail API ElectroHuila

## Resumen de Implementación

Esta API está **100% lista para producción** con las siguientes características:

- ✅ **6 Endpoints** (5 POST + 1 GET health check)
- ✅ **5 Templates HTML** profesionales y responsivos
- ✅ **Autenticación con API Key** en todos los endpoints POST
- ✅ **Sistema de Logging** automático en archivos JSON
- ✅ **Reintentos Automáticos** con backoff exponencial (3 intentos)
- ✅ **CORS** habilitado para integraciones cross-origin
- ✅ **Estadísticas y Logs** vía endpoints GET
- ✅ **Validación de emails** con regex
- ✅ **Documentación completa** (README.md, TESTING.md)

## Archivos del Proyecto

```
mi-api-gmail/
├── config/
│   └── emailSettings.js          # Configuración SMTP con variables de entorno
├── middleware/
│   └── auth.js                   # Middleware de autenticación con API Key
├── routes/
│   └── email.js                  # 6 endpoints + logging + reintentos
├── templates/
│   └── emailTemplates.js         # 5 templates HTML profesionales
├── utils/
│   ├── logger.js                 # Sistema de logging en JSON
│   └── retryHandler.js           # Backoff exponencial
├── logs/                         # Logs auto-generados (gitignored)
├── .env                          # Variables de entorno (gitignored)
├── .env.example                  # Ejemplo de configuración
├── .gitignore                    # Excluye node_modules, .env, logs
├── index.js                      # Servidor Express con CORS
├── package.json                  # Dependencias actualizadas
├── README.md                     # Documentación completa
├── TESTING.md                    # Guía de pruebas
└── DEPLOYMENT.md                 # Este archivo
```

## Instalación en Servidor de Producción

### Opción 1: Servidor Linux (Ubuntu/Debian)

```bash
# 1. Instalar Node.js 18+ (si no está instalado)
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# 2. Clonar/copiar el proyecto
cd /var/www
# O copiar manualmente la carpeta mi-api-gmail

# 3. Instalar dependencias
cd mi-api-gmail
npm install --production

# 4. Configurar variables de entorno
cp .env.example .env
nano .env  # Editar con credenciales reales

# 5. Probar que funciona
npm start

# 6. Configurar PM2 para mantener la API corriendo
npm install -g pm2
pm2 start index.js --name "gmail-api"
pm2 save
pm2 startup  # Seguir instrucciones para auto-start
```

### Opción 2: Servidor Windows

```powershell
# 1. Instalar Node.js 18+ desde https://nodejs.org

# 2. Copiar carpeta mi-api-gmail a C:\inetpub\gmail-api

# 3. Instalar dependencias
cd C:\inetpub\gmail-api
npm install --production

# 4. Configurar .env
copy .env.example .env
notepad .env  # Editar credenciales

# 5. Probar
npm start

# 6. Instalar como servicio Windows con NSSM
# Descargar NSSM: https://nssm.cc/download
nssm install GmailAPI "C:\Program Files\nodejs\node.exe" "C:\inetpub\gmail-api\index.js"
nssm start GmailAPI
```

### Opción 3: Docker

```dockerfile
# Dockerfile (crear en la raíz del proyecto)
FROM node:18-alpine

WORKDIR /app

COPY package*.json ./
RUN npm install --production

COPY . .

EXPOSE 4000

CMD ["npm", "start"]
```

```bash
# Build
docker build -t gmail-api:1.0.0 .

# Run
docker run -d \
  --name gmail-api \
  -p 4000:4000 \
  -e GMAIL_USER=tu-email@gmail.com \
  -e GMAIL_PASSWORD=tu-app-password \
  -e GMAIL_API_KEY=tu-api-key-segura \
  -v $(pwd)/logs:/app/logs \
  gmail-api:1.0.0
```

## Configuración de Variables de Entorno

### Variables Requeridas

```env
# Gmail SMTP (OBLIGATORIAS)
GMAIL_USER=tu-email@gmail.com
GMAIL_PASSWORD=xxxx xxxx xxxx xxxx  # App Password de Gmail

# API Key (OBLIGATORIA - cambiar en producción)
GMAIL_API_KEY=genera-una-clave-segura-aqui

# Configuración (opcionales)
PORT=4000
NODE_ENV=production
FROM_EMAIL=tu-email@gmail.com
FROM_NAME=Sistema de Citas ElectroHuila
```

### Generar API Key Segura

```bash
# Linux/Mac
openssl rand -hex 32

# Node.js
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"

# Online
# https://www.uuidgenerator.net/
```

### Obtener App Password de Gmail

1. Ir a https://myaccount.google.com/security
2. Activar **"Verificación en 2 pasos"**
3. Buscar **"Contraseñas de aplicaciones"**
4. Generar nueva contraseña para **"Correo"**
5. Copiar la contraseña de 16 caracteres (sin espacios)
6. Pegar en `GMAIL_PASSWORD` del `.env`

## Integración con Backend .NET

### 1. Agregar al `appsettings.json`

```json
{
  "GmailApi": {
    "BaseUrl": "http://localhost:4000",
    "ApiKey": "tu-api-key-super-segura"
  }
}
```

### 2. Crear Cliente HTTP

Ver sección **"Integración con Backend .NET"** en `README.md` para código completo.

### 3. Registrar Servicio

```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

### 4. Usar en Controladores

```csharp
[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly IEmailService _emailService;

    [HttpPost]
    public async Task<IActionResult> CrearCita([FromBody] CitaDto cita)
    {
        // Guardar en BD
        await _citaRepository.Guardar(cita);

        // Enviar email de confirmación
        await _emailService.EnviarConfirmacionCita(cita);

        return Ok(cita);
    }

    [HttpPost("{id}/recordatorio")]
    public async Task<IActionResult> EnviarRecordatorio(int id)
    {
        var cita = await _citaRepository.ObtenerPorId(id);

        // Enviar recordatorio
        await _emailService.EnviarRecordatorioCita(cita);

        return Ok();
    }
}
```

## Nginx Reverse Proxy (Recomendado)

```nginx
# /etc/nginx/sites-available/gmail-api
server {
    listen 80;
    server_name api.electrohuila.com;

    location / {
        proxy_pass http://localhost:4000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }
}
```

```bash
# Habilitar sitio
sudo ln -s /etc/nginx/sites-available/gmail-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## SSL/HTTPS con Let's Encrypt

```bash
# Instalar Certbot
sudo apt-get install certbot python3-certbot-nginx

# Obtener certificado SSL
sudo certbot --nginx -d api.electrohuila.com

# Auto-renovación
sudo certbot renew --dry-run
```

## Monitoreo y Logs

### Ver Logs en Tiempo Real

```bash
# Con PM2
pm2 logs gmail-api

# Logs de la aplicación
tail -f logs/gmail-$(date +%Y-%m-%d).log
```

### Monitoreo con PM2

```bash
# Ver status
pm2 status

# Ver métricas
pm2 monit

# Ver logs específicos
pm2 logs gmail-api --lines 100
```

### Rotación de Logs

Los logs se crean automáticamente por día (`gmail-YYYY-MM-DD.log`).

Para limpiar logs antiguos:

```bash
# Eliminar logs de más de 30 días
find logs/ -name "gmail-*.log" -mtime +30 -delete
```

O configurar cron job:

```bash
# Editar crontab
crontab -e

# Agregar línea (ejecutar diariamente a las 2 AM)
0 2 * * * find /var/www/mi-api-gmail/logs/ -name "gmail-*.log" -mtime +30 -delete
```

## Seguridad en Producción

### 1. Cambiar API Key

Generar una clave segura y actualizar en `.env` y en el backend .NET.

### 2. Configurar Firewall

```bash
# Permitir solo puerto 4000 desde localhost (si usas Nginx)
sudo ufw allow from 127.0.0.1 to any port 4000

# O permitir desde IP específica del backend
sudo ufw allow from IP_DEL_BACKEND to any port 4000
```

### 3. Rate Limiting (Opcional)

Instalar `express-rate-limit`:

```bash
npm install express-rate-limit
```

Agregar en `index.js`:

```javascript
const rateLimit = require('express-rate-limit');

const limiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutos
  max: 100 // límite de 100 requests por IP
});

app.use('/email/', limiter);
```

### 4. Restricción CORS

Cambiar en `index.js`:

```javascript
app.use(cors({
  origin: 'https://electrohuila.com', // Solo tu frontend
  methods: ['GET', 'POST'],
  allowedHeaders: ['Content-Type', 'X-API-Key', 'Authorization']
}));
```

## Troubleshooting en Producción

### Error: Puerto 4000 ya en uso

```bash
# Ver qué proceso usa el puerto
lsof -i :4000
# O en Windows
netstat -ano | findstr :4000

# Matar proceso
kill -9 PID
# O cambiar puerto en .env
PORT=4001
```

### Error: SMTP timeout

- Verificar firewall (puerto 587 saliente)
- Verificar que Gmail permite "apps menos seguras"
- Verificar App Password es correcto

### Emails en SPAM

- Configurar SPF/DKIM en el dominio
- Usar email corporativo en lugar de Gmail personal
- Evitar palabras spam en subject/body
- Verificar con https://mail-tester.com

### Logs no se crean

```bash
# Verificar permisos
ls -la logs/
chmod 755 logs/

# Crear manualmente
mkdir -p logs
touch logs/gmail-$(date +%Y-%m-%d).log
```

## Backup

### Backup de Logs

```bash
# Comprimir logs mensuales
tar -czf backup-logs-$(date +%Y-%m).tar.gz logs/

# Mover a storage
mv backup-logs-*.tar.gz /backup/gmail-api/
```

### Backup de Configuración

```bash
# Solo .env (¡cuidado con seguridad!)
cp .env .env.backup

# Todo el proyecto (sin node_modules)
tar --exclude='node_modules' --exclude='logs' -czf gmail-api-backup.tar.gz mi-api-gmail/
```

## Actualización de la API

```bash
# 1. Backup
cp .env .env.backup
tar -czf backup-$(date +%Y%m%d).tar.gz mi-api-gmail/

# 2. Actualizar código
cd mi-api-gmail
git pull  # Si usas git
# O copiar archivos nuevos

# 3. Instalar dependencias nuevas
npm install --production

# 4. Reiniciar servicio
pm2 restart gmail-api

# 5. Verificar
curl http://localhost:4000/email/status
```

## Checklist de Producción

Antes de poner en producción, verificar:

- [ ] Variables de entorno configuradas correctamente
- [ ] API Key cambiada a una clave segura
- [ ] Gmail App Password configurado
- [ ] Permisos de carpeta `logs/` correctos (755)
- [ ] PM2 o servicio Windows configurado
- [ ] Auto-start habilitado
- [ ] Nginx reverse proxy (si aplica)
- [ ] SSL/HTTPS configurado (si aplica)
- [ ] Firewall configurado
- [ ] CORS restringido a dominios permitidos
- [ ] Rate limiting configurado (opcional)
- [ ] Monitoreo configurado
- [ ] Rotación de logs configurada
- [ ] Backup strategy definida
- [ ] Backend .NET integrado y probado
- [ ] Emails de prueba enviados y recibidos
- [ ] Documentación entregada al equipo

## Contacto y Soporte

- **Documentación:** `README.md`
- **Pruebas:** `TESTING.md`
- **Despliegue:** Este archivo (`DEPLOYMENT.md`)

---

**Versión:** 1.0.0
**Estado:** Production Ready ✅
**Última actualización:** 2025-11-23
