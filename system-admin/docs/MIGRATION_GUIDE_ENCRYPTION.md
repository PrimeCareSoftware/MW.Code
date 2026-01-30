# 🔄 Guia de Migração - Criptografia de Dados Existentes

> **Data:** Janeiro de 2026  
> **Criticidade:** ALTA - Requer planejamento e backup  
> **Tempo Estimado:** 2-8 horas (depende do volume de dados)

---

## 📋 Visão Geral

Este guia detalha o processo completo de migração de dados existentes para o sistema de criptografia. A migração é **irreversível** sem backup, portanto siga todos os passos com atenção.

---

## ⚠️ Avisos Importantes

🔴 **CRÍTICO:**
- Dados criptografados SEM a chave são **irrecuperáveis**
- Faça backup ANTES de iniciar
- Teste em ambiente de desenvolvimento primeiro
- Planeje janela de manutenção (sistema ficará offline)

🟡 **ATENÇÃO:**
- Migração aumenta tamanho do banco em ~30-50%
- Performance pode ser impactada temporariamente
- Processo não pode ser interrompido no meio (use batches)

---

## 📝 Checklist Pré-Migração

### 1. Backup Completo

```bash
# PostgreSQL
pg_dump -h localhost -U medicsoft -d medicsoft_db \
  -F c -f "backup-pre-encryption-$(date +%Y%m%d).dump"

# SQL Server
sqlcmd -S localhost -Q "BACKUP DATABASE medicsoft_db TO DISK='C:\Backups\pre-encryption.bak'"
```

**Verificar backup:**
```bash
# PostgreSQL - Verificar integridade
pg_restore --list backup-pre-encryption-*.dump | head -20

# Verificar tamanho
ls -lh backup-pre-encryption-*.dump
```

✅ Checklist:
- [ ] Backup criado com sucesso
- [ ] Tamanho do backup condiz com expectativa
- [ ] Backup testado (restauração em ambiente de teste)
- [ ] Backup copiado para local seguro (S3/Azure/outro servidor)

### 2. Ambiente de Teste

```bash
# 1. Criar banco de teste
createdb medicsoft_db_test

# 2. Restaurar backup
pg_restore -h localhost -U medicsoft -d medicsoft_db_test backup-pre-encryption-*.dump

# 3. Configurar appsettings.Test.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=medicsoft_db_test;..."
  },
  "EncryptionSettings": {
    "Enabled": true
  }
}

# 4. Executar migração de teste
./scripts/encryption/encrypt-existing-data.sh --test
```

✅ Checklist:
- [ ] Banco de teste criado e populado
- [ ] Aplicação conecta ao banco de teste
- [ ] Migração de teste executada sem erros
- [ ] Dados descriptografados corretamente após migração

### 3. Geração de Chave

```bash
# Gerar chave mestre
dotnet run --project src/MedicSoft.Api -- generate-encryption-key

# Chave será salva em: encryption-keys/master.key
```

✅ Checklist:
- [ ] Chave gerada (arquivo master.key existe)
- [ ] Chave tem 44 caracteres Base64
- [ ] Chave backuped em local SEGURO (fora do servidor)
- [ ] Apenas equipe autorizada tem acesso

**⚠️ NUNCA commite a chave no Git!**

### 4. Estimativa de Volume

```sql
-- Total de registros a criptografar
SELECT 
  'Patients' as table_name, COUNT(*) as total_records,
  COUNT(*) * 3 as fields_to_encrypt -- Document, MedicalHistory, Allergies
FROM Patients
UNION ALL
SELECT 
  'MedicalRecords', COUNT(*), COUNT(*) * 9 -- 9 campos criptografados
FROM MedicalRecords;

-- Estimativa de tempo (3ms por campo)
-- Exemplo: 10.000 patients * 3 campos * 3ms = 90 segundos
```

✅ Checklist:
- [ ] Volume calculado
- [ ] Tempo estimado calculado
- [ ] Janela de manutenção planejada
- [ ] Stakeholders notificados

---

## 🚀 Processo de Migração

### Passo 1: Preparação (30 min)

```bash
# 1. Ativar modo manutenção
touch /var/www/medicsoft/maintenance.flag

# 2. Parar aplicação
sudo systemctl stop medicsoft-api

# 3. Backup final
pg_dump -h localhost -U medicsoft -d medicsoft_db \
  -F c -f "backup-final-pre-encryption-$(date +%Y%m%d-%H%M%S).dump"

# 4. Verificar que não há conexões ativas
psql -h localhost -U medicsoft -d medicsoft_db -c \
  "SELECT count(*) FROM pg_stat_activity WHERE datname = 'medicsoft_db';"
```

### Passo 2: Criar Migration do EF Core

```bash
# Criar migration para adicionar colunas de hash
cd src/MedicSoft.Repository
dotnet ef migrations add AddEncryptionHashColumns

# Revisar migration gerada
cat Migrations/*_AddEncryptionHashColumns.cs

# Aplicar migration
dotnet ef database update
```

