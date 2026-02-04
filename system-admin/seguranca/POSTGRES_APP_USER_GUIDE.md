# 🔐 Guia de Segurança: Usuários de Aplicação PostgreSQL

## 📋 Visão Geral

Este documento fornece instruções detalhadas sobre como criar e configurar usuários de aplicação PostgreSQL para o **Omni Care Software**, garantindo que o usuário master do banco de dados (`postgres` ou outro superusuário) **nunca seja usado diretamente** nas connection strings da aplicação.

## ⚠️ Por Que Não Usar o Usuário Master?

### Riscos de Segurança

1. **Privilégios Excessivos**: O usuário master tem acesso total ao banco, incluindo:
   - Criar/deletar databases
   - Criar/deletar usuários
   - Modificar configurações do servidor
   - Acessar dados de outros bancos
   
2. **Violação do Princípio do Menor Privilégio**: A aplicação só precisa de:
   - SELECT, INSERT, UPDATE, DELETE em tabelas específicas
   - EXECUTE em sequences
   - Não precisa de DDL (CREATE, ALTER, DROP)

3. **Dificulta Auditoria**: Impossível distinguir entre:
   - Ações administrativas legítimas
   - Ações da aplicação
   - Possíveis comprometimentos

4. **Compliance (LGPD/HIPAA)**: Regulamentações exigem:
   - Separação de privilégios
   - Auditoria detalhada de acessos
   - Controle granular de permissões

### Benefícios de Usuários de Aplicação

✅ **Isolamento**: Aplicação não pode afetar estrutura do banco  
✅ **Auditoria**: Logs identificam claramente origem das queries  
✅ **Segurança**: Comprometimento da aplicação não compromete o banco  
✅ **Compliance**: Atende requisitos de separação de privilégios  
✅ **Rollback Fácil**: Possível revogar permissões sem afetar admin  

## 🏗️ Estrutura de Usuários Recomendada

### Três Tipos de Usuários

```
┌─────────────────────────────────────────────┐
│  postgres (master) - Apenas Admin/DBA       │
│  ↓                                          │
│  omnicare_app - Aplicação Principal         │
│  ↓                                          │
│  omnicare_readonly - Leitura/Relatórios     │
└─────────────────────────────────────────────┘
```

1. **postgres/master**: Administração e migrations apenas
2. **omnicare_app**: Aplicação principal (API)
3. **omnicare_readonly**: Consultas, BI, relatórios

## 🚀 Criação dos Usuários de Aplicação

### Passo 1: Conectar como Administrador

```bash
# Via Docker/Podman
docker compose exec postgres psql -U postgres

# Via psql local
psql -h localhost -U postgres -d primecare
```

### Passo 2: Criar Usuário de Aplicação Principal

```sql
-- ================================================
-- USUÁRIO PRINCIPAL DA APLICAÇÃO
-- ================================================

-- 1. Criar usuário com senha forte
CREATE USER omnicare_app WITH PASSWORD 'SuaSenhaForteAqui!2024';

-- 2. Comentário para documentação
COMMENT ON ROLE omnicare_app IS 'Usuário para a aplicação Omni Care Software - Acesso completo DML';

-- 3. Configurar parâmetros de conexão
ALTER ROLE omnicare_app SET statement_timeout = '30s';  -- Timeout de 30s para queries
ALTER ROLE omnicare_app SET idle_in_transaction_session_timeout = '60s';  -- Timeout para transações ociosas

-- 4. Garantir acesso ao database
GRANT CONNECT ON DATABASE primecare TO omnicare_app;
GRANT USAGE ON SCHEMA public TO omnicare_app;

-- 5. Permissões em todas as tabelas existentes
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO omnicare_app;

-- 6. Permissões em sequences (para auto-increment)
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO omnicare_app;

-- 7. Permissões padrão para tabelas futuras
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO omnicare_app;

ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT USAGE, SELECT ON SEQUENCES TO omnicare_app;

-- 8. Verificar permissões
\du omnicare_app
```

### Passo 3: Criar Usuário Readonly (Opcional)

```sql
-- ================================================
-- USUÁRIO READONLY (BI/RELATÓRIOS)
-- ================================================

-- 1. Criar usuário
CREATE USER omnicare_readonly WITH PASSWORD 'OutraSenhaForte!2024';

COMMENT ON ROLE omnicare_readonly IS 'Usuário somente leitura para relatórios e BI';

-- 2. Configurar timeouts
ALTER ROLE omnicare_readonly SET statement_timeout = '120s';  -- Queries mais longas permitidas
ALTER ROLE omnicare_readonly SET default_transaction_read_only = on;  -- Somente leitura

-- 3. Garantir acesso
GRANT CONNECT ON DATABASE primecare TO omnicare_readonly;
GRANT USAGE ON SCHEMA public TO omnicare_readonly;

-- 4. Apenas SELECT
GRANT SELECT ON ALL TABLES IN SCHEMA public TO omnicare_readonly;

-- 5. Permissões padrão para tabelas futuras
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT SELECT ON TABLES TO omnicare_readonly;

-- 6. Verificar
\du omnicare_readonly
```

