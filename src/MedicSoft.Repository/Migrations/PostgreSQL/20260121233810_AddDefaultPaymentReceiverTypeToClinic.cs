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
            // The column was already added by migration 20260121193310_AddPaymentTrackingFields as int
            // We need to alter it to use string instead of int
            // Using explicit SQL to handle the data conversion safely
            migrationBuilder.Sql(@"
                ALTER TABLE ""Clinics"" 
                ALTER COLUMN ""DefaultPaymentReceiverType"" 
                TYPE character varying(50) 
                USING CASE 
                    WHEN ""DefaultPaymentReceiverType""::integer = 1 THEN 'Clinic'
                    WHEN ""DefaultPaymentReceiverType""::integer = 2 THEN 'Secretary'
                    ELSE 'Secretary'
                END;
            ");
            
            // Note: Appointment payment tracking columns (IsPaid, PaidAt, PaidByUserId, PaymentReceivedBy)
            // were already added by migration 20260121193310_AddPaymentTrackingFields
            // No need to add them again here
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert DefaultPaymentReceiverType back to int type
            // Using explicit SQL to handle the data conversion safely
            migrationBuilder.Sql(@"
                ALTER TABLE ""Clinics"" 
                ALTER COLUMN ""DefaultPaymentReceiverType"" 
                TYPE integer 
                USING CASE 
                    WHEN ""DefaultPaymentReceiverType"" = 'Clinic' THEN 1
                    WHEN ""DefaultPaymentReceiverType"" = 'Secretary' THEN 2
                    ELSE 2
                END;
                
                ALTER TABLE ""Clinics"" 
                ALTER COLUMN ""DefaultPaymentReceiverType"" 
                SET DEFAULT 2;
            ");
            
            // Note: Appointment payment tracking columns are NOT dropped here
            // because they were added by migration 20260121193310_AddPaymentTrackingFields
            // and should be dropped by that migration's Down() method
        }
    }
}
