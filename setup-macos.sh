#!/bin/bash

# Script de Configuração Inicial para macOS - MedicWarehouse
# Este script configura o ambiente de desenvolvimento no macOS

set -e

# Diretório do script (raiz do projeto)
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
MAGENTA='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  🍎 Setup MedicWarehouse - macOS                      ║${NC}"
echo -e "${BLUE}║     Complete Environment Setup                         ║${NC}"
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
echo -e "${BLUE}[1/11] Verificando Homebrew...${NC}"
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
echo -e "${BLUE}[2/11] Verificando .NET 8 SDK...${NC}"
if ! command_exists dotnet; then
    echo -e "${YELLOW}→${NC} Instalando .NET 8 SDK..."
    brew install --cask dotnet-sdk
    echo -e "${GREEN}✓${NC} .NET 8 SDK instalado"
else
    DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "versão desconhecida")
    echo -e "${GREEN}✓${NC} .NET SDK já está instalado (versão: $DOTNET_VERSION)"
fi
echo ""

# Instalar Node.js
echo -e "${BLUE}[3/11] Verificando Node.js...${NC}"
if ! command_exists node; then
    echo -e "${YELLOW}→${NC} Instalando Node.js..."
    brew install node@20
    echo -e "${GREEN}✓${NC} Node.js instalado"
else
    NODE_VERSION=$(node --version 2>/dev/null || echo "versão desconhecida")
    echo -e "${GREEN}✓${NC} Node.js já está instalado (versão: $NODE_VERSION)"
fi
echo ""

# Instalar Podman
echo -e "${BLUE}[4/11] Verificando Podman...${NC}"
if ! command_exists podman; then
    echo -e "${YELLOW}→${NC} Instalando Podman..."
    brew install podman podman-compose
    
    echo -e "${YELLOW}→${NC} Inicializando máquina virtual do Podman..."
    if podman machine init; then
        echo -e "${GREEN}✓${NC} Máquina virtual do Podman inicializada"
        
        echo -e "${YELLOW}→${NC} Iniciando máquina virtual do Podman..."
        if podman machine start; then
            echo -e "${GREEN}✓${NC} Podman instalado e configurado"
        else
            echo -e "${YELLOW}⚠️  Erro ao iniciar máquina do Podman. Tente: podman machine start${NC}"
        fi
    else
        echo -e "${YELLOW}⚠️  Erro ao inicializar máquina do Podman.${NC}"
        echo -e "${YELLOW}    Verifique se a virtualização está habilitada.${NC}"
    fi
else
    echo -e "${GREEN}✓${NC} Podman já está instalado"
    
    # Verificar se a máquina está rodando
    if ! podman machine list 2>/dev/null | grep -q "Currently running"; then
        echo -e "${YELLOW}→${NC} Iniciando máquina virtual do Podman..."
        if podman machine start; then
            echo -e "${GREEN}✓${NC} Máquina do Podman iniciada"
        else
            echo -e "${YELLOW}⚠️  Erro ao iniciar máquina do Podman${NC}"
        fi
    fi
fi
echo ""

# Instalar Git (geralmente já vem no macOS)
echo -e "${BLUE}[5/11] Verificando Git...${NC}"
if ! command_exists git; then
    echo -e "${YELLOW}→${NC} Instalando Git..."
    brew install git
    echo -e "${GREEN}✓${NC} Git instalado"
else
    GIT_VERSION=$(git --version)
    echo -e "${GREEN}✓${NC} Git já está instalado ($GIT_VERSION)"
fi
echo ""

# Configurar variáveis de ambiente
echo -e "${BLUE}[6/11] Configurando variáveis de ambiente...${NC}"
if [ ! -f "$SCRIPT_DIR/.env" ]; then
    echo -e "${YELLOW}→${NC} Criando arquivo .env a partir do .env.example..."
    cp "$SCRIPT_DIR/.env.example" "$SCRIPT_DIR/.env"
    echo -e "${GREEN}✓${NC} Arquivo .env criado"
    echo -e "${YELLOW}⚠️  IMPORTANTE: Edite o arquivo .env e configure suas variáveis!${NC}"
    echo -e "${YELLOW}   Principalmente: POSTGRES_PASSWORD e JWT_SECRET_KEY${NC}"
else
    echo -e "${GREEN}✓${NC} Arquivo .env já existe"
fi
echo ""