## 📦 Permissões por Banco de Dados

O Omni Care Software usa **três bancos de dados separados**:

### 1. Banco Principal (primecare)

```sql
-- Conectar ao banco principal
\c primecare

-- Garantir permissões
GRANT CONNECT ON DATABASE primecare TO omnicare_app;
GRANT USAGE ON SCHEMA public TO omnicare_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO omnicare_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO omnicare_app;

ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO omnicare_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT USAGE, SELECT ON SEQUENCES TO omnicare_app;
```

### 2. Portal do Paciente (patientportal)

```sql
-- Criar banco se não existir
CREATE DATABASE patientportal;

-- Conectar
\c patientportal

-- Criar usuário específico ou usar o mesmo
CREATE USER patientportal_app WITH PASSWORD 'SenhaPortalPaciente!2024';

GRANT CONNECT ON DATABASE patientportal TO patientportal_app;
GRANT USAGE ON SCHEMA public TO patientportal_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO patientportal_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO patientportal_app;

ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO patientportal_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT USAGE, SELECT ON SEQUENCES TO patientportal_app;
```

### 3. Telemedicina (telemedicine)

```sql
-- Criar banco se não existir
CREATE DATABASE telemedicine;

-- Conectar
\c telemedicine

-- Criar usuário específico
CREATE USER telemedicine_app WITH PASSWORD 'SenhaTelemedicina!2024';

GRANT CONNECT ON DATABASE telemedicine TO telemedicine_app;
GRANT USAGE ON SCHEMA public TO telemedicine_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO telemedicine_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO telemedicine_app;

ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO telemedicine_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT USAGE, SELECT ON SEQUENCES TO telemedicine_app;
```

## 🔧 Aplicar Migrations com Usuário de Aplicação

### Problema: Entity Framework precisa de DDL

Entity Framework Migrations precisa de permissões para:
- CREATE TABLE
- ALTER TABLE
- CREATE INDEX
- DROP (em alguns casos)

### Solução 1: Usuário Admin para Migrations (Recomendado)

```bash
# Usar usuário master apenas para migrations
export DB_ADMIN_USER=postgres
export DB_ADMIN_PASSWORD=senha_admin

dotnet ef database update \
    --context MedicSoftDbContext \
    --project src/MedicSoft.Repository \
    --startup-project src/MedicSoft.Api \
    --connection "Host=localhost;Database=primecare;Username=$DB_ADMIN_USER;Password=$DB_ADMIN_PASSWORD"

# Aplicação usa usuário normal
export DB_APP_USER=omnicare_app
export DB_APP_PASSWORD=senha_app
```

### Solução 2: Conceder Permissões DDL Temporárias

```sql
-- APENAS durante migrations
GRANT CREATE ON SCHEMA public TO omnicare_app;

-- Executar migrations...

-- REVOGAR após migrations
REVOKE CREATE ON SCHEMA public FROM omnicare_app;
```

### Solução 3: Usuário Dedicado para Migrations

```sql
-- Criar usuário específico para migrations
CREATE USER omnicare_migrations WITH PASSWORD 'SenhaMigrations!2024';

GRANT CONNECT ON DATABASE primecare TO omnicare_migrations;
GRANT USAGE, CREATE ON SCHEMA public TO omnicare_migrations;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO omnicare_migrations;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO omnicare_migrations;

ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT ALL PRIVILEGES ON TABLES TO omnicare_migrations;
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT ALL PRIVILEGES ON SEQUENCES TO omnicare_migrations;
```

## 🔑 Configurar Connection Strings

### appsettings.json (Desenvolvimento)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=primecare;Username=omnicare_app;Password=SuaSenhaForteAqui!2024;Include Error Detail=true"
  }
}
```

### appsettings.Production.json (Produção)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD};SSL Mode=Require;Trust Server Certificate=false"
  }
}
```

### Variáveis de Ambiente (Produção - Recomendado)

```bash
# .env ou configuração do servidor
export DB_HOST=seu-servidor.postgres.database.azure.com
export DB_PORT=5432
export DB_NAME=primecare
export DB_USER=omnicare_app
export DB_PASSWORD=SuaSenhaSeguraGeradaPorGerenciadorDeSenhas!2024
```

### Docker Compose

