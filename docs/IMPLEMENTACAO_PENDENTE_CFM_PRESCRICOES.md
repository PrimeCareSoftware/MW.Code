# Implementação Pendente: CFM 1.821 & Receitas Digitais

## Status Atual

### Backend ✅ (90% Completo)
- ✅ Entidades de domínio implementadas:
  - `InformedConsent` - Consentimento informado
  - `ClinicalExamination` - Exame clínico com sinais vitais
  - `DiagnosticHypothesis` - Hipóteses diagnósticas com CID-10
  - `TherapeuticPlan` - Plano terapêutico
  - `DigitalPrescription` - Prescrições digitais
  - `DigitalPrescriptionItem` - Itens de prescrição
  - `SNGPCReport` - Relatórios ANVISA
  
- ✅ Repositórios implementados para todas as entidades
- ✅ Commands e Handlers implementados
- ✅ Controllers da API implementados:
  - `/api/InformedConsents`
  - `/api/ClinicalExaminations`
  - `/api/DiagnosticHypotheses`
  - `/api/TherapeuticPlans`
  - `/api/DigitalPrescriptions`
  - `/api/SNGPCReports`

- ✅ Testes unitários parciais (DiagnosticHypothesis, ClinicalExamination)

### Frontend ⚠️ (30% Completo)
- ✅ Modelos TypeScript básicos
- ✅ Serviços básicos criados:
  - `InformedConsentService`
  - `ClinicalExaminationService`
  - `DiagnosticHypothesisService`
  - `TherapeuticPlanService`
  - `DigitalPrescriptionService`

- ⚠️ Componentes parcialmente implementados:
  - `prescription-type-selector.component.ts` (básico)
  - `sngpc-dashboard.component.ts` (básico)

- ❌ Faltam componentes principais:
  - Formulário de consentimento informado
  - Formulário de exame clínico com sinais vitais
  - Formulário de hipóteses diagnósticas com busca CID-10
  - Formulário de plano terapêutico
  - Formulário completo de prescrição digital
  - Visualização/impressão de prescrição
  - Interface de assinatura digital

### Documentação ✅ (80% Completo)
- ✅ `CFM_1821_IMPLEMENTACAO.md` - Documentação da implementação
- ✅ `DIGITAL_PRESCRIPTIONS.md` - Documentação de prescrições
- ✅ `ESPECIFICACAO_CFM_1821.md` - Especificação técnica
- ⚠️ Guias de usuário precisam ser criados

## Plano de Implementação Detalhado

### Fase 1: Componentes Frontend CFM 1.821 (Prioridade Alta)

#### 1.1 Informed Consent Component ✅ (Criado)
- [x] Criar `informed-consent-form.component.ts`
- [x] Formulário para criar consentimento
- [x] Lista de consentimentos existentes
- [x] Aceite imediato opcional
- [ ] Integrar no fluxo de atendimento

#### 1.2 Clinical Examination Component
**Arquivo**: `frontend/medicwarehouse-app/src/app/pages/attendance/components/clinical-examination-form.component.ts`

**Funcionalidades**:
- Formulário de sinais vitais com validações de range:
  - Pressão Arterial (50-300 sistólica / 30-200 diastólica)
  - Frequência Cardíaca (30-220 bpm)
  - Frequência Respiratória (8-60 irpm)
  - Temperatura (32-45°C)
  - Saturação O2 (0-100%)
- Campo de exame físico sistemático (mínimo 20 caracteres)
- Campo de estado geral
- Validações visuais de ranges anormais
- Auto-save opcional

#### 1.3 Diagnostic Hypothesis Component
**Arquivo**: `frontend/medicwarehouse-app/src/app/pages/attendance/components/diagnostic-hypothesis-form.component.ts`

**Funcionalidades**:
- Lista de hipóteses diagnósticas (múltiplas)
- Busca/autocomplete de CID-10
- Tipo: Principal ou Secundário
- Validação de formato CID-10
- CRUD inline de diagnósticos

