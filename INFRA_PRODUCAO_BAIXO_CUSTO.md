# 🏗️ Guia de Infraestrutura de Produção com Baixo Custo

## 📋 Visão Geral

Este guia apresenta estratégias para colocar o MedicWarehouse em produção com **custo mínimo** enquanto você ainda não tem clientes grandes, permitindo crescimento gradual conforme a demanda aumenta.

## 💰 Comparativo de Custos Mensais

### Opção 1: Plataforma PaaS Completa (Recomendada para Início) 💚

| Serviço | Plataforma | Custo | Recursos |
|---------|-----------|-------|----------|
| **Backend + DB** | Railway | $5-20 | 512MB RAM, PostgreSQL incluído |
| **Frontend** | Vercel/Netlify | $0 | CDN global, SSL automático |
| **Total** | - | **$5-20/mês** | Pronto para até 10-20 clínicas |

### Opção 2: VPS Tradicional (Para Quem Prefere Controle)

| Serviço | Plataforma | Custo | Recursos |
|---------|-----------|-------|----------|
| **VPS** | Hetzner/DigitalOcean | $5-10 | 2GB RAM, 1 vCPU |
| **Total** | - | **$5-10/mês** | Requer configuração manual |

### Opção 3: Free Tier (Apenas para Testes) ⚠️

| Serviço | Plataforma | Custo | Limitações |
|---------|-----------|-------|----------|
| **Backend** | Render Free | $0 | Sleep após 15min inatividade |
| **Database** | Neon Free | $0 | 0.5GB PostgreSQL |
| **Frontend** | Vercel Free | $0 | Sem limitações práticas |
| **Total** | - | **$0/mês** | Apenas para MVP/Demonstração |

## 🚀 Opção 1: Railway (Mais Simples e Recomendada)

### Por que Railway?

- ✅ **Setup em 5 minutos**: Deploy automático via GitHub
- ✅ **PostgreSQL incluído**: Sem custos adicionais de banco
- ✅ **$5/mês**: Créditos incluídos + pay-as-you-go
- ✅ **SSL automático**: HTTPS grátis
- ✅ **Backups**: Backups automáticos do banco
- ✅ **Escalabilidade**: Cresce conforme necessidade
- ✅ **Monitoramento**: Logs e métricas integrados

### Passo a Passo: Deploy no Railway

#### 1. Preparação do Projeto

**a) Migrar de SQL Server para PostgreSQL**

SQL Server é caro em produção. PostgreSQL é gratuito e muito usado em PaaS.

```bash
# 1. Instalar provider do PostgreSQL
cd src/MedicSoft.Repository
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# 2. Atualizar DbContext para suportar ambos
# Ver seção "Configuração para PostgreSQL" abaixo
```

**b) Atualizar `appsettings.Production.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DATABASE_URL}"
  }
}
```

#### 2. Deploy no Railway

1. **Criar conta**: https://railway.app (login com GitHub)

2. **Criar novo projeto**: 
   - Click em "New Project"
   - Selecione "Deploy from GitHub repo"
   - Escolha `MedicWarehouse/MW.Code`

3. **Adicionar PostgreSQL**:
   - Click em "+ New"
   - Selecione "Database" → "PostgreSQL"
   - Railway criará e conectará automaticamente

4. **Configurar Variáveis de Ambiente**:
   ```bash
   # Estas serão auto-configuradas pelo Railway:
   DATABASE_URL=postgresql://... (automático)
   
   # Você precisa adicionar:
   ASPNETCORE_ENVIRONMENT=Production
   JWT_SECRET_KEY=sua-chave-super-segura-minimo-32-caracteres
   ```

5. **Deploy automático**:
   - Railway detecta .NET e faz build automático
   - Em ~3-5 minutos seu backend estará no ar!

6. **Obter URL da API**:
   - Railway fornece URL tipo: `medicwarehouse.railway.app`
   - Configure SSL (já vem automático)

#### 3. Deploy do Frontend (Vercel)

1. **Criar conta**: https://vercel.com (login com GitHub)

2. **Importar projeto**:
   - "Add New" → "Project"
   - Selecione o repositório
   - Configure:
     ```
     Framework Preset: Angular
     Root Directory: frontend/medicwarehouse-app
     Build Command: npm run build
     Output Directory: dist/medicwarehouse-app/browser
     ```

3. **Variáveis de ambiente**:
   ```bash
   API_URL=https://medicwarehouse.railway.app
   ```

4. **Deploy**: Vercel faz deploy em ~2 minutos

### Custos Estimados Railway

```
Uso Estimado para 10-20 clínicas pequenas:
- Backend: 512MB RAM, ~50-100 requisições/dia = $5-10/mês
- PostgreSQL: 1GB storage = Incluído
- Egress: ~10GB = $1-2/mês

Total: $5-15/mês
```

### Escalabilidade Railway

