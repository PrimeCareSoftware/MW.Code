# 🚀 Guia Rápido: Do Zero à Produção em 30 Minutos

## 📋 Objetivo

Colocar o MedicWarehouse em produção **com custo mínimo** ($5-20/mês) de forma rápida e segura.

## 🎯 O Que Você Vai Conseguir

- ✅ Backend .NET rodando com PostgreSQL
- ✅ Frontend Angular hospedado
- ✅ HTTPS automático (SSL)
- ✅ Backups automáticos
- ✅ Monitoramento básico
- ✅ **Custo: $5-20/mês** (suficiente para 10-50 clínicas pequenas)

## ⚡ Opção 1: Railway (Mais Rápido - Recomendado)

**Tempo: 30 minutos | Custo: $5-15/mês**

### Passo 1: Preparação (5 min)

```bash
# 1. Clone o repositório (se ainda não tem)
git clone https://github.com/MedicWarehouse/MW.Code.git
cd MW.Code

# 2. Gere uma chave JWT segura
# Linux/Mac:
openssl rand -base64 32

# Windows PowerShell:
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))

# Copie o resultado - você vai precisar!
```

### Passo 2: Deploy Backend (15 min)

1. **Criar conta Railway**: https://railway.app (login com GitHub)

2. **Criar projeto**:
   - Click "New Project"
   - "Deploy from GitHub repo"
   - Selecione `MW.Code`

3. **Adicionar PostgreSQL**:
   - Click "+ New"
   - "Database" → "PostgreSQL"
   - Railway conecta automaticamente!

4. **Configurar variáveis** (click no serviço API → Variables):
   ```
   ASPNETCORE_ENVIRONMENT=Production
   JWT_SECRET_KEY=cole-sua-chave-aqui
   ```

5. **Aguardar deploy** (~3-5 min)

6. **Copiar URL da API** (Settings → Networking → Generate Domain)

### Passo 3: Deploy Frontend (10 min)

1. **Criar conta Vercel**: https://vercel.com (login com GitHub)

2. **Importar projeto**:
   - "Add New" → "Project"
   - Selecione `MW.Code`

3. **Configurar**:
   ```
   Framework: Angular
   Root Directory: frontend/medicwarehouse-app
   Build Command: npm run build
   Output Directory: dist/medicwarehouse-app/browser
   ```

4. **Adicionar variável**:
   ```
   API_URL=https://sua-api.up.railway.app
   ```

5. **Deploy!** (~2 min)

6. **Atualizar CORS** no Railway:
   - Volte ao Railway
   - API → Variables → Adicionar:
     ```
     Cors__AllowedOrigins__0=https://seu-app.vercel.app
     ```

### ✅ Pronto! Seu sistema está no ar!

- Frontend: `https://seu-app.vercel.app`
- Backend: `https://sua-api.up.railway.app`
- Swagger: `https://sua-api.up.railway.app/swagger`

---

## 🔧 Opção 2: VPS (Mais Controle)

**Tempo: 1-2 horas | Custo: $5-10/mês**

### Passo 1: Criar VPS (10 min)

1. **Recomendação**: Hetzner CX21 (~$5/mês) ou DigitalOcean ($6/mês)
2. **OS**: Ubuntu 24.04 LTS
3. **SSH**: Conecte ao servidor

### Passo 2: Instalar Podman (5 min)

**Opção A: Podman (Recomendado - Gratuito e Open-Source)**
```bash
# Instalar Podman e Podman Compose
sudo apt update
sudo apt install -y podman podman-compose

# Verificar
podman --version
podman-compose --version
```

**Opção B: Docker (Alternativa)**
```bash
# Instalar Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

# Instalar Docker Compose
apt-get install docker-compose-plugin

# Verificar
docker --version
docker compose version
```

> **💡 Recomendação**: Use Podman para evitar custos de licenciamento Docker em produção.

### Passo 3: Preparar Aplicação (10 min)

```bash
# Clonar repositório
git clone https://github.com/MedicWarehouse/MW.Code.git
cd MW.Code

# Criar arquivo .env
nano .env
```

Cole no arquivo `.env`:
```bash
POSTGRES_PASSWORD=sua-senha-super-segura-aqui
JWT_SECRET_KEY=sua-chave-jwt-de-32-caracteres-aqui
ASPNETCORE_ENVIRONMENT=Production
API_URL=http://seu-ip-ou-dominio:5000
```

Salve: `Ctrl+X` → `Y` → `Enter`

### Passo 4: Iniciar Aplicação (5 min)

**Com Podman:**
```bash
# Build e start
podman-compose -f podman-compose.production.yml up -d

# Ver logs
podman-compose -f podman-compose.production.yml logs -f

# Aguarde ~5 minutos para tudo iniciar
```

**Com Docker (alternativa):**
```bash
# Build e start
docker-compose -f podman-compose.production.yml up -d

# Ver logs
docker-compose -f podman-compose.production.yml logs -f
```

### Passo 5: Configurar Nginx e SSL (30 min)

