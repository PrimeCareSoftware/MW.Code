# 🔒 Sistema de Auditoria e Compliance LGPD

## 📋 Visão Geral

O MedicSoft implementa um sistema completo de auditoria e compliance com a **Lei Geral de Proteção de Dados (LGPD - Lei 13.709/2018)**, garantindo a rastreabilidade de todas as operações sensíveis e o atendimento aos direitos dos titulares de dados.

## 🎯 Funcionalidades Implementadas

### 1. Sistema de Auditoria Completo

#### **AuditLog** - Registro de Ações
Todas as operações do sistema são registradas automaticamente:

- ✅ Operações CRUD (Create, Read, Update, Delete)
- ✅ Autenticação (Login, Logout, falhas de login)
- ✅ Mudanças de senha e MFA
- ✅ Exportação e impressão de dados
- ✅ Compartilhamento de informações
- ✅ Acessos negados e tentativas não autorizadas

**Campos registrados:**
- Usuário (ID, nome, email)
- Ação executada
- Entidade afetada
- IP e User-Agent
- Valores antes/depois (para updates)
- Resultado (sucesso/falha/não autorizado)
- Categoria de dados (LGPD)
- Finalidade legal

#### **DataAccessLog** - Rastreamento de Acesso a Dados Sensíveis
Registro específico para acessos a dados pessoais e de saúde (LGPD Art. 37):

- Quem acessou (usuário e papel)
- O que foi acessado (entidade e campos específicos)
- Paciente titular dos dados
- Motivo do acesso
- Autorização (aprovado/negado)

### 2. Gestão de Consentimentos

#### **DataConsentLog** - Registro de Consentimentos
Mantém histórico completo de consentimentos (LGPD Art. 8):

**Tipos de consentimento:**
- Tratamento médico
- Compartilhamento de dados
- Marketing e comunicações
- Pesquisa clínica
- Telemedicina

**Funcionalidades:**
- ✅ Registro de consentimento com data e hora
- ✅ Texto exato apresentado ao titular
- ✅ Versão do termo de consentimento
- ✅ Método de consentimento (WEB, MOBILE, PAPEL)
- ✅ IP e User-Agent como evidência
- ✅ Revogação de consentimento
- ✅ Expiração automática
- ✅ Consulta de status ativo/revogado/expirado

#### **APIs de Consentimento**
```
POST   /api/consent                      - Registra novo consentimento
POST   /api/consent/{id}/revoke          - Revoga consentimento
GET    /api/consent/patient/{id}         - Lista consentimentos do paciente
GET    /api/consent/patient/{id}/active  - Lista consentimentos ativos
GET    /api/consent/patient/{id}/has-consent?purpose=Treatment - Verifica consentimento
```

### 3. Direito ao Esquecimento (Art. 18, VI)

#### **DataDeletionRequest** - Solicitações de Exclusão
Sistema completo para gerenciar requisições de exclusão/anonimização:

**Tipos de exclusão:**
- Completa - Exclusão total dos dados
- Anonimização - Mantém dados estatísticos
- Parcial - Exclusão específica

**Workflow:**
1. Paciente solicita exclusão
2. Requisição entra em fila (status: Pending)
3. Administrador processa (status: Processing)
4. Aprovação legal (se necessário)
5. Execução da anonimização (status: Completed)
6. Ou rejeição com motivo (status: Rejected)

#### **APIs de Direito ao Esquecimento**
```
POST   /api/datadeletion/request                - Cria requisição de exclusão
POST   /api/datadeletion/{id}/process          - Processa requisição (Admin)
POST   /api/datadeletion/{id}/complete         - Completa exclusão (Admin)
POST   /api/datadeletion/{id}/reject           - Rejeita requisição
POST   /api/datadeletion/{id}/legal-approval   - Aprova legalmente
GET    /api/datadeletion/pending               - Lista requisições pendentes
GET    /api/datadeletion/patient/{id}          - Lista requisições do paciente
```

### 4. Portabilidade de Dados (Art. 18, V)

#### **DataPortabilityService** - Exportação de Dados
O paciente pode solicitar exportação completa de seus dados em múltiplos formatos:

**Formatos disponíveis:**
- 📄 JSON - Formato estruturado para integração
- 📄 XML - Formato estruturado alternativo
- 📄 PDF - Relatório legível por humanos
- 📦 Pacote ZIP - Todos os formatos + README

**Dados incluídos:**
- Informações pessoais
- Histórico médico completo
- Consultas e atendimentos
- Prescrições e receitas
- Resultados de exames
- Consentimentos registrados

#### **APIs de Portabilidade**
```
GET    /api/dataportability/patient/{id}/export/json     - Exporta como JSON
GET    /api/dataportability/patient/{id}/export/xml      - Exporta como XML
GET    /api/dataportability/patient/{id}/export/pdf      - Exporta como PDF
GET    /api/dataportability/patient/{id}/export/package  - Pacote completo ZIP
GET    /api/dataportability/info                         - Informações sobre portabilidade
```

