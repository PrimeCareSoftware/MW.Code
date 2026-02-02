# Sistema de Rotinas de Notificação Configuráveis

## Visão Geral

O sistema de rotinas de notificação permite criar e gerenciar envios automáticos de notificações (SMS, Email, WhatsApp) de forma personalizada e programável. Ideal para automatizar lembretes de consultas, confirmações, avisos de pagamento e muito mais.

## Características Principais

### 1. Canais de Notificação
- **SMS**: Mensagens de texto
- **WhatsApp**: Mensagens via WhatsApp Business API
- **Email**: Notificações por email
- **Push**: Notificações push (mobile/web)

### 2. Tipos de Notificação
- `AppointmentReminder`: Lembrete de consulta
- `AppointmentConfirmation`: Confirmação de agendamento
- `AppointmentCancellation`: Cancelamento de consulta
- `AppointmentRescheduled`: Reagendamento
- `PaymentReminder`: Lembrete de pagamento
- `PrescriptionReady`: Receita pronta
- `ExamResults`: Resultados de exame
- `General`: Notificação geral

### 3. Tipos de Agendamento
- **Daily**: Execução diária em horário específico
- **Weekly**: Execução em dias específicos da semana
- **Monthly**: Execução em dia específico do mês
- **Custom**: Expressão customizada (tipo cron)
- **BeforeAppointment**: X horas/dias antes da consulta
- **AfterAppointment**: X horas/dias depois da consulta

### 4. Escopo
- **Clinic**: Rotina específica da clínica (multi-tenant)
- **System**: Rotina global do sistema (apenas administradores)

## API Endpoints

### Listar todas as rotinas
```http
GET /api/notificationroutines
Authorization: Bearer {token}
X-Tenant-Id: {tenant-id}
```

**Resposta:**
```json
[
  {
    "id": "guid",
    "name": "Lembrete Diário de Consultas",
    "description": "Envia lembretes para consultas do dia seguinte",
    "channel": "WhatsApp",
    "type": "AppointmentReminder",
    "messageTemplate": "Olá {patientName}, você tem consulta amanhã às {appointmentTime}",
    "scheduleType": "Daily",
    "scheduleConfiguration": "{\"time\":\"18:00\"}",
    "scope": "Clinic",
    "isActive": true,
    "maxRetries": 3,
    "recipientFilter": "{\"hasAppointmentNextDay\":true}",
    "lastExecutedAt": "2025-10-09T18:00:00Z",
    "nextExecutionAt": "2025-10-10T18:00:00Z",
    "createdAt": "2025-10-01T10:00:00Z",
    "updatedAt": "2025-10-09T18:01:00Z",
    "tenantId": "clinic-abc"
  }
]
```

### Listar rotinas ativas
```http
GET /api/notificationroutines/active
Authorization: Bearer {token}
X-Tenant-Id: {tenant-id}
```

### Obter rotina específica
```http
GET /api/notificationroutines/{id}
Authorization: Bearer {token}
X-Tenant-Id: {tenant-id}
```

### Criar nova rotina
```http
POST /api/notificationroutines
Authorization: Bearer {token}
X-Tenant-Id: {tenant-id}
Content-Type: application/json

{
  "name": "Lembrete de Consulta WhatsApp",
  "description": "Envia lembrete via WhatsApp 24h antes da consulta",
  "channel": "WhatsApp",
  "type": "AppointmentReminder",
  "messageTemplate": "Olá {patientName}, você tem consulta amanhã às {appointmentTime} com Dr(a). {doctorName}. Confirme sua presença respondendo SIM.",
  "scheduleType": "Daily",
  "scheduleConfiguration": "{\"time\":\"18:00\"}",
  "scope": "Clinic",
  "maxRetries": 3,
  "recipientFilter": "{\"hasAppointmentNextDay\":true}"
}
```

