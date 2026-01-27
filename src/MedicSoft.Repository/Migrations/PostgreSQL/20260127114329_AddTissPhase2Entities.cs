using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddTissPhase2Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TissGlosas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuideId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroGuia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataGlosa = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataIdentificacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    CodigoGlosa = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DescricaoGlosa = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ValorGlosado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorOriginal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SequenciaItem = table.Column<int>(type: "integer", nullable: true),
                    CodigoProcedimento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NomeProcedimento = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    JustificativaRecurso = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TissGlosas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TissGlosas_TissGuides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "TissGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TissOperadoraConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WebServiceUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Usuario = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SenhaEncriptada = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CertificadoDigitalPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TimeoutSegundos = table.Column<int>(type: "integer", nullable: false),
                    TentativasReenvio = table.Column<int>(type: "integer", nullable: false),
                    UsaSoapHeader = table.Column<bool>(type: "boolean", nullable: false),
                    UsaCertificadoDigital = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MapeamentoTabelasJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TissOperadoraConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TissOperadoraConfigs_HealthInsuranceOperators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "HealthInsuranceOperators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TissRecursosGlosa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GlosaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Justificativa = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DataResposta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Resultado = table.Column<int>(type: "integer", nullable: true),
                    JustificativaOperadora = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ValorDeferido = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    AnexosJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TissRecursosGlosa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TissRecursosGlosa_TissGlosas_GlosaId",
                        column: x => x.GlosaId,
                        principalTable: "TissGlosas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TissGlosas_GuideId",
                table: "TissGlosas",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TissGlosas_TenantId",
                table: "TissGlosas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TissGlosas_TenantId_DataGlosa",
                table: "TissGlosas",
                columns: new[] { "TenantId", "DataGlosa" });

            migrationBuilder.CreateIndex(
                name: "IX_TissGlosas_TenantId_NumeroGuia",
                table: "TissGlosas",
                columns: new[] { "TenantId", "NumeroGuia" });

            migrationBuilder.CreateIndex(
                name: "IX_TissGlosas_TenantId_Status",
                table: "TissGlosas",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TissGlosas_TenantId_Tipo",
                table: "TissGlosas",
                columns: new[] { "TenantId", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_TissOperadoraConfigs_OperatorId",
                table: "TissOperadoraConfigs",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TissOperadoraConfigs_TenantId",
                table: "TissOperadoraConfigs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TissOperadoraConfigs_TenantId_OperatorId",
                table: "TissOperadoraConfigs",
                columns: new[] { "TenantId", "OperatorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TissRecursosGlosa_GlosaId",
                table: "TissRecursosGlosa",
                column: "GlosaId");

            migrationBuilder.CreateIndex(
                name: "IX_TissRecursosGlosa_TenantId",
                table: "TissRecursosGlosa",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TissRecursosGlosa_TenantId_DataEnvio",
                table: "TissRecursosGlosa",
                columns: new[] { "TenantId", "DataEnvio" });

            migrationBuilder.CreateIndex(
                name: "IX_TissRecursosGlosa_TenantId_Resultado",
                table: "TissRecursosGlosa",
                columns: new[] { "TenantId", "Resultado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TissOperadoraConfigs");

            migrationBuilder.DropTable(
                name: "TissRecursosGlosa");

            migrationBuilder.DropTable(
                name: "TissGlosas");
        }
    }
}