## 📊 Estrutura do Banco de Dados

### Tabelas de Auditoria

#### `audit_logs`
```sql
- id (uuid, PK)
- timestamp (timestamp)
- user_id, user_name, user_email
- action (enum)
- entity_type, entity_id
- ip_address, user_agent
- old_values, new_values (jsonb)
- result (enum)
- data_category (enum)
- lgpd_purpose (enum)
- tenant_id
```

#### `data_access_logs`
```sql
- id (uuid, PK)
- timestamp (timestamp)
- user_id, user_name, user_role
- entity_type, entity_id
- fields_accessed (jsonb)
- patient_id, patient_name
- access_reason
- ip_address, location
- was_authorized (boolean)
- denial_reason
- tenant_id
```

#### `data_consent_logs`
```sql
- id (uuid, PK)
- patient_id, patient_name
- type (enum)
- purpose (enum)
- description
- status (enum: Active, Revoked, Expired)
- consent_date, expiration_date, revoked_date
- revocation_reason
- ip_address, user_agent
- consent_text, consent_version
- consent_method
- tenant_id
```

#### `data_deletion_requests`
```sql
- id (uuid, PK)
- patient_id, patient_name, patient_email
- request_date
- reason
- request_type (enum: Complete, Anonymization, Partial)
- status (enum: Pending, Processing, Completed, Rejected)
- processed_date, completed_date
- processed_by_user_id, processed_by_user_name
- processing_notes, rejection_reason
- requires_legal_approval (boolean)
- legal_approval_date, legal_approver
- ip_address, user_agent
- tenant_id
```

## 🔧 Configuração e Uso

### 1. Registro Automático de Auditoria

As operações são auditadas automaticamente através do `AuditService`:

```csharp
// Injetado automaticamente via DI
private readonly IAuditService _auditService;

// Registrar ação
await _auditService.LogAsync(new CreateAuditLogDto
{
    UserId = userId,
    UserName = userName,
    Action = AuditAction.READ,
    EntityType = "Patient",
    EntityId = patientId.ToString(),
    // ... outros campos
});
```

### 2. Gestão de Consentimentos

```csharp
// Registrar consentimento
var consentId = await _consentService.RecordConsentAsync(
    patientId: patientId,
    patientName: "João Silva",
    type: ConsentType.MedicalTreatment,
    purpose: ConsentPurpose.Treatment,
    description: "Consentimento para procedimento X",
    expirationDate: DateTime.UtcNow.AddYears(1),
    ipAddress: "192.168.1.1",
    consentText: "Texto completo do termo...",
    consentVersion: "1.0",
    consentMethod: "WEB",
    userAgent: "Mozilla/5.0...",
    tenantId: tenantId
);

// Revogar consentimento
await _consentService.RevokeConsentAsync(
    consentId, 
    reason: "Paciente solicitou revogação",
    tenantId
);

// Verificar consentimento ativo
bool hasConsent = await _consentService.HasActiveConsentAsync(
    patientId, 
    ConsentPurpose.Treatment,
    tenantId
);
```

### 3. Direito ao Esquecimento

```csharp
// Solicitar exclusão
var requestId = await _deletionService.RequestDataDeletionAsync(
    patientId: patientId,
    patientName: "João Silva",
    patientEmail: "joao@email.com",
    reason: "Não utilizo mais o serviço",
    requestType: DeletionRequestType.Anonymization,
    ipAddress: "192.168.1.1",
    userAgent: "Mozilla/5.0...",
    requiresLegalApproval: true,
    tenantId: tenantId
);

// Admin processa
await _deletionService.ProcessDataDeletionRequestAsync(
    requestId,
    userId: adminId,
    userName: "Admin",
    notes: "Verificado e aprovado",
    tenantId
);

// Completa anonimização
await _deletionService.CompleteDataDeletionRequestAsync(requestId, tenantId);
```

### 4. Exportação de Dados

```csharp
// JSON
var jsonData = await _portabilityService.ExportPatientDataAsJsonAsync(
    patientId, 
    tenantId
);

// XML
var xmlData = await _portabilityService.ExportPatientDataAsXmlAsync(
    patientId,
    tenantId
);

// PDF
var pdfData = await _portabilityService.ExportPatientDataAsPdfAsync(
    patientId,
    tenantId
);

// Pacote completo
var package = await _portabilityService.CreatePatientDataPackageAsync(
    patientId,
    tenantId
);
```

## ⚖️ Conformidade LGPD

### Artigos Atendidos

| Artigo | Descrição | Status |
|--------|-----------|--------|
| **Art. 8** | Consentimento do titular | ✅ Implementado |
| **Art. 18, I** | Confirmação de tratamento de dados | ✅ Implementado |
| **Art. 18, II** | Acesso aos dados | ✅ Implementado |
| **Art. 18, V** | Portabilidade dos dados | ✅ Implementado |
| **Art. 18, VI** | Eliminação dos dados (direito ao esquecimento) | ✅ Implementado |
| **Art. 18, IX** | Revogação do consentimento | ✅ Implementado |
| **Art. 37** | Registro de operações de tratamento | ✅ Implementado |

