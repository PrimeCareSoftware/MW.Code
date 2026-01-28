# 📊 Fase 3: Analytics e BI - System Admin

**Prioridade:** 🔥🔥 P1 - ALTA  
**Status:** Planejamento  
**Esforço:** 2 meses | 2 desenvolvedores  
**Custo Estimado:** R$ 78.000  
**Prazo:** Q2 2026

---

## 📋 Contexto

### Situação Atual

O system-admin possui dashboard básico com métricas simples, mas falta capacidade de análise profunda e relatórios customizáveis.

**Funcionalidades Existentes:**
- ✅ Dashboard com gráficos básicos (ApexCharts)
- ✅ Métricas de MRR, clientes, usuários
- ❌ Sem dashboards customizáveis
- ❌ Sem relatórios avançados
- ❌ Sem análise de coortes
- ❌ Sem agendamento de relatórios

### Objetivo da Fase 3

Transformar o system-admin em uma **plataforma de BI** com:
1. Dashboards customizáveis (drag-and-drop)
2. Biblioteca de relatórios profissionais
3. Análise de coortes para retenção e receita
4. Agendamento e exportação automatizada

**Inspiração:** Forest Admin, Metabase, Stripe Analytics

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais
1. Criar editor de dashboards com widgets drag-and-drop
2. Implementar biblioteca de relatórios pré-construídos
3. Adicionar análise de coortes (retenção, MRR, comportamento)
4. Agendar relatórios automáticos com envio por email

### Benefícios Esperados
- 📊 **Análise self-service** - criar dashboards sem programação
- 📈 **Insights profundos** - entender padrões de churn e crescimento
- ⏰ **Automação** - relatórios recorrentes sem intervenção manual
- 📤 **Exportação profissional** - PDFs com branding

---

## 📝 Tarefas Detalhadas

### 1. Dashboards Customizáveis (4 semanas)

#### 1.1 Backend - Dashboard Engine

**Entidades:**
```csharp
// Entities/CustomDashboard.cs
public class CustomDashboard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Layout { get; set; } // JSON com grid layout
    public bool IsDefault { get; set; }
    public bool IsPublic { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<DashboardWidget> Widgets { get; set; }
}

// Entities/DashboardWidget.cs
public class DashboardWidget
{
    public int Id { get; set; }
    public int DashboardId { get; set; }
    public CustomDashboard Dashboard { get; set; }
    
    public string Type { get; set; } // line, bar, pie, metric, table, map, markdown
    public string Title { get; set; }
    public string Config { get; set; } // JSON com configurações
    public string Query { get; set; } // SQL query ou endpoint
    public int RefreshInterval { get; set; } // segundos (0 = manual)
    
    public int GridX { get; set; }
    public int GridY { get; set; }
    public int GridWidth { get; set; }
    public int GridHeight { get; set; }
}

// Entities/WidgetTemplate.cs
public class WidgetTemplate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; } // financial, customer, operational
    public string Type { get; set; }
    public string DefaultConfig { get; set; }
    public string DefaultQuery { get; set; }
    public bool IsSystem { get; set; }
}
```

**Serviço de Dashboards:**
```csharp
// Services/Dashboards/DashboardService.cs
public interface IDashboardService
{
    Task<List<CustomDashboardDto>> GetAllDashboards();
    Task<CustomDashboardDto> GetDashboard(int id);
    Task<CustomDashboardDto> CreateDashboard(CreateDashboardDto dto);
    Task<CustomDashboardDto> UpdateDashboard(int id, UpdateDashboardDto dto);
    Task DeleteDashboard(int id);
    Task<DashboardWidgetDto> AddWidget(int dashboardId, CreateWidgetDto dto);
    Task UpdateWidgetPosition(int widgetId, WidgetPositionDto position);
    Task<object> ExecuteWidgetQuery(int widgetId);
    Task<string> ExportDashboard(int id, ExportFormat format);
}

public class DashboardService : IDashboardService
{
    public async Task<object> ExecuteWidgetQuery(int widgetId)
    {
        var widget = await _context.DashboardWidgets
            .Include(w => w.Dashboard)
            .FirstOrDefaultAsync(w => w.Id == widgetId);
            
        if (widget == null)
            throw new NotFoundException("Widget not found");
            
        // Se tem query SQL customizada
        if (!string.IsNullOrEmpty(widget.Query))
        {
            // IMPORTANTE: Validar query para segurança (apenas SELECT)
            if (!IsQuerySafe(widget.Query))
                throw new SecurityException("Query contains unsafe operations");
                
            return await ExecuteSqlQuery(widget.Query);
        }
        
        // Se é um template pré-definido, usar endpoint configurado
        var config = JsonSerializer.Deserialize<WidgetConfig>(widget.Config);
        return await ExecuteEndpoint(config.Endpoint, config.Parameters);
    }
    
    private bool IsQuerySafe(string query)
    {
        var upperQuery = query.ToUpper().Trim();
        
        // Apenas SELECT é permitido
        if (!upperQuery.StartsWith("SELECT"))
            return false;
            
        // Proibir comandos perigosos
        var dangerousKeywords = new[] 
        { 
            "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", 
            "ALTER", "EXEC", "EXECUTE", "TRUNCATE" 
        };
        
        return !dangerousKeywords.Any(k => upperQuery.Contains(k));
    }
    
    private async Task<object> ExecuteSqlQuery(string query)
    {
        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.CommandTimeout = 30; // 30 segundos max
        
        using var reader = await command.ExecuteReaderAsync();
        var results = new List<Dictionary<string, object>>();
        
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            results.Add(row);
        }
        
        return results;
    }
}
```

