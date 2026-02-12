# 📚 Guia de Migrações do Banco de Dados

Este guia explica como aplicar e gerenciar migrações do Entity Framework Core no Omni Care Software.

> **💡 Tendo problemas com migrações?** Consulte o [Guia de Troubleshooting](TROUBLESHOOTING_MIGRATIONS.md) para soluções detalhadas de problemas comuns.

> **🚀 Fazendo deploy em produção?** Consulte o [Checklist de Deployment - IsException Column](DEPLOYMENT_CHECKLIST_ISEXCEPTION.md) para garantir que todas as migrações críticas sejam aplicadas corretamente.

## ⚡ Início Rápido

### Aplicar Todas as Migrações (Recomendado)

Use o script automatizado para aplicar todas as migrações em todos os contextos:

```bash
./run-all-migrations.sh
```

Ou com uma string de conexão personalizada:

```bash
./run-all-migrations.sh "Host=localhost;Database=primecare;Username=postgres;Password=SuaSenha"
```

## 🔍 Entendendo o Problema

### Erro: "relation does not exist"

Se você vê erros como:
```
42P01: relation "crm.SentimentAnalyses" does not exist
42P01: relation "crm.Complaints" does not exist
42P01: relation "crm.MarketingAutomations" does not exist
```

**Isso significa que as migrações do banco de dados não foram aplicadas.**

## ✅ Solução

### 1. Verifique se o PostgreSQL está Rodando

```bash
# Usando Podman
podman ps | grep postgres

# Usando Docker
docker ps | grep postgres
```

Se não estiver rodando, inicie o PostgreSQL:

```bash
# Usando Podman
podman-compose up postgres -d

# Usando Docker
docker-compose up postgres -d
```

### 2. Aplique as Migrações

**Opção A: Script Automatizado (Recomendado)**

```bash
./run-all-migrations.sh
```

Este script aplica migrações em ordem para todos os contextos:
- MedicSoftDbContext (aplicação principal)
- PatientPortalDbContext (portal do paciente)
- TelemedicineDbContext (telemedicina)
- Outros microserviços

**Opção B: Aplicar Manualmente para Contexto Específico**

```bash
# Para o contexto principal (MedicSoftDbContext)
cd src/MedicSoft.Api
dotnet ef database update --connection "Host=localhost;Database=primecare;Username=postgres;Password=Abc!123456"
```

### 3. Verifique se as Migrações Foram Aplicadas

```bash
cd src/MedicSoft.Api
dotnet ef migrations list
```

Você deve ver todas as migrações marcadas como "Applied".

## 🔧 Comandos Úteis do Entity Framework

### Listar Migrações

```bash
cd src/MedicSoft.Api
dotnet ef migrations list
```

### Ver Status do Banco de Dados

```bash
cd src/MedicSoft.Api
dotnet ef database get-migrations
```

### Criar Nova Migração

```bash
cd src/MedicSoft.Api
dotnet ef migrations add NomeDaMigracao
```

### Reverter Última Migração

```bash
cd src/MedicSoft.Api
dotnet ef database update MigracaoAnterior
```

### Remover Última Migração (Não Aplicada)

```bash
cd src/MedicSoft.Api
dotnet ef migrations remove
```

## 📋 Migrações Importantes do CRM

As seguintes migrações criam as tabelas do CRM que são mencionadas nos erros:

| Migração | Data | Descrição |
|----------|------|-----------|
| `20260127205215_AddCRMEntities` | 27/01/2026 | Cria schema `crm` e todas as tabelas CRM principais |
| `20260127211405_AddPatientJourneyTagsAndEngagement` | 27/01/2026 | Adiciona tags e engagement ao CRM |
| `20260129200623_AddModuleConfigurationHistoryAndEnhancedModules` | 29/01/2026 | Cria tabela SystemNotifications e outras melhorias |
| `20260206145542_AddChatSystem` | 06/02/2026 | Adiciona sistema de chat interno |

## 🚨 Troubleshooting

> **📖 Para soluções detalhadas de problemas comuns, consulte o [Guia de Troubleshooting](TROUBLESHOOTING_MIGRATIONS.md)**

### Erros Comuns - Links Rápidos

