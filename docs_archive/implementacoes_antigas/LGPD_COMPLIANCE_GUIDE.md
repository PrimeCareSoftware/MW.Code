# 📘 Guia de Compliance LGPD - MedicSoft

**Versão:** 2.0  
**Data:** 26 de Janeiro de 2026  
**Status:** ✅ Completo (Fase 2)

---

## 📜 Índice

1. [Visão Geral](#visão-geral)
2. [Artigos LGPD Atendidos](#artigos-lgpd-atendidos)
3. [Sistema de Auditoria](#sistema-de-auditoria)
4. [Direitos dos Titulares](#direitos-dos-titulares)
5. [Gestão de Consentimentos](#gestão-de-consentimentos)
6. [Segurança e Controles](#segurança-e-controles)
7. [Relatórios para ANPD](#relatórios-para-anpd)
8. [Checklist de Compliance](#checklist-de-compliance)

---

## 🎯 Visão Geral

O MedicSoft implementa controles técnicos e organizacionais para garantir conformidade total com a **Lei Geral de Proteção de Dados (Lei 13.709/2018)**, especialmente considerando a sensibilidade dos dados de saúde (Art. 11).

### Dados Tratados

| Categoria | Exemplos | Base Legal | Retenção |
|-----------|----------|------------|----------|
| **Dados Pessoais** | Nome, CPF, Email, Telefone | Art. 7º - Legítimo interesse | 20 anos (CFM 1.821/2007) |
| **Dados Sensíveis** | Prontuário médico, diagnósticos | Art. 11 - Prestação de serviços de saúde | 20 anos (CFM 1.821/2007) |
| **Dados de Saúde** | Prescrições, exames, alergias | Art. 11 - Prestação de serviços de saúde | 20 anos (CFM 1.821/2007) |

---

## ✅ Artigos LGPD Atendidos

### Art. 8º - Consentimento

**Implementação:**
- Entidade `DataConsentLog` registra todos os consentimentos
- Versão do texto do consentimento armazenada
- Data, hora, IP, User-Agent registrados
- Método de captura (WEB/MOBILE/PAPEL)
- Possibilidade de revogação a qualquer momento

**Endpoints:**
```
POST /api/consent
POST /api/consent/{id}/revoke
GET /api/consent/patient/{id}
```

---

### Art. 9º - Titular Pode Acessar Dados

**Implementação:**
- Interface de visualização de dados pessoais
- Histórico completo de acessos aos dados
- Transparência sobre quem acessou e quando

**Endpoints:**
```
GET /api/patients/{id}
GET /api/data-access-logs/patient/{id}
```

---

### Art. 18, I - Confirmação de Tratamento

**Implementação:**
- Confirmação automática de tratamento de dados
- Relatórios de atividades de tratamento
- Histórico completo de operações

**Como Atender:**
```sql
-- Query para confirmar tratamento
SELECT * FROM audit_logs 
WHERE entity_type = 'Patient' 
  AND entity_id = '[patient-id]'
ORDER BY timestamp DESC;
```

---

### Art. 18, II - Acesso aos Dados

**Implementação:**
- Portal do paciente com acesso completo aos dados
- APIs de consulta com autenticação forte
- Auditoria de todos os acessos

**Endpoints:**
```
GET /api/patients/{id}
GET /api/medical-records/patient/{id}
GET /api/appointments/patient/{id}
GET /api/prescriptions/patient/{id}
```

---

### Art. 18, III - Correção de Dados

**Implementação:**
- Paciente pode solicitar correção de dados incompletos ou imprecisos
- Workflow de aprovação por profissional responsável
- Histórico de correções (versionamento)

**Endpoints:**
```
PUT /api/patients/{id}
PATCH /api/patients/{id}
```

**Auditoria:**
- Toda correção é registrada em `audit_logs` com valores antigos e novos

---

### Art. 18, IV - Anonimização, Bloqueio ou Eliminação

**Implementação:**
- Entidade `DataDeletionRequest` gerencia solicitações
- Status: Pending → Processing → Completed/Rejected
- Anonimização mantém dados clínicos (CFM 1.821/2007)
- Aprovação legal obrigatória

**Processo de Anonimização:**
1. Paciente solicita via API ou interface
2. Requisição fica pendente de aprovação legal
3. DPO/Legal aprova
4. Sistema anonimiza:
   - Nome → "Paciente Anonimizado {GUID}"
   - Email → "anonymized.{GUID}@example.com"
   - Telefone → "+55 00000000000"
   - CPF → Gerado aleatório válido
   - Endereço → "Rua Anonimizada, 0000..."
5. Dados clínicos mantidos para fins estatísticos

**Endpoints:**
```
POST /api/datadeletion/request
POST /api/datadeletion/{id}/process
POST /api/datadeletion/{id}/complete
POST /api/datadeletion/{id}/legal-approval
```

**Serviço:**
```csharp
await _dataDeletionService.AnonymizePatientDataAsync(patientId, tenantId);
```

---

### Art. 18, V - Portabilidade de Dados

**Implementação:**
- Exportação em JSON (estruturado, legível por máquinas)
- Exportação em XML (compatibilidade com sistemas legados)
- Exportação em PDF (legível por humanos, formatado)
- Pacote ZIP completo (todos os formatos + README)

**GatherPatientDataAsync - Dados Coletados:**
```json
{
  "ExportMetadata": {
    "ExportDate": "2026-01-26T12:00:00Z",
    "PatientId": "...",
    "LgpdCompliance": "LGPD Lei 13.709/2018 - Art. 18, V (Portabilidade)"
  },
  "PersonalInformation": { /* Nome, email, telefone, documento, etc */ },
  "MedicalRecords": [ /* Prontuários completos */ ],
  "Appointments": [ /* Histórico de consultas */ ],
  "Prescriptions": [ /* Prescrições médicas */ ],
  "ExamRequests": [ /* Solicitações de exames */ ],
  "Consents": [ /* Histórico de consentimentos */ ],
  "DataAccessHistory": [ /* Quem acessou e quando */ ],
  "LgpdRights": { /* Informações sobre direitos do titular */ }
}
```

**PDF Export - QuestPDF:**
- Documento profissional formatado
- Seções: Informações Pessoais, Registros Médicos, Consentimentos, Direitos LGPD
- Rodapé: Número de página, informações sobre LGPD
- Cabeçalho: Data de exportação, ID do paciente, referência legal

**Endpoints:**
```
GET /api/dataportability/patient/{id}/export/json
GET /api/dataportability/patient/{id}/export/xml
GET /api/dataportability/patient/{id}/export/pdf
GET /api/dataportability/patient/{id}/export/package (ZIP)
```

**Prazo:** Exportação em menos de 30 segundos

---

### Art. 18, IX - Revogação de Consentimento

**Implementação:**
- Botão de revogação em cada consentimento ativo
- Revogação imediata com registro de motivo
- Histórico completo de revogações
- Notificação ao titular sobre consequências

**Endpoints:**
```
POST /api/consent/{id}/revoke
```

**Serviço:**
```csharp
await _consentService.RevokeConsentAsync(consentId, reason, tenantId);
```

---

### Art. 37 - Registro de Operações de Tratamento

**Implementação:**

#### 1. Middleware Global (LgpdAuditMiddleware)
Registra automaticamente todas as operações em endpoints sensíveis:

**Endpoints Auditados:**
- `/api/patients` - Dados pessoais
- `/api/medical-records` - Dados sensíveis de saúde
- `/api/appointments` - Agendamentos
- `/api/prescriptions` - Prescrições médicas
- `/api/exam-requests` - Exames
- `/api/consent` - Consentimentos
- `/api/data-portability` - Portabilidade
- `/api/data-deletion` - Direito ao esquecimento

**Informações Registradas:**
```json
{
  "userId": "...",
  "userName": "...",
  "userEmail": "...",
  "action": "READ|CREATE|UPDATE|DELETE|EXPORT",
  "entityType": "Patient|MedicalRecord|...",
  "entityId": "...",
  "ipAddress": "...",
  "userAgent": "...",
  "requestPath": "/api/...",
  "httpMethod": "GET|POST|PUT|DELETE",
  "result": "SUCCESS|FAILED|UNAUTHORIZED",
  "dataCategory": "PUBLIC|PERSONAL|SENSITIVE|CONFIDENTIAL",
  "purpose": "HEALTHCARE|BILLING|LEGAL_OBLIGATION|...",
  "severity": "INFO|WARNING|ERROR|CRITICAL",
  "timestamp": "2026-01-26T12:00:00Z"
}
```

#### 2. Tabela audit_logs
- Retenção: 7+ anos (conformidade LGPD)
- Imutabilidade: Append-only (sem UPDATE/DELETE)
- Backup: Automático diário
- Índices otimizados para consultas

**Schema:**
```sql
CREATE TABLE audit_logs (
  id UUID PRIMARY KEY,
  timestamp TIMESTAMP NOT NULL,
  user_id VARCHAR NOT NULL,
  user_name VARCHAR NOT NULL,
  user_email VARCHAR NOT NULL,
  action VARCHAR NOT NULL,
  entity_type VARCHAR NOT NULL,
  entity_id VARCHAR NOT NULL,
  ip_address VARCHAR NOT NULL,
  user_agent TEXT,
  request_path VARCHAR NOT NULL,
  http_method VARCHAR NOT NULL,
  result VARCHAR NOT NULL,
  data_category VARCHAR NOT NULL,
  purpose VARCHAR NOT NULL,
  severity VARCHAR NOT NULL,
  tenant_id VARCHAR NOT NULL,
  old_values JSONB,
  new_values JSONB,
  changed_fields TEXT[],
  failure_reason TEXT,
  status_code INTEGER
);

-- Índices para performance
CREATE INDEX idx_audit_logs_timestamp ON audit_logs(timestamp);
CREATE INDEX idx_audit_logs_user_id ON audit_logs(user_id);
CREATE INDEX idx_audit_logs_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX idx_audit_logs_tenant ON audit_logs(tenant_id);
```

---

## 🔐 Segurança e Controles

### 1. Controles de Acesso

- **Autenticação:** JWT com refresh tokens
- **Autorização:** Role-based access control (RBAC)
- **MFA:** Autenticação multifator para operações críticas
- **Logs:** Todos os acessos são registrados

### 2. Criptografia

- **Em trânsito:** TLS 1.3
- **Em repouso:** AES-256 (banco de dados)
- **Backups:** Criptografados

### 3. Auditoria

- **Automática:** Middleware captura todas as operações
- **Imutável:** Logs não podem ser alterados
- **Rastreável:** Quem, quando, o quê, por quê

---

## 📊 Relatórios para ANPD

### 1. Relatório de Acessos (últimos 6 meses)

```sql
SELECT 
    DATE(timestamp) as Data,
    COUNT(*) as TotalAcessos,
    COUNT(DISTINCT user_id) as UsuariosUnicos,
    COUNT(CASE WHEN data_category = 'SENSITIVE' THEN 1 END) as AcessosDadosSensiveis
FROM audit_logs
WHERE timestamp >= NOW() - INTERVAL '6 months'
  AND tenant_id = '{tenant-id}'
GROUP BY DATE(timestamp)
ORDER BY Data DESC;
```

### 2. Relatório de Incidentes (últimos 30 dias)

```sql
SELECT *
FROM audit_logs
WHERE result = 'UNAUTHORIZED'
  AND timestamp >= NOW() - INTERVAL '30 days'
  AND tenant_id = '{tenant-id}'
ORDER BY timestamp DESC;
```

### 3. Relatório de Portabilidade

```sql
SELECT 
    COUNT(*) as TotalExportacoes,
    AVG(EXTRACT(EPOCH FROM (completed_at - created_at))) as TempoMedioSegundos
FROM data_portability_requests
WHERE tenant_id = '{tenant-id}'
  AND status = 'COMPLETED';
```

### 4. Relatório de Exclusões/Anonimizações

```sql
SELECT 
    request_type,
    status,
    COUNT(*) as Total
FROM data_deletion_requests
WHERE tenant_id = '{tenant-id}'
GROUP BY request_type, status;
```

---

## ✅ Checklist de Compliance LGPD

### Requisitos Técnicos

- [x] Sistema de auditoria implementado
- [x] Logs de todas as operações sensíveis
- [x] Gestão de consentimentos
- [x] Portabilidade de dados (JSON, XML, PDF)
- [x] Direito ao esquecimento (anonimização)
- [x] Criptografia em trânsito e em repouso
- [x] Controles de acesso (RBAC)
- [x] Autenticação forte (JWT + MFA)
- [x] Backup automático de logs
- [x] Retenção de dados por 7+ anos

### Requisitos Organizacionais

- [ ] Nomear DPO (Data Protection Officer)
- [ ] Criar política de privacidade
- [ ] Treinar equipe em LGPD
- [ ] Estabelecer processos de resposta a incidentes
- [ ] Documentar fluxos de tratamento de dados
- [ ] Realizar DPIA (Data Protection Impact Assessment)

### Requisitos Legais

- [x] Art. 8º - Consentimento ✅
- [x] Art. 9º - Acesso do titular ✅
- [x] Art. 18, I - Confirmação de tratamento ✅
- [x] Art. 18, II - Acesso aos dados ✅
- [x] Art. 18, III - Correção ✅
- [x] Art. 18, IV - Anonimização/Exclusão ✅
- [x] Art. 18, V - Portabilidade ✅
- [x] Art. 18, IX - Revogação de consentimento ✅
- [x] Art. 37 - Registro de operações ✅
- [x] Art. 46 - Segurança da informação ✅

---

## 📞 Contato

**Data Protection Officer (DPO):**  
Email: dpo@medicsoft.com.br  
Telefone: +55 (XX) XXXX-XXXX

**Suporte Técnico:**  
Email: suporte@medicsoft.com.br  
Telefone: +55 (XX) XXXX-XXXX

---

## 📚 Referências

1. [Lei Geral de Proteção de Dados - Lei 13.709/2018](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
2. [Guia ANPD - Segurança da Informação](https://www.gov.br/anpd/)
3. [CFM Resolução 1.821/2007](https://www.in.gov.br/materia/-/asset_publisher/Kujrw0TZC2Mb/content/id/35393339) - Retenção de prontuários
4. [CFM Resolução 1.638/2002](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2002/1638) - Prontuário eletrônico

---

**Última Atualização:** 26 de Janeiro de 2026  
**Versão do Sistema:** 2.0 (Fase 2 - Completa)