**Templates de Widgets Pré-construídos:**
```csharp
// Data/WidgetTemplateSeeder.cs
public class WidgetTemplateSeeder
{
    public static void Seed(AppDbContext context)
    {
        var templates = new List<WidgetTemplate>
        {
            // Financial Templates
            new WidgetTemplate
            {
                Name = "MRR Over Time",
                Description = "Monthly Recurring Revenue trend",
                Category = "financial",
                Type = "line",
                IsSystem = true,
                DefaultQuery = @"
                    SELECT 
                        DATE_TRUNC('month', created_at) as month,
                        SUM(mrr) as total_mrr
                    FROM subscriptions
                    WHERE created_at >= NOW() - INTERVAL '12 months'
                    GROUP BY month
                    ORDER BY month",
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    xAxis = "month",
                    yAxis = "total_mrr",
                    color = "#10b981"
                })
            },
            
            new WidgetTemplate
            {
                Name = "Revenue Breakdown",
                Description = "MRR by plan type",
                Category = "financial",
                Type = "pie",
                IsSystem = true,
                DefaultQuery = @"
                    SELECT 
                        p.name as plan,
                        SUM(s.mrr) as revenue
                    FROM subscriptions s
                    JOIN plans p ON s.plan_id = p.id
                    WHERE s.status = 'Active'
                    GROUP BY p.name",
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    labelField = "plan",
                    valueField = "revenue"
                })
            },
            
            // Customer Templates
            new WidgetTemplate
            {
                Name = "Active Customers",
                Description = "Total active customers count",
                Category = "customer",
                Type = "metric",
                IsSystem = true,
                DefaultQuery = @"
                    SELECT COUNT(*) as value
                    FROM clinics
                    WHERE is_active = true",
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    format = "number",
                    icon = "people",
                    color = "#3b82f6"
                })
            },
            
            new WidgetTemplate
            {
                Name = "Customer Growth",
                Description = "New customers over time",
                Category = "customer",
                Type = "bar",
                IsSystem = true,
                DefaultQuery = @"
                    SELECT 
                        DATE_TRUNC('month', created_at) as month,
                        COUNT(*) as new_customers
                    FROM clinics
                    WHERE created_at >= NOW() - INTERVAL '12 months'
                    GROUP BY month
                    ORDER BY month"
            },
            
            // Operational Templates
            new WidgetTemplate
            {
                Name = "Open Tickets",
                Description = "Current open support tickets",
                Category = "operational",
                Type = "metric",
                IsSystem = true,
                DefaultQuery = @"
                    SELECT COUNT(*) as value
                    FROM tickets
                    WHERE status = 'Open'",
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    format = "number",
                    icon = "support",
                    color = "#f59e0b",
                    threshold = new { warning = 10, critical = 20 }
                })
            }
        };
        
        context.WidgetTemplates.AddRange(templates);
        context.SaveChanges();
    }
}
```

**Endpoints API:**
```csharp
// Controllers/SystemAdmin/DashboardsController.cs
[ApiController]
[Route("api/system-admin/dashboards")]
[Authorize(Roles = "SystemAdmin")]
public class DashboardsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CustomDashboardDto>>> GetAll()
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomDashboardDto>> Get(int id)
    
    [HttpPost]
    public async Task<ActionResult<CustomDashboardDto>> Create(
        [FromBody] CreateDashboardDto dto)
    
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomDashboardDto>> Update(
        int id, [FromBody] UpdateDashboardDto dto)
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    
    [HttpPost("{id}/widgets")]
    public async Task<ActionResult<DashboardWidgetDto>> AddWidget(
        int id, [FromBody] CreateWidgetDto dto)
    
    [HttpGet("widgets/{widgetId}/data")]
    public async Task<ActionResult<object>> GetWidgetData(int widgetId)
    
    [HttpGet("templates")]
    public async Task<ActionResult<List<WidgetTemplateDto>>> GetTemplates()
    
    [HttpPost("{id}/export")]
    public async Task<IActionResult> Export(
        int id, [FromQuery] ExportFormat format = ExportFormat.Pdf)
}
```

#### 1.2 Frontend - Dashboard Editor

