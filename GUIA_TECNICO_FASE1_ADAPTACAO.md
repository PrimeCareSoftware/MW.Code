# Guia Técnico - Fase 1: Sistema de Configuração de Negócio

## Visão Geral

Este guia documenta a implementação da Fase 1 do refatoramento do sistema Omni Care para adaptação a diferentes tipos profissionais de clínicas e empresas.

## Componentes Implementados

### 1. BusinessConfiguration Entity

Entidade que armazena a configuração de negócio e feature flags para cada clínica.

**Localização:** `src/MedicSoft.Domain/Entities/BusinessConfiguration.cs`

**Propriedades:**
- `BusinessType`: Tipo de negócio (Solo, Small, Medium, Large Clinic)
- `PrimarySpecialty`: Especialidade principal do profissional
- 17 Feature Flags booleanos organizados por categoria:
  - **Clínicos**: ElectronicPrescription, LabIntegration, VaccineControl, InventoryManagement
  - **Administrativos**: MultiRoom, ReceptionQueue, FinancialModule, HealthInsurance
  - **Consultas**: Telemedicine, HomeVisit, GroupSessions
  - **Marketing**: PublicProfile, OnlineBooking, PatientReviews
  - **Avançados**: BiReports, ApiAccess, WhiteLabel

**Configuração Automática:**
O sistema configura automaticamente os feature flags baseado em:
1. Tipo de negócio (SoloPractitioner, SmallClinic, etc.)
2. Especialidade profissional (Psicólogo, Nutricionista, etc.)

**Exemplo de uso:**
```csharp
// Criar uma configuração para um psicólogo autônomo
var config = new BusinessConfiguration(
    clinicId: Guid.Parse("..."),
    businessType: BusinessType.SoloPractitioner,
    primarySpecialty: ProfessionalSpecialty.Psicologo,
    tenantId: "tenant-123"
);

// Features são configuradas automaticamente:
// - Telemedicine: true
// - GroupSessions: true
// - MultiRoom: false
// - InventoryManagement: false
```

### 2. BusinessType Enum

Define os tipos de negócio disponíveis:

**Localização:** `src/MedicSoft.Domain/Enums/BusinessType.cs`

**Valores:**
- `SoloPractitioner` (1): Profissional solo, pode não ter consultório físico
- `SmallClinic` (2): Clínica pequena (2-5 profissionais)
- `MediumClinic` (3): Clínica média (6-20 profissionais)
- `LargeClinic` (4): Clínica grande (20+ profissionais)

### 3. TerminologyMap Value Object

Mapeia terminologia específica por especialidade profissional.

**Localização:** `src/MedicSoft.Domain/ValueObjects/TerminologyMap.cs`

**Termos mapeados:**
- appointment: "Consulta" ou "Sessão"
- professional: "Médico", "Psicólogo", "Nutricionista", etc.
- registration: "CRM", "CRP", "CRN", "CRO", "CREFITO", etc.
- client: "Paciente" ou "Cliente"
- mainDocument: "Prontuário", "Odontograma", "Avaliação", etc.
- exitDocument: "Receita", "Relatório", "Plano Alimentar", etc.

**Exemplo:**
```csharp
// Obter terminologia para psicólogo
var terminology = TerminologyMap.For(ProfessionalSpecialty.Psicologo);
// Resultado:
// - appointment: "Sessão"
// - professional: "Psicólogo"
// - registration: "CRP"
// - mainDocument: "Prontuário"
// - exitDocument: "Relatório Psicológico"
```

### 4. DocumentTemplate Entity

Gerencia templates de documentos por especialidade.

**Localização:** `src/MedicSoft.Domain/Entities/DocumentTemplate.cs`

**Tipos de Templates (DocumentTemplateType):**
- MedicalRecord, Prescription, MedicalCertificate, LabTestRequest
- PsychologicalReport, NutritionPlan, DentalBudget, Odontogram
- PhysicalTherapyEvaluation, TreatmentPlan, SessionEvolution
- DischargeReport, Referral, InformedConsent, Custom

**Recursos:**
- Templates do sistema (não podem ser excluídos)
- Templates personalizados por clínica
- Suporte a variáveis dinâmicas (JSON)
- Controle de ativação/desativação

### 5. API Endpoints

**Controller:** `BusinessConfigurationController`

**Endpoints disponíveis:**

#### GET /api/businessconfiguration/clinic/{clinicId}
Obtém a configuração de negócio para uma clínica.

**Resposta:**
```json
{
  "id": "guid",
  "clinicId": "guid",
  "businessType": 1,
  "primarySpecialty": 2,
  "electronicPrescription": false,
  "telemedicine": true,
  "groupSessions": true,
  ...
}
```

