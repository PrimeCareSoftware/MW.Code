using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.Dashboards
{
    /// <summary>
    /// Service to seed widget templates for Category 4.1
    /// Adds 10+ widget types with ready-to-use templates
    /// </summary>
    public class WidgetTemplateSeedService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<WidgetTemplateSeedService> _logger;

        public WidgetTemplateSeedService(MedicSoftDbContext context, ILogger<WidgetTemplateSeedService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SeedWidgetTemplatesAsync()
        {
            _logger.LogInformation("Starting widget template seeding for Category 4.1");

            var templates = GetWidgetTemplates();

            foreach (var template in templates)
            {
                // Check if template already exists
                var existing = await _context.Set<WidgetTemplate>()
                    .FirstOrDefaultAsync(t => t.Name == template.Name && t.Category == template.Category);

                if (existing == null)
                {
                    _context.Set<WidgetTemplate>().Add(template);
                    _logger.LogInformation("Adding widget template: {Name}", template.Name);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Widget template seeding completed. Added {Count} templates", templates.Count);
        }

        private List<WidgetTemplate> GetWidgetTemplates()
        {
            var templates = new List<WidgetTemplate>();

            // ============ Financial Category ============
            templates.Add(new WidgetTemplate
            {
                Name = "Receita Mensal (Gauge)",
                Description = "Indicador de receita mensal com meta visual em gauge",
                Category = "financial",
                Type = "gauge",
                Icon = "speed",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    min = 0,
                    max = 100000,
                    target = 80000,
                    colors = new[] { "#FF0000", "#FFA500", "#00FF00" }
                }),
                DefaultQuery = @"
SELECT 
    SUM(TotalValue) as value,
    80000 as target
FROM Payments 
WHERE PaymentDate >= DATE_TRUNC('month', CURRENT_DATE)
  AND PaymentDate < DATE_TRUNC('month', CURRENT_DATE) + INTERVAL '1 month'
  AND Status = 'Paid'"
            });

            templates.Add(new WidgetTemplate
            {
                Name = "Fluxo de Caixa (Waterfall)",
                Description = "Visualização de entradas e saídas em cascata",
                Category = "financial",
                Type = "waterfall",
                Icon = "waterfall_chart",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    xField = "category",
                    yField = "amount",
                    colors = new { positive = "#00C851", negative = "#ff4444", total = "#33b5e5" }
                }),
                DefaultQuery = @"
SELECT 'Receitas' as category, SUM(TotalValue) as amount, 'positive' as type
FROM Payments WHERE PaymentDate >= DATE_TRUNC('month', CURRENT_DATE)
UNION ALL
SELECT 'Despesas', SUM(TotalValue) * -1, 'negative'
FROM Expenses WHERE ExpenseDate >= DATE_TRUNC('month', CURRENT_DATE)
UNION ALL
SELECT 'Saldo Final', 
  (SELECT SUM(TotalValue) FROM Payments WHERE PaymentDate >= DATE_TRUNC('month', CURRENT_DATE)) - 
  (SELECT SUM(TotalValue) FROM Expenses WHERE ExpenseDate >= DATE_TRUNC('month', CURRENT_DATE)), 
  'total'"
            });

            templates.Add(new WidgetTemplate
            {
                Name = "Receita por Categoria (Funnel)",
                Description = "Funil de conversão de receitas por categoria",
                Category = "financial",
                Type = "funnel",
                Icon = "filter_alt",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    labelField = "category",
                    valueField = "amount",
                    colors = new[] { "#4285f4", "#34a853", "#fbbc05", "#ea4335" }
                }),
                DefaultQuery = @"
SELECT 
    p.ProcedureName as category,
    SUM(py.TotalValue) as amount
FROM Payments py
JOIN Procedures p ON py.ProcedureId = p.Id
WHERE py.PaymentDate >= DATE_TRUNC('month', CURRENT_DATE)
GROUP BY p.ProcedureName
ORDER BY amount DESC
LIMIT 5"
            });

            // ============ Operational Category ============
            templates.Add(new WidgetTemplate
            {
                Name = "Agendamentos por Hora (Heatmap)",
                Description = "Mapa de calor de agendamentos por hora/dia",
                Category = "operational",
                Type = "heatmap",
                Icon = "grid_on",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    xField = "hour",
                    yField = "weekday",
                    valueField = "count",
                    colorRange = new[] { "#f7fbff", "#08519c" }
                }),
                DefaultQuery = @"
