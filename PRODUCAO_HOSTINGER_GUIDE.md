# 🚀 Guia de Deploy em Produção - Hostinger KVM 2

> **📋 Guia Especializado para Produção**  
> Este guia foca no deploy seguro para produção usando Hostinger KVM 2, **SEM o Portal do Paciente**.

## 🎯 O que será implantado?

### ✅ Componentes INCLUÍDOS:
- 🌐 **Site/Sistema Principal** (Frontend Angular - medicwarehouse-app)
- 🔧 **Sistema de Administração** (Frontend Angular - mw-system-admin)
- 🔌 **API Principal** (Backend .NET 8 - MedicSoft.Api)
- 📹 **Microserviço de Telemedicina** (Backend .NET 8 - Telemedicine.Api)
- 🗄️ **Banco de Dados PostgreSQL 16**

### ❌ Componentes EXCLUÍDOS:
- ~~Portal do Paciente (Frontend)~~
- ~~API do Portal do Paciente~~

---

## 📊 Requisitos do Servidor

### Plano Recomendado: Hostinger KVM 2

| Recurso | Especificação | Uso Estimado |
|---------|--------------|--------------|
| **RAM** | 8GB | 4-6GB em uso normal |
| **CPU** | 4 vCPU | 2-3 vCPU em uso médio |
| **Disco** | 100GB NVMe | 20-30GB inicial |
| **Tráfego** | 4TB/mês | Ilimitado na prática |
| **Custo** | ~R$ 39,99/mês | Contrato anual |

**Capacidade estimada**: 10-30 clínicas pequenas ou 20-50 usuários simultâneos

---

## 🏗️ Arquitetura da Solução

```
                    Internet (HTTPS)
                           |
                    Nginx (Porta 80/443)
                    [SSL + Proxy Reverso]
                           |
         ┏━━━━━━━━━━━━━━━━━┻━━━━━━━━━━━━━━━━┓
         ┃                                   ┃
   APIs (.NET 8)                    Frontends (Angular)
   ┣━ MedicSoft.Api (5000)          ┣━ medicwarehouse-app
   ┗━ Telemedicine.Api (5084)       ┗━ mw-system-admin
         |
         ↓
   PostgreSQL 16 (5432)
   ┣━ primecare (API Principal)
   ┗━ telemedicine (Telemedicina)
```

---

## 🌐 Estrutura de Subdomínios

Com 1 domínio (ex: `suaclinica.com.br`), você criará **5 subdomínios**:

| Subdomínio | Aplicação | Porta Interna | Tipo |
|------------|-----------|---------------|------|
| `suaclinica.com.br` | medicwarehouse-app | 80 (Nginx) | Frontend |
| `api.suaclinica.com.br` | MedicSoft.Api | 5000 | API |
| `tele.suaclinica.com.br` | Telemedicine.Api | 5084 | API |
| `admin.suaclinica.com.br` | mw-system-admin | 80 (Nginx) | Frontend |
| `docs.suaclinica.com.br` | mw-docs (opcional) | 80 (Nginx) | Frontend |

**🔒 Todos com SSL/HTTPS gratuito via Let's Encrypt!**

---

## 📋 Índice

