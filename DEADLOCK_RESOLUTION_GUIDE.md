# DEADLOCK RESOLUTION GUIDE

**Issue:** Multiple application instances trying to run migrations simultaneously  
**Error Code:** `40P01: deadlock detected`  
**Location:** DELETE FROM "ReportTemplates" during migration `20260227185528_AddEmailConfirmationToOwners`

---

## ⚡ QUICK FIX (Recommended)

Run the safe migration script that ensures only one instance applies migrations:

```bash
cd /Users/igorlessarobainadesouza/Documents/MW.Code
./safe_migrate.sh
```

This script will:
- Check for running instances and offer to kill them
- Acquire a lock to prevent concurrent migrations
- Apply migrations in a single process
- Clean up automatically

---

## 🔧 MANUAL RESOLUTION

### Step 1: Kill All Running Instances

```bash
# Find all running instances
ps aux | grep -i "dotnet.*MedicSoft" | grep -v grep

# Kill them all
pkill -f "dotnet.*MedicSoft"

# Or kill by specific PIDs (from the error log you see: 9971, 10605, 14145, 16742)
kill -9 9971 10605 14145 16742 2>/dev/null || true
```

### Step 2: Check Database Connections

Connect to your PostgreSQL database and run:

```sql
-- Check active connections
SELECT pid, usename, application_name, state, query_start
FROM pg_stat_activity 
WHERE datname = current_database()
  AND pid <> pg_backend_pid();

-- Kill connections if needed (CAREFUL!)
SELECT pg_terminate_backend(pid) 
FROM pg_stat_activity 
WHERE datname = current_database()
  AND pid <> pg_backend_pid()
  AND usename != 'postgres';
```

Or use the provided SQL file:
```bash
psql -U your_user -d your_database -f check_db_locks.sql
```

### Step 3: Run Single Instance

```bash
cd /Users/igorlessarobainadesouza/Documents/MW.Code/src/MedicSoft.Api

# Build first
dotnet build

# Run - this will apply migrations
dotnet run
```

**IMPORTANT:** Run ONLY ONE terminal/instance!

---

## 🛡️ PREVENTION

### Option 1: Use the Safe Script (Recommended)

Always use `./safe_migrate.sh` instead of `dotnet run` when you need to apply migrations.

### Option 2: Disable Auto-Migration in Program.cs

Modify [Program.cs](MedicSoft.Api/Program.cs) to NOT automatically apply migrations on startup:

```csharp
// COMMENT OUT OR REMOVE THIS SECTION (around line 1175)
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
//     context.Database.Migrate();  // <-- THIS LINE
// }
```

Then apply migrations manually when needed:
```bash
dotnet ef database update --context MedicSoftDbContext
```

### Option 3: Add Distributed Lock (Production-Ready)

Implement a distributed lock using Redis or database advisory locks before running migrations. Example pseudocode:

```csharp
var lockAcquired = await TryAcquireMigrationLockAsync();
if (lockAcquired)
{
    try 
    {
        context.Database.Migrate();
    }
    finally 
    {
        await ReleaseMigrationLockAsync();
    }
}
```

---

## 📊 DIAGNOSIS TOOLS

### Created Files:

1. **[safe_migrate.sh](safe_migrate.sh)** - Safe migration script with locking
2. **[check_db_locks.sql](check_db_locks.sql)** - Check database locks and connections
3. **[kill_db_connections.sql](kill_db_connections.sql)** - Emergency connection killer

### Check Current State:

```bash
# No running instances?
ps aux | grep -i medicsoft | grep -v grep

# Check lock file
ls -la /tmp/medicsoft_migration.lock

# View recent logs
tail -f src/MedicSoft.Api/Logs/primecare-errors-*.log
```

---

## 🎯 ROOT CAUSE

The migration `20260227185528_AddEmailConfirmationToOwners` contains:
- Multiple DELETE statements for ReportTemplates
- Multiple DELETE statements for WidgetTemplates
- Data seeding operations

When multiple instances run simultaneously:
1. **Process A** starts DELETE FROM "ReportTemplates" → acquires locks
2. **Process B** tries same DELETE → waits for Process A
3. **Process A** tries to ALTER table → needs lock held by Process B
4. **DEADLOCK** → PostgreSQL kills one process

---

## ✅ VERIFICATION

After fixing, verify success:

```bash
# Check logs for success message
tail -n 50 src/MedicSoft.Api/Logs/primecare-*.log | grep -i "sucesso\|success"

# Verify migration was applied
dotnet ef migrations list --context MedicSoftDbContext

# Check database
psql -U your_user -d your_database -c "SELECT * FROM \"__EFMigrationsHistory\" ORDER BY \"MigrationId\" DESC LIMIT 5;"
```

Expected output should include:
```
20260227185528_AddEmailConfirmationToOwners
```

---

## 🆘 STILL STUCK?

1. **Database corrupted?** Consider backup and restore
2. **Migration half-applied?** Check which steps completed:
   ```sql
   SELECT * FROM "__EFMigrationsHistory" 
   WHERE "MigrationId" = '20260227185528_AddEmailConfirmationToOwners';
   ```
3. **Need to rollback?**
   ```bash
   dotnet ef database update <previous_migration_name> --context MedicSoftDbContext
   ```

---

## 📝 SUMMARY

**Problem:** Multiple instances → Concurrent migrations → Deadlock  
**Solution:** Run single instance with lock mechanism  
**Tool:** `./safe_migrate.sh`  
**Prevention:** Use distributed locking or manual migrations
