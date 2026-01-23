# 📋 Prompt 17: CRM Avançado e Customer Experience

**Prioridade:** 🔥 P2 - Médio  
**Complexidade:** ⚡⚡⚡ Alta  
**Tempo Estimado:** 3-4 meses | 2 desenvolvedores  
**Custo:** R$ 110.000  
**Pré-requisitos:** Sistema base de pacientes e consultas funcionando

---

## 🎯 Objetivo

Implementar um sistema CRM completo com mapeamento de jornada do paciente, automação de marketing, pesquisas NPS/CSAT automatizadas, ouvidoria integrada, análise de sentimento com IA, e integração com canais de comunicação (Email, SMS, WhatsApp) para melhorar retenção e prever churn.

---

## 📊 Contexto do Sistema

### Problema Atual
- Falta de visibilidade da jornada do paciente
- Comunicação reativa e não personalizada
- Sem métricas de satisfação estruturadas
- Dificuldade em identificar pacientes em risco de churn
- Reclamações não gerenciadas adequadamente
- Marketing sem segmentação comportamental

### Solução Proposta
Sistema CRM que:
- Mapeia 7 estágios da jornada do paciente
- Automatiza campanhas segmentadas
- Coleta NPS/CSAT automaticamente
- Gerencia reclamações via ouvidoria
- Analisa sentimentos com IA
- Integra email/SMS/WhatsApp
- Prediz churn e identifica oportunidades

---

## 🏗️ Arquitetura da Solução

### 1. Patient Journey Mapping (5 semanas)

#### 1.1 Modelo de Jornada
```csharp
// src/MedicSoft.Core/Entities/CRM/PatientJourney.cs
public class PatientJourney
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Paciente Paciente { get; set; }
    
    // Estágios da jornada
    public List<JourneyStage> Stages { get; set; }
    public JourneyStageEnum CurrentStage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Métricas
    public int TotalTouchpoints { get; set; }
    public decimal LifetimeValue { get; set; }
    public int NpsScore { get; set; }
    public double SatisfactionScore { get; set; }
    public ChurnRiskLevel ChurnRisk { get; set; }
}

public enum JourneyStageEnum
{
    Descoberta = 1,      // Lead capture, marketing inicial
    Consideracao = 2,    // Avaliando opções, comparando
    PrimeiraConsulta = 3, // Primeiro atendimento
    Tratamento = 4,       // Durante o tratamento
    Retorno = 5,          // Consultas de retorno
    Fidelizacao = 6,      // Cliente recorrente
    Advocacia = 7         // Promotor da marca
}

public class JourneyStage
{
    public Guid Id { get; set; }
    public JourneyStageEnum Stage { get; set; }
    public DateTime EnteredAt { get; set; }
    public DateTime? ExitedAt { get; set; }
    public int DurationDays { get; set; }
    
    // Touchpoints no estágio
    public List<PatientTouchpoint> Touchpoints { get; set; }
    
    // Eventos que movem para próximo estágio
    public string ExitTrigger { get; set; }
}

public class PatientTouchpoint
{
    public Guid Id { get; set; }
    public Guid JourneyStageId { get; set; }
    public DateTime Timestamp { get; set; }
    
    public TouchpointType Type { get; set; }
    public string Channel { get; set; } // Email, SMS, WhatsApp, Phone, InPerson
    public string Description { get; set; }
    public TouchpointDirection Direction { get; set; } // Inbound, Outbound
    
    // Sentimento associado
    public SentimentAnalysis Sentiment { get; set; }
}

public enum TouchpointType
{
    MarketingCampaign,
    PhoneCall,
    EmailInteraction,
    SmsInteraction,
    WhatsAppMessage,
    WebsiteVisit,
    Appointment,
    Survey,
    Complaint,
    SocialMedia
}
```

#### 1.2 Serviço de Jornada
```csharp
// src/MedicSoft.Api/Services/CRM/PatientJourneyService.cs
public class PatientJourneyService : IPatientJourneyService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PatientJourneyService> _logger;
    
    public async Task<PatientJourney> GetOrCreateJourneyAsync(Guid pacienteId)
    {
        var journey = await _context.PatientJourneys
            .Include(j => j.Stages)
            .ThenInclude(s => s.Touchpoints)
            .FirstOrDefaultAsync(j => j.PacienteId == pacienteId);
            
        if (journey == null)
        {
            journey = new PatientJourney
            {
                PacienteId = pacienteId,
                CurrentStage = JourneyStageEnum.Descoberta,
                CreatedAt = DateTime.UtcNow,
                Stages = new List<JourneyStage>
                {
                    new JourneyStage
                    {
                        Stage = JourneyStageEnum.Descoberta,
                        EnteredAt = DateTime.UtcNow
                    }
                }
            };
            _context.PatientJourneys.Add(journey);
            await _context.SaveChangesAsync();
        }
        
        return journey;
    }
    
    public async Task AdvanceStageAsync(Guid pacienteId, JourneyStageEnum newStage, string trigger)
    {
        var journey = await GetOrCreateJourneyAsync(pacienteId);
        
        // Fechar estágio atual
        var currentStage = journey.Stages.FirstOrDefault(s => s.ExitedAt == null);
        if (currentStage != null)
        {
            currentStage.ExitedAt = DateTime.UtcNow;
            currentStage.DurationDays = (DateTime.UtcNow - currentStage.EnteredAt).Days;
            currentStage.ExitTrigger = trigger;
        }
        
        // Criar novo estágio
        journey.Stages.Add(new JourneyStage
        {
            Stage = newStage,
            EnteredAt = DateTime.UtcNow
        });
        
        journey.CurrentStage = newStage;
        journey.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        // Trigger automações para o novo estágio
        await TriggerStageAutomationsAsync(pacienteId, newStage);
    }
    
    public async Task AddTouchpointAsync(Guid pacienteId, PatientTouchpoint touchpoint)
    {
        var journey = await GetOrCreateJourneyAsync(pacienteId);
        var currentStage = journey.Stages.FirstOrDefault(s => s.ExitedAt == null);
        
        if (currentStage != null)
        {
            currentStage.Touchpoints.Add(touchpoint);
            journey.TotalTouchpoints++;
            await _context.SaveChangesAsync();
        }
    }
    
    private async Task TriggerStageAutomationsAsync(Guid pacienteId, JourneyStageEnum stage)
    {
        // Buscar automações configuradas para este estágio
        var automations = await _context.MarketingAutomations
            .Where(a => a.TriggerStage == stage && a.IsActive)
            .ToListAsync();
            
        foreach (var automation in automations)
        {
            await ExecuteAutomationAsync(pacienteId, automation);
        }
    }
}
```

