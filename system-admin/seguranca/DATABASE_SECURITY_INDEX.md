# 🗂️ Índice: Segurança de Banco de Dados PostgreSQL

## 📋 Visão Geral

Este índice organiza toda a documentação relacionada à segurança de banco de dados PostgreSQL no Omni Care Software.

## 🎯 Por Onde Começar?

### Novo no Projeto?
1. Leia: [Quick Start - 5 minutos](POSTGRES_APP_USER_QUICKSTART.md)
2. Execute: `scripts/create-postgres-app-users.sh`
3. Configure: Connection strings nos appsettings
4. Teste: Conexão e operações básicas

### Já Usa Usuário Master?
1. **URGENTE**: Migre para usuários de aplicação
2. Leia: [Guia de Migração](POSTGRES_APP_USER_QUICKSTART.md#-atualizar-connection-strings)
3. Implemente em desenvolvimento primeiro
4. Depois aplique em produção

### Administrador de Banco?
1. Leia: [Guia Completo](POSTGRES_APP_USER_GUIDE.md)
2. Entenda: [Permissões necessárias](POSTGRES_APP_USER_GUIDE.md#-permissões-por-banco-de-dados)
3. Configure: [Auditoria e monitoramento](POSTGRES_APP_USER_GUIDE.md#-auditoria-e-monitoramento)

## 📚 Documentação Principal

### Guias de Configuração

| Documento | Descrição | Público | Tempo |
|-----------|-----------|---------|-------|
| **[POSTGRES_APP_USER_QUICKSTART.md](POSTGRES_APP_USER_QUICKSTART.md)** | Guia rápido de 5 minutos | Desenvolvedores | 5 min |
| **[POSTGRES_APP_USER_GUIDE.md](POSTGRES_APP_USER_GUIDE.md)** | Documentação completa e técnica | DBAs, DevOps | 30 min |
| **[SECURITY_GUIDE.md](SECURITY_GUIDE.md)** | Guia geral de segurança | Todos | 45 min |

### Scripts de Automação

| Script | Plataforma | Descrição |
|--------|-----------|-----------|
| `scripts/create-postgres-app-users.sh` | Linux/Mac | Criação automática de usuários |
| `scripts/create-postgres-app-users.ps1` | Windows | Versão PowerShell |

### Infraestrutura

| Documento | Descrição |
|-----------|-----------|
| [DOCKER_POSTGRES_SETUP.md](../infrastructure/DOCKER_POSTGRES_SETUP.md) | Setup com Docker/Podman |
| [MIGRACAO_POSTGRESQL.md](../infrastructure/MIGRACAO_POSTGRESQL.md) | Migração de SQL Server |

## 🔐 Conceitos de Segurança

### Por Que Não Usar Usuário Master?

#### ❌ Problemas
- Privilégios excessivos (pode deletar banco, criar usuários)
- Dificulta auditoria (tudo aparece como "postgres")
- Viola princípio do menor privilégio
- Não atende compliance (LGPD, HIPAA, SOC2)
- Risco de segurança crítico se comprometido

#### ✅ Solução: Usuários de Aplicação
- Permissões mínimas (apenas DML)
- Auditoria clara por aplicação
- Isolamento de segurança
- Compliance automático
- Fácil revogar se comprometido

### Arquitetura de Usuários

```
┌─────────────────────────────────────────────────┐
│  postgres (master)                              │
│  ↓ Apenas admin/migrations                      │
│                                                  │
│  omnicare_app (primecare)                       │
│  ↓ API principal - DML only                     │
│                                                  │
│  patientportal_app (patientportal)              │
│  ↓ Portal do Paciente - DML only                │
│                                                  │
│  telemedicine_app (telemedicine)                │
│  ↓ Telemedicina - DML only                      │
│                                                  │
│  omnicare_readonly (todos)                      │
│  ↓ BI/Relatórios - SELECT only                  │
└─────────────────────────────────────────────────┘
```

### Permissões por Tipo de Usuário

| Usuário | CONNECT | SELECT | INSERT | UPDATE | DELETE | CREATE | ALTER | DROP |
|---------|---------|--------|--------|--------|--------|--------|-------|------|
| postgres | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| omnicare_app | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| omnicare_readonly | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |

## 🚀 Implementação

### Passo a Passo Completo

#### 1. Criar Usuários

```bash
# Automático (recomendado)
cd scripts
./create-postgres-app-users.sh

# Manual (se necessário)
# Ver: POSTGRES_APP_USER_GUIDE.md#-criação-dos-usuários-de-aplicação
```

#### 2. Atualizar Connection Strings

**Desenvolvimento:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=primecare;Username=omnicare_app;Password=***"
  }
}
```

**Produção:**
```bash
export DB_USER=omnicare_app
export DB_PASSWORD=***
```

#### 3. Testar

```bash
# Testar conexão
psql -h localhost -U omnicare_app -d primecare

# Testar aplicação
dotnet run --project src/MedicSoft.Api
```

#### 4. Migrations

```bash
# Use usuário admin apenas para migrations
dotnet ef database update \
  --connection "Host=localhost;Database=primecare;Username=postgres;Password=***"
```

### Checklist de Implementação

#### Desenvolvimento
- [ ] Executar script de criação de usuários
- [ ] Atualizar appsettings.Development.json
- [ ] Testar conexão com novo usuário
- [ ] Testar operações CRUD
- [ ] Verificar que DDL falha (segurança)
- [ ] Atualizar Docker Compose se necessário

#### Produção
- [ ] Criar usuários em servidor de produção
- [ ] Configurar variáveis de ambiente
- [ ] Habilitar SSL/TLS (SSL Mode=Require)
- [ ] Configurar pg_hba.conf (IP whitelist)
- [ ] Limitar conexões por usuário
- [ ] Habilitar logging de conexões
- [ ] Documentar credenciais em Vault/1Password
- [ ] Fazer backup antes de mudança
- [ ] Testar em staging primeiro
- [ ] Deploy em produção
- [ ] Monitorar logs pós-deploy
- [ ] Revogar acesso "postgres" de aplicações

## 🔍 Operações Comuns

### Verificar Permissões

```sql
-- Ver permissões de um usuário
\dp
\du omnicare_app

-- Query detalhada
SELECT grantee, table_name, privilege_type 
FROM information_schema.table_privileges 
WHERE grantee = 'omnicare_app';
```

### Monitorar Conexões

```sql
-- Conexões ativas
SELECT usename, count(*) 
FROM pg_stat_activity 
WHERE usename = 'omnicare_app' 
GROUP BY usename;

-- Queries lentas
SELECT pid, now() - query_start AS duration, query
FROM pg_stat_activity
WHERE usename = 'omnicare_app'
  AND state = 'active'
  AND now() - query_start > interval '5 seconds';
```

### Alterar Senha

```sql
-- Rotacionar senha
ALTER USER omnicare_app WITH PASSWORD 'NovaSenhaForte!2025';

-- Lembre-se de atualizar connection strings!
```

### Adicionar Permissões

```sql
-- Se criou novas tabelas manualmente
GRANT SELECT, INSERT, UPDATE, DELETE ON "NomeTabela" TO omnicare_app;

-- Para sequences
GRANT USAGE, SELECT ON "NomeSequence" TO omnicare_app;
```

## 🆘 Troubleshooting

### Problemas Comuns

| Erro | Causa | Solução |
|------|-------|---------|
| permission denied for schema public | Falta USAGE no schema | `GRANT USAGE ON SCHEMA public TO user` |
| permission denied for sequence | Falta permissão em sequence | `GRANT USAGE, SELECT ON ALL SEQUENCES` |
| must be owner of table | Tentando ALTER sem permissão | Use usuário admin ou transfira ownership |
| cannot execute UPDATE in read-only | User configurado como readonly | `ALTER ROLE ... SET default_transaction_read_only = off` |
| connection refused | Usuário não existe ou pg_hba.conf | Verificar `\du` e pg_hba.conf |

Ver: [Troubleshooting Completo](POSTGRES_APP_USER_GUIDE.md#-troubleshooting)

## 📊 Compliance e Auditoria

### LGPD
- ✅ Separação de privilégios
- ✅ Auditoria por usuário
- ✅ Princípio do menor privilégio
- ✅ Logs detalhados de acesso

### HIPAA
- ✅ Controle de acesso granular
- ✅ Auditoria completa
- ✅ Isolamento de dados
- ✅ Criptografia em trânsito (SSL)

### SOC2
- ✅ Segregação de funções
- ✅ Monitoramento contínuo
- ✅ Controle de mudanças
- ✅ Gestão de credenciais

## 🔗 Links Úteis

### Documentação Externa
- [PostgreSQL GRANT](https://www.postgresql.org/docs/current/sql-grant.html)
- [PostgreSQL Role Attributes](https://www.postgresql.org/docs/current/role-attributes.html)
- [Npgsql Connection Strings](https://www.npgsql.org/doc/connection-string-parameters.html)
- [OWASP Database Security](https://cheatsheetseries.owasp.org/cheatsheets/Database_Security_Cheat_Sheet.html)

### Documentação Interna
- [Guia de Segurança Geral](SECURITY_GUIDE.md)
- [Criptografia de Dados](PRODUCTION_ENCRYPTION_GUIDE.md)
- [Rotação de Chaves](KEY_ROTATION_GUIDE.md)
- [Compliance LGPD](LGPD_COMPLIANCE_DOCUMENTATION.md)
- [Setup Docker](../infrastructure/DOCKER_POSTGRES_SETUP.md)

## 📈 Próximos Passos

### Curto Prazo (1-2 semanas)
1. Implementar usuários de aplicação em desenvolvimento
2. Testar em ambiente de staging
3. Migrar produção gradualmente
4. Treinar equipe

### Médio Prazo (1-3 meses)
1. Implementar rotação automática de senhas
2. Configurar alertas de segurança
3. Implementar Row-Level Security (RLS)
4. Auditoria trimestral de permissões

### Longo Prazo (3-12 meses)
1. Integrar com Azure Key Vault / AWS Secrets Manager
2. Implementar backup automatizado com criptografia
3. Certificate-based authentication
4. Disaster recovery testing regular

## 📞 Suporte

### Para Dúvidas
1. Consultar esta documentação
2. Verificar logs: `docker compose logs postgres`
3. Testar em ambiente local primeiro
4. Contatar DevOps/DBA se necessário

### Para Incidentes de Segurança
- Email: security@omnicaresoftware.com
- **Não divulgar vulnerabilidades publicamente**
- Seguir processo de divulgação responsável

---

**Criado**: Fevereiro 2026  
**Versão**: 1.0  
**Próxima Revisão**: Maio 2026  
**Mantido por**: Equipe de Segurança e DevOps
