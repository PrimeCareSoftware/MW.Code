using System;
using System.Collections.Generic;
using System.Text.Json;
using MedicSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Repository.Seeders
{
    /// <summary>
    /// Seeder for report templates
    /// Phase 3: Analytics and BI
    /// </summary>
    public static class ReportTemplateSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var templates = new List<ReportTemplate>
            {
                // Financial Reports
                new ReportTemplate
                {
                    Id = 1,
                    Name = "Financial Summary Report",
                    Description = "Comprehensive financial performance report including MRR, revenue, and growth metrics",
                    Category = "financial",
                    IsSystem = true,
                    Icon = "assessment",
                    SupportedFormats = "pdf,excel,csv",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "startDate", type = "date", required = true, label = "Start Date" },
                            new { name = "endDate", type = "date", required = true, label = "End Date" }
                        },
                        sections = new[]
                        {
                            new { title = "Revenue Overview", type = "summary" },
                            new { title = "MRR Trend", type = "chart", chartType = "line" },
                            new { title = "Revenue by Plan", type = "chart", chartType = "pie" },
                            new { title = "Top Customers", type = "table" }
                        }
                    }),
                    Query = @"
SELECT 
    DATE_TRUNC('month', cs.""CreatedAt"") as month,
    COUNT(DISTINCT cs.""ClinicId"") as customer_count,
    SUM(p.""MonthlyPrice"") as mrr,
    SUM(p.""MonthlyPrice"" * 12) as arr
FROM ""ClinicSubscriptions"" cs
INNER JOIN ""SubscriptionPlans"" p ON cs.""SubscriptionPlanId"" = p.""Id""
WHERE cs.""CreatedAt"" >= @startDate AND cs.""CreatedAt"" <= @endDate
    AND cs.""Status"" = 'Active'
GROUP BY DATE_TRUNC('month', cs.""CreatedAt"")
ORDER BY month"
                },

                new ReportTemplate
                {
                    Id = 2,
                    Name = "Revenue Breakdown Report",
                    Description = "Detailed breakdown of revenue by plans, clinics, and payment methods",
                    Category = "financial",
                    IsSystem = true,
                    Icon = "pie_chart",
                    SupportedFormats = "pdf,excel",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "month", type = "month", required = true, label = "Report Month" }
                        }
                    }),
                    Query = @"
SELECT 
    p.""Name"" as plan_name,
    COUNT(cs.""Id"") as subscription_count,
    SUM(p.""MonthlyPrice"") as total_mrr,
    AVG(p.""MonthlyPrice"") as avg_price
FROM ""ClinicSubscriptions"" cs
INNER JOIN ""SubscriptionPlans"" p ON cs.""SubscriptionPlanId"" = p.""Id""
WHERE DATE_TRUNC('month', cs.""CreatedAt"") = DATE_TRUNC('month', @month::date)
    AND cs.""Status"" = 'Active'
GROUP BY p.""Name""
ORDER BY total_mrr DESC"
                },

                // Customer Reports
                new ReportTemplate
                {
                    Id = 3,
                    Name = "Customer Acquisition Report",
                    Description = "Analysis of new customer acquisition trends and conversion metrics",
                    Category = "customer",
                    IsSystem = true,
                    Icon = "person_add",
                    SupportedFormats = "pdf,excel,csv",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "startDate", type = "date", required = true, label = "Start Date" },
                            new { name = "endDate", type = "date", required = true, label = "End Date" }
                        }
                    }),
                    Query = @"
SELECT 
    DATE_TRUNC('month', c.""CreatedAt"") as month,
    COUNT(c.""Id"") as new_customers,
    COUNT(DISTINCT u.""Id"") as new_users,
    COUNT(cs.""Id"") as new_subscriptions
FROM ""Clinics"" c
LEFT JOIN ""Users"" u ON c.""Id"" = u.""ClinicId"" AND u.""CreatedAt"" >= @startDate AND u.""CreatedAt"" <= @endDate
LEFT JOIN ""ClinicSubscriptions"" cs ON c.""Id"" = cs.""ClinicId"" AND cs.""CreatedAt"" >= @startDate AND cs.""CreatedAt"" <= @endDate
WHERE c.""CreatedAt"" >= @startDate AND c.""CreatedAt"" <= @endDate
GROUP BY DATE_TRUNC('month', c.""CreatedAt"")
ORDER BY month"
                },

                new ReportTemplate
                {
                    Id = 4,
                    Name = "Customer Churn Report",
                    Description = "Comprehensive churn analysis with reasons and trends",
                    Category = "customer",
                    IsSystem = true,
                    Icon = "exit_to_app",
                    SupportedFormats = "pdf,excel",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "startDate", type = "date", required = true, label = "Start Date" },
                            new { name = "endDate", type = "date", required = true, label = "End Date" }
                        }
                    }),
                    Query = @"
