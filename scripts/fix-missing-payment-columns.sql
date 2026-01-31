-- Emergency fix script for missing payment tracking columns in Appointments table
-- This script can be run directly in PostgreSQL if the migration hasn't been applied
-- Run this script as a PostgreSQL superuser or database owner

-- Enable verbose output
\set ON_ERROR_STOP on
\set VERBOSITY verbose

\echo '=========================================='
\echo 'Fix Missing Payment Columns in Appointments'
\echo '=========================================='
\echo ''

-- Check current state
\echo 'Checking current state of Appointments table...'
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_schema = 'public' 
            AND LOWER(table_name) = 'appointments' 
            AND LOWER(column_name) = 'ispaid'
        ) THEN 'IsPaid column EXISTS'
        ELSE 'IsPaid column MISSING'
    END as ispaid_status,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_schema = 'public' 
            AND LOWER(table_name) = 'appointments' 
            AND LOWER(column_name) = 'paidat'
        ) THEN 'PaidAt column EXISTS'
        ELSE 'PaidAt column MISSING'
    END as paidat_status,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_schema = 'public' 
            AND LOWER(table_name) = 'appointments' 
            AND LOWER(column_name) = 'paidbyuserid'
        ) THEN 'PaidByUserId column EXISTS'
        ELSE 'PaidByUserId column MISSING'
    END as paidbyuserid_status,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_schema = 'public' 
            AND LOWER(table_name) = 'appointments' 
            AND LOWER(column_name) = 'paymentreceivedby'
        ) THEN 'PaymentReceivedBy column EXISTS'
        ELSE 'PaymentReceivedBy column MISSING'
    END as paymentreceivedby_status,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_schema = 'public' 
            AND LOWER(table_name) = 'appointments' 
            AND LOWER(column_name) = 'paymentamount'
        ) THEN 'PaymentAmount column EXISTS'
        ELSE 'PaymentAmount column MISSING'
    END as paymentamount_status,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_schema = 'public' 
            AND LOWER(table_name) = 'appointments' 
            AND LOWER(column_name) = 'paymentmethod'
        ) THEN 'PaymentMethod column EXISTS'
        ELSE 'PaymentMethod column MISSING'
    END as paymentmethod_status;

\echo ''
\echo 'Adding missing columns (if any)...'

-- Add IsPaid column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'public'
        AND LOWER(table_name) = 'appointments' 
        AND LOWER(column_name) = 'ispaid'
    ) THEN
        ALTER TABLE "Appointments" ADD COLUMN "IsPaid" boolean NOT NULL DEFAULT false;
        RAISE NOTICE '✓ Added IsPaid column';
    ELSE
        RAISE NOTICE '- IsPaid column already exists';
    END IF;
END $$;

-- Add PaidAt column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'public'
        AND LOWER(table_name) = 'appointments' 
        AND LOWER(column_name) = 'paidat'
    ) THEN
        ALTER TABLE "Appointments" ADD COLUMN "PaidAt" timestamp without time zone NULL;
        RAISE NOTICE '✓ Added PaidAt column';
    ELSE
        RAISE NOTICE '- PaidAt column already exists';
    END IF;
END $$;

-- Add PaidByUserId column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'public'
        AND LOWER(table_name) = 'appointments' 
        AND LOWER(column_name) = 'paidbyuserid'
    ) THEN
        ALTER TABLE "Appointments" ADD COLUMN "PaidByUserId" uuid NULL;
        RAISE NOTICE '✓ Added PaidByUserId column';
    ELSE
        RAISE NOTICE '- PaidByUserId column already exists';
    END IF;
END $$;

-- Add PaymentReceivedBy column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'public'
        AND LOWER(table_name) = 'appointments' 
        AND LOWER(column_name) = 'paymentreceivedby'
    ) THEN
        ALTER TABLE "Appointments" ADD COLUMN "PaymentReceivedBy" integer NULL;
        RAISE NOTICE '✓ Added PaymentReceivedBy column';
    ELSE
        RAISE NOTICE '- PaymentReceivedBy column already exists';
    END IF;
END $$;

-- Add PaymentAmount column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'public'
        AND LOWER(table_name) = 'appointments' 
        AND LOWER(column_name) = 'paymentamount'
    ) THEN
        ALTER TABLE "Appointments" ADD COLUMN "PaymentAmount" numeric NULL;
        RAISE NOTICE '✓ Added PaymentAmount column';
    ELSE
        RAISE NOTICE '- PaymentAmount column already exists';
    END IF;
END $$;

-- Add PaymentMethod column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'public'
        AND LOWER(table_name) = 'appointments' 
        AND LOWER(column_name) = 'paymentmethod'
    ) THEN
        ALTER TABLE "Appointments" ADD COLUMN "PaymentMethod" integer NULL;
        RAISE NOTICE '✓ Added PaymentMethod column';
    ELSE
        RAISE NOTICE '- PaymentMethod column already exists';
    END IF;
END $$;

-- Create index for PaidByUserId if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_indexes
        WHERE schemaname = 'public'
        AND LOWER(tablename) = 'appointments'
        AND LOWER(indexname) = 'ix_appointments_paidbyuserid'
    ) THEN
        CREATE INDEX "IX_Appointments_PaidByUserId" ON "Appointments" ("PaidByUserId");
        RAISE NOTICE '✓ Created index IX_Appointments_PaidByUserId';
    ELSE
        RAISE NOTICE '- Index IX_Appointments_PaidByUserId already exists';
    END IF;
END $$;

-- Add foreign key for PaidByUserId if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.table_constraints
        WHERE constraint_schema = 'public'
        AND LOWER(constraint_name) = 'fk_appointments_users_paidbyuserid'
        AND LOWER(table_name) = 'appointments'
    ) THEN
        ALTER TABLE "Appointments" 
        ADD CONSTRAINT "FK_Appointments_Users_PaidByUserId" 
        FOREIGN KEY ("PaidByUserId") 
        REFERENCES "Users" ("Id") 
        ON DELETE RESTRICT;
        RAISE NOTICE '✓ Created foreign key FK_Appointments_Users_PaidByUserId';
    ELSE
        RAISE NOTICE '- Foreign key FK_Appointments_Users_PaidByUserId already exists';
    END IF;
END $$;

-- Add DefaultPaymentReceiverType column to Clinics if it doesn't exist
-- Default value 2 = PaymentReceiverType.Secretary
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'public'
        AND LOWER(table_name) = 'clinics' 
        AND LOWER(column_name) = 'defaultpaymentreceivertype'
    ) THEN
        ALTER TABLE "Clinics" ADD COLUMN "DefaultPaymentReceiverType" integer NOT NULL DEFAULT 2;
        RAISE NOTICE '✓ Added DefaultPaymentReceiverType column to Clinics';
    ELSE
        RAISE NOTICE '- DefaultPaymentReceiverType column already exists in Clinics';
    END IF;
END $$;

\echo ''
\echo 'Verifying final state...'
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public'
  AND LOWER(table_name) = 'appointments'
  AND LOWER(column_name) IN ('ispaid', 'paidat', 'paidbyuserid', 'paymentreceivedby', 'paymentamount', 'paymentmethod')
ORDER BY column_name;

\echo ''
\echo '=========================================='
\echo 'Fix completed successfully!'
\echo '=========================================='
\echo ''
\echo 'You can now:'
\echo '1. Restart your application'
\echo '2. Try seeding demo data again: POST /api/DataSeeder/seed-demo'
\echo ''
