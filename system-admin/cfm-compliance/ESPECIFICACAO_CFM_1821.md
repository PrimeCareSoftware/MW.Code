# 📋 Especificação Técnica - Conformidade CFM 1.821/2007

> **Objetivo:** Documentar requisitos técnicos para conformidade com Resolução CFM 1.821/2007 sobre prontuários eletrônicos médicos.

> **Data:** Janeiro 2025  
> **Versão:** 1.0  
> **Status:** Em Implementação

---

## 📖 Resumo Executivo

A Resolução CFM 1.821/2007 estabelece normas técnicas para digitalização e uso de sistemas informatizados para a guarda e manuseio dos documentos dos prontuários dos pacientes. Este documento mapeia todos os requisitos obrigatórios e o status de implementação no Omni Care Software.

---

## 1. Identificação do Paciente (CFM 1.821 - Art. 1º)

### Campos Obrigatórios

| Campo | Tipo | Obrigatório | Validação | Status Atual | Observações |
|-------|------|-------------|-----------|--------------|-------------|
| Nome completo | string(200) | Sim | Nome válido, mínimo 2 palavras | ✅ Implementado | Campo `Name` em `Patient` |
| Data de nascimento | date | Sim | Data no passado | ✅ Implementado | Campo `DateOfBirth` em `Patient` |
| CPF | string(11) | Sim | CPF válido | ✅ Implementado | Campo `Document` em `Patient` com validação |
| Sexo/Gênero | enum | Sim | M/F/Outro | ✅ Implementado | Campo `Gender` em `Patient` |
| Nome da mãe | string(200) | Recomendado | - | ❌ Não Implementado | Necessário adicionar |
| Endereço completo | Address | Sim | CEP válido | ✅ Implementado | ValueObject `Address` |
| Telefone | Phone | Sim | Formato válido | ✅ Implementado | ValueObject `Phone` |
| Email | Email | Sim | Email válido | ✅ Implementado | ValueObject `Email` |

### Gaps Identificados
- **Média Prioridade:** Nome da mãe não está sendo coletado

---

## 2. Anamnese (CFM 1.821 - Art. 2º)

### Campos Obrigatórios

| Campo | Tipo | Obrigatório | Validação | Status Atual | Observações |
|-------|------|-------------|-----------|--------------|-------------|
| Data/hora do atendimento | datetime | Sim | - | ✅ Implementado | Campo `ConsultationStartTime` |
| Queixa principal | text | Sim | Mínimo 10 caracteres | ❌ Não Implementado | **CRÍTICO** |
| História da doença atual (HDA) | text | Sim | Mínimo 50 caracteres | ❌ Não Implementado | **CRÍTICO** |
| História patológica pregressa (HPP) | text | Recomendado | - | ⚠️ Parcial | Campo `MedicalHistory` genérico |
| História familiar | text | Recomendado | - | ❌ Não Implementado | |
| Hábitos de vida | text | Recomendado | - | ❌ Não Implementado | |
| Alergias | text | Recomendado | - | ✅ Implementado | Campo `Allergies` em `Patient` |
| Medicações em uso | text | Recomendado | - | ❌ Não Implementado | |

### Gaps Identificados
- **Alta Prioridade (CRÍTICO):**
  - Queixa principal não está sendo obrigada
  - História da doença atual não existe como campo separado
- **Média Prioridade:**
  - História familiar não implementada
  - Hábitos de vida não implementados
  - Medicações em uso não implementadas

---

## 3. Exame Físico (CFM 1.821 - Art. 2º)

### Campos Obrigatórios