### Atualizar rotina
```http
PUT /api/notificationroutines/{id}
Authorization: Bearer {token}
X-Tenant-Id: {tenant-id}
Content-Type: application/json

{
  "name": "Nome Atualizado",
  "description": "Descrição atualizada",
  "channel": "SMS",
  "type": "AppointmentReminder",
  "messageTemplate": "Novo template",
  "scheduleType": "Weekly",
  "scheduleConfiguration": "{\"days\":[\"monday\",\"wednesday\",\"friday\"],\"time\":\"09:00\"}",
  "maxRetries": 5,
  "recipientFilter": "{\"filter\":\"updated\"}"
}
```

### Excluir rotina
```http
DELETE /api/notificationroutines/{id}
Authorization: Bearer {token}
X-Tenant-Id: {tenant-id}
```

### Ativar rotina
```http
POST /api/notificationroutines/{id}/activate
Authorization: Bearer {token}
X-Tenant-Id: {tenant-id}
```

### Desativar rotina
```http
POST /api/notificationroutines/{id}/deactivate
Authorization: Bearer {token}
X-Tenant-Id: {tenant-id}
```

## Exemplos de Configuração de Agendamento

### Diário às 18:00
```json
{
  "scheduleType": "Daily",
  "scheduleConfiguration": "{\"time\":\"18:00\"}"
}
```

### Semanal (Segunda, Quarta e Sexta às 09:00)
```json
{
  "scheduleType": "Weekly",
  "scheduleConfiguration": "{\"days\":[\"monday\",\"wednesday\",\"friday\"],\"time\":\"09:00\"}"
}
```

### Mensal (dia 1 às 10:00)
```json
{
  "scheduleType": "Monthly",
  "scheduleConfiguration": "{\"day\":1,\"time\":\"10:00\"}"
}
```

### 24 horas antes da consulta
```json
{
  "scheduleType": "BeforeAppointment",
  "scheduleConfiguration": "{\"hours\":24}"
}
```

### 2 horas depois da consulta
```json
{
  "scheduleType": "AfterAppointment",
  "scheduleConfiguration": "{\"hours\":2}"
}
```

## Placeholders para Templates de Mensagem

Use placeholders no template de mensagem para personalização:

- `{patientName}`: Nome do paciente
- `{appointmentTime}`: Horário da consulta
- `{appointmentDate}`: Data da consulta
- `{doctorName}`: Nome do médico
- `{clinicName}`: Nome da clínica
- `{clinicPhone}`: Telefone da clínica
- `{clinicAddress}`: Endereço da clínica

**Exemplo:**
```
Olá {patientName}, você tem consulta em {appointmentDate} às {appointmentTime} 
com Dr(a). {doctorName} na {clinicName}. 
Para confirmar ou reagendar, ligue para {clinicPhone}.
```

## Filtros de Destinatários

Configure quem receberá as notificações usando JSON no campo `recipientFilter`:

### Todos os pacientes com consulta no dia seguinte
```json
{
  "hasAppointmentNextDay": true
}
```

### Pacientes com consultas específicas
```json
{
  "appointmentType": "FirstConsultation",
  "appointmentStatus": "Scheduled"
}
```

### Pacientes com pagamentos pendentes
```json
{
  "hasUnpaidInvoices": true,
  "overdueInvoices": true
}
```

### Filtro customizado
```json
{
  "ageRange": {
    "min": 18,
    "max": 65
  },
  "hasEmail": true,
  "isActive": true
}
```

## Segurança e Permissões

### Rotinas de Clínica
- Acessíveis apenas para usuários da clínica (tenant)
- Podem ser criadas pelo dono da clínica ou usuários autorizados
- Isoladas por tenant (multi-tenant)

### Rotinas do Sistema
- Apenas administradores do sistema podem criar
- Aplicam-se globalmente a todas as clínicas
- TenantId deve ser "system-admin"

## Retentativas

O sistema suporta até 10 tentativas de envio (configurável por rotina). Se uma notificação falhar:

1. Status muda para `Failed`
2. Contador de retentativas é incrementado
3. Sistema tenta reenviar automaticamente
4. Após atingir `maxRetries`, notificação é marcada como falha permanente

## Boas Práticas

### 1. Nomeie suas rotinas de forma clara
```
✅ "Lembrete WhatsApp 24h Antes - Consultas"
❌ "Rotina 1"
```

### 2. Use descrições detalhadas
```
✅ "Envia lembrete via WhatsApp 24 horas antes de consultas agendadas para pacientes que confirmaram presença"
❌ "Lembrete"
```

### 3. Configure retentativas adequadas
- SMS/WhatsApp: 3 retentativas
- Email: 5 retentativas
- Push: 2 retentativas

### 4. Teste mensagens antes de ativar
Crie a rotina como `inativa`, revise o template, depois ative.

### 5. Monitore execuções
Verifique `lastExecutedAt` e `nextExecutionAt` para garantir que a rotina está executando corretamente.

### 6. Use filtros específicos
Quanto mais específico o filtro, mais relevante a notificação para o destinatário.

## Troubleshooting

### Rotina não está executando
1. Verifique se `isActive` está `true`
2. Confirme que `nextExecutionAt` está no passado ou nulo
3. Valide a configuração do agendamento

### Mensagens não estão sendo enviadas
1. Verifique se há destinatários no filtro
2. Confirme configuração dos serviços de notificação (SMS/WhatsApp/Email)
3. Revise os logs de erro

### Notificações duplicadas
1. Verifique se não há múltiplas rotinas com mesma configuração
2. Confirme que `nextExecutionAt` está sendo atualizado corretamente

## Exemplo Completo: Sistema de Lembretes

```json
{
  "name": "Sistema Completo de Lembretes",
  "routines": [
    {
      "name": "Lembrete WhatsApp 24h Antes",
      "description": "Envia lembrete via WhatsApp 24 horas antes da consulta",
      "channel": "WhatsApp",
      "type": "AppointmentReminder",
      "messageTemplate": "Olá {patientName}! Lembrete: você tem consulta amanhã às {appointmentTime} com Dr(a). {doctorName}. Confirme sua presença respondendo SIM.",
      "scheduleType": "Daily",
      "scheduleConfiguration": "{\"time\":\"18:00\"}",
      "scope": "Clinic",
      "maxRetries": 3,
      "recipientFilter": "{\"hasAppointmentNextDay\":true}"
    },
    {
      "name": "SMS Confirmação Agendamento",
      "description": "Confirma agendamento imediatamente via SMS",
      "channel": "SMS",
      "type": "AppointmentConfirmation",
      "messageTemplate": "Consulta agendada para {appointmentDate} às {appointmentTime} na {clinicName}. Para cancelar, ligue {clinicPhone}.",
      "scheduleType": "BeforeAppointment",
      "scheduleConfiguration": "{\"hours\":0}",
      "scope": "Clinic",
      "maxRetries": 3
    },
    {
      "name": "Email Lembrete Pagamento",
      "description": "Envia lembrete de pagamento toda segunda-feira",
      "channel": "Email",
      "type": "PaymentReminder",
      "messageTemplate": "Prezado(a) {patientName}, você possui faturas pendentes. Acesse nosso portal para regularizar.",
      "scheduleType": "Weekly",
      "scheduleConfiguration": "{\"days\":[\"monday\"],\"time\":\"09:00\"}",
      "scope": "Clinic",
      "maxRetries": 5,
      "recipientFilter": "{\"hasUnpaidInvoices\":true}"
    }
  ]
}
```

## Próximos Passos

1. Implemente os serviços de notificação (SMS, WhatsApp, Email)
2. Configure as credenciais dos provedores de notificação
3. Crie um job em background para executar `ExecuteDueRoutinesAsync()`
4. Implemente monitoramento e logs de execução
5. Crie relatórios de efetividade das notificações
