# Sistema de Alertas - Documentação Técnica

## Visão Geral

O sistema de alertas foi implementado para facilitar o uso diário do sistema médico, baseado em análise de ferramentas de mercado líderes (Doctolib, ZocDoc, 1Doc). 

## Arquitetura

### Camadas

```
┌─────────────────────────────────────────┐
│   Frontend (Angular)                     │
│   - AlertCenter Component                │
│   - SignalR Client                       │
└─────────────┬───────────────────────────┘
              │ SignalR WebSocket
┌─────────────▼───────────────────────────┐
│   API Layer                              │
│   - AlertsController (REST)              │
│   - AlertHub (SignalR)                   │
└─────────────┬───────────────────────────┘
              │
┌─────────────▼───────────────────────────┐
│   Application Layer                      │
│   - AlertService                         │
│   - AlertProcessingJob (Hangfire)        │
└─────────────┬───────────────────────────┘
              │
┌─────────────▼───────────────────────────┐
│   Domain Layer                           │
│   - Alert Entity                         │
│   - AlertConfiguration Entity            │
│   - AlertEnums                           │
└─────────────┬───────────────────────────┘
              │
┌─────────────▼───────────────────────────┐
│   Database (PostgreSQL)                  │
│   - Alerts Table                         │
│   - AlertConfigurations Table            │
└──────────────────────────────────────────┘
```

## Entidades

### Alert

Entidade principal que representa um alerta no sistema.

**Propriedades:**
- `Category`: AlertCategory - Tipo do alerta (30+ categorias)
- `Priority`: AlertPriority - Prioridade (Low, Normal, High, Critical)
- `Status`: AlertStatus - Estado atual (Active, Acknowledged, Resolved, Dismissed, Expired)
- `Title`: string - Título do alerta
- `Message`: string - Mensagem detalhada
- `ActionUrl`: string? - URL para ação relacionada
- `SuggestedAction`: AlertAction - Ação sugerida
- `RecipientType`: AlertRecipientType - Tipo de destinatário
- `UserId`, `Role`, `ClinicId`: Destinatário específico
- `RelatedEntityType`, `RelatedEntityId`: Entidade relacionada
- `AcknowledgedAt`, `AcknowledgedBy`: Tracking de reconhecimento
- `ResolvedAt`, `ResolvedBy`, `ResolutionNotes`: Tracking de resolução
- `ExpiresAt`: Data/hora de expiração
- `DeliveryChannels`: Lista de canais de entrega

**Métodos:**
- `Acknowledge(userId)`: Marca alerta como reconhecido
- `Resolve(userId, notes)`: Marca alerta como resolvido
- `Dismiss(userId)`: Dispensa o alerta
- `IsExpired()`: Verifica se expirou
- `RequiresUrgentAction()`: Verifica se requer ação urgente

### AlertConfiguration

Configuração de regras para geração automática de alertas.

**Propriedades:**
- `Name`, `Description`: Identificação
- `Category`, `Priority`: Tipo e prioridade do alerta
- `IsEnabled`: Se a regra está ativa
- `TriggerType`, `TriggerConditions`: Gatilho e condições (JSON)
- `TitleTemplate`, `MessageTemplate`: Templates de mensagem
- `RecipientType`, `RecipientFilter`: Destinatários
- `ExpirationHours`: Horas até expiração
- `AlertsGeneratedCount`, `LastTriggeredAt`: Estatísticas

## Categorias de Alertas

### Agendamento (1-9)
- `AppointmentConflict`: Conflito de horários
- `AppointmentNoShow`: Paciente não compareceu
- `AppointmentConfirmationPending`: Confirmação pendente
- `AppointmentOverdue`: Consulta atrasada

### Financeiro (10-15)
- `PaymentOverdue`: Pagamento vencido
- `PaymentDueSoon`: Pagamento a vencer
- `InvoicePending`: Fatura pendente
- `LowCashFlow`: Fluxo de caixa baixo
- `SubscriptionExpiring`: Assinatura expirando
- `SubscriptionExpired`: Assinatura expirada

### Compliance (20-24)
- `DocumentExpiring`: Documento vencendo
- `DocumentExpired`: Documento vencido
- `MissingSignature`: Falta assinatura digital
- `ComplianceViolation`: Violação de compliance
- `AuditRequired`: Auditoria necessária