**Migration esperada:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<string>(
        name: "DocumentHash",
        table: "Patients",
        maxLength: 100,
        nullable: true);

    migrationBuilder.CreateIndex(
        name: "IX_Patients_DocumentHash",
        table: "Patients",
        column: "DocumentHash");
}
```

### Passo 3: Executar Migração de Dados

```bash
# Modo TESTE (apenas simula)
./scripts/encryption/encrypt-existing-data.sh --test --batch-size 1000

# Modo PRODUÇÃO (criptografa de verdade)
./scripts/encryption/encrypt-existing-data.sh --batch-size 1000
```

**Saída esperada:**
```
[2026-01-30 14:00:00] === Starting Encryption Migration ===
[2026-01-30 14:00:00] Batch Size: 1000
[2026-01-30 14:00:00] Test Mode: false
[2026-01-30 14:00:05] Database connection successful
[2026-01-30 14:00:10] Backup created: ./backups/encryption-20260130-140000/pre-encryption-backup.dump
[2026-01-30 14:00:15] Starting data encryption...
[2026-01-30 14:01:45] Encrypted 3000 fields in Patients table (1000 records)
[2026-01-30 14:03:20] Encrypted 9000 fields in MedicalRecords table (1000 records)
[2026-01-30 14:03:25] Encryption completed successfully
[2026-01-30 14:03:30] Verifying encryption...
[2026-01-30 14:03:35] Encryption verification passed
[2026-01-30 14:03:35] === Encryption Migration Complete ===
```

### Passo 4: Verificação

```bash
# 1. Verificar campos criptografados no banco
psql -h localhost -U medicsoft -d medicsoft_db -c \
  "SELECT LEFT(\"Document\", 50) as encrypted_doc, \"DocumentHash\" 
   FROM \"Patients\" LIMIT 5;"

# Esperado: Document deve ser Base64 longo, DocumentHash deve ter 44 chars
```

```bash
# 2. Testar descriptografia via aplicação
dotnet run --project src/MedicSoft.Api -- test-decryption
```

```bash
# 3. Verificar logs de auditoria
psql -h localhost -U medicsoft -d medicsoft_db -c \
  "SELECT \"Action\", \"EntityType\", \"CreatedAt\" 
   FROM \"AuditLogs\" 
   WHERE \"Action\" LIKE '%ENCRYPT%' 
   ORDER BY \"CreatedAt\" DESC LIMIT 10;"
```

### Passo 5: Reiniciar Sistema

```bash
# 1. Iniciar aplicação
sudo systemctl start medicsoft-api

# 2. Verificar status
sudo systemctl status medicsoft-api

# 3. Verificar logs
tail -f /var/log/medicsoft/application.log

# 4. Teste funcional: buscar paciente por CPF
curl -X POST http://localhost:5000/api/patients/search \
  -H "Content-Type: application/json" \
  -d '{"document": "123.456.789-00"}'
```

### Passo 6: Desativar Manutenção

```bash
# Remover flag de manutenção
rm /var/www/medicsoft/maintenance.flag

# Notificar usuários
echo "Sistema disponível" | mail -s "MedicSoft Online" users@medicsoft.com
```

---

## 🔍 Verificação de Sucesso

### Testes Funcionais

**1. Buscar paciente por CPF (via hash)**
```bash
# CPF pesquisável através do hash
curl http://localhost:5000/api/patients/search?cpf=123.456.789-00
# Deve retornar paciente com CPF descriptografado
```

**2. Criar novo paciente**
```bash
curl -X POST http://localhost:5000/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Teste Encryption",
    "document": "987.654.321-00",
    "dateOfBirth": "1990-01-01",
    ...
  }'

# Verificar no banco que Document está criptografado
psql -c "SELECT \"Document\" FROM \"Patients\" WHERE \"Name\" = 'Teste Encryption';"
```

**3. Atualizar registro existente**
```bash
# Atualizar deve re-criptografar
curl -X PUT http://localhost:5000/api/patients/{id} \
  -H "Content-Type: application/json" \
  -d '{"medicalHistory": "Nova informação sensível"}'
```

**4. Ler prontuário médico**
```bash
# Deve descriptografar automaticamente
curl http://localhost:5000/api/medical-records/{id}
# JSON deve conter campos em texto claro
```

### Testes de Performance

```bash
# Benchmark de leitura
curl http://localhost:5000/api/patients?page=1&size=100
# Medir tempo de resposta (deve ser <5s para 100 registros)

# Benchmark de escrita
for i in {1..10}; do
  curl -X POST http://localhost:5000/api/patients -d '...' &
