# 🐳 Podman PostgreSQL Setup Guide

Este guia fornece instruções completas para configurar e executar o PrimeCare Software com PostgreSQL usando Podman (ou Docker como alternativa).

## 🆓 Por que Podman?

O **Podman** é uma alternativa gratuita e open-source ao Docker, ideal para produção:

- ✅ **100% Gratuito**: Licença Apache 2.0, sem custos para uso comercial
- ✅ **Daemonless**: Mais seguro (não requer processo root rodando)
- ✅ **Compatível com Docker**: Usa mesmos comandos e arquivos OCI
- ✅ **Rootless**: Pode rodar sem privilégios root
- ✅ **Production-ready**: Usado por Red Hat, IBM e outras grandes empresas

## 📋 Pré-requisitos

- Podman instalado (versão 4.0 ou superior) **OU** Docker (versão 20.10+)
- Podman Compose instalado **OU** Docker Compose (versão 2.0+)
- 2GB de RAM disponível (mínimo)
- 10GB de espaço em disco

### Instalar Podman

**Linux (Ubuntu/Debian):**
```bash
sudo apt update
sudo apt install -y podman podman-compose
```

**Linux (Fedora/RHEL):**
```bash
sudo dnf install -y podman podman-compose
```

**macOS:**
```bash
brew install podman podman-compose
podman machine init
podman machine start
```

**Windows:**
- Baixar: https://github.com/containers/podman/releases
- Ou use WSL2 com Linux

**Verificar instalação:**
```bash
podman --version
podman-compose --version
```

## 🚀 Quick Start - Desenvolvimento

### 1. Criar arquivo `.env`

Crie um arquivo `.env` na raiz do projeto:

```bash
# .env
POSTGRES_PASSWORD=MedicW@rehouse2024!
JWT_SECRET_KEY=PrimeCare Software-SuperSecretKey-2024-Development-MinLength32Chars!
```

### 2. Iniciar PostgreSQL e a Aplicação

**Com Podman (recomendado):**
```bash
# Iniciar todos os serviços
podman-compose up -d

# Ver logs
podman-compose logs -f

# Verificar status
podman-compose ps
```

**Com Docker (alternativa):**
```bash
# Iniciar todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Verificar status
docker-compose ps
```

### 3. Aplicar Migrations

**Com Podman:**
```bash
# Aplicar migrations no banco de dados
podman-compose exec api dotnet ef database update --context MedicSoftDbContext

# Ou, se preferir executar localmente:
dotnet ef database update --context MedicSoftDbContext --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

**Com Docker:**
```bash
docker-compose exec api dotnet ef database update --context MedicSoftDbContext
```

### 4. Acessar a Aplicação

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Frontend**: http://localhost:4200
- **System Admin**: http://localhost:4201

## 🔧 Configuração Detalhada

### Apenas PostgreSQL (sem API)

Se você quer executar apenas o PostgreSQL localmente:

**Com Podman:**
```bash
# Iniciar apenas o PostgreSQL
podman-compose up postgres -d

# Verificar se está rodando
podman-compose ps postgres

# Ver logs do PostgreSQL
podman-compose logs -f postgres
```

**Com Docker:**
```bash
docker-compose up postgres -d
docker-compose ps postgres
docker-compose logs -f postgres
```

### Conectar ao PostgreSQL

**Com Podman:**
```bash
# Via Podman
podman-compose exec postgres psql -U postgres -d medicwarehouse

# Via psql local (se instalado)
psql -h localhost -p 5432 -U postgres -d medicwarehouse
```

**Com Docker:**
```bash
docker-compose exec postgres psql -U postgres -d medicwarehouse
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

## 🏗️ Estrutura do podman-compose.yml

O arquivo `podman-compose.yml` (também compatível com `docker-compose.yml`) define os serviços:

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

**Com Podman:**
```bash
# Backup completo
podman-compose exec postgres pg_dump -U postgres medicwarehouse > backup_$(date +%Y%m%d_%H%M%S).sql

# Backup apenas schema
podman-compose exec postgres pg_dump -U postgres --schema-only medicwarehouse > schema_backup.sql

# Backup apenas dados
podman-compose exec postgres pg_dump -U postgres --data-only medicwarehouse > data_backup.sql
```

**Com Docker:**
```bash
docker-compose exec postgres pg_dump -U postgres medicwarehouse > backup_$(date +%Y%m%d_%H%M%S).sql
```

### Restaurar Backup

**Com Podman:**
```bash
# Restaurar de um backup
podman-compose exec -T postgres psql -U postgres medicwarehouse < backup.sql

# Ou, se o banco não existir ainda:
podman-compose exec postgres createdb -U postgres medicwarehouse
podman-compose exec -T postgres psql -U postgres medicwarehouse < backup.sql
```

**Com Docker:**
```bash
docker-compose exec -T postgres psql -U postgres medicwarehouse < backup.sql
```

## 🔍 Troubleshooting

### Erro: "unable to copy from source" ou "i/o timeout" ao fazer pull da imagem

Se você encontrar erros de timeout ao tentar baixar a imagem do PostgreSQL:

```
Error: unable to copy from source docker://postgres:16-alpine: copying system image from manifest list: parsing image configuration: Get "https://...": dial tcp ...: i/o timeout
```

**Causa:** Problemas de conectividade com o Docker Hub via CDN Cloudflare.

**Solução 1 - Pré-baixar a imagem (Recomendado):**
```bash
# Com Podman - tentar baixar manualmente
podman pull docker.io/library/postgres:16-alpine

# Se ainda falhar, tentar com alternativa do Quay.io
podman pull quay.io/fedora/postgresql:16

# Depois iniciar os serviços
podman-compose up postgres -d
```

