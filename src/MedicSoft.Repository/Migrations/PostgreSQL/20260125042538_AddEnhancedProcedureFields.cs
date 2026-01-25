using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddEnhancedProcedureFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcceptedHealthInsurances",
                table: "Procedures",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowInExclusiveProcedureAttendance",
                table: "Procedures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowInMedicalAttendance",
                table: "Procedures",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "Procedures",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedHealthInsurances",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "AllowInExclusiveProcedureAttendance",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "AllowInMedicalAttendance",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Procedures");
        }
    }
}
