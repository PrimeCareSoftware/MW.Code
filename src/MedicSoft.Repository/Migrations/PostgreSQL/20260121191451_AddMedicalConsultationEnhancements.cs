using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddMedicalConsultationEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfessionalSignature",
                table: "MedicalRecords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "ClinicalExaminations",
                type: "numeric(4,2)",
                precision: 4,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "ClinicalExaminations",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HealthInsurancePlanId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionalId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_HealthInsurancePlanId",
                table: "Appointments",
                column: "HealthInsurancePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ProfessionalId",
                table: "Appointments",
                column: "ProfessionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_HealthInsurancePlans_HealthInsurancePlanId",
                table: "Appointments",
                column: "HealthInsurancePlanId",
                principalTable: "HealthInsurancePlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ProfessionalId",
                table: "Appointments",
                column: "ProfessionalId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_HealthInsurancePlans_HealthInsurancePlanId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ProfessionalId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_HealthInsurancePlanId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ProfessionalId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ProfessionalSignature",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ClinicalExaminations");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "ClinicalExaminations");

            migrationBuilder.DropColumn(
                name: "HealthInsurancePlanId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ProfessionalId",
                table: "Appointments");
        }
    }
}