**Solução 2 - Usar registro alternativo:**
```bash
# Editar registries.conf (Linux)
sudo nano /etc/containers/registries.conf

# Adicionar registros alternativos no início da lista:
[registries.search]
registries = ['quay.io', 'docker.io']
```

**Solução 3 - Configurar timeout maior:**
```bash
# Editar podman.conf (Linux)
sudo nano /etc/containers/containers.conf

# Adicionar na seção [engine]:
[engine]
image_pull_timeout = "10m"
```

**Solução 4 - Aguardar e tentar novamente:**
```bash
# Às vezes o problema é temporário no CDN
# Aguarde alguns minutos e tente novamente
podman-compose up postgres -d
```

**Nota:** Os arquivos compose já foram atualizados para usar a imagem qualificada `docker.io/library/postgres:16-alpine` e `pull_policy: missing` para evitar pulls desnecessários.

### Erro: "database does not exist"

**Com Podman:**
```bash
# Criar o banco manualmente
podman-compose exec postgres createdb -U postgres medicwarehouse

# Aplicar migrations
dotnet ef database update --context MedicSoftDbContext --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

**Com Docker:**
```bash
docker-compose exec postgres createdb -U postgres medicwarehouse
```

### Erro: "password authentication failed"

Verifique se a senha no `.env` corresponde à senha no `appsettings.json`:

**Com Podman:**
```bash
# Resetar senha do PostgreSQL
podman-compose down
podman volume rm mwcode_postgres_data
podman-compose up -d
```

**Com Docker:**
```bash
docker-compose down
docker volume rm mwcode_postgres_data
docker-compose up -d
```

### Erro: "port 5432 already in use"

```bash
# Verificar o que está usando a porta
sudo lsof -i :5432

# Parar o PostgreSQL local
sudo systemctl stop postgresql

# Ou, mudar a porta no podman-compose.yml:
ports:
  - "5433:5432"  # Mapeia para porta 5433 no host
```

### Container não inicia

**Com Podman:**
```bash
# Ver logs detalhados
podman-compose logs postgres

# Remover volumes e reiniciar
podman-compose down -v
podman-compose up -d
```

**Com Docker:**
```bash
docker-compose logs postgres
docker-compose down -v
docker-compose up -d
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

**Com Podman:**
```bash
# Conectar ao PostgreSQL
podman-compose exec postgres psql -U postgres medicwarehouse

# Truncar tabelas (mantém estrutura)
TRUNCATE TABLE Patients, Appointments, Clinics CASCADE;

# Ou resetar completamente o banco
DROP DATABASE medicwarehouse;
CREATE DATABASE medicwarehouse;
```

**Com Docker:**
```bash
docker-compose exec postgres psql -U postgres medicwarehouse
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

**Com Podman:**
```bash
# Conectar ao PostgreSQL
podman-compose exec postgres psql -U postgres medicwarehouse

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

**Com Docker:**
```bash
docker-compose exec postgres psql -U postgres medicwarehouse
```

### Estatísticas do Container

**Com Podman:**
```bash
# Ver uso de recursos
podman stats medicwarehouse-postgres

# Ver logs em tempo real
podman-compose logs -f postgres
```

**Com Docker:**
```bash
docker stats medicwarehouse-postgres
docker-compose logs -f postgres
```

## 🧹 Manutenção

### Vacuum (Limpeza)

**Com Podman:**
```bash
# Conectar ao PostgreSQL
podman-compose exec postgres psql -U postgres medicwarehouse

# Executar vacuum
VACUUM ANALYZE;

# Vacuum completo (mais intensivo)
VACUUM FULL;
```

**Com Docker:**
```bash
docker-compose exec postgres psql -U postgres medicwarehouse
```

### Atualizar Estatísticas

Similar ao comando acima - conecte ao PostgreSQL e execute `ANALYZE;`

## 🎯 Ambiente de Produção

Para produção, use o `podman-compose.production.yml`:

**Com Podman:**
```bash
# Iniciar em produção
podman-compose -f podman-compose.production.yml up -d

# Ver documentação de produção
cat DEPLOY_RAILWAY_GUIDE.md
```

**Com Docker:**
```bash
docker-compose -f podman-compose.production.yml up -d
```

### Diferenças para Produção

1. **Volumes persistentes** configurados
2. **Limites de recursos** definidos
3. **Healthchecks** habilitados
4. **Backups automáticos** (via Railway/Render)
5. **SSL/TLS** obrigatório

## 📚 Recursos Adicionais

- [Documentação do Podman](https://docs.podman.io/)
- [Documentação do PostgreSQL](https://www.postgresql.org/docs/)
- [Guia de Migração SQL Server → PostgreSQL](MIGRACAO_POSTGRESQL.md)
- [Podman Compose Documentation](https://github.com/containers/podman-compose)
- [Npgsql - .NET PostgreSQL Provider](https://www.npgsql.org/)

## ✅ Checklist de Setup

- [ ] Podman (ou Docker) instalado
- [ ] Podman Compose (ou Docker Compose) instalado
- [ ] Arquivo `.env` criado com senhas seguras
- [ ] `podman-compose up -d` executado com sucesso
- [ ] PostgreSQL está rodando (`podman-compose ps`)
- [ ] Migrations aplicadas (`dotnet ef database update`)
- [ ] API acessível em http://localhost:5000/swagger
- [ ] Frontend acessível em http://localhost:4200
- [ ] Dados de teste carregados (seeders)

---

**Criado por**: GitHub Copilot  
**Versão**: 2.0  
**Data**: Novembro 2024  
**Última atualização**: Migração Docker → Podman (mantendo compatibilidade com Docker)
