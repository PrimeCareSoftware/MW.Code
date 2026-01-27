# Guia Técnico de Implementação - Análise de Dados de Cadastro

## 📋 Visão Geral

Este guia fornece instruções técnicas detalhadas para implementar as estratégias descritas no documento `ANALISE_DADOS_CADASTRO_ESTRATEGIAS.md`.

## 🏗️ Arquitetura Atual

### Componentes Existentes

#### 1. Entidade de Domínio
```csharp
// MedicSoft.Domain.Entities.SalesFunnelMetric
public class SalesFunnelMetric : BaseEntity
{
    public string SessionId { get; private set; }
    public int Step { get; private set; }              // 1-6
    public string StepName { get; private set; }
    public string Action { get; private set; }         // entered, completed, abandoned
    public string? CapturedData { get; private set; }  // JSON sanitizado
    public string? PlanId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Referrer { get; private set; }
    public Guid? ClinicId { get; private set; }
    public Guid? OwnerId { get; private set; }
    public bool IsConverted { get; private set; }
    public long? DurationMs { get; private set; }
    public string? Metadata { get; private set; }      // JSON com UTM params, etc.
}
```

#### 2. API Endpoints Disponíveis
```
POST   /api/SalesFunnel/track          - Rastrear evento (público)
POST   /api/SalesFunnel/convert        - Marcar conversão (público)
GET    /api/SalesFunnel/stats          - Estatísticas (autenticado)
GET    /api/SalesFunnel/incomplete     - Sessões incompletas (autenticado)
GET    /api/SalesFunnel/session/{id}   - Métricas de sessão (autenticado)
GET    /api/SalesFunnel/recent         - Sessões recentes (autenticado)
```

#### 3. Serviço de Application
```csharp
public interface ISalesFunnelService
{
    Task<TrackEventResponseDto> TrackEventAsync(TrackSalesFunnelEventDto eventDto, string? ipAddress, string? userAgent);
    Task<TrackEventResponseDto> MarkConversionAsync(MarkConversionDto conversionDto);
    Task<FunnelStatsDto> GetFunnelStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<IncompleteSessionDto>> GetIncompleteSessions(int hoursOld = 24, int limit = 100);
    Task<IEnumerable<SalesFunnelMetricDto>> GetSessionMetricsAsync(string sessionId);
    Task<IEnumerable<SalesFunnelMetricDto>> GetRecentSessions(int limit = 100);
}
```

## 🔧 Implementações Práticas

### 1. Sistema de Email de Recuperação

#### 1.1 Criar Serviço de Email de Recuperação

