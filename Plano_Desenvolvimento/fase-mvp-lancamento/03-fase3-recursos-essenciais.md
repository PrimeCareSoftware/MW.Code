# Prompt 03: Fase 3 - Recursos Essenciais (Mês 5-7)

## 📋 Contexto

A Fase 3 adiciona recursos essenciais que diferenciam o Omni Care no mercado e aumentam significativamente o valor percebido pelos clientes. Estes recursos são fortemente solicitados e têm impacto direto na retenção.

**Referência**: `MVP_IMPLEMENTATION_GUIDE.md` - Fase 3
**Status**: 📋 Planejado
**Prioridade**: P1 - Alta
**Estimativa**: 3 meses (Mês 5-7)
**Equipe**: 2-3 desenvolvedores

## 🎯 Objetivos

1. Implementar integração WhatsApp Business API
2. Implementar sistema de lembretes automáticos (Email/SMS)
3. Implementar backup automático diário
4. Implementar Dashboard Analytics básico
5. Implementar relatórios customizáveis

## 📚 Tarefas

### 1. Integração WhatsApp Business API (4 semanas)

**1.1 Setup e Configuração**

- [ ] Criar conta WhatsApp Business API
- [ ] Obter aprovação do Facebook Business
- [ ] Configurar webhook para receber mensagens
- [ ] Configurar templates de mensagens pré-aprovados pelo WhatsApp

**Templates Necessários**:
```
1. confirmation_appointment (confirmação de consulta)
2. reminder_appointment (lembrete de consulta)
3. appointment_canceled (cancelamento de consulta)
4. appointment_rescheduled (reagendamento de consulta)
5. document_ready (documento disponível para download)
6. payment_reminder (lembrete de pagamento)
```

**1.2 Implementação Backend**

```csharp
// src/API/Controllers/WhatsAppController.cs
[ApiController]
[Route("api/[controller]")]
public class WhatsAppController : ControllerBase
{
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] WhatsAppMessageRequest request)
    {
        // Enviar mensagem via WhatsApp Business API
    }
    
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] WhatsAppWebhookPayload payload)
    {
        // Processar mensagens recebidas
    }
    
    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates()
    {
        // Listar templates aprovados
    }
}
```

**Entidades**:
```csharp
public class WhatsAppMessage
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid? PatientId { get; set; }
    public string PhoneNumber { get; set; }
    public string TemplateName { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
    public string Status { get; set; } // sent, delivered, read, failed
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
```

**1.3 Implementação Frontend**

```typescript
// Componente de configuração WhatsApp
// frontend/medicwarehouse-app/src/app/pages/settings/whatsapp-config/

interface WhatsAppConfig {
  enabled: boolean;
  phoneNumberId: string;
  accessToken: string;
  verifyToken: string;
  webhookUrl: string;
  templates: WhatsAppTemplate[];
}

interface WhatsAppTemplate {
  name: string;
  category: string;
  language: string;
  status: 'approved' | 'pending' | 'rejected';
  components: TemplateComponent[];
}
```

- [ ] Página de configuração do WhatsApp
- [ ] Teste de envio de mensagem
- [ ] Visualização de histórico de mensagens
- [ ] Dashboard de métricas (enviadas, entregues, lidas)

**1.4 Integrações**
- [ ] Enviar confirmação automática ao agendar consulta
- [ ] Permitir envio manual de mensagens
- [ ] Integrar com sistema de lembretes (próxima tarefa)

### 2. Lembretes Automáticos (3 semanas)

**2.1 Sistema de Lembretes**

```csharp
// src/Core/Entities/ReminderConfiguration.cs
public class ReminderConfiguration
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public bool EmailEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public bool WhatsAppEnabled { get; set; }
    
    // Configuração de timing
    public List<int> ReminderMinutesBefore { get; set; } // Ex: [1440, 60] = 24h e 1h antes
    
    // Templates
    public string EmailTemplate { get; set; }
    public string SmsTemplate { get; set; }
    public string WhatsAppTemplate { get; set; }
}

public class ScheduledReminder
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public ReminderChannel Channel { get; set; } // Email, SMS, WhatsApp
    public DateTime ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public ReminderStatus Status { get; set; } // Pending, Sent, Failed, Canceled
}
```