# Restaurar dependências do .NET
echo -e "${BLUE}[7/11] Restaurando dependências do .NET...${NC}"
dotnet restore "$SCRIPT_DIR/MedicWarehouse.sln"
echo -e "${GREEN}✓${NC} Dependências do .NET restauradas"
echo ""

# Iniciar o PostgreSQL com Podman
echo -e "${BLUE}[8/11] Iniciando PostgreSQL com Podman...${NC}"
echo -e "${YELLOW}→${NC} Verificando se o PostgreSQL já está rodando..."
POSTGRES_RUNNING=false
if podman ps --format "{{.Names}}" 2>/dev/null | grep -q "medicwarehouse-postgres"; then
    echo -e "${GREEN}✓${NC} PostgreSQL já está rodando"
    POSTGRES_RUNNING=true
else
    echo -e "${YELLOW}→${NC} Iniciando container PostgreSQL..."
    cd "$SCRIPT_DIR"
    if podman-compose up postgres -d 2>/dev/null; then
        echo -e "${GREEN}✓${NC} PostgreSQL iniciado com sucesso"
        echo -e "${YELLOW}→${NC} Aguardando PostgreSQL inicializar (15 segundos)..."
        sleep 15
        POSTGRES_RUNNING=true
    else
        echo -e "${YELLOW}⚠️  Não foi possível iniciar o PostgreSQL automaticamente${NC}"
        echo -e "${YELLOW}   Execute manualmente: podman-compose up postgres -d${NC}"
        POSTGRES_RUNNING=false
    fi
fi
echo ""

# Aplicar migrations do banco de dados
echo -e "${BLUE}[9/11] Aplicando migrations do banco de dados...${NC}"
if [ "$POSTGRES_RUNNING" = true ]; then
    echo -e "${YELLOW}→${NC} Aplicando migrations da API principal..."
    cd "$SCRIPT_DIR/src/MedicSoft.Api"
    if dotnet ef database update --context MedicSoftDbContext --project ../MedicSoft.Repository 2>/dev/null; then
        echo -e "${GREEN}✓${NC} Migrations da API principal aplicadas"
    else
        echo -e "${YELLOW}⚠️  Erro ao aplicar migrations da API principal${NC}"
        echo -e "${YELLOW}   Execute manualmente após iniciar o PostgreSQL:${NC}"
        echo -e "${YELLOW}   cd src/MedicSoft.Api && dotnet ef database update --context MedicSoftDbContext --project ../MedicSoft.Repository${NC}"
    fi
    cd "$SCRIPT_DIR"
else
    echo -e "${YELLOW}⚠️  PostgreSQL não está rodando. Pulando migrations.${NC}"
    echo -e "${YELLOW}   Execute manualmente após iniciar o PostgreSQL:${NC}"
    echo -e "${YELLOW}   cd src/MedicSoft.Api && dotnet ef database update --context MedicSoftDbContext --project ../MedicSoft.Repository${NC}"
fi
echo ""

# Instalar dependências do frontend
echo -e "${BLUE}[10/11] Instalando dependências do frontend...${NC}"

# Frontend principal
if [ -d "$SCRIPT_DIR/frontend/medicwarehouse-app" ]; then
    echo -e "${YELLOW}→${NC} Instalando dependências do medicwarehouse-app..."
    cd "$SCRIPT_DIR/frontend/medicwarehouse-app"
    npm install --silent
    cd "$SCRIPT_DIR"
    echo -e "${GREEN}✓${NC} Dependências do medicwarehouse-app instaladas"
fi

# System Admin
if [ -d "$SCRIPT_DIR/frontend/mw-system-admin" ]; then
    echo -e "${YELLOW}→${NC} Instalando dependências do mw-system-admin..."
    cd "$SCRIPT_DIR/frontend/mw-system-admin"
    npm install --silent
    cd "$SCRIPT_DIR"
    echo -e "${GREEN}✓${NC} Dependências do mw-system-admin instaladas"
fi

# Documentação
if [ -d "$SCRIPT_DIR/frontend/mw-docs" ]; then
    echo -e "${YELLOW}→${NC} Instalando dependências do mw-docs..."
    cd "$SCRIPT_DIR/frontend/mw-docs"
    npm install --silent
    cd "$SCRIPT_DIR"
    echo -e "${GREEN}✓${NC} Dependências do mw-docs instaladas"
fi

# Site institucional
if [ -d "$SCRIPT_DIR/frontend/mw-site" ]; then
    echo -e "${YELLOW}→${NC} Instalando dependências do mw-site..."
    cd "$SCRIPT_DIR/frontend/mw-site"
    npm install --silent
    cd "$SCRIPT_DIR"
    echo -e "${GREEN}✓${NC} Dependências do mw-site instaladas"