**Dashboard Editor Component:**
```typescript
// system-admin/src/app/dashboards/dashboard-editor/dashboard-editor.component.ts
import GridStack from 'gridstack';
import 'gridstack/dist/gridstack.min.css';

@Component({
  selector: 'app-dashboard-editor',
  standalone: true,
  template: `
    <div class="dashboard-editor">
      <!-- Toolbar -->
      <mat-toolbar>
        <mat-form-field class="dashboard-name">
          <input matInput [(ngModel)]="dashboard.name" placeholder="Nome do Dashboard">
        </mat-form-field>
        
        <span class="spacer"></span>
        
        <button mat-button (click)="openWidgetLibrary()">
          <mat-icon>add</mat-icon> Adicionar Widget
        </button>
        
        <button mat-button (click)="toggleEditMode()">
          <mat-icon>{{ editMode ? 'lock_open' : 'lock' }}</mat-icon>
          {{ editMode ? 'Bloquear' : 'Editar' }}
        </button>
        
        <button mat-raised-button color="primary" (click)="saveDashboard()">
          <mat-icon>save</mat-icon> Salvar
        </button>
        
        <button mat-button [matMenuTriggerFor]="moreMenu">
          <mat-icon>more_vert</mat-icon>
        </button>
      </mat-toolbar>
      
      <!-- Grid -->
      <div class="grid-stack" #gridstack>
        <div 
          *ngFor="let widget of dashboard.widgets"
          class="grid-stack-item"
          [attr.gs-id]="widget.id"
          [attr.gs-x]="widget.gridX"
          [attr.gs-y]="widget.gridY"
          [attr.gs-w]="widget.gridWidth"
          [attr.gs-h]="widget.gridHeight"
        >
          <div class="grid-stack-item-content">
            <app-dashboard-widget 
              [widget]="widget"
              [editMode]="editMode"
              (edit)="editWidget($event)"
              (delete)="deleteWidget($event)"
            />
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-editor {
      height: 100vh;
      display: flex;
      flex-direction: column;
    }
    
    .grid-stack {
      flex: 1;
      background: #f5f5f5;
      padding: 16px;
    }
    
    .grid-stack-item-content {
      background: white;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      overflow: hidden;
    }
  `]
})
export class DashboardEditorComponent implements OnInit, AfterViewInit {
  @ViewChild('gridstack') gridstackEl: ElementRef;
  
  dashboard: CustomDashboard;
  editMode = true;
  grid: GridStack;
  
  ngAfterViewInit() {
    this.initGridStack();
  }
  
  initGridStack() {
    this.grid = GridStack.init({
      cellHeight: 80,
      column: 12,
      animate: true,
      float: true,
      resizable: {
        handles: 'e, se, s, sw, w'
      }
    }, this.gridstackEl.nativeElement);
    
    // Listen to changes
    this.grid.on('change', (event, items) => {
      items.forEach(item => {
        const widget = this.dashboard.widgets.find(w => w.id === item.id);
        if (widget) {
          widget.gridX = item.x;
          widget.gridY = item.y;
          widget.gridWidth = item.w;
          widget.gridHeight = item.h;
        }
      });
    });
  }
  
  toggleEditMode() {
    this.editMode = !this.editMode;
    if (this.editMode) {
      this.grid.enable();
    } else {
      this.grid.disable();
    }
  }
  
  async openWidgetLibrary() {
    const dialogRef = this.dialog.open(WidgetLibraryDialogComponent, {
      width: '800px',
      data: { dashboardId: this.dashboard.id }
    });
    
    const widget = await dialogRef.afterClosed().toPromise();
    if (widget) {
      this.dashboard.widgets.push(widget);
      this.grid.addWidget({
        id: widget.id,
        x: 0,
        y: 0,
        w: 4,
        h: 3
      });
    }
  }
  
  async saveDashboard() {
    await this.dashboardService.update(this.dashboard.id, this.dashboard);
    this.snackBar.open('Dashboard salvo!', 'OK', { duration: 3000 });
  }
}
```

