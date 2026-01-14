#!/bin/bash

# 🌱 Script para popular o banco de dados com dados de exemplo
# Este script cria automaticamente todos os dados necessários para começar a testar o sistema

set -e  # Exit on error

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configurações padrão
API_URL="${API_URL:-http://localhost:5000}"
API_HEALTH_ENDPOINT="${API_URL}/health"
API_SEED_ENDPOINT="${API_URL}/api/data-seeder/seed-demo"
API_INFO_ENDPOINT="${API_URL}/api/data-seeder/demo-info"
API_LOGIN_ENDPOINT="${API_URL}/api/auth/login"

# Função para exibir mensagens
print_header() {
    echo ""
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
    echo ""
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

# Função para verificar se a API está rodando
check_api_health() {
    print_info "Verificando se a API está rodando..."
    
    if curl -s -f "$API_HEALTH_ENDPOINT" > /dev/null 2>&1; then
        print_success "API está rodando em $API_URL"
        return 0
    else
        print_error "API não está acessível em $API_URL"
        print_info "Verifique se a aplicação está rodando:"
        print_info "  - dotnet run (no diretório src/MedicSoft.Api)"
        print_info "  - ou docker-compose up -d / podman-compose up -d"
        return 1
    fi
}

# Função para obter informações sobre os dados que serão criados
get_demo_info() {
    print_info "Obtendo informações sobre os dados de exemplo..."
    
    response=$(curl -s "$API_INFO_ENDPOINT")
    
    if [ $? -eq 0 ]; then
        print_success "Informações obtidas com sucesso!"
        echo ""
        echo "$response" | jq -r '.entities[]' 2>/dev/null || echo "$response"
        echo ""
    else
        print_warning "Não foi possível obter informações sobre os dados"
    fi
}

# Função para popular os dados
seed_demo_data() {
    print_info "Populando banco de dados com dados de exemplo..."
    
    response=$(curl -s -w "\n%{http_code}" -X POST "$API_SEED_ENDPOINT")
    http_code=$(echo "$response" | tail -n 1)
    body=$(echo "$response" | head -n -1)
    
    if [ "$http_code" = "200" ]; then
        print_success "Dados de exemplo criados com sucesso!"
        echo ""
        
        # Extrair e exibir credenciais
        tenant_id=$(echo "$body" | jq -r '.tenantId' 2>/dev/null)
        owner_username=$(echo "$body" | jq -r '.credentials.owner.username' 2>/dev/null)
        owner_password=$(echo "$body" | jq -r '.credentials.owner.password' 2>/dev/null)
        
        echo -e "${GREEN}📋 TenantID: $tenant_id${NC}"
        echo ""
        echo -e "${GREEN}🔑 Credenciais de acesso:${NC}"
        echo ""
        echo -e "${YELLOW}👑 Owner (Proprietário):${NC}"
        echo "   Username: $owner_username"
        echo "   Password: $owner_password"
        echo ""
        
        echo -e "${YELLOW}👥 Usuários:${NC}"
        echo "$body" | jq -r '.credentials.users[] | "   \(.role): \(.username) / \(.password)"' 2>/dev/null
        echo ""
        
        # Salvar credenciais em arquivo
        echo "$body" | jq '.' > /tmp/demo-credentials.json 2>/dev/null
        print_success "Credenciais salvas em: /tmp/demo-credentials.json"
        
        return 0
    elif [ "$http_code" = "400" ]; then
        print_warning "Os dados de exemplo já existem no banco de dados"
        
        error_msg=$(echo "$body" | jq -r '.error' 2>/dev/null)
        if [ ! -z "$error_msg" ] && [ "$error_msg" != "null" ]; then
            print_info "Mensagem: $error_msg"
        fi
        
        print_info ""
        print_info "Opções:"
        print_info "  1. Use as credenciais existentes (veja docs/SEED_API_GUIDE.md)"
        print_info "  2. Limpe o banco e recrie: curl -X DELETE $API_URL/api/data-seeder/clear-database"
        print_info "  3. Então execute este script novamente"
        
        return 2
    else
        print_error "Erro ao criar dados de exemplo (HTTP $http_code)"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
        return 1
    fi
}

# Função para fazer login e obter token
login_and_get_token() {
    local username="$1"
    local password="$2"
    local tenant_id="$3"
    
    print_info "Fazendo login como $username..."
    
    response=$(curl -s -w "\n%{http_code}" -X POST "$API_LOGIN_ENDPOINT" \
        -H "Content-Type: application/json" \
        -d "{
            \"username\": \"$username\",
            \"password\": \"$password\",
            \"tenantId\": \"$tenant_id\"
        }")
    
    http_code=$(echo "$response" | tail -n 1)
    body=$(echo "$response" | head -n -1)
    
    if [ "$http_code" = "200" ]; then
        token=$(echo "$body" | jq -r '.token' 2>/dev/null)
        
        if [ ! -z "$token" ] && [ "$token" != "null" ]; then
            print_success "Login realizado com sucesso!"
            echo ""
            echo -e "${GREEN}🎟️  Token JWT:${NC}"
            echo "$token"
            echo ""
            
            # Salvar token em arquivo
            echo "$token" > /tmp/jwt-token.txt
            print_success "Token salvo em: /tmp/jwt-token.txt"
            
            echo ""
            print_info "Use este token nas próximas requisições:"
            echo ""
            echo "curl -X GET $API_URL/api/patients \\"
            echo "  -H \"Authorization: Bearer $token\" \\"
            echo "  -H \"X-Tenant-ID: $tenant_id\""
            
            return 0
        else
            print_error "Token não foi retornado na resposta"
            return 1
        fi
    else
        print_error "Erro ao fazer login (HTTP $http_code)"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
        return 1
    fi
}

# Função principal
main() {
    print_header "🌱 PrimeCare - Seed Demo Data"
    
    echo -e "${BLUE}Este script irá:${NC}"
    echo "  1. Verificar se a API está rodando"
    echo "  2. Obter informações sobre os dados de exemplo"
    echo "  3. Popular o banco de dados com dados completos"
    echo "  4. Fazer login e obter um token JWT"
    echo ""
    
    # Verificar se jq está instalado
    if ! command -v jq &> /dev/null; then
        print_warning "jq não está instalado. Alguns outputs podem não ser formatados."
        print_info "Para instalar: apt-get install jq (Ubuntu/Debian) ou brew install jq (macOS)"
        echo ""
    fi
    
    # 1. Verificar se a API está rodando
    if ! check_api_health; then
        exit 1
    fi
    
    echo ""
    
    # 2. Obter informações sobre os dados
    get_demo_info
    
    # Perguntar se o usuário deseja continuar
    echo ""
    read -p "Deseja popular o banco de dados com estes dados? (s/N) " -n 1 -r
    echo ""
    
    if [[ ! $REPLY =~ ^[SsYy]$ ]]; then
        print_info "Operação cancelada pelo usuário"
        exit 0
    fi
    
    echo ""
    
    # 3. Popular dados
    if ! seed_demo_data; then
        if [ $? -eq 2 ]; then
            # Dados já existem, perguntar se quer fazer login
            echo ""
            read -p "Deseja fazer login com as credenciais existentes? (s/N) " -n 1 -r
            echo ""
            
            if [[ $REPLY =~ ^[SsYy]$ ]]; then
                echo ""
                login_and_get_token "dr.silva" "Doctor@123" "demo-clinic-001"
            fi
        fi
        exit 0
    fi
    
    echo ""
    
    # 4. Fazer login
    print_header "🔐 Autenticação"
    
    read -p "Deseja fazer login automaticamente? (S/n) " -n 1 -r
    echo ""
    
    if [[ ! $REPLY =~ ^[Nn]$ ]]; then
        echo ""
        login_and_get_token "dr.silva" "Doctor@123" "demo-clinic-001"
    fi
    
    echo ""
    print_header "🎉 Pronto!"
    
    print_success "Sistema pronto para testes!"
    echo ""
    print_info "Próximos passos:"
    print_info "  📖 Ver documentação: docs/SEED_API_GUIDE.md"
    print_info "  📮 Usar Postman: Importe PrimeCare-Postman-Collection.json"
    print_info "  🌐 Acessar frontend: http://localhost:4200"
    print_info "  📱 Testar APIs: Use o token salvo em /tmp/jwt-token.txt"
    echo ""
}

# Executar script
main "$@"