SELECT 
    EXTRACT(HOUR FROM AppointmentTime) as hour,
    TO_CHAR(AppointmentDate, 'Day') as weekday,
    COUNT(*) as count
FROM Appointments
WHERE AppointmentDate >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY hour, weekday
ORDER BY hour, weekday"
            });

            templates.Add(new WidgetTemplate
            {
                Name = "Taxa de Ocupação (Radar)",
                Description = "Gráfico radar de ocupação por especialidade",
                Category = "operational",
                Type = "radar",
                Icon = "radar",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    categoryField = "specialty",
                    valueField = "occupancy",
                    maxValue = 100
                }),
                DefaultQuery = @"
SELECT 
    u.Specialty as specialty,
    (COUNT(a.Id) * 100.0 / (COUNT(DISTINCT a.DoctorId) * 8 * 22)) as occupancy
FROM Users u
LEFT JOIN Appointments a ON a.DoctorId = u.Id 
    AND a.AppointmentDate >= DATE_TRUNC('month', CURRENT_DATE)
WHERE u.Role = 'Doctor'
GROUP BY u.Specialty
HAVING COUNT(DISTINCT a.DoctorId) > 0"
            });

            templates.Add(new WidgetTemplate
            {
                Name = "Distribuição de Pacientes (Treemap)",
                Description = "Treemap de pacientes por faixa etária e gênero",
                Category = "operational",
                Type = "treemap",
                Icon = "account_tree",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    labelField = "category",
                    valueField = "count",
                    colorField = "group"
                }),
                DefaultQuery = @"
SELECT 
    CASE 
        WHEN EXTRACT(YEAR FROM AGE(BirthDate)) < 18 THEN '0-17 anos'
        WHEN EXTRACT(YEAR FROM AGE(BirthDate)) < 30 THEN '18-29 anos'
        WHEN EXTRACT(YEAR FROM AGE(BirthDate)) < 50 THEN '30-49 anos'
        WHEN EXTRACT(YEAR FROM AGE(BirthDate)) < 65 THEN '50-64 anos'
        ELSE '65+ anos'
    END as category,
    Gender as group,
    COUNT(*) as count
FROM Patients
WHERE IsActive = true
GROUP BY category, group
ORDER BY category, group"
            });

            templates.Add(new WidgetTemplate
            {
                Name = "Calendário de Agendamentos",
                Description = "Visualização em calendário dos agendamentos",
                Category = "operational",
                Type = "calendar",
                Icon = "calendar_month",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    dateField = "date",
                    valueField = "count",
                    colorRange = new[] { "#e8f5e9", "#1b5e20" }
                }),
                DefaultQuery = @"
SELECT 
    AppointmentDate::date as date,
    COUNT(*) as count
FROM Appointments
WHERE AppointmentDate >= CURRENT_DATE - INTERVAL '90 days'
  AND AppointmentDate < CURRENT_DATE + INTERVAL '30 days'
GROUP BY date
ORDER BY date"
            });

            // ============ Customer Category ============
            templates.Add(new WidgetTemplate
            {
                Name = "Satisfação do Paciente (Gauge)",
                Description = "Indicador de satisfação média dos pacientes",
                Category = "customer",
                Type = "gauge",
                Icon = "sentiment_satisfied",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    min = 0,
                    max = 10,
                    target = 8,
                    colors = new[] { "#FF0000", "#FFA500", "#00FF00" }
                }),
                DefaultQuery = @"
SELECT 
    AVG(Rating) as value,
    8.0 as target
