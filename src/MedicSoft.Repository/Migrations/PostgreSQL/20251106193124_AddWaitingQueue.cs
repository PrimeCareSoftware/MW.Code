using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddWaitingQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subdomain",
                table: "Clinics",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WaitingQueueConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayMode = table.Column<int>(type: "integer", nullable: false),
                    ShowEstimatedWaitTime = table.Column<bool>(type: "boolean", nullable: false),
                    ShowPatientNames = table.Column<bool>(type: "boolean", nullable: false),
                    ShowPriority = table.Column<bool>(type: "boolean", nullable: false),
                    AutoRefreshSeconds = table.Column<int>(type: "integer", nullable: false),
                    EnableSoundNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    ShowPosition = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingQueueConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaitingQueueConfigurations_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaitingQueueEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CalledTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TriageNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EstimatedWaitTimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingQueueEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaitingQueueEntries_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaitingQueueEntries_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaitingQueueEntries_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueConfigurations_ClinicId",
                table: "WaitingQueueConfigurations",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueConfigurations_TenantId",
                table: "WaitingQueueConfigurations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueConfigurations_TenantId_Clinic",
                table: "WaitingQueueConfigurations",
                columns: new[] { "TenantId", "ClinicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueEntries_AppointmentId",
                table: "WaitingQueueEntries",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueEntries_ClinicId",
                table: "WaitingQueueEntries",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueEntries_PatientId",
                table: "WaitingQueueEntries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueEntries_TenantId",
                table: "WaitingQueueEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueEntries_TenantId_CheckInTime",
                table: "WaitingQueueEntries",
                columns: new[] { "TenantId", "CheckInTime" });

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueEntries_TenantId_Clinic_Status",
                table: "WaitingQueueEntries",
                columns: new[] { "TenantId", "ClinicId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WaitingQueueEntries_TenantId_Position",
                table: "WaitingQueueEntries",
                columns: new[] { "TenantId", "Position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaitingQueueConfigurations");

            migrationBuilder.DropTable(
                name: "WaitingQueueEntries");

            migrationBuilder.DropColumn(
                name: "Subdomain",
                table: "Clinics");
        }
    }
}
