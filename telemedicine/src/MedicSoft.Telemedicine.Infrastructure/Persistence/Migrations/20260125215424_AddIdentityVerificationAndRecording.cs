using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Telemedicine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityVerificationAndRecording : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DocumentPhotoPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SelfiePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CrmCardPhotoPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CrmNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CrmState = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VerifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    VerificationNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TelemedicineSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityVerifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelemedicineRecordings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordingPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileFormat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    RecordingStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordingCompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "boolean", nullable: false),
                    EncryptionKeyId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ConsentId = table.Column<Guid>(type: "uuid", nullable: false),
                    RetentionUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletionReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemedicineRecordings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityVerifications_TelemedicineSessionId",
                table: "IdentityVerifications",
                column: "TelemedicineSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityVerifications_TenantId_Status",
                table: "IdentityVerifications",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityVerifications_TenantId_UserId_UserType",
                table: "IdentityVerifications",
                columns: new[] { "TenantId", "UserId", "UserType" });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityVerifications_TenantId_UserId_UserType_Status_Valid~",
                table: "IdentityVerifications",
                columns: new[] { "TenantId", "UserId", "UserType", "Status", "ValidUntil" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineRecordings_TenantId_IsDeleted",
                table: "TelemedicineRecordings",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineRecordings_TenantId_RetentionUntil",
                table: "TelemedicineRecordings",
                columns: new[] { "TenantId", "RetentionUntil" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineRecordings_TenantId_SessionId",
                table: "TelemedicineRecordings",
                columns: new[] { "TenantId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineRecordings_TenantId_Status",
                table: "TelemedicineRecordings",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityVerifications");

            migrationBuilder.DropTable(
                name: "TelemedicineRecordings");
        }
    }
}
