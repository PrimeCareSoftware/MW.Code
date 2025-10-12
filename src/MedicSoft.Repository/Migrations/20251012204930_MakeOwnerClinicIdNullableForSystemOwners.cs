using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations
{
    /// <inheritdoc />
    public partial class MakeOwnerClinicIdNullableForSystemOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Owners_Clinics_ClinicId",
                table: "Owners");

            // Alter the column to allow NULL
            migrationBuilder.AlterColumn<Guid>(
                name: "ClinicId",
                table: "Owners",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // Recreate the foreign key constraint with nullable support
            migrationBuilder.AddForeignKey(
                name: "FK_Owners_Clinics_ClinicId",
                table: "Owners",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Owners_Clinics_ClinicId",
                table: "Owners");

            // Alter the column back to NOT NULL
            // Note: This will fail if there are any NULL values in the column
            migrationBuilder.AlterColumn<Guid>(
                name: "ClinicId",
                table: "Owners",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // Recreate the foreign key constraint with NOT NULL
            migrationBuilder.AddForeignKey(
                name: "FK_Owners_Clinics_ClinicId",
                table: "Owners",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
