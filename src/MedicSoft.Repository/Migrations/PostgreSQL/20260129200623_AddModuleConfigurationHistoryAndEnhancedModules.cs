using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddModuleConfigurationHistoryAndEnhancedModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create ReportTemplates table if it doesn't exist
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""ReportTemplates"" (
                    ""Id"" uuid NOT NULL,
                    ""Name"" character varying(200) NOT NULL,
                    ""Description"" character varying(1000),
                    ""Category"" character varying(50) NOT NULL,
                    ""Configuration"" TEXT,
                    ""Query"" TEXT,
                    ""IsSystem"" boolean NOT NULL DEFAULT false,
                    ""Icon"" character varying(50),
                    ""SupportedFormats"" character varying(100),
                    ""CreatedAt"" timestamp with time zone NOT NULL,
                    ""UpdatedAt"" timestamp with time zone,
                    ""TenantId"" text NOT NULL DEFAULT '',
                    CONSTRAINT ""PK_ReportTemplates"" PRIMARY KEY (""Id"")
                );
                
                CREATE INDEX IF NOT EXISTS ""IX_ReportTemplates_Category"" ON ""ReportTemplates"" (""Category"");
                CREATE INDEX IF NOT EXISTS ""IX_ReportTemplates_IsSystem"" ON ""ReportTemplates"" (""IsSystem"");
            ");

            // Create WidgetTemplates table if it doesn't exist
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""WidgetTemplates"" (
                    ""Id"" uuid NOT NULL,
                    ""Name"" character varying(200) NOT NULL,
                    ""Description"" character varying(1000),
                    ""Category"" character varying(50) NOT NULL,
                    ""Type"" character varying(50) NOT NULL,
                    ""DefaultConfig"" TEXT,
                    ""DefaultQuery"" TEXT,
                    ""IsSystem"" boolean NOT NULL DEFAULT false,
                    ""Icon"" character varying(50),
                    ""CreatedAt"" timestamp with time zone NOT NULL,
                    ""UpdatedAt"" timestamp with time zone,
                    ""TenantId"" text NOT NULL DEFAULT '',
                    CONSTRAINT ""PK_WidgetTemplates"" PRIMARY KEY (""Id"")
                );
                
                CREATE INDEX IF NOT EXISTS ""IX_WidgetTemplates_Category"" ON ""WidgetTemplates"" (""Category"");
                CREATE INDEX IF NOT EXISTS ""IX_WidgetTemplates_Type"" ON ""WidgetTemplates"" (""Type"");
                CREATE INDEX IF NOT EXISTS ""IX_WidgetTemplates_IsSystem"" ON ""WidgetTemplates"" (""IsSystem"");
            ");

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("0b910675-cd7d-40f9-8e49-1244b627ddbb"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("18f87ada-dff3-46c9-97ec-88342562cdd3"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("2e5202a1-c4d4-44e1-b672-d34e99d77199"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("30b710de-a08c-4936-b8f1-30c2a5c83c90"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6278dcb2-6a97-40c3-bc28-aac9236d1235"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("829e9d57-4958-43be-919a-137b13e7870c"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("b6141b73-7248-43ab-abb0-224c838dc34a"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("c31b15ff-4e6e-44ac-93ce-22b2f1e54863"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("c3800180-4262-49da-8ef5-7332814f6dc8"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f27fc3ac-85c4-42b2-933f-b06cf4e0b08b"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1c4aaead-6560-4f94-b1fb-be105710af42"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1fdb0dfb-0fb6-46d7-89f5-3a6e78d9d1db"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("44841731-240d-4809-a7fd-7655b07e05d7"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("48f64549-e423-42c2-8264-601b8e85f5e1"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("54df762e-915a-4835-8a8c-801158594978"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("831df0e5-467c-4fdf-a927-4bd855adf276"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ae63e286-d8b2-43c4-b536-7e0c20329e96"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d18f3ff8-89a3-41c3-b56e-81d059e14172"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("d303c499-f95f-4686-b697-bcd74be1999c"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("eb616bce-484b-4879-bd2d-5bdd00cf2d58"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("fd12b02f-b7eb-475a-848d-829361677245"));

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "public",
                table: "user_sessions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                schema: "public",
                table: "user_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EnabledModules",
                table: "SubscriptionPlans",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModuleConfigurationHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PreviousConfiguration = table.Column<string>(type: "jsonb", nullable: true),
                    NewConfiguration = table.Column<string>(type: "jsonb", nullable: true),
                    ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleConfigurationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleConfigurationHistories_ModuleConfigurations_ModuleCon~",
                        column: x => x.ModuleConfigurationId,
                        principalTable: "ModuleConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("139a39f4-2fa4-48c0-ade4-1240d03d11d7"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2339), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("1e623521-7114-437c-b99f-234f3d3b0559"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2671), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("262ac068-2da4-4b34-9b99-e099781304d2"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2823), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("34b20eb2-9878-4ca8-b168-481bf32d2d81"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2710), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("40c85ddd-8214-4887-9ee9-578e46bb2fce"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2928), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("7ea7bea9-5301-44b1-8a42-22f56b739835"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2771), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("b2a6a082-8a55-4ce3-905a-1a53e437c8b5"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2626), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("e29961a2-ad04-47f7-8eac-1d3edd7ebffb"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2972), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("e2e21f47-7378-40b4-bad9-661de0e784f8"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2864), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"BirthDate\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR p.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("eef183ad-162e-42b7-8146-10e3ef408331"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2896), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(p.\"Id\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"Patients\" p ON c.\"Id\" = p.\"ClinicId\"\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("47267533-8c6e-405e-ae27-5fc078f6bf93"), "clinical", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2159), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("4b4ae0e8-a5b2-41f2-b162-debdfe9f4d5f"), "operational", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(1915), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("51b6f548-cb5a-49ea-834f-0210295ae229"), "financial", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(1538), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("5a8eb84c-68ed-4c42-9260-7215613f039e"), "customer", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(1686), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("87fff2e6-518f-4a6e-8848-5b44edea87bb"), "operational", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(1971), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("8f06477b-b64e-4a05-ad9b-79fb15a298e7"), "financial", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(1593), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("93477580-c577-44fa-b423-4b6ec58f4634"), "operational", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2013), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("c7f6cd8a-de5a-48e3-a92d-1e09ab742660"), "clinical", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(2082), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("cd05dff1-655e-49eb-a0fb-8588114fa734"), "financial", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(1043), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("e812da40-b535-4d45-bd34-783d469409b4"), "customer", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(1775), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("ea4507d3-0649-4dcd-bdce-92323b8a2303"), "customer", new DateTime(2026, 1, 29, 20, 6, 20, 742, DateTimeKind.Utc).AddTicks(1654), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurationHistories_ChangedAt",
                table: "ModuleConfigurationHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurationHistories_ClinicId_ModuleName",
                table: "ModuleConfigurationHistories",
                columns: new[] { "ClinicId", "ModuleName" });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurationHistories_ModuleConfigurationId",
                table: "ModuleConfigurationHistories",
                column: "ModuleConfigurationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleConfigurationHistories");

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("139a39f4-2fa4-48c0-ade4-1240d03d11d7"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("1e623521-7114-437c-b99f-234f3d3b0559"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("262ac068-2da4-4b34-9b99-e099781304d2"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("34b20eb2-9878-4ca8-b168-481bf32d2d81"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("40c85ddd-8214-4887-9ee9-578e46bb2fce"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7ea7bea9-5301-44b1-8a42-22f56b739835"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("b2a6a082-8a55-4ce3-905a-1a53e437c8b5"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e29961a2-ad04-47f7-8eac-1d3edd7ebffb"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e2e21f47-7378-40b4-bad9-661de0e784f8"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("eef183ad-162e-42b7-8146-10e3ef408331"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("47267533-8c6e-405e-ae27-5fc078f6bf93"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4b4ae0e8-a5b2-41f2-b162-debdfe9f4d5f"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("51b6f548-cb5a-49ea-834f-0210295ae229"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("5a8eb84c-68ed-4c42-9260-7215613f039e"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("87fff2e6-518f-4a6e-8848-5b44edea87bb"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("8f06477b-b64e-4a05-ad9b-79fb15a298e7"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("93477580-c577-44fa-b423-4b6ec58f4634"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("c7f6cd8a-de5a-48e3-a92d-1e09ab742660"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("cd05dff1-655e-49eb-a0fb-8588114fa734"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e812da40-b535-4d45-bd34-783d469409b4"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ea4507d3-0649-4dcd-bdce-92323b8a2303"));

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "public",
                table: "user_sessions");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                schema: "public",
                table: "user_sessions");

            migrationBuilder.DropColumn(
                name: "EnabledModules",
                table: "SubscriptionPlans");

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0b910675-cd7d-40f9-8e49-1244b627ddbb"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9526), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("18f87ada-dff3-46c9-97ec-88342562cdd3"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9566), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("2e5202a1-c4d4-44e1-b672-d34e99d77199"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9787), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("30b710de-a08c-4936-b8f1-30c2a5c83c90"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9836), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("6278dcb2-6a97-40c3-bc28-aac9236d1235"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9723), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"BirthDate\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR p.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("829e9d57-4958-43be-919a-137b13e7870c"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9190), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("b6141b73-7248-43ab-abb0-224c838dc34a"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9754), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(p.\"Id\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"Patients\" p ON c.\"Id\" = p.\"ClinicId\"\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("c31b15ff-4e6e-44ac-93ce-22b2f1e54863"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9636), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("c3800180-4262-49da-8ef5-7332814f6dc8"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9689), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("f27fc3ac-85c4-42b2-933f-b06cf4e0b08b"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9466), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1c4aaead-6560-4f94-b1fb-be105710af42"), "customer", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8683), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("1fdb0dfb-0fb6-46d7-89f5-3a6e78d9d1db"), "operational", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8857), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("44841731-240d-4809-a7fd-7655b07e05d7"), "financial", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8461), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("48f64549-e423-42c2-8264-601b8e85f5e1"), "clinical", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(9053), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("54df762e-915a-4835-8a8c-801158594978"), "customer", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8606), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("831df0e5-467c-4fdf-a927-4bd855adf276"), "clinical", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8972), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("ae63e286-d8b2-43c4-b536-7e0c20329e96"), "financial", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(7871), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("d18f3ff8-89a3-41c3-b56e-81d059e14172"), "operational", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8815), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("d303c499-f95f-4686-b697-bcd74be1999c"), "financial", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8519), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("eb616bce-484b-4879-bd2d-5bdd00cf2d58"), "operational", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8899), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("fd12b02f-b7eb-475a-848d-829361677245"), "customer", new DateTime(2026, 1, 29, 3, 12, 29, 563, DateTimeKind.Utc).AddTicks(8576), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null }
                });
        }
    }
}
