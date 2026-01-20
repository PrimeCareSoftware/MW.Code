# 📊 Prompt: BI e Analytics Avançados

## 📊 Status
- **Prioridade**: 🔥 MÉDIA
- **Progresso**: 20% (Dashboard financeiro básico implementado)
- **Esforço**: 3-4 meses | 2 devs
- **Prazo**: Q2/2026

## 🎯 Contexto

Implementar sistema completo de Business Intelligence e Analytics Avançados com dashboards interativos, análise preditiva com Machine Learning, e benchmarking anônimo para gerar insights valiosos para a gestão da clínica.

## ✅ O que já existe

- ✅ Dashboard financeiro básico
- ✅ Métricas operacionais simples
- ✅ Relatórios de agendamentos

## 🎯 O que falta implementar

### 1. Dashboard Clínico

**Métricas:**
- Taxa de ocupação da agenda
- Tempo médio de consulta por médico
- Taxa de no-show (faltas)
- Top 10 diagnósticos (CID-10)
- Distribuição demográfica de pacientes
- Taxa de retorno de pacientes
- Tempo médio de espera
- Eficiência da agenda por período

### 2. Dashboard Financeiro Avançado

**Métricas:**
- Receita por fonte (particular, convênios, telemedicina)
- Ticket médio por tipo de consulta
- CLV (Customer Lifetime Value)
- Projeções de receita
- Análise de sazonalidade
- Taxa de conversão de orçamentos
- Índice de inadimplência
- Lucratividade por médico/especialidade
- Comparação mês a mês (MoM) e ano a ano (YoY)

### 3. Dashboard Operacional

**Métricas:**
- Tempo médio de espera (recepção + consultório)
- Eficiência da agenda (slots preenchidos vs disponíveis)
- Horários de pico
- Capacidade ociosa por período
- Taxa de reagendamento
- Tempo médio entre agendamento e consulta
- Distribuição de consultas por dia da semana/hora

### 4. Dashboard de Qualidade

**Métricas:**
- NPS (Net Promoter Score)
- CSAT (Customer Satisfaction Score)
- Taxa de retorno de pacientes
- Reclamações por categoria
- Satisfação por médico
- Tempo de resposta a reclamações
- Taxa de resolução no primeiro contato

### 5. Análise Preditiva com ML

