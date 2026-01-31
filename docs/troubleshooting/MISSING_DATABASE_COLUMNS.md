# Troubleshooting: Missing Database Columns

## Problem

You may encounter database errors like:

```
Npgsql.PostgresException (0x80004005): 42703: column "IsPaid" of relation "Appointments" does not exist
```

or similar errors for other columns such as `PaymentAmount`, `PaymentMethod`, `PaidAt`, `PaidByUserId`, `PaymentReceivedBy`.

## Root Cause

This issue occurs when:

1. The database was created before certain migrations were added to the codebase
2. The migrations history table (`__EFMigrationsHistory`) incorrectly marks older migrations as "applied" even though their changes weren't actually executed
3. The database schema is out of sync with the Entity Framework Core model

## Solution

### Option 1: Automatic Fix (Recommended)

The application now includes a safety migration (`20260131130000_EnsurePaymentTrackingColumnsExist`) that automatically checks for missing columns and adds them if needed.

**To apply this fix:**

1. Simply restart your application. The migration will run automatically on startup.
2. Check the logs to confirm the migration was applied successfully:
   ```
   [INF] Aplicando migrações do banco de dados...
   [INF] Migrações do banco de dados aplicadas com sucesso
   ```

### Option 2: Manual Migration Application

If the automatic fix doesn't work, you can manually apply migrations:

**Using the migration script:**

```bash
# From the repository root
./run-all-migrations.sh "Host=localhost;Database=primecare;Username=postgres;Password=YourPassword"
```

**Using dotnet ef CLI:**

```bash
# From the repository root
cd src/MedicSoft.Api
dotnet ef database update --context MedicSoftDbContext
```

### Option 3: Fresh Database (Development Only)

If you're in a development environment with no important data, you can recreate the database:

```bash
# Drop and recreate the database
cd src/MedicSoft.Api
dotnet ef database drop --context MedicSoftDbContext --force
dotnet ef database update --context MedicSoftDbContext
```

**⚠️ WARNING:** This will delete all data in the database. Only use this in development environments.

### Option 4: Manual SQL (Advanced)

If none of the above options work, you can manually add the missing columns using SQL:

```sql
-- Add IsPaid column
ALTER TABLE "Appointments" ADD COLUMN IF NOT EXISTS "IsPaid" boolean NOT NULL DEFAULT false;

-- Add PaidAt column
ALTER TABLE "Appointments" ADD COLUMN IF NOT EXISTS "PaidAt" timestamp without time zone NULL;

-- Add PaidByUserId column
ALTER TABLE "Appointments" ADD COLUMN IF NOT EXISTS "PaidByUserId" uuid NULL;

-- Add PaymentReceivedBy column
ALTER TABLE "Appointments" ADD COLUMN IF NOT EXISTS "PaymentReceivedBy" integer NULL;

-- Add PaymentAmount column
ALTER TABLE "Appointments" ADD COLUMN IF NOT EXISTS "PaymentAmount" numeric NULL;

-- Add PaymentMethod column
ALTER TABLE "Appointments" ADD COLUMN IF NOT EXISTS "PaymentMethod" integer NULL;

-- Create index
CREATE INDEX IF NOT EXISTS "IX_Appointments_PaidByUserId" ON "Appointments" ("PaidByUserId");

-- Add foreign key
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.table_constraints
        WHERE constraint_name = 'FK_Appointments_Users_PaidByUserId'
        AND table_name = 'Appointments'
    ) THEN
        ALTER TABLE "Appointments" 
        ADD CONSTRAINT "FK_Appointments_Users_PaidByUserId" 
        FOREIGN KEY ("PaidByUserId") 
        REFERENCES "Users" ("Id") 
        ON DELETE RESTRICT;
    END IF;
END $$;

-- Add DefaultPaymentReceiverType to Clinics
ALTER TABLE "Clinics" ADD COLUMN IF NOT EXISTS "DefaultPaymentReceiverType" integer NOT NULL DEFAULT 2;
```

## Prevention

To prevent this issue in the future:

1. **Always apply migrations after pulling new code:**
   ```bash
   ./run-all-migrations.sh
   ```

2. **Check migration status before starting the application:**
   ```bash
   cd src/MedicSoft.Api
   dotnet ef migrations list --context MedicSoftDbContext
   ```

3. **Never manually modify the `__EFMigrationsHistory` table** unless you know exactly what you're doing

4. **Keep your development database up to date** by regularly applying migrations

## Related Migrations

The following migrations add payment tracking fields to the Appointments table:

- `20260121193310_AddPaymentTrackingFields` - Adds IsPaid, PaidAt, PaidByUserId, PaymentReceivedBy
- `20260123011851_AddRoomConfigurationAndPaymentDetails` - Adds PaymentAmount, PaymentMethod, RoomNumber
- `20260131130000_EnsurePaymentTrackingColumnsExist` - Safety migration that adds missing columns

## Verification

After applying the fix, verify that all columns exist:

```sql
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'Appointments'
AND column_name IN ('IsPaid', 'PaidAt', 'PaidByUserId', 'PaymentReceivedBy', 'PaymentAmount', 'PaymentMethod')
ORDER BY column_name;
```

Expected output:
```
     column_name      |          data_type           | is_nullable
----------------------+------------------------------+-------------
 IsPaid               | boolean                      | NO
 PaidAt               | timestamp without time zone  | YES
 PaidByUserId         | uuid                         | YES
 PaymentAmount        | numeric                      | YES
 PaymentMethod        | integer                      | YES
 PaymentReceivedBy    | integer                      | YES
```

## Getting Help

If you continue to experience issues after trying these solutions:

1. Check the application logs for detailed error messages
2. Verify your database connection string is correct
3. Ensure PostgreSQL is running and accessible
4. Check that the database user has sufficient permissions to ALTER tables
5. Open an issue on GitHub with the full error message and logs
