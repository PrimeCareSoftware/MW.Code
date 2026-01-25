# Fluxo de Atendimento Médico

## Visão Geral

Este documento descreve o fluxo otimizado de atendimento médico implementado no sistema, seguindo as melhores práticas para clínicas médicas e sistemas SaaS de saúde, em conformidade com o CFM 1.821/2007.

## 1. Campos Essenciais da Consulta

### 1.1 Identificação do Atendimento

**Entidade**: `Appointment`

- `PatientId` (Guid) - ID do paciente
- `ClinicId` (Guid) - ID da clínica/unidade
- `ProfessionalId` (Guid?) - ID do médico/profissional
- `ScheduledDate` (DateTime) - Data do atendimento
- `ScheduledTime` (TimeSpan) - Hora do atendimento
- `Mode` (AppointmentMode) - Tipo de atendimento:
  - `InPerson` - Presencial
  - `Telemedicine` - Telemedicina
- `PaymentType` (PaymentType) - Forma de pagamento:
  - `Private` - Particular
  - `HealthInsurance` - Convênio
- `HealthInsurancePlanId` (Guid?) - ID do plano de saúde (obrigatório se PaymentType = HealthInsurance)
- `Status` (AppointmentStatus) - Status do atendimento:
  - `Scheduled` - Aguardando (agendado)
  - `Confirmed` - Confirmado
  - `InProgress` - Em atendimento
  - `Completed` - Finalizado
  - `Cancelled` - Cancelado
  - `NoShow` - Faltou
- `IsPaid` (bool) - Se o pagamento foi recebido
- `PaidAt` (DateTime?) - Data/hora do recebimento do pagamento
- `PaidByUserId` (Guid?) - ID do usuário que recebeu o pagamento
- `PaymentReceivedBy` (PaymentReceiverType?) - Tipo de recebedor:
  - `Doctor` - Médico
  - `Secretary` - Secretária/Recepção
  - `Other` - Outro funcionário

### 1.2 Dados Clínicos

**Entidade**: `MedicalRecord`

#### Campos Obrigatórios (CFM 1.821)
- `ChiefComplaint` (string) - Queixa principal (mínimo 10 caracteres)
- `HistoryOfPresentIllness` (string) - História da Doença Atual - HDA (mínimo 50 caracteres)

#### Campos Recomendados
- `PastMedicalHistory` (string) - História patológica pregressa
- `FamilyHistory` (string) - História familiar
- `LifestyleHabits` (string) - Hábitos de vida
- `CurrentMedications` (string) - Medicamentos de uso contínuo

**Entidade**: `ClinicalExamination`

#### Sinais Vitais
- `BloodPressureSystolic` (decimal?) - Pressão arterial sistólica (mmHg)
- `BloodPressureDiastolic` (decimal?) - Pressão arterial diastólica (mmHg)
- `HeartRate` (int?) - Frequência cardíaca (bpm)
- `RespiratoryRate` (int?) - Frequência respiratória (rpm)
- `Temperature` (decimal?) - Temperatura (°C)
- `OxygenSaturation` (decimal?) - Saturação de oxigênio (%)

#### Medidas Antropométricas
- `Weight` (decimal?) - Peso (kg)
- `Height` (decimal?) - Altura (metros)
- `BMI` (decimal?) - IMC (calculado automaticamente: peso / altura²)

#### Exame Físico
- `SystematicExamination` (string) - Exame físico sistemático (obrigatório, mínimo 20 caracteres)
- `GeneralState` (string?) - Estado geral

### 1.3 Diagnóstico

**Entidade**: `DiagnosticHypothesis`

- `Description` (string) - Descrição da hipótese diagnóstica
- `ICD10Code` (string) - Código CID-10 (obrigatório, validado)
- `Type` (DiagnosisType) - Tipo do diagnóstico:
  - `Principal` - Diagnóstico principal
  - `Secondary` - Diagnóstico secundário
- `DiagnosedAt` (DateTime) - Data do diagnóstico

### 1.4 Plano Terapêutico

**Entidade**: `TherapeuticPlan`

