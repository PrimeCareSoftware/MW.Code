using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddOwnerClinicLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnerClinicLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrimaryOwner = table.Column<bool>(type: "boolean", nullable: false),
                    Role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OwnershipPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    InactivatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InactivationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerClinicLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OwnerClinicLinks_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnerClinicLinks_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerClinicLinks_ClinicId_IsPrimaryOwner",
                table: "OwnerClinicLinks",
                columns: new[] { "ClinicId", "IsPrimaryOwner" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerClinicLinks_Owner_Clinic",
                table: "OwnerClinicLinks",
                columns: new[] { "OwnerId", "ClinicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OwnerClinicLinks_OwnerId",
                table: "OwnerClinicLinks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerClinicLinks_TenantId_ClinicId",
                table: "OwnerClinicLinks",
                columns: new[] { "TenantId", "ClinicId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnerClinicLinks");
        }
    }
}