#### 1.3 Frontend - Visualização de Jornada
```typescript
// frontend/src/components/CRM/PatientJourneyTimeline.tsx
import React, { useEffect, useState } from 'react';
import { Timeline, Card, Badge, Tooltip } from 'antd';
import { 
  SearchOutlined, 
  CalendarOutlined, 
  MedicineBoxOutlined,
  HeartOutlined,
  TrophyOutlined 
} from '@ant-design/icons';

interface PatientJourneyProps {
  patientId: string;
}

const stageIcons = {
  Descoberta: <SearchOutlined />,
  Consideracao: <CalendarOutlined />,
  PrimeiraConsulta: <MedicineBoxOutlined />,
  Tratamento: <MedicineBoxOutlined />,
  Retorno: <CalendarOutlined />,
  Fidelizacao: <HeartOutlined />,
  Advocacia: <TrophyOutlined />
};

const stageColors = {
  Descoberta: '#1890ff',
  Consideracao: '#13c2c2',
  PrimeiraConsulta: '#52c41a',
  Tratamento: '#faad14',
  Retorno: '#722ed1',
  Fidelizacao: '#eb2f96',
  Advocacia: '#f5222d'
};

export const PatientJourneyTimeline: React.FC<PatientJourneyProps> = ({ patientId }) => {
  const [journey, setJourney] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchJourney();
  }, [patientId]);

  const fetchJourney = async () => {
    try {
      const response = await fetch(`/api/crm/patient-journey/${patientId}`);
      const data = await response.json();
      setJourney(data);
    } catch (error) {
      console.error('Error fetching journey:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Carregando...</div>;

  return (
    <Card title="Jornada do Paciente" className="journey-timeline">
      <Timeline mode="left">
        {journey.stages.map((stage: any, index: number) => (
          <Timeline.Item
            key={stage.id}
            dot={stageIcons[stage.stage]}
            color={stageColors[stage.stage]}
          >
            <Card size="small" style={{ borderLeft: `3px solid ${stageColors[stage.stage]}` }}>
              <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <h4>{stage.stage}</h4>
                <Badge 
                  count={stage.touchpoints?.length || 0} 
                  style={{ backgroundColor: stageColors[stage.stage] }}
                />
              </div>
              
              <p>
                <strong>Entrada:</strong> {new Date(stage.enteredAt).toLocaleDateString('pt-BR')}
              </p>
              
              {stage.exitedAt && (
                <>
                  <p>
                    <strong>Saída:</strong> {new Date(stage.exitedAt).toLocaleDateString('pt-BR')}
                  </p>
                  <p>
                    <strong>Duração:</strong> {stage.durationDays} dias
                  </p>
                  <p>
                    <strong>Motivo:</strong> {stage.exitTrigger}
                  </p>
                </>
              )}
              
              {stage.touchpoints && stage.touchpoints.length > 0 && (
                <div style={{ marginTop: 10 }}>
                  <strong>Touchpoints:</strong>
                  <ul style={{ margin: '5px 0', paddingLeft: 20 }}>
                    {stage.touchpoints.slice(0, 3).map((tp: any) => (
                      <li key={tp.id}>
                        <Tooltip title={tp.description}>
                          {tp.channel} - {tp.type}
                        </Tooltip>
                      </li>
                    ))}
                    {stage.touchpoints.length > 3 && (
                      <li>... e mais {stage.touchpoints.length - 3}</li>
                    )}
                  </ul>
                </div>
              )}
            </Card>
          </Timeline.Item>
        ))}
      </Timeline>
      
      <Card size="small" style={{ marginTop: 20, backgroundColor: '#f0f2f5' }}>
        <h4>Métricas da Jornada</h4>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 10 }}>
          <div>
            <strong>Total Touchpoints:</strong> {journey.totalTouchpoints}
          </div>
          <div>
            <strong>LTV:</strong> R$ {journey.lifetimeValue.toFixed(2)}
          </div>
          <div>
            <strong>NPS:</strong> {journey.npsScore}
          </div>
          <div>
            <strong>Risco Churn:</strong> 
            <Badge 
              status={journey.churnRisk === 'High' ? 'error' : journey.churnRisk === 'Medium' ? 'warning' : 'success'} 
              text={journey.churnRisk}
            />
          </div>
        </div>
      </Card>
    </Card>
  );
};
```

---

### 2. Automação de Marketing (4 semanas)

