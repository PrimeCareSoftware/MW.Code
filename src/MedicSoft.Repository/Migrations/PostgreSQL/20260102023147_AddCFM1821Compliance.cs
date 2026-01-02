using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddCFM1821Compliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotherName",
                table: "Patients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChiefComplaint",
                table: "MedicalRecords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "MedicalRecords",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClosedByUserId",
                table: "MedicalRecords",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentMedications",
                table: "MedicalRecords",
                type: "character varying(3000)",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FamilyHistory",
                table: "MedicalRecords",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HistoryOfPresentIllness",
                table: "MedicalRecords",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "MedicalRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LifestyleHabits",
                table: "MedicalRecords",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PastMedicalHistory",
                table: "MedicalRecords",
                type: "character varying(3000)",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClinicalExaminations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    BloodPressureSystolic = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    BloodPressureDiastolic = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    HeartRate = table.Column<int>(type: "integer", nullable: true),
                    RespiratoryRate = table.Column<int>(type: "integer", nullable: true),
                    Temperature = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    OxygenSaturation = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    SystematicExamination = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    GeneralState = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalExaminations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalExaminations_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosticHypotheses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ICD10Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DiagnosedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosticHypotheses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagnosticHypotheses_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InformedConsents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsentText = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IPAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    DigitalSignature = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RegisteredByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformedConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InformedConsents_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InformedConsents_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TherapeuticPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Treatment = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    MedicationPrescription = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    ExamRequests = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Referrals = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PatientGuidance = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapeuticPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TherapeuticPlans_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_TenantId_IsClosed",
                table: "MedicalRecords",
                columns: new[] { "TenantId", "IsClosed" });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalExaminations_MedicalRecordId",
                table: "ClinicalExaminations",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalExaminations_TenantId",
                table: "ClinicalExaminations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalExaminations_TenantId_MedicalRecord",
                table: "ClinicalExaminations",
                columns: new[] { "TenantId", "MedicalRecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticHypotheses_MedicalRecordId",
                table: "DiagnosticHypotheses",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticHypotheses_TenantId",
                table: "DiagnosticHypotheses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticHypotheses_TenantId_ICD10Code",
                table: "DiagnosticHypotheses",
                columns: new[] { "TenantId", "ICD10Code" });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticHypotheses_TenantId_MedicalRecord",
                table: "DiagnosticHypotheses",
                columns: new[] { "TenantId", "MedicalRecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_InformedConsents_MedicalRecordId",
                table: "InformedConsents",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_InformedConsents_PatientId",
                table: "InformedConsents",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_InformedConsents_TenantId",
                table: "InformedConsents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InformedConsents_TenantId_IsAccepted",
                table: "InformedConsents",
                columns: new[] { "TenantId", "IsAccepted" });

            migrationBuilder.CreateIndex(
                name: "IX_InformedConsents_TenantId_MedicalRecord",
                table: "InformedConsents",
                columns: new[] { "TenantId", "MedicalRecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_InformedConsents_TenantId_Patient",
                table: "InformedConsents",
                columns: new[] { "TenantId", "PatientId" });

            migrationBuilder.CreateIndex(
                name: "IX_TherapeuticPlans_MedicalRecordId",
                table: "TherapeuticPlans",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TherapeuticPlans_TenantId",
                table: "TherapeuticPlans",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TherapeuticPlans_TenantId_MedicalRecord",
                table: "TherapeuticPlans",
                columns: new[] { "TenantId", "MedicalRecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_TherapeuticPlans_TenantId_ReturnDate",
                table: "TherapeuticPlans",
                columns: new[] { "TenantId", "ReturnDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicalExaminations");

            migrationBuilder.DropTable(
                name: "DiagnosticHypotheses");

            migrationBuilder.DropTable(
                name: "InformedConsents");

            migrationBuilder.DropTable(
                name: "TherapeuticPlans");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_TenantId_IsClosed",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "MotherName",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ChiefComplaint",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "ClosedByUserId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "CurrentMedications",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "FamilyHistory",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "HistoryOfPresentIllness",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "LifestyleHabits",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "PastMedicalHistory",
                table: "MedicalRecords");
        }
    }
}
