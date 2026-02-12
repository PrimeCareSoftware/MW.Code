# 🔧 Troubleshooting de Migrações - Guia de Resolução de Problemas

Este guia fornece soluções detalhadas para problemas comuns relacionados a migrações do Entity Framework Core no Omni Care Software.

---

## 🌍 English Section - Critical Migration Issue

### ⚠️ "Relation Does Not Exist" Error (42P01)

**For comprehensive English documentation on this critical issue, see:**
- **[MIGRATION_BEST_PRACTICES.md](./MIGRATION_BEST_PRACTICES.md)** - Complete defensive migration pattern guide
- **[src/MedicSoft.Repository/Migrations/README.md](./src/MedicSoft.Repository/Migrations/README.md)** - Quick reference for migrations

#### Critical Error Pattern

```
Npgsql.PostgresException (0x80004005): 42P01: relation "DashboardWidgets" does not exist
ALTER TABLE "DashboardWidgets" ALTER COLUMN "UpdatedAt" TYPE timestamp with time zone;
```

#### Root Cause

This error occurs when migrations attempt to ALTER tables that were created with `CREATE TABLE IF NOT EXISTS`. If the table creation was skipped (because the table already existed), future ALTER operations will fail.

#### Tables Requiring Special Attention

The following tables were created with conditional creation and **require defensive checks** in all future ALTER operations:

**Analytics Tables** (from `20260203150000_AddAnalyticsDashboardTables`):
- ✅ `CustomDashboards`
- ✅ `DashboardWidgets`
- ✅ `WidgetTemplates`
- ✅ `ReportTemplates`

**System Tables**:
- ✅ `SystemNotifications`
- ✅ `NotificationRules`
- ✅ `SubscriptionCredits`

**Tables with Conditional Columns**:
- ⚠️ `Appointments` - Payment columns
- ⚠️ `Clinics` - `DefaultPaymentReceiverType`
- ⚠️ `Users` - MFA grace period columns

#### Quick Fix Pattern

When altering these tables, always use defensive checks:

```csharp
// ❌ WRONG - Will fail if table doesn't exist
migrationBuilder.AlterColumn<DateTime>(
    name: "UpdatedAt",
    table: "DashboardWidgets",
    type: "timestamp with time zone");

// ✅ CORRECT - Checks existence first
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'DashboardWidgets' 
            AND table_schema = 'public'
        ) AND EXISTS (
            SELECT 1 FROM information_schema.columns 
            WHERE table_name = 'DashboardWidgets' 
            AND column_name = 'UpdatedAt'
            AND table_schema = 'public'
        ) THEN
            ALTER TABLE ""DashboardWidgets"" 
            ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
        END IF;
    END $$;
");
```

#### Complete Documentation

For complete patterns, examples, and best practices, see [MIGRATION_BEST_PRACTICES.md](./MIGRATION_BEST_PRACTICES.md).

---

### ⚠️ Missing Column Error: `column b.IsException does not exist` (42703)

#### Error Pattern

```
Npgsql.PostgresException (0x80004005): 42703: column b.IsException does not exist
POSITION: 68

Microsoft.EntityFrameworkCore.Query
   at MedicSoft.Repository.Repositories.BlockedTimeSlotRepository.GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid clinicId, String tenantId)
   at MedicSoft.Application.Handlers.Queries.Appointments.GetBlockedTimeSlotsRangeQueryHandler.Handle(GetBlockedTimeSlotsRangeQuery request, CancellationToken cancellationToken)
   at MedicSoft.Api.Controllers.BlockedTimeSlotsController.GetByDateRange(DateTime startDate, DateTime endDate, Guid clinicId)
```

#### Affected Endpoints

- `GET /api/appointments?clinicId={id}&date={date}` - 500 Error
- `GET /api/blocked-time-slots/range?startDate={start}&endDate={end}&clinicId={id}` - 500 Error

#### Root Cause

**Error Code:** `42703` (PostgreSQL "column does not exist")

**Affected Table:** `BlockedTimeSlots`

**Affected Column:** `IsException`

The `IsException` column was introduced in migration `20260210140000_AddRecurrenceSeriesAndExceptions` but may not have been applied to all database instances due to:
1. Migration not run on the target database
2. Partial migration failure
3. Database restored from an old backup
4. Environment-specific deployment issues

#### Quick Fix

