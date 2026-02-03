using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddDocumentHashToPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create missing tables that are referenced later in this or subsequent migrations
            
            // SubscriptionCredits table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""SubscriptionCredits"" (
                    ""Id"" serial NOT NULL,
                    ""SubscriptionId"" uuid NOT NULL,
                    ""Days"" integer NOT NULL,
                    ""Reason"" text NOT NULL,
                    ""GrantedAt"" timestamp with time zone NOT NULL,
                    ""GrantedBy"" uuid NOT NULL,
                    ""GrantedByUserId"" uuid NOT NULL,
                    CONSTRAINT ""PK_SubscriptionCredits"" PRIMARY KEY (""Id"")
                );
                
                CREATE INDEX IF NOT EXISTS ""IX_SubscriptionCredits_GrantedByUserId"" ON ""SubscriptionCredits"" (""GrantedByUserId"");
                CREATE INDEX IF NOT EXISTS ""IX_SubscriptionCredits_SubscriptionId"" ON ""SubscriptionCredits"" (""SubscriptionId"");
            ");

            // SystemNotifications table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""SystemNotifications"" (
                    ""Id"" uuid NOT NULL,
                    ""Type"" text NOT NULL,
                    ""Category"" text NOT NULL,
                    ""Title"" text NOT NULL,
                    ""Message"" text NOT NULL,
                    ""ActionUrl"" text,
                    ""ActionLabel"" text,
                    ""IsRead"" boolean NOT NULL DEFAULT false,
                    ""ReadAt"" timestamp with time zone,
                    ""Data"" text,
                    ""CreatedAt"" timestamp with time zone NOT NULL,
                    ""UpdatedAt"" timestamp with time zone,
                    ""TenantId"" text NOT NULL DEFAULT '',
                    CONSTRAINT ""PK_SystemNotifications"" PRIMARY KEY (""Id"")
                );
                
                CREATE INDEX IF NOT EXISTS ""IX_SystemNotifications_Category"" ON ""SystemNotifications"" (""Category"");
                CREATE INDEX IF NOT EXISTS ""IX_SystemNotifications_IsRead"" ON ""SystemNotifications"" (""IsRead"");
                CREATE INDEX IF NOT EXISTS ""IX_SystemNotifications_CreatedAt"" ON ""SystemNotifications"" (""CreatedAt"");
            ");

            // NotificationRules table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""NotificationRules"" (
                    ""Id"" uuid NOT NULL,
                    ""Trigger"" text NOT NULL,
                    ""IsEnabled"" boolean NOT NULL DEFAULT true,
                    ""Conditions"" text,
                    ""Actions"" text,
                    ""CreatedAt"" timestamp with time zone NOT NULL,
                    ""UpdatedAt"" timestamp with time zone,
                    ""TenantId"" text NOT NULL DEFAULT '',
                    CONSTRAINT ""PK_NotificationRules"" PRIMARY KEY (""Id"")
                );
                
                CREATE INDEX IF NOT EXISTS ""IX_NotificationRules_Trigger"" ON ""NotificationRules"" (""Trigger"");
                CREATE INDEX IF NOT EXISTS ""IX_NotificationRules_IsEnabled"" ON ""NotificationRules"" (""IsEnabled"");
            ");

            // CustomDashboards table - Using proper EF Core migration methods
            // Check if table exists before creating to handle both fresh and existing databases
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'CustomDashboards' AND table_schema = 'public') THEN
                        CREATE TABLE ""CustomDashboards"" (
                            ""Id"" uuid NOT NULL,
                            ""Name"" character varying(200) NOT NULL,
                            ""Description"" character varying(1000) NOT NULL,
                            ""Layout"" TEXT NOT NULL,
                            ""IsDefault"" boolean NOT NULL DEFAULT false,
                            ""IsPublic"" boolean NOT NULL DEFAULT false,
                            ""CreatedBy"" character varying(450) NOT NULL,
                            ""CreatedAt"" timestamp with time zone NOT NULL,
                            ""UpdatedAt"" timestamp with time zone,
                            ""TenantId"" text NOT NULL DEFAULT '',
                            CONSTRAINT ""PK_CustomDashboards"" PRIMARY KEY (""Id"")
                        );
                        
                        CREATE INDEX ""IX_CustomDashboards_CreatedBy"" ON ""CustomDashboards"" (""CreatedBy"");
                        CREATE INDEX ""IX_CustomDashboards_IsDefault"" ON ""CustomDashboards"" (""IsDefault"");
                        CREATE INDEX ""IX_CustomDashboards_TenantId"" ON ""CustomDashboards"" (""TenantId"");
                    END IF;
                END $$;
            ");

            // DashboardWidgets table - Using proper EF Core migration methods
            // Check if table exists before creating to handle both fresh and existing databases
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'DashboardWidgets' AND table_schema = 'public') THEN
                        CREATE TABLE ""DashboardWidgets"" (
                            ""Id"" uuid NOT NULL,
                            ""DashboardId"" uuid NOT NULL,
                            ""Type"" character varying(50) NOT NULL,
                            ""Title"" character varying(200) NOT NULL,
                            ""Query"" TEXT NOT NULL,
                            ""Config"" TEXT NOT NULL,
                            ""GridX"" integer NOT NULL DEFAULT 0,
                            ""GridY"" integer NOT NULL DEFAULT 0,
                            ""GridWidth"" integer NOT NULL DEFAULT 4,
                            ""GridHeight"" integer NOT NULL DEFAULT 3,
                            ""RefreshInterval"" integer NOT NULL DEFAULT 0,
                            ""CreatedAt"" timestamp with time zone NOT NULL,
                            ""UpdatedAt"" timestamp with time zone,
                            ""TenantId"" text NOT NULL DEFAULT '',
                            CONSTRAINT ""PK_DashboardWidgets"" PRIMARY KEY (""Id""),
                            CONSTRAINT ""FK_DashboardWidgets_CustomDashboards_DashboardId"" FOREIGN KEY (""DashboardId"") 
                                REFERENCES ""CustomDashboards"" (""Id"") ON DELETE CASCADE
                        );
                        
                        CREATE INDEX ""IX_DashboardWidgets_DashboardId"" ON ""DashboardWidgets"" (""DashboardId"");
                        CREATE INDEX ""IX_DashboardWidgets_Type"" ON ""DashboardWidgets"" (""Type"");
                    END IF;
                END $$;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_AutomationActions_MarketingAutomations_MarketingAutomationI~",
                schema: "crm",
                table: "AutomationActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintInteractions_Complaints_ComplaintId2",
                schema: "crm",
                table: "ComplaintInteractions");

            migrationBuilder.DropForeignKey(
                name: "FK_JourneyStages_PatientJourneys_PatientJourneyId2",
                schema: "crm",
                table: "JourneyStages");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientTouchpoints_JourneyStages_JourneyStageId1",
                schema: "crm",
                table: "PatientTouchpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyQuestionResponses_SurveyResponses_SurveyResponseId1",
                schema: "crm",
                table: "SurveyQuestionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyQuestions_Surveys_SurveyId2",
                schema: "crm",
                table: "SurveyQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResponses_Surveys_SurveyId1",
                schema: "crm",
                table: "SurveyResponses");

            migrationBuilder.DropIndex(
                name: "IX_SurveyResponses_SurveyId1",
                schema: "crm",
                table: "SurveyResponses");

            migrationBuilder.DropIndex(
                name: "IX_SurveyQuestions_SurveyId2",
                schema: "crm",
                table: "SurveyQuestions");

            migrationBuilder.DropIndex(
                name: "IX_SurveyQuestionResponses_SurveyResponseId1",
                schema: "crm",
                table: "SurveyQuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_PatientTouchpoints_JourneyStageId1",
                schema: "crm",
                table: "PatientTouchpoints");

            migrationBuilder.DropIndex(
                name: "IX_JourneyStages_PatientJourneyId2",
                schema: "crm",
                table: "JourneyStages");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintInteractions_ComplaintId2",
                schema: "crm",
                table: "ComplaintInteractions");

            migrationBuilder.DropIndex(
                name: "IX_AutomationActions_MarketingAutomationId1",
                schema: "crm",
                table: "AutomationActions");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Severity",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_TenantId",
                table: "AuditLogs");

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("013e9e43-ddda-48e1-9153-83e25501ae02"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("18e75c2e-332b-4af5-9af4-5e29f394235d"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("31748a32-9a83-4648-ae86-f61e4f8c229c"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("53a9efad-3ba5-4118-9d12-176aeba9e901"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("645018e4-0bc9-4d02-85bb-b2f60232ed5d"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("740e044c-c0dd-4f89-b4c0-27fbb3a44254"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("83dc5fe1-7984-4fb4-a45d-581f50e1a921"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("b32c7a68-5fae-4f05-8534-f99a16b228b3"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ea8fa21e-885e-4d6c-8896-66cfebc6f3e8"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f99c3a25-1049-42d2-a352-92273929a447"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("02c0c0ac-fed4-45b2-8c10-883704ec2f4a"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("2cd6edbc-9746-48dd-ae79-07a4f8d14001"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4018dfbc-aaf3-4e44-988a-5914b435f5d4"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("4ac26e3c-0fef-489d-8ff7-62722154678c"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("5ab6dbe7-71d8-428f-879e-7287c2ffea47"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("71371967-8b17-4110-8540-1d2b7260f913"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("72fef700-4032-43a4-a96f-7cce4a84e772"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7a509af0-0d2b-4e70-aeb3-3732abd47843"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("cbb5e96a-caa7-4842-bb57-398964f79674"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e63e701e-c3ba-4b16-8c4c-245013e86c50"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ea3dfe4a-cf69-4671-920a-caad64629cd8"));

            migrationBuilder.DropColumn(
                name: "SurveyId1",
                schema: "crm",
                table: "SurveyResponses");

            migrationBuilder.DropColumn(
                name: "SurveyId2",
                schema: "crm",
                table: "SurveyQuestions");

            migrationBuilder.DropColumn(
                name: "SurveyResponseId1",
                schema: "crm",
                table: "SurveyQuestionResponses");

            migrationBuilder.DropColumn(
                name: "JourneyStageId1",
                schema: "crm",
                table: "PatientTouchpoints");

            migrationBuilder.DropColumn(
                name: "PatientJourneyId2",
                schema: "crm",
                table: "JourneyStages");

            migrationBuilder.DropColumn(
                name: "ComplaintId2",
                schema: "crm",
                table: "ComplaintInteractions");

            migrationBuilder.DropColumn(
                name: "MarketingAutomationId1",
                schema: "crm",
                table: "AutomationActions");

            // Conditionally alter Workflows table columns only if table and columns exist with old type
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Only proceed if the Workflows table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'Workflows'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'Workflows' 
                            AND column_name = 'UpdatedAt'
                            AND data_type = 'timestamp without time zone'
                        ) THEN
                            ALTER TABLE ""Workflows"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                        END IF;
                        
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'Workflows' 
                            AND column_name = 'CreatedAt'
                            AND data_type = 'timestamp without time zone'
                        ) THEN
                            ALTER TABLE ""Workflows"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone;
                        END IF;
                    END IF;
                END $$;
            ");

            // Conditionally alter WorkflowExecutions table columns only if table and columns exist with old type
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Only proceed if the WorkflowExecutions table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'WorkflowExecutions'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowExecutions' 
                            AND column_name = 'StartedAt'
                            AND data_type = 'timestamp without time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowExecutions"" ALTER COLUMN ""StartedAt"" TYPE timestamp with time zone;
                        END IF;
                        
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowExecutions' 
                            AND column_name = 'CompletedAt'
                            AND data_type = 'timestamp without time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowExecutions"" ALTER COLUMN ""CompletedAt"" TYPE timestamp with time zone;
                        END IF;
                    END IF;
                END $$;
            ");

            // Conditionally alter WorkflowActions table columns only if table and columns exist with old type
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Only proceed if the WorkflowActions table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'WorkflowActions'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowActions' 
                            AND column_name = 'UpdatedAt'
                            AND data_type = 'timestamp without time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowActions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                        END IF;
                        
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowActions' 
                            AND column_name = 'CreatedAt'
                            AND data_type = 'timestamp without time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowActions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone;
                        END IF;
                    END IF;
                END $$;
            ");

            // Conditionally alter WorkflowActionExecutions table columns only if table and columns exist with old type
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Only proceed if the WorkflowActionExecutions table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'WorkflowActionExecutions'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowActionExecutions' 
                            AND column_name = 'StartedAt'
                            AND data_type = 'timestamp without time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowActionExecutions"" ALTER COLUMN ""StartedAt"" TYPE timestamp with time zone;
                        END IF;
                        
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowActionExecutions' 
                            AND column_name = 'CompletedAt'
                            AND data_type = 'timestamp without time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowActionExecutions"" ALTER COLUMN ""CompletedAt"" TYPE timestamp with time zone;
                        END IF;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WidgetTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WidgetTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WidgetTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WidgetTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'LastSuccessAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""LastSuccessAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'LastFailureAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""LastFailureAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'LastDeliveryAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""LastDeliveryAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'NextRetryAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""NextRetryAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'FailedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""FailedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'DeliveredAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""DeliveredAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'CompletedTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""CompletedTime"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'CheckInTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""CheckInTime"" TYPE timestamp with time zone, ALTER COLUMN ""CheckInTime"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'CalledTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""CalledTime"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueConfigurations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueConfigurations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueConfigurations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueConfigurations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Users"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' 
                        AND column_name = 'LastLoginAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Users"" ALTER COLUMN ""LastLoginAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Users"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstLoginAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MfaGracePeriodEndsAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserClinicLinks' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""UserClinicLinks"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserClinicLinks' 
                        AND column_name = 'LinkedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""UserClinicLinks"" ALTER COLUMN ""LinkedDate"" TYPE timestamp with time zone, ALTER COLUMN ""LinkedDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserClinicLinks' 
                        AND column_name = 'InactivatedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""UserClinicLinks"" ALTER COLUMN ""InactivatedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserClinicLinks' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""UserClinicLinks"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'StartedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""StartedAt"" TYPE timestamp with time zone, ALTER COLUMN ""StartedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'LastActivityAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""LastActivityAt"" TYPE timestamp with time zone, ALTER COLUMN ""LastActivityAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'ExpiresAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""ExpiresAt"" TYPE timestamp with time zone, ALTER COLUMN ""ExpiresAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TwoFactorBackupCodes' 
                        AND column_name = 'UsedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TwoFactorBackupCodes"" ALTER COLUMN ""UsedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TwoFactorAuth' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TwoFactorAuth"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TwoFactorAuth' 
                        AND column_name = 'EnabledAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TwoFactorAuth"" ALTER COLUMN ""EnabledAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TwoFactorAuth' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TwoFactorAuth"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TussProcedures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TussProcedures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TussProcedures' 
                        AND column_name = 'LastUpdated'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TussProcedures"" ALTER COLUMN ""LastUpdated"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TussProcedures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TussProcedures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissRecursosGlosa' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissRecursosGlosa"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissRecursosGlosa' 
                        AND column_name = 'DataResposta'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissRecursosGlosa"" ALTER COLUMN ""DataResposta"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissRecursosGlosa' 
                        AND column_name = 'DataEnvio'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissRecursosGlosa"" ALTER COLUMN ""DataEnvio"" TYPE timestamp with time zone, ALTER COLUMN ""DataEnvio"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissRecursosGlosa' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissRecursosGlosa"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissOperadoraConfigs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissOperadoraConfigs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissOperadoraConfigs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissOperadoraConfigs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuides' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGuides"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuides' 
                        AND column_name = 'ServiceDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGuides"" ALTER COLUMN ""ServiceDate"" TYPE timestamp with time zone, ALTER COLUMN ""ServiceDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuides' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGuides"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuideProcedures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGuideProcedures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuideProcedures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGuideProcedures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGlosas' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGlosas"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGlosas' 
                        AND column_name = 'DataIdentificacao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGlosas"" ALTER COLUMN ""DataIdentificacao"" TYPE timestamp with time zone, ALTER COLUMN ""DataIdentificacao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGlosas' 
                        AND column_name = 'DataGlosa'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGlosas"" ALTER COLUMN ""DataGlosa"" TYPE timestamp with time zone, ALTER COLUMN ""DataGlosa"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGlosas' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissGlosas"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'SubmittedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""SubmittedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'ProcessedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""ProcessedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'CreatedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""CreatedDate"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tickets' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Tickets"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tickets' 
                        AND column_name = 'LastStatusChangeAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Tickets"" ALTER COLUMN ""LastStatusChangeAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tickets' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Tickets"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketHistory' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TicketHistory"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketHistory' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TicketHistory"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketHistory' 
                        AND column_name = 'ChangedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TicketHistory"" ALTER COLUMN ""ChangedAt"" TYPE timestamp with time zone, ALTER COLUMN ""ChangedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketComments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TicketComments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketComments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TicketComments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketAttachments' 
                        AND column_name = 'UploadedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TicketAttachments"" ALTER COLUMN ""UploadedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UploadedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketAttachments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TicketAttachments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketAttachments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TicketAttachments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TherapeuticPlans' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TherapeuticPlans"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TherapeuticPlans' 
                        AND column_name = 'ReturnDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TherapeuticPlans"" ALTER COLUMN ""ReturnDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TherapeuticPlans' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""TherapeuticPlans"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tags' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Tags"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tags' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Tags"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SystemNotifications' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SystemNotifications"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SystemNotifications' 
                        AND column_name = 'ReadAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SystemNotifications"" ALTER COLUMN ""ReadAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SystemNotifications' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SystemNotifications"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Surveys' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""Surveys"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Surveys' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""Surveys"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyResponses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyResponses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyResponses' 
                        AND column_name = 'StartedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyResponses"" ALTER COLUMN ""StartedAt"" TYPE timestamp with time zone, ALTER COLUMN ""StartedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyResponses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyResponses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyResponses' 
                        AND column_name = 'CompletedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyResponses"" ALTER COLUMN ""CompletedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestionResponses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestionResponses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestionResponses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestionResponses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestionResponses' 
                        AND column_name = 'AnsweredAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestionResponses"" ALTER COLUMN ""AnsweredAt"" TYPE timestamp with time zone, ALTER COLUMN ""AnsweredAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Suppliers' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Suppliers"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Suppliers' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Suppliers"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SubscriptionPlans' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SubscriptionPlans"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SubscriptionPlans' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SubscriptionPlans"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SubscriptionCredits' 
                        AND column_name = 'GrantedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SubscriptionCredits"" ALTER COLUMN ""GrantedAt"" TYPE timestamp with time zone, ALTER COLUMN ""GrantedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SoapRecords' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SoapRecords"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SoapRecords' 
                        AND column_name = 'RecordDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SoapRecords"" ALTER COLUMN ""RecordDate"" TYPE timestamp with time zone, ALTER COLUMN ""RecordDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SoapRecords' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SoapRecords"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SoapRecords' 
                        AND column_name = 'CompletionDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SoapRecords"" ALTER COLUMN ""CompletionDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcTransmissions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SngpcTransmissions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcTransmissions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SngpcTransmissions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcTransmissions' 
                        AND column_name = 'AttemptedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SngpcTransmissions"" ALTER COLUMN ""AttemptedAt"" TYPE timestamp with time zone, ALTER COLUMN ""AttemptedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'TransmittedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""TransmittedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'ReportPeriodStart'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""ReportPeriodStart"" TYPE timestamp with time zone, ALTER COLUMN ""ReportPeriodStart"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'ReportPeriodEnd'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""ReportPeriodEnd"" TYPE timestamp with time zone, ALTER COLUMN ""ReportPeriodEnd"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'LastAttemptAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""LastAttemptAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'GeneratedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""GeneratedAt"" TYPE timestamp with time zone, ALTER COLUMN ""GeneratedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcAlerts' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SngpcAlerts"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcAlerts' 
                        AND column_name = 'ResolvedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SngpcAlerts"" ALTER COLUMN ""ResolvedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcAlerts' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SngpcAlerts"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcAlerts' 
                        AND column_name = 'AcknowledgedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SngpcAlerts"" ALTER COLUMN ""AcknowledgedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SentimentAnalyses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SentimentAnalyses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SentimentAnalyses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SentimentAnalyses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SentimentAnalyses' 
                        AND column_name = 'AnalyzedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""SentimentAnalyses"" ALTER COLUMN ""AnalyzedAt"" TYPE timestamp with time zone, ALTER COLUMN ""AnalyzedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'DataHoraSaida'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""DataHoraSaida"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'DataHoraEntrada'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""DataHoraEntrada"" TYPE timestamp with time zone, ALTER COLUMN ""DataHoraEntrada"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'DataHoraChamada'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""DataHoraChamada"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'DataHoraAtendimento'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""DataHoraAtendimento"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ScheduledReports' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ScheduledReports"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ScheduledReports' 
                        AND column_name = 'NextRunAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ScheduledReports"" ALTER COLUMN ""NextRunAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ScheduledReports' 
                        AND column_name = 'LastRunAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ScheduledReports"" ALTER COLUMN ""LastRunAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ScheduledReports' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ScheduledReports"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SalesFunnelMetrics' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SalesFunnelMetrics"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SalesFunnelMetrics' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""SalesFunnelMetrics"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReportTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ReportTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReportTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ReportTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReceivablePayments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ReceivablePayments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReceivablePayments' 
                        AND column_name = 'PaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ReceivablePayments"" ALTER COLUMN ""PaymentDate"" TYPE timestamp with time zone, ALTER COLUMN ""PaymentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReceivablePayments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ReceivablePayments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ProfilePermissions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ProfilePermissions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ProfilePermissions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ProfilePermissions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Procedures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Procedures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Procedures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Procedures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ProcedureMaterials' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ProcedureMaterials"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ProcedureMaterials' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ProcedureMaterials"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionSequenceControls' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionSequenceControls"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionSequenceControls' 
                        AND column_name = 'LastGeneratedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionSequenceControls"" ALTER COLUMN ""LastGeneratedAt"" TYPE timestamp with time zone, ALTER COLUMN ""LastGeneratedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionSequenceControls' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionSequenceControls"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionItems' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionItems"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionItems' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionItems"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PlanoContas' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PlanoContas"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PlanoContas' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PlanoContas"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'ProcessedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""ProcessedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'PaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""PaymentDate"" TYPE timestamp with time zone, ALTER COLUMN ""PaymentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'CancellationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""CancellationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PayablePayments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PayablePayments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PayablePayments' 
                        AND column_name = 'PaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PayablePayments"" ALTER COLUMN ""PaymentDate"" TYPE timestamp with time zone, ALTER COLUMN ""PaymentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PayablePayments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PayablePayments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientTouchpoints' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientTouchpoints"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientTouchpoints' 
                        AND column_name = 'Timestamp'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientTouchpoints"" ALTER COLUMN ""Timestamp"" TYPE timestamp with time zone, ALTER COLUMN ""Timestamp"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientTouchpoints' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientTouchpoints"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Patients' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Patients"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "Document",
                table: "Patients",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Patients' 
                        AND column_name = 'DateOfBirth'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Patients"" ALTER COLUMN ""DateOfBirth"" TYPE timestamp with time zone, ALTER COLUMN ""DateOfBirth"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Patients' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Patients"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<string>(
                name: "DocumentHash",
                table: "Patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientJourneys' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientJourneys"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientJourneys' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientJourneys"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientHealthInsurances' 
                        AND column_name = 'ValidUntil'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PatientHealthInsurances"" ALTER COLUMN ""ValidUntil"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientHealthInsurances' 
                        AND column_name = 'ValidFrom'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PatientHealthInsurances"" ALTER COLUMN ""ValidFrom"" TYPE timestamp with time zone, ALTER COLUMN ""ValidFrom"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientHealthInsurances' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PatientHealthInsurances"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientHealthInsurances' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PatientHealthInsurances"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientClinicLinks' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PatientClinicLinks"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientClinicLinks' 
                        AND column_name = 'LinkedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PatientClinicLinks"" ALTER COLUMN ""LinkedAt"" TYPE timestamp with time zone, ALTER COLUMN ""LinkedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientClinicLinks' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PatientClinicLinks"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'VerifiedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""VerifiedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'UsedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""UsedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'ExpiresAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""ExpiresAt"" TYPE timestamp with time zone, ALTER COLUMN ""ExpiresAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Owners' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Owners"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Owners' 
                        AND column_name = 'LastLoginAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Owners"" ALTER COLUMN ""LastLoginAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Owners' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Owners"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'OwnerClinicLinks' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""OwnerClinicLinks"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'OwnerClinicLinks' 
                        AND column_name = 'LinkedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""OwnerClinicLinks"" ALTER COLUMN ""LinkedDate"" TYPE timestamp with time zone, ALTER COLUMN ""LinkedDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'OwnerClinicLinks' 
                        AND column_name = 'InactivatedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""OwnerClinicLinks"" ALTER COLUMN ""InactivatedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'OwnerClinicLinks' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""OwnerClinicLinks"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'owner_sessions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""owner_sessions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'owner_sessions' 
                        AND column_name = 'LastActivityAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""owner_sessions"" ALTER COLUMN ""LastActivityAt"" TYPE timestamp with time zone, ALTER COLUMN ""LastActivityAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'owner_sessions' 
                        AND column_name = 'ExpiresAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""owner_sessions"" ALTER COLUMN ""ExpiresAt"" TYPE timestamp with time zone, ALTER COLUMN ""ExpiresAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'owner_sessions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE public.""owner_sessions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'SentAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""SentAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'ReadAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""ReadAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'DeliveredAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""DeliveredAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRules' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRules"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRules' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRules"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRoutines' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRoutines"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRoutines' 
                        AND column_name = 'NextExecutionAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRoutines"" ALTER COLUMN ""NextExecutionAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRoutines' 
                        AND column_name = 'LastExecutedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRoutines"" ALTER COLUMN ""LastExecutedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRoutines' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRoutines"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MonthlyControlledBalances' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MonthlyControlledBalances"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MonthlyControlledBalances' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MonthlyControlledBalances"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MonthlyControlledBalances' 
                        AND column_name = 'ClosedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MonthlyControlledBalances"" ALTER COLUMN ""ClosedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurationHistories' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurationHistories"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurationHistories' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurationHistories"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurationHistories' 
                        AND column_name = 'ChangedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurationHistories"" ALTER COLUMN ""ChangedAt"" TYPE timestamp with time zone, ALTER COLUMN ""ChangedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Medications' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Medications"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Medications' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Medications"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordVersions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordVersions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordVersions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordVersions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordVersions' 
                        AND column_name = 'ChangedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordVersions"" ALTER COLUMN ""ChangedAt"" TYPE timestamp with time zone, ALTER COLUMN ""ChangedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordSignatures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordSignatures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordSignatures' 
                        AND column_name = 'SignedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordSignatures"" ALTER COLUMN ""SignedAt"" TYPE timestamp with time zone, ALTER COLUMN ""SignedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordSignatures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordSignatures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'ReopenedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""ReopenedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'ConsultationStartTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""ConsultationStartTime"" TYPE timestamp with time zone, ALTER COLUMN ""ConsultationStartTime"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'ConsultationEndTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""ConsultationEndTime"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'ClosedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""ClosedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordAccessLogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordAccessLogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordAccessLogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordAccessLogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordAccessLogs' 
                        AND column_name = 'AccessedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordAccessLogs"" ALTER COLUMN ""AccessedAt"" TYPE timestamp with time zone, ALTER COLUMN ""AccessedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Materials' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Materials"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Materials' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Materials"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MarketingAutomations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""MarketingAutomations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MarketingAutomations' 
                        AND column_name = 'LastExecutedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""MarketingAutomations"" ALTER COLUMN ""LastExecutedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MarketingAutomations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""MarketingAutomations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LoginAttempts' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""LoginAttempts"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LoginAttempts' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""LoginAttempts"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LoginAttempts' 
                        AND column_name = 'AttemptTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""LoginAttempts"" ALTER COLUMN ""AttemptTime"" TYPE timestamp with time zone, ALTER COLUMN ""AttemptTime"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LancamentosContabeis' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""LancamentosContabeis"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LancamentosContabeis' 
                        AND column_name = 'DataLancamento'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""LancamentosContabeis"" ALTER COLUMN ""DataLancamento"" TYPE timestamp with time zone, ALTER COLUMN ""DataLancamento"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LancamentosContabeis' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""LancamentosContabeis"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'JourneyStages' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""JourneyStages"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'JourneyStages' 
                        AND column_name = 'ExitedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""JourneyStages"" ALTER COLUMN ""ExitedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'JourneyStages' 
                        AND column_name = 'EnteredAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""JourneyStages"" ALTER COLUMN ""EnteredAt"" TYPE timestamp with time zone, ALTER COLUMN ""EnteredAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'JourneyStages' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""JourneyStages"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'SentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""SentDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'PaidDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""PaidDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'IssueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""IssueDate"" TYPE timestamp with time zone, ALTER COLUMN ""IssueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'DueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""DueDate"" TYPE timestamp with time zone, ALTER COLUMN ""DueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'CancellationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""CancellationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InvoiceConfigurations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""InvoiceConfigurations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InvoiceConfigurations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""InvoiceConfigurations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InvoiceConfigurations' 
                        AND column_name = 'CertificateExpirationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""InvoiceConfigurations"" ALTER COLUMN ""CertificateExpirationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InformedConsents' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""InformedConsents"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InformedConsents' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""InformedConsents"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InformedConsents' 
                        AND column_name = 'AcceptedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""InformedConsents"" ALTER COLUMN ""AcceptedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ImpostosNotas' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ImpostosNotas"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ImpostosNotas' 
                        AND column_name = 'DataCalculo'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ImpostosNotas"" ALTER COLUMN ""DataCalculo"" TYPE timestamp with time zone, ALTER COLUMN ""DataCalculo"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ImpostosNotas' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ImpostosNotas"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsurancePlans' 
                        AND column_name = 'ValidUntil'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsurancePlans"" ALTER COLUMN ""ValidUntil"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsurancePlans' 
                        AND column_name = 'ValidFrom'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsurancePlans"" ALTER COLUMN ""ValidFrom"" TYPE timestamp with time zone, ALTER COLUMN ""ValidFrom"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsurancePlans' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsurancePlans"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsurancePlans' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsurancePlans"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsuranceOperators' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsuranceOperators"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsuranceOperators' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsuranceOperators"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosures' 
                        AND column_name = 'SettlementDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosures"" ALTER COLUMN ""SettlementDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosures' 
                        AND column_name = 'ClosureDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosures"" ALTER COLUMN ""ClosureDate"" TYPE timestamp with time zone, ALTER COLUMN ""ClosureDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosureItems' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosureItems"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosureItems' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosureItems"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FilasEspera' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""FilasEspera"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FilasEspera' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""FilasEspera"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Expenses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Expenses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Expenses' 
                        AND column_name = 'PaidDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Expenses"" ALTER COLUMN ""PaidDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Expenses' 
                        AND column_name = 'DueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Expenses"" ALTER COLUMN ""DueDate"" TYPE timestamp with time zone, ALTER COLUMN ""DueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Expenses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Expenses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'ScheduledDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""ScheduledDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'RequestedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""RequestedDate"" TYPE timestamp with time zone, ALTER COLUMN ""RequestedDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'CompletedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""CompletedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamCatalogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ExamCatalogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamCatalogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ExamCatalogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'EmailTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""EmailTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'EmailTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""EmailTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'IssueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""IssueDate"" TYPE timestamp with time zone, ALTER COLUMN ""IssueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'CancellationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""CancellationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'AuthorizationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""AuthorizationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'PeriodoInicio'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""PeriodoInicio"" TYPE timestamp with time zone, ALTER COLUMN ""PeriodoInicio"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'PeriodoFim'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""PeriodoFim"" TYPE timestamp with time zone, ALTER COLUMN ""PeriodoFim"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'DataGeracao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""DataGeracao"" TYPE timestamp with time zone, ALTER COLUMN ""DataGeracao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'SignedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""SignedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'ReportedToSNGPCAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""ReportedToSNGPCAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'IssuedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""IssuedAt"" TYPE timestamp with time zone, ALTER COLUMN ""IssuedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'ExpiresAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""ExpiresAt"" TYPE timestamp with time zone, ALTER COLUMN ""ExpiresAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptionItems' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptionItems"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptionItems' 
                        AND column_name = 'ManufactureDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptionItems"" ALTER COLUMN ""ManufactureDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptionItems' 
                        AND column_name = 'ExpiryDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptionItems"" ALTER COLUMN ""ExpiryDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptionItems' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptionItems"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DiagnosticHypotheses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DiagnosticHypotheses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DiagnosticHypotheses' 
                        AND column_name = 'DiagnosedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DiagnosticHypotheses"" ALTER COLUMN ""DiagnosedAt"" TYPE timestamp with time zone, ALTER COLUMN ""DiagnosedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DiagnosticHypotheses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DiagnosticHypotheses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataProcessingConsents' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataProcessingConsents"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataProcessingConsents' 
                        AND column_name = 'RevokedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataProcessingConsents"" ALTER COLUMN ""RevokedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataProcessingConsents' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataProcessingConsents"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataProcessingConsents' 
                        AND column_name = 'ConsentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataProcessingConsents"" ALTER COLUMN ""ConsentDate"" TYPE timestamp with time zone, ALTER COLUMN ""ConsentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'RequestDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""RequestDate"" TYPE timestamp with time zone, ALTER COLUMN ""RequestDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'ProcessedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""ProcessedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'LegalApprovalDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""LegalApprovalDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'CompletedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""CompletedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'RevokedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""RevokedDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'ExpirationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""ExpirationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'ConsentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""ConsentDate"" TYPE timestamp with time zone, ALTER COLUMN ""ConsentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataAccessLogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataAccessLogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataAccessLogs' 
                        AND column_name = 'Timestamp'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataAccessLogs"" ALTER COLUMN ""Timestamp"" TYPE timestamp with time zone, ALTER COLUMN ""Timestamp"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataAccessLogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DataAccessLogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DashboardWidgets' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DashboardWidgets"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DashboardWidgets' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""DashboardWidgets"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CustomDashboards' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CustomDashboards"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CustomDashboards' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CustomDashboards"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'RegisteredAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""RegisteredAt"" TYPE timestamp with time zone, ALTER COLUMN ""RegisteredAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'DocumentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""DocumentDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'Date'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""Date"" TYPE timestamp with time zone, ALTER COLUMN ""Date"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultationFormProfiles' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultationFormProfiles"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultationFormProfiles' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultationFormProfiles"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultationFormConfigurations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultationFormConfigurations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultationFormConfigurations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultationFormConfigurations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'UltimaAtualizacao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""UltimaAtualizacao"" TYPE timestamp with time zone, ALTER COLUMN ""UltimaAtualizacao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'DataConsolidacao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""DataConsolidacao"" TYPE timestamp with time zone, ALTER COLUMN ""DataConsolidacao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'Data'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""Data"" TYPE timestamp with time zone, ALTER COLUMN ""Data"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConfiguracoesFiscais' 
                        AND column_name = 'VigenciaInicio'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConfiguracoesFiscais"" ALTER COLUMN ""VigenciaInicio"" TYPE timestamp with time zone, ALTER COLUMN ""VigenciaInicio"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConfiguracoesFiscais' 
                        AND column_name = 'VigenciaFim'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConfiguracoesFiscais"" ALTER COLUMN ""VigenciaFim"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConfiguracoesFiscais' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConfiguracoesFiscais"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConfiguracoesFiscais' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ConfiguracoesFiscais"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'ResolvedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""ResolvedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'ReceivedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""ReceivedAt"" TYPE timestamp with time zone, ALTER COLUMN ""ReceivedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'FirstResponseAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""FirstResponseAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'ClosedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""ClosedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ComplaintInteractions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""ComplaintInteractions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ComplaintInteractions' 
                        AND column_name = 'InteractionDate'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""ComplaintInteractions"" ALTER COLUMN ""InteractionDate"" TYPE timestamp with time zone, ALTER COLUMN ""InteractionDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ComplaintInteractions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""ComplaintInteractions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Companies' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Companies"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Companies' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Companies"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicTags' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicTags"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicTags' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicTags"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicTags' 
                        AND column_name = 'AssignedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicTags"" ALTER COLUMN ""AssignedAt"" TYPE timestamp with time zone, ALTER COLUMN ""AssignedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'TrialEndDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""TrialEndDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'StartDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""StartDate"" TYPE timestamp with time zone, ALTER COLUMN ""StartDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'PlanChangeDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""PlanChangeDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'NextPaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""NextPaymentDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'ManualOverrideSetAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""ManualOverrideSetAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'LastPaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""LastPaymentDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'FrozenStartDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""FrozenStartDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'FrozenEndDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""FrozenEndDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'EndDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""EndDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'CancellationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""CancellationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Clinics' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Clinics"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Clinics' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Clinics"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicCustomizations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicCustomizations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicCustomizations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicCustomizations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicalExaminations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicalExaminations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicalExaminations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ClinicalExaminations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ChurnPredictions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""ChurnPredictions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ChurnPredictions' 
                        AND column_name = 'PredictedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""ChurnPredictions"" ALTER COLUMN ""PredictedAt"" TYPE timestamp with time zone, ALTER COLUMN ""PredictedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ChurnPredictions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""ChurnPredictions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'DataExpiracao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""DataExpiracao"" TYPE timestamp with time zone, ALTER COLUMN ""DataExpiracao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'DataEmissao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""DataEmissao"" TYPE timestamp with time zone, ALTER COLUMN ""DataEmissao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'DataCadastro'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""DataCadastro"" TYPE timestamp with time zone, ALTER COLUMN ""DataCadastro"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CashFlowEntries' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CashFlowEntries"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CashFlowEntries' 
                        AND column_name = 'TransactionDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CashFlowEntries"" ALTER COLUMN ""TransactionDate"" TYPE timestamp with time zone, ALTER COLUMN ""TransactionDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CashFlowEntries' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""CashFlowEntries"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'BalancosPatrimoniais' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""BalancosPatrimoniais"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'BalancosPatrimoniais' 
                        AND column_name = 'DataReferencia'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""BalancosPatrimoniais"" ALTER COLUMN ""DataReferencia"" TYPE timestamp with time zone, ALTER COLUMN ""DataReferencia"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'BalancosPatrimoniais' 
                        AND column_name = 'DataGeracao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""BalancosPatrimoniais"" ALTER COLUMN ""DataGeracao"" TYPE timestamp with time zone, ALTER COLUMN ""DataGeracao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'BalancosPatrimoniais' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""BalancosPatrimoniais"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AutomationActions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""AutomationActions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AutomationActions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE crm.""AutomationActions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'RequestDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""RequestDate"" TYPE timestamp with time zone, ALTER COLUMN ""RequestDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'ExpirationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""ExpirationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'AuthorizationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""AuthorizationDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuditLogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AuditLogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuditLogs' 
                        AND column_name = 'Timestamp'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AuditLogs"" ALTER COLUMN ""Timestamp"" TYPE timestamp with time zone, ALTER COLUMN ""Timestamp"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuditLogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AuditLogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'DataUltimaValidacao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""DataUltimaValidacao"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'DataTimestamp'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""DataTimestamp"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'DataHoraAssinatura'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""DataHoraAssinatura"" TYPE timestamp with time zone, ALTER COLUMN ""DataHoraAssinatura"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ApuracoesImpostos' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ApuracoesImpostos"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ApuracoesImpostos' 
                        AND column_name = 'DataPagamento'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ApuracoesImpostos"" ALTER COLUMN ""DataPagamento"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ApuracoesImpostos' 
                        AND column_name = 'DataApuracao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ApuracoesImpostos"" ALTER COLUMN ""DataApuracao"" TYPE timestamp with time zone, ALTER COLUMN ""DataApuracao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ApuracoesImpostos' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""ApuracoesImpostos"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'ScheduledDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""ScheduledDate"" TYPE timestamp with time zone, ALTER COLUMN ""ScheduledDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'PaidAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""PaidAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'CheckOutTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""CheckOutTime"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'CheckInTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""CheckInTime"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AppointmentProcedures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AppointmentProcedures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AppointmentProcedures' 
                        AND column_name = 'PerformedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AppointmentProcedures"" ALTER COLUMN ""PerformedAt"" TYPE timestamp with time zone, ALTER COLUMN ""PerformedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AppointmentProcedures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AppointmentProcedures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisResponses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisResponses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisResponses' 
                        AND column_name = 'ResponseDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisResponses"" ALTER COLUMN ""ResponseDate"" TYPE timestamp with time zone, ALTER COLUMN ""ResponseDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisResponses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisResponses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'SettlementDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""SettlementDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'IssueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""IssueDate"" TYPE timestamp with time zone, ALTER COLUMN ""IssueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'DueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""DueDate"" TYPE timestamp with time zone, ALTER COLUMN ""DueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'PaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""PaymentDate"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'IssueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""IssueDate"" TYPE timestamp with time zone, ALTER COLUMN ""IssueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'DueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""DueDate"" TYPE timestamp with time zone, ALTER COLUMN ""DueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'UnlocksAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""UnlocksAt"" TYPE timestamp with time zone, ALTER COLUMN ""UnlocksAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'UnlockedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""UnlockedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'LockedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""LockedAt"" TYPE timestamp with time zone, ALTER COLUMN ""LockedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccessProfiles' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccessProfiles"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccessProfiles' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp without time zone'
                    ) THEN
                        ALTER TABLE ""AccessProfiles"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.CreateTable(
                name: "EncryptionKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    KeyVersion = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RotatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RotatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Algorithm = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Purpose = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EncryptedKeyMaterial = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptionKeys", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("281e375c-fd64-4c65-9d93-171ee8b6e51d"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5539), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("43b0955f-f827-4ac0-bed4-18ca4de1c74c"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5630), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("5e9263dd-3d37-4147-991d-9df9ab5c26a5"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5691), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("8a6774f5-35cd-45d5-bc4c-9262fb225c7e"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5847), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("9a03c809-fc26-4621-b433-a9d2dd6d54d3"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5746), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("9d8b9bd8-be9e-4d89-ab4b-26d90e8ba019"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5903), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null },
                    { new Guid("a97d16b6-6550-4f2a-a02f-e737bca1b609"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5813), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(p.\"Id\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"Patients\" p ON c.\"Id\" = p.\"ClinicId\"\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("e812e3db-d9a6-4ebd-aa0d-a8792b863eb3"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5782), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"BirthDate\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR p.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("fb9da3b9-68a0-4a24-8d46-eab835f91a4e"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5235), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("fe35c51f-f851-49d9-9089-1ac91bd6bd13"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5588), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0021743f-cc28-4615-b697-196996b30a9b"), "operational", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4893), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("10076eaa-e24a-4af9-910b-97fc5f2a3fce"), "operational", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4810), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("243b238e-58ac-499c-abf1-8af8f4fcc5b7"), "clinical", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4958), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("649eaabe-f802-4331-8bc5-83116c04aec3"), "financial", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(3836), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null },
                    { new Guid("68c0b4b9-7425-4176-aad5-14ce745ae249"), "clinical", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(5037), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("7218a0f2-64f5-4e11-a060-572aef4aa6ff"), "customer", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4516), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("ba56a0ba-2ede-451c-a688-ee3e04985eed"), "financial", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4458), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("c0dcba7b-ba40-4e63-a0c6-020b519496cf"), "customer", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4668), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("cb2f80d1-1dff-4353-b0fb-ff45380c3945"), "operational", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4854), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("f05e6d79-11e2-4cae-89f1-d614873f7b0b"), "financial", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4403), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("fbb70aec-2d0e-4237-8b74-7094bc05ca8e"), "customer", new DateTime(2026, 1, 31, 2, 59, 30, 990, DateTimeKind.Utc).AddTicks(4598), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_DocumentHash",
                table: "Patients",
                column: "DocumentHash");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Tenant_Action_Time",
                table: "AuditLogs",
                columns: new[] { "TenantId", "Action", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Tenant_Entity",
                table: "AuditLogs",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Tenant_HighSeverity_Time",
                table: "AuditLogs",
                columns: new[] { "TenantId", "Severity", "Timestamp" },
                filter: "\"Severity\" IN ('WARNING', 'ERROR', 'CRITICAL')");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Tenant_Severity",
                table: "AuditLogs",
                columns: new[] { "TenantId", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Tenant_Time",
                table: "AuditLogs",
                columns: new[] { "TenantId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Tenant_User_Time",
                table: "AuditLogs",
                columns: new[] { "TenantId", "UserId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_EncryptionKeys_CreatedAt",
                table: "EncryptionKeys",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EncryptionKeys_IsActive_TenantId",
                table: "EncryptionKeys",
                columns: new[] { "IsActive", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_EncryptionKeys_KeyId_KeyVersion_TenantId",
                table: "EncryptionKeys",
                columns: new[] { "KeyId", "KeyVersion", "TenantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncryptionKeys");

            migrationBuilder.DropIndex(
                name: "IX_Patients_DocumentHash",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Tenant_Action_Time",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Tenant_Entity",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Tenant_HighSeverity_Time",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Tenant_Severity",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Tenant_Time",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Tenant_User_Time",
                table: "AuditLogs");

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("281e375c-fd64-4c65-9d93-171ee8b6e51d"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("43b0955f-f827-4ac0-bed4-18ca4de1c74c"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("5e9263dd-3d37-4147-991d-9df9ab5c26a5"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("8a6774f5-35cd-45d5-bc4c-9262fb225c7e"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9a03c809-fc26-4621-b433-a9d2dd6d54d3"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("9d8b9bd8-be9e-4d89-ab4b-26d90e8ba019"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("a97d16b6-6550-4f2a-a02f-e737bca1b609"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("e812e3db-d9a6-4ebd-aa0d-a8792b863eb3"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("fb9da3b9-68a0-4a24-8d46-eab835f91a4e"));

            migrationBuilder.DeleteData(
                table: "ReportTemplates",
                keyColumn: "Id",
                keyValue: new Guid("fe35c51f-f851-49d9-9089-1ac91bd6bd13"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("0021743f-cc28-4615-b697-196996b30a9b"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10076eaa-e24a-4af9-910b-97fc5f2a3fce"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("243b238e-58ac-499c-abf1-8af8f4fcc5b7"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("649eaabe-f802-4331-8bc5-83116c04aec3"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("68c0b4b9-7425-4176-aad5-14ce745ae249"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("7218a0f2-64f5-4e11-a060-572aef4aa6ff"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("ba56a0ba-2ede-451c-a688-ee3e04985eed"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("c0dcba7b-ba40-4e63-a0c6-020b519496cf"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("cb2f80d1-1dff-4353-b0fb-ff45380c3945"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f05e6d79-11e2-4cae-89f1-d614873f7b0b"));

            migrationBuilder.DeleteData(
                table: "WidgetTemplates",
                keyColumn: "Id",
                keyValue: new Guid("fbb70aec-2d0e-4237-8b74-7094bc05ca8e"));

            migrationBuilder.DropColumn(
                name: "FirstLoginAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MfaGracePeriodEndsAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DocumentHash",
                table: "Patients");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    -- Only proceed if the Workflows table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'Workflows'
                        AND table_schema = 'public'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'Workflows' 
                            AND column_name = 'UpdatedAt'
                            AND table_schema = 'public'
                            AND data_type = 'timestamp with time zone'
                        ) THEN
                            ALTER TABLE ""Workflows"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                        END IF;
                        
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'Workflows' 
                            AND column_name = 'CreatedAt'
                            AND table_schema = 'public'
                            AND data_type = 'timestamp with time zone'
                        ) THEN
                            ALTER TABLE ""Workflows"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                        END IF;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    -- Only proceed if the WorkflowExecutions table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'WorkflowExecutions'
                        AND table_schema = 'public'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowExecutions' 
                            AND column_name = 'StartedAt'
                            AND table_schema = 'public'
                            AND data_type = 'timestamp with time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowExecutions"" ALTER COLUMN ""StartedAt"" TYPE timestamp without time zone, ALTER COLUMN ""StartedAt"" SET NOT NULL;
                        END IF;
                        
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowExecutions' 
                            AND column_name = 'CompletedAt'
                            AND table_schema = 'public'
                            AND data_type = 'timestamp with time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowExecutions"" ALTER COLUMN ""CompletedAt"" TYPE timestamp without time zone;
                        END IF;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    -- Only proceed if the WorkflowActions table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'WorkflowActions'
                        AND table_schema = 'public'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowActions' 
                            AND column_name = 'UpdatedAt'
                            AND table_schema = 'public'
                            AND data_type = 'timestamp with time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowActions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                        END IF;
                        
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowActions' 
                            AND column_name = 'CreatedAt'
                            AND table_schema = 'public'
                            AND data_type = 'timestamp with time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowActions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                        END IF;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    -- Only proceed if the WorkflowActionExecutions table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'WorkflowActionExecutions'
                        AND table_schema = 'public'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowActionExecutions' 
                            AND column_name = 'StartedAt'
                            AND table_schema = 'public'
                            AND data_type = 'timestamp with time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowActionExecutions"" ALTER COLUMN ""StartedAt"" TYPE timestamp without time zone, ALTER COLUMN ""StartedAt"" SET NOT NULL;
                        END IF;
                        
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'WorkflowActionExecutions' 
                            AND column_name = 'CompletedAt'
                            AND table_schema = 'public'
                            AND data_type = 'timestamp with time zone'
                        ) THEN
                            ALTER TABLE ""WorkflowActionExecutions"" ALTER COLUMN ""CompletedAt"" TYPE timestamp without time zone;
                        END IF;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WidgetTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WidgetTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WidgetTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WidgetTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'LastSuccessAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""LastSuccessAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'LastFailureAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""LastFailureAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'LastDeliveryAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""LastDeliveryAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookSubscriptions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookSubscriptions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'NextRetryAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""NextRetryAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'FailedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""FailedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'DeliveredAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""DeliveredAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WebhookDeliveries' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""WebhookDeliveries"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'CompletedTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""CompletedTime"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'CheckInTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""CheckInTime"" TYPE timestamp without time zone, ALTER COLUMN ""CheckInTime"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueEntries' 
                        AND column_name = 'CalledTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueEntries"" ALTER COLUMN ""CalledTime"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueConfigurations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueConfigurations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'WaitingQueueConfigurations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""WaitingQueueConfigurations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Users"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' 
                        AND column_name = 'LastLoginAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Users"" ALTER COLUMN ""LastLoginAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Users"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserClinicLinks' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""UserClinicLinks"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserClinicLinks' 
                        AND column_name = 'LinkedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""UserClinicLinks"" ALTER COLUMN ""LinkedDate"" TYPE timestamp without time zone, ALTER COLUMN ""LinkedDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserClinicLinks' 
                        AND column_name = 'InactivatedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""UserClinicLinks"" ALTER COLUMN ""InactivatedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserClinicLinks' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""UserClinicLinks"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'StartedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""StartedAt"" TYPE timestamp without time zone, ALTER COLUMN ""StartedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'LastActivityAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""LastActivityAt"" TYPE timestamp without time zone, ALTER COLUMN ""LastActivityAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'ExpiresAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""ExpiresAt"" TYPE timestamp without time zone, ALTER COLUMN ""ExpiresAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'user_sessions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""user_sessions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TwoFactorBackupCodes' 
                        AND column_name = 'UsedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TwoFactorBackupCodes"" ALTER COLUMN ""UsedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TwoFactorAuth' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TwoFactorAuth"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TwoFactorAuth' 
                        AND column_name = 'EnabledAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TwoFactorAuth"" ALTER COLUMN ""EnabledAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TwoFactorAuth' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TwoFactorAuth"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TussProcedures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TussProcedures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TussProcedures' 
                        AND column_name = 'LastUpdated'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TussProcedures"" ALTER COLUMN ""LastUpdated"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TussProcedures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TussProcedures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissRecursosGlosa' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissRecursosGlosa"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissRecursosGlosa' 
                        AND column_name = 'DataResposta'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissRecursosGlosa"" ALTER COLUMN ""DataResposta"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissRecursosGlosa' 
                        AND column_name = 'DataEnvio'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissRecursosGlosa"" ALTER COLUMN ""DataEnvio"" TYPE timestamp without time zone, ALTER COLUMN ""DataEnvio"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissRecursosGlosa' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissRecursosGlosa"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissOperadoraConfigs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissOperadoraConfigs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissOperadoraConfigs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissOperadoraConfigs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuides' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGuides"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuides' 
                        AND column_name = 'ServiceDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGuides"" ALTER COLUMN ""ServiceDate"" TYPE timestamp without time zone, ALTER COLUMN ""ServiceDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuides' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGuides"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuideProcedures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGuideProcedures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGuideProcedures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGuideProcedures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGlosas' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGlosas"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGlosas' 
                        AND column_name = 'DataIdentificacao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGlosas"" ALTER COLUMN ""DataIdentificacao"" TYPE timestamp without time zone, ALTER COLUMN ""DataIdentificacao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGlosas' 
                        AND column_name = 'DataGlosa'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGlosas"" ALTER COLUMN ""DataGlosa"" TYPE timestamp without time zone, ALTER COLUMN ""DataGlosa"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissGlosas' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissGlosas"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'SubmittedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""SubmittedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'ProcessedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""ProcessedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'CreatedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""CreatedDate"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TissBatches' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TissBatches"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tickets' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Tickets"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tickets' 
                        AND column_name = 'LastStatusChangeAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Tickets"" ALTER COLUMN ""LastStatusChangeAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tickets' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Tickets"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketHistory' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TicketHistory"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketHistory' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TicketHistory"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketHistory' 
                        AND column_name = 'ChangedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TicketHistory"" ALTER COLUMN ""ChangedAt"" TYPE timestamp without time zone, ALTER COLUMN ""ChangedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketComments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TicketComments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketComments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TicketComments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketAttachments' 
                        AND column_name = 'UploadedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TicketAttachments"" ALTER COLUMN ""UploadedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UploadedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketAttachments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TicketAttachments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TicketAttachments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TicketAttachments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TherapeuticPlans' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TherapeuticPlans"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TherapeuticPlans' 
                        AND column_name = 'ReturnDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TherapeuticPlans"" ALTER COLUMN ""ReturnDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'TherapeuticPlans' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""TherapeuticPlans"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tags' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Tags"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Tags' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Tags"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SystemNotifications' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SystemNotifications"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SystemNotifications' 
                        AND column_name = 'ReadAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SystemNotifications"" ALTER COLUMN ""ReadAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SystemNotifications' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SystemNotifications"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Surveys' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""Surveys"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Surveys' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""Surveys"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyResponses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyResponses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyResponses' 
                        AND column_name = 'StartedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyResponses"" ALTER COLUMN ""StartedAt"" TYPE timestamp without time zone, ALTER COLUMN ""StartedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyResponses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyResponses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyResponses' 
                        AND column_name = 'CompletedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyResponses"" ALTER COLUMN ""CompletedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<Guid>(
                name: "SurveyId1",
                schema: "crm",
                table: "SurveyResponses",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<Guid>(
                name: "SurveyId2",
                schema: "crm",
                table: "SurveyQuestions",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestionResponses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestionResponses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestionResponses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestionResponses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SurveyQuestionResponses' 
                        AND column_name = 'AnsweredAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SurveyQuestionResponses"" ALTER COLUMN ""AnsweredAt"" TYPE timestamp without time zone, ALTER COLUMN ""AnsweredAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<Guid>(
                name: "SurveyResponseId1",
                schema: "crm",
                table: "SurveyQuestionResponses",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Suppliers' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Suppliers"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Suppliers' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Suppliers"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SubscriptionPlans' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SubscriptionPlans"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SubscriptionPlans' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SubscriptionPlans"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SubscriptionCredits' 
                        AND column_name = 'GrantedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SubscriptionCredits"" ALTER COLUMN ""GrantedAt"" TYPE timestamp without time zone, ALTER COLUMN ""GrantedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SoapRecords' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SoapRecords"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SoapRecords' 
                        AND column_name = 'RecordDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SoapRecords"" ALTER COLUMN ""RecordDate"" TYPE timestamp without time zone, ALTER COLUMN ""RecordDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SoapRecords' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SoapRecords"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SoapRecords' 
                        AND column_name = 'CompletionDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SoapRecords"" ALTER COLUMN ""CompletionDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcTransmissions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SngpcTransmissions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcTransmissions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SngpcTransmissions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcTransmissions' 
                        AND column_name = 'AttemptedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SngpcTransmissions"" ALTER COLUMN ""AttemptedAt"" TYPE timestamp without time zone, ALTER COLUMN ""AttemptedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'TransmittedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""TransmittedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'ReportPeriodStart'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""ReportPeriodStart"" TYPE timestamp without time zone, ALTER COLUMN ""ReportPeriodStart"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'ReportPeriodEnd'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""ReportPeriodEnd"" TYPE timestamp without time zone, ALTER COLUMN ""ReportPeriodEnd"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'LastAttemptAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""LastAttemptAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'GeneratedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""GeneratedAt"" TYPE timestamp without time zone, ALTER COLUMN ""GeneratedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SNGPCReports' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SNGPCReports"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcAlerts' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SngpcAlerts"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcAlerts' 
                        AND column_name = 'ResolvedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SngpcAlerts"" ALTER COLUMN ""ResolvedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcAlerts' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SngpcAlerts"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SngpcAlerts' 
                        AND column_name = 'AcknowledgedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SngpcAlerts"" ALTER COLUMN ""AcknowledgedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SentimentAnalyses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SentimentAnalyses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SentimentAnalyses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SentimentAnalyses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SentimentAnalyses' 
                        AND column_name = 'AnalyzedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""SentimentAnalyses"" ALTER COLUMN ""AnalyzedAt"" TYPE timestamp without time zone, ALTER COLUMN ""AnalyzedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'DataHoraSaida'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""DataHoraSaida"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'DataHoraEntrada'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""DataHoraEntrada"" TYPE timestamp without time zone, ALTER COLUMN ""DataHoraEntrada"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'DataHoraChamada'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""DataHoraChamada"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'DataHoraAtendimento'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""DataHoraAtendimento"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SenhasFila' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SenhasFila"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ScheduledReports' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ScheduledReports"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ScheduledReports' 
                        AND column_name = 'NextRunAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ScheduledReports"" ALTER COLUMN ""NextRunAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ScheduledReports' 
                        AND column_name = 'LastRunAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ScheduledReports"" ALTER COLUMN ""LastRunAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ScheduledReports' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ScheduledReports"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SalesFunnelMetrics' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SalesFunnelMetrics"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'SalesFunnelMetrics' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""SalesFunnelMetrics"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReportTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ReportTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReportTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ReportTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReceivablePayments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ReceivablePayments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReceivablePayments' 
                        AND column_name = 'PaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ReceivablePayments"" ALTER COLUMN ""PaymentDate"" TYPE timestamp without time zone, ALTER COLUMN ""PaymentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ReceivablePayments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ReceivablePayments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ProfilePermissions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ProfilePermissions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ProfilePermissions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ProfilePermissions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Procedures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Procedures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Procedures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Procedures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ProcedureMaterials' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ProcedureMaterials"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ProcedureMaterials' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ProcedureMaterials"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionSequenceControls' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionSequenceControls"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionSequenceControls' 
                        AND column_name = 'LastGeneratedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionSequenceControls"" ALTER COLUMN ""LastGeneratedAt"" TYPE timestamp without time zone, ALTER COLUMN ""LastGeneratedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionSequenceControls' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionSequenceControls"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionItems' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionItems"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PrescriptionItems' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PrescriptionItems"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PlanoContas' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PlanoContas"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PlanoContas' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PlanoContas"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'ProcessedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""ProcessedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'PaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""PaymentDate"" TYPE timestamp without time zone, ALTER COLUMN ""PaymentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Payments' 
                        AND column_name = 'CancellationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Payments"" ALTER COLUMN ""CancellationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PayablePayments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PayablePayments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PayablePayments' 
                        AND column_name = 'PaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PayablePayments"" ALTER COLUMN ""PaymentDate"" TYPE timestamp without time zone, ALTER COLUMN ""PaymentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PayablePayments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PayablePayments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientTouchpoints' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientTouchpoints"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientTouchpoints' 
                        AND column_name = 'Timestamp'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientTouchpoints"" ALTER COLUMN ""Timestamp"" TYPE timestamp without time zone, ALTER COLUMN ""Timestamp"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientTouchpoints' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientTouchpoints"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<Guid>(
                name: "JourneyStageId1",
                schema: "crm",
                table: "PatientTouchpoints",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Patients' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Patients"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "Document",
                table: "Patients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Patients' 
                        AND column_name = 'DateOfBirth'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Patients"" ALTER COLUMN ""DateOfBirth"" TYPE timestamp without time zone, ALTER COLUMN ""DateOfBirth"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Patients' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Patients"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientJourneys' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientJourneys"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientJourneys' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""PatientJourneys"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientHealthInsurances' 
                        AND column_name = 'ValidUntil'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PatientHealthInsurances"" ALTER COLUMN ""ValidUntil"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientHealthInsurances' 
                        AND column_name = 'ValidFrom'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PatientHealthInsurances"" ALTER COLUMN ""ValidFrom"" TYPE timestamp without time zone, ALTER COLUMN ""ValidFrom"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientHealthInsurances' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PatientHealthInsurances"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientHealthInsurances' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PatientHealthInsurances"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientClinicLinks' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PatientClinicLinks"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientClinicLinks' 
                        AND column_name = 'LinkedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PatientClinicLinks"" ALTER COLUMN ""LinkedAt"" TYPE timestamp without time zone, ALTER COLUMN ""LinkedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PatientClinicLinks' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PatientClinicLinks"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'VerifiedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""VerifiedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'UsedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""UsedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'ExpiresAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""ExpiresAt"" TYPE timestamp without time zone, ALTER COLUMN ""ExpiresAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'PasswordResetTokens' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""PasswordResetTokens"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Owners' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Owners"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Owners' 
                        AND column_name = 'LastLoginAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Owners"" ALTER COLUMN ""LastLoginAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Owners' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Owners"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'OwnerClinicLinks' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""OwnerClinicLinks"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'OwnerClinicLinks' 
                        AND column_name = 'LinkedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""OwnerClinicLinks"" ALTER COLUMN ""LinkedDate"" TYPE timestamp without time zone, ALTER COLUMN ""LinkedDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'OwnerClinicLinks' 
                        AND column_name = 'InactivatedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""OwnerClinicLinks"" ALTER COLUMN ""InactivatedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'OwnerClinicLinks' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""OwnerClinicLinks"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'owner_sessions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""owner_sessions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'owner_sessions' 
                        AND column_name = 'LastActivityAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""owner_sessions"" ALTER COLUMN ""LastActivityAt"" TYPE timestamp without time zone, ALTER COLUMN ""LastActivityAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'owner_sessions' 
                        AND column_name = 'ExpiresAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""owner_sessions"" ALTER COLUMN ""ExpiresAt"" TYPE timestamp without time zone, ALTER COLUMN ""ExpiresAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'owner_sessions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE public.""owner_sessions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'SentAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""SentAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'ReadAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""ReadAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'DeliveredAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""DeliveredAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Notifications' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Notifications"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRules' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRules"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRules' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRules"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRoutines' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRoutines"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRoutines' 
                        AND column_name = 'NextExecutionAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRoutines"" ALTER COLUMN ""NextExecutionAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRoutines' 
                        AND column_name = 'LastExecutedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRoutines"" ALTER COLUMN ""LastExecutedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'NotificationRoutines' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""NotificationRoutines"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MonthlyControlledBalances' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MonthlyControlledBalances"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MonthlyControlledBalances' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MonthlyControlledBalances"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MonthlyControlledBalances' 
                        AND column_name = 'ClosedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MonthlyControlledBalances"" ALTER COLUMN ""ClosedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurationHistories' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurationHistories"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurationHistories' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurationHistories"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ModuleConfigurationHistories' 
                        AND column_name = 'ChangedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ModuleConfigurationHistories"" ALTER COLUMN ""ChangedAt"" TYPE timestamp without time zone, ALTER COLUMN ""ChangedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Medications' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Medications"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Medications' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Medications"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordVersions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordVersions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordVersions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordVersions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordVersions' 
                        AND column_name = 'ChangedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordVersions"" ALTER COLUMN ""ChangedAt"" TYPE timestamp without time zone, ALTER COLUMN ""ChangedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordSignatures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordSignatures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordSignatures' 
                        AND column_name = 'SignedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordSignatures"" ALTER COLUMN ""SignedAt"" TYPE timestamp without time zone, ALTER COLUMN ""SignedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordSignatures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordSignatures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'ReopenedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""ReopenedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'ConsultationStartTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""ConsultationStartTime"" TYPE timestamp without time zone, ALTER COLUMN ""ConsultationStartTime"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'ConsultationEndTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""ConsultationEndTime"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecords' 
                        AND column_name = 'ClosedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecords"" ALTER COLUMN ""ClosedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordAccessLogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordAccessLogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordAccessLogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordAccessLogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MedicalRecordAccessLogs' 
                        AND column_name = 'AccessedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""MedicalRecordAccessLogs"" ALTER COLUMN ""AccessedAt"" TYPE timestamp without time zone, ALTER COLUMN ""AccessedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Materials' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Materials"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Materials' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Materials"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MarketingAutomations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""MarketingAutomations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MarketingAutomations' 
                        AND column_name = 'LastExecutedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""MarketingAutomations"" ALTER COLUMN ""LastExecutedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'MarketingAutomations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""MarketingAutomations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LoginAttempts' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""LoginAttempts"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LoginAttempts' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""LoginAttempts"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LoginAttempts' 
                        AND column_name = 'AttemptTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""LoginAttempts"" ALTER COLUMN ""AttemptTime"" TYPE timestamp without time zone, ALTER COLUMN ""AttemptTime"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LancamentosContabeis' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""LancamentosContabeis"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LancamentosContabeis' 
                        AND column_name = 'DataLancamento'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""LancamentosContabeis"" ALTER COLUMN ""DataLancamento"" TYPE timestamp without time zone, ALTER COLUMN ""DataLancamento"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'LancamentosContabeis' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""LancamentosContabeis"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'JourneyStages' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""JourneyStages"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'JourneyStages' 
                        AND column_name = 'ExitedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""JourneyStages"" ALTER COLUMN ""ExitedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'JourneyStages' 
                        AND column_name = 'EnteredAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""JourneyStages"" ALTER COLUMN ""EnteredAt"" TYPE timestamp without time zone, ALTER COLUMN ""EnteredAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'JourneyStages' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""JourneyStages"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<Guid>(
                name: "PatientJourneyId2",
                schema: "crm",
                table: "JourneyStages",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'SentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""SentDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'PaidDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""PaidDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'IssueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""IssueDate"" TYPE timestamp without time zone, ALTER COLUMN ""IssueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'DueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""DueDate"" TYPE timestamp without time zone, ALTER COLUMN ""DueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Invoices' 
                        AND column_name = 'CancellationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Invoices"" ALTER COLUMN ""CancellationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InvoiceConfigurations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""InvoiceConfigurations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InvoiceConfigurations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""InvoiceConfigurations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InvoiceConfigurations' 
                        AND column_name = 'CertificateExpirationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""InvoiceConfigurations"" ALTER COLUMN ""CertificateExpirationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InformedConsents' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""InformedConsents"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InformedConsents' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""InformedConsents"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'InformedConsents' 
                        AND column_name = 'AcceptedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""InformedConsents"" ALTER COLUMN ""AcceptedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ImpostosNotas' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ImpostosNotas"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ImpostosNotas' 
                        AND column_name = 'DataCalculo'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ImpostosNotas"" ALTER COLUMN ""DataCalculo"" TYPE timestamp without time zone, ALTER COLUMN ""DataCalculo"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ImpostosNotas' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ImpostosNotas"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsurancePlans' 
                        AND column_name = 'ValidUntil'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsurancePlans"" ALTER COLUMN ""ValidUntil"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsurancePlans' 
                        AND column_name = 'ValidFrom'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsurancePlans"" ALTER COLUMN ""ValidFrom"" TYPE timestamp without time zone, ALTER COLUMN ""ValidFrom"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsurancePlans' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsurancePlans"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsurancePlans' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsurancePlans"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsuranceOperators' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsuranceOperators"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'HealthInsuranceOperators' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""HealthInsuranceOperators"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosures' 
                        AND column_name = 'SettlementDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosures"" ALTER COLUMN ""SettlementDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosures' 
                        AND column_name = 'ClosureDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosures"" ALTER COLUMN ""ClosureDate"" TYPE timestamp without time zone, ALTER COLUMN ""ClosureDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosureItems' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosureItems"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FinancialClosureItems' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""FinancialClosureItems"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FilasEspera' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""FilasEspera"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'FilasEspera' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""FilasEspera"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Expenses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Expenses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Expenses' 
                        AND column_name = 'PaidDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Expenses"" ALTER COLUMN ""PaidDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Expenses' 
                        AND column_name = 'DueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Expenses"" ALTER COLUMN ""DueDate"" TYPE timestamp without time zone, ALTER COLUMN ""DueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Expenses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Expenses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'ScheduledDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""ScheduledDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'RequestedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""RequestedDate"" TYPE timestamp without time zone, ALTER COLUMN ""RequestedDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamRequests' 
                        AND column_name = 'CompletedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ExamRequests"" ALTER COLUMN ""CompletedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamCatalogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ExamCatalogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ExamCatalogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ExamCatalogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'EmailTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""EmailTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'EmailTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""EmailTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'IssueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""IssueDate"" TYPE timestamp without time zone, ALTER COLUMN ""IssueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'CancellationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""CancellationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ElectronicInvoices' 
                        AND column_name = 'AuthorizationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ElectronicInvoices"" ALTER COLUMN ""AuthorizationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'PeriodoInicio'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""PeriodoInicio"" TYPE timestamp without time zone, ALTER COLUMN ""PeriodoInicio"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'PeriodoFim'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""PeriodoFim"" TYPE timestamp without time zone, ALTER COLUMN ""PeriodoFim"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'DataGeracao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""DataGeracao"" TYPE timestamp without time zone, ALTER COLUMN ""DataGeracao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DREs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DREs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'SignedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""SignedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'ReportedToSNGPCAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""ReportedToSNGPCAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'IssuedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""IssuedAt"" TYPE timestamp without time zone, ALTER COLUMN ""IssuedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'ExpiresAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""ExpiresAt"" TYPE timestamp without time zone, ALTER COLUMN ""ExpiresAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptionItems' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptionItems"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptionItems' 
                        AND column_name = 'ManufactureDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptionItems"" ALTER COLUMN ""ManufactureDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptionItems' 
                        AND column_name = 'ExpiryDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptionItems"" ALTER COLUMN ""ExpiryDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DigitalPrescriptionItems' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DigitalPrescriptionItems"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DiagnosticHypotheses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DiagnosticHypotheses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DiagnosticHypotheses' 
                        AND column_name = 'DiagnosedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DiagnosticHypotheses"" ALTER COLUMN ""DiagnosedAt"" TYPE timestamp without time zone, ALTER COLUMN ""DiagnosedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DiagnosticHypotheses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DiagnosticHypotheses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataProcessingConsents' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataProcessingConsents"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataProcessingConsents' 
                        AND column_name = 'RevokedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataProcessingConsents"" ALTER COLUMN ""RevokedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataProcessingConsents' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataProcessingConsents"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataProcessingConsents' 
                        AND column_name = 'ConsentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataProcessingConsents"" ALTER COLUMN ""ConsentDate"" TYPE timestamp without time zone, ALTER COLUMN ""ConsentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'RequestDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""RequestDate"" TYPE timestamp without time zone, ALTER COLUMN ""RequestDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'ProcessedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""ProcessedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'LegalApprovalDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""LegalApprovalDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataDeletionRequests' 
                        AND column_name = 'CompletedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataDeletionRequests"" ALTER COLUMN ""CompletedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'RevokedDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""RevokedDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'ExpirationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""ExpirationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataConsentLogs' 
                        AND column_name = 'ConsentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataConsentLogs"" ALTER COLUMN ""ConsentDate"" TYPE timestamp without time zone, ALTER COLUMN ""ConsentDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataAccessLogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataAccessLogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataAccessLogs' 
                        AND column_name = 'Timestamp'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataAccessLogs"" ALTER COLUMN ""Timestamp"" TYPE timestamp without time zone, ALTER COLUMN ""Timestamp"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DataAccessLogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DataAccessLogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DashboardWidgets' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DashboardWidgets"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'DashboardWidgets' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""DashboardWidgets"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CustomDashboards' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CustomDashboards"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CustomDashboards' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CustomDashboards"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'RegisteredAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""RegisteredAt"" TYPE timestamp without time zone, ALTER COLUMN ""RegisteredAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'DocumentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""DocumentDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'Date'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""Date"" TYPE timestamp without time zone, ALTER COLUMN ""Date"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ControlledMedicationRegistries' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ControlledMedicationRegistries"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultationFormProfiles' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultationFormProfiles"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultationFormProfiles' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultationFormProfiles"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultationFormConfigurations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultationFormConfigurations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultationFormConfigurations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultationFormConfigurations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'UltimaAtualizacao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""UltimaAtualizacao"" TYPE timestamp without time zone, ALTER COLUMN ""UltimaAtualizacao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'DataConsolidacao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""DataConsolidacao"" TYPE timestamp without time zone, ALTER COLUMN ""DataConsolidacao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'Data'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""Data"" TYPE timestamp without time zone, ALTER COLUMN ""Data"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConsultasDiarias' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConsultasDiarias"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConfiguracoesFiscais' 
                        AND column_name = 'VigenciaInicio'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConfiguracoesFiscais"" ALTER COLUMN ""VigenciaInicio"" TYPE timestamp without time zone, ALTER COLUMN ""VigenciaInicio"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConfiguracoesFiscais' 
                        AND column_name = 'VigenciaFim'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConfiguracoesFiscais"" ALTER COLUMN ""VigenciaFim"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConfiguracoesFiscais' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConfiguracoesFiscais"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ConfiguracoesFiscais' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ConfiguracoesFiscais"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'ResolvedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""ResolvedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'ReceivedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""ReceivedAt"" TYPE timestamp without time zone, ALTER COLUMN ""ReceivedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'FirstResponseAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""FirstResponseAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Complaints' 
                        AND column_name = 'ClosedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""Complaints"" ALTER COLUMN ""ClosedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ComplaintInteractions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""ComplaintInteractions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ComplaintInteractions' 
                        AND column_name = 'InteractionDate'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""ComplaintInteractions"" ALTER COLUMN ""InteractionDate"" TYPE timestamp without time zone, ALTER COLUMN ""InteractionDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ComplaintInteractions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""ComplaintInteractions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<Guid>(
                name: "ComplaintId2",
                schema: "crm",
                table: "ComplaintInteractions",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Companies' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Companies"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Companies' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Companies"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicTags' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicTags"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicTags' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicTags"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicTags' 
                        AND column_name = 'AssignedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicTags"" ALTER COLUMN ""AssignedAt"" TYPE timestamp without time zone, ALTER COLUMN ""AssignedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'TrialEndDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""TrialEndDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'StartDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""StartDate"" TYPE timestamp without time zone, ALTER COLUMN ""StartDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'PlanChangeDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""PlanChangeDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'NextPaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""NextPaymentDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'ManualOverrideSetAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""ManualOverrideSetAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'LastPaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""LastPaymentDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'FrozenStartDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""FrozenStartDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'FrozenEndDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""FrozenEndDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'EndDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""EndDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicSubscriptions' 
                        AND column_name = 'CancellationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicSubscriptions"" ALTER COLUMN ""CancellationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Clinics' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Clinics"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Clinics' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Clinics"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicCustomizations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicCustomizations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicCustomizations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicCustomizations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicalExaminations' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicalExaminations"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ClinicalExaminations' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ClinicalExaminations"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ChurnPredictions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""ChurnPredictions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ChurnPredictions' 
                        AND column_name = 'PredictedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""ChurnPredictions"" ALTER COLUMN ""PredictedAt"" TYPE timestamp without time zone, ALTER COLUMN ""PredictedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ChurnPredictions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""ChurnPredictions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'DataExpiracao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""DataExpiracao"" TYPE timestamp without time zone, ALTER COLUMN ""DataExpiracao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'DataEmissao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""DataEmissao"" TYPE timestamp without time zone, ALTER COLUMN ""DataEmissao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'DataCadastro'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""DataCadastro"" TYPE timestamp without time zone, ALTER COLUMN ""DataCadastro"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CertificadosDigitais' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CertificadosDigitais"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CashFlowEntries' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CashFlowEntries"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CashFlowEntries' 
                        AND column_name = 'TransactionDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CashFlowEntries"" ALTER COLUMN ""TransactionDate"" TYPE timestamp without time zone, ALTER COLUMN ""TransactionDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'CashFlowEntries' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""CashFlowEntries"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'BalancosPatrimoniais' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""BalancosPatrimoniais"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'BalancosPatrimoniais' 
                        AND column_name = 'DataReferencia'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""BalancosPatrimoniais"" ALTER COLUMN ""DataReferencia"" TYPE timestamp without time zone, ALTER COLUMN ""DataReferencia"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'BalancosPatrimoniais' 
                        AND column_name = 'DataGeracao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""BalancosPatrimoniais"" ALTER COLUMN ""DataGeracao"" TYPE timestamp without time zone, ALTER COLUMN ""DataGeracao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'BalancosPatrimoniais' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""BalancosPatrimoniais"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AutomationActions' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""AutomationActions"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""UpdatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AutomationActions' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'crm'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE crm.""AutomationActions"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<Guid>(
                name: "MarketingAutomationId1",
                schema: "crm",
                table: "AutomationActions",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'RequestDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""RequestDate"" TYPE timestamp without time zone, ALTER COLUMN ""RequestDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'ExpirationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""ExpirationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuthorizationRequests' 
                        AND column_name = 'AuthorizationDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AuthorizationRequests"" ALTER COLUMN ""AuthorizationDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuditLogs' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AuditLogs"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuditLogs' 
                        AND column_name = 'Timestamp'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AuditLogs"" ALTER COLUMN ""Timestamp"" TYPE timestamp without time zone, ALTER COLUMN ""Timestamp"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuditLogs' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AuditLogs"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'DataUltimaValidacao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""DataUltimaValidacao"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'DataTimestamp'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""DataTimestamp"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'DataHoraAssinatura'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""DataHoraAssinatura"" TYPE timestamp without time zone, ALTER COLUMN ""DataHoraAssinatura"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AssinaturasDigitais' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AssinaturasDigitais"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ApuracoesImpostos' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ApuracoesImpostos"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ApuracoesImpostos' 
                        AND column_name = 'DataPagamento'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ApuracoesImpostos"" ALTER COLUMN ""DataPagamento"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ApuracoesImpostos' 
                        AND column_name = 'DataApuracao'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ApuracoesImpostos"" ALTER COLUMN ""DataApuracao"" TYPE timestamp without time zone, ALTER COLUMN ""DataApuracao"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'ApuracoesImpostos' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""ApuracoesImpostos"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'ScheduledDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""ScheduledDate"" TYPE timestamp without time zone, ALTER COLUMN ""ScheduledDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'PaidAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""PaidAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'CheckOutTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""CheckOutTime"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Appointments' 
                        AND column_name = 'CheckInTime'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""Appointments"" ALTER COLUMN ""CheckInTime"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AppointmentProcedures' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AppointmentProcedures"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AppointmentProcedures' 
                        AND column_name = 'PerformedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AppointmentProcedures"" ALTER COLUMN ""PerformedAt"" TYPE timestamp without time zone, ALTER COLUMN ""PerformedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AppointmentProcedures' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AppointmentProcedures"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisTemplates' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisTemplates"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisTemplates' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisTemplates"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisResponses' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisResponses"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisResponses' 
                        AND column_name = 'ResponseDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisResponses"" ALTER COLUMN ""ResponseDate"" TYPE timestamp without time zone, ALTER COLUMN ""ResponseDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AnamnesisResponses' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AnamnesisResponses"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'SettlementDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""SettlementDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'IssueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""IssueDate"" TYPE timestamp without time zone, ALTER COLUMN ""IssueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'DueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""DueDate"" TYPE timestamp without time zone, ALTER COLUMN ""DueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsReceivable' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsReceivable"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'PaymentDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""PaymentDate"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'IssueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""IssueDate"" TYPE timestamp without time zone, ALTER COLUMN ""IssueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'DueDate'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""DueDate"" TYPE timestamp without time zone, ALTER COLUMN ""DueDate"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountsPayable' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountsPayable"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'UnlocksAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""UnlocksAt"" TYPE timestamp without time zone, ALTER COLUMN ""UnlocksAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'UnlockedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""UnlockedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'LockedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""LockedAt"" TYPE timestamp without time zone, ALTER COLUMN ""LockedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccountLockouts' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccountLockouts"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccessProfiles' 
                        AND column_name = 'UpdatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccessProfiles"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AccessProfiles' 
                        AND column_name = 'CreatedAt'
                        AND table_schema = 'public'
                        AND data_type = 'timestamp with time zone'
                    ) THEN
                        ALTER TABLE ""AccessProfiles"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone, ALTER COLUMN ""CreatedAt"" SET NOT NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.InsertData(
                table: "ReportTemplates",
                columns: new[] { "Id", "Category", "Configuration", "CreatedAt", "Description", "Icon", "IsSystem", "Name", "Query", "SupportedFormats", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("013e9e43-ddda-48e1-9153-83e25501ae02"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(7138), "Detailed analysis of appointment scheduling, cancellations, and no-shows", "event_note", true, "Appointment Analytics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(CASE WHEN a.\"Status\" = 'Completed' THEN 1 END) as completed,\n    COUNT(CASE WHEN a.\"Status\" = 'Cancelled' THEN 1 END) as cancelled,\n    COUNT(CASE WHEN a.\"Status\" = 'NoShow' THEN 1 END) as no_shows\nFROM \"Appointments\" a\nINNER JOIN \"Clinics\" c ON a.\"ClinicId\" = c.\"Id\"\nWHERE a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nGROUP BY c.\"TradeName\"\nORDER BY total_appointments DESC", "pdf,excel", "", null },
                    { new Guid("18e75c2e-332b-4af5-9af4-5e29f394235d"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(6828), "Detailed breakdown of revenue by plans, clinics, and payment methods", "pie_chart", true, "Revenue Breakdown Report", "\nSELECT \n    p.\"Name\" as plan_name,\n    COUNT(cs.\"Id\") as subscription_count,\n    SUM(p.\"MonthlyPrice\") as total_mrr,\n    AVG(p.\"MonthlyPrice\") as avg_price\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE DATE_TRUNC('month', cs.\"CreatedAt\") = DATE_TRUNC('month', @month::date)\n    AND cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"\nORDER BY total_mrr DESC", "pdf,excel", "", null },
                    { new Guid("31748a32-9a83-4648-ae86-f61e4f8c229c"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(7524), "Analysis of subscription lifecycle from acquisition to churn", "loop", true, "Subscription Lifecycle Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    p.\"Name\" as plan,\n    cs.\"StartDate\" as subscription_start,\n    cs.\"EndDate\" as subscription_end,\n    cs.\"Status\" as status,\n    p.\"MonthlyPrice\" as monthly_price,\n    EXTRACT(MONTH FROM AGE(COALESCE(cs.\"EndDate\", CURRENT_DATE), cs.\"StartDate\")) as months_active\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"StartDate\" >= @startDate AND cs.\"StartDate\" <= @endDate\nORDER BY cs.\"StartDate\" DESC", "pdf,excel", "", null },
                    { new Guid("53a9efad-3ba5-4118-9d12-176aeba9e901"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(6998), "Comprehensive churn analysis with reasons and trends", "exit_to_app", true, "Customer Churn Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"EndDate\") as month,\n    COUNT(cs.\"Id\") as churned_subscriptions,\n    SUM(p.\"MonthlyPrice\") as lost_mrr,\n    c.\"Name\" as clinic_name,\n    cs.\"Status\" as status\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nINNER JOIN \"Clinics\" c ON cs.\"ClinicId\" = c.\"Id\"\nWHERE cs.\"EndDate\" >= @startDate AND cs.\"EndDate\" <= @endDate\n    AND cs.\"Status\" = 'Cancelled'\nGROUP BY DATE_TRUNC('month', cs.\"EndDate\"), c.\"Name\", cs.\"Status\"\nORDER BY month DESC", "pdf,excel", "", null },
                    { new Guid("645018e4-0bc9-4d02-85bb-b2f60232ed5d"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(7250), "Overview of user activity, logins, and engagement metrics", "analytics", true, "User Activity Report", "\nSELECT \n    u.\"UserName\" as username,\n    u.\"Email\" as email,\n    c.\"TradeName\" as clinic,\n    COUNT(us.\"Id\") as login_count,\n    MAX(us.\"LastActivityAt\") as last_activity\nFROM \"Users\" u\nLEFT JOIN \"UserSessions\" us ON u.\"Id\" = us.\"UserId\" \n    AND us.\"CreatedAt\" >= @startDate AND us.\"CreatedAt\" <= @endDate\nLEFT JOIN \"Clinics\" c ON u.\"ClinicId\" = c.\"Id\"\nWHERE u.\"IsActive\" = true\nGROUP BY u.\"UserName\", u.\"Email\", c.\"TradeName\"\nORDER BY login_count DESC", "pdf,excel,csv", "", null },
                    { new Guid("740e044c-c0dd-4f89-b4c0-27fbb3a44254"), "clinical", "{\"parameters\":[{\"name\":\"clinicId\",\"type\":\"guid\",\"required\":false,\"label\":\"Clinic (Optional)\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(7327), "Statistical analysis of patient demographics and distribution", "people", true, "Patient Demographics Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as total_patients,\n    COUNT(CASE WHEN p.\"Gender\" = 'Male' THEN 1 END) as male_count,\n    COUNT(CASE WHEN p.\"Gender\" = 'Female' THEN 1 END) as female_count,\n    AVG(EXTRACT(YEAR FROM AGE(p.\"BirthDate\"))) as average_age\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nWHERE (@clinicId IS NULL OR p.\"ClinicId\" = @clinicId)\nGROUP BY c.\"TradeName\"\nORDER BY total_patients DESC", "pdf,excel", "", null },
                    { new Guid("83dc5fe1-7984-4fb4-a45d-581f50e1a921"), "financial", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}],\"sections\":[{\"title\":\"Revenue Overview\",\"type\":\"summary\"},{\"title\":\"MRR Trend\",\"type\":\"chart\",\"chartType\":\"line\"},{\"title\":\"Revenue by Plan\",\"type\":\"chart\",\"chartType\":\"pie\"},{\"title\":\"Top Customers\",\"type\":\"table\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(6438), "Comprehensive financial performance report including MRR, revenue, and growth metrics", "assessment", true, "Financial Summary Report", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    COUNT(DISTINCT cs.\"ClinicId\") as customer_count,\n    SUM(p.\"MonthlyPrice\") as mrr,\n    SUM(p.\"MonthlyPrice\" * 12) as arr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("b32c7a68-5fae-4f05-8534-f99a16b228b3"), "customer", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(6912), "Analysis of new customer acquisition trends and conversion metrics", "person_add", true, "Customer Acquisition Report", "\nSELECT \n    DATE_TRUNC('month', c.\"CreatedAt\") as month,\n    COUNT(c.\"Id\") as new_customers,\n    COUNT(DISTINCT u.\"Id\") as new_users,\n    COUNT(cs.\"Id\") as new_subscriptions\nFROM \"Clinics\" c\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"CreatedAt\" >= @startDate AND u.\"CreatedAt\" <= @endDate\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"CreatedAt\" >= @startDate AND cs.\"CreatedAt\" <= @endDate\nWHERE c.\"CreatedAt\" >= @startDate AND c.\"CreatedAt\" <= @endDate\nGROUP BY DATE_TRUNC('month', c.\"CreatedAt\")\nORDER BY month", "pdf,excel,csv", "", null },
                    { new Guid("ea8fa21e-885e-4d6c-8896-66cfebc6f3e8"), "operational", "{\"parameters\":[{\"name\":\"startDate\",\"type\":\"date\",\"required\":true,\"label\":\"Start Date\"},{\"name\":\"endDate\",\"type\":\"date\",\"required\":true,\"label\":\"End Date\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(7397), "Overview of system health, errors, and performance metrics", "health_and_safety", true, "System Health Report", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(cs.\"Id\") as active_subscriptions,\n    COUNT(u.\"Id\") as active_users,\n    COUNT(a.\"Id\") as total_appointments,\n    COUNT(p.\"Id\") as total_patients\nFROM \"Clinics\" c\nLEFT JOIN \"ClinicSubscriptions\" cs ON c.\"Id\" = cs.\"ClinicId\" AND cs.\"Status\" = 'Active'\nLEFT JOIN \"Users\" u ON c.\"Id\" = u.\"ClinicId\" AND u.\"IsActive\" = true\nLEFT JOIN \"Appointments\" a ON c.\"Id\" = a.\"ClinicId\" AND a.\"AppointmentDate\" >= @startDate AND a.\"AppointmentDate\" <= @endDate\nLEFT JOIN \"Patients\" p ON c.\"Id\" = p.\"ClinicId\"\nWHERE c.\"IsActive\" = true\nGROUP BY c.\"TradeName\"\nORDER BY active_subscriptions DESC", "pdf,excel", "", null },
                    { new Guid("f99c3a25-1049-42d2-a352-92273929a447"), "financial", "{\"parameters\":[{\"name\":\"month\",\"type\":\"month\",\"required\":true,\"label\":\"Report Month\"}],\"sections\":[{\"title\":\"Financial KPIs\",\"type\":\"metrics\"},{\"title\":\"Customer Metrics\",\"type\":\"metrics\"},{\"title\":\"Growth Trends\",\"type\":\"chart\"},{\"title\":\"Top Performers\",\"type\":\"table\"}]}", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(7608), "High-level executive summary with key metrics and trends", "dashboard", true, "Executive Dashboard Report", "\nWITH monthly_stats AS (\n    SELECT \n        COUNT(DISTINCT cs.\"ClinicId\") as total_customers,\n        SUM(p.\"MonthlyPrice\") as total_mrr,\n        COUNT(CASE WHEN cs.\"Status\" = 'Cancelled' THEN 1 END) as churned_customers\n    FROM \"ClinicSubscriptions\" cs\n    INNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\n    WHERE DATE_TRUNC('month', cs.\"CreatedAt\") <= DATE_TRUNC('month', @month::date)\n)\nSELECT * FROM monthly_stats", "pdf", "", null }
                });

            migrationBuilder.InsertData(
                table: "WidgetTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultConfig", "DefaultQuery", "Description", "Icon", "IsSystem", "Name", "TenantId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("02c0c0ac-fed4-45b2-8c10-883704ec2f4a"), "operational", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5795), "{\"labelField\":\"status\",\"valueField\":\"count\"}", "\nSELECT \n    \"Status\" as status,\n    COUNT(*) as count\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'\nGROUP BY \"Status\"", "Distribution of appointments by status", "pie_chart", true, "Appointments by Status", "", "pie", null },
                    { new Guid("2cd6edbc-9746-48dd-ae79-07a4f8d14001"), "clinical", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5992), "{\"format\":\"number\",\"icon\":\"local_hospital\",\"color\":\"#f97316\"}", "\nSELECT COUNT(*) as value\nFROM \"Patients\"", "Total number of registered patients", "local_hospital", true, "Total Patients", "", "metric", null },
                    { new Guid("4018dfbc-aaf3-4e44-988a-5914b435f5d4"), "customer", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5418), "{\"format\":\"number\",\"icon\":\"people\",\"color\":\"#3b82f6\"}", "\nSELECT COUNT(DISTINCT \"ClinicId\") as value\nFROM \"ClinicSubscriptions\"\nWHERE \"Status\" = 'Active'", "Total number of active clinic customers", "people", true, "Active Customers", "", "metric", null },
                    { new Guid("4ac26e3c-0fef-489d-8ff7-62722154678c"), "operational", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5875), "{\"format\":\"number\",\"icon\":\"person\",\"color\":\"#06b6d4\"}", "\nSELECT COUNT(*) as value\nFROM \"Users\"\nWHERE \"IsActive\" = true", "Number of active users in the system", "person", true, "Active Users", "", "metric", null },
                    { new Guid("5ab6dbe7-71d8-428f-879e-7287c2ffea47"), "customer", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5566), "{\"format\":\"percent\",\"icon\":\"warning\",\"color\":\"#ef4444\",\"threshold\":{\"warning\":5,\"critical\":10}}", "\nSELECT \n    ROUND(\n        CAST(COUNT(CASE WHEN \"Status\" = 'Cancelled' AND \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / \n        NULLIF(COUNT(CASE WHEN \"EndDate\" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,\n        2\n    ) as value\nFROM \"ClinicSubscriptions\"", "Monthly customer churn percentage", "warning", true, "Churn Rate", "", "metric", null },
                    { new Guid("71371967-8b17-4110-8540-1d2b7260f913"), "operational", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5715), "{\"format\":\"number\",\"icon\":\"event\",\"color\":\"#8b5cf6\"}", "\nSELECT COUNT(*) as value\nFROM \"Appointments\"\nWHERE \"AppointmentDate\" >= CURRENT_DATE - INTERVAL '30 days'", "Total appointments scheduled", "event", true, "Total Appointments", "", "metric", null },
                    { new Guid("72fef700-4032-43a4-a96f-7cce4a84e772"), "financial", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5318), "{\"format\":\"currency\",\"icon\":\"attach_money\",\"color\":\"#10b981\"}", "\nSELECT SUM(p.\"MonthlyPrice\") as value\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'", "Current Monthly Recurring Revenue", "attach_money", true, "Total MRR", "", "metric", null },
                    { new Guid("7a509af0-0d2b-4e70-aeb3-3732abd47843"), "customer", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5462), "{\"xAxis\":\"month\",\"yAxis\":\"new_customers\",\"color\":\"#3b82f6\"}", "\nSELECT \n    DATE_TRUNC('month', \"CreatedAt\") as month,\n    COUNT(DISTINCT \"ClinicId\") as new_customers\nFROM \"ClinicSubscriptions\"\nWHERE \"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\nGROUP BY DATE_TRUNC('month', \"CreatedAt\")\nORDER BY month", "New customers acquired each month", "trending_up", true, "Customer Growth", "", "bar", null },
                    { new Guid("cbb5e96a-caa7-4842-bb57-398964f79674"), "clinical", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(6152), "{\"xAxis\":\"clinic\",\"yAxis\":\"patient_count\",\"color\":\"#f97316\"}", "\nSELECT \n    c.\"TradeName\" as clinic,\n    COUNT(p.\"Id\") as patient_count\nFROM \"Patients\" p\nINNER JOIN \"Clinics\" c ON p.\"ClinicId\" = c.\"Id\"\nGROUP BY c.\"TradeName\"\nORDER BY patient_count DESC\nLIMIT 10", "Patient distribution across clinics", "bar_chart", true, "Patients by Clinic", "", "bar", null },
                    { new Guid("e63e701e-c3ba-4b16-8c4c-245013e86c50"), "financial", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(5235), "{\"labelField\":\"plan\",\"valueField\":\"revenue\",\"format\":\"currency\"}", "\nSELECT \n    p.\"Name\" as plan,\n    SUM(p.\"MonthlyPrice\") as revenue\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"Status\" = 'Active'\nGROUP BY p.\"Name\"", "MRR distribution by plan type", "pie_chart", true, "Revenue Breakdown", "", "pie", null },
                    { new Guid("ea3dfe4a-cf69-4671-920a-caad64629cd8"), "financial", new DateTime(2026, 1, 30, 2, 23, 14, 89, DateTimeKind.Utc).AddTicks(4726), "{\"xAxis\":\"month\",\"yAxis\":\"total_mrr\",\"color\":\"#10b981\",\"format\":\"currency\"}", "\nSELECT \n    DATE_TRUNC('month', cs.\"CreatedAt\") as month,\n    SUM(p.\"MonthlyPrice\") as total_mrr\nFROM \"ClinicSubscriptions\" cs\nINNER JOIN \"SubscriptionPlans\" p ON cs.\"SubscriptionPlanId\" = p.\"Id\"\nWHERE cs.\"CreatedAt\" >= CURRENT_DATE - INTERVAL '12 months'\n    AND cs.\"Status\" = 'Active'\nGROUP BY DATE_TRUNC('month', cs.\"CreatedAt\")\nORDER BY month", "Monthly Recurring Revenue trend over the last 12 months", "trending_up", true, "MRR Over Time", "", "line", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponses_SurveyId1",
                schema: "crm",
                table: "SurveyResponses",
                column: "SurveyId1");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestions_SurveyId2",
                schema: "crm",
                table: "SurveyQuestions",
                column: "SurveyId2");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestionResponses_SurveyResponseId1",
                schema: "crm",
                table: "SurveyQuestionResponses",
                column: "SurveyResponseId1");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTouchpoints_JourneyStageId1",
                schema: "crm",
                table: "PatientTouchpoints",
                column: "JourneyStageId1");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyStages_PatientJourneyId2",
                schema: "crm",
                table: "JourneyStages",
                column: "PatientJourneyId2");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintInteractions_ComplaintId2",
                schema: "crm",
                table: "ComplaintInteractions",
                column: "ComplaintId2");

            migrationBuilder.CreateIndex(
                name: "IX_AutomationActions_MarketingAutomationId1",
                schema: "crm",
                table: "AutomationActions",
                column: "MarketingAutomationId1");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Severity",
                table: "AuditLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId",
                table: "AuditLogs",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationActions_MarketingAutomations_MarketingAutomationI~",
                schema: "crm",
                table: "AutomationActions",
                column: "MarketingAutomationId1",
                principalSchema: "crm",
                principalTable: "MarketingAutomations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintInteractions_Complaints_ComplaintId2",
                schema: "crm",
                table: "ComplaintInteractions",
                column: "ComplaintId2",
                principalSchema: "crm",
                principalTable: "Complaints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyStages_PatientJourneys_PatientJourneyId2",
                schema: "crm",
                table: "JourneyStages",
                column: "PatientJourneyId2",
                principalSchema: "crm",
                principalTable: "PatientJourneys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTouchpoints_JourneyStages_JourneyStageId1",
                schema: "crm",
                table: "PatientTouchpoints",
                column: "JourneyStageId1",
                principalSchema: "crm",
                principalTable: "JourneyStages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyQuestionResponses_SurveyResponses_SurveyResponseId1",
                schema: "crm",
                table: "SurveyQuestionResponses",
                column: "SurveyResponseId1",
                principalSchema: "crm",
                principalTable: "SurveyResponses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyQuestions_Surveys_SurveyId2",
                schema: "crm",
                table: "SurveyQuestions",
                column: "SurveyId2",
                principalSchema: "crm",
                principalTable: "Surveys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResponses_Surveys_SurveyId1",
                schema: "crm",
                table: "SurveyResponses",
                column: "SurveyId1",
                principalSchema: "crm",
                principalTable: "Surveys",
                principalColumn: "Id");
        }
    }
}
