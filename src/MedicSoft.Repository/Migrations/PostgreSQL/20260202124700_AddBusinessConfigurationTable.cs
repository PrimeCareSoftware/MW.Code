using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddBusinessConfigurationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessType = table.Column<int>(type: "integer", nullable: false),
                    PrimarySpecialty = table.Column<int>(type: "integer", nullable: false),
                    ElectronicPrescription = table.Column<bool>(type: "boolean", nullable: false),
                    LabIntegration = table.Column<bool>(type: "boolean", nullable: false),
                    VaccineControl = table.Column<bool>(type: "boolean", nullable: false),
                    InventoryManagement = table.Column<bool>(type: "boolean", nullable: false),
                    MultiRoom = table.Column<bool>(type: "boolean", nullable: false),
                    ReceptionQueue = table.Column<bool>(type: "boolean", nullable: false),
                    FinancialModule = table.Column<bool>(type: "boolean", nullable: false),
                    HealthInsurance = table.Column<bool>(type: "boolean", nullable: false),
                    Telemedicine = table.Column<bool>(type: "boolean", nullable: false),
                    HomeVisit = table.Column<bool>(type: "boolean", nullable: false),
                    GroupSessions = table.Column<bool>(type: "boolean", nullable: false),
                    PublicProfile = table.Column<bool>(type: "boolean", nullable: false),
                    OnlineBooking = table.Column<bool>(type: "boolean", nullable: false),
                    PatientReviews = table.Column<bool>(type: "boolean", nullable: false),
                    BiReports = table.Column<bool>(type: "boolean", nullable: false),
                    ApiAccess = table.Column<bool>(type: "boolean", nullable: false),
                    WhiteLabel = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessConfigurations_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessConfigurations_ClinicId",
                table: "BusinessConfigurations",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessConfigurations_TenantId",
                table: "BusinessConfigurations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessConfigurations_TenantId_ClinicId",
                table: "BusinessConfigurations",
                columns: new[] { "TenantId", "ClinicId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessConfigurations");
        }
    }
}