```csharp
// MedicSoft.Application/Services/LeadRecoveryService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Application.Services;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    public interface ILeadRecoveryService
    {
        Task ProcessAbandonedLeadsAsync();
        Task SendRecoveryEmailAsync(string sessionId);
        Task<int> GetLeadScore(string sessionId);
    }

    public class LeadRecoveryService : ILeadRecoveryService
    {
        private readonly ISalesFunnelMetricRepository _metricRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<LeadRecoveryService> _logger;

        public LeadRecoveryService(
            ISalesFunnelMetricRepository metricRepository,
            IEmailService emailService,
            ILogger<LeadRecoveryService> logger)
        {
            _metricRepository = metricRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ProcessAbandonedLeadsAsync()
        {
            // Buscar sessões abandonadas há 2 horas
            var twoHoursAgo = DateTime.UtcNow.AddHours(-2);
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            
            var abandonedSessions = await _metricRepository.GetIncompleteSessions(twoHoursAgo, 100);
            
            foreach (var session in abandonedSessions)
            {
                // Verificar se já enviamos email para esta sessão
                var alreadySent = await _metricRepository.HasRecoveryEmailSent(session.SessionId);
                if (alreadySent)
                    continue;

                // Calcular lead score
                var score = await GetLeadScore(session.SessionId);
                
                // Enviar email apenas para leads com score > 30
                if (score >= 30)
                {
                    await SendRecoveryEmailAsync(session.SessionId);
                    await _metricRepository.MarkRecoveryEmailSent(session.SessionId);
                    
                    _logger.LogInformation("Recovery email sent to session {SessionId} (score: {Score})", 
                        session.SessionId, score);
                }
            }
        }

        public async Task SendRecoveryEmailAsync(string sessionId)
        {
            var metrics = await _metricRepository.GetBySessionIdAsync(sessionId);
            if (!metrics.Any())
                return;

            // Pegar último metric para obter dados
            var lastMetric = metrics.OrderByDescending(m => m.Step).First();
            
            // Extrair email dos dados capturados
            var email = ExtractEmailFromCapturedData(lastMetric.CapturedData);
            if (string.IsNullOrEmpty(email))
                return;

            // Gerar link de retomada
            var resumeUrl = $"https://medicwarehouse.com/cadastro?sessionId={sessionId}&step={lastMetric.Step}";

            // Selecionar template baseado na etapa
            var emailTemplate = GetEmailTemplateForStep(lastMetric.Step);
            
            await _emailService.SendAsync(
                to: email,
                subject: emailTemplate.Subject,
                htmlBody: emailTemplate.Body.Replace("{resumeUrl}", resumeUrl)
            );
        }

        public async Task<int> GetLeadScore(string sessionId)
        {
            var metrics = await _metricRepository.GetBySessionIdAsync(sessionId);
            if (!metrics.Any())
                return 0;

            int score = 0;
            var lastMetric = metrics.OrderByDescending(m => m.Step).First();
            
            // Pontos pela etapa alcançada
            score += lastMetric.Step * 10; // 10-60 pontos
            
            // Pontos pelo tempo gasto (engajamento)
            var totalDuration = metrics.Sum(m => m.DurationMs ?? 0);
            if (totalDuration > 300000) score += 20; // >5min
            else if (totalDuration > 120000) score += 10; // >2min
            
            // Pontos pelo plano visualizado
            if (!string.IsNullOrEmpty(lastMetric.PlanId))
            {
                if (lastMetric.PlanId.Contains("premium", StringComparison.OrdinalIgnoreCase))
                    score += 30;
                else if (lastMetric.PlanId.Contains("standard", StringComparison.OrdinalIgnoreCase))
                    score += 20;
                else
                    score += 10;
            }
            
            // Pontos por dados preenchidos
            if (!string.IsNullOrEmpty(lastMetric.CapturedData))
            {
                var hasEmail = lastMetric.CapturedData.Contains("email", StringComparison.OrdinalIgnoreCase);
                var hasPhone = lastMetric.CapturedData.Contains("phone", StringComparison.OrdinalIgnoreCase);
                
                if (hasEmail) score += 15;
                if (hasPhone) score += 15;
            }
            
            return score;
        }

        private string? ExtractEmailFromCapturedData(string? capturedData)
        {
            if (string.IsNullOrEmpty(capturedData))
                return null;

            try
            {
                var json = System.Text.Json.JsonDocument.Parse(capturedData);
                if (json.RootElement.TryGetProperty("email", out var emailElement))
                    return emailElement.GetString();
                if (json.RootElement.TryGetProperty("ownerEmail", out var ownerEmailElement))
                    return ownerEmailElement.GetString();
            }
            catch
            {
                // Ignorar erros de parsing
            }

            return null;
        }

        private EmailTemplate GetEmailTemplateForStep(int step)
        {
            return step switch
            {
                1 or 2 => new EmailTemplate
                {
                    Subject = "Complete seu cadastro no MedicWarehouse em apenas 3 minutos",
                    Body = @"
                        <h2>Olá!</h2>
                        <p>Notamos que você começou seu cadastro no MedicWarehouse, mas não finalizou.</p>
                        <p>Leva apenas 3 minutos para completar e você pode começar a usar nossa plataforma imediatamente!</p>
                        <p><a href='{resumeUrl}' style='background-color: #4CAF50; color: white; padding: 14px 20px; text-decoration: none; border-radius: 4px;'>
                            Continuar Cadastro
                        </a></p>
                        <p>Se tiver alguma dúvida, estamos aqui para ajudar!</p>
                    "
                },
                3 or 4 => new EmailTemplate
                {
                    Subject = "Seus dados estão seguros conosco - Complete seu cadastro",
                    Body = @"
                        <h2>Segurança e Privacidade</h2>
                        <p>Entendemos que seus dados são importantes. No MedicWarehouse, levamos a segurança a sério:</p>
                        <ul>
                            <li>✓ 100% compatível com LGPD</li>
                            <li>✓ Criptografia de ponta a ponta</li>
                            <li>✓ Servidores no Brasil</li>
                            <li>✓ Backup automático diário</li>
                        </ul>
                        <p><a href='{resumeUrl}' style='background-color: #4CAF50; color: white; padding: 14px 20px; text-decoration: none; border-radius: 4px;'>
                            Continuar Cadastro Seguro
                        </a></p>
                    "
                },
                5 or 6 => new EmailTemplate
                {
                    Subject = "🎁 Oferta Especial: 30 dias grátis no MedicWarehouse",
                    Body = @"
                        <h2>Oferta Exclusiva Para Você!</h2>
                        <p>Vimos que você está interessado no MedicWarehouse. Complete seu cadastro hoje e ganhe:</p>
                        <ul>
                            <li>🎁 30 dias de trial gratuito</li>
                            <li>📚 Acesso completo a todos os recursos</li>
                            <li>🎓 Treinamento personalizado</li>
                            <li>💬 Suporte dedicado</li>
                        </ul>
                        <p>Não perca esta oportunidade!</p>
                        <p><a href='{resumeUrl}' style='background-color: #4CAF50; color: white; padding: 14px 20px; text-decoration: none; border-radius: 4px;'>
                            Ativar Oferta Especial
                        </a></p>
                        <p><small>Oferta válida por 48 horas</small></p>
                    "
                },
                _ => new EmailTemplate
                {
                    Subject = "Complete seu cadastro no MedicWarehouse",
                    Body = @"
                        <p>Continue seu cadastro: <a href='{resumeUrl}'>Clique aqui</a></p>
                    "
                }
            };
        }

        private class EmailTemplate
        {
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }
    }
}
```

