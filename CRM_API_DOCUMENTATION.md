# 🔌 CRM Advanced - API Documentation

## Base URL
```
https://api.primecare.com.br/api/v1/crm
```

## Authentication
All endpoints require Bearer token authentication:
```http
Authorization: Bearer {your_token}
```

---

## 📊 Patient Journey API

### Get Patient Journey
Get the complete journey for a patient.

```http
GET /patient-journey/{patientId}
```

**Response:**
```json
{
  "id": "uuid",
  "pacienteId": "uuid",
  "currentStage": "PrimeiraConsulta",
  "totalTouchpoints": 15,
  "lifetimeValue": 5000.00,
  "npsScore": 9,
  "satisfactionScore": 4.5,
  "churnRisk": "Low",
  "stages": [
    {
      "id": "uuid",
      "stage": "Descoberta",
      "enteredAt": "2026-01-01T10:00:00Z",
      "exitedAt": "2026-01-15T14:30:00Z",
      "durationDays": 14,
      "exitTrigger": "Consulta agendada",
      "touchpoints": [...]
    }
  ]
}
```

### Advance Journey Stage
Move patient to next journey stage.

```http
POST /patient-journey/{patientId}/advance
```

**Request Body:**
```json
{
  "newStage": "PrimeiraConsulta",
  "trigger": "Consulta realizada"
}
```

### Add Touchpoint
Register a patient interaction.

```http
POST /patient-journey/{patientId}/touchpoints
```

**Request Body:**
```json
{
  "type": "EmailInteraction",
  "channel": "Email",
  "description": "Email de confirmação enviado",
  "direction": "Outbound"
}
```

### Update Journey Metrics
Update patient journey metrics.

```http
PUT /patient-journey/{patientId}/metrics
```

**Request Body:**
```json
{
  "lifetimeValue": 6500.00,
  "npsScore": 10,
  "satisfactionScore": 4.8,
  "churnRisk": "Low"
}
```

---

## 🤖 Marketing Automation API

### List Automations
Get all marketing automations.

```http
GET /marketing-automation
```

**Query Parameters:**
- `isActive` (boolean): Filter by active status
- `triggerType` (string): Filter by trigger type
- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 20)

**Response:**
```json
{
  "items": [
    {
      "id": "uuid",
      "name": "Boas-vindas Novo Paciente",
      "description": "...",
      "isActive": true,
      "triggerType": "StageChange",
      "triggerStage": "PrimeiraConsulta",
      "delayMinutes": 60,
      "timesExecuted": 150,
      "successRate": 0.95
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 20
}
```

### Create Automation
Create a new marketing automation.

```http
POST /marketing-automation
```

**Request Body:**
```json
{
  "name": "Follow-up Pós Consulta",
  "description": "Enviado 24h após consulta",
  "triggerType": "Event",
  "triggerEvent": "appointment_completed",
  "delayMinutes": 1440,
  "segmentFilter": "{\"specialty\": \"Cardiologia\"}",
  "tags": ["vip", "cardiologia"]
}
```

### Update Automation
Update an existing automation.

```http
PUT /marketing-automation/{automationId}
```

### Activate/Deactivate Automation
Toggle automation status.

```http
POST /marketing-automation/{automationId}/activate
POST /marketing-automation/{automationId}/deactivate
```

### Add Action to Automation
Add an action to automation workflow.

```http
POST /marketing-automation/{automationId}/actions
```

**Request Body (Email Action):**
```json
{
  "order": 1,
  "type": "SendEmail",
  "emailTemplateId": "uuid"
}
```

**Request Body (SMS Action):**
```json
{
  "order": 2,
  "type": "SendSMS",
  "messageTemplate": "Olá {{nome_paciente}}, confirme sua consulta...",
  "channel": "SMS"
}
```

### Get Automation Execution History
Get execution history for an automation.

```http
GET /marketing-automation/{automationId}/executions?startDate=2026-01-01&endDate=2026-01-31
```

---

## 📧 Email Templates API

### List Templates
```http
GET /email-templates
```

### Create Template
```http
POST /email-templates
```

