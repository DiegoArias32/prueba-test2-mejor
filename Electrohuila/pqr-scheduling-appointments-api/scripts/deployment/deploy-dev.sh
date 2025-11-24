#!/bin/bash

# =============================================
# ElectroHuila - Development Deployment Script
# =============================================

set -e  # Exit on error

echo "=========================================="
echo "ElectroHuila - Development Deployment"
echo "=========================================="

# Configuration
PROJECT_NAME="ElectroHuila"
ENVIRONMENT="Development"
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WEB_PROJECT="$PROJECT_DIR/Web/ElectroHuila.WebApi.csproj"
PUBLISH_DIR="$PROJECT_DIR/publish/dev"

echo ""
echo "Project Directory: $PROJECT_DIR"
echo "Environment: $ENVIRONMENT"
echo ""

# Step 1: Clean previous builds
echo "[1/7] Cleaning previous builds..."
dotnet clean "$WEB_PROJECT" --configuration Debug
rm -rf "$PUBLISH_DIR"

# Step 2: Restore dependencies
echo ""
echo "[2/7] Restoring dependencies..."
dotnet restore "$PROJECT_DIR"

# Step 3: Build project
echo ""
echo "[3/7] Building project..."
dotnet build "$WEB_PROJECT" --configuration Debug --no-restore

# Step 4: Run tests
echo ""
echo "[4/7] Running tests..."
dotnet test "$PROJECT_DIR" --configuration Debug --no-build --verbosity quiet || {
    echo "⚠️  Warning: Some tests failed, but continuing deployment..."
}

# Step 5: Publish application
echo ""
echo "[5/7] Publishing application..."
dotnet publish "$WEB_PROJECT" \
    --configuration Debug \
    --output "$PUBLISH_DIR" \
    --no-build \
    --verbosity quiet

# Step 6: Copy configuration files
echo ""
echo "[6/7] Copying configuration files..."
cp "$PROJECT_DIR/Web/appsettings.Development.json" "$PUBLISH_DIR/" 2>/dev/null || true

# Step 7: Start application
echo ""
echo "[7/7] Starting application..."
cd "$PUBLISH_DIR"

echo ""
echo "=========================================="
echo "✅ Deployment completed successfully!"
echo "=========================================="
echo ""
echo "Application published to: $PUBLISH_DIR"
echo ""
echo "To run the application:"
echo "  cd $PUBLISH_DIR"
echo "  dotnet ElectroHuila.WebApi.dll"
echo ""
echo "API will be available at:"
echo "  - https://localhost:7001"
echo "  - http://localhost:5001"
echo "  - Swagger: https://localhost:7001/swagger"
echo ""

# Optional: Start the application automatically
read -p "Do you want to start the application now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]
then
    echo "Starting application..."
    dotnet ElectroHuila.WebApi.dll
fi