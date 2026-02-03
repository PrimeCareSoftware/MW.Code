# 01 - Configuração do Ambiente de Desenvolvimento

> **Objetivo:** Instalar todas as ferramentas necessárias para executar o Omni Care Software  
> **Tempo estimado:** 15-20 minutos  
> **Pré-requisitos:** Nenhum

## 📋 Índice

1. [Requisitos Mínimos](#requisitos-mínimos)
2. [Instalação por Sistema Operacional](#instalação-por-sistema-operacional)
3. [Verificação da Instalação](#verificação-da-instalação)
4. [Próximos Passos](#próximos-passos)

## 💻 Requisitos Mínimos

### Hardware
- **CPU:** 4 cores (recomendado: 8 cores)
- **RAM:** 8 GB (recomendado: 16 GB)
- **Disco:** 20 GB livres (SSD recomendado)
- **Internet:** Conexão estável para download de dependências

### Software
- **Sistema Operacional:** Windows 10+, macOS 10.15+, ou Linux (Ubuntu 20.04+)
- **Navegador:** Chrome 90+, Firefox 88+, Safari 14+, ou Edge 90+

## 🔧 Instalação por Sistema Operacional

### Windows 10/11

#### Opção 1: Setup Automatizado (Recomendado)

```powershell
# Execute no PowerShell como Administrador
cd /caminho/para/MW.Code
.\setup-windows.ps1
```

Este script irá instalar automaticamente:
- .NET 8 SDK
- Node.js 20 LTS
- PostgreSQL 16
- Git
- Visual Studio Code

#### Opção 2: Instalação Manual

1. **Instalar .NET 8 SDK**
   ```powershell
   # Baixe e instale de: https://dotnet.microsoft.com/download/dotnet/8.0
   # Ou use winget:
   winget install Microsoft.DotNet.SDK.8
   ```

2. **Instalar Node.js 20 LTS**
   ```powershell
   # Baixe e instale de: https://nodejs.org/
   # Ou use winget:
   winget install OpenJS.NodeJS.LTS
   ```

3. **Instalar PostgreSQL 16**
   ```powershell
   # Baixe e instale de: https://www.postgresql.org/download/windows/
   # Ou use winget:
   winget install PostgreSQL.PostgreSQL
   ```

4. **Instalar Git**
   ```powershell
   winget install Git.Git
   ```

5. **Instalar Visual Studio Code (opcional)**
   ```powershell
   winget install Microsoft.VisualStudioCode
   ```

6. **Instalar Angular CLI**
   ```powershell
   npm install -g @angular/cli@20
   ```

### macOS

#### Opção 1: Setup Automatizado (Recomendado)

```bash
# Execute no Terminal
cd /caminho/para/MW.Code
chmod +x setup-macos.sh
./setup-macos.sh
```

#### Opção 2: Instalação Manual

1. **Instalar Homebrew** (se ainda não tiver)
   ```bash
   /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
   ```

2. **Instalar .NET 8 SDK**
   ```bash
   brew install --cask dotnet-sdk
   ```

3. **Instalar Node.js 20 LTS**
   ```bash
   brew install node@20
   brew link node@20
   ```

4. **Instalar PostgreSQL 16**
   ```bash
   brew install postgresql@16
   brew services start postgresql@16
   ```

5. **Instalar Git** (geralmente já vem instalado)
   ```bash
   brew install git
   ```

6. **Instalar Visual Studio Code (opcional)**
   ```bash
   brew install --cask visual-studio-code
   ```

7. **Instalar Angular CLI**
   ```bash
   npm install -g @angular/cli@20
   ```

### Linux (Ubuntu/Debian)

1. **Atualizar sistema**
   ```bash
   sudo apt update && sudo apt upgrade -y
   ```

2. **Instalar .NET 8 SDK**
   ```bash
   wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   sudo apt update
   sudo apt install -y dotnet-sdk-8.0
   ```

3. **Instalar Node.js 20 LTS**
   ```bash
   curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
   sudo apt install -y nodejs
   ```

4. **Instalar PostgreSQL 16**
   ```bash
   sudo apt install -y postgresql-16 postgresql-contrib-16
   sudo systemctl start postgresql
   sudo systemctl enable postgresql
   ```

5. **Instalar Git**
   ```bash
   sudo apt install -y git
   ```

6. **Instalar Visual Studio Code (opcional)**
   ```bash
   sudo snap install code --classic
   ```

7. **Instalar Angular CLI**
   ```bash
   sudo npm install -g @angular/cli@20
   ```

### Linux (Fedora/Red Hat)

1. **Atualizar sistema**
   ```bash
   sudo dnf update -y
   ```

2. **Instalar .NET 8 SDK**
   ```bash
   sudo dnf install dotnet-sdk-8.0 -y
   ```

3. **Instalar Node.js 20 LTS**
   ```bash
   sudo dnf module install nodejs:20 -y
   ```

4. **Instalar PostgreSQL 16**
   ```bash
   sudo dnf install postgresql16-server postgresql16-contrib -y
   sudo postgresql-setup --initdb
   sudo systemctl start postgresql
   sudo systemctl enable postgresql
   ```

5. **Instalar Git**
   ```bash
   sudo dnf install git -y
   ```

6. **Instalar Angular CLI**
   ```bash
   sudo npm install -g @angular/cli@20
   ```

## ✅ Verificação da Instalação

Execute os seguintes comandos para verificar se tudo foi instalado corretamente:

```bash
# Verificar versão do .NET
dotnet --version
# Esperado: 8.0.x

# Verificar versão do Node.js
node --version
# Esperado: v20.x.x

# Verificar versão do npm
npm --version
# Esperado: 10.x.x

# Verificar versão do Angular CLI
ng version
# Esperado: Angular CLI: 20.x.x

# Verificar versão do PostgreSQL
psql --version
# Esperado: psql (PostgreSQL) 16.x

# Verificar versão do Git
git --version
# Esperado: git version 2.x.x
```

### Checklist de Verificação

- [ ] .NET 8 SDK instalado e funcionando
- [ ] Node.js 20 LTS instalado e funcionando
- [ ] npm instalado e funcionando
- [ ] Angular CLI 20 instalado globalmente
- [ ] PostgreSQL 16 instalado e serviço rodando
- [ ] Git instalado e configurado
- [ ] Editor de código instalado (VS Code recomendado)

## 🔧 Configurações Adicionais

### Configurar Git (se ainda não configurado)

```bash
git config --global user.name "Seu Nome"
git config --global user.email "seu.email@example.com"
```

### Extensões Recomendadas para VS Code

Se você usa Visual Studio Code, instale as seguintes extensões:

- **C# Dev Kit** - Para desenvolvimento .NET
- **Angular Language Service** - Para desenvolvimento Angular
- **ESLint** - Para linting de código JavaScript/TypeScript
- **PostgreSQL** - Para gerenciar banco de dados
- **GitLens** - Para melhor integração com Git
- **Docker** - Se for usar containers

Para instalar via linha de comando:

```bash
code --install-extension ms-dotnettools.csdevkit
code --install-extension Angular.ng-template
code --install-extension dbaeumer.vscode-eslint
code --install-extension ckolkman.vscode-postgres
code --install-extension eamodio.gitlens
code --install-extension ms-azuretools.vscode-docker
```

## 🚨 Problemas Comuns

### Problema: "dotnet: command not found"

**Solução:** Adicione o .NET ao PATH ou reinicie o terminal após a instalação.

### Problema: "psql: command not found"

**Solução:** Adicione o PostgreSQL ao PATH:
- **Windows:** Adicione `C:\Program Files\PostgreSQL\16\bin` ao PATH
- **macOS:** `export PATH="/opt/homebrew/opt/postgresql@16/bin:$PATH"`
- **Linux:** Geralmente já está no PATH após instalação

### Problema: Erro de permissão ao instalar pacotes npm globalmente

**Solução:** Configure o diretório npm para o usuário:

```bash
mkdir ~/.npm-global
npm config set prefix '~/.npm-global'
echo 'export PATH=~/.npm-global/bin:$PATH' >> ~/.bashrc
source ~/.bashrc
```

### Problema: PostgreSQL não inicia automaticamente

**Windows:**
```powershell
# Inicie o serviço PostgreSQL
net start postgresql-x64-16
```

**macOS:**
```bash
brew services start postgresql@16
```

**Linux:**
```bash
sudo systemctl start postgresql
sudo systemctl enable postgresql  # Para iniciar automaticamente
```

## 📚 Documentação Adicional

- [Guia Multiplataforma Completo](../../system-admin/guias/GUIA_MULTIPLATAFORMA.md)
- [Common Issues](../../docs/COMMON_ISSUES.md)
- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/)
- [Angular Documentation](https://angular.io/docs)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/16/)

## ⏭️ Próximos Passos

Após completar a instalação de todas as ferramentas:

1. ✅ Verifique que todos os comandos funcionam
2. ➡️ Vá para [02-Configuracao-Backend.md](02-Configuracao-Backend.md) para configurar a API .NET
3. Depois siga para [03-Configuracao-Frontend.md](03-Configuracao-Frontend.md)
4. E então [04-Configuracao-Banco-Dados.md](04-Configuracao-Banco-Dados.md)

---

**Dúvidas?** Consulte a [documentação principal](../../README.md) ou abra uma issue no GitHub.
