# 📊 Prompt: Benchmarking Anônimo

## 📊 Status
- **Prioridade**: BAIXA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 1 mês | 1 dev
- **Prazo**: Q3/2026

## 🎯 Contexto

Sistema de benchmarking anônimo que permite clínicas compararem sua performance com médias do mercado em métricas como ticket médio, taxa de no-show, tempo de consulta, receita por paciente, satisfação (NPS) e eficiência da agenda.

## 📋 Justificativa

### Benefícios
- ✅ Identificar áreas de melhoria
- ✅ Estabelecer metas realistas
- ✅ Comparação com mercado
- ✅ Insights competitivos
- ✅ Motivação da equipe

## 🏗️ Arquitetura

```csharp
// Métricas Anônimas
public class AnonymousBenchmarkData : Entity
{
    public Guid Id { get; set; }
    public string AnonymousTenantId { get; set; }  // Hash do TenantId
    public DateTime Period { get; set; }
    public string ClinicSize { get; set; }  // Pequena, Média, Grande
    public string ClinicType { get; set; }  // Geral, Especializada
    public string Region { get; set; }  // Sul, Sudeste, etc
    
    // Métricas Financeiras
    public decimal AverageTicket { get; set; }
    public decimal RevenuePerPatient { get; set; }
    public decimal RevenuePerDoctor { get; set; }
    
    // Métricas Operacionais
    public double NoShowRate { get; set; }
    public double AverageConsultationTime { get; set; }
    public double AgendaEfficiency { get; set; }
    
    // Métricas de Qualidade
    public int NpsScore { get; set; }
    public double PatientReturnRate { get; set; }
}

// Benchmark Report
public class BenchmarkReport
{
    public Dictionary<string, MetricComparison> Metrics { get; set; }
    public List<Insight> Insights { get; set; }
}

public class MetricComparison
{
    public string MetricName { get; set; }
    public decimal YourValue { get; set; }
    public decimal MarketAverage { get; set; }
    public decimal Top25Percentile { get; set; }
    public decimal Bottom25Percentile { get; set; }
    public int YourRanking { get; set; }  // Percentil
}
```

## 🎨 Frontend

```typescript
@Component({
  selector: 'app-benchmark-dashboard',
  template: `
    <h2>Benchmarking</h2>
    
    <mat-card *ngFor="let metric of metrics">
      <h3>{{ metric.metricName }}</h3>
      <div class="comparison">
        <span class="your-value">Seu: {{ metric.yourValue }}</span>
        <span class="market-avg">Mercado: {{ metric.marketAverage }}</span>
      </div>
      <mat-progress-bar mode="determinate" [value]="metric.yourRanking"></mat-progress-bar>
      <p>Você está no top {{ metric.yourRanking }}% do mercado</p>
    </mat-card>
  `
})
export class BenchmarkDashboardComponent {
  metrics: MetricComparison[] = [];
}
```

## ✅ Checklist

- [ ] Agregação anônima de dados
- [ ] Cálculo de percentis
- [ ] Dashboard de comparação
- [ ] Insights automáticos
- [ ] Filtros (região, tamanho, tipo)

## 💰 Investimento

- **Esforço**: 1 mês | 1 dev
- **Custo**: R$ 45k

## 🎯 Critérios de Aceitação

- [ ] Dados anônimos
- [ ] Comparação com mercado funciona
- [ ] Dashboard visual
- [ ] Insights gerados automaticamente
