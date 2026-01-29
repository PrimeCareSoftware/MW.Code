# 📊 Fase 4: Automação e Workflows - System Admin

**Prioridade:** 🔥🔥 P1 - ALTA  
**Status:** ✅ Backend Implementado | ✅ Frontend Implementado  
**Atualizado:** 29 de Janeiro de 2026  
**Esforço:** 2 meses | 2-3 desenvolvedores  
**Custo Estimado:** R$ 78.000  
**Prazo:** Q3 2026

---

## 📋 Contexto

### Situação Atual

O backend da Fase 4 foi implementado com sucesso!

**✅ Funcionalidades Implementadas (Backend):**
- ✅ Sistema de workflows event-driven operacional
- ✅ Engine de workflows com triggers e ações configuráveis
- ✅ Smart actions para tarefas administrativas (7 ações)
- ✅ Sistema de webhooks com retry exponencial
- ✅ Background jobs automatizados (Hangfire)
- ✅ Audit logging completo
- ✅ Integrações preparadas (Stripe, SendGrid, Twilio, Slack)

**✅ Funcionalidades Implementadas (Frontend):**
- ✅ Editor visual de workflows (Angular)
- ✅ Interface de gerenciamento de webhooks
- ✅ Diálogos de smart actions
- ✅ Dashboard de monitoramento de execuções

### Objetivo da Fase 4

Automatizar tarefas administrativas com:
1. Sistema de workflows event-driven
2. Smart actions contextuais
3. Integrações e webhooks
4. Background jobs automatizados

**Inspiração:** Retool, Zapier, Zendesk Automations

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais
1. Criar engine de workflows com triggers e ações
2. Implementar smart actions contextuais (impersonation, créditos, etc)
3. Adicionar sistema de webhooks (outbound/inbound)
4. Integrar com ferramentas externas (Stripe, SendGrid, Twilio, Slack)

### Benefícios Esperados
- ⚡ **-70% tempo** em tarefas repetitivas
- 🎯 **Gestão proativa** - prevenção de churn automática
- 🔗 **Integração completa** com stack de ferramentas
- 🤖 **Escalabilidade** - workflows sem intervenção manual

---

## 📝 Tarefas Detalhadas

### 1. Sistema de Workflows (4 semanas)

#### 1.1 Backend - Workflow Engine

**Entidades:**
```csharp
// Entities/Workflow.cs
public class Workflow
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsEnabled { get; set; }
    public string TriggerType { get; set; } // time, event
    public string TriggerConfig { get; set; } // JSON
    public List<WorkflowAction> Actions { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
}

// Entities/WorkflowAction.cs
public class WorkflowAction
{
    public int Id { get; set; }
    public int WorkflowId { get; set; }
    public Workflow Workflow { get; set; }
    
    public int Order { get; set; }
    public string ActionType { get; set; } // email, sms, webhook, tag, ticket
    public string Config { get; set; } // JSON
    public string Condition { get; set; } // JSON (if/else logic)
    public int? DelaySeconds { get; set; } // opcional
}

// Entities/WorkflowExecution.cs
public class WorkflowExecution
{
    public int Id { get; set; }
    public int WorkflowId { get; set; }
    public Workflow Workflow { get; set; }
    
    public string Status { get; set; } // pending, running, completed, failed
    public string TriggerData { get; set; } // JSON - dados que dispararam
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Error { get; set; }
    public List<WorkflowActionExecution> ActionExecutions { get; set; }
}
```

**Workflow Engine:**
```csharp
// Services/Workflows/WorkflowEngine.cs
public interface IWorkflowEngine
{
    Task<WorkflowExecution> ExecuteWorkflow(int workflowId, object triggerData);
    Task RegisterTrigger(string triggerType, Func<object, Task> callback);
    Task<List<Workflow>> GetWorkflowsByTrigger(string triggerType);
}

public class WorkflowEngine : IWorkflowEngine
{
    private readonly Dictionary<string, List<Func<object, Task>>> _triggers = new();
    
    public async Task<WorkflowExecution> ExecuteWorkflow(int workflowId, object triggerData)
    {
        var workflow = await _context.Workflows
            .Include(w => w.Actions.OrderBy(a => a.Order))
            .FirstOrDefaultAsync(w => w.Id == workflowId);
            
        if (workflow == null || !workflow.IsEnabled)
            return null;
            
        var execution = new WorkflowExecution
        {
            WorkflowId = workflowId,
            Status = "running",
            StartedAt = DateTime.UtcNow,
            TriggerData = JsonSerializer.Serialize(triggerData),
            ActionExecutions = new List<WorkflowActionExecution>()
        };
        
        _context.WorkflowExecutions.Add(execution);
        await _context.SaveChangesAsync();
        
        try
        {
            foreach (var action in workflow.Actions)
            {
                // Verificar condições (if/else)
                if (!await EvaluateCondition(action.Condition, triggerData, execution))
                    continue;
                    
                // Delay se configurado
                if (action.DelaySeconds.HasValue && action.DelaySeconds > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(action.DelaySeconds.Value));
                }
                
                var actionExecution = new WorkflowActionExecution
                {
                    WorkflowActionId = action.Id,
                    StartedAt = DateTime.UtcNow,
                    Status = "running"
                };
                
                execution.ActionExecutions.Add(actionExecution);
                await _context.SaveChangesAsync();
                
                try
                {
                    await ExecuteAction(action, triggerData);
                    
                    actionExecution.Status = "completed";
                    actionExecution.CompletedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    actionExecution.Status = "failed";
                    actionExecution.Error = ex.Message;
                    actionExecution.CompletedAt = DateTime.UtcNow;
                    
                    // Se uma ação falhar, continuar ou parar?
                    // Configurável no workflow
                    if (workflow.StopOnError)
                        throw;
                }
                
                await _context.SaveChangesAsync();
            }
            
            execution.Status = "completed";
            execution.CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            execution.Status = "failed";
            execution.Error = ex.Message;
            execution.CompletedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        return execution;
    }
    
    private async Task ExecuteAction(WorkflowAction action, object triggerData)
    {
        var config = JsonSerializer.Deserialize<Dictionary<string, object>>(action.Config);
        
        switch (action.ActionType)
        {
            case "send_email":
                await ExecuteSendEmailAction(config, triggerData);
                break;
            case "send_sms":
                await ExecuteSendSmsAction(config, triggerData);
                break;
            case "create_notification":
                await ExecuteCreateNotificationAction(config, triggerData);
                break;
            case "add_tag":
                await ExecuteAddTagAction(config, triggerData);
                break;
            case "create_ticket":
                await ExecuteCreateTicketAction(config, triggerData);
                break;
            case "webhook":
                await ExecuteWebhookAction(config, triggerData);
                break;
            default:
                throw new NotSupportedException($"Action type '{action.ActionType}' not supported");
        }
    }
    
    private async Task ExecuteSendEmailAction(Dictionary<string, object> config, object triggerData)
    {
        var to = ReplaceVariables(config["to"].ToString(), triggerData);
        var subject = ReplaceVariables(config["subject"].ToString(), triggerData);
        var body = ReplaceVariables(config["body"].ToString(), triggerData);
        
        await _emailService.SendEmailAsync(to, subject, body);
    }
    
    private string ReplaceVariables(string template, object data)
    {
        // Substituir variáveis {{ clinic.name }}, {{ user.email }}, etc
        var json = JsonSerializer.Serialize(data);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        
        foreach (var kvp in dict)
        {
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value?.ToString() ?? "");
        }
        
        return template;
    }
    
    private async Task<bool> EvaluateCondition(string conditionJson, object triggerData, WorkflowExecution execution)
    {
        if (string.IsNullOrEmpty(conditionJson))
            return true;
            
        var condition = JsonSerializer.Deserialize<ConditionDto>(conditionJson);
        
        // Avaliar condição simples
        // Exemplo: { "field": "clinic.mrr", "operator": ">=", "value": 1000 }
        var value = GetFieldValue(condition.Field, triggerData);
        
        return condition.Operator switch
        {
            "==" => value?.ToString() == condition.Value?.ToString(),
            "!=" => value?.ToString() != condition.Value?.ToString(),
            ">" => decimal.Parse(value?.ToString() ?? "0") > decimal.Parse(condition.Value?.ToString() ?? "0"),
            ">=" => decimal.Parse(value?.ToString() ?? "0") >= decimal.Parse(condition.Value?.ToString() ?? "0"),
            "<" => decimal.Parse(value?.ToString() ?? "0") < decimal.Parse(condition.Value?.ToString() ?? "0"),
            "<=" => decimal.Parse(value?.ToString() ?? "0") <= decimal.Parse(condition.Value?.ToString() ?? "0"),
            "contains" => value?.ToString()?.Contains(condition.Value?.ToString() ?? "") ?? false,
            _ => true
        };
    }
}
```