#### 2.1 Modelo de Automação
```csharp
// src/MedicSoft.Core/Entities/CRM/MarketingAutomation.cs
public class MarketingAutomation
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    
    // Triggers
    public AutomationTriggerType TriggerType { get; set; }
    public JourneyStageEnum? TriggerStage { get; set; }
    public string TriggerEvent { get; set; } // "appointment_scheduled", "no_show", etc.
    public int? DelayMinutes { get; set; } // Delay após trigger
    
    // Segmentação
    public string SegmentFilter { get; set; } // JSON com filtros
    public List<string> Tags { get; set; }
    
    // Ações
    public List<AutomationAction> Actions { get; set; }
    
    // Métricas
    public int TimesExecuted { get; set; }
    public DateTime? LastExecutedAt { get; set; }
    public double SuccessRate { get; set; }
}

public enum AutomationTriggerType
{
    StageChange,
    Event,
    Scheduled,
    BehaviorBased,
    DateBased
}

public class AutomationAction
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public ActionType Type { get; set; }
    
    // Email
    public Guid? EmailTemplateId { get; set; }
    public EmailTemplate EmailTemplate { get; set; }
    
    // SMS/WhatsApp
    public string MessageTemplate { get; set; }
    public string Channel { get; set; } // Email, SMS, WhatsApp
    
    // Tags/Score
    public string TagToAdd { get; set; }
    public int? ScoreChange { get; set; }
    
    // Condicional
    public string Condition { get; set; } // Expressão condicional
}

public enum ActionType
{
    SendEmail,
    SendSMS,
    SendWhatsApp,
    AddTag,
    RemoveTag,
    ChangeScore,
    CreateTask,
    SendNotification,
    WebhookCall
}

public class EmailTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string HtmlBody { get; set; }
    public string PlainTextBody { get; set; }
    
    // Variáveis disponíveis
    public List<string> AvailableVariables { get; set; } // {{nome_paciente}}, etc.
}
```

#### 2.2 Engine de Automação
```csharp
// src/MedicSoft.Api/Services/CRM/MarketingAutomationEngine.cs
public class MarketingAutomationEngine : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MarketingAutomationEngine> _logger;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Buscar automações pendentes
                var pendingAutomations = await context.AutomationExecutions
                    .Where(ae => ae.Status == ExecutionStatus.Pending 
                              && ae.ScheduledFor <= DateTime.UtcNow)
                    .Include(ae => ae.Automation)
                    .ThenInclude(a => a.Actions)
                    .ToListAsync();
                
                foreach (var execution in pendingAutomations)
                {
                    await ExecuteAutomationAsync(execution, scope.ServiceProvider);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in automation engine");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
    
    private async Task ExecuteAutomationAsync(
        AutomationExecution execution, 
        IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            execution.Status = ExecutionStatus.Running;
            execution.StartedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            
            var automation = execution.Automation;
            var paciente = await context.Pacientes.FindAsync(execution.PacienteId);
            
            // Verificar segmentação
            if (!await IsPatientInSegmentAsync(paciente, automation.SegmentFilter))
            {
                execution.Status = ExecutionStatus.Skipped;
                execution.CompletedAt = DateTime.UtcNow;
                execution.Result = "Patient not in segment";
                await context.SaveChangesAsync();
                return;
            }
            
            // Executar ações em ordem
            foreach (var action in automation.Actions.OrderBy(a => a.Order))
            {
                await ExecuteActionAsync(action, paciente, serviceProvider);
            }
            
            execution.Status = ExecutionStatus.Completed;
            execution.CompletedAt = DateTime.UtcNow;
            execution.Result = "Success";
            
            automation.TimesExecuted++;
            automation.LastExecutedAt = DateTime.UtcNow;
            
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing automation {AutomationId}", execution.AutomationId);
            execution.Status = ExecutionStatus.Failed;
            execution.CompletedAt = DateTime.UtcNow;
            execution.Result = $"Error: {ex.Message}";
            await context.SaveChangesAsync();
        }
    }
    
    private async Task ExecuteActionAsync(
        AutomationAction action, 
        Paciente paciente, 
        IServiceProvider serviceProvider)
    {
        switch (action.Type)
        {
            case ActionType.SendEmail:
                var emailService = serviceProvider.GetRequiredService<IEmailService>();
                var emailTemplate = await RenderEmailTemplateAsync(action.EmailTemplate, paciente);
                await emailService.SendAsync(paciente.Email, emailTemplate.Subject, emailTemplate.HtmlBody);
                break;
                
            case ActionType.SendSMS:
                var smsService = serviceProvider.GetRequiredService<ISmsService>();
                var smsMessage = RenderTemplate(action.MessageTemplate, paciente);
                await smsService.SendAsync(paciente.Telefone, smsMessage);
                break;
                
            case ActionType.SendWhatsApp:
                var whatsAppService = serviceProvider.GetRequiredService<IWhatsAppService>();
                var whatsAppMessage = RenderTemplate(action.MessageTemplate, paciente);
                await whatsAppService.SendAsync(paciente.Celular, whatsAppMessage);
                break;
                
            case ActionType.AddTag:
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                var journey = await context.PatientJourneys
                    .FirstOrDefaultAsync(j => j.PacienteId == paciente.Id);
                if (journey != null)
                {
                    // Add tag logic
                }
                break;
        }
    }
}
```

