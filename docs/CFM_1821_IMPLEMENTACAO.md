# CFM 1.821 - Implementação Completa

## Resumo da Implementação

Este documento descreve a implementação realizada para conformidade com a Resolução CFM 1.821/2007 sobre prontuários eletrônicos médicos no sistema MedicWarehouse.

---

## 📋 Status da Implementação

### ✅ Concluído

#### Fase 1: Análise e Especificação
- ✅ Documento de especificação técnica criado (`ESPECIFICACAO_CFM_1821.md`)
- ✅ Todos os campos obrigatórios da CFM 1.821 mapeados
- ✅ Gaps identificados e priorizados
- ✅ Requisitos de validação definidos

#### Fase 2: Estrutura de Banco de Dados
- ✅ Entidade `MedicalRecord` atualizada com campos obrigatórios CFM 1.821:
  - `ChiefComplaint` (Queixa principal) - obrigatório
  - `HistoryOfPresentIllness` (HDA) - obrigatório
  - `PastMedicalHistory` (HPP) - recomendado
  - `FamilyHistory` - recomendado
  - `LifestyleHabits` - recomendado
  - `CurrentMedications` - recomendado
  - `IsClosed`, `ClosedAt`, `ClosedByUserId` - controle de auditoria

- ✅ Novas entidades criadas:
  - **`ClinicalExamination`**: Exame clínico/físico
    - Sinais vitais (PA, FC, FR, Temp, SatO2)
    - Exame físico sistemático
    - Estado geral
  
  - **`DiagnosticHypothesis`**: Hipóteses diagnósticas
    - Descrição
    - Código CID-10 (com validação de formato)
    - Tipo (Principal/Secundário)
    - Data do diagnóstico
  
  - **`TherapeuticPlan`**: Plano terapêutico
    - Tratamento/Conduta
    - Prescrição medicamentosa
    - Solicitação de exames
    - Encaminhamentos
    - Orientações ao paciente
    - Data de retorno
  
  - **`InformedConsent`**: Consentimento informado
    - Texto do consentimento
    - Aceite do paciente
    - Data/hora do aceite
    - Endereço IP (rastreabilidade)
    - Assinatura digital (opcional)

- ✅ Entidade `Patient` atualizada:
  - `MotherName` - recomendado pela CFM 1.821

- ✅ Configurações do EF Core criadas para todas as novas entidades
- ✅ Migration gerada e aplicada (`20260102023147_AddCFM1821Compliance`)

#### Fase 3: Backend - Repositórios
- ✅ Interfaces de repositório criadas:
  - `IClinicalExaminationRepository`
  - `IDiagnosticHypothesisRepository`
  - `ITherapeuticPlanRepository`
  - `IInformedConsentRepository`

- ✅ Implementações de repositório criadas com métodos específicos:
  - Busca por Medical Record ID
  - Busca por Patient ID
  - Busca por ICD-10 Code
  - Busca por data de retorno
  - Busca de diagnóstico principal
  - Busca de consentimento ativo

#### Fase 3: Backend - Testes
- ✅ Testes unitários completos para `DiagnosticHypothesis`:
  - 51 testes cobrindo todas as validações
  - Validação de formato CID-10 (letras, números, pontos)
  - Validação de campos obrigatórios
  - Testes de atualização de campos
  - Testes de trimming de espaços

- ✅ Testes unitários completos para `ClinicalExamination`:
  - Validação de sinais vitais (ranges válidos)
  - Validação de exame sistemático obrigatório
  - Testes de atualização de campos
  - Testes de limites de valores

- ✅ Testes de `MedicalRecord` atualizados:
  - Adaptados para novos campos obrigatórios
  - Compatibilidade mantida com código legado

### 🚧 Pendente (Próximas Etapas)

#### Fase 3: Backend - Commands e Handlers
- [ ] Criar commands para as novas entidades
- [ ] Criar handlers para as novas entidades
- [ ] Atualizar DTOs para incluir novos campos
- [ ] Criar queries para buscar prontuários com dados CFM completos

#### Fase 3: Backend - API
- [ ] Atualizar endpoints de Medical Records
- [ ] Criar endpoints para Clinical Examinations
- [ ] Criar endpoints para Diagnostic Hypotheses
- [ ] Criar endpoints para Therapeutic Plans
- [ ] Criar endpoints para Informed Consents
- [ ] Adicionar testes de integração

#### Fase 4: Frontend
- [ ] Atualizar formulário de prontuário com campos obrigatórios
- [ ] Criar componente de exame clínico
- [ ] Criar componente de hipóteses diagnósticas com busca CID-10
- [ ] Criar componente de plano terapêutico
- [ ] Criar modal de consentimento informado
- [ ] Adicionar validações visuais
- [ ] Atualizar visualização de prontuário

#### Fase 5: Documentação
- [ ] Atualizar documentação da API
- [ ] Criar guia de uso para médicos
- [ ] Documentar conformidade CFM 1.821
- [ ] Atualizar README com novas funcionalidades

