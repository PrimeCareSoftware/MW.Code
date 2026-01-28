# SQL Query Security Guidelines - Phase 3 Analytics

## 🔒 Overview

This document outlines the security rules and guidelines for writing SQL queries in custom dashboard widgets. The system implements multiple layers of security to prevent SQL injection, unauthorized data access, and database manipulation.

---

## ✅ Security Validation Layers

### Layer 1: Query Type Validation
- **Rule:** Only `SELECT` statements are allowed
- **Check:** Query must start with `SELECT` keyword
- **Purpose:** Prevent data modification operations

### Layer 2: Dangerous Keyword Blocking
- **Blocked Keywords:**
  - Data Modification: `INSERT`, `UPDATE`, `DELETE`, `TRUNCATE`
  - Schema Modification: `DROP`, `CREATE`, `ALTER`
  - Execution: `EXEC`, `EXECUTE`, `CALL`, `PROCEDURE`
  - Security: `GRANT`, `REVOKE`
  - Advanced: `MERGE`
  
- **Case-Insensitive:** Checks uppercase version of query
- **Purpose:** Prevent destructive operations

### Layer 3: Multiple Statement Detection
- **Rule:** No semicolons (`;`) allowed
- **Purpose:** Prevent query chaining/stacking attacks
- **Example Blocked:**
  ```sql
  SELECT * FROM Users; DROP TABLE Users;
  ```

### Layer 4: SQL Comment Blocking
- **Blocked Patterns:**
  - Line comments: `--`
  - Block comments: `/* */`
- **Purpose:** Prevent comment-based injection techniques
- **Example Blocked:**
  ```sql
  SELECT * FROM Users WHERE Id = 1 -- AND Status = 'Active'
  ```

### Layer 5: Execution Limits
- **Timeout:** 30 seconds maximum
- **Row Limit:** 10,000 rows maximum
- **Purpose:** Prevent denial of service and memory exhaustion

### Layer 6: Connection Safety
- **Read-Only:** Uses standard connection (no elevated privileges)
- **Connection Pool:** Managed by Entity Framework Core
- **Auto-Close:** Connection closed automatically after execution

---

## ✅ Allowed SQL Features

### SELECT Statements
```sql
-- ✅ ALLOWED: Basic SELECT
SELECT "Id", "Name", "Email"
FROM "Users"

-- ✅ ALLOWED: WITH clause (CTEs)
WITH ActiveUsers AS (
  SELECT * FROM "Users" WHERE "IsActive" = true
)
SELECT * FROM ActiveUsers

-- ✅ ALLOWED: Subqueries
SELECT c."Name",
  (SELECT COUNT(*) FROM "Appointments" WHERE "ClinicId" = c."Id") as appointment_count
FROM "Clinics" c
```

### JOIN Operations
```sql
-- ✅ ALLOWED: INNER JOIN
SELECT c."Name", cs."Status"
FROM "Clinics" c
INNER JOIN "ClinicSubscriptions" cs ON c."Id" = cs."ClinicId"

-- ✅ ALLOWED: LEFT/RIGHT JOIN
SELECT c."Name", COUNT(p."Id") as patient_count
FROM "Clinics" c
LEFT JOIN "Patients" p ON c."Id" = p."ClinicId"
GROUP BY c."Name"

-- ✅ ALLOWED: Multiple JOINs
SELECT 
  c."Name" as clinic,
  u."Name" as user,
  a."AppointmentDate"
FROM "Appointments" a
INNER JOIN "Clinics" c ON a."ClinicId" = c."Id"
INNER JOIN "Users" u ON a."UserId" = u."Id"
```

### WHERE Clauses
```sql
-- ✅ ALLOWED: Filtering
SELECT * FROM "Clinics"
WHERE "IsActive" = true
  AND "CreatedAt" >= CURRENT_DATE - INTERVAL '1 year'

-- ✅ ALLOWED: IN/NOT IN
SELECT * FROM "Patients"
WHERE "ClinicId" IN (1, 2, 3)

-- ✅ ALLOWED: LIKE pattern matching
SELECT * FROM "Users"
WHERE "Email" LIKE '%@medicwarehouse.com'

-- ✅ ALLOWED: BETWEEN
SELECT * FROM "Appointments"
WHERE "AppointmentDate" BETWEEN '2026-01-01' AND '2026-12-31'

-- ✅ ALLOWED: IS NULL/IS NOT NULL
SELECT * FROM "Patients"
WHERE "Email" IS NOT NULL
```