done
wait
# Medir throughput
```

### Checklist Final

✅ **Funcionalidade:**
- [ ] Busca por CPF funciona
- [ ] Novos registros são criptografados
- [ ] Registros existentes são descriptografados
- [ ] Atualização re-criptografa corretamente

✅ **Segurança:**
- [ ] Dados no banco estão em Base64 (criptografados)
- [ ] Chave de criptografia está segura (fora do Git)
- [ ] Logs de auditoria registram operações
- [ ] Backup da chave existe

✅ **Performance:**
- [ ] Tempo de resposta aceitável (<3s para 100 registros)
- [ ] Sem erros de timeout
- [ ] CPU/Memória dentro do normal

---

## 🚨 Rollback (Se Necessário)

### Cenário 1: Migração Falhou no Meio

```bash
# 1. Parar aplicação
sudo systemctl stop medicsoft-api

# 2. Restaurar backup
pg_restore -h localhost -U medicsoft -d medicsoft_db \
  --clean --if-exists backup-final-pre-encryption-*.dump

# 3. Desabilitar criptografia temporariamente
# appsettings.json:
{
  "EncryptionSettings": {
    "Enabled": false
  }
}

# 4. Reiniciar
sudo systemctl start medicsoft-api
```

### Cenário 2: Descriptografia Falhando

```bash
# Verificar chave
cat encryption-keys/master.key
# Deve ter 44 caracteres Base64

# Se chave corrompida, restaurar do backup
cp backup-encryption-keys/master.key encryption-keys/master.key

# Reiniciar aplicação
sudo systemctl restart medicsoft-api
```

### Cenário 3: Performance Inaceitável

```bash
# Desabilitar temporariamente interceptor de criptografia
# Program.cs: Comentar linha
// options.AddInterceptors(sp.GetRequiredService<EncryptionInterceptor>());

# Dados permanecem criptografados no banco
# Aplicação retorna dados criptografados (Base64)
# Investigar causa raiz antes de reverter
```

---

## 📊 Monitoramento Pós-Migração

### Métricas a Observar (Primeiras 24h)

**1. Tempo de Resposta**
```bash
# APM / Application Insights
avg(response_time) by endpoint
# Esperado: <10% de aumento
```

**2. Erros de Descriptografia**
```bash
# Buscar em logs
grep "Failed to decrypt" /var/log/medicsoft/application.log
# Esperado: 0 erros
```

**3. Utilização de CPU**
```bash
top -u medicsoft
# Esperado: +5-10% CPU durante picos
```

**4. Tamanho do Banco**
```sql
SELECT pg_size_pretty(pg_database_size('medicsoft_db'));
-- Esperado: +30-50% do tamanho original
```

### Alertas Recomendados

```yaml
# alerts.yaml
alerts:
  - name: DecryptionErrors
    condition: count(errors) WHERE message LIKE '%decrypt%' > 0
    severity: HIGH
    action: notify_devops

  - name: PerformanceDegradation
    condition: avg(response_time) > baseline * 1.5
    severity: MEDIUM
    action: investigate

  - name: DiskSpaceEncryption
    condition: disk_usage > 80%
    severity: HIGH
    action: expand_storage
```

---

## 📚 Troubleshooting

### Problema 1: "Encryption key cannot be null"

**Causa:** Chave de criptografia não encontrada

**Solução:**
```bash
# Verificar se arquivo existe
ls -la encryption-keys/master.key

# Se não existe, gerar nova (CUIDADO: perde dados já criptografados!)
dotnet run --project src/MedicSoft.Api -- generate-encryption-key

# Ou restaurar do backup
cp backup-keys/master.key encryption-keys/master.key
```

### Problema 2: "Failed to decrypt data"

**Causa:** Dados criptografados com chave diferente

**Solução:**
```bash
# Verificar versão da chave
cat encryption-keys/master.key | head -c 10
# Comparar com backup

# Se usou chave errada, restaurar chave correta
cp backup-keys/master.key encryption-keys/master.key

# Reiniciar aplicação
sudo systemctl restart medicsoft-api
```

### Problema 3: "Property already encrypted"

**Causa:** Tentativa de criptografar dados já criptografados (dupla criptografia)

**Solução:**
```bash
# Verificar se interceptor está duplicado
# Program.cs: Apenas 1 instância de EncryptionInterceptor

# Se dados foram duplamente criptografados:
# 1. Restaurar backup
# 2. Executar migração novamente
```

### Problema 4: Busca por CPF não funciona

**Causa:** `DocumentHash` não foi gerado

**Solução:**
```sql
-- Verificar se hash existe
SELECT "Document", "DocumentHash" FROM "Patients" LIMIT 5;

-- Se DocumentHash é NULL, re-gerar hashes
UPDATE "Patients"
SET "DocumentHash" = encode(sha256("Document"::bytea), 'base64')
WHERE "DocumentHash" IS NULL;
```

---

## 📞 Suporte

**Durante Migração:**
- 📧 Email: devops@medicsoft.com
- 📱 Telefone: +55 11 9999-9999 (plantão)
- 💬 Slack: #medicsoft-devops

**Pós-Migração:**
- 📧 Email: support@medicsoft.com
- 📖 Docs: `/system-admin/docs/CRIPTOGRAFIA_DADOS_MEDICOS.md`

---

**Última Atualização:** Janeiro 2026  
**Versão:** 1.0  
**Status:** Aprovado para Produção