**Widget Component:**
```typescript
// system-admin/src/app/dashboards/dashboard-widget/dashboard-widget.component.ts
@Component({
  selector: 'app-dashboard-widget',
  standalone: true,
  template: `
    <div class="widget-container">
      <!-- Header -->
      <div class="widget-header">
        <h3>{{ widget.title }}</h3>
        <div class="widget-actions" *ngIf="editMode">
          <button mat-icon-button (click)="edit.emit(widget)">
            <mat-icon>settings</mat-icon>
          </button>
          <button mat-icon-button (click)="delete.emit(widget)">
            <mat-icon>delete</mat-icon>
          </button>
        </div>
      </div>
      
      <!-- Content -->
      <div class="widget-content" [ngSwitch]="widget.type">
        <!-- Line Chart -->
        <apx-chart
          *ngSwitchCase="'line'"
          [series]="chartData.series"
          [chart]="{ type: 'line', height: '100%' }"
          [xaxis]="chartData.xaxis"
        ></apx-chart>
        
        <!-- Bar Chart -->
        <apx-chart
          *ngSwitchCase="'bar'"
          [series]="chartData.series"
          [chart]="{ type: 'bar', height: '100%' }"
        ></apx-chart>
        
        <!-- Pie Chart -->
        <apx-chart
          *ngSwitchCase="'pie'"
          [series]="chartData.series"
          [chart]="{ type: 'pie', height: '100%' }"
          [labels]="chartData.labels"
        ></apx-chart>
        
        <!-- Metric Card -->
        <div *ngSwitchCase="'metric'" class="metric-card">
          <mat-icon [style.color]="config.color">{{ config.icon }}</mat-icon>
          <div class="metric-value">{{ data?.value | number }}</div>
          <div class="metric-change" [class.positive]="data?.change > 0">
            {{ data?.change > 0 ? '+' : '' }}{{ data?.change }}%
          </div>
        </div>
        
        <!-- Table -->
        <table *ngSwitchCase="'table'" mat-table [dataSource]="data">
          <!-- Dynamic columns based on data -->
        </table>
        
        <!-- Markdown -->
        <div *ngSwitchCase="'markdown'" [innerHTML]="markdownContent | markdown">
        </div>
      </div>
      
      <!-- Loading/Error States -->
      <div *ngIf="loading" class="widget-loading">
        <mat-spinner diameter="40"></mat-spinner>
      </div>
      
      <div *ngIf="error" class="widget-error">
        <mat-icon>error</mat-icon>
        <p>{{ error }}</p>
      </div>
    </div>
  `
})
export class DashboardWidgetComponent implements OnInit {
  @Input() widget: DashboardWidget;
  @Input() editMode = false;
  @Output() edit = new EventEmitter<DashboardWidget>();
  @Output() delete = new EventEmitter<DashboardWidget>();
  
  data: any;
  chartData: any;
  config: any;
  loading = false;
  error: string;
  
  async ngOnInit() {
    this.config = JSON.parse(this.widget.config);
    await this.loadData();
    
    // Auto-refresh
    if (this.widget.refreshInterval > 0) {
      interval(this.widget.refreshInterval * 1000).subscribe(() => {
        this.loadData();
      });
    }
  }
  
  async loadData() {
    try {
      this.loading = true;
      this.error = null;
      
      this.data = await this.dashboardService.getWidgetData(this.widget.id);
      
      // Transform data for charts
      if (['line', 'bar', 'pie'].includes(this.widget.type)) {
        this.chartData = this.transformDataForChart(this.data, this.config);
      }
    } catch (error) {
      this.error = error.message;
    } finally {
      this.loading = false;
    }
  }
  
  transformDataForChart(data: any[], config: any): any {
    if (this.widget.type === 'pie') {
      return {
        series: data.map(d => d[config.valueField]),
        labels: data.map(d => d[config.labelField])
      };
    } else {
      return {
        series: [{
          name: config.yAxis,
          data: data.map(d => d[config.yAxis])
        }],
        xaxis: {
          categories: data.map(d => d[config.xAxis])
        }
      };
    }
  }
}
```

---

### 2. Relatórios Avançados (3 semanas)

#### 2.1 Backend - Report Engine

**Biblioteca de Relatórios:**
```csharp
// Services/Reports/ReportService.cs
public interface IReportService
{
    Task<List<ReportTemplateDto>> GetAvailableReports();
    Task<ReportResultDto> GenerateReport(int templateId, ReportParametersDto parameters);
    Task<byte[]> ExportReport(ReportResultDto report, ExportFormat format);
    Task ScheduleReport(ScheduleReportDto dto);
    Task<List<ScheduledReportDto>> GetScheduledReports();
}

public class ReportService : IReportService
{
    // Relatórios Financeiros
    public async Task<ReportResultDto> GenerateMrrBreakdownReport(ReportParametersDto parameters)
    {
        var startDate = parameters.StartDate;
        var endDate = parameters.EndDate;
        
        var data = await _context.Subscriptions
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .GroupBy(s => new { s.PlanId, Month = s.CreatedAt.Month })
            .Select(g => new
            {
                Plan = g.Key,
                NewMrr = g.Where(s => s.Status == "Active").Sum(s => s.Mrr),
                ChurnedMrr = g.Where(s => s.Status == "Cancelled").Sum(s => s.Mrr)
            })
            .ToListAsync();
            
        return new ReportResultDto
        {
            Title = "MRR Breakdown",
            GeneratedAt = DateTime.UtcNow,
            Data = data,
            Charts = new[]
            {
                new ChartDto
                {
                    Type = "stacked-bar",
                    Title = "MRR by Plan",
                    Data = data
                }
            }
        };
    }
    
    public async Task<ReportResultDto> GenerateChurnAnalysisReport(ReportParametersDto parameters)
    {
        var months = 12;
        var churnData = new List<object>();
        
        for (int i = 0; i < months; i++)
        {
            var month = DateTime.UtcNow.AddMonths(-i);
            var startOfMonth = new DateTime(month.Year, month.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            
            var activeStart = await _context.Subscriptions
                .CountAsync(s => s.Status == "Active" && s.CreatedAt < startOfMonth);
                
            var churned = await _context.Subscriptions
                .CountAsync(s => 
                    s.Status == "Cancelled" && 
                    s.CancelledAt >= startOfMonth && 
                    s.CancelledAt < endOfMonth);
                    
            var churnRate = activeStart > 0 ? (double)churned / activeStart * 100 : 0;
            
            churnData.Add(new
            {
                Month = startOfMonth,
                ActiveCustomers = activeStart,
                ChurnedCustomers = churned,
                ChurnRate = Math.Round(churnRate, 2),
                RevenueChurn = await CalculateRevenueChurn(startOfMonth, endOfMonth)
            });
        }
        
        return new ReportResultDto
        {
            Title = "Churn Analysis",
            GeneratedAt = DateTime.UtcNow,
            Data = churnData.OrderBy(d => ((dynamic)d).Month),
            Charts = new[]
            {
                new ChartDto
                {
                    Type = "line",
                    Title = "Churn Rate Over Time",
                    Data = churnData
                }
            }
        };
    }
    
    // Exportação em PDF
    public async Task<byte[]> ExportReportToPdf(ReportResultDto report)
    {
        var pdf = new PdfDocument();
        var page = pdf.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Arial", 12);
        
        // Header com logo
        gfx.DrawString("PrimeCare - Report", 
            new XFont("Arial", 20, XFontStyle.Bold), 
            XBrushes.Black, 
            new XPoint(50, 50));
            
        gfx.DrawString(report.Title, font, XBrushes.Black, new XPoint(50, 80));
        gfx.DrawString($"Generated: {report.GeneratedAt:yyyy-MM-dd HH:mm}", 
            new XFont("Arial", 10), 
            XBrushes.Gray, 
            new XPoint(50, 100));
            
        // Render data table
        int y = 130;
        foreach (var row in report.Data)
        {
            gfx.DrawString(row.ToString(), font, XBrushes.Black, new XPoint(50, y));
            y += 20;
        }
        
        // Render charts (convert to images first)
        foreach (var chart in report.Charts)
        {
            var chartImage = await RenderChartToImage(chart);
            // Draw image in PDF
        }
        
        using var stream = new MemoryStream();
        pdf.Save(stream);
        return stream.ToArray();
    }
}
```