#### 1.2 Criar Background Job

```csharp
// MedicSoft.Api/BackgroundJobs/LeadRecoveryJob.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;

namespace MedicSoft.Api.BackgroundJobs
{
    public class LeadRecoveryJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LeadRecoveryJob> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30); // Rodar a cada 30 minutos

        public LeadRecoveryJob(
            IServiceProvider serviceProvider,
            ILogger<LeadRecoveryJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Lead Recovery Job started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var recoveryService = scope.ServiceProvider.GetRequiredService<ILeadRecoveryService>();
                    
                    _logger.LogInformation("Processing abandoned leads...");
                    await recoveryService.ProcessAbandonedLeadsAsync();
                    _logger.LogInformation("Abandoned leads processed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing abandoned leads");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Lead Recovery Job stopped");
        }
    }
}
```

#### 1.3 Registrar Serviços

```csharp
// MedicSoft.Api/Program.cs (adicionar no ConfigureServices)

// Registrar serviço de recuperação de leads
builder.Services.AddScoped<ILeadRecoveryService, LeadRecoveryService>();

// Registrar background job
builder.Services.AddHostedService<LeadRecoveryJob>();
```

### 2. Dashboard de Analytics

#### 2.1 Criar Controller de Analytics

```csharp
// MedicSoft.Api/Controllers/RegistrationAnalyticsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Services;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SystemAdmin")] // Apenas admins do sistema
    public class RegistrationAnalyticsController : ControllerBase
    {
        private readonly IRegistrationAnalyticsService _analyticsService;

        public RegistrationAnalyticsController(IRegistrationAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Get funnel conversion overview
        /// </summary>
        [HttpGet("funnel-overview")]
        public async Task<ActionResult<FunnelOverviewDto>> GetFunnelOverview(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var overview = await _analyticsService.GetFunnelOverviewAsync(startDate, endDate);
            return Ok(overview);
        }

        /// <summary>
        /// Get conversion rate by traffic source
        /// </summary>
        [HttpGet("conversion-by-source")]
        public async Task<ActionResult<IEnumerable<SourceConversionDto>>> GetConversionBySource(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var data = await _analyticsService.GetConversionBySourceAsync(startDate, endDate);
            return Ok(data);
        }

        /// <summary>
        /// Get conversion rate by device type
        /// </summary>
        [HttpGet("conversion-by-device")]
        public async Task<ActionResult<IEnumerable<DeviceConversionDto>>> GetConversionByDevice(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var data = await _analyticsService.GetConversionByDeviceAsync(startDate, endDate);
            return Ok(data);
        }

        /// <summary>
        /// Get average time to convert
        /// </summary>
        [HttpGet("time-to-convert")]
        public async Task<ActionResult<TimeToConvertDto>> GetTimeToConvert(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var data = await _analyticsService.GetTimeToConvertAsync(startDate, endDate);
            return Ok(data);
        }

        /// <summary>
        /// Get plan selection statistics
        /// </summary>
        [HttpGet("plan-stats")]
        public async Task<ActionResult<IEnumerable<PlanStatsDto>>> GetPlanStats(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var data = await _analyticsService.GetPlanStatsAsync(startDate, endDate);
            return Ok(data);
        }

        /// <summary>
        /// Get hourly conversion pattern
        /// </summary>
        [HttpGet("hourly-pattern")]
        public async Task<ActionResult<IEnumerable<HourlyConversionDto>>> GetHourlyPattern(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var data = await _analyticsService.GetHourlyPatternAsync(startDate, endDate);
            return Ok(data);
        }

        /// <summary>
        /// Get geographic distribution
        /// </summary>
        [HttpGet("geographic-distribution")]
        public async Task<ActionResult<IEnumerable<GeographicDistributionDto>>> GetGeographicDistribution(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var data = await _analyticsService.GetGeographicDistributionAsync(startDate, endDate);
            return Ok(data);
        }

        /// <summary>
        /// Get high-value abandoned leads (lead scoring)
        /// </summary>
        [HttpGet("high-value-leads")]
        public async Task<ActionResult<IEnumerable<HighValueLeadDto>>> GetHighValueLeads(
            [FromQuery] int minimumScore = 70,
            [FromQuery] int limit = 50)
        {
            var leads = await _analyticsService.GetHighValueLeadsAsync(minimumScore, limit);
            return Ok(leads);
        }
    }
}
```

