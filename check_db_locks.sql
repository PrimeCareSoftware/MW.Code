-- Check for active database connections and locks
SELECT 
    pid,
    usename,
    application_name,
    client_addr,
    state,
    query_start,
    state_change,
    wait_event_type,
    wait_event,
    LEFT(query, 100) as query_preview
FROM pg_stat_activity 
WHERE datname = current_database()
  AND pid <> pg_backend_pid()
ORDER BY query_start DESC;

-- Check for locks on ReportTemplates table
SELECT 
    l.locktype,
    l.relation::regclass,
    l.mode,
    l.transactionid AS tid,
    l.virtualtransaction AS vtid,
    l.pid,
    l.granted,
    a.usename,
    a.query_start,
    a.state,
    LEFT(a.query, 80) AS query
FROM pg_catalog.pg_locks l
LEFT JOIN pg_catalog.pg_stat_activity a ON l.pid = a.pid
WHERE l.relation = 'ReportTemplates'::regclass
   OR l.locktype = 'transactionid'
ORDER BY l.granted, a.query_start;