- `Treatment` (string) - Conduta/Tratamento (obrigatório, mínimo 20 caracteres)
- `MedicationPrescription` (string?) - Prescrição medicamentosa
- `ExamRequests` (string?) - Solicitação de exames
- `Referrals` (string?) - Encaminhamentos
- `PatientGuidance` (string?) - Orientações ao paciente
- `ReturnDate` (DateTime?) - Data de retorno sugerida

### 1.5 Prescrição

**Entidade**: `PrescriptionItem`

- `MedicationId` (Guid) - ID do medicamento
- `Dosage` (string) - Dosagem
- `Frequency` (string) - Frequência
- `DurationDays` (int) - Duração em dias
- `Quantity` (int) - Quantidade
- `Instructions` (string?) - Observações

### 1.6 Finalização

**Entidade**: `MedicalRecord`

- `IsClosed` (bool) - Prontuário fechado (impede alterações)
- `ClosedAt` (DateTime?) - Data/hora de fechamento
- `ClosedByUserId` (Guid?) - ID do usuário que fechou
- `ProfessionalSignature` (string?) - Assinatura digital do profissional

## 2. Fluxo Otimizado do Atendimento

### 2.1 Chegada do Paciente
1. Check-in na recepção ou autoatendimento
2. Status do `Appointment` atualizado para `Confirmed` ou permanece `Scheduled`

### 2.2 Triagem (Opcional)
1. Realizada por enfermagem
2. Coleta de sinais vitais (`ClinicalExamination`)
3. Registro da queixa inicial

### 2.3 Médico Inicia Atendimento
1. Médico chama o paciente
2. Método `CheckIn()` altera status para `InProgress`
3. Sistema carrega automaticamente:
   - Histórico do paciente
   - Últimas consultas
   - Medicamentos ativos
   - Alergias (destacadas)

### 2.4 Registro da Consulta
Fluxo recomendado na interface:
1. Queixa Principal (`ChiefComplaint`)
2. História da Doença Atual (`HistoryOfPresentIllness`)
3. Sinais Vitais e Exame Físico (`ClinicalExamination`)
4. Diagnóstico (`DiagnosticHypothesis`)
5. Plano Terapêutico (`TherapeuticPlan`)
6. Prescrição (`PrescriptionItem`)

### 2.5 Finalização
1. Médico finaliza o atendimento através do botão "Finalizar Atendimento"
2. Método `CloseMedicalRecord(userId, signature)` é chamado
3. Sistema valida campos obrigatórios:
   - Queixa principal
   - História da doença atual
   - Pelo menos um exame clínico
   - Pelo menos um diagnóstico
   - Pelo menos um plano terapêutico
4. Status do prontuário alterado para `IsClosed = true`
5. Método `CheckOut()` do `Appointment` altera status para `Completed`
6. **Opcional**: Médico pode registrar que recebeu o pagamento neste momento

### 2.6 Controle de Pagamento
1. **Antes do Atendimento**: Secretária pode registrar pagamento recebido
2. **Durante/Após Atendimento**: Médico pode registrar pagamento ao finalizar consulta
3. **Status Visível**: Indicador de pagamento (Pago/Pendente) exibido na tela de atendimento
4. **Configuração da Clínica**: Owner define quem normalmente recebe pagamentos:
   - Médico (ao finalizar atendimento)
   - Secretária (antes/após atendimento)
   - Outro funcionário
5. **Rastreabilidade**: Sistema registra quem recebeu o pagamento e quando

### 2.7 Pós-Atendimento
1. Geração automática de documentos (receita, atestado, exames)
2. Envio de documentos ao paciente
3. Liberação para faturamento
4. Atualização de relatórios

## 3. Boas Práticas e Otimizações

### 3.1 UX (Experiência do Usuário)
- **Campos opcionais por padrão**: Apenas campos obrigatórios marcados como required
- **Autocomplete**: Busca assistida para CID-10 e medicamentos
- **Templates por especialidade**: Formulários customizáveis via `ConsultationFormConfiguration`
- **Atalhos de teclado**: Para agilizar navegação
- **Salvamento automático**: Prevenção de perda de dados (a cada 30 segundos)
- **Indicador de pagamento**: Exibe status de pagamento na tela de atendimento
- **Opção de registro de pagamento**: Médico pode registrar recebimento ao finalizar