**2.2 Job de Processamento**

```csharp
// src/Infrastructure/BackgroundJobs/ReminderProcessorJob.cs
public class ReminderProcessorJob : IHostedService
{
    // Rodar a cada 5 minutos
    // 1. Buscar lembretes pendentes
    // 2. Verificar se chegou a hora de enviar
    // 3. Enviar via canal configurado (Email/SMS/WhatsApp)
    // 4. Atualizar status
    // 5. Registrar log
}
```

**2.3 Implementação de Canais**

**Email**:
- [ ] Integrar com SendGrid ou similar
- [ ] Template HTML responsivo
- [ ] Link para confirmar/cancelar consulta
- [ ] Adicionar ao calendário (iCal attachment)

**SMS**:
- [ ] Integrar com Twilio ou similar
- [ ] Template curto (max 160 caracteres)
- [ ] Link curto para confirmar/cancelar

**WhatsApp**:
- [ ] Usar integração da tarefa anterior
- [ ] Template pré-aprovado
- [ ] Botões de ação (confirmar/cancelar)

**2.4 Frontend**

```typescript
// frontend/medicwarehouse-app/src/app/pages/settings/reminders/

interface ReminderSettings {
  enabled: boolean;
  channels: {
    email: boolean;
    sms: boolean;
    whatsapp: boolean;
  };
  timings: number[]; // minutos antes da consulta
  templates: {
    email: string;
    sms: string;
    whatsapp: string;
  };
}
```

- [ ] Página de configuração de lembretes
- [ ] Preview de templates
- [ ] Editor de templates com variáveis dinâmicas
- [ ] Teste de envio

### 3. Backup Automático Diário (1 semana)

**3.1 Estratégia de Backup**

```yaml
# Backup Strategy
frequency: daily
time: 02:00 AM (local timezone)
retention:
  daily: 7 days
  weekly: 4 weeks
  monthly: 12 months
storage:
  primary: Azure Blob Storage
  secondary: AWS S3 (disaster recovery)
encryption: AES-256
```

**3.2 Implementação**

```csharp
// src/Infrastructure/BackgroundJobs/DatabaseBackupJob.cs
public class DatabaseBackupJob : IHostedService
{
    public async Task BackupDatabase()
    {
        // 1. Criar dump do PostgreSQL
        var dumpFile = await CreateDatabaseDump();
        
        // 2. Comprimir (gzip)
        var compressedFile = await CompressFile(dumpFile);
        
        // 3. Criptografar
        var encryptedFile = await EncryptFile(compressedFile);
        
        // 4. Upload para Azure Blob Storage
        await UploadToAzure(encryptedFile);
        
        // 5. Upload para AWS S3 (redundância)
        await UploadToS3(encryptedFile);
        
        // 6. Limpar arquivos temporários
        CleanupTempFiles();
        
        // 7. Aplicar política de retenção
        await ApplyRetentionPolicy();
        
        // 8. Notificar admins
        await NotifyAdmins("Backup completed successfully");
    }
}
```

**3.3 Testes de Recuperação**

- [ ] Documentar processo de restore
- [ ] Testar restore mensalmente
- [ ] Medir tempo de recuperação (RTO)
- [ ] Validar integridade dos dados restaurados

**3.4 Monitoramento**

- [ ] Dashboard de status de backups
- [ ] Alertas se backup falhar
- [ ] Métricas: tamanho do backup, tempo de execução
- [ ] Logs detalhados de cada backup

### 4. Dashboard Analytics Básico (3 semanas)

**4.1 Métricas Principais**