**Request Body:**
```json
{
  "name": "Confirmação de Consulta",
  "subject": "Sua consulta está confirmada!",
  "htmlBody": "<html>...</html>",
  "plainTextBody": "...",
  "availableVariables": ["{{nome_paciente}}", "{{data_consulta}}"]
}
```

### Update Template
```http
PUT /email-templates/{templateId}
```

### Delete Template
```http
DELETE /email-templates/{templateId}
```

---

## 📋 Surveys API

### List Surveys
Get all surveys.

```http
GET /surveys
```

**Query Parameters:**
- `type` (string): NPS, CSAT, CES, Custom
- `isActive` (boolean): Filter by active status

**Response:**
```json
{
  "items": [
    {
      "id": "uuid",
      "name": "NPS Pós-Consulta",
      "type": "NPS",
      "isActive": true,
      "averageScore": 8.5,
      "totalResponses": 250,
      "responseRate": 65
    }
  ]
}
```

### Create Survey
```http
POST /surveys
```

**Request Body:**
```json
{
  "name": "Pesquisa de Satisfação",
  "description": "Avaliação geral do atendimento",
  "type": "CSAT",
  "triggerStage": "PrimeiraConsulta",
  "triggerEvent": "appointment_completed",
  "delayHours": 24
}
```

### Add Question to Survey
```http
POST /surveys/{surveyId}/questions
```

**Request Body:**
```json
{
  "order": 1,
  "questionText": "Quão satisfeito você está com nosso atendimento?",
  "type": "StarRating",
  "isRequired": true
}
```

### Get Survey Responses
Get all responses for a survey.

```http
GET /surveys/{surveyId}/responses
```

**Query Parameters:**
- `startDate` (date): Filter start date
- `endDate` (date): Filter end date
- `completed` (boolean): Only completed responses

### Submit Survey Response
Patient submits a survey response.

```http
POST /surveys/{surveyId}/responses
```

**Request Body:**
```json
{
  "patientId": "uuid",
  "questionResponses": [
    {
      "questionId": "uuid",
      "numericAnswer": 9
    },
    {
      "questionId": "uuid",
      "textAnswer": "Excelente atendimento!"
    }
  ]
}
```

### Get Survey Analytics
Get aggregated analytics for a survey.

```http
GET /surveys/{surveyId}/analytics
```

**Response:**
```json
{
  "surveyId": "uuid",
  "totalResponses": 250,
  "averageScore": 8.5,
  "npsScore": 45,
  "promotersPercentage": 60,
  "neutralsPercentage": 25,
  "detractorsPercentage": 15,
  "responsesByDay": {...},
  "sentimentDistribution": {...}
}
```

---

## 🎯 Complaints (Ouvidoria) API

### List Complaints
```http
GET /complaints
```

**Query Parameters:**
- `status` (string): Filter by status
- `priority` (string): Filter by priority
- `category` (string): Filter by category
- `patientId` (uuid): Filter by patient
- `assignedToUserId` (uuid): Filter by assigned user

**Response:**
```json
{
  "items": [
    {
      "id": "uuid",
      "protocolNumber": "OUV-2026-00123",
      "patientId": "uuid",
      "subject": "Demora no atendimento",
      "category": "WaitTime",
      "priority": "High",
      "status": "InProgress",
      "receivedAt": "2026-01-20T10:00:00Z",
      "responseTimeMinutes": 45,
      "assignedToUserName": "João Silva"
    }
  ]
}
```

### Create Complaint
```http
POST /complaints
```

**Request Body:**
```json
{
  "patientId": "uuid",
  "subject": "Problema com agendamento",
  "description": "Detalhes da reclamação...",
  "category": "Scheduling",
  "priority": "Medium"
}
```

**Response:**
```json
{
  "id": "uuid",
  "protocolNumber": "OUV-2026-00124",
  "status": "Received",
  "receivedAt": "2026-01-27T14:30:00Z"
}
```

### Update Complaint Status
```http
PUT /complaints/{complaintId}/status
```

**Request Body:**
```json
{
  "status": "InProgress"
}
```