FROM Tickets
WHERE Status = 'Closed' 
  AND Rating IS NOT NULL
  AND CreatedAt >= DATE_TRUNC('month', CURRENT_DATE)"
            });

            templates.Add(new WidgetTemplate
            {
                Name = "Distribuição Geográfica (Scatter)",
                Description = "Gráfico de dispersão de pacientes por região",
                Category = "customer",
                Type = "scatter",
                Icon = "scatter_plot",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    xField = "latitude",
                    yField = "longitude",
                    sizeField = "count",
                    colorField = "city"
                }),
                DefaultQuery = @"
SELECT 
    City as city,
    AVG(CAST(SUBSTRING(AddressLatitude, 1, 10) AS FLOAT)) as latitude,
    AVG(CAST(SUBSTRING(AddressLongitude, 1, 10) AS FLOAT)) as longitude,
    COUNT(*) as count
FROM Patients
WHERE AddressLatitude IS NOT NULL 
  AND AddressLongitude IS NOT NULL
  AND IsActive = true
GROUP BY City
HAVING COUNT(*) > 5"
            });

            templates.Add(new WidgetTemplate
            {
                Name = "Crescimento de Base (Area)",
                Description = "Gráfico de área do crescimento da base de pacientes",
                Category = "customer",
                Type = "area",
                Icon = "show_chart",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    xField = "month",
                    yField = "total",
                    smooth = true,
                    fillOpacity = 0.5
                }),
                DefaultQuery = @"
SELECT 
    TO_CHAR(CreatedAt, 'YYYY-MM') as month,
    COUNT(*) as total
FROM Patients
WHERE CreatedAt >= CURRENT_DATE - INTERVAL '12 months'
  AND IsActive = true
GROUP BY month
ORDER BY month"
            });

            // ============ Clinical Category ============
            templates.Add(new WidgetTemplate
            {
                Name = "Diagnósticos Mais Comuns (Donut)",
                Description = "Gráfico de rosquinha dos diagnósticos mais frequentes",
                Category = "clinical",
                Type = "donut",
                Icon = "donut_large",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    labelField = "diagnosis",
                    valueField = "count",
                    innerRadius = 0.6
                }),
                DefaultQuery = @"
SELECT 
    DiagnosisDescription as diagnosis,
    COUNT(*) as count
FROM DiagnosticHypothesis
WHERE CreatedAt >= DATE_TRUNC('month', CURRENT_DATE) - INTERVAL '3 months'
  AND IsActive = true
GROUP BY DiagnosisDescription
ORDER BY count DESC
LIMIT 10"
            });

            templates.Add(new WidgetTemplate
            {
                Name = "Tempo Médio de Atendimento",
                Description = "Comparação de tempo médio de atendimento por médico",
                Category = "clinical",
                Type = "bar",
                Icon = "timer",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    xField = "doctor",
                    yField = "avgTime",
                    color = "#4285f4"
                }),
                DefaultQuery = @"
SELECT 
    u.Username as doctor,
    AVG(EXTRACT(EPOCH FROM (a.EndTime - a.StartTime)) / 60) as avgTime
FROM Appointments a
JOIN Users u ON a.DoctorId = u.Id
WHERE a.Status = 'Completed'
  AND a.AppointmentDate >= DATE_TRUNC('month', CURRENT_DATE)
  AND a.EndTime IS NOT NULL
GROUP BY u.Username
ORDER BY avgTime DESC
LIMIT 10"
            });

            // ============ Performance Metrics ============
            templates.Add(new WidgetTemplate
            {
                Name = "Taxa de Conversão (Funnel)",
                Description = "Funil de conversão do lead ao paciente ativo",
                Category = "operational",
                Type = "funnel",
                Icon = "analytics",
                IsSystem = true,
                DefaultConfig = JsonSerializer.Serialize(new
                {
                    labelField = "stage",
                    valueField = "count"
                }),
                DefaultQuery = @"
SELECT 'Leads' as stage, COUNT(*) as count FROM Patients WHERE Status = 'Lead'
UNION ALL
SELECT 'Contatos', COUNT(*) FROM Patients WHERE Status = 'Contacted'
UNION ALL
SELECT 'Agendados', COUNT(*) FROM Patients WHERE Status = 'Scheduled'
UNION ALL
SELECT 'Ativos', COUNT(*) FROM Patients WHERE Status = 'Active'"
            });

            return templates;
        }
    }
}
