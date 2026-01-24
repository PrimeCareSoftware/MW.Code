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
            migrationBuilder.CreateTable(
                name: "ControlledMedicationRegistries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RegistryType = table.Column<int>(type: "integer", nullable: false),
                    MedicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ActiveIngredient = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AnvisaList = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Concentration = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PharmaceuticalForm = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    QuantityIn = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    QuantityOut = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PrescriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PatientName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PatientCPF = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    DoctorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DoctorCRM = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SupplierName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SupplierCNPJ = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    RegisteredByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlledMedicationRegistries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControlledMedicationRegistries_DigitalPrescriptions_Prescri~",
                        column: x => x.PrescriptionId,
                        principalTable: "DigitalPrescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ControlledMedicationRegistries_Users_RegisteredByUserId",
                        column: x => x.RegisteredByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyControlledBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    MedicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ActiveIngredient = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AnvisaList = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    InitialBalance = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalIn = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalOut = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    CalculatedFinalBalance = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    PhysicalBalance = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    Discrepancy = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    DiscrepancyReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ClosedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyControlledBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyControlledBalances_Users_ClosedByUserId",
                        column: x => x.ClosedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SngpcTransmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SNGPCReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    AttemptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProtocolNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AnvisaResponse = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ErrorCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TransmissionMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EndpointUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    HttpStatusCode = table.Column<int>(type: "integer", nullable: true),
                    ResponseTimeMs = table.Column<long>(type: "bigint", nullable: true),
                    XmlHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    XmlSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    InitiatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SngpcTransmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SngpcTransmissions_SNGPCReports_SNGPCReportId",
                        column: x => x.SNGPCReportId,
                        principalTable: "SNGPCReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SngpcTransmissions_Users_InitiatedByUserId",
                        column: x => x.InitiatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ControlledMedicationRegistries_DocumentNumber",
                table: "ControlledMedicationRegistries",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ControlledMedicationRegistries_PrescriptionId",
                table: "ControlledMedicationRegistries",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlledMedicationRegistries_RegisteredByUserId",
                table: "ControlledMedicationRegistries",
                column: "RegisteredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlledMedicationRegistries_TenantId_AnvisaList",
                table: "ControlledMedicationRegistries",
                columns: new[] { "TenantId", "AnvisaList" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlledMedicationRegistries_TenantId_Date",
                table: "ControlledMedicationRegistries",
                columns: new[] { "TenantId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlledMedicationRegistries_TenantId_MedicationName",
                table: "ControlledMedicationRegistries",
                columns: new[] { "TenantId", "MedicationName" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlledMedicationRegistries_TenantId_PrescriptionId",
                table: "ControlledMedicationRegistries",
                columns: new[] { "TenantId", "PrescriptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlledMedicationRegistries_TenantId_Type_Date",
                table: "ControlledMedicationRegistries",
                columns: new[] { "TenantId", "RegistryType", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyControlledBalances_ClosedByUserId",
                table: "MonthlyControlledBalances",
                column: "ClosedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyControlledBalances_TenantId_Medication_Year",
                table: "MonthlyControlledBalances",
                columns: new[] { "TenantId", "MedicationName", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyControlledBalances_TenantId_Status",
                table: "MonthlyControlledBalances",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyControlledBalances_TenantId_Year_Month",
                table: "MonthlyControlledBalances",
                columns: new[] { "TenantId", "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyControlledBalances_TenantId_Year_Month_Medication",
                table: "MonthlyControlledBalances",
                columns: new[] { "TenantId", "Year", "Month", "MedicationName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SngpcTransmissions_InitiatedByUserId",
                table: "SngpcTransmissions",
                column: "InitiatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SngpcTransmissions_ProtocolNumber",
                table: "SngpcTransmissions",
                column: "ProtocolNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SngpcTransmissions_ReportId_Status",
                table: "SngpcTransmissions",
                columns: new[] { "SNGPCReportId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SngpcTransmissions_TenantId_AttemptedAt",
                table: "SngpcTransmissions",
                columns: new[] { "TenantId", "AttemptedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SngpcTransmissions_TenantId_ReportId_Attempt",
                table: "SngpcTransmissions",
                columns: new[] { "TenantId", "SNGPCReportId", "AttemptNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_SngpcTransmissions_TenantId_Status",
                table: "SngpcTransmissions",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlledMedicationRegistries");

            migrationBuilder.DropTable(
                name: "MonthlyControlledBalances");

            migrationBuilder.DropTable(
                name: "SngpcTransmissions");
        }
    }
}
