using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysToTokenTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationTokens_PatientUsers_PatientUserId",
                table: "EmailVerificationTokens",
                column: "PatientUserId",
                principalTable: "PatientUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetTokens_PatientUsers_PatientUserId",
                table: "PasswordResetTokens",
                column: "PatientUserId",
                principalTable: "PatientUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationTokens_PatientUsers_PatientUserId",
                table: "EmailVerificationTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetTokens_PatientUsers_PatientUserId",
                table: "PasswordResetTokens");
        }
    }
}
