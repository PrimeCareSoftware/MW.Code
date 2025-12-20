-- Script to register missing migrations in the __EFMigrationsHistory table
-- This is needed because the database was partially created before migrations were properly tracked

-- These migrations should already be applied to the database based on the table structures we see
-- We're just registering them in the history so EF Core knows they've been applied

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES 
  ('20251106193124_AddWaitingQueue', '8.0.0'),
  ('20251110193734_AddSessionManagement', '8.0.0'),
  ('20251119194448_AddOwnerClinicLink', '8.0.0'),
  ('20251129140804_AddNotificationsTable', '8.0.0')
ON CONFLICT DO NOTHING;