### 3.2 Regras de Negócio
**Não permitir finalizar sem**:
- Queixa principal
- História da doença atual
- Pelo menos um exame clínico
- Pelo menos um diagnóstico
- Pelo menos um plano terapêutico

**Alertas**:
- Alergias do paciente (destacadas)
- Interações medicamentosas (implementação futura)

### 3.3 Arquitetura
**Agregados principais**:
- `Appointment` - Agendamento
- `MedicalRecord` - Prontuário (agregado raiz)
  - `ClinicalExamination` - Exames clínicos
  - `DiagnosticHypothesis` - Diagnósticos
  - `TherapeuticPlan` - Planos terapêuticos
  - `PrescriptionItem` - Itens de prescrição
  - `InformedConsent` - Consentimentos

**Versionamento**: Todo histórico é imutável para auditoria.

### 3.4 LGPD e Auditoria
**Logs de auditoria** (`BaseEntity`):
- `CreatedAt` - Data/hora de criação
- `UpdatedAt` - Data/hora de última atualização
- `TenantId` - Identificação do tenant (multi-tenancy)

**Rastreabilidade**:
- Quem alterou (`ClosedByUserId`)
- Quando alterou (`ClosedAt`, `UpdatedAt`)
- O quê foi alterado (versionamento de entidades)

**Imutabilidade**:
- Prontuários fechados não podem ser alterados (apenas com reabertura explícita)
- Histórico médico nunca é sobrescrito (apenas nova versão)

## 4. Validações e Restrições

### 4.1 Validações de Negócio

**Appointment**:
- Data agendada não pode estar no passado (exceto com flag `allowHistoricalData`)
- Duração deve ser positiva
- Se PaymentType = HealthInsurance, HealthInsurancePlanId é obrigatório
- Não é possível alterar tipo, modo ou pagamento de consultas finalizadas ou canceladas

**ClinicalExamination**:
- Pressão sistólica: 50-300 mmHg
- Pressão diastólica: 30-200 mmHg
- Frequência cardíaca: 30-220 bpm
- Frequência respiratória: 8-60 rpm
- Temperatura: 32-45 °C
- Saturação de oxigênio: 0-100%
- Peso: 0.5-500 kg
- Altura: 0.3-3.0 metros
- Exame sistemático: mínimo 20 caracteres

**DiagnosticHypothesis**:
- Código CID-10 validado (formato: Letra + 2 dígitos, opcionalmente .1-2 dígitos)
- Exemplos válidos: A00, A00.0, A00.01, Z99.9

**MedicalRecord**:
- Queixa principal: mínimo 10 caracteres
- História da doença atual: mínimo 50 caracteres
- Não pode ser fechado sem campos obrigatórios
- Não pode ser alterado após fechamento (exceto com reabertura)

**TherapeuticPlan**:
- Tratamento: mínimo 20 caracteres
- Data de retorno deve estar no futuro

### 4.2 Transições de Status

**Appointment Status**:
```
Scheduled → Confirmed → InProgress → Completed
         ↓            ↓           ↓
      Cancelled    NoShow    Cancelled
```

**Regras**:
- `Confirm()`: Apenas de Scheduled
- `CheckIn()`: Apenas de Scheduled ou Confirmed
- `CheckOut()`: Apenas de InProgress
- `Cancel()`: Não permitido se Completed ou já Cancelled
- `MarkAsNoShow()`: Apenas de Scheduled ou Confirmed

## 5. Exemplos de Uso

### 5.1 Criar um Agendamento
```csharp
var appointment = new Appointment(
    patientId: patientGuid,
    clinicId: clinicGuid,
    scheduledDate: DateTime.Today.AddDays(1),
    scheduledTime: new TimeSpan(14, 30, 0),
    durationMinutes: 30,
    type: AppointmentType.Regular,
    tenantId: "clinic-abc",
    mode: AppointmentMode.InPerson,
    paymentType: PaymentType.HealthInsurance,
    professionalId: doctorGuid,
    healthInsurancePlanId: insuranceGuid,
    notes: "Consulta de rotina"
);
```