**Triggers de Eventos:**
```csharp
// Events/ClinicEvents.cs
public class ClinicCreatedEvent
{
    public int ClinicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SubscriptionExpiredEvent
{
    public int ClinicId { get; set; }
    public int SubscriptionId { get; set; }
    public DateTime ExpiredAt { get; set; }
}

public class InactivityDetectedEvent
{
    public int ClinicId { get; set; }
    public int DaysSinceLastActivity { get; set; }
}

// Services/Events/EventPublisher.cs
public interface IEventPublisher
{
    Task PublishAsync<T>(T eventData) where T : class;
}

public class EventPublisher : IEventPublisher
{
    private readonly IWorkflowEngine _workflowEngine;
    
    public async Task PublishAsync<T>(T eventData) where T : class
    {
        var eventType = typeof(T).Name;
        
        // Buscar workflows com este trigger
        var workflows = await _workflowEngine.GetWorkflowsByTrigger(eventType);
        
        // Executar cada workflow em background
        foreach (var workflow in workflows)
        {
            BackgroundJob.Enqueue(() => 
                _workflowEngine.ExecuteWorkflow(workflow.Id, eventData));
        }
    }
}

// Usar em Services
public class ClinicService
{
    public async Task<Clinic> CreateClinic(CreateClinicDto dto)
    {
        var clinic = new Clinic { /* ... */ };
        await _context.Clinics.AddAsync(clinic);
        await _context.SaveChangesAsync();
        
        // Publicar evento
        await _eventPublisher.PublishAsync(new ClinicCreatedEvent
        {
            ClinicId = clinic.Id,
            Name = clinic.Name,
            Email = clinic.Email,
            CreatedAt = clinic.CreatedAt
        });
        
        return clinic;
    }
}
```

**Background Jobs para Triggers Temporais:**
```csharp
// Jobs/WorkflowJobs.cs
public class WorkflowJobs
{
    // Executar a cada hora
    [AutomaticRetry(Attempts = 3)]
    public async Task CheckSubscriptionExpirations()
    {
        var expiredSubscriptions = await _context.Subscriptions
            .Where(s => 
                s.Status == "Active" && 
                s.ExpiresAt <= DateTime.UtcNow &&
                s.ExpiresAt > DateTime.UtcNow.AddHours(-1)) // apenas última hora
            .ToListAsync();
            
        foreach (var subscription in expiredSubscriptions)
        {
            await _eventPublisher.PublishAsync(new SubscriptionExpiredEvent
            {
                ClinicId = subscription.ClinicId,
                SubscriptionId = subscription.Id,
                ExpiredAt = subscription.ExpiresAt
            });
        }
    }
    
    // Executar diariamente
    public async Task CheckTrialExpiring()
    {
        var expiringTrials = await _context.Subscriptions
            .Where(s => 
                s.Status == "Trial" && 
                s.TrialEndsAt <= DateTime.UtcNow.AddDays(3) &&
                s.TrialEndsAt > DateTime.UtcNow.AddDays(2))
            .ToListAsync();
            
        foreach (var trial in expiringTrials)
        {
            await _eventPublisher.PublishAsync(new TrialExpiringEvent
            {
                ClinicId = trial.ClinicId,
                DaysRemaining = (trial.TrialEndsAt - DateTime.UtcNow).Days
            });
        }
    }
    
    public async Task CheckInactiveClients()
    {
        var inactiveClinics = await _context.Clinics
            .Where(c => 
                c.IsActive &&
                c.LastActivityAt < DateTime.UtcNow.AddDays(-30) &&
                c.LastActivityAt >= DateTime.UtcNow.AddDays(-31))
            .ToListAsync();
            
        foreach (var clinic in inactiveClinics)
        {
            await _eventPublisher.PublishAsync(new InactivityDetectedEvent
            {
                ClinicId = clinic.Id,
                DaysSinceLastActivity = (DateTime.UtcNow - clinic.LastActivityAt).Days
            });
        }
    }
}

// Configurar jobs
RecurringJob.AddOrUpdate<WorkflowJobs>(
    "check-subscriptions",
    x => x.CheckSubscriptionExpirations(),
    Cron.Hourly);

RecurringJob.AddOrUpdate<WorkflowJobs>(
    "check-trials",
    x => x.CheckTrialExpiring(),
    Cron.Daily);

RecurringJob.AddOrUpdate<WorkflowJobs>(
    "check-inactive",
    x => x.CheckInactiveClients(),
    Cron.Daily);
```

