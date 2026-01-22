# 📋 Atendimento e Consulta - Guia de Configuração e Testes

## 📌 Visão Geral

Este guia fornece instruções completas para configurar e testar o módulo de Atendimento e Consultas do PrimeCare Software, cobrindo todo o fluxo desde o agendamento até a conclusão do atendimento.

## 🔧 Pré-requisitos

- Sistema iniciado (API + Frontend)
- Usuário com perfil Owner ou Medic logado
- Pacientes cadastrados
- Agenda configurada
- Procedimentos/serviços cadastrados

## 📖 Índice

1. [Configuração Inicial](#configuração-inicial)
2. [Cenários de Teste - Agendamento](#cenários-de-teste---agendamento)
3. [Cenários de Teste - Sala de Espera](#cenários-de-teste---sala-de-espera)
4. [Cenários de Teste - Atendimento](#cenários-de-teste---atendimento)
5. [Cenários de Teste - Prontuário Eletrônico](#cenários-de-teste---prontuário-eletrônico)
6. [Cenários de Teste - Prescrições](#cenários-de-teste---prescrições)
7. [Cenários de Teste - Conclusão](#cenários-de-teste---conclusão)
8. [API Testing](#api-testing)
9. [Troubleshooting](#troubleshooting)

---

## 🔧 Configuração Inicial

### 1. Configurar Agenda Médica

**Passos:**
1. Acesse o menu **"Configurações"** → **"Agenda"**
2. Configure os horários de atendimento:
   - **Segunda a Sexta:** 08:00 - 18:00
   - **Intervalo de Almoço:** 12:00 - 13:00
   - **Duração Padrão da Consulta:** 30 minutos
   - **Intervalo entre Consultas:** 5 minutos

3. Configure os dias de trabalho por médico
4. Defina bloqueios para feriados/férias

**Resultado Esperado:**
- ✅ Agenda salva com sucesso
- ✅ Horários disponíveis para agendamento

---

### 2. Cadastrar Tipos de Consulta

**Passos:**
1. Acesse **"Configurações"** → **"Tipos de Consulta"**
2. Cadastre os tipos:
   - **Consulta Normal** (30 min) - R$ 200,00
   - **Retorno** (20 min) - R$ 100,00
   - **Primeira Consulta** (45 min) - R$ 250,00
   - **Exames** (15 min) - R$ 150,00
   - **Procedimentos** (60 min) - R$ 400,00

**Resultado Esperado:**
- ✅ Tipos criados e disponíveis para agendamento

---

### 3. Verificar Permissões

**Perfis com acesso ao Atendimento:**
- ✅ Owner (acesso total)
- ✅ Medic (acesso às suas consultas)
- ✅ Secretary (agendamento e check-in)
- ✅ Nurse (sala de espera e triagem)
- ❌ SystemAdmin (sem acesso)

---

## 🧪 Cenários de Teste - Agendamento

### Cenário 1.1: Criar Agendamento Simples

**Objetivo:** Agendar consulta normal para paciente

**Passos:**
1. Acesse **"Agenda"** ou **"Agendamentos"**
2. Clique em **"+ Novo Agendamento"**
3. Preencha:
   - **Paciente:** Maria Silva Santos
   - **Médico:** Dr. João Santos
   - **Data:** 25/01/2026
   - **Horário:** 14:00
   - **Tipo:** Consulta Normal
   - **Duração:** 30 minutos
   - **Convênio:** Particular
   - **Observações:** Paciente com dor de cabeça

4. Clique em **"Agendar"**

**Resultado Esperado:**
- ✅ Agendamento criado com sucesso
- ✅ Aparece no calendário
- ✅ Paciente e médico notificados (se configurado)
- ✅ Status inicial: "Agendado"

---

### Cenário 1.2: Agendar com Convênio

**Objetivo:** Criar agendamento usando convênio médico

**Pré-requisito:** Paciente com convênio cadastrado

**Passos:**
1. Crie novo agendamento
2. Selecione paciente com convênio
3. Em **"Tipo de Pagamento"**, selecione **"Convênio"**
4. Selecione o convênio do paciente
5. Preencha o número da carteirinha (auto-preenchido)
6. Complete o agendamento

**Resultado Esperado:**
- ✅ Agendamento vinculado ao convênio
- ✅ Valor será faturado ao convênio
- ✅ Carteirinha validada

---

### Cenário 1.3: Agendamento Recorrente

**Objetivo:** Criar série de agendamentos

**Passos:**
1. Crie novo agendamento
2. Marque **"Consulta Recorrente"**
3. Configure:
   - **Frequência:** Semanal
   - **Dia da Semana:** Segunda-feira
   - **Horário:** 10:00
   - **Repetir por:** 4 semanas

4. Confirme

**Resultado Esperado:**
- ✅ 4 agendamentos criados automaticamente
- ✅ Todos aparecem no calendário
- ✅ Podem ser editados individualmente

---

### Cenário 1.4: Verificar Conflitos de Horário

**Objetivo:** Validar que não há sobreposição de horários

**Passos:**
1. Tente agendar consulta às 14:00
2. Tente agendar outra consulta no mesmo horário para o mesmo médico

**Resultado Esperado:**
- ❌ Erro: "Horário já ocupado"
- ✅ Sugestão de próximos horários disponíveis

---

### Cenário 1.5: Lista de Espera

**Objetivo:** Adicionar paciente à lista de espera

**Passos:**
1. Tente agendar para horário ocupado
2. Sistema oferece **"Adicionar à Lista de Espera"**
3. Confirme adição
4. Quando houver cancelamento, paciente é notificado

**Resultado Esperado:**
- ✅ Paciente adicionado à lista
- ✅ Notificação automática quando vaga abrir

---

## 🧪 Cenários de Teste - Sala de Espera

### Cenário 2.1: Check-in do Paciente

**Objetivo:** Registrar chegada do paciente

**Passos:**
1. Na data da consulta, acesse **"Sala de Espera"**
2. Localize o agendamento
3. Clique em **"Check-in"**
4. Confirme horário de chegada

**Resultado Esperado:**
- ✅ Status alterado para "Aguardando Atendimento"
- ✅ Horário de chegada registrado
- ✅ Médico notificado

---

### Cenário 2.2: Triagem (Enfermagem)

**Objetivo:** Registrar dados vitais antes da consulta

**Perfil:** Nurse ou Secretary

**Passos:**
1. Na sala de espera, clique em **"Triagem"**
2. Registre:
   - **Pressão Arterial:** 120/80 mmHg
   - **Temperatura:** 36.5°C
   - **Peso:** 70 kg
   - **Altura:** 1.65 m
   - **Frequência Cardíaca:** 75 bpm
   - **Saturação O2:** 98%

3. Adicione observações se necessário
4. Salve

**Resultado Esperado:**
- ✅ Dados salvos no prontuário
- ✅ Disponíveis para o médico durante consulta
- ✅ Status alterado para "Em Triagem"

---

### Cenário 2.3: Fila de Atendimento

**Objetivo:** Gerenciar ordem de atendimento

**Passos:**
1. Visualize a fila de espera
2. Arraste e solte para reordenar (se necessário)
3. Priorize emergências

**Resultado Esperado:**
- ✅ Ordem personalizada mantida
- ✅ Pacientes atendidos na sequência correta

---

## 🧪 Cenários de Teste - Atendimento

### Cenário 3.1: Iniciar Atendimento

**Objetivo:** Médico inicia consulta com paciente

**Perfil:** Medic ou Owner

**Passos:**
1. Na sala de espera, clique em **"Iniciar Atendimento"**
2. Sistema abre prontuário eletrônico
3. Timer inicia automaticamente
4. Status muda para "Em Atendimento"

**Resultado Esperado:**
- ✅ Tela de atendimento aberta
- ✅ Dados do paciente carregados
- ✅ Histórico disponível
- ✅ Timer rodando

---

### Cenário 3.2: Visualizar Histórico do Paciente

**Objetivo:** Acessar consultas anteriores

**Passos:**
1. Durante atendimento, clique na aba **"Histórico"**
2. Visualize consultas anteriores
3. Clique em uma consulta para ver detalhes

**Resultado Esperado:**
- ✅ Lista cronológica de consultas
- ✅ Diagnósticos anteriores
- ✅ Prescrições passadas
- ✅ Exames realizados

---

### Cenário 3.3: Registrar Anamnese

**Objetivo:** Documentar queixa principal e história

**Passos:**
1. Na aba **"Anamnese"**, preencha:
   - **Queixa Principal:** Dor de cabeça há 3 dias
   - **História da Doença Atual:** Dor frontal, pulsátil, intensidade 7/10
   - **História Pregressa:** Hipertensão controlada
   - **História Familiar:** Pai diabético
   - **Hábitos:** Não fuma, não bebe

2. Salve (auto-save a cada 30 segundos)

**Resultado Esperado:**
- ✅ Anamnese salva
- ✅ Disponível em consultas futuras

---

### Cenário 3.4: Exame Físico

**Objetivo:** Registrar achados do exame físico

**Passos:**
1. Na aba **"Exame Físico"**, preencha:
   - **Estado Geral:** Bom
   - **Cabeça e Pescoço:** Sem alterações
   - **Cardiovascular:** RCR 2T, BNF
   - **Respiratório:** MV+ bilateralmente
   - **Abdome:** Flácido, RHA+, indolor
   - **Extremidades:** Sem edemas

2. Use templates salvos se disponível
3. Salve

**Resultado Esperado:**
- ✅ Exame físico documentado
- ✅ Integrado ao prontuário

---

## 🧪 Cenários de Teste - Prontuário Eletrônico

### Cenário 4.1: Anexar Documentos

**Objetivo:** Adicionar exames e documentos ao prontuário

**Passos:**
1. Clique em **"Anexos"**
2. Faça upload de:
   - Exame de sangue (PDF)
   - Raio-X (JPEG)
   - Eletrocardiograma (PDF)

3. Adicione descrição para cada documento

**Resultado Esperado:**
- ✅ Documentos anexados
- ✅ Thumbnail gerado para imagens
- ✅ Disponível para visualização futura

---

### Cenário 4.2: Registrar Diagnóstico

**Objetivo:** Documentar CID-10

**Passos:**
1. Na aba **"Diagnóstico"**, clique em **"Adicionar CID"**
2. Busque por: "Cefaleia"
3. Selecione: **G44.2 - Cefaleia tensional**
4. Adicione observações se necessário
5. Salve

**Resultado Esperado:**
- ✅ CID registrado
- ✅ Vinculado à consulta
- ✅ Usado para estatísticas

---

### Cenário 4.3: Adicionar Hipótese Diagnóstica

**Objetivo:** Registrar possíveis diagnósticos

**Passos:**
1. Durante investigação, adicione hipóteses:
   - Hipótese 1: Enxaqueca
   - Hipótese 2: Cefaleia tensional
   - Hipótese 3: Sinusite

2. Marque a confirmada após exames

**Resultado Esperado:**
- ✅ Hipóteses registradas
- ✅ Raciocínio clínico documentado

---

## 🧪 Cenários de Teste - Prescrições

### Cenário 5.1: Prescrever Medicamentos

**Objetivo:** Criar receita médica

**Passos:**
1. Clique em **"Nova Prescrição"**
2. Adicione medicamentos:
   
   **Medicamento 1:**
   - Nome: Dipirona
   - Dosagem: 500mg
   - Via: Oral
   - Frequência: 6/6 horas
   - Duração: 3 dias
   - Observações: Se dor

   **Medicamento 2:**
   - Nome: Paracetamol
   - Dosagem: 750mg
   - Via: Oral
   - Frequência: 8/8 horas
   - Duração: 5 dias

3. Adicione recomendações gerais
4. Gere a receita

**Resultado Esperado:**
- ✅ Receita gerada em PDF
- ✅ Assinada digitalmente (se configurado)
- ✅ Disponível para impressão
- ✅ Armazenada no prontuário

---

### Cenário 5.2: Prescrição de Medicamento Controlado

**Objetivo:** Receita com notificação de receita B

**Passos:**
1. Adicione medicamento controlado:
   - Nome: Clonazepam
   - Dosagem: 2mg
   - Via: Oral
   - Frequência: 1x ao dia
   - Duração: 30 dias

2. Sistema identifica como controlado
3. Gera notificação de receita B

**Resultado Esperado:**
- ✅ Receita B gerada separadamente
- ✅ Numeração sequencial
- ✅ Campos obrigatórios preenchidos
- ✅ Conforme Portaria 344/98

---

### Cenário 5.3: Solicitar Exames

**Objetivo:** Criar solicitação de exames

**Passos:**
1. Clique em **"Solicitar Exames"**
2. Selecione exames:
   - Hemograma completo
   - Glicemia de jejum
   - TSH e T4 livre
   - Raio-X de tórax

3. Adicione indicação clínica
4. Gere a solicitação

**Resultado Esperado:**
- ✅ Solicitação em PDF
- ✅ Código TUSS dos exames
- ✅ Autorização para convênio (se aplicável)

---

## 🧪 Cenários de Teste - Conclusão

### Cenário 6.1: Finalizar Consulta

**Objetivo:** Concluir atendimento e gerar documentação

**Passos:**
1. Revise todas as informações
2. Clique em **"Finalizar Consulta"**
3. Sistema para o timer
4. Confirme diagnóstico final
5. Escolha documentos para gerar:
   - [ ] Receita médica
   - [ ] Atestado médico
   - [ ] Solicitação de exames
   - [ ] Relatório médico

6. Confirme finalização

**Resultado Esperado:**
- ✅ Status alterado para "Concluído"
- ✅ Duração total registrada
- ✅ Documentos gerados
- ✅ Prontuário fechado

---

### Cenário 6.2: Gerar Atestado Médico

**Objetivo:** Emitir atestado de comparecimento/afastamento

**Passos:**
1. Durante finalização, selecione **"Atestado Médico"**
2. Escolha tipo:
   - Comparecimento (sem CID)
   - Afastamento (com CID e dias)

3. Para afastamento, preencha:
   - **Dias de Afastamento:** 3 dias
   - **CID (opcional):** G44.2

4. Gere o atestado

**Resultado Esperado:**
- ✅ Atestado em PDF
- ✅ Assinado digitalmente
- ✅ Numeração sequencial
- ✅ Válido legalmente

---

### Cenário 6.3: Processar Pagamento

**Objetivo:** Registrar pagamento da consulta

**Passos:**
1. Após finalizar consulta, vá para **"Financeiro"**
2. Localize o fechamento da consulta
3. Registre pagamento:
   - **Forma:** Cartão de Crédito
   - **Valor:** R$ 200,00
   - **Parcelas:** 1x

4. Confirme

**Resultado Esperado:**
- ✅ Pagamento registrado
- ✅ Recibo gerado
- ✅ Entrada no caixa
- ✅ Status: "Pago"

---

### Cenário 6.4: Agendar Retorno

**Objetivo:** Marcar consulta de retorno

**Passos:**
1. Durante finalização, marque **"Agendar Retorno"**
2. Selecione data: 7 dias úteis
3. Horário: 14:00
4. Tipo: Retorno
5. Confirme

**Resultado Esperado:**
- ✅ Retorno agendado automaticamente
- ✅ Paciente notificado
- ✅ Vinculado à consulta original

---

## 🔌 API Testing

### Endpoint: Criar Agendamento

```bash
curl -X POST "http://localhost:5000/api/appointments" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "patientId": "patient-uuid",
    "medicId": "medic-uuid",
    "appointmentDate": "2026-01-25T14:00:00Z",
    "duration": 30,
    "appointmentType": "Consulta Normal",
    "paymentType": "Particular",
    "observations": "Paciente com dor de cabeça"
  }'
```

---

### Endpoint: Check-in

```bash
curl -X POST "http://localhost:5000/api/appointments/{appointment_id}/checkin" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "checkinTime": "2026-01-25T13:55:00Z"
  }'
```

---

### Endpoint: Iniciar Atendimento

```bash
curl -X POST "http://localhost:5000/api/appointments/{appointment_id}/start" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "startTime": "2026-01-25T14:00:00Z"
  }'
```

---

### Endpoint: Salvar Anamnese

```bash
curl -X POST "http://localhost:5000/api/medical-records/{record_id}/anamnesis" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "chiefComplaint": "Dor de cabeça há 3 dias",
    "historyOfPresentIllness": "Dor frontal, pulsátil, intensidade 7/10",
    "pastMedicalHistory": "Hipertensão controlada",
    "familyHistory": "Pai diabético",
    "socialHistory": "Não fuma, não bebe"
  }'
```

---

### Endpoint: Finalizar Consulta

```bash
curl -X POST "http://localhost:5000/api/appointments/{appointment_id}/complete" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "endTime": "2026-01-25T14:30:00Z",
    "diagnosis": "G44.2",
    "generatePrescription": true,
    "generateCertificate": true
  }'
```

---

## 🐛 Troubleshooting

### Problema 1: Timer não inicia

**Causa:** JavaScript desabilitado ou erro no frontend

**Solução:**
1. Verifique console do navegador
2. Recarregue a página
3. Timer pode ser iniciado manualmente

---

### Problema 2: Histórico não carrega

**Causa:** Consultas anteriores de outra clínica

**Solução:**
1. Verifique TenantId
2. Confirme que paciente pertence à clínica
3. Verifique permissões de acesso

---

### Problema 3: Prescrição não gera PDF

**Causa:** Assinatura digital não configurada

**Solução:**
1. Configure certificado digital em **Configurações**
2. Ou use modo sem assinatura (apenas para testes)
3. Verifique templates de PDF

---

### Problema 4: Convênio não aparece

**Causa:** Convênio não cadastrado ou inativo

**Solução:**
1. Cadastre convênio em **Configurações** → **Convênios**
2. Verifique se está ativo
3. Vincule ao paciente

---

## ✅ Checklist de Validação Final

- [ ] Criar agendamento simples
- [ ] Agendar com convênio
- [ ] Agendamento recorrente
- [ ] Verificar conflitos de horário
- [ ] Adicionar à lista de espera
- [ ] Check-in do paciente
- [ ] Realizar triagem
- [ ] Gerenciar fila de atendimento
- [ ] Iniciar atendimento
- [ ] Visualizar histórico
- [ ] Registrar anamnese
- [ ] Documentar exame físico
- [ ] Anexar documentos
- [ ] Registrar diagnóstico (CID)
- [ ] Prescrever medicamentos
- [ ] Prescrição de controlados
- [ ] Solicitar exames
- [ ] Finalizar consulta
- [ ] Gerar atestado médico
- [ ] Processar pagamento
- [ ] Agendar retorno
- [ ] Testes de API (fluxo completo)

---

## 📚 Documentação Relacionada

- [Cadastro de Paciente](01-CADASTRO-PACIENTE.md)
- [Módulo Financeiro](03-MODULO-FINANCEIRO.md)
- [Fluxo de Consulta](../MEDICAL_CONSULTATION_FLOW.md)
- [Prontuário Eletrônico](../PATIENT_HISTORY_API.md)
