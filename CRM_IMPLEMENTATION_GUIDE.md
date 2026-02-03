# 📘 Guia de Implementação do CRM Avançado

## Visão Geral

Este documento descreve a implementação do sistema **CRM Avançado e Customer Experience** do Omni Care, conforme especificado no prompt 17 do Plano de Desenvolvimento (fase-4-analytics-otimizacao).

## 🎯 Objetivos do Sistema

O CRM Avançado implementa:
- **Patient Journey Mapping**: Mapeamento completo da jornada do paciente em 7 estágios
- **Automação de Marketing**: Campanhas automáticas baseadas em comportamento e estágios
- **NPS/CSAT**: Pesquisas de satisfação automatizadas
- **Ouvidoria**: Sistema de gestão de reclamações e feedback
- **Análise de Sentimento com IA**: Usando Azure Cognitive Services
- **Predição de Churn**: Modelo de ML para identificar pacientes em risco

## 📊 Arquitetura

### Estrutura de Domínio

Todas as entidades estão em `src/MedicSoft.Domain/Entities/CRM/`:

```
CRM/
├── Enumerações
│   ├── JourneyStageEnum.cs
│   ├── TouchpointType.cs
│   ├── TouchpointDirection.cs
│   ├── ChurnRiskLevel.cs
│   ├── AutomationTriggerType.cs
│   ├── ActionType.cs
│   ├── SurveyType.cs
│   ├── QuestionType.cs
│   ├── ComplaintStatus.cs
│   ├── ComplaintCategory.cs
│   ├── ComplaintPriority.cs
│   └── SentimentType.cs
│
├── Patient Journey
│   ├── PatientJourney.cs
│   ├── JourneyStage.cs
│   └── PatientTouchpoint.cs
│
├── Marketing Automation
│   ├── MarketingAutomation.cs
│   ├── AutomationAction.cs
│   └── EmailTemplate.cs
│
├── Surveys
│   ├── Survey.cs
│   ├── SurveyQuestion.cs
│   ├── SurveyResponse.cs
│   └── SurveyQuestionResponse.cs
│
├── Ouvidoria
│   ├── Complaint.cs
│   └── ComplaintInteraction.cs
│
└── IA/ML
    ├── SentimentAnalysis.cs
    └── ChurnPrediction.cs
```

## 🚀 Módulos Implementados

### 1. Patient Journey Mapping

#### Conceito
Mapeia a jornada completa do paciente através de 7 estágios distintos:

1. **Descoberta** - Lead capture, marketing inicial
2. **Consideração** - Avaliando opções, comparando
3. **Primeira Consulta** - Primeiro atendimento
4. **Tratamento** - Durante o tratamento
5. **Retorno** - Consultas de retorno
6. **Fidelização** - Cliente recorrente
7. **Advocacia** - Promotor da marca

#### Entidades Principais

**PatientJourney**
```csharp
public class PatientJourney : BaseEntity
{
    public Guid PacienteId { get; private set; }
    public IReadOnlyCollection<JourneyStage> Stages { get; }
    public JourneyStageEnum CurrentStage { get; private set; }
    
    // Métricas
    public int TotalTouchpoints { get; private set; }
    public decimal LifetimeValue { get; private set; }
    public int NpsScore { get; private set; }
    public double SatisfactionScore { get; private set; }
    public ChurnRiskLevel ChurnRisk { get; private set; }
}
```

**JourneyStage**
```csharp
public class JourneyStage : BaseEntity
{
    public JourneyStageEnum Stage { get; private set; }
    public DateTime EnteredAt { get; private set; }
    public DateTime? ExitedAt { get; private set; }
    public int DurationDays { get; private set; }
    public IReadOnlyCollection<PatientTouchpoint> Touchpoints { get; }
    public string? ExitTrigger { get; private set; }
}
```

