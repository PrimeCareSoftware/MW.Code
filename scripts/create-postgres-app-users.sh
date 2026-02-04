#!/bin/bash
# ================================================
# Script de Criação de Usuários PostgreSQL
# Omni Care Software - Usuários de Aplicação
# ================================================
#
# Este script cria usuários de aplicação para o PostgreSQL
# com as permissões mínimas necessárias, seguindo o princípio
# do menor privilégio.
#
# USO:
#   ./create-postgres-app-users.sh
#
# PRÉ-REQUISITOS:
#   - PostgreSQL instalado e rodando
#   - Acesso como superusuário (postgres)
#   - psql disponível no PATH
#

set -e  # Parar em caso de erro

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para imprimir mensagens
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Banner
echo "================================================"
echo "  Criação de Usuários PostgreSQL - Omni Care"
echo "================================================"
echo ""

# Verificar se psql está disponível
if ! command -v psql &> /dev/null; then
    print_error "psql não encontrado. Instale PostgreSQL client."
    exit 1
fi

# Configurações padrão
DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
ADMIN_USER="${ADMIN_USER:-postgres}"

print_info "Configuração:"
echo "  Host: $DB_HOST"
echo "  Porta: $DB_PORT"
echo "  Admin User: $ADMIN_USER"
echo ""

# Pedir senha do admin
echo -n "Senha do usuário $ADMIN_USER: "
read -s ADMIN_PASSWORD
echo ""

# Testar conexão
print_info "Testando conexão com PostgreSQL..."
export PGPASSWORD="$ADMIN_PASSWORD"
if ! psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -c '\q' 2>/dev/null; then
    print_error "Falha ao conectar ao PostgreSQL. Verifique credenciais."
    exit 1
fi
print_success "Conectado com sucesso!"
echo ""

# ================================================
# FUNÇÕES
# ================================================

generate_password() {
    # Gera senha aleatória segura (32 caracteres)
    openssl rand -base64 32 | tr -d "=+/" | cut -c1-32
}

create_user() {
    local username=$1
    local password=$2
    local description=$3
    
    print_info "Criando usuário: $username"
    
    # Verificar se usuário já existe
    local user_exists=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -tAc "SELECT 1 FROM pg_roles WHERE rolname='$username'")
    
    if [ "$user_exists" = "1" ]; then
        print_warning "Usuário $username já existe. Pulando criação..."
        return 0
    fi
    
    # Criar usuário
    psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" <<EOF
        CREATE USER $username WITH PASSWORD '$password';
        COMMENT ON ROLE $username IS '$description';
EOF
    
    print_success "Usuário $username criado!"
}

grant_app_permissions() {
    local username=$1
    local database=$2
    
    print_info "Concedendo permissões DML para $username em $database..."
    
    psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -d "$database" <<EOF
        -- Conectar ao database
        GRANT CONNECT ON DATABASE $database TO $username;
        GRANT USAGE ON SCHEMA public TO $username;
        
        -- Permissões DML em tabelas existentes
        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO $username;
        
        -- Permissões em sequences
        GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO $username;
        
        -- Permissões padrão para objetos futuros
        ALTER DEFAULT PRIVILEGES IN SCHEMA public 
            GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO $username;
        
        ALTER DEFAULT PRIVILEGES IN SCHEMA public 
            GRANT USAGE, SELECT ON SEQUENCES TO $username;
        
        -- Configurar timeouts
        ALTER ROLE $username SET statement_timeout = '30s';
        ALTER ROLE $username SET idle_in_transaction_session_timeout = '60s';
EOF
    
    print_success "Permissões concedidas para $username em $database"
}

grant_readonly_permissions() {
    local username=$1
    local database=$2
    
    print_info "Concedendo permissões READONLY para $username em $database..."
    
    psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -d "$database" <<EOF
        -- Conectar ao database
        GRANT CONNECT ON DATABASE $database TO $username;
        GRANT USAGE ON SCHEMA public TO $username;
        
        -- Apenas SELECT
        GRANT SELECT ON ALL TABLES IN SCHEMA public TO $username;
        
        -- Permissões padrão para tabelas futuras
        ALTER DEFAULT PRIVILEGES IN SCHEMA public 
            GRANT SELECT ON TABLES TO $username;
        
        -- Configurar para readonly
        ALTER ROLE $username SET statement_timeout = '120s';
        ALTER ROLE $username SET default_transaction_read_only = on;
EOF
    
    print_success "Permissões readonly concedidas para $username em $database"
}

