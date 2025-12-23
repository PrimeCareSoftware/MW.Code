# Fix: Token Validation and Creation Issues

## Problem Statement (Portuguese)
A API que valida token está sempre trazendo null, e a API responsável por criar os tokens não está alimentando a tabela. Acredito que possa ser esse o problema.

**Translation:** The API that validates tokens is always returning null, and the API responsible for creating tokens is not populating the table. I believe this might be the problem.

## Root Cause Analysis

The primary issue was in the database initialization code in `Program.cs`:

### Before (Incorrect)
```csharp
// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
    try
    {
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
    }
}
```

### Problem with `EnsureCreated()`
- `EnsureCreated()` creates the database schema directly from the current model
- It **does NOT** run migrations
- It **does NOT** track migration history
- This causes schema drift between migrations and actual database
- Migration-specific SQL commands (like the `AddSessionManagement` migration) are not executed

### After (Correct)
```csharp
// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed: {Message}", ex.Message);
        Console.WriteLine($"Database migration failed: {ex.Message}");
    }
}
```

## Impact of the Fix

### 1. Session Management
The `AddSessionManagement` migration (20251110193734) adds `CurrentSessionId` column to both `Users` and `Owners` tables:
- Without this column, session tracking doesn't work
- Session validation always fails
- Tokens appear invalid even when they're technically valid

### 2. PasswordResetTokens Table
- The table structure might have been created by `EnsureCreated()` but without proper migration tracking
- Any future migrations that modify this table would fail or be skipped
- The table should work correctly after applying migrations

### 3. Token Validation
Token validation (`/api/auth/validate` endpoint) can return null for several reasons:
- Token expired
- Invalid signature
- Missing or invalid claims
- **Session validation failure due to missing CurrentSessionId column** ← Fixed by this change

## How to Verify the Fix

### 1. Check Database Migrations
Connect to your PostgreSQL database and verify the migrations table:

```sql
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
```

You should see all migrations applied, including:
- `20251103174434_InitialPostgreSQL`
- `20251106193124_AddWaitingQueue`
- `20251110193734_AddSessionManagement` ← Most important for this fix
- `20251119194448_AddOwnerClinicLink`
- `20251129140804_AddNotificationsTable`

### 2. Verify CurrentSessionId Column
Check if the `CurrentSessionId` column exists:

```sql
SELECT column_name, data_type, character_maximum_length
FROM information_schema.columns
WHERE table_name = 'Users' AND column_name = 'CurrentSessionId';

SELECT column_name, data_type, character_maximum_length
FROM information_schema.columns
WHERE table_name = 'Owners' AND column_name = 'CurrentSessionId';
```

Both queries should return a result showing `CurrentSessionId` as `character varying(200)`.

### 3. Test Token Creation and Validation

#### a) Login (Creates Token and Session)
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: demo-clinic-001" \
  -d '{
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "demo-clinic-001"
  }'
```

Expected response: JWT token with session information

#### b) Validate Token
```bash
TOKEN="<your-jwt-token-from-step-a>"

curl -X POST http://localhost:5000/api/auth/validate \
  -H "Content-Type: application/json" \
  -d "{
    \"token\": \"$TOKEN\"
  }"
```

Expected response:
```json
{
  "isValid": true,
  "username": "admin",
  "role": "Admin",
  "tenantId": "demo-clinic-001"
}
```

#### c) Validate Session
```bash
curl -X POST http://localhost:5000/api/auth/validate-session \
  -H "Content-Type: application/json" \
  -d "{
    \"token\": \"$TOKEN\"
  }"
```

Expected response:
```json
{
  "isValid": true,
  "message": "Sessão válida"
}
```

#### d) Test Password Reset Token Creation
```bash
curl -X POST http://localhost:5000/api/password-recovery/request \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: demo-clinic-001" \
  -d '{
    "usernameOrEmail": "admin",
    "method": 0
  }'
```

Then verify the token was saved to the database:
```sql
SELECT "Id", "UserId", "Token", "IsUsed", "IsVerified", "ExpiresAt", "CreatedAt"
FROM "PasswordResetTokens"
ORDER BY "CreatedAt" DESC
LIMIT 5;
```

## Additional Improvements

### Better Error Logging
The fix also includes better logging for database migration issues:
- Logs when migrations start
- Logs success confirmation
- Logs detailed error information if migration fails

### Migration Tracking
With `Migrate()`, the `__EFMigrationsHistory` table is properly maintained, ensuring:
- Consistent schema across environments
- Ability to roll back migrations if needed
- Clear audit trail of schema changes

## Prevention

To prevent similar issues in the future:

1. **Always use `Migrate()` for production applications**
   - `EnsureCreated()` is only for testing/prototyping
   - `Migrate()` ensures proper schema versioning

2. **Test migrations in a separate environment first**
   - Verify migrations apply cleanly
   - Check for any data loss or conflicts

3. **Monitor migration logs**
   - Ensure migrations complete successfully
   - Investigate any warnings or errors

4. **Use idempotent migrations**
   - Like the `AddSessionManagement` migration
   - Check if changes already exist before applying
   - Prevents errors when re-running migrations

## Related Files Modified

- `src/MedicSoft.Api/Program.cs` - Changed database initialization from `EnsureCreated()` to `Migrate()`

## Testing

All 778 existing tests pass after this change:
```bash
cd /home/runner/work/MW.Code/MW.Code
dotnet test tests/MedicSoft.Test/MedicSoft.Test.csproj
```

Result: ✅ Passed: 778, Failed: 0, Skipped: 0