SELECT 
    DATE_TRUNC('month', cs.""EndDate"") as month,
    COUNT(cs.""Id"") as churned_subscriptions,
    SUM(p.""MonthlyPrice"") as lost_mrr,
    c.""Name"" as clinic_name,
    cs.""Status"" as status
FROM ""ClinicSubscriptions"" cs
INNER JOIN ""SubscriptionPlans"" p ON cs.""SubscriptionPlanId"" = p.""Id""
INNER JOIN ""Clinics"" c ON cs.""ClinicId"" = c.""Id""
WHERE cs.""EndDate"" >= @startDate AND cs.""EndDate"" <= @endDate
    AND cs.""Status"" = 'Cancelled'
GROUP BY DATE_TRUNC('month', cs.""EndDate""), c.""Name"", cs.""Status""
ORDER BY month DESC"
                },

                // Operational Reports
                new ReportTemplate
                {
                    Id = 5,
                    Name = "Appointment Analytics Report",
                    Description = "Detailed analysis of appointment scheduling, cancellations, and no-shows",
                    Category = "operational",
                    IsSystem = true,
                    Icon = "event_note",
                    SupportedFormats = "pdf,excel",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "startDate", type = "date", required = true, label = "Start Date" },
                            new { name = "endDate", type = "date", required = true, label = "End Date" }
                        }
                    }),
                    Query = @"
SELECT 
    c.""TradeName"" as clinic,
    COUNT(a.""Id"") as total_appointments,
    COUNT(CASE WHEN a.""Status"" = 'Completed' THEN 1 END) as completed,
    COUNT(CASE WHEN a.""Status"" = 'Cancelled' THEN 1 END) as cancelled,
    COUNT(CASE WHEN a.""Status"" = 'NoShow' THEN 1 END) as no_shows
FROM ""Appointments"" a
INNER JOIN ""Clinics"" c ON a.""ClinicId"" = c.""Id""
WHERE a.""AppointmentDate"" >= @startDate AND a.""AppointmentDate"" <= @endDate
GROUP BY c.""TradeName""
ORDER BY total_appointments DESC"
                },

                new ReportTemplate
                {
                    Id = 6,
                    Name = "User Activity Report",
                    Description = "Overview of user activity, logins, and engagement metrics",
                    Category = "operational",
                    IsSystem = true,
                    Icon = "analytics",
                    SupportedFormats = "pdf,excel,csv",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "startDate", type = "date", required = true, label = "Start Date" },
                            new { name = "endDate", type = "date", required = true, label = "End Date" }
                        }
                    }),
                    Query = @"
SELECT 
    u.""UserName"" as username,
    u.""Email"" as email,
    c.""TradeName"" as clinic,
    COUNT(us.""Id"") as login_count,
    MAX(us.""LastActivityAt"") as last_activity
FROM ""Users"" u
LEFT JOIN ""UserSessions"" us ON u.""Id"" = us.""UserId"" 
    AND us.""CreatedAt"" >= @startDate AND us.""CreatedAt"" <= @endDate
LEFT JOIN ""Clinics"" c ON u.""ClinicId"" = c.""Id""
WHERE u.""IsActive"" = true
GROUP BY u.""UserName"", u.""Email"", c.""TradeName""
ORDER BY login_count DESC"
                },

                // Clinical Reports
                new ReportTemplate
                {
                    Id = 7,
                    Name = "Patient Demographics Report",
                    Description = "Statistical analysis of patient demographics and distribution",
                    Category = "clinical",
                    IsSystem = true,
                    Icon = "people",
                    SupportedFormats = "pdf,excel",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "clinicId", type = "guid", required = false, label = "Clinic (Optional)" }
                        }
                    }),
                    Query = @"
SELECT 
    c.""TradeName"" as clinic,
    COUNT(p.""Id"") as total_patients,
    COUNT(CASE WHEN p.""Gender"" = 'Male' THEN 1 END) as male_count,
    COUNT(CASE WHEN p.""Gender"" = 'Female' THEN 1 END) as female_count,
    AVG(EXTRACT(YEAR FROM AGE(p.""BirthDate""))) as average_age