#### 2.3 Frontend - Construtor de Automações
```typescript
// frontend/src/components/CRM/AutomationBuilder.tsx
import React, { useState } from 'react';
import { Card, Form, Select, Input, Button, Space, Divider, Steps } from 'antd';
import { PlusOutlined, DeleteOutlined } from '@ant-design/icons';

interface AutomationBuilderProps {
  onSave: (automation: any) => void;
}

export const AutomationBuilder: React.FC<AutomationBuilderProps> = ({ onSave }) => {
  const [form] = Form.useForm();
  const [currentStep, setCurrentStep] = useState(0);
  const [actions, setActions] = useState<any[]>([]);

  const addAction = () => {
    setActions([...actions, { type: 'SendEmail', order: actions.length }]);
  };

  const removeAction = (index: number) => {
    setActions(actions.filter((_, i) => i !== index));
  };

  const steps = [
    {
      title: 'Configuração',
      content: (
        <>
          <Form.Item label="Nome" name="name" rules={[{ required: true }]}>
            <Input placeholder="Ex: Boas-vindas Novos Pacientes" />
          </Form.Item>
          
          <Form.Item label="Descrição" name="description">
            <Input.TextArea rows={3} />
          </Form.Item>
          
          <Form.Item label="Tipo de Gatilho" name="triggerType" rules={[{ required: true }]}>
            <Select>
              <Select.Option value="StageChange">Mudança de Estágio</Select.Option>
              <Select.Option value="Event">Evento</Select.Option>
              <Select.Option value="Scheduled">Agendado</Select.Option>
              <Select.Option value="BehaviorBased">Comportamento</Select.Option>
              <Select.Option value="DateBased">Data Específica</Select.Option>
            </Select>
          </Form.Item>
          
          <Form.Item label="Estágio" name="triggerStage">
            <Select>
              <Select.Option value="Descoberta">Descoberta</Select.Option>
              <Select.Option value="Consideracao">Consideração</Select.Option>
              <Select.Option value="PrimeiraConsulta">Primeira Consulta</Select.Option>
              <Select.Option value="Tratamento">Tratamento</Select.Option>
              <Select.Option value="Retorno">Retorno</Select.Option>
              <Select.Option value="Fidelizacao">Fidelização</Select.Option>
              <Select.Option value="Advocacia">Advocacia</Select.Option>
            </Select>
          </Form.Item>
          
          <Form.Item label="Delay (minutos)" name="delayMinutes">
            <Input type="number" placeholder="0" />
          </Form.Item>
        </>
      )
    },
    {
      title: 'Segmentação',
      content: (
        <>
          <Form.Item label="Filtro de Idade" name="ageFilter">
            <Select mode="multiple">
              <Select.Option value="0-18">0-18 anos</Select.Option>
              <Select.Option value="19-35">19-35 anos</Select.Option>
              <Select.Option value="36-60">36-60 anos</Select.Option>
              <Select.Option value="60+">60+ anos</Select.Option>
            </Select>
          </Form.Item>
          
          <Form.Item label="Convênios" name="convenios">
            <Select mode="multiple">
              <Select.Option value="particular">Particular</Select.Option>
              <Select.Option value="unimed">Unimed</Select.Option>
              <Select.Option value="sulamerica">Sulamérica</Select.Option>
            </Select>
          </Form.Item>
          
          <Form.Item label="Tags" name="tags">
            <Select mode="tags" placeholder="Adicione tags">
              <Select.Option value="vip">VIP</Select.Option>
              <Select.Option value="faltante">Faltante</Select.Option>
              <Select.Option value="inadimplente">Inadimplente</Select.Option>
            </Select>
          </Form.Item>
        </>
      )
    },
    {
      title: 'Ações',
      content: (
        <>
          {actions.map((action, index) => (
            <Card 
              key={index} 
              size="small" 
              style={{ marginBottom: 16 }}
              extra={
                <Button 
                  type="text" 
                  danger 
                  icon={<DeleteOutlined />} 
                  onClick={() => removeAction(index)}
                />
              }
            >
              <Form.Item label="Tipo de Ação">
                <Select 
                  value={action.type}
                  onChange={(value) => {
                    const newActions = [...actions];
                    newActions[index].type = value;
                    setActions(newActions);
                  }}
                >
                  <Select.Option value="SendEmail">Enviar Email</Select.Option>
                  <Select.Option value="SendSMS">Enviar SMS</Select.Option>
                  <Select.Option value="SendWhatsApp">Enviar WhatsApp</Select.Option>
                  <Select.Option value="AddTag">Adicionar Tag</Select.Option>
                  <Select.Option value="CreateTask">Criar Tarefa</Select.Option>
                </Select>
              </Form.Item>
              
              {action.type === 'SendEmail' && (
                <Form.Item label="Template de Email">
                  <Select placeholder="Selecione um template">
                    <Select.Option value="welcome">Boas-vindas</Select.Option>
                    <Select.Option value="reminder">Lembrete</Select.Option>
                    <Select.Option value="followup">Follow-up</Select.Option>
                  </Select>
                </Form.Item>
              )}
              
              {(action.type === 'SendSMS' || action.type === 'SendWhatsApp') && (
                <Form.Item label="Mensagem">
                  <Input.TextArea 
                    rows={3}
                    placeholder="Use {{nome_paciente}}, {{data_consulta}}, etc."
                  />
                </Form.Item>
              )}
              
              {action.type === 'AddTag' && (
                <Form.Item label="Tag">
                  <Input placeholder="Ex: contato_realizado" />
                </Form.Item>
              )}
            </Card>
          ))}
          
          <Button 
            type="dashed" 
            onClick={addAction} 
            block 
            icon={<PlusOutlined />}
          >
            Adicionar Ação
          </Button>
        </>
      )
    }
  ];

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      const automation = {
        ...values,
        actions,
        isActive: true
      };
      onSave(automation);
    } catch (error) {
      console.error('Validation failed:', error);
    }
  };

  return (
    <Card title="Criar Automação de Marketing">
      <Steps current={currentStep} style={{ marginBottom: 24 }}>
        {steps.map(step => (
          <Steps.Step key={step.title} title={step.title} />
        ))}
      </Steps>
      
      <Form form={form} layout="vertical">
        {steps[currentStep].content}
      </Form>
      
      <Divider />
      
      <Space>
        {currentStep > 0 && (
          <Button onClick={() => setCurrentStep(currentStep - 1)}>
            Anterior
          </Button>
        )}
        
        {currentStep < steps.length - 1 && (
          <Button type="primary" onClick={() => setCurrentStep(currentStep + 1)}>
            Próximo
          </Button>
        )}
        
        {currentStep === steps.length - 1 && (
          <Button type="primary" onClick={handleSave}>
            Salvar Automação
          </Button>
        )}
      </Space>
    </Card>
  );
};
```

