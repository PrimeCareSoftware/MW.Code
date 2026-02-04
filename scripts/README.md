# Scripts de Manutenção e Configuração

Este diretório contém scripts úteis para configuração, manutenção e administração do sistema Omni Care Software.

## 📋 Scripts Disponíveis

### Segurança e Banco de Dados

#### `create-postgres-app-users.sh` / `create-postgres-app-users.ps1`
Cria usuários de aplicação PostgreSQL com permissões mínimas necessárias, seguindo o princípio do menor privilégio.

**Uso:**
```bash
# Linux/Mac
./create-postgres-app-users.sh

# Windows PowerShell
.\create-postgres-app-users.ps1
```

**O que faz:**
- Cria usuários: `omnicare_app`, `patientportal_app`, `telemedicine_app`, `omnicare_readonly`
- Gera senhas seguras automaticamente (32 caracteres)
- Configura permissões DML (SELECT, INSERT, UPDATE, DELETE)
- Configura permissões em sequences
- Define timeouts e configurações de segurança
- Salva credenciais em arquivo temporário

**Pré-requisitos:**
- PostgreSQL instalado e rodando
- `psql` disponível no PATH
- Acesso como superusuário (postgres)

**Documentação:**
- [Guia Completo](../system-admin/seguranca/POSTGRES_APP_USER_GUIDE.md)
- [Quick Start](../system-admin/seguranca/POSTGRES_APP_USER_QUICKSTART.md)

---

### Migrations

#### `init-db-postgres.sql`
Script SQL para inicialização do banco de dados PostgreSQL.

#### `init-db.sql` / `init-db-sqlserver.sql`
Scripts legados de inicialização (SQL Server).

---

### Outros Scripts

Consulte os subdiretórios `migrations/` e outros para scripts específicos de migration e manutenção.

## 🔐 Segurança

⚠️ **IMPORTANTE:**
- Nunca commite senhas ou credenciais nos scripts
- Use variáveis de ambiente para informações sensíveis
- Delete arquivos de credenciais temporários após uso
- Revise scripts antes de executar em produção

## 📚 Documentação Adicional

- [Guia de Segurança](../system-admin/seguranca/SECURITY_GUIDE.md)
- [Docker PostgreSQL Setup](../system-admin/infrastructure/DOCKER_POSTGRES_SETUP.md)
- [Migrations Guide](../MIGRATIONS_GUIDE.md)

---

**Última Atualização**: Fevereiro 2026
