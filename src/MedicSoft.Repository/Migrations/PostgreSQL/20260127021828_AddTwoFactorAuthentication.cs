using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddTwoFactorAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwoFactorAuth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecretKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EnabledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EnabledByIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorAuth", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TwoFactorBackupCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HashedCode = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TwoFactorAuthId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorBackupCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwoFactorBackupCodes_TwoFactorAuth_TwoFactorAuthId",
                        column: x => x.TwoFactorAuthId,
                        principalTable: "TwoFactorAuth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorAuth_TenantId",
                table: "TwoFactorAuth",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorAuth_UserId",
                table: "TwoFactorAuth",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorAuth_UserId_IsEnabled",
                table: "TwoFactorAuth",
                columns: new[] { "UserId", "IsEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorAuth_UserId_TenantId",
                table: "TwoFactorAuth",
                columns: new[] { "UserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorBackupCodes_TwoFactorAuthId",
                table: "TwoFactorBackupCodes",
                column: "TwoFactorAuthId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwoFactorBackupCodes");

            migrationBuilder.DropTable(
                name: "TwoFactorAuth");
        }
    }
}
