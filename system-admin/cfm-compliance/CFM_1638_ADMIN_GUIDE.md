# 🔧 Guia do Administrador - CFM 1.638/2002 Versionamento e Auditoria

**Versão:** 1.0  
**Última Atualização:** Janeiro 2026  
**Audiência:** Administradores de Sistema, TI e Compliance

---

## 📑 Índice

1. [Visão Geral](#visao-geral)
2. [Configuração Inicial](#configuracao)
3. [Monitoramento de Versões](#monitoramento)
4. [Gerenciamento de Logs de Auditoria](#logs)
5. [Consultas e Relatórios](#relatorios)
6. [Retenção de Dados](#retencao)
7. [Backup e Recuperação](#backup)
8. [Troubleshooting](#troubleshooting)
9. [Compliance e Fiscalização](#compliance)

---

## 🏥 Visão Geral {#visao-geral}

O sistema de versionamento CFM 1.638/2002 implementa:

### Arquitetura

```
┌─────────────────────┐
│ MedicalRecord       │ ◄─── Prontuário Principal
│ - Id                │
│ - IsClosed          │
│ - CurrentVersion    │
└─────────────────────┘
         │
         │ 1:N
         ▼
┌─────────────────────┐
│ MedicalRecordVersion│ ◄─── Histórico de Versões
│ - Id                │
│ - Version           │
│ - ChangeType        │
│ - SnapshotJson      │      (Event Sourcing)
│ - ContentHash       │      (SHA-256)
│ - PreviousVersionHash│     (Blockchain-like)
└─────────────────────┘

┌─────────────────────┐
│MedicalRecordAccessLog│ ◄─── Logs de Auditoria
│ - Id                │
│ - AccessType        │      (20+ anos retenção)
│ - IpAddress         │
│ - UserAgent         │
└─────────────────────┘

┌─────────────────────┐
│MedicalRecordSignature│ ◄─── Assinaturas Digitais
│ - SignatureValue    │      (ICP-Brasil futuro)
│ - CertificateData   │
└─────────────────────┘
```

### Tabelas no Banco de Dados

- **MedicalRecords** - Prontuários (com flags de estado)
- **MedicalRecordVersions** - Histórico de versões
- **MedicalRecordAccessLogs** - Logs de auditoria
- **MedicalRecordSignatures** - Infraestrutura de assinatura

---

## ⚙️ Configuração Inicial {#configuracao}

### 1. Verificar Migração

```bash
cd /home/runner/work/MW.Code/MW.Code/src/MedicSoft.Repository
dotnet ef database update --startup-project ../MedicSoft.Api/MedicSoft.Api.csproj
```

**Verificar migração aplicada:**

```sql
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
WHERE "MigrationId" LIKE '%Cfm1638%';
```

Esperado: `20260123215326_AddCfm1638VersioningAndAudit`

---

### 2. Migrar Dados Existentes

Se já existem prontuários sem versão inicial:

```bash
psql -d medicsoft -f /home/runner/work/MW.Code/MW.Code/scripts/migrations/cfm-1638-initial-version-migration.sql
```

**Verificar sucesso:**

```sql
SELECT 
    COUNT(DISTINCT mr."Id") as total_records,
    COUNT(DISTINCT mrv."MedicalRecordId") as records_with_versions
FROM "MedicalRecords" mr
LEFT JOIN "MedicalRecordVersions" mrv ON mr."Id" = mrv."MedicalRecordId";
```

✅ Se `total_records` = `records_with_versions`, migração OK!

---

### 3. Configurar Permissões

Certifique-se de que as permissões estão configuradas:

```sql
-- Verificar permissões CFM 1.638
SELECT "Name", "Description"
FROM "Permissions"
WHERE "Name" LIKE 'medical-records%';
```

**Permissões necessárias:**
- `medical-records:read` - Visualizar prontuários
- `medical-records:write` - Editar prontuários
- `medical-records:close` - Fechar prontuários
- `medical-records:reopen` - Reabrir prontuários
- `medical-records:versions:read` - Ver histórico
- `medical-records:audit:read` - Ver logs de auditoria

---

### 4. Configurar Logs

**appsettings.json:**

```json
{
  "Logging": {
    "LogLevel": {
      "MedicSoft.Application.Services.MedicalRecordVersionService": "Information",
      "MedicSoft.Application.Services.MedicalRecordAuditService": "Information"
    }
  },
  "MedicalRecordSettings": {
    "EnableVersioning": true,
    "EnableAuditLogging": true,
    "MinimumReopenJustificationLength": 20,
    "VersionRetentionYears": 20
  }
}
```

---

## 📊 Monitoramento de Versões {#monitoramento}

### Dashboard de Versões

**Query: Estatísticas Gerais**

```sql
SELECT 
    COUNT(DISTINCT "MedicalRecordId") as total_records,
    COUNT(*) as total_versions,
    AVG(version_count) as avg_versions_per_record,
    MAX(version_count) as max_versions
FROM (
    SELECT 
        "MedicalRecordId",
        COUNT(*) as version_count
    FROM "MedicalRecordVersions"
    GROUP BY "MedicalRecordId"
) subquery;
```

**Resultado esperado:**
```
total_records | total_versions | avg_versions | max_versions
--------------+----------------+--------------+-------------
    1245      |     3821       |     3.07     |     28
```

---

### Prontuários com Muitas Versões

Identifique prontuários que foram muito alterados:

```sql
SELECT 
    mr."Id",
    p."Name" as patient_name,
    COUNT(mrv."Id") as version_count,
    MAX(mrv."ChangedAt") as last_modified
FROM "MedicalRecords" mr
JOIN "Patients" p ON mr."PatientId" = p."Id"
JOIN "MedicalRecordVersions" mrv ON mr."Id" = mrv."MedicalRecordId"
GROUP BY mr."Id", p."Name"
HAVING COUNT(mrv."Id") > 10
ORDER BY version_count DESC
LIMIT 20;
```

**Análise:**
- > 10 versões: Normal para casos complexos
- > 20 versões: Investigar se há problema de workflow
- > 50 versões: Possível uso inadequado (muitas reaberturas)

---

### Prontuários Reabertos Frequentemente

```sql
SELECT 
    mr."Id",
    p."Name" as patient_name,
    COUNT(CASE WHEN mrv."ChangeType" = 'Reopened' THEN 1 END) as reopen_count,
    COUNT(CASE WHEN mrv."ChangeType" = 'Closed' THEN 1 END) as close_count
FROM "MedicalRecords" mr
JOIN "Patients" p ON mr."PatientId" = p."Id"
JOIN "MedicalRecordVersions" mrv ON mr."Id" = mrv."MedicalRecordId"
GROUP BY mr."Id", p."Name"
HAVING COUNT(CASE WHEN mrv."ChangeType" = 'Reopened' THEN 1 END) > 3
ORDER BY reopen_count DESC;
```

**⚠️ Atenção:**
- Muitas reaberturas podem indicar:
  - Médico não revisa antes de fechar
  - Workflow inadequado
  - Necessidade de treinamento

---

### Verificar Integridade das Versões

Cada versão tem hash SHA-256. Verifique integridade:

```sql
-- Versões sem hash (erro de implementação)
SELECT 
    mrv."Id",
    mrv."Version",
    mrv."ChangedAt"
FROM "MedicalRecordVersions" mrv
WHERE mrv."ContentHash" IS NULL 
   OR mrv."ContentHash" = '';
```

**Deve retornar 0 linhas!**

---

## 📋 Gerenciamento de Logs de Auditoria {#logs}

### Consultar Logs Recentes

```sql
-- Últimos 100 acessos
SELECT 
    mal."AccessType",
    u."Name" as user_name,
    mal."AccessedAt",
    mal."IpAddress",
    p."Name" as patient_name
FROM "MedicalRecordAccessLogs" mal
JOIN "Users" u ON mal."UserId" = u."Id"
JOIN "MedicalRecords" mr ON mal."MedicalRecordId" = mr."Id"
JOIN "Patients" p ON mr."PatientId" = p."Id"
ORDER BY mal."AccessedAt" DESC
LIMIT 100;
```

---

### Acessos por Tipo

```sql
SELECT 
    "AccessType",
    COUNT(*) as total,
    COUNT(DISTINCT "UserId") as unique_users,
    COUNT(DISTINCT "MedicalRecordId") as unique_records
FROM "MedicalRecordAccessLogs"
WHERE "AccessedAt" >= NOW() - INTERVAL '30 days'
GROUP BY "AccessType"
ORDER BY total DESC;
```

**Resultado típico:**
```
AccessType | total | unique_users | unique_records
-----------+-------+--------------+---------------
View       | 5234  | 42           | 1823
Edit       | 1456  | 38           | 892
Close      | 734   | 35           | 734
Reopen     | 89    | 12           | 85
Print      | 234   | 18           | 198
Export     | 56    | 8            | 52
```

---

### Detectar Acessos Suspeitos

**1. Acessos fora do horário:**

```sql
-- Acessos entre 22h e 6h
SELECT 
    u."Name",
    mal."AccessType",
    mal."AccessedAt",
    mal."IpAddress"
FROM "MedicalRecordAccessLogs" mal
JOIN "Users" u ON mal."UserId" = u."Id"
WHERE EXTRACT(HOUR FROM mal."AccessedAt") >= 22 
   OR EXTRACT(HOUR FROM mal."AccessedAt") < 6
ORDER BY mal."AccessedAt" DESC;
```

---

**2. Múltiplos acessos em curto período:**

```sql
-- Usuário acessou > 50 prontuários em 1 hora
SELECT 
    u."Name",
    COUNT(DISTINCT mal."MedicalRecordId") as records_accessed,
    DATE_TRUNC('hour', mal."AccessedAt") as hour
FROM "MedicalRecordAccessLogs" mal
JOIN "Users" u ON mal."UserId" = u."Id"
WHERE mal."AccessedAt" >= NOW() - INTERVAL '7 days'
GROUP BY u."Name", DATE_TRUNC('hour', mal."AccessedAt")
HAVING COUNT(DISTINCT mal."MedicalRecordId") > 50
ORDER BY records_accessed DESC;
```

---

**3. Acessos de IPs incomuns:**

```sql
-- IPs que acessaram apenas 1 vez
SELECT 
    mal."IpAddress",
    COUNT(*) as access_count,
    MAX(mal."AccessedAt") as last_access
FROM "MedicalRecordAccessLogs" mal
GROUP BY mal."IpAddress"
HAVING COUNT(*) = 1
ORDER BY last_access DESC;
```

---

### Relatório de Acesso por Usuário

```sql
SELECT 
    u."Name",
    u."Email",
    COUNT(mal."Id") as total_accesses,
    COUNT(DISTINCT mal."MedicalRecordId") as unique_records,
    MAX(mal."AccessedAt") as last_access
FROM "Users" u
LEFT JOIN "MedicalRecordAccessLogs" mal ON u."Id" = mal."UserId"
WHERE mal."AccessedAt" >= NOW() - INTERVAL '30 days'
GROUP BY u."Id", u."Name", u."Email"
ORDER BY total_accesses DESC;
```

---

## 📈 Consultas e Relatórios {#relatorios}

### Relatório Mensal de Compliance

```sql
-- Resumo mensal de prontuários
WITH monthly_stats AS (
    SELECT 
        DATE_TRUNC('month', mrv."ChangedAt") as month,
        COUNT(DISTINCT mrv."MedicalRecordId") as records_modified,
        COUNT(*) as total_versions,
        COUNT(CASE WHEN mrv."ChangeType" = 'Created' THEN 1 END) as created,
        COUNT(CASE WHEN mrv."ChangeType" = 'Updated' THEN 1 END) as updated,
        COUNT(CASE WHEN mrv."ChangeType" = 'Closed' THEN 1 END) as closed,
        COUNT(CASE WHEN mrv."ChangeType" = 'Reopened' THEN 1 END) as reopened
    FROM "MedicalRecordVersions" mrv
    WHERE mrv."ChangedAt" >= NOW() - INTERVAL '12 months'
    GROUP BY DATE_TRUNC('month', mrv."ChangedAt")
)
SELECT 
    TO_CHAR(month, 'YYYY-MM') as mes,
    records_modified,
    total_versions,
    created,
    updated,
    closed,
    reopened,
    ROUND(reopened::numeric / NULLIF(closed, 0) * 100, 2) as reopen_rate
FROM monthly_stats
ORDER BY month DESC;
```

---

### Relatório de Conformidade CFM 1.638

```sql
-- Verificação de conformidade
SELECT 
    'Total de Prontuários' as metrica,
    COUNT(*)::text as valor
FROM "MedicalRecords"

UNION ALL

SELECT 
    'Prontuários com Versão',
    COUNT(DISTINCT "MedicalRecordId")::text
FROM "MedicalRecordVersions"

UNION ALL

SELECT 
    'Taxa de Versionamento (%)',
    ROUND(
        COUNT(DISTINCT mrv."MedicalRecordId")::numeric / 
        COUNT(DISTINCT mr."Id") * 100, 2
    )::text
FROM "MedicalRecords" mr
LEFT JOIN "MedicalRecordVersions" mrv ON mr."Id" = mrv."MedicalRecordId"

UNION ALL

SELECT 
    'Total de Versões',
    COUNT(*)::text
FROM "MedicalRecordVersions"

UNION ALL

SELECT 
    'Prontuários Fechados',
    COUNT(*)::text
FROM "MedicalRecords"
WHERE "IsClosed" = true

UNION ALL

SELECT 
    'Total de Acessos Auditados',
    COUNT(*)::text
FROM "MedicalRecordAccessLogs"

UNION ALL

SELECT 
    'Período de Logs (dias)',
    EXTRACT(DAY FROM (MAX("AccessedAt") - MIN("AccessedAt")))::text
FROM "MedicalRecordAccessLogs";
```

**Resultado esperado:**
```
metrica                        | valor
-------------------------------+-------
Total de Prontuários           | 1245
Prontuários com Versão         | 1245
Taxa de Versionamento (%)      | 100.00
Total de Versões               | 3821
Prontuários Fechados           | 892
Total de Acessos Auditados     | 15234
Período de Logs (dias)         | 365
```

---

## 💾 Retenção de Dados {#retencao}

### Política de Retenção

**CFM 1.638/2002 exige:**
- ✅ Versões: Permanente (nunca deletar)
- ✅ Logs de auditoria: 20+ anos
- ✅ Assinaturas: Permanente

### Monitorar Crescimento do Banco

```sql
-- Tamanho das tabelas
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as size
FROM pg_tables
WHERE tablename IN (
    'MedicalRecords',
    'MedicalRecordVersions',
    'MedicalRecordAccessLogs',
    'MedicalRecordSignatures'
)
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

### Arquivamento de Logs Antigos

Para logs > 5 anos, considere arquivamento:

```sql
-- Criar tabela de arquivo
CREATE TABLE "MedicalRecordAccessLogs_Archive" (
    LIKE "MedicalRecordAccessLogs" INCLUDING ALL
);

-- Mover logs antigos
INSERT INTO "MedicalRecordAccessLogs_Archive"
SELECT * FROM "MedicalRecordAccessLogs"
WHERE "AccessedAt" < NOW() - INTERVAL '5 years';

-- ⚠️ NÃO DELETE - Apenas para reduzir índices ativos
-- Mantenha os dados arquivados acessíveis
```

---

## 🔄 Backup e Recuperação {#backup}

### Estratégia de Backup

**Crítico - Backup Diário:**
```bash
#!/bin/bash
# backup-cfm1638.sh

BACKUP_DIR="/backups/medicsoft"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Backup das tabelas CFM 1.638
pg_dump -h localhost -U medicsoft -d medicsoft \
    -t "MedicalRecords" \
    -t "MedicalRecordVersions" \
    -t "MedicalRecordAccessLogs" \
    -t "MedicalRecordSignatures" \
    --compress=9 \
    -f "$BACKUP_DIR/cfm1638_$TIMESTAMP.sql.gz"

# Retenção: 7 anos
find $BACKUP_DIR -name "cfm1638_*.sql.gz" -mtime +2555 -delete
```

**Agendar no cron:**
```bash
0 2 * * * /usr/local/bin/backup-cfm1638.sh
```

---

### Recuperação de Versão Específica

Se precisar recuperar uma versão específica:

```sql
-- Ver snapshot JSON da versão
SELECT 
    "Version",
    "ChangeType",
    "ChangedAt",
    "SnapshotJson"
FROM "MedicalRecordVersions"
WHERE "MedicalRecordId" = 'uuid-do-prontuario'
ORDER BY "Version";

-- JSON contém estado completo do prontuário naquele momento
```

---

## 🔧 Troubleshooting {#troubleshooting}

### Problema: Versão não foi criada

**Sintoma:** Alteração foi salva mas não aparece no histórico

**Diagnóstico:**

```sql
-- Verificar última versão
SELECT * FROM "MedicalRecordVersions"
WHERE "MedicalRecordId" = 'uuid'
ORDER BY "Version" DESC
LIMIT 1;

-- Verificar logs de erro
SELECT * FROM "ApplicationLogs"
WHERE "Message" ILIKE '%MedicalRecordVersionService%'
  AND "LogLevel" = 'Error'
ORDER BY "Timestamp" DESC;
```

**Solução:** Verificar se serviço está registrado no DI

---

### Problema: Hash de integridade inválido

**Sintoma:** Hash não bate com o conteúdo

**Verificação:**

```csharp
// Recalcular hash
var service = new MedicalRecordVersionService();
var recalculatedHash = service.CalculateContentHash(snapshotJson);

// Comparar com hash armazenado
if (recalculatedHash != version.ContentHash)
{
    // ALERTA: Possível adulteração!
}
```

---

### Problema: Log de auditoria não registrado

**Diagnóstico:**

```sql
-- Verificar se há logs recentes
SELECT COUNT(*) as recent_logs
FROM "MedicalRecordAccessLogs"
WHERE "AccessedAt" >= NOW() - INTERVAL '1 hour';
```

**Se = 0, verificar:**
1. Serviço `MedicalRecordAuditService` registrado?
2. Middleware de auditoria ativo?
3. Erros nos logs da aplicação?

---

## ✅ Compliance e Fiscalização {#compliance}

### Preparação para Auditoria CFM

Quando o CFM solicitar auditoria, você precisará fornecer:

#### 1. Relatório de Conformidade

```sql
-- Executar query de conformidade (seção Relatórios)
-- Salvar resultado em PDF
```

#### 2. Evidências de Versionamento

```sql
-- Exemplo de prontuário com histórico completo
SELECT 
    mrv."Version",
    mrv."ChangeType",
    mrv."ChangedAt",
    u."Name" as changed_by,
    mrv."ChangeReason"
FROM "MedicalRecordVersions" mrv
JOIN "Users" u ON mrv."ChangedByUserId" = u."Id"
WHERE mrv."MedicalRecordId" = 'uuid-exemplo'
ORDER BY mrv."Version";
```

#### 3. Logs de Auditoria

```sql
-- Últimos 90 dias de acessos
SELECT 
    mal."AccessType",
    mal."AccessedAt",
    u."Name",
    mal."IpAddress"
FROM "MedicalRecordAccessLogs" mal
JOIN "Users" u ON mal."UserId" = u."Id"
WHERE mal."AccessedAt" >= NOW() - INTERVAL '90 days'
ORDER BY mal."AccessedAt" DESC;
```

#### 4. Política de Segurança

Documente:
- ✅ Controle de acesso implementado
- ✅ Versionamento automático
- ✅ Imutabilidade de prontuários fechados
- ✅ Retenção de dados conforme CFM
- ✅ Backup e recuperação

---

### Checklist de Compliance

```
☐ 100% dos prontuários têm versão inicial
☐ Logs de auditoria de 100% dos acessos
☐ Backup diário funcionando
☐ Retenção de dados ≥ 20 anos configurada
☐ Permissões de acesso configuradas
☐ Documentação atualizada
☐ Equipe treinada
☐ Política de segurança documentada
```

---

## 📞 Suporte Técnico

### Contatos

- **Email:** devops@primecare.com.br
- **Slack:** #medicsoft-admin
- **Emergência:** (11) 99999-9999

### Recursos Úteis

- [Documentação Técnica CFM 1.638](./CFM-1638-VERSIONING-README.md)
- [Guia do Usuário](./CFM_1638_USER_GUIDE.md)
- [Resolução CFM 1.638/2002](http://www.portalmedico.org.br/resolucoes/cfm/2002/1638_2002.htm)

---

**Última Atualização:** Janeiro 2026  
**Versão:** 1.0  
**Mantido por:** Equipe DevOps MedicSoft