#### 1.4 Therapeutic Plan Component
**Arquivo**: `frontend/medicwarehouse-app/src/app/pages/attendance/components/therapeutic-plan-form.component.ts`

**Funcionalidades**:
- Tratamento/Conduta (mínimo 20 caracteres)
- Prescrição medicamentosa (integra com editor rico existente)
- Solicitação de exames
- Encaminhamentos
- Orientações ao paciente
- Data de retorno com date picker

### Fase 2: Componentes Frontend Prescrições Digitais (Prioridade Alta)

#### 2.1 Digital Prescription Form Component
**Arquivo**: `frontend/medicwarehouse-app/src/app/pages/prescriptions/digital-prescription-form.component.ts`

**Funcionalidades**:
- Seleção de tipo de receita (integra com prescription-type-selector existente)
- Informações do médico (CRM, UF) - auto-preenchido
- Lista de medicamentos prescritos
- Editor de item de prescrição:
  - Nome do medicamento (autocomplete)
  - Dosagem
  - Frequência
  - Duração do tratamento
  - Quantidade
  - Instruções de uso
  - Classificação ANVISA (se controlado)
- Observações gerais
- Botão de assinar (preparado para ICP-Brasil)
- Preview antes de finalizar

#### 2.2 Digital Prescription View Component
**Arquivo**: `frontend/medicwarehouse-app/src/app/pages/prescriptions/digital-prescription-view.component.ts`

**Funcionalidades**:
- Visualização formatada da prescrição
- QR Code para verificação
- Status da prescrição (ativa, expirada, assinada)
- Botão de impressão
- Indicador de SNGPC se aplicável
- Data de validade

#### 2.3 SNGPC Dashboard Enhancement
**Arquivo**: Melhorar `frontend/medicwarehouse-app/src/app/pages/prescriptions/sngpc-dashboard.component.ts`

**Funcionalidades adicionais**:
- Filtros por período
- Status de transmissão
- Download de XML gerado
- Indicadores visuais de deadlines
- Relatórios não transmitidos destacados

### Fase 3: Integração no Fluxo de Atendimento (Prioridade Alta)

#### 3.1 Atualizar Attendance Component
**Arquivo**: `frontend/medicwarehouse-app/src/app/pages/attendance/attendance.ts` e `.html`

**Mudanças**:
- Adicionar aba ou seção para "Consentimento Informado"
- Adicionar aba ou seção para "Exame Clínico" com sinais vitais
- Adicionar aba ou seção para "Hipóteses Diagnósticas"
- Adicionar aba ou seção para "Plano Terapêutico"
- Integrar componentes criados
- Validar campos obrigatórios antes de finalizar atendimento
- Indicadores visuais de completude CFM 1.821

#### 3.2 Criar Wizard de Atendimento (Opcional mas Recomendado)
**Arquivo**: `frontend/medicwarehouse-app/src/app/pages/attendance/attendance-wizard.component.ts`

**Estrutura**:
1. Anamnese (já existe)
2. Exame Clínico (novo)
3. Hipóteses Diagnósticas (novo)
4. Plano Terapêutico (novo)
5. Prescrição (se aplicável)
6. Consentimento Informado (novo)
7. Finalização

### Fase 4: Testes (Prioridade Média)

#### 4.1 Testes de Componentes
- Testes unitários para cada componente Angular
- Testes de integração dos formulários
- Validação de campos obrigatórios

#### 4.2 Testes End-to-End
- Fluxo completo de atendimento com CFM 1.821
- Criação de prescrição digital completa
- Geração de relatório SNGPC
- Verificação de QR Code

### Fase 5: Documentação de Usuário (Prioridade Média)

#### 5.1 Guia do Médico
**Arquivo**: `docs/GUIA_MEDICO_CFM_1821.md` (já existe, precisa atualizar)

**Conteúdo**:
- Como preencher prontuário conforme CFM 1.821
- Campos obrigatórios e recomendados
- Boas práticas de preenchimento
- Screenshots dos formulários

#### 5.2 Guia de Prescrições Digitais
**Arquivo**: `docs/GUIA_PRESCRICOES_DIGITAIS.md` (criar)