**Templates de Workflows Prontos:**
```csharp
// Data/WorkflowTemplateSeeder.cs
public class WorkflowTemplateSeeder
{
    public static void Seed(AppDbContext context)
    {
        // Workflow 1: Onboarding Automático
        var onboarding = new Workflow
        {
            Name = "Onboarding Automático",
            Description = "Sequência de boas-vindas para novas clínicas",
            IsEnabled = true,
            TriggerType = "ClinicCreatedEvent",
            Actions = new List<WorkflowAction>
            {
                new WorkflowAction
                {
                    Order = 1,
                    ActionType = "send_email",
                    Config = JsonSerializer.Serialize(new
                    {
                        to = "{{email}}",
                        subject = "Bem-vindo ao PrimeCare, {{name}}!",
                        body = "Olá {{name}},\n\nBem-vindo ao PrimeCare! Estamos felizes em tê-lo conosco..."
                    })
                },
                new WorkflowAction
                {
                    Order = 2,
                    ActionType = "create_ticket",
                    Config = JsonSerializer.Serialize(new
                    {
                        subject = "Verificar dados cadastrais - {{name}}",
                        priority = "normal",
                        category = "onboarding"
                    })
                },
                new WorkflowAction
                {
                    Order = 3,
                    ActionType = "add_tag",
                    Config = JsonSerializer.Serialize(new
                    {
                        tagName = "Onboarding"
                    })
                },
                new WorkflowAction
                {
                    Order = 4,
                    ActionType = "send_email",
                    DelaySeconds = 604800, // 7 dias
                    Config = JsonSerializer.Serialize(new
                    {
                        to = "{{email}}",
                        subject = "Como está sendo sua experiência com o PrimeCare?",
                        body = "Olá {{name}},\n\nJá faz uma semana desde que você começou..."
                    })
                }
            }
        };
        
        // Workflow 2: Prevenção de Churn
        var churnPrevention = new Workflow
        {
            Name = "Prevenção de Churn",
            Description = "Ações proativas para clientes inativos",
            IsEnabled = true,
            TriggerType = "InactivityDetectedEvent",
            Actions = new List<WorkflowAction>
            {
                new WorkflowAction
                {
                    Order = 1,
                    ActionType = "add_tag",
                    Config = JsonSerializer.Serialize(new
                    {
                        tagName = "At-risk"
                    })
                },
                new WorkflowAction
                {
                    Order = 2,
                    ActionType = "create_notification",
                    Config = JsonSerializer.Serialize(new
                    {
                        type = "warning",
                        title = "Clínica Inativa Detectada",
                        message = "{{name}} está inativa há {{daysSinceLastActivity}} dias"
                    })
                },
                new WorkflowAction
                {
                    Order = 3,
                    ActionType = "send_email",
                    Config = JsonSerializer.Serialize(new
                    {
                        to = "{{email}}",
                        subject = "Sentimos sua falta, {{name}}!",
                        body = "Olá {{name}},\n\nNotamos que você não acessa o sistema há algum tempo..."
                    })
                },
                new WorkflowAction
                {
                    Order = 4,
                    ActionType = "create_ticket",
                    Config = JsonSerializer.Serialize(new
                    {
                        subject = "Follow-up - Cliente inativo: {{name}}",
                        priority = "high",
                        category = "retention"
                    })
                }
            }
        };
        
        // Workflow 3: Trial Expirando
        var trialExpiring = new Workflow
        {
            Name = "Trial Expirando",
            Description = "Conversão de trial para pago",
            IsEnabled = true,
            TriggerType = "TrialExpiringEvent",
            Actions = new List<WorkflowAction>
            {
                new WorkflowAction
                {
                    Order = 1,
                    ActionType = "send_email",
                    Config = JsonSerializer.Serialize(new
                    {
                        to = "{{email}}",
                        subject = "Seu trial expira em {{daysRemaining}} dias!",
                        body = "Olá {{name}},\n\nSeu período de trial está chegando ao fim..."
                    })
                },
                new WorkflowAction
                {
                    Order = 2,
                    ActionType = "create_notification",
                    Config = JsonSerializer.Serialize(new
                    {
                        type = "warning",
                        title = "Trial Expirando",
                        message = "Trial de {{name}} expira em {{daysRemaining}} dias",
                        actionUrl = "/clinics/{{clinicId}}"
                    })
                },
                new WorkflowAction
                {
                    Order = 3,
                    ActionType = "add_tag",
                    Config = JsonSerializer.Serialize(new
                    {
                        tagName = "Trial-ending"
                    })
                }
            }
        };
        
        context.Workflows.AddRange(onboarding, churnPrevention, trialExpiring);
        context.SaveChanges();
    }
}
```

#### 1.2 Frontend - Workflow Builder