**PatientTouchpoint**
```csharp
public class PatientTouchpoint : BaseEntity
{
    public TouchpointType Type { get; private set; }
    public string Channel { get; private set; } // Email, SMS, WhatsApp, Phone, InPerson
    public string Description { get; private set; }
    public TouchpointDirection Direction { get; private set; } // Inbound/Outbound
    public Guid? SentimentAnalysisId { get; private set; }
}
```

#### Uso

```csharp
// Criar jornada para novo paciente
var journey = new PatientJourney(pacienteId, tenantId);

// Avançar para próximo estágio
journey.AdvanceToStage(JourneyStageEnum.PrimeiraConsulta, "Consulta agendada", tenantId);

// Adicionar touchpoint
var touchpoint = new PatientTouchpoint(
    stageId,
    TouchpointType.EmailInteraction,
    "Email",
    "Email de confirmação enviado",
    TouchpointDirection.Outbound,
    tenantId
);
journey.AddTouchpoint(touchpoint);

// Atualizar métricas
journey.UpdateMetrics(
    lifetimeValue: 5000m,
    npsScore: 9,
    satisfactionScore: 4.5,
    churnRisk: ChurnRiskLevel.Low
);
```

### 2. Automação de Marketing

#### Conceito
Sistema de automação que permite criar campanhas baseadas em:
- Mudanças de estágio na jornada
- Eventos específicos (consulta agendada, no-show, etc)
- Agendamentos
- Comportamento do paciente
- Datas especiais (aniversário, etc)

#### Entidades Principais

**MarketingAutomation**
```csharp
public class MarketingAutomation : BaseEntity
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public AutomationTriggerType TriggerType { get; private set; }
    public JourneyStageEnum? TriggerStage { get; private set; }
    public int? DelayMinutes { get; private set; }
    public IReadOnlyCollection<AutomationAction> Actions { get; }
    
    // Métricas
    public int TimesExecuted { get; private set; }
    public double SuccessRate { get; private set; }
}
```

**AutomationAction**
```csharp
public class AutomationAction : BaseEntity
{
    public int Order { get; private set; }
    public ActionType Type { get; private set; } // SendEmail, SendSMS, SendWhatsApp, etc
    public Guid? EmailTemplateId { get; private set; }
    public string? MessageTemplate { get; private set; }
    public string? Channel { get; private set; }
}
```

**EmailTemplate**
```csharp
public class EmailTemplate : BaseEntity
{
    public string Name { get; private set; }
    public string Subject { get; private set; }
    public string HtmlBody { get; private set; }
    public IReadOnlyCollection<string> AvailableVariables { get; }
}
```

#### Uso

```csharp
// Criar automação
var automation = new MarketingAutomation(
    "Boas-vindas Novo Paciente",
    "Enviado após primeira consulta",
    AutomationTriggerType.StageChange,
    tenantId
);

// Configurar trigger
automation.ConfigureTrigger(
    triggerStage: JourneyStageEnum.PrimeiraConsulta,
    triggerEvent: null,
    delayMinutes: 60 // 1 hora depois
);

// Adicionar ação de email
var action = new AutomationAction(automation.Id, 1, ActionType.SendEmail, tenantId);
action.ConfigureEmailAction(emailTemplateId);
automation.AddAction(action);

// Ativar
automation.Activate();
```

### 3. NPS/CSAT - Pesquisas de Satisfação

#### Conceito
Sistema de pesquisas automatizadas com suporte para:
- **NPS** (Net Promoter Score) - Escala 0-10
- **CSAT** (Customer Satisfaction Score) - Escala 1-5
- **CES** (Customer Effort Score)
- Pesquisas customizadas

#### Entidades Principais

**Survey**
```csharp
public class Survey : BaseEntity
{
    public string Name { get; private set; }
    public SurveyType Type { get; private set; } // NPS, CSAT, CES, Custom
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<SurveyQuestion> Questions { get; }
    
    // Configuração de disparo
    public JourneyStageEnum? TriggerStage { get; private set; }
    public int? DelayHours { get; private set; }
    
    // Métricas
    public double AverageScore { get; private set; }
    public int TotalResponses { get; private set; }
}
```