---

### 3. Sistema NPS/CSAT (3 semanas)

#### 3.1 Modelo de Pesquisas
```csharp
// src/MedicSoft.Core/Entities/CRM/Survey.cs
public class Survey
{
    public Guid Id { get; set; }
    public SurveyType Type { get; set; } // NPS, CSAT, CES
    public string Name { get; set; }
    public bool IsActive { get; set; }
    
    // Configuração de envio
    public SurveyTrigger Trigger { get; set; }
    public int DelayHours { get; set; }
    public List<string> Channels { get; set; } // Email, SMS, WhatsApp
    
    // Perguntas
    public List<SurveyQuestion> Questions { get; set; }
    
    // Métricas
    public int TotalSent { get; set; }
    public int TotalResponses { get; set; }
    public double ResponseRate => TotalSent > 0 ? (double)TotalResponses / TotalSent : 0;
}

public enum SurveyType
{
    NPS,   // Net Promoter Score (0-10)
    CSAT,  // Customer Satisfaction (1-5)
    CES    // Customer Effort Score (1-7)
}

public enum SurveyTrigger
{
    AfterAppointment,
    AfterTreatment,
    AfterPayment,
    Periodic,
    Manual
}

public class SurveyQuestion
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    
    // Para perguntas de múltipla escolha
    public List<string> Options { get; set; }
}

public enum QuestionType
{
    NPSScale,      // 0-10
    CSATScale,     // 1-5 stars
    MultipleChoice,
    OpenText,
    YesNo
}

public class SurveyResponse
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public Survey Survey { get; set; }
    
    public Guid PacienteId { get; set; }
    public Paciente Paciente { get; set; }
    
    public Guid? ConsultaId { get; set; }
    public Consulta Consulta { get; set; }
    
    public DateTime SentAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string Channel { get; set; }
    
    // Respostas
    public List<QuestionResponse> Responses { get; set; }
    
    // Score calculado
    public int? NPSScore { get; set; }
    public int? CSATScore { get; set; }
    public NPSCategory? Category { get; set; } // Detractor, Passive, Promoter
    
    // Análise de sentimento
    public SentimentAnalysis Sentiment { get; set; }
}

public enum NPSCategory
{
    Detractor = 1,  // 0-6
    Passive = 2,    // 7-8
    Promoter = 3    // 9-10
}

public class QuestionResponse
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public SurveyQuestion Question { get; set; }
    
    public int? NumericAnswer { get; set; }
    public string TextAnswer { get; set; }
    public List<string> SelectedOptions { get; set; }
}
```

