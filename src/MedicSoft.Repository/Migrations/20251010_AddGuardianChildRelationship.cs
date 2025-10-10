using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations
{
    /// <summary>
    /// Migration to add Guardian-Child relationship to Patient entity
    /// Allows tracking of responsible adults for children under 18
    /// </summary>
    public partial class AddGuardianChildRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add GuardianId column to Patients table
            migrationBuilder.AddColumn<Guid>(
                name: "GuardianId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true);

            // Create index on GuardianId for performance
            migrationBuilder.CreateIndex(
                name: "IX_Patients_GuardianId",
                table: "Patients",
                column: "GuardianId");

            // Add foreign key constraint with NO ACTION on delete
            // to prevent cascading deletes
            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Patients_GuardianId",
                table: "Patients",
                column: "GuardianId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Patients_GuardianId",
                table: "Patients");

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_Patients_GuardianId",
                table: "Patients");

            // Drop column
            migrationBuilder.DropColumn(
                name: "GuardianId",
                table: "Patients");
        }
    }
}
