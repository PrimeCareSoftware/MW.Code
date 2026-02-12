using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    /// <summary>
    /// Defensive migration to ensure IsException column exists in BlockedTimeSlots table.
    /// 
    /// CONTEXT: This migration was created to address a critical issue where the IsException column
    /// was missing from the BlockedTimeSlots table in some database instances, causing 500 errors.
    /// 
    /// The original migration 20260210140000_AddRecurrenceSeriesAndExceptions.cs includes the
    /// IsException column in its Up() method. However, due to potential deployment issues or
    /// database inconsistencies, some environments may not have this column, resulting in errors:
    /// "42703: column b.IsException does not exist"
    /// 
    /// This migration uses defensive SQL patterns with IF NOT EXISTS checks to ensure:
    /// 1. The column is added if it doesn't exist
    /// 2. The migration can safely run on databases that already have the column
    /// 3. Backward compatibility is maintained
    /// 
    /// Related Migration: 20260210140000_AddRecurrenceSeriesAndExceptions.cs
    /// </summary>
    public partial class AddMissingIsExceptionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Defensive check: Add IsException column only if it doesn't already exist
            // This ensures the migration is idempotent and can run on databases that may have
            // already received this column through other means (manual fix, different migration path)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Check if the BlockedTimeSlots table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'BlockedTimeSlots' 
                        AND table_schema = 'public'
                    ) THEN
                        -- Check if IsException column does NOT exist
                        IF NOT EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'BlockedTimeSlots' 
                            AND column_name = 'IsException'
                            AND table_schema = 'public'
                        ) THEN
                            -- Add the missing column
                            ALTER TABLE ""BlockedTimeSlots"" 
                            ADD COLUMN ""IsException"" boolean NOT NULL DEFAULT false;
                            
                            RAISE NOTICE 'IsException column added to BlockedTimeSlots table';
                        ELSE
                            RAISE NOTICE 'IsException column already exists in BlockedTimeSlots table - skipping';
                        END IF;
                    ELSE
                        RAISE WARNING 'BlockedTimeSlots table does not exist - skipping IsException column addition';
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Defensive check: Drop IsException column only if it exists
            // This ensures the Down migration is also idempotent
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Check if the BlockedTimeSlots table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'BlockedTimeSlots' 
                        AND table_schema = 'public'
                    ) THEN
                        -- Check if IsException column EXISTS
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'BlockedTimeSlots' 
                            AND column_name = 'IsException'
                            AND table_schema = 'public'
                        ) THEN
                            -- Drop the column
                            ALTER TABLE ""BlockedTimeSlots"" 
                            DROP COLUMN ""IsException"";
                            
                            RAISE NOTICE 'IsException column dropped from BlockedTimeSlots table';
                        ELSE
                            RAISE NOTICE 'IsException column does not exist in BlockedTimeSlots table - skipping';
                        END IF;
                    ELSE
                        RAISE WARNING 'BlockedTimeSlots table does not exist - skipping IsException column removal';
                    END IF;
                END $$;
            ");
        }
    }
}