```typescript
// frontend/medicwarehouse-app/src/app/pages/analytics/dashboard/

interface AnalyticsDashboard {
  period: 'day' | 'week' | 'month' | 'year';
  
  appointments: {
    total: number;
    completed: number;
    canceled: number;
    noShow: number;
    bySpecialty: Record<string, number>;
    byProfessional: Record<string, number>;
    trend: TrendData[];
  };
  
  patients: {
    total: number;
    new: number;
    returning: number;
    byAgeGroup: Record<string, number>;
    byGender: Record<string, number>;
    trend: TrendData[];
  };
  
  revenue: {
    total: number;
    paid: number;
    pending: number;
    overdue: number;
    byPaymentMethod: Record<string, number>;
    trend: TrendData[];
  };
  
  operations: {
    avgWaitTime: number; // minutos
    avgAppointmentDuration: number; // minutos
    occupancyRate: number; // % da agenda ocupada
    utilizationRate: number; // % do tempo útil usado
  };
}
```

**4.2 Visualizações**

- [ ] Cards com métricas principais (KPIs)
- [ ] Gráficos de linha (tendências ao longo do tempo)
- [ ] Gráficos de barra (comparações)
- [ ] Gráficos de pizza (distribuições)
- [ ] Tabelas de ranking (top profissionais, especialidades, etc)

**4.3 Filtros**

- [ ] Período (dia, semana, mês, ano, customizado)
- [ ] Profissional
- [ ] Especialidade
- [ ] Convênio
- [ ] Comparação com período anterior

**4.4 Exportação**

- [ ] Exportar para PDF
- [ ] Exportar para Excel
- [ ] Enviar por email
- [ ] Agendar relatórios automáticos

### 5. Relatórios Customizáveis (3 semanas)

**5.1 Report Builder**

```typescript
// frontend/medicwarehouse-app/src/app/pages/reports/builder/

interface ReportDefinition {
  id: string;
  name: string;
  description: string;
  
  dataSource: 'appointments' | 'patients' | 'revenue' | 'procedures';
  
  columns: ReportColumn[];
  filters: ReportFilter[];
  groupBy: string[];
  orderBy: OrderBy[];
  
  chartType?: 'line' | 'bar' | 'pie' | 'table';
  
  schedule?: {
    frequency: 'daily' | 'weekly' | 'monthly';
    time: string;
    recipients: string[];
  };
}

interface ReportColumn {
  field: string;
  label: string;
  aggregation?: 'sum' | 'avg' | 'count' | 'min' | 'max';
  format?: 'currency' | 'date' | 'number' | 'percentage';
}

interface ReportFilter {
  field: string;
  operator: 'equals' | 'contains' | 'greaterThan' | 'lessThan' | 'between';
  value: any;
}
```

**5.2 Relatórios Pré-configurados**

- [ ] **Relatório de Agendamentos**: Todas as consultas por período
- [ ] **Relatório de Receita**: Financeiro detalhado
- [ ] **Relatório de Pacientes**: Novos pacientes, retornos
- [ ] **Relatório de Produtividade**: Por profissional
- [ ] **Relatório de Convênios**: Atendimentos por convênio
- [ ] **Relatório de Procedimentos**: Procedimentos realizados
- [ ] **Relatório de Faturamento TISS**: Para envio a operadoras

**5.3 Interface do Builder**

- [ ] Drag-and-drop para adicionar colunas
- [ ] Filtros visuais (não precisa SQL)
- [ ] Preview em tempo real
- [ ] Salvar relatório customizado
- [ ] Compartilhar relatório com equipe

**5.4 Backend**

```csharp
// src/API/Controllers/ReportsController.cs
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpPost("build")]
    public async Task<IActionResult> BuildReport([FromBody] ReportDefinition definition)
    {
        // Gerar SQL dinamicamente baseado na definição
        // Executar query
        // Retornar resultados
    }
    
    [HttpPost("schedule")]
    public async Task<IActionResult> ScheduleReport([FromBody] ReportSchedule schedule)
    {
        // Agendar job para gerar e enviar relatório
    }
    
    [HttpGet("export/{reportId}")]
    public async Task<IActionResult> ExportReport(Guid reportId, [FromQuery] string format)
    {
        // Exportar relatório em PDF, Excel, CSV
    }
}
```

## ✅ Critérios de Sucesso

### WhatsApp Integration
- [ ] Integração funcionando com WhatsApp Business API
- [ ] Pelo menos 5 templates aprovados
- [ ] Taxa de entrega > 95%
- [ ] Webhook processando mensagens recebidas