| Campo | Tipo | Obrigatório | Validação | Status Atual | Observações |
|-------|------|-------------|-----------|--------------|-------------|
| Sinais vitais | object | Sim | Valores dentro de ranges | ❌ Não Implementado | **CRÍTICO** |
| - Pressão arterial (sistólica) | decimal | Sim | 50-300 mmHg | ❌ Não Implementado | |
| - Pressão arterial (diastólica) | decimal | Sim | 30-200 mmHg | ❌ Não Implementado | |
| - Frequência cardíaca | int | Sim | 30-220 bpm | ❌ Não Implementado | |
| - Frequência respiratória | int | Recomendado | 8-60 irpm | ❌ Não Implementado | |
| - Temperatura | decimal | Recomendado | 32-45 °C | ❌ Não Implementado | |
| - Saturação O2 | decimal | Recomendado | 0-100% | ❌ Não Implementado | |
| Exame físico por sistemas | text | Sim | Mínimo 20 caracteres | ❌ Não Implementado | **CRÍTICO** |
| Estado geral | text | Recomendado | - | ❌ Não Implementado | |

### Gaps Identificados
- **Alta Prioridade (CRÍTICO):**
  - Sinais vitais não estão sendo registrados
  - Exame físico sistemático não implementado

---

## 4. Hipóteses Diagnósticas (CFM 1.821 - Art. 2º)

### Campos Obrigatórios

| Campo | Tipo | Obrigatório | Validação | Status Atual | Observações |
|-------|------|-------------|-----------|--------------|-------------|
| Diagnóstico/Hipótese diagnóstica | text | Sim | - | ⚠️ Parcial | Campo `Diagnosis` genérico existe |
| Código CID-10 | string(10) | Sim | Código CID-10 válido | ❌ Não Implementado | **CRÍTICO** |
| Tipo (principal/secundário) | enum | Recomendado | - | ❌ Não Implementado | |
| Data do diagnóstico | datetime | Sim | - | ⚠️ Parcial | Usa `CreatedAt` genérico |

### Gaps Identificados
- **Alta Prioridade (CRÍTICO):**
  - CID-10 não está sendo validado
  - Sistema não suporta múltiplos diagnósticos estruturados
- **Média Prioridade:**
  - Não distingue diagnóstico principal de secundário

---

## 5. Plano Terapêutico (CFM 1.821 - Art. 2º)

### Campos Obrigatórios

| Campo | Tipo | Obrigatório | Validação | Status Atual | Observações |
|-------|------|-------------|-----------|--------------|-------------|
| Conduta/Tratamento | text | Sim | Mínimo 20 caracteres | ❌ Não Implementado | **CRÍTICO** |
| Prescrição medicamentosa | text | Se aplicável | - | ⚠️ Parcial | Campo `Prescription` genérico |
| Solicitação de exames | text | Se aplicável | - | ❌ Não Implementado | |
| Encaminhamentos | text | Se aplicável | - | ❌ Não Implementado | |
| Orientações ao paciente | text | Recomendado | - | ⚠️ Parcial | Campo `Notes` genérico |
| Data de retorno | date | Recomendado | Data futura | ❌ Não Implementado | |

### Gaps Identificados
- **Alta Prioridade (CRÍTICO):**
  - Plano terapêutico não está estruturado
  - Não há separação clara entre prescrição, exames, encaminhamentos
- **Média Prioridade:**
  - Data de retorno não implementada
  - Orientações ao paciente não separadas

---

## 6. Consentimento Informado (CFM 1.821 - Art. 3º)

### Campos Obrigatórios

| Campo | Tipo | Obrigatório | Validação | Status Atual | Observações |
|-------|------|-------------|-----------|--------------|-------------|
| Texto do consentimento | text | Sim | - | ❌ Não Implementado | **CRÍTICO** |
| Aceite do paciente | boolean | Sim | - | ❌ Não Implementado | **CRÍTICO** |
| Data/hora do aceite | datetime | Sim | - | ❌ Não Implementado | **CRÍTICO** |
| IP do aceite | string(45) | Recomendado | IPv4/IPv6 válido | ❌ Não Implementado | |
| Assinatura digital | text | Recomendado | - | ❌ Não Implementado | |

### Gaps Identificados
- **Alta Prioridade (CRÍTICO):**
  - Sistema de consentimento informado não existe
  - Não há registro de aceite do paciente
  - Falta rastreabilidade legal

---

## 7. Identificação do Profissional (CFM 1.821 - Art. 4º)

