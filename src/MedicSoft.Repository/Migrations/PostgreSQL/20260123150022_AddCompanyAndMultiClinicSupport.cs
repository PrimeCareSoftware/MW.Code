using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddCompanyAndMultiClinicSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentClinicId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Clinics",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TradeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Document = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Subdomain = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClinicLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsPreferredClinic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    InactivatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InactivationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClinicLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClinicLinks_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserClinicLinks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Data migration: Create Company for each existing Clinic
            migrationBuilder.Sql(@"
                INSERT INTO ""Companies"" (""Id"", ""Name"", ""TradeName"", ""Document"", ""DocumentType"", ""Phone"", ""Email"", ""IsActive"", ""Subdomain"", ""CreatedAt"", ""TenantId"")
                SELECT 
                    gen_random_uuid() as ""Id"",
                    ""Name"" as ""Name"",
                    ""TradeName"" as ""TradeName"",
                    ""Document"" as ""Document"",
                    ""DocumentType"" as ""DocumentType"",
                    ""Phone"" as ""Phone"",
                    ""Email"" as ""Email"",
                    ""IsActive"" as ""IsActive"",
                    ""Subdomain"" as ""Subdomain"",
                    ""CreatedAt"" as ""CreatedAt"",
                    ""TenantId"" as ""TenantId""
                FROM ""Clinics""
                GROUP BY ""Document"", ""DocumentType"", ""Name"", ""TradeName"", ""Phone"", ""Email"", ""IsActive"", ""Subdomain"", ""CreatedAt"", ""TenantId""
            ");

            // Data migration: Link Clinics to their respective Companies
            migrationBuilder.Sql(@"
                UPDATE ""Clinics"" c
                SET ""CompanyId"" = comp.""Id""
                FROM ""Companies"" comp
                WHERE c.""Document"" = comp.""Document"" AND c.""TenantId"" = comp.""TenantId""
            ");

            // Data migration: Create UserClinicLink for each existing User.ClinicId relationship
            migrationBuilder.Sql(@"
                INSERT INTO ""UserClinicLinks"" (""Id"", ""UserId"", ""ClinicId"", ""LinkedDate"", ""IsActive"", ""IsPreferredClinic"", ""CreatedAt"", ""TenantId"")
                SELECT 
                    gen_random_uuid() as ""Id"",
                    u.""Id"" as ""UserId"",
                    u.""ClinicId"" as ""ClinicId"",
                    u.""CreatedAt"" as ""LinkedDate"",
                    true as ""IsActive"",
                    true as ""IsPreferredClinic"",
                    u.""CreatedAt"" as ""CreatedAt"",
                    u.""TenantId"" as ""TenantId""
                FROM ""Users"" u
                WHERE u.""ClinicId"" IS NOT NULL
            ");

            // Data migration: Set CurrentClinicId for existing users
            migrationBuilder.Sql(@"
                UPDATE ""Users""
                SET ""CurrentClinicId"" = ""ClinicId""
                WHERE ""ClinicId"" IS NOT NULL
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentClinicId",
                table: "Users",
                column: "CurrentClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_CompanyId",
                table: "Clinics",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Document",
                table: "Companies",
                column: "Document",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_IsActive",
                table: "Companies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Subdomain",
                table: "Companies",
                column: "Subdomain",
                unique: true,
                filter: "\"Subdomain\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TenantId",
                table: "Companies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClinicLinks_ClinicId",
                table: "UserClinicLinks",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClinicLinks_TenantId",
                table: "UserClinicLinks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClinicLinks_UserId",
                table: "UserClinicLinks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClinicLinks_UserId_ClinicId_TenantId",
                table: "UserClinicLinks",
                columns: new[] { "UserId", "ClinicId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClinicLinks_UserId_IsActive",
                table: "UserClinicLinks",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.AddForeignKey(
                name: "FK_Clinics_Companies_CompanyId",
                table: "Clinics",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Clinics_CurrentClinicId",
                table: "Users",
                column: "CurrentClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clinics_Companies_CompanyId",
                table: "Clinics");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Clinics_CurrentClinicId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "UserClinicLinks");

            migrationBuilder.DropIndex(
                name: "IX_Users_CurrentClinicId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Clinics_CompanyId",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "CurrentClinicId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Clinics");
        }
    }
}