### Assign Complaint
```http
POST /complaints/{complaintId}/assign
```

**Request Body:**
```json
{
  "userId": "uuid",
  "userName": "Maria Santos"
}
```

### Add Interaction
```http
POST /complaints/{complaintId}/interactions
```

**Request Body:**
```json
{
  "userId": "uuid",
  "userName": "João Silva",
  "message": "Estamos investigando o ocorrido...",
  "isInternal": false
}
```

### Record Satisfaction
Patient rates the complaint resolution.

```http
POST /complaints/{complaintId}/satisfaction
```

**Request Body:**
```json
{
  "rating": 4,
  "feedback": "Problema resolvido rapidamente"
}
```

### Get Complaint Metrics
```http
GET /complaints/metrics
```

**Response:**
```json
{
  "totalOpen": 15,
  "totalResolved": 250,
  "averageResponseTimeMinutes": 35,
  "averageResolutionTimeMinutes": 1440,
  "resolutionRate": 0.95,
  "averageSatisfactionRating": 4.2,
  "byCategory": {...},
  "byPriority": {...}
}
```

---

## 🧠 Sentiment Analysis API

### Analyze Text
Analyze sentiment of a text.

```http
POST /sentiment-analysis
```

**Request Body:**
```json
{
  "sourceText": "O atendimento foi excelente, mas a espera foi longa",
  "sourceType": "SurveyComment",
  "sourceId": "uuid"
}
```

**Response:**
```json
{
  "id": "uuid",
  "sentiment": "Mixed",
  "positiveScore": 0.65,
  "neutralScore": 0.10,
  "negativeScore": 0.25,
  "confidenceScore": 0.92,
  "topics": ["atendimento", "espera"],
  "analyzedAt": "2026-01-27T14:30:00Z"
}
```

### Get Sentiment by Source
Get sentiment analysis for a specific source.

```http
GET /sentiment-analysis/source/{sourceType}/{sourceId}
```

### Get Sentiment Trends
Get sentiment trends over time.

```http
GET /sentiment-analysis/trends?startDate=2026-01-01&endDate=2026-01-31
```

**Response:**
```json
{
  "period": {
    "start": "2026-01-01",
    "end": "2026-01-31"
  },
  "overallSentiment": {
    "positive": 0.65,
    "neutral": 0.20,
    "negative": 0.15
  },
  "byDay": [...],
  "topPositiveTopics": ["atendimento", "profissionalismo"],
  "topNegativeTopics": ["espera", "estacionamento"],
  "alerts": [
    {
      "date": "2026-01-15",
      "reason": "Spike in negative sentiment",
      "affectedPatients": 5
    }
  ]
}
```

---

## 📉 Churn Prediction API

### Get Churn Prediction
Get churn prediction for a patient.

```http
GET /churn-prediction/{patientId}
```

**Response:**
```json
{
  "id": "uuid",
  "patientId": "uuid",
  "churnProbability": 0.75,
  "riskLevel": "High",
  "predictedAt": "2026-01-27T14:30:00Z",
  "riskFactors": [
    "Alto número de cancelamentos",
    "Não retorna há 90 dias",
    "Satisfação abaixo da média"
  ],
  "recommendedActions": [
    "Entrar em contato via WhatsApp",
    "Oferecer desconto na próxima consulta",
    "Resolver reclamação pendente"
  ],
  "features": {
    "daysSinceLastVisit": 90,
    "totalVisits": 5,
    "lifetimeValue": 2500.00,
    "averageSatisfactionScore": 3.2,
    "complaintsCount": 2,
    "noShowCount": 1,
    "cancelledAppointmentsCount": 3
  }
}
```

### Calculate Churn Prediction
Trigger a new churn calculation.

```http
POST /churn-prediction/{patientId}/calculate
```

**Response:**
```json
{
  "success": true,
  "predictionId": "uuid",
  "message": "Churn prediction calculated successfully"
}
```

### List High Risk Patients
Get list of patients at high churn risk.

```http
GET /churn-prediction/high-risk
```