### 3. Queries SQL Otimizadas

#### 3.1 Índices Recomendados

```sql
-- Criar índices para melhorar performance das queries
CREATE INDEX IX_SalesFunnelMetrics_SessionId ON SalesFunnelMetrics(SessionId);
CREATE INDEX IX_SalesFunnelMetrics_CreatedAt ON SalesFunnelMetrics(CreatedAt);
CREATE INDEX IX_SalesFunnelMetrics_IsConverted ON SalesFunnelMetrics(IsConverted);
CREATE INDEX IX_SalesFunnelMetrics_Step_Action ON SalesFunnelMetrics(Step, Action);
CREATE INDEX IX_SalesFunnelMetrics_PlanId ON SalesFunnelMetrics(PlanId) WHERE PlanId IS NOT NULL;

-- Índice composto para queries de análise temporal
CREATE INDEX IX_SalesFunnelMetrics_CreatedAt_IsConverted 
ON SalesFunnelMetrics(CreatedAt, IsConverted) 
INCLUDE (SessionId, Step, Action);
```

#### 3.2 View Materializada para Dashboard

```sql
-- Criar view materializada para métricas diárias (melhor performance)
CREATE MATERIALIZED VIEW daily_funnel_metrics AS
SELECT 
    DATE(CreatedAt) as MetricDate,
    Step,
    StepName,
    Action,
    PlanId,
    CASE 
        WHEN UserAgent LIKE '%Mobile%' THEN 'Mobile'
        WHEN UserAgent LIKE '%Tablet%' THEN 'Tablet'
        ELSE 'Desktop'
    END as DeviceType,
    COUNT(DISTINCT SessionId) as SessionCount,
    COUNT(DISTINCT CASE WHEN IsConverted = true THEN SessionId END) as Conversions,
    AVG(DurationMs) as AvgDuration
FROM SalesFunnelMetrics
WHERE CreatedAt >= CURRENT_DATE - INTERVAL '90 days'
GROUP BY 
    DATE(CreatedAt),
    Step,
    StepName,
    Action,
    PlanId,
    DeviceType;

-- Criar índice na view materializada
CREATE INDEX IX_daily_funnel_metrics_date ON daily_funnel_metrics(MetricDate DESC);

-- Atualizar view diariamente via cron job
-- 0 1 * * * REFRESH MATERIALIZED VIEW CONCURRENTLY daily_funnel_metrics;
```

#### 3.3 Query: Top Leads para Contato

