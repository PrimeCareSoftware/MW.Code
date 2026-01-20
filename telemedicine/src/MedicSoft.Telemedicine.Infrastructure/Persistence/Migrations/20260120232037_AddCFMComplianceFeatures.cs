using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Telemedicine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCFMComplianceFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionQuality",
                table: "TelemedicineSessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ConsentDate",
                table: "TelemedicineSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConsentId",
                table: "TelemedicineSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConsentIpAddress",
                table: "TelemedicineSessions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstAppointmentJustification",
                table: "TelemedicineSessions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFirstAppointment",
                table: "TelemedicineSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PatientConsented",
                table: "TelemedicineSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TelemedicineConsents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConsentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConsentText = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AcceptsRecording = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptsDataSharing = table.Column<bool>(type: "boolean", nullable: false),
                    DigitalSignature = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevocationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemedicineConsents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineConsents_AppointmentId",
                table: "TelemedicineConsents",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineConsents_TenantId_ConsentDate",
                table: "TelemedicineConsents",
                columns: new[] { "TenantId", "ConsentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineConsents_TenantId_PatientId",
                table: "TelemedicineConsents",
                columns: new[] { "TenantId", "PatientId" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineConsents_TenantId_PatientId_IsActive",
                table: "TelemedicineConsents",
                columns: new[] { "TenantId", "PatientId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemedicineConsents");

            migrationBuilder.DropColumn(
                name: "ConnectionQuality",
                table: "TelemedicineSessions");

            migrationBuilder.DropColumn(
                name: "ConsentDate",
                table: "TelemedicineSessions");

            migrationBuilder.DropColumn(
                name: "ConsentId",
                table: "TelemedicineSessions");

            migrationBuilder.DropColumn(
                name: "ConsentIpAddress",
                table: "TelemedicineSessions");

            migrationBuilder.DropColumn(
                name: "FirstAppointmentJustification",
                table: "TelemedicineSessions");

            migrationBuilder.DropColumn(
                name: "IsFirstAppointment",
                table: "TelemedicineSessions");

            migrationBuilder.DropColumn(
                name: "PatientConsented",
                table: "TelemedicineSessions");
        }
    }
}