**Agendamento de Relatórios:**
```csharp
// Jobs/ScheduledReportJob.cs
public class ScheduledReportJob
{
    private readonly IReportService _reportService;
    private readonly IEmailService _emailService;
    
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteScheduledReport(int scheduledReportId)
    {
        var scheduled = await _context.ScheduledReports
            .Include(s => s.Recipients)
            .FirstOrDefaultAsync(s => s.Id == scheduledReportId);
            
        if (scheduled == null || !scheduled.IsEnabled)
            return;
            
        // Gerar relatório
        var report = await _reportService.GenerateReport(
            scheduled.ReportTemplateId, 
            scheduled.Parameters);
            
        // Exportar para formato configurado
        var fileBytes = await _reportService.ExportReport(
            report, 
            scheduled.ExportFormat);
            
        // Enviar por email
        foreach (var recipient in scheduled.Recipients)
        {
            await _emailService.SendEmailWithAttachment(
                to: recipient.Email,
                subject: $"Relatório Agendado: {report.Title}",
                body: $"Segue em anexo o relatório {report.Title} gerado em {report.GeneratedAt}.",
                attachment: fileBytes,
                attachmentName: $"{report.Title}_{DateTime.UtcNow:yyyyMMdd}.pdf"
            );
        }
        
        // Registrar execução
        scheduled.LastExecutedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}

// Configurar jobs recorrentes
RecurringJob.AddOrUpdate<ScheduledReportJob>(
    "daily-reports",
    x => x.ExecuteDailyReports(),
    Cron.Daily);
```

#### 2.2 Frontend - Report Generator

