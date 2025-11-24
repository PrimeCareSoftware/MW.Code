#!/bin/bash

# Script de Configuração Inicial para macOS - MedicWarehouse
# Este script configura o ambiente de desenvolvimento no macOS

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  🍎 Setup MedicWarehouse - macOS                      ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════╝${NC}"
echo ""

# Função para verificar se um comando existe
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Função para instalar via Homebrew
install_with_brew() {
    local package=$1
    local name=$2
    
    if command_exists "$name"; then
        echo -e "${GREEN}✓${NC} $name já está instalado"
    else
        echo -e "${YELLOW}→${NC} Instalando $name..."
        brew install "$package"
        echo -e "${GREEN}✓${NC} $name instalado com sucesso"
    fi
}

# Verificar se Homebrew está instalado
echo -e "${BLUE}[1/7] Verificando Homebrew...${NC}"
if ! command_exists brew; then
    echo -e "${YELLOW}⚠️  Homebrew não encontrado. Instalando...${NC}"
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
    
    # Adicionar ao PATH (para Apple Silicon)
    if [[ $(uname -m) == 'arm64' ]]; then
        echo 'eval "$(/opt/homebrew/bin/brew shellenv)"' >> ~/.zprofile
        eval "$(/opt/homebrew/bin/brew shellenv)"
    fi
    echo -e "${GREEN}✓${NC} Homebrew instalado"
else
    echo -e "${GREEN}✓${NC} Homebrew já está instalado"
    echo -e "${YELLOW}→${NC} Atualizando Homebrew..."
    brew update
fi
echo ""

# Instalar .NET 8 SDK
echo -e "${BLUE}[2/7] Verificando .NET 8 SDK...${NC}"
if ! command_exists dotnet; then
    echo -e "${YELLOW}→${NC} Instalando .NET 8 SDK..."
    brew install --cask dotnet-sdk
    echo -e "${GREEN}✓${NC} .NET 8 SDK instalado"
else
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓${NC} .NET SDK já está instalado (versão: $DOTNET_VERSION)"
fi
echo ""

# Instalar Node.js
echo -e "${BLUE}[3/7] Verificando Node.js...${NC}"
if ! command_exists node; then
    echo -e "${YELLOW}→${NC} Instalando Node.js..."
    brew install node@20
    echo -e "${GREEN}✓${NC} Node.js instalado"
else
    NODE_VERSION=$(node --version)
    echo -e "${GREEN}✓${NC} Node.js já está instalado (versão: $NODE_VERSION)"
fi
echo ""

# Instalar Podman
echo -e "${BLUE}[4/7] Verificando Podman...${NC}"
if ! command_exists podman; then
    echo -e "${YELLOW}→${NC} Instalando Podman..."
    brew install podman podman-compose
    
    echo -e "${YELLOW}→${NC} Inicializando máquina virtual do Podman..."
    podman machine init
    podman machine start
    
    echo -e "${GREEN}✓${NC} Podman instalado e configurado"
else
    echo -e "${GREEN}✓${NC} Podman já está instalado"
    
    # Verificar se a máquina está rodando
    if ! podman machine list | grep -q "Currently running"; then
        echo -e "${YELLOW}→${NC} Iniciando máquina virtual do Podman..."
        podman machine start
    fi
fi
echo ""

# Instalar Git (geralmente já vem no macOS)
echo -e "${BLUE}[5/7] Verificando Git...${NC}"
if ! command_exists git; then
    echo -e "${YELLOW}→${NC} Instalando Git..."
    brew install git
    echo -e "${GREEN}✓${NC} Git instalado"
else
    GIT_VERSION=$(git --version)
    echo -e "${GREEN}✓${NC} Git já está instalado ($GIT_VERSION)"
fi
echo ""

# Restaurar dependências do .NET
echo -e "${BLUE}[6/7] Restaurando dependências do .NET...${NC}"
dotnet restore
echo -e "${GREEN}✓${NC} Dependências do .NET restauradas"
echo ""

# Instalar dependências do frontend
echo -e "${BLUE}[7/7] Instalando dependências do frontend...${NC}"

if [ -d "frontend/medicwarehouse-app" ]; then
    echo -e "${YELLOW}→${NC} Instalando dependências do medicwarehouse-app..."
    cd frontend/medicwarehouse-app
    npm install
    cd ../..
    echo -e "${GREEN}✓${NC} Dependências do medicwarehouse-app instaladas"
fi

if [ -d "frontend/mw-system-admin" ]; then
    echo -e "${YELLOW}→${NC} Instalando dependências do mw-system-admin..."
    cd frontend/mw-system-admin
    npm install
    cd ../..
    echo -e "${GREEN}✓${NC} Dependências do mw-system-admin instaladas"
fi
echo ""

# Resumo final
echo -e "${GREEN}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  ✅ Configuração Concluída!                            ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${BLUE}📋 Ferramentas instaladas:${NC}"
echo -e "   • .NET SDK: $(dotnet --version)"
echo -e "   • Node.js: $(node --version)"
echo -e "   • npm: $(npm --version)"
echo -e "   • Podman: $(podman --version | head -1)"
echo -e "   • Git: $(git --version)"
echo ""
echo -e "${BLUE}📚 Próximos passos:${NC}"
echo -e "   1. Configure o banco de dados: ${YELLOW}podman-compose up postgres -d${NC}"
echo -e "   2. Aplique as migrations: ${YELLOW}cd src/MedicSoft.Api && dotnet ef database update${NC}"
echo -e "   3. Inicie a API: ${YELLOW}cd src/MedicSoft.Api && dotnet run${NC}"
echo -e "   4. Inicie o frontend: ${YELLOW}cd frontend/medicwarehouse-app && npm start${NC}"
echo ""
echo -e "${BLUE}📖 Documentação completa:${NC}"
echo -e "   • ${YELLOW}GUIA_INICIO_RAPIDO_LOCAL.md${NC}"
echo -e "   • ${YELLOW}README.md${NC}"
echo ""
echo -e "${GREEN}🎉 Ambiente pronto para desenvolvimento!${NC}"
echo ""