### Aggregate Functions
```sql
-- ✅ ALLOWED: COUNT, SUM, AVG, MIN, MAX
SELECT 
  COUNT(*) as total_patients,
  AVG("Age") as avg_age,
  MIN("CreatedAt") as first_registration,
  MAX("CreatedAt") as last_registration
FROM "Patients"

-- ✅ ALLOWED: GROUP BY
SELECT 
  "Status",
  COUNT(*) as count
FROM "Appointments"
GROUP BY "Status"

-- ✅ ALLOWED: HAVING
SELECT 
  "ClinicId",
  COUNT(*) as patient_count
FROM "Patients"
GROUP BY "ClinicId"
HAVING COUNT(*) > 100
```

### Date/Time Functions
```sql
-- ✅ ALLOWED: Date functions
SELECT 
  DATE_TRUNC('month', "CreatedAt") as month,
  COUNT(*) as count
FROM "Clinics"
WHERE "CreatedAt" >= NOW() - INTERVAL '12 months'
GROUP BY DATE_TRUNC('month', "CreatedAt")

-- ✅ ALLOWED: EXTRACT
SELECT 
  EXTRACT(YEAR FROM "AppointmentDate") as year,
  EXTRACT(MONTH FROM "AppointmentDate") as month,
  COUNT(*) as count
FROM "Appointments"
GROUP BY year, month
```

### ORDER BY and LIMIT
```sql
-- ✅ ALLOWED: Sorting
SELECT "Name", "CreatedAt"
FROM "Clinics"
ORDER BY "CreatedAt" DESC

-- ✅ ALLOWED: LIMIT and OFFSET
SELECT "Name", "Email"
FROM "Users"
ORDER BY "CreatedAt" DESC
LIMIT 100 OFFSET 0

-- ✅ ALLOWED: Top N queries
SELECT "TradeName", COUNT(p."Id") as patient_count
FROM "Clinics" c
LEFT JOIN "Patients" p ON c."Id" = p."ClinicId"
GROUP BY c."TradeName"
ORDER BY patient_count DESC
LIMIT 10
```

### CASE Expressions
```sql
-- ✅ ALLOWED: Conditional logic
SELECT 
  "Name",
  CASE 
    WHEN "Status" = 'Active' THEN 'Active Customer'
    WHEN "Status" = 'Trial' THEN 'Trial Period'
    ELSE 'Inactive'
  END as status_label
FROM "ClinicSubscriptions"
```

### Window Functions
```sql
-- ✅ ALLOWED: ROW_NUMBER, RANK, etc.
SELECT 
  "Name",
  "MonthlyPrice",
  ROW_NUMBER() OVER (ORDER BY "MonthlyPrice" DESC) as price_rank
FROM "Plans"

-- ✅ ALLOWED: Aggregate window functions
SELECT 
  "ClinicId",
  "AppointmentDate",
  COUNT(*) OVER (PARTITION BY "ClinicId") as clinic_total_appointments
FROM "Appointments"
```

---

## ❌ Prohibited SQL Features

### Data Modification
```sql
-- ❌ BLOCKED: INSERT
INSERT INTO "Users" ("Name", "Email") VALUES ('Test', 'test@test.com')

-- ❌ BLOCKED: UPDATE
UPDATE "Clinics" SET "IsActive" = false WHERE "Id" = 1

-- ❌ BLOCKED: DELETE
DELETE FROM "Patients" WHERE "ClinicId" = 1

-- ❌ BLOCKED: TRUNCATE
TRUNCATE TABLE "Users"
```

