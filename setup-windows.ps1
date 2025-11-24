# Script de Configuração Inicial para Windows - MedicWarehouse
# Execute este script no PowerShell como Administrador

# Requer execução como administrador
#Requires -RunAsAdministrator

$ErrorActionPreference = "Stop"

Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║  🪟 Setup MedicWarehouse - Windows                    ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""

# Função para verificar se um comando existe
function Test-CommandExists {
    param($command)
    try {
        if (Get-Command $command -ErrorAction Stop) {
            return $true
        }
    }
    catch {
        return $false
    }
}

# Verificar se Winget está disponível
Write-Host "[1/7] Verificando gerenciador de pacotes..." -ForegroundColor Blue
if (-not (Test-CommandExists "winget")) {
    Write-Host "⚠️  winget não encontrado. Por favor, atualize o Windows ou instale o App Installer da Microsoft Store." -ForegroundColor Yellow
    Write-Host "    Você precisará instalar as dependências manualmente:" -ForegroundColor Yellow
    Write-Host "    1. .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    Write-Host "    2. Node.js 20+: https://nodejs.org/" -ForegroundColor Yellow
    Write-Host "    3. Git: https://git-scm.com/download/win" -ForegroundColor Yellow
    Write-Host "    4. Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    Write-Host "       ou Podman Desktop: https://podman-desktop.io/" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Pressione Enter para continuar ou Ctrl+C para sair"
} else {
    Write-Host "✓ winget encontrado" -ForegroundColor Green
}
Write-Host ""

# Instalar .NET 8 SDK
Write-Host "[2/7] Verificando .NET 8 SDK..." -ForegroundColor Blue
if (Test-CommandExists "dotnet") {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK já está instalado (versão: $dotnetVersion)" -ForegroundColor Green
} else {
    Write-Host "→ Instalando .NET 8 SDK..." -ForegroundColor Yellow
    if (Test-CommandExists "winget") {
        winget install Microsoft.DotNet.SDK.8 --silent --accept-package-agreements --accept-source-agreements
        Write-Host "✓ .NET 8 SDK instalado" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Por favor, instale manualmente: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    }
}
Write-Host ""

# Instalar Node.js
Write-Host "[3/7] Verificando Node.js..." -ForegroundColor Blue
if (Test-CommandExists "node") {
    $nodeVersion = node --version
    Write-Host "✓ Node.js já está instalado (versão: $nodeVersion)" -ForegroundColor Green
} else {
    Write-Host "→ Instalando Node.js..." -ForegroundColor Yellow
    if (Test-CommandExists "winget") {
        winget install OpenJS.NodeJS.LTS --silent --accept-package-agreements --accept-source-agreements
        Write-Host "✓ Node.js instalado" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Por favor, instale manualmente: https://nodejs.org/" -ForegroundColor Yellow
    }
}
Write-Host ""

# Verificar Docker ou Podman
Write-Host "[4/7] Verificando Docker/Podman..." -ForegroundColor Blue
$hasDocker = Test-CommandExists "docker"
$hasPodman = Test-CommandExists "podman"