### 5.2 Registrar Exame Clínico
```csharp
var examination = new ClinicalExamination(
    medicalRecordId: recordGuid,
    tenantId: "clinic-abc",
    systematicExamination: "Paciente em bom estado geral, corado, hidratado...",
    bloodPressureSystolic: 120m,
    bloodPressureDiastolic: 80m,
    heartRate: 72,
    respiratoryRate: 16,
    temperature: 36.5m,
    oxygenSaturation: 98m,
    weight: 70.5m,
    height: 1.75m
);

// BMI calculado automaticamente: 23.02
var bmi = examination.BMI;
```

### 5.3 Adicionar Diagnóstico
```csharp
var diagnosis = new DiagnosticHypothesis(
    medicalRecordId: recordGuid,
    tenantId: "clinic-abc",
    description: "Hipertensão arterial sistêmica",
    icd10Code: "I10",
    type: DiagnosisType.Principal
);
```

### 5.4 Finalizar Prontuário
```csharp
medicalRecord.CloseMedicalRecord(
    closedByUserId: doctorGuid,
    professionalSignature: "Dr. João Silva - CRM 12345"
);
```

## 6. Customização por Clínica

Cada clínica pode customizar quais campos exibir através da entidade `ConsultationFormConfiguration`:

```csharp
var config = new ConsultationFormConfiguration(
    clinicId: clinicGuid,
    configurationName: "Cardiologia",
    tenantId: "clinic-abc",
    showChiefComplaint: true,
    showHistoryOfPresentIllness: true,
    showPastMedicalHistory: true,
    showFamilyHistory: true,
    showLifestyleHabits: true,
    showCurrentMedications: true
);
```

## 7. Conformidade Legal

Este fluxo está em conformidade com:
- **CFM 1.821/2007**: Resolução sobre prontuário eletrônico
- **LGPD**: Lei Geral de Proteção de Dados
- **Código de Ética Médica**: Sigilo e guarda de informações

## 8. Próximos Passos

### Frontend
- [x] Implementar interface de registro de consulta
- [x] Adicionar salvamento automático (autosave a cada 30 segundos)
- [x] Implementar botão de finalização de atendimento
- [x] Exibir status de pagamento na tela de atendimento
- [x] Implementar opção de registro de pagamento pelo médico
- [ ] Adicionar calculadora de IMC visual
- [ ] Implementar busca assistida de CID-10
- [ ] Criar templates de consulta por especialidade
- [ ] Implementar assinatura digital

### Backend
- [x] Adicionar controle de pagamento aos agendamentos
- [x] Implementar endpoint para finalizar atendimento pelo médico
- [x] Adicionar configuração de tipo de recebedor de pagamento na clínica
- [ ] Implementar geração de documentos (PDF)
- [ ] Adicionar alertas de interação medicamentosa
- [ ] Implementar faturamento automático
- [ ] Adicionar relatórios gerenciais
- [ ] Implementar notificações pós-consulta

### Integrações
- [ ] Integração com sistemas de convênio
- [ ] Integração com laboratórios
- [ ] Integração com farmácias
- [ ] Integração com telemedicina

## 9. Funcionalidades Implementadas (Janeiro 2026)

### Autosave (Salvamento Automático)
- **Frequência**: A cada 30 segundos
- **Inteligente**: Não salva se não houver alterações ou se acabou de salvar manualmente
- **Silencioso**: Não exibe mensagens de sucesso para não interromper o fluxo do médico
- **Previne perda de dados**: Mesmo que o navegador seja fechado acidentalmente

### Controle de Pagamento
- **Visibilidade**: Status de pagamento (Pago/Pendente) exibido na tela de atendimento
- **Flexibilidade**: Pagamento pode ser registrado:
  - Pela secretária antes/após o atendimento
  - Pelo médico ao finalizar a consulta
  - Por outro funcionário autorizado
- **Rastreabilidade**: Sistema registra quem recebeu e quando
- **Configurável**: Owner define o padrão de recebimento da clínica

### Finalização de Atendimento
- **Botão dedicado**: "Finalizar Atendimento" substitui "Finalizar e Notificar Secretaria"
- **Check-out automático**: Atualiza status do agendamento para "Completed"
- **Opção de pagamento**: Checkbox para registrar recebimento ao finalizar
- **Integração completa**: Finaliza prontuário médico e agendamento simultaneamente
