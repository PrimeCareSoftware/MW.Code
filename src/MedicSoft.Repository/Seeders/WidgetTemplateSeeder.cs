using System.Collections.Generic;
using System.Text.Json;
using MedicSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Repository.Seeders
{
    /// <summary>
    /// Seeder for widget templates
    /// Phase 3: Analytics and BI
    /// </summary>
    public static class WidgetTemplateSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var templates = new List<WidgetTemplate>
            {
                // Financial Templates
                new WidgetTemplate
                {
                    Id = 1,
                    Name = "MRR Over Time",
                    Description = "Monthly Recurring Revenue trend over the last 12 months",
                    Category = "financial",
                    Type = "line",
                    IsSystem = true,
                    Icon = "trending_up",
                    DefaultQuery = @"
SELECT 
    DATE_TRUNC('month', cs.""CreatedAt"") as month,
    SUM(p.""MonthlyPrice"") as total_mrr
FROM ""ClinicSubscriptions"" cs
INNER JOIN ""Plans"" p ON cs.""PlanId"" = p.""Id""
WHERE cs.""CreatedAt"" >= CURRENT_DATE - INTERVAL '12 months'
    AND cs.""Status"" = 'Active'
GROUP BY DATE_TRUNC('month', cs.""CreatedAt"")
ORDER BY month",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        xAxis = "month",
                        yAxis = "total_mrr",
                        color = "#10b981",
                        format = "currency"
                    })
                },
                
                new WidgetTemplate
                {
                    Id = 2,
                    Name = "Revenue Breakdown",
                    Description = "MRR distribution by plan type",
                    Category = "financial",
                    Type = "pie",
                    IsSystem = true,
                    Icon = "pie_chart",
                    DefaultQuery = @"
SELECT 
    p.""Name"" as plan,
    SUM(p.""MonthlyPrice"") as revenue
FROM ""ClinicSubscriptions"" cs
INNER JOIN ""Plans"" p ON cs.""PlanId"" = p.""Id""
WHERE cs.""Status"" = 'Active'
GROUP BY p.""Name""",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        labelField = "plan",
                        valueField = "revenue",
                        format = "currency"
                    })
                },

                new WidgetTemplate
                {
                    Id = 3,
                    Name = "Total MRR",
                    Description = "Current Monthly Recurring Revenue",
                    Category = "financial",
                    Type = "metric",
                    IsSystem = true,
                    Icon = "attach_money",
                    DefaultQuery = @"
SELECT SUM(p.""MonthlyPrice"") as value
FROM ""ClinicSubscriptions"" cs
INNER JOIN ""Plans"" p ON cs.""PlanId"" = p.""Id""
WHERE cs.""Status"" = 'Active'",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        format = "currency",
                        icon = "attach_money",
                        color = "#10b981"
                    })
                },
                
                // Customer Templates
                new WidgetTemplate
                {
                    Id = 4,
                    Name = "Active Customers",
                    Description = "Total number of active clinic customers",
                    Category = "customer",
                    Type = "metric",
                    IsSystem = true,
                    Icon = "people",
                    DefaultQuery = @"
SELECT COUNT(DISTINCT ""ClinicId"") as value
FROM ""ClinicSubscriptions""
WHERE ""Status"" = 'Active'",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        format = "number",
                        icon = "people",
                        color = "#3b82f6"
                    })
                },
                
                new WidgetTemplate
                {
                    Id = 5,
                    Name = "Customer Growth",
                    Description = "New customers acquired each month",
                    Category = "customer",
                    Type = "bar",
                    IsSystem = true,
                    Icon = "trending_up",
                    DefaultQuery = @"
SELECT 
    DATE_TRUNC('month', ""CreatedAt"") as month,
    COUNT(DISTINCT ""ClinicId"") as new_customers
FROM ""ClinicSubscriptions""
WHERE ""CreatedAt"" >= CURRENT_DATE - INTERVAL '12 months'
GROUP BY DATE_TRUNC('month', ""CreatedAt"")
ORDER BY month",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        xAxis = "month",
                        yAxis = "new_customers",
                        color = "#3b82f6"
                    })
                },

                new WidgetTemplate
                {
                    Id = 6,
                    Name = "Churn Rate",
                    Description = "Monthly customer churn percentage",
                    Category = "customer",
                    Type = "metric",
                    IsSystem = true,
                    Icon = "warning",
                    DefaultQuery = @"
SELECT 
    ROUND(
        CAST(COUNT(CASE WHEN ""Status"" = 'Cancelled' AND ""EndDate"" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / 
        NULLIF(COUNT(CASE WHEN ""EndDate"" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,
        2
    ) as value
FROM ""ClinicSubscriptions""",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        format = "percent",
                        icon = "warning",
                        color = "#ef4444",
                        threshold = new { warning = 5, critical = 10 }
                    })
                },
                
                // Operational Templates
                new WidgetTemplate
                {
                    Id = 7,
                    Name = "Total Appointments",
                    Description = "Total appointments scheduled",
                    Category = "operational",
                    Type = "metric",
                    IsSystem = true,
                    Icon = "event",
                    DefaultQuery = @"
SELECT COUNT(*) as value
FROM ""Appointments""
WHERE ""AppointmentDate"" >= CURRENT_DATE - INTERVAL '30 days'",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        format = "number",
                        icon = "event",
                        color = "#8b5cf6"
                    })
                },

                new WidgetTemplate
                {
                    Id = 8,
                    Name = "Appointments by Status",
                    Description = "Distribution of appointments by status",
                    Category = "operational",
                    Type = "pie",
                    IsSystem = true,
                    Icon = "pie_chart",
                    DefaultQuery = @"
SELECT 
    ""Status"" as status,
    COUNT(*) as count
FROM ""Appointments""
WHERE ""AppointmentDate"" >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY ""Status""",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        labelField = "status",
                        valueField = "count"
                    })
                },

                new WidgetTemplate
                {
                    Id = 9,
                    Name = "Active Users",
                    Description = "Number of active users in the system",
                    Category = "operational",
                    Type = "metric",
                    IsSystem = true,
                    Icon = "person",
                    DefaultQuery = @"
SELECT COUNT(*) as value
FROM ""Users""
WHERE ""IsActive"" = true",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        format = "number",
                        icon = "person",
                        color = "#06b6d4"
                    })
                },

                // Clinical Templates
                new WidgetTemplate
                {
                    Id = 10,
                    Name = "Total Patients",
                    Description = "Total number of registered patients",
                    Category = "clinical",
                    Type = "metric",
                    IsSystem = true,
                    Icon = "local_hospital",
                    DefaultQuery = @"
SELECT COUNT(*) as value
FROM ""Patients""",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        format = "number",
                        icon = "local_hospital",
                        color = "#f97316"
                    })
                },

                new WidgetTemplate
                {
                    Id = 11,
                    Name = "Patients by Clinic",
                    Description = "Patient distribution across clinics",
                    Category = "clinical",
                    Type = "bar",
                    IsSystem = true,
                    Icon = "bar_chart",
                    DefaultQuery = @"
SELECT 
    c.""TradeName"" as clinic,
    COUNT(p.""Id"") as patient_count
FROM ""Patients"" p
INNER JOIN ""Clinics"" c ON p.""ClinicId"" = c.""Id""
GROUP BY c.""TradeName""
ORDER BY patient_count DESC
LIMIT 10",
                    DefaultConfig = JsonSerializer.Serialize(new
                    {
                        xAxis = "clinic",
                        yAxis = "patient_count",
                        color = "#f97316"
                    })
                }
            };

            modelBuilder.Entity<WidgetTemplate>().HasData(templates);
        }
    }
}
