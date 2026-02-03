using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddEnableOnlineAppointmentSchedulingToClinic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("149f701d-082c-411d-9f40-a53e4779c62f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("23af1665-ed46-45e3-8204-76cd202b26ed"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("48e71dd6-b60a-4e71-a128-457f2b22b3a5"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4c4a0751-bb00-4b44-a536-666f8d553e1f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4d722672-4f06-4e64-b456-ce21ee355e7d"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("53b6aaf3-8e9a-45ec-a342-e415bafc8c09"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("711ee80a-9adb-475e-abef-4aee7f832248"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("b656ff29-f5cc-4206-b899-f22aca065574"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("cb03a04f-0b83-474a-91c0-e77216e78916"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e2438c44-53e7-4127-bf1f-082c6b84b33e"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("0818dfb6-acbf-4c91-8211-c3c8326afc95"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("14dfda39-5832-40cf-9e66-20f1f532a4b9"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("2daad020-4b76-4c47-9700-8b0e9e72935a"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("2f4c128b-9272-421e-8223-e233bdb6c364"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("32d39125-f361-4325-8767-0fb6089ce06b"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4148676e-a443-4cf9-b379-dc0eb05399c9"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4ddd6559-a2f1-4667-bbfe-bda8dd5fb1ad"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7cdf211f-4378-4c94-bf9f-9d8018792564"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7de35751-c1b3-4ee6-add4-820952f4a459"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("8e5dc434-76da-4ea8-b090-f02ed5b0dddd"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f3d62217-d2b8-42bd-af4c-ebd14c473842"));

            migrationBuilder.AddColumn<bool>(
                name: "EnableOnlineAppointmentScheduling",
                table: "Clinics",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1381e125-5602-4c68-a909-3609f0895538"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6479), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("1cbd4e13-139d-4a83-bcd0-c2578f1f4d86"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6759), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("1f7d5bf9-eb6c-4cda-a2d7-d30a2e2bb72d"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6432), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("34e823b6-2c7f-45d5-92f2-9eb0dd77575d"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6659), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("3880d839-675a-4eb9-8176-13b4b1763d36"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6589), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("48184733-c39c-4238-bb4a-1a50c04af279"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6694), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"BirthDate\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR p.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("4de9595d-2563-4469-a01d-5ae39254e070"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6800), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("9bb89cc4-0fb1-427c-95c9-9d20c1184836"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6090), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("afdfb4cf-7eb1-4c9b-9b64-0e644df25c86"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6519), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("ceb5511c-3297-4646-ab74-3fb7cb23848e"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(6725), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(p.\"Id\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"Patients\" p ON c.\"Id\" = p.\"ClinicId\"\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0d0c4c51-0b68-40a2-8eef-77d1d42ea14a"), "financial", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5372), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("175e8a20-319a-44ca-87dd-3619d9e97821"), "customer", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5552), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("483f51bc-7522-4bb6-ae9f-a284e02ee0db"), "customer", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5470), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("4ce5035a-3166-4693-b24e-b2cc65ef8b3e"), "operational", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5767), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("7c5d47e3-fca7-4821-8b19-e0136ab465ec"), "customer", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5428), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("7fb836e8-a79a-4c58-b309-7639e5d01fb2"), "clinical", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5912), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("7fd65f06-b54f-4ef8-ab0c-f26519b654bc"), "operational", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5724), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("ddbceacb-fc14-45bd-85a1-e634cf3ed80f"), "financial", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5321), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("e1d2f3f5-5f6f-4c46-82bf-90c402b5b9a0"), "financial", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(4915), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("e3f9203d-580d-47af-87cd-045e10ce1bc2"), "clinical", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5831), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("ec853190-6968-4ce9-b9cc-761395c3e52f"), "operational", new DateTime(2026, 2, 3, 18, 31, 59, 264, DateTimeKind.Utc).AddTicks(5680), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1381e125-5602-4c68-a909-3609f0895538"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1cbd4e13-139d-4a83-bcd0-c2578f1f4d86"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1f7d5bf9-eb6c-4cda-a2d7-d30a2e2bb72d"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("34e823b6-2c7f-45d5-92f2-9eb0dd77575d"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("3880d839-675a-4eb9-8176-13b4b1763d36"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("48184733-c39c-4238-bb4a-1a50c04af279"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4de9595d-2563-4469-a01d-5ae39254e070"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9bb89cc4-0fb1-427c-95c9-9d20c1184836"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("afdfb4cf-7eb1-4c9b-9b64-0e644df25c86"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ceb5511c-3297-4646-ab74-3fb7cb23848e"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("0d0c4c51-0b68-40a2-8eef-77d1d42ea14a"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("175e8a20-319a-44ca-87dd-3619d9e97821"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("483f51bc-7522-4bb6-ae9f-a284e02ee0db"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4ce5035a-3166-4693-b24e-b2cc65ef8b3e"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7c5d47e3-fca7-4821-8b19-e0136ab465ec"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7fb836e8-a79a-4c58-b309-7639e5d01fb2"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7fd65f06-b54f-4ef8-ab0c-f26519b654bc"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ddbceacb-fc14-45bd-85a1-e634cf3ed80f"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e1d2f3f5-5f6f-4c46-82bf-90c402b5b9a0"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e3f9203d-580d-47af-87cd-045e10ce1bc2"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ec853190-6968-4ce9-b9cc-761395c3e52f"));

            migrationBuilder.DropColumn(
                name: "EnableOnlineAppointmentScheduling",
                table: "Clinics");

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("149f701d-082c-411d-9f40-a53e4779c62f"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(1128), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("23af1665-ed46-45e3-8204-76cd202b26ed"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(1281), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("48e71dd6-b60a-4e71-a128-457f2b22b3a5"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(955), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("4c4a0751-bb00-4b44-a536-666f8d553e1f"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(581), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("4d722672-4f06-4e64-b456-ce21ee355e7d"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(906), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("53b6aaf3-8e9a-45ec-a342-e415bafc8c09"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(1234), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("711ee80a-9adb-475e-abef-4aee7f832248"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(998), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("b656ff29-f5cc-4206-b899-f22aca065574"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(1077), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("cb03a04f-0b83-474a-91c0-e77216e78916"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(1198), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(p.\"Id\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"Patients\" p ON c.\"Id\" = p.\"ClinicId\"\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("e2438c44-53e7-4127-bf1f-082c6b84b33e"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(1166), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"BirthDate\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR p.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0818dfb6-acbf-4c91-8211-c3c8326afc95"), "operational", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(221), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("14dfda39-5832-40cf-9e66-20f1f532a4b9"), "financial", new DateTime(2026, 2, 3, 18, 4, 48, 606, DateTimeKind.Utc).AddTicks(9261), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("2daad020-4b76-4c47-9700-8b0e9e72935a"), "operational", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(114), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("2f4c128b-9272-421e-8223-e233bdb6c364"), "financial", new DateTime(2026, 2, 3, 18, 4, 48, 606, DateTimeKind.Utc).AddTicks(9785), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("32d39125-f361-4325-8767-0fb6089ce06b"), "operational", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(173), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("4148676e-a443-4cf9-b379-dc0eb05399c9"), "clinical", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(301), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("4ddd6559-a2f1-4667-bbfe-bda8dd5fb1ad"), "financial", new DateTime(2026, 2, 3, 18, 4, 48, 606, DateTimeKind.Utc).AddTicks(9723), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("7cdf211f-4378-4c94-bf9f-9d8018792564"), "customer", new DateTime(2026, 2, 3, 18, 4, 48, 606, DateTimeKind.Utc).AddTicks(9903), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("7de35751-c1b3-4ee6-add4-820952f4a459"), "customer", new DateTime(2026, 2, 3, 18, 4, 48, 606, DateTimeKind.Utc).AddTicks(9872), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("8e5dc434-76da-4ea8-b090-f02ed5b0dddd"), "clinical", new DateTime(2026, 2, 3, 18, 4, 48, 607, DateTimeKind.Utc).AddTicks(398), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("f3d62217-d2b8-42bd-af4c-ebd14c473842"), "customer", new DateTime(2026, 2, 3, 18, 4, 48, 606, DateTimeKind.Utc).AddTicks(9972), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null }
                });
        }
    }
}
