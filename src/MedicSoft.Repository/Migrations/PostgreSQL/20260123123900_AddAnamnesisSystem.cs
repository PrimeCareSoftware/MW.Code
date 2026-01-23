using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddAnamnesisSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnamnesisTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Specialty = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SectionsJson = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnamnesisTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnamnesisResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AnswersJson = table.Column<string>(type: "text", nullable: false),
                    IsComplete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnamnesisResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnamnesisResponses_AnamnesisTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "AnamnesisTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnamnesisResponses_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnamnesisResponses_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnamnesisResponses_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_AppointmentId",
                table: "AnamnesisResponses",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_DoctorId",
                table: "AnamnesisResponses",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_PatientId",
                table: "AnamnesisResponses",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_ResponseDate",
                table: "AnamnesisResponses",
                column: "ResponseDate");

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_TemplateId",
                table: "AnamnesisResponses",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_TenantId",
                table: "AnamnesisResponses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_TenantId_AppointmentId",
                table: "AnamnesisResponses",
                columns: new[] { "TenantId", "AppointmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_TenantId_DoctorId",
                table: "AnamnesisResponses",
                columns: new[] { "TenantId", "DoctorId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisResponses_TenantId_PatientId",
                table: "AnamnesisResponses",
                columns: new[] { "TenantId", "PatientId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisTemplates_IsActive",
                table: "AnamnesisTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisTemplates_TenantId",
                table: "AnamnesisTemplates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisTemplates_TenantId_Specialty",
                table: "AnamnesisTemplates",
                columns: new[] { "TenantId", "Specialty" });

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisTemplates_TenantId_Specialty_IsDefault",
                table: "AnamnesisTemplates",
                columns: new[] { "TenantId", "Specialty", "IsDefault" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnamnesisResponses");

            migrationBuilder.DropTable(
                name: "AnamnesisTemplates");
        }
    }
}