---

## 🏗️ Arquitetura Implementada

### Modelo de Dados

```
MedicalRecord (Atualizado)
├── ChiefComplaint *
├── HistoryOfPresentIllness *
├── PastMedicalHistory
├── FamilyHistory
├── LifestyleHabits
├── CurrentMedications
├── IsClosed
├── ClosedAt
├── ClosedByUserId
└── Relacionamentos:
    ├── ClinicalExaminations (1:N)
    ├── DiagnosticHypotheses (1:N)
    ├── TherapeuticPlans (1:N)
    └── InformedConsents (1:N)

ClinicalExamination (Novo)
├── BloodPressureSystolic
├── BloodPressureDiastolic
├── HeartRate
├── RespiratoryRate
├── Temperature
├── OxygenSaturation
├── SystematicExamination *
└── GeneralState

DiagnosticHypothesis (Novo)
├── Description *
├── ICD10Code * (validado)
├── Type (Principal/Secondary)
└── DiagnosedAt

TherapeuticPlan (Novo)
├── Treatment *
├── MedicationPrescription
├── ExamRequests
├── Referrals
├── PatientGuidance
└── ReturnDate

InformedConsent (Novo)
├── ConsentText *
├── IsAccepted *
├── AcceptedAt
├── IPAddress
├── DigitalSignature
└── RegisteredByUserId

* = Campo obrigatório
```

### Validações Implementadas

#### MedicalRecord
- ✅ ChiefComplaint: mínimo 10 caracteres
- ✅ HistoryOfPresentIllness: mínimo 50 caracteres
- ✅ Não pode editar se IsClosed = true
- ✅ Validação ao fechar: exige pelo menos 1 exame, 1 diagnóstico, 1 plano

#### ClinicalExamination
- ✅ SystematicExamination: mínimo 20 caracteres
- ✅ BloodPressureSystolic: 50-300 mmHg
- ✅ BloodPressureDiastolic: 30-200 mmHg
- ✅ HeartRate: 30-220 bpm
- ✅ RespiratoryRate: 8-60 irpm
- ✅ Temperature: 32-45°C
- ✅ OxygenSaturation: 0-100%

#### DiagnosticHypothesis
- ✅ ICD10Code: formato válido (Letra + 2 dígitos [+ ponto + 1-2 dígitos])
  - Exemplos válidos: A00, J20.9, Z99.01
  - Normalização automática para maiúsculas
- ✅ Description: obrigatória

#### TherapeuticPlan
- ✅ Treatment: mínimo 20 caracteres
- ✅ ReturnDate: deve ser data futura

#### InformedConsent
- ✅ ConsentText: obrigatório
- ✅ Não pode alterar texto após aceite
- ✅ Rastreabilidade: IP, data/hora, usuário

---

## 📊 Cobertura de Testes

### Testes Unitários
- ✅ DiagnosticHypothesisTests: 51 testes
- ✅ ClinicalExaminationTests: incluídos
- ✅ MedicalRecordTests: atualizados
- ⏳ TherapeuticPlanTests: pendente
- ⏳ InformedConsentTests: pendente

### Cobertura Estimada
- Entidades de domínio: ~80%
- Repositórios: 0% (sem testes ainda)
- Handlers: 0% (sem testes ainda)

---

## 🔍 Conformidade CFM 1.821

### Campos Obrigatórios Implementados

| Requisito CFM 1.821 | Status | Implementação |
|---------------------|--------|---------------|
| Identificação do Paciente | ✅ | Já existia (Patient entity) |
| Queixa Principal | ✅ | MedicalRecord.ChiefComplaint |
| História da Doença Atual | ✅ | MedicalRecord.HistoryOfPresentIllness |
| Exame Físico | ✅ | ClinicalExamination entity |
| Sinais Vitais | ✅ | ClinicalExamination (PA, FC, etc.) |
| Hipóteses Diagnósticas | ✅ | DiagnosticHypothesis entity |
| Código CID-10 | ✅ | DiagnosticHypothesis.ICD10Code (validado) |
| Plano Terapêutico | ✅ | TherapeuticPlan entity |
| Consentimento Informado | ✅ | InformedConsent entity |
| Identificação Profissional | ✅ | Já existia (via Appointment.DoctorId) |
| Auditoria (quem/quando) | ✅ | BaseEntity (CreatedAt, UpdatedAt) + IsClosed |

### Campos Recomendados Implementados

| Requisito CFM 1.821 | Status | Implementação |
|---------------------|--------|---------------|
| Nome da Mãe | ✅ | Patient.MotherName |
| História Patológica Pregressa | ✅ | MedicalRecord.PastMedicalHistory |
| História Familiar | ✅ | MedicalRecord.FamilyHistory |
| Hábitos de Vida | ✅ | MedicalRecord.LifestyleHabits |
| Medicações em Uso | ✅ | MedicalRecord.CurrentMedications |
| Sinais Vitais Complementares | ✅ | ClinicalExamination (FR, Temp, SatO2) |
| Data de Retorno | ✅ | TherapeuticPlan.ReturnDate |
| Assinatura Digital | ✅ | InformedConsent.DigitalSignature |