```
10-20 clínicas     → $5-15/mês    (Plano Hobby: 512MB)
20-50 clínicas     → $15-30/mês   (Plano Hobby: 1GB)
50-100 clínicas    → $30-60/mês   (Plano Pro: 2GB)
100-500 clínicas   → $100-300/mês (Plano Pro: 4-8GB)
```

## 🔧 Opção 2: VPS Tradicional (Hetzner/DigitalOcean)

### Por que VPS?

- ✅ **Controle total**: Você gerencia tudo
- ✅ **Previsível**: Custo fixo mensal
- ✅ **Barato**: $5-10/mês para início
- ❌ **Mais trabalho**: Você configura tudo
- ❌ **Requer conhecimento**: Linux, Docker, Nginx

### Recomendação: Hetzner (Melhor custo/benefício da Europa)

```
Hetzner CX21:
- 2 vCPU
- 4GB RAM
- 40GB SSD
- 20TB tráfego
= €4.51/mês (~$5 USD)

Suporta: 20-50 clínicas pequenas
```

### Passo a Passo: VPS com Docker

#### 1. Criar VPS

```bash
# 1. Criar conta na Hetzner: https://www.hetzner.com
# 2. Criar servidor CX21 (Ubuntu 24.04)
# 3. Conectar via SSH
ssh root@seu-ip
```

#### 2. Instalar Docker

```bash
# Instalar Docker e Docker Compose
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh
apt-get install docker-compose-plugin

# Verificar instalação
docker --version
docker compose version
```

#### 3. Preparar Aplicação

```bash
# Clonar repositório
git clone https://github.com/MedicWarehouse/MW.Code.git
cd MW.Code

# Criar arquivo .env para produção
cat > .env.production << EOF
POSTGRES_PASSWORD=sua-senha-super-segura
DATABASE_URL=postgresql://medicwarehouse:sua-senha-super-segura@postgres:5432/medicwarehouse
JWT_SECRET_KEY=sua-chave-jwt-super-segura-minimo-32-caracteres
ASPNETCORE_ENVIRONMENT=Production
EOF
```

#### 4. Usar Docker Compose Otimizado

Use o arquivo `docker-compose.production.yml` (criado neste PR)

```bash
# Build e start
docker compose -f docker-compose.production.yml up -d

# Ver logs
docker compose -f docker-compose.production.yml logs -f

# Verificar status
docker compose -f docker-compose.production.yml ps
```

#### 5. Configurar Nginx e SSL

```bash
# Instalar Nginx
apt-get install nginx certbot python3-certbot-nginx

# Configurar Nginx
nano /etc/nginx/sites-available/medicwarehouse
```

Conteúdo do arquivo:
```nginx
server {
    listen 80;
    server_name seu-dominio.com;

    # Backend API
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Frontend
    location / {
        proxy_pass http://localhost:4200;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

```bash
# Ativar site
ln -s /etc/nginx/sites-available/medicwarehouse /etc/nginx/sites-enabled/
nginx -t
systemctl restart nginx

# Configurar SSL (Certbot)
certbot --nginx -d seu-dominio.com
```

#### 6. Monitoramento Básico

```bash
# Ver uso de recursos
docker stats

# Ver logs
docker compose -f docker-compose.production.yml logs --tail=100 -f api

# Backup automático do banco
# Ver seção "Backups" abaixo
```

## 🆓 Opção 3: Free Tier (Apenas MVP/Demonstração)

### ⚠️ Limitações Importantes

- Backend "dorme" após 15 minutos de inatividade
- Primeira requisição pode demorar 30-60 segundos
- **Não use para clientes reais pagantes**
- Ideal apenas para demonstrações e testes

### Plataformas Free

1. **Backend**: Render.com (Free tier)
   - 512MB RAM
   - Sleep após 15min inatividade
   - 750 horas/mês grátis

2. **Database**: Neon.tech (Free tier)
   - 0.5GB PostgreSQL
   - Sleep após 5 min inatividade
   - 1 projeto grátis

3. **Frontend**: Vercel (Free tier)
   - CDN global
   - SSL automático
   - Sem limites práticos

### Setup Rápido Free Tier

**1. Backend no Render**:
```
1. Criar conta: render.com
2. New → Web Service → Connect GitHub
3. Configurar:
   - Build: dotnet publish -c Release
   - Start: dotnet MedicSoft.Api.dll
   - Plano: Free
```

**2. Database no Neon**:
```
1. Criar conta: neon.tech
2. Create Project → PostgreSQL
3. Copiar connection string
4. Adicionar no Render como variável DATABASE_URL
```

**3. Frontend no Vercel** (mesmo processo da Opção 1)

## 🔄 Configuração para PostgreSQL

### 1. Atualizar DbContext

Edite `src/MedicSoft.Repository/ApplicationDbContext.cs`:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        // Detectar tipo de banco automaticamente
        if (connectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
        else
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
```

### 2. Adicionar Pacote NuGet

