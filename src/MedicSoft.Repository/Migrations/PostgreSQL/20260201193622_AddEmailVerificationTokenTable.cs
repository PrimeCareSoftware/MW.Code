using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddEmailVerificationTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("05b36156-870d-4bfa-9def-4995f69d5f18"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6949398c-b1e3-402c-96b2-a036cd59fe99"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6cb15d6f-5425-412b-9182-dfd5523280ec"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("774fcc61-e774-4c16-b76e-51a16d575a94"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7e8f78fe-6e42-4eba-9135-77f575254cf1"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9c19a2a5-d2d4-4cca-a6bf-1097fc877ca2"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("aba8e1dc-dee2-470c-9daa-891b1cc1367c"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d0cc5b23-56c4-48a2-815d-5901b7090013"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d7746dc1-c1ee-42bd-a5c9-5d860978f83d"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("faf533c5-0c09-4bc1-992f-98ad60609236"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("03ceedb2-18df-406b-998d-eed8b920dcd1"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("05686f70-bb75-41ec-940f-724db3e6f25a"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("106a16b9-4935-47c3-89d4-472bcafee324"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("25d50273-a525-4b8e-b382-bcc13bfffd7c"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("325621e7-d67a-4069-8b82-a7b128a546cb"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("70470003-4a5f-4dea-a7cf-e846cc610575"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("8323aebe-4a88-44d1-9846-6ba4e6444029"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9d2dd734-1be6-4ef0-9df9-54af091ae3be"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("acdb97ca-587d-4048-9b79-7cd3276daa18"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d1be656f-b211-403c-a9e8-43a87252464e"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e7c9c7b2-ef85-495b-95d5-aea7cc8936d3"));

            migrationBuilder.CreateTable(
                name: "EmailVerificationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationAttempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailVerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0c45609a-e37c-4dc7-b236-94323b0980ae"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8960), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"BirthDate\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR p.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("31b0e048-e23d-4b73-a09d-70800d68f920"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8926), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("3df6cb59-a6ff-4504-b0ec-3f199a083c58"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(9073), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("3f22582f-b9df-4c3e-a4ec-c42a9552626a"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8990), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(p.\"Id\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"Patients\" p ON c.\"Id\" = p.\"ClinicId\"\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("4086d65a-6906-4fdf-a509-e76a18506471"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8879), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("7edd625e-2249-4d95-abe0-58ccbd85627f"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(9032), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("86cc2910-a884-48f5-b6f5-c62dba046abd"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8810), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("d0c038a6-8e31-466e-b348-d2ff9bc6e18d"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8443), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("db795f8b-22a8-4340-96c5-26cace4aeace"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8770), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("faee9381-64bd-469f-bc02-7d8408517105"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8721), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1b8ab3ff-e7b9-467e-a5a1-333b3a970080"), "operational", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8016), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("2aceaccd-c46f-4794-b899-4c354d7a97e4"), "operational", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8063), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("42c8c685-8e10-43ed-af10-37febfb73c85"), "customer", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(7787), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("4e985e89-ccda-429b-9b73-c14faf8487b7"), "clinical", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8170), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("5cf77de6-d056-4135-b1d1-ba5bd560713a"), "clinical", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8239), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("6df53e80-56d6-43e4-8b62-4c5b2452fcd2"), "customer", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(7888), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("74cbbac8-4dc1-4304-b8c8-f019699750f7"), "operational", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(8105), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("a06b701c-11c2-419d-b0f8-a4b9717ace1d"), "financial", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(7674), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("a501bb99-6acc-4918-addd-b2b82be44884"), "customer", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(7814), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("bcff43cf-8fc5-4bbc-996b-d6ef73e19184"), "financial", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(7266), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("e0afe9d2-44a4-4b99-acb0-382527361820"), "financial", new DateTime(2026, 2, 1, 19, 36, 20, 146, DateTimeKind.Utc).AddTicks(7728), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_Code_UserId_TenantId",
                table: "EmailVerificationTokens",
                columns: new[] { "Code", "UserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_TenantId_ExpiresAt",
                table: "EmailVerificationTokens",
                columns: new[] { "TenantId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_UserId_TenantId_CreatedAt",
                table: "EmailVerificationTokens",
                columns: new[] { "UserId", "TenantId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailVerificationTokens");

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("0c45609a-e37c-4dc7-b236-94323b0980ae"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("31b0e048-e23d-4b73-a09d-70800d68f920"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("3df6cb59-a6ff-4504-b0ec-3f199a083c58"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("3f22582f-b9df-4c3e-a4ec-c42a9552626a"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4086d65a-6906-4fdf-a509-e76a18506471"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7edd625e-2249-4d95-abe0-58ccbd85627f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("86cc2910-a884-48f5-b6f5-c62dba046abd"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d0c038a6-8e31-466e-b348-d2ff9bc6e18d"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("db795f8b-22a8-4340-96c5-26cace4aeace"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("faee9381-64bd-469f-bc02-7d8408517105"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1b8ab3ff-e7b9-467e-a5a1-333b3a970080"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("2aceaccd-c46f-4794-b899-4c354d7a97e4"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("42c8c685-8e10-43ed-af10-37febfb73c85"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4e985e89-ccda-429b-9b73-c14faf8487b7"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("5cf77de6-d056-4135-b1d1-ba5bd560713a"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6df53e80-56d6-43e4-8b62-4c5b2452fcd2"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("74cbbac8-4dc1-4304-b8c8-f019699750f7"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("a06b701c-11c2-419d-b0f8-a4b9717ace1d"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("a501bb99-6acc-4918-addd-b2b82be44884"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("bcff43cf-8fc5-4bbc-996b-d6ef73e19184"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e0afe9d2-44a4-4b99-acb0-382527361820"));

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("05b36156-870d-4bfa-9def-4995f69d5f18"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(191), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("6949398c-b1e3-402c-96b2-a036cd59fe99"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9758), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("6cb15d6f-5425-412b-9182-dfd5523280ec"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(304), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(p.\"Id\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"Patients\" p ON c.\"Id\" = p.\"ClinicId\"\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("774fcc61-e774-4c16-b76e-51a16d575a94"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(130), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("7e8f78fe-6e42-4eba-9135-77f575254cf1"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(91), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("9c19a2a5-d2d4-4cca-a6bf-1097fc877ca2"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(40), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("aba8e1dc-dee2-470c-9daa-891b1cc1367c"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(387), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("d0cc5b23-56c4-48a2-815d-5901b7090013"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(343), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("d7746dc1-c1ee-42bd-a5c9-5d860978f83d"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(241), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("faf533c5-0c09-4bc1-992f-98ad60609236"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 2, 1, 18, 33, 47, 155, DateTimeKind.Utc).AddTicks(274), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"BirthDate\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR p.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("03ceedb2-18df-406b-998d-eed8b920dcd1"), "clinical", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9550), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("05686f70-bb75-41ec-940f-724db3e6f25a"), "operational", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9400), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("106a16b9-4935-47c3-89d4-472bcafee324"), "operational", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9306), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("25d50273-a525-4b8e-b382-bcc13bfffd7c"), "financial", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9001), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("325621e7-d67a-4069-8b82-a7b128a546cb"), "financial", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(8951), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("70470003-4a5f-4dea-a7cf-e846cc610575"), "customer", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9063), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("8323aebe-4a88-44d1-9846-6ba4e6444029"), "customer", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9093), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("9d2dd734-1be6-4ef0-9df9-54af091ae3be"), "customer", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9173), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("acdb97ca-587d-4048-9b79-7cd3276daa18"), "financial", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(8534), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("d1be656f-b211-403c-a9e8-43a87252464e"), "operational", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9357), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("e7c9c7b2-ef85-495b-95d5-aea7cc8936d3"), "clinical", new DateTime(2026, 2, 1, 18, 33, 47, 154, DateTimeKind.Utc).AddTicks(9469), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null }
                });
        }
    }
}