**Query Parameters:**
- `riskLevel` (string): Low, Medium, High, Critical
- `minProbability` (decimal): Minimum churn probability (0-1)

**Response:**
```json
{
  "items": [
    {
      "patientId": "uuid",
      "patientName": "João Silva",
      "churnProbability": 0.85,
      "riskLevel": "Critical",
      "daysSinceLastVisit": 120,
      "primaryRiskFactor": "Não retorna há 4 meses",
      "recommendedAction": "Contato urgente necessário"
    }
  ],
  "totalCount": 25
}
```

### Record Retention Action
Record an action taken to retain a patient.

```http
POST /churn-prediction/{patientId}/retention-action
```

**Request Body:**
```json
{
  "action": "Ligação telefônica realizada",
  "outcome": "Consulta reagendada",
  "notes": "Paciente estava aguardando retorno de exame"
}
```

---

## 📊 Analytics & Dashboards API

### Get CRM Dashboard
Get overall CRM metrics.

```http
GET /analytics/dashboard
```

**Query Parameters:**
- `startDate` (date): Filter start date
- `endDate` (date): Filter end date

**Response:**
```json
{
  "period": {
    "start": "2026-01-01",
    "end": "2026-01-31"
  },
  "patientMetrics": {
    "totalActive": 3000,
    "newPatients": 150,
    "retentionRate": 0.85,
    "churnRate": 0.05
  },
  "journeyMetrics": {
    "byStage": {
      "Descoberta": 200,
      "Consideracao": 150,
      "PrimeiraConsulta": 180,
      "Tratamento": 1200,
      "Retorno": 800,
      "Fidelizacao": 400,
      "Advocacia": 70
    },
    "averageTouchpointsPerPatient": 12,
    "averageStageProgression": 45
  },
  "satisfactionMetrics": {
    "npsScore": 52,
    "csatScore": 4.3,
    "responseRate": 0.68
  },
  "ouvidoriaMetrics": {
    "openComplaints": 15,
    "averageResponseTime": 35,
    "resolutionRate": 0.95,
    "satisfactionRating": 4.2
  },
  "churnMetrics": {
    "byRiskLevel": {
      "Low": 2400,
      "Medium": 400,
      "High": 150,
      "Critical": 50
    }
  }
}
```

### Get Journey Analytics
```http
GET /analytics/journey
```

### Get Satisfaction Analytics
```http
GET /analytics/satisfaction
```

### Get Ouvidoria Analytics
```http
GET /analytics/ouvidoria
```

### Get Churn Analytics
```http
GET /analytics/churn
```

---

## 🔔 Webhooks

### Configure Webhook
Register a webhook endpoint for CRM events.

```http
POST /webhooks
```

**Request Body:**
```json
{
  "url": "https://your-api.com/webhooks/crm",
  "events": [
    "patient.journey.stage_changed",
    "survey.response.received",
    "complaint.created",
    "complaint.resolved",
    "churn.risk_level_changed"
  ],
  "secret": "your_webhook_secret"
}
```

### Webhook Event Format
```json
{
  "eventType": "patient.journey.stage_changed",
  "timestamp": "2026-01-27T14:30:00Z",
  "data": {
    "patientId": "uuid",
    "previousStage": "Consideracao",
    "newStage": "PrimeiraConsulta",
    "trigger": "Consulta agendada"
  },
  "signature": "sha256_signature"
}
```

---

## 🚨 Error Handling

All endpoints return standard error responses:

**400 Bad Request**
```json
{
  "error": "ValidationError",
  "message": "Invalid request parameters",
  "details": [
    {
      "field": "email",
      "message": "Invalid email format"
    }
  ]
}
```

**401 Unauthorized**
```json
{
  "error": "Unauthorized",
  "message": "Invalid or expired token"
}
```

**404 Not Found**
```json
{
  "error": "NotFound",
  "message": "Resource not found"
}
```

**500 Internal Server Error**
```json
{
  "error": "InternalServerError",
  "message": "An unexpected error occurred",
  "requestId": "uuid"
}
```

---

## 🪝 Webhooks API