### Lembretes Automáticos
- [ ] Lembretes sendo enviados automaticamente
- [ ] Suporte para Email, SMS e WhatsApp
- [ ] Taxa de entrega > 90%
- [ ] Redução de no-shows em pelo menos 20%

### Backup Automático
- [ ] Backup rodando diariamente sem falhas
- [ ] Política de retenção funcionando corretamente
- [ ] Teste de restore bem-sucedido
- [ ] Tempo de backup < 30 min

### Dashboard Analytics
- [ ] Dashboard carregando em < 3s
- [ ] Todas as métricas principais implementadas
- [ ] Filtros funcionando corretamente
- [ ] Exportação para PDF/Excel funcionando

### Relatórios Customizáveis
- [ ] Report builder intuitivo e funcional
- [ ] Pelo menos 7 relatórios pré-configurados
- [ ] Usuário consegue criar relatório customizado em < 5 min
- [ ] Relatórios agendados funcionando

## 📊 Métricas a Monitorar

### WhatsApp
- **Taxa de Entrega**: Meta > 95%
- **Taxa de Leitura**: Meta > 70%
- **Mensagens/Mês**: Baseline
- **Custo por Mensagem**: Monitorar

### Lembretes
- **Redução de No-Shows**: Meta -20%
- **Taxa de Confirmação**: Meta > 60%
- **Taxa de Entrega**: Meta > 90%

### Backup
- **Taxa de Sucesso**: Meta 100%
- **Tempo de Backup**: Meta < 30 min
- **RTO (Recovery Time)**: Meta < 4h

### Analytics
- **Tempo de Carregamento**: Meta < 3s
- **Taxa de Uso**: Meta > 50% dos usuários
- **Relatórios Exportados/Mês**: Baseline

## 🔗 Dependências

### Pré-requisitos
- Prompt 02: Fase 2 - Validação completo
- Sistema de agendamento estável
- Sistema de notificações básico

### Bloqueia
- Prompt 04: Fase 4 - Recursos Avançados

## 📂 Arquivos Principais

```
src/
├── API/Controllers/
│   ├── WhatsAppController.cs (criar)
│   ├── RemindersController.cs (criar)
│   ├── BackupsController.cs (criar)
│   ├── AnalyticsController.cs (criar)
│   └── ReportsController.cs (criar)
├── Core/Entities/
│   ├── WhatsAppMessage.cs (criar)
│   ├── ReminderConfiguration.cs (criar)
│   └── ReportDefinition.cs (criar)
└── Infrastructure/BackgroundJobs/
    ├── ReminderProcessorJob.cs (criar)
    └── DatabaseBackupJob.cs (criar)

frontend/medicwarehouse-app/src/app/
├── pages/
│   ├── settings/
│   │   ├── whatsapp-config/
│   │   └── reminders/
│   ├── analytics/
│   │   └── dashboard/
│   └── reports/
│       ├── builder/
│       └── library/
└── services/
    ├── whatsapp.service.ts (criar)
    ├── reminders.service.ts (criar)
    └── reports.service.ts (criar)
```

## 🔐 Segurança

- [ ] Criptografar tokens do WhatsApp no banco
- [ ] Backups criptografados com AES-256
- [ ] Logs de acesso aos relatórios
- [ ] Permissões para criar/editar relatórios
- [ ] Rate limiting em APIs de envio de mensagens

## 📝 Notas

- **Custos**: WhatsApp e SMS têm custo por mensagem, monitorar usage
- **Créditos Early Adopter**: R$ 100 em créditos para SMS/WhatsApp
- **Compliance**: Obter consentimento do paciente antes de enviar mensagens
- **Backups**: Testar restore regularmente, não apenas confiar que funciona

## 🚀 Próximos Passos

Após concluir este prompt:
1. Iniciar Prompt 04: Fase 4 - Recursos Avançados (Mês 8-10)
2. Monitorar redução de no-shows
3. Coletar feedback sobre analytics e relatórios
4. Otimizar custos de mensagens
