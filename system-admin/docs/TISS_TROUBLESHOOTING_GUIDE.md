# 🔧 TISS Troubleshooting Guide - Guia de Solução de Problemas

**Versão:** 1.0  
**Última Atualização:** Janeiro 2026  
**Status:** ✅ PRODUCTION-READY

---

## 📑 Índice

1. [Problemas com Operadoras](#problemas-operadoras)
2. [Problemas com Carteirinhas](#problemas-carteirinhas)
3. [Problemas com Autorizações](#problemas-autorizacoes)
4. [Problemas com Guias](#problemas-guias)
5. [Problemas com XML](#problemas-xml)
6. [Problemas com Lotes](#problemas-lotes)
7. [Problemas com Webservice](#problemas-webservice)
8. [Problemas com Glosas](#problemas-glosas)
9. [Problemas de Performance](#problemas-performance)
10. [Logs e Debugging](#logs-debugging)

---

## 🏥 Problemas com Operadoras {#problemas-operadoras}

### ❌ Operadora não aparece na lista

**Sintomas:**
- Operadora cadastrada mas não aparece no dropdown
- Erro ao buscar operadoras

**Causas Possíveis:**
1. Operadora marcada como inativa
2. Problemas com filtro de busca
3. Cache desatualizado no frontend

**Solução:**

```sql
-- Verificar status da operadora
SELECT "Id", "TradeName", "IsActive", "TenantId" 
FROM "HealthInsuranceOperators" 
WHERE "TradeName" ILIKE '%nome_operadora%';

-- Reativar se necessário
UPDATE "HealthInsuranceOperators" 
SET "IsActive" = true 
WHERE "Id" = 'uuid-da-operadora';
```

**Frontend:**
```typescript
// Limpar cache do navegador
localStorage.clear();
sessionStorage.clear();
// Recarregar página
location.reload();
```

---

### ❌ Número ANS duplicado

**Sintomas:**
- Erro ao cadastrar operadora: "ANS registration number already exists"

**Causa:**
- Tentativa de cadastrar operadora com número ANS já existente

**Solução:**

```sql
-- Verificar operadora existente
SELECT "Id", "TradeName", "AnsRegistrationNumber", "IsActive"
FROM "HealthInsuranceOperators"
WHERE "AnsRegistrationNumber" = '123456';

-- Opção 1: Atualizar operadora existente
-- Opção 2: Verificar se o número ANS está correto
```

---

## 💳 Problemas com Carteirinhas {#problemas-carteirinhas}

### ❌ Validação de carteirinha falha

**Sintomas:**
- API retorna "Carteirinha inválida"
- Paciente afirma que carteirinha está válida

**Checklist de Verificação:**

1. **Formato do número:**
   ```
   Correto: 1234567890123456 (16 dígitos)
   Incorreto: 123-456-789 (com traços/espaços)
   ```

2. **Validade:**
   ```sql
   SELECT "CardNumber", "ValidFrom", "ValidUntil", "IsActive"
   FROM "PatientHealthInsurances"
   WHERE "CardNumber" = '1234567890123456';
   ```

3. **Operadora ativa:**
   ```sql
   SELECT phi."CardNumber", op."TradeName", op."IsActive"
   FROM "PatientHealthInsurances" phi
   JOIN "HealthInsurancePlans" hp ON phi."PlanId" = hp."Id"
   JOIN "HealthInsuranceOperators" op ON hp."OperatorId" = op."Id"
   WHERE phi."CardNumber" = '1234567890123456';
   ```

**Solução Rápida:**

```sql
-- Verificar e corrigir data de validade
UPDATE "PatientHealthInsurances"
SET "ValidUntil" = '2026-12-31',
    "IsActive" = true
WHERE "CardNumber" = '1234567890123456';
```

---

### ❌ Paciente possui múltiplas carteirinhas

**Sintomas:**
- Sistema retorna múltiplos planos ativos
- Usuário não sabe qual selecionar

**Diagnóstico:**

```sql
-- Verificar todos os planos ativos do paciente
SELECT 
    phi."Id",
    phi."CardNumber",
    hp."PlanName",
    op."TradeName",
    phi."IsPrimary",
    phi."ValidUntil"
FROM "PatientHealthInsurances" phi
JOIN "HealthInsurancePlans" hp ON phi."PlanId" = hp."Id"
JOIN "HealthInsuranceOperators" op ON hp."OperatorId" = op."Id"
WHERE phi."PatientId" = 'uuid-do-paciente'
  AND phi."IsActive" = true
ORDER BY phi."IsPrimary" DESC, phi."ValidUntil" DESC;
```

**Solução:**
- Marcar um plano como primário
- Inativar planos expirados

```sql
-- Definir plano primário
UPDATE "PatientHealthInsurances"
SET "IsPrimary" = true
WHERE "Id" = 'uuid-do-plano-principal';

-- Remover flag de outros planos
UPDATE "PatientHealthInsurances"
SET "IsPrimary" = false
WHERE "PatientId" = 'uuid-do-paciente'
  AND "Id" != 'uuid-do-plano-principal';
```

---

## 🔐 Problemas com Autorizações {#problemas-autorizacoes}

### ❌ Autorização pendente há dias

**Sintomas:**
- Status permanece "Pendente"
- Operadora já respondeu mas sistema não atualizou

**Diagnóstico:**

```sql
-- Verificar autorizações pendentes antigas
SELECT 
    ar."Id",
    ar."AuthorizationNumber",
    ar."Status",
    ar."RequestedAt",
    EXTRACT(DAY FROM (NOW() - ar."RequestedAt")) as days_pending,
    op."TradeName"
FROM "AuthorizationRequests" ar
JOIN "PatientHealthInsurances" phi ON ar."PatientHealthInsuranceId" = phi."Id"
JOIN "HealthInsurancePlans" hp ON phi."PlanId" = hp."Id"
JOIN "HealthInsuranceOperators" op ON hp."OperatorId" = op."Id"
WHERE ar."Status" = 'Pendente'
  AND ar."RequestedAt" < NOW() - INTERVAL '5 days'
ORDER BY ar."RequestedAt";
```

**Solução Manual:**

```http
PUT /api/authorization-requests/{id}/status
```

```json
{
  "status": "Autorizado",
  "authorizationCode": "12345678",
  "validUntil": "2026-03-31",
  "notes": "Atualizado manualmente via contato telefônico"
}
```

---

### ❌ Procedimento não requer autorização mas sistema solicita

**Causa:**
- Configuração incorreta no cadastro do procedimento TUSS

**Solução:**

```sql
-- Verificar configuração do procedimento
SELECT "Code", "Description", "RequiresAuthorization"
FROM "TussProcedures"
WHERE "Code" = '10101012';

-- Corrigir se necessário
UPDATE "TussProcedures"
SET "RequiresAuthorization" = false
WHERE "Code" = '10101012';
```

---

## 📄 Problemas com Guias {#problemas-guias}

### ❌ Não é possível criar guia

**Sintomas:**
- Erro: "Patient health insurance not found"
- Erro: "Authorization required but not provided"

**Checklist:**

1. **Verificar vínculo paciente-plano:**
   ```sql
   SELECT * FROM "PatientHealthInsurances"
   WHERE "PatientId" = 'uuid-paciente'
     AND "IsActive" = true;
   ```

2. **Verificar se procedimento requer autorização:**
   ```sql
   SELECT tp."RequiresAuthorization"
   FROM "TussProcedures" tp
   WHERE tp."Id" = 'uuid-procedimento';
   ```

3. **Verificar autorização existente:**
   ```sql
   SELECT * FROM "AuthorizationRequests"
   WHERE "PatientHealthInsuranceId" = 'uuid'
     AND "Status" = 'Autorizado'
     AND "ValidUntil" >= CURRENT_DATE;
   ```

---

### ❌ Valor da guia incorreto

**Sintomas:**
- Total da guia não bate com soma dos procedimentos
- Valores zerados

**Diagnóstico:**

```sql
-- Verificar cálculo da guia
SELECT 
    tg."GuideNumber",
    tg."TotalAmount",
    SUM(tgp."TotalPrice") as calculated_total
FROM "TissGuides" tg
LEFT JOIN "TissGuideProcedures" tgp ON tgp."TissGuideId" = tg."Id"
WHERE tg."Id" = 'uuid-da-guia'
GROUP BY tg."Id", tg."GuideNumber", tg."TotalAmount";
```

**Solução:**

```sql
-- Recalcular total da guia
UPDATE "TissGuides"
SET "TotalAmount" = (
    SELECT COALESCE(SUM(tgp."TotalPrice"), 0)
    FROM "TissGuideProcedures" tgp
    WHERE tgp."TissGuideId" = "TissGuides"."Id"
)
WHERE "Id" = 'uuid-da-guia';
```

---

## 📦 Problemas com XML {#problemas-xml}

### ❌ Validação de XML falha

**Sintomas:**
- Erro: "XML validation failed against XSD schema"
- Rejeição pela operadora

**Principais Erros:**

#### 1. Campos Obrigatórios Faltando

**Erro:**
```xml
Element 'numeroGuia': This element is not expected.
```

**Solução:**
- Verificar se todos os campos obrigatórios do TISS 4.02.00 estão preenchidos
- Consultar documentação ANS

```csharp
// Verificar no código
var validator = new TissXmlValidatorService();
var result = await validator.ValidateXmlAsync(xmlContent);

foreach (var error in result.Errors)
{
    Console.WriteLine($"Campo: {error.Field}, Erro: {error.Message}");
}
```

#### 2. Formato de Data Incorreto

**Erro:**
```xml
Element 'dataAtendimento': '29/01/2026' is not a valid value of the atomic type 'xs:date'
```

**Solução:**
- Usar formato ISO 8601: `2026-01-29`

```csharp
// Correto
guideDate = DateTime.Parse("2026-01-29");

// Incorreto
guideDate = DateTime.Parse("29/01/2026");
```

#### 3. Encoding Incorreto

**Erro:**
- Caracteres especiais aparecem como `???` ou `Ã§`

**Solução:**
```csharp
// Garantir UTF-8
var xmlSettings = new XmlWriterSettings
{
    Encoding = Encoding.UTF8,
    Indent = true
};
```

---

### ❌ Namespace incorreto no XML

**Sintomas:**
- XML rejeitado por namespace inválido

**Verificar:**
```xml
<!-- Correto -->
<ans:mensagemTISS xmlns:ans="http://www.ans.gov.br/padroes/tiss/schemas" 
                  xsi:schemaLocation="...">
```

**Namespace deve ser:**
- `http://www.ans.gov.br/padroes/tiss/schemas`

---

## 📋 Problemas com Lotes {#problemas-lotes}

### ❌ Não é possível adicionar guia ao lote

**Sintomas:**
- Erro: "Guide already in another batch"
- Erro: "Guide status must be 'Created'"

**Diagnóstico:**

```sql
-- Verificar status da guia
SELECT 
    "Id",
    "GuideNumber",
    "Status",
    "TissBatchId"
FROM "TissGuides"
WHERE "Id" = 'uuid-da-guia';
```

**Possíveis Status:**
- ✅ `Created` - Pode ser adicionada ao lote
- ❌ `InBatch` - Já está em um lote
- ❌ `Sent` - Já foi enviada
- ❌ `Paid` - Já foi paga

**Solução:**

```sql
-- Remover guia de lote anterior (se necessário)
UPDATE "TissGuides"
SET "Status" = 'Created',
    "TissBatchId" = NULL
WHERE "Id" = 'uuid-da-guia'
  AND "Status" = 'InBatch';
```

---

### ❌ Lote está muito grande

**Sintomas:**
- Timeout ao gerar XML
- Arquivo XML > 10MB

**Recomendações ANS:**
- Máximo 100 guias por lote
- Arquivo XML até 10MB

**Solução:**

```sql
-- Verificar tamanho do lote
SELECT 
    tb."BatchNumber",
    COUNT(tg."Id") as total_guides,
    SUM(tg."TotalAmount") as total_amount
FROM "TissBatches" tb
LEFT JOIN "TissGuides" tg ON tg."TissBatchId" = tb."Id"
WHERE tb."Id" = 'uuid-do-lote'
GROUP BY tb."Id", tb."BatchNumber";
```

**Dividir lote:**
```csharp
// Criar novo lote com parte das guias
var guidesToMove = guides.Skip(100).ToList();
var newBatch = new TissBatch { /* ... */ };
```

---

## 🌐 Problemas com Webservice {#problemas-webservice}

### ❌ Timeout na conexão

**Sintomas:**
- Erro: "The operation has timed out"
- Lote não enviado via webservice

**Causas:**
1. URL do webservice incorreta
2. Firewall bloqueando conexão
3. Certificado SSL inválido
4. Serviço da operadora fora do ar

**Diagnóstico:**

```bash
# Testar conectividade
curl -v https://ws.unimed.com.br/tiss

# Verificar certificado SSL
openssl s_client -connect ws.unimed.com.br:443
```

**Configuração:**

```sql
-- Verificar configuração do webservice
SELECT 
    "TradeName",
    "WebserviceUrl",
    "WebserviceAuthType",
    "SupportsTissWebservice"
FROM "HealthInsuranceOperators"
WHERE "Id" = 'uuid-operadora';
```

**Aumentar timeout:**

```csharp
// appsettings.json
{
  "TissSettings": {
    "WebserviceTimeout": 120 // segundos
  }
}
```

---

### ❌ Autenticação no webservice falha

**Sintomas:**
- Erro 401 Unauthorized
- Erro 403 Forbidden

**Tipos de autenticação suportados:**
1. Basic Authentication
2. Bearer Token
3. Certificate (mTLS)

**Solução:**

```sql
-- Verificar credenciais
SELECT 
    "WebserviceAuthType",
    "WebserviceUsername",
    "WebserviceAuthToken"
FROM "HealthInsuranceOperators"
WHERE "Id" = 'uuid-operadora';
```

**Atualizar credenciais:**

```http
PUT /api/health-insurance-operators/{id}
```

```json
{
  "webserviceUsername": "usuario@clinica",
  "webservicePassword": "nova_senha",
  "webserviceAuthToken": "Bearer eyJhbGc..."
}
```

---

## 💰 Problemas com Glosas {#problemas-glosas}

### ❌ Glosa não está aparecendo no relatório

**Diagnóstico:**

```sql
-- Verificar glosas registradas
SELECT 
    tg."GuideNumber",
    tgl."GlosaType",
    tgl."GlosaReason",
    tgl."GlosaAmount",
    tgl."GlosaDate"
FROM "TissGlosas" tgl
JOIN "TissGuides" tg ON tgl."TissGuideId" = tg."Id"
WHERE tg."Id" = 'uuid-da-guia';
```

**Verificar se guia tem status correto:**

```sql
UPDATE "TissGuides"
SET "Status" = 'Glosada',
    "GlosaAmount" = 150.00
WHERE "Id" = 'uuid-da-guia';
```

---

### ❌ Como criar recurso de glosa

**Passo a passo:**

1. **Identificar a glosa:**
   ```sql
   SELECT * FROM "TissGlosas"
   WHERE "TissGuideId" = 'uuid-da-guia';
   ```

2. **Criar recurso:**
   ```http
   POST /api/tiss-glosa-appeals
   ```

   ```json
   {
     "glosaId": "uuid-da-glosa",
     "appealDate": "2026-02-10",
     "justification": "Procedimento foi autorizado...",
     "supportingDocuments": [...]
   }
   ```

3. **Anexar documentos:**
   - Autorização prévia
   - Prontuário médico
   - Exames complementares

---

## 🚀 Problemas de Performance {#problemas-performance}

### ❌ Listagem de guias está lenta

**Sintomas:**
- Consulta demora > 5 segundos
- Timeout no frontend

**Otimizações:**

1. **Adicionar paginação:**
   ```http
   GET /api/tiss-guides?page=1&pageSize=20
   ```

2. **Verificar índices:**
   ```sql
   -- Criar índices faltantes
   CREATE INDEX IF NOT EXISTS idx_tiss_guides_status 
   ON "TissGuides"("Status");
   
   CREATE INDEX IF NOT EXISTS idx_tiss_guides_operator_date
   ON "TissGuides"("OperatorId", "GuideDate");
   ```

3. **Filtrar por período:**
   ```http
   GET /api/tiss-guides?dateFrom=2026-01-01&dateTo=2026-01-31
   ```

---

### ❌ Geração de XML demora muito

**Sintomas:**
- Geração de lote > 2 minutos
- Timeout

**Causas:**
- Lote muito grande (> 100 guias)
- Consultas N+1

**Solução:**

1. **Dividir lote:**
   - Máximo 100 guias por lote

2. **Otimizar consultas:**
   ```csharp
   // Usar Include para evitar N+1
   var batch = await _context.TissBatches
       .Include(b => b.Guides)
           .ThenInclude(g => g.Procedures)
               .ThenInclude(p => p.TussProcedure)
       .FirstOrDefaultAsync(b => b.Id == batchId);
   ```

---

## 📊 Logs e Debugging {#logs-debugging}

### 🔍 Onde encontrar logs

**Application Logs:**
```bash
# Docker
docker logs medicsoft-api

# Seq (se configurado)
http://localhost:5341

# Arquivo
tail -f /var/log/medicsoft/application.log
```

**Database Logs:**
```sql
-- Logs de erro da aplicação
SELECT * FROM "ApplicationLogs"
WHERE "LogLevel" = 'Error'
  AND "Message" ILIKE '%TISS%'
ORDER BY "Timestamp" DESC
LIMIT 50;
```

---

### 🔧 Habilitar log detalhado

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "MedicSoft.Application.Services.Tiss": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

---

### 📋 Queries úteis para debugging

**1. Verificar integridade de dados:**

```sql
-- Guias sem procedimentos
SELECT tg."GuideNumber"
FROM "TissGuides" tg
LEFT JOIN "TissGuideProcedures" tgp ON tgp."TissGuideId" = tg."Id"
WHERE tgp."Id" IS NULL;

-- Lotes vazios
SELECT tb."BatchNumber"
FROM "TissBatches" tb
LEFT JOIN "TissGuides" tg ON tg."TissBatchId" = tb."Id"
WHERE tg."Id" IS NULL;

-- Pacientes sem plano ativo
SELECT p."Name"
FROM "Patients" p
LEFT JOIN "PatientHealthInsurances" phi ON phi."PatientId" = p."Id" 
    AND phi."IsActive" = true
WHERE phi."Id" IS NULL;
```

**2. Estatísticas rápidas:**

```sql
-- Dashboard rápido
SELECT 
    COUNT(CASE WHEN "Status" = 'Created' THEN 1 END) as criadas,
    COUNT(CASE WHEN "Status" = 'InBatch' THEN 1 END) as em_lote,
    COUNT(CASE WHEN "Status" = 'Sent' THEN 1 END) as enviadas,
    COUNT(CASE WHEN "Status" = 'Paid' THEN 1 END) as pagas,
    COUNT(CASE WHEN "Status" = 'Glosada' THEN 1 END) as glosadas,
    SUM("TotalAmount") as valor_total
FROM "TissGuides"
WHERE "GuideDate" >= '2026-01-01';
```

---

## 🆘 Suporte Avançado

### Ainda com problemas?

1. **Verificar documentação:**
   - [TISS API Reference](./TISS_API_REFERENCE.md)
   - [TISS Implementation Status](../implementacoes/TISS_IMPLEMENTATION_STATUS.md)
   - [TISS Documentation Index](../regras-negocio/TISS_DOCUMENTATION_INDEX.md)

2. **Coletar informações:**
   - Logs de erro completos
   - Query SQL que falha
   - Request/Response da API
   - Screenshot do erro (se frontend)

3. **Criar issue no GitHub:**
   - Descrever problema detalhadamente
   - Incluir passos para reproduzir
   - Anexar logs e screenshots

4. **Entrar em contato:**
   - Email: suporte@primecare.com.br
   - Slack: #medicsoft-tiss

---

**Última Atualização:** Janeiro 2026  
**Versão:** 1.0  
**Mantido por:** Equipe MedicSoft