### Campos Obrigatórios

| Campo | Tipo | Obrigatório | Validação | Status Atual | Observações |
|-------|------|-------------|-----------|--------------|-------------|
| Nome completo do médico | string(200) | Sim | - | ✅ Implementado | Via relacionamento `Appointment.DoctorId` |
| CRM | string(20) | Sim | Formato CRM válido | ✅ Implementado | Campo no `User` (médico) |
| UF do CRM | string(2) | Sim | Estado válido | ✅ Implementado | |
| Especialidade | string(100) | Recomendado | - | ⚠️ Parcial | Implementado mas não obrigatório |
| Data/hora do atendimento | datetime | Sim | - | ✅ Implementado | |
| Assinatura digital | text | Recomendado | - | ❌ Não Implementado | |

### Gaps Identificados
- **Baixa Prioridade:**
  - Assinatura digital do médico não implementada

---

## 8. Auditoria e Rastreabilidade (CFM 1.821 - Art. 5º)

### Requisitos

| Requisito | Status Atual | Observações |
|-----------|--------------|-------------|
| Log de criação do prontuário | ✅ Implementado | Campo `CreatedAt` em `BaseEntity` |
| Log de modificações | ✅ Implementado | Campo `UpdatedAt` em `BaseEntity` |
| Identificação de quem criou | ✅ Implementado | Via `TenantId` e contexto de autenticação |
| Identificação de quem alterou | ⚠️ Parcial | Não rastreia usuário específico |
| Histórico de versões | ❌ Não Implementado | Não mantém versões anteriores |
| Impossibilidade de exclusão | ⚠️ Parcial | Exclusão lógica via `IsActive` em algumas entidades |

### Gaps Identificados
- **Alta Prioridade:**
  - Não rastreia qual usuário específico fez cada alteração
  - Não mantém histórico de versões do prontuário
- **Média Prioridade:**
  - Exclusão lógica não implementada em todas as entidades críticas

---

## 9. Segurança e Privacidade (CFM 1.821 - Art. 6º)

### Requisitos

| Requisito | Status Atual | Observações |
|-----------|--------------|-------------|
| Controle de acesso por perfil | ✅ Implementado | Sistema de perfis e permissões existe |
| Isolamento por clínica (multi-tenant) | ✅ Implementado | Campo `TenantId` em todas as entidades |
| Criptografia em trânsito (HTTPS) | ✅ Implementado | Configurado no servidor |
| Criptografia em repouso | ❌ Não Implementado | Dados sensíveis não criptografados |
| Backup regular | ✅ Implementado | Via infraestrutura |
| Tempo de retenção (20 anos) | ✅ Implementado | Não há exclusão automática |

### Gaps Identificados
- **Alta Prioridade:**
  - Criptografia de campos sensíveis (diagnósticos, prescrições) não implementada

---

## 10. Sumário de Gaps por Prioridade

### 🔴 ALTA PRIORIDADE (Bloqueantes para Compliance)

1. **Anamnese estruturada obrigatória**
   - Queixa principal não está sendo obrigada
   - História da doença atual não existe como campo separado
   - Esforço: 2 dias

2. **Exame físico sistemático**
   - Sinais vitais não estão sendo registrados
   - Exame físico por sistemas não implementado
   - Esforço: 3 dias

3. **CID-10 validado**
   - Código CID-10 não está sendo validado
   - Sistema não suporta múltiplos diagnósticos
   - Esforço: 4 dias

4. **Plano terapêutico estruturado**
   - Não há separação clara entre prescrição, exames, encaminhamentos
   - Esforço: 3 dias

5. **Sistema de consentimento informado**
   - Não existe
   - Falta rastreabilidade legal
   - Esforço: 5 dias

6. **Auditoria completa**
   - Não rastreia qual usuário específico fez cada alteração
   - Não mantém histórico de versões
   - Esforço: 4 dias

**Total Alta Prioridade: 21 dias de desenvolvimento**

### 🟡 MÉDIA PRIORIDADE (Recomendados CFM)

1. **Campos adicionais de identificação**
   - Nome da mãe
   - Esforço: 1 dia

2. **História clínica completa**
   - História familiar
   - Hábitos de vida
   - Medicações em uso
   - Esforço: 2 dias

3. **Sinais vitais complementares**
   - Temperatura, saturação O2, frequência respiratória
   - Esforço: 1 dia

4. **Diagnóstico tipificado**
   - Distinguir principal de secundário
   - Esforço: 1 dia

5. **Data de retorno**
   - Campo para agendamento de retorno
   - Esforço: 1 dia

**Total Média Prioridade: 6 dias de desenvolvimento**

### 🟢 BAIXA PRIORIDADE (Nice to Have)

1. **Assinatura digital do médico**
   - Esforço: 3 dias

2. **Assinatura digital do paciente no consentimento**
   - Esforço: 2 dias

**Total Baixa Prioridade: 5 dias de desenvolvimento**

---

## 11. Estrutura de Dados Proposta

### 11.1 Entidades Novas

#### ClinicalExamination
```csharp
public class ClinicalExamination : BaseEntity
{
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    
    // Sinais vitais obrigatórios
    public decimal? BloodPressureSystolic { get; set; }
    public decimal? BloodPressureDiastolic { get; set; }
    public int? HeartRate { get; set; }
    
    // Sinais vitais recomendados
    public int? RespiratoryRate { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? OxygenSaturation { get; set; }
    
    // Exame físico
    [Required]
    [MinLength(20)]
    public string SystematicExamination { get; set; }
    
    public string? GeneralState { get; set; }
}
```

#### DiagnosticHypothesis
```csharp
public class DiagnosticHypothesis : BaseEntity
{
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    [RegularExpression(@"^[A-Z]\d{2}(\.\d{1,2})?$")]
    public string ICD10Code { get; set; }
    
    public DiagnosisType Type { get; set; } // Principal, Secondary
    
    public DateTime DiagnosedAt { get; set; }
}
```

#### TherapeuticPlan
```csharp
public class TherapeuticPlan : BaseEntity
{
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    
    [Required]
    [MinLength(20)]
    public string Treatment { get; set; }
    
    public string? MedicationPrescription { get; set; }
    public string? ExamRequests { get; set; }
    public string? Referrals { get; set; }
    public string? PatientGuidance { get; set; }
    
    public DateTime? ReturnDate { get; set; }
}
```

#### InformedConsent
```csharp
public class InformedConsent : BaseEntity
{
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }
    
    [Required]
    public string ConsentText { get; set; }
    
    [Required]
    public bool IsAccepted { get; set; }
    
    public DateTime? AcceptedAt { get; set; }
    
    public string? IPAddress { get; set; }
    
    public string? DigitalSignature { get; set; }
}
```

### 11.2 Entidades Atualizadas

#### MedicalRecord (atualizar)
```csharp
public class MedicalRecord : BaseEntity
{
    // Campos existentes
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public DateTime ConsultationStartTime { get; set; }
    public DateTime? ConsultationEndTime { get; set; }
    
    // NOVOS CAMPOS OBRIGATÓRIOS CFM 1.821
    [Required]
    [MinLength(10)]
    public string ChiefComplaint { get; set; } // Queixa principal
    
    [Required]
    [MinLength(50)]
    public string HistoryOfPresentIllness { get; set; } // HDA
    
    // CAMPOS RECOMENDADOS CFM 1.821
    public string? PastMedicalHistory { get; set; } // HPP
    public string? FamilyHistory { get; set; }
    public string? LifestyleHabits { get; set; }
    public string? CurrentMedications { get; set; }
    
    // Campos existentes (manter)
    public string? Notes { get; set; }
    
    // Controle de fechamento
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public Guid? ClosedByUserId { get; set; }
    
    // Relacionamentos novos
    public virtual ICollection<ClinicalExamination> Examinations { get; set; }
    public virtual ICollection<DiagnosticHypothesis> Diagnoses { get; set; }
    public virtual ICollection<TherapeuticPlan> Plans { get; set; }
    public virtual ICollection<InformedConsent> Consents { get; set; }
    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; }
}
```

