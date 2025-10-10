using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations
{
    /// <summary>
    /// Migration to add NotificationRoutines table for configurable automated notifications
    /// </summary>
    public partial class AddNotificationRoutines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationRoutines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MessageTemplate = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    ScheduleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScheduleConfiguration = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    RecipientFilter = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextExecutionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRoutines", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRoutines_TenantId_IsActive",
                table: "NotificationRoutines",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRoutines_Scope_IsActive",
                table: "NotificationRoutines",
                columns: new[] { "Scope", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRoutines_NextExecutionAt",
                table: "NotificationRoutines",
                column: "NextExecutionAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRoutines_Channel_TenantId",
                table: "NotificationRoutines",
                columns: new[] { "Channel", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRoutines_Type_TenantId",
                table: "NotificationRoutines",
                columns: new[] { "Type", "TenantId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationRoutines");
        }
    }
}
