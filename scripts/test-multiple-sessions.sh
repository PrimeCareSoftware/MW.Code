#!/bin/bash

# Script para testar múltiplas sessões simultâneas
# Este script verifica se a correção de sessões está funcionando corretamente

set -e

# Cores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Configurações
API_URL="http://localhost:5000/api"
AUTH_API_URL="http://localhost:5000/api"  # Main API (microservices descontinuados)
TENANT_ID="demo-clinic-001"

echo -e "${BLUE}╔══════════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  Teste de Múltiplas Sessões Simultâneas                     ║${NC}"
echo -e "${BLUE}╚══════════════════════════════════════════════════════════════╝${NC}"
echo ""

# Verificar se a API está rodando
echo -e "${YELLOW}→${NC} Verificando se a API está disponível..."
if ! curl -s -f "${AUTH_API_URL}/auth/health" > /dev/null 2>&1; then
    echo -e "${RED}✗${NC} API não está rodando em ${AUTH_API_URL}"
    echo -e "${YELLOW}Dica:${NC} Execute 'cd src/MedicSoft.Api && dotnet run'"
    exit 1
fi
echo -e "${GREEN}✓${NC} API está disponível"
echo ""

# Função para fazer login
do_login() {
    local description=$1
    echo -e "${YELLOW}→${NC} ${description}"
    
    LOGIN_DATA='{
      "username": "admin",
      "password": "Admin@123",
      "tenantId": "'${TENANT_ID}'"
    }'
    
    RESPONSE=$(curl -s -X POST "${AUTH_API_URL}/auth/login" \
        -H "Content-Type: application/json" \
        -d "$LOGIN_DATA")
    
    TOKEN=$(echo $RESPONSE | jq -r '.token' 2>/dev/null)
    
    if [ "$TOKEN" != "null" ] && [ ! -z "$TOKEN" ]; then
        echo -e "${GREEN}✓${NC} Login realizado com sucesso!"
        echo "Token (primeiros 50 chars): ${TOKEN:0:50}..."
        echo "$TOKEN"
    else
        echo -e "${RED}✗${NC} Falha no login!"
        echo "$RESPONSE"
        return 1
    fi
    echo ""
}

# Função para validar sessão
validate_session() {
    local token=$1
    local description=$2
    
    echo -e "${YELLOW}→${NC} ${description}"
    
    VALIDATE_DATA='{
      "token": "'${token}'"
    }'
    
    RESPONSE=$(curl -s -X POST "${AUTH_API_URL}/auth/validate-session" \
        -H "Content-Type: application/json" \
        -d "$VALIDATE_DATA")
    
    IS_VALID=$(echo $RESPONSE | jq -r '.isValid' 2>/dev/null)
    MESSAGE=$(echo $RESPONSE | jq -r '.message' 2>/dev/null)
    
    if [ "$IS_VALID" == "true" ]; then
        echo -e "${GREEN}✓${NC} Sessão válida: $MESSAGE"
        return 0
    else
        echo -e "${RED}✗${NC} Sessão inválida: $MESSAGE"
        return 1
    fi
    echo ""
}

# Teste 1: Login único
echo -e "${BLUE}[Teste 1/4] Login único${NC}"
TOKEN1=$(do_login "Fazendo login pela primeira vez")
sleep 2

# Teste 2: Validar primeira sessão
echo -e "${BLUE}[Teste 2/4] Validar primeira sessão${NC}"
validate_session "$TOKEN1" "Validando sessão 1"
sleep 2

# Teste 3: Segundo login (simulando outra aba/dispositivo)
echo -e "${BLUE}[Teste 3/4] Segundo login (nova sessão)${NC}"
TOKEN2=$(do_login "Fazendo login novamente (simulando outra aba)")
sleep 2

# Teste 4: Validar ambas as sessões
echo -e "${BLUE}[Teste 4/4] Validar ambas as sessões${NC}"
echo ""

SUCCESS_COUNT=0
TOTAL_TESTS=2

echo -e "${YELLOW}→${NC} Validando primeira sessão após segundo login..."
if validate_session "$TOKEN1" "Validando sessão 1 (deve ainda estar válida)"; then
    SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
fi
sleep 1

echo -e "${YELLOW}→${NC} Validando segunda sessão..."
if validate_session "$TOKEN2" "Validando sessão 2 (deve estar válida)"; then
    SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
fi
sleep 1

# Resultado final
echo ""
echo -e "${BLUE}╔══════════════════════════════════════════════════════════════╗${NC}"
if [ $SUCCESS_COUNT -eq $TOTAL_TESTS ]; then
    echo -e "${GREEN}║  ✓ SUCESSO! Todas as sessões estão funcionando!             ║${NC}"
    echo -e "${GREEN}║                                                              ║${NC}"
    echo -e "${GREEN}║  O sistema agora suporta múltiplas sessões simultâneas!     ║${NC}"
    echo -e "${BLUE}╚══════════════════════════════════════════════════════════════╝${NC}"
    exit 0
else
    echo -e "${RED}║  ✗ FALHA! Algumas sessões foram invalidadas                 ║${NC}"
    echo -e "${RED}║                                                              ║${NC}"
    echo -e "${RED}║  Resultado: $SUCCESS_COUNT/$TOTAL_TESTS testes passaram                       ║${NC}"
    echo -e "${BLUE}╚══════════════════════════════════════════════════════════════╝${NC}"
    exit 1
fi
