# Guia de Configuração de Monitoramento - System Admin

**Versão:** 1.0  
**Data:** Janeiro 2026  
**Status:** Ativo

---

## 📋 Visão Geral

Este guia apresenta como configurar e usar o sistema de monitoramento completo do System Admin, incluindo RUM (Real User Monitoring), error tracking e dashboards de performance.

---

## 🎯 Objetivos do Monitoramento

### Performance Monitoring
- Coletar Web Vitals (FCP, LCP, FID, CLS, TTFB)
- Monitorar tempo de carregamento de páginas
- Identificar páginas lentas
- Rastrear duração de chamadas API

### Error Tracking
- Capturar erros JavaScript
- Rastrear erros HTTP
- Classificar por severidade
- Correlacionar com contexto de usuário

### Real User Monitoring (RUM)
- Métricas de usuários reais
- Análise por dispositivo/browser
- Performance geográfica
- Tendências ao longo do tempo

---

## 🚀 Configuração Frontend

### 1. RUM Service

O RUM Service é automaticamente inicializado e coleta métricas:

```typescript
// Já configurado em app.config.ts
import { RealUserMonitoringService } from '@app/services/rum.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    RealUserMonitoringService, // Inicia automaticamente
    // ...
  ]
};
```

### 2. Error Tracking Service

O Error Tracking Service captura erros automaticamente:

```typescript
// Configurado como ErrorHandler global
import { ErrorTrackingService } from '@app/services/error-tracking.service';

export const appConfig: ApplicationConfig = {
  providers: [
    {
      provide: ErrorHandler,
      useClass: ErrorTrackingService
    },
    // ...
  ]
};
```

### 3. Manual Tracking

#### Track Custom Metrics

```typescript
import { RealUserMonitoringService } from '@app/services/rum.service';

export class MyComponent {
  constructor(private rum: RealUserMonitoringService) {}
  
  async fetchData() {
    const start = performance.now();
    
    try {
      const data = await this.http.get('/api/data').toPromise();
      const duration = performance.now() - start;
      
      // Track API call
      this.rum.trackApiCall('/api/data', duration, 200);
      
      return data;
    } catch (error) {
      const duration = performance.now() - start;
      this.rum.trackApiCall('/api/data', duration, 500);
      throw error;
    }
  }
}
```

#### Track Custom Events

```typescript
this.rum.trackCustomMetric('feature_usage', {
  feature: 'export_report',
  format: 'pdf',
  duration: 2500
});
```

#### Track Errors with Context

```typescript
import { ErrorTrackingService } from '@app/services/error-tracking.service';

export class MyComponent {
  constructor(private errorTracking: ErrorTrackingService) {}
  
  async doOperation() {
    try {
      await this.performRiskyOperation();
    } catch (error) {
      this.errorTracking.trackCustomError(
        'Operation failed',
        'high',
        {
          operation: 'performRiskyOperation',
          userId: this.currentUser.id,
          clinicId: this.clinic.id,
          timestamp: new Date().toISOString()
        }
      );
    }
  }
}
```

---

## 🔧 Configuração Backend

### 1. Monitoring Service

O MonitoringService está registrado automaticamente:

```csharp
// Program.cs
builder.Services.AddSingleton<IMonitoringService, MonitoringService>();
```

### 2. Endpoints Disponíveis

#### Receber Métricas RUM

```http
POST /api/system-admin/monitoring/rum/metrics
Content-Type: application/json

{
  "metric": "fcp",
  "value": 850.5,
  "url": "/dashboard",
  "userAgent": "Mozilla/5.0...",
  "connectionType": "4g"
}
```

#### Receber Erros

```http
POST /api/system-admin/monitoring/errors
Content-Type: application/json

{
  "message": "Cannot read property 'id' of undefined",
  "stack": "Error: ...\n  at MyComponent...",
  "severity": "high",
  "url": "/clinics/123",
  "userAgent": "Mozilla/5.0...",
  "context": {
    "userId": "user-123",
    "action": "save_clinic"
  }
}
```

#### Obter Web Vitals Summary

```http
GET /api/system-admin/monitoring/web-vitals?days=7
Authorization: Bearer {token}

Response:
{
  "avgFcp": 850.5,
  "avgLcp": 2150.3,
  "avgFid": 45.2,
  "avgCls": 0.08,
  "avgTtfb": 520.1,
  "sampleCount": 1250,
  "lastUpdated": "2026-01-29T12:00:00Z"
}
```

#### Obter Páginas Lentas

```http
GET /api/system-admin/monitoring/slow-pages?limit=10
Authorization: Bearer {token}

Response:
[
  {
    "url": "/reports/custom",
    "avgLoadTime": 3500.5,
    "viewCount": 125,
    "p50LoadTime": 2800.0,
    "p95LoadTime": 5200.0,
    "p99LoadTime": 6500.0
  }
]
```