### Create Webhook Subscription
Create a new webhook subscription to receive CRM event notifications.

```http
POST /webhooks
```

**Request Body:**
```json
{
  "name": "Patient Journey Webhook",
  "description": "Receive notifications for patient journey events",
  "targetUrl": "https://example.com/webhooks/crm",
  "subscribedEvents": [1, 2, 10, 20],
  "maxRetries": 3,
  "retryDelaySeconds": 60
}
```

**Response:** `201 Created`
```json
{
  "id": "uuid",
  "name": "Patient Journey Webhook",
  "description": "Receive notifications for patient journey events",
  "targetUrl": "https://example.com/webhooks/crm",
  "isActive": false,
  "secret": "generated-secret-key",
  "subscribedEvents": [1, 2, 10, 20],
  "maxRetries": 3,
  "retryDelaySeconds": 60,
  "totalDeliveries": 0,
  "successfulDeliveries": 0,
  "failedDeliveries": 0,
  "createdAt": "2026-01-28T02:00:00Z",
  "updatedAt": "2026-01-28T02:00:00Z"
}
```

### List All Webhooks
```http
GET /webhooks
```

### Update Webhook Subscription
```http
PUT /webhooks/{id}
```

### Activate/Deactivate Webhook
```http
POST /webhooks/{id}/activate
POST /webhooks/{id}/deactivate
```

### Regenerate Secret
```http
POST /webhooks/{id}/regenerate-secret
```

### Get Webhook Deliveries
```http
GET /webhooks/{subscriptionId}/deliveries?limit=50
```

**Response:** `200 OK`
```json
[
  {
    "id": "uuid",
    "subscriptionId": "uuid",
    "event": "JourneyStageChanged",
    "status": "Delivered",
    "targetUrl": "https://example.com/webhooks/crm",
    "attemptCount": 1,
    "responseStatusCode": 200,
    "deliveredAt": "2026-01-28T02:30:00Z"
  }
]
```

### Retry Failed Delivery
```http
POST /webhooks/deliveries/{id}/retry
```

### Webhook Event Types
```http
GET /webhooks/events
```

**Available Events:**
- `1` JourneyStageChanged
- `2` TouchpointCreated
- `10` AutomationExecuted
- `11` CampaignSent
- `20` SurveyCreated
- `21` SurveyCompleted
- `22` NpsScoreCalculated
- `30` ComplaintCreated
- `31` ComplaintStatusChanged
- `32` ComplaintResolved
- `40` SentimentAnalyzed
- `41` NegativeSentimentDetected
- `50` ChurnRiskCalculated
- `51` HighChurnRiskDetected

### Webhook Payload Structure
```json
{
  "id": "uuid",
  "event": "JourneyStageChanged",
  "timestamp": "2026-01-28T02:30:00Z",
  "tenantId": "your-tenant-id",
  "data": {
    "patientId": "uuid",
    "previousStage": "Descoberta",
    "newStage": "Consideracao"
  }
}
```

### Webhook Security
All webhooks include HMAC-SHA256 signature:

**Headers:**
```http
X-Webhook-Signature: base64-encoded-signature
X-Webhook-Event: JourneyStageChanged
X-Webhook-Delivery-Id: uuid
```

**Verification Example (C#):**
```csharp
using System.Security.Cryptography;

public bool VerifySignature(string payload, string signature, string secret)
{
    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    return signature == Convert.ToBase64String(hash);
}
```

---

## 📊 Rate Limiting

API requests are rate-limited:
- **Standard**: 100 requests/minute
- **Premium**: 1000 requests/minute

Headers returned:
```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1643298600
```

---

## 🔒 Security

- All endpoints use HTTPS
- Bearer token authentication required
- Tokens expire after 24 hours
- Refresh tokens available for long-running operations
- Webhook signatures for event verification

---

## 📦 SDKs

Official SDKs available:
- C# / .NET
- JavaScript / TypeScript
- Python
- Java

---

## 📞 Support

API Documentation: https://docs.primecare.com.br/api/crm
Support Email: api-support@primecare.com.br
Status Page: https://status.primecare.com.br