```sql
-- Identificar leads de alto valor que abandonaram recentemente
WITH session_summary AS (
    SELECT 
        SessionId,
        MAX(Step) as MaxStep,
        MAX(CreatedAt) as LastActivity,
        MAX(PlanId) as SelectedPlan,
        SUM(DurationMs) as TotalDuration,
        MAX(CASE WHEN CapturedData LIKE '%email%' THEN CapturedData END) as DataWithEmail,
        MAX(CASE WHEN CapturedData LIKE '%phone%' THEN CapturedData END) as DataWithPhone
    FROM SalesFunnelMetrics
    WHERE IsConverted = false
    AND CreatedAt >= NOW() - INTERVAL '7 days'
    GROUP BY SessionId
),
scored_leads AS (
    SELECT 
        SessionId,
        MaxStep,
        LastActivity,
        SelectedPlan,
        -- Cálculo de score
        (MaxStep * 10) + -- 10-60 pontos pela etapa
        (CASE WHEN TotalDuration > 300000 THEN 20 WHEN TotalDuration > 120000 THEN 10 ELSE 0 END) + -- Tempo
        (CASE WHEN SelectedPlan LIKE '%premium%' THEN 30 WHEN SelectedPlan LIKE '%standard%' THEN 20 ELSE 10 END) + -- Plano
        (CASE WHEN DataWithEmail IS NOT NULL THEN 15 ELSE 0 END) + -- Email fornecido
        (CASE WHEN DataWithPhone IS NOT NULL THEN 15 ELSE 0 END) -- Telefone fornecido
        as LeadScore,
        DataWithEmail,
        DataWithPhone
    FROM session_summary
)
SELECT 
    SessionId,
    MaxStep,
    LastActivity,
    ROUND((EXTRACT(EPOCH FROM (NOW() - LastActivity)) / 3600)::numeric, 1) as HoursSinceAbandonment,
    SelectedPlan,
    LeadScore,
    DataWithEmail,
    DataWithPhone
FROM scored_leads
WHERE LeadScore >= 70
ORDER BY LeadScore DESC, LastActivity DESC
LIMIT 50;
```

### 4. Integração com Google Analytics 4

#### 4.1 Enviar Eventos para GA4

```csharp
// MedicSoft.Application/Services/GoogleAnalyticsService.cs
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    public interface IGoogleAnalyticsService
    {
        Task TrackFunnelEventAsync(string sessionId, int step, string action, string? planId = null);
    }

    public class GoogleAnalyticsService : IGoogleAnalyticsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _measurementId;
        private readonly string _apiSecret;
        private readonly ILogger<GoogleAnalyticsService> _logger;

        public GoogleAnalyticsService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GoogleAnalyticsService> logger)
        {
            _httpClient = httpClient;
            _measurementId = configuration["GoogleAnalytics:MeasurementId"] ?? "";
            _apiSecret = configuration["GoogleAnalytics:ApiSecret"] ?? "";
            _logger = logger;
        }

        public async Task TrackFunnelEventAsync(string sessionId, int step, string action, string? planId = null)
        {
            if (string.IsNullOrEmpty(_measurementId) || string.IsNullOrEmpty(_apiSecret))
            {
                _logger.LogWarning("Google Analytics not configured");
                return;
            }

            try
            {
                var payload = new
                {
                    client_id = sessionId,
                    events = new[]
                    {
                        new
                        {
                            name = "registration_funnel",
                            @params = new
                            {
                                step = step,
                                action = action,
                                plan_id = planId ?? "none",
                                event_category = "Registration",
                                event_label = $"Step {step}"
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"https://www.google-analytics.com/mp/collect?measurement_id={_measurementId}&api_secret={_apiSecret}";
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to send event to GA4: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending event to GA4");
            }
        }
    }
}
```

### 5. Sistema de Testes A/B

#### 5.1 Implementar Framework de A/B Testing

```csharp
// MedicSoft.Application/Services/ABTestService.cs
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IABTestService
    {
        string GetVariant(string testName, string sessionId);
        Task TrackVariantExposureAsync(string testName, string variant, string sessionId);
        Task<ABTestResultsDto> GetTestResultsAsync(string testName);
    }

    public class ABTestService : IABTestService
    {
        private readonly IABTestRepository _abTestRepository;

        public ABTestService(IABTestRepository abTestRepository)
        {
            _abTestRepository = abTestRepository;
        }

        public string GetVariant(string testName, string sessionId)
        {
            // Usar hash consistente para garantir que o mesmo sessionId sempre receba a mesma variante
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{testName}:{sessionId}"));
            var hashValue = BitConverter.ToUInt32(hashBytes, 0);
            
            // Split 50/50 entre variante A e B
            return (hashValue % 2 == 0) ? "A" : "B";
        }

        public async Task TrackVariantExposureAsync(string testName, string variant, string sessionId)
        {
            await _abTestRepository.TrackExposureAsync(testName, variant, sessionId);
        }

        public async Task<ABTestResultsDto> GetTestResultsAsync(string testName)
        {
            return await _abTestRepository.GetResultsAsync(testName);
        }
    }

    public class ABTestResultsDto
    {
        public string TestName { get; set; } = string.Empty;
        public VariantResults VariantA { get; set; } = new();
        public VariantResults VariantB { get; set; } = new();
        public double PValue { get; set; }
        public bool IsStatisticallySignificant { get; set; }
    }

    public class VariantResults
    {
        public string Variant { get; set; } = string.Empty;
        public int Exposures { get; set; }
        public int Conversions { get; set; }
        public double ConversionRate { get; set; }
    }
}
```

