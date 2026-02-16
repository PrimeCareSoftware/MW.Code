using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    /// <summary>
    /// Adds ProfessionalSpecialty enum column to Users table to strongly type professional specialties.
    /// 
    /// CONTEXT: This migration enhances the multi-professional system by adding a strongly-typed
    /// ProfessionalSpecialty field that links to the user's AccessProfile ConsultationFormProfile.
    /// This allows the attendance screen to automatically load the correct form configuration
    /// based on the professional's specialty.
    /// 
    /// The ProfessionalSpecialty enum values:
    /// - Medico = 1
    /// - Psicologo = 2
    /// - Nutricionista = 3
    /// - Fisioterapeuta = 4
    /// - Dentista = 5
    /// - Enfermeiro = 6
    /// - TerapeutaOcupacional = 7
    /// - Fonoaudiologo = 8
    /// - Veterinario = 9
    /// - Outro = 99
    /// 
    /// The existing Specialty string column is kept for backward compatibility.
    /// </summary>
    public partial class AddProfessionalSpecialtyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ProfessionalSpecialty column (nullable integer)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Check if the Users table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'Users' 
                        AND table_schema = 'public'
                    ) THEN
                        -- Check if ProfessionalSpecialty column does NOT exist
                        IF NOT EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'Users' 
                            AND column_name = 'ProfessionalSpecialty'
                            AND table_schema = 'public'
                        ) THEN
                            -- Add the column as nullable integer
                            ALTER TABLE ""Users"" 
                            ADD COLUMN ""ProfessionalSpecialty"" integer NULL;
                            
                            RAISE NOTICE 'ProfessionalSpecialty column added to Users table';
                        ELSE
                            RAISE NOTICE 'ProfessionalSpecialty column already exists in Users table - skipping';
                        END IF;
                    ELSE
                        RAISE WARNING 'Users table does not exist - skipping ProfessionalSpecialty column addition';
                    END IF;
                END $$;
            ");

            // Create an index for better query performance
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Check if the index does NOT exist
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE tablename = 'Users' 
                        AND indexname = 'IX_Users_ProfessionalSpecialty'
                    ) THEN
                        -- Create index
                        CREATE INDEX ""IX_Users_ProfessionalSpecialty"" 
                        ON ""Users"" (""ProfessionalSpecialty"") 
                        WHERE ""ProfessionalSpecialty"" IS NOT NULL;
                        
                        RAISE NOTICE 'Index IX_Users_ProfessionalSpecialty created';
                    ELSE
                        RAISE NOTICE 'Index IX_Users_ProfessionalSpecialty already exists - skipping';
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the index
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Check if the index EXISTS
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE tablename = 'Users' 
                        AND indexname = 'IX_Users_ProfessionalSpecialty'
                    ) THEN
                        DROP INDEX ""IX_Users_ProfessionalSpecialty"";
                        RAISE NOTICE 'Index IX_Users_ProfessionalSpecialty dropped';
                    ELSE
                        RAISE NOTICE 'Index IX_Users_ProfessionalSpecialty does not exist - skipping';
                    END IF;
                END $$;
            ");

            // Drop the column
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Check if the Users table exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_name = 'Users' 
                        AND table_schema = 'public'
                    ) THEN
                        -- Check if ProfessionalSpecialty column EXISTS
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'Users' 
                            AND column_name = 'ProfessionalSpecialty'
                            AND table_schema = 'public'
                        ) THEN
                            -- Drop the column
                            ALTER TABLE ""Users"" 
                            DROP COLUMN ""ProfessionalSpecialty"";
                            
                            RAISE NOTICE 'ProfessionalSpecialty column dropped from Users table';
                        ELSE
                            RAISE NOTICE 'ProfessionalSpecialty column does not exist in Users table - skipping';
                        END IF;
                    ELSE
                        RAISE WARNING 'Users table does not exist - skipping ProfessionalSpecialty column removal';
                    END IF;
                END $$;
            ");
        }
    }
}