### Schema Modification
```sql
-- ❌ BLOCKED: DROP
DROP TABLE "Patients"

-- ❌ BLOCKED: CREATE
CREATE TABLE "NewTable" ("Id" INT)

-- ❌ BLOCKED: ALTER
ALTER TABLE "Users" ADD COLUMN "NewField" VARCHAR(100)
```

### Execution Commands
```sql
-- ❌ BLOCKED: EXEC/EXECUTE
EXEC sp_executesql 'SELECT * FROM Users'
EXECUTE sp_configure

-- ❌ BLOCKED: CALL procedures
CALL calculate_mrr()
```

### Security Commands
```sql
-- ❌ BLOCKED: GRANT
GRANT ALL PRIVILEGES ON "Users" TO public

-- ❌ BLOCKED: REVOKE
REVOKE SELECT ON "Patients" FROM public
```

### Multiple Statements
```sql
-- ❌ BLOCKED: Semicolon-separated queries
SELECT * FROM "Users"; SELECT * FROM "Patients"

-- ❌ BLOCKED: Query chaining
SELECT * FROM "Clinics"; DROP TABLE "Clinics"
```

### SQL Comments
```sql
-- ❌ BLOCKED: Line comments
SELECT * FROM "Users" -- WHERE "IsActive" = true

-- ❌ BLOCKED: Block comments
SELECT * FROM "Users" /* dangerous comment */
```

---

## 🛡️ Injection Prevention

### Parameterization (Not Applicable)
- **Note:** Custom queries do NOT support parameters
- All values must be hard-coded in query
- System validates entire query string

### Safe String Literals
```sql
-- ✅ SAFE: Use single quotes for strings
SELECT * FROM "Users" WHERE "Email" = 'user@example.com'

-- ✅ SAFE: Escape single quotes
SELECT * FROM "Users" WHERE "Name" = 'O''Brien'
```

### Table and Column Names
```sql
-- ✅ SAFE: Use double quotes for identifiers (PostgreSQL)
SELECT "Id", "Name", "Email" FROM "Users"

-- ❌ UNSAFE: Unquoted identifiers (may cause errors)
SELECT Id, Name, Email FROM Users
```

---

## ⚡ Performance Best Practices

### 1. Always Use WHERE Clauses
```sql
-- ❌ BAD: No filtering (slow, returns all data)
SELECT * FROM "Appointments"

-- ✅ GOOD: Filter by date
SELECT * FROM "Appointments"
WHERE "AppointmentDate" >= CURRENT_DATE - INTERVAL '30 days'
```

### 2. Aggregate Large Datasets
```sql
-- ❌ BAD: Returns thousands of rows
SELECT "ClinicId", "CreatedAt" FROM "Patients"

-- ✅ GOOD: Aggregate by month
SELECT 
  DATE_TRUNC('month', "CreatedAt") as month,
  COUNT(*) as patient_count
FROM "Patients"
GROUP BY DATE_TRUNC('month', "CreatedAt")
```

### 3. Use LIMIT for Large Tables
```sql
-- ❌ BAD: No limit
SELECT * FROM "Patients" ORDER BY "CreatedAt" DESC

-- ✅ GOOD: Top 100 records
SELECT * FROM "Patients" 
ORDER BY "CreatedAt" DESC 
LIMIT 100
```

### 4. Optimize JOINs
```sql
-- ✅ GOOD: Use INNER JOIN when possible (faster)
SELECT c."Name", cs."Status"
FROM "Clinics" c
INNER JOIN "ClinicSubscriptions" cs ON c."Id" = cs."ClinicId"

-- ⚠️ CAUTION: LEFT JOIN returns more rows
SELECT c."Name", COUNT(p."Id") as patient_count
FROM "Clinics" c
LEFT JOIN "Patients" p ON c."Id" = p."ClinicId"
GROUP BY c."Name"
```

### 5. Avoid SELECT *
```sql
-- ❌ BAD: Returns all columns (slower, more data)
SELECT * FROM "Patients"

-- ✅ GOOD: Select only needed columns
SELECT "Id", "Name", "Email" FROM "Patients"
```

---

## 🧪 Testing Queries

