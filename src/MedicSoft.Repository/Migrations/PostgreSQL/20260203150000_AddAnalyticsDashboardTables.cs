using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddAnalyticsDashboardTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create CustomDashboards table only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'CustomDashboards' AND table_schema = 'public') THEN
                        CREATE TABLE ""CustomDashboards"" (
                            ""Id"" uuid NOT NULL,
                            ""Name"" character varying(200) NOT NULL,
                            ""Description"" character varying(1000),
                            ""Layout"" TEXT,
                            ""IsDefault"" boolean NOT NULL DEFAULT false,
                            ""IsPublic"" boolean NOT NULL DEFAULT false,
                            ""CreatedBy"" character varying(450) NOT NULL,
                            ""CreatedAt"" timestamp without time zone NOT NULL,
                            ""UpdatedAt"" timestamp without time zone,
                            CONSTRAINT ""PK_CustomDashboards"" PRIMARY KEY (""Id"")
                        );
                        
                        CREATE INDEX ""IX_CustomDashboards_CreatedBy"" ON ""CustomDashboards"" (""CreatedBy"");
                        CREATE INDEX ""IX_CustomDashboards_IsDefault"" ON ""CustomDashboards"" (""IsDefault"");
                        CREATE INDEX ""IX_CustomDashboards_IsPublic"" ON ""CustomDashboards"" (""IsPublic"");
                    END IF;
                END $$;
            ");

            // Create DashboardWidgets table only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'DashboardWidgets' AND table_schema = 'public') THEN
                        CREATE TABLE ""DashboardWidgets"" (
                            ""Id"" uuid NOT NULL,
                            ""DashboardId"" uuid NOT NULL,
                            ""Type"" character varying(50) NOT NULL,
                            ""Title"" character varying(200) NOT NULL,
                            ""Config"" TEXT NOT NULL,
                            ""Query"" TEXT,
                            ""RefreshInterval"" integer NOT NULL DEFAULT 0,
                            ""GridX"" integer NOT NULL DEFAULT 0,
                            ""GridY"" integer NOT NULL DEFAULT 0,
                            ""GridWidth"" integer NOT NULL DEFAULT 4,
                            ""GridHeight"" integer NOT NULL DEFAULT 3,
                            ""CreatedAt"" timestamp without time zone NOT NULL,
                            ""UpdatedAt"" timestamp without time zone,
                            CONSTRAINT ""PK_DashboardWidgets"" PRIMARY KEY (""Id"")
                        );
                        
                        CREATE INDEX ""IX_DashboardWidgets_DashboardId"" ON ""DashboardWidgets"" (""DashboardId"");
                        CREATE INDEX ""IX_DashboardWidgets_Type"" ON ""DashboardWidgets"" (""Type"");
                    END IF;
                END $$;
            ");
            
            // Add FK constraint separately after both tables exist, only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_constraint 
                        WHERE conname = 'FK_DashboardWidgets_CustomDashboards_DashboardId'
                    ) THEN
                        ALTER TABLE ""DashboardWidgets"" 
                        ADD CONSTRAINT ""FK_DashboardWidgets_CustomDashboards_DashboardId"" 
                        FOREIGN KEY (""DashboardId"") 
                        REFERENCES ""CustomDashboards"" (""Id"") ON DELETE CASCADE;
                    END IF;
                END $$;
            ");

            // Create WidgetTemplates table only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'WidgetTemplates' AND table_schema = 'public') THEN
                        CREATE TABLE ""WidgetTemplates"" (
                            ""Id"" uuid NOT NULL,
                            ""Name"" character varying(200) NOT NULL,
                            ""Description"" character varying(1000),
                            ""Category"" character varying(50) NOT NULL,
                            ""Type"" character varying(50) NOT NULL,
                            ""DefaultConfig"" TEXT,
                            ""DefaultQuery"" TEXT,
                            ""IsSystem"" boolean NOT NULL DEFAULT false,
                            ""Icon"" character varying(50),
                            ""CreatedAt"" timestamp without time zone NOT NULL,
                            ""UpdatedAt"" timestamp without time zone,
                            CONSTRAINT ""PK_WidgetTemplates"" PRIMARY KEY (""Id"")
                        );
                        
                        CREATE INDEX ""IX_WidgetTemplates_Category"" ON ""WidgetTemplates"" (""Category"");
                        CREATE INDEX ""IX_WidgetTemplates_Type"" ON ""WidgetTemplates"" (""Type"");
                        CREATE INDEX ""IX_WidgetTemplates_IsSystem"" ON ""WidgetTemplates"" (""IsSystem"");
                    END IF;
                END $$;
            ");

            // Create ReportTemplates table only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'ReportTemplates' AND table_schema = 'public') THEN
                        CREATE TABLE ""ReportTemplates"" (
                            ""Id"" uuid NOT NULL,
                            ""Name"" character varying(200) NOT NULL,
                            ""Description"" character varying(1000),
                            ""Category"" character varying(50) NOT NULL,
                            ""Configuration"" TEXT,
                            ""Query"" TEXT,
                            ""IsSystem"" boolean NOT NULL DEFAULT false,
                            ""Icon"" character varying(50),
                            ""SupportedFormats"" character varying(100),
                            ""CreatedAt"" timestamp without time zone NOT NULL,
                            ""UpdatedAt"" timestamp without time zone,
                            CONSTRAINT ""PK_ReportTemplates"" PRIMARY KEY (""Id"")
                        );
                        
                        CREATE INDEX ""IX_ReportTemplates_Category"" ON ""ReportTemplates"" (""Category"");
                        CREATE INDEX ""IX_ReportTemplates_IsSystem"" ON ""ReportTemplates"" (""IsSystem"");
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop tables only if they exist
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS ""DashboardWidgets"";
                DROP TABLE IF EXISTS ""WidgetTemplates"";
                DROP TABLE IF EXISTS ""ReportTemplates"";
                DROP TABLE IF EXISTS ""CustomDashboards"";
            ");
        }
    }
}
