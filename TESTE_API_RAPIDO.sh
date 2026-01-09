#!/bin/bash

# Script de Teste Rápido da API PrimeCare Software
# Este script testa os endpoints principais da API

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configurações
API_URL="http://localhost:5000/api"
TENANT_ID="demo-clinic-001"

echo -e "${BLUE}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  PrimeCare Software - Teste Rápido de API                 ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════╝${NC}"
echo ""

# Função para testar endpoint
test_endpoint() {
    local method=$1
    local endpoint=$2
    local description=$3
    local data=$4
    local token=$5
    
    echo -e "${YELLOW}→${NC} Testando: ${description}"
    
    if [ -z "$token" ]; then
        if [ -z "$data" ]; then
            response=$(curl -s -X $method "${API_URL}${endpoint}" -H "Content-Type: application/json")
        else
            response=$(curl -s -X $method "${API_URL}${endpoint}" -H "Content-Type: application/json" -d "$data")
        fi
    else
        if [ -z "$data" ]; then
            response=$(curl -s -X $method "${API_URL}${endpoint}" \
                -H "Content-Type: application/json" \
                -H "Authorization: Bearer $token" \
                -H "X-Tenant-Id: $TENANT_ID")
        else
            response=$(curl -s -X $method "${API_URL}${endpoint}" \
                -H "Content-Type: application/json" \
                -H "Authorization: Bearer $token" \
                -H "X-Tenant-Id: $TENANT_ID" \
                -d "$data")
        fi
    fi
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓${NC} Sucesso: $endpoint"
        echo "$response" | jq '.' 2>/dev/null || echo "$response"
    else
        echo -e "${RED}✗${NC} Falhou: $endpoint"
        echo "$response"
    fi
    echo ""
}

# 1. Verificar informações do seeder
echo -e "${BLUE}[1/8] Verificando informações do seeder...${NC}"
test_endpoint "GET" "/data-seeder/demo-info" "Informações dos dados demo"

# 2. Popular dados demo (se necessário)
echo -e "${BLUE}[2/8] Populando dados demo...${NC}"
test_endpoint "POST" "/data-seeder/seed-demo" "Popular dados de demonstração"

# 3. Fazer login
echo -e "${BLUE}[3/8] Fazendo login...${NC}"
LOGIN_DATA='{
  "username": "admin",
  "password": "Admin@123",
  "tenantId": "demo-clinic-001"
}'
LOGIN_RESPONSE=$(curl -s -X POST "${API_URL}/auth/login" \
    -H "Content-Type: application/json" \
    -d "$LOGIN_DATA")

TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.token' 2>/dev/null)

if [ "$TOKEN" != "null" ] && [ ! -z "$TOKEN" ]; then
    echo -e "${GREEN}✓${NC} Login realizado com sucesso!"
    echo "Token: ${TOKEN:0:50}..."
else
    echo -e "${RED}✗${NC} Falha no login!"
    echo "$LOGIN_RESPONSE"
    exit 1
fi
echo ""

# 4. Listar pacientes
echo -e "${BLUE}[4/8] Listando pacientes...${NC}"
test_endpoint "GET" "/patients" "Listar todos os pacientes" "" "$TOKEN"

# 5. Listar agendamentos
echo -e "${BLUE}[5/8] Listando agendamentos...${NC}"
test_endpoint "GET" "/appointments" "Listar agendamentos" "" "$TOKEN"

# 6. Listar procedimentos
echo -e "${BLUE}[6/8] Listando procedimentos...${NC}"
test_endpoint "GET" "/procedures" "Listar procedimentos" "" "$TOKEN"

# 7. Relatório financeiro
echo -e "${BLUE}[7/8] Obtendo relatório financeiro...${NC}"
test_endpoint "GET" "/reports/financial-summary?startDate=2024-01-01&endDate=2024-12-31" "Resumo financeiro" "" "$TOKEN"

# 8. Listar medicamentos
echo -e "${BLUE}[8/8] Listando medicamentos...${NC}"
test_endpoint "GET" "/medications" "Listar medicamentos" "" "$TOKEN"

echo -e "${GREEN}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  Testes Concluídos!                                    ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${YELLOW}Dica:${NC} Acesse http://localhost:5000/swagger para testar mais endpoints!"
echo -e "${YELLOW}Dica:${NC} Use o token acima para autenticar no Swagger (botão Authorize)"
echo ""
