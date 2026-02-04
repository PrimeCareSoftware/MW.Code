# ================================================
# Script de Criação de Usuários PostgreSQL
# Omni Care Software - Usuários de Aplicação
# PowerShell Version
# ================================================
#
# Este script cria usuários de aplicação para o PostgreSQL
# com as permissões mínimas necessárias, seguindo o princípio
# do menor privilégio.
#
# USO:
#   .\create-postgres-app-users.ps1
#
# PRÉ-REQUISITOS:
#   - PostgreSQL instalado e rodando
#   - psql disponível no PATH
#

param(
    [string]$DBHost = "localhost",
    [string]$DBPort = "5432",
    [string]$AdminUser = "postgres"
)

# Função para gerar senha segura
function Generate-Password {
    $bytes = New-Object byte[] 32
    [Security.Cryptography.RNGCryptoServiceProvider]::Create().GetBytes($bytes)
    return [Convert]::ToBase64String($bytes).Substring(0, 32) -replace '[/+=]', ''
}

# Banner
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Criação de Usuários PostgreSQL - Omni Care" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se psql está disponível
if (-not (Get-Command psql -ErrorAction SilentlyContinue)) {
    Write-Host "[ERROR] psql não encontrado. Instale PostgreSQL client." -ForegroundColor Red
    exit 1
}

Write-Host "[INFO] Configuração:" -ForegroundColor Blue
Write-Host "  Host: $DBHost"
Write-Host "  Porta: $DBPort"
Write-Host "  Admin User: $AdminUser"
Write-Host ""

# Pedir senha do admin
$SecureAdminPassword = Read-Host "Senha do usuário $AdminUser" -AsSecureString
$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecureAdminPassword)
$AdminPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)

# Configurar variável de ambiente
$env:PGPASSWORD = $AdminPassword

# Testar conexão
Write-Host "[INFO] Testando conexão com PostgreSQL..." -ForegroundColor Blue
$null = & psql -h $DBHost -p $DBPort -U $AdminUser -c '\q' 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Falha ao conectar ao PostgreSQL. Verifique credenciais." -ForegroundColor Red
    exit 1
}
Write-Host "[SUCCESS] Conectado com sucesso!" -ForegroundColor Green
Write-Host ""

# Funções
function Create-User {
    param(
        [string]$Username,
        [string]$Password,
        [string]$Description
    )
    
    Write-Host "[INFO] Criando usuário: $Username" -ForegroundColor Blue
    
    # Verificar se usuário já existe
    $userExists = & psql -h $DBHost -p $DBPort -U $AdminUser -tAc "SELECT 1 FROM pg_roles WHERE rolname='$Username'"
    
    if ($userExists -eq "1") {
        Write-Host "[WARNING] Usuário $Username já existe. Pulando criação..." -ForegroundColor Yellow
        return
    }
    
    # Criar usuário
    $sql = @"
CREATE USER $Username WITH PASSWORD '$Password';
COMMENT ON ROLE $Username IS '$Description';
"@
    
    $sql | & psql -h $DBHost -p $DBPort -U $AdminUser
    Write-Host "[SUCCESS] Usuário $Username criado!" -ForegroundColor Green
}

function Grant-AppPermissions {
    param(
        [string]$Username,
        [string]$Database
    )
    
    Write-Host "[INFO] Concedendo permissões DML para $Username em $Database..." -ForegroundColor Blue
    
    $sql = @"
GRANT CONNECT ON DATABASE $Database TO $Username;
GRANT USAGE ON SCHEMA public TO $Username;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO $Username;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO $Username;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO $Username;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT ON SEQUENCES TO $Username;
ALTER ROLE $Username SET statement_timeout = '30s';
ALTER ROLE $Username SET idle_in_transaction_session_timeout = '60s';
"@
    
    $sql | & psql -h $DBHost -p $DBPort -U $AdminUser -d $Database
    Write-Host "[SUCCESS] Permissões concedidas para $Username em $Database" -ForegroundColor Green
}