---

## 📊 Dashboards e Visualização

### 1. Web Vitals Dashboard

Crie um dashboard para monitorar Web Vitals:

```typescript
export class PerformanceDashboardComponent implements OnInit {
  webVitals$: Observable<WebVitalsSummaryDto>;
  slowPages$: Observable<PagePerformanceDto[]>;
  
  constructor(private http: HttpClient) {}
  
  ngOnInit() {
    this.loadMetrics();
  }
  
  loadMetrics() {
    this.webVitals$ = this.http.get<WebVitalsSummaryDto>(
      '/api/system-admin/monitoring/web-vitals?days=7'
    );
    
    this.slowPages$ = this.http.get<PagePerformanceDto[]>(
      '/api/system-admin/monitoring/slow-pages?limit=10'
    );
  }
}
```

Template:

```html
<div class="performance-dashboard">
  <h2>Web Vitals (últimos 7 dias)</h2>
  
  <div class="metrics-grid" *ngIf="webVitals$ | async as vitals">
    <app-metric-card
      title="FCP"
      [value]="vitals.avgFcp"
      unit="ms"
      [threshold]="1000"
      [good]="vitals.avgFcp < 1000"
    ></app-metric-card>
    
    <app-metric-card
      title="LCP"
      [value]="vitals.avgLcp"
      unit="ms"
      [threshold]="2500"
      [good]="vitals.avgLcp < 2500"
    ></app-metric-card>
    
    <app-metric-card
      title="FID"
      [value]="vitals.avgFid"
      unit="ms"
      [threshold]="100"
      [good]="vitals.avgFid < 100"
    ></app-metric-card>
    
    <app-metric-card
      title="CLS"
      [value]="vitals.avgCls"
      [threshold]="0.1"
      [good]="vitals.avgCls < 0.1"
    ></app-metric-card>
  </div>
  
  <h2>Páginas Mais Lentas</h2>
  
  <table mat-table [dataSource]="slowPages$ | async">
    <ng-container matColumnDef="url">
      <th mat-header-cell *matHeaderCellDef>URL</th>
      <td mat-cell *matCellDef="let page">{{ page.url }}</td>
    </ng-container>
    
    <ng-container matColumnDef="avgLoadTime">
      <th mat-header-cell *matHeaderCellDef>Tempo Médio</th>
      <td mat-cell *matCellDef="let page">
        {{ page.avgLoadTime | number:'1.0-0' }}ms
      </td>
    </ng-container>
    
    <ng-container matColumnDef="p95LoadTime">
      <th mat-header-cell *matHeaderCellDef>P95</th>
      <td mat-cell *matCellDef="let page">
        {{ page.p95LoadTime | number:'1.0-0' }}ms
      </td>
    </ng-container>
    
    <ng-container matColumnDef="viewCount">
      <th mat-header-cell *matHeaderCellDef>Visualizações</th>
      <td mat-cell *matCellDef="let page">{{ page.viewCount }}</td>
    </ng-container>
    
    <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
    <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
  </table>
</div>
```

---

## 🔔 Alertas

### 1. Configurar Alertas de Performance

```typescript
export class AlertService {
  constructor(
    private http: HttpClient,
    private notification: NotificationService
  ) {
    this.setupPerformanceAlerts();
  }
  
  private setupPerformanceAlerts() {
    // Check Web Vitals a cada 5 minutos
    interval(5 * 60 * 1000)
      .pipe(
        switchMap(() => 
          this.http.get<WebVitalsSummaryDto>(
            '/api/system-admin/monitoring/web-vitals?days=1'
          )
        )
      )
      .subscribe(vitals => {
        if (vitals.avgLcp > 3000) {
          this.notification.error(
            `LCP acima do limite: ${vitals.avgLcp}ms (limite: 2500ms)`
          );
        }
        
        if (vitals.avgFcp > 1500) {
          this.notification.warning(
            `FCP acima do ideal: ${vitals.avgFcp}ms (ideal: < 1000ms)`
          );
        }
      });
  }
}
```

### 2. Alertas de Erros

```typescript
export class ErrorAlertService {
  private errorCount = 0;
  private readonly ERROR_THRESHOLD = 10;
  private readonly TIME_WINDOW = 5 * 60 * 1000; // 5 minutos
  
  constructor(private notification: NotificationService) {
    this.resetCounter();
  }
  
  trackError(error: ErrorTrackingDto) {
    this.errorCount++;
    
    if (this.errorCount >= this.ERROR_THRESHOLD) {
      this.notification.error(
        `Alto volume de erros: ${this.errorCount} erros em 5 minutos`
      );
    }
  }
  
  private resetCounter() {
    setInterval(() => {
      this.errorCount = 0;
    }, this.TIME_WINDOW);
  }
}
```

