using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddGlobalDocumentTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create GlobalDocumentTemplates table with lowercase name for PostgreSQL compatibility
            migrationBuilder.CreateTable(
                name: "globaldocumenttemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Specialty = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Variables = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_globaldocumenttemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_globaldocumenttemplates_tenantid",
                table: "globaldocumenttemplates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "ix_globaldocumenttemplates_type",
                table: "globaldocumenttemplates",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "ix_globaldocumenttemplates_specialty",
                table: "globaldocumenttemplates",
                column: "Specialty");

            migrationBuilder.CreateIndex(
                name: "ix_globaldocumenttemplates_isactive",
                table: "globaldocumenttemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "ix_globaldocumenttemplates_name_type_tenantid",
                table: "globaldocumenttemplates",
                columns: new[] { "Name", "Type", "TenantId" });

            // Add GlobalTemplateId column to DocumentTemplates table
            migrationBuilder.AddColumn<Guid>(
                name: "GlobalTemplateId",
                table: "DocumentTemplates",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_documenttemplates_globaltemplateid",
                table: "DocumentTemplates",
                column: "GlobalTemplateId");

            migrationBuilder.AddForeignKey(
                name: "fk_documenttemplates_globaldocumenttemplates_globaltemplateid",
                table: "DocumentTemplates",
                column: "GlobalTemplateId",
                principalTable: "globaldocumenttemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documenttemplates_globaldocumenttemplates_globaltemplateid",
                table: "DocumentTemplates");

            migrationBuilder.DropIndex(
                name: "ix_documenttemplates_globaltemplateid",
                table: "DocumentTemplates");

            migrationBuilder.DropColumn(
                name: "GlobalTemplateId",
                table: "DocumentTemplates");

            migrationBuilder.DropTable(
                name: "globaldocumenttemplates");
        }
    }
}
