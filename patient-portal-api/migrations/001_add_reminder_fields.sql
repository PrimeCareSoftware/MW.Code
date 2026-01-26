-- Migration: Add reminder tracking fields to Appointments table
-- Date: 2026-01-26
-- Description: Adds ReminderSent and ReminderSentAt columns to track appointment reminder status

-- Add ReminderSent column (boolean flag)
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'Appointments' 
        AND column_name = 'ReminderSent'
    ) THEN
        ALTER TABLE "Appointments" 
        ADD COLUMN "ReminderSent" boolean DEFAULT false NOT NULL;
        
        -- Create index for performance
        CREATE INDEX "IX_Appointments_ReminderSent" 
        ON "Appointments" ("ReminderSent", "ScheduledDate", "Status");
        
        RAISE NOTICE 'Added ReminderSent column and index';
    ELSE
        RAISE NOTICE 'ReminderSent column already exists';
    END IF;
END $$;

-- Add ReminderSentAt column (timestamp)
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'Appointments' 
        AND column_name = 'ReminderSentAt'
    ) THEN
        ALTER TABLE "Appointments" 
        ADD COLUMN "ReminderSentAt" timestamp with time zone NULL;
        
        RAISE NOTICE 'Added ReminderSentAt column';
    ELSE
        RAISE NOTICE 'ReminderSentAt column already exists';
    END IF;
END $$;

-- Create composite index for reminder query optimization
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE indexname = 'IX_Appointments_ReminderQuery'
    ) THEN
        CREATE INDEX "IX_Appointments_ReminderQuery" 
        ON "Appointments" ("Status", "ScheduledDate", "ScheduledTime", "ReminderSent")
        WHERE "ReminderSent" = false;
        
        RAISE NOTICE 'Created composite index for reminder queries';
    ELSE
        RAISE NOTICE 'Composite index already exists';
    END IF;
END $$;

-- Verification
DO $$
DECLARE
    reminder_sent_exists boolean;
    reminder_sent_at_exists boolean;
BEGIN
    SELECT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'Appointments' AND column_name = 'ReminderSent'
    ) INTO reminder_sent_exists;
    
    SELECT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'Appointments' AND column_name = 'ReminderSentAt'
    ) INTO reminder_sent_at_exists;
    
    IF reminder_sent_exists AND reminder_sent_at_exists THEN
        RAISE NOTICE 'Migration completed successfully!';
        RAISE NOTICE 'Columns ReminderSent and ReminderSentAt are now available';
    ELSE
        RAISE EXCEPTION 'Migration failed - columns not created';
    END IF;
END $$;
