using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddTagAndClinicTagTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Tags table
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    IsAutomatic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AutomationRules = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            // Create ClinicTags table (many-to-many)
            migrationBuilder.CreateTable(
                name: "ClinicTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAutoAssigned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicTags_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClinicTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for performance
            migrationBuilder.CreateIndex(
                name: "IX_Tags_TenantId",
                table: "Tags",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Category",
                table: "Tags",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_IsAutomatic",
                table: "Tags",
                column: "IsAutomatic");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicTags_ClinicId",
                table: "ClinicTags",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicTags_TagId",
                table: "ClinicTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicTags_TenantId",
                table: "ClinicTags",
                column: "TenantId");

            // Unique constraint to prevent duplicate tag assignments
            migrationBuilder.CreateIndex(
                name: "IX_ClinicTags_ClinicId_TagId",
                table: "ClinicTags",
                columns: new[] { "ClinicId", "TagId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ClinicTags");
            migrationBuilder.DropTable(name: "Tags");
        }
    }
}