```bash
# Instalar Nginx e Certbot
apt-get update
apt-get install nginx certbot python3-certbot-nginx -y

# Criar configuração
nano /etc/nginx/sites-available/medicwarehouse
```

Cole:
```nginx
server {
    listen 80;
    server_name seu-dominio.com;

    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location / {
        proxy_pass http://localhost:4200;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
    }
}
```

```bash
# Ativar site
ln -s /etc/nginx/sites-available/medicwarehouse /etc/nginx/sites-enabled/
nginx -t
systemctl restart nginx

# Configurar SSL (substitua seu-dominio.com)
certbot --nginx -d seu-dominio.com
```

### ✅ Pronto! Acesse seu sistema:

- URL: `https://seu-dominio.com`
- API: `https://seu-dominio.com/api`
- Swagger: `https://seu-dominio.com/api/swagger`

---

## 🆓 Opção 3: Free Tier (Apenas Testes)

**Tempo: 45 min | Custo: $0/mês**

⚠️ **Atenção**: Backend "dorme" após 15min de inatividade. Não use para clientes reais!

### Setup Rápido

**1. Backend** - Render.com (Free):
- Criar conta: render.com
- New → Web Service → Connect GitHub
- Configurar build: `dotnet publish -c Release`
- Start: `dotnet src/MedicSoft.Api/bin/Release/net8.0/publish/MedicSoft.Api.dll`
- Adicionar variável: `JWT_SECRET_KEY`

**2. Database** - Neon.tech (Free):
- Criar conta: neon.tech
- Create Project → PostgreSQL
- Copiar connection string
- Adicionar no Render como `DATABASE_URL`

**3. Frontend** - Vercel (Free):
- Mesmo processo da Opção 1
- Totalmente grátis, sem limitações práticas

---

## 📊 Comparativo Final

| Característica | Railway | VPS | Free Tier |
|---------------|---------|-----|-----------|
| **Custo** | $5-15/mês | $5-10/mês | $0 |
| **Setup** | 30 min | 1-2h | 45 min |
| **Dificuldade** | Fácil | Média | Fácil |
| **SSL** | Automático | Manual | Automático |
| **Backups** | Automático | Manual | Manual |
| **Performance** | Excelente | Excelente | Regular |
| **Uptime** | 99.9% | Depende | 99% (com sleep) |
| **Para Produção** | ✅ Sim | ✅ Sim | ❌ Só demos |

---

## 🔒 Checklist de Segurança (Obrigatório!)

Antes de ir ao ar:

- [ ] JWT_SECRET_KEY forte (32+ caracteres aleatórios)
- [ ] POSTGRES_PASSWORD forte (12+ caracteres)
- [ ] HTTPS configurado (SSL/TLS)
- [ ] CORS configurado (apenas seus domínios)
- [ ] Backups configurados
- [ ] Monitoramento ativo
- [ ] Rate limiting ativado
- [ ] Senhas não commitadas no Git

---

## 🎯 Próximos Passos Após Deploy

1. **Testar tudo**:
   ```bash
   # Testar API
   curl https://sua-api/health
   
   # Testar frontend
   # Abra no navegador
   ```

2. **Criar primeiro usuário**:
   ```bash
   # Via Swagger ou Postman
   POST /api/data-seeder/seed-system-owner
   ```

3. **Configurar backups** (ver guia específico)

4. **Adicionar domínio próprio** (opcional):
   - Railway: Settings → Custom Domain
   - Vercel: Settings → Domains

5. **Monitorar custos**:
   - Railway: Dashboard → Usage
   - Configure alertas em $10/mês

---

## 🆘 Problemas Comuns

### "Migration failed"
```bash
# Aplicar migrations manualmente
railway run dotnet ef database update
```

### "CORS error"
```bash
# Verificar domínios no Railway
# Adicionar exato: https://seu-app.vercel.app
```

### "502 Bad Gateway"
```bash
# Ver logs
railway logs
# ou
docker compose logs -f api
```

### "Connection refused"
```bash
# Verificar DATABASE_URL
railway variables
```

---

## 📚 Documentação Completa

Consulte os guias detalhados:

- **[INFRA_PRODUCAO_BAIXO_CUSTO.md](INFRA_PRODUCAO_BAIXO_CUSTO.md)** - Guia completo de infraestrutura
- **[DEPLOY_RAILWAY_GUIDE.md](DEPLOY_RAILWAY_GUIDE.md)** - Passo a passo detalhado Railway
- **[MIGRACAO_POSTGRESQL.md](MIGRACAO_POSTGRESQL.md)** - Migração SQL Server → PostgreSQL
- **[README.md](README.md)** - Documentação geral do projeto

---

## 💡 Dicas de Ouro

1. **Comece com Railway**: É o mais fácil e barato
2. **Use PostgreSQL**: Economiza 90% vs SQL Server
3. **Monitore custos**: Configure alertas desde o início
4. **Faça backups**: Desde o dia 1
5. **Teste antes**: Use free tier para experimentar
6. **Cresça gradualmente**: Escale conforme necessidade

---

**Precisa de ajuda?** Abra uma issue no GitHub!

**Boa sorte com seu deploy! 🚀**