### Estoque (30-33)
- `LowStock`: Estoque baixo
- `OutOfStock`: Sem estoque
- `MaterialExpiring`: Material vencendo
- `MaterialExpired`: Material vencido

### Clínicos (40-44)
- `PrescriptionDueRenewal`: Receita precisa renovação
- `ExamResultReady`: Resultado de exame pronto
- `LabResultAbnormal`: Resultado anormal
- `VaccineReminder`: Lembrete de vacina
- `FollowUpRequired`: Retorno necessário

### Operacionais (50-54)
- `SystemMaintenance`: Manutenção do sistema
- `BackupFailed`: Falha no backup
- `HighServerLoad`: Alta carga no servidor
- `IntegrationError`: Erro de integração
- `SecurityAlert`: Alerta de segurança

## API Endpoints

### REST API

**Base URL:** `/api/alerts`

#### Criar Alerta
```http
POST /api/alerts
Content-Type: application/json
Authorization: Bearer {token}

{
  "category": "AppointmentOverdue",
  "priority": "High",
  "title": "Consulta Atrasada",
  "message": "Consulta com João Silva está 15 minutos atrasada",
  "recipientType": "User",
  "userId": "guid",
  "actionUrl": "/appointments/123",
  "suggestedAction": "ViewDetails"
}
```

#### Obter Alertas do Usuário
```http
GET /api/alerts/my-alerts
Authorization: Bearer {token}
```

#### Obter Contagem de Alertas
```http
GET /api/alerts/my-alerts/count
Authorization: Bearer {token}
```

#### Alertas Críticos
```http
GET /api/alerts/critical
Authorization: Bearer {token}
```

#### Estatísticas
```http
GET /api/alerts/statistics
Authorization: Bearer {token}
```

#### Reconhecer Alerta
```http
POST /api/alerts/{id}/acknowledge
Authorization: Bearer {token}
```

#### Resolver Alerta
```http
POST /api/alerts/{id}/resolve
Content-Type: application/json
Authorization: Bearer {token}

{
  "alertId": "guid",
  "notes": "Problema resolvido com sucesso"
}
```

#### Dispensar Alerta
```http
POST /api/alerts/{id}/dismiss
Authorization: Bearer {token}
```

### SignalR Hub

**Endpoint:** `/hubs/alerts`

#### Eventos do Cliente → Servidor
- `AcknowledgeAlert(alertId)`: Reconhece um alerta

#### Eventos do Servidor → Cliente
- `ReceiveAlert(alert)`: Recebe novo alerta
- `ReceiveCriticalAlert(alert)`: Recebe alerta crítico
- `AlertAcknowledged(alertId)`: Alerta foi reconhecido (sincroniza entre dispositivos)

#### Grupos
- `tenant_{tenantId}`: Todos os usuários de um tenant
- `clinic_{clinicId}`: Todos os usuários de uma clínica

## Jobs Automáticos (Hangfire)

### 1. Marcar Alertas Expirados
- **Frequência:** A cada hora
- **Função:** Marca alertas com ExpiresAt vencido como Expired

### 2. Limpar Alertas Antigos
- **Frequência:** Diariamente às 03:00 UTC
- **Função:** Remove alertas resolvidos/dispensados com mais de 90 dias

### 3. Verificar Consultas Atrasadas
- **Frequência:** A cada 15 minutos
- **Função:** Gera alertas para consultas com mais de 15 minutos de atraso

### 4. Verificar Pagamentos Vencidos
- **Frequência:** Diariamente às 08:00 UTC
- **Função:** Gera alertas para pagamentos vencidos (AccountsReceivable)

### 5. Verificar Estoque Baixo
- **Frequência:** Diariamente às 07:00 UTC
- **Função:** Gera alertas para materiais com estoque abaixo do mínimo

### 6. Verificar Assinaturas Expirando
- **Frequência:** Diariamente às 06:00 UTC
- **Função:** Gera alertas para assinaturas que expiram em 7 dias ou menos

## Índices do Banco de Dados

Para performance otimizada, foram criados os seguintes índices:

1. `IX_Alerts_UserId`
2. `IX_Alerts_Status`
3. `IX_Alerts_Priority`
4. `IX_Alerts_Category`
5. `IX_Alerts_TenantId`
6. `IX_Alerts_CreatedAt`
7. `IX_Alerts_ExpiresAt`
8. `IX_Alerts_RecipientType_ClinicId` (composto)
9. `IX_AlertConfigurations_TenantId`
10. `IX_AlertConfigurations_Category`
11. `IX_AlertConfigurations_IsEnabled`

## Segurança

### Isolamento Multi-Tenant
- Todos os alertas são isolados por `TenantId`
- Queries sempre filtram por tenant do usuário autenticado
- SignalR usa grupos por tenant/clínica

### Autorização
- Endpoints REST requerem autenticação (`[Authorize]`)
- AlertHub requer autenticação
- Usuários só veem alertas destinados a eles ou sua clínica

### Validação
- Validação de entrada em DTOs
- Validação de domínio nas entidades
- Tratamento de erros em jobs com retry automático

## Melhorias Futuras

### Fase 2 - Frontend
- [ ] Componente AlertCenter no Angular
- [ ] Notificações visuais e sonoras
- [ ] Painel de alertas com filtros
- [ ] Integração SignalR no frontend

### Fase 3 - Extensões
- [ ] Preferências de notificação por usuário
- [ ] Configuração de alertas por clínica
- [ ] Templates customizáveis de mensagens
- [ ] Relatórios de alertas
- [ ] Métricas de performance

### Fase 4 - Integrações
- [ ] Envio de alertas por SMS (Twilio)
- [ ] Envio de alertas por Email (SendGrid)
- [ ] Envio de alertas por WhatsApp
- [ ] Push notifications para PWA

## Exemplos de Uso

### Criar Alerta Programaticamente

```csharp
var alert = await _alertService.CreateAlertAsync(new CreateAlertDto
{
    Category = AlertCategory.PaymentOverdue,
    Priority = AlertPriority.High,
    Title = "Pagamento Vencido",
    Message = "Pagamento de R$ 500,00 venceu há 5 dias",
    RecipientType = AlertRecipientType.Clinic,
    ClinicId = clinicId,
    ActionUrl = "/financial/receivables/123",
    SuggestedAction = AlertAction.Contact,
    ActionLabel = "Contatar Paciente",
    ExpiresAt = DateTime.UtcNow.AddDays(7)
}, tenantId);

// Enviar via SignalR
await _alertHub.Clients.Group($"clinic_{clinicId}")
    .SendAsync("ReceiveAlert", alert);
```

### Integração Frontend (TypeScript/Angular)

```typescript
import * as signalR from '@microsoft/signalr';

// Conectar ao hub
const connection = new signalR.HubConnectionBuilder()
  .withUrl('/hubs/alerts')
  .withAutomaticReconnect()
  .build();

// Ouvir alertas
connection.on('ReceiveAlert', (alert: AlertDto) => {
  console.log('Novo alerta:', alert);
  this.showNotification(alert);
});

// Ouvir alertas críticos
connection.on('ReceiveCriticalAlert', (alert: AlertDto) => {
  console.log('Alerta CRÍTICO:', alert);
  this.showCriticalNotification(alert);
});

// Iniciar conexão
await connection.start();
```

## Troubleshooting

### Alertas não aparecem
1. Verificar se o job Hangfire está rodando
2. Verificar se os índices do banco foram criados
3. Verificar logs do AlertProcessingJob
4. Verificar conexão SignalR no frontend

### Performance lenta
1. Verificar índices do banco de dados
2. Verificar se queries estão usando índices (EXPLAIN ANALYZE)
3. Considerar aumentar cache de alertas ativos
4. Verificar volume de alertas por tenant

### Jobs não executam
1. Verificar Hangfire dashboard em `/hangfire`
2. Verificar se jobs estão registrados
3. Verificar logs de erro
4. Verificar permissões do banco de dados

## Referências

- [SignalR Documentation](https://docs.microsoft.com/aspnet/core/signalr)
- [Hangfire Documentation](https://docs.hangfire.io)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)

## Autores

- Sistema implementado em Fevereiro 2026
- Baseado em análise de mercado de sistemas similares
