using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddPricingConfigurationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("04eb5b81-09ce-4cdb-9b41-42aed0f4ef21"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("06a5c59c-e23b-4da3-bca5-71f3bfe4c84f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("3f533c5b-7da9-40d0-b5f9-032c9b3d88f5"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4a2f3740-106e-4b1a-b83d-50b8ab37824c"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9396e03b-859e-4e50-a5a7-79aa412e4a73"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9f332f1d-732e-498b-89a9-79eb4789f20c"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("a8e762ed-0b64-4e65-afc4-8f6371e078af"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("c5a83e89-c560-4027-82ee-6749f9cd1be8"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("c819711b-33cc-42b7-aa2e-6a5a5629a953"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("fe6f9525-c3f9-4100-aa4f-b884daa38bd3"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1521b74d-ba7a-42c0-8c96-91ab47a08abc"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("28c581e1-a7ed-4783-a617-1bb0f398d2e6"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("53b90d9c-3a21-4d67-b1bc-7b036d16ed91"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("8b04c17b-ff39-4022-891b-8ac179c38c90"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("928b24bf-6335-45d0-9dfb-457980a38116"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("a415d2a2-ca19-4120-afb5-f2a53985a089"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("b4318db7-c9a9-4181-bb95-178d89558980"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("bd8087df-f568-4b5b-8a2e-bf4c198bd465"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("c73b38ef-81ad-4f01-9173-4bad80937fde"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("da04d8eb-d1d4-415d-b6dc-68105254db7e"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f41b82ef-b241-4c6e-87d1-81659e479f39"));

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentProcedureId",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TissGuideId",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClinicPricingConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultConsultationPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    FollowUpConsultationPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    TelemedicineConsultationPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    DefaultProcedurePolicy = table.Column<int>(type: "integer", nullable: false),
                    ConsultationDiscountPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    ConsultationDiscountFixedAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicPricingConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicPricingConfigurations_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcedurePricingConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcedureId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsultationPolicy = table.Column<int>(type: "integer", nullable: true),
                    ConsultationDiscountPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    ConsultationDiscountFixedAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    CustomPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedurePricingConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcedurePricingConfigurations_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcedurePricingConfigurations_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("243e4182-09be-4c01-aecc-c325e8cb45cb"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(3134), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("33ff9abd-3daf-41cf-9abb-d0e5be1b6df2"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(3014), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"DateOfBirth\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"PatientClinicLinks\" pcl ON p.\"Id\" = pcl.\"PatientId\" AND pcl.\"IsActive\" = true\nINNER JOIN \"Clinics\" c ON pcl.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR pcl.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("41f16f02-3660-4595-bfa9-226cbad3a00a"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(3090), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("709b774d-d4e7-42d9-9239-55db9e216b6a"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2848), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("9aa10a3d-4711-4579-83fe-215d724c87a1"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2469), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("a2b7cc32-0c5f-4048-a8c1-9b05eba6094f"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2758), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("b52d98a9-bf1e-4140-935a-7455a71afec7"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2976), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("e6663fb9-cc7f-4d45-872d-ac89365aa1dc"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(3054), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(DISTINCT pcl.\"PatientId\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"PatientClinicLinks\" pcl ON c.\"Id\" = pcl.\"ClinicId\" AND pcl.\"IsActive\" = true\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("f5c36abb-1894-428e-b5c9-6ebd5d8d0567"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2920), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("f95d2785-3bdd-4ddc-9590-cb794f4428af"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2807), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("122b8d05-ffc5-4286-aa86-579e4ac55e1a"), "operational", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2034), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("4432f10e-e2a1-418c-957e-3de76f3a063e"), "customer", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(1794), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("4eb5bc44-ecaa-4faa-8df0-26f01c8c72d8"), "customer", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(1897), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("542bc739-98d0-41d7-8726-6083784736f7"), "customer", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(1824), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("5bc6326e-cb5d-4362-800f-2ab526f5724f"), "clinical", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2297), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("724fb1ad-787a-41a3-8eee-f8175125639b"), "operational", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2125), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("74322a15-a754-476a-adf3-74a041b4545f"), "operational", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2080), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("7e05037d-d13e-4264-b275-becc60495baf"), "financial", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(1668), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("9eb07873-bbad-479e-b71c-15b01d3cb8ac"), "clinical", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(2195), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("b52a85a5-7cfd-4fd8-aa01-132cd7321fb1"), "financial", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(1184), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("c61cbea8-c507-4438-87bd-dcf14b28e18d"), "financial", new DateTime(2026, 2, 9, 18, 41, 0, 653, DateTimeKind.Utc).AddTicks(1724), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AppointmentProcedureId",
                table: "Payments",
                column: "AppointmentProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TissGuideId",
                table: "Invoices",
                column: "TissGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicPricingConfigurations_ClinicId",
                table: "ClinicPricingConfigurations",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedurePricingConfigurations_ClinicId",
                table: "ProcedurePricingConfigurations",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedurePricingConfigurations_ProcedureId",
                table: "ProcedurePricingConfigurations",
                column: "ProcedureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_TissGuides_TissGuideId",
                table: "Invoices",
                column: "TissGuideId",
                principalTable: "TissGuides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AppointmentProcedures_AppointmentProcedureId",
                table: "Payments",
                column: "AppointmentProcedureId",
                principalTable: "AppointmentProcedures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_TissGuides_TissGuideId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AppointmentProcedures_AppointmentProcedureId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "ClinicPricingConfigurations");

            migrationBuilder.DropTable(
                name: "ProcedurePricingConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_Payments_AppointmentProcedureId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_TissGuideId",
                table: "Invoices");

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("243e4182-09be-4c01-aecc-c325e8cb45cb"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("33ff9abd-3daf-41cf-9abb-d0e5be1b6df2"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("41f16f02-3660-4595-bfa9-226cbad3a00a"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("709b774d-d4e7-42d9-9239-55db9e216b6a"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9aa10a3d-4711-4579-83fe-215d724c87a1"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("a2b7cc32-0c5f-4048-a8c1-9b05eba6094f"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("b52d98a9-bf1e-4140-935a-7455a71afec7"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e6663fb9-cc7f-4d45-872d-ac89365aa1dc"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f5c36abb-1894-428e-b5c9-6ebd5d8d0567"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f95d2785-3bdd-4ddc-9590-cb794f4428af"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("122b8d05-ffc5-4286-aa86-579e4ac55e1a"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4432f10e-e2a1-418c-957e-3de76f3a063e"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4eb5bc44-ecaa-4faa-8df0-26f01c8c72d8"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("542bc739-98d0-41d7-8726-6083784736f7"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("5bc6326e-cb5d-4362-800f-2ab526f5724f"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("724fb1ad-787a-41a3-8eee-f8175125639b"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("74322a15-a754-476a-adf3-74a041b4545f"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7e05037d-d13e-4264-b275-becc60495baf"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9eb07873-bbad-479e-b71c-15b01d3cb8ac"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("b52a85a5-7cfd-4fd8-aa01-132cd7321fb1"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("c61cbea8-c507-4438-87bd-dcf14b28e18d"));

            migrationBuilder.DropColumn(
                name: "AppointmentProcedureId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TissGuideId",
                table: "Invoices");

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("04eb5b81-09ce-4cdb-9b41-42aed0f4ef21"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7122), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("06a5c59c-e23b-4da3-bca5-71f3bfe4c84f"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7163), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("3f533c5b-7da9-40d0-b5f9-032c9b3d88f5"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7283), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("4a2f3740-106e-4b1a-b83d-50b8ab37824c"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7320), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"DateOfBirth\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"PatientClinicLinks\" pcl ON p.\"Id\" = pcl.\"PatientId\" AND pcl.\"IsActive\" = true\nINNER JOIN \"Clinics\" c ON pcl.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR pcl.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("9396e03b-859e-4e50-a5a7-79aa412e4a73"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7360), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(DISTINCT pcl.\"PatientId\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"PatientClinicLinks\" pcl ON c.\"Id\" = pcl.\"ClinicId\" AND pcl.\"IsActive\" = true\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("9f332f1d-732e-498b-89a9-79eb4789f20c"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7441), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("a8e762ed-0b64-4e65-afc4-8f6371e078af"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7228), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("c5a83e89-c560-4027-82ee-6749f9cd1be8"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6771), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("c819711b-33cc-42b7-aa2e-6a5a5629a953"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7071), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("fe6f9525-c3f9-4100-aa4f-b884daa38bd3"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(7395), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1521b74d-ba7a-42c0-8c96-91ab47a08abc"), "financial", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(5972), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("28c581e1-a7ed-4783-a617-1bb0f398d2e6"), "clinical", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6495), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("53b90d9c-3a21-4d67-b1bc-7b036d16ed91"), "operational", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6419), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("8b04c17b-ff39-4022-891b-8ac179c38c90"), "financial", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6032), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("928b24bf-6335-45d0-9dfb-457980a38116"), "customer", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6092), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("a415d2a2-ca19-4120-afb5-f2a53985a089"), "clinical", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6592), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("b4318db7-c9a9-4181-bb95-178d89558980"), "operational", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6376), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("bd8087df-f568-4b5b-8a2e-bf4c198bd465"), "customer", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6122), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("c73b38ef-81ad-4f01-9173-4bad80937fde"), "financial", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(5538), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("da04d8eb-d1d4-415d-b6dc-68105254db7e"), "customer", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6186), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("f41b82ef-b241-4c6e-87d1-81659e479f39"), "operational", new DateTime(2026, 2, 8, 15, 47, 11, 859, DateTimeKind.Utc).AddTicks(6329), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null }
                });
        }
    }
}