FROM ""Patients"" p
INNER JOIN ""Clinics"" c ON p.""ClinicId"" = c.""Id""
WHERE (@clinicId IS NULL OR p.""ClinicId"" = @clinicId)
GROUP BY c.""TradeName""
ORDER BY total_patients DESC"
                },

                // Compliance Reports
                new ReportTemplate
                {
                    Id = 8,
                    Name = "System Health Report",
                    Description = "Overview of system health, errors, and performance metrics",
                    Category = "operational",
                    IsSystem = true,
                    Icon = "health_and_safety",
                    SupportedFormats = "pdf,excel",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "startDate", type = "date", required = true, label = "Start Date" },
                            new { name = "endDate", type = "date", required = true, label = "End Date" }
                        }
                    }),
                    Query = @"
SELECT 
    c.""TradeName"" as clinic,
    COUNT(cs.""Id"") as active_subscriptions,
    COUNT(u.""Id"") as active_users,
    COUNT(a.""Id"") as total_appointments,
    COUNT(p.""Id"") as total_patients
FROM ""Clinics"" c
LEFT JOIN ""ClinicSubscriptions"" cs ON c.""Id"" = cs.""ClinicId"" AND cs.""Status"" = 'Active'
LEFT JOIN ""Users"" u ON c.""Id"" = u.""ClinicId"" AND u.""IsActive"" = true
LEFT JOIN ""Appointments"" a ON c.""Id"" = a.""ClinicId"" AND a.""AppointmentDate"" >= @startDate AND a.""AppointmentDate"" <= @endDate
LEFT JOIN ""Patients"" p ON c.""Id"" = p.""ClinicId""
WHERE c.""IsActive"" = true
GROUP BY c.""TradeName""
ORDER BY active_subscriptions DESC"
                },

                new ReportTemplate
                {
                    Id = 9,
                    Name = "Subscription Lifecycle Report",
                    Description = "Analysis of subscription lifecycle from acquisition to churn",
                    Category = "financial",
                    IsSystem = true,
                    Icon = "loop",
                    SupportedFormats = "pdf,excel",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "startDate", type = "date", required = true, label = "Start Date" },
                            new { name = "endDate", type = "date", required = true, label = "End Date" }
                        }
                    }),
                    Query = @"
SELECT 
    c.""TradeName"" as clinic,
    p.""Name"" as plan,
    cs.""StartDate"" as subscription_start,
    cs.""EndDate"" as subscription_end,
    cs.""Status"" as status,
    p.""MonthlyPrice"" as monthly_price,
    EXTRACT(MONTH FROM AGE(COALESCE(cs.""EndDate"", CURRENT_DATE), cs.""StartDate"")) as months_active
FROM ""ClinicSubscriptions"" cs
INNER JOIN ""Clinics"" c ON cs.""ClinicId"" = c.""Id""
INNER JOIN ""SubscriptionPlans"" p ON cs.""SubscriptionPlanId"" = p.""Id""
WHERE cs.""StartDate"" >= @startDate AND cs.""StartDate"" <= @endDate
ORDER BY cs.""StartDate"" DESC"
                },

                new ReportTemplate
                {
                    Id = 10,
                    Name = "Executive Dashboard Report",
                    Description = "High-level executive summary with key metrics and trends",
                    Category = "financial",
                    IsSystem = true,
                    Icon = "dashboard",
                    SupportedFormats = "pdf",
                    CreatedAt = DateTime.UtcNow,
                    Configuration = JsonSerializer.Serialize(new
                    {
                        parameters = new[]
                        {
                            new { name = "month", type = "month", required = true, label = "Report Month" }
                        },
                        sections = new[]
                        {
                            new { title = "Financial KPIs", type = "metrics" },
                            new { title = "Customer Metrics", type = "metrics" },
                            new { title = "Growth Trends", type = "chart" },
                            new { title = "Top Performers", type = "table" }
                        }
                    }),
                    Query = @"
WITH monthly_stats AS (
    SELECT 
        COUNT(DISTINCT cs.""ClinicId"") as total_customers,
        SUM(p.""MonthlyPrice"") as total_mrr,
        COUNT(CASE WHEN cs.""Status"" = 'Cancelled' THEN 1 END) as churned_customers
    FROM ""ClinicSubscriptions"" cs
    INNER JOIN ""SubscriptionPlans"" p ON cs.""SubscriptionPlanId"" = p.""Id""
    WHERE DATE_TRUNC('month', cs.""CreatedAt"") <= DATE_TRUNC('month', @month::date)
)
SELECT * FROM monthly_stats"
                }
            };

            modelBuilder.Entity<ReportTemplate>().HasData(templates);
        }
    }
}
