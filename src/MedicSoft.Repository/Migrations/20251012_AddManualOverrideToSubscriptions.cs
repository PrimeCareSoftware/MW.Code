using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations
{
    /// <summary>
    /// Migration to add manual override functionality to subscriptions
    /// and improve owner/admin capabilities for managing users and permissions
    /// </summary>
    public partial class AddManualOverrideToSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add manual override fields to ClinicSubscriptions table
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
