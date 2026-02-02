# 02 - Cenários de Testes de Agendamento

> **Módulo:** Agendamento de Consultas  
> **Tempo estimado:** 30-40 minutos  
> **Pré-requisitos:** Sistema configurado, usuário logado

## 🎯 Objetivo dos Testes

Validar o módulo de agendamento de consultas, incluindo:
- ✅ Criar agendamento
- ✅ Visualizar agenda
- ✅ Editar agendamento
- ✅ Cancelar agendamento
- ✅ Verificar conflitos de horário
- ✅ Envio de notificações
- ✅ Filtros e busca

## 🔧 Preparação

### Usuários de Teste
- **Admin:** admin@demo.com / Admin@123
- **Médico:** doctor@demo.com / Doctor@123
- **Secretária:** secretary@demo.com / Secretary@123

### URLs
- **Agendamentos:** http://localhost:4200/appointments

## 📝 Casos de Teste

### CT-APPT-001: Criar Novo Agendamento

**Objetivo:** Criar agendamento com sucesso

**Passos:**
1. Faça login como secretária
2. Acesse menu "Agendamentos"
3. Clique em "Novo Agendamento"
4. Preencha:
   - Paciente: Selecione "João Silva"
   - Médico: Selecione "Dr. Carlos"
   - Data: Amanhã
   - Horário: 14:00
   - Duração: 30 minutos
   - Tipo: Consulta
   - Observações: "Primeira consulta"
5. Clique em "Salvar"

**Resultado Esperado:**
- ✅ Mensagem de sucesso
- ✅ Agendamento aparece na agenda
- ✅ Notificação enviada ao paciente
- ✅ Email de confirmação enviado

**Prioridade:** 🔴 Crítica

---

### CT-APPT-002: Validar Conflito de Horário

**Objetivo:** Sistema deve prevenir agendamentos no mesmo horário

**Passos:**
1. Crie um agendamento: Dr. Carlos, amanhã 10:00
2. Tente criar outro agendamento: Dr. Carlos, amanhã 10:00

**Resultado Esperado:**
- ✅ Mensagem de erro: "Já existe agendamento neste horário"
- ✅ Agendamento não é criado
- ✅ Horários disponíveis são sugeridos

**Prioridade:** 🔴 Crítica

---

### CT-APPT-003: Editar Agendamento Existente

**Objetivo:** Modificar dados de um agendamento

**Passos:**
1. Acesse um agendamento existente
2. Clique em "Editar"
3. Altere o horário para 15:00
4. Clique em "Salvar"

**Resultado Esperado:**
- ✅ Agendamento atualizado
- ✅ Notificação de alteração enviada
- ✅ Histórico de alterações registrado

**Prioridade:** 🟡 Média

---

### CT-APPT-004: Cancelar Agendamento

**Objetivo:** Cancelar um agendamento com motivo

**Passos:**
1. Acesse um agendamento futuro
2. Clique em "Cancelar"
3. Informe motivo: "Paciente solicitou reagendamento"
4. Confirme o cancelamento

**Resultado Esperado:**
- ✅ Status muda para "Cancelado"
- ✅ Horário fica disponível novamente
- ✅ Notificação enviada ao paciente
- ✅ Motivo registrado no histórico

**Prioridade:** 🔴 Crítica

---

### CT-APPT-005: Visualizar Agenda do Dia

**Objetivo:** Ver todos os agendamentos de um dia específico

**Passos:**
1. Acesse "Agendamentos"
2. Selecione data de hoje
3. Visualize a agenda

**Resultado Esperado:**
- ✅ Mostra todos os agendamentos do dia
- ✅ Ordenados por horário
- ✅ Mostra status (confirmado, pendente, concluído, cancelado)
- ✅ Exibe médico, paciente e horário

**Prioridade:** 🔴 Crítica

---

### CT-APPT-006: Filtrar por Médico

**Objetivo:** Filtrar agendamentos de um médico específico

**Passos:**
1. Acesse "Agendamentos"
2. No filtro "Médico", selecione "Dr. Carlos"
3. Aplique o filtro

**Resultado Esperado:**
- ✅ Mostra apenas agendamentos do Dr. Carlos
- ✅ Contador atualizado
- ✅ Possível exportar lista

**Prioridade:** 🟡 Média

---

### CT-APPT-007: Filtrar por Status

**Objetivo:** Filtrar por status do agendamento

**Passos:**
1. Acesse "Agendamentos"
2. Selecione filtro "Status": Confirmado
3. Aplique

**Resultado Esperado:**
- ✅ Mostra apenas confirmados
- ✅ Outros status ocultos

**Prioridade:** 🟡 Média

---

### CT-APPT-008: Buscar Paciente

**Objetivo:** Buscar agendamentos por nome do paciente

**Passos:**
1. Acesse "Agendamentos"
2. No campo de busca, digite "João"
3. Pressione Enter

**Resultado Esperado:**
- ✅ Mostra agendamentos com "João" no nome
- ✅ Busca case-insensitive
- ✅ Busca por nome parcial funciona