```typescript
// system-admin/src/app/workflows/workflow-builder/workflow-builder.component.ts
@Component({
  selector: 'app-workflow-builder',
  standalone: true,
  template: `
    <div class="workflow-builder">
      <!-- Header -->
      <mat-toolbar>
        <mat-form-field>
          <input matInput [(ngModel)]="workflow.name" placeholder="Nome do Workflow">
        </mat-form-field>
        
        <mat-slide-toggle [(ngModel)]="workflow.isEnabled">
          {{ workflow.isEnabled ? 'Ativo' : 'Inativo' }}
        </mat-slide-toggle>
        
        <span class="spacer"></span>
        
        <button mat-raised-button color="primary" (click)="saveWorkflow()">
          Salvar
        </button>
        <button mat-button (click)="testWorkflow()">
          Testar
        </button>
      </mat-toolbar>
      
      <!-- Trigger Selection -->
      <div class="workflow-trigger">
        <h3>1. Quando isso acontecer...</h3>
        <mat-form-field>
          <mat-select [(ngModel)]="workflow.triggerType" placeholder="Selecionar Trigger">
            <mat-optgroup label="Eventos">
              <mat-option value="ClinicCreatedEvent">Nova clínica cadastrada</mat-option>
              <mat-option value="SubscriptionExpiredEvent">Assinatura vencida</mat-option>
              <mat-option value="TrialExpiringEvent">Trial expirando</mat-option>
              <mat-option value="InactivityDetectedEvent">Inatividade detectada</mat-option>
              <mat-option value="PaymentFailedEvent">Pagamento falhou</mat-option>
            </mat-optgroup>
            <mat-optgroup label="Tempo">
              <mat-option value="Daily">Diariamente</mat-option>
              <mat-option value="Weekly">Semanalmente</mat-option>
              <mat-option value="Monthly">Mensalmente</mat-option>
            </mat-optgroup>
          </mat-select>
        </mat-form-field>
      </div>
      
      <!-- Actions List -->
      <div class="workflow-actions">
        <h3>2. Fazer isso...</h3>
        
        <div 
          cdkDropList
          (cdkDropListDropped)="reorderActions($event)"
          class="actions-list"
        >
          <div 
            *ngFor="let action of workflow.actions; let i = index"
            cdkDrag
            class="action-item"
          >
            <div class="action-header">
              <mat-icon cdkDragHandle>drag_indicator</mat-icon>
              <span class="action-number">{{ i + 1 }}</span>
              <mat-icon>{{ getActionIcon(action.actionType) }}</mat-icon>
              <span>{{ getActionLabel(action.actionType) }}</span>
              
              <button mat-icon-button (click)="editAction(action)">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button (click)="deleteAction(i)">
                <mat-icon>delete</mat-icon>
              </button>
            </div>
            
            <!-- Action Details Preview -->
            <div class="action-details">
              <ng-container [ngSwitch]="action.actionType">
                <div *ngSwitchCase="'send_email'">
                  Para: {{ action.config.to }}<br>
                  Assunto: {{ action.config.subject }}
                </div>
                <div *ngSwitchCase="'add_tag'">
                  Tag: {{ action.config.tagName }}
                </div>
                <div *ngSwitchCase="'create_ticket'">
                  {{ action.config.subject }}
                </div>
              </ng-container>
              
              <div *ngIf="action.delaySeconds" class="action-delay">
                ⏱️ Aguardar {{ formatDelay(action.delaySeconds) }}
              </div>
              
              <div *ngIf="action.condition" class="action-condition">
                🔀 Se {{ formatCondition(action.condition) }}
              </div>
            </div>
          </div>
        </div>
        
        <!-- Add Action Button -->
        <button mat-raised-button (click)="addAction()">
          <mat-icon>add</mat-icon> Adicionar Ação
        </button>
      </div>
    </div>
  `
})
export class WorkflowBuilderComponent implements OnInit {
  workflow: Workflow;
  
  addAction() {
    const dialogRef = this.dialog.open(AddActionDialogComponent, {
      width: '600px',
      data: { triggerType: this.workflow.triggerType }
    });
    
    dialogRef.afterClosed().subscribe(action => {
      if (action) {
        this.workflow.actions.push(action);
      }
    });
  }
  
  editAction(action: WorkflowAction) {
    const dialogRef = this.dialog.open(EditActionDialogComponent, {
      width: '600px',
      data: { action, triggerType: this.workflow.triggerType }
    });
    
    dialogRef.afterClosed().subscribe(updated => {
      if (updated) {
        Object.assign(action, updated);
      }
    });
  }
  
  async testWorkflow() {
    const result = await this.workflowService.test(this.workflow);
    
    this.dialog.open(WorkflowTestResultDialogComponent, {
      width: '800px',
      data: result
    });
  }
  
  formatDelay(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const days = Math.floor(hours / 24);
    
    if (days > 0) return `${days} dia(s)`;
    if (hours > 0) return `${hours} hora(s)`;
    if (minutes > 0) return `${minutes} minuto(s)`;
    return `${seconds} segundo(s)`;
  }
}
```

---

### 2. Smart Actions (3 semanas)

#### 2.1 Backend - Action System