### Step 1: Test in Database Client
Before adding query to widget:
1. Open pgAdmin, DBeaver, or similar tool
2. Connect to database (read-only if possible)
3. Run query and verify results
4. Check execution time (should be < 5 seconds)

### Step 2: Use EXPLAIN ANALYZE
```sql
-- Get query execution plan
EXPLAIN ANALYZE
SELECT 
  DATE_TRUNC('month', "CreatedAt") as month,
  COUNT(*) as count
FROM "Clinics"
WHERE "CreatedAt" >= CURRENT_DATE - INTERVAL '12 months'
GROUP BY DATE_TRUNC('month', "CreatedAt");
```

Look for:
- **Seq Scan:** May need index
- **Execution Time:** Should be < 5000ms
- **Rows:** Should match expected count

### Step 3: Validate Query Safety
Run through security checklist:
- [ ] Starts with SELECT?
- [ ] No INSERT/UPDATE/DELETE?
- [ ] No DROP/CREATE/ALTER?
- [ ] No semicolons?
- [ ] No SQL comments?
- [ ] Returns < 10,000 rows?
- [ ] Executes in < 30 seconds?

---

## 📊 Example Queries

### Financial Metrics
```sql
-- Total MRR
SELECT SUM(p."MonthlyPrice") as value
FROM "ClinicSubscriptions" cs
INNER JOIN "Plans" p ON cs."PlanId" = p."Id"
WHERE cs."Status" = 'Active';

-- MRR Trend (12 months)
SELECT 
  DATE_TRUNC('month', cs."CreatedAt") as month,
  SUM(p."MonthlyPrice") as total_mrr
FROM "ClinicSubscriptions" cs
INNER JOIN "Plans" p ON cs."PlanId" = p."Id"
WHERE cs."CreatedAt" >= CURRENT_DATE - INTERVAL '12 months'
  AND cs."Status" = 'Active'
GROUP BY DATE_TRUNC('month', cs."CreatedAt")
ORDER BY month;
```

### Customer Metrics
```sql
-- Active Customers
SELECT COUNT(DISTINCT "ClinicId") as value
FROM "ClinicSubscriptions"
WHERE "Status" = 'Active';

-- Churn Rate
SELECT 
  ROUND(
    CAST(COUNT(CASE WHEN "Status" = 'Cancelled' AND "EndDate" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END) AS DECIMAL) / 
    NULLIF(COUNT(CASE WHEN "EndDate" >= CURRENT_DATE - INTERVAL '1 month' THEN 1 END), 0) * 100,
    2
  ) as value
FROM "ClinicSubscriptions";
```

### Operational Metrics
```sql
-- Total Appointments (Last 30 Days)
SELECT COUNT(*) as value
FROM "Appointments"
WHERE "AppointmentDate" >= CURRENT_DATE - INTERVAL '30 days';

-- Appointments by Status
SELECT 
  "Status",
  COUNT(*) as count
FROM "Appointments"
WHERE "AppointmentDate" >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY "Status";
```

---

## 🚨 Error Handling

### Common Errors

**"Query contains unsafe operations"**
- **Cause:** Query blocked by security filter
- **Solution:** Remove prohibited keywords, comments, or multiple statements

**"Query timeout exceeded"**
- **Cause:** Query took > 30 seconds
- **Solution:** Add WHERE clause, indexes, or aggregate data

**"Too many rows returned"**
- **Cause:** Query returned > 10,000 rows
- **Solution:** Add LIMIT clause or aggregate with GROUP BY

**"Column does not exist"**
- **Cause:** Invalid table or column name
- **Solution:** Use double quotes around identifiers: `"ColumnName"`

**"Syntax error"**
- **Cause:** Invalid SQL syntax
- **Solution:** Test query in database client first

---

## 📞 Support

For security concerns or questions:
- **Email:** security@medicwarehouse.com
- **Documentation:** Review this guide and Dashboard Creation Guide
- **Escalation:** Contact SystemAdmin team

---

**Last Updated:** January 28, 2026  
**Version:** 1.0  
**Security Level:** High  
**Compliance:** LGPD, GDPR compatible