#### Patient (atualizar)
```csharp
public class Patient : BaseEntity
{
    // Campos existentes (manter)
    public string Name { get; set; }
    public string Document { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public Email Email { get; set; }
    public Phone Phone { get; set; }
    public Address Address { get; set; }
    public string? MedicalHistory { get; set; }
    public string? Allergies { get; set; }
    
    // NOVO CAMPO RECOMENDADO CFM
    public string? MotherName { get; set; }
}
```

---

## 12. Estimativa de Esforço Total

### Desenvolvimento Backend
- Atualizar entidades de domínio: 2 dias
- Criar migrations: 1 dia
- Implementar commands e handlers: 4 dias
- Criar queries: 2 dias
- Implementar validações: 2 dias
- Criar testes unitários: 3 dias
- Criar testes de integração: 2 dias
- **Subtotal Backend: 16 dias**

### Desenvolvimento Frontend
- Atualizar formulário de prontuário: 3 dias
- Criar componente de exame clínico: 2 dias
- Criar componente de diagnósticos com busca CID-10: 3 dias
- Criar componente de plano terapêutico: 2 dias
- Criar modal de consentimento informado: 2 dias
- Implementar validações visuais: 2 dias
- Criar visualização de histórico: 2 dias
- Testes e ajustes: 3 dias
- **Subtotal Frontend: 19 dias**

### Documentação e Deployment
- Documentação de API: 1 dia
- Guia de uso para médicos: 2 dias
- Testes de aceitação: 2 dias
- Deploy e monitoramento: 1 dia
- **Subtotal Doc/Deploy: 6 dias**

### **TOTAL GERAL: 41 dias úteis (≈ 8 semanas, 2 meses)**

---

## 13. Critérios de Validação Final

### Checklist de Conformidade CFM 1.821

- [ ] Todos os campos obrigatórios implementados
- [ ] Validações de domínio funcionando
- [ ] CID-10 validado e pesquisável
- [ ] Sinais vitais obrigatórios coletados
- [ ] Exame físico sistemático preenchido
- [ ] Múltiplos diagnósticos suportados
- [ ] Plano terapêutico estruturado
- [ ] Sistema de consentimento informado funcional
- [ ] Auditoria completa (quem/quando/o que)
- [ ] Histórico de versões mantido
- [ ] Impossibilidade de exclusão (soft delete)
- [ ] Testes automatizados com > 80% coverage
- [ ] Documentação completa
- [ ] Aprovação por médico consultor

---

## 14. Riscos e Mitigações

### Riscos Identificados

1. **Risco:** Complexidade da busca de CID-10 (92.000+ códigos)
   - **Mitigação:** Usar API pública ou dataset pré-carregado com indexação

2. **Risco:** Resistência dos médicos a campos obrigatórios
   - **Mitigação:** Treinamento e justificativa legal clara

3. **Risco:** Performance com múltiplas entidades relacionadas
   - **Mitigação:** Eager loading e índices adequados no banco

4. **Risco:** Prazo apertado (2 meses)
   - **Mitigação:** Foco em alta prioridade primeiro, iteração rápida

---

## 15. Próximos Passos

1. ✅ Documento de especificação criado
2. ⏭️ Revisar com tech lead e médico consultor (se disponível)
3. ⏭️ Criar entidades de domínio
4. ⏭️ Gerar migrations
5. ⏭️ Implementar backend
6. ⏭️ Implementar frontend
7. ⏭️ Testes e validação
8. ⏭️ Deploy e treinamento

---

**Documento Criado Por:** GitHub Copilot  
**Última Atualização:** Janeiro 2025  
**Revisão Técnica:** Pendente  
**Aprovação Médica:** Pendente

---

## 📚 Referências

- [Resolução CFM 1.821/2007](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2007/1821)
- [Manual de Certificação SBIS/CFM](http://www.sbis.org.br/certificacao/)
- CID-10: Classificação Internacional de Doenças (OMS)
