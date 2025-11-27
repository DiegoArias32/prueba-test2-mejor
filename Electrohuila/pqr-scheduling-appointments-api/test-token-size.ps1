# Test JWT Token Size
# This script helps verify that the JWT token size is within acceptable limits

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "JWT Token Size Test" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$apiUrl = "http://localhost:5000/api/v1"
$username = "admin"
$password = Read-Host "Enter password for user '$username'" -AsSecureString

# Convert SecureString to plain text
$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
$plainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)

# Login request
Write-Host "1. Logging in as '$username'..." -ForegroundColor Yellow

$loginBody = @{
    username = $username
    password = $plainPassword
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$apiUrl/auth/login" `
        -Method POST `
        -ContentType "application/json" `
        -Body $loginBody

    $token = $response.token

    if (-not $token) {
        Write-Host "ERROR: No token received from login" -ForegroundColor Red
        exit 1
    }

    Write-Host "✓ Login successful" -ForegroundColor Green
    Write-Host ""

    # Analyze token
    Write-Host "2. Analyzing JWT token..." -ForegroundColor Yellow
    Write-Host ""

    # Token size
    $tokenSize = $token.Length
    Write-Host "   Token size: $tokenSize characters" -ForegroundColor $(if ($tokenSize -lt 2000) { "Green" } else { "Red" })

    # Decode token (basic, without verification)
    $parts = $token.Split('.')
    if ($parts.Length -eq 3) {
        # Decode header
        $headerJson = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($parts[0] + "=="))
        Write-Host ""
        Write-Host "   Header:" -ForegroundColor Cyan
        Write-Host "   $headerJson" -ForegroundColor Gray

        # Decode payload
        # Add padding if needed
        $payload = $parts[1]
        while ($payload.Length % 4 -ne 0) {
            $payload += "="
        }
        $payloadJson = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($payload))
        $payloadObj = $payloadJson | ConvertFrom-Json

        Write-Host ""
        Write-Host "   Payload:" -ForegroundColor Cyan
        Write-Host "   $payloadJson" -ForegroundColor Gray

        # Check permissions format
        Write-Host ""
        Write-Host "3. Checking permissions format..." -ForegroundColor Yellow
        Write-Host ""

        if ($payloadObj.permissions) {
            Write-Host "   ✓ NEW FORMAT: Single 'permissions' claim found" -ForegroundColor Green
            $permissionsList = $payloadObj.permissions -split ','
            Write-Host "   Number of permissions: $($permissionsList.Length)" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "   Permissions (comma-separated):" -ForegroundColor Cyan
            Write-Host "   $($payloadObj.permissions)" -ForegroundColor Gray
        }
        elseif ($payloadObj.permission) {
            Write-Host "   ⚠ OLD FORMAT: Individual 'permission' claims found" -ForegroundColor Yellow
            Write-Host "   This format is deprecated and causes long tokens" -ForegroundColor Yellow
        }
        else {
            Write-Host "   ⚠ No permissions found in token" -ForegroundColor Yellow
        }
    }

    Write-Host ""
    Write-Host "=====================================" -ForegroundColor Cyan
    Write-Host "Summary" -ForegroundColor Cyan
    Write-Host "=====================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "   Token Size: $tokenSize chars" -ForegroundColor Cyan
    Write-Host "   URL Limit:  2048 chars" -ForegroundColor Cyan
    Write-Host ""

    if ($tokenSize -lt 1500) {
        Write-Host "   Status: ✓ EXCELLENT (well within limit)" -ForegroundColor Green
    }
    elseif ($tokenSize -lt 2000) {
        Write-Host "   Status: ✓ GOOD (within limit)" -ForegroundColor Green
    }
    elseif ($tokenSize -lt 2048) {
        Write-Host "   Status: ⚠ WARNING (close to limit)" -ForegroundColor Yellow
    }
    else {
        Write-Host "   Status: ✗ ERROR (exceeds limit!)" -ForegroundColor Red
        Write-Host "   WebSocket connection will fail!" -ForegroundColor Red
    }

    Write-Host ""
    Write-Host "   Full Token (for testing at https://jwt.io):" -ForegroundColor Cyan
    Write-Host "   $token" -ForegroundColor Gray
    Write-Host ""

    # Test WebSocket URL
    $wsUrl = "ws://localhost:5000/hubs/notifications?access_token=$token"
    $wsUrlSize = $wsUrl.Length
    Write-Host "   WebSocket URL Size: $wsUrlSize chars" -ForegroundColor Cyan
    Write-Host ""

}
catch {
    Write-Host "ERROR: Login failed" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
