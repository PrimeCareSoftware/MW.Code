-- Script to verify database schema for common issues
-- Run this with: psql -U postgres -d primecare -f scripts/verify-database-schema.sql

\echo '========================================='
\echo 'PrimeCare Database Schema Verification'
\echo '========================================='
\echo ''

-- Check if Appointments table exists
\echo 'Checking if Appointments table exists...'
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.tables 
            WHERE table_schema = 'public' 
            AND table_name = 'Appointments'
        ) THEN '✓ Appointments table exists'
        ELSE '✗ ERROR: Appointments table does not exist'
    END as status;

\echo ''
\echo 'Checking payment tracking columns in Appointments table...'

-- Check for payment tracking columns
SELECT 
    column_name,
    data_type,
    is_nullable,
    '✓ Present' as status
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'Appointments'
  AND column_name IN ('IsPaid', 'PaidAt', 'PaidByUserId', 'PaymentReceivedBy', 'PaymentAmount', 'PaymentMethod')
ORDER BY 
    CASE column_name
        WHEN 'IsPaid' THEN 1
        WHEN 'PaidAt' THEN 2
        WHEN 'PaidByUserId' THEN 3
        WHEN 'PaymentReceivedBy' THEN 4
        WHEN 'PaymentAmount' THEN 5
        WHEN 'PaymentMethod' THEN 6
    END;

\echo ''
\echo 'Expected columns: IsPaid, PaidAt, PaidByUserId, PaymentReceivedBy, PaymentAmount, PaymentMethod'
\echo ''

-- Count how many payment tracking columns exist
SELECT 
    CASE 
        WHEN COUNT(*) = 6 THEN '✓ All payment tracking columns are present (6/6)'
        WHEN COUNT(*) = 0 THEN '✗ ERROR: No payment tracking columns found (0/6)'
        ELSE '⚠ WARNING: Only ' || COUNT(*) || ' out of 6 payment tracking columns found'
    END as summary
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'Appointments'
  AND column_name IN ('IsPaid', 'PaidAt', 'PaidByUserId', 'PaymentReceivedBy', 'PaymentAmount', 'PaymentMethod');

\echo ''
\echo 'Checking for payment tracking foreign key...'

-- Check for foreign key
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE constraint_name = 'FK_Appointments_Users_PaidByUserId'
            AND table_name = 'Appointments'
            AND table_schema = 'public'
        ) THEN '✓ Foreign key FK_Appointments_Users_PaidByUserId exists'
        ELSE '✗ WARNING: Foreign key FK_Appointments_Users_PaidByUserId does not exist'
    END as status;

\echo ''
\echo 'Checking for payment tracking index...'

-- Check for index
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1
            FROM pg_indexes
            WHERE tablename = 'Appointments'
            AND indexname = 'IX_Appointments_PaidByUserId'
        ) THEN '✓ Index IX_Appointments_PaidByUserId exists'
        ELSE '✗ WARNING: Index IX_Appointments_PaidByUserId does not exist'
    END as status;

\echo ''
\echo 'Checking DefaultPaymentReceiverType in Clinics table...'

-- Check for DefaultPaymentReceiverType in Clinics
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_schema = 'public' 
            AND table_name = 'Clinics'
            AND column_name = 'DefaultPaymentReceiverType'
        ) THEN '✓ DefaultPaymentReceiverType column exists in Clinics table'
        ELSE '✗ WARNING: DefaultPaymentReceiverType column does not exist in Clinics table'
    END as status;

\echo ''
\echo '========================================='
\echo 'Verification Complete'
\echo '========================================='
\echo ''
\echo 'If any errors were found, please run migrations:'
\echo '  Option 1: ./run-all-migrations.sh'
\echo '  Option 2: cd src/MedicSoft.Api && dotnet ef database update'
\echo '  Option 3: Restart the application (migrations apply automatically)'
\echo ''
\echo 'For help, see: QUICK_FIX_ISPAID_ERROR.md'
\echo ''