```typescript
// system-admin/src/app/reports/report-generator/report-generator.component.ts
@Component({
  selector: 'app-report-generator',
  standalone: true,
  template: `
    <mat-stepper linear #stepper>
      <!-- Step 1: Selecionar Template -->
      <mat-step label="Selecionar Relatório">
        <div class="report-templates">
          <mat-card 
            *ngFor="let template of templates"
            class="template-card"
            [class.selected]="selectedTemplate?.id === template.id"
            (click)="selectTemplate(template)"
          >
            <mat-card-header>
              <mat-icon mat-card-avatar>{{ template.icon }}</mat-icon>
              <mat-card-title>{{ template.name }}</mat-card-title>
              <mat-card-subtitle>{{ template.category }}</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              {{ template.description }}
            </mat-card-content>
          </mat-card>
        </div>
        <div class="stepper-actions">
          <button mat-raised-button color="primary" (click)="stepper.next()" [disabled]="!selectedTemplate">
            Próximo
          </button>
        </div>
      </mat-step>
      
      <!-- Step 2: Configurar Parâmetros -->
      <mat-step label="Parâmetros">
        <form [formGroup]="parametersForm">
          <mat-form-field>
            <mat-label>Data Inicial</mat-label>
            <input matInput [matDatepicker]="startPicker" formControlName="startDate">
            <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
            <mat-datepicker #startPicker></mat-datepicker>
          </mat-form-field>
          
          <mat-form-field>
            <mat-label>Data Final</mat-label>
            <input matInput [matDatepicker]="endPicker" formControlName="endDate">
            <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
            <mat-datepicker #endPicker></mat-datepicker>
          </mat-form-field>
          
          <!-- Filtros dinâmicos baseados no template -->
          <div *ngFor="let param of selectedTemplate?.parameters">
            <mat-form-field *ngIf="param.type === 'select'">
              <mat-select [formControlName]="param.name" [placeholder]="param.label">
                <mat-option *ngFor="let option of param.options" [value]="option.value">
                  {{ option.label }}
                </mat-option>
              </mat-select>
            </mat-form-field>
          </div>
        </form>
        <div class="stepper-actions">
          <button mat-button (click)="stepper.previous()">Voltar</button>
          <button mat-raised-button color="primary" (click)="stepper.next()">
            Próximo
          </button>
        </div>
      </mat-step>
      
      <!-- Step 3: Preview -->
      <mat-step label="Preview">
        <div class="report-preview">
          <h2>{{ reportResult?.title }}</h2>
          <p class="generated-date">Gerado em: {{ reportResult?.generatedAt | date:'medium' }}</p>
          
          <!-- Charts -->
          <div *ngFor="let chart of reportResult?.charts" class="chart-container">
            <h3>{{ chart.title }}</h3>
            <apx-chart
              [series]="chart.series"
              [chart]="{ type: chart.type, height: 350 }"
            ></apx-chart>
          </div>
          
          <!-- Data Table -->
          <table mat-table [dataSource]="reportResult?.data" class="report-table">
            <!-- Dynamic columns -->
          </table>
        </div>
        <div class="stepper-actions">
          <button mat-button (click)="stepper.previous()">Voltar</button>
          <button mat-raised-button (click)="exportReport('pdf')">
            <mat-icon>picture_as_pdf</mat-icon> Exportar PDF
          </button>
          <button mat-button (click)="exportReport('excel')">
            <mat-icon>table_chart</mat-icon> Exportar Excel
          </button>
          <button mat-button (click)="openScheduleDialog()">
            <mat-icon>schedule</mat-icon> Agendar
          </button>
        </div>
      </mat-step>
    </mat-stepper>
  `
})
export class ReportGeneratorComponent implements OnInit {
  templates: ReportTemplate[] = [];
  selectedTemplate: ReportTemplate;
  parametersForm: FormGroup;
  reportResult: ReportResult;
  
  async ngOnInit() {
    this.templates = await this.reportService.getAvailableReports();
  }
  
  selectTemplate(template: ReportTemplate) {
    this.selectedTemplate = template;
    this.buildParametersForm();
  }
  
  buildParametersForm() {
    const controls = {
      startDate: ['', Validators.required],
      endDate: ['', Validators.required]
    };
    
    // Add dynamic parameters
    this.selectedTemplate.parameters?.forEach(param => {
      controls[param.name] = [param.defaultValue || '', param.required ? Validators.required : []];
    });
    
    this.parametersForm = this.fb.group(controls);
  }
  
  async generatePreview() {
    this.reportResult = await this.reportService.generateReport(
      this.selectedTemplate.id,
      this.parametersForm.value
    );
  }
  
  async exportReport(format: 'pdf' | 'excel') {
    const blob = await this.reportService.exportReport(this.reportResult, format);
    saveAs(blob, `${this.reportResult.title}.${format}`);
  }
  
  openScheduleDialog() {
    this.dialog.open(ScheduleReportDialogComponent, {
      data: {
        template: this.selectedTemplate,
        parameters: this.parametersForm.value
      }
    });
  }
}
```

---

### 3. Cohort Analysis (3 semanas)

#### 3.1 Backend - Cohort Analytics