**Option 1: Run Migrations (Recommended)**
```bash
cd src/MedicSoft.Api
dotnet ef database update
```

**Option 2: Manual SQL Fix (If migration fails)**
```sql
-- Check if column exists
SELECT column_name 
FROM information_schema.columns 
WHERE table_name = 'BlockedTimeSlots' 
  AND column_name = 'IsException';

-- If not exists, add it manually
ALTER TABLE "BlockedTimeSlots" 
ADD COLUMN "IsException" boolean NOT NULL DEFAULT false;
```

#### Defensive Fix Migration

A defensive fix migration **`20260212001705_AddMissingIsExceptionColumn`** was created to ensure the column exists. This migration:
- ✅ Uses `IF NOT EXISTS` checks to prevent errors
- ✅ Can safely run on databases that already have the column
- ✅ Provides clear logging about what actions were taken
- ✅ Is idempotent (can be run multiple times safely)

To apply the defensive fix:
```bash
cd src/MedicSoft.Api
dotnet ef database update 20260212001705_AddMissingIsExceptionColumn
```

#### Verification

After fixing, verify the column exists:

**Using psql:**
```bash
psql -U postgres -d primecare -c "\d BlockedTimeSlots" | grep IsException
```

**Using SQL:**
```sql
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'BlockedTimeSlots' 
  AND column_name = 'IsException';
```

Expected output:
```
 column_name | data_type | is_nullable | column_default 
-------------+-----------+-------------+----------------
 IsException | boolean   | NO          | false
```

#### Prevention

**Always verify migrations are applied before deploying:**
```bash
# Check for pending migrations
cd src/MedicSoft.Api
dotnet ef migrations list

# Apply all pending migrations
dotnet ef database update
```

**Add to CI/CD pipeline:**
```yaml
- name: Apply Database Migrations
  run: |
    cd src/MedicSoft.Api
    dotnet ef database update
```

#### Related Files

- **Entity:** `src/MedicSoft.Domain/Entities/BlockedTimeSlot.cs` (line 23)
- **Configuration:** `src/MedicSoft.Repository/Configurations/BlockedTimeSlotConfiguration.cs` (lines 46-48)
- **Original Migration:** `src/MedicSoft.Repository/Migrations/PostgreSQL/20260210140000_AddRecurrenceSeriesAndExceptions.cs`
- **Fix Migration:** `src/MedicSoft.Repository/Migrations/PostgreSQL/20260212001705_AddMissingIsExceptionColumn.cs`
- **Repository:** `src/MedicSoft.Repository/Repositories/BlockedTimeSlotRepository.cs`

---

## 📋 Índice (Portuguese Section / Seção em Português)