save_credentials() {
    local filename="postgres-credentials-$(date +%Y%m%d-%H%M%S).txt"
    
    print_info "Salvando credenciais em $filename..."
    
    cat > "$filename" <<EOF
# ================================================
# Credenciais PostgreSQL - Omni Care Software
# Gerado em: $(date)
# ================================================

# IMPORTANTE:
# 1. Guarde este arquivo em local seguro (1Password, Vault, etc)
# 2. Não commite este arquivo no Git
# 3. Delete este arquivo após copiar as credenciais
# 4. Configure as connection strings conforme instruções

# ================================================
# APLICAÇÃO PRINCIPAL (primecare)
# ================================================
Usuário: omnicare_app
Senha: $APP_PASSWORD

Connection String:
Host=$DB_HOST;Port=$DB_PORT;Database=primecare;Username=omnicare_app;Password=$APP_PASSWORD;SSL Mode=Require

# ================================================
# PORTAL DO PACIENTE (patientportal)
# ================================================
Usuário: patientportal_app
Senha: $PORTAL_PASSWORD

Connection String:
Host=$DB_HOST;Port=$DB_PORT;Database=patientportal;Username=patientportal_app;Password=$PORTAL_PASSWORD;SSL Mode=Require

# ================================================
# TELEMEDICINA (telemedicine)
# ================================================
Usuário: telemedicine_app
Senha: $TELE_PASSWORD

Connection String:
Host=$DB_HOST;Port=$DB_PORT;Database=telemedicine;Username=telemedicine_app;Password=$TELE_PASSWORD;SSL Mode=Require

# ================================================
# READONLY (Relatórios/BI)
# ================================================
Usuário: omnicare_readonly
Senha: $READONLY_PASSWORD

Connection String:
Host=$DB_HOST;Port=$DB_PORT;Database=primecare;Username=omnicare_readonly;Password=$READONLY_PASSWORD;SSL Mode=Require

# ================================================
# PRÓXIMOS PASSOS
# ================================================

1. Copie as connection strings acima

2. Atualize appsettings.json:
   {
     "ConnectionStrings": {
       "DefaultConnection": "<connection string acima>"
     }
   }

3. Para produção, use variáveis de ambiente:
   export DB_USER=omnicare_app
   export DB_PASSWORD=$APP_PASSWORD

4. DELETE este arquivo após copiar as credenciais!

EOF
    
    # Restringir permissões do arquivo
    chmod 600 "$filename"
    
    print_success "Credenciais salvas em: $filename"
    print_warning "IMPORTANTE: Guarde este arquivo em local seguro e delete após usar!"
}

# ================================================
# EXECUÇÃO PRINCIPAL
# ================================================

echo ""
print_info "Iniciando criação de usuários..."
echo ""

# Gerar senhas
print_info "Gerando senhas seguras..."
APP_PASSWORD=$(generate_password)
PORTAL_PASSWORD=$(generate_password)
TELE_PASSWORD=$(generate_password)
READONLY_PASSWORD=$(generate_password)
print_success "Senhas geradas!"
echo ""

# ================================================
# 1. BANCO PRINCIPAL (primecare)
# ================================================
echo "================================================"
echo "  1. Banco Principal (primecare)"
echo "================================================"

# Verificar se database existe
DB_EXISTS=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -tAc "SELECT 1 FROM pg_database WHERE datname='primecare'")
if [ "$DB_EXISTS" != "1" ]; then
    print_info "Banco 'primecare' não existe. Criando..."
    psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -c "CREATE DATABASE primecare"
    print_success "Banco 'primecare' criado!"
fi

create_user "omnicare_app" "$APP_PASSWORD" "Usuário da aplicação principal - DML access"
grant_app_permissions "omnicare_app" "primecare"

echo ""

# ================================================
# 2. PORTAL DO PACIENTE (patientportal)
# ================================================
echo "================================================"
echo "  2. Portal do Paciente (patientportal)"
echo "================================================"

DB_EXISTS=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -tAc "SELECT 1 FROM pg_database WHERE datname='patientportal'")
if [ "$DB_EXISTS" != "1" ]; then
    print_info "Banco 'patientportal' não existe. Criando..."
    psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -c "CREATE DATABASE patientportal"
    print_success "Banco 'patientportal' criado!"
fi

create_user "patientportal_app" "$PORTAL_PASSWORD" "Usuário do Portal do Paciente - DML access"
grant_app_permissions "patientportal_app" "patientportal"

echo ""

# ================================================
# 3. TELEMEDICINA (telemedicine)
# ================================================
echo "================================================"
echo "  3. Telemedicina (telemedicine)"
echo "================================================"

DB_EXISTS=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -tAc "SELECT 1 FROM pg_database WHERE datname='telemedicine'")
if [ "$DB_EXISTS" != "1" ]; then
    print_info "Banco 'telemedicine' não existe. Criando..."
    psql -h "$DB_HOST" -p "$DB_PORT" -U "$ADMIN_USER" -c "CREATE DATABASE telemedicine"
    print_success "Banco 'telemedicine' criado!"
fi

create_user "telemedicine_app" "$TELE_PASSWORD" "Usuário da Telemedicina - DML access"
grant_app_permissions "telemedicine_app" "telemedicine"

echo ""

# ================================================
# 4. READONLY USER
# ================================================
echo "================================================"
echo "  4. Usuário Readonly (Relatórios/BI)"
echo "================================================"

create_user "omnicare_readonly" "$READONLY_PASSWORD" "Usuário somente leitura - Reports e BI"
grant_readonly_permissions "omnicare_readonly" "primecare"
grant_readonly_permissions "omnicare_readonly" "patientportal"
grant_readonly_permissions "omnicare_readonly" "telemedicine"

echo ""

# ================================================
# SALVAR CREDENCIAIS
# ================================================
save_credentials

echo ""
echo "================================================"
echo "  ✅ Setup Completo!"
echo "================================================"
echo ""
print_success "Todos os usuários foram criados com sucesso!"
echo ""
print_info "Próximos passos:"
echo "  1. Copie as credenciais do arquivo gerado"
echo "  2. Atualize as connection strings nos appsettings"
echo "  3. DELETE o arquivo de credenciais após usar"
echo "  4. Configure variáveis de ambiente em produção"
echo "  5. Leia: system-admin/seguranca/POSTGRES_APP_USER_GUIDE.md"
echo ""
print_warning "LEMBRE-SE: Nunca use o usuário 'postgres' nas connection strings!"
echo ""

# Limpar variável de senha
unset PGPASSWORD
