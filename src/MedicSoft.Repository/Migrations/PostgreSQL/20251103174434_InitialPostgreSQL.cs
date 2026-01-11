using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent migration using CREATE TABLE/INDEX IF NOT EXISTS
            // This allows the migration to be safely re-run even if tables already exist
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""Clinics"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(200) NOT NULL,
        ""TradeName"" character varying(200) NOT NULL,
        ""Document"" character varying(50) NOT NULL,
        ""Phone"" character varying(30) NOT NULL,
        ""Email"" character varying(250) NOT NULL,
        ""Address"" character varying(500) NOT NULL,
        ""OpeningTime"" interval NOT NULL,
        ""ClosingTime"" interval NOT NULL,
        ""AppointmentDurationMinutes"" integer NOT NULL,
        ""AllowEmergencySlots"" boolean NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Clinics"" PRIMARY KEY (""Id"")
    );

CREATE TABLE IF NOT EXISTS ""Materials"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(200) NOT NULL,
        ""Code"" character varying(50) NOT NULL,
        ""Description"" character varying(1000) NOT NULL,
        ""Unit"" character varying(50) NOT NULL,
        ""UnitPrice"" numeric(18,2) NOT NULL,
        ""StockQuantity"" integer NOT NULL,
        ""MinimumStock"" integer NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Materials"" PRIMARY KEY (""Id"")
    );

CREATE TABLE IF NOT EXISTS ""MedicalRecordTemplates"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(200) NOT NULL,
        ""Description"" character varying(1000) NOT NULL,
        ""TemplateContent"" character varying(10000) NOT NULL,
        ""Category"" character varying(100) NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_MedicalRecordTemplates"" PRIMARY KEY (""Id"")
    );

CREATE TABLE IF NOT EXISTS ""Medications"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(300) NOT NULL,
        ""GenericName"" character varying(300),
        ""Manufacturer"" character varying(200),
        ""ActiveIngredient"" character varying(300),
        ""Dosage"" character varying(100) NOT NULL,
        ""PharmaceuticalForm"" character varying(100) NOT NULL,
        ""Concentration"" character varying(100),
        ""AdministrationRoute"" character varying(100),
        ""Category"" character varying(50) NOT NULL,
        ""RequiresPrescription"" boolean NOT NULL,
        ""IsControlled"" boolean NOT NULL,
        ""AnvisaRegistration"" character varying(50),
        ""Barcode"" character varying(50),
        ""Description"" character varying(1000),
        ""IsActive"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Medications"" PRIMARY KEY (""Id"")
    );

CREATE TABLE IF NOT EXISTS ""NotificationRoutines"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(200) NOT NULL,
        ""Description"" character varying(1000) NOT NULL,
        ""Channel"" character varying(50) NOT NULL,
        ""Type"" character varying(50) NOT NULL,
        ""MessageTemplate"" character varying(5000) NOT NULL,
        ""ScheduleType"" character varying(50) NOT NULL,
        ""ScheduleConfiguration"" character varying(2000) NOT NULL,
        ""Scope"" character varying(50) NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""MaxRetries"" integer NOT NULL,
        ""RecipientFilter"" character varying(2000),
        ""LastExecutedAt"" timestamp with time zone,
        ""NextExecutionAt"" timestamp with time zone,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_NotificationRoutines"" PRIMARY KEY (""Id"")
    );

CREATE TABLE IF NOT EXISTS ""Patients"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(200) NOT NULL,
        ""Document"" character varying(50) NOT NULL,
        ""DateOfBirth"" timestamp with time zone NOT NULL,
        ""Gender"" character varying(20) NOT NULL,
        ""Email"" character varying(250) NOT NULL,
        ""PhoneCountryCode"" character varying(10) NOT NULL,
        ""PhoneNumber"" character varying(20) NOT NULL,
        ""AddressStreet"" character varying(200) NOT NULL,
        ""AddressNumber"" character varying(20) NOT NULL,
        ""AddressComplement"" character varying(100),
        ""AddressNeighborhood"" character varying(100) NOT NULL,
        ""AddressCity"" character varying(100) NOT NULL,
        ""AddressState"" character varying(50) NOT NULL,
        ""AddressZipCode"" character varying(20) NOT NULL,
        ""AddressCountry"" character varying(50) NOT NULL,
        ""MedicalHistory"" character varying(2000),
        ""Allergies"" character varying(1000),
        ""IsActive"" boolean NOT NULL,
        ""GuardianId"" uuid,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Patients"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_Patients_Patients_GuardianId"" FOREIGN KEY (""GuardianId"") REFERENCES ""Patients"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""PrescriptionTemplates"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(200) NOT NULL,
        ""Description"" character varying(1000) NOT NULL,
        ""TemplateContent"" character varying(10000) NOT NULL,
        ""Category"" character varying(100) NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_PrescriptionTemplates"" PRIMARY KEY (""Id"")
    );

