using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ManualOverrideActive",
                table: "ClinicSubscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ManualOverrideReason",
                table: "ClinicSubscriptions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ManualOverrideSetAt",
                table: "ClinicSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManualOverrideSetBy",
                table: "ClinicSubscriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfessionalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Specialty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Owners_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Owners_ClinicId",
                table: "Owners",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_Email",
                table: "Owners",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_TenantId_IsActive",
                table: "Owners",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Owners_Username",
                table: "Owners",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropColumn(
                name: "ManualOverrideActive",
                table: "ClinicSubscriptions");

            migrationBuilder.DropColumn(
                name: "ManualOverrideReason",
                table: "ClinicSubscriptions");

            migrationBuilder.DropColumn(
                name: "ManualOverrideSetAt",
                table: "ClinicSubscriptions");

            migrationBuilder.DropColumn(
                name: "ManualOverrideSetBy",
                table: "ClinicSubscriptions");
        }
    }
}
