# Implementação TISS Fase 1 - Resumo de Progresso

## 📊 Status da Implementação

**Data:** Janeiro 2026  
**Versão:** 1.0  
**Status Geral:** 40% completo (camadas de domínio e repositório)

## ✅ O Que Foi Implementado

### 1. Camada de Domínio (100% completo)

Foram criadas **8 novas entidades** seguindo o padrão DDD estabelecido no projeto:

#### 1.1 HealthInsuranceOperator
- **Arquivo:** `src/MedicSoft.Domain/Entities/HealthInsuranceOperator.cs`
- **Descrição:** Representa uma operadora de plano de saúde (ex: Unimed, Bradesco Saúde)
- **Campos principais:**
  - Nome comercial e razão social
  - Registro ANS (obrigatório)
  - CNPJ
  - Configurações de integração (Manual, WebPortal, TissXml, RestApi)
  - Configurações TISS (versão, suporte a XML, email para lotes)
- **Métodos:** UpdateBasicInfo, ConfigureIntegration, ConfigureTiss, Activate, Deactivate

#### 1.2 PatientHealthInsurance
- **Arquivo:** `src/MedicSoft.Domain/Entities/PatientHealthInsurance.cs`
- **Descrição:** Representa o vínculo entre paciente e plano de saúde (carteirinha)
- **Campos principais:**
  - Número da carteirinha e código de validação
  - Período de validade
  - Informações do titular (se dependente)
- **Métodos:** UpdateCardInfo, UpdateValidityPeriod, UpdateHolderInfo, IsValid

#### 1.3 AuthorizationRequest
- **Arquivo:** `src/MedicSoft.Domain/Entities/AuthorizationRequest.cs`
- **Descrição:** Solicitação de autorização prévia de procedimentos
- **Campos principais:**
  - Número da solicitação
  - Status (Pending, Approved, Denied, Expired, Cancelled)
  - Procedimento solicitado (código TUSS)
  - Número de autorização da operadora
  - Indicação clínica e diagnóstico
- **Métodos:** Approve, Deny, Cancel, MarkAsExpired, IsExpired, IsValidForUse

#### 1.4 TissBatch
- **Arquivo:** `src/MedicSoft.Domain/Entities/TissBatch.cs`
- **Descrição:** Lote de faturamento TISS para envio à operadora
- **Campos principais:**
  - Número do lote
  - Status (Draft, ReadyToSend, Sent, Processing, Processed, PartiallyPaid, Paid, Rejected)
  - Arquivos XML (gerado e resposta)
  - Valores aprovados e glosados
  - Protocolo de recebimento
- **Métodos:** AddGuide, RemoveGuide, GenerateXml, Submit, ProcessResponse, MarkAsPaid

#### 1.5 TissGuide
- **Arquivo:** `src/MedicSoft.Domain/Entities/TissGuide.cs`
- **Descrição:** Guia TISS individual de atendimento
- **Campos principais:**
  - Número da guia
  - Tipo (Consultation, SPSADT, Hospitalization, Fees, Dental)
  - Status (Draft, Sent, Approved, PartiallyApproved, Rejected, Paid)
  - Valor total e valores de glosa
  - Número de autorização
- **Métodos:** AddProcedure, RemoveProcedure, MarkAsSent, Approve, Reject, MarkAsPaid

#### 1.6 TissGuideProcedure
- **Arquivo:** `src/MedicSoft.Domain/Entities/TissGuideProcedure.cs`
- **Descrição:** Procedimento dentro de uma guia TISS
- **Campos principais:**
  - Código TUSS do procedimento
  - Quantidade e preço unitário
  - Valores aprovados e glosados pela operadora
- **Métodos:** UpdateQuantity, UpdateUnitPrice, ProcessOperatorResponse

#### 1.7 TussProcedure
- **Arquivo:** `src/MedicSoft.Domain/Entities/TussProcedure.cs`
- **Descrição:** Tabela de procedimentos TUSS (ANS)
- **Campos principais:**
  - Código TUSS (8 dígitos)
  - Descrição do procedimento
  - Categoria (Consultas, Exames, Cirurgias, etc.)
  - Preço de referência (AMB/CBHPM)
  - Requer autorização prévia?
- **Métodos:** UpdateInfo, UpdateReferencePrice, Activate, Deactivate

