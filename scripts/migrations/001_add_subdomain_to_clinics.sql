-- Migration: Add Subdomain column to Clinics table
-- Date: 2026-01-11
-- Issue: Column c.Subdomain does not exist error in seed-demo API
--
-- This script adds the missing Subdomain column to the Clinics table.
-- The column is nullable and of type TEXT.

-- Check if column already exists before adding
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_name = 'Clinics'
          AND column_name = 'Subdomain'
    ) THEN
        ALTER TABLE "Clinics" ADD COLUMN "Subdomain" TEXT NULL;
        RAISE NOTICE 'Column Subdomain added to Clinics table successfully';
    ELSE
        RAISE NOTICE 'Column Subdomain already exists in Clinics table, skipping';
    END IF;
END $$;
