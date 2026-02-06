using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddAppointmentPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Index for most common query (daily agenda)
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClinicId_ScheduledDate_TenantId",
                table: "Appointments",
                columns: new[] { "ClinicId", "ScheduledDate", "TenantId" });

            // Index for filtering by professional
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ProfessionalId_ScheduledDate",
                table: "Appointments",
                columns: new[] { "ProfessionalId", "ScheduledDate" });

            // Index for patient appointment lookups
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId_ScheduledDate",
                table: "Appointments",
                columns: new[] { "PatientId", "ScheduledDate" });

            // Index for queries by status and date
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Status_ScheduledDate_TenantId",
                table: "Appointments",
                columns: new[] { "Status", "ScheduledDate", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_ClinicId_ScheduledDate_TenantId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ProfessionalId_ScheduledDate",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId_ScheduledDate",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_Status_ScheduledDate_TenantId",
                table: "Appointments");
        }
    }
}