#### 1.8 HealthInsurancePlan (expandido)
- **Arquivo:** `src/MedicSoft.Domain/Entities/HealthInsurancePlan.cs`
- **Descrição:** Plano de saúde da operadora (expandido com campos TISS)
- **Novos campos:**
  - Vínculo com operadora (OperatorId)
  - Código do plano na operadora
  - Registro ANS do plano
  - Tipo do plano (Individual, Enterprise, Collective)
  - Coberturas (consultas, exames, procedimentos)
  - Requer autorização prévia?
- **Retrocompatibilidade:** Campos antigos marcados como `[Obsolete]` mas mantidos

---

### 2. Camada de Repositório (100% completo)

Foram criadas **7 interfaces e 7 implementações de repositórios**:

#### Interfaces (src/MedicSoft.Domain/Interfaces/)
1. `IHealthInsuranceOperatorRepository` - Busca por registro ANS, documento, nome
2. `IPatientHealthInsuranceRepository` - Busca por paciente, carteirinha, plano
3. `IAuthorizationRequestRepository` - Busca por status, paciente, número de autorização
4. `ITissBatchRepository` - Busca por clínica, operadora, status, com guias
5. `ITissGuideRepository` - Busca por lote, agendamento, status, com procedimentos
6. `ITissGuideProcedureRepository` - Busca por guia, código TUSS
7. `ITussProcedureRepository` - Busca por código, descrição, categoria

#### Implementações (src/MedicSoft.Repository/Repositories/)
- Todos os repositórios estendem `BaseRepository<T>`
- Incluem queries otimizadas com `Include()` para navegação
- Filtram por `TenantId` para isolamento multi-tenant
- Suportam paginação e ordenação

---

### 3. Configuração Entity Framework (100% completo)

Foram criadas **7 configurações EF** (src/MedicSoft.Repository/Configurations/):

1. `HealthInsuranceOperatorConfiguration` - Índices por registro ANS, nome comercial
2. `PatientHealthInsuranceConfiguration` - Índice único por carteirinha
3. `AuthorizationRequestConfiguration` - Índices por status, número de autorização
4. `TissBatchConfiguration` - Relacionamentos com Clinic e Operator, cascades corretos
5. `TissGuideConfiguration` - Relacionamentos com Batch, Appointment, PatientHealthInsurance
6. `TissGuideProcedureConfiguration` - Relacionamento com TissGuide
7. `TussProcedureConfiguration` - Índice único por código TUSS

**HealthInsurancePlanConfiguration** foi expandido mantendo retrocompatibilidade:
- Novos campos: PlanName, PlanCode, RegisterNumber, Type, Coverages
- Campos legados preservados com mesmo nome de coluna
- Novo relacionamento com HealthInsuranceOperator

#### DbContext Atualizado
- Arquivo: `src/MedicSoft.Repository/Context/MedicSoftDbContext.cs`
- Adicionados 7 novos DbSets
- Aplicadas todas as configurações
- Comentários indicando "TISS Phase 1"

---

## 📋 O Que Falta Implementar

### 4. Migrations (Próximo passo crítico)

**Prioridade: ALTA**

Criar migrations para adicionar as novas tabelas ao banco de dados:

```bash
# No diretório src/MedicSoft.Repository
dotnet ef migrations add AddTissPhase1Entities --context MedicSoftDbContext --output-dir Migrations/PostgreSQL
```

**Tabelas a serem criadas:**
1. HealthInsuranceOperators
2. PatientHealthInsurances
3. AuthorizationRequests
4. TissBatches
5. TissGuides
6. TissGuideProcedures
7. TussProcedures

**Alterações em tabelas existentes:**
- HealthInsurancePlans (adicionar colunas: OperatorId, PlanName, PlanCode, RegisterNumber, Type, CoversConsultations, CoversExams, CoversProcedures, RequiresPriorAuthorization)

---

### 5. Camada de Aplicação (Services)

**Prioridade: ALTA**

Criar serviços de aplicação para orquestrar a lógica de negócio:

#### 5.1 HealthInsuranceOperatorService
```csharp
// src/MedicSoft.Application/Services/HealthInsuranceOperatorService.cs
public interface IHealthInsuranceOperatorService
{
    Task<HealthInsuranceOperatorDto> CreateAsync(CreateHealthInsuranceOperatorDto dto);
    Task<HealthInsuranceOperatorDto> UpdateAsync(Guid id, UpdateHealthInsuranceOperatorDto dto);
    Task<IEnumerable<HealthInsuranceOperatorDto>> GetAllAsync(string tenantId);
    Task<HealthInsuranceOperatorDto?> GetByIdAsync(Guid id, string tenantId);
    Task DeleteAsync(Guid id, string tenantId);
}
```