#### 3.2 Serviço de NPS
```csharp
// src/MedicSoft.Api/Services/CRM/NPSService.cs
public class NPSService : INPSService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    
    public async Task SendPostAppointmentSurveyAsync(Guid consultaId)
    {
        var consulta = await _context.Consultas
            .Include(c => c.Paciente)
            .FirstOrDefaultAsync(c => c.Id == consultaId);
            
        if (consulta == null) return;
        
        var survey = await _context.Surveys
            .Where(s => s.Type == SurveyType.NPS 
                     && s.Trigger == SurveyTrigger.AfterAppointment 
                     && s.IsActive)
            .Include(s => s.Questions)
            .FirstOrDefaultAsync();
            
        if (survey == null) return;
        
        // Criar resposta pendente
        var response = new SurveyResponse
        {
            SurveyId = survey.Id,
            PacienteId = consulta.PacienteId,
            ConsultaId = consultaId,
            SentAt = DateTime.UtcNow,
            Channel = "Email"
        };
        
        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();
        
        // Gerar link único
        var surveyLink = $"https://app.medicsoft.com/survey/{response.Id}";
        
        // Enviar email
        await _emailService.SendAsync(
            consulta.Paciente.Email,
            "Como foi sua experiência?",
            GenerateSurveyEmailHtml(consulta.Paciente, surveyLink)
        );
        
        survey.TotalSent++;
        await _context.SaveChangesAsync();
    }
    
    public async Task<NPSMetrics> CalculateNPSAsync(DateTime startDate, DateTime endDate)
    {
        var responses = await _context.SurveyResponses
            .Where(r => r.Survey.Type == SurveyType.NPS
                     && r.RespondedAt >= startDate
                     && r.RespondedAt <= endDate
                     && r.NPSScore.HasValue)
            .ToListAsync();
            
        var promoters = responses.Count(r => r.NPSScore >= 9);
        var passives = responses.Count(r => r.NPSScore >= 7 && r.NPSScore <= 8);
        var detractors = responses.Count(r => r.NPSScore <= 6);
        var total = responses.Count;
        
        var npsScore = total > 0 
            ? ((double)(promoters - detractors) / total) * 100 
            : 0;
            
        return new NPSMetrics
        {
            Score = Math.Round(npsScore, 1),
            Promoters = promoters,
            Passives = passives,
            Detractors = detractors,
            TotalResponses = total,
            PromotersPercent = (double)promoters / total * 100,
            PassivesPercent = (double)passives / total * 100,
            DetractorsPercent = (double)detractors / total * 100
        };
    }
    
    public async Task<List<SurveyInsight>> GetInsightsAsync(DateTime startDate, DateTime endDate)
    {
        var insights = new List<SurveyInsight>();
        
        // Insight 1: Principais reclamações de detratores
        var detractorComments = await _context.SurveyResponses
            .Where(r => r.Category == NPSCategory.Detractor
                     && r.RespondedAt >= startDate
                     && r.RespondedAt <= endDate)
            .SelectMany(r => r.Responses)
            .Where(qr => !string.IsNullOrEmpty(qr.TextAnswer))
            .Select(qr => qr.TextAnswer)
            .ToListAsync();
            
        var commonIssues = ExtractCommonTopics(detractorComments);
        insights.Add(new SurveyInsight
        {
            Title = "Principais Reclamações",
            Type = "Detractor",
            Topics = commonIssues
        });
        
        // Insight 2: O que promotores mais elogiam
        var promoterComments = await _context.SurveyResponses
            .Where(r => r.Category == NPSCategory.Promoter
                     && r.RespondedAt >= startDate
                     && r.RespondedAt <= endDate)
            .SelectMany(r => r.Responses)
            .Where(qr => !string.IsNullOrEmpty(qr.TextAnswer))
            .Select(qr => qr.TextAnswer)
            .ToListAsync();
            
        var strengths = ExtractCommonTopics(promoterComments);
        insights.Add(new SurveyInsight
        {
            Title = "Principais Elogios",
            Type = "Promoter",
            Topics = strengths
        });
        
        return insights;
    }
}
```

---

### 4. Ouvidoria (2 semanas)

#### 4.1 Modelo de Ouvidoria
```csharp
// src/MedicSoft.Core/Entities/CRM/Complaint.cs
public class Complaint
{
    public Guid Id { get; set; }
    public string Protocol { get; set; } // Protocolo único
    
    public Guid PacienteId { get; set; }
    public Paciente Paciente { get; set; }
    
    // Classificação
    public ComplaintType Type { get; set; }
    public ComplaintSeverity Severity { get; set; }
    public ComplaintCategory Category { get; set; }
    
    // Conteúdo
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> Attachments { get; set; }
    
    // Fluxo
    public ComplaintStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int SLA { get; set; } // Prazo em horas
    public bool IsOverdue => DateTime.UtcNow > CreatedAt.AddHours(SLA) && Status != ComplaintStatus.Resolved;
    
    // Atribuição
    public Guid? AssignedToId { get; set; }
    public Usuario AssignedTo { get; set; }
    public string Department { get; set; }
    
    // Resposta
    public List<ComplaintResponse> Responses { get; set; }
    public string Resolution { get; set; }
    public int? SatisfactionRating { get; set; } // 1-5 após resolução
    
    // Análise
    public SentimentAnalysis Sentiment { get; set; }
}

public enum ComplaintType
{
    Reclamacao,
    Sugestao,
    Elogio,
    Duvida
}

public enum ComplaintSeverity
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum ComplaintCategory
{
    Atendimento,
    Medico,
    Infraestrutura,
    Financeiro,
    Agendamento,
    Medicamento,
    Outro
}

public enum ComplaintStatus
{
    New,
    InProgress,
    WaitingPatient,
    Resolved,
    Closed
}

public class ComplaintResponse
{
    public Guid Id { get; set; }
    public Guid ComplaintId { get; set; }
    public Guid UserId { get; set; }
    public Usuario User { get; set; }
    
    public string Message { get; set; }
    public bool IsInternal { get; set; } // Visível apenas internamente
    public DateTime CreatedAt { get; set; }
    
    public List<string> Attachments { get; set; }
}
```

---

### 5. Análise de Sentimento com IA (2 semanas)

#### 5.1 Integração com Azure Cognitive Services
```csharp
// src/MedicSoft.Api/Services/CRM/SentimentAnalysisService.cs
using Azure.AI.TextAnalytics;

public class SentimentAnalysisService : ISentimentAnalysisService
{
    private readonly TextAnalyticsClient _client;
    private readonly ILogger<SentimentAnalysisService> _logger;
    
    public SentimentAnalysisService(IConfiguration configuration, ILogger<SentimentAnalysisService> logger)
    {
        var endpoint = configuration["Azure:CognitiveServices:Endpoint"];
        var key = configuration["Azure:CognitiveServices:Key"];
        _client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(key));
        _logger = logger;
    }
    
    public async Task<SentimentAnalysis> AnalyzeTextAsync(string text)
    {
        try
        {
            var response = await _client.AnalyzeSentimentAsync(text, language: "pt");
            var sentiment = response.Value;
            
            return new SentimentAnalysis
            {
                OverallSentiment = MapSentiment(sentiment.Sentiment),
                PositiveScore = sentiment.ConfidenceScores.Positive,
                NeutralScore = sentiment.ConfidenceScores.Neutral,
                NegativeScore = sentiment.ConfidenceScores.Negative,
                KeyPhrases = await ExtractKeyPhrasesAsync(text),
                Entities = await ExtractEntitiesAsync(text)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing sentiment");
            return null;
        }
    }
    
    private async Task<List<string>> ExtractKeyPhrasesAsync(string text)
    {
        var response = await _client.ExtractKeyPhrasesAsync(text, language: "pt");
        return response.Value.ToList();
    }
    
    private async Task<List<string>> ExtractEntitiesAsync(string text)
    {
        var response = await _client.RecognizeEntitiesAsync(text, language: "pt");
        return response.Value.Select(e => e.Text).ToList();
    }
}

public class SentimentAnalysis
{
    public SentimentType OverallSentiment { get; set; }
    public double PositiveScore { get; set; }
    public double NeutralScore { get; set; }
    public double NegativeScore { get; set; }
    public List<string> KeyPhrases { get; set; }
    public List<string> Entities { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

public enum SentimentType
{
    Positive,
    Neutral,
    Negative,
    Mixed
}
```

