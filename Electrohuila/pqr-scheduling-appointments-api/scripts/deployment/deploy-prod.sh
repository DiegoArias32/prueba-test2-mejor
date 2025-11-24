#!/bin/bash

# =============================================
# ElectroHuila - Production Deployment Script
# =============================================

set -e  # Exit on error

echo "=========================================="
echo "ElectroHuila - Production Deployment"
echo "=========================================="

# Configuration
PROJECT_NAME="ElectroHuila"
ENVIRONMENT="Production"
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WEB_PROJECT="$PROJECT_DIR/Web/ElectroHuila.WebApi.csproj"
PUBLISH_DIR="$PROJECT_DIR/publish/prod"
BACKUP_DIR="$PROJECT_DIR/backups/$(date +%Y%m%d_%H%M%S)"

echo ""
echo "Project Directory: $PROJECT_DIR"
echo "Environment: $ENVIRONMENT"
echo ""

# Warning for production deployment
echo "⚠️  WARNING: You are about to deploy to PRODUCTION!"
read -p "Are you sure you want to continue? (yes/no) " -r
echo
if [[ ! $REPLY =~ ^[Yy][Ee][Ss]$ ]]
then
    echo "Deployment cancelled."
    exit 0
fi

# Step 1: Backup current deployment (if exists)
if [ -d "$PUBLISH_DIR" ]; then
    echo ""
    echo "[1/9] Creating backup of current deployment..."
    mkdir -p "$BACKUP_DIR"
    cp -r "$PUBLISH_DIR" "$BACKUP_DIR/"
    echo "Backup created at: $BACKUP_DIR"
fi

# Step 2: Clean previous builds
echo ""
echo "[2/9] Cleaning previous builds..."
dotnet clean "$WEB_PROJECT" --configuration Release
rm -rf "$PUBLISH_DIR"

# Step 3: Restore dependencies
echo ""
echo "[3/9] Restoring dependencies..."
dotnet restore "$PROJECT_DIR"

# Step 4: Build project
echo ""
echo "[4/9] Building project in Release mode..."
dotnet build "$WEB_PROJECT" --configuration Release --no-restore

# Step 5: Run tests
echo ""
echo "[5/9] Running tests..."
dotnet test "$PROJECT_DIR" --configuration Release --no-build --verbosity normal

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Tests failed! Deployment aborted."
    echo "Please fix the failing tests before deploying to production."
    exit 1
fi

# Step 6: Publish application
echo ""
echo "[6/9] Publishing application..."
dotnet publish "$WEB_PROJECT" \
    --configuration Release \
    --output "$PUBLISH_DIR" \
    --no-build \
    --verbosity minimal \
    /p:DebugType=None \
    /p:DebugSymbols=false

# Step 7: Copy production configuration
echo ""
echo "[7/9] Copying production configuration..."
if [ -f "$PROJECT_DIR/Web/appsettings.Production.json" ]; then
    cp "$PROJECT_DIR/Web/appsettings.Production.json" "$PUBLISH_DIR/"
else
    echo "⚠️  Warning: appsettings.Production.json not found!"
fi

# Step 8: Set file permissions
echo ""
echo "[8/9] Setting file permissions..."
chmod -R 755 "$PUBLISH_DIR"

# Step 9: Verify deployment
echo ""
echo "[9/9] Verifying deployment..."
if [ -f "$PUBLISH_DIR/ElectroHuila.WebApi.dll" ]; then
    echo "✅ ElectroHuila.WebApi.dll found"
else
    echo "❌ ElectroHuila.WebApi.dll not found!"
    exit 1
fi

# Display deployment summary
echo ""
echo "=========================================="
echo "✅ Production Deployment Completed!"
echo "=========================================="
echo ""
echo "Deployment Details:"
echo "  - Environment: $ENVIRONMENT"
echo "  - Published to: $PUBLISH_DIR"
echo "  - Backup location: $BACKUP_DIR"
echo "  - Deployment date: $(date)"
echo ""
echo "Next Steps:"
echo "  1. Stop the running application service"
echo "  2. Copy files to production server:"
echo "     rsync -avz $PUBLISH_DIR/ user@prod-server:/var/www/electrohuila/"
echo "  3. Update environment variables on server"
echo "  4. Restart the application service"
echo "  5. Verify application is running"
echo "  6. Monitor logs for any issues"
echo ""
echo "Production Checklist:"
echo "  ☐ Database migrations applied"
echo "  ☐ Environment variables configured"
echo "  ☐ SSL certificates valid"
echo "  ☐ Firewall rules configured"
echo "  ☐ Monitoring alerts enabled"
echo "  ☐ Backup schedule verified"
echo ""

# Optional: Create deployment log
LOG_FILE="$PROJECT_DIR/logs/deployments.log"
mkdir -p "$PROJECT_DIR/logs"
echo "$(date) - Production deployment completed - Version: $(git rev-parse --short HEAD 2>/dev/null || echo 'unknown')" >> "$LOG_FILE"

echo "Deployment logged to: $LOG_FILE"
echo ""