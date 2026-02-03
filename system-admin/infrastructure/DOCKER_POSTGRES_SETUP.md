# 🐳 Docker PostgreSQL Setup Guide

Este guia fornece instruções completas para configurar e executar o Omni Care Software com PostgreSQL usando Docker.

## 📋 Pré-requisitos

- Docker instalado (versão 20.10 ou superior)
- Docker Compose instalado (versão 2.0 ou superior)
- 2GB de RAM disponível (mínimo)
- 10GB de espaço em disco

## 🚀 Quick Start - Desenvolvimento

### 1. Criar arquivo `.env`

Crie um arquivo `.env` na raiz do projeto:

```bash
# .env
POSTGRES_PASSWORD=MedicW@rehouse2024!
JWT_SECRET_KEY=Omni Care Software-SuperSecretKey-2024-Development-MinLength32Chars!
```

### 2. Iniciar PostgreSQL e a Aplicação

```bash
# Iniciar todos os serviços
docker compose up -d

# Ver logs
docker compose logs -f

# Verificar status
docker compose ps
```

### 3. Aplicar Migrations

```bash
# Aplicar migrations no banco de dados
docker compose exec api dotnet ef database update --context MedicSoftDbContext

# Ou, se preferir executar localmente:
dotnet ef database update --context MedicSoftDbContext --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

### 4. Acessar a Aplicação

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Frontend**: http://localhost:4200
- **System Admin**: http://localhost:4201

## 🔧 Configuração Detalhada

### Apenas PostgreSQL (sem API)

Se você quer executar apenas o PostgreSQL localmente:

```bash
# Iniciar apenas o PostgreSQL
docker compose up postgres -d

# Verificar se está rodando
docker compose ps postgres

# Ver logs do PostgreSQL
docker compose logs -f postgres
```

### Conectar ao PostgreSQL

```bash
# Via Docker
docker compose exec postgres psql -U postgres -d medicwarehouse

# Via psql local (se instalado)
psql -h localhost -p 5432 -U postgres -d medicwarehouse
```

### Comandos Úteis do PostgreSQL

```sql
-- Listar todas as tabelas
\dt

-- Descrever uma tabela
\d Patients

-- Ver todas as databases
\l

-- Ver conexões ativas
SELECT * FROM pg_stat_activity;

-- Ver tamanho do banco
SELECT pg_size_pretty(pg_database_size('medicwarehouse'));
```

## 🏗️ Estrutura do docker-compose.yml

### Serviço PostgreSQL

```yaml
postgres:
  image: postgres:16-alpine
  container_name: medicwarehouse-postgres
  environment:
    POSTGRES_DB: medicwarehouse
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
  ports:
    - "5432:5432"
  volumes:
    - postgres_data:/var/lib/postgresql/data
  healthcheck:
    test: ["CMD-SHELL", "pg_isready -U postgres -d medicwarehouse"]
    interval: 10s
    timeout: 5s
    retries: 5
```

### Connection String

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medicwarehouse;Username=postgres;Password=postgres;Include Error Detail=true"
  }
}
```

## 📦 Backup e Restore

### Criar Backup

```bash
# Backup completo
docker compose exec postgres pg_dump -U postgres medicwarehouse > backup_$(date +%Y%m%d_%H%M%S).sql

# Backup apenas schema
docker compose exec postgres pg_dump -U postgres --schema-only medicwarehouse > schema_backup.sql

# Backup apenas dados
docker compose exec postgres pg_dump -U postgres --data-only medicwarehouse > data_backup.sql
```

### Restaurar Backup

```bash
# Restaurar de um backup
docker compose exec -T postgres psql -U postgres medicwarehouse < backup.sql

# Ou, se o banco não existir ainda:
docker compose exec postgres createdb -U postgres medicwarehouse
docker compose exec -T postgres psql -U postgres medicwarehouse < backup.sql
```

## 🔍 Troubleshooting

### Erro: "database does not exist"

```bash
# Criar o banco manualmente
docker compose exec postgres createdb -U postgres medicwarehouse

# Aplicar migrations
dotnet ef database update --context MedicSoftDbContext --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

### Erro: "password authentication failed"

Verifique se a senha no `.env` corresponde à senha no `appsettings.json`:

```bash
# Resetar senha do PostgreSQL
docker compose down
docker volume rm mwcode_postgres_data
docker compose up -d
```

### Erro: "port 5432 already in use"

```bash
# Verificar o que está usando a porta
sudo lsof -i :5432

# Parar o PostgreSQL local
sudo systemctl stop postgresql

# Ou, mudar a porta no docker-compose.yml:
ports:
  - "5433:5432"  # Mapeia para porta 5433 no host
```

### Container não inicia

```bash
# Ver logs detalhados
docker compose logs postgres

# Remover volumes e reiniciar
docker compose down -v
docker compose up -d
```

## 🗄️ Gerenciamento de Dados

### Seeders (Carga Inicial)

```bash
# Executar seeders via API
curl -X POST http://localhost:5000/api/dev/seed

# Ou via dotnet (se implementado)
dotnet run --project src/MedicSoft.Api -- seed
```

### Limpar Dados de Teste

```bash
# Conectar ao PostgreSQL
docker compose exec postgres psql -U postgres medicwarehouse

# Truncar tabelas (mantém estrutura)
TRUNCATE TABLE Patients, Appointments, Clinics CASCADE;

# Ou resetar completamente o banco
DROP DATABASE medicwarehouse;
CREATE DATABASE medicwarehouse;
```

## 🔐 Segurança

### Produção

⚠️ **NUNCA** use senhas padrão em produção!

```bash
# Gerar senha forte
openssl rand -base64 32

# Configurar no .env
POSTGRES_PASSWORD=sua_senha_super_forte_aqui
```

### Restrições de Acesso

Para produção, não exponha a porta 5432 diretamente:

```yaml
postgres:
  # Remove o ports: para não expor para fora do Docker
  # ports:
  #   - "5432:5432"
```

## 📊 Monitoramento

### Ver Performance

```bash
# Conectar ao PostgreSQL
docker compose exec postgres psql -U postgres medicwarehouse

# Ver queries lentas
SELECT pid, now() - pg_stat_activity.query_start AS duration, query 
FROM pg_stat_activity 
WHERE state = 'active' AND now() - pg_stat_activity.query_start > interval '5 seconds';

# Ver tamanho das tabelas
SELECT schemaname, tablename, 
  pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables 
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

### Estatísticas do Container

```bash
# Ver uso de recursos
docker stats medicwarehouse-postgres

# Ver logs em tempo real
docker compose logs -f postgres
```

## 🧹 Manutenção

### Vacuum (Limpeza)

```bash
# Conectar ao PostgreSQL
docker compose exec postgres psql -U postgres medicwarehouse

# Executar vacuum
VACUUM ANALYZE;

# Vacuum completo (mais intensivo)
VACUUM FULL;
```

### Atualizar Estatísticas

```bash
# Conectar ao PostgreSQL
docker compose exec postgres psql -U postgres medicwarehouse

# Atualizar estatísticas
ANALYZE;
```

## 🎯 Ambiente de Produção

Para produção, use o `docker-compose.production.yml`:

```bash
# Iniciar em produção
docker compose -f docker-compose.production.yml up -d

# Ver documentação de produção
cat DEPLOY_RAILWAY_GUIDE.md
```

### Diferenças para Produção

1. **Volumes persistentes** configurados
2. **Limites de recursos** definidos
3. **Healthchecks** habilitados
4. **Backups automáticos** (via Railway/Render)
5. **SSL/TLS** obrigatório

## 📚 Recursos Adicionais

- [Documentação do PostgreSQL](https://www.postgresql.org/docs/)
- [Guia de Migração SQL Server → PostgreSQL](MIGRACAO_POSTGRESQL.md)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Npgsql - .NET PostgreSQL Provider](https://www.npgsql.org/)

## ✅ Checklist de Setup

- [ ] Docker e Docker Compose instalados
- [ ] Arquivo `.env` criado com senhas seguras
- [ ] `docker compose up -d` executado com sucesso
- [ ] PostgreSQL está rodando (`docker compose ps`)
- [ ] Migrations aplicadas (`dotnet ef database update`)
- [ ] API acessível em http://localhost:5000/swagger
- [ ] Frontend acessível em http://localhost:4200
- [ ] Dados de teste carregados (seeders)

---

**Criado por**: GitHub Copilot  
**Versão**: 1.0  
**Data**: Novembro 2024  
**Última atualização**: Migração para PostgreSQL completa
