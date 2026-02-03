using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddSalesforceLeadManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesforceLeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PlanId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PlanName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastStepReached = table.Column<int>(type: "integer", nullable: false),
                    LeadSource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Referrer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UtmCampaign = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UtmSource = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UtmMedium = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CapturedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SalesforceLeadId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsSyncedToSalesforce = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SyncAttempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastSyncError = table.Column<string>(type: "text", nullable: true),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesforceLeads", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesforceLeads_SessionId",
                table: "SalesforceLeads",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesforceLeads_Email",
                table: "SalesforceLeads",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_SalesforceLeads_Status",
                table: "SalesforceLeads",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SalesforceLeads_IsSyncedToSalesforce",
                table: "SalesforceLeads",
                column: "IsSyncedToSalesforce");

            migrationBuilder.CreateIndex(
                name: "IX_SalesforceLeads_SalesforceLeadId",
                table: "SalesforceLeads",
                column: "SalesforceLeadId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesforceLeads_TenantId",
                table: "SalesforceLeads",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesforceLeads_CapturedAt",
                table: "SalesforceLeads",
                column: "CapturedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesforceLeads");
        }
    }
}