**SurveyResponse**
```csharp
public class SurveyResponse : BaseEntity
{
    public Guid SurveyId { get; private set; }
    public Guid PatientId { get; private set; }
    public IReadOnlyCollection<SurveyQuestionResponse> QuestionResponses { get; }
    public bool IsCompleted { get; private set; }
    public int? NpsScore { get; private set; }
    public int? CsatScore { get; private set; }
}
```

#### Uso

```csharp
// Criar pesquisa NPS
var survey = new Survey(
    "NPS Pós-Consulta",
    "Pesquisa enviada após consulta",
    SurveyType.NPS,
    tenantId
);

// Configurar disparo automático
survey.ConfigureTrigger(
    triggerStage: JourneyStageEnum.PrimeiraConsulta,
    triggerEvent: "appointment_completed",
    delayHours: 24
);

// Adicionar questão NPS
var question = new SurveyQuestion(
    survey.Id,
    1,
    "Em uma escala de 0 a 10, quanto você recomendaria nossos serviços?",
    QuestionType.NumericScale,
    isRequired: true,
    tenantId
);
survey.AddQuestion(question);

// Ativar
survey.Activate();
```

### 4. Ouvidoria - Gestão de Reclamações

#### Conceito
Sistema completo de ouvidoria com:
- Protocolos únicos
- Categorização e priorização
- SLA tracking
- Histórico de interações
- Métricas de satisfação com resolução

#### Entidades Principais

**Complaint**
```csharp
public class Complaint : BaseEntity
{
    public string ProtocolNumber { get; private set; }
    public Guid PatientId { get; private set; }
    public string Subject { get; private set; }
    public ComplaintCategory Category { get; private set; }
    public ComplaintPriority Priority { get; private set; }
    public ComplaintStatus Status { get; private set; }
    
    // SLA
    public DateTime ReceivedAt { get; private set; }
    public DateTime? FirstResponseAt { get; private set; }
    public int? ResponseTimeMinutes { get; private set; }
    
    // Satisfação
    public int? SatisfactionRating { get; private set; }
}
```

#### Uso

```csharp
// Criar reclamação
var complaint = new Complaint(
    protocolNumber: "OUV-2026-00123",
    patientId: patientId,
    subject: "Demora no atendimento",
    description: "Esperei mais de 1 hora...",
    category: ComplaintCategory.WaitTime,
    tenantId
);

// Atribuir a um atendente
complaint.AssignTo(userId, "João Silva");

// Atualizar status
complaint.UpdateStatus(ComplaintStatus.InProgress);

// Adicionar interação
var interaction = new ComplaintInteraction(
    complaint.Id,
    userId,
    "João Silva",
    "Estamos investigando o ocorrido",
    isInternal: false,
    tenantId
);
complaint.AddInteraction(interaction);

// Resolver e coletar feedback
complaint.UpdateStatus(ComplaintStatus.Resolved);
complaint.RecordSatisfaction(rating: 4, feedback: "Problema resolvido rapidamente");
```

### 5. Análise de Sentimento com IA

#### Conceito
Integração com Azure Cognitive Services para análise automática de sentimento em:
- Comentários de pesquisas
- Textos de reclamações
- Interações via email/chat
- Redes sociais

#### Entidade Principal

**SentimentAnalysis**
```csharp
public class SentimentAnalysis : BaseEntity
{
    public string SourceText { get; private set; }
    public string SourceType { get; private set; }
    public SentimentType Sentiment { get; private set; } // Positive, Neutral, Negative, Mixed
    public double PositiveScore { get; private set; }
    public double NeutralScore { get; private set; }
    public double NegativeScore { get; private set; }
    public double ConfidenceScore { get; private set; }
    public IReadOnlyCollection<string> Topics { get; }
}
```

#### Uso

