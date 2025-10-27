# 🚂 Guia Completo de Deploy no Railway

## 📋 Visão Geral

Este guia mostra passo a passo como fazer deploy do MedicWarehouse no Railway com **PostgreSQL**, a opção mais econômica e simples para começar.

**Tempo estimado**: 30 minutos  
**Custo inicial**: ~$5-15/mês  
**Suporta**: 10-50 clínicas pequenas

## 🎯 Por que Railway?

✅ **Setup em minutos**: Deploy automático via GitHub  
✅ **PostgreSQL incluído**: Sem custos extras de banco  
✅ **SSL automático**: HTTPS grátis  
✅ **Backups**: Automáticos no plano pago  
✅ **Escalável**: Cresce conforme necessidade  
✅ **Logs**: Integrados e em tempo real  
✅ **$5 grátis**: Todo mês para começar

## 🔧 Pré-requisitos

1. Conta no GitHub (já tem o repositório)
2. Conta no Railway (criar em: https://railway.app)
3. Migração do SQL Server para PostgreSQL (ver abaixo)

## 📦 Parte 1: Preparar o Projeto para PostgreSQL

### Passo 1: Adicionar Suporte ao PostgreSQL

```bash
# 1. Navegar para o projeto Repository
cd src/MedicSoft.Repository

# 2. Adicionar pacote Npgsql
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0

# 3. Voltar para raiz
cd ../..
```

### Passo 2: Atualizar ApplicationDbContext

Edite: `src/MedicSoft.Repository/Data/ApplicationDbContext.cs`

Adicione este método (ou atualize se já existir):

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        // Auto-detectar tipo de banco baseado na connection string
        if (connectionString?.Contains("Host=", StringComparison.OrdinalIgnoreCase) == true ||
            connectionString?.Contains("postgres", StringComparison.OrdinalIgnoreCase) == true)
        {
            // PostgreSQL
            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });
        }
        else
        {
            // SQL Server (para desenvolvimento local)
            optionsBuilder.UseSqlServer(connectionString);
        }

        if (_env.IsDevelopment())
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
    }
}
```

### Passo 3: Criar Migration para PostgreSQL

```bash
# Gerar nova migration para PostgreSQL
dotnet ef migrations add InitialPostgreSQL \
  --context ApplicationDbContext \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api \
  --output-dir Migrations/PostgreSQL

# Build para verificar
dotnet build
```

### Passo 4: Atualizar appsettings

Edite: `src/MedicSoft.Api/appsettings.Production.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DATABASE_URL}"
  },
  "JwtSettings": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "ExpiryMinutes": 60,
    "Issuer": "MedicWarehouse",
    "Audience": "MedicWarehouse-API"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

### Passo 5: Commitar Mudanças

```bash
git add .
git commit -m "feat: adicionar suporte PostgreSQL para Railway"
git push origin main
```

## 🚀 Parte 2: Deploy no Railway

### Passo 1: Criar Conta e Projeto

1. Acesse: https://railway.app
2. Click em **"Start a New Project"**
3. Login com GitHub
4. Click em **"Deploy from GitHub repo"**
5. Selecione o repositório: `MedicWarehouse/MW.Code`
6. Railway detectará automaticamente que é um projeto .NET

### Passo 2: Adicionar PostgreSQL

1. No dashboard do projeto, click em **"+ New"**
2. Selecione **"Database"**
3. Click em **"Add PostgreSQL"**
4. Railway criará o banco e configurará automaticamente a variável `DATABASE_URL`

### Passo 3: Configurar Backend API

1. Click no serviço da API (auto-criado)
2. Vá em **"Settings"** → **"Root Directory"**
   - Defina: `src/MedicSoft.Api`
3. Vá em **"Variables"**
4. Adicione as seguintes variáveis:

```bash
# Ambiente
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:8080

# Database (auto-configurado pelo Railway, verifique se existe)
DATABASE_URL=postgres://...

# JWT (IMPORTANTE: Use chave forte!)
JWT_SECRET_KEY=sua-chave-jwt-super-segura-minimo-32-caracteres-aleatórios

# Security
Security__RequireHttps=true
RateLimiting__EnableRateLimiting=true

# CORS (adicione seu domínio do frontend depois)
Cors__AllowedOrigins__0=https://seu-app.vercel.app
```

