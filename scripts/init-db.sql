-- Create extension for UUID generation
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create default tenant if not exists
DO $$
BEGIN
    -- This could be enhanced with actual tenant management tables
    -- For now, we'll just create the database schema
    RAISE NOTICE 'Database initialized for MedicWarehouse';
END $$;