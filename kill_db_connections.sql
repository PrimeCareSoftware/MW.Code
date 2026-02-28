-- EMERGENCY: Kill all non-superuser connections to the database
-- Use this to clear deadlocks before running migrations

-- First, see what will be terminated
SELECT 
    pid,
    usename,
    application_name,
    state,
    query_start,
    LEFT(query, 100) as query
FROM pg_stat_activity 
WHERE datname = current_database()
  AND pid <> pg_backend_pid()
  AND usename != 'postgres';  -- Don't kill superuser connections

-- Uncomment the line below to actually terminate the connections
-- SELECT pg_terminate_backend(pid) 
-- FROM pg_stat_activity 
-- WHERE datname = current_database()
--   AND pid <> pg_backend_pid()
--   AND usename != 'postgres';