```csharp
// Services/SmartActions/SmartActionService.cs
public interface ISmartActionService
{
    Task<string> ImpersonateClinic(int clinicId, int adminUserId);
    Task GrantCredit(int clinicId, int days, string reason);
    Task ApplyDiscount(int clinicId, decimal percentage, int months);
    Task SuspendTemporarily(int clinicId, DateTime? reactivationDate);
    Task<byte[]> ExportClinicData(int clinicId);
    Task MigratePlan(int clinicId, int newPlanId, bool proRata);
}

public class SmartActionService : ISmartActionService
{
    public async Task<string> ImpersonateClinic(int clinicId, int adminUserId)
    {
        var clinic = await _context.Clinics.FindAsync(clinicId);
        var admin = await _context.Users.FindAsync(adminUserId);
        
        if (clinic == null || admin == null)
            throw new NotFoundException("Clinic or admin not found");
            
        // Criar token de impersonation
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, clinic.OwnerId.ToString()),
            new Claim("TenantId", clinic.TenantId),
            new Claim("Impersonated", "true"),
            new Claim("ImpersonatorId", adminUserId.ToString()),
            new Claim("ImpersonatorName", admin.Name)
        };
        
        var token = _jwtService.GenerateToken(claims, expireHours: 2);
        
        // Registrar no audit log
        await _auditService.LogAsync(new AuditLogDto
        {
            Action = "Impersonate",
            EntityType = "Clinic",
            EntityId = clinicId.ToString(),
            UserId = adminUserId,
            Details = $"Admin {admin.Name} impersonated clinic {clinic.Name}",
            IpAddress = _httpContext.Connection.RemoteIpAddress?.ToString()
        });
        
        return token;
    }
    
    public async Task GrantCredit(int clinicId, int days, string reason)
    {
        var clinic = await _context.Clinics
            .Include(c => c.Subscription)
            .FirstOrDefaultAsync(c => c.Id == clinicId);
            
        if (clinic?.Subscription == null)
            throw new NotFoundException("Clinic or subscription not found");
            
        // Estender assinatura
        clinic.Subscription.ExpiresAt = clinic.Subscription.ExpiresAt.AddDays(days);
        
        // Criar crédito
        var credit = new SubscriptionCredit
        {
            SubscriptionId = clinic.Subscription.Id,
            Days = days,
            Reason = reason,
            GrantedAt = DateTime.UtcNow,
            GrantedBy = _currentUser.Id
        };
        
        _context.SubscriptionCredits.Add(credit);
        
        // Notificar cliente
        await _emailService.SendEmailAsync(
            to: clinic.Email,
            subject: "Você ganhou dias grátis!",
            body: $"Olá {clinic.Name},\n\n" +
                  $"Concedemos {days} dias grátis em sua assinatura.\n" +
                  $"Motivo: {reason}\n\n" +
                  $"Aproveite!"
        );
        
        await _context.SaveChangesAsync();
        
        // Audit log
        await _auditService.LogAsync(new AuditLogDto
        {
            Action = "GrantCredit",
            EntityType = "Clinic",
            EntityId = clinicId.ToString(),
            Details = $"Granted {days} days. Reason: {reason}"
        });
    }
    
    public async Task ApplyDiscount(int clinicId, decimal percentage, int months)
    {
        var clinic = await _context.Clinics
            .Include(c => c.Subscription)
            .FirstOrDefaultAsync(c => c.Id == clinicId);
            
        if (clinic?.Subscription == null)
            throw new NotFoundException("Clinic or subscription not found");
            
        // Gerar cupom único
        var coupon = new Coupon
        {
            Code = $"ADMIN-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
            Type = "percentage",
            Value = percentage,
            ValidMonths = months,
            ClinicId = clinicId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.Id
        };
        
        _context.Coupons.Add(coupon);
        
        // Aplicar imediatamente
        clinic.Subscription.CouponId = coupon.Id;
        clinic.Subscription.DiscountPercentage = percentage;
        clinic.Subscription.DiscountExpiresAt = DateTime.UtcNow.AddMonths(months);
        
        await _context.SaveChangesAsync();
        
        // Notificar cliente
        await _emailService.SendEmailAsync(
            to: clinic.Email,
            subject: "Desconto especial aplicado!",
            body: $"Olá {clinic.Name},\n\n" +
                  $"Aplicamos um desconto de {percentage}% em sua assinatura por {months} meses.\n" +
                  $"Cupom: {coupon.Code}\n\n" +
                  $"Aproveite!"
        );
    }
    
    public async Task<byte[]> ExportClinicData(int clinicId)
    {
        // LGPD compliance - direito aos dados
        var clinic = await _context.Clinics
            .Include(c => c.Users)
            .Include(c => c.Patients)
            .Include(c => c.Appointments)
            .Include(c => c.Subscription)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == clinicId);
            
        if (clinic == null)
            throw new NotFoundException("Clinic not found");
            
        var data = new
        {
            clinic = new
            {
                clinic.Name,
                clinic.Cnpj,
                clinic.Email,
                clinic.Phone,
                clinic.Address
            },
            users = clinic.Users.Select(u => new
            {
                u.Name,
                u.Email,
                u.Role,
                u.CreatedAt
            }),
            patients = clinic.Patients.Select(p => new
            {
                p.Name,
                p.Cpf,
                p.BirthDate,
                p.Phone
            }),
            appointments = clinic.Appointments.Select(a => new
            {
                a.Date,
                a.PatientName,
                a.DoctorName,
                a.Status
            }),
            subscription = new
            {
                clinic.Subscription.Plan.Name,
                clinic.Subscription.Status,
                clinic.Subscription.Mrr,
                clinic.Subscription.CreatedAt
            }
        };
        
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        // Audit log
        await _auditService.LogAsync(new AuditLogDto
        {
            Action = "ExportData",
            EntityType = "Clinic",
            EntityId = clinicId.ToString(),
            Details = "Exported all clinic data (LGPD compliance)"
        });
        
        return Encoding.UTF8.GetBytes(json);
    }
}
```

#### 2.2 Frontend - Smart Actions UI

```typescript
// system-admin/src/app/clinics/smart-actions/smart-actions.component.ts
@Component({
  selector: 'app-smart-actions',
  standalone: true,
  template: `
    <div class="smart-actions">
      <button 
        mat-menu-item 
        (click)="impersonateClinic()"
        class="action-item"
      >
        <mat-icon>login</mat-icon>
        <div class="action-content">
          <span class="action-title">Login como</span>
          <span class="action-description">Acessar sistema como esta clínica</span>
        </div>
      </button>
      
      <button mat-menu-item (click)="grantCredit()">
        <mat-icon>card_giftcard</mat-icon>
        <div class="action-content">
          <span class="action-title">Conceder Crédito</span>
          <span class="action-description">Adicionar dias grátis</span>
        </div>
      </button>
      
      <button mat-menu-item (click)="applyDiscount()">
        <mat-icon>local_offer</mat-icon>
        <div class="action-content">
          <span class="action-title">Aplicar Desconto</span>
          <span class="action-description">Gerar cupom de desconto</span>
        </div>
      </button>
      
      <button mat-menu-item (click)="sendEmail()">
        <mat-icon>email</mat-icon>
        <div class="action-content">
          <span class="action-title">Enviar Email</span>
          <span class="action-description">Email personalizado</span>
        </div>
      </button>
      
      <button mat-menu-item (click)="migratePlan()">
        <mat-icon>swap_horiz</mat-icon>
        <div class="action-content">
          <span class="action-title">Migrar de Plano</span>
          <span class="action-description">Upgrade/downgrade</span>
        </div>
      </button>
      
      <button mat-menu-item (click)="exportData()">
        <mat-icon>download</mat-icon>
        <div class="action-content">
          <span class="action-title">Exportar Dados</span>
          <span class="action-description">Backup completo (LGPD)</span>
        </div>
      </button>
      
      <button mat-menu-item (click)="suspend()" class="action-danger">
        <mat-icon>block</mat-icon>
        <div class="action-content">
          <span class="action-title">Suspender Temporariamente</span>
          <span class="action-description">Bloquear acesso</span>
        </div>
      </button>
    </div>
  `
})
export class SmartActionsComponent {
  @Input() clinic: Clinic;
  
  async impersonateClinic() {
    const confirmed = await this.confirmDialog.open({
      title: 'Fazer Login como Clínica',
      message: 'Você está prestes a acessar o sistema como esta clínica. Esta ação será registrada no audit log.',
      confirmText: 'Continuar',
      cancelText: 'Cancelar'
    });
    
    if (confirmed) {
      const token = await this.smartActionService.impersonateClinic(this.clinic.id);
      
      // Abrir em nova aba com token
      const url = `${environment.appUrl}/admin-impersonate?token=${token}`;
      window.open(url, '_blank');
      
      this.snackBar.open('Abrindo sistema como clínica...', 'OK', { duration: 3000 });
    }
  }
  
  async grantCredit() {
    const dialogRef = this.dialog.open(GrantCreditDialogComponent, {
      width: '500px',
      data: { clinic: this.clinic }
    });
    
    const result = await dialogRef.afterClosed().toPromise();
    if (result) {
      await this.smartActionService.grantCredit(
        this.clinic.id,
        result.days,
        result.reason
      );
      
      this.snackBar.open(`${result.days} dias concedidos!`, 'OK', { duration: 3000 });
    }
  }
  
  async exportData() {
    const confirmed = await this.confirmDialog.open({
      title: 'Exportar Dados da Clínica',
      message: 'Exportar todos os dados desta clínica (LGPD compliance)?',
      confirmText: 'Exportar'
    });
    
    if (confirmed) {
      const blob = await this.smartActionService.exportData(this.clinic.id);
      saveAs(blob, `clinic-${this.clinic.id}-data.json`);
    }
  }
}

// Grant Credit Dialog
@Component({
  selector: 'app-grant-credit-dialog',
  template: `
    <h2 mat-dialog-title>Conceder Crédito</h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field>
          <mat-label>Número de Dias</mat-label>
          <input matInput type="number" formControlName="days" min="1">
        </mat-form-field>
        
        <mat-form-field>
          <mat-label>Motivo</mat-label>
          <textarea matInput formControlName="reason" rows="3"></textarea>
          <mat-hint>Este motivo será registrado no audit log</mat-hint>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions>
      <button mat-button mat-dialog-close>Cancelar</button>
      <button 
        mat-raised-button 
        color="primary" 
        [mat-dialog-close]="form.value"
        [disabled]="form.invalid"
      >
        Conceder
      </button>
    </mat-dialog-actions>
  `
})
export class GrantCreditDialogComponent {
  form = this.fb.group({
    days: [30, [Validators.required, Validators.min(1)]],
    reason: ['', Validators.required]
  });
  
  constructor(private fb: FormBuilder) {}
}
```

