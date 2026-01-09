#!/bin/bash

# Script para gerar documentação portátil do PrimeCare Software
# Pode ser executado de qualquer lugar do repositório

# Cores para output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  📱 Gerador de Documentação Portátil - PrimeCare Software  ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════╝${NC}"
echo ""

# Detectar diretório raiz do projeto
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Verificar se Node.js está instalado
if ! command -v node &> /dev/null; then
    echo -e "${YELLOW}⚠️  Node.js não encontrado. Por favor, instale Node.js 18+ para continuar.${NC}"
    exit 1
fi

# Verificar se dependências estão instaladas
if [ ! -d "node_modules" ]; then
    echo -e "${YELLOW}📦 Instalando dependências...${NC}"
    npm install
    echo ""
fi

# Executar o gerador
echo -e "${GREEN}🚀 Gerando documentação...${NC}"
echo ""
node gerar-documentacao.js

echo ""
echo -e "${GREEN}✅ Concluído!${NC}"
echo ""
echo -e "${BLUE}📖 Arquivos gerados:${NC}"
echo -e "   - PrimeCare Software-Documentacao-Completa.md"
echo -e "   - PrimeCare Software-Documentacao-Completa.html"
echo ""
echo -e "${BLUE}💡 Próximos passos:${NC}"
echo -e "   1. Abra o arquivo HTML no navegador"
echo -e "   2. Para PDF: Ctrl+P > Salvar como PDF"
echo -e "   3. Para mobile: Transfira o HTML para seu celular"
echo ""