```csharp
// Criar análise
var analysis = new SentimentAnalysis(
    sourceText: "O atendimento foi excelente!",
    sourceType: "SurveyComment",
    sourceId: surveyResponseId,
    tenantId
);

// (Resultado viria do Azure Cognitive Services)
analysis.SetAnalysisResult(
    sentiment: SentimentType.Positive,
    positiveScore: 0.95,
    neutralScore: 0.03,
    negativeScore: 0.02,
    confidenceScore: 0.98
);

analysis.AddTopic("atendimento");
analysis.AddTopic("qualidade");
```

### 6. Predição de Churn

#### Conceito
Modelo de Machine Learning (ML.NET) que prediz probabilidade de churn baseado em:
- Dias desde última visita
- Total de visitas
- Lifetime value
- Score de satisfação
- Número de reclamações
- No-shows e cancelamentos

#### Entidade Principal

**ChurnPrediction**
```csharp
public class ChurnPrediction : BaseEntity
{
    public Guid PatientId { get; private set; }
    public double ChurnProbability { get; private set; } // 0-1
    public ChurnRiskLevel RiskLevel { get; private set; }
    public IReadOnlyCollection<string> RiskFactors { get; }
    public IReadOnlyCollection<string> RecommendedActions { get; }
    
    // Features
    public int DaysSinceLastVisit { get; private set; }
    public int TotalVisits { get; private set; }
    public decimal LifetimeValue { get; private set; }
}
```

#### Uso

```csharp
// Criar predição
var prediction = new ChurnPrediction(patientId, tenantId);

// Definir features
prediction.SetFeatures(
    daysSinceLastVisit: 90,
    totalVisits: 5,
    lifetimeValue: 2500m,
    averageSatisfactionScore: 3.2,
    complaintsCount: 2,
    noShowCount: 1,
    cancelledAppointmentsCount: 3
);

// (Resultado viria do modelo ML)
prediction.SetPrediction(
    churnProbability: 0.75,
    riskLevel: ChurnRiskLevel.High
);

// Adicionar fatores e ações
prediction.AddRiskFactor("Alto número de cancelamentos");
prediction.AddRiskFactor("Satisfação abaixo da média");
prediction.AddRecommendedAction("Entrar em contato via WhatsApp");
prediction.AddRecommendedAction("Oferecer desconto na próxima consulta");
```

## 🔧 Próximos Passos de Implementação

### Fase 1: Database e Migrations
1. Criar DbContext configurations para todas as entidades
2. Gerar migrations no Entity Framework Core
3. Configurar relacionamentos e índices

### Fase 2: Application Layer
1. Criar DTOs para todas as entidades
2. Implementar Commands (CQRS)
3. Implementar Queries (CQRS)
4. Criar Handlers

### Fase 3: Services
1. PatientJourneyService
2. MarketingAutomationEngine
3. SurveyService
4. ComplaintService
5. SentimentAnalysisService (Azure)
6. ChurnPredictionService (ML.NET)

### Fase 4: API Controllers
1. PatientJourneyController
2. MarketingAutomationController
3. SurveyController
4. ComplaintController
5. CrmAnalyticsController

### Fase 5: Frontend
1. Journey Timeline Component
2. Automation Builder
3. Survey Creator
4. Complaint Portal
5. Dashboards

## 📚 Recursos Adicionais

- [Manual do Usuário](./CRM_USER_MANUAL.md)
- [Documentação da API](./CRM_API_DOCUMENTATION.md)
- [Prompt Original](./Plano_Desenvolvimento/fase-4-analytics-otimizacao/17-crm-avancado.md)

## 🤝 Contribuindo

Para contribuir com o desenvolvimento do CRM:
1. Siga os padrões de código do projeto
2. Adicione testes unitários para novas funcionalidades
3. Atualize a documentação
4. Submeta Pull Request para revisão

## 📞 Suporte

Para questões sobre a implementação, consulte:
- Time de Desenvolvimento Omni Care
- Documentação técnica no repositório
- Issues no GitHub