- [Erros Comuns](#erros-comuns)
  - [Tabela Não Existe (42P01)](#tabela-não-existe-42p01)
  - [SystemNotifications Não Existe](#systemnotifications-não-existe)
  - [Migrações Pendentes](#migrações-pendentes)
  - [Timeout Durante Migração](#timeout-durante-migração)
  - [Erro de Permissão](#erro-de-permissão)
- [Comandos de Diagnóstico](#comandos-de-diagnóstico)
- [Soluções Passo a Passo](#soluções-passo-a-passo)
- [Prevenção de Problemas](#prevenção-de-problemas)

---

## 🚨 Erros Comuns

### Tabela Não Existe (42P01)

#### Sintomas

```
Npgsql.PostgresException (0x80004005): 42P01: relation "SystemNotifications" does not exist
Npgsql.PostgresException (0x80004005): 42P01: relation "crm.MarketingAutomations" does not exist
```

#### Causa

Este erro ocorre quando:
1. A aplicação tenta acessar uma tabela que ainda não foi criada
2. As migrações que criam essas tabelas não foram aplicadas ao banco de dados
3. O banco de dados foi recriado sem executar as migrações

#### Solução Rápida

```bash
# Opção 1: Usar o script automatizado (RECOMENDADO)
./run-all-migrations.sh

# Opção 2: Aplicar manualmente
cd src/MedicSoft.Api
dotnet ef database update
```

#### Solução Detalhada

1. **Verifique se o PostgreSQL está rodando:**
   ```bash
   # Com Podman
   podman ps | grep postgres
   
   # Com Docker
   docker ps | grep postgres
   ```

2. **Se não estiver rodando, inicie:**
   ```bash
   # Com Podman
   podman-compose up postgres -d
   
   # Com Docker
   docker-compose up postgres -d
   ```

3. **Verifique quais migrações estão pendentes:**
   ```bash
   cd src/MedicSoft.Api
   dotnet ef migrations list
   ```

4. **Aplique as migrações:**
   ```bash
   dotnet ef database update
   ```

5. **Verifique se a tabela foi criada:**
   ```bash
   psql -U postgres -d primecare -c "\dt SystemNotifications"
   ```

---

### SystemNotifications Não Existe

#### Descrição Detalhada

A tabela `SystemNotifications` é criada pela migração `20260129200623_AddModuleConfigurationHistoryAndEnhancedModules`. Se você está vendo este erro, significa que esta migração específica não foi aplicada.

#### Identificar o Problema

```bash
cd src/MedicSoft.Api

# Verificar se a migração foi aplicada
dotnet ef migrations list | grep "AddModuleConfigurationHistoryAndEnhancedModules"

# Se aparecer sem indicador de aplicação (ex: "(Pending)"), ela não foi aplicada
```

#### Verificar no Banco de Dados

```sql
-- Conectar ao banco
psql -U postgres -d primecare

-- Verificar se a tabela existe
SELECT table_name 
FROM information_schema.tables 
WHERE table_name = 'SystemNotifications';

-- Verificar migrações aplicadas
SELECT "MigrationId", "ProductVersion" 
FROM "__EFMigrationsHistory" 
ORDER BY "MigrationId" DESC 
LIMIT 10;
```

#### Solução

```bash
# Aplicar a migração específica ou todas as pendentes
cd src/MedicSoft.Api
dotnet ef database update 20260129200623_AddModuleConfigurationHistoryAndEnhancedModules

# Ou aplicar todas
dotnet ef database update
```

---

### Migrações Pendentes

#### Como Identificar

A aplicação agora detecta automaticamente migrações pendentes no startup e registra no log:

```
[Warning] Existem 3 migrações pendentes que serão aplicadas: 
20260129200623_AddModuleConfigurationHistoryAndEnhancedModules, 
20260206145542_AddChatSystem, 
20260127211405_AddPatientJourneyTagsAndEngagement
```

#### Listar Todas as Migrações Pendentes

```bash
cd src/MedicSoft.Api
dotnet ef migrations list

# A saída mostrará:
# 20260127205215_AddCRMEntities (Applied)
# 20260129200623_AddModuleConfigurationHistoryAndEnhancedModules (Pending)
# 20260206145542_AddChatSystem (Pending)
```

#### Aplicar Migrações Pendentes

```bash
# Aplicar todas as pendentes
dotnet ef database update

# Aplicar até uma migração específica
dotnet ef database update 20260129200623_AddModuleConfigurationHistoryAndEnhancedModules
```

---

### Timeout Durante Migração

#### Sintomas

```
Npgsql.NpgsqlException: Exception while reading from stream
System.TimeoutException: Timeout during reading attempt
```

#### Causas Comuns

1. Migração muito grande ou complexa
2. Banco de dados com muitos dados
3. Recursos limitados (CPU/RAM)
4. Rede lenta (se banco remoto)

#### Solução

1. **Aumentar o timeout na string de conexão:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=primecare;Username=postgres;Password=Abc!123456;Timeout=300;CommandTimeout=300"
     }
   }
   ```

2. **Aplicar migrações em etapas:**
   ```bash
   # Listar migrações
   dotnet ef migrations list
   
   # Aplicar uma por vez
   dotnet ef database update 20260127205215_AddCRMEntities
   dotnet ef database update 20260129200623_AddModuleConfigurationHistoryAndEnhancedModules
   # ... e assim por diante
   ```

3. **Verificar recursos do sistema:**
   ```bash
   # Verificar memória disponível
   free -h
   
   # Verificar uso de CPU
   top
   ```

---

### Erro de Permissão

#### Sintomas

```
42501: permission denied to create extension "uuid-ossp"
42501: permission denied to create schema
```

#### Solução

1. **Conectar como superusuário:**
   ```bash
   psql -U postgres
   ```

2. **Criar extensões necessárias:**
   ```sql
   -- Conectar ao banco de dados
   \c primecare
   
   -- Criar extensões
   CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
   CREATE EXTENSION IF NOT EXISTS "pgcrypto";
   ```

3. **Garantir permissões ao usuário:**
   ```sql
   -- Dar permissões ao usuário da aplicação
   GRANT CREATE ON DATABASE primecare TO seu_usuario;
   GRANT ALL ON SCHEMA public TO seu_usuario;
   GRANT ALL ON SCHEMA crm TO seu_usuario;
   ```

4. **Aplicar migrações novamente:**
   ```bash
   cd src/MedicSoft.Api
   dotnet ef database update
   ```

---

## 🔍 Comandos de Diagnóstico

### Verificar Estado do Banco de Dados

```bash
# 1. Verificar se o PostgreSQL está acessível
psql -U postgres -d primecare -c "SELECT version();"

# 2. Listar todas as tabelas
psql -U postgres -d primecare -c "\dt"

# 3. Listar tabelas do schema CRM
psql -U postgres -d primecare -c "\dt crm.*"

# 4. Verificar migrações aplicadas
psql -U postgres -d primecare -c 'SELECT "MigrationId" FROM "__EFMigrationsHistory" ORDER BY "MigrationId";'
```

### Verificar Estado das Migrações

```bash
cd src/MedicSoft.Api

# Listar todas as migrações
dotnet ef migrations list

# Ver detalhes de uma migração
dotnet ef migrations script 20260129200623_AddModuleConfigurationHistoryAndEnhancedModules

# Gerar script SQL de todas as migrações pendentes
dotnet ef migrations script --idempotent --output migrations.sql
```

### Verificar Logs da Aplicação

```bash
# Ver logs recentes
tail -f logs/medicsoft-*.log

# Buscar erros de migração
grep -i "migration" logs/medicsoft-*.log | grep -i "error"

# Buscar erros de tabela não encontrada
grep "42P01" logs/medicsoft-*.log
```

---

## 🛠️ Soluções Passo a Passo

### Situação 1: Primeira Instalação - Banco Vazio

**Problema:** Iniciando a aplicação pela primeira vez e nenhuma tabela existe.

**Solução:**

```bash
# 1. Verificar PostgreSQL está rodando
podman ps | grep postgres

# 2. Se não estiver, iniciar
podman-compose up postgres -d

# 3. Aguardar PostgreSQL inicializar (10-15 segundos)
sleep 15

# 4. Aplicar todas as migrações
./run-all-migrations.sh

# 5. Verificar se funcionou
psql -U postgres -d primecare -c "\dt" | grep SystemNotifications
```

### Situação 2: Banco Existente com Migrações Antigas

**Problema:** Banco já tem dados mas faltam tabelas novas (ex: SystemNotifications).

**Solução:**

```bash
# 1. Fazer backup primeiro (IMPORTANTE!)
pg_dump -U postgres -d primecare > backup_$(date +%Y%m%d_%H%M%S).sql

# 2. Verificar quais migrações estão pendentes
cd src/MedicSoft.Api
dotnet ef migrations list

# 3. Aplicar migrações pendentes
dotnet ef database update

# 4. Verificar se as novas tabelas foram criadas
psql -U postgres -d primecare -c "\dt SystemNotifications"
```

### Situação 3: Migração Falhou Parcialmente

**Problema:** Uma migração começou mas falhou no meio, deixando o banco em estado inconsistente.

**Solução:**

```bash
# 1. Ver qual foi a última migração aplicada com sucesso
psql -U postgres -d primecare -c 'SELECT "MigrationId" FROM "__EFMigrationsHistory" ORDER BY "MigrationId" DESC LIMIT 1;'

# 2. Tentar reverter para a última migração bem-sucedida
cd src/MedicSoft.Api
dotnet ef database update <ultima_migracao_bem_sucedida>

# 3. Aplicar novamente
dotnet ef database update

# Se ainda falhar, pode ser necessário correção manual no banco
```

### Situação 4: Preciso Recomeçar do Zero

**Problema:** O banco está em um estado problemático e você quer recomeçar (APENAS DESENVOLVIMENTO!).

**Solução:**

```bash
# ⚠️ ATENÇÃO: Isso apagará TODOS OS DADOS! Use apenas em desenvolvimento!

# 1. Fazer backup se tiver dados importantes
pg_dump -U postgres -d primecare > backup_antes_reset.sql

# 2. Dropar o banco
psql -U postgres -c "DROP DATABASE IF EXISTS primecare;"

# 3. Recriar o banco
psql -U postgres -c "CREATE DATABASE primecare;"

# 4. Aplicar todas as migrações
cd src/MedicSoft.Api
dotnet ef database update

# 5. Ou usar o script
cd ../..
./run-all-migrations.sh
```

---

## 🛡️ Prevenção de Problemas

### Checklist Antes de Iniciar a Aplicação

```bash
# 1. PostgreSQL está rodando?
podman ps | grep postgres

# 2. Consigo conectar?
psql -U postgres -d primecare -c "SELECT 1;"

# 3. Existem migrações pendentes?
cd src/MedicSoft.Api
dotnet ef migrations list | grep Pending

# 4. Se sim, aplicar
dotnet ef database update

# 5. Verificar tabelas críticas existem
psql -U postgres -d primecare -c "\dt SystemNotifications"
psql -U postgres -d primecare -c "\dt crm.MarketingAutomations"
```

### Boas Práticas

1. **Sempre execute migrações antes de iniciar a aplicação**
   - Use o script `./run-all-migrations.sh` no CI/CD
   - Configure health checks para verificar migrações

2. **Faça backup antes de aplicar migrações em produção**
   ```bash
   pg_dump -U postgres -d primecare > backup_pre_migration_$(date +%Y%m%d_%H%M%S).sql
   ```

3. **Teste migrações em staging primeiro**
   - Nunca aplique migrações não testadas em produção
   - Valide que a aplicação funciona após as migrações

4. **Monitore os logs durante migrações**
   ```bash
   tail -f logs/medicsoft-*.log | grep -i migration
   ```

5. **Use transações para migrações complexas**
   - EF Core faz isso automaticamente
   - Se uma migração falhar, o banco volta ao estado anterior

### Script de Verificação Automática

Crie um script `check-migrations.sh`:

```bash
#!/bin/bash
set -e

echo "🔍 Verificando estado das migrações..."

# Verificar PostgreSQL
if ! podman ps | grep -q postgres; then
    echo "❌ PostgreSQL não está rodando!"
    exit 1
fi

# Verificar conexão
if ! psql -U postgres -d primecare -c "SELECT 1;" > /dev/null 2>&1; then
    echo "❌ Não foi possível conectar ao PostgreSQL!"
    exit 1
fi

# Verificar migrações pendentes
cd src/MedicSoft.Api
PENDING=$(dotnet ef migrations list 2>/dev/null | grep -c "Pending" || echo "0")

if [ "$PENDING" -gt 0 ]; then
    echo "⚠️  Existem $PENDING migrações pendentes!"
    echo "Execute: ./run-all-migrations.sh"
    exit 1
fi

echo "✅ Todas as migrações estão aplicadas!"
```

---

## 📞 Ainda Precisa de Ajuda?

Se você tentou todas as soluções acima e ainda está com problemas:

1. **Colete informações:**
   ```bash
   # Versão do .NET
   dotnet --version
   
   # Versão do PostgreSQL
   psql -U postgres -c "SELECT version();"
   
   # Logs recentes
   tail -100 logs/medicsoft-*.log > debug_logs.txt
   
   # Estado das migrações
   cd src/MedicSoft.Api
   dotnet ef migrations list > migrations_status.txt
   ```

2. **Verifique a documentação:**
   - [MIGRATIONS_GUIDE.md](MIGRATIONS_GUIDE.md) - Guia completo de migrações
   - [README.md](README.md) - Documentação geral do projeto

3. **Abra uma issue:**
   - Inclua os arquivos coletados acima
   - Descreva o que você tentou
   - Inclua mensagens de erro completas

---

## 🔗 Links Úteis

- [Entity Framework Core Migrations - Microsoft Docs](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [PostgreSQL Error Codes](https://www.postgresql.org/docs/current/errcodes-appendix.html)
- [Npgsql Documentation](https://www.npgsql.org/doc/)
- [Guia de Migrações do Projeto](MIGRATIONS_GUIDE.md)

---

## 📝 Notas Importantes

- **42P01** é o código SQL State do PostgreSQL para "tabela/relação não existe"
- **SystemNotifications** foi introduzida na migração `20260129200623_AddModuleConfigurationHistoryAndEnhancedModules`
- A aplicação **não inicia** se houver problemas de migração - isso é intencional para evitar corrupção de dados
- Logs estão em `logs/medicsoft-*.log` e são essenciais para diagnóstico
- O script `run-all-migrations.sh` é a maneira mais segura de aplicar migrações

---

Última atualização: 2026-02-07
