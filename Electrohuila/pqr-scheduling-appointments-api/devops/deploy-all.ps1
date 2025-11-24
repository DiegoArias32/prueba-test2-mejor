# Script para desplegar todos los entornos de ElectroHuila API
# Uso: .\devops\deploy-all.ps1

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  ElectroHuila PQR API - Despliegue" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Funcion para imprimir mensajes
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Green
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Detener todos los contenedores existentes
Write-Info "Deteniendo contenedores existentes..."
Push-Location devops/dev
docker-compose down 2>$null
Pop-Location

Push-Location devops/qa
docker-compose down 2>$null
Pop-Location

Push-Location devops/staging
docker-compose down 2>$null
Pop-Location

Push-Location devops/main
docker-compose down 2>$null
Pop-Location

Write-Host ""
Write-Info "Desplegando entornos..."
Write-Host ""

# Construir e iniciar DEV
Write-Host ">>> Desplegando Entorno: DEV" -ForegroundColor Yellow
Push-Location devops/dev
docker-compose up -d --build
if ($LASTEXITCODE -eq 0) {
    Write-Info "OK - Entorno DEV desplegado en http://localhost:5000"
} else {
    Write-Error-Custom "ERROR - Error al desplegar DEV"
}
Pop-Location
Write-Host ""

# Construir e iniciar QA
Write-Host ">>> Desplegando Entorno: QA" -ForegroundColor Yellow
Push-Location devops/qa
docker-compose up -d --build
if ($LASTEXITCODE -eq 0) {
    Write-Info "OK - Entorno QA desplegado en http://localhost:5002"
} else {
    Write-Error-Custom "ERROR - Error al desplegar QA"
}
Pop-Location
Write-Host ""

# Construir e iniciar STAGING
Write-Host ">>> Desplegando Entorno: STAGING" -ForegroundColor Yellow
Push-Location devops/staging
docker-compose up -d --build
if ($LASTEXITCODE -eq 0) {
    Write-Info "OK - Entorno STAGING desplegado en http://localhost:5001"
} else {
    Write-Error-Custom "ERROR - Error al desplegar STAGING"
}
Pop-Location
Write-Host ""

# Construir e iniciar MAIN
Write-Host ">>> Desplegando Entorno: MAIN" -ForegroundColor Yellow
Push-Location devops/main
docker-compose up -d --build
if ($LASTEXITCODE -eq 0) {
    Write-Info "OK - Entorno MAIN desplegado en http://localhost:8080"
} else {
    Write-Error-Custom "ERROR - Error al desplegar MAIN"
}
Pop-Location
Write-Host ""

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Resumen de Despliegue" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  DEV       -> http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  QA        -> http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  STAGING   -> http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  MAIN      -> http://localhost:8080/swagger" -ForegroundColor White
Write-Host ""
Write-Info "Para ver todos los contenedores:"
Write-Host "  docker ps" -ForegroundColor Yellow
Write-Host ""
Write-Info "Para ver logs de un entorno especifico:"
Write-Host "  cd devops/dev && docker-compose logs -f" -ForegroundColor Yellow
Write-Host ""