1. [Preparação Inicial](#1-preparação-inicial)
2. [Configuração do VPS](#2-configuração-do-vps)
3. [Instalação dos Componentes](#3-instalação-dos-componentes)
4. [Configuração do Banco de Dados](#4-configuração-do-banco-de-dados)
5. [Deploy das APIs](#5-deploy-das-apis)
6. [Deploy dos Frontends](#6-deploy-dos-frontends)
7. [Configuração do Nginx](#7-configuração-do-nginx)
8. [Configuração de Subdomínios na Hostinger](#8-configuração-de-subdomínios-na-hostinger)
9. [Configuração de SSL](#9-configuração-de-ssl)
10. [Segurança para Dados Sensíveis](#10-segurança-para-dados-sensíveis)
11. [Backups e Monitoramento](#11-backups-e-monitoramento)
12. [Checklist Final](#12-checklist-final)

---

## 1. Preparação Inicial

### 1.1. Informações que Você Precisará

Antes de começar, tenha em mãos:

- [ ] Acesso ao painel da Hostinger
- [ ] Domínio registrado (ex: `suaclinica.com.br`)
- [ ] Chave API do Daily.co (para telemedicina)
- [ ] Editor de texto para anotar senhas geradas

### 1.2. Contratar o VPS

1. Acesse: https://www.hostinger.com.br/vps
2. Escolha o plano **KVM 2** (8GB RAM, 4 vCPU)
3. Selecione **Ubuntu 22.04 LTS** como sistema operacional
4. Defina uma senha forte para o usuário root
5. Finalize a compra

**⏱️ Tempo de ativação**: 5-15 minutos após o pagamento

---

## 2. Configuração do VPS

### 2.1. Primeiro Acesso via SSH

```bash
# Conectar ao servidor (substitua IP_DO_SEU_VPS)
ssh root@IP_DO_SEU_VPS

# Você será solicitado a confirmar a conexão (digite: yes)
# Digite a senha que você definiu na Hostinger
```

### 2.2. Atualizar o Sistema

```bash
# Atualizar lista de pacotes
apt update

# Atualizar pacotes instalados
apt upgrade -y

# Instalar pacotes essenciais
apt install -y curl wget git ufw nano htop
```

### 2.3. Configurar Firewall (UFW)

```bash
# Permitir SSH (IMPORTANTE: não pule este passo!)
ufw allow 22/tcp

# Permitir HTTP e HTTPS
ufw allow 80/tcp
ufw allow 443/tcp

# Ativar firewall
ufw --force enable

# Verificar status
ufw status verbose
```

**🔒 Segurança**: As portas 5000 e 5084 NÃO serão expostas externamente. O Nginx fará proxy reverso.

### 2.4. Criar Swap (Importante!)

```bash
# Criar arquivo de swap de 4GB (50% da RAM)
fallocate -l 4G /swapfile

# Configurar permissões
chmod 600 /swapfile

# Configurar como swap
mkswap /swapfile
swapon /swapfile

# Tornar permanente
echo '/swapfile none swap sw 0 0' >> /etc/fstab

# Verificar
swapon --show
free -h
```

### 2.5. Criar Usuário de Deploy

```bash
# Criar usuário
adduser deploy

# Adicionar ao grupo sudo
usermod -aG sudo deploy

# Configurar diretório de deploy
mkdir -p /var/www/primecare
chown -R deploy:deploy /var/www/primecare
```

---

## 3. Instalação dos Componentes

### 3.1. Instalar .NET 8 SDK

```bash
# Adicionar repositório Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Atualizar e instalar
apt update
apt install -y dotnet-sdk-8.0

# Verificar
dotnet --version
# Deve mostrar: 8.0.x
```

### 3.2. Instalar PostgreSQL 16

```bash
# Adicionar repositório oficial do PostgreSQL
sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
wget -qO- https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add -

# Instalar
apt update
apt install -y postgresql-16 postgresql-contrib-16

# Iniciar e habilitar
systemctl start postgresql
systemctl enable postgresql

# Verificar
systemctl status postgresql
```

### 3.3. Instalar Node.js 18 (para build do Angular)

```bash
# Instalar via NodeSource
curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
apt install -y nodejs

# Instalar Angular CLI globalmente
npm install -g @angular/cli@latest

# Verificar
node --version   # v18.x.x
npm --version    # 9.x.x
ng version       # Angular CLI 17.x
```

### 3.4. Instalar Nginx

```bash
# Instalar
apt install -y nginx

# Iniciar e habilitar
systemctl start nginx
systemctl enable nginx

# Verificar
systemctl status nginx
```

### 3.5. Instalar Certbot (SSL Gratuito)

```bash
# Instalar Certbot
apt install -y certbot python3-certbot-nginx

# Verificar
certbot --version
```

---

## 4. Configuração do Banco de Dados

### 4.1. Configurar PostgreSQL para Produção

```bash
# Acessar PostgreSQL
sudo -u postgres psql
```

Execute os seguintes comandos SQL:

```sql
-- Criar usuário com senha FORTE
CREATE USER primecare_user WITH PASSWORD 'SuaSenhaForte123!@#$';

-- Criar banco de dados principal
CREATE DATABASE primecare;

-- Criar banco de dados de telemedicina
CREATE DATABASE telemedicine;

-- Dar permissões
GRANT ALL PRIVILEGES ON DATABASE primecare TO primecare_user;
GRANT ALL PRIVILEGES ON DATABASE telemedicine TO primecare_user;

-- Configurar owner
ALTER DATABASE primecare OWNER TO primecare_user;
ALTER DATABASE telemedicine OWNER TO primecare_user;

-- Sair
\q
```

### 4.2. Configurar Acesso Remoto (Opcional, mas recomendado para backups)

```bash
# Editar configuração do PostgreSQL
nano /etc/postgresql/16/main/postgresql.conf
```

Altere a linha:
```
listen_addresses = 'localhost'
```

**🔒 Segurança**: Mantenha como 'localhost' a menos que precise de acesso externo

### 4.3. Otimizar PostgreSQL para Produção

```bash
# Editar configuração
nano /etc/postgresql/16/main/postgresql.conf
```

Adicione ou modifique:

```ini
# Otimizações para 8GB RAM
shared_buffers = 2GB
effective_cache_size = 6GB
maintenance_work_mem = 512MB
checkpoint_completion_target = 0.9
wal_buffers = 16MB
default_statistics_target = 100
random_page_cost = 1.1
effective_io_concurrency = 200
work_mem = 10MB
min_wal_size = 1GB
max_wal_size = 4GB
max_worker_processes = 4
max_parallel_workers_per_gather = 2
max_parallel_workers = 4
max_parallel_maintenance_workers = 2
```

```bash
# Reiniciar PostgreSQL
systemctl restart postgresql
```

---

## 5. Deploy das APIs

### 5.1. Clonar o Repositório

```bash
# Mudar para usuário deploy
su - deploy

# Clonar repositório
cd /var/www/primecare
git clone https://github.com/PrimeCareSoftware/MW.Code.git
cd MW.Code
```

### 5.2. Configurar Variáveis de Ambiente

```bash
# Criar arquivo .env para produção
nano /var/www/primecare/.env.production
```

Adicione o seguinte conteúdo:

```bash
# PostgreSQL
POSTGRES_PASSWORD=SuaSenhaForte123!@#$

# JWT (GERAR UMA CHAVE FORTE DE 64 CARACTERES)
JWT_SECRET_KEY=Gere-Uma-Chave-Super-Segura-De-Pelo-Menos-64-Caracteres-Aqui!

# Daily.co (Telemedicina)
DAILYCO_API_KEY=sua-chave-dailyco-aqui

# CORS Origins
CORS_ORIGIN_1=https://suaclinica.com.br
CORS_ORIGIN_2=https://admin.suaclinica.com.br
```

**🔒 Segurança**: Proteja este arquivo!
```bash
chmod 600 /var/www/primecare/.env.production
```

### 5.3. Deploy da API Principal

```bash
# Navegar para o projeto
cd /var/www/primecare/MW.Code/src/MedicSoft.Api

# Publicar para produção
dotnet publish -c Release -o /var/www/primecare/api

# Criar arquivo de configuração de produção
nano /var/www/primecare/api/appsettings.Production.json
```

Adicione:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=primecare;Username=primecare_user;Password=SuaSenhaForte123!@#$;Include Error Detail=false"
  },
  "JwtSettings": {
    "SecretKey": "Gere-Uma-Chave-Super-Segura-De-Pelo-Menos-64-Caracteres-Aqui!",
    "ExpiryMinutes": 60,
    "Issuer": "PrimeCare Software",
    "Audience": "PrimeCare-API"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://suaclinica.com.br",
      "https://admin.suaclinica.com.br",
      "https://api.suaclinica.com.br"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Security": {
    "RequireHttps": true,
    "MinPasswordLength": 12,
    "RequireStrongPasswords": true
  },
  "RateLimiting": {
    "EnableRateLimiting": true,
    "PermitLimit": 100,
    "WindowSeconds": 60
  }
}
```

**🔒 Proteger arquivo de configuração**:
```bash
chmod 600 /var/www/primecare/api/appsettings.Production.json
```

### 5.4. Deploy da API de Telemedicina

```bash
# Navegar para o projeto
cd /var/www/primecare/MW.Code/telemedicine/src/MedicSoft.Telemedicine.Api

# Publicar
dotnet publish -c Release -o /var/www/primecare/telemedicine-api

# Criar arquivo de configuração
nano /var/www/primecare/telemedicine-api/appsettings.Production.json
```

Adicione:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=telemedicine;Username=primecare_user;Password=SuaSenhaForte123!@#$;Include Error Detail=false"
  },
  "JwtSettings": {
    "SecretKey": "Gere-Uma-Chave-Super-Segura-De-Pelo-Menos-64-Caracteres-Aqui!",
    "ExpiryMinutes": 60,
    "Issuer": "PrimeCare Software",
    "Audience": "PrimeCare-API"
  },
  "DailyCo": {
    "ApiKey": "sua-chave-dailyco-aqui",
    "Domain": "primecare"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://suaclinica.com.br",
      "https://admin.suaclinica.com.br"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning"
    }
  }
}
```

**🔒 Proteger**:
```bash
chmod 600 /var/www/primecare/telemedicine-api/appsettings.Production.json
```

### 5.5. Executar Migrations

```bash
# API Principal
cd /var/www/primecare/api
ASPNETCORE_ENVIRONMENT=Production dotnet MedicSoft.Api.dll --migrate

# Telemedicina
cd /var/www/primecare/telemedicine-api
ASPNETCORE_ENVIRONMENT=Production dotnet MedicSoft.Telemedicine.Api.dll --migrate
```

### 5.6. Criar Serviços Systemd

```bash
# Voltar para root
exit

# Criar serviço da API Principal
nano /etc/systemd/system/primecare-api.service
```

Adicione:

```ini
[Unit]
Description=PrimeCare API Principal
After=network.target postgresql.service

[Service]
Type=notify
User=deploy
WorkingDirectory=/var/www/primecare/api
ExecStart=/usr/bin/dotnet /var/www/primecare/api/MedicSoft.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=primecare-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Criar serviço da API de Telemedicina
nano /etc/systemd/system/primecare-telemedicine.service
```

Adicione:

```ini
[Unit]
Description=PrimeCare Telemedicina API
After=network.target postgresql.service

[Service]
Type=notify
User=deploy
WorkingDirectory=/var/www/primecare/telemedicine-api
ExecStart=/usr/bin/dotnet /var/www/primecare/telemedicine-api/MedicSoft.Telemedicine.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=primecare-telemedicine
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Recarregar systemd
systemctl daemon-reload

# Habilitar e iniciar serviços
systemctl enable primecare-api
systemctl enable primecare-telemedicine
systemctl start primecare-api
systemctl start primecare-telemedicine

# Verificar status
systemctl status primecare-api
systemctl status primecare-telemedicine
```

### 5.7. Verificar que APIs estão rodando

```bash
# Testar API Principal
curl http://localhost:5000/health

# Testar Telemedicina
curl http://localhost:5084/health

# Ver logs em tempo real
journalctl -u primecare-api -f
journalctl -u primecare-telemedicine -f
```

---

## 6. Deploy dos Frontends

### 6.1. Build do Frontend Principal

```bash
# Mudar para usuário deploy
su - deploy
cd /var/www/primecare/MW.Code/frontend/medicwarehouse-app

# Instalar dependências
npm install

# Configurar URL da API para produção
nano src/environments/environment.prod.ts
```

Verifique/atualize:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.suaclinica.com.br',
  telemedicineApiUrl: 'https://tele.suaclinica.com.br'
};
```

```bash
# Build para produção
ng build --configuration production

# Copiar arquivos para diretório de deploy
mkdir -p /var/www/primecare/frontend/medicwarehouse-app
cp -r dist/medicwarehouse-app/browser/* /var/www/primecare/frontend/medicwarehouse-app/
```

### 6.2. Build do System Admin

```bash
cd /var/www/primecare/MW.Code/frontend/mw-system-admin

# Instalar dependências
npm install

# Configurar URL da API
nano src/environments/environment.prod.ts
```

Atualize:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.suaclinica.com.br'
};
```

```bash
# Build
ng build --configuration production

# Copiar
mkdir -p /var/www/primecare/frontend/mw-system-admin
cp -r dist/mw-system-admin/browser/* /var/www/primecare/frontend/mw-system-admin/
```

### 6.3. Configurar Permissões

```bash
# Voltar para root
exit

# Configurar permissões
chown -R deploy:www-data /var/www/primecare/frontend
chmod -R 755 /var/www/primecare/frontend
```

---

## 7. Configuração do Nginx

### 7.1. Remover Configuração Padrão

```bash
rm /etc/nginx/sites-enabled/default
```

### 7.2. Configurar API Principal

```bash
nano /etc/nginx/sites-available/api.suaclinica.com.br
```

Adicione:

```nginx
# API Principal - api.suaclinica.com.br
server {
    listen 80;
    server_name api.suaclinica.com.br;

    # Logs
    access_log /var/log/nginx/api-access.log;
    error_log /var/log/nginx/api-error.log;

    # Security Headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Referrer-Policy "strict-origin-when-cross-origin" always;

    # Rate Limiting
    limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;
    limit_req zone=api_limit burst=20 nodelay;

    # Proxy para API
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # SignalR WebSockets
    location /hubs/ {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### 7.3. Configurar API de Telemedicina

```bash
nano /etc/nginx/sites-available/tele.suaclinica.com.br
```

Adicione:

```nginx
# API de Telemedicina - tele.suaclinica.com.br
server {
    listen 80;
    server_name tele.suaclinica.com.br;

    access_log /var/log/nginx/tele-access.log;
    error_log /var/log/nginx/tele-error.log;

    # Security Headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # Rate Limiting
    limit_req_zone $binary_remote_addr zone=tele_limit:10m rate=5r/s;
    limit_req zone=tele_limit burst=10 nodelay;

    location / {
        proxy_pass http://localhost:5084;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }

    # WebRTC/SignalR
    location /hubs/ {
        proxy_pass http://localhost:5084;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### 7.4. Configurar Frontend Principal

```bash
nano /etc/nginx/sites-available/suaclinica.com.br
```

Adicione:

```nginx
# Frontend Principal - suaclinica.com.br
server {
    listen 80;
    server_name suaclinica.com.br www.suaclinica.com.br;

    root /var/www/primecare/frontend/medicwarehouse-app;
    index index.html;

    access_log /var/log/nginx/frontend-access.log;
    error_log /var/log/nginx/frontend-error.log;

    # Security Headers
    add_header X-Frame-Options "DENY" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Content-Security-Policy "default-src 'self' https:; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline';" always;

    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript application/x-javascript application/xml+rss application/javascript application/json;

    # Cache static assets
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # Angular routing
    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

### 7.5. Configurar System Admin

```bash
nano /etc/nginx/sites-available/admin.suaclinica.com.br
```

Adicione:

```nginx
# System Admin - admin.suaclinica.com.br
server {
    listen 80;
    server_name admin.suaclinica.com.br;

    root /var/www/primecare/frontend/mw-system-admin;
    index index.html;

    access_log /var/log/nginx/admin-access.log;
    error_log /var/log/nginx/admin-error.log;

    # Security Headers
    add_header X-Frame-Options "DENY" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # Gzip
    gzip on;
    gzip_types text/plain text/css text/xml text/javascript application/javascript application/json;

    # Cache
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # Angular routing
    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

### 7.6. Habilitar Sites e Testar

```bash
# Criar symlinks para habilitar sites
ln -s /etc/nginx/sites-available/api.suaclinica.com.br /etc/nginx/sites-enabled/
ln -s /etc/nginx/sites-available/tele.suaclinica.com.br /etc/nginx/sites-enabled/
ln -s /etc/nginx/sites-available/suaclinica.com.br /etc/nginx/sites-enabled/
ln -s /etc/nginx/sites-available/admin.suaclinica.com.br /etc/nginx/sites-enabled/

# Testar configuração
nginx -t

# Se OK, reiniciar Nginx
systemctl restart nginx
```

---

## 8. Configuração de Subdomínios na Hostinger

### 8.1. Acessar Painel da Hostinger

1. Entre em https://hpanel.hostinger.com
2. Vá em **"Domínios"** no menu lateral
3. Clique no seu domínio (ex: `suaclinica.com.br`)

### 8.2. Criar Registros DNS

Você precisa criar registros tipo **A** para cada subdomínio:

#### Passo a Passo:

1. No painel do domínio, clique em **"DNS / Name Servers"**
2. Role até **"DNS Records"**
3. Clique em **"Add Record"** ou **"Adicionar Registro"**

#### Criar os seguintes registros:

| Tipo | Nome | Aponta para | TTL |
|------|------|-------------|-----|
| A | @ | IP_DO_SEU_VPS | 3600 |
| A | www | IP_DO_SEU_VPS | 3600 |
| A | api | IP_DO_SEU_VPS | 3600 |
| A | tele | IP_DO_SEU_VPS | 3600 |
| A | admin | IP_DO_SEU_VPS | 3600 |

**Para cada registro:**
1. Selecione **Type: A**
2. Em **Name**, digite o subdomínio (ex: `api`, `tele`, `admin`)
3. Em **Points to** ou **Value**, digite o IP do seu VPS
4. Em **TTL**, deixe 3600 (1 hora)
5. Clique em **"Add Record"** ou **"Salvar"**

### 8.3. Verificar Propagação do DNS

A propagação do DNS pode levar de 15 minutos a 48 horas. Para verificar:

```bash
# No seu computador local, teste cada subdomínio:
nslookup api.suaclinica.com.br
nslookup tele.suaclinica.com.br
nslookup admin.suaclinica.com.br
nslookup suaclinica.com.br
```

**Alternativa online**: Use https://dnschecker.org

---

## 9. Configuração de SSL

### 9.1. Aguardar Propagação do DNS

**IMPORTANTE**: Antes de configurar SSL, certifique-se que:
- ✅ Os registros DNS estão configurados
- ✅ O DNS propagou (pode levar até 48h)
- ✅ Os domínios apontam para seu VPS

### 9.2. Obter Certificados SSL (Let's Encrypt)

```bash
# Certificado para o domínio principal e www
certbot --nginx -d suaclinica.com.br -d www.suaclinica.com.br

# Certificado para API
certbot --nginx -d api.suaclinica.com.br

# Certificado para Telemedicina
certbot --nginx -d tele.suaclinica.com.br

# Certificado para Admin
certbot --nginx -d admin.suaclinica.com.br
```

Para cada comando:
1. Digite seu email quando solicitado
2. Aceite os termos de serviço (Y)
3. Escolha se quer compartilhar email (N ou Y)
4. Escolha opção **2** (redirect para HTTPS)

### 9.3. Testar Renovação Automática

```bash
# Testar renovação (dry-run)
certbot renew --dry-run

# Se OK, o certbot renovará automaticamente via cron
```

### 9.4. Verificar SSL

Acesse cada URL no navegador:
- https://suaclinica.com.br
- https://api.suaclinica.com.br/swagger
- https://tele.suaclinica.com.br/swagger
- https://admin.suaclinica.com.br

Você deve ver o **cadeado verde** 🔒 no navegador.

---

## 10. Segurança para Dados Sensíveis

### 10.1. Configurações Essenciais de Segurança

#### ✅ Checklist de Segurança LGPD/HIPAA

- [ ] **Criptografia em Trânsito**: SSL/TLS (HTTPS) em todos os endpoints
- [ ] **Criptografia em Repouso**: Dados sensíveis criptografados no banco
- [ ] **Senhas Fortes**: Mínimo 12 caracteres, incluindo maiúsculas, minúsculas, números e símbolos
- [ ] **Autenticação Forte**: JWT com expiração de 60 minutos
- [ ] **Rate Limiting**: Proteção contra ataques de força bruta
- [ ] **Firewall**: Apenas portas 22, 80, 443 expostas
- [ ] **Logs de Auditoria**: Todos os acessos a dados sensíveis registrados
- [ ] **Backups Criptografados**: Backups diários com criptografia
- [ ] **Atualizações**: Sistema sempre atualizado
- [ ] **2FA**: Autenticação de dois fatores habilitada (quando disponível)

### 10.2. Hardening do PostgreSQL

```bash
# Editar pg_hba.conf
nano /etc/postgresql/16/main/pg_hba.conf
```

Certifique-se de que APENAS localhost pode acessar:

```
# TYPE  DATABASE        USER            ADDRESS                 METHOD
local   all             postgres                                peer
local   all             all                                     peer
host    all             all             127.0.0.1/32            scram-sha-256
host    all             all             ::1/128                 scram-sha-256
```

```bash
# Reiniciar PostgreSQL
systemctl restart postgresql
```

### 10.3. Configurar Fail2Ban (Proteção contra Ataques)

```bash
# Instalar Fail2Ban
apt install -y fail2ban

# Configurar
nano /etc/fail2ban/jail.local
```

Adicione:

```ini
[DEFAULT]
bantime = 3600
findtime = 600
maxretry = 5

[sshd]
enabled = true
port = 22
logpath = /var/log/auth.log

[nginx-http-auth]
enabled = true
filter = nginx-http-auth
port = http,https
logpath = /var/log/nginx/error.log

[nginx-limit-req]
enabled = true
filter = nginx-limit-req
port = http,https
logpath = /var/log/nginx/error.log
```

```bash
# Iniciar Fail2Ban
systemctl enable fail2ban
systemctl start fail2ban

# Verificar status
fail2ban-client status
```

### 10.4. Configurar Logs de Auditoria

```bash
# Criar diretório de logs
mkdir -p /var/log/primecare
chown deploy:deploy /var/log/primecare

# Configurar rotação de logs
nano /etc/logrotate.d/primecare
```

Adicione:

```
/var/log/primecare/*.log {
    daily
    rotate 90
    compress
    delaycompress
    notifempty
    create 0640 deploy deploy
    sharedscripts
    postrotate
        systemctl reload nginx > /dev/null 2>&1 || true
    endscript
}
```

### 10.5. Desabilitar Informações Sensíveis em Headers

```bash
# Editar configuração do Nginx
nano /etc/nginx/nginx.conf
```

Adicione dentro de `http {}`:

```nginx
# Ocultar versão do Nginx
server_tokens off;

# Ocultar informações do servidor
more_clear_headers 'Server';
more_clear_headers 'X-Powered-By';
```

### 10.6. Configurar TLS Seguro

```bash
# Editar configuração global do Nginx
nano /etc/nginx/nginx.conf
```

Adicione:

```nginx
# SSL Configuration
ssl_protocols TLSv1.2 TLSv1.3;
ssl_prefer_server_ciphers on;
ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384:DHE-RSA-AES256-GCM-SHA384;
ssl_session_cache shared:SSL:10m;
ssl_session_timeout 10m;
ssl_stapling on;
ssl_stapling_verify on;
```

### 10.7. Monitoramento de Segurança

```bash
# Instalar ferramentas de monitoramento
apt install -y aide rkhunter

# Inicializar AIDE (detecta mudanças em arquivos)
aide --init
mv /var/lib/aide/aide.db.new /var/lib/aide/aide.db

# Executar varredura de rootkits
rkhunter --update
rkhunter --check --skip-keypress
```

---

## 11. Backups e Monitoramento

### 11.1. Script de Backup Automatizado

```bash
# Criar script de backup
nano /var/www/primecare/scripts/backup.sh
```

Adicione:

```bash
#!/bin/bash

# Configurações
BACKUP_DIR="/var/backups/primecare"
DATE=$(date +%Y%m%d_%H%M%S)
RETENTION_DAYS=30

# Criar diretório se não existir
mkdir -p $BACKUP_DIR

# Backup PostgreSQL - Banco Principal
echo "Backing up primecare database..."
sudo -u postgres pg_dump primecare | gzip > $BACKUP_DIR/primecare_$DATE.sql.gz

# Backup PostgreSQL - Telemedicina
echo "Backing up telemedicine database..."
sudo -u postgres pg_dump telemedicine | gzip > $BACKUP_DIR/telemedicine_$DATE.sql.gz

# Backup de configurações
echo "Backing up configurations..."
tar -czf $BACKUP_DIR/configs_$DATE.tar.gz \
    /etc/nginx/sites-available \
    /etc/systemd/system/primecare-*.service \
    /var/www/primecare/api/appsettings.Production.json \
    /var/www/primecare/telemedicine-api/appsettings.Production.json

# Limpar backups antigos
echo "Cleaning old backups..."
find $BACKUP_DIR -name "*.sql.gz" -mtime +$RETENTION_DAYS -delete
find $BACKUP_DIR -name "*.tar.gz" -mtime +$RETENTION_DAYS -delete

echo "Backup completed: $DATE"
```

```bash
# Dar permissão de execução
chmod +x /var/www/primecare/scripts/backup.sh

# Testar backup
/var/www/primecare/scripts/backup.sh
```

### 11.2. Configurar Cron para Backups Diários

```bash
# Editar crontab
crontab -e
```

Adicione:

```bash
# Backup diário às 2h da manhã
0 2 * * * /var/www/primecare/scripts/backup.sh >> /var/log/primecare/backup.log 2>&1
```

### 11.3. Script de Monitoramento

```bash
nano /var/www/primecare/scripts/monitor.sh
```

Adicione:

```bash
#!/bin/bash

# Verificar serviços
echo "=== Checking Services ==="
systemctl is-active primecare-api && echo "✓ API Principal: OK" || echo "✗ API Principal: FAIL"
systemctl is-active primecare-telemedicine && echo "✓ Telemedicina: OK" || echo "✗ Telemedicina: FAIL"
systemctl is-active nginx && echo "✓ Nginx: OK" || echo "✗ Nginx: FAIL"
systemctl is-active postgresql && echo "✓ PostgreSQL: OK" || echo "✗ PostgreSQL: FAIL"

# Verificar uso de recursos
echo ""
echo "=== Resource Usage ==="
free -h | grep "Mem:"
df -h | grep "/$"

# Verificar portas
echo ""
echo "=== Port Check ==="
netstat -tulpn | grep ":5000 " > /dev/null && echo "✓ API (5000): Listening" || echo "✗ API (5000): Not listening"
netstat -tulpn | grep ":5084 " > /dev/null && echo "✓ Telemedicina (5084): Listening" || echo "✗ Telemedicina (5084): Not listening"
netstat -tulpn | grep ":80 " > /dev/null && echo "✓ Nginx HTTP (80): Listening" || echo "✗ Nginx HTTP (80): Not listening"
netstat -tulpn | grep ":443 " > /dev/null && echo "✓ Nginx HTTPS (443): Listening" || echo "✗ Nginx HTTPS (443): Not listening"

# Verificar certificados SSL
echo ""
echo "=== SSL Certificates ==="
certbot certificates 2>/dev/null | grep "Expiry Date" || echo "Run certbot certificates manually"
```

```bash
# Dar permissão
chmod +x /var/www/primecare/scripts/monitor.sh

# Testar
/var/www/primecare/scripts/monitor.sh
```

### 11.4. Configurar Alertas por Email (Opcional)

```bash
# Instalar mailutils
apt install -y mailutils

# Testar envio de email
echo "Test from PrimeCare Server" | mail -s "Test Alert" seu-email@exemplo.com
```

Para configurar alertas automáticos, adicione ao cron:

```bash
# Monitoramento a cada hora
0 * * * * /var/www/primecare/scripts/monitor.sh | mail -s "PrimeCare Status Report" seu-email@exemplo.com
```

---

## 12. Checklist Final

### ✅ Infraestrutura

- [ ] VPS Hostinger KVM 2 contratado e ativo
- [ ] Ubuntu 22.04 LTS instalado
- [ ] Swap de 4GB configurado
- [ ] Firewall (UFW) ativo permitindo apenas portas 22, 80, 443
- [ ] Domínio registrado e apontando para o VPS

### ✅ Software Instalado

- [ ] .NET 8 SDK instalado e funcionando
- [ ] PostgreSQL 16 instalado e rodando
- [ ] Node.js 18 e Angular CLI instalados
- [ ] Nginx instalado e rodando
- [ ] Certbot instalado

### ✅ Banco de Dados

- [ ] Banco `primecare` criado
- [ ] Banco `telemedicine` criado
- [ ] Usuário `primecare_user` criado com permissões
- [ ] Migrations executadas com sucesso
- [ ] PostgreSQL otimizado para produção

### ✅ APIs Deployadas

- [ ] API Principal publicada em `/var/www/primecare/api`
- [ ] API Telemedicina publicada em `/var/www/primecare/telemedicine-api`
- [ ] Arquivos appsettings.Production.json configurados
- [ ] Serviços systemd criados e ativos
- [ ] APIs respondendo em localhost:5000 e localhost:5084

### ✅ Frontends Deployados

- [ ] Frontend principal buildado e copiado
- [ ] System Admin buildado e copiado
- [ ] Arquivos servidos corretamente pelo Nginx

### ✅ Nginx Configurado

- [ ] Configurações criadas para todos os sites/APIs
- [ ] Proxy reverso funcionando para APIs
- [ ] Headers de segurança configurados
- [ ] Gzip e cache configurados
- [ ] Rate limiting ativo
- [ ] Nginx testado e rodando sem erros

### ✅ DNS e Subdomínios

- [ ] Registros A criados na Hostinger para:
  - [ ] @ (domínio raiz)
  - [ ] www
  - [ ] api
  - [ ] tele
  - [ ] admin
- [ ] DNS propagado (verificado com nslookup ou dnschecker.org)

### ✅ SSL/TLS

- [ ] Certificados SSL instalados para todos os domínios:
  - [ ] suaclinica.com.br
  - [ ] api.suaclinica.com.br
  - [ ] tele.suaclinica.com.br
  - [ ] admin.suaclinica.com.br
- [ ] Todos os sites acessíveis via HTTPS
- [ ] Renovação automática testada

### ✅ Segurança

- [ ] Senhas fortes configuradas (PostgreSQL, JWT, etc.)
- [ ] Portas 5000 e 5084 NÃO expostas externamente
- [ ] CORS configurado corretamente
- [ ] Firewall configurado (apenas 22, 80, 443)
- [ ] Fail2Ban instalado e ativo
- [ ] Headers de segurança configurados
- [ ] TLS 1.2+ apenas
- [ ] Logs de auditoria configurados
- [ ] Rotação de logs configurada

### ✅ Backups e Monitoramento

- [ ] Script de backup criado e testado
- [ ] Cron job de backup diário configurado
- [ ] Backups sendo salvos em `/var/backups/primecare`
- [ ] Script de monitoramento criado
- [ ] Alertas configurados (opcional)

### ✅ Testes Finais

- [ ] https://suaclinica.com.br acessível e funcionando
- [ ] https://api.suaclinica.com.br/swagger acessível
- [ ] https://tele.suaclinica.com.br/swagger acessível
- [ ] https://admin.suaclinica.com.br acessível
- [ ] Login no sistema funcionando
- [ ] Criação de consultas funcionando
- [ ] Telemedicina funcionando (criar sala de vídeo)
- [ ] APIs respondendo corretamente
- [ ] Sem erros nos logs

### ✅ Documentação

- [ ] IPs e senhas anotados em local seguro
- [ ] Procedimento de atualização documentado
- [ ] Contatos de suporte anotados
- [ ] Plano de disaster recovery criado

---

## 🆘 Troubleshooting

### API não inicia

```bash
# Ver logs completos
journalctl -u primecare-api -n 100 --no-pager
journalctl -u primecare-telemedicine -n 100 --no-pager

# Verificar se a porta está em uso
netstat -tulpn | grep 5000
netstat -tulpn | grep 5084

# Testar manualmente
cd /var/www/primecare/api
sudo -u deploy dotnet MedicSoft.Api.dll
```

### Frontend mostra página em branco

```bash
# Verificar arquivos
ls -la /var/www/primecare/frontend/medicwarehouse-app/

# Deve ter index.html e outros arquivos

# Ver logs do Nginx
tail -f /var/log/nginx/error.log

# Verificar permissões
namei -l /var/www/primecare/frontend/medicwarehouse-app/index.html
```

### Erro de conexão com banco

```bash
# Verificar PostgreSQL
systemctl status postgresql

# Testar conexão
sudo -u postgres psql -c "SELECT version();"

# Verificar usuário e banco
sudo -u postgres psql -c "\du"
sudo -u postgres psql -c "\l"

# Testar credenciais
psql -h localhost -U primecare_user -d primecare -c "SELECT 1;"
```

### CORS Error no Frontend

1. Verificar `appsettings.Production.json`
2. Certificar que os domínios estão corretos em `Cors.AllowedOrigins`
3. Reiniciar API: `systemctl restart primecare-api`
4. Limpar cache do navegador (Ctrl+Shift+Del)
5. Verificar que está usando HTTPS, não HTTP

### SSL não funciona

```bash
# Verificar certificados
certbot certificates

# Ver logs
tail -f /var/log/letsencrypt/letsencrypt.log

# Tentar novamente
certbot --nginx -d seu-dominio.com.br --force-renew
```

### DNS não propaga

- Aguarde até 48 horas
- Verifique em https://dnschecker.org
- Confirme que o IP está correto no painel Hostinger
- Tente limpar cache DNS local: `ipconfig /flushdns` (Windows) ou `sudo systemd-resolve --flush-caches` (Linux)

### Site lento

```bash
# Ver uso de recursos
htop

# Ver processos
ps aux | grep dotnet
ps aux | grep postgres

# Ver logs de erro
tail -f /var/log/nginx/error.log
journalctl -u primecare-api -f
```

### Disco cheio

```bash
# Ver uso de disco
df -h

# Limpar logs antigos
journalctl --vacuum-time=7d

# Limpar arquivos temporários
apt clean
apt autoclean

# Ver maiores arquivos/pastas
du -h --max-depth=1 / | sort -h
```

---

## 📞 Suporte

### Documentação Adicional

- **Guia Completo Hostinger**: [DEPLOY_HOSTINGER_GUIA_COMPLETO.md](system-admin/infrastructure/DEPLOY_HOSTINGER_GUIA_COMPLETO.md)
- **Início Rápido**: [DEPLOY_HOSTINGER_INICIO_RAPIDO.md](system-admin/infrastructure/DEPLOY_HOSTINGER_INICIO_RAPIDO.md)
- **README Principal**: [README.md](README.md)

### Ferramentas Úteis

- **Verificar SSL**: https://www.ssllabs.com/ssltest/
- **Verificar DNS**: https://dnschecker.org
- **Verificar Headers de Segurança**: https://securityheaders.com
- **Teste de Velocidade**: https://gtmetrix.com

---

## 🎉 Conclusão

Parabéns! Se você seguiu todos os passos, seu sistema PrimeCare está agora rodando em produção de forma **segura** e **escalável** no Hostinger.

### Próximos Passos

1. **Testar todas as funcionalidades** do sistema
2. **Treinar usuários** no uso da plataforma
3. **Monitorar logs** diariamente nos primeiros dias
4. **Configurar backups externos** (S3, Backblaze, etc.)
5. **Implementar CI/CD** para deploys automatizados
6. **Adicionar monitoramento avançado** (Grafana, Prometheus)
7. **Considerar CDN** (Cloudflare) para melhor performance global

### Custos Mensais Estimados

| Item | Custo |
|------|-------|
| VPS Hostinger KVM 2 | R$ 39,99 |
| Domínio .com.br | R$ 3,33 (R$ 40/ano) |
| SSL Let's Encrypt | R$ 0,00 (gratuito) |
| Daily.co (Telemedicina) | USD 0-99 (~R$ 0-500) |
| Backup externo (opcional) | R$ 10-30 |
| **TOTAL** | **R$ 53 - 573/mês** |

### Manutenção Recomendada

- **Diária**: Verificar logs e monitoramento
- **Semanal**: Verificar backups e espaço em disco
- **Mensal**: Atualizar sistema operacional e dependências
- **Trimestral**: Revisar segurança e performance
- **Semestral**: Testar restore de backups

---

**Última atualização**: Fevereiro 2026  
**Versão**: 1.0  
**Autor**: PrimeCare Software Team

🚀 **Bom deploy e sucesso com seu sistema!**