---

### 3. Integrações e Webhooks (3 semanas)

#### 3.1 Backend - Webhook System

```csharp
// Entities/WebhookSubscription.cs
public class WebhookSubscription
{
    public int Id { get; set; }
    public string Url { get; set; }
    public List<string> Events { get; set; } // JSON array
    public string Secret { get; set; } // para validação HMAC
    public bool IsEnabled { get; set; }
    public int RetryCount { get; set; } = 3;
    public DateTime CreatedAt { get; set; }
}

// Entities/WebhookDelivery.cs
public class WebhookDelivery
{
    public int Id { get; set; }
    public int WebhookSubscriptionId { get; set; }
    public WebhookSubscription Subscription { get; set; }
    
    public string Event { get; set; }
    public string Payload { get; set; }
    public string Status { get; set; } // pending, delivered, failed
    public int AttemptCount { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseBody { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

// Services/Webhooks/WebhookService.cs
public interface IWebhookService
{
    Task SendWebhook(string eventName, object payload);
    Task<List<WebhookSubscription>> GetSubscriptionsForEvent(string eventName);
    Task RetryFailedDelivery(int deliveryId);
}

public class WebhookService : IWebhookService
{
    public async Task SendWebhook(string eventName, object payload)
    {
        var subscriptions = await GetSubscriptionsForEvent(eventName);
        
        foreach (var subscription in subscriptions)
        {
            // Criar delivery record
            var delivery = new WebhookDelivery
            {
                WebhookSubscriptionId = subscription.Id,
                Event = eventName,
                Payload = JsonSerializer.Serialize(payload),
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };
            
            _context.WebhookDeliveries.Add(delivery);
            await _context.SaveChangesAsync();
            
            // Enviar em background
            BackgroundJob.Enqueue(() => DeliverWebhook(delivery.Id));
        }
    }
    
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
    public async Task DeliverWebhook(int deliveryId)
    {
        var delivery = await _context.WebhookDeliveries
            .Include(d => d.Subscription)
            .FirstOrDefaultAsync(d => d.Id == deliveryId);
            
        if (delivery == null || delivery.Status == "delivered")
            return;
            
        delivery.AttemptCount++;
        
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);
            
            var request = new HttpRequestMessage(HttpMethod.Post, delivery.Subscription.Url);
            request.Content = new StringContent(delivery.Payload, Encoding.UTF8, "application/json");
            
            // Adicionar signature HMAC para validação
            var signature = GenerateHmacSignature(delivery.Payload, delivery.Subscription.Secret);
            request.Headers.Add("X-Webhook-Signature", signature);
            request.Headers.Add("X-Webhook-Event", delivery.Event);
            request.Headers.Add("X-Webhook-Id", delivery.Id.ToString());
            
            var response = await client.SendAsync(request);
            
            delivery.ResponseCode = ((int)response.StatusCode).ToString();
            delivery.ResponseBody = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                delivery.Status = "delivered";
                delivery.DeliveredAt = DateTime.UtcNow;
            }
            else
            {
                delivery.Status = "failed";
                
                // Retry se não atingiu limite
                if (delivery.AttemptCount < delivery.Subscription.RetryCount)
                {
                    BackgroundJob.Schedule(() => DeliverWebhook(deliveryId), 
                        TimeSpan.FromSeconds(Math.Pow(2, delivery.AttemptCount) * 60));
                }
            }
        }
        catch (Exception ex)
        {
            delivery.Status = "failed";
            delivery.ResponseBody = ex.Message;
            
            if (delivery.AttemptCount < delivery.Subscription.RetryCount)
            {
                BackgroundJob.Schedule(() => DeliverWebhook(deliveryId), 
                    TimeSpan.FromSeconds(Math.Pow(2, delivery.AttemptCount) * 60));
            }
        }
        
        await _context.SaveChangesAsync();
    }
    
    private string GenerateHmacSignature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }
}
```