---

### 6. Predição de Churn (3 semanas)

#### 6.1 Modelo de Churn
```csharp
// src/MedicSoft.Api/Services/CRM/ChurnPredictionService.cs
using Microsoft.ML;
using Microsoft.ML.Data;

public class ChurnPredictionService : IChurnPredictionService
{
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private readonly ApplicationDbContext _context;
    
    public class ChurnData
    {
        public float DaysSinceLastAppointment { get; set; }
        public float TotalAppointments { get; set; }
        public float NoShowRate { get; set; }
        public float AverageRating { get; set; }
        public float TotalComplaints { get; set; }
        public float NPSScore { get; set; }
        public float DaysSinceRegistration { get; set; }
        public float LifetimeValue { get; set; }
        public bool HasChurned { get; set; } // Label
    }
    
    public class ChurnPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool WillChurn { get; set; }
        
        public float Probability { get; set; }
        public float Score { get; set; }
    }
    
    public async Task TrainModelAsync()
    {
        var trainingData = await PrepareTrainingDataAsync();
        var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
        
        var pipeline = _mlContext.Transforms.Concatenate("Features",
                nameof(ChurnData.DaysSinceLastAppointment),
                nameof(ChurnData.TotalAppointments),
                nameof(ChurnData.NoShowRate),
                nameof(ChurnData.AverageRating),
                nameof(ChurnData.TotalComplaints),
                nameof(ChurnData.NPSScore),
                nameof(ChurnData.DaysSinceRegistration),
                nameof(ChurnData.LifetimeValue))
            .Append(_mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: nameof(ChurnData.HasChurned)))
            .Append(_mlContext.Transforms.CopyColumns("PredictedLabel", "PredictedLabel"));
        
        _model = pipeline.Fit(dataView);
    }
    
    public async Task<ChurnPrediction> PredictChurnAsync(Guid pacienteId)
    {
        var paciente = await _context.Pacientes.FindAsync(pacienteId);
        var features = await ExtractFeaturesAsync(paciente);
        
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<ChurnData, ChurnPrediction>(_model);
        return predictionEngine.Predict(features);
    }
    
    public async Task<List<PatientChurnRisk>> GetHighRiskPatientsAsync()
    {
        var patients = await _context.Pacientes.ToListAsync();
        var risks = new List<PatientChurnRisk>();
        
        foreach (var patient in patients)
        {
            var prediction = await PredictChurnAsync(patient.Id);
            
            if (prediction.Probability > 0.7) // Alta probabilidade de churn
            {
                risks.Add(new PatientChurnRisk
                {
                    PacienteId = patient.Id,
                    Nome = patient.Nome,
                    ChurnProbability = prediction.Probability,
                    RiskLevel = GetRiskLevel(prediction.Probability),
                    RecommendedActions = GenerateRecommendations(patient, prediction)
                });
            }
        }
        
        return risks.OrderByDescending(r => r.ChurnProbability).ToList();
    }
}
```

---

## 📝 Tarefas de Implementação

### Sprint 1: Patient Journey (Semanas 1-5)
- [ ] Criar entidades de jornada e touchpoints
- [ ] Implementar serviço de tracking
- [ ] Desenvolver lógica de transição de estágios
- [ ] Criar dashboard de visualização
- [ ] Implementar API endpoints
- [ ] Testes unitários e integração

### Sprint 2: Automação de Marketing (Semanas 6-9)
- [ ] Criar modelo de automações
- [ ] Implementar engine de execução
- [ ] Desenvolver construtor visual
- [ ] Integrar com serviços de envio
- [ ] Implementar templates
- [ ] Sistema de variáveis dinâmicas

### Sprint 3: NPS/CSAT (Semanas 10-12)
- [ ] Criar modelo de pesquisas
- [ ] Implementar triggers automáticos
- [ ] Desenvolver interface de resposta
- [ ] Sistema de cálculo de métricas
- [ ] Dashboard de análise
- [ ] Insights automáticos

### Sprint 4: Ouvidoria (Semanas 13-14)
- [ ] Modelo de reclamações
- [ ] Sistema de protocolos
- [ ] Fluxo de atendimento
- [ ] Portal do paciente
- [ ] Dashboard de gestão
- [ ] Relatórios de SLA

### Sprint 5: IA e Sentimento (Semanas 15-16)
- [ ] Integração Azure Cognitive Services
- [ ] Análise de comentários
- [ ] Extração de tópicos
- [ ] Classificação automática
- [ ] Alertas de sentimento negativo

