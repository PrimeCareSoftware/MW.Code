# 🌍 Guia de Desenvolvimento Multiplataforma - PrimeCare Software

## 📋 Visão Geral

Este guia explica como desenvolver o PrimeCare Software em diferentes sistemas operacionais (macOS, Windows e Linux), mantendo compatibilidade total entre plataformas.

## ✅ Compatibilidade Garantida

O PrimeCare Software foi projetado para ser **totalmente cross-platform**:

- ✅ **Backend (.NET 8)**: Funciona nativamente em macOS, Windows e Linux
- ✅ **Frontend (Angular/Node.js)**: Compatível com todos os sistemas operacionais
- ✅ **Banco de Dados (PostgreSQL)**: Disponível via Docker/Podman em todas as plataformas
- ✅ **Scripts**: Versões para Shell (.sh) e Windows (.bat/.ps1) fornecidas
- ✅ **Line Endings**: Configuração `.editorconfig` garante consistência

## 🍎 Configuração no macOS

### Pré-requisitos

- **macOS**: 10.15 (Catalina) ou superior
- **Homebrew**: Gerenciador de pacotes para macOS

### Setup Automatizado

Execute o script de configuração automatizada:

```bash
# Clonar o repositório
git clone https://github.com/PrimeCare Software/MW.Code.git
cd MW.Code

# Executar script de setup
./setup-macos.sh
```

O script instalará automaticamente:
- ✅ Homebrew (se não instalado)
- ✅ .NET 8 SDK
- ✅ Node.js 20+
- ✅ Podman e Podman Compose
- ✅ Git
- ✅ Todas as dependências do projeto

### Setup Manual

Se preferir instalar manualmente:

```bash
# Instalar Homebrew
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Instalar .NET 8 SDK
brew install --cask dotnet-sdk

# Instalar Node.js
brew install node@20

# Instalar Podman
brew install podman podman-compose
podman machine init
podman machine start

# Instalar Git (se necessário)
brew install git

# Restaurar dependências do projeto
cd MW.Code
dotnet restore
cd frontend/medicwarehouse-app && npm install && cd ../..
cd frontend/mw-system-admin && npm install && cd ../..
```

### Executar o Sistema

```bash
# 1. Iniciar PostgreSQL
podman-compose up postgres -d

# 2. Aplicar migrations (em um novo terminal)
cd src/MedicSoft.Api
dotnet ef database update --context MedicSoftDbContext --project ../MedicSoft.Repository
cd ../..

# 3. Iniciar API
cd src/MedicSoft.Api
dotnet run

# 4. Iniciar Frontend (em outro terminal)
cd frontend/medicwarehouse-app
npm start
```

### Dicas para macOS

- **Apple Silicon (M1/M2/M3)**: Todos os componentes são nativos para ARM64
- **Rosetta 2**: Não é necessário para o PrimeCare Software
- **Permissões**: Use `sudo` apenas quando solicitado pelo Homebrew
- **Terminal**: Terminal.app, iTerm2 ou outro de sua preferência funcionam igualmente

## 🪟 Configuração no Windows

### Pré-requisitos

- **Windows**: 10 (versão 1809+) ou Windows 11
- **PowerShell**: 5.1 ou superior (já incluso no Windows)

### Setup Automatizado

Execute o script de configuração automatizada no PowerShell como **Administrador**:

```powershell
# Clonar o repositório
git clone https://github.com/PrimeCare Software/MW.Code.git
cd MW.Code

# Executar script de setup (PowerShell como Administrador)
.\setup-windows.ps1
```

O script instalará automaticamente (via winget):
- ✅ .NET 8 SDK
- ✅ Node.js 20+
- ✅ Git
- ✅ Informará sobre Docker/Podman

### Setup Manual

Se preferir instalar manualmente:

```powershell
# Instalar winget (se não disponível)
# Baixe e instale "App Installer" da Microsoft Store

# Instalar .NET 8 SDK
winget install Microsoft.DotNet.SDK.8

# Instalar Node.js
winget install OpenJS.NodeJS.LTS

# Instalar Git
winget install Git.Git

# Instalar Docker Desktop (escolha um)
winget install Docker.DockerDesktop
# OU Podman Desktop
# Baixe de: https://podman-desktop.io/

# Restaurar dependências do projeto
cd MW.Code
dotnet restore
cd frontend\medicwarehouse-app
npm install
cd ..\..
cd frontend\mw-system-admin
npm install
cd ..\..
```

### Executar o Sistema

**Usando Docker:**
```powershell
# 1. Iniciar PostgreSQL
docker-compose up postgres -d

# 2. Aplicar migrations (em um novo terminal)
cd src\MedicSoft.Api
dotnet ef database update --context MedicSoftDbContext --project ..\MedicSoft.Repository
cd ..\..

# 3. Iniciar API
cd src\MedicSoft.Api
dotnet run

# 4. Iniciar Frontend (em outro terminal)
cd frontend\medicwarehouse-app
npm start
```

**Usando Podman:**
```powershell
# Mesmos comandos, substitua 'docker-compose' por 'podman-compose'
podman-compose up postgres -d
```

### Dicas para Windows

- **PowerShell vs CMD**: Use PowerShell para melhor experiência
- **Windows Terminal**: Recomendado (disponível na Microsoft Store)
- **WSL2**: Opcional, mas permite usar comandos Linux nativamente
- **Docker Desktop**: Requer licença para uso corporativo
- **Podman Desktop**: Alternativa 100% gratuita ao Docker
- **Paths**: Use `\` em vez de `/` nos caminhos do Windows

## 🐧 Configuração no Linux

### Ubuntu/Debian

```bash
# Atualizar sistema
sudo apt update && sudo apt upgrade -y

# Instalar .NET 8 SDK
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-8.0

# Instalar Node.js 20
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt install -y nodejs

# Instalar Podman
sudo apt install -y podman podman-compose

# Instalar Git
sudo apt install -y git

# Clonar e configurar projeto
git clone https://github.com/PrimeCare Software/MW.Code.git
cd MW.Code
dotnet restore
cd frontend/medicwarehouse-app && npm install && cd ../..
cd frontend/mw-system-admin && npm install && cd ../..
```

### Fedora/RHEL/CentOS

```bash
# Instalar .NET 8 SDK
sudo dnf install dotnet-sdk-8.0

# Instalar Node.js 20
sudo dnf install nodejs

# Instalar Podman
sudo dnf install podman podman-compose

# Instalar Git
sudo dnf install git

# Clonar e configurar projeto
git clone https://github.com/PrimeCare Software/MW.Code.git
cd MW.Code
dotnet restore
cd frontend/medicwarehouse-app && npm install && cd ../..
cd frontend/mw-system-admin && npm install && cd ../..
```

## 🔧 Configurações Cross-Platform

### 1. Line Endings (Fins de Linha)

O projeto inclui `.editorconfig` para garantir line endings consistentes:

- **Unix/macOS/Linux**: LF (`\n`)
- **Windows**: Scripts `.bat` e `.ps1` usam CRLF (`\r\n`)
- **Git**: Configure para conversão automática

```bash
# Configurar Git para lidar com line endings automaticamente
git config --global core.autocrlf input    # macOS/Linux
git config --global core.autocrlf true     # Windows
```

### 2. Caminhos de Arquivos

O código usa `Path.Combine()` para garantir compatibilidade:

```csharp
// ✅ CORRETO - Cross-platform
var filePath = Path.Combine("folder", "file.txt");

// ❌ ERRADO - Específico do Windows
var windowsPath = "folder\\file.txt";

