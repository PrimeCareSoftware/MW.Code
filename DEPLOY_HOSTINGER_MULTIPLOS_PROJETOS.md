# 🚀 Deploy na Hostinger - Múltiplos Projetos (APIs + Angular + Banco de Dados)

## 📋 Visão Geral

Este guia é especialmente para você que precisa fazer deploy de **múltiplas aplicações** no Hostinger:
- ✅ **2 APIs .NET** (MedicSoft.Api + PatientPortal.Api)
- ✅ **3-4 Aplicações Angular** (medicwarehouse-app, mw-system-admin, patient-portal, mw-docs)
- ✅ **1 Banco de Dados PostgreSQL**

**Tempo estimado**: 3-4 horas (primeira vez)  
**Nível**: Intermediário  
**Pré-requisitos**: Conhecimento básico de terminal/linha de comando

---

## 🎯 Índice

1. [Escolhendo o Plano Ideal da Hostinger](#1-escolhendo-o-plano-ideal-da-hostinger)
2. [Arquitetura da Solução](#2-arquitetura-da-solução)
3. [Configuração Inicial do VPS](#3-configuração-inicial-do-vps)
4. [Instalação dos Componentes](#4-instalação-dos-componentes)
5. [Configuração do Banco de Dados](#5-configuração-do-banco-de-dados)
6. [Deploy das APIs .NET](#6-deploy-das-apis-net)
7. [Deploy das Aplicações Angular](#7-deploy-das-aplicações-angular)
8. [Configuração do Nginx (Proxy Reverso)](#8-configuração-do-nginx-proxy-reverso)
9. [Configuração de Domínios e SSL](#9-configuração-de-domínios-e-ssl)
10. [Monitoramento e Manutenção](#10-monitoramento-e-manutenção)

---

## 1. Escolhendo o Plano Ideal da Hostinger

### 🔍 Análise de Recursos Necessários

Para rodar **2 APIs .NET + 3-4 Angular apps + PostgreSQL**, você precisa considerar:

| Componente | RAM Necessária | CPU | Disco |
|------------|---------------|-----|-------|
| **MedicSoft.Api** (.NET 8) | 512MB - 1GB | 1 vCPU | 500MB |
| **PatientPortal.Api** (.NET 8) | 512MB - 1GB | 1 vCPU | 500MB |
| **PostgreSQL 16** | 512MB - 2GB | 1 vCPU | 5-10GB |
| **Nginx** | 64MB | 0.5 vCPU | 100MB |
| **Frontend Apps** (estáticos) | 0MB* | 0 vCPU* | 500MB |
| **Sistema Operacional** | 512MB | - | 5GB |
| **Margem de Segurança** | 1GB | 1 vCPU | 5GB |
| **TOTAL ESTIMADO** | **4-6GB** | **3-4 vCPU** | **12-22GB** |

*Os arquivos Angular são estáticos e servidos pelo Nginx, não consomem RAM/CPU adicionais.

### 💰 Planos da Hostinger - Comparação

| Plano | RAM | CPU | Disco | Tráfego | Preço/mês* | Recomendação |
|-------|-----|-----|-------|---------|------------|--------------|
| **KVM 1** | 4GB | 2 vCPU | 50GB NVMe | 2TB | R$ 19,99 | ⚠️ Mínimo aceitável |
| **KVM 2** | 8GB | 4 vCPU | 100GB NVMe | 4TB | R$ 39,99 | ✅ **RECOMENDADO** |
| **KVM 3** | 12GB | 6 vCPU | 150GB NVMe | 6TB | R$ 59,99 | 💎 Ideal para produção |
| **KVM 4** | 16GB | 8 vCPU | 200GB NVMe | 8TB | R$ 79,99 | 🚀 Alto desempenho |

*Preços aproximados para contrato anual (podem variar com promoções)

### 🎯 Nossa Recomendação

#### Para Desenvolvimento/Testes:
**Plano KVM 1** (R$ 19,99/mês)
- ✅ Suficiente para testes
- ⚠️ Pode ficar lento com muitos acessos simultâneos
- ⚠️ Pouca margem para crescimento
- **Ideal para**: 1-5 usuários simultâneos

#### Para Produção (RECOMENDADO):
**Plano KVM 2** (R$ 39,99/mês)
- ✅ Ótimo custo-benefício
- ✅ RAM suficiente para 2 APIs + PostgreSQL
- ✅ Margem para picos de acesso
- ✅ Pode crescer até 20-30 clínicas
- **Ideal para**: 10-20 usuários simultâneos

#### Para Alta Disponibilidade:
**Plano KVM 3** (R$ 59,99/mês)
- ✅ Performance excelente
- ✅ Suporta backups sem impacto
- ✅ Pode escalar até 50-100 clínicas
- **Ideal para**: 30-50 usuários simultâneos

### 💡 Dicas para Economizar

1. **Contrate período mais longo**: Desconto de até 75% em planos anuais
2. **Comece com KVM 2**: É fácil fazer upgrade depois, mas downgrade pode ser complicado
3. **Use promoções**: Hostinger frequentemente tem promoções (Black Friday, Cyber Monday)
4. **Código de cupom**: Procure por cupons de desconto antes de finalizar

### 🔄 Cenários de Upgrade

| Situação | Ação Recomendada |
|----------|------------------|
| CPU frequentemente > 80% | Upgrade para plano superior |
| RAM constantemente > 85% | Upgrade urgente necessário |
| Disco > 80% usado | Limpar logs ou fazer upgrade |
| Tempo de resposta > 3s | Otimizar queries ou fazer upgrade |
| Mais de 50 acessos simultâneos | Considerar KVM 3 ou superior |

---

## 2. Arquitetura da Solução

### 🏗️ Estrutura de Diretórios

```
/var/www/primecare/
├── api/                          # API Principal (porta 5000)
│   ├── MedicSoft.Api/
│   └── appsettings.Production.json
├── patient-portal-api/           # API Portal do Paciente (porta 5001)
│   ├── PatientPortal.Api/
│   └── appsettings.Production.json
├── frontend/
│   ├── medicwarehouse-app/       # App Principal
│   ├── mw-system-admin/          # Admin
│   ├── patient-portal/           # Portal do Paciente
│   └── mw-docs/                  # Documentação
└── logs/
    ├── api/
    ├── patient-portal-api/
    └── nginx/
```

### 🌐 Configuração de Domínios

Você precisará de pelo menos 1 domínio principal. Recomendamos usar subdomínios:

```
# Exemplo com domínio: meuprimecare.com.br

# Frontend Principal
https://meuprimecare.com.br               → medicwarehouse-app

# API Principal
https://api.meuprimecare.com.br           → MedicSoft.Api (porta 5000)

# Portal do Paciente
https://paciente.meuprimecare.com.br      → patient-portal (frontend)
https://api-paciente.meuprimecare.com.br  → PatientPortal.Api (porta 5001)

# Admin
https://admin.meuprimecare.com.br         → mw-system-admin

# Documentação (opcional)
https://docs.meuprimecare.com.br          → mw-docs
```

### 🔒 Portas e Serviços

| Serviço | Porta Interna | Porta Externa | Acesso |
|---------|--------------|---------------|--------|
| PostgreSQL | 5432 | - | Local apenas |
| MedicSoft.Api | 5000 | 443 (via Nginx) | api.dominio.com.br |
| PatientPortal.Api | 5001 | 443 (via Nginx) | api-paciente.dominio.com.br |
| Nginx | 80, 443 | 80, 443 | Direto |

---

## 3. Configuração Inicial do VPS

### Passo 1: Primeiro Acesso

```bash
# Conectar ao VPS (substitua pelo seu IP)
ssh root@185.123.456.789

# Atualizar sistema
apt update && apt upgrade -y

# Instalar ferramentas essenciais
apt install -y curl wget git unzip nano htop ufw
```

### Passo 2: Criar Usuário de Deploy

```bash
# Criar usuário
adduser primecare
# Defina uma senha forte

# Adicionar ao grupo sudo
usermod -aG sudo primecare

# Permitir sudo sem senha (opcional, para automação)
echo "primecare ALL=(ALL) NOPASSWD:ALL" | sudo tee /etc/sudoers.d/primecare
```

### Passo 3: Configurar Firewall

```bash
# Habilitar UFW
ufw --force enable

# Permitir SSH
ufw allow 22/tcp

# Permitir HTTP e HTTPS
ufw allow 80/tcp
ufw allow 443/tcp

# Permitir APIs (apenas localmente)
# As portas 5000 e 5001 NÃO devem ser expostas diretamente
# O Nginx irá fazer proxy reverso

# Verificar status
ufw status verbose
```

### Passo 4: Configurar Swap (importante!)

Com múltiplas aplicações, é essencial ter swap:

```bash
# Criar arquivo de swap de 2GB
sudo fallocate -l 2G /swapfile

# Configurar permissões
sudo chmod 600 /swapfile

# Configurar como swap
sudo mkswap /swapfile
sudo swapon /swapfile

# Tornar permanente
echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab

# Verificar
sudo swapon --show
free -h
```

---

## 4. Instalação dos Componentes

### 4.1. Instalar .NET 8 SDK

```bash
# Adicionar repositório Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Instalar .NET 8 SDK
sudo apt update
sudo apt install -y dotnet-sdk-8.0

# Verificar
dotnet --version
# Deve mostrar: 8.0.x
```

### 4.2. Instalar PostgreSQL 16

```bash
# Adicionar repositório oficial
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -

# Instalar PostgreSQL
sudo apt update
sudo apt install -y postgresql-16 postgresql-contrib-16

# Iniciar e habilitar
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Verificar status
sudo systemctl status postgresql
```

### 4.3. Instalar Node.js 18 (para build do Angular)

```bash
# Instalar Node.js via NodeSource
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs

# Instalar Angular CLI globalmente
sudo npm install -g @angular/cli

# Verificar instalações
node --version   # v18.x.x
npm --version    # 9.x.x
ng version       # Angular CLI 17.x
```

### 4.4. Instalar Nginx

```bash
# Instalar Nginx
sudo apt install -y nginx

# Iniciar e habilitar
sudo systemctl start nginx
sudo systemctl enable nginx

# Verificar status
sudo systemctl status nginx
```

### 4.5. Instalar Certbot (para SSL gratuito)

```bash
# Instalar Certbot
sudo apt install -y certbot python3-certbot-nginx

# Verificar instalação
certbot --version
```

---

## 5. Configuração do Banco de Dados

### 5.1. Configurar PostgreSQL

```bash
# Acessar PostgreSQL como usuário postgres
sudo -u postgres psql
```

Execute os seguintes comandos SQL:

```sql
-- Criar usuário para o PrimeCare
CREATE USER primecare_user WITH PASSWORD 'SuaSenhaForte123!';

-- Criar banco de dados principal
CREATE DATABASE primecare_db;

-- Criar banco de dados do Portal do Paciente
CREATE DATABASE patient_portal_db;

-- Dar permissões
GRANT ALL PRIVILEGES ON DATABASE primecare_db TO primecare_user;
GRANT ALL PRIVILEGES ON DATABASE patient_portal_db TO primecare_user;

-- Configurar owner
ALTER DATABASE primecare_db OWNER TO primecare_user;
ALTER DATABASE patient_portal_db OWNER TO primecare_user;

-- Listar bancos criados
\l

-- Sair
\q
```

### 5.2. Configurar Acesso Remoto (Opcional)

Se você precisar acessar o banco de dados remotamente para desenvolvimento:

```bash
# Editar postgresql.conf
sudo nano /etc/postgresql/16/main/postgresql.conf
```

Encontre e descomente/altere:
```conf
listen_addresses = 'localhost'  # Mantenha 'localhost' por segurança
```

Para acesso remoto seguro, use SSH Tunnel ao invés de expor a porta:
```bash
# No seu computador local:
ssh -L 5432:localhost:5432 primecare@seu-ip-vps
```

### 5.3. Otimizar PostgreSQL para Múltiplas Aplicações

```bash
# Editar configurações
sudo nano /etc/postgresql/16/main/postgresql.conf
```

Ajuste estas configurações baseado no seu plano:

```conf
# Para KVM 2 (8GB RAM):
shared_buffers = 2GB                  # 25% da RAM
effective_cache_size = 6GB            # 75% da RAM
maintenance_work_mem = 512MB
work_mem = 32MB
max_connections = 100

# Para KVM 1 (4GB RAM):
shared_buffers = 1GB
effective_cache_size = 3GB
maintenance_work_mem = 256MB
work_mem = 16MB
max_connections = 50
```

Reiniciar PostgreSQL:
```bash
sudo systemctl restart postgresql
```

---

## 6. Deploy das APIs .NET

### 6.1. Preparar Estrutura de Diretórios

```bash
# Criar diretórios
sudo mkdir -p /var/www/primecare/{api,patient-portal-api,logs/api,logs/patient-portal-api}

# Criar diretório de deploy temporário
sudo mkdir -p /home/primecare/deploy

# Ajustar permissões
sudo chown -R primecare:primecare /var/www/primecare
sudo chown -R primecare:primecare /home/primecare/deploy
```

### 6.2. Clonar Repositório

```bash
# Mudar para usuário primecare
su - primecare

# Clonar repositório
cd /home/primecare/deploy
git clone https://github.com/PrimeCareSoftware/MW.Code.git
cd MW.Code
```

### 6.3. Deploy da API Principal (MedicSoft.Api)

```bash
# Navegar para o projeto
cd /home/primecare/deploy/MW.Code/src/MedicSoft.Api

# Criar arquivo de configuração de produção
cat > appsettings.Production.json << 'EOF'
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=primecare_db;Username=primecare_user;Password=SuaSenhaForte123!"
  },
  "JwtSettings": {
    "SecretKey": "GERE_UMA_CHAVE_SEGURA_DE_PELO_MENOS_32_CARACTERES",
    "ExpiryMinutes": 60,
    "Issuer": "PrimeCare Software",
    "Audience": "PrimeCare Software-API"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://meuprimecare.com.br",
      "https://admin.meuprimecare.com.br",
      "https://paciente.meuprimecare.com.br",
      "https://docs.meuprimecare.com.br"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
EOF

# Publicar aplicação
dotnet publish -c Release -o /var/www/primecare/api

# Verificar arquivos publicados
ls -la /var/www/primecare/api
```

### 6.4. Deploy da API Portal do Paciente

```bash
# Navegar para o projeto
cd /home/primecare/deploy/MW.Code/patient-portal-api/PatientPortal.Api

# Criar arquivo de configuração de produção
cat > appsettings.Production.json << 'EOF'
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=patient_portal_db;Username=primecare_user;Password=SuaSenhaForte123!"
  },
  "JwtSettings": {
    "SecretKey": "MESMA_CHAVE_DA_API_PRINCIPAL_OU_OUTRA_SEGURA",
    "ExpiryMinutes": 60,
    "Issuer": "PrimeCare Patient Portal",
    "Audience": "PrimeCare Patient Portal API"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://paciente.meuprimecare.com.br"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
EOF

# Publicar aplicação
dotnet publish -c Release -o /var/www/primecare/patient-portal-api

# Verificar arquivos publicados
ls -la /var/www/primecare/patient-portal-api
```

### 6.5. Criar Serviços Systemd para APIs

#### API Principal:

```bash
sudo nano /etc/systemd/system/primecare-api.service
```

Conteúdo:
```ini
[Unit]
Description=PrimeCare API
After=network.target postgresql.service

[Service]
Type=notify
User=primecare
WorkingDirectory=/var/www/primecare/api
ExecStart=/usr/bin/dotnet /var/www/primecare/api/MedicSoft.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=primecare-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=ASPNETCORE_URLS=http://localhost:5000

# Limites de recursos
LimitNOFILE=65536
TimeoutStopSec=90

[Install]
WantedBy=multi-user.target
```

#### API Portal do Paciente:

```bash
sudo nano /etc/systemd/system/patient-portal-api.service
```

Conteúdo:
```ini
[Unit]
Description=PrimeCare Patient Portal API
After=network.target postgresql.service

[Service]
Type=notify
User=primecare
WorkingDirectory=/var/www/primecare/patient-portal-api
ExecStart=/usr/bin/dotnet /var/www/primecare/patient-portal-api/PatientPortal.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=patient-portal-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=ASPNETCORE_URLS=http://localhost:5001

# Limites de recursos
LimitNOFILE=65536
TimeoutStopSec=90

[Install]
WantedBy=multi-user.target
```

### 6.6. Executar Migrations

```bash
# API Principal
cd /var/www/primecare/api
sudo -u primecare dotnet ef database update --project /home/primecare/deploy/MW.Code/src/MedicSoft.Api

# API Portal do Paciente
cd /var/www/primecare/patient-portal-api
sudo -u primecare dotnet ef database update --project /home/primecare/deploy/MW.Code/patient-portal-api/PatientPortal.Api
```

### 6.7. Iniciar Serviços das APIs

```bash
# Recarregar systemd
sudo systemctl daemon-reload

# Iniciar e habilitar serviços
sudo systemctl start primecare-api
sudo systemctl enable primecare-api

sudo systemctl start patient-portal-api
sudo systemctl enable patient-portal-api

# Verificar status
sudo systemctl status primecare-api
sudo systemctl status patient-portal-api

# Ver logs em tempo real
sudo journalctl -u primecare-api -f
sudo journalctl -u patient-portal-api -f
```

---

## 7. Deploy das Aplicações Angular

### 7.1. Build das Aplicações

```bash
# Voltar ao diretório do repositório
cd /home/primecare/deploy/MW.Code/frontend
```

#### 7.1.1. Medicwarehouse App (Principal)

```bash
cd medicwarehouse-app

# Criar environment de produção
cat > src/environments/environment.prod.ts << 'EOF'
export const environment = {
  production: true,
  apiUrl: 'https://api.meuprimecare.com.br',
  appName: 'PrimeCare Software',
  version: '1.0.0'
};
EOF

# Instalar dependências
npm install

# Build de produção
ng build --configuration production

# Copiar arquivos
sudo mkdir -p /var/www/primecare/frontend/medicwarehouse-app
sudo cp -r dist/medicwarehouse-app/browser/* /var/www/primecare/frontend/medicwarehouse-app/
```

#### 7.1.2. System Admin

```bash
cd ../mw-system-admin

# Criar environment de produção
cat > src/environments/environment.prod.ts << 'EOF'
export const environment = {
  production: true,
  apiUrl: 'https://api.meuprimecare.com.br',
  appName: 'PrimeCare Admin',
  version: '1.0.0'
};
EOF

# Instalar dependências
npm install

# Build de produção
ng build --configuration production

# Copiar arquivos
sudo mkdir -p /var/www/primecare/frontend/mw-system-admin
sudo cp -r dist/mw-system-admin/browser/* /var/www/primecare/frontend/mw-system-admin/
```

#### 7.1.3. Patient Portal

```bash
cd ../patient-portal

# Criar environment de produção
cat > src/environments/environment.prod.ts << 'EOF'
export const environment = {
  production: true,
  apiUrl: 'https://api-paciente.meuprimecare.com.br',
  appName: 'Portal do Paciente',
  version: '1.0.0'
};
EOF

# Instalar dependências
npm install

# Build de produção
ng build --configuration production

# Copiar arquivos
sudo mkdir -p /var/www/primecare/frontend/patient-portal
sudo cp -r dist/patient-portal/browser/* /var/www/primecare/frontend/patient-portal/
```

#### 7.1.4. Documentação (Opcional)

```bash
cd ../mw-docs

# Instalar dependências
npm install

# Build de produção
ng build --configuration production

# Copiar arquivos
sudo mkdir -p /var/www/primecare/frontend/mw-docs
sudo cp -r dist/mw-docs/browser/* /var/www/primecare/frontend/mw-docs/
```

### 7.2. Ajustar Permissões

```bash
# Ajustar permissões de todos os frontends
sudo chown -R www-data:www-data /var/www/primecare/frontend
sudo chmod -R 755 /var/www/primecare/frontend
```

---

## 8. Configuração do Nginx (Proxy Reverso)

### 8.1. Remover Configuração Padrão

```bash
sudo rm /etc/nginx/sites-enabled/default
```

### 8.2. Configuração Principal (API)

```bash
sudo nano /etc/nginx/sites-available/primecare-api
```

Conteúdo:
```nginx
# Rate limiting
limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;

# Upstream para API Principal
upstream primecare_api {
    server localhost:5000;
    keepalive 32;
}

# Upstream para Patient Portal API
upstream patient_portal_api {
    server localhost:5001;
    keepalive 32;
}

# API Principal
server {
    listen 80;
    server_name api.meuprimecare.com.br;
    
    # Redirect para HTTPS (será configurado depois)
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name api.meuprimecare.com.br;
    
    # SSL certificates (será preenchido pelo Certbot)
    # ssl_certificate /etc/letsencrypt/live/api.meuprimecare.com.br/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/api.meuprimecare.com.br/privkey.pem;
    
    # SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    
    # Logs
    access_log /var/www/primecare/logs/api/access.log;
    error_log /var/www/primecare/logs/api/error.log;
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    
    # Client body size
    client_max_body_size 50M;
    
    location / {
        # Rate limiting
        limit_req zone=api_limit burst=20 nodelay;
        
        proxy_pass http://primecare_api;
        proxy_http_version 1.1;
        
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host $server_name;
        
        proxy_cache_bypass $http_upgrade;
        proxy_buffering off;
        proxy_read_timeout 300s;
        proxy_connect_timeout 75s;
    }
}

# API Portal do Paciente
server {
    listen 80;
    server_name api-paciente.meuprimecare.com.br;
    
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name api-paciente.meuprimecare.com.br;
    
    # SSL certificates (será preenchido pelo Certbot)
    # ssl_certificate /etc/letsencrypt/live/api-paciente.meuprimecare.com.br/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/api-paciente.meuprimecare.com.br/privkey.pem;
    
    # SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    
    # Logs
    access_log /var/www/primecare/logs/patient-portal-api/access.log;
    error_log /var/www/primecare/logs/patient-portal-api/error.log;
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    
    client_max_body_size 50M;
    
    location / {
        limit_req zone=api_limit burst=20 nodelay;
        
        proxy_pass http://patient_portal_api;
        proxy_http_version 1.1;
        
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host $server_name;
        
        proxy_cache_bypass $http_upgrade;
        proxy_buffering off;
        proxy_read_timeout 300s;
        proxy_connect_timeout 75s;
    }
}
```

### 8.3. Configuração dos Frontends

```bash
sudo nano /etc/nginx/sites-available/primecare-frontend
```

Conteúdo:
```nginx
# Frontend Principal (Medicwarehouse App)
server {
    listen 80;
    server_name meuprimecare.com.br www.meuprimecare.com.br;
    
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name meuprimecare.com.br www.meuprimecare.com.br;
    
    # SSL certificates
    # ssl_certificate /etc/letsencrypt/live/meuprimecare.com.br/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/meuprimecare.com.br/privkey.pem;
    
    root /var/www/primecare/frontend/medicwarehouse-app;
    index index.html;
    
    # Logs
    access_log /var/log/nginx/primecare-frontend-access.log;
    error_log /var/log/nginx/primecare-frontend-error.log;
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    
    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}

# System Admin
server {
    listen 80;
    server_name admin.meuprimecare.com.br;
    
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name admin.meuprimecare.com.br;
    
    # SSL certificates
    # ssl_certificate /etc/letsencrypt/live/admin.meuprimecare.com.br/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/admin.meuprimecare.com.br/privkey.pem;
    
    root /var/www/primecare/frontend/mw-system-admin;
    index index.html;
    
    # Logs
    access_log /var/log/nginx/admin-access.log;
    error_log /var/log/nginx/admin-error.log;
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    
    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}

# Patient Portal
server {
    listen 80;
    server_name paciente.meuprimecare.com.br;
    
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name paciente.meuprimecare.com.br;
    
    # SSL certificates
    # ssl_certificate /etc/letsencrypt/live/paciente.meuprimecare.com.br/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/paciente.meuprimecare.com.br/privkey.pem;
    
    root /var/www/primecare/frontend/patient-portal;
    index index.html;
    
    # Logs
    access_log /var/log/nginx/patient-portal-access.log;
    error_log /var/log/nginx/patient-portal-error.log;
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    
    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}

# Documentação (Opcional)
server {
    listen 80;
    server_name docs.meuprimecare.com.br;
    
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name docs.meuprimecare.com.br;
    
    # SSL certificates
    # ssl_certificate /etc/letsencrypt/live/docs.meuprimecare.com.br/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/docs.meuprimecare.com.br/privkey.pem;
    
    root /var/www/primecare/frontend/mw-docs;
    index index.html;
    
    # Logs
    access_log /var/log/nginx/docs-access.log;
    error_log /var/log/nginx/docs-error.log;
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    
    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

### 8.4. Ativar Configurações

```bash
# Criar links simbólicos
sudo ln -s /etc/nginx/sites-available/primecare-api /etc/nginx/sites-enabled/
sudo ln -s /etc/nginx/sites-available/primecare-frontend /etc/nginx/sites-enabled/

# Testar configuração
sudo nginx -t

# Reiniciar Nginx
sudo systemctl restart nginx
```

---

## 9. Configuração de Domínios e SSL

### 9.1. Configurar DNS

No painel do seu provedor de domínio (ex: Registro.br, GoDaddy), crie os seguintes registros:

| Tipo | Nome | Valor | TTL |
|------|------|-------|-----|
| A | @ | IP_DO_SEU_VPS | 3600 |
| A | www | IP_DO_SEU_VPS | 3600 |
| A | api | IP_DO_SEU_VPS | 3600 |
| A | api-paciente | IP_DO_SEU_VPS | 3600 |
| A | admin | IP_DO_SEU_VPS | 3600 |
| A | paciente | IP_DO_SEU_VPS | 3600 |
| A | docs | IP_DO_SEU_VPS | 3600 |

**Aguarde**: A propagação DNS pode levar de 15 minutos a 48 horas.

### 9.2. Instalar Certificados SSL (HTTPS)

Após a propagação do DNS:

```bash
# Obter certificados para todos os domínios
sudo certbot --nginx -d meuprimecare.com.br -d www.meuprimecare.com.br
sudo certbot --nginx -d api.meuprimecare.com.br
sudo certbot --nginx -d api-paciente.meuprimecare.com.br
sudo certbot --nginx -d admin.meuprimecare.com.br
sudo certbot --nginx -d paciente.meuprimecare.com.br
sudo certbot --nginx -d docs.meuprimecare.com.br

# O Certbot irá:
# 1. Obter certificados do Let's Encrypt
# 2. Configurar automaticamente o Nginx
# 3. Configurar renovação automática
```

### 9.3. Verificar Renovação Automática

```bash
# Testar renovação
sudo certbot renew --dry-run

# Verificar timer de renovação
sudo systemctl status certbot.timer
```

### 9.4. Reiniciar Nginx

```bash
sudo systemctl restart nginx
```

---

## 10. Monitoramento e Manutenção

### 10.1. Script de Monitoramento

Crie um script para monitorar o sistema:

```bash
sudo nano /usr/local/bin/primecare-status.sh
```

Conteúdo:
```bash
#!/bin/bash

echo "================================================"
echo "   PRIMECARE - STATUS DO SISTEMA"
echo "================================================"
echo ""

# APIs
echo "🔧 SERVIÇOS:"
echo "  API Principal:      $(systemctl is-active primecare-api)"
echo "  Patient Portal API: $(systemctl is-active patient-portal-api)"
echo "  PostgreSQL:         $(systemctl is-active postgresql)"
echo "  Nginx:              $(systemctl is-active nginx)"
echo ""

# Uso de recursos
echo "💻 RECURSOS:"
echo "  CPU: $(top -bn1 | grep "Cpu(s)" | sed "s/.*, *\([0-9.]*\)%* id.*/\1/" | awk '{print 100 - $1"%"}')"
echo "  RAM: $(free -h | awk '/^Mem:/ {print $3 "/" $2 " (" int($3/$2*100) "%)"}')"
echo "  Disk: $(df -h / | awk 'NR==2 {print $3 "/" $2 " (" $5 ")"}')"
echo ""

# Conexões PostgreSQL
echo "🗄️ POSTGRESQL:"
echo "  Conexões ativas: $(sudo -u postgres psql -c "SELECT count(*) FROM pg_stat_activity;" -t | xargs)"
echo "  Tamanho primecare_db: $(sudo -u postgres psql -c "SELECT pg_size_pretty(pg_database_size('primecare_db'));" -t | xargs)"
echo "  Tamanho patient_portal_db: $(sudo -u postgres psql -c "SELECT pg_size_pretty(pg_database_size('patient_portal_db'));" -t | xargs)"
echo ""

# Últimos erros
echo "⚠️ ÚLTIMOS ERROS (API Principal):"
sudo journalctl -u primecare-api --since "10 minutes ago" | grep -i error | tail -3
echo ""

echo "⚠️ ÚLTIMOS ERROS (Patient Portal API):"
sudo journalctl -u patient-portal-api --since "10 minutes ago" | grep -i error | tail -3
echo ""

echo "================================================"
```

Tornar executável:
```bash
sudo chmod +x /usr/local/bin/primecare-status.sh
```

Usar:
```bash
primecare-status.sh
```

### 10.2. Configurar Backups Automáticos

```bash
sudo nano /usr/local/bin/primecare-backup.sh
```

Conteúdo:
```bash
#!/bin/bash

# Configurações
BACKUP_DIR="/home/primecare/backups"
DATE=$(date +%Y%m%d_%H%M%S)
RETENTION_DAYS=7

# Criar diretório de backup
mkdir -p $BACKUP_DIR

# Backup PostgreSQL
echo "Fazendo backup dos bancos de dados..."
sudo -u postgres pg_dump primecare_db | gzip > $BACKUP_DIR/primecare_db_$DATE.sql.gz
sudo -u postgres pg_dump patient_portal_db | gzip > $BACKUP_DIR/patient_portal_db_$DATE.sql.gz

# Backup arquivos de configuração
echo "Fazendo backup de configurações..."
tar -czf $BACKUP_DIR/config_$DATE.tar.gz \
    /etc/nginx/sites-available/ \
    /etc/systemd/system/primecare-api.service \
    /etc/systemd/system/patient-portal-api.service \
    /var/www/primecare/api/appsettings.Production.json \
    /var/www/primecare/patient-portal-api/appsettings.Production.json

# Remover backups antigos
echo "Removendo backups antigos..."
find $BACKUP_DIR -name "*.gz" -mtime +$RETENTION_DAYS -delete
find $BACKUP_DIR -name "*.tar.gz" -mtime +$RETENTION_DAYS -delete

echo "Backup concluído: $BACKUP_DIR"
ls -lh $BACKUP_DIR | tail -5
```

Tornar executável:
```bash
sudo chmod +x /usr/local/bin/primecare-backup.sh
```

Agendar backup diário:
```bash
# Editar crontab
crontab -e

# Adicionar linha para backup diário às 2h da manhã
0 2 * * * /usr/local/bin/primecare-backup.sh >> /var/log/primecare-backup.log 2>&1
```

### 10.3. Monitoramento de Logs

```bash
# Ver logs das APIs em tempo real
sudo journalctl -u primecare-api -f
sudo journalctl -u patient-portal-api -f

# Ver logs do Nginx
sudo tail -f /var/log/nginx/primecare-frontend-access.log
sudo tail -f /var/log/nginx/primecare-frontend-error.log

# Ver erros de todas as APIs
sudo journalctl -u primecare-api -u patient-portal-api --since "1 hour ago" | grep -i error
```

### 10.4. Script de Atualização

```bash
sudo nano /usr/local/bin/primecare-update.sh
```

Conteúdo:
```bash
#!/bin/bash

echo "🚀 Iniciando atualização do PrimeCare..."

# Backup antes de atualizar
echo "📦 Fazendo backup..."
/usr/local/bin/primecare-backup.sh

# Ir para diretório do código
cd /home/primecare/deploy/MW.Code

# Atualizar código
echo "📥 Baixando atualizações..."
git pull origin main

# Atualizar API Principal
echo "🔧 Atualizando API Principal..."
cd src/MedicSoft.Api
dotnet publish -c Release -o /var/www/primecare/api
sudo systemctl restart primecare-api

# Atualizar Patient Portal API
echo "🔧 Atualizando Patient Portal API..."
cd ../../patient-portal-api/PatientPortal.Api
dotnet publish -c Release -o /var/www/primecare/patient-portal-api
sudo systemctl restart patient-portal-api

# Atualizar Frontends (exemplo com medicwarehouse-app)
echo "🎨 Atualizando Frontend Principal..."
cd ../../frontend/medicwarehouse-app
npm install
ng build --configuration production
sudo cp -r dist/medicwarehouse-app/browser/* /var/www/primecare/frontend/medicwarehouse-app/

# Aguardar APIs iniciarem
echo "⏳ Aguardando serviços iniciarem..."
sleep 10

# Verificar status
echo "✅ Verificando status..."
sudo systemctl status primecare-api --no-pager
sudo systemctl status patient-portal-api --no-pager

echo "✨ Atualização concluída!"
```

Tornar executável:
```bash
sudo chmod +x /usr/local/bin/primecare-update.sh
```

---

## 📊 Resumo - Checklist Final

### Antes de Ir para Produção

- [ ] VPS contratado e configurado
- [ ] DNS configurado e propagado
- [ ] SSL instalado em todos os domínios
- [ ] PostgreSQL instalado e otimizado
- [ ] Banco de dados criados (primecare_db e patient_portal_db)
- [ ] Migrations executadas com sucesso
- [ ] API Principal rodando na porta 5000
- [ ] Patient Portal API rodando na porta 5001
- [ ] 4 frontends Angular compilados e servidos
- [ ] Nginx configurado como proxy reverso
- [ ] Firewall (UFW) configurado corretamente
- [ ] Backups automáticos configurados
- [ ] Monitoramento configurado
- [ ] Testar todos os endpoints das APIs
- [ ] Testar todas as aplicações frontend
- [ ] Verificar logs de erro
- [ ] Documentar credenciais em local seguro

### URLs para Testar

- [ ] https://meuprimecare.com.br (Frontend Principal)
- [ ] https://admin.meuprimecare.com.br (System Admin)
- [ ] https://paciente.meuprimecare.com.br (Portal do Paciente)
- [ ] https://docs.meuprimecare.com.br (Documentação)
- [ ] https://api.meuprimecare.com.br/swagger (API Principal)
- [ ] https://api-paciente.meuprimecare.com.br/swagger (Patient Portal API)

---

## 🆘 Troubleshooting

### Problema: API não inicia

```bash
# Verificar logs
sudo journalctl -u primecare-api -n 50

# Verificar se a porta está em uso
sudo netstat -tulpn | grep 5000

# Verificar permissões
ls -la /var/www/primecare/api
```

### Problema: Frontend mostra página em branco

```bash
# Verificar se os arquivos existem
ls -la /var/www/primecare/frontend/medicwarehouse-app/

# Verificar logs do Nginx
sudo tail -f /var/log/nginx/primecare-frontend-error.log

# Verificar configuração do Nginx
sudo nginx -t
```

### Problema: Erro de conexão com banco de dados

```bash
# Verificar se PostgreSQL está rodando
sudo systemctl status postgresql

# Testar conexão
sudo -u postgres psql -c "SELECT version();"

# Verificar connection string no appsettings.Production.json
cat /var/www/primecare/api/appsettings.Production.json | grep ConnectionStrings
```

### Problema: CORS Error no Frontend

1. Verificar se o domínio está em `Cors__AllowedOrigins` no `appsettings.Production.json`
2. Reiniciar a API após qualquer mudança
3. Verificar se está usando HTTPS (não HTTP)

### Problema: Alto uso de memória

```bash
# Ver processos consumindo mais memória
ps aux --sort=-%mem | head -10

# Adicionar mais swap
sudo fallocate -l 4G /swapfile2
sudo chmod 600 /swapfile2
sudo mkswap /swapfile2
sudo swapon /swapfile2
```

---

## 💰 Estimativa de Custos Mensais

| Item | Custo Mensal |
|------|--------------|
| VPS Hostinger KVM 2 | R$ 39,99 |
| Domínio (.com.br) | R$ 3,33 (R$ 40/ano) |
| SSL Certificates | R$ 0,00 (Let's Encrypt) |
| Backup externo (opcional) | R$ 10-30 |
| **TOTAL** | **R$ 43,32 - 73,32** |

**Economia vs. Cloud tradicional**: 60-80%

---

## 📚 Próximos Passos

1. **Configurar monitoramento avançado**: Instalar Grafana + Prometheus
2. **Configurar CI/CD**: GitHub Actions para deploy automático
3. **Implementar cache**: Redis para melhorar performance
4. **Configurar CDN**: Cloudflare para distribuir conteúdo estático
5. **Backup externo**: Configurar backup para storage externo (S3, BackBlaze)

---

## 🤝 Suporte

- **Documentação Completa**: [DEPLOY_HOSTINGER_GUIA_COMPLETO.md](./DEPLOY_HOSTINGER_GUIA_COMPLETO.md)
- **Guia de Início Rápido**: [DEPLOY_HOSTINGER_INICIO_RAPIDO.md](./DEPLOY_HOSTINGER_INICIO_RAPIDO.md)
- **Problemas Comuns**: [docs/COMMON_ISSUES.md](../docs/COMMON_ISSUES.md)

---

## ✅ Conclusão

Seguindo este guia, você terá:
- ✅ 2 APIs .NET rodando de forma isolada
- ✅ 4 aplicações Angular otimizadas
- ✅ 1 banco de dados PostgreSQL configurado
- ✅ SSL em todos os domínios
- ✅ Backups automáticos
- ✅ Monitoramento básico

**Custo total**: R$ 40-75/mês  
**Capacidade**: 10-30 usuários simultâneos  
**Tempo de setup**: 3-4 horas

Agora seu PrimeCare Software está no ar! 🎉