**Como gerar chave JWT segura:**
```bash
# Linux/Mac
openssl rand -base64 32

# Windows PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

### Passo 4: Configurar Domínio

1. Em **"Settings"** → **"Networking"**
2. Click em **"Generate Domain"**
3. Railway gerará uma URL tipo: `medicwarehouse-api-production.up.railway.app`
4. Copie esta URL - você vai precisar para o frontend

### Passo 5: Deploy e Verificação

1. Railway iniciará o build automaticamente
2. Aguarde ~3-5 minutos
3. Verifique os logs em **"Deployments"** → último deploy → **"View Logs"**
4. Quando ver "Application started", o backend está no ar!

### Passo 6: Testar API

```bash
# Testar health check
curl https://sua-api.up.railway.app/health

# Testar Swagger
# Abra no navegador: https://sua-api.up.railway.app/swagger
```

### Passo 7: Aplicar Migrations

Railway aplicará migrations automaticamente no startup, mas se precisar forçar:

1. Vá em **"Variables"** do serviço da API
2. Adicione temporariamente:
   ```
   RAILWAY_RUN_MIGRATIONS=true
   ```
3. Redeploy do serviço

Ou execute via Railway CLI:

```bash
# Instalar Railway CLI
npm i -g @railway/cli

# Login
railway login

# Link ao projeto
railway link

# Executar migration
railway run dotnet ef database update --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

## 🌐 Parte 3: Deploy Frontend no Vercel

### Passo 1: Preparar Frontend

1. Edite: `frontend/medicwarehouse-app/src/environments/environment.prod.ts`

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://sua-api.up.railway.app'
};
```

```bash
git add .
git commit -m "chore: configurar API URL para produção"
git push
```

### Passo 2: Deploy no Vercel

1. Acesse: https://vercel.com
2. Login com GitHub
3. Click **"Add New"** → **"Project"**
4. Selecione o repositório `MW.Code`
5. Configure:

```
Framework Preset: Angular
Root Directory: frontend/medicwarehouse-app
Build Command: npm run build
Output Directory: dist/medicwarehouse-app/browser
Install Command: npm install
```

6. Em **"Environment Variables"**, adicione:
```
NODE_ENV=production
```

7. Click **"Deploy"**

### Passo 3: Configurar CORS no Backend

1. Volte ao Railway
2. Na API, atualize a variável de ambiente:
```
Cors__AllowedOrigins__0=https://seu-app.vercel.app
```
3. Redeploy da API

### Passo 4: Deploy System Admin (Opcional)

Repita o processo acima para:
- Root Directory: `frontend/mw-system-admin`
- Output Directory: `dist/mw-system-admin/browser`

Vercel suporta múltiplos projetos do mesmo repositório.

## 💰 Parte 4: Gerenciar Custos

### Ver Uso Atual

1. No dashboard do Railway, click em **"Usage"**
2. Veja consumo de CPU, RAM, Network
3. Configure alertas em **"Settings"** → **"Billing"**

### Planos Railway

```
Hobby Plan (Padrão):
- $5/mês em créditos grátis
- $0.000463/GB-hora RAM
- $0.000231/vCPU-hora
- $0.10/GB egress

Estimativa para 20 clínicas:
- RAM: 512MB x 730h = $0.17
- CPU: 0.5 vCPU x 730h = $0.08
- Egress: ~10GB = $1.00
Total: ~$1.25/mês (coberto pelos $5 grátis!)
```

### Planos Vercel

```
Hobby (Grátis):
- 100GB bandwidth
- Domínios ilimitados
- SSL automático
- Perfeito para começar!
```

## 🔍 Parte 5: Monitoramento e Logs

### Ver Logs da API

1. Railway → Selecione serviço API
2. Click em **"Deployments"**
3. Click no deploy ativo → **"View Logs"**
4. Logs em tempo real!

### Métricas

1. Railway → **"Metrics"**
2. Veja:
   - CPU Usage
   - Memory Usage
   - Network Traffic
   - Response Times

### Alertas

1. Railway → **"Settings"** → **"Notifications"**
2. Configure alertas para:
   - Deploy failures
   - High resource usage
   - Budget limits

## 🔒 Parte 6: Segurança

### Checklist de Segurança

- [x] HTTPS configurado (Railway automático)
- [x] JWT_SECRET_KEY forte e aleatória
- [x] DATABASE_URL não exposta
- [x] CORS configurado para domínios específicos
- [x] Rate limiting ativado
- [ ] Configurar Cloudflare (opcional, mas recomendado)
- [ ] Backups configurados (ver abaixo)

### Configurar Backups

#### Opção 1: Railway Backups (Plano Pro - $20/mês)

1. Upgrade para Pro
2. Backups automáticos diários
3. Retenção de 7 dias

#### Opção 2: Backups Manuais (Grátis)

```bash
# Via Railway CLI
railway login
railway link

