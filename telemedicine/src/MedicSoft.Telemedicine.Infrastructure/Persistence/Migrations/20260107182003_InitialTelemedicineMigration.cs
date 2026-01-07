using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Telemedicine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialTelemedicineMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelemedicineSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RoomUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Duration_StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration_EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordingUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SessionNotes = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemedicineSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineSessions_TenantId_AppointmentId",
                table: "TelemedicineSessions",
                columns: new[] { "TenantId", "AppointmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineSessions_TenantId_ClinicId",
                table: "TelemedicineSessions",
                columns: new[] { "TenantId", "ClinicId" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineSessions_TenantId_PatientId",
                table: "TelemedicineSessions",
                columns: new[] { "TenantId", "PatientId" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineSessions_TenantId_ProviderId",
                table: "TelemedicineSessions",
                columns: new[] { "TenantId", "ProviderId" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineSessions_TenantId_Status",
                table: "TelemedicineSessions",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemedicineSessions");
        }
    }
}
