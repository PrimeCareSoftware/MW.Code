# 🚀 Hostinger - Guia de Início Rápido (30 minutos)

> **Para iniciantes**: Este guia resume os passos essenciais para colocar o Omni Care no ar rapidamente.  
> **Para guia detalhado**: Veja [DEPLOY_HOSTINGER_GUIA_COMPLETO.md](DEPLOY_HOSTINGER_GUIA_COMPLETO.md)

## 📋 Pré-requisitos

- ✅ Conta na Hostinger (criar em: https://hostinger.com.br)
- ✅ VPS contratado (KVM 1 ou superior)
- ✅ Sistema operacional: Ubuntu 22.04 LTS
- ✅ 30-60 minutos de tempo disponível

## 🎯 Visão Geral - O que vamos fazer

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  Contratar  │ -> │  Configurar │ -> │   Instalar  │
│  VPS + SSH  │    │  Segurança  │    │ Componentes │
└─────────────┘    └─────────────┘    └─────────────┘
                                              |
                                              v
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Testar!   │ <- │   Deploy    │ <- │  Configurar │
│             │    │  Backend +  │    │  PostgreSQL │
│             │    │  Frontend   │    │             │
└─────────────┘    └─────────────┘    └─────────────┘
```

---

## Etapa 1: Contratação e Acesso (5 min) ⏱️

### 1.1 Contratar VPS

1. Acesse: https://www.hostinger.com.br/vps
2. Escolha: **KVM 1** (4GB RAM) - R$ 19,99/mês
3. Complete o pagamento
4. Escolha: **Ubuntu 22.04 LTS**
5. Defina senha forte para root
6. Aguarde confirmação por email

### 1.2 Primeiro Acesso

```bash
# Conectar via SSH (substitua pelo seu IP)
ssh root@185.123.456.789

# Atualizar sistema
apt update && apt upgrade -y
```

---

## Etapa 2: Instalação Rápida (10 min) ⏱️

### 2.1 Instalar Tudo de Uma Vez

```bash
# Copie e cole este bloco inteiro:

# Ferramentas básicas
apt install -y curl wget git unzip nano ufw

# Firewall
ufw allow 22/tcp && ufw allow 80/tcp && ufw allow 443/tcp && ufw allow 5000/tcp
ufw --force enable

# .NET 8
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
apt update && apt install -y dotnet-sdk-8.0

# PostgreSQL 16
sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add -
apt update && apt install -y postgresql-16

# Node.js 18
curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
apt install -y nodejs

# Nginx
apt install -y nginx
systemctl start nginx && systemctl enable nginx

echo "✅ Instalação concluída!"
```

### 2.2 Verificar Instalações

```bash
# Deve mostrar versões corretas:
dotnet --version  # 8.0.x
node --version    # v18.x
psql --version    # 16.x
nginx -v          # 1.x
```

---

## Etapa 3: Configurar Banco de Dados (5 min) ⏱️

```bash
# Acessar PostgreSQL
sudo -u postgres psql

# Execute estes comandos SQL:
CREATE USER primecare WITH PASSWORD 'MinhaSenh@123!';
CREATE DATABASE primecare;
GRANT ALL PRIVILEGES ON DATABASE primecare TO primecare;
ALTER DATABASE primecare OWNER TO primecare;
\q

# Testar conexão
psql -U primecare -d primecare -h localhost
# Digite a senha quando solicitado
# Se conectar, está OK! Digite \q para sair
```

---

## Etapa 4: Deploy do Backend (8 min) ⏱️

### 4.1 Clonar e Configurar

```bash
# Criar diretórios
mkdir -p /var/www/primecare
cd /var/www/primecare

# Clonar código
git clone https://github.com/Omni CareSoftware/MW.Code.git
cd MW.Code

# Gerar chave JWT forte
JWT_KEY=$(openssl rand -base64 32)
echo "Sua chave JWT: $JWT_KEY"
# COPIE E GUARDE ESTA CHAVE!
```

### 4.2 Configurar appsettings

```bash
# Editar configuração
nano src/MedicSoft.Api/appsettings.Production.json
```

**Cole isto (AJUSTE a senha e JWT_KEY):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=primecare;Username=primecare;Password=MinhaSenh@123!"
  },
  "JwtSettings": {
    "SecretKey": "COLE_SUA_CHAVE_JWT_AQUI",
    "ExpiryMinutes": 60,
    "Issuer": "Omni Care Software",
    "Audience": "Omni Care Software-API"
  },
  "AllowedHosts": "*"
}
```

Salvar: `Ctrl+O`, Enter, `Ctrl+X`

### 4.3 Build e Deploy

```bash
# Build
cd /var/www/primecare/MW.Code
dotnet restore
dotnet publish src/MedicSoft.Api/MedicSoft.Api.csproj -c Release -o /var/www/primecare/api

# Aplicar Migrations
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"

dotnet ef database update \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api

# Criar serviço systemd
cat > /etc/systemd/system/omnicare-api.service << 'EOF'
[Unit]
Description=Omni Care API
After=network.target

[Service]
WorkingDirectory=/var/www/primecare/api
ExecStart=/usr/bin/dotnet /var/www/primecare/api/MedicSoft.Api.dll
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
EOF

# Iniciar serviço
systemctl daemon-reload
systemctl enable omnicare-api
systemctl start omnicare-api

# Verificar se está rodando
systemctl status omnicare-api
```

**✅ Se mostrar "active (running)", backend está OK!**

---

## Etapa 5: Deploy do Frontend (7 min) ⏱️

### 5.1 Build Frontend

```bash
cd /var/www/primecare/MW.Code/frontend/medicwarehouse-app

# Configurar API URL
nano src/environments/environment.prod.ts
```

**Cole:**
```typescript
export const environment = {
  production: true,
  apiUrl: 'http://localhost:5000'  // Mudar depois para seu domínio
};
```

**Build:**
```bash
npm install
npm run build -- --configuration=production

# Copiar para pasta web
mkdir -p /var/www/primecare/frontend
cp -r dist/medicwarehouse-app/browser/* /var/www/primecare/frontend/
chown -R www-data:www-data /var/www/primecare/frontend
```

### 5.2 Configurar Nginx

```bash
# Criar configuração
cat > /etc/nginx/sites-available/primecare << 'EOF'
server {
    listen 80;
    server_name _;

    location / {
        root /var/www/primecare/frontend;
        index index.html;
        try_files $uri $uri/ /index.html;
    }

    location /api/ {
        proxy_pass http://localhost:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
EOF

# Ativar site
ln -s /etc/nginx/sites-available/primecare /etc/nginx/sites-enabled/
rm -f /etc/nginx/sites-enabled/default
nginx -t
systemctl restart nginx
```

---

## Etapa 6: Testar! (5 min) ⏱️

### 6.1 Testes Básicos

```bash
# Testar API
curl http://localhost:5000/health
# Deve retornar: {"status":"Healthy"}

# Verificar serviços
systemctl status omnicare-api nginx postgresql
# Todos devem estar "active (running)"
```

### 6.2 Teste no Navegador

1. Abra: `http://SEU-IP-DO-VPS`
2. Você deve ver o Omni Care Software! 🎉

3. Abra: `http://SEU-IP-DO-VPS/swagger`
4. Você deve ver a documentação da API!

---

## ✅ Checklist Rápido

```
□ VPS contratado e acessível via SSH
□ Ubuntu 22.04 instalado
□ Firewall configurado
□ .NET 8 instalado
□ PostgreSQL instalado e rodando
□ Node.js instalado
□ Nginx instalado e rodando
□ Banco de dados criado
□ Backend compilado e rodando
□ Frontend buildado e acessível
□ API respondendo em /health
□ Site abrindo no navegador
```

---

## 🔧 Comandos Úteis

### Ver Logs

```bash
# Logs da API
journalctl -u omnicare-api -f

# Logs do Nginx
tail -f /var/log/nginx/error.log

# Logs do PostgreSQL
tail -f /var/log/postgresql/postgresql-16-main.log
```

### Reiniciar Serviços

```bash
systemctl restart omnicare-api
systemctl restart nginx
systemctl restart postgresql
```

### Status dos Serviços

```bash
systemctl status omnicare-api
systemctl status nginx
systemctl status postgresql
```

---

## 🆘 Problemas Comuns

### API não inicia

```bash
# Ver erro
journalctl -u omnicare-api -n 50

# Verificar connection string
nano /var/www/primecare/MW.Code/src/MedicSoft.Api/appsettings.Production.json
```

### Site não abre

```bash
# Verificar Nginx
nginx -t
systemctl restart nginx

# Verificar firewall
ufw status
```

### Banco não conecta

```bash
# Verificar se está rodando
systemctl status postgresql

# Testar conexão
psql -U primecare -d primecare -h localhost
```

---

## 📱 Próximos Passos

Após tudo funcionando:

1. **Configurar Domínio** (ver guia completo)
2. **Instalar SSL/HTTPS** (ver guia completo)
3. **Configurar Backups** (ver guia completo)
4. **Monitoramento** (logs, alertas)

---

## 📚 Documentação Completa

Para instruções detalhadas e explicações:
- **[DEPLOY_HOSTINGER_GUIA_COMPLETO.md](DEPLOY_HOSTINGER_GUIA_COMPLETO.md)** - Guia passo a passo completo com explicações

Para outras opções de infraestrutura:
- **[INFRA_DOCS_INDEX.md](INFRA_DOCS_INDEX.md)** - Índice de toda documentação de infraestrutura
- **[DEPLOY_RAILWAY_GUIDE.md](DEPLOY_RAILWAY_GUIDE.md)** - Deploy no Railway (alternativa mais simples)
- **[INFRA_PRODUCAO_BAIXO_CUSTO.md](INFRA_PRODUCAO_BAIXO_CUSTO.md)** - Comparação de custos

---

## 💰 Custo Mensal Estimado

| Item | Valor |
|------|-------|
| VPS Hostinger KVM 1 | R$ 19,99/mês |
| Domínio .com.br | R$ 3,33/mês |
| SSL (Let's Encrypt) | R$ 0 (grátis) |
| **Total** | **~R$ 23/mês** |

---

**🎉 Parabéns!** Se você chegou até aqui, seu sistema está no ar!

Agora você pode:
- Criar usuários
- Cadastrar pacientes
- Agendar consultas
- Testar todas as funcionalidades

**Dica**: Sempre faça backups antes de fazer atualizações!

---

**Criado por**: GitHub Copilot  
**Versão**: 1.0  
**Data**: Janeiro 2025