function Grant-ReadonlyPermissions {
    param(
        [string]$Username,
        [string]$Database
    )
    
    Write-Host "[INFO] Concedendo permissões READONLY para $Username em $Database..." -ForegroundColor Blue
    
    $sql = @"
GRANT CONNECT ON DATABASE $Database TO $Username;
GRANT USAGE ON SCHEMA public TO $Username;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO $Username;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO $Username;
ALTER ROLE $Username SET statement_timeout = '120s';
ALTER ROLE $Username SET default_transaction_read_only = on;
"@
    
    $sql | & psql -h $DBHost -p $DBPort -U $AdminUser -d $Database
    Write-Host "[SUCCESS] Permissões readonly concedidas para $Username em $Database" -ForegroundColor Green
}

function Save-Credentials {
    param(
        [hashtable]$Credentials
    )
    
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $filename = "postgres-credentials-$timestamp.txt"
    
    Write-Host "[INFO] Salvando credenciais em $filename..." -ForegroundColor Blue
    
    $content = @"
# ================================================
# Credenciais PostgreSQL - Omni Care Software
# Gerado em: $(Get-Date)
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
Senha: $($Credentials.AppPassword)

Connection String:
Host=$DBHost;Port=$DBPort;Database=primecare;Username=omnicare_app;Password=$($Credentials.AppPassword);SSL Mode=Require

# ================================================
# PORTAL DO PACIENTE (patientportal)
# ================================================
Usuário: patientportal_app
Senha: $($Credentials.PortalPassword)

Connection String:
Host=$DBHost;Port=$DBPort;Database=patientportal;Username=patientportal_app;Password=$($Credentials.PortalPassword);SSL Mode=Require

# ================================================
# TELEMEDICINA (telemedicine)
# ================================================
Usuário: telemedicine_app
Senha: $($Credentials.TelePassword)

Connection String:
Host=$DBHost;Port=$DBPort;Database=telemedicine;Username=telemedicine_app;Password=$($Credentials.TelePassword);SSL Mode=Require

# ================================================
# READONLY (Relatórios/BI)
# ================================================
Usuário: omnicare_readonly
Senha: $($Credentials.ReadonlyPassword)

Connection String:
Host=$DBHost;Port=$DBPort;Database=primecare;Username=omnicare_readonly;Password=$($Credentials.ReadonlyPassword);SSL Mode=Require

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
   `$env:DB_USER = "omnicare_app"
   `$env:DB_PASSWORD = "$($Credentials.AppPassword)"

4. DELETE este arquivo após copiar as credenciais!
"@
    
    $content | Out-File -FilePath $filename -Encoding UTF8
    
    Write-Host "[SUCCESS] Credenciais salvas em: $filename" -ForegroundColor Green
    Write-Host "[WARNING] IMPORTANTE: Guarde este arquivo em local seguro e delete após usar!" -ForegroundColor Yellow
}

# ================================================
# EXECUÇÃO PRINCIPAL
# ================================================

Write-Host ""
Write-Host "[INFO] Iniciando criação de usuários..." -ForegroundColor Blue
Write-Host ""

# Gerar senhas
Write-Host "[INFO] Gerando senhas seguras..." -ForegroundColor Blue
$credentials = @{
    AppPassword = Generate-Password
    PortalPassword = Generate-Password
    TelePassword = Generate-Password
    ReadonlyPassword = Generate-Password
}
Write-Host "[SUCCESS] Senhas geradas!" -ForegroundColor Green
Write-Host ""

# ================================================
# 1. BANCO PRINCIPAL (primecare)
# ================================================
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  1. Banco Principal (primecare)" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