```bash
cd src/MedicSoft.Repository
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### 3. Migrations para PostgreSQL

```bash
# Gerar migration para PostgreSQL
dotnet ef migrations add InitialPostgreSQL --context ApplicationDbContext --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api

# Aplicar migration
dotnet ef database update --context ApplicationDbContext --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

## 💾 Estratégia de Backups

### Railway (Automático)

- ✅ Backups automáticos diários
- ✅ Retenção de 7 dias (plano pago)
- ✅ Restore com 1 clique

### VPS (Manual/Automatizado)

Script de backup automático:

```bash
# Criar script de backup
cat > /opt/backup-medicwarehouse.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/backups/medicwarehouse"
DATE=$(date +%Y%m%d_%H%M%S)
mkdir -p $BACKUP_DIR

# Backup PostgreSQL
docker exec medicwarehouse-postgres pg_dump -U medicwarehouse medicwarehouse > $BACKUP_DIR/db_$DATE.sql

# Comprimir
gzip $BACKUP_DIR/db_$DATE.sql

# Manter apenas últimos 7 dias
find $BACKUP_DIR -name "db_*.sql.gz" -mtime +7 -delete

# Upload para S3 ou B2 (opcional)
# aws s3 cp $BACKUP_DIR/db_$DATE.sql.gz s3://seu-bucket/
EOF

chmod +x /opt/backup-medicwarehouse.sh

# Agendar com cron (diário às 3h da manhã)
echo "0 3 * * * /opt/backup-medicwarehouse.sh" | crontab -
```

## 📊 Monitoramento de Custos

### Railway

```bash
# Ver uso atual
railway status

# Ver fatura
railway billing
```

### VPS

```bash
# Monitorar recursos
htop
docker stats

# Disco
df -h

# Memória
free -h
```

## 🎯 Recomendação Final

### Para Começar AGORA (0-20 clínicas):

**🏆 Use Railway + Vercel**
- Custo: $5-15/mês
- Tempo de setup: 30 minutos
- Zero manutenção
- SSL automático
- Backups incluídos

### Quando Crescer (20-100 clínicas):

**🚀 Migre para VPS (Hetzner/DigitalOcean)**
- Custo: $10-40/mês
- Mais controle
- Maior capacidade
- Mesmo stack Docker

### Quando Decolar (100+ clínicas):

**☁️ Cloud Profissional (AWS/Azure/GCP)**
- Custo: $200+/mês
- Auto-scaling
- Multi-region
- SLA garantido

## 🔒 Checklist de Segurança para Produção

Antes de colocar no ar:

- [ ] JWT_SECRET_KEY forte (mínimo 32 caracteres aleatórios)
- [ ] HTTPS configurado (SSL/TLS)
- [ ] Variáveis sensíveis em variáveis de ambiente (nunca no código)
- [ ] CORS configurado corretamente (apenas seu domínio)
- [ ] Rate limiting ativado
- [ ] Backups automáticos configurados
- [ ] Monitoramento de logs ativo
- [ ] Firewall configurado (apenas portas 80, 443, 22)
- [ ] Senha do banco de dados forte
- [ ] Atualizações de segurança automáticas
- [ ] WAF (Web Application Firewall) - considerar Cloudflare Free

## 📚 Recursos Adicionais

- [Railway Docs](https://docs.railway.app/)
- [Vercel Docs](https://vercel.com/docs)
- [PostgreSQL em Produção](https://www.postgresql.org/docs/current/index.html)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Nginx Configuration](https://nginx.org/en/docs/)

## 🆘 Troubleshooting

### Problema: Backend não conecta ao banco

```bash
# Railway
# Verificar variável DATABASE_URL está configurada
railway variables

# VPS
# Testar conexão com banco
docker exec -it medicwarehouse-postgres psql -U medicwarehouse -d medicwarehouse
```

### Problema: Frontend não consegue chamar API

```bash
# Verificar CORS no appsettings.Production.json
# Adicionar origem do frontend em AllowedOrigins
```

### Problema: Memória insuficiente

```bash
# Railway: Upgrade para plano maior
# VPS: Adicionar swap ou aumentar RAM
sudo fallocate -l 2G /swapfile
sudo chmod 600 /swapfile
sudo mkswap /swapfile
sudo swapon /swapfile
```

## 💡 Dicas Importantes

1. **Comece simples**: Railway é o caminho mais fácil
2. **Use PostgreSQL**: Economiza muito vs SQL Server
3. **CDN para frontend**: Vercel/Netlify são gratuitos e rápidos
4. **Monitore custos**: Configure alertas de billing
5. **Backups regulares**: Seu ativo mais importante são os dados
6. **SSL sempre**: HTTPS não é opcional
7. **Logs estruturados**: Use Serilog ou similar para debugging
8. **Environment variables**: Nunca commite secrets no Git

---

**Criado por**: GitHub Copilot  
**Data**: Outubro 2025  
**Versão**: 1.0
