using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddRecurrenceSeriesAndExceptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns to BlockedTimeSlots table
            migrationBuilder.AddColumn<Guid>(
                name: "RecurringSeriesId",
                table: "BlockedTimeSlots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalOccurrenceDate",
                table: "BlockedTimeSlots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsException",
                table: "BlockedTimeSlots",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Add new columns to RecurringAppointmentPatterns table
            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveEndDate",
                table: "RecurringAppointmentPatterns",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentPatternId",
                table: "RecurringAppointmentPatterns",
                type: "uuid",
                nullable: true);

            // Create RecurrenceExceptions table
            migrationBuilder.CreateTable(
                name: "RecurrenceExceptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecurringPatternId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecurringSeriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExceptionType = table.Column<int>(type: "integer", nullable: false),
                    NewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NewStartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    NewEndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurrenceExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurrenceExceptions_RecurringAppointmentPatterns_Recurrin~",
                        column: x => x.RecurringPatternId,
                        principalTable: "RecurringAppointmentPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for BlockedTimeSlots new columns
            migrationBuilder.CreateIndex(
                name: "IX_BlockedTimeSlots_TenantId_RecurringSeriesId",
                table: "BlockedTimeSlots",
                columns: new[] { "TenantId", "RecurringSeriesId" });

            // Create indexes for RecurrenceExceptions
            migrationBuilder.CreateIndex(
                name: "IX_RecurrenceExceptions_TenantId_PatternId_OriginalDate",
                table: "RecurrenceExceptions",
                columns: new[] { "TenantId", "RecurringPatternId", "OriginalDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurrenceExceptions_TenantId_SeriesId_OriginalDate",
                table: "RecurrenceExceptions",
                columns: new[] { "TenantId", "RecurringSeriesId", "OriginalDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurrenceExceptions_TenantId_SeriesId",
                table: "RecurrenceExceptions",
                columns: new[] { "TenantId", "RecurringSeriesId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurrenceExceptions_TenantId",
                table: "RecurrenceExceptions",
                column: "TenantId");

            // Create index for RecurringAppointmentPatterns parent pattern
            migrationBuilder.CreateIndex(
                name: "IX_RecurringAppointmentPatterns_TenantId_ParentPatternId",
                table: "RecurringAppointmentPatterns",
                columns: new[] { "TenantId", "ParentPatternId" });

            // Add foreign key for ParentPatternId
            migrationBuilder.AddForeignKey(
                name: "FK_RecurringAppointmentPatterns_RecurringAppointmentPatterns_~",
                table: "RecurringAppointmentPatterns",
                column: "ParentPatternId",
                principalTable: "RecurringAppointmentPatterns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Migrate existing data: Generate unique SeriesId for each existing recurring blocked slot
            // This ensures backward compatibility
            migrationBuilder.Sql(@"
                UPDATE ""BlockedTimeSlots""
                SET ""RecurringSeriesId"" = gen_random_uuid()
                WHERE ""IsRecurring"" = true 
                  AND ""RecurringPatternId"" IS NOT NULL 
                  AND ""RecurringSeriesId"" IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_RecurringAppointmentPatterns_RecurringAppointmentPatterns_~",
                table: "RecurringAppointmentPatterns");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_BlockedTimeSlots_TenantId_RecurringSeriesId",
                table: "BlockedTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_RecurringAppointmentPatterns_TenantId_ParentPatternId",
                table: "RecurringAppointmentPatterns");

            // Drop RecurrenceExceptions table
            migrationBuilder.DropTable(
                name: "RecurrenceExceptions");

            // Drop columns from RecurringAppointmentPatterns
            migrationBuilder.DropColumn(
                name: "EffectiveEndDate",
                table: "RecurringAppointmentPatterns");

            migrationBuilder.DropColumn(
                name: "ParentPatternId",
                table: "RecurringAppointmentPatterns");

            // Drop columns from BlockedTimeSlots
            migrationBuilder.DropColumn(
                name: "RecurringSeriesId",
                table: "BlockedTimeSlots");

            migrationBuilder.DropColumn(
                name: "OriginalOccurrenceDate",
                table: "BlockedTimeSlots");

            migrationBuilder.DropColumn(
                name: "IsException",
                table: "BlockedTimeSlots");
        }
    }
}