```csharp
// Services/Analytics/CohortAnalysisService.cs
public interface ICohortAnalysisService
{
    Task<CohortRetentionDto> GetRetentionAnalysis(int months = 12);
    Task<CohortRevenueDto> GetRevenueAnalysis(int months = 12);
    Task<CohortBehaviorDto> GetBehaviorAnalysis(int months = 12);
}

public class CohortAnalysisService : ICohortAnalysisService
{
    public async Task<CohortRetentionDto> GetRetentionAnalysis(int months)
    {
        var result = new CohortRetentionDto();
        var cohorts = new List<CohortDto>();
        
        for (int i = 0; i < months; i++)
        {
            var cohortMonth = DateTime.UtcNow.AddMonths(-i);
            var cohortStart = new DateTime(cohortMonth.Year, cohortMonth.Month, 1);
            var cohortEnd = cohortStart.AddMonths(1);
            
            // Clientes que entraram neste mês (cohort)
            var cohortCustomerIds = await _context.Clinics
                .Where(c => c.CreatedAt >= cohortStart && c.CreatedAt < cohortEnd)
                .Select(c => c.Id)
                .ToListAsync();
                
            if (cohortCustomerIds.Count == 0)
                continue;
                
            var cohort = new CohortDto
            {
                Month = cohortStart,
                Size = cohortCustomerIds.Count,
                RetentionRates = new List<double>()
            };
            
            // Calcular retenção para cada mês subsequente
            for (int j = 0; j <= Math.Min(i, 11); j++)
            {
                var checkMonth = cohortStart.AddMonths(j);
                var checkMonthEnd = checkMonth.AddMonths(1);
                
                var retained = await _context.Subscriptions
                    .CountAsync(s => 
                        cohortCustomerIds.Contains(s.ClinicId) &&
                        s.Status == "Active" &&
                        s.CreatedAt <= checkMonthEnd);
                        
                var retentionRate = (double)retained / cohortCustomerIds.Count * 100;
                cohort.RetentionRates.Add(Math.Round(retentionRate, 2));
            }
            
            cohorts.Add(cohort);
        }
        
        result.Cohorts = cohorts;
        return result;
    }
    
    public async Task<CohortRevenueDto> GetRevenueAnalysis(int months)
    {
        var result = new CohortRevenueDto();
        var cohorts = new List<RevenueCohortDto>();
        
        for (int i = 0; i < months; i++)
        {
            var cohortMonth = DateTime.UtcNow.AddMonths(-i);
            var cohortStart = new DateTime(cohortMonth.Year, cohortMonth.Month, 1);
            var cohortEnd = cohortStart.AddMonths(1);
            
            var cohortCustomerIds = await _context.Clinics
                .Where(c => c.CreatedAt >= cohortStart && c.CreatedAt < cohortEnd)
                .Select(c => c.Id)
                .ToListAsync();
                
            if (cohortCustomerIds.Count == 0)
                continue;
                
            var cohort = new RevenueCohortDto
            {
                Month = cohortStart,
                Size = cohortCustomerIds.Count,
                MrrByMonth = new List<decimal>(),
                ExpansionMrr = new List<decimal>(),
                ContractionMrr = new List<decimal>()
            };
            
            for (int j = 0; j <= Math.Min(i, 11); j++)
            {
                var checkMonth = cohortStart.AddMonths(j);
                var checkMonthEnd = checkMonth.AddMonths(1);
                
                // MRR total da cohort neste mês
                var mrr = await _context.Subscriptions
                    .Where(s => 
                        cohortCustomerIds.Contains(s.ClinicId) &&
                        s.Status == "Active" &&
                        s.CreatedAt <= checkMonthEnd)
                    .SumAsync(s => s.Mrr);
                    
                cohort.MrrByMonth.Add(mrr);
                
                // Expansion (upgrades)
                var expansion = await CalculateExpansionMrr(cohortCustomerIds, checkMonth, checkMonthEnd);
                cohort.ExpansionMrr.Add(expansion);
                
                // Contraction (downgrades)
                var contraction = await CalculateContractionMrr(cohortCustomerIds, checkMonth, checkMonthEnd);
                cohort.ContractionMrr.Add(contraction);
            }
            
            // Calcular LTV da cohort
            cohort.Ltv = cohort.MrrByMonth.Sum() / cohort.Size;
            
            cohorts.Add(cohort);
        }
        
        result.Cohorts = cohorts;
        return result;
    }
}
```

#### 3.2 Frontend - Cohort Visualization

