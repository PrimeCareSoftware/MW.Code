# Quick Fix: IsPaid Column Missing Error

## 🚨 Error Message

```
Npgsql.PostgresException (0x80004005): 42703: column "IsPaid" of relation "Appointments" does not exist
```

## ⚡ Quick Solution

**Option 1: Restart the Application (Easiest)**

The application automatically applies database migrations on startup. Simply restart your application:

```bash
# Stop the application (Ctrl+C if running in terminal)
# Then start it again
cd src/MedicSoft.Api
dotnet run
```

The migration will be applied automatically and you'll see:
```
Aplicando migrações do banco de dados...
Migrações do banco de dados aplicadas com sucesso
```

**Option 2: Apply Migrations Manually**

```bash
# From the repository root
./run-all-migrations.sh
```

Or with a custom connection string:

```bash
./run-all-migrations.sh "Host=localhost;Database=primecare;Username=postgres;Password=YourPassword"
```

**Option 3: Using dotnet ef CLI**

```bash
cd src/MedicSoft.Api
dotnet ef database update --context MedicSoftDbContext
```

## ✅ Verification

After applying the migration, verify the columns exist:

**Option 1: Run the verification script**

```bash
psql -U postgres -d primecare -f scripts/verify-database-schema.sql
```

This script will check all payment tracking columns and show clear status messages.

**Option 2: Manual SQL query**

```sql
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'Appointments'
  AND column_name IN ('IsPaid', 'PaidAt', 'PaidByUserId', 'PaymentReceivedBy', 'PaymentAmount', 'PaymentMethod')
ORDER BY column_name;
```

Expected: All 6 columns should be listed.

## 📚 More Information

- **Detailed Troubleshooting Guide:** [docs/troubleshooting/MISSING_DATABASE_COLUMNS.md](docs/troubleshooting/MISSING_DATABASE_COLUMNS.md)
- **Complete Migration Guide:** [MIGRATIONS_GUIDE.md](MIGRATIONS_GUIDE.md)
- **Technical Details:** [FIX_SUMMARY_ISPAID_COLUMN.md](FIX_SUMMARY_ISPAID_COLUMN.md)

## 🤔 Why This Happens

This error occurs when:
1. The database was created before payment tracking features were added
2. Migrations have not been applied yet
3. The database schema is out of sync with the application code

## 🛡️ Prevention

To prevent this in the future:
- Always run migrations after pulling new code: `./run-all-migrations.sh`
- The application applies migrations automatically on startup (no manual action needed)
- Keep your development database up to date

## 🆘 Still Having Issues?

If the error persists after trying these solutions:

1. Check the application logs in the `logs/` directory
2. Verify PostgreSQL is running: `podman ps | grep postgres` or `docker ps | grep postgres`
3. Check your database connection string in `appsettings.Development.json`
4. See the detailed troubleshooting guide: [docs/troubleshooting/MISSING_DATABASE_COLUMNS.md](docs/troubleshooting/MISSING_DATABASE_COLUMNS.md)