fi
echo ""

# Popular banco de dados com dados demo
echo -e "${BLUE}[11/11] Populando banco de dados com dados demo...${NC}"
echo -e "${YELLOW}→${NC} Verificando se a API está rodando para popular dados..."
if curl -s http://localhost:5000/health > /dev/null 2>&1; then
    echo -e "${YELLOW}→${NC} Executando data seeder..."
    if curl -s -X POST http://localhost:5000/api/data-seeder/seed-demo > /dev/null 2>&1; then
        echo -e "${GREEN}✓${NC} Dados demo populados com sucesso"
    else
        echo -e "${YELLOW}⚠️  Não foi possível popular dados demo automaticamente${NC}"
        echo -e "${YELLOW}   Execute após iniciar a API: curl -X POST http://localhost:5000/api/data-seeder/seed-demo${NC}"
    fi
else
    echo -e "${YELLOW}⚠️  API não está rodando. Pule esta etapa por enquanto.${NC}"
    echo -e "${YELLOW}   Execute após iniciar a API: curl -X POST http://localhost:5000/api/data-seeder/seed-demo${NC}"
fi
echo ""

# Resumo final
echo -e "${GREEN}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  ✅ Configuração Concluída!                            ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${BLUE}📋 Ferramentas instaladas:${NC}"
echo -e "   • .NET SDK: $(dotnet --version 2>/dev/null || echo 'Instalado')"
echo -e "   • Node.js: $(node --version 2>/dev/null || echo 'Instalado')"
echo -e "   • npm: $(npm --version 2>/dev/null || echo 'Instalado')"
echo -e "   • Podman: $(podman --version 2>/dev/null | head -1 || echo 'Instalado')"
echo -e "   • Git: $(git --version 2>/dev/null || echo 'Instalado')"
echo ""
echo -e "${BLUE}🗄️  Serviços de Infraestrutura:${NC}"
echo -e "   • PostgreSQL: ${YELLOW}http://localhost:5432${NC} (Database: medicwarehouse)"
echo ""
echo -e "${BLUE}🚀 Backend APIs Disponíveis:${NC}"
echo ""
echo -e "${CYAN}Monolítico (Modo Tradicional):${NC}"
echo -e "   • API Principal: ${YELLOW}http://localhost:5000${NC}"
echo -e "     - Swagger: ${YELLOW}http://localhost:5000/swagger${NC}"
echo -e "     - Inicie com: ${YELLOW}cd src/MedicSoft.Api && dotnet run${NC}"
echo ""
echo -e "${CYAN}Microserviços (Modo Arquitetura Moderna):${NC}"
echo -e "   • Auth API: ${YELLOW}http://localhost:5001${NC}"
echo -e "   • Patients API: ${YELLOW}http://localhost:5002${NC}"
echo -e "   • Appointments API: ${YELLOW}http://localhost:5003${NC}"
echo -e "   • Medical Records API: ${YELLOW}http://localhost:5004${NC}"
echo -e "   • Billing API: ${YELLOW}http://localhost:5005${NC}"
echo -e "   • System Admin API: ${YELLOW}http://localhost:5006${NC}"
echo -e "   • Telemedicine API: ${YELLOW}http://localhost:5084${NC}"
echo -e "   - Inicie todos com: ${YELLOW}podman-compose -f docker-compose.microservices.yml up -d${NC}"
echo ""
echo -e "${BLUE}🖥️  Frontend Applications:${NC}"
echo -e "   • MedicWarehouse App (Clínicas): ${YELLOW}http://localhost:4200${NC}"
echo -e "     - Inicie com: ${YELLOW}cd frontend/medicwarehouse-app && npm start${NC}"
echo -e "   • System Admin Panel: ${YELLOW}http://localhost:4201${NC}"
echo -e "     - Inicie com: ${YELLOW}cd frontend/mw-system-admin && npm start${NC}"
echo -e "   • Documentação (mw-docs): ${YELLOW}http://localhost:4202${NC}"
echo -e "     - Inicie com: ${YELLOW}cd frontend/mw-docs && npm start${NC}"
echo -e "   • Site Institucional: ${YELLOW}http://localhost:4203${NC}"
echo -e "     - Inicie com: ${YELLOW}cd frontend/mw-site && npm start${NC}"
echo ""
echo -e "${BLUE}📚 Comandos Úteis do Podman:${NC}"
echo ""
echo -e "${CYAN}Modo Monolítico:${NC}"
echo -e "   ${YELLOW}podman-compose up -d${NC}              # Inicia toda a stack (API + Frontend)"
echo -e "   ${YELLOW}podman-compose up postgres -d${NC}     # Inicia apenas PostgreSQL"
echo -e "   ${YELLOW}podman-compose down${NC}               # Para todos os containers"
echo -e "   ${YELLOW}podman-compose logs -f${NC}            # Ver logs em tempo real"
echo -e "   ${YELLOW}podman-compose ps${NC}                 # Lista containers rodando"
echo ""
echo -e "${CYAN}Modo Microserviços:${NC}"
echo -e "   ${YELLOW}podman-compose -f docker-compose.microservices.yml up -d${NC}    # Inicia todos os microserviços"
echo -e "   ${YELLOW}podman-compose -f docker-compose.microservices.yml down${NC}     # Para todos os microserviços"
echo -e "   ${YELLOW}podman-compose -f docker-compose.microservices.yml logs -f${NC}  # Ver logs"
echo ""
echo -e "${BLUE}📚 Próximos Passos Recomendados:${NC}"
echo ""
echo -e "${CYAN}Opção 1 - Modo Simples (Desenvolvimento Rápido):${NC}"
echo -e "   1. ${YELLOW}podman-compose up postgres -d${NC}  (já iniciado ✓)"
echo -e "   2. ${YELLOW}cd src/MedicSoft.Api && dotnet run${NC}"
echo -e "   3. ${YELLOW}curl -X POST http://localhost:5000/api/data-seeder/seed-demo${NC}"
echo -e "   4. ${YELLOW}cd ../../frontend/medicwarehouse-app && npm start${NC}"
echo -e "   5. Acesse: ${YELLOW}http://localhost:4200${NC}"
echo ""
echo -e "${CYAN}Opção 2 - Modo Completo com Podman-Compose (Stack Inteira):${NC}"
echo -e "   1. ${YELLOW}podman-compose up -d${NC}"
echo -e "   2. Aguarde ~30s para inicialização"
echo -e "   3. ${YELLOW}curl -X POST http://localhost:5000/api/data-seeder/seed-demo${NC}"
echo -e "   4. Acesse: ${YELLOW}http://localhost:4200${NC}"
echo ""
echo -e "${CYAN}Opção 3 - Modo Microserviços (Arquitetura Moderna):${NC}"
echo -e "   1. ${YELLOW}podman-compose -f docker-compose.microservices.yml up -d${NC}"
echo -e "   2. Aguarde ~45s para todos os serviços iniciarem"
echo -e "   3. ${YELLOW}curl -X POST http://localhost:5000/api/data-seeder/seed-demo${NC}"
echo -e "   4. Configure frontend: ${YELLOW}useMicroservices: true${NC} no environment.ts"
echo -e "   5. Acesse: ${YELLOW}http://localhost:4200${NC}"
echo ""
echo -e "${BLUE}🔐 Credenciais Demo:${NC}"
echo -e "   • Proprietário: ${YELLOW}owner.demo${NC} / ${YELLOW}Pass@123${NC}"
echo -e "   • Admin: ${YELLOW}admin.demo${NC} / ${YELLOW}Pass@123${NC}"
echo -e "   • Médico: ${YELLOW}doctor.demo${NC} / ${YELLOW}Pass@123${NC}"
echo -e "   • Recepcionista: ${YELLOW}receptionist.demo${NC} / ${YELLOW}Pass@123${NC}"
echo ""
echo -e "${BLUE}📖 Documentação Completa:${NC}"
echo -e "   • ${YELLOW}docs/GUIA_INICIO_RAPIDO_LOCAL.md${NC} - Guia passo a passo"
echo -e "   • ${YELLOW}docs/GUIA_MULTIPLATAFORMA.md${NC} - macOS, Windows, Linux"
echo -e "   • ${YELLOW}README.md${NC} - Visão geral do projeto"
echo -e "   • ${YELLOW}microservices/README.md${NC} - Arquitetura de microserviços"
echo -e "   • ${YELLOW}telemedicine/README.md${NC} - Microserviço de telemedicina"
echo ""
echo -e "${GREEN}🎉 Ambiente pronto para desenvolvimento!${NC}"
echo ""
echo -e "${YELLOW}💡 Dica: Use 'podman-compose ps' para verificar quais serviços estão rodando${NC}"
echo ""
