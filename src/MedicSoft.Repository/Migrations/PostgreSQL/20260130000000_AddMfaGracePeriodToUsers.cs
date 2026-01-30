using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddMfaGracePeriodToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "mfa_grace_period_ends_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "first_login_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            // Create index for efficient MFA compliance queries
            migrationBuilder.CreateIndex(
                name: "ix_users_mfa_grace_period",
                table: "users",
                column: "mfa_grace_period_ends_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_mfa_grace_period",
                table: "users");

            migrationBuilder.DropColumn(
                name: "mfa_grace_period_ends_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "first_login_at",
                table: "users");
        }
    }
}
