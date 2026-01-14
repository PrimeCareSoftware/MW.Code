# 🌱 Script para popular o banco de dados com dados de exemplo
# Este script cria automaticamente todos os dados necessários para começar a testar o sistema

param(
    [string]$ApiUrl = "http://localhost:5000"
)

$ErrorActionPreference = "Stop"

# Configurações
$ApiHealthEndpoint = "$ApiUrl/health"
$ApiSeedEndpoint = "$ApiUrl/api/data-seeder/seed-demo"
$ApiInfoEndpoint = "$ApiUrl/api/data-seeder/demo-info"
$ApiLoginEndpoint = "$ApiUrl/api/auth/login"

# Funções de output com cores
function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Blue
    Write-Host $Message -ForegroundColor Blue
    Write-Host "========================================" -ForegroundColor Blue
    Write-Host ""
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-ErrorMsg {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️  $Message" -ForegroundColor Cyan
}

# Função para verificar se a API está rodando
function Test-ApiHealth {
    Write-Info "Verificando se a API está rodando..."
    
    try {
        $response = Invoke-RestMethod -Uri $ApiHealthEndpoint -Method Get -ErrorAction Stop
        Write-Success "API está rodando em $ApiUrl"
        return $true
    }
    catch {
        Write-ErrorMsg "API não está acessível em $ApiUrl"
        Write-Info "Verifique se a aplicação está rodando:"
        Write-Info "  - dotnet run (no diretório src/MedicSoft.Api)"
        Write-Info "  - ou docker-compose up -d / podman-compose up -d"
        return $false
    }
}

# Função para obter informações sobre os dados que serão criados
function Get-DemoInfo {
    Write-Info "Obtendo informações sobre os dados de exemplo..."
    
    try {
        $response = Invoke-RestMethod -Uri $ApiInfoEndpoint -Method Get
        Write-Success "Informações obtidas com sucesso!"
        Write-Host ""
        
        foreach ($entity in $response.entities) {
            Write-Host $entity
        }
        Write-Host ""
        
        return $response
    }
    catch {
        Write-Warning "Não foi possível obter informações sobre os dados"
        Write-Host $_.Exception.Message
    }
}

# Função para popular os dados
function New-DemoData {
    Write-Info "Populando banco de dados com dados de exemplo..."
    
    try {
        $response = Invoke-RestMethod -Uri $ApiSeedEndpoint -Method Post
        Write-Success "Dados de exemplo criados com sucesso!"
        Write-Host ""
        
        # Exibir credenciais
        Write-Host "📋 TenantID: $($response.tenantId)" -ForegroundColor Green
        Write-Host ""
        Write-Host "🔑 Credenciais de acesso:" -ForegroundColor Green
        Write-Host ""
        Write-Host "👑 Owner (Proprietário):" -ForegroundColor Yellow
        Write-Host "   Username: $($response.credentials.owner.username)"
        Write-Host "   Password: $($response.credentials.owner.password)"
        Write-Host ""
        
        Write-Host "👥 Usuários:" -ForegroundColor Yellow
        foreach ($user in $response.credentials.users) {
            Write-Host "   $($user.role): $($user.username) / $($user.password)"
        }
        Write-Host ""
        
        # Salvar credenciais em arquivo
        $response | ConvertTo-Json -Depth 10 | Out-File -FilePath "$env:TEMP\demo-credentials.json" -Encoding UTF8
        Write-Success "Credenciais salvas em: $env:TEMP\demo-credentials.json"
        
        return $response
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        
        if ($statusCode -eq 400) {
            Write-Warning "Os dados de exemplo já existem no banco de dados"
            
            try {
                $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
                if ($errorBody.error) {
                    Write-Info "Mensagem: $($errorBody.error)"
                }
            }
            catch {}
            
            Write-Info ""
            Write-Info "Opções:"
            Write-Info "  1. Use as credenciais existentes (veja docs/SEED_API_GUIDE.md)"
            Write-Info "  2. Limpe o banco e recrie: Invoke-RestMethod -Uri '$ApiUrl/api/data-seeder/clear-database' -Method Delete"
            Write-Info "  3. Então execute este script novamente"
            
            return $null
        }
        else {
            Write-ErrorMsg "Erro ao criar dados de exemplo (HTTP $statusCode)"
            Write-Host $_.Exception.Message
            return $null
        }
    }
}

