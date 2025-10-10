# Exemplo de Uso - Sistema de Rotinas de Notificação

## Cenário: Clínica Odontológica com Múltiplas Rotinas

### 1. Configuração Inicial

A **Clínica Sorriso Feliz** quer automatizar suas notificações para reduzir faltas e melhorar a comunicação com os pacientes.

### 2. Rotinas Implementadas

#### Rotina 1: Lembrete WhatsApp 24h Antes
**Objetivo**: Reduzir faltas enviando lembretes um dia antes

```bash
POST /api/notificationroutines
Authorization: Bearer eyJhbGc...
X-Tenant-Id: clinica-sorriso-feliz
Content-Type: application/json

{
  "name": "Lembrete WhatsApp 24h Antes",
  "description": "Envia lembrete automático via WhatsApp para consultas do dia seguinte",
  "channel": "WhatsApp",
  "type": "AppointmentReminder",
  "messageTemplate": "Olá {patientName}! 😊\n\nLembramos que você tem uma consulta amanhã ({appointmentDate}) às {appointmentTime} com Dr(a). {doctorName} na Clínica Sorriso Feliz.\n\n📍 Endereço: {clinicAddress}\n📞 Dúvidas: {clinicPhone}\n\nPor favor, confirme sua presença respondendo SIM ou ligue para reagendar.",
  "scheduleType": "Daily",
  "scheduleConfiguration": "{\"time\":\"18:00\"}",
  "scope": "Clinic",
  "maxRetries": 3,
  "recipientFilter": "{\"hasAppointmentNextDay\":true,\"appointmentStatus\":\"Scheduled\"}"
}
```

**Resposta**:
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "name": "Lembrete WhatsApp 24h Antes",
  "description": "Envia lembrete automático via WhatsApp para consultas do dia seguinte",
  "channel": "WhatsApp",
  "type": "AppointmentReminder",
  "messageTemplate": "Olá {patientName}! 😊...",
  "scheduleType": "Daily",
  "scheduleConfiguration": "{\"time\":\"18:00\"}",
  "scope": "Clinic",
  "isActive": true,
  "maxRetries": 3,
  "recipientFilter": "{\"hasAppointmentNextDay\":true,\"appointmentStatus\":\"Scheduled\"}",
  "lastExecutedAt": null,
  "nextExecutionAt": null,
  "createdAt": "2025-10-10T15:30:00Z",
  "updatedAt": null,
  "tenantId": "clinica-sorriso-feliz"
}
```

#### Rotina 2: Confirmação Imediata por SMS
**Objetivo**: Confirmar agendamento logo após ele ser criado

```bash
POST /api/notificationroutines

{
  "name": "Confirmação Imediata SMS",
  "description": "Confirma o agendamento via SMS assim que ele é criado",
  "channel": "SMS",
  "type": "AppointmentConfirmation",
  "messageTemplate": "Clínica Sorriso Feliz: Consulta agendada para {appointmentDate} às {appointmentTime} com Dr(a). {doctorName}. Para cancelar: {clinicPhone}",
  "scheduleType": "BeforeAppointment",
  "scheduleConfiguration": "{\"hours\":0}",
  "scope": "Clinic",
  "maxRetries": 3,
  "recipientFilter": "{\"appointmentStatus\":\"Scheduled\"}"
}
```

#### Rotina 3: Lembrete de Pagamento Semanal
**Objetivo**: Cobrar faturas pendentes toda segunda-feira

```bash
POST /api/notificationroutines

