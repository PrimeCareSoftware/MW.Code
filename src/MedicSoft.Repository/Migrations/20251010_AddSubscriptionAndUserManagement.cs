using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionAndUserManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create SubscriptionPlans table
            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrialDays = table.Column<int>(type: "int", nullable: false),
                    MaxUsers = table.Column<int>(type: "int", nullable: false),
                    MaxPatients = table.Column<int>(type: "int", nullable: false),
                    HasReports = table.Column<bool>(type: "bit", nullable: false),
                    HasWhatsAppIntegration = table.Column<bool>(type: "bit", nullable: false),
                    HasSMSNotifications = table.Column<bool>(type: "bit", nullable: false),
                    HasTissExport = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            // Create Users table
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfessionalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Specialty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create ClinicSubscriptions table
            migrationBuilder.CreateTable(
                name: "ClinicSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrialEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancellationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFrozen = table.Column<bool>(type: "bit", nullable: false),
                    FrozenStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FrozenEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PendingPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PendingPlanPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlanChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsUpgrade = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicSubscriptions_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicSubscriptions_SubscriptionPlans_PendingPlanId",
                        column: x => x.PendingPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create indexes for SubscriptionPlans
            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_Type",
                table: "SubscriptionPlans",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_IsActive",
                table: "SubscriptionPlans",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_TenantId_Type",
                table: "SubscriptionPlans",
                columns: new[] { "TenantId", "Type" });

            // Create indexes for Users
            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClinicId",
                table: "Users",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_IsActive",
                table: "Users",
                columns: new[] { "TenantId", "IsActive" });

            // Create indexes for ClinicSubscriptions
            migrationBuilder.CreateIndex(
                name: "IX_ClinicSubscriptions_ClinicId",
                table: "ClinicSubscriptions",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicSubscriptions_Status",
                table: "ClinicSubscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicSubscriptions_NextPaymentDate",
                table: "ClinicSubscriptions",
                column: "NextPaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicSubscriptions_TenantId_Status",
                table: "ClinicSubscriptions",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicSubscriptions_SubscriptionPlanId",
                table: "ClinicSubscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicSubscriptions_PendingPlanId",
                table: "ClinicSubscriptions",
                column: "PendingPlanId");

            // Create ModuleConfigurations table
            migrationBuilder.CreateTable(
                name: "ModuleConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleConfigurations_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for ModuleConfigurations
            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurations_ClinicId",
                table: "ModuleConfigurations",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurations_ClinicId_ModuleName",
                table: "ModuleConfigurations",
                columns: new[] { "ClinicId", "ModuleName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurations_TenantId_IsEnabled",
                table: "ModuleConfigurations",
                columns: new[] { "TenantId", "IsEnabled" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleConfigurations");

            migrationBuilder.DropTable(
                name: "ClinicSubscriptions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");
        }
    }
}
