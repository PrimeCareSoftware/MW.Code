# 🚨 Deployment Checklist - IsException Column Migration

## 📋 Issue Context

**Error:** `42703: column b.IsException does not exist`

**Affected Endpoints:**
- `GET /api/appointments`
- `GET /api/blocked-time-slots/range`

**Root Cause:** Missing `IsException` column in `BlockedTimeSlots` table

---

## ✅ Pre-Deployment Checklist

### 1. Verify Migrations Exist
Ensure the following migrations are present in your repository:
- [ ] `20260210140000_AddRecurrenceSeriesAndExceptions.cs` - Original migration
- [ ] `20260212001705_AddMissingIsExceptionColumn.cs` - Defensive migration

**Verification Command:**
```bash
ls -la src/MedicSoft.Repository/Migrations/PostgreSQL/ | grep -E "20260210140000|20260212001705"
```

### 2. Check Application Auto-Migration Configuration
- [ ] Verify `Program.cs` has auto-migration enabled (lines 876-898)
- [ ] Ensure database connection string is correctly configured
- [ ] Verify database user has CREATE/ALTER permissions

**Location:** `src/MedicSoft.Api/Program.cs` (lines 876-898)

---

## 🚀 Deployment Steps

### Option A: Automatic Migration (Recommended)

The application **automatically applies migrations on startup**. This is the safest and recommended approach.

**Steps:**
1. Deploy the new version with migrations included
2. Start the application
3. Monitor startup logs for migration messages:
   ```
   Aplicando {Count} migrações pendentes...
   Migrações do banco de dados aplicadas com sucesso
   ```
4. Verify the column exists (see verification section below)

**Advantages:**
- ✅ No manual intervention required
- ✅ Migrations applied atomically during startup
- ✅ Errors halt application startup (prevents data corruption)
- ✅ Works consistently across all environments

### Option B: Manual Migration (Emergency/Maintenance Window)

Use this option if you prefer to apply migrations during a maintenance window or if automatic migration fails.

**Steps:**

1. **Using the migration script (easiest):**
   ```bash
   cd /home/runner/work/MW.Code/MW.Code
   ./run-all-migrations.sh "Host=YOUR_HOST;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD"
   ```

2. **Using dotnet ef CLI:**
   ```bash
   cd src/MedicSoft.Api
   dotnet ef database update --connection "Host=YOUR_HOST;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD"
   ```

3. **Using direct SQL (last resort):**
   ```sql
   -- Check if column exists
   SELECT column_name 
   FROM information_schema.columns 
   WHERE table_name = 'BlockedTimeSlots' 
     AND column_name = 'IsException';
   
   -- If not exists, add it
   DO $$
   BEGIN
       IF NOT EXISTS (
           SELECT 1 FROM information_schema.columns 
           WHERE table_name = 'BlockedTimeSlots' 
           AND column_name = 'IsException'
       ) THEN
           ALTER TABLE "BlockedTimeSlots" 
           ADD COLUMN "IsException" boolean NOT NULL DEFAULT false;
           
           RAISE NOTICE 'IsException column added successfully';
       ELSE
           RAISE NOTICE 'IsException column already exists';
       END IF;
   END $$;
   ```

---

## ✅ Post-Deployment Verification

### 1. Verify Column Exists

**Using psql:**
```bash
psql -U postgres -d YOUR_DATABASE -c "SELECT column_name, data_type, is_nullable, column_default FROM information_schema.columns WHERE table_name = 'BlockedTimeSlots' AND column_name = 'IsException';"
```

**Expected Output:**
```
 column_name | data_type | is_nullable | column_default 
-------------+-----------+-------------+----------------
 IsException | boolean   | NO          | false
```

**Using SQL Query:**
```sql
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'BlockedTimeSlots' 
  AND column_name = 'IsException';
```

### 2. Test Affected Endpoints

