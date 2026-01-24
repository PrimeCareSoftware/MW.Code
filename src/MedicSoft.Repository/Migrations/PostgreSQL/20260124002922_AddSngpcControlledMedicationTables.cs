using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddSngpcControlledMedicationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent migration using CREATE TABLE/INDEX IF NOT EXISTS
            // This allows the migration to be safely re-run even if tables already exist
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""ControlledMedicationRegistries"" (
    ""Id"" uuid NOT NULL,
    ""Date"" timestamp without time zone NOT NULL,
    ""RegistryType"" integer NOT NULL,
    ""MedicationName"" character varying(200) NOT NULL,
    ""ActiveIngredient"" character varying(200) NOT NULL,
    ""AnvisaList"" character varying(10) NOT NULL,
    ""Concentration"" character varying(50) NOT NULL,
    ""PharmaceuticalForm"" character varying(50) NOT NULL,
    ""QuantityIn"" numeric(18,3) NOT NULL,
    ""QuantityOut"" numeric(18,3) NOT NULL,
    ""Balance"" numeric(18,3) NOT NULL,
    ""DocumentType"" character varying(50) NOT NULL,
    ""DocumentNumber"" character varying(100) NOT NULL,
    ""DocumentDate"" timestamp without time zone,
    ""PrescriptionId"" uuid,
    ""PatientName"" character varying(200),
    ""PatientCPF"" character varying(14),
    ""DoctorName"" character varying(200),
    ""DoctorCRM"" character varying(20),
    ""SupplierName"" character varying(200),
    ""SupplierCNPJ"" character varying(18),
    ""RegisteredByUserId"" uuid NOT NULL,
    ""RegisteredAt"" timestamp without time zone NOT NULL,
    ""CreatedAt"" timestamp without time zone NOT NULL,
    ""UpdatedAt"" timestamp without time zone,
    ""TenantId"" character varying(100) NOT NULL,
    CONSTRAINT ""PK_ControlledMedicationRegistries"" PRIMARY KEY (""Id"")
);

CREATE TABLE IF NOT EXISTS ""MonthlyControlledBalances"" (
    ""Id"" uuid NOT NULL,
    ""Year"" integer NOT NULL,
    ""Month"" integer NOT NULL,
    ""MedicationName"" character varying(200) NOT NULL,
    ""ActiveIngredient"" character varying(200) NOT NULL,
    ""AnvisaList"" character varying(10) NOT NULL,
    ""InitialBalance"" numeric(18,3) NOT NULL,
    ""TotalIn"" numeric(18,3) NOT NULL,
    ""TotalOut"" numeric(18,3) NOT NULL,
    ""CalculatedFinalBalance"" numeric(18,3) NOT NULL,
    ""PhysicalBalance"" numeric(18,3),
    ""Discrepancy"" numeric(18,3),
    ""DiscrepancyReason"" character varying(500),
    ""Status"" integer NOT NULL,
    ""ClosedAt"" timestamp without time zone,
    ""ClosedByUserId"" uuid,
    ""CreatedAt"" timestamp without time zone NOT NULL,
    ""UpdatedAt"" timestamp without time zone,
    ""TenantId"" character varying(100) NOT NULL,
    CONSTRAINT ""PK_MonthlyControlledBalances"" PRIMARY KEY (""Id"")
);

CREATE TABLE IF NOT EXISTS ""SngpcTransmissions"" (
    ""Id"" uuid NOT NULL,
    ""SNGPCReportId"" uuid NOT NULL,
    ""AttemptNumber"" integer NOT NULL,
    ""AttemptedAt"" timestamp without time zone NOT NULL,
    ""Status"" integer NOT NULL,
    ""ProtocolNumber"" character varying(100),
    ""AnvisaResponse"" text,
    ""ErrorMessage"" character varying(1000),
    ""ErrorCode"" character varying(50),
    ""TransmissionMethod"" character varying(50),
    ""EndpointUrl"" character varying(500),
    ""HttpStatusCode"" integer,
    ""ResponseTimeMs"" bigint,
    ""XmlHash"" character varying(64),
    ""XmlSizeBytes"" bigint,
    ""InitiatedByUserId"" uuid,
    ""CreatedAt"" timestamp without time zone NOT NULL,
    ""UpdatedAt"" timestamp without time zone,
    ""TenantId"" character varying(100) NOT NULL,
    CONSTRAINT ""PK_SngpcTransmissions"" PRIMARY KEY (""Id"")
);

-- Add foreign keys only if they don't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'FK_ControlledMedicationRegistries_DigitalPrescriptions_Prescri~'
    ) THEN
        ALTER TABLE ""ControlledMedicationRegistries""
            ADD CONSTRAINT ""FK_ControlledMedicationRegistries_DigitalPrescriptions_Prescri~""
            FOREIGN KEY (""PrescriptionId"") REFERENCES ""DigitalPrescriptions"" (""Id"")
            ON DELETE RESTRICT;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'FK_ControlledMedicationRegistries_Users_RegisteredByUserId'
    ) THEN
        ALTER TABLE ""ControlledMedicationRegistries""
            ADD CONSTRAINT ""FK_ControlledMedicationRegistries_Users_RegisteredByUserId""
            FOREIGN KEY (""RegisteredByUserId"") REFERENCES ""Users"" (""Id"")
            ON DELETE RESTRICT;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'FK_MonthlyControlledBalances_Users_ClosedByUserId'
    ) THEN
        ALTER TABLE ""MonthlyControlledBalances""
            ADD CONSTRAINT ""FK_MonthlyControlledBalances_Users_ClosedByUserId""
            FOREIGN KEY (""ClosedByUserId"") REFERENCES ""Users"" (""Id"")
            ON DELETE RESTRICT;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'FK_SngpcTransmissions_SNGPCReports_SNGPCReportId'
    ) THEN
        ALTER TABLE ""SngpcTransmissions""
            ADD CONSTRAINT ""FK_SngpcTransmissions_SNGPCReports_SNGPCReportId""
            FOREIGN KEY (""SNGPCReportId"") REFERENCES ""SNGPCReports"" (""Id"")
            ON DELETE CASCADE;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'FK_SngpcTransmissions_Users_InitiatedByUserId'
    ) THEN
        ALTER TABLE ""SngpcTransmissions""
            ADD CONSTRAINT ""FK_SngpcTransmissions_Users_InitiatedByUserId""
            FOREIGN KEY (""InitiatedByUserId"") REFERENCES ""Users"" (""Id"")
            ON DELETE RESTRICT;
    END IF;