#### 5.2 PatientHealthInsuranceService
```csharp
// src/MedicSoft.Application/Services/PatientHealthInsuranceService.cs
public interface IPatientHealthInsuranceService
{
    Task<PatientHealthInsuranceDto> LinkPatientToPlanAsync(LinkPatientToPlanDto dto);
    Task<IEnumerable<PatientHealthInsuranceDto>> GetByPatientIdAsync(Guid patientId, string tenantId);
    Task<bool> ValidateCardAsync(string cardNumber, string tenantId);
}
```

#### 5.3 AuthorizationRequestService
```csharp
// src/MedicSoft.Application/Services/AuthorizationRequestService.cs
public interface IAuthorizationRequestService
{
    Task<AuthorizationRequestDto> CreateRequestAsync(CreateAuthorizationRequestDto dto);
    Task<AuthorizationRequestDto> ApproveAsync(Guid id, string authorizationNumber, DateTime? expirationDate);
    Task<AuthorizationRequestDto> DenyAsync(Guid id, string denialReason);
    Task<IEnumerable<AuthorizationRequestDto>> GetPendingAsync(string tenantId);
}
```

#### 5.4 TissGuideService
```csharp
// src/MedicSoft.Application/Services/TissGuideService.cs
public interface ITissGuideService
{
    Task<TissGuideDto> CreateGuideAsync(CreateTissGuideDto dto);
    Task AddProcedureToGuideAsync(Guid guideId, AddProcedureDto dto);
    Task<TissGuideDto> FinalizeGuideAsync(Guid guideId);
}
```

#### 5.5 TissBatchService
```csharp
// src/MedicSoft.Application/Services/TissBatchService.cs
public interface ITissBatchService
{
    Task<TissBatchDto> CreateBatchAsync(CreateTissBatchDto dto);
    Task AddGuideToBatchAsync(Guid batchId, Guid guideId);
    Task<string> GenerateXmlAsync(Guid batchId); // Retorna caminho do XML gerado
    Task SubmitBatchAsync(Guid batchId);
    Task ProcessResponseAsync(Guid batchId, ProcessBatchResponseDto dto);
}
```

#### 5.6 TissXmlGeneratorService (CRÍTICO)
```csharp
// src/MedicSoft.Application/Services/TissXmlGeneratorService.cs
public interface ITissXmlGeneratorService
{
    Task<string> GenerateBatchXmlAsync(TissBatch batch);
    Task<bool> ValidateXmlAsync(string xmlPath);
    Task<TissXmlValidationResult> ValidateAgainstSchemaAsync(string xmlPath);
}
```

**Implementação:** Gerar XML conforme padrão TISS 4.02.00+, validar contra schemas XSD oficiais da ANS.

#### 5.7 TussProcedureService
```csharp
// src/MedicSoft.Application/Services/TussProcedureService.cs
public interface ITussProcedureService
{
    Task ImportTussTableAsync(string filePath); // Importar tabela TUSS oficial
    Task<IEnumerable<TussProcedureDto>> SearchProceduresAsync(string query, string tenantId);
    Task<TussProcedureDto?> GetByCodeAsync(string code, string tenantId);
}
```

---

### 6. Camada de API (Controllers)

**Prioridade: ALTA**

Criar controllers RESTful para expor a funcionalidade:

#### 6.1 HealthInsuranceOperatorController
```
GET    /api/health-insurance-operators          - Listar operadoras
GET    /api/health-insurance-operators/{id}     - Buscar operadora
POST   /api/health-insurance-operators          - Criar operadora
PUT    /api/health-insurance-operators/{id}     - Atualizar operadora
DELETE /api/health-insurance-operators/{id}     - Desativar operadora
```

#### 6.2 HealthInsurancePlanController (expandir existente)
```
GET    /api/health-insurance-plans                           - Listar planos
GET    /api/health-insurance-plans/{id}                      - Buscar plano
GET    /api/health-insurance-plans/operator/{operatorId}     - Planos por operadora
POST   /api/health-insurance-plans                           - Criar plano
PUT    /api/health-insurance-plans/{id}                      - Atualizar plano
DELETE /api/health-insurance-plans/{id}                      - Desativar plano
```