**Conteúdo**:
- Tipos de receita e quando usar
- Como prescrever medicamentos controlados
- Fluxo de assinatura digital
- SNGPC: como gerar e transmitir relatórios
- Resolução de problemas comuns

#### 5.3 Vídeos Tutoriais (Opcional)
- Screencast do fluxo completo de atendimento
- Tutorial de prescrições controladas
- Tutorial de SNGPC

## Estimativa de Esforço

### Desenvolvimento Frontend
- Informed Consent Form: ✅ Completo (2h)
- Clinical Examination Form: 3-4 horas
- Diagnostic Hypothesis Form: 3-4 horas
- Therapeutic Plan Form: 2-3 horas
- Digital Prescription Form: 4-6 horas
- Digital Prescription View: 2-3 horas
- SNGPC Dashboard Enhancement: 2-3 horas
- Integração no Attendance: 4-6 horas
- **Total Desenvolvimento**: ~25-35 horas

### Testes
- Testes Unitários: 8-10 horas
- Testes de Integração: 4-6 horas
- Testes E2E: 4-6 horas
- **Total Testes**: ~16-22 horas

### Documentação
- Atualização de guias: 2-3 horas
- Criação de guias novos: 3-4 horas
- Screenshots e exemplos: 2-3 horas
- **Total Documentação**: ~7-10 horas

### Total Estimado: 48-67 horas (1-1.5 semanas para 1 desenvolvedor full-time)

## Priorização

### Must-Have (Crítico para Compliance)
1. ✅ Informed Consent Form
2. Clinical Examination Form (sinais vitais obrigatórios)
3. Diagnostic Hypothesis Form (CID-10 obrigatório)
4. Therapeutic Plan Form
5. Integração básica no fluxo de atendimento
6. Digital Prescription Form completo
7. SNGPC Dashboard funcional

### Should-Have (Melhora UX)
1. Digital Prescription View com impressão
2. Wizard de atendimento estruturado
3. Validações visuais de completude
4. Auto-save de formulários

### Nice-to-Have (Opcional)
1. Vídeos tutoriais
2. Templates de consentimento pré-definidos
3. Busca avançada de CID-10 com IA
4. Assinatura digital ICP-Brasil integrada (pode ser fase posterior)

## Próximos Passos Imediatos

1. ✅ Criar Informed Consent Form Component
2. Criar Clinical Examination Form Component
3. Criar Diagnostic Hypothesis Form Component
4. Criar Therapeutic Plan Form Component
5. Integrar componentes no Attendance page
6. Testar fluxo completo end-to-end
7. Atualizar documentação
8. Criar Digital Prescription Form
9. Melhorar SNGPC Dashboard
10. Testes finais e ajustes

## Observações Técnicas

### Dependências
- Todos os serviços backend já estão disponíveis
- Modelos TypeScript precisam ser verificados/atualizados
- FormBuilder do Angular para formulários reativos
- Signals do Angular 18 para state management
- RxJS para chamadas assíncronas

### Padrões de Código
- Standalone components (já usado no projeto)
- Control flow syntax (@if, @for) do Angular 17+
- Signals para estado reativo
- FormGroup com validações reativas
- Tratamento de erros consistente

### Validações Importantes
- CID-10: Formato com letras e números (ex: A00, J20.9, Z99.01)
- Sinais vitais: Ranges específicos por tipo
- Campos obrigatórios CFM: Mínimo de caracteres
- Consentimento: Mínimo 50 caracteres

## Status de Conclusão

- Backend: 90% ✅
- Frontend: 30% ⚠️ → Objetivo: 100%
- Testes: 40% ⚠️ → Objetivo: 80%
- Documentação: 80% ✅ → Objetivo: 100%

**Meta**: Atingir 100% de compliance CFM 1.821 e 100% de funcionalidade em prescrições digitais.

---

**Última atualização**: 7 de Janeiro de 2026
**Responsável**: Equipe de Desenvolvimento MedicWarehouse
