using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoFactorTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwoFactorTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationAttempts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwoFactorTokens_PatientUsers_PatientUserId",
                        column: x => x.PatientUserId,
                        principalTable: "PatientUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorTokens_Code_PatientUserId",
                table: "TwoFactorTokens",
                columns: new[] { "Code", "PatientUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorTokens_CreatedAt",
                table: "TwoFactorTokens",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorTokens_PatientUserId_ExpiresAt",
                table: "TwoFactorTokens",
                columns: new[] { "PatientUserId", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwoFactorTokens");
        }
    }
}
