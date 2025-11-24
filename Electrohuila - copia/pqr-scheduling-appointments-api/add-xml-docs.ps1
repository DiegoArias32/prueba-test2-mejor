# Script para agregar documentación XML básica a archivos C# sin documentación
# Este script agrega documentación básica a clases, propiedades, métodos, etc.

$sourceDir = "C:\Users\USUARIO\Videos\PQR_AgendamientoDeCitas\backend\src"

function Add-XmlDocToFile {
    param(
        [string]$filePath
    )

    $content = Get-Content $filePath -Raw
    $modified = $false

    # Si ya tiene documentación XML, saltar
    if ($content -match '///\s*<summary>') {
        Write-Host "  Ya tiene documentación: $filePath" -ForegroundColor Yellow
        return $false
    }

    Write-Host "  Procesando: $filePath" -ForegroundColor Cyan

    # Agregar documentación a clases públicas
    $content = $content -replace '(?m)^(\s*)(public\s+(class|interface|record|struct)\s+(\w+))', @'
$1/// <summary>
$1/// $4
$1/// </summary>
$1$2
'@

    # Agregar documentación a propiedades públicas
    $content = $content -replace '(?m)^(\s*)(public\s+\w+(\?|\s+)\s+(\w+)\s*\{\s*get)', @'
$1/// <summary>
$1/// $4
$1/// </summary>
$1$2
'@

    # Agregar documentación a métodos públicos
    $content = $content -replace '(?m)^(\s*)(public\s+\w+(\?|\s+)\s+(\w+)\s*\()', @'
$1/// <summary>
$1/// $4
$1/// </summary>
$1$2
'@

    Set-Content $filePath -Value $content -NoNewline
    return $true
}

# Obtener todos los archivos .cs excluyendo obj y bin
$files = Get-ChildItem -Path $sourceDir -Filter "*.cs" -Recurse |
    Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' }

$count = 0
foreach ($file in $files) {
    if (Add-XmlDocToFile -filePath $file.FullName) {
        $count++
    }
}

Write-Host "`nArchivos modificados: $count" -ForegroundColor Green