```typescript
// system-admin/src/app/analytics/cohort-analysis/cohort-analysis.component.ts
@Component({
  selector: 'app-cohort-analysis',
  standalone: true,
  template: `
    <div class="cohort-analysis">
      <mat-tab-group>
        <!-- Retention Analysis -->
        <mat-tab label="Retenção">
          <div class="cohort-heatmap">
            <table class="cohort-table">
              <thead>
                <tr>
                  <th>Cohort</th>
                  <th>Tamanho</th>
                  <th *ngFor="let month of monthHeaders">Mês {{ month }}</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let cohort of retentionData.cohorts">
                  <td>{{ cohort.month | date:'MMM yyyy' }}</td>
                  <td>{{ cohort.size }}</td>
                  <td 
                    *ngFor="let rate of cohort.retentionRates"
                    [style.background-color]="getHeatmapColor(rate)"
                    class="retention-cell"
                  >
                    {{ rate }}%
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          
          <!-- Average Retention Chart -->
          <div class="chart-container">
            <h3>Retenção Média por Mês</h3>
            <apx-chart
              [series]="averageRetentionSeries"
              [chart]="{ type: 'line', height: 350 }"
              [xaxis]="{ categories: monthHeaders }"
              [yaxis]="{ max: 100, title: { text: 'Retention %' } }"
            ></apx-chart>
          </div>
        </mat-tab>
        
        <!-- Revenue Analysis -->
        <mat-tab label="Receita">
          <div class="revenue-analysis">
            <div class="cohort-cards">
              <mat-card *ngFor="let cohort of revenueData.cohorts" class="cohort-card">
                <mat-card-header>
                  <mat-card-title>{{ cohort.month | date:'MMM yyyy' }}</mat-card-title>
                  <mat-card-subtitle>{{ cohort.size }} clientes</mat-card-subtitle>
                </mat-card-header>
                <mat-card-content>
                  <div class="metric">
                    <span class="label">LTV:</span>
                    <span class="value">{{ cohort.ltv | currency:'BRL' }}</span>
                  </div>
                  <div class="metric">
                    <span class="label">MRR Inicial:</span>
                    <span class="value">{{ cohort.mrrByMonth[0] | currency:'BRL' }}</span>
                  </div>
                  <div class="metric">
                    <span class="label">MRR Atual:</span>
                    <span class="value">
                      {{ cohort.mrrByMonth[cohort.mrrByMonth.length - 1] | currency:'BRL' }}
                    </span>
                  </div>
                </mat-card-content>
              </mat-card>
            </div>
            
            <!-- MRR Over Time Chart -->
            <div class="chart-container">
              <h3>MRR por Cohort ao Longo do Tempo</h3>
              <apx-chart
                [series]="mrrCohortSeries"
                [chart]="{ type: 'line', height: 400 }"
                [stroke]="{ width: 2 }"
              ></apx-chart>
            </div>
          </div>
        </mat-tab>
        
        <!-- Behavior Analysis -->
        <mat-tab label="Comportamento">
          <div class="behavior-analysis">
            <h3>Features Mais Usadas por Cohort</h3>
            <div class="feature-comparison">
              <!-- Comparação de uso de features entre cohorts -->
            </div>
          </div>
        </mat-tab>
      </mat-tab-group>
    </div>
  `,
  styles: [`
    .cohort-table {
      width: 100%;
      border-collapse: collapse;
    }
    
    .retention-cell {
      text-align: center;
      padding: 8px;
      color: white;
      font-weight: bold;
    }
    
    .cohort-cards {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
      gap: 16px;
      margin-bottom: 32px;
    }
  `]
})
export class CohortAnalysisComponent implements OnInit {
  retentionData: CohortRetention;
  revenueData: CohortRevenue;
  monthHeaders = Array.from({ length: 12 }, (_, i) => i);
  
  async ngOnInit() {
    await Promise.all([
      this.loadRetentionData(),
      this.loadRevenueData()
    ]);
  }
  
  getHeatmapColor(rate: number): string {
    // Verde para alta retenção, vermelho para baixa
    if (rate >= 80) return '#10b981'; // green
    if (rate >= 60) return '#fbbf24'; // yellow
    if (rate >= 40) return '#f59e0b'; // orange
    return '#ef4444'; // red
  }
  
  get averageRetentionSeries() {
    const averages = this.monthHeaders.map(month => {
      const rates = this.retentionData.cohorts
        .filter(c => c.retentionRates.length > month)
        .map(c => c.retentionRates[month]);
      return rates.reduce((a, b) => a + b, 0) / rates.length;
    });
    
    return [{
      name: 'Retenção Média',
      data: averages
    }];
  }
  
  get mrrCohortSeries() {
    return this.revenueData.cohorts.map(cohort => ({
      name: cohort.month.toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' }),
      data: cohort.mrrByMonth
    }));
  }
}
```

---

## ✅ Critérios de Sucesso

### Dashboards
- [ ] Editor drag-and-drop funcional
- [ ] 5+ widgets pré-construídos
- [ ] Queries SQL customizadas (validadas)
- [ ] Auto-refresh configurável
- [ ] Exportação de dashboards (JSON)
- [ ] Compartilhamento de dashboards

### Relatórios
- [ ] 10+ templates de relatórios
- [ ] Wizard intuitivo de geração
- [ ] Exportação PDF com branding
- [ ] Exportação Excel com múltiplas abas
- [ ] Agendamento funcionando
- [ ] Envio por email automático

### Cohort Analysis
- [ ] Visualização de heatmap de retenção
- [ ] Análise de receita por cohort
- [ ] Cálculo correto de LTV
- [ ] Identificação de padrões de churn
- [ ] Comparação entre cohorts

### Performance
- [ ] Dashboards carregam em < 3s
- [ ] Widgets atualizam em < 2s
- [ ] Queries SQL com timeout de 30s
- [ ] Exportação PDF em < 10s

---

## 🧪 Testes e Validação

### Testes Unitários
```csharp
public class CohortAnalysisServiceTests
{
    [Fact]
    public async Task GetRetentionAnalysis_ShouldCalculateCorrectly()
    {
        // Test retention calculation
        var result = await _service.GetRetentionAnalysis(6);
        Assert.NotEmpty(result.Cohorts);
        Assert.All(result.Cohorts, c => Assert.InRange(c.RetentionRates[0], 0, 100));
    }
}
```

---

## 📚 Documentação

- Guia de criação de dashboards
- Catálogo de widgets disponíveis
- SQL query guidelines
- Interpretação de cohort analysis
- Configuração de relatórios agendados

---

## 🔄 Próximos Passos

Após Fase 3:
1. ✅ Validar relatórios com stakeholders
2. ➡️ **Fase 4: Automação e Workflows**

---

## 📞 Referências

- **Metabase:** https://www.metabase.com
- **Forest Admin:** https://www.forestadmin.com
- **Stripe Analytics:** https://stripe.com/docs/analytics

---

**Criado:** Janeiro 2026  
**Versão:** 1.0  
**Status:** Pronto para implementação