#### POST /api/businessconfiguration
Cria uma nova configuração de negócio.

**Body:**
```json
{
  "clinicId": "guid",
  "businessType": 1,
  "primarySpecialty": 2
}
```

#### PUT /api/businessconfiguration/{id}/business-type
Atualiza o tipo de negócio.

**Body:**
```json
{
  "businessType": 2
}
```

#### PUT /api/businessconfiguration/{id}/primary-specialty
Atualiza a especialidade principal.

**Body:**
```json
{
  "primarySpecialty": 3
}
```

#### PUT /api/businessconfiguration/{id}/feature
Atualiza um feature flag específico.

**Body:**
```json
{
  "featureName": "Telemedicine",
  "enabled": true
}
```

#### GET /api/businessconfiguration/clinic/{clinicId}/feature/{featureName}
Verifica se um feature está habilitado.

**Resposta:**
```json
{
  "featureName": "Telemedicine",
  "enabled": true
}
```

#### GET /api/businessconfiguration/clinic/{clinicId}/terminology
Obtém o mapa de terminologia para a clínica.

**Resposta:**
```json
{
  "appointment": "Sessão",
  "professional": "Psicólogo",
  "registration": "CRP",
  "client": "Paciente",
  "mainDocument": "Prontuário",
  "exitDocument": "Relatório Psicológico"
}
```

## Migrações de Banco de Dados

Duas migrações foram criadas:

### 1. AddBusinessConfigurationTable
**Arquivo:** `20260202124700_AddBusinessConfigurationTable.cs`

Cria a tabela `BusinessConfigurations` com:
- 17 feature flags booleanos
- Relacionamento com Clinics
- Índices otimizados para queries

### 2. AddDocumentTemplateTable
**Arquivo:** `20260202125900_AddDocumentTemplateTable.cs`

Cria a tabela `DocumentTemplates` com:
- Suporte a templates do sistema e personalizados
- Relacionamento opcional com Clinics
- Índices por especialidade, tipo e tenant

**Para aplicar as migrações:**
```bash
cd src/MedicSoft.Repository
dotnet ef database update --context MedicSoftDbContext
```

## Configurações Inteligentes por Perfil

### Psicólogo Autônomo (Solo + Psicologo)
- ✅ Telemedicine, GroupSessions, PublicProfile, OnlineBooking, PatientReviews
- ❌ ElectronicPrescription, LabIntegration, MultiRoom, HealthInsurance

### Nutricionista (Solo + Nutricionista)
- ✅ LabIntegration, Telemedicine, HomeVisit, HealthInsurance
- ❌ ElectronicPrescription, MultiRoom

### Clínica Odontológica Pequena (SmallClinic + Dentista)
- ✅ ElectronicPrescription, LabIntegration, MultiRoom, ReceptionQueue, HealthInsurance
- ❌ BiReports, ApiAccess, WhiteLabel

### Clínica Médica Grande (LargeClinic + Medico)
- ✅ Todos os recursos incluindo BiReports, ApiAccess, WhiteLabel

## Próximos Passos

### Fase 2: Frontend Integration
- [ ] Criar componente Angular para configuração de negócio
- [ ] Implementar UI para gerenciamento de feature flags
- [ ] Integrar terminologia dinâmica nos componentes
- [ ] Criar editor de templates de documentos

### Fase 3: Onboarding Diferenciado
- [ ] Wizard de onboarding por perfil profissional
- [ ] Configuração automática no primeiro acesso
- [ ] Tutoriais específicos por especialidade

### Fase 4: Expansão de Templates
- [ ] Criar templates padrão para todas especialidades
- [ ] Editor visual de templates
- [ ] Sistema de preview de documentos

## Documentos Relacionados

- [PLANO_ADAPTACAO_MULTI_NEGOCIOS.md](../../Plano_Desenvolvimento/PLANO_ADAPTACAO_MULTI_NEGOCIOS.md) - Plano estratégico completo
- [INDEX_ADAPTACAO_MULTI_NEGOCIOS.md](../../Plano_Desenvolvimento/INDEX_ADAPTACAO_MULTI_NEGOCIOS.md) - Índice da documentação
- [ANALISE_MERCADO_SAAS_SAUDE.md](../../Plano_Desenvolvimento/ANALISE_MERCADO_SAAS_SAUDE.md) - Análise de mercado

## Contato

- **GitHub Issues:** Para bugs e melhorias
- **Email:** produto@omnicare.com.br
- **Documentação:** `/Plano_Desenvolvimento/`

---

**Versão:** 1.0  
**Data:** 02 de Fevereiro de 2026  
**Status:** ✅ Fase 1 Completa - Backend Implementado