**Prioridade:** 🟡 Média

---

### CT-APPT-009: Confirmar Chegada do Paciente

**Objetivo:** Marcar que paciente chegou

**Passos:**
1. Acesse agendamento do dia
2. Clique em "Confirmar Chegada"

**Resultado Esperado:**
- ✅ Status muda para "Paciente Chegou"
- ✅ Horário de chegada registrado
- ✅ Médico é notificado

**Prioridade:** 🟡 Média

---

### CT-APPT-010: Visualização em Calendário

**Objetivo:** Ver agendamentos em formato de calendário

**Passos:**
1. Acesse "Agendamentos"
2. Mude visualização para "Calendário"
3. Navegue pelos dias

**Resultado Esperado:**
- ✅ Calendário mensal exibido
- ✅ Dias com agendamentos destacados
- ✅ Ao clicar em dia, mostra lista
- ✅ Possível criar agendamento clicando em horário vazio

**Prioridade:** 🟢 Baixa

---

### CT-APPT-011: Enviar Lembrete Manual

**Objetivo:** Enviar lembrete ao paciente

**Passos:**
1. Acesse agendamento futuro
2. Clique em "Enviar Lembrete"
3. Escolha canal: SMS, Email, WhatsApp
4. Confirme envio

**Resultado Esperado:**
- ✅ Lembrete enviado com sucesso
- ✅ Registro de envio criado
- ✅ Mensagem de confirmação

**Prioridade:** 🟡 Média

---

### CT-APPT-012: Bloquear Horário

**Objetivo:** Bloquear horário para não permitir agendamentos

**Passos:**
1. Acesse agenda
2. Selecione horário vazio
3. Escolha "Bloquear Horário"
4. Informe motivo: "Reunião interna"
5. Salve

**Resultado Esperado:**
- ✅ Horário bloqueado
- ✅ Não aparece como disponível
- ✅ Mostra motivo ao passar o mouse

**Prioridade:** 🟡 Média

---

### CT-APPT-013: Reagendar Consulta

**Objetivo:** Reagendar para nova data/horário

**Passos:**
1. Acesse agendamento existente
2. Clique em "Reagendar"
3. Selecione nova data: Próxima semana, 10:00
4. Salve

**Resultado Esperado:**
- ✅ Agendamento movido para nova data
- ✅ Notificação enviada
- ✅ Histórico mantém registro da alteração

**Prioridade:** 🔴 Crítica

---

### CT-APPT-014: Agendar Retorno

**Objetivo:** Criar agendamento de retorno a partir de consulta

**Passos:**
1. Após finalizar consulta
2. Clique em "Agendar Retorno"
3. Sistema sugere data (30 dias)
4. Confirme ou ajuste
5. Salve

**Resultado Esperado:**
- ✅ Novo agendamento criado
- ✅ Marcado como "Retorno"
- ✅ Vinculado à consulta anterior

**Prioridade:** 🟡 Média

---

### CT-APPT-015: Exportar Agenda

**Objetivo:** Exportar lista de agendamentos

**Passos:**
1. Aplique filtros desejados
2. Clique em "Exportar"
3. Escolha formato: Excel
4. Confirme

**Resultado Esperado:**
- ✅ Arquivo .xlsx baixado
- ✅ Contém todos os dados filtrados
- ✅ Formatação preservada

**Prioridade:** 🟢 Baixa

---

### CT-APPT-016: Notificação Automática 24h Antes

**Objetivo:** Verificar envio automático de lembretes

**Pré-condições:**
- Agendar consulta para exatamente 24h no futuro
- Aguardar ou simular job

**Resultado Esperado:**
- ✅ Lembrete enviado automaticamente
- ✅ Log de envio registrado
- ✅ Status atualizado

**Prioridade:** 🟡 Média

---

## ✅ Critérios de Aceite

### Funcionalidades Básicas
- [ ] Criar agendamento funciona
- [ ] Editar agendamento funciona
- [ ] Cancelar agendamento funciona
- [ ] Visualizar agenda funciona

### Validações
- [ ] Conflito de horário é detectado
- [ ] Campos obrigatórios são validados
- [ ] Data/hora no passado não é permitida

### Notificações
- [ ] Confirmação de agendamento enviada
- [ ] Lembretes automáticos funcionam
- [ ] Alterações notificadas

### Interface
- [ ] Calendário exibe corretamente
- [ ] Filtros funcionam
- [ ] Busca funciona
- [ ] Responsivo em mobile

## 📚 Documentação Relacionada

- [Appointment Scheduling Summary](../../APPOINTMENT_SCHEDULING_SUMMARY.md)
- [Guia da Secretária](../../GUIA_SECRETARIA_AGENDAMENTO.md)

## ⏭️ Próximos Passos

Após completar os testes de agendamento:
1. ✅ Todos os casos de teste executados
2. ➡️ Vá para [03-Testes-Prontuario.md](03-Testes-Prontuario.md)

---

**Encontrou um bug?** Documente e reporte no GitHub.
