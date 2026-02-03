using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class RefactorSalesforceLeadsToStandaloneLeadManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old SalesforceLeads table
            migrationBuilder.DropTable(
                name: "SalesforceLeads");

            // Create new Leads table
            migrationBuilder.CreateTable(
                name: "Leads",
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
                    AssignedToUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextFollowUpDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false, defaultValue: 50),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            // Create LeadActivities table
            migrationBuilder.CreateTable(
                name: "LeadActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LeadId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PerformedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    PerformedByUserName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ActivityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    Outcome = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadActivities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for Leads
            migrationBuilder.CreateIndex(
                name: "IX_Leads_SessionId",
                table: "Leads",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Email",
                table: "Leads",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Phone",
                table: "Leads",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Status",
                table: "Leads",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_AssignedToUserId",
                table: "Leads",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_NextFollowUpDate",
                table: "Leads",
                column: "NextFollowUpDate");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Score",
                table: "Leads",
                column: "Score");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_TenantId",
                table: "Leads",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_CapturedAt",
                table: "Leads",
                column: "CapturedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_IsDeleted",
                table: "Leads",
                column: "IsDeleted");

            // Create indexes for LeadActivities
            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_LeadId",
                table: "LeadActivities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_Type",
                table: "LeadActivities",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_ActivityDate",
                table: "LeadActivities",
                column: "ActivityDate");

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_PerformedByUserId",
                table: "LeadActivities",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_IsDeleted",
                table: "LeadActivities",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new tables
            migrationBuilder.DropTable(
                name: "LeadActivities");

            migrationBuilder.DropTable(
                name: "Leads");

            // Recreate old SalesforceLeads table
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
    }
}