// ❌ ERRADO - Específico do Unix
var unixPath = "folder/file.txt";
```

### 3. Scripts Duplicados

O projeto fornece versões de scripts para cada plataforma:

| Funcionalidade | macOS/Linux | Windows |
|---------------|-------------|---------|
| Setup inicial | `setup-macos.sh` | `setup-windows.ps1` |
| Teste de API | `TESTE_API_RAPIDO.sh` | - |
| Gerar docs | `gerar.sh` | `gerar.bat` |

### 4. Variáveis de Ambiente

Configure variáveis de ambiente de forma consistente:

**macOS/Linux (.bashrc, .zshrc):**
```bash
export ASPNETCORE_ENVIRONMENT=Development
export DATABASE_URL="Host=localhost;Port=5432;Database=medicwarehouse;..."
```

**Windows (PowerShell):**
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DATABASE_URL = "Host=localhost;Port=5432;Database=medicwarehouse;..."
```

### 5. Docker vs Podman

Ambos funcionam em todas as plataformas:

| Comando Docker | Comando Podman |
|---------------|----------------|
| `docker-compose up` | `podman-compose up` |
| `docker ps` | `podman ps` |
| `docker images` | `podman images` |

**Dica**: No macOS/Linux, você pode criar um alias:
```bash
# Adicionar ao ~/.bashrc ou ~/.zshrc
alias docker=podman
alias docker-compose=podman-compose
```

## 🛠️ IDEs Recomendadas

### Visual Studio Code (Recomendado para todas as plataformas)

**Vantagens:**
- ✅ Idêntico em macOS, Windows e Linux
- ✅ Excelente suporte para C# e TypeScript
- ✅ Integração nativa com Git
- ✅ Terminal integrado

**Extensões essenciais:**
- C# (Microsoft)
- Angular Language Service
- EditorConfig for VS Code
- GitLens
- Thunder Client (para testar APIs)

**Download:** https://code.visualstudio.com/

### Outras Opções

| IDE | macOS | Windows | Linux | Melhor Para |
|-----|-------|---------|-------|-------------|
| Visual Studio 2022 | ✅ | ✅ | ❌ | C# development |
| JetBrains Rider | ✅ | ✅ | ✅ | C# development profissional |
| WebStorm | ✅ | ✅ | ✅ | Angular/Frontend |
| Vim/Neovim | ✅ | ✅ | ✅ | Terminal-based |

## 🧪 Testes Cross-Platform

### Executar testes do Backend

```bash
# Funciona em todas as plataformas
dotnet test
```

### Executar testes do Frontend

```bash
# Funciona em todas as plataformas
cd frontend/medicwarehouse-app
npm test
```

### CI/CD

O projeto usa GitHub Actions que testa automaticamente em **ubuntu-latest**, garantindo compatibilidade Linux.

## 🐛 Troubleshooting

### Problema: "Line endings diferentes após git clone"

**Solução:**
```bash
# Reconfigurar line endings
git config core.autocrlf input  # macOS/Linux
git config core.autocrlf true   # Windows

# Re-checkout dos arquivos
git rm --cached -r .
git reset --hard
```

### Problema: "Permissão negada ao executar scripts no macOS/Linux"

**Solução:**
```bash
chmod +x setup-macos.sh
chmod +x TESTE_API_RAPIDO.sh
chmod +x documentacao-portatil/gerar.sh
```

### Problema: "Cannot execute scripts no Windows (PowerShell)"

**Solução:**
```powershell
# Executar PowerShell como Administrador
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Problema: "Podman machine não inicia no macOS"

**Solução:**
```bash
# Resetar máquina do Podman
podman machine stop
podman machine rm
podman machine init
podman machine start
```

### Problema: "Docker Desktop não inicia no Windows"

**Solução:**
1. Verificar se WSL2 está instalado: `wsl --status`
2. Instalar WSL2 se necessário: `wsl --install`
3. Reiniciar o Windows
4. Iniciar Docker Desktop

### Problema: "Porta 5432 já está em uso"

**Solução:**
```bash
# Verificar o que está usando a porta
# macOS/Linux:
lsof -i :5432