```yaml
services:
  api:
    image: omnicare-api
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=primecare;Username=omnicare_app;Password=${DB_PASSWORD}
    depends_on:
      - postgres
  
  postgres:
    image: postgres:16-alpine
    environment:
      - POSTGRES_DB=primecare
      - POSTGRES_USER=postgres  # Apenas para admin
      - POSTGRES_PASSWORD=${POSTGRES_ADMIN_PASSWORD}
```

## 🛡️ Boas Práticas de Segurança

### 1. Senhas Fortes

```bash
# Gerar senha segura
openssl rand -base64 32

# Ou
pwgen -s 32 1
```

**Requisitos**:
- Mínimo 24 caracteres
- Letras maiúsculas e minúsculas
- Números
- Caracteres especiais
- **NUNCA** use senhas padrão como "postgres", "admin", "123456"

### 2. Rotação de Senhas

```sql
-- Alterar senha do usuário de aplicação
ALTER USER omnicare_app WITH PASSWORD 'NovaSenhaForte!2025';

-- Atualizar connection strings na aplicação
-- Reiniciar aplicação
```

**Recomendação**: Rotacionar senhas a cada 90 dias

### 3. Conexões SSL/TLS

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-server;Database=primecare;Username=omnicare_app;Password=***;SSL Mode=Require;Trust Server Certificate=false"
  }
}
```

### 4. Limitar Conexões

```sql
-- Limitar número de conexões simultâneas
ALTER USER omnicare_app CONNECTION LIMIT 100;

-- Ver conexões ativas
SELECT usename, count(*) 
FROM pg_stat_activity 
WHERE usename = 'omnicare_app' 
GROUP BY usename;
```

### 5. IP Whitelist (se aplicável)

```bash
# pg_hba.conf
# TYPE  DATABASE    USER            ADDRESS         METHOD
host    primecare   omnicare_app    10.0.0.0/8      scram-sha-256
host    primecare   postgres        127.0.0.1/32    scram-sha-256
```

## 🔍 Auditoria e Monitoramento

### Verificar Permissões Atuais

```sql
-- Verificar permissões de um usuário
\dp  -- List table permissions
\du omnicare_app  -- User details

-- Query detalhada
SELECT 
    grantee, 
    table_schema, 
    table_name, 
    privilege_type 
FROM information_schema.table_privileges 
WHERE grantee = 'omnicare_app'
ORDER BY table_name;
```

### Monitorar Queries do Usuário

```sql
-- Queries ativas do usuário
SELECT 
    pid,
    usename,
    application_name,
    client_addr,
    state,
    query,
    query_start
FROM pg_stat_activity
WHERE usename = 'omnicare_app'
ORDER BY query_start DESC;

-- Queries lentas
SELECT 
    pid,
    now() - pg_stat_activity.query_start AS duration,
    usename,
    query
FROM pg_stat_activity
WHERE usename = 'omnicare_app'
  AND state = 'active'
  AND now() - pg_stat_activity.query_start > interval '5 seconds';
```

### Log de Conexões

```sql
-- Habilitar log de conexões (postgresql.conf)
log_connections = on
log_disconnections = on
log_statement = 'mod'  -- Log INSERT/UPDATE/DELETE

-- Ver logs
tail -f /var/log/postgresql/postgresql-16-main.log
```

## 🧪 Testes de Segurança

### Teste 1: Verificar Isolamento

```sql
-- Conectar como omnicare_app
\c primecare omnicare_app

-- Tentar criar tabela (deve falhar)
CREATE TABLE test_table (id INT);
-- ERROR: permission denied for schema public

-- Tentar criar database (deve falhar)
CREATE DATABASE test_db;
-- ERROR: permission denied to create database

-- Tentar acessar outro database (deve falhar)
\c postgres
-- FATAL: permission denied for database "postgres"
```

### Teste 2: Verificar Operações DML

```sql
-- Conectar como omnicare_app
\c primecare omnicare_app

-- SELECT (deve funcionar)
SELECT COUNT(*) FROM "Patients";

-- INSERT (deve funcionar)
INSERT INTO "Clinics" ("Name", "TenantId", "CreatedAt") 
VALUES ('Test Clinic', 'test-tenant', NOW());

-- UPDATE (deve funcionar)
UPDATE "Clinics" SET "Name" = 'Updated Clinic' WHERE "Name" = 'Test Clinic';

-- DELETE (deve funcionar)
DELETE FROM "Clinics" WHERE "Name" = 'Updated Clinic';
```

### Teste 3: Verificar Readonly

```sql
-- Conectar como omnicare_readonly
\c primecare omnicare_readonly

-- SELECT (deve funcionar)
SELECT COUNT(*) FROM "Patients";