# Função para fazer login e obter token
function Get-AuthToken {
    param(
        [string]$Username,
        [string]$Password,
        [string]$TenantId
    )
    
    Write-Info "Fazendo login como $Username..."
    
    $body = @{
        username = $Username
        password = $Password
        tenantId = $TenantId
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri $ApiLoginEndpoint -Method Post -Body $body -ContentType "application/json"
        
        if ($response.token) {
            Write-Success "Login realizado com sucesso!"
            Write-Host ""
            Write-Host "🎟️  Token JWT:" -ForegroundColor Green
            Write-Host $response.token
            Write-Host ""
            
            # Salvar token em arquivo
            $response.token | Out-File -FilePath "$env:TEMP\jwt-token.txt" -Encoding UTF8 -NoNewline
            Write-Success "Token salvo em: $env:TEMP\jwt-token.txt"
            
            Write-Host ""
            Write-Info "Use este token nas próximas requisições:"
            Write-Host ""
            Write-Host "`$token = Get-Content '$env:TEMP\jwt-token.txt'" -ForegroundColor Gray
            Write-Host "`$headers = @{" -ForegroundColor Gray
            Write-Host "    'Authorization' = `"Bearer `$token`"" -ForegroundColor Gray
            Write-Host "    'X-Tenant-ID' = '$TenantId'" -ForegroundColor Gray
            Write-Host "}" -ForegroundColor Gray
            Write-Host "Invoke-RestMethod -Uri '$ApiUrl/api/patients' -Headers `$headers" -ForegroundColor Gray
            
            return $response.token
        }
        else {
            Write-ErrorMsg "Token não foi retornado na resposta"
            return $null
        }
    }
    catch {
        Write-ErrorMsg "Erro ao fazer login"
        Write-Host $_.Exception.Message
        return $null
    }
}

# Função principal
function Main {
    Write-Header "🌱 PrimeCare - Seed Demo Data"
    
    Write-Host "Este script irá:" -ForegroundColor Blue
    Write-Host "  1. Verificar se a API está rodando"
    Write-Host "  2. Obter informações sobre os dados de exemplo"
    Write-Host "  3. Popular o banco de dados com dados completos"
    Write-Host "  4. Fazer login e obter um token JWT"
    Write-Host ""
    
    # 1. Verificar se a API está rodando
    if (-not (Test-ApiHealth)) {
        exit 1
    }
    
    Write-Host ""
    
    # 2. Obter informações sobre os dados
    $info = Get-DemoInfo
    
    # Perguntar se o usuário deseja continuar
    Write-Host ""
    $response = Read-Host "Deseja popular o banco de dados com estes dados? (s/N)"
    
    if ($response -notmatch '^[SsYy]$') {
        Write-Info "Operação cancelada pelo usuário"
        exit 0
    }
    
    Write-Host ""
    
    # 3. Popular dados
    $seedResult = New-DemoData
    
    if ($null -eq $seedResult) {
        # Dados já existem, perguntar se quer fazer login
        Write-Host ""
        $response = Read-Host "Deseja fazer login com as credenciais existentes? (s/N)"
        
        if ($response -match '^[SsYy]$') {
            Write-Host ""
            $token = Get-AuthToken -Username "dr.silva" -Password "Doctor@123" -TenantId "demo-clinic-001"
        }
        exit 0
    }
    
    Write-Host ""
    
    # 4. Fazer login
    Write-Header "🔐 Autenticação"
    
    $response = Read-Host "Deseja fazer login automaticamente? (S/n)"
    
    if ($response -notmatch '^[Nn]$') {
        Write-Host ""
        $token = Get-AuthToken -Username "dr.silva" -Password "Doctor@123" -TenantId "demo-clinic-001"
    }
    
    Write-Host ""
    Write-Header "🎉 Pronto!"
    
    Write-Success "Sistema pronto para testes!"
    Write-Host ""
    Write-Info "Próximos passos:"
    Write-Info "  📖 Ver documentação: docs\SEED_API_GUIDE.md"
    Write-Info "  📮 Usar Postman: Importe PrimeCare-Postman-Collection.json"
    Write-Info "  🌐 Acessar frontend: http://localhost:4200"
    Write-Info "  📱 Testar APIs: Use o token salvo em $env:TEMP\jwt-token.txt"
    Write-Host ""
}

# Executar script
Main
