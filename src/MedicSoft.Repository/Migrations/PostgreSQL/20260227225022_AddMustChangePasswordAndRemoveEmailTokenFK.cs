using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddMustChangePasswordAndRemoveEmailTokenFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationTokens_Users_UserId",
                table: "EmailVerificationTokens");

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("56eb275a-b0b3-4c38-9ffb-d5a65baf2377"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("5bd4b914-b717-4984-a88d-27767cbf0b3f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("5ed45cd7-539c-4213-a70e-b7f66e00d2dc"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("610aed06-0bc2-4fd6-b64d-42da36da2efa"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6bd1d346-91ff-49a0-93bc-cef039bf2dee"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("799a2f18-7713-4c13-b80f-6d0aef30c579"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("830fa650-1cf1-48b0-a865-016a0148730a"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ddc4544d-22f8-435e-a4b9-f3f51d0ed7dc"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ee1935c4-efc7-436c-99ca-6300157c21b2"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f02a7edc-c98b-47cc-9938-1d7755a7830f"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("5684732e-1bae-49f7-a91d-b4d8be19f0e2"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("57671cd0-8d01-4ba1-865d-32e6f2e8f89b"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("71039eb7-0cf5-44bf-a205-116294f044f9"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("87e398fb-4e0f-4d64-b86b-23820a2ab403"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("93ff1da7-5b48-432e-b0e9-1b94354a9fd9"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("b629d3c8-52f5-4446-a752-357778780590"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("bd2eb4fd-fcf3-4f67-a3a3-d2ca0592f6b3"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("bf162aca-ede8-442a-8be1-3d559da286c9"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d76e98cd-8e35-400d-9e9d-3fb05061b563"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ec4d62c2-2d66-45d8-8f47-b54ce9556285"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f614cd80-f6f4-452d-921e-9bb3b2a77e8d"));

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Owners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("12ec77b1-f3b8-46ff-9dbf-9a7050292d59"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(7941), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(DISTINCT pcl.\"PatientId\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"PatientClinicLinks\" pcl ON c.\"Id\" = pcl.\"ClinicId\" AND pcl.\"IsActive\" = true\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("1f616b91-a3c8-49b4-853f-756ce6f8b95f"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6921), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("34968057-dd06-4e32-8a8b-a1ffbfc3ac04"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(8100), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("43193b91-7c4b-456e-8e5d-a8834e8b7f95"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(7860), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"DateOfBirth\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"PatientClinicLinks\" pcl ON p.\"Id\" = pcl.\"PatientId\" AND pcl.\"IsActive\" = true\nINNER JOIN \"Clinics\" c ON pcl.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR pcl.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("6ff63cda-efe8-4bad-8ac1-79dabafed6d4"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(7389), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("9578934c-46a7-4dc9-bef9-2b535566ad6f"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(8017), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("ba3e3115-0092-427b-a54c-8fa1a5788ce6"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(7481), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("ec8d247b-5f06-41f2-af82-75f73b152ee3"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(7304), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("f5fcd172-1e53-48fd-8d88-fd5e025efb1f"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(7783), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("ff559f0f-28f8-47fb-a8c2-b040f97df0ff"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(7681), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1414a845-5e13-4e4f-acb2-553259ddba0a"), "financial", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(5882), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("60cecd53-06b7-4c02-b6dc-c0a4a156af18"), "operational", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6373), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("6250abfd-0ceb-496f-9031-bd843900d88d"), "operational", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6515), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("70418ae5-13e5-43ff-bc56-4840444e42d0"), "clinical", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6729), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("758493d7-63bc-467a-8879-14101c0035f6"), "clinical", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6598), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("7dd2fb1c-da05-4a0e-a503-c01e2caa5019"), "customer", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6186), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("9b208193-f594-4a3e-846c-49a4658131ce"), "customer", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6084), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("9b48fb95-0d42-4f91-8937-1cdfaa5328f9"), "financial", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(4880), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("d14e2968-2e4d-459f-b45c-277a987b528e"), "operational", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6445), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("d1fb39dd-9b63-4cbc-a7d8-1b708db16e88"), "financial", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(5734), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("d8a1b5f3-1fda-4890-8b22-18d96173f724"), "customer", new DateTime(2026, 2, 27, 22, 50, 20, 126, DateTimeKind.Utc).AddTicks(6019), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("12ec77b1-f3b8-46ff-9dbf-9a7050292d59"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1f616b91-a3c8-49b4-853f-756ce6f8b95f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("34968057-dd06-4e32-8a8b-a1ffbfc3ac04"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("43193b91-7c4b-456e-8e5d-a8834e8b7f95"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6ff63cda-efe8-4bad-8ac1-79dabafed6d4"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9578934c-46a7-4dc9-bef9-2b535566ad6f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ba3e3115-0092-427b-a54c-8fa1a5788ce6"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ec8d247b-5f06-41f2-af82-75f73b152ee3"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f5fcd172-1e53-48fd-8d88-fd5e025efb1f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ff559f0f-28f8-47fb-a8c2-b040f97df0ff"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1414a845-5e13-4e4f-acb2-553259ddba0a"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("60cecd53-06b7-4c02-b6dc-c0a4a156af18"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6250abfd-0ceb-496f-9031-bd843900d88d"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("70418ae5-13e5-43ff-bc56-4840444e42d0"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("758493d7-63bc-467a-8879-14101c0035f6"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7dd2fb1c-da05-4a0e-a503-c01e2caa5019"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9b208193-f594-4a3e-846c-49a4658131ce"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9b48fb95-0d42-4f91-8937-1cdfaa5328f9"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d14e2968-2e4d-459f-b45c-277a987b528e"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d1fb39dd-9b63-4cbc-a7d8-1b708db16e88"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d8a1b5f3-1fda-4890-8b22-18d96173f724"));

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Owners");

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("56eb275a-b0b3-4c38-9ffb-d5a65baf2377"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(1953), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("5bd4b914-b717-4984-a88d-27767cbf0b3f"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(1067), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("5ed45cd7-539c-4213-a70e-b7f66e00d2dc"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(2030), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"DateOfBirth\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"PatientClinicLinks\" pcl ON p.\"Id\" = pcl.\"PatientId\" AND pcl.\"IsActive\" = true\nINNER JOIN \"Clinics\" c ON pcl.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR pcl.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("610aed06-0bc2-4fd6-b64d-42da36da2efa"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(1548), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("6bd1d346-91ff-49a0-93bc-cef039bf2dee"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(2182), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("799a2f18-7713-4c13-b80f-6d0aef30c579"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(2109), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(DISTINCT pcl.\"PatientId\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"PatientClinicLinks\" pcl ON c.\"Id\" = pcl.\"ClinicId\" AND pcl.\"IsActive\" = true\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("830fa650-1cf1-48b0-a865-016a0148730a"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(1637), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("ddc4544d-22f8-435e-a4b9-f3f51d0ed7dc"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(1454), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("ee1935c4-efc7-436c-99ca-6300157c21b2"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(1776), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("f02a7edc-c98b-47cc-9938-1d7755a7830f"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(2274), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5684732e-1bae-49f7-a91d-b4d8be19f0e2"), "financial", new DateTime(2026, 2, 27, 18, 55, 25, 824, DateTimeKind.Utc).AddTicks(9919), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("57671cd0-8d01-4ba1-865d-32e6f2e8f89b"), "customer", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(243), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("71039eb7-0cf5-44bf-a205-116294f044f9"), "clinical", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(703), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("87e398fb-4e0f-4d64-b86b-23820a2ab403"), "customer", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(13), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("93ff1da7-5b48-432e-b0e9-1b94354a9fd9"), "customer", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(147), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("b629d3c8-52f5-4446-a752-357778780590"), "financial", new DateTime(2026, 2, 27, 18, 55, 25, 824, DateTimeKind.Utc).AddTicks(9823), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("bd2eb4fd-fcf3-4f67-a3a3-d2ca0592f6b3"), "financial", new DateTime(2026, 2, 27, 18, 55, 25, 824, DateTimeKind.Utc).AddTicks(8938), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("bf162aca-ede8-442a-8be1-3d559da286c9"), "operational", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(433), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("d76e98cd-8e35-400d-9e9d-3fb05061b563"), "operational", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(610), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("ec4d62c2-2d66-45d8-8f47-b54ce9556285"), "clinical", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(856), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("f614cd80-f6f4-452d-921e-9bb3b2a77e8d"), "operational", new DateTime(2026, 2, 27, 18, 55, 25, 825, DateTimeKind.Utc).AddTicks(525), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationTokens_Users_UserId",
                table: "EmailVerificationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
