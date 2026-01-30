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
            // Convert DefaultPaymentReceiverType from int to string (enum conversion)
            // The column was already added by migration 20260121193310_AddPaymentTrackingFields
            // We need to alter it to use string instead of int
            migrationBuilder.AlterColumn<string>(
                name: "DefaultPaymentReceiverType",
                table: "Clinics",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);
            
            // Note: Appointment payment tracking columns (IsPaid, PaidAt, PaidByUserId, PaymentReceivedBy)
            // were already added by migration 20260121193310_AddPaymentTrackingFields
            // No need to add them again here
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert DefaultPaymentReceiverType back to int type
            migrationBuilder.AlterColumn<int>(
                name: "DefaultPaymentReceiverType",
                table: "Clinics",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
            
            // Note: Appointment payment tracking columns are NOT dropped here
            // because they were added by migration 20260121193310_AddPaymentTrackingFields
            // and should be dropped by that migration's Down() method
        }
    }
}