**Test appointments endpoint:**
```bash
curl -X GET "https://YOUR_API_URL/api/appointments?clinicId=YOUR_CLINIC_ID&date=2026-02-15" \
     -H "Authorization: Bearer YOUR_TOKEN"
```

**Expected:** HTTP 200 with appointments data (not 500 error)

**Test blocked time slots endpoint:**
```bash
curl -X GET "https://YOUR_API_URL/api/blocked-time-slots/range?startDate=2026-02-01&endDate=2026-02-28&clinicId=YOUR_CLINIC_ID" \
     -H "Authorization: Bearer YOUR_TOKEN"
```

**Expected:** HTTP 200 with blocked time slots data (not 500 error)

### 3. Check Application Logs

**Search for migration confirmation:**
```bash
# For systemd services
journalctl -u medicsoft-api -n 100 | grep -i "migração\|migration"

# For Docker containers
docker logs medicsoft-api | grep -i "migração\|migration"
```

**Expected log entries:**
```
Aplicando 2 migrações pendentes...
Migrações do banco de dados aplicadas com sucesso
```

### 4. Monitor for Errors

**Check for column-related errors:**
```bash
# For systemd services
journalctl -u medicsoft-api -n 200 | grep -i "IsException\|42703"

# For Docker containers
docker logs medicsoft-api | grep -i "IsException\|42703"
```

**Expected:** No errors related to `IsException` or error code `42703`

---

## 🔍 Troubleshooting

### Issue: Migration Not Applied Automatically

**Symptoms:**
- Application starts but IsException column doesn't exist
- Still seeing 42703 errors

**Solutions:**
1. Check database connection string in `appsettings.Production.json`
2. Verify database user has CREATE/ALTER permissions
3. Check application logs for migration errors
4. Apply migration manually (see Option B above)

### Issue: Permission Denied

**Error:** `permission denied for table BlockedTimeSlots`

**Solution:**
```sql
-- Grant necessary permissions to application user
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO your_app_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO your_app_user;
```

### Issue: Migration Applied but Still Getting Errors

**Solutions:**
1. Restart the application to reload schema cache
2. Verify the column exists (see verification steps)
3. Check if using connection pooling - may need to clear connection pool
4. Verify Entity Framework model is up to date

---

## 📚 Related Documentation

- **Migration Troubleshooting:** [TROUBLESHOOTING_MIGRATIONS.md](./TROUBLESHOOTING_MIGRATIONS.md) (lines 84-205)
- **Migration Best Practices:** [MIGRATION_BEST_PRACTICES.md](./MIGRATION_BEST_PRACTICES.md)
- **General Migration Guide:** [MIGRATIONS_GUIDE.md](./MIGRATIONS_GUIDE.md)
- **Run All Migrations Script:** [run-all-migrations.sh](./run-all-migrations.sh)

---

## 🎯 Deployment Environments Checklist

Use this checklist to track migration deployment across all environments:

- [ ] **Local Development** - IsException column verified
- [ ] **Staging/QA** - Migration applied and tested
- [ ] **Production (Primary)** - Migration applied and verified
- [ ] **Production (DR/Backup)** - Migration applied if applicable
- [ ] **Customer Instances** (if multi-tenant) - All instances migrated

---

## 📞 Support

If you encounter issues not covered in this checklist:

1. Check [TROUBLESHOOTING_MIGRATIONS.md](./TROUBLESHOOTING_MIGRATIONS.md)
2. Review application logs for detailed error messages
3. Contact the development team with:
   - Error messages and logs
   - Database version and environment details
   - Steps taken so far

---

## 🔒 Security Notes

- ✅ Migrations use defensive SQL with IF NOT EXISTS checks
- ✅ Migrations are idempotent (safe to run multiple times)
- ✅ No data loss - column has DEFAULT value of `false`
- ✅ Backward compatible - existing queries will work

---

**Last Updated:** 2026-02-12  
**Migration Versions:** 20260210140000, 20260212001705  
**Affected Table:** BlockedTimeSlots  
**Affected Column:** IsException
