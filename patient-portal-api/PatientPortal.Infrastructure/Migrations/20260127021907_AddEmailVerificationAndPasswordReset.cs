using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationAndPasswordReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create EmailVerificationTokens table only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables 
                                  WHERE table_schema = 'public' 
                                  AND table_name = 'EmailVerificationTokens') THEN
                        CREATE TABLE ""EmailVerificationTokens"" (
                            ""Id"" uuid NOT NULL,
                            ""PatientUserId"" uuid NOT NULL,
                            ""Token"" character varying(512) NOT NULL,
                            ""ExpiresAt"" timestamp with time zone NOT NULL,
                            ""CreatedAt"" timestamp with time zone NOT NULL,
                            ""IsUsed"" boolean NOT NULL,
                            ""UsedAt"" timestamp with time zone,
                            CONSTRAINT ""PK_EmailVerificationTokens"" PRIMARY KEY (""Id"")
                        );
                    END IF;
                    
                    -- Create indexes if they don't exist
                    IF NOT EXISTS (SELECT 1 FROM pg_indexes 
                                  WHERE schemaname = 'public' 
                                  AND tablename = 'EmailVerificationTokens' 
                                  AND indexname = 'IX_EmailVerificationTokens_Token') THEN
                        CREATE UNIQUE INDEX ""IX_EmailVerificationTokens_Token"" 
                            ON ""EmailVerificationTokens"" (""Token"");
                    END IF;
                    
                    IF NOT EXISTS (SELECT 1 FROM pg_indexes 
                                  WHERE schemaname = 'public' 
                                  AND tablename = 'EmailVerificationTokens' 
                                  AND indexname = 'IX_EmailVerificationTokens_PatientUserId_ExpiresAt') THEN
                        CREATE INDEX ""IX_EmailVerificationTokens_PatientUserId_ExpiresAt"" 
                            ON ""EmailVerificationTokens"" (""PatientUserId"", ""ExpiresAt"");
                    END IF;
                END $$;
            ");

            // Create PasswordResetTokens table only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables 
                                  WHERE table_schema = 'public' 
                                  AND table_name = 'PasswordResetTokens') THEN
                        CREATE TABLE ""PasswordResetTokens"" (
                            ""Id"" uuid NOT NULL,
                            ""PatientUserId"" uuid NOT NULL,
                            ""Token"" character varying(512) NOT NULL,
                            ""ExpiresAt"" timestamp with time zone NOT NULL,
                            ""CreatedAt"" timestamp with time zone NOT NULL,
                            ""IsUsed"" boolean NOT NULL,
                            ""UsedAt"" timestamp with time zone,
                            ""CreatedByIp"" character varying(50),
                            CONSTRAINT ""PK_PasswordResetTokens"" PRIMARY KEY (""Id"")
                        );
                    END IF;
                    
                    -- Create indexes if they don't exist
                    IF NOT EXISTS (SELECT 1 FROM pg_indexes 
                                  WHERE schemaname = 'public' 
                                  AND tablename = 'PasswordResetTokens' 
                                  AND indexname = 'IX_PasswordResetTokens_Token') THEN
                        CREATE UNIQUE INDEX ""IX_PasswordResetTokens_Token"" 
                            ON ""PasswordResetTokens"" (""Token"");
                    END IF;
                    
                    IF NOT EXISTS (SELECT 1 FROM pg_indexes 
                                  WHERE schemaname = 'public' 
                                  AND tablename = 'PasswordResetTokens' 
                                  AND indexname = 'IX_PasswordResetTokens_PatientUserId_ExpiresAt') THEN
                        CREATE INDEX ""IX_PasswordResetTokens_PatientUserId_ExpiresAt"" 
                            ON ""PasswordResetTokens"" (""PatientUserId"", ""ExpiresAt"");
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop tables only if they exist
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS ""EmailVerificationTokens"";
            ");

            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS ""PasswordResetTokens"";
            ");
        }
    }
}