**Integrações Nativas:**
```csharp
// Services/Integrations/StripeIntegrationService.cs
public class StripeIntegrationService
{
    public async Task SyncPayment(string stripeEventJson)
    {
        var stripeEvent = EventUtility.ParseEvent(stripeEventJson);
        
        switch (stripeEvent.Type)
        {
            case "invoice.payment_succeeded":
                var invoice = (Invoice)stripeEvent.Data.Object;
                await HandlePaymentSucceeded(invoice);
                break;
                
            case "invoice.payment_failed":
                var failedInvoice = (Invoice)stripeEvent.Data.Object;
                await HandlePaymentFailed(failedInvoice);
                break;
                
            case "customer.subscription.deleted":
                var subscription = (Subscription)stripeEvent.Data.Object;
                await HandleSubscriptionCancelled(subscription);
                break;
        }
    }
}

// Services/Integrations/SendGridIntegrationService.cs
public class SendGridIntegrationService
{
    public async Task SendTransactionalEmail(string to, string templateId, object data)
    {
        var client = new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));
        var msg = new SendGridMessage();
        msg.SetFrom(new EmailAddress("noreply@primecare.com", "PrimeCare"));
        msg.AddTo(new EmailAddress(to));
        msg.SetTemplateId(templateId);
        msg.SetTemplateData(data);
        
        await client.SendEmailAsync(msg);
    }
}

// Services/Integrations/SlackIntegrationService.cs
public class SlackIntegrationService
{
    public async Task SendNotification(string channel, string message)
    {
        var webhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL");
        
        using var client = new HttpClient();
        var payload = new
        {
            channel,
            text = message,
            username = "PrimeCare Bot",
            icon_emoji = ":hospital:"
        };
        
        await client.PostAsJsonAsync(webhookUrl, payload);
    }
}
```

#### 3.2 Frontend - Webhook Management

```typescript
// system-admin/src/app/integrations/webhook-manager.component.ts
@Component({
  selector: 'app-webhook-manager',
  standalone: true,
  template: `
    <div class="webhook-manager">
      <button mat-raised-button color="primary" (click)="createWebhook()">
        <mat-icon>add</mat-icon> Novo Webhook
      </button>
      
      <table mat-table [dataSource]="webhooks">
        <ng-container matColumnDef="url">
          <th mat-header-cell *matHeaderCellDef>URL</th>
          <td mat-cell *matCellDef="let webhook">{{ webhook.url }}</td>
        </ng-container>
        
        <ng-container matColumnDef="events">
          <th mat-header-cell *matHeaderCellDef>Eventos</th>
          <td mat-cell *matCellDef="let webhook">
            <mat-chip-set>
              <mat-chip *ngFor="let event of webhook.events">{{ event }}</mat-chip>
            </mat-chip-set>
          </td>
        </ng-container>
        
        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let webhook">
            <mat-slide-toggle 
              [(ngModel)]="webhook.isEnabled"
              (change)="toggleWebhook(webhook)"
            ></mat-slide-toggle>
          </td>
        </ng-container>
        
        <ng-container matColumnDef="deliveries">
          <th mat-header-cell *matHeaderCellDef>Entregas</th>
          <td mat-cell *matCellDef="let webhook">
            <button mat-button (click)="viewDeliveries(webhook)">
              Ver Histórico
            </button>
          </td>
        </ng-container>
        
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Ações</th>
          <td mat-cell *matCellDef="let webhook">
            <button mat-icon-button [matMenuTriggerFor]="menu">
              <mat-icon>more_vert</mat-icon>
            </button>
            <mat-menu #menu="matMenu">
              <button mat-menu-item (click)="editWebhook(webhook)">Editar</button>
              <button mat-menu-item (click)="testWebhook(webhook)">Testar</button>
              <button mat-menu-item (click)="deleteWebhook(webhook)">Excluir</button>
            </mat-menu>
          </td>
        </ng-container>
        
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
      </table>
    </div>
  `
})
export class WebhookManagerComponent implements OnInit {
  webhooks: WebhookSubscription[] = [];
  displayedColumns = ['url', 'events', 'status', 'deliveries', 'actions'];
  
  async testWebhook(webhook: WebhookSubscription) {
    const result = await this.webhookService.test(webhook.id);
    
    if (result.success) {
      this.snackBar.open('Webhook enviado com sucesso!', 'OK', { duration: 3000 });
    } else {
      this.snackBar.open(`Erro: ${result.error}`, 'OK', { duration: 5000 });
    }
  }
}
```

---

## 📦 Implementação Frontend Realizada (Janeiro 2026)

### ✅ Estrutura Criada

**Modelos TypeScript:**
- `frontend/mw-system-admin/src/app/models/workflow.model.ts` - Interfaces Workflow, WorkflowAction, WorkflowExecution, WorkflowActionExecution
- `frontend/mw-system-admin/src/app/models/smart-action.model.ts` - DTOs para 7 smart actions
- `frontend/mw-system-admin/src/app/models/webhook.model.ts` - WebhookSubscription e WebhookDelivery

**Serviços:**
- `frontend/mw-system-admin/src/app/services/workflow.service.ts` - API integration para workflows
- `frontend/mw-system-admin/src/app/services/smart-action.service.ts` - API integration para smart actions
- `frontend/mw-system-admin/src/app/services/webhook.service.ts` - API integration para webhooks

**Componentes - Workflows:**
- `frontend/mw-system-admin/src/app/pages/workflows/workflows-list.*` - Listagem de workflows com CRUD
- `frontend/mw-system-admin/src/app/pages/workflows/workflow-editor.*` - Editor visual de workflows
- `frontend/mw-system-admin/src/app/pages/workflows/workflow-executions.*` - Histórico de execuções

**Componentes - Webhooks:**
- `frontend/mw-system-admin/src/app/pages/webhooks/webhooks-list.*` - Gerenciamento de webhooks
- `frontend/mw-system-admin/src/app/pages/webhooks/webhook-deliveries.*` - Histórico de entregas

**Componentes - Smart Actions:**
- `frontend/mw-system-admin/src/app/components/smart-actions-dialog/*` - Diálogo reutilizável para 7 ações

**Rotas Adicionadas:**
```typescript
// app.routes.ts
{ path: 'workflows', loadComponent: ... },
{ path: 'workflows/create', loadComponent: ... },
{ path: 'workflows/:id/edit', loadComponent: ... },
{ path: 'workflows/:id/executions', loadComponent: ... },
{ path: 'webhooks', loadComponent: ... },
{ path: 'webhooks/:id/deliveries', loadComponent: ... }
```

### ✅ Funcionalidades Implementadas