### Bases Legais (Art. 7)

O sistema registra a base legal para cada operação:
- **Consentimento** - Para marketing, pesquisa
- **Obrigação legal** - Para registros médicos obrigatórios
- **Execução de contrato** - Para prestação de serviços de saúde
- **Interesse legítimo** - Para melhorias de qualidade

### Categorias de Dados

- **Públicos** - Dados não sensíveis
- **Pessoais** - Nome, CPF, endereço, etc.
- **Sensíveis** - Dados de saúde, biométricos (Art. 11)
- **Confidenciais** - Segredos comerciais

## 🔐 Segurança

### Proteção dos Logs de Auditoria

- ✅ **Append-only** - Logs são imutáveis, apenas inserção
- ✅ **Acesso restrito** - Apenas SystemAdmin e ClinicOwner
- ✅ **Criptografia** - Dados sensíveis criptografados em repouso
- ✅ **Backup automático** - Retenção por 7+ anos
- ✅ **Particionamento** - Por tenant e período

### Indexes de Performance

```sql
-- DataAccessLogs
CREATE INDEX idx_data_access_logs_patient_id ON data_access_logs(patient_id);
CREATE INDEX idx_data_access_logs_user_id ON data_access_logs(user_id);
CREATE INDEX idx_data_access_logs_timestamp ON data_access_logs(timestamp);

-- DataConsentLogs
CREATE INDEX idx_data_consent_logs_patient_id ON data_consent_logs(patient_id);
CREATE INDEX idx_data_consent_logs_status ON data_consent_logs(status);
CREATE INDEX idx_data_consent_logs_patient_status ON data_consent_logs(patient_id, status);

-- DataDeletionRequests
CREATE INDEX idx_data_deletion_requests_patient_id ON data_deletion_requests(patient_id);
CREATE INDEX idx_data_deletion_requests_status ON data_deletion_requests(status);
```

## 📈 Relatórios e Dashboards

### Relatórios Disponíveis

1. **Relatório de Atividades por Usuário**
   - GET `/api/audit/user/{userId}`
   - Todas as ações de um usuário específico

2. **Histórico de Entidade**
   - GET `/api/audit/entity/{type}/{id}`
   - Todas as mudanças em uma entidade

3. **Eventos de Segurança**
   - GET `/api/audit/security-events`
   - Tentativas de acesso não autorizado

4. **Relatório LGPD**
   - GET `/api/audit/lgpd-report/{userId}`
   - Relatório completo para ANPD

5. **Histórico de Acesso do Paciente**
   - Quem acessou dados do paciente e quando
   - Finalidade do acesso

## 🧪 Testes

### Testes Unitários

Testar os serviços principais:
```bash
dotnet test --filter Category=LGPD
```

### Testes de Compliance

1. ✅ Verificar que todas operações sensíveis são logadas
2. ✅ Confirmar que logs são imutáveis
3. ✅ Testar exportação de dados de paciente
4. ✅ Validar processo de direito ao esquecimento
5. ✅ Verificar registro de consentimentos

## 📚 Referências

### Legislação
- [Lei 13.709/2018 - LGPD](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [Guia ANPD - Segurança da Informação](https://www.gov.br/anpd/)
- [CFM Resolução 1.821/2007](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2007/1821) - Prontuário Médico

### Documentação Técnica
- [Guia Completo de Implementação](./Plano_Desenvolvimento/fase-2-seguranca-lgpd/08-auditoria-lgpd.md) - 2.857 linhas de código e configuração
- [Documentação da API](./API_DOCUMENTATION.md)
- [Guia de Segurança](./SECURITY.md)
- [OWASP Logging Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Logging_Cheat_Sheet.html)

## 🚀 Próximos Passos

### Melhorias Futuras

- [ ] Dashboard visual de auditoria
- [ ] Alertas automáticos de atividades suspeitas
- [ ] Exportação de relatórios para ANPD
- [ ] Elasticsearch para busca avançada de logs
- [ ] Machine Learning para detecção de anomalias
- [ ] Integração com sistemas externos (TISS, eSocial)

### Manutenção

- Revisar consentimentos expirados mensalmente
- Processar requisições de exclusão em até 48 horas
- Auditar logs de segurança semanalmente
- Manter backup de logs por no mínimo 7 anos
- Atualizar documentação conforme mudanças na LGPD

---

## 📞 Suporte

Para dúvidas sobre o sistema de auditoria LGPD:
- Email: dpo@primecaresoftware.com
- Documentação: https://docs.primecaresoftware.com/lgpd
- Implementação Detalhada: [Plano de Desenvolvimento - Fase 2](./Plano_Desenvolvimento/fase-2-seguranca-lgpd/)

**Última atualização:** 26 de Janeiro de 2026  
**Versão:** 2.0.0 - Documentação Completa Expandida