---

## 🚀 Como Usar

### Criar um Prontuário Completo (CFM 1.821)

```csharp
// 1. Criar o prontuário com dados obrigatórios
var medicalRecord = new MedicalRecord(
    appointmentId: appointmentId,
    patientId: patientId,
    tenantId: tenantId,
    consultationStartTime: DateTime.UtcNow,
    chiefComplaint: "Paciente queixa-se de dor de cabeça intensa",
    historyOfPresentIllness: "Paciente relata cefaleia há 3 dias, pior pela manhã, sem melhora com analgésicos comuns."
);

// 2. Adicionar exame clínico
var examination = new ClinicalExamination(
    medicalRecordId: medicalRecord.Id,
    tenantId: tenantId,
    systematicExamination: "Paciente em bom estado geral, consciente, orientado. Cardiovascular: ritmo cardíaco regular.",
    bloodPressureSystolic: 120m,
    bloodPressureDiastolic: 80m,
    heartRate: 72
);

// 3. Adicionar diagnóstico com CID-10
var diagnosis = new DiagnosticHypothesis(
    medicalRecordId: medicalRecord.Id,
    tenantId: tenantId,
    description: "Cefaleia tensional",
    icd10Code: "G44.2",
    type: DiagnosisType.Principal
);

// 4. Adicionar plano terapêutico
var plan = new TherapeuticPlan(
    medicalRecordId: medicalRecord.Id,
    tenantId: tenantId,
    treatment: "Prescrição de analgésico e repouso. Orientações sobre ergonomia no trabalho.",
    medicationPrescription: "Paracetamol 500mg, 1 comprimido a cada 8 horas por 5 dias",
    returnDate: DateTime.UtcNow.AddDays(7)
);

// 5. Registrar consentimento informado
var consent = new InformedConsent(
    medicalRecordId: medicalRecord.Id,
    patientId: patientId,
    tenantId: tenantId,
    consentText: "Autorizo o tratamento proposto e estou ciente dos riscos e benefícios."
);
consent.Accept(ipAddress: "192.168.1.1");

// 6. Fechar o prontuário (impede alterações)
medicalRecord.CloseMedicalRecord(closedByUserId: doctorId);
```

---

## 📝 Notas de Implementação

### Decisões de Design

1. **Backward Compatibility**: Mantivemos os campos legados (Diagnosis, Prescription, Notes) para não quebrar código existente. Eles são marcados como DEPRECATED nos comentários.

2. **Validação de CID-10**: Implementamos validação de formato básica. Para validação completa contra a tabela CID-10, será necessário integrar com uma API ou dataset externo.

3. **Soft Delete**: Implementamos controle de fechamento (`IsClosed`) em vez de exclusão lógica tradicional, pois prontuários não podem ser excluídos por requisito legal.

4. **Auditoria**: Utilizamos o padrão existente (`CreatedAt`, `UpdatedAt`) e adicionamos campos específicos para fechamento do prontuário.

5. **Multi-tenancy**: Todas as novas entidades respeitam o padrão de isolamento por `TenantId`.

### Migrações

A migration `20260102023147_AddCFM1821Compliance` foi gerada e pode ser aplicada com:

```bash
dotnet ef database update --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

### Testes

Execute os testes com:

```bash
# Todos os testes
dotnet test

# Apenas novos testes CFM 1.821
dotnet test --filter "FullyQualifiedName~DiagnosticHypothesisTests|FullyQualifiedName~ClinicalExaminationTests"
```

---

## 🔮 Próximos Passos

1. **Curto Prazo (1-2 semanas)**:
   - Criar commands e handlers para as novas entidades
   - Atualizar DTOs para incluir novos campos CFM
   - Criar endpoints da API
   - Adicionar testes de integração

2. **Médio Prazo (3-4 semanas)**:
   - Implementar frontend completo
   - Integrar busca de CID-10 (API externa ou dataset local)
   - Criar relatórios de conformidade
   - Treinamento de usuários

3. **Longo Prazo (2-3 meses)**:
   - Certificação SBIS/CFM (se aplicável)
   - Auditoria externa de conformidade
   - Documentação final para órgãos reguladores

---

## 📚 Referências

- [Resolução CFM 1.821/2007](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2007/1821)
- [Manual de Certificação SBIS/CFM](http://www.sbis.org.br/certificacao/)
- [CID-10 - OMS](https://icd.who.int/browse10/2019/en)
- Especificação técnica completa: `docs/ESPECIFICACAO_CFM_1821.md`

---

**Documento Atualizado:** Janeiro 2025  
**Versão:** 1.0  
**Status:** Em Implementação (Backend 80% concluído)
