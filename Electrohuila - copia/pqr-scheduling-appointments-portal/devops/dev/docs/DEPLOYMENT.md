# Guía de Despliegue - Ambiente Development

## 📋 Información del Ambiente

- **Rama:** `HU-01-dev` o cualquier rama de desarrollo
- **Puerto:** 3000
- **URL:** http://localhost:3000
- **API Backend:** http://localhost:8080

## 🚀 Comandos de Despliegue

### Opción 1: Despliegue con Docker Compose (Recomendado)

```bash
# Navegar al directorio del proyecto
cd "c:\Users\XJuan\Desktop\ADSO - 2901817\PQR\pqr-scheduling-appointments-portal"

# Asegurarse de estar en la rama correcta
git checkout HU-01-dev
git pull origin HU-01-dev

# Construir e iniciar el contenedor
cd devops/dev
docker-compose up -d --build

# Ver logs del contenedor
docker-compose logs -f

# Detener el contenedor
docker-compose down

# Detener y eliminar volúmenes
docker-compose down -v
```

### Opción 2: Despliegue con Docker directo

```bash
# Construir la imagen
docker build -t pqr-frontend-dev:latest -f devops/dev/dockerfile .

# Ejecutar el contenedor
docker run -d \
  --name pqr-frontend-dev \
  -p 3000:3000 \
  --env-file devops/dev/properties/.env.dev \
  pqr-frontend-dev:latest

# Ver logs
docker logs -f pqr-frontend-dev

# Detener y eliminar
docker stop pqr-frontend-dev
docker rm pqr-frontend-dev
```

### Opción 3: Desarrollo local (sin Docker)

```bash
# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm run dev

# Ejecutar en modo producción
npm run build
npm start
```

## 🔧 Configuración

Editar el archivo `devops/dev/properties/.env.dev` con las variables necesarias:

```env
NEXT_PUBLIC_API_URL=http://localhost:8080
NEXT_PUBLIC_ENV=development
NODE_ENV=development
```

## 📊 Verificar Estado

```bash
# Ver contenedores en ejecución
docker ps

# Ver estado del servicio
docker-compose ps

# Verificar salud del contenedor
docker inspect pqr-frontend-dev --format='{{.State.Health.Status}}'
```

## 🐛 Troubleshooting

### Error: Puerto 3000 ya en uso

```bash
# Windows PowerShell
Get-Process -Id (Get-NetTCPConnection -LocalPort 3000).OwningProcess
Stop-Process -Id <PID>

# O usar otro puerto en docker-compose.yml
ports:
  - "3030:3000"
```

### Error: No se puede conectar al backend

Verificar que:
1. El backend esté corriendo en el puerto 8080
2. La variable `NEXT_PUBLIC_API_URL` esté correcta en `.env.dev`
3. No haya problemas de CORS

### Limpiar todo y empezar de nuevo

```bash
# Detener todos los contenedores
docker-compose down -v

# Eliminar imágenes antiguas
docker rmi pqr-frontend-dev:latest

# Limpiar caché de Docker
docker system prune -a

# Reconstruir
docker-compose up -d --build
```

## 📝 Notas

- Este ambiente es para desarrollo, no usar en producción
- Los cambios en el código requieren reconstruir la imagen
- Para desarrollo activo, usar `npm run dev` sin Docker
