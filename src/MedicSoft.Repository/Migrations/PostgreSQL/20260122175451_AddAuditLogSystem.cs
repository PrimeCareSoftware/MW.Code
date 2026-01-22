using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddAuditLogSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActionDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityDisplayName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RequestPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    ChangedFields = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    DataCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProcessingConsents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ConsentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RevokedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PurposeDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DataCategories = table.Column<string>(type: "text", nullable: false),
                    ConsentText = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ConsentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProcessingConsents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Severity",
                table: "AuditLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId",
                table: "AuditLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProcessingConsents_ConsentDate",
                table: "DataProcessingConsents",
                column: "ConsentDate");

            migrationBuilder.CreateIndex(
                name: "IX_DataProcessingConsents_TenantId",
                table: "DataProcessingConsents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProcessingConsents_UserId",
                table: "DataProcessingConsents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "DataProcessingConsents");
        }
    }
}