if ($hasDocker) {
    Write-Host "✓ Docker já está instalado" -ForegroundColor Green
    try {
        docker --version
    } catch {
        Write-Host "⚠️  Docker instalado mas não está rodando. Inicie o Docker Desktop." -ForegroundColor Yellow
    }
} elseif ($hasPodman) {
    Write-Host "✓ Podman já está instalado" -ForegroundColor Green
    try {
        podman --version
    } catch {
        Write-Host "⚠️  Podman instalado mas pode não estar configurado corretamente." -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠️  Nem Docker nem Podman foram encontrados." -ForegroundColor Yellow
    Write-Host "    Recomendado para desenvolvimento:" -ForegroundColor Yellow
    Write-Host "    • Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    Write-Host "    • Podman Desktop: https://podman-desktop.io/" -ForegroundColor Yellow
    Write-Host "    • Ou use WSL2 com Linux" -ForegroundColor Yellow
}
Write-Host ""

# Instalar Git
Write-Host "[5/7] Verificando Git..." -ForegroundColor Blue
if (Test-CommandExists "git") {
    $gitVersion = git --version
    Write-Host "✓ Git já está instalado ($gitVersion)" -ForegroundColor Green
} else {
    Write-Host "→ Instalando Git..." -ForegroundColor Yellow
    if (Test-CommandExists "winget") {
        winget install Git.Git --silent --accept-package-agreements --accept-source-agreements
        Write-Host "✓ Git instalado" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Por favor, instale manualmente: https://git-scm.com/download/win" -ForegroundColor Yellow
    }
}
Write-Host ""

# Restaurar dependências do .NET
Write-Host "[6/7] Restaurando dependências do .NET..." -ForegroundColor Blue
if (Test-CommandExists "dotnet") {
    try {
        dotnet restore
        Write-Host "✓ Dependências do .NET restauradas" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  Erro ao restaurar dependências do .NET" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠️  .NET SDK não encontrado, pulando restauração" -ForegroundColor Yellow
}
Write-Host ""

# Instalar dependências do frontend
Write-Host "[7/7] Instalando dependências do frontend..." -ForegroundColor Blue

if (Test-Path "frontend/medicwarehouse-app") {
    Write-Host "→ Instalando dependências do medicwarehouse-app..." -ForegroundColor Yellow
    Push-Location frontend/medicwarehouse-app
    try {
        npm install
        Write-Host "✓ Dependências do medicwarehouse-app instaladas" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  Erro ao instalar dependências do medicwarehouse-app" -ForegroundColor Yellow
    }
    Pop-Location
}

if (Test-Path "frontend/mw-system-admin") {
    Write-Host "→ Instalando dependências do mw-system-admin..." -ForegroundColor Yellow
    Push-Location frontend/mw-system-admin
    try {
        npm install
        Write-Host "✓ Dependências do mw-system-admin instaladas" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  Erro ao instalar dependências do mw-system-admin" -ForegroundColor Yellow
    }
    Pop-Location
}
Write-Host ""

# Resumo final
Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  ✅ Configuração Concluída!                            ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Ferramentas instaladas:" -ForegroundColor Blue
if (Test-CommandExists "dotnet") { 
    try { Write-Host "   • .NET SDK: $(dotnet --version)" } 
    catch { Write-Host "   • .NET SDK: Instalado (versão indisponível)" }
}
if (Test-CommandExists "node") { 
    try { Write-Host "   • Node.js: $(node --version)" } 
    catch { Write-Host "   • Node.js: Instalado (versão indisponível)" }
}
if (Test-CommandExists "npm") { 
    try { Write-Host "   • npm: $(npm --version)" } 
    catch { Write-Host "   • npm: Instalado (versão indisponível)" }
}
if (Test-CommandExists "docker") { 
    try { Write-Host "   • Docker: $(docker --version)" } 
    catch { Write-Host "   • Docker: Instalado (versão indisponível)" }
}
if (Test-CommandExists "podman") { 
    try { Write-Host "   • Podman: $(podman --version)" } 
    catch { Write-Host "   • Podman: Instalado (versão indisponível)" }
}
if (Test-CommandExists "git") { 
    try { Write-Host "   • Git: $(git --version)" } 
    catch { Write-Host "   • Git: Instalado (versão indisponível)" }
}
Write-Host ""
Write-Host "📚 Próximos passos:" -ForegroundColor Blue

# Determinar comando de container
$containerCmd = "docker-compose"
if ($hasPodman -and -not $hasDocker) {
    $containerCmd = "podman-compose"
}

if ($hasDocker -or $hasPodman) {
    Write-Host "   1. Configure o banco de dados: " -NoNewline -ForegroundColor White
    Write-Host "$containerCmd up postgres -d" -ForegroundColor Yellow
} else {
    Write-Host "   1. Instale Docker ou Podman e configure o banco de dados" -ForegroundColor Yellow
}
Write-Host "   2. Aplique as migrations: " -NoNewline -ForegroundColor White
Write-Host "cd src\MedicSoft.Api; dotnet ef database update" -ForegroundColor Yellow
Write-Host "   3. Inicie a API: " -NoNewline -ForegroundColor White
Write-Host "cd src\MedicSoft.Api; dotnet run" -ForegroundColor Yellow
Write-Host "   4. Inicie o frontend: " -NoNewline -ForegroundColor White
Write-Host "cd frontend\medicwarehouse-app; npm start" -ForegroundColor Yellow
Write-Host ""
Write-Host "📖 Documentação completa:" -ForegroundColor Blue
Write-Host "   • " -NoNewline -ForegroundColor White
Write-Host "GUIA_INICIO_RAPIDO_LOCAL.md" -ForegroundColor Yellow
Write-Host "   • " -NoNewline -ForegroundColor White
Write-Host "README.md" -ForegroundColor Yellow
Write-Host ""
Write-Host "🎉 Ambiente pronto para desenvolvimento!" -ForegroundColor Green
Write-Host ""
Write-Host "💡 Dica: Reinicie o PowerShell/Terminal para garantir que todas as variáveis de ambiente estejam atualizadas." -ForegroundColor Cyan
Write-Host ""
