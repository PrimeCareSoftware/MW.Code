using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddSessionManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: Only add column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'public' AND table_name = 'Users' AND column_name = 'CurrentSessionId'
                    ) THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""CurrentSessionId"" character varying(200);
                    END IF;
                END $$;
            ");

            // Idempotent: Only add column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'public' AND table_name = 'Owners' AND column_name = 'CurrentSessionId'
                    ) THEN
                        ALTER TABLE ""Owners"" ADD COLUMN ""CurrentSessionId"" character varying(200);
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSessionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrentSessionId",
                table: "Owners");
        }
    }
}
