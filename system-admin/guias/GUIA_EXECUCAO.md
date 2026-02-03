# 📋 Guia de Execução Passo a Passo - Omni Care Software

Este guia fornece instruções detalhadas para executar o sistema Omni Care Software em seu PC, começando pela criação do banco de dados SQL Server via Docker.

## 📑 Índice

1. [Pré-requisitos](#pré-requisitos)
2. [Passo 1: Instalar Docker](#passo-1-instalar-docker)
3. [Passo 2: Clonar o Repositório](#passo-2-clonar-o-repositório)
4. [Passo 3: Criar o Banco de Dados SQL Server via Docker](#passo-3-criar-o-banco-de-dados-sql-server-via-docker)
5. [Passo 4: Configurar a Aplicação](#passo-4-configurar-a-aplicação)
6. [Passo 5: Executar com Docker Compose](#passo-5-executar-com-docker-compose)
7. [Passo 6: Executar em Modo Desenvolvimento](#passo-6-executar-em-modo-desenvolvimento)
8. [Passo 7: Acessar a Aplicação](#passo-7-acessar-a-aplicação)
9. [Solução de Problemas](#solução-de-problemas)

---

## Pré-requisitos

### Software Necessário

- **Windows 10/11** (ou Linux/macOS)
- **Docker Desktop** 4.0 ou superior
- **.NET 8 SDK** (apenas para desenvolvimento local)
- **Node.js 18+** e **npm** (apenas para desenvolvimento do frontend)
- **Git** para clonar o repositório

### Verificar Instalações

```bash
# Verificar Docker
docker --version
docker-compose --version

# Verificar .NET (opcional, para desenvolvimento)
dotnet --version

# Verificar Node.js (opcional, para desenvolvimento)
node --version
npm --version
```

---

## Passo 1: Instalar Docker

### Windows

1. Baixe o **Docker Desktop** em: https://www.docker.com/products/docker-desktop
2. Execute o instalador e siga as instruções
3. Após a instalação, reinicie o computador se solicitado
4. Abra o Docker Desktop para iniciar o serviço
5. Verifique se o Docker está rodando: abra o PowerShell ou CMD e execute:
   ```bash
   docker --version
   ```

### Linux (Ubuntu/Debian)

```bash
# Atualizar repositórios
sudo apt update

# Instalar dependências
sudo apt install apt-transport-https ca-certificates curl software-properties-common

# Adicionar chave GPG do Docker
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -

# Adicionar repositório do Docker
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"

# Instalar Docker
sudo apt update
sudo apt install docker-ce docker-ce-cli containerd.io docker-compose-plugin

# Adicionar usuário ao grupo docker
sudo usermod -aG docker ${USER}

# Recarregar grupos (ou fazer logout/login)
newgrp docker

# Verificar instalação
docker --version
docker compose version
```

### macOS

1. Baixe o **Docker Desktop** em: https://www.docker.com/products/docker-desktop
2. Arraste o Docker.app para a pasta Applications
3. Abra o Docker Desktop
4. Verifique a instalação no Terminal:
   ```bash
   docker --version
   ```

---

## Passo 2: Clonar o Repositório

Abra o terminal (PowerShell, CMD, ou Terminal Linux/macOS) e execute:

```bash
# Clone o repositório
git clone https://github.com/Omni Care Software/MW.Code.git

# Entre no diretório do projeto
cd MW.Code

# Verifique os arquivos
ls
```

Você deve ver a seguinte estrutura:
```
MW.Code/
├── docker-compose.yml
├── README.md
├── GUIA_EXECUCAO.md
├── src/
├── frontend/
├── scripts/
└── tests/
```

---

## Passo 3: Criar o Banco de Dados SQL Server via Docker

### Opção 1: SQL Server Container Individual

Se você deseja apenas criar o banco de dados SQL Server primeiro:

```bash
# Criar e executar container SQL Server
docker run -d \
  --name medicwarehouse-sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=MedicW@rehouse2024!" \
  -e "MSSQL_PID=Developer" \
  -p 1433:1433 \
  -v sqlserver_data:/var/opt/mssql \
  mcr.microsoft.com/mssql/server:2022-latest

# Verificar se o container está rodando
docker ps

# Ver logs do SQL Server
docker logs medicwarehouse-sqlserver
```

**Importante**: A senha deve ter pelo menos 8 caracteres, incluindo letras maiúsculas, minúsculas, números e símbolos.

### Criar o Banco de Dados

Aguarde alguns segundos para o SQL Server inicializar completamente, depois execute:

```bash
# Conectar ao SQL Server e criar o banco de dados
docker exec -it medicwarehouse-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "MedicW@rehouse2024!" \
  -Q "CREATE DATABASE Omni Care Software; SELECT name FROM sys.databases;"
```

Você deve ver a lista de bancos de dados, incluindo `Omni Care Software`.

### Opção 2: Executar Tudo com Docker Compose (Recomendado)

Pule para o [Passo 5](#passo-5-executar-com-docker-compose) se preferir executar tudo de uma vez (recomendado).

---

## Passo 4: Configurar a Aplicação

### 4.1. Verificar Configurações

O arquivo de configuração principal está em `src/MedicSoft.Api/appsettings.json`. As configurações padrão já estão corretas para o Docker Compose:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=Omni Care Software;User Id=sa;Password=MedicW@rehouse2024!;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "Omni Care Software-SuperSecretKey-2024-Development",
    "ExpiryMinutes": 60,
    "Issuer": "Omni Care Software",
    "Audience": "Omni Care Software-API"
  }
}
```

### 4.2. Configuração para Desenvolvimento Local

Se você for executar sem Docker, edite o arquivo para usar `localhost`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=Omni Care Software;User Id=sa;Password=MedicW@rehouse2024!;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

---

## Passo 5: Executar com Docker Compose

Esta é a forma **mais simples e recomendada** de executar todo o sistema.

### 5.1. Build e Execução

```bash
# Certifique-se de estar no diretório raiz do projeto
cd MW.Code

# Build e iniciar todos os containers
docker-compose up -d --build

# Ver logs de todos os serviços
docker-compose logs -f

# Ou ver logs de um serviço específico
docker-compose logs -f api
docker-compose logs -f sqlserver
docker-compose logs -f frontend
```

### 5.2. Verificar Status dos Containers

```bash
# Listar containers em execução
docker-compose ps

# Deve mostrar 3 containers rodando:
# - medicwarehouse-sqlserver (porta 1433)
# - medicwarehouse-api (porta 5000)
# - medicwarehouse-frontend (porta 4200)
```

### 5.3. Aguardar Inicialização

- **SQL Server**: ~10-30 segundos para estar pronto
- **API .NET**: ~10-20 segundos após o SQL Server
- **Frontend Angular**: ~30-60 segundos para build e inicialização

Aguarde até ver mensagens como:
```
medicwarehouse-api       | info: Microsoft.Hosting.Lifetime[14]
medicwarehouse-api       |       Now listening on: http://[::]:8080
medicwarehouse-frontend  | Compiled successfully
```

---

## Passo 6: Executar em Modo Desenvolvimento

Se você deseja desenvolver e testar localmente sem Docker (requer .NET 8 SDK e Node.js):

### 6.1. Executar Apenas o Banco de Dados via Docker

```bash
# Criar o SQL Server
docker run -d \
  --name medicwarehouse-sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=MedicW@rehouse2024!" \
  -e "MSSQL_PID=Developer" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest

# Criar o banco de dados
docker exec -it medicwarehouse-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "MedicW@rehouse2024!" \
  -Q "CREATE DATABASE Omni Care Software;"
```

### 6.2. Executar a API .NET

Abra um novo terminal:

```bash
# Navegar para o projeto da API
cd MW.Code/src/MedicSoft.Api

# Restaurar dependências
dotnet restore

# Executar a API
dotnet run

# A API estará disponível em:
# - https://localhost:7100 (HTTPS)
# - http://localhost:5000 (HTTP)
```

### 6.3. Executar o Frontend Angular

Abra outro terminal:

```bash
# Navegar para o frontend
cd MW.Code/frontend/medicwarehouse-app

# Instalar dependências (primeira vez apenas)
npm install

# Executar em modo de desenvolvimento
npm start
# ou
ng serve

# O frontend estará disponível em:
# - http://localhost:4200
```

---

## Passo 7: Acessar a Aplicação

### 7.1. Swagger API (Documentação Interativa)

- **URL**: http://localhost:5000
- Aqui você pode testar todos os endpoints da API
- Para endpoints protegidos, você precisa fazer login primeiro

### 7.2. Frontend Angular

- **URL**: http://localhost:4200
- Interface web completa do sistema

### 7.3. Banco de Dados SQL Server

Você pode conectar ao SQL Server usando ferramentas como:

- **SQL Server Management Studio (SSMS)**
- **Azure Data Studio**
- **DBeaver**
- **VS Code com extensão SQL Server**

**Credenciais de Conexão:**
- **Server**: `localhost,1433`
- **Database**: `Omni Care Software`
- **User**: `sa`
- **Password**: `MedicW@rehouse2024!`
- **Authentication**: SQL Server Authentication

### 7.4. Testar a API

#### 1. Obter Token de Autenticação

Abra o Swagger (http://localhost:5000/swagger para Docker ou https://localhost:7107/swagger para desenvolvimento local) e execute:

```
POST /api/auth/login
```

Body:
```json
{
  "username": "admin",
  "password": "admin123",
  "tenantId": "default-tenant"
}
```

Você receberá um token JWT. Copie o token.

#### 2. Autorizar no Swagger

1. Clique no botão **"Authorize"** no topo da página Swagger
2. Cole o token no campo `Value` no formato: `Bearer {seu-token-aqui}`
3. Clique em **"Authorize"** e depois **"Close"**

#### 3. Testar Endpoints

Agora você pode testar qualquer endpoint protegido:

- Listar pacientes: `GET /api/patients`
- Criar paciente: `POST /api/patients`
- Buscar agendamentos: `GET /api/appointments/agenda`

---

## Solução de Problemas

### Problema 1: Porta 1433 já está em uso

**Sintoma**: Erro ao iniciar o SQL Server container
```
Error starting userland proxy: listen tcp 0.0.0.0:1433: bind: address already in use
```

**Solução**:
```bash
# Verificar o que está usando a porta 1433
# Windows (PowerShell como Admin):
netstat -ano | findstr :1433

# Linux/macOS:
lsof -i :1433

# Parar o serviço/processo ou usar outra porta
# Para usar porta 1434 por exemplo, modifique o docker-compose.yml:
ports:
  - "1434:1433"

# E atualize a connection string para: Server=localhost,1434;...
```

### Problema 2: Senha do SQL Server inválida

**Sintoma**: Erro de autenticação ao conectar no SQL Server

**Solução**: A senha deve atender aos requisitos:
- Mínimo 8 caracteres
- Letras maiúsculas e minúsculas
- Números
- Caracteres especiais

Use uma senha forte como: `MedicW@rehouse2024!`

### Problema 3: API não consegue conectar ao SQL Server

**Sintoma**: Erro de conexão ao banco de dados

**Solução**:
```bash
# 1. Verificar se o SQL Server está rodando
docker ps | grep sqlserver

# 2. Ver logs do SQL Server
docker logs medicwarehouse-sqlserver

# 3. Testar conexão manualmente
docker exec -it medicwarehouse-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "MedicW@rehouse2024!"

# Se conectar com sucesso, o problema pode estar na connection string
# Verifique o arquivo appsettings.json
```

### Problema 4: Frontend não carrega

**Sintoma**: Página em branco ou erro 404

**Solução**:
```bash
# 1. Verificar se o container está rodando
docker ps | grep frontend

# 2. Ver logs
docker-compose logs frontend

# 3. Reconstruir o frontend
docker-compose down
docker-compose up -d --build frontend

# 4. Verificar se a porta 4200 está livre
# Windows:
netstat -ano | findstr :4200

# Linux/macOS:
lsof -i :4200
```

### Problema 5: Erro "Database initialization failed"

**Sintoma**: API inicia mas o banco de dados não é criado

**Solução**:
```bash
# 1. Criar o banco manualmente
docker exec -it medicwarehouse-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "MedicW@rehouse2024!" \
  -Q "CREATE DATABASE Omni Care Software;"

# 2. Ou usar migrations (se configuradas)
cd src/MedicSoft.Api
dotnet ef database update

# 3. Reiniciar a API
docker-compose restart api
```

### Problema 6: Container não inicia

**Sintoma**: Container para imediatamente após iniciar

**Solução**:
```bash
# 1. Ver logs detalhados
docker logs medicwarehouse-sqlserver --tail 100

# 2. Verificar se aceitou a EULA
# No docker-compose.yml, deve ter:
environment:
  - ACCEPT_EULA=Y

# 3. Remover container e volume, e recriar
docker-compose down -v
docker-compose up -d
```

### Problema 7: Permissões negadas (Linux)

**Sintoma**: Erro de permissão ao executar Docker

**Solução**:
```bash
# Adicionar usuário ao grupo docker
sudo usermod -aG docker $USER

# Recarregar grupos (ou fazer logout/login)
newgrp docker

# Testar
docker ps
```

### Problema 8: Docker Desktop não inicia (Windows)

**Sintoma**: Docker Desktop não abre ou trava

**Solução**:
1. Verifique se a virtualização está habilitada na BIOS
2. Habilite o WSL 2 (Windows Subsystem for Linux):
   ```powershell
   wsl --install
   wsl --set-default-version 2
   ```
3. Reinicie o computador
4. Reinstale o Docker Desktop se necessário

---

## Comandos Úteis

### Docker Compose

```bash
# Iniciar todos os serviços
docker-compose up -d

# Parar todos os serviços
docker-compose down

# Parar e remover volumes (apaga dados)
docker-compose down -v

# Ver logs
docker-compose logs -f

# Reconstruir imagens
docker-compose build --no-cache

# Reiniciar um serviço específico
docker-compose restart api

# Ver status dos serviços
docker-compose ps
```

### Docker

```bash
# Listar containers rodando
docker ps

# Listar todos os containers (incluindo parados)
docker ps -a

# Parar um container
docker stop medicwarehouse-sqlserver

# Remover um container
docker rm medicwarehouse-sqlserver

# Ver logs de um container
docker logs medicwarehouse-api -f

# Executar comando dentro do container
docker exec -it medicwarehouse-sqlserver bash

# Limpar recursos não usados
docker system prune -a
```

### .NET

```bash
# Restaurar dependências
dotnet restore

# Build do projeto
dotnet build

# Executar projeto
dotnet run

# Executar testes
dotnet test

# Criar migration (Entity Framework)
dotnet ef migrations add NomeDaMigration

# Aplicar migrations
dotnet ef database update

# Ver informações do EF
dotnet ef
```

### Node.js / Angular

```bash
# Instalar dependências
npm install

# Executar em desenvolvimento
npm start
# ou
ng serve

# Build para produção
npm run build
# ou
ng build --configuration production

# Executar testes
npm test

# Verificar versão do Angular CLI
ng version
```

---

## Estrutura do Projeto

```
MW.Code/
├── docker-compose.yml          # Orquestração dos containers
├── README.md                   # Documentação principal
├── GUIA_EXECUCAO.md           # Este guia
├── IMPLEMENTATION.md          # Detalhes de implementação
├── Omni Care Software.sln         # Solution .NET
│
├── src/                       # Backend .NET 8
│   ├── MedicSoft.Api/         # API REST com JWT
│   ├── MedicSoft.Application/ # Camada de aplicação (CQRS)
│   ├── MedicSoft.Domain/      # Entidades e lógica de domínio
│   ├── MedicSoft.Repository/  # Acesso a dados (EF Core)
│   └── MedicSoft.CrossCutting/ # Serviços transversais
│
├── frontend/                  # Frontend Angular 20
│   └── medicwarehouse-app/    # Aplicação Angular
│       ├── src/
│       ├── package.json
│       └── angular.json
│
├── scripts/                   # Scripts de inicialização
│   └── init-db.sql           # Script de inicialização do BD
│
└── tests/                    # Testes
    └── MedicSoft.Test/       # Testes unitários e de integração
```

---

## Próximos Passos

Após executar o sistema com sucesso:

1. **Explore o Swagger**: Teste todos os endpoints da API
2. **Acesse o Frontend**: Navegue pelas funcionalidades
3. **Crie Dados de Teste**: Adicione pacientes, clínicas e agendamentos
4. **Consulte o Banco**: Use SSMS ou Azure Data Studio para ver as tabelas
5. **Leia o IMPLEMENTATION.md**: Entenda a arquitetura e fluxo de trabalho
6. **Personalize**: Ajuste configurações conforme sua necessidade

---

## Recursos Adicionais

- **Documentação .NET 8**: https://learn.microsoft.com/dotnet/
- **Documentação Angular 20**: https://angular.dev/overview
- **Documentação SQL Server**: https://learn.microsoft.com/sql/
- **Documentação Docker**: https://docs.docker.com/
- **Entity Framework Core**: https://learn.microsoft.com/ef/core/

---

## Suporte

Se você encontrar problemas não listados aqui:

1. Verifique os logs dos containers: `docker-compose logs`
2. Consulte a [documentação oficial](https://github.com/Omni Care Software/MW.Code)
3. Abra uma issue no GitHub
4. Entre em contato: contato@omnicaresoftware.com

---

**Desenvolvido com ❤️ pela equipe Omni Care Software**