END $$;

-- Create indexes only if they don't exist
CREATE INDEX IF NOT EXISTS ""IX_ControlledMedicationRegistries_DocumentNumber"" 
    ON ""ControlledMedicationRegistries"" (""DocumentNumber"");

CREATE INDEX IF NOT EXISTS ""IX_ControlledMedicationRegistries_PrescriptionId"" 
    ON ""ControlledMedicationRegistries"" (""PrescriptionId"");

CREATE INDEX IF NOT EXISTS ""IX_ControlledMedicationRegistries_RegisteredByUserId"" 
    ON ""ControlledMedicationRegistries"" (""RegisteredByUserId"");

CREATE INDEX IF NOT EXISTS ""IX_ControlledMedicationRegistries_TenantId_AnvisaList"" 
    ON ""ControlledMedicationRegistries"" (""TenantId"", ""AnvisaList"");

CREATE INDEX IF NOT EXISTS ""IX_ControlledMedicationRegistries_TenantId_Date"" 
    ON ""ControlledMedicationRegistries"" (""TenantId"", ""Date"");

CREATE INDEX IF NOT EXISTS ""IX_ControlledMedicationRegistries_TenantId_MedicationName"" 
    ON ""ControlledMedicationRegistries"" (""TenantId"", ""MedicationName"");

CREATE INDEX IF NOT EXISTS ""IX_ControlledMedicationRegistries_TenantId_PrescriptionId"" 
    ON ""ControlledMedicationRegistries"" (""TenantId"", ""PrescriptionId"");

CREATE INDEX IF NOT EXISTS ""IX_ControlledMedicationRegistries_TenantId_Type_Date"" 
    ON ""ControlledMedicationRegistries"" (""TenantId"", ""RegistryType"", ""Date"");

CREATE INDEX IF NOT EXISTS ""IX_MonthlyControlledBalances_ClosedByUserId"" 
    ON ""MonthlyControlledBalances"" (""ClosedByUserId"");

CREATE INDEX IF NOT EXISTS ""IX_MonthlyControlledBalances_TenantId_Medication_Year"" 
    ON ""MonthlyControlledBalances"" (""TenantId"", ""MedicationName"", ""Year"");

CREATE INDEX IF NOT EXISTS ""IX_MonthlyControlledBalances_TenantId_Status"" 
    ON ""MonthlyControlledBalances"" (""TenantId"", ""Status"");

CREATE INDEX IF NOT EXISTS ""IX_MonthlyControlledBalances_TenantId_Year_Month"" 
    ON ""MonthlyControlledBalances"" (""TenantId"", ""Year"", ""Month"");

-- Create unique index only if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE indexname = 'IX_MonthlyControlledBalances_TenantId_Year_Month_Medication'
    ) THEN
        CREATE UNIQUE INDEX ""IX_MonthlyControlledBalances_TenantId_Year_Month_Medication"" 
            ON ""MonthlyControlledBalances"" (""TenantId"", ""Year"", ""Month"", ""MedicationName"");
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS ""IX_SngpcTransmissions_InitiatedByUserId"" 
    ON ""SngpcTransmissions"" (""InitiatedByUserId"");

CREATE INDEX IF NOT EXISTS ""IX_SngpcTransmissions_ProtocolNumber"" 
    ON ""SngpcTransmissions"" (""ProtocolNumber"");

CREATE INDEX IF NOT EXISTS ""IX_SngpcTransmissions_ReportId_Status"" 
    ON ""SngpcTransmissions"" (""SNGPCReportId"", ""Status"");

CREATE INDEX IF NOT EXISTS ""IX_SngpcTransmissions_TenantId_AttemptedAt"" 
    ON ""SngpcTransmissions"" (""TenantId"", ""AttemptedAt"");

CREATE INDEX IF NOT EXISTS ""IX_SngpcTransmissions_TenantId_ReportId_Attempt"" 
    ON ""SngpcTransmissions"" (""TenantId"", ""SNGPCReportId"", ""AttemptNumber"");

CREATE INDEX IF NOT EXISTS ""IX_SngpcTransmissions_TenantId_Status"" 
    ON ""SngpcTransmissions"" (""TenantId"", ""Status"");
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Idempotent rollback using DROP TABLE IF EXISTS
            migrationBuilder.Sql(@"
DROP TABLE IF EXISTS ""ControlledMedicationRegistries"" CASCADE;
DROP TABLE IF EXISTS ""MonthlyControlledBalances"" CASCADE;
DROP TABLE IF EXISTS ""SngpcTransmissions"" CASCADE;
");
        }
    }
}