#### 6.3 PatientHealthInsuranceController
```
GET    /api/patients/{patientId}/health-insurance            - Listar planos do paciente
POST   /api/patients/{patientId}/health-insurance            - Vincular plano ao paciente
PUT    /api/patients/{patientId}/health-insurance/{id}       - Atualizar vínculo
DELETE /api/patients/{patientId}/health-insurance/{id}       - Desativar vínculo
GET    /api/patients/{patientId}/health-insurance/validate   - Validar elegibilidade
```

#### 6.4 AuthorizationRequestController
```
GET    /api/authorizations                      - Listar autorizações
GET    /api/authorizations/{id}                 - Buscar autorização
POST   /api/authorizations                      - Criar solicitação
PUT    /api/authorizations/{id}/approve         - Aprovar autorização
PUT    /api/authorizations/{id}/deny            - Negar autorização
DELETE /api/authorizations/{id}                 - Cancelar autorização
```

#### 6.5 TissGuideController
```
GET    /api/tiss/guides                         - Listar guias
GET    /api/tiss/guides/{id}                    - Buscar guia
POST   /api/tiss/guides                         - Criar guia
PUT    /api/tiss/guides/{id}                    - Atualizar guia
POST   /api/tiss/guides/{id}/procedures         - Adicionar procedimento
POST   /api/tiss/guides/{id}/finalize           - Finalizar guia
```

#### 6.6 TissBatchController
```
GET    /api/tiss/batches                        - Listar lotes
GET    /api/tiss/batches/{id}                   - Buscar lote
POST   /api/tiss/batches                        - Criar lote
POST   /api/tiss/batches/{id}/add-guide         - Adicionar guia ao lote
POST   /api/tiss/batches/{id}/generate-xml      - Gerar XML do lote
GET    /api/tiss/batches/{id}/download-xml      - Download do XML
POST   /api/tiss/batches/{id}/submit            - Enviar lote
POST   /api/tiss/batches/{id}/process-return    - Processar retorno
```

#### 6.7 TussProcedureController
```
GET    /api/tuss/procedures                     - Listar procedimentos TUSS
GET    /api/tuss/procedures/search              - Buscar por código ou descrição
GET    /api/tuss/procedures/{code}              - Buscar procedimento
POST   /api/tuss/procedures/import              - Importar tabela TUSS
```

---

### 7. Camada de Frontend (Angular)

**Prioridade: MÉDIA-ALTA**

#### 7.1 Operadoras
- `health-insurance-operators-list.component.ts` - Lista de operadoras
- `health-insurance-operator-form.component.ts` - Formulário criar/editar
- `health-insurance-operator-detail.component.ts` - Detalhes da operadora

#### 7.2 Planos
- `health-insurance-plans-list.component.ts` - Lista de planos
- `health-insurance-plan-form.component.ts` - Formulário criar/editar
- `health-insurance-plan-selector.component.ts` - Seletor de plano no cadastro de paciente

#### 7.3 Vínculo Paciente-Plano
- `patient-health-insurance-form.component.ts` - Vincular plano ao paciente
- `patient-health-insurance-card.component.ts` - Card exibindo plano do paciente

#### 7.4 Autorizações
- `authorization-request-list.component.ts` - Lista de autorizações
- `authorization-request-form.component.ts` - Solicitar autorização
- `authorization-pending-dashboard.component.ts` - Dashboard de pendentes

#### 7.5 Guias TISS
- `tiss-guide-list.component.ts` - Lista de guias
- `tiss-guide-form.component.ts` - Criar/editar guia
- `tiss-guide-procedures.component.ts` - Gerenciar procedimentos da guia

#### 7.6 Lotes de Faturamento
- `tiss-batch-list.component.ts` - Lista de lotes
- `tiss-batch-form.component.ts` - Criar lote
- `tiss-batch-detail.component.ts` - Detalhes do lote
- `tiss-batch-xml-preview.component.ts` - Preview do XML

#### 7.7 Relatórios
- `tiss-reports-dashboard.component.ts` - Dashboard de relatórios TISS
- `operator-performance.component.ts` - Performance por operadora

---

### 8. Documentação

**Prioridade: MÉDIA**

#### 8.1 Swagger/OpenAPI
- Adicionar anotações XML nos controllers
- Configurar Swagger para exibir exemplos de DTOs
- Documentar códigos de erro HTTP

#### 8.2 Guias de Uso
- `docs/TISS_USER_GUIDE.md` - Guia do usuário para clínicas
- `docs/TISS_INTEGRATION_GUIDE.md` - Guia técnico de integração
- `docs/TISS_FAQ.md` - Perguntas frequentes