---

## 📈 Análise de Dados

### 1. Tendências de Performance

```typescript
export class PerformanceTrendsComponent {
  chartData: ApexAxisChartSeries = [];
  
  async loadTrends() {
    const last30Days = [];
    
    for (let i = 29; i >= 0; i--) {
      const date = new Date();
      date.setDate(date.getDate() - i);
      
      const vitals = await this.getWebVitalsForDate(date);
      last30Days.push({
        date: date.toISOString().split('T')[0],
        fcp: vitals.avgFcp,
        lcp: vitals.avgLcp,
        fid: vitals.avgFid
      });
    }
    
    this.chartData = [
      {
        name: 'FCP',
        data: last30Days.map(d => d.fcp)
      },
      {
        name: 'LCP',
        data: last30Days.map(d => d.lcp)
      },
      {
        name: 'FID',
        data: last30Days.map(d => d.fid)
      }
    ];
  }
}
```

### 2. Análise por Device/Browser

```typescript
export class DeviceAnalysisComponent {
  async analyzeByDevice() {
    const metrics = await this.loadAllMetrics();
    
    const byDevice = metrics.reduce((acc, m) => {
      const device = this.detectDevice(m.userAgent);
      if (!acc[device]) acc[device] = [];
      acc[device].push(m);
      return acc;
    }, {} as Record<string, RumMetricDto[]>);
    
    return Object.entries(byDevice).map(([device, metrics]) => ({
      device,
      avgLoadTime: this.average(metrics.map(m => m.value)),
      count: metrics.length
    }));
  }
  
  private detectDevice(userAgent: string): string {
    if (/mobile/i.test(userAgent)) return 'Mobile';
    if (/tablet/i.test(userAgent)) return 'Tablet';
    return 'Desktop';
  }
}
```

---

## 🛠️ Integração com Ferramentas Externas

### Application Insights (Opcional)

Se você tiver Application Insights configurado:

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// MonitoringService.cs
public class MonitoringService : IMonitoringService
{
    private readonly TelemetryClient _telemetry;
    
    public MonitoringService(
        ILogger<MonitoringService> logger,
        TelemetryClient telemetry)
    {
        _logger = logger;
        _telemetry = telemetry;
    }
    
    public Task TrackRumMetricAsync(RumMetricDto metric)
    {
        // Track to Application Insights
        _telemetry.TrackMetric(
            metric.Metric,
            metric.Value,
            new Dictionary<string, string>
            {
                { "url", metric.Url },
                { "userAgent", metric.UserAgent ?? "" }
            }
        );
        
        // Também salva localmente
        // ...
    }
}
```

### Sentry (Opcional)

Para error tracking avançado:

```typescript
import * as Sentry from '@sentry/angular';

// app.config.ts
export const appConfig: ApplicationConfig = {
  providers: [
    {
      provide: ErrorHandler,
      useValue: Sentry.createErrorHandler()
    },
    {
      provide: Sentry.TraceService,
      deps: [Router]
    },
    // ...
  ]
};

// main.ts
Sentry.init({
  dsn: environment.sentryDsn,
  integrations: [
    Sentry.browserTracingIntegration(),
    Sentry.replayIntegration()
  ],
  tracesSampleRate: 1.0,
  replaysSessionSampleRate: 0.1,
  replaysOnErrorSampleRate: 1.0
});
```

---

## ✅ Checklist de Setup

### Initial Setup
- [x] RUM Service configurado
- [x] Error Tracking Service configurado
- [x] Backend endpoints criados
- [x] MonitoringService registrado no DI

### Monitoramento Ativo
- [ ] Dashboard de Web Vitals criado
- [ ] Alertas configurados
- [ ] Integração com Application Insights (opcional)
- [ ] Integração com Sentry (opcional)

### Manutenção
- [ ] Revisar métricas semanalmente
- [ ] Investigar páginas lentas
- [ ] Analisar erros frequentes
- [ ] Otimizar com base em dados

---

## 📊 Métricas-Chave para Acompanhar

### Diariamente
- Volume de erros
- LCP médio
- Páginas mais lentas

### Semanalmente
- Tendências de Web Vitals
- Performance por device/browser
- Taxa de erros por feature

### Mensalmente
- Comparação mês a mês
- ROI de otimizações
- Satisfação do usuário (NPS correlacionado com performance)

---

## 📚 Referências

- **Web Vitals**: https://web.dev/vitals/
- **RUM Best Practices**: https://web.dev/vitals-measurement-getting-started/
- **Application Insights**: https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview
- **Sentry Documentation**: https://docs.sentry.io/platforms/javascript/guides/angular/

---

**Última Atualização:** Janeiro 2026  
**Próxima Revisão:** Março 2026  
**Responsável:** Equipe de DevOps