**Workflow Builder (Editor Visual):**
- Criar/editar workflows
- Configurar triggers (eventos, tempo, manual)
- Adicionar/remover/reordenar ações
- Configurar delays e condições
- Testar workflows
- Habilitar/desabilitar

**Workflow List:**
- Visualizar todos workflows
- Filtrar por status (ativo/inativo)
- Ver estatísticas de execução
- Editar/deletar workflows
- Duplicar workflows

**Workflow Executions:**
- Histórico completo de execuções
- Status de cada ação
- Logs detalhados
- Filtrar por status/data
- Reexecutar workflows

**Smart Actions Dialog:**
- 7 ações administrativas:
  1. **Impersonate** - Login como cliente
  2. **Grant Credit** - Conceder dias grátis
  3. **Apply Discount** - Aplicar desconto
  4. **Suspend** - Suspender temporariamente
  5. **Export Data** - Exportar dados (LGPD)
  6. **Migrate Plan** - Migrar plano
  7. **Send Custom Email** - Email personalizado
- Validação de formulários
- Feedback de sucesso/erro
- Audit logging automático

**Webhook Management:**
- Criar/editar webhooks
- Configurar URL e eventos
- Ativar/desativar
- Regenerar secrets
- Testar entregas
- Ver histórico de deliveries
- Retry manual de falhas

### 🎨 Padrões de UI Utilizados

- **PrimeNG Components:** Button, Table, Dialog, Form Fields, etc.
- **Standalone Components:** Arquitetura moderna do Angular 17+
- **Signals:** Reatividade com Angular Signals
- **Responsive Design:** Mobile-friendly
- **Loading States:** Indicadores de carregamento
- **Error Handling:** Mensagens de erro amigáveis
- **Accessibility:** ARIA labels e navegação por teclado

### 🔒 Segurança

- System Admin Guard em todas as rotas
- Validação de inputs
- Sanitização de dados
- CSRF protection
- Audit logging de todas ações
- Confirmações para ações destrutivas

**Documentação Completa:** Ver `PHASE4_FRONTEND_IMPLEMENTATION_SUMMARY.md`

---

## ✅ Critérios de Sucesso

### Workflows
- [x] Engine de workflows operacional (backend implementado)
- [x] 5+ triggers de eventos (ClinicCreatedEvent, SubscriptionExpiredEvent, TrialExpiringEvent, InactivityDetectedEvent, PaymentFailedEvent)
- [x] 6+ tipos de ações (send_email, send_sms, create_notification, add_tag, create_ticket, webhook)
- [x] Editor visual funcional (frontend implementado)
- [x] Execuções registradas e auditadas (WorkflowExecutions e WorkflowActionExecutions)
- [x] Retry automático em falhas (implementado via Hangfire)

### Smart Actions
- [x] 7+ smart actions implementadas (ImpersonateClinic, GrantCredit, ApplyDiscount, SuspendTemporarily, ExportData, MigratePlan, SendCustomEmail)
- [x] Impersonation seguro com audit log
- [x] Concessão de créditos funcional
- [x] Exportação de dados (LGPD)
- [x] Todas ações registradas (via IAuditService)

### Webhooks
- [x] Sistema de webhooks operacional (backend implementado)
- [x] Retry exponencial configurável
- [x] HMAC signature para segurança
- [x] Histórico de entregas (WebhookDelivery entity)
- [x] 3+ integrações nativas (Stripe, SendGrid, Twilio, Slack)

---

## 🧪 Testes e Validação

```csharp
public class WorkflowEngineTests
{
    [Fact]
    public async Task ExecuteWorkflow_ShouldExecuteAllActions()
    {
        var workflow = CreateTestWorkflow();
        var execution = await _engine.ExecuteWorkflow(workflow.Id, new { clinicId = 1 });
        
        Assert.Equal("completed", execution.Status);
        Assert.Equal(workflow.Actions.Count, execution.ActionExecutions.Count);
    }
}
```

---

## 📚 Documentação

Ver também:
- **PHASE4_WORKFLOW_AUTOMATION_IMPLEMENTATION.md** - Documentação completa da implementação backend
- **PHASE4_FRONTEND_IMPLEMENTATION_SUMMARY.md** - Documentação completa da implementação frontend
- Guia de criação de workflows (a ser criado)
- Catálogo de triggers e ações (ver PHASE4 doc)
- Smart actions reference (ver PHASE4 doc)
- Webhook integration guide (a ser criado)
- Security best practices (ver SECURITY_SUMMARY_FASE4.md)

---

## 📊 Status de Implementação

### ✅ Backend (100% Completo)
- [x] Entidades de domínio (Workflow, WorkflowAction, WorkflowExecution, WorkflowActionExecution)
- [x] Eventos de domínio (ClinicCreatedEvent, SubscriptionExpiredEvent, TrialExpiringEvent, InactivityDetectedEvent)
- [x] WorkflowEngine com suporte a triggers e ações
- [x] EventPublisher para arquitetura event-driven
- [x] SmartActionService com 7 ações administrativas
- [x] Configurações EF Core
- [x] Migration criada (20260128230900_AddWorkflowAutomation)
- [x] Controllers (WorkflowController, SmartActionController)
- [x] Background Jobs (WorkflowJobs)
- [x] Seeders (WorkflowTemplateSeeder)
- [x] Testes unitários base

### ✅ Frontend (100% Completo)
- [x] Workflow Builder Component (editor visual)
- [x] Workflow List Component
- [x] Workflow Execution History Component
- [x] Smart Actions Dialog Components
- [x] Webhook Management Component
- [x] Routing configuration
- [x] Services e interfaces TypeScript

### 📋 Próximas Tarefas
1. Aplicar migration ao banco de dados (`dotnet ef database update`)
2. ✅ ~~Implementar componentes Angular do frontend~~ (Concluído)
3. Criar testes end-to-end
4. Documentar guias de usuário
5. Validar com usuários admin

---

## 🔄 Próximos Passos

Após Fase 4:
1. ✅ Validar automações com admins
2. ➡️ **Fase 5: Experiência e Usabilidade**

---

## 📞 Referências

- **Zapier:** https://zapier.com
- **Retool:** https://retool.com
- **Zendesk Automations:** https://www.zendesk.com

---

**Criado:** Janeiro 2026  
**Versão:** 1.2  
**Última Atualização:** 29 de Janeiro de 2026  
**Status:** ✅ Backend implementado | ✅ Frontend implementado