CREATE TABLE IF NOT EXISTS ""Procedures"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(200) NOT NULL,
        ""Code"" character varying(50) NOT NULL,
        ""Description"" character varying(1000) NOT NULL,
        ""Category"" integer NOT NULL,
        ""Price"" numeric(18,2) NOT NULL,
        ""DurationMinutes"" integer NOT NULL,
        ""RequiresMaterials"" boolean NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Procedures"" PRIMARY KEY (""Id"")
    );

CREATE TABLE IF NOT EXISTS ""SubscriptionPlans"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(100) NOT NULL,
        ""Description"" character varying(500) NOT NULL,
        ""MonthlyPrice"" numeric(18,2) NOT NULL,
        ""TrialDays"" integer NOT NULL,
        ""MaxUsers"" integer NOT NULL,
        ""MaxPatients"" integer NOT NULL,
        ""HasReports"" boolean NOT NULL,
        ""HasWhatsAppIntegration"" boolean NOT NULL,
        ""HasSMSNotifications"" boolean NOT NULL,
        ""HasTissExport"" boolean NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""Type"" integer NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_SubscriptionPlans"" PRIMARY KEY (""Id"")
    );

CREATE TABLE IF NOT EXISTS ""Expenses"" (
        ""Id"" uuid NOT NULL,
        ""ClinicId"" uuid NOT NULL,
        ""Description"" character varying(500) NOT NULL,
        ""Category"" integer NOT NULL,
        ""Amount"" numeric(18,2) NOT NULL,
        ""DueDate"" timestamp with time zone NOT NULL,
        ""PaidDate"" timestamp with time zone,
        ""Status"" integer NOT NULL,
        ""PaymentMethod"" integer,
        ""PaymentReference"" character varying(200),
        ""SupplierName"" character varying(200),
        ""SupplierDocument"" character varying(20),
        ""Notes"" character varying(1000),
        ""CancellationReason"" character varying(500),
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Expenses"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_Expenses_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""ModuleConfigurations"" (
        ""Id"" uuid NOT NULL,
        ""ClinicId"" uuid NOT NULL,
        ""ModuleName"" character varying(100) NOT NULL,
        ""IsEnabled"" boolean NOT NULL,
        ""Configuration"" character varying(2000),
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_ModuleConfigurations"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_ModuleConfigurations_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE CASCADE
    );