#### 8.3 Documentação de Código
- Adicionar XML comments em todos os métodos públicos
- Documentar fluxos complexos com diagramas

---

### 9. Testes

**Prioridade: ALTA**

#### 9.1 Testes Unitários (Domínio)
```csharp
// tests/MedicSoft.Domain.Tests/Entities/
- HealthInsuranceOperatorTests.cs
- PatientHealthInsuranceTests.cs
- AuthorizationRequestTests.cs
- TissBatchTests.cs
- TissGuideTests.cs
- TissGuideProcedureTests.cs
- TussProcedureTests.cs
```

#### 9.2 Testes de Integração (Repositórios)
```csharp
// tests/MedicSoft.Repository.Tests/Repositories/
- HealthInsuranceOperatorRepositoryTests.cs
- PatientHealthInsuranceRepositoryTests.cs
- AuthorizationRequestRepositoryTests.cs
- TissBatchRepositoryTests.cs
- TissGuideRepositoryTests.cs
- TissGuideProcedureRepositoryTests.cs
- TussProcedureRepositoryTests.cs
```

#### 9.3 Testes de API (Controllers)
```csharp
// tests/MedicSoft.Api.Tests/Controllers/
- HealthInsuranceOperatorControllerTests.cs
- PatientHealthInsuranceControllerTests.cs
- AuthorizationRequestControllerTests.cs
- TissGuideControllerTests.cs
- TissBatchControllerTests.cs
- TussProcedureControllerTests.cs
```

#### 9.4 Testes de Validação XML
```csharp
// tests/MedicSoft.Application.Tests/Services/
- TissXmlGeneratorServiceTests.cs
- TissXmlValidatorTests.cs
```

**Objetivo:** Manter cobertura de testes > 80%

---

## 🎯 Próximos Passos Recomendados

### Passo 1: Criar Migrations (URGENTE)
```bash
cd src/MedicSoft.Repository
dotnet ef migrations add AddTissPhase1Entities --context MedicSoftDbContext --output-dir Migrations/PostgreSQL
dotnet ef database update
```

### Passo 2: Criar DTOs e Mappers
Criar DTOs para todas as entidades TISS e configurar AutoMapper

### Passo 3: Implementar Services
Começar pelos serviços mais críticos:
1. TissXmlGeneratorService (geração de XML TISS)
2. AuthorizationRequestService
3. TissGuideService
4. TissBatchService

### Passo 4: Implementar Controllers
Criar controllers na ordem de dependência

### Passo 5: Implementar Frontend
Começar pelos módulos essenciais:
1. Cadastro de operadoras
2. Cadastro de planos
3. Vínculo paciente-plano

### Passo 6: Testes e Validação
Criar testes unitários e de integração

### Passo 7: Documentação
Documentar APIs e criar guias de uso

---

## 📊 Estimativas de Esforço Restante

| Tarefa | Esforço | Prioridade |
|--------|---------|------------|
| Migrations | 1 dia | ALTA |
| DTOs e Mappers | 2 dias | ALTA |
| Services (XML, Auth, Guide, Batch) | 2 semanas | ALTA |
| Controllers API | 1 semana | ALTA |
| Frontend - Operadoras e Planos | 1 semana | MÉDIA |
| Frontend - Autorizações | 3 dias | MÉDIA |
| Frontend - Guias e Lotes | 1 semana | MÉDIA |
| Testes Unitários | 1 semana | ALTA |
| Testes de Integração | 3 dias | ALTA |
| Documentação | 2 dias | MÉDIA |
| **TOTAL** | **5-6 semanas** | - |

---

## 🔗 Referências

- [HEALTH_INSURANCE_INTEGRATION_GUIDE.md](/docs/HEALTH_INSURANCE_INTEGRATION_GUIDE.md) - Guia completo de integração TISS
- [PLANO_DESENVOLVIMENTO.md](/docs/PLANO_DESENVOLVIMENTO.md) - Plano de desenvolvimento geral
- [Padrão TISS ANS](https://www.ans.gov.br/prestadores/tiss-troca-de-informacao-de-saude-suplementar) - Documentação oficial
- [Tabela TUSS](https://www.ans.gov.br/prestadores/tiss-troca-de-informacao-de-saude-suplementar/padrao-tiss-componente-organizacional) - Terminologia Unificada

---

**Documento criado em:** Janeiro 2026  
**Última atualização:** Janeiro 2026  
**Responsável:** GitHub Copilot - TISS Phase 1 Implementation
