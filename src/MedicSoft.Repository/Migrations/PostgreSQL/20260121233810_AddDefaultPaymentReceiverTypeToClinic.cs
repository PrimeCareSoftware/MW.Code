using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddDefaultPaymentReceiverTypeToClinic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultPaymentReceiverType",
                table: "Clinics",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Appointments",
                type: "timestamp with time zone",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultPaymentReceiverType",
                table: "Clinics");

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