-- INSERT (deve falhar)
INSERT INTO "Patients" ("Name") VALUES ('Test');
-- ERROR: permission denied for table "Patients"
```

## 📋 Checklist de Implementação

### Setup Inicial

- [ ] **Conectar ao PostgreSQL como administrador**
- [ ] **Criar usuário `omnicare_app` com senha forte**
- [ ] **Conceder permissões DML (SELECT, INSERT, UPDATE, DELETE)**
- [ ] **Conceder permissões em sequences**
- [ ] **Configurar default privileges para tabelas futuras**
- [ ] **Criar usuário `omnicare_readonly` (opcional)**
- [ ] **Criar usuários para bancos adicionais (patientportal, telemedicine)**

### Configuração da Aplicação

- [ ] **Atualizar `appsettings.Development.json` com novo usuário**
- [ ] **Atualizar `appsettings.Production.json` com variáveis de ambiente**
- [ ] **Configurar variáveis de ambiente no servidor de produção**
- [ ] **Atualizar Docker Compose / Kubernetes secrets**
- [ ] **Remover qualquer referência ao usuário `postgres` em configs**

### Migrations

- [ ] **Decidir estratégia de migrations (Admin vs DDL temporário vs Migrations user)**
- [ ] **Criar script de migration separado se necessário**
- [ ] **Testar migrations em ambiente de desenvolvimento**
- [ ] **Documentar processo de migration para equipe**

### Testes

- [ ] **Testar conexão com novo usuário**
- [ ] **Verificar que operações DML funcionam (SELECT, INSERT, UPDATE, DELETE)**
- [ ] **Verificar que DDL falha (CREATE, ALTER, DROP)**
- [ ] **Testar isolamento entre databases**
- [ ] **Testar usuário readonly (se aplicável)**

### Segurança

- [ ] **Habilitar SSL/TLS para conexões**
- [ ] **Configurar pg_hba.conf para restringir IPs (se aplicável)**
- [ ] **Limitar número de conexões do usuário**
- [ ] **Habilitar logging de conexões e modificações**
- [ ] **Configurar rotação de senhas (90 dias)**

### Documentação

- [ ] **Documentar senhas em gerenciador seguro (1Password, Vault)**
- [ ] **Atualizar runbooks de deploy**
- [ ] **Treinar equipe no novo processo**
- [ ] **Documentar processo de troubleshooting**

### Monitoramento

- [ ] **Configurar alertas para falhas de conexão**
- [ ] **Monitorar queries lentas do usuário de aplicação**
- [ ] **Revisar logs de acesso mensalmente**
- [ ] **Auditar permissões trimestralmente**

## 🚨 Troubleshooting

### Erro: "permission denied for schema public"

```sql
-- Conceder USAGE no schema
GRANT USAGE ON SCHEMA public TO omnicare_app;
```

### Erro: "permission denied for sequence"

```sql
-- Conceder permissão em sequences
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO omnicare_app;

-- Para sequences futuras
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT USAGE, SELECT ON SEQUENCES TO omnicare_app;
```

### Erro: "must be owner of table"

```sql
-- Transferir ownership (se realmente necessário)
ALTER TABLE "TableName" OWNER TO omnicare_app;

-- Ou conceder permissões específicas
GRANT SELECT, INSERT, UPDATE, DELETE ON "TableName" TO omnicare_app;
```

### Erro: "cannot execute UPDATE in a read-only transaction"

```sql
-- Desabilitar read-only mode
ALTER ROLE omnicare_app SET default_transaction_read_only = off;
```

### Conexão recusada

```bash
# Verificar se usuário existe
psql -h localhost -U postgres -c "\du omnicare_app"

# Verificar pg_hba.conf
sudo cat /etc/postgresql/16/main/pg_hba.conf

# Recarregar configuração
sudo systemctl reload postgresql
```

## 📚 Referências

- [PostgreSQL GRANT Documentation](https://www.postgresql.org/docs/current/sql-grant.html)
- [PostgreSQL Role Attributes](https://www.postgresql.org/docs/current/role-attributes.html)
- [Npgsql Connection Strings](https://www.npgsql.org/doc/connection-string-parameters.html)
- [PostgreSQL Security Best Practices](https://www.postgresql.org/docs/current/security.html)
- [OWASP Database Security](https://cheatsheetseries.owasp.org/cheatsheets/Database_Security_Cheat_Sheet.html)

## 📞 Suporte

Para dúvidas sobre configuração de usuários de aplicação:
- Consulte este documento primeiro
- Verifique logs de conexão: `docker compose logs postgres`
- Entre em contato com o time de DevOps/DBA

---

**Última Atualização**: Fevereiro 2026  
**Versão**: 1.0  
**Autor**: Equipe de Segurança Omni Care Software  
**Status**: Produção