{
  "name": "Lembrete de Pagamento - Segunda-feira",
  "description": "Envia lembrete de faturas pendentes toda segunda-feira às 9h",
  "channel": "Email",
  "type": "PaymentReminder",
  "messageTemplate": "Prezado(a) {patientName},\n\nIdentificamos que você possui faturas pendentes na Clínica Sorriso Feliz.\n\nPor favor, acesse nosso portal ou entre em contato para regularizar sua situação.\n\nTelefone: {clinicPhone}\nEndereço: {clinicAddress}\n\nAtenciosamente,\nEquipe Clínica Sorriso Feliz",
  "scheduleType": "Weekly",
  "scheduleConfiguration": "{\"days\":[\"monday\"],\"time\":\"09:00\"}",
  "scope": "Clinic",
  "maxRetries": 5,
  "recipientFilter": "{\"hasUnpaidInvoices\":true,\"invoicesOverdue\":true}"
}
```

#### Rotina 4: Pesquisa de Satisfação 2h Após Consulta
**Objetivo**: Coletar feedback dos pacientes

```bash
POST /api/notificationroutines

{
  "name": "Pesquisa de Satisfação",
  "description": "Envia pesquisa de satisfação 2 horas após a consulta",
  "channel": "WhatsApp",
  "type": "General",
  "messageTemplate": "Olá {patientName}! 😊\n\nEsperamos que sua consulta com Dr(a). {doctorName} tenha sido ótima!\n\nPor favor, avalie nosso atendimento de 1 a 5:\n1⭐ - Muito insatisfeito\n5⭐ - Muito satisfeito\n\nSua opinião é muito importante para nós!",
  "scheduleType": "AfterAppointment",
  "scheduleConfiguration": "{\"hours\":2}",
  "scope": "Clinic",
  "maxRetries": 2,
  "recipientFilter": "{\"appointmentStatus\":\"Completed\"}"
}
```

### 3. Gerenciando Rotinas

#### Listar Todas as Rotinas Ativas

```bash
GET /api/notificationroutines/active
Authorization: Bearer eyJhbGc...
X-Tenant-Id: clinica-sorriso-feliz
```

**Resposta**:
```json
[
  {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "name": "Lembrete WhatsApp 24h Antes",
    "isActive": true,
    "lastExecutedAt": "2025-10-09T18:00:00Z",
    "nextExecutionAt": "2025-10-10T18:00:00Z"
  },
  {
    "id": "b2c3d4e5-f6g7-8901-bcde-f12345678901",
    "name": "Confirmação Imediata SMS",
    "isActive": true,
    "lastExecutedAt": "2025-10-10T14:30:00Z",
    "nextExecutionAt": null
  },
  {
    "id": "c3d4e5f6-g7h8-9012-cdef-123456789012",
    "name": "Lembrete de Pagamento - Segunda-feira",
    "isActive": true,
    "lastExecutedAt": "2025-10-07T09:00:00Z",
    "nextExecutionAt": "2025-10-14T09:00:00Z"
  },
  {
    "id": "d4e5f6g7-h8i9-0123-defg-234567890123",
    "name": "Pesquisa de Satisfação",
    "isActive": true,
    "lastExecutedAt": "2025-10-10T14:00:00Z",
    "nextExecutionAt": null
  }
]
```

#### Atualizar Rotina (Exemplo: Alterar Horário)

```bash
PUT /api/notificationroutines/a1b2c3d4-e5f6-7890-abcd-ef1234567890