### 6. Retargeting com Facebook Pixel

#### 6.1 Adicionar Script no Frontend

```html
<!-- Adicionar no head do arquivo de cadastro -->
<script>
!function(f,b,e,v,n,t,s)
{if(f.fbq)return;n=f.fbq=function(){n.callMethod?
n.callMethod.apply(n,arguments):n.queue.push(arguments)};
if(!f._fbq)f._fbq=n;n.push=n;n.loaded=!0;n.version='2.0';
n.queue=[];t=b.createElement(e);t.async=!0;
t.src=v;s=b.getElementsByTagName(e)[0];
s.parentNode.insertBefore(t,s)}(window, document,'script',
'https://connect.facebook.net/en_US/fbevents.js');
fbq('init', 'YOUR_PIXEL_ID');
fbq('track', 'PageView');
</script>

<script>
// Trackear eventos de funil
function trackFunnelStep(step) {
    fbq('track', 'RegistrationStep' + step, {
        content_name: 'Registration Flow',
        status: 'initiated'
    });
}

// Trackear abandono
function trackAbandonment(step) {
    fbq('track', 'AbandonRegistration', {
        content_name: 'Registration Flow',
        step: step
    });
}

// Trackear conversão
function trackConversion(planId, value) {
    fbq('track', 'CompleteRegistration', {
        content_name: 'Registration',
        value: value,
        currency: 'BRL',
        plan: planId
    });
}
</script>
```

## 📊 Monitoramento e Alertas

### Configurar Alertas no Sistema

```csharp
// MedicSoft.Application/Services/FunnelMonitoringService.cs
using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    public interface IFunnelMonitoringService
    {
        Task CheckAnomaliesAsync();
    }

    public class FunnelMonitoringService : IFunnelMonitoringService
    {
        private readonly ISalesFunnelMetricRepository _metricRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<FunnelMonitoringService> _logger;

        public FunnelMonitoringService(
            ISalesFunnelMetricRepository metricRepository,
            IEmailService emailService,
            ILogger<FunnelMonitoringService> logger)
        {
            _metricRepository = metricRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task CheckAnomaliesAsync()
        {
            // Comparar taxa de conversão atual com média da semana passada
            var currentRate = await _metricRepository.GetConversionRateAsync(
                DateTime.UtcNow.AddHours(-24), 
                DateTime.UtcNow
            );

            var historicalRate = await _metricRepository.GetConversionRateAsync(
                DateTime.UtcNow.AddDays(-8),
                DateTime.UtcNow.AddDays(-1)
            );

            var dropPercentage = ((historicalRate - currentRate) / historicalRate) * 100;

            if (dropPercentage > 10)
            {
                await SendAlertAsync(
                    "⚠️ ALERTA: Queda na Taxa de Conversão",
                    $"A taxa de conversão caiu {dropPercentage:F1}% nas últimas 24h. " +
                    $"Taxa atual: {currentRate:F1}% | Média histórica: {historicalRate:F1}%"
                );
            }

            // Verificar spikes de abandono em etapas específicas
            for (int step = 1; step <= 6; step++)
            {
                var abandonmentRate = await _metricRepository.GetAbandonmentRateAsync(step);
                if (abandonmentRate > 40) // Se mais de 40% abandonam em uma etapa
                {
                    await SendAlertAsync(
                        $"⚠️ ALERTA: Alta Taxa de Abandono na Etapa {step}",
                        $"Taxa de abandono na etapa {step} está em {abandonmentRate:F1}%"
                    );
                }
            }
        }

        private async Task SendAlertAsync(string subject, string message)
        {
            _logger.LogWarning("ALERT: {Subject} - {Message}", subject, message);
            
            await _emailService.SendAsync(
                to: "admin@medicwarehouse.com",
                subject: subject,
                htmlBody: $"<h2>{subject}</h2><p>{message}</p>"
            );
        }
    }
}
```

