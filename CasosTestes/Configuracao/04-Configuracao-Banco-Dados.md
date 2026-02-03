# 04 - Configuração do Banco de Dados (PostgreSQL)

> **Objetivo:** Configurar o PostgreSQL e executar migrations para criar o schema do banco  
> **Tempo estimado:** 10-15 minutos  
> **Pré-requisitos:** [01-Configuracao-Ambiente.md](01-Configuracao-Ambiente.md) completo

## 📋 Índice

1. [Visão Geral do Banco](#visão-geral-do-banco)
2. [Criar Banco de Dados](#criar-banco-de-dados)
3. [Configurar Conexão](#configurar-conexão)
4. [Executar Migrations](#executar-migrations)
5. [Seed de Dados Iniciais](#seed-de-dados-iniciais)
6. [Verificação](#verificação)
7. [Backup e Restore](#backup-e-restore)

## 🗄️ Visão Geral do Banco

O Omni Care Software usa **PostgreSQL 16** como banco de dados principal.

### Características

- ✅ **Multi-tenant:** Dados isolados por tenant (TenantId)
- ✅ **Entity Framework Core 8:** ORM para acesso a dados
- ✅ **Migrations:** Versionamento do schema
- ✅ **100+ Tabelas:** Estrutura completa
- ✅ **Índices otimizados:** Performance garantida
- ✅ **Soft Delete:** Dados nunca são permanentemente deletados
- ✅ **Auditoria:** CreatedAt, UpdatedAt, CreatedBy, UpdatedBy em todas as tabelas

### Principais Tabelas

| Tabela | Descrição | Registros Típicos |
|--------|-----------|-------------------|
| Tenants | Clínicas/Consultórios | 1-1000 |
| Users | Usuários do sistema | 10-10000 |
| Patients | Pacientes | 100-100000 |
| Doctors | Médicos | 5-500 |
| Appointments | Consultas/Agendamentos | 1000-1000000 |
| MedicalRecords | Prontuários SOAP | 1000-1000000 |
| Prescriptions | Prescrições | 500-500000 |
| Analytics* | Métricas e KPIs | Variável |

## 🔧 Criar Banco de Dados

### Opção 1: Via psql (Linha de Comando)

```bash
# Conectar ao PostgreSQL como superusuário
psql -U postgres

# Dentro do psql:
CREATE DATABASE medicsoft_dev;

# Criar usuário específico (recomendado)
CREATE USER medicsoft_user WITH PASSWORD 'sua_senha_segura';

# Dar permissões ao usuário
GRANT ALL PRIVILEGES ON DATABASE medicsoft_dev TO medicsoft_user;

# Conectar ao banco criado
\c medicsoft_dev

# Criar extensões necessárias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

# Sair do psql
\q
```

### Opção 2: Via pgAdmin (Interface Gráfica)

1. Abra o pgAdmin
2. Conecte-se ao servidor PostgreSQL (localhost)
3. Botão direito em "Databases" > "Create" > "Database"
4. Nome: `medicsoft_dev`
5. Owner: `postgres` ou o usuário que você criou
6. Clique em "Save"

### Opção 3: Via Script Automatizado

```bash
# Windows (PowerShell)
cd MW.Code
.\scripts\create-database.ps1

# macOS/Linux
cd MW.Code
chmod +x scripts/create-database.sh
./scripts/create-database.sh
```

## 🔗 Configurar Conexão

### 1. Editar appsettings.json

No arquivo `src/MedicSoft.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medicsoft_dev;Username=medicsoft_user;Password=sua_senha_segura;Include Error Detail=true"
  }
}
```

### 2. Testar Conexão

```bash
cd src/MedicSoft.Api

# Testar conexão com o banco
dotnet ef database drop --force
dotnet ef database ensure-created
```

Se não houver erros, a conexão está funcionando!

## 🔄 Executar Migrations

### O que são Migrations?

Migrations são arquivos que contêm as mudanças incrementais no schema do banco de dados. Eles permitem:
- Versionar o banco de dados
- Aplicar mudanças de forma controlada
- Reverter mudanças se necessário

### Visualizar Migrations Pendentes

```bash
cd src/MedicSoft.Api
dotnet ef migrations list
```

### Executar Todas as Migrations

#### Opção 1: Script Automatizado (Recomendado)

```bash
# Windows
.\run-all-migrations.ps1

# macOS/Linux
chmod +x run-all-migrations.sh
./run-all-migrations.sh
```

Este script executa migrations para:
- API Principal (MedicSoft.Api)
- Portal do Paciente (PatientPortal)
- Analytics (MedicSoft.Analytics)
- Telemedicina (Telemedicine)

#### Opção 2: Manual

```bash
# API Principal
cd src/MedicSoft.Api
dotnet ef database update

# Portal do Paciente
cd ../../patient-portal-api
dotnet ef database update

# Analytics
cd ../src/MedicSoft.Analytics
dotnet ef database update

# Telemedicina
cd ../../telemedicine
dotnet ef database update
```

### Verificar Status das Migrations

```bash
cd src/MedicSoft.Api
dotnet ef migrations list

# Saída esperada:
# 20240101000000_InitialCreate (Applied)
# 20240115000000_AddLGPDFeatures (Applied)
# 20240201000000_AddAnalyticsTables (Applied)
# ... mais migrations
```

### Criar Nova Migration (Para Desenvolvedores)

```bash
cd src/MedicSoft.Api
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

## 🌱 Seed de Dados Iniciais

### 1. Executar Seed Automático

O seed cria dados iniciais para testes:
- Tenant de demonstração
- Usuário administrador
- Pacientes de exemplo
- Médicos de exemplo
- Agendamentos de exemplo

```bash
cd src/MedicSoft.Api

# Executar seed
dotnet run --seed

# Ou via script
dotnet run -- --seed
```

### 2. Dados Criados pelo Seed

#### Tenant Demo
- **Nome:** Clínica Demonstração
- **Subdomain:** demo
- **Status:** Ativo

#### Usuário Admin
- **Email:** admin@demo.com
- **Senha:** Admin@123
- **Role:** SystemAdmin

#### Usuário Médico
- **Email:** doctor@demo.com
- **Senha:** Doctor@123
- **Role:** Doctor

#### Usuário Secretária
- **Email:** secretary@demo.com
- **Senha:** Secretary@123
- **Role:** Secretary

#### Pacientes (5-10 exemplos)
- João Silva
- Maria Santos
- Pedro Oliveira
- Ana Costa
- etc.

### 3. Seed Manual (SQL)

Se preferir executar SQL diretamente:

```bash
psql -U medicsoft_user -d medicsoft_dev -f scripts/seed-data.sql
```

## ✅ Verificação

### 1. Verificar Tabelas Criadas

```bash
psql -U medicsoft_user -d medicsoft_dev

# No psql:
\dt

# Deve listar 100+ tabelas, incluindo:
# Tenants
# Users
# Patients
# Doctors
# Appointments
# MedicalRecords
# Prescriptions
# Analytics...
```

### 2. Verificar Dados Seed

```sql
-- Conectar ao banco
psql -U medicsoft_user -d medicsoft_dev

-- Verificar tenants
SELECT * FROM "Tenants";

-- Verificar usuários
SELECT "Id", "Email", "Role" FROM "Users";

-- Verificar pacientes
SELECT COUNT(*) FROM "Patients";

-- Verificar agendamentos
SELECT COUNT(*) FROM "Appointments";
```

### 3. Testar via API

```bash
# Testar conexão do backend
cd src/MedicSoft.Api
dotnet run

# Em outro terminal, testar endpoint
curl http://localhost:5000/api/status
```

Resposta esperada:
```json
{
  "database": "Connected",
  "migrations": "Up to date",
  "tables": 100
}
```

### Checklist de Verificação

- [ ] Banco de dados `medicsoft_dev` criado
- [ ] Usuário `medicsoft_user` criado com permissões
- [ ] Extensões UUID e pg_trgm instaladas
- [ ] Connection string configurada no appsettings.json
- [ ] Todas as migrations executadas com sucesso
- [ ] 100+ tabelas criadas no banco
- [ ] Seed de dados iniciais executado
- [ ] Tenant demo criado
- [ ] Usuários de teste criados
- [ ] Backend conecta ao banco sem erros

## 🔧 Operações Úteis

### Ver Schema de uma Tabela

```sql
\d "Patients"
```

### Ver Índices de uma Tabela

```sql
\di "Patients"
```

### Ver Tamanho do Banco

```sql
SELECT pg_size_pretty(pg_database_size('medicsoft_dev'));
```

### Listar Conexões Ativas

```sql
SELECT * FROM pg_stat_activity WHERE datname = 'medicsoft_dev';
```

### Matar Conexões (se necessário)

```sql
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'medicsoft_dev'
  AND pid <> pg_backend_pid();
```

## 📦 Backup e Restore

### Criar Backup

```bash
# Backup completo
pg_dump -U medicsoft_user -d medicsoft_dev > backup_$(date +%Y%m%d).sql

# Backup apenas schema
pg_dump -U medicsoft_user -d medicsoft_dev --schema-only > schema_backup.sql

# Backup apenas dados
pg_dump -U medicsoft_user -d medicsoft_dev --data-only > data_backup.sql
```

### Restaurar Backup

```bash
# Restaurar backup completo
psql -U medicsoft_user -d medicsoft_dev < backup_20260202.sql

# Ou criar novo banco e restaurar
createdb -U postgres medicsoft_dev_restore
psql -U medicsoft_user -d medicsoft_dev_restore < backup_20260202.sql
```

## 🚨 Problemas Comuns

### Problema: "database does not exist"

**Solução:**
```bash
psql -U postgres
CREATE DATABASE medicsoft_dev;
\q
```

### Problema: "password authentication failed"

**Solução:** Verifique a senha no arquivo `pg_hba.conf`:

```bash
# Localizar pg_hba.conf
# Windows: C:\Program Files\PostgreSQL\16\data\pg_hba.conf
# macOS: /opt/homebrew/var/postgresql@16/pg_hba.conf
# Linux: /etc/postgresql/16/main/pg_hba.conf

# Mudar para:
local   all             all                                     trust
host    all             all             127.0.0.1/32            md5
```

Reinicie o PostgreSQL após alterar.

### Problema: "peer authentication failed"

**Solução (Linux):**
```bash
sudo -u postgres psql
ALTER USER medicsoft_user WITH PASSWORD 'nova_senha';
\q
```

### Problema: Migrations falham

**Solução:**
```bash
# Reverter última migration
dotnet ef database update NomeDaMigrationAnterior

# Ou resetar completamente
dotnet ef database drop --force
dotnet ef database update
```

### Problema: "column already exists"

**Solução:** O banco pode estar em um estado inconsistente:
```bash
# Opção 1: Resetar completamente
dotnet ef database drop --force
dotnet ef database update

# Opção 2: Ajustar migration manualmente
# Edite o arquivo de migration e remova a coluna duplicada
```

## 📊 Monitoramento

### Ver Consultas Lentas

```sql
SELECT pid, now() - pg_stat_activity.query_start AS duration, query
FROM pg_stat_activity
WHERE state = 'active' AND now() - pg_stat_activity.query_start > interval '5 seconds';
```

### Ver Estatísticas de Uso

```sql
SELECT schemaname, tablename, 
       pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC
LIMIT 10;
```

## 📚 Documentação Adicional

- [Migrations Guide](../../MIGRATIONS_GUIDE.md)
- [Entity Diagram](../../system-admin/docs/ENTITY_DIAGRAM.md)
- [Database Schema](../../system-admin/docs/DATABASE_SCHEMA.md)
- [PostgreSQL Official Docs](https://www.postgresql.org/docs/16/)

## ⏭️ Próximos Passos

Agora que o banco de dados está configurado:

1. ✅ PostgreSQL instalado e configurado
2. ✅ Banco de dados criado
3. ✅ Migrations executadas
4. ✅ Dados seed carregados
5. ➡️ Opcionalmente, configure [05-Configuracao-Docker-Podman.md](05-Configuracao-Docker-Podman.md)
6. ➡️ Ou vá direto para os [cenários de testes](../CenariosTestesQA/)

**Sistema totalmente funcional! 🎉**

Faça login em http://localhost:4200 com:
- **Email:** admin@demo.com
- **Senha:** Admin@123

---

**Dúvidas?** Consulte a documentação ou use `psql` para explorar o banco.
