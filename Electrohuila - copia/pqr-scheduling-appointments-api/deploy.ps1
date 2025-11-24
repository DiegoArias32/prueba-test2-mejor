# Script de despliegue para ElectroHuila API (PowerShell)
# Uso: .\deploy.ps1 -Environment dev
# Ejemplo: .\deploy.ps1 -Environment dev

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("dev", "qa", "staging", "main")]
    [string]$Environment
)

# Función para imprimir mensajes
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Green
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

function Write-Warning-Custom {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

Write-Info "Iniciando despliegue en entorno: $Environment"

# Directorio del entorno
$EnvDir = ".\devops\$Environment"

# Verificar que existe el directorio
if (-not (Test-Path $EnvDir)) {
    Write-Error-Custom "El directorio $EnvDir no existe"
    exit 1
}

# Verificar que existe el archivo .env
$EnvFile = Join-Path $EnvDir ".env"
if (-not (Test-Path $EnvFile)) {
    Write-Warning-Custom "No se encontró archivo .env en $EnvDir"
    Write-Info "Copiando .env.example a .env"
    $EnvExample = Join-Path $EnvDir ".env.example"
    if (Test-Path $EnvExample) {
        Copy-Item $EnvExample $EnvFile
        Write-Warning-Custom "Por favor, edite el archivo $EnvFile con los valores correctos"
    } else {
        Write-Error-Custom "No existe el archivo .env.example"
    }
    exit 1
}

# Ir al directorio del entorno
Push-Location $EnvDir

try {
    Write-Info "Deteniendo contenedores existentes..."
    docker-compose down 2>$null

    Write-Info "Construyendo imagen Docker..."
    docker-compose build --no-cache

    if ($LASTEXITCODE -ne 0) {
        Write-Error-Custom "Error al construir la imagen Docker"
        exit 1
    }

    Write-Info "Iniciando contenedores..."
    docker-compose up -d

    if ($LASTEXITCODE -ne 0) {
        Write-Error-Custom "Error al iniciar los contenedores"
        exit 1
    }

    Write-Info "Esperando a que la API esté lista..."
    Start-Sleep -Seconds 10

    # Verificar el estado de los contenedores
    $containerStatus = docker-compose ps
    if ($containerStatus -match "Up") {
        Write-Info "✓ Despliegue completado exitosamente"
        Write-Info "La API está corriendo en:"

        switch ($Environment) {
            "dev" { Write-Host "  http://localhost:5000" -ForegroundColor Cyan }
            "staging" { Write-Host "  http://localhost:5001" -ForegroundColor Cyan }
            "qa" { Write-Host "  http://localhost:5002" -ForegroundColor Cyan }
            "main" { Write-Host "  http://localhost:80" -ForegroundColor Cyan }
        }

        Write-Host ""
        Write-Info "Para ver los logs:"
        Write-Host "  docker-compose logs -f" -ForegroundColor Yellow

        Write-Info "Para verificar el estado:"
        Write-Host "  docker-compose ps" -ForegroundColor Yellow

        Write-Info "Para acceder a Swagger:"
        switch ($Environment) {
            "dev" { Write-Host "  http://localhost:5000/swagger" -ForegroundColor Cyan }
            "staging" { Write-Host "  http://localhost:5001/swagger" -ForegroundColor Cyan }
            "qa" { Write-Host "  http://localhost:5002/swagger" -ForegroundColor Cyan }
            "main" { Write-Host "  http://localhost:80/swagger" -ForegroundColor Cyan }
        }
    }
    else {
        Write-Error-Custom "El despliegue falló. Verifique los logs:"
        Write-Error-Custom "El despliegue fallo. Verifique los logs:"
        docker-compose logs
        exit 1
    }
}
finally {
    Pop-Location
}
