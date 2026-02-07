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
            // Create GlobalDocumentTemplates table
            migrationBuilder.CreateTable(
                name: "GlobalDocumentTemplates",
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
                    table.PrimaryKey("PK_GlobalDocumentTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalDocumentTemplates_TenantId",
                table: "GlobalDocumentTemplates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalDocumentTemplates_Type",
                table: "GlobalDocumentTemplates",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalDocumentTemplates_Specialty",
                table: "GlobalDocumentTemplates",
                column: "Specialty");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalDocumentTemplates_IsActive",
                table: "GlobalDocumentTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalDocumentTemplates_Name_Type_TenantId",
                table: "GlobalDocumentTemplates",
                columns: new[] { "Name", "Type", "TenantId" });

            // Add GlobalTemplateId column to DocumentTemplates table
            migrationBuilder.AddColumn<Guid>(
                name: "GlobalTemplateId",
                table: "DocumentTemplates",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_GlobalTemplateId",
                table: "DocumentTemplates",
                column: "GlobalTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTemplates_GlobalDocumentTemplates_GlobalTemplateId",
                table: "DocumentTemplates",
                column: "GlobalTemplateId",
                principalTable: "GlobalDocumentTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTemplates_GlobalDocumentTemplates_GlobalTemplateId",
                table: "DocumentTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTemplates_GlobalTemplateId",
                table: "DocumentTemplates");

            migrationBuilder.DropColumn(
                name: "GlobalTemplateId",
                table: "DocumentTemplates");

            migrationBuilder.DropTable(
                name: "GlobalDocumentTemplates");
        }
    }
}