CREATE TABLE IF NOT EXISTS ""Owners"" (
        ""Id"" uuid NOT NULL,
        ""Username"" character varying(100) NOT NULL,
        ""Email"" character varying(200) NOT NULL,
        ""PasswordHash"" character varying(500) NOT NULL,
        ""FullName"" character varying(200) NOT NULL,
        ""Phone"" character varying(20) NOT NULL,
        ""ClinicId"" uuid,
        ""IsActive"" boolean NOT NULL,
        ""LastLoginAt"" timestamp with time zone,
        ""ProfessionalId"" character varying(50),
        ""Specialty"" character varying(100),
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Owners"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_Owners_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""Users"" (
        ""Id"" uuid NOT NULL,
        ""Username"" character varying(100) NOT NULL,
        ""Email"" character varying(200) NOT NULL,
        ""PasswordHash"" character varying(500) NOT NULL,
        ""FullName"" character varying(200) NOT NULL,
        ""Phone"" character varying(20) NOT NULL,
        ""ClinicId"" uuid,
        ""Role"" integer NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""LastLoginAt"" timestamp with time zone,
        ""ProfessionalId"" character varying(50),
        ""Specialty"" character varying(100),
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Users"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_Users_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""Appointments"" (
        ""Id"" uuid NOT NULL,
        ""PatientId"" uuid NOT NULL,
        ""ClinicId"" uuid NOT NULL,
        ""ScheduledDate"" timestamp with time zone NOT NULL,
        ""ScheduledTime"" interval NOT NULL,
        ""DurationMinutes"" integer NOT NULL,
        ""Type"" integer NOT NULL,
        ""Status"" integer NOT NULL,
        ""Notes"" character varying(1000),
        ""CancellationReason"" character varying(500),
        ""CheckInTime"" timestamp with time zone,
        ""CheckOutTime"" timestamp with time zone,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Appointments"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_Appointments_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_Appointments_Patients_PatientId"" FOREIGN KEY (""PatientId"") REFERENCES ""Patients"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""WaitingQueueConfigurations"" (
        ""Id"" uuid NOT NULL,
        ""ClinicId"" uuid NOT NULL,
        ""DisplayMode"" integer NOT NULL,
        ""ShowEstimatedWaitTime"" boolean NOT NULL,
        ""ShowPatientNames"" boolean NOT NULL,
        ""ShowPriority"" boolean NOT NULL,
        ""ShowPosition"" boolean NOT NULL,
        ""AutoRefreshSeconds"" integer NOT NULL,
        ""EnableSoundNotifications"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_WaitingQueueConfigurations"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_WaitingQueueConfigurations_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""WaitingQueueEntries"" (
        ""Id"" uuid NOT NULL,
        ""AppointmentId"" uuid NOT NULL,
        ""ClinicId"" uuid NOT NULL,
        ""PatientId"" uuid NOT NULL,
        ""Position"" integer NOT NULL,
        ""Priority"" integer NOT NULL,
        ""Status"" integer NOT NULL,
        ""CheckInTime"" timestamp with time zone NOT NULL,
        ""CalledTime"" timestamp with time zone,
        ""CompletedTime"" timestamp with time zone,
        ""TriageNotes"" character varying(1000),
        ""EstimatedWaitTimeMinutes"" integer NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_WaitingQueueEntries"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_WaitingQueueEntries_Appointments_AppointmentId"" FOREIGN KEY (""AppointmentId"") REFERENCES ""Appointments"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_WaitingQueueEntries_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_WaitingQueueEntries_Patients_PatientId"" FOREIGN KEY (""PatientId"") REFERENCES ""Patients"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""HealthInsurancePlans"" (
        ""Id"" uuid NOT NULL,
        ""PatientId"" uuid NOT NULL,
        ""InsuranceName"" character varying(200) NOT NULL,
        ""PlanNumber"" character varying(100) NOT NULL,
        ""PlanType"" character varying(100),
        ""ValidFrom"" timestamp with time zone NOT NULL,
        ""ValidUntil"" timestamp with time zone,
        ""HolderName"" character varying(200),
        ""IsActive"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_HealthInsurancePlans"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_HealthInsurancePlans_Patients_PatientId"" FOREIGN KEY (""PatientId"") REFERENCES ""Patients"" (""Id"") ON DELETE CASCADE
    );

CREATE TABLE IF NOT EXISTS ""PatientClinicLinks"" (
        ""Id"" uuid NOT NULL,
        ""PatientId"" uuid NOT NULL,
        ""ClinicId"" uuid NOT NULL,
        ""LinkedAt"" timestamp with time zone NOT NULL,
        ""IsActive"" boolean NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_PatientClinicLinks"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_PatientClinicLinks_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_PatientClinicLinks_Patients_PatientId"" FOREIGN KEY (""PatientId"") REFERENCES ""Patients"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""ProcedureMaterials"" (
        ""Id"" uuid NOT NULL,
        ""ProcedureId"" uuid NOT NULL,
        ""MaterialId"" uuid NOT NULL,
        ""Quantity"" integer NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_ProcedureMaterials"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_ProcedureMaterials_Materials_MaterialId"" FOREIGN KEY (""MaterialId"") REFERENCES ""Materials"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_ProcedureMaterials_Procedures_ProcedureId"" FOREIGN KEY (""ProcedureId"") REFERENCES ""Procedures"" (""Id"") ON DELETE CASCADE
    );

CREATE TABLE IF NOT EXISTS ""ClinicSubscriptions"" (
        ""Id"" uuid NOT NULL,
        ""ClinicId"" uuid NOT NULL,
        ""SubscriptionPlanId"" uuid NOT NULL,
        ""StartDate"" timestamp with time zone NOT NULL,
        ""EndDate"" timestamp with time zone,
        ""TrialEndDate"" timestamp with time zone,
        ""Status"" integer NOT NULL,
        ""LastPaymentDate"" timestamp with time zone,
        ""NextPaymentDate"" timestamp with time zone,
        ""CurrentPrice"" numeric(18,2) NOT NULL,
        ""CancellationReason"" character varying(500),
        ""CancellationDate"" timestamp with time zone,
        ""IsFrozen"" boolean NOT NULL,
        ""FrozenStartDate"" timestamp with time zone,
        ""FrozenEndDate"" timestamp with time zone,
        ""PendingPlanId"" uuid,
        ""PendingPlanPrice"" numeric(18,2),
        ""PlanChangeDate"" timestamp with time zone,
        ""IsUpgrade"" boolean NOT NULL,
        ""ManualOverrideActive"" boolean NOT NULL DEFAULT FALSE,
        ""ManualOverrideReason"" character varying(500),
        ""ManualOverrideSetAt"" timestamp with time zone,
        ""ManualOverrideSetBy"" character varying(100),
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_ClinicSubscriptions"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_ClinicSubscriptions_Clinics_ClinicId"" FOREIGN KEY (""ClinicId"") REFERENCES ""Clinics"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_ClinicSubscriptions_SubscriptionPlans_PendingPlanId"" FOREIGN KEY (""PendingPlanId"") REFERENCES ""SubscriptionPlans"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_ClinicSubscriptions_SubscriptionPlans_SubscriptionPlanId"" FOREIGN KEY (""SubscriptionPlanId"") REFERENCES ""SubscriptionPlans"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""PasswordResetTokens"" (
        ""Id"" uuid NOT NULL,
        ""UserId"" uuid NOT NULL,
        ""Token"" character varying(100) NOT NULL,
        ""VerificationCode"" character varying(10) NOT NULL,
        ""Method"" integer NOT NULL,
        ""Destination"" character varying(200) NOT NULL,
        ""ExpiresAt"" timestamp with time zone NOT NULL,
        ""IsUsed"" boolean NOT NULL,
        ""IsVerified"" boolean NOT NULL,
        ""VerifiedAt"" timestamp with time zone,
        ""UsedAt"" timestamp with time zone,
        ""VerificationAttempts"" integer NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_PasswordResetTokens"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_PasswordResetTokens_Users_UserId"" FOREIGN KEY (""UserId"") REFERENCES ""Users"" (""Id"") ON DELETE CASCADE
    );

CREATE TABLE IF NOT EXISTS ""AppointmentProcedures"" (
        ""Id"" uuid NOT NULL,
        ""AppointmentId"" uuid NOT NULL,
        ""ProcedureId"" uuid NOT NULL,
        ""PatientId"" uuid NOT NULL,
        ""PriceCharged"" numeric(18,2) NOT NULL,
        ""Notes"" character varying(1000),
        ""PerformedAt"" timestamp with time zone NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_AppointmentProcedures"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_AppointmentProcedures_Appointments_AppointmentId"" FOREIGN KEY (""AppointmentId"") REFERENCES ""Appointments"" (""Id"") ON DELETE CASCADE,
        CONSTRAINT ""FK_AppointmentProcedures_Patients_PatientId"" FOREIGN KEY (""PatientId"") REFERENCES ""Patients"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_AppointmentProcedures_Procedures_ProcedureId"" FOREIGN KEY (""ProcedureId"") REFERENCES ""Procedures"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""ExamRequests"" (
        ""Id"" uuid NOT NULL,
        ""AppointmentId"" uuid NOT NULL,
        ""PatientId"" uuid NOT NULL,
        ""ExamType"" integer NOT NULL,
        ""ExamName"" character varying(200) NOT NULL,
        ""Description"" character varying(2000) NOT NULL,
        ""Urgency"" integer NOT NULL,
        ""Status"" integer NOT NULL,
        ""RequestedDate"" timestamp with time zone NOT NULL,
        ""ScheduledDate"" timestamp with time zone,
        ""CompletedDate"" timestamp with time zone,
        ""Results"" character varying(5000),
        ""Notes"" character varying(1000),
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_ExamRequests"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_ExamRequests_Appointments_AppointmentId"" FOREIGN KEY (""AppointmentId"") REFERENCES ""Appointments"" (""Id"") ON DELETE CASCADE,
        CONSTRAINT ""FK_ExamRequests_Patients_PatientId"" FOREIGN KEY (""PatientId"") REFERENCES ""Patients"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""MedicalRecords"" (
        ""Id"" uuid NOT NULL,
        ""AppointmentId"" uuid NOT NULL,
        ""PatientId"" uuid NOT NULL,
        ""Diagnosis"" character varying(2000) NOT NULL,
        ""Prescription"" character varying(5000) NOT NULL,
        ""Notes"" character varying(3000) NOT NULL,
        ""ConsultationDurationMinutes"" integer NOT NULL,
        ""ConsultationStartTime"" timestamp with time zone NOT NULL,
        ""ConsultationEndTime"" timestamp with time zone,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_MedicalRecords"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_MedicalRecords_Appointments_AppointmentId"" FOREIGN KEY (""AppointmentId"") REFERENCES ""Appointments"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_MedicalRecords_Patients_PatientId"" FOREIGN KEY (""PatientId"") REFERENCES ""Patients"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""Payments"" (
        ""Id"" uuid NOT NULL,
        ""AppointmentId"" uuid,
        ""ClinicSubscriptionId"" uuid,
        ""Amount"" numeric(18,2) NOT NULL,
        ""Method"" integer NOT NULL,
        ""Status"" integer NOT NULL,
        ""PaymentDate"" timestamp with time zone NOT NULL,
        ""ProcessedDate"" timestamp with time zone,
        ""TransactionId"" character varying(200),
        ""Notes"" character varying(1000),
        ""CancellationReason"" character varying(500),
        ""CancellationDate"" timestamp with time zone,
        ""CardLastFourDigits"" character varying(4),
        ""PixKey"" character varying(200),
        ""PixTransactionId"" character varying(200),
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Payments"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_Payments_Appointments_AppointmentId"" FOREIGN KEY (""AppointmentId"") REFERENCES ""Appointments"" (""Id"") ON DELETE RESTRICT,
        CONSTRAINT ""FK_Payments_ClinicSubscriptions_ClinicSubscriptionId"" FOREIGN KEY (""ClinicSubscriptionId"") REFERENCES ""ClinicSubscriptions"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""PrescriptionItems"" (
        ""Id"" uuid NOT NULL,
        ""MedicalRecordId"" uuid NOT NULL,
        ""MedicationId"" uuid NOT NULL,
        ""Dosage"" character varying(200) NOT NULL,
        ""Frequency"" character varying(200) NOT NULL,
        ""DurationDays"" integer NOT NULL,
        ""Instructions"" character varying(500),
        ""Quantity"" integer NOT NULL,
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_PrescriptionItems"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_PrescriptionItems_MedicalRecords_MedicalRecordId"" FOREIGN KEY (""MedicalRecordId"") REFERENCES ""MedicalRecords"" (""Id"") ON DELETE CASCADE,
        CONSTRAINT ""FK_PrescriptionItems_Medications_MedicationId"" FOREIGN KEY (""MedicationId"") REFERENCES ""Medications"" (""Id"") ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS ""Invoices"" (
        ""Id"" uuid NOT NULL,
        ""InvoiceNumber"" character varying(50) NOT NULL,
        ""PaymentId"" uuid NOT NULL,
        ""Type"" integer NOT NULL,
        ""Status"" integer NOT NULL,
        ""IssueDate"" timestamp with time zone NOT NULL,
        ""DueDate"" timestamp with time zone NOT NULL,
        ""Amount"" numeric(18,2) NOT NULL,
        ""TaxAmount"" numeric(18,2) NOT NULL,
        ""TotalAmount"" numeric(18,2) NOT NULL,
        ""Description"" character varying(1000),
        ""Notes"" character varying(1000),
        ""SentDate"" timestamp with time zone,
        ""PaidDate"" timestamp with time zone,
        ""CancellationDate"" timestamp with time zone,
        ""CancellationReason"" character varying(500),
        ""CustomerName"" character varying(200) NOT NULL,
        ""CustomerDocument"" character varying(50),
        ""CustomerAddress"" character varying(500),
        ""CreatedAt"" timestamp with time zone NOT NULL,
        ""UpdatedAt"" timestamp with time zone,
        ""TenantId"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_Invoices"" PRIMARY KEY (""Id""),
        CONSTRAINT ""FK_Invoices_Payments_PaymentId"" FOREIGN KEY (""PaymentId"") REFERENCES ""Payments"" (""Id"") ON DELETE RESTRICT
    );

CREATE INDEX IF NOT EXISTS ""IX_AppointmentProcedures_AppointmentId"" ON ""AppointmentProcedures"" (""AppointmentId"");

CREATE INDEX IF NOT EXISTS ""IX_AppointmentProcedures_PatientId"" ON ""AppointmentProcedures"" (""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_AppointmentProcedures_ProcedureId"" ON ""AppointmentProcedures"" (""ProcedureId"");

CREATE INDEX IF NOT EXISTS ""IX_Appointments_ClinicId"" ON ""Appointments"" (""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_Appointments_PatientId"" ON ""Appointments"" (""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_Appointments_TenantId"" ON ""Appointments"" (""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_Appointments_TenantId_Date_Clinic"" ON ""Appointments"" (""TenantId"", ""ScheduledDate"", ""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_Appointments_TenantId_Patient"" ON ""Appointments"" (""TenantId"", ""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_Appointments_TenantId_Status"" ON ""Appointments"" (""TenantId"", ""Status"");

CREATE INDEX IF NOT EXISTS ""IX_Clinics_TenantId"" ON ""Clinics"" (""TenantId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Clinics_TenantId_Document"" ON ""Clinics"" (""TenantId"", ""Document"");

CREATE INDEX IF NOT EXISTS ""IX_ClinicSubscriptions_ClinicId"" ON ""ClinicSubscriptions"" (""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_ClinicSubscriptions_NextPaymentDate"" ON ""ClinicSubscriptions"" (""NextPaymentDate"");

CREATE INDEX IF NOT EXISTS ""IX_ClinicSubscriptions_PendingPlanId"" ON ""ClinicSubscriptions"" (""PendingPlanId"");

CREATE INDEX IF NOT EXISTS ""IX_ClinicSubscriptions_Status"" ON ""ClinicSubscriptions"" (""Status"");

CREATE INDEX IF NOT EXISTS ""IX_ClinicSubscriptions_SubscriptionPlanId"" ON ""ClinicSubscriptions"" (""SubscriptionPlanId"");

CREATE INDEX IF NOT EXISTS ""IX_ClinicSubscriptions_TenantId_Status"" ON ""ClinicSubscriptions"" (""TenantId"", ""Status"");

CREATE INDEX IF NOT EXISTS ""IX_ExamRequests_AppointmentId"" ON ""ExamRequests"" (""AppointmentId"");

CREATE INDEX IF NOT EXISTS ""IX_ExamRequests_PatientId"" ON ""ExamRequests"" (""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_ExamRequests_Status"" ON ""ExamRequests"" (""Status"");

CREATE INDEX IF NOT EXISTS ""IX_ExamRequests_Status_Urgency"" ON ""ExamRequests"" (""Status"", ""Urgency"");

CREATE INDEX IF NOT EXISTS ""IX_Expenses_Category"" ON ""Expenses"" (""Category"");

CREATE INDEX IF NOT EXISTS ""IX_Expenses_ClinicId"" ON ""Expenses"" (""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_Expenses_DueDate"" ON ""Expenses"" (""DueDate"");

CREATE INDEX IF NOT EXISTS ""IX_Expenses_Status"" ON ""Expenses"" (""Status"");

CREATE INDEX IF NOT EXISTS ""IX_Expenses_TenantId"" ON ""Expenses"" (""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_HealthInsurancePlans_PatientId"" ON ""HealthInsurancePlans"" (""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_HealthInsurancePlans_PlanNumber"" ON ""HealthInsurancePlans"" (""PlanNumber"");

CREATE INDEX IF NOT EXISTS ""IX_HealthInsurancePlans_TenantId"" ON ""HealthInsurancePlans"" (""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_HealthInsurancePlans_TenantId_PatientId"" ON ""HealthInsurancePlans"" (""TenantId"", ""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_Invoices_DueDate"" ON ""Invoices"" (""DueDate"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Invoices_InvoiceNumber"" ON ""Invoices"" (""InvoiceNumber"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Invoices_PaymentId"" ON ""Invoices"" (""PaymentId"");

CREATE INDEX IF NOT EXISTS ""IX_Invoices_Status"" ON ""Invoices"" (""Status"");

CREATE INDEX IF NOT EXISTS ""IX_Invoices_TenantId"" ON ""Invoices"" (""TenantId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Materials_Code_TenantId"" ON ""Materials"" (""Code"", ""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_MedicalRecords_AppointmentId"" ON ""MedicalRecords"" (""AppointmentId"");

CREATE INDEX IF NOT EXISTS ""IX_MedicalRecords_PatientId"" ON ""MedicalRecords"" (""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_MedicalRecords_TenantId"" ON ""MedicalRecords"" (""TenantId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_MedicalRecords_TenantId_Appointment"" ON ""MedicalRecords"" (""TenantId"", ""AppointmentId"");

CREATE INDEX IF NOT EXISTS ""IX_MedicalRecords_TenantId_Patient"" ON ""MedicalRecords"" (""TenantId"", ""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_MedicalRecordTemplates_TenantId_Category"" ON ""MedicalRecordTemplates"" (""TenantId"", ""Category"");

CREATE INDEX IF NOT EXISTS ""IX_MedicalRecordTemplates_TenantId_Name"" ON ""MedicalRecordTemplates"" (""TenantId"", ""Name"");

CREATE INDEX IF NOT EXISTS ""IX_Medications_Category"" ON ""Medications"" (""Category"");

CREATE INDEX IF NOT EXISTS ""IX_Medications_IsActive"" ON ""Medications"" (""IsActive"");

CREATE INDEX IF NOT EXISTS ""IX_Medications_TenantId"" ON ""Medications"" (""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_Medications_TenantId_Name"" ON ""Medications"" (""TenantId"", ""Name"");

CREATE INDEX IF NOT EXISTS ""IX_ModuleConfigurations_ClinicId"" ON ""ModuleConfigurations"" (""ClinicId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_ModuleConfigurations_ClinicId_ModuleName"" ON ""ModuleConfigurations"" (""ClinicId"", ""ModuleName"");

CREATE INDEX IF NOT EXISTS ""IX_ModuleConfigurations_TenantId_IsEnabled"" ON ""ModuleConfigurations"" (""TenantId"", ""IsEnabled"");

CREATE INDEX IF NOT EXISTS ""IX_NotificationRoutines_Channel_TenantId"" ON ""NotificationRoutines"" (""Channel"", ""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_NotificationRoutines_NextExecutionAt"" ON ""NotificationRoutines"" (""NextExecutionAt"");

CREATE INDEX IF NOT EXISTS ""IX_NotificationRoutines_Scope_IsActive"" ON ""NotificationRoutines"" (""Scope"", ""IsActive"");

CREATE INDEX IF NOT EXISTS ""IX_NotificationRoutines_TenantId_IsActive"" ON ""NotificationRoutines"" (""TenantId"", ""IsActive"");

CREATE INDEX IF NOT EXISTS ""IX_NotificationRoutines_Type_TenantId"" ON ""NotificationRoutines"" (""Type"", ""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_Owners_ClinicId"" ON ""Owners"" (""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_Owners_Email"" ON ""Owners"" (""Email"");

CREATE INDEX IF NOT EXISTS ""IX_Owners_TenantId_IsActive"" ON ""Owners"" (""TenantId"", ""IsActive"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Owners_Username"" ON ""Owners"" (""Username"");

CREATE INDEX IF NOT EXISTS ""IX_PasswordResetTokens_TenantId_IsUsed_ExpiresAt"" ON ""PasswordResetTokens"" (""TenantId"", ""IsUsed"", ""ExpiresAt"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_PasswordResetTokens_Token"" ON ""PasswordResetTokens"" (""Token"");

CREATE INDEX IF NOT EXISTS ""IX_PasswordResetTokens_UserId"" ON ""PasswordResetTokens"" (""UserId"");

CREATE INDEX IF NOT EXISTS ""IX_PatientClinicLinks_ClinicId"" ON ""PatientClinicLinks"" (""ClinicId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_PatientClinicLinks_Patient_Clinic_Tenant"" ON ""PatientClinicLinks"" (""PatientId"", ""ClinicId"", ""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_PatientClinicLinks_PatientId"" ON ""PatientClinicLinks"" (""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_PatientClinicLinks_TenantId_ClinicId"" ON ""PatientClinicLinks"" (""TenantId"", ""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_Patients_GuardianId"" ON ""Patients"" (""GuardianId"");

CREATE INDEX IF NOT EXISTS ""IX_Patients_Name"" ON ""Patients"" (""Name"");

CREATE INDEX IF NOT EXISTS ""IX_Patients_TenantId"" ON ""Patients"" (""TenantId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Patients_TenantId_Document"" ON ""Patients"" (""TenantId"", ""Document"");

CREATE INDEX IF NOT EXISTS ""IX_Payments_AppointmentId"" ON ""Payments"" (""AppointmentId"");

CREATE INDEX IF NOT EXISTS ""IX_Payments_ClinicSubscriptionId"" ON ""Payments"" (""ClinicSubscriptionId"");

CREATE INDEX IF NOT EXISTS ""IX_Payments_PaymentDate"" ON ""Payments"" (""PaymentDate"");

CREATE INDEX IF NOT EXISTS ""IX_Payments_Status"" ON ""Payments"" (""Status"");

CREATE INDEX IF NOT EXISTS ""IX_Payments_TenantId"" ON ""Payments"" (""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_PrescriptionItems_MedicalRecordId"" ON ""PrescriptionItems"" (""MedicalRecordId"");

CREATE INDEX IF NOT EXISTS ""IX_PrescriptionItems_MedicationId"" ON ""PrescriptionItems"" (""MedicationId"");

CREATE INDEX IF NOT EXISTS ""IX_PrescriptionItems_TenantId_MedicalRecordId"" ON ""PrescriptionItems"" (""TenantId"", ""MedicalRecordId"");

CREATE INDEX IF NOT EXISTS ""IX_PrescriptionTemplates_TenantId_Category"" ON ""PrescriptionTemplates"" (""TenantId"", ""Category"");

CREATE INDEX IF NOT EXISTS ""IX_PrescriptionTemplates_TenantId_Name"" ON ""PrescriptionTemplates"" (""TenantId"", ""Name"");

CREATE INDEX IF NOT EXISTS ""IX_ProcedureMaterials_MaterialId"" ON ""ProcedureMaterials"" (""MaterialId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_ProcedureMaterials_ProcedureId_MaterialId_TenantId"" ON ""ProcedureMaterials"" (""ProcedureId"", ""MaterialId"", ""TenantId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Procedures_Code_TenantId"" ON ""Procedures"" (""Code"", ""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_SubscriptionPlans_IsActive"" ON ""SubscriptionPlans"" (""IsActive"");

CREATE INDEX IF NOT EXISTS ""IX_SubscriptionPlans_TenantId_Type"" ON ""SubscriptionPlans"" (""TenantId"", ""Type"");

CREATE INDEX IF NOT EXISTS ""IX_SubscriptionPlans_Type"" ON ""SubscriptionPlans"" (""Type"");

CREATE INDEX IF NOT EXISTS ""IX_Users_ClinicId"" ON ""Users"" (""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_Users_Email"" ON ""Users"" (""Email"");

CREATE INDEX IF NOT EXISTS ""IX_Users_Role"" ON ""Users"" (""Role"");

CREATE INDEX IF NOT EXISTS ""IX_Users_TenantId_IsActive"" ON ""Users"" (""TenantId"", ""IsActive"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Users_Username"" ON ""Users"" (""Username"");

CREATE INDEX IF NOT EXISTS ""IX_WaitingQueueConfigurations_ClinicId"" ON ""WaitingQueueConfigurations"" (""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_WaitingQueueConfigurations_TenantId"" ON ""WaitingQueueConfigurations"" (""TenantId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_WaitingQueueConfigurations_TenantId_Clinic"" ON ""WaitingQueueConfigurations"" (""TenantId"", ""ClinicId"");

CREATE UNIQUE INDEX IF NOT EXISTS ""IX_WaitingQueueEntries_AppointmentId"" ON ""WaitingQueueEntries"" (""AppointmentId"");

CREATE INDEX IF NOT EXISTS ""IX_WaitingQueueEntries_ClinicId"" ON ""WaitingQueueEntries"" (""ClinicId"");

CREATE INDEX IF NOT EXISTS ""IX_WaitingQueueEntries_PatientId"" ON ""WaitingQueueEntries"" (""PatientId"");

CREATE INDEX IF NOT EXISTS ""IX_WaitingQueueEntries_TenantId"" ON ""WaitingQueueEntries"" (""TenantId"");

CREATE INDEX IF NOT EXISTS ""IX_WaitingQueueEntries_TenantId_CheckInTime"" ON ""WaitingQueueEntries"" (""TenantId"", ""CheckInTime"");

CREATE INDEX IF NOT EXISTS ""IX_WaitingQueueEntries_TenantId_Position"" ON ""WaitingQueueEntries"" (""TenantId"", ""Position"");

CREATE INDEX IF NOT EXISTS ""IX_WaitingQueueEntries_TenantId_Clinic_Status"" ON ""WaitingQueueEntries"" (""TenantId"", ""ClinicId"", ""Status"");");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Idempotent rollback using DROP TABLE IF EXISTS
            // Drop tables in reverse dependency order
            migrationBuilder.Sql(@"
DROP TABLE IF EXISTS ""AppointmentProcedures"";
DROP TABLE IF EXISTS ""ExamRequests"";
DROP TABLE IF EXISTS ""Expenses"";
DROP TABLE IF EXISTS ""HealthInsurancePlans"";
DROP TABLE IF EXISTS ""Invoices"";
DROP TABLE IF EXISTS ""MedicalRecordTemplates"";
DROP TABLE IF EXISTS ""ModuleConfigurations"";
DROP TABLE IF EXISTS ""NotificationRoutines"";
DROP TABLE IF EXISTS ""Owners"";
DROP TABLE IF EXISTS ""PasswordResetTokens"";
DROP TABLE IF EXISTS ""PatientClinicLinks"";
DROP TABLE IF EXISTS ""PrescriptionItems"";
DROP TABLE IF EXISTS ""PrescriptionTemplates"";
DROP TABLE IF EXISTS ""ProcedureMaterials"";
DROP TABLE IF EXISTS ""Payments"";
DROP TABLE IF EXISTS ""Users"";
DROP TABLE IF EXISTS ""WaitingQueueEntries"";
DROP TABLE IF EXISTS ""WaitingQueueConfigurations"";
DROP TABLE IF EXISTS ""MedicalRecords"";
DROP TABLE IF EXISTS ""Medications"";
DROP TABLE IF EXISTS ""Materials"";
DROP TABLE IF EXISTS ""Procedures"";
DROP TABLE IF EXISTS ""ClinicSubscriptions"";
DROP TABLE IF EXISTS ""Appointments"";
DROP TABLE IF EXISTS ""SubscriptionPlans"";
DROP TABLE IF EXISTS ""Clinics"";
DROP TABLE IF EXISTS ""Patients"";
");
        }
    }
}
