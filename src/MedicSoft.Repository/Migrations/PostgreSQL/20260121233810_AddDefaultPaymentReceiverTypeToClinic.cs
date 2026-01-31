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
            // Check if column exists first to handle cases where the previous migration didn't complete
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    -- Check if the column exists (case-insensitive check for PostgreSQL compatibility)
                    IF EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE LOWER(table_name) = 'clinics' 
                        AND LOWER(column_name) = 'defaultpaymentreceivertype'
                        AND table_schema = 'public'
                    ) THEN
                        -- Column exists, alter it from int to varchar
                        ALTER TABLE ""Clinics"" 
                        ALTER COLUMN ""DefaultPaymentReceiverType"" 
                        TYPE character varying(50) 
                        USING CASE 
                            WHEN ""DefaultPaymentReceiverType""::integer = 1 THEN 'Clinic'
                            WHEN ""DefaultPaymentReceiverType""::integer = 2 THEN 'Secretary'
                            ELSE 'Secretary'
                        END;
                    ELSE
                        -- Column doesn't exist, create it directly as varchar with default value
                        ALTER TABLE ""Clinics"" 
                        ADD COLUMN ""DefaultPaymentReceiverType"" character varying(50) NOT NULL DEFAULT 'Secretary';
                    END IF;
                END $$;
            ");
            
            // Note: Appointment payment tracking columns (IsPaid, PaidAt, PaidByUserId, PaymentReceivedBy)
            // were already added by migration 20260121193310_AddPaymentTrackingFields
            // No need to add them again here
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert DefaultPaymentReceiverType back to int type or drop it if it was created by this migration
            // Using explicit SQL to handle the data conversion safely
            migrationBuilder.Sql(@"
                DO $$ 
                DECLARE
                    col_type TEXT;
                BEGIN
                    -- Check if the column exists and get its data type in one query
                    SELECT data_type INTO col_type
                    FROM information_schema.columns 
                    WHERE LOWER(table_name) = 'clinics' 
                    AND LOWER(column_name) = 'defaultpaymentreceivertype'
                    AND table_schema = 'public';
                    
                    -- If column exists and is varchar, proceed with conversion or drop
                    IF col_type = 'character varying' THEN
                        -- Check if previous migration created it as int
                        IF EXISTS (
                            SELECT 1 
                            FROM ""__EFMigrationsHistory"" 
                            WHERE ""MigrationId"" = '20260121193310_AddPaymentTrackingFields'
                        ) THEN
                            -- Previous migration exists, convert back to int
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
                        ELSE
                            -- Previous migration doesn't exist, drop the column
                            ALTER TABLE ""Clinics"" 
                            DROP COLUMN ""DefaultPaymentReceiverType"";
                        END IF;
                    END IF;
                END $$;
            ");
            
            // Note: Appointment payment tracking columns are NOT dropped here
            // because they were added by migration 20260121193310_AddPaymentTrackingFields
            // and should be dropped by that migration's Down() method
        }
    }
}