**Modelos:**
- Previsão de demanda (quantas consultas na próxima semana/mês)
- Risco de no-show (probabilidade de falta)
- Projeção de receita
- Identificação de churn de pacientes
- Identificação de padrões de doenças
- Previsão de estoque de medicamentos (se aplicável)

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// Entidades de Analytics
public class AnalyticsMetric : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public MetricType Type { get; set; }
    public string MetricName { get; set; }
    public decimal Value { get; set; }
    public DateTime CalculatedAt { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class PredictionResult : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public PredictionType Type { get; set; }
    public DateTime PredictedDate { get; set; }
    public decimal PredictedValue { get; set; }
    public decimal ConfidenceScore { get; set; }
    public string ModelVersion { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum MetricType
{
    // Clinical
    OccupancyRate,
    AverageConsultationTime,
    NoShowRate,
    ReturnRate,
    
    // Financial
    Revenue,
    TicketAverage,
    CLV,
    Churn,
    
    // Operational
    WaitingTime,
    AgendaEfficiency,
    PeakHours,
    
    // Quality
    NPS,
    CSAT,
    ComplaintRate
}

public enum PredictionType
{
    DemandForecast,
    NoShowPrediction,
    RevenueForecast,
    ChurnPrediction
}
```

### Camada de Aplicação (Application Layer)

```csharp
// Service Interface
public interface IAnalyticsService
{
    // Dashboards
    Task<ClinicalDashboard> GetClinicalDashboard(DateTime startDate, DateTime endDate);
    Task<FinancialDashboard> GetFinancialDashboard(DateTime startDate, DateTime endDate);
    Task<OperationalDashboard> GetOperationalDashboard(DateTime startDate, DateTime endDate);
    Task<QualityDashboard> GetQualityDashboard(DateTime startDate, DateTime endDate);
    
    // Métricas específicas
    Task<decimal> CalculateOccupancyRate(DateTime date);
    Task<decimal> CalculateNoShowRate(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateNPS(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateCLV(Guid patientId);
    
    // Análises avançadas
    Task<List<TopDiagnosis>> GetTopDiagnoses(DateTime startDate, DateTime endDate, int limit);
    Task<List<PeakHour>> GetPeakHours(DateTime startDate, DateTime endDate);
    Task<Dictionary<string, decimal>> GetRevenueBySource(DateTime startDate, DateTime endDate);
    
    // Comparações
    Task<ComparisonResult> ComparePeriods(DateTime period1Start, DateTime period1End,
        DateTime period2Start, DateTime period2End);
    
    // Benchmarking
    Task<BenchmarkingReport> GetBenchmarkingReport();
}

public interface IPredictionService
{
    Task<DemandForecast> PredictDemand(DateTime startDate, DateTime endDate);
    Task<NoShowPrediction> PredictNoShow(Guid appointmentId);
    Task<RevenueForecast> ForecastRevenue(DateTime forecastDate);
    Task<ChurnPrediction> PredictChurn(Guid patientId);
}

// DTOs
public record ClinicalDashboard(
    decimal OccupancyRate,
    TimeSpan AverageConsultationTime,
    decimal NoShowRate,
    List<TopDiagnosis> TopDiagnoses,
    DemographicDistribution Demographics,
    decimal ReturnRate
);

public record FinancialDashboard(
    decimal TotalRevenue,
    decimal RevenueGrowth,
    Dictionary<string, decimal> RevenueBySource,
    decimal TicketAverage,
    decimal AverageCLV,
    RevenueTrend Trend,
    List<RevenueProjection> Projections
);

public record OperationalDashboard(
    TimeSpan AverageWaitingTime,
    decimal AgendaEfficiency,
    List<PeakHour> PeakHours,
    decimal IdleCapacity,
    decimal ReschedulingRate
);

public record QualityDashboard(
    decimal NPS,
    decimal CSAT,
    int TotalComplaints,
    Dictionary<string, int> ComplaintsByCategory,
    Dictionary<string, decimal> SatisfactionByDoctor
);

public record TopDiagnosis(
    string Icd10Code,
    string DiagnosisName,
    int Count,
    decimal Percentage
);

public record PeakHour(
    int Hour,
    int AppointmentCount,
    decimal OccupancyPercentage
);

public record DemandForecast(
    DateTime ForecastDate,
    int PredictedAppointments,
    decimal ConfidenceScore,
    List<int> HistoricalData
);

public record NoShowPrediction(
    Guid AppointmentId,
    decimal Probability,
    string RiskLevel,  // Low, Medium, High
    List<string> RiskFactors
);
```

### Machine Learning com ML.NET

```csharp
// Prediction Models
public class NoShowPredictionModel
{
    [LoadColumn(0)]
    public float PatientAge { get; set; }
    
    [LoadColumn(1)]
    public float DaysSinceScheduling { get; set; }
    
    [LoadColumn(2)]
    public float PreviousNoShows { get; set; }
    
    [LoadColumn(3)]
    public float DayOfWeek { get; set; }
    
    [LoadColumn(4)]
    public float HourOfDay { get; set; }
    
    [LoadColumn(5)]
    public float IsFirstAppointment { get; set; }
    
    [LoadColumn(6)]
    public float AppointmentValue { get; set; }
    
    [LoadColumn(7)]
    [ColumnName("Label")]
    public bool NoShow { get; set; }
}

public class NoShowPrediction
{
    [ColumnName("PredictedLabel")]
    public bool WillNoShow { get; set; }
    
    [ColumnName("Probability")]
    public float Probability { get; set; }
    
    [ColumnName("Score")]
    public float Score { get; set; }
}

// ML Service
public class MachineLearningService : IPredictionService
{
    private readonly MLContext _mlContext;
    private ITransformer _noShowModel;
    private ITransformer _demandModel;
    private ITransformer _churnModel;
    
    public MachineLearningService()
    {
        _mlContext = new MLContext();
        LoadModels();
    }
    
    public async Task<NoShowPrediction> PredictNoShow(Guid appointmentId)
    {
        // Load appointment data
        var appointment = await GetAppointmentData(appointmentId);
        
        // Prepare input
        var input = new NoShowPredictionModel
        {
            PatientAge = appointment.Patient.Age,
            DaysSinceScheduling = (appointment.Date - appointment.CreatedAt).Days,
            PreviousNoShows = appointment.Patient.NoShowCount,
            DayOfWeek = (int)appointment.Date.DayOfWeek,
            HourOfDay = appointment.Date.Hour,
            IsFirstAppointment = appointment.IsFirstAppointment ? 1 : 0,
            AppointmentValue = (float)appointment.Value
        };
        
        // Make prediction
        var predictionEngine = _mlContext.Model
            .CreatePredictionEngine<NoShowPredictionModel, NoShowPrediction>(_noShowModel);
        
        var prediction = predictionEngine.Predict(input);
        
        // Determine risk factors
        var riskFactors = new List<string>();
        if (input.PreviousNoShows > 2)
            riskFactors.Add("Histórico de faltas");
        if (input.DaysSinceScheduling > 30)
            riskFactors.Add("Agendamento com muita antecedência");
        if (input.IsFirstAppointment == 1)
            riskFactors.Add("Primeira consulta");
        
        return new NoShowPrediction(
            appointmentId,
            prediction.Probability,
            prediction.Probability > 0.7 ? "High" : 
                prediction.Probability > 0.4 ? "Medium" : "Low",
            riskFactors
        );
    }
    
    public async Task<DemandForecast> PredictDemand(DateTime startDate, DateTime endDate)
    {
        // Time series forecasting using ML.NET
        var historicalData = await GetHistoricalAppointmentCounts();
        
        // Train forecast model
        var dataView = _mlContext.Data.LoadFromEnumerable(historicalData);
        
        var pipeline = _mlContext.Forecasting.ForecastBySsa(
            outputColumnName: "ForecastedAppointments",
            inputColumnName: "AppointmentCount",
            windowSize: 7,
            seriesLength: 30,
            trainSize: historicalData.Count,
            horizon: (endDate - startDate).Days
        );
        
        var model = pipeline.Fit(dataView);
        
        // Make prediction
        var forecastEngine = model.CreateTimeSeriesEngine<AppointmentCountData, AppointmentCountForecast>(_mlContext);
        var forecast = forecastEngine.Predict();
        
        return new DemandForecast(
            endDate,
            (int)forecast.ForecastedAppointments[0],
            0.85m,  // Confidence score
            historicalData.Select(d => d.AppointmentCount).ToList()
        );
    }
    
    private void LoadModels()
    {
        // Load pre-trained models
        _noShowModel = _mlContext.Model.Load("models/noshow-model.zip", out _);
        _demandModel = _mlContext.Model.Load("models/demand-model.zip", out _);
        _churnModel = _mlContext.Model.Load("models/churn-model.zip", out _);
    }
}
```

### Camada de API (API Layer)

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly IPredictionService _predictionService;
    
    [HttpGet("dashboard/clinical")]
    public async Task<IActionResult> GetClinicalDashboard(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var dashboard = await _analyticsService.GetClinicalDashboard(startDate, endDate);
        return Ok(dashboard);
    }
    
    [HttpGet("dashboard/financial")]
    public async Task<IActionResult> GetFinancialDashboard(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var dashboard = await _analyticsService.GetFinancialDashboard(startDate, endDate);
        return Ok(dashboard);
    }
    
    [HttpGet("dashboard/operational")]
    public async Task<IActionResult> GetOperationalDashboard(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var dashboard = await _analyticsService.GetOperationalDashboard(startDate, endDate);
        return Ok(dashboard);
    }
    
    [HttpGet("dashboard/quality")]
    public async Task<IActionResult> GetQualityDashboard(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var dashboard = await _analyticsService.GetQualityDashboard(startDate, endDate);
        return Ok(dashboard);
    }
    
    [HttpGet("predict/demand")]
    public async Task<IActionResult> PredictDemand(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var forecast = await _predictionService.PredictDemand(startDate, endDate);
        return Ok(forecast);
    }
    
    [HttpGet("predict/noshow/{appointmentId}")]
    public async Task<IActionResult> PredictNoShow(Guid appointmentId)
    {
        var prediction = await _predictionService.PredictNoShow(appointmentId);
        return Ok(prediction);
    }
    
    [HttpGet("benchmarking")]
    public async Task<IActionResult> GetBenchmarking()
    {
        var report = await _analyticsService.GetBenchmarkingReport();
        return Ok(report);
    }
}
```

## 🎨 Frontend (Angular)

### Componentes Necessários

```typescript
// Analytics Dashboard Component
@Component({
  selector: 'app-analytics-dashboard',
  template: `
    <mat-tab-group>
      <mat-tab label="Clínico">
        <app-clinical-dashboard></app-clinical-dashboard>
      </mat-tab>
      
      <mat-tab label="Financeiro">
        <app-financial-dashboard></app-financial-dashboard>
      </mat-tab>
      
      <mat-tab label="Operacional">
        <app-operational-dashboard></app-operational-dashboard>
      </mat-tab>
      
      <mat-tab label="Qualidade">
        <app-quality-dashboard></app-quality-dashboard>
      </mat-tab>
      
      <mat-tab label="Previsões">
        <app-predictions-dashboard></app-predictions-dashboard>
      </mat-tab>
      
      <mat-tab label="Benchmarking">
        <app-benchmarking-dashboard></app-benchmarking-dashboard>
      </mat-tab>
    </mat-tab-group>
  `
})
export class AnalyticsDashboardComponent { }

// Clinical Dashboard Component
@Component({
  selector: 'app-clinical-dashboard',
  template: `
    <div class="dashboard-grid">
      <mat-card class="metric-card">
        <mat-card-header>
          <mat-card-title>Taxa de Ocupação</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="metric-value">{{ occupancyRate | percent }}</div>
          <app-gauge-chart [value]="occupancyRate" [max]="1"></app-gauge-chart>
        </mat-card-content>
      </mat-card>
      
      <mat-card class="metric-card">
        <mat-card-header>
          <mat-card-title>Taxa de No-Show</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="metric-value">{{ noShowRate | percent }}</div>
          <div class="metric-trend" [class.positive]="noShowTrend < 0">
            <mat-icon>{{ noShowTrend < 0 ? 'trending_down' : 'trending_up' }}</mat-icon>
            {{ noShowTrend | percent }}
          </div>
        </mat-card-content>
      </mat-card>
      
      <mat-card class="chart-card">
        <mat-card-header>
          <mat-card-title>Top 10 Diagnósticos (CID-10)</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <canvas #diagnosisChart></canvas>
        </mat-card-content>
      </mat-card>
      
      <mat-card class="chart-card">
        <mat-card-header>
          <mat-card-title>Distribuição Demográfica</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <canvas #demographicsChart></canvas>
        </mat-card-content>
      </mat-card>
    </div>
  `
})
export class ClinicalDashboardComponent implements OnInit {
  occupancyRate: number;
  noShowRate: number;
  noShowTrend: number;
  
  async ngOnInit() {
    await this.loadData();
    this.renderCharts();
  }
  
  async loadData() {
    const dashboard = await this.analyticsService
      .getClinicalDashboard(this.startDate, this.endDate);
    
    this.occupancyRate = dashboard.occupancyRate;
    this.noShowRate = dashboard.noShowRate;
    // ... load other data
  }
  
  renderCharts() {
    // Use Chart.js, Plotly.js, or other library
    this.renderDiagnosisChart();
    this.renderDemographicsChart();
  }
}
```

### Chart.js Integration

```typescript
renderDiagnosisChart() {
  const ctx = this.diagnosisChart.nativeElement.getContext('2d');
  
  new Chart(ctx, {
    type: 'bar',
    data: {
      labels: this.topDiagnoses.map(d => d.diagnosisName),
      datasets: [{
        label: 'Número de Casos',
        data: this.topDiagnoses.map(d => d.count),
        backgroundColor: 'rgba(54, 162, 235, 0.5)',
        borderColor: 'rgba(54, 162, 235, 1)',
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  });
}
```

## 📋 Checklist de Implementação

### Backend

- [ ] Criar entidades de analytics
- [ ] Implementar serviços de cálculo de métricas
- [ ] Criar dashboards (Clinical, Financial, Operational, Quality)
- [ ] Implementar análises avançadas
- [ ] Configurar ML.NET
- [ ] Treinar modelos de previsão (no-show, demanda, churn)
- [ ] Implementar serviço de predição
- [ ] Criar controllers de analytics
- [ ] Implementar cache de métricas
- [ ] Implementar benchmarking anônimo
- [ ] Adicionar migrations
- [ ] Implementar testes

### Frontend

- [ ] Criar componentes de dashboard
- [ ] Integrar Chart.js ou Plotly.js
- [ ] Implementar filtros de período
- [ ] Criar visualizações interativas
- [ ] Implementar comparação de períodos
- [ ] Criar gauge charts
- [ ] Implementar dashboards responsivos
- [ ] Adicionar exportação de dados (CSV, PDF)
- [ ] Criar relatórios executivos

### Machine Learning

- [ ] Coletar dados históricos
- [ ] Preparar datasets de treinamento
- [ ] Treinar modelo de no-show
- [ ] Treinar modelo de demanda
- [ ] Treinar modelo de churn
- [ ] Validar modelos
- [ ] Implementar pipeline de retreinamento
- [ ] Monitorar performance dos modelos

## 🧪 Testes

### Testes Unitários
```csharp
public class AnalyticsServiceTests
{
    [Fact]
    public async Task ShouldCalculateOccupancyRate()
    {
        // Test occupancy calculation
    }
    
    [Fact]
    public async Task ShouldCalculateNPS()
    {
        // Test NPS calculation
    }
}
```

## 📚 Referências

- [PENDING_TASKS.md - Seção BI e Analytics](../../PENDING_TASKS.md#10-bi-e-analytics-avançados)
- [ML.NET Documentation](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet)
- [Chart.js Documentation](https://www.chartjs.org/docs/latest/)

## 💰 Investimento

- **Desenvolvimento**: 3-4 meses, 2 devs
- **Custo**: R$ 90k
- **ROI Esperado**: Insights valiosos, melhor tomada de decisão

## ✅ Critérios de Aceitação

1. ✅ 4 dashboards funcionando (Clinical, Financial, Operational, Quality)
2. ✅ Gráficos interativos com Chart.js
3. ✅ Métricas calculadas corretamente
4. ✅ Filtros de período funcionando
5. ✅ Comparação de períodos implementada
6. ✅ Modelo de previsão de no-show com 75%+ de acurácia
7. ✅ Modelo de previsão de demanda funcionando
8. ✅ Benchmarking anônimo disponível
9. ✅ Exportação de dados (CSV, PDF)
10. ✅ Performance otimizada (< 3s para carregar dashboard)

---

**Última Atualização**: Janeiro 2026
**Status**: Pronto para desenvolvimento
**Próximo Passo**: Coletar dados históricos e treinar modelos ML
