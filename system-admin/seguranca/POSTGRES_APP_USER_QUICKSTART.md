# 🚀 Guia Rápido: Migração para Usuário de Aplicação PostgreSQL

## 📋 Resumo Executivo

Este documento fornece um **guia rápido** para migrar do usuário master (`postgres`) para usuários de aplicação dedicados no PostgreSQL. Para documentação completa, consulte [POSTGRES_APP_USER_GUIDE.md](./POSTGRES_APP_USER_GUIDE.md).

## ⚡ Setup Rápido (5 minutos)

### Opção 1: Script Automático (Recomendado)

```bash
# Linux/Mac
cd scripts
./create-postgres-app-users.sh

# Windows PowerShell
cd scripts
.\create-postgres-app-users.ps1
```

O script irá:
1. ✅ Criar usuários: `omnicare_app`, `patientportal_app`, `telemedicine_app`, `omnicare_readonly`
2. ✅ Gerar senhas seguras automaticamente
3. ✅ Configurar todas as permissões necessárias
4. ✅ Salvar credenciais em arquivo temporário

### Opção 2: Manual (SQL)

```sql
-- Conectar como postgres
psql -U postgres

-- Criar usuário
CREATE USER omnicare_app WITH PASSWORD 'SuaSenhaForte!2024';

-- Conceder permissões
\c primecare
GRANT CONNECT ON DATABASE primecare TO omnicare_app;
GRANT USAGE ON SCHEMA public TO omnicare_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO omnicare_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO omnicare_app;

ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO omnicare_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT USAGE, SELECT ON SEQUENCES TO omnicare_app;
```

## 🔧 Atualizar Connection Strings

### Desenvolvimento (`appsettings.Development.json`)

**ANTES:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=primecare;Username=postgres;Password=Abc!123456"
  }
}
```

**DEPOIS:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=primecare;Username=omnicare_app;Password=SuaSenhaForte!2024"
  }
}
```

### Produção (Variáveis de Ambiente)

```bash
# .env ou servidor de produção
export DB_HOST=seu-servidor.postgres.database.com
export DB_PORT=5432
export DB_NAME=primecare
export DB_USER=omnicare_app
export DB_PASSWORD=SenhaProdução!2024
```

**appsettings.Production.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD};SSL Mode=Require"
  }
}
```

## 🏃 Testar a Mudança

### 1. Testar Conexão

```bash
# Conectar com novo usuário
psql -h localhost -U omnicare_app -d primecare

# Dentro do psql:
SELECT COUNT(*) FROM "Patients";  -- Deve funcionar
```

### 2. Testar Aplicação

```bash
# Iniciar API
cd src/MedicSoft.Api
dotnet run

# Verificar logs
# Deve ver: "Database connection established successfully"
```

### 3. Verificar Isolamento

```sql
-- Conectar como omnicare_app
\c primecare omnicare_app

-- Tentar operação não permitida (deve falhar)
CREATE TABLE test (id INT);
-- ERROR: permission denied for schema public ✅

-- Operação permitida (deve funcionar)
SELECT COUNT(*) FROM "Patients";  ✅
```

## 🔐 Migrations: O Que Fazer?

### Problema
Entity Framework precisa de permissões DDL (CREATE, ALTER, DROP) para migrations, mas o usuário de aplicação não tem essas permissões.

### Solução Recomendada
Use o usuário `postgres` **apenas** para migrations:

```bash
# 1. Aplicar migrations com admin
export DB_ADMIN_USER=postgres
export DB_ADMIN_PASSWORD=senha_admin

dotnet ef database update \
    --context MedicSoftDbContext \
    --project src/MedicSoft.Repository \
    --startup-project src/MedicSoft.Api \
    --connection "Host=localhost;Database=primecare;Username=$DB_ADMIN_USER;Password=$DB_ADMIN_PASSWORD"

# 2. Aplicação usa usuário normal
# (configurado em appsettings.json)
dotnet run --project src/MedicSoft.Api
```

### Alternativa: Usuário de Migrations

Se preferir, crie um usuário dedicado para migrations:

```sql
CREATE USER omnicare_migrations WITH PASSWORD 'SenhaMigrations!2024';
GRANT ALL PRIVILEGES ON DATABASE primecare TO omnicare_migrations;
```

## 📊 Usuários e Suas Funções

| Usuário | Banco(s) | Permissões | Uso |
|---------|----------|------------|-----|
| `postgres` | Todos | Superusuário | Admin, migrations, manutenção |
| `omnicare_app` | primecare | SELECT, INSERT, UPDATE, DELETE | API principal |
| `patientportal_app` | patientportal | SELECT, INSERT, UPDATE, DELETE | Portal do Paciente |
| `telemedicine_app` | telemedicine | SELECT, INSERT, UPDATE, DELETE | Telemedicina |
| `omnicare_readonly` | Todos | SELECT apenas | BI, relatórios, análises |

## ⚠️ Problemas Comuns

### Erro: "permission denied for schema public"

**Solução:**
```sql
GRANT USAGE ON SCHEMA public TO omnicare_app;
```

### Erro: "permission denied for sequence"

**Solução:**
```sql
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO omnicare_app;
```

### Aplicação não conecta

**Debug:**
```bash
# Verificar se usuário existe
psql -U postgres -c "\du omnicare_app"

# Testar conexão
psql -h localhost -U omnicare_app -d primecare
```

### Migration falha

**Solução:**
```bash
# Use usuário admin para migrations
dotnet ef database update --connection "...Username=postgres;..."
```

## 📋 Checklist de Implementação

### Desenvolvimento

- [ ] Executar script de criação de usuários
- [ ] Atualizar `appsettings.Development.json` com novo usuário
- [ ] Testar conexão: `psql -U omnicare_app -d primecare`
- [ ] Testar aplicação: `dotnet run`
- [ ] Verificar logs de conexão
- [ ] Testar operações CRUD básicas

### Produção

- [ ] Criar usuários em servidor de produção
- [ ] Configurar variáveis de ambiente
- [ ] Atualizar `appsettings.Production.json`
- [ ] Configurar SSL/TLS: `SSL Mode=Require`
- [ ] Testar conexão antes de deploy
- [ ] Fazer backup do banco antes de mudança
- [ ] Documentar credenciais em gerenciador seguro (1Password, Vault)
- [ ] Revocar acesso ao usuário `postgres` de aplicações

### Pós-Deploy

- [ ] Monitorar logs de conexão
- [ ] Verificar performance (não deve mudar)
- [ ] Confirmar que aplicação funciona normalmente
- [ ] Deletar arquivo de credenciais temporário
- [ ] Atualizar runbooks de deploy
- [ ] Treinar equipe no novo processo

## 🔗 Documentação Adicional

| Documento | Descrição |
|-----------|-----------|
| [POSTGRES_APP_USER_GUIDE.md](./POSTGRES_APP_USER_GUIDE.md) | Guia completo com detalhes técnicos |
| [SECURITY_GUIDE.md](./SECURITY_GUIDE.md) | Guia geral de segurança |
| [DOCKER_POSTGRES_SETUP.md](../infrastructure/DOCKER_POSTGRES_SETUP.md) | Setup com Docker |

## 💡 Dicas Importantes

1. **NUNCA** commite senhas no Git
2. Use gerenciador de senhas (1Password, Vault)
3. Senhas devem ter no mínimo 24 caracteres
4. Rotacione senhas a cada 90 dias
5. Use SSL/TLS em produção (`SSL Mode=Require`)
6. Monitore logs de conexão regularmente
7. Faça backup antes de grandes mudanças

## 🆘 Suporte

**Problemas?**
1. Consulte [Troubleshooting](./POSTGRES_APP_USER_GUIDE.md#-troubleshooting) no guia completo
2. Verifique logs: `docker compose logs postgres`
3. Entre em contato com DevOps/DBA

---

**Criado**: Fevereiro 2026  
**Versão**: 1.0  
**Próxima Revisão**: Maio 2026