# Windows (PowerShell):
netstat -ano | findstr :5432

# Parar PostgreSQL local
# macOS/Linux:
sudo systemctl stop postgresql

# Windows:
# Parar serviço via "Serviços" ou:
Stop-Service postgresql-x64-14  # Ajustar versão
```

### Problema: "npm install falha com EACCES (Linux/macOS)"

**Solução:**
```bash
# NÃO use sudo com npm!
# Configure npm para usar diretório local
mkdir ~/.npm-global
npm config set prefix '~/.npm-global'
echo 'export PATH=~/.npm-global/bin:$PATH' >> ~/.profile
source ~/.profile
```

## 📊 Comparação de Comandos

### Comandos de Terminal

| Função | macOS/Linux | Windows (PowerShell) | Windows (CMD) |
|--------|-------------|----------------------|---------------|
| Listar arquivos | `ls -la` | `ls` ou `dir` | `dir` |
| Mudar diretório | `cd path/to/dir` | `cd path\to\dir` | `cd path\to\dir` |
| Limpar tela | `clear` | `Clear-Host` ou `cls` | `cls` |
| Variável env | `export VAR=value` | `$env:VAR = "value"` | `set VAR=value` |
| Ver variável | `echo $VAR` | `echo $env:VAR` | `echo %VAR%` |
| Executar script | `./script.sh` | `.\script.ps1` | `script.bat` |
| Encontrar processo | `ps aux \| grep name` | `Get-Process name` | `tasklist \| findstr name` |
| Matar processo | `kill -9 PID` | `Stop-Process -Id PID` | `taskkill /PID pid /F` |

## 🎯 Checklist de Desenvolvimento Cross-Platform

Antes de fazer commit de código, verifique:

- [ ] Usar `Path.Combine()` para caminhos de arquivos
- [ ] Não hardcodar separadores de caminho (`/` ou `\`)
- [ ] Scripts .sh devem ter permissão de execução
- [ ] Fornecer versões .bat/.ps1 para scripts .sh
- [ ] Testar em pelo menos 2 sistemas operacionais diferentes
- [ ] Verificar line endings com `.editorconfig`
- [ ] Usar variáveis de ambiente em vez de paths absolutos
- [ ] Documentar requisitos específicos de plataforma

## 📚 Recursos Adicionais

- **[GUIA_INICIO_RAPIDO_LOCAL.md](GUIA_INICIO_RAPIDO_LOCAL.md)**: Setup rápido do sistema
- **[DOCKER_TO_PODMAN_MIGRATION.md](DOCKER_TO_PODMAN_MIGRATION.md)**: Migração Docker → Podman
- **[README.md](../README.md)**: Documentação completa do projeto
- **.NET Cross-Platform**: https://docs.microsoft.com/dotnet/core/compatibility/
- **Node.js Multi-platform**: https://nodejs.org/en/docs/guides/

## 💡 Dicas Finais

1. **Use VS Code**: Experiência idêntica em todas as plataformas
2. **Configure Git corretamente**: Line endings são importantes
3. **Prefira Podman**: 100% gratuito em todas as plataformas
4. **Teste localmente**: Antes de fazer push, teste em sua plataforma
5. **Documente diferenças**: Se encontrar algo específico de plataforma, documente
6. **Use scripts de setup**: `setup-macos.sh` e `setup-windows.ps1`
7. **Mantenha PATH consistente**: Evite paths absolutos no código

## 🤝 Contribuindo

Ao contribuir com o projeto:

1. Teste suas mudanças em pelo menos 2 plataformas
2. Atualize este guia se adicionar dependências específicas de plataforma
3. Mantenha compatibilidade com macOS, Windows e Linux
4. Use ferramentas cross-platform sempre que possível

---

**Última Atualização:** Novembro 2024  
**Versão:** 1.0  
**Plataformas Testadas:** macOS 13+, Windows 10/11, Ubuntu 22.04+
