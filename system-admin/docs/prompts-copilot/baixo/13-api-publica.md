# 🔌 Prompt: API Pública para Integrações

## 📊 Status
- **Prioridade**: BAIXA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 1-2 meses | 1 dev
- **Prazo**: Q3/2026

## 🎯 Contexto

Criar API pública bem documentada com autenticação OAuth 2.0, rate limiting, webhooks e SDK para permitir integrações de terceiros (contabilidade, marketing, laboratórios, equipamentos médicos, sistemas de pagamento).

## 📋 Justificativa

### Benefícios
- ✅ Ecossistema de integrações
- ✅ Parcerias estratégicas
- ✅ Diferencial competitivo
- ✅ Automação de processos
- ✅ Expansão de funcionalidades

## 🏗️ Arquitetura

### API Endpoints Públicos

```csharp
// Public API Controllers
[ApiController]
[Route("public/v1/[controller]")]
[Authorize(Policy = "PublicApiKey")]
public class PublicPatientsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PatientDto>>> GetPatients(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        // Implementação
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
    {
        // Implementação
    }
    
    [HttpPost]
    public async Task<ActionResult<PatientDto>> CreatePatient(
        [FromBody] CreatePatientRequest request)
    {
        // Implementação
    }
}

[ApiController]
[Route("public/v1/[controller]")]
public class PublicAppointmentsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        // Implementação
    }
    
    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment(
        [FromBody] CreateAppointmentRequest request)
    {
        // Implementação
    }
}
```

### OAuth 2.0 Implementation

```csharp
// API Key Management
public class ApiKey : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }  // Hashed
    public string Secret { get; set; }  // Hashed
    public List<string> Scopes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int RequestsPerMinute { get; set; }
    public int RequestsToday { get; set; }
    public DateTime? LastUsedAt { get; set; }
}

// Rate Limiting Middleware
public class RateLimitingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var apiKey = GetApiKey(context);
        
        if (!await CheckRateLimitAsync(apiKey))
        {
            context.Response.StatusCode = 429;  // Too Many Requests
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                retryAfter = 60
            });
            return;
        }
        
        await _next(context);
    }
}
```

### Webhooks

```csharp
// Webhook Configuration
public class WebhookSubscription : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public string Url { get; set; }
    public List<WebhookEvent> Events { get; set; }
    public string Secret { get; set; }
    public bool IsActive { get; set; }
    public int FailureCount { get; set; }
}

public enum WebhookEvent
{
    PatientCreated,
    PatientUpdated,
    AppointmentCreated,
    AppointmentCancelled,
    PaymentReceived
}

// Webhook Service
public class WebhookService : IWebhookService
{
    public async Task SendWebhookAsync(WebhookEvent eventType, object payload)
    {
        var subscriptions = await _repository.GetActiveSubscriptionsAsync(eventType);
        
        foreach (var subscription in subscriptions)
        {
            await SendWebhookRequestAsync(subscription, payload);
        }
    }
    
    private async Task SendWebhookRequestAsync(WebhookSubscription subscription, object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var signature = ComputeHmacSha256(json, subscription.Secret);
        
        var request = new HttpRequestMessage(HttpMethod.Post, subscription.Url);
        request.Headers.Add("X-Webhook-Signature", signature);
        request.Headers.Add("X-Webhook-Event", eventType.ToString());
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try
        {
            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                await HandleWebhookFailureAsync(subscription);
            }
        }
        catch (Exception ex)
        {
            await HandleWebhookFailureAsync(subscription);
        }
    }
}
```

## 📚 Documentação da API

### OpenAPI/Swagger

```csharp
// Startup.cs
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("public-v1", new OpenApiInfo
    {
        Title = "PrimeCare Public API",
        Version = "v1",
        Description = "API pública para integrações de terceiros",
        Contact = new OpenApiContact
        {
            Name = "Suporte PrimeCare",
            Email = "api@primecare.com.br"
        }
    });
    
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-API-Key",
        Description = "API Key para autenticação"
    });
});
```

### SDK Examples

```javascript
// JavaScript/Node.js SDK
const PrimeCare = require('@primecare/sdk');

const client = new PrimeCare({
  apiKey: 'your-api-key',
  apiSecret: 'your-api-secret'
});

// Listar pacientes
const patients = await client.patients.list({
  page: 1,
  pageSize: 50
});

// Criar agendamento
const appointment = await client.appointments.create({
  patientId: 'patient-id',
  doctorId: 'doctor-id',
  datetime: '2026-06-15T10:00:00Z',
  type: 'Consultation'
});
```

```python
# Python SDK
from primecare import PrimeCareClient

client = PrimeCareClient(
    api_key='your-api-key',
    api_secret='your-api-secret'
)

# Listar pacientes
patients = client.patients.list(page=1, page_size=50)

# Criar agendamento
appointment = client.appointments.create(
    patient_id='patient-id',
    doctor_id='doctor-id',
    datetime='2026-06-15T10:00:00',
    type='Consultation'
)
```

## ✅ Checklist de Implementação

### Backend
- [ ] Criar API controllers públicos
- [ ] Implementar autenticação OAuth 2.0
- [ ] Sistema de API keys
- [ ] Rate limiting
- [ ] Webhooks
- [ ] Documentação OpenAPI/Swagger
- [ ] Versionamento da API (v1, v2)
- [ ] Logs de uso da API
- [ ] Sandbox environment

### SDKs
- [ ] JavaScript/Node.js SDK
- [ ] Python SDK
- [ ] PHP SDK
- [ ] C# SDK
- [ ] Ruby SDK

### Frontend Admin
- [ ] Painel de gestão de API keys
- [ ] Dashboard de uso da API
- [ ] Configuração de webhooks
- [ ] Documentação interativa

### Documentação
- [ ] Guia de início rápido
- [ ] Referência completa da API
- [ ] Exemplos de código
- [ ] Casos de uso
- [ ] Changelog
- [ ] Troubleshooting

### Testes
- [ ] Testes de API endpoints
- [ ] Testes de autenticação
- [ ] Testes de rate limiting
- [ ] Testes de webhooks
- [ ] Testes de SDKs

## 💰 Investimento

- **Esforço**: 1-2 meses | 1 dev
- **Custo**: R$ 45-90k

### ROI Esperado
- Novas parcerias e integrações
- Expansão de mercado
- Receita adicional por uso da API
- Ecossistema de desenvolvedores

## 🎯 Critérios de Aceitação

- [ ] API pública funcionando
- [ ] Autenticação OAuth 2.0
- [ ] Rate limiting por API key
- [ ] Webhooks operacionais
- [ ] Documentação completa (Swagger)
- [ ] SDKs em 3+ linguagens
- [ ] Sandbox environment
- [ ] Dashboard de monitoramento
- [ ] Exemplos de código
- [ ] Portal do desenvolvedor
