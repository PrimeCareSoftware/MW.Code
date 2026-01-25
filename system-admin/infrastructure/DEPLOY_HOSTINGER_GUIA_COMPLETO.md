# 🚀 Guia Completo de Deploy no Hostinger - Para Iniciantes

## 📋 Visão Geral

Este guia foi criado especialmente para **iniciantes** e mostra **passo a passo** como fazer o deploy completo do PrimeCare Software no Hostinger.

**Tempo estimado**: 2-3 horas (primeira vez)  
**Custo inicial**: R$ 19,99 - R$ 59,99/mês (VPS)  
**Nível**: Iniciante (explicações detalhadas)  
**Suporta**: 10-50 clínicas pequenas

## 🎯 O que você vai aprender

- ✅ Como contratar e configurar um VPS na Hostinger
- ✅ Como instalar todos os componentes necessários (PostgreSQL, .NET, etc)
- ✅ Como fazer deploy do backend (API .NET)
- ✅ Como fazer deploy do frontend (Angular)
- ✅ Como configurar domínio e SSL (HTTPS)
- ✅ Como fazer backups e monitoramento
- ✅ Como resolver problemas comuns

## 📚 Índice

1. [Entendendo o que é VPS](#1-entendendo-o-que-é-vps)
2. [Contratando o VPS na Hostinger](#2-contratando-o-vps-na-hostinger)
3. [Primeiro Acesso e Configuração Inicial](#3-primeiro-acesso-e-configuração-inicial)
4. [Instalando Componentes Necessários](#4-instalando-componentes-necessários)
5. [Configurando o Banco de Dados PostgreSQL](#5-configurando-o-banco-de-dados-postgresql)
6. [Deploy do Backend (.NET API)](#6-deploy-do-backend-net-api)
7. [Deploy do Frontend (Angular)](#7-deploy-do-frontend-angular)
8. [Configurando Domínio e SSL](#8-configurando-domínio-e-ssl)
9. [Configurando Backups Automáticos](#9-configurando-backups-automáticos)
10. [Monitoramento e Manutenção](#10-monitoramento-e-manutenção)
11. [Troubleshooting - Resolvendo Problemas](#11-troubleshooting)

---

## 1. Entendendo o que é VPS

### O que é VPS?

**VPS** significa "Virtual Private Server" (Servidor Virtual Privado). É como ter um computador rodando 24/7 na internet onde você pode instalar seus programas e deixá-los acessíveis para todos.

**Diferenças:**
- **Hospedagem Compartilhada**: Várias pessoas compartilham o mesmo servidor (limitado)
- **VPS**: Você tem seu próprio servidor virtual (recomendado para aplicações .NET)
- **Servidor Dedicado**: Você tem uma máquina física inteira (caro, não necessário no início)

### Por que VPS para PrimeCare Software?

- ✅ Você tem controle total do servidor
- ✅ Pode instalar .NET 8, PostgreSQL e qualquer outra ferramenta
- ✅ Recursos dedicados (RAM, CPU, disco)
- ✅ Pode instalar certificado SSL
- ✅ Melhor performance que hospedagem compartilhada

---

## 2. Contratando o VPS na Hostinger

### Passo 1: Criar Conta na Hostinger

1. Acesse: https://www.hostinger.com.br/
2. Clique em **"VPS"** no menu superior
3. Escolha um plano:

| Plano | RAM | CPU | Disco | Preço/mês |
|-------|-----|-----|-------|-----------|
| **KVM 1** | 4GB | 2 vCPU | 50GB NVMe | ~R$ 19,99 |
| **KVM 2** | 8GB | 4 vCPU | 100GB NVMe | ~R$ 39,99 |
| **KVM 3** | 12GB | 6 vCPU | 150GB NVMe | ~R$ 59,99 |

**Recomendação para início:** KVM 1 ou KVM 2

### Passo 2: Finalizar Compra

1. Adicione ao carrinho
2. Escolha período (quanto mais longo, mais desconto)
3. Complete o pagamento
4. Aguarde o email de confirmação (5-15 minutos)

### Passo 3: Escolher Sistema Operacional

Após a compra, você receberá um email para configurar o VPS:

1. Entre no painel da Hostinger
2. Vá em **"VPS"** → Seu servidor
3. Clique em **"Configurar"**
4. Escolha: **Ubuntu 22.04 LTS** (mais estável e com suporte longo)
5. Defina uma senha **forte** para o usuário root
6. Clique em **"Concluir Configuração"**

**⚠️ IMPORTANTE**: Guarde esta senha em local seguro! Você vai precisar dela.

---

## 3. Primeiro Acesso e Configuração Inicial

### Passo 1: Acessar via SSH

**O que é SSH?** É uma forma segura de acessar e controlar seu servidor remotamente via terminal/linha de comando.

**No Windows:**
1. Abra o **PowerShell** ou **CMD**
2. Digite:
```bash
ssh root@seu-ip-do-vps
```

**No Mac/Linux:**
1. Abra o **Terminal**
2. Digite o mesmo comando acima

**Exemplo:**
```bash
ssh root@185.123.456.789
```

3. Digite "yes" quando perguntado sobre fingerprint
4. Digite a senha que você definiu
5. Você está dentro do servidor! 🎉

### Passo 2: Atualizar o Sistema

Execute estes comandos (copie e cole um por vez):

```bash
# Atualizar lista de pacotes
sudo apt update

# Instalar atualizações
sudo apt upgrade -y

# Instalar ferramentas básicas
sudo apt install -y curl wget git unzip nano
```

**O que cada comando faz:**
- `apt update`: Atualiza a lista de programas disponíveis
- `apt upgrade`: Instala as atualizações
- `apt install`: Instala programas necessários

### Passo 3: Criar Usuário Não-Root (Segurança)

Por segurança, não devemos usar o usuário root para tudo:

```bash
# Criar usuário (substitua 'primecare' pelo nome que quiser)
adduser primecare

# Adicionar ao grupo sudo (permissões administrativas)
usermod -aG sudo primecare

# Testar novo usuário
su - primecare

# Voltar ao root
exit
```

### Passo 4: Configurar Firewall

```bash
# Instalar firewall
sudo apt install -y ufw

# Permitir SSH (para não perder acesso!)
sudo ufw allow 22/tcp

# Permitir HTTP e HTTPS (para o site)
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Permitir porta da API (5000)
sudo ufw allow 5000/tcp

# Ativar firewall
sudo ufw enable

# Verificar status
sudo ufw status
```

**Explicação:**
- Porta 22: SSH (acesso remoto)
- Porta 80: HTTP (site sem criptografia)
- Porta 443: HTTPS (site com SSL)
- Porta 5000: API do PrimeCare

---

## 4. Instalando Componentes Necessários

### Passo 1: Instalar .NET 8 SDK

```bash
# Adicionar repositório da Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Atualizar e instalar .NET 8
sudo apt update
sudo apt install -y dotnet-sdk-8.0

# Verificar instalação
dotnet --version
# Deve mostrar: 8.0.x
```

### Passo 2: Instalar PostgreSQL 16

```bash
# Adicionar repositório do PostgreSQL
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -

# Instalar PostgreSQL
sudo apt update
sudo apt install -y postgresql-16 postgresql-contrib-16

# Verificar se está rodando
sudo systemctl status postgresql
# Deve mostrar "active (running)"
```

### Passo 3: Instalar Node.js 18+ (para build do frontend)

```bash
# Instalar Node.js via NodeSource
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs

# Verificar instalação
node --version
# Deve mostrar: v18.x.x

npm --version
# Deve mostrar: 9.x.x
```

### Passo 4: Instalar Nginx (servidor web)

```bash
# Instalar Nginx
sudo apt install -y nginx

# Iniciar Nginx
sudo systemctl start nginx
sudo systemctl enable nginx

# Verificar se está rodando
sudo systemctl status nginx
```

**Teste**: Abra seu navegador e acesse `http://seu-ip-do-vps`  
Você deve ver a página padrão do Nginx! 🎉

---

## 5. Configurando o Banco de Dados PostgreSQL

### Passo 1: Acessar PostgreSQL

```bash
# Mudar para usuário postgres
sudo -u postgres psql
```

Você verá um prompt assim: `postgres=#`

### Passo 2: Criar Banco e Usuário

Execute estes comandos SQL (dentro do psql):

```sql
-- Criar usuário
CREATE USER primecare WITH PASSWORD 'SuaSenhaSuperForte123!';

-- Criar banco de dados
CREATE DATABASE primecare;

-- Dar permissões ao usuário
GRANT ALL PRIVILEGES ON DATABASE primecare TO primecare;

-- Permitir criar schemas
ALTER DATABASE primecare OWNER TO primecare;

-- Sair do psql
\q
```

### Passo 3: Permitir Conexões Remotas (se necessário)

```bash
# Editar arquivo de configuração
sudo nano /etc/postgresql/16/main/postgresql.conf
```

**No nano:**
1. Procure a linha: `#listen_addresses = 'localhost'`
2. Remova o `#` e mude para: `listen_addresses = '*'`
3. Salve: `Ctrl + O`, Enter, `Ctrl + X`

```bash
# Editar arquivo de autenticação
sudo nano /etc/postgresql/16/main/pg_hba.conf
```

**Adicione no final:**
```
# IPv4 connections
host    all             all             0.0.0.0/0            scram-sha-256
```

**Reiniciar PostgreSQL:**
```bash
sudo systemctl restart postgresql
```

### Passo 4: Testar Conexão

```bash
# Testar localmente
psql -U primecare -d primecare -h localhost

# Se pedir senha, digite a que você criou
# Se conectar, está funcionando!
# Digite \q para sair
```

---

## 6. Deploy do Backend (.NET API)

### Passo 1: Clonar o Repositório

```bash
# Criar diretório para aplicações
sudo mkdir -p /var/www/primecare
sudo chown -R $USER:$USER /var/www/primecare

# Clonar repositório
cd /var/www/primecare
git clone https://github.com/PrimeCareSoftware/MW.Code.git
cd MW.Code
```

### Passo 2: Configurar Variáveis de Ambiente

```bash
# Criar arquivo .env
nano /var/www/primecare/MW.Code/.env
```

**Adicione estas variáveis:**
```bash
# Database
POSTGRES_PASSWORD=SuaSenhaSuperForte123!
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=primecare;Username=primecare;Password=SuaSenhaSuperForte123!

# JWT (gerar chave forte - ver comando abaixo)
JWT_SECRET_KEY=gere-uma-chave-aleatoria-de-32-caracteres-ou-mais

# Ambiente
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:5000

# CORS (adicionar seu domínio depois)
Cors__AllowedOrigins__0=http://seu-dominio.com
Cors__AllowedOrigins__1=https://seu-dominio.com
```

**Para gerar chave JWT forte:**
```bash
openssl rand -base64 32
```

**Copie o resultado e cole em `JWT_SECRET_KEY`**

Salve: `Ctrl + O`, Enter, `Ctrl + X`

### Passo 3: Configurar appsettings.Production.json

```bash
# Editar arquivo
nano /var/www/primecare/MW.Code/src/MedicSoft.Api/appsettings.Production.json
```

**Conteúdo (ajuste conforme necessário):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=primecare;Username=primecare;Password=SuaSenhaSuperForte123!"
  },
  "JwtSettings": {
    "SecretKey": "sua-chave-jwt-gerada",
    "ExpiryMinutes": 60,
    "Issuer": "PrimeCare Software",
    "Audience": "PrimeCare Software-API"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [
      "http://seu-dominio.com",
      "https://seu-dominio.com"
    ]
  }
}
```

### Passo 4: Build da Aplicação

```bash
# Ir para o diretório da API
cd /var/www/primecare/MW.Code

# Restaurar dependências
dotnet restore

# Build da aplicação
dotnet build -c Release

# Publicar
dotnet publish src/MedicSoft.Api/MedicSoft.Api.csproj -c Release -o /var/www/primecare/api
```

**O que cada comando faz:**
- `restore`: Baixa todas as bibliotecas necessárias
- `build`: Compila o código
- `publish`: Cria uma versão pronta para produção

### Passo 5: Aplicar Migrations (Criar Tabelas)

```bash
# Voltar para o diretório do código
cd /var/www/primecare/MW.Code

# Aplicar migrations
dotnet ef database update \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api \
  --context MedicSoftDbContext \
  --connection "Host=localhost;Port=5432;Database=primecare;Username=primecare;Password=SuaSenhaSuperForte123!"
```

**Se der erro**, instale o EF Core Tools:
```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
```

### Passo 6: Criar Serviço Systemd (Manter API Rodando)

```bash
# Criar arquivo de serviço
sudo nano /etc/systemd/system/primecare-api.service
```

**Adicione este conteúdo:**
```ini
[Unit]
Description=PrimeCare Software API
After=network.target postgresql.service

[Service]
WorkingDirectory=/var/www/primecare/api
ExecStart=/usr/bin/dotnet /var/www/primecare/api/MedicSoft.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=primecare-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=primecare;Username=primecare;Password=SuaSenhaSuperForte123!

[Install]
WantedBy=multi-user.target
```

**Ativar e iniciar serviço:**
```bash
# Recarregar systemd
sudo systemctl daemon-reload

# Ativar serviço (iniciar no boot)
sudo systemctl enable primecare-api

# Iniciar serviço
sudo systemctl start primecare-api

# Verificar status
sudo systemctl status primecare-api
```

**Deve mostrar**: "active (running)" ✅

### Passo 7: Testar API

```bash
# Testar localmente
curl http://localhost:5000/health

# Deve retornar algo como: {"status":"Healthy"}
```

**No navegador**: `http://seu-ip:5000/swagger`  
Você deve ver a documentação da API! 🎉

---

## 7. Deploy do Frontend (Angular)

### Passo 1: Build do Frontend

```bash
# Ir para diretório do frontend
cd /var/www/primecare/MW.Code/frontend/medicwarehouse-app

# Instalar dependências
npm install

# Configurar API URL
nano src/environments/environment.prod.ts
```

**Edite para:**
```typescript
export const environment = {
  production: true,
  apiUrl: 'http://seu-dominio.com:5000'  // ou https se tiver SSL
};
```

**Build para produção:**
```bash
npm run build -- --configuration=production
```

**Isso criará os arquivos em**: `dist/medicwarehouse-app/browser/`

### Passo 2: Mover para Diretório do Nginx

```bash
# Criar diretório
sudo mkdir -p /var/www/primecare/frontend

# Copiar arquivos
sudo cp -r dist/medicwarehouse-app/browser/* /var/www/primecare/frontend/

# Dar permissões
sudo chown -R www-data:www-data /var/www/primecare/frontend
```

### Passo 3: Configurar Nginx

```bash
# Criar configuração do site
sudo nano /etc/nginx/sites-available/primecare
```

**Adicione:**
```nginx
server {
    listen 80;
    server_name seu-dominio.com www.seu-dominio.com;

    # Frontend Angular
    location / {
        root /var/www/primecare/frontend;
        index index.html;
        try_files $uri $uri/ /index.html;
    }

    # API Backend
    location /api/ {
        proxy_pass http://localhost:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Swagger (opcional, remova em produção)
    location /swagger {
        proxy_pass http://localhost:5000/swagger;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
    }
}
```

**Ativar site:**
```bash
# Criar link simbólico
sudo ln -s /etc/nginx/sites-available/primecare /etc/nginx/sites-enabled/

# Remover site padrão
sudo rm /etc/nginx/sites-enabled/default

# Testar configuração
sudo nginx -t

# Se OK, reiniciar Nginx
sudo systemctl restart nginx
```

### Passo 4: Testar Frontend

Abra seu navegador e acesse: `http://seu-ip-do-vps`

Você deve ver o PrimeCare Software rodando! 🎉

---

## 8. Configurando Domínio e SSL

### Passo 1: Configurar Domínio

**Se você tem um domínio:**

1. Entre no painel do seu registrador de domínio (Registro.br, Hostinger, etc)
2. Vá em **DNS Settings** ou **Gerenciar DNS**
3. Adicione um registro **A**:
   - **Host**: @ (ou deixe em branco)
   - **Valor**: Seu IP do VPS
   - **TTL**: 3600 ou automático
4. Adicione um registro **A** para www:
   - **Host**: www
   - **Valor**: Seu IP do VPS
   - **TTL**: 3600 ou automático

**Aguarde 15-30 minutos** para propagação do DNS

### Passo 2: Instalar Certbot (SSL Grátis)

```bash
# Instalar Certbot
sudo apt install -y certbot python3-certbot-nginx

# Obter certificado SSL (substitua seu-dominio.com)
sudo certbot --nginx -d seu-dominio.com -d www.seu-dominio.com

# Seguir as instruções:
# 1. Digite seu email
# 2. Aceite os termos (Y)
# 3. Escolha se quer compartilhar email (N)
# 4. Redirecionar HTTP para HTTPS? (2 - Sim)
```

**Pronto!** Seu site agora tem SSL! 🔒

Acesse: `https://seu-dominio.com`

### Passo 3: Renovação Automática

```bash
# Testar renovação
sudo certbot renew --dry-run

# Se funcionar, o Certbot já está configurado para renovar automaticamente!
```

---

## 9. Configurando Backups Automáticos

### Passo 1: Criar Script de Backup

```bash
# Criar diretório para backups
sudo mkdir -p /var/backups/primecare

# Criar script
sudo nano /usr/local/bin/backup-primecare.sh
```

**Adicione:**
```bash
#!/bin/bash

# Configurações
BACKUP_DIR="/var/backups/primecare"
DATE=$(date +"%Y%m%d_%H%M%S")
DB_NAME="primecare"
DB_USER="primecare"
DB_PASS="SuaSenhaSuperForte123!"

# Criar backup do banco
echo "Iniciando backup..."
PGPASSWORD=$DB_PASS pg_dump -U $DB_USER -h localhost $DB_NAME | gzip > "$BACKUP_DIR/backup_$DATE.sql.gz"

# Manter apenas últimos 7 backups
find $BACKUP_DIR -name "backup_*.sql.gz" -type f -mtime +7 -delete

echo "Backup concluído: backup_$DATE.sql.gz"
```

**Dar permissão:**
```bash
sudo chmod +x /usr/local/bin/backup-primecare.sh

# Testar
sudo /usr/local/bin/backup-primecare.sh
```

### Passo 2: Agendar Backups Diários

```bash
# Editar crontab
sudo crontab -e
```

**Adicione no final:**
```bash
# Backup diário às 3h da manhã
0 3 * * * /usr/local/bin/backup-primecare.sh >> /var/log/primecare-backup.log 2>&1
```

Salve e saia.

**Backups serão criados automaticamente todo dia às 3h!** ✅

---

## 10. Monitoramento e Manutenção

### Ver Logs da API

```bash
# Ver logs em tempo real
sudo journalctl -u primecare-api -f

# Ver últimas 100 linhas
sudo journalctl -u primecare-api -n 100

# Ver logs de hoje
sudo journalctl -u primecare-api --since today
```

### Ver Logs do Nginx

```bash
# Logs de acesso
sudo tail -f /var/log/nginx/access.log

# Logs de erro
sudo tail -f /var/log/nginx/error.log
```

### Ver Status dos Serviços

```bash
# API
sudo systemctl status primecare-api

# PostgreSQL
sudo systemctl status postgresql

# Nginx
sudo systemctl status nginx
```

### Monitorar Recursos

```bash
# Ver uso de CPU e memória
htop

# Se não tiver htop, instale:
sudo apt install -y htop

# Ver uso de disco
df -h

# Ver processos usando mais memória
ps aux --sort=-%mem | head -10
```

### Atualizar Aplicação

```bash
# Ir para o diretório
cd /var/www/primecare/MW.Code

# Fazer backup antes!
sudo /usr/local/bin/backup-primecare.sh

# Puxar atualizações
git pull origin main

# Build novamente
dotnet publish src/MedicSoft.Api/MedicSoft.Api.csproj -c Release -o /var/www/primecare/api

# Aplicar novas migrations (se houver)
dotnet ef database update \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api

# Reiniciar API
sudo systemctl restart primecare-api

# Atualizar frontend
cd frontend/medicwarehouse-app
npm install
npm run build -- --configuration=production
sudo cp -r dist/medicwarehouse-app/browser/* /var/www/primecare/frontend/
sudo systemctl restart nginx
```

---

## 11. Troubleshooting - Resolvendo Problemas

### Problema: API não inicia

**Verificar logs:**
```bash
sudo journalctl -u primecare-api -n 50
```

**Causas comuns:**
- Connection string errada → Verificar em `appsettings.Production.json`
- Porta 5000 em uso → Mudar porta ou matar processo: `sudo lsof -ti:5000 | xargs sudo kill -9`
- Migrations não aplicadas → Rodar `dotnet ef database update`

### Problema: Não consigo acessar o site

**Verificar Nginx:**
```bash
sudo nginx -t
sudo systemctl status nginx
sudo systemctl restart nginx
```

**Verificar firewall:**
```bash
sudo ufw status
# Se porta 80/443 não estiver aberta:
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
```

### Problema: SSL não funciona

**Renovar certificado:**
```bash
sudo certbot renew --force-renewal
sudo systemctl restart nginx
```

### Problema: Banco de dados não conecta

**Verificar se PostgreSQL está rodando:**
```bash
sudo systemctl status postgresql
sudo systemctl start postgresql
```

**Testar conexão:**
```bash
psql -U primecare -d primecare -h localhost
```

### Problema: Frontend não carrega dados

**Verificar configuração da API URL:**
```bash
# Verificar environment.prod.ts
cat /var/www/primecare/MW.Code/frontend/medicwarehouse-app/src/environments/environment.prod.ts
```

**Verificar CORS:**
- Domínio do frontend deve estar em `Cors__AllowedOrigins` no backend

### Problema: Memória cheia

**Ver uso:**
```bash
free -h
```

**Limpar cache:**
```bash
sudo sync; echo 3 | sudo tee /proc/sys/vm/drop_caches
```

**Adicionar swap (memória virtual):**
```bash
sudo fallocate -l 2G /swapfile
sudo chmod 600 /swapfile
sudo mkswap /swapfile
sudo swapon /swapfile
echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab
```

---

## 📊 Checklist Final

Antes de considerar o deploy concluído, verifique:

- [ ] PostgreSQL instalado e rodando
- [ ] .NET 8 instalado e funcionando
- [ ] Backend compilado e publicado
- [ ] Migrations aplicadas
- [ ] API rodando como serviço (systemd)
- [ ] API acessível na porta 5000
- [ ] Frontend buildado
- [ ] Nginx instalado e configurado
- [ ] Frontend acessível via navegador
- [ ] Domínio configurado (DNS)
- [ ] SSL instalado e funcionando (HTTPS)
- [ ] Firewall configurado
- [ ] Backups automáticos configurados
- [ ] Logs verificados e funcionando

---

## 🎓 Conceitos Importantes para Iniciantes

### O que é uma API?

API (Application Programming Interface) é a parte do sistema que recebe e processa requisições. No PrimeCare, a API .NET:
- Processa login
- Gerencia pacientes
- Salva dados no banco
- Envia respostas para o frontend

### O que é Frontend?

Frontend é a interface visual que o usuário vê e interage (o site). No PrimeCare, é feito em Angular.

### O que é um Proxy Reverso?

Nginx age como intermediário:
- Recebe requisições do navegador
- Serve arquivos estáticos (HTML, CSS, JS)
- Encaminha requisições da API para o backend .NET
- Gerencia SSL/HTTPS

### Fluxo de uma Requisição

```
Usuário → Navegador → Nginx → Backend API → PostgreSQL
                                     ↓
Usuário ← Navegador ← Nginx ← Resposta JSON
```

---

## 💰 Estimativa de Custos Mensal

| Item | Custo |
|------|-------|
| VPS Hostinger KVM 1 (4GB) | R$ 19,99 - R$ 39,99 |
| Domínio (.com.br) | R$ 40/ano (~R$ 3,33/mês) |
| SSL Certbot | R$ 0 (grátis) |
| **Total** | **~R$ 23 - R$ 43/mês** |

**Suporta:** 10-30 clínicas pequenas

---

## 📚 Próximos Passos

Depois do deploy básico funcionando:

1. **Configurar Email** (SMTP para notificações)
2. **Configurar Backup Remoto** (enviar para outro servidor/cloud)
3. **Configurar Monitoramento** (Uptime Robot, New Relic)
4. **Otimizar Performance** (cache, CDN)
5. **Documentar Processos** para sua equipe

---

## 🆘 Precisa de Ajuda?

- **Documentação Oficial**: Ver outros arquivos em `/docs`
- **Logs**: Sempre verificar logs quando algo não funciona
- **Comunidade**: GitHub Issues do projeto
- **Hostinger**: Suporte via ticket/chat

---

## ✅ Conclusão

Parabéns! Se você seguiu todos os passos, agora você tem:

- ✅ Um VPS configurado e seguro
- ✅ PostgreSQL rodando e configurado
- ✅ Backend .NET em produção
- ✅ Frontend Angular acessível
- ✅ Domínio com SSL (HTTPS)
- ✅ Backups automáticos
- ✅ Sistema pronto para receber clientes!

**Dica Final**: Documente tudo que você fez! Quando precisar fazer de novo ou ensinar alguém, você vai agradecer ter anotado os passos.

---

**Criado por**: GitHub Copilot  
**Versão**: 1.0  
**Data**: Janeiro 2025  
**Para**: Usuários iniciantes que querem usar Hostinger