$dbExists = & psql -h $DBHost -p $DBPort -U $AdminUser -tAc "SELECT 1 FROM pg_database WHERE datname='primecare'"
if ($dbExists -ne "1") {
    Write-Host "[INFO] Banco 'primecare' não existe. Criando..." -ForegroundColor Blue
    & psql -h $DBHost -p $DBPort -U $AdminUser -c "CREATE DATABASE primecare"
    Write-Host "[SUCCESS] Banco 'primecare' criado!" -ForegroundColor Green
}

Create-User -Username "omnicare_app" -Password $credentials.AppPassword -Description "Usuário da aplicação principal - DML access"
Grant-AppPermissions -Username "omnicare_app" -Database "primecare"

Write-Host ""

# ================================================
# 2. PORTAL DO PACIENTE (patientportal)
# ================================================
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  2. Portal do Paciente (patientportal)" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

$dbExists = & psql -h $DBHost -p $DBPort -U $AdminUser -tAc "SELECT 1 FROM pg_database WHERE datname='patientportal'"
if ($dbExists -ne "1") {
    Write-Host "[INFO] Banco 'patientportal' não existe. Criando..." -ForegroundColor Blue
    & psql -h $DBHost -p $DBPort -U $AdminUser -c "CREATE DATABASE patientportal"
    Write-Host "[SUCCESS] Banco 'patientportal' criado!" -ForegroundColor Green
}

Create-User -Username "patientportal_app" -Password $credentials.PortalPassword -Description "Usuário do Portal do Paciente - DML access"
Grant-AppPermissions -Username "patientportal_app" -Database "patientportal"

Write-Host ""

# ================================================
# 3. TELEMEDICINA (telemedicine)
# ================================================
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  3. Telemedicina (telemedicine)" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

$dbExists = & psql -h $DBHost -p $DBPort -U $AdminUser -tAc "SELECT 1 FROM pg_database WHERE datname='telemedicine'"
if ($dbExists -ne "1") {
    Write-Host "[INFO] Banco 'telemedicine' não existe. Criando..." -ForegroundColor Blue
    & psql -h $DBHost -p $DBPort -U $AdminUser -c "CREATE DATABASE telemedicine"
    Write-Host "[SUCCESS] Banco 'telemedicine' criado!" -ForegroundColor Green
}

Create-User -Username "telemedicine_app" -Password $credentials.TelePassword -Description "Usuário da Telemedicina - DML access"
Grant-AppPermissions -Username "telemedicine_app" -Database "telemedicine"

Write-Host ""

# ================================================
# 4. READONLY USER
# ================================================
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  4. Usuário Readonly (Relatórios/BI)" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

Create-User -Username "omnicare_readonly" -Password $credentials.ReadonlyPassword -Description "Usuário somente leitura - Reports e BI"
Grant-ReadonlyPermissions -Username "omnicare_readonly" -Database "primecare"
Grant-ReadonlyPermissions -Username "omnicare_readonly" -Database "patientportal"
Grant-ReadonlyPermissions -Username "omnicare_readonly" -Database "telemedicine"

Write-Host ""

# ================================================
# SALVAR CREDENCIAIS
# ================================================
Save-Credentials -Credentials $credentials

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  ✅ Setup Completo!" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "[SUCCESS] Todos os usuários foram criados com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "[INFO] Próximos passos:" -ForegroundColor Blue
Write-Host "  1. Copie as credenciais do arquivo gerado"
Write-Host "  2. Atualize as connection strings nos appsettings"
Write-Host "  3. DELETE o arquivo de credenciais após usar"
Write-Host "  4. Configure variáveis de ambiente em produção"
Write-Host "  5. Leia: system-admin/seguranca/POSTGRES_APP_USER_GUIDE.md"
Write-Host ""
Write-Host "[WARNING] LEMBRE-SE: Nunca use o usuário 'postgres' nas connection strings!" -ForegroundColor Yellow
Write-Host ""

# Limpar variável de senha
Remove-Variable -Name AdminPassword -ErrorAction SilentlyContinue
$env:PGPASSWORD = $null