## 🔒 Considerações de Segurança e LGPD

### 1. Sanitização de Dados Sensíveis

```csharp
// MedicSoft.Application/Services/DataSanitizationService.cs
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MedicSoft.Application.Services
{
    public static class DataSanitizationService
    {
        private static readonly string[] SensitiveFields = new[]
        {
            "password", "senha", "credit_card", "cartao", "cvv", "security_code"
        };

        public static string SanitizeCapturedData(string jsonData)
        {
            try
            {
                var document = JsonDocument.Parse(jsonData);
                var sanitized = new System.Collections.Generic.Dictionary<string, object>();

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    var key = property.Name.ToLower();
                    
                    // Remove campos sensíveis
                    if (SensitiveFields.Any(sf => key.Contains(sf)))
                    {
                        sanitized[property.Name] = "[REDACTED]";
                        continue;
                    }

                    // Mascara CPF parcialmente
                    if (key.Contains("cpf"))
                    {
                        var value = property.Value.GetString() ?? "";
                        sanitized[property.Name] = MaskCpf(value);
                        continue;
                    }

                    // Mantém outros campos
                    sanitized[property.Name] = property.Value.ToString();
                }

                return JsonSerializer.Serialize(sanitized);
            }
            catch
            {
                return jsonData;
            }
        }

        private static string MaskCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length < 11)
                return cpf;

            // Mostra apenas os 3 primeiros e 2 últimos dígitos
            // Exemplo: 123.456.789-00 vira 123.***.***-00
            var cleaned = Regex.Replace(cpf, @"[^\d]", "");
            if (cleaned.Length == 11)
                return $"{cleaned.Substring(0, 3)}.***. ***-{cleaned.Substring(9, 2)}";
            
            return cpf;
        }
    }
}
```

### 2. Política de Retenção de Dados

```sql
-- Criar job para limpar dados antigos (LGPD compliance)
CREATE OR REPLACE FUNCTION cleanup_old_funnel_metrics()
RETURNS void AS $$
BEGIN
    -- Deletar métricas não convertidas com mais de 90 dias
    DELETE FROM SalesFunnelMetrics
    WHERE IsConverted = false
    AND CreatedAt < NOW() - INTERVAL '90 days';
    
    -- Anonimizar IPs de métricas antigas
    UPDATE SalesFunnelMetrics
    SET IpAddress = NULL,
        UserAgent = NULL
    WHERE CreatedAt < NOW() - INTERVAL '30 days';
END;
$$ LANGUAGE plpgsql;

-- Agendar execução semanal
-- 0 2 * * 0 SELECT cleanup_old_funnel_metrics();
```

## 🚀 Deploy e Configuração

### Variáveis de Ambiente Necessárias

```bash
# appsettings.json ou environment variables

# Email Service
"EmailService:Provider": "SendGrid",
"EmailService:ApiKey": "your_sendgrid_api_key",
"EmailService:FromEmail": "noreply@medicwarehouse.com",
"EmailService:FromName": "MedicWarehouse",

# Google Analytics
"GoogleAnalytics:MeasurementId": "G-XXXXXXXXXX",
"GoogleAnalytics:ApiSecret": "your_ga4_api_secret",

# Facebook Pixel
"FacebookPixel:PixelId": "your_pixel_id",

# Lead Recovery
"LeadRecovery:Enabled": true,
"LeadRecovery:IntervalMinutes": 30,
"LeadRecovery:MinimumLeadScore": 30,

# Monitoring
"Monitoring:AlertEmail": "admin@medicwarehouse.com",
"Monitoring:ConversionDropThreshold": 10,
```

## 📚 Próximos Passos

1. **Implementar serviço de email de recuperação** (Fase 1)
2. **Configurar background job** para processar leads abandonados
3. **Criar dashboard de analytics** no frontend
4. **Integrar com Google Analytics 4** para análise detalhada
5. **Configurar retargeting** via Facebook/Google Ads
6. **Implementar sistema de A/B testing** 
7. **Configurar alertas** de anomalias
8. **Machine Learning** para previsão de churn (fase avançada)

---

**Documento complementar**: Consulte `ANALISE_DADOS_CADASTRO_ESTRATEGIAS.md` para estratégias de negócio e ideias de otimização.