- **[Tabela não existe (42P01)](TROUBLESHOOTING_MIGRATIONS.md#tabela-não-existe-42p01)** - Erro mais comum
- **[SystemNotifications não existe](TROUBLESHOOTING_MIGRATIONS.md#systemnotifications-não-existe)** - Migração específica não aplicada
- **[Migrações pendentes](TROUBLESHOOTING_MIGRATIONS.md#migrações-pendentes)** - Como detectar e aplicar
- **[Timeout durante migração](TROUBLESHOOTING_MIGRATIONS.md#timeout-durante-migração)** - Migrações demoradas
- **[Erro de permissão](TROUBLESHOOTING_MIGRATIONS.md#erro-de-permissão)** - Problemas de acesso ao banco

### Problema: Migration falha com erro de permissão

**Erro:**
```
permission denied to create extension "uuid-ossp"
```

**Solução:**
Execute como superusuário do PostgreSQL:

```sql
-- Conecte como postgres
psql -U postgres -d primecare

-- Habilite a extensão
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
```

### Problema: Migration falha por timeout

**Erro:**
```
Npgsql.NpgsqlException: Exception while connecting
```

**Solução:**
1. Verifique se o PostgreSQL está rodando
2. Verifique a string de conexão
3. Verifique se o firewall não está bloqueando a porta 5432

### Problema: Schema "crm" não existe

**Solução:**
A migração `20260127205215_AddCRMEntities` cria o schema automaticamente. Aplique-a:

```bash
cd src/MedicSoft.Api
dotnet ef database update 20260127205215_AddCRMEntities
```

### Problema: Tabelas CRM não existem (MarketingAutomations, SurveyQuestionResponses)

**Erro:**
```
42P01: relation "crm.MarketingAutomations" does not exist
42P01: relation "crm.SurveyQuestionResponses" does not exist
```

**Causa:**
As migrações do CRM não foram aplicadas ao banco de dados. Isso pode acontecer se:
- É a primeira vez executando a aplicação
- O banco de dados foi recriado manualmente
- As migrações foram revertidas acidentalmente
- Há problemas de permissão ao criar o schema `crm`

**Solução:**

1. **Verifique se o PostgreSQL está rodando:**
```bash
podman ps | grep postgres
# ou
docker ps | grep postgres
```

2. **Aplique todas as migrações:**
```bash
./run-all-migrations.sh
```

3. **Ou aplique manualmente a migração do CRM:**
```bash
cd src/MedicSoft.Api
dotnet ef database update 20260127205215_AddCRMEntities
```

4. **Verifique se as tabelas foram criadas:**
```sql
-- Conecte ao banco
psql -U postgres -d primecare

-- Liste as tabelas do schema crm
\dt crm.*

-- Você deve ver:
-- crm.AutomationActions
-- crm.ChurnPredictions
-- crm.ComplaintInteractions
-- crm.Complaints
-- crm.EmailTemplates
-- crm.JourneyStages
-- crm.MarketingAutomations
-- crm.PatientJourneys
-- crm.PatientTouchpoints
-- crm.SentimentAnalyses
-- crm.SurveyQuestionResponses
-- crm.SurveyQuestions
-- crm.SurveyResponses
-- crm.Surveys
-- crm.WebhookDeliveries
-- crm.WebhookSubscriptions
```

5. **Se as tabelas ainda não existirem, force a recriação:**
```bash
cd src/MedicSoft.Api
# Remove todas as migrações aplicadas
dotnet ef database update 0
# Reaplica todas as migrações
dotnet ef database update
```

⚠️ **ATENÇÃO:** O comando `dotnet ef database update 0` irá **apagar todos os dados** do banco. Use apenas em ambiente de desenvolvimento!

## 🔐 Configuração da String de Conexão

### Desenvolvimento Local

Em `src/MedicSoft.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=primecare;Username=postgres;Password=Abc!123456"
  }
}
```

### Produção

**⚠️ IMPORTANTE:** Nunca commite senhas em arquivos de configuração!

Use variáveis de ambiente:

```bash
export DATABASE_CONNECTION_STRING="Host=seu-servidor;Database=primecare;Username=usuario;Password=senha-segura"
```

Ou configure em `appsettings.Production.json` com senhas seguras gerenciadas por Azure Key Vault, AWS Secrets Manager, etc.

## 📊 Aplicação Automática de Migrações

O Omni Care Software **aplica migrações automaticamente** quando a aplicação inicia.

Veja em `src/MedicSoft.Api/Program.cs`:

```csharp
// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
    
    try
    {
        Log.Information("Aplicando migrações do banco de dados...");
        context.Database.Migrate();
        Log.Information("Migrações do banco de dados aplicadas com sucesso");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Falha ao aplicar migrações do banco de dados");
        throw; // Halt application startup if migrations fail
    }
}
```

**Como funciona:**
1. Quando a aplicação inicia, ela verifica se há migrações pendentes
2. Se há, aplica automaticamente
3. Se falha, **a aplicação não inicia** e mostra erro detalhado

## 🎯 Melhores Práticas

1. **Sempre execute migrations antes de iniciar a aplicação em produção**
2. **Faça backup do banco antes de aplicar migrations em produção**
3. **Teste migrations em ambiente de staging primeiro**
4. **Use o script `run-all-migrations.sh` para garantir ordem correta**
5. **Monitore os logs durante aplicação de migrations**

## 📞 Suporte

Se você continuar tendo problemas com migrações:

1. **Consulte o [Guia de Troubleshooting](TROUBLESHOOTING_MIGRATIONS.md)** para soluções detalhadas
2. Verifique os logs da aplicação em `logs/`
3. Verifique se todas as dependências estão instaladas
4. Abra uma issue no GitHub com os logs de erro

## 🔗 Links Úteis

- **[Troubleshooting de Migrações](TROUBLESHOOTING_MIGRATIONS.md)** - Guia completo de resolução de problemas
- [Entity Framework Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql - PostgreSQL .NET Driver](https://www.npgsql.org/)
