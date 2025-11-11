#!/bin/bash
set -e

echo "======================================"
echo "Building Static Frontend for GitHub Pages"
echo "======================================"

# Define base directory
BASE_DIR="/home/runner/work/MW.Code/MW.Code"
FRONTEND_DIR="$BASE_DIR/frontend"
STATIC_DIR="$BASE_DIR/front-static"

# Clean and create front-static directory
echo ""
echo "Preparing front-static directory..."
rm -rf "$STATIC_DIR"
mkdir -p "$STATIC_DIR"

# Build medicwarehouse-app
echo ""
echo "======================================"
echo "Building medicwarehouse-app..."
echo "======================================"
cd "$FRONTEND_DIR/medicwarehouse-app"
if [ ! -d "node_modules" ]; then
  echo "Installing dependencies for medicwarehouse-app..."
  npm ci
fi
npm run build -- --configuration=static
cp -r dist/medicwarehouse-app/browser "$STATIC_DIR/medicwarehouse-app"
echo "✓ medicwarehouse-app built successfully"

# Build mw-site
echo ""
echo "======================================"
echo "Building mw-site..."
echo "======================================"
cd "$FRONTEND_DIR/mw-site"
if [ ! -d "node_modules" ]; then
  echo "Installing dependencies for mw-site..."
  npm ci
fi
npm run build -- --configuration=static
cp -r dist/mw-site/browser "$STATIC_DIR/mw-site"
echo "✓ mw-site built successfully"

# Build mw-system-admin
echo ""
echo "======================================"
echo "Building mw-system-admin..."
echo "======================================"
cd "$FRONTEND_DIR/mw-system-admin"
if [ ! -d "node_modules" ]; then
  echo "Installing dependencies for mw-system-admin..."
  npm ci
fi
npm run build -- --configuration=static
cp -r dist/mw-system-admin/browser "$STATIC_DIR/mw-system-admin"
echo "✓ mw-system-admin built successfully"

# Build mw-docs
echo ""
echo "======================================"
echo "Building mw-docs..."
echo "======================================"
cd "$FRONTEND_DIR/mw-docs"
if [ ! -d "node_modules" ]; then
  echo "Installing dependencies for mw-docs..."
  npm ci
fi
npm run build -- --configuration=static
cp -r dist/mw-docs/browser "$STATIC_DIR/mw-docs"
echo "✓ mw-docs built successfully"

# Copy portable documentation
echo ""
echo "======================================"
echo "Copying portable documentation..."
echo "======================================"
cd "$BASE_DIR/documentacao-portatil"
if [ ! -d "node_modules" ]; then
  echo "Installing dependencies for documentation..."
  npm ci
fi
npm run gerar
mkdir -p "$STATIC_DIR/documentacao"
cp MedicWarehouse-Documentacao-Completa.html "$STATIC_DIR/documentacao/"
cp MedicWarehouse-Documentacao-Completa.md "$STATIC_DIR/documentacao/"
echo "✓ Documentation copied successfully"

echo ""
echo "======================================"
echo "Build completed successfully!"
echo "======================================"
echo ""
echo "Static files are in: $STATIC_DIR"
echo ""
echo "Applications built:"
echo "  - medicwarehouse-app (Main clinic management)"
echo "  - mw-site (Marketing site)"
echo "  - mw-system-admin (System owner dashboard)"
echo "  - mw-docs (Documentation)"
echo "  - documentacao (Portable documentation)"
echo ""
