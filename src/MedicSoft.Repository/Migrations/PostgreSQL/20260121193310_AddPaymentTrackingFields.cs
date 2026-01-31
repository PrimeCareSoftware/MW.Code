using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddPaymentTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add payment tracking fields to Appointments table
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Appointments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaidByUserId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentReceivedBy",
                table: "Appointments",
                type: "integer",
                nullable: true);

            // Add payment receiver configuration to Clinics table
            migrationBuilder.AddColumn<int>(
                name: "DefaultPaymentReceiverType",
                table: "Clinics",
                type: "integer",
                nullable: false,
                defaultValue: 2); // PaymentReceiverType.Secretary by default

            // Create index for PaidByUserId
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PaidByUserId",
                table: "Appointments",
                column: "PaidByUserId");

            // Add foreign key for PaidByUserId
            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_PaidByUserId",
                table: "Appointments",
                column: "PaidByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_PaidByUserId",
                table: "Appointments");

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PaidByUserId",
                table: "Appointments");

            // Drop columns from Clinics
            migrationBuilder.DropColumn(
                name: "DefaultPaymentReceiverType",
                table: "Clinics");

            // Drop columns from Appointments
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaidByUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentReceivedBy",
                table: "Appointments");
        }
    }
}