{
  "name": "Lembrete WhatsApp 24h Antes",
  "description": "Envia lembrete automático via WhatsApp para consultas do dia seguinte",
  "channel": "WhatsApp",
  "type": "AppointmentReminder",
  "messageTemplate": "Olá {patientName}! 😊...",
  "scheduleType": "Daily",
  "scheduleConfiguration": "{\"time\":\"17:00\"}",  // Alterado de 18:00 para 17:00
  "maxRetries": 3,
  "recipientFilter": "{\"hasAppointmentNextDay\":true,\"appointmentStatus\":\"Scheduled\"}"
}
```

#### Desativar Rotina Temporariamente

```bash
POST /api/notificationroutines/c3d4e5f6-g7h8-9012-cdef-123456789012/deactivate
```

Durante feriados ou períodos de férias, você pode desativar rotinas temporariamente.

#### Reativar Rotina

```bash
POST /api/notificationroutines/c3d4e5f6-g7h8-9012-cdef-123456789012/activate
```

### 4. Casos de Uso Avançados

#### Rotina Customizada: Lembrete de Retorno

Para especialidades que requerem retorno periódico (ex: ortodontia):

```json
{
  "name": "Lembrete de Retorno - Ortodontia",
  "description": "Lembra pacientes de ortodontia a agendarem retorno mensal",
  "channel": "WhatsApp",
  "type": "General",
  "messageTemplate": "Olá {patientName}! Está na hora de agendar seu retorno de ortodontia. Entre em contato: {clinicPhone}",
  "scheduleType": "Monthly",
  "scheduleConfiguration": "{\"day\":1,\"time\":\"10:00\"}",
  "scope": "Clinic",
  "maxRetries": 3,
  "recipientFilter": "{\"hasActiveTreatment\":true,\"treatmentType\":\"Orthodontics\",\"daysSinceLastAppointment\":30}"
}
```

#### Rotina de Sistema (Admin): Manutenção Programada

Administradores podem criar rotinas que afetam todas as clínicas:

```json
{
  "name": "Aviso de Manutenção Programada",
  "description": "Notifica todas as clínicas sobre manutenção do sistema",
  "channel": "Email",
  "type": "General",
  "messageTemplate": "Prezado usuário, informamos que haverá manutenção programada do sistema no dia {maintenanceDate} das {maintenanceStartTime} às {maintenanceEndTime}. O sistema estará indisponível neste período.",
  "scheduleType": "Custom",
  "scheduleConfiguration": "{\"cronExpression\":\"0 0 12 * * 6\"}",  // Sábados ao meio-dia
  "scope": "System",
  "maxRetries": 5
}
```

**Nota**: Requer `tenantId: "system-admin"` e permissões de administrador.

### 5. Monitoramento e Métricas

#### Verificar Status de Execução

```bash
GET /api/notificationroutines/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

Informações importantes:
- `lastExecutedAt`: Última vez que a rotina foi executada
- `nextExecutionAt`: Próxima execução agendada
- `isActive`: Se a rotina está ativa

#### Dashboard de Rotinas (Exemplo Conceitual)

```
+------------------------------------------+
| Rotinas Ativas: 4                        |
| Executadas Hoje: 12                      |
| Notificações Enviadas: 156               |
| Taxa de Sucesso: 94.2%                   |
+------------------------------------------+

Próximas Execuções:
- Lembrete WhatsApp 24h Antes: Hoje às 18:00
- Lembrete de Pagamento: Segunda-feira às 09:00
- Confirmação Imediata: Sob demanda
- Pesquisa de Satisfação: Sob demanda
```

### 6. Benefícios Alcançados

1. **Redução de Faltas**: 35% de redução em no-shows após implementação
2. **Melhor Comunicação**: Pacientes sentem-se mais conectados à clínica
3. **Automação**: Economiza 10+ horas/semana da equipe administrativa
4. **Satisfação**: NPS aumentou de 7.2 para 8.9
5. **Receita**: Redução de 20% em faturas vencidas

### 7. Boas Práticas Aplicadas

✅ **Templates Personalizados**: Mensagens amigáveis com emojis
✅ **Horários Apropriados**: Notificações enviadas em horário comercial
✅ **Filtros Específicos**: Apenas pacientes relevantes recebem cada mensagem
✅ **Múltiplos Canais**: WhatsApp para lembretes, SMS para confirmações, Email para cobranças
✅ **Monitoramento**: Verificação regular das métricas de execução

---

## Próximos Passos

1. **Implementar Serviços de Envio**: Integrar com provedores de SMS/WhatsApp/Email
2. **Background Job**: Configurar job para executar `ExecuteDueRoutinesAsync()`
3. **Analytics**: Dashboard com métricas detalhadas de cada rotina
4. **A/B Testing**: Testar diferentes templates e horários
5. **Templates Pré-configurados**: Biblioteca de templates prontos para usar