# Backup
railway run pg_dump $DATABASE_URL > backup-$(date +%Y%m%d).sql

# Agendar no cron (Linux/Mac)
crontab -e
# Adicionar: 0 3 * * * cd /path/to/MW.Code && railway run pg_dump $DATABASE_URL > backup-$(date +%Y%m%d).sql
```

#### Opção 3: Cloudflare + S3/B2

Use GitHub Actions para backups automáticos:

```yaml
# .github/workflows/backup-db.yml
name: Database Backup
on:
  schedule:
    - cron: '0 3 * * *' # Diário às 3h
  workflow_dispatch:

jobs:
  backup:
    runs-on: ubuntu-latest
    steps:
      - name: Backup Database
        run: |
          # Install Railway CLI
          npm i -g @railway/cli
          
          # Backup
          railway run pg_dump $DATABASE_URL > backup.sql
          
          # Upload para S3/B2 (configurar credenciais nos secrets)
```

## 🚨 Troubleshooting

### Problema: Build falha no Railway

**Solução**:
```bash
# Verificar se o projeto compila localmente
dotnet build

# Ver logs do Railway
railway logs --service api
```

### Problema: API não conecta ao PostgreSQL

**Solução**:
```bash
# Verificar DATABASE_URL
railway variables

# Testar conexão
railway run psql $DATABASE_URL
```

### Problema: Migration não aplicou

**Solução**:
```bash
# Aplicar manualmente
railway run dotnet ef database update --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

### Problema: CORS error no frontend

**Solução**:
1. Verificar `Cors__AllowedOrigins__0` no Railway
2. Incluir domínio EXATO (com https://)
3. Redeploy da API

### Problema: 502 Bad Gateway

**Solução**:
1. Ver logs: `railway logs`
2. Aumentar timeout: `Settings → Deploy → Build Timeout`
3. Verificar porta: app deve escutar em `8080`

## 📊 Monitoramento de Performance

### Métricas Importantes

```bash
# Via Railway CLI
railway metrics

# Ou no dashboard Railway:
- Response time médio (deve ser < 500ms)
- CPU usage (deve ficar < 70%)
- Memory usage (< 80%)
- Error rate (< 1%)
```

### Quando Escalar?

Considere upgrade quando:
- CPU > 80% constantemente
- Memory > 90%
- Response time > 1s
- Mais de 50 clínicas ativas

## 🎉 Conclusão

Parabéns! Seu sistema está no ar com:

- ✅ Backend .NET na Railway
- ✅ PostgreSQL gerenciado
- ✅ Frontend na Vercel
- ✅ HTTPS automático
- ✅ Logs em tempo real
- ✅ Custo: ~$5-15/mês

### Próximos Passos

1. Configure domínio próprio (Railway + Vercel)
2. Configure backups automáticos
3. Adicione monitoramento (Sentry, New Relic)
4. Configure CI/CD para deploy automático
5. Documente processos para sua equipe

## 📚 Recursos

- [Railway Docs](https://docs.railway.app)
- [Vercel Docs](https://vercel.com/docs)
- [PostgreSQL Migration Guide](https://www.postgresql.org/docs/current/migration.html)
- [.NET on Railway](https://docs.railway.app/guides/dotnet)

## 💡 Dicas Finais

1. **Monitore custos**: Configure alertas em $10/mês
2. **Teste tudo**: Faça deploy de staging primeiro
3. **Backups**: Implemente desde o dia 1
4. **Logs**: Use structured logging (Serilog)
5. **Performance**: Cache agressivo no frontend
6. **Segurança**: Audite secrets regularmente

---

**Criado por**: GitHub Copilot  
**Versão**: 1.0  
**Última atualização**: Outubro 2025
