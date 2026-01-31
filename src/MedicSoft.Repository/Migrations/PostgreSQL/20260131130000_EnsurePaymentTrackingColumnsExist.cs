using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class EnsurePaymentTrackingColumnsExist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsPaid column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_schema = 'public'
                        AND LOWER(table_name) = 'appointments' 
                        AND LOWER(column_name) = 'ispaid'
                    ) THEN
                        ALTER TABLE ""Appointments"" ADD COLUMN ""IsPaid"" boolean NOT NULL DEFAULT false;
                    END IF;
                END $$;
            ");

            // Add PaidAt column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_schema = 'public'
                        AND LOWER(table_name) = 'appointments' 
                        AND LOWER(column_name) = 'paidat'
                    ) THEN
                        ALTER TABLE ""Appointments"" ADD COLUMN ""PaidAt"" timestamp without time zone NULL;
                    END IF;
                END $$;
            ");

            // Add PaidByUserId column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_schema = 'public'
                        AND LOWER(table_name) = 'appointments' 
                        AND LOWER(column_name) = 'paidbyuserid'
                    ) THEN
                        ALTER TABLE ""Appointments"" ADD COLUMN ""PaidByUserId"" uuid NULL;
                    END IF;
                END $$;
            ");

            // Add PaymentReceivedBy column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_schema = 'public'
                        AND LOWER(table_name) = 'appointments' 
                        AND LOWER(column_name) = 'paymentreceivedby'
                    ) THEN
                        ALTER TABLE ""Appointments"" ADD COLUMN ""PaymentReceivedBy"" integer NULL;
                    END IF;
                END $$;
            ");

            // Add PaymentAmount column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_schema = 'public'
                        AND LOWER(table_name) = 'appointments' 
                        AND LOWER(column_name) = 'paymentamount'
                    ) THEN
                        ALTER TABLE ""Appointments"" ADD COLUMN ""PaymentAmount"" numeric NULL;
                    END IF;
                END $$;
            ");

            // Add PaymentMethod column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_schema = 'public'
                        AND LOWER(table_name) = 'appointments' 
                        AND LOWER(column_name) = 'paymentmethod'
                    ) THEN
                        ALTER TABLE ""Appointments"" ADD COLUMN ""PaymentMethod"" integer NULL;
                    END IF;
                END $$;
            ");

            // Create index for PaidByUserId if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM pg_indexes
                        WHERE LOWER(schemaname) = 'public'
                        AND LOWER(tablename) = 'appointments'
                        AND LOWER(indexname) = 'ix_appointments_paidbyuserid'
                    ) THEN
                        CREATE INDEX ""IX_Appointments_PaidByUserId"" ON ""Appointments"" (""PaidByUserId"");
                    END IF;
                END $$;
            ");

            // Add foreign key for PaidByUserId if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM information_schema.table_constraints
                        WHERE LOWER(constraint_schema) = 'public'
                        AND LOWER(constraint_name) = 'fk_appointments_users_paidbyuserid'
                        AND LOWER(table_name) = 'appointments'
                    ) THEN
                        ALTER TABLE ""Appointments"" 
                        ADD CONSTRAINT ""FK_Appointments_Users_PaidByUserId"" 
                        FOREIGN KEY (""PaidByUserId"") 
                        REFERENCES ""Users"" (""Id"") 
                        ON DELETE RESTRICT;
                    END IF;
                END $$;
            ");

            // Add DefaultPaymentReceiverType column to Clinics if it doesn't exist
            // Default value 2 = PaymentReceiverType.Secretary
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_schema = 'public'
                        AND LOWER(table_name) = 'clinics' 
                        AND LOWER(column_name) = 'defaultpaymentreceivertype'
                    ) THEN
                        ALTER TABLE ""Clinics"" ADD COLUMN ""DefaultPaymentReceiverType"" integer NOT NULL DEFAULT 2;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This is a safety migration that should not be rolled back
            // as it only adds columns if they don't exist
            // If you really need to remove these columns, use the original migration rollback:
            // 20260121193310_AddPaymentTrackingFields
            // 20260123011851_AddRoomConfigurationAndPaymentDetails
        }
    }
}