### Sprint 6: Predição de Churn (Semanas 17-19)
- [ ] Preparação de dados de treinamento
- [ ] Treinamento de modelo ML.NET
- [ ] Serviço de predição
- [ ] Dashboard de riscos
- [ ] Ações recomendadas
- [ ] Monitoramento contínuo

### Sprint 7: Integrações (Semana 20)
- [ ] Email (SendGrid/AWS SES)
- [ ] SMS (Twilio)
- [ ] WhatsApp Business API
- [ ] Webhooks
- [ ] Testes end-to-end

---

## 🧪 Testes

### Testes Unitários
```csharp
// tests/MedicSoft.Tests/CRM/PatientJourneyServiceTests.cs
public class PatientJourneyServiceTests
{
    [Fact]
    public async Task AdvanceStage_ShouldCloseCurrentAndOpenNew()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        using var context = new ApplicationDbContext(options);
        var service = new PatientJourneyService(context, Mock.Of<ILogger<PatientJourneyService>>());
        
        var pacienteId = Guid.NewGuid();
        await service.GetOrCreateJourneyAsync(pacienteId);
        
        // Act
        await service.AdvanceStageAsync(pacienteId, JourneyStageEnum.PrimeiraConsulta, "Consulta agendada");
        
        // Assert
        var journey = await service.GetOrCreateJourneyAsync(pacienteId);
        Assert.Equal(JourneyStageEnum.PrimeiraConsulta, journey.CurrentStage);
        Assert.Equal(2, journey.Stages.Count);
        Assert.NotNull(journey.Stages[0].ExitedAt);
    }
}
```

### Testes de Integração
```csharp
[Fact]
public async Task AutomationEngine_ShouldExecuteOnTrigger()
{
    // Test automation execution flow
}

[Fact]
public async Task NPSService_ShouldCalculateCorrectScore()
{
    // Test NPS calculation
}
```

---

## 📊 Métricas de Sucesso

### KPIs Principais
- **NPS Score:** > 50 (Excelente)
- **CSAT Score:** > 4.5/5
- **Taxa de Resposta:** > 30%
- **Churn Rate:** < 10% (redução de 30%)
- **Retenção:** > 85%
- **LTV médio:** Aumento de 40%

### Métricas de Automação
- **Taxa de Abertura (Email):** > 40%
- **Taxa de Clique:** > 15%
- **Conversão de Campanhas:** > 5%
- **Tempo de Resposta Ouvidoria:** < 24h
- **Resolução Primeira Interação:** > 60%

### Métricas Operacionais
- **Touchpoints/Paciente/Mês:** > 3
- **Precisão Predição Churn:** > 80%
- **Automações Ativas:** > 10
- **Tempo Médio Jornada:** Redução de 20%

---

## 🚀 Deploy e Rollout

### Fase 1: Piloto (Semanas 1-2)
- Ativar para 100 pacientes selecionados
- Coletar feedback inicial
- Ajustar automações

### Fase 2: Expansão Gradual (Semanas 3-4)
- 25% dos pacientes ativos
- Monitorar performance
- Otimizar triggers

### Fase 3: Full Rollout (Semana 5)
- 100% dos pacientes
- Todas automações ativas
- Monitoramento contínuo

---

## 📚 Documentação Necessária

1. **Manual de Usuário:**
   - Como criar automações
   - Interpretação de métricas NPS
   - Gestão de ouvidoria

2. **Guia de Configuração:**
   - Integração de canais
   - Templates de email
   - Regras de segmentação

3. **Documentação Técnica:**
   - Arquitetura do sistema
   - API de automações
   - Modelo de ML de churn

4. **Playbook de CRM:**
   - Melhores práticas
   - Estratégias de retenção
   - Gestão de reclamações

---

## 💰 ROI Esperado

### Investimento
- **Desenvolvimento:** R$ 110.000
- **Azure Cognitive Services:** R$ 500/mês
- **SendGrid/Twilio:** R$ 1.000/mês
- **WhatsApp Business:** R$ 800/mês
- **Total Ano 1:** R$ 137.600

### Retorno Estimado (Ano 1)

#### Redução de Churn
- Churn atual: 15% (450 pacientes/ano)
- Redução esperada: 30%
- Pacientes retidos: 135
- LTV médio: R$ 2.500
- **Ganho: R$ 337.500**

#### Aumento de Retenção
- Retenção atual: 75%
- Retenção esperada: 85%
- Aumento: 10% (300 pacientes)
- LTV adicional: R$ 2.500
- **Ganho: R$ 750.000**

#### Eficiência Operacional
- Automação de follow-ups: 20h/semana
- Custo hora: R$ 50
- **Economia: R$ 52.000/ano**

#### Marketing Mais Efetivo
- Taxa conversão atual: 2%
- Taxa conversão esperada: 5%
- Aumento conversão: 150%
- Novos pacientes: 450
- Receita por paciente: R$ 800
- **Ganho: R$ 360.000**

### ROI Total
- **Ganho Total:** R$ 1.499.500
- **Investimento:** R$ 137.600
- **ROI:** 989%
- **Payback:** 1,1 meses

---

## 🎯 Próximos Passos

1. **Imediato:**
   - Aprovar arquitetura
   - Definir priorização de features
   - Configurar Azure Cognitive Services
   - Contratar serviços de comunicação

2. **Curto Prazo:**
   - Sprint 1: Patient Journey
   - Configurar integrações
   - Preparar templates

3. **Médio Prazo:**
   - Treinar modelo de churn
   - Criar automações
   - Implementar NPS

4. **Longo Prazo:**
   - Otimização contínua
   - Novos canais
   - Advanced analytics
