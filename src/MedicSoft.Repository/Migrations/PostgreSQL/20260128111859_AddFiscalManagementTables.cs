using System;
using System.Collections.Generic;
using MedicSoft.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddFiscalManagementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApuracaoImpostosId",
                table: "ElectronicInvoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApuracoesImpostos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Mes = table.Column<int>(type: "integer", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    DataApuracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FaturamentoBruto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Deducoes = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPIS = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalCOFINS = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalIR = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalCSLL = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalISS = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalINSS = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ReceitaBruta12Meses = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    AliquotaEfetiva = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ValorDAS = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ComprovantesPagamento = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApuracoesImpostos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApuracoesImpostos_Clinics_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracoesFiscais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Regime = table.Column<int>(type: "integer", nullable: false),
                    VigenciaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VigenciaFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OptanteSimplesNacional = table.Column<bool>(type: "boolean", nullable: false),
                    AnexoSimples = table.Column<int>(type: "integer", nullable: true),
                    FatorR = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    AliquotaISS = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    AliquotaPIS = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    AliquotaCOFINS = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    AliquotaIR = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    AliquotaCSLL = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    RetemINSS = table.Column<bool>(type: "boolean", nullable: false),
                    AliquotaINSS = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    CodigoServico = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CNAE = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InscricaoMunicipal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ISS_Retido = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesFiscais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracoesFiscais_Clinics_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImpostosNotas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotaFiscalId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValorBruto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorDesconto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AliquotaPIS = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ValorPIS = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AliquotaCOFINS = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ValorCOFINS = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AliquotaIR = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ValorIR = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AliquotaCSLL = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ValorCSLL = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AliquotaISS = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ValorISS = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ISSRetido = table.Column<bool>(type: "boolean", nullable: false),
                    CodigoServicoMunicipal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AliquotaINSS = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ValorINSS = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    INSSRetido = table.Column<bool>(type: "boolean", nullable: false),
                    DataCalculo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegimeTributario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpostosNotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpostosNotas_ElectronicInvoices_NotaFiscalId",
                        column: x => x.NotaFiscalId,
                        principalTable: "ElectronicInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanoContas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Natureza = table.Column<int>(type: "integer", nullable: false),
                    ContaPaiId = table.Column<Guid>(type: "uuid", nullable: true),
                    Analitica = table.Column<bool>(type: "boolean", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    Nivel = table.Column<int>(type: "integer", nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoContas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanoContas_Clinics_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanoContas_PlanoContas_ContaPaiId",
                        column: x => x.ContaPaiId,
                        principalTable: "PlanoContas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WebhookSubscriptions",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TargetUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Secret = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SubscribedEvents = table.Column<IReadOnlyCollection<WebhookEvent>>(type: "jsonb", nullable: false),
                    MaxRetries = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    RetryDelaySeconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 60),
                    TotalDeliveries = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SuccessfulDeliveries = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FailedDeliveries = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastDeliveryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSuccessAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastFailureAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LancamentosContabeis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanoContasId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataLancamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Historico = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Origem = table.Column<int>(type: "integer", nullable: false),
                    DocumentoOrigemId = table.Column<Guid>(type: "uuid", nullable: true),
                    NumeroDocumento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LoteId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LancamentosContabeis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LancamentosContabeis_Clinics_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LancamentosContabeis_PlanoContas_PlanoContasId",
                        column: x => x.PlanoContasId,
                        principalTable: "PlanoContas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WebhookDeliveries",
                schema: "crm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TargetUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NextRetryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "integer", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookDeliveries_WebhookSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "crm",
                        principalTable: "WebhookSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicInvoices_ApuracaoImpostosId",
                table: "ElectronicInvoices",
                column: "ApuracaoImpostosId");

            migrationBuilder.CreateIndex(
                name: "IX_ApuracoesImpostos_ClinicaId_Mes_Ano",
                table: "ApuracoesImpostos",
                columns: new[] { "ClinicaId", "Mes", "Ano" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApuracoesImpostos_DataApuracao",
                table: "ApuracoesImpostos",
                column: "DataApuracao");

            migrationBuilder.CreateIndex(
                name: "IX_ApuracoesImpostos_Status",
                table: "ApuracoesImpostos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApuracoesImpostos_TenantId",
                table: "ApuracoesImpostos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesFiscais_ClinicaId_VigenciaInicio",
                table: "ConfiguracoesFiscais",
                columns: new[] { "ClinicaId", "VigenciaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesFiscais_Regime",
                table: "ConfiguracoesFiscais",
                column: "Regime");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesFiscais_TenantId",
                table: "ConfiguracoesFiscais",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpostosNotas_DataCalculo",
                table: "ImpostosNotas",
                column: "DataCalculo");

            migrationBuilder.CreateIndex(
                name: "IX_ImpostosNotas_NotaFiscalId",
                table: "ImpostosNotas",
                column: "NotaFiscalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImpostosNotas_RegimeTributario",
                table: "ImpostosNotas",
                column: "RegimeTributario");

            migrationBuilder.CreateIndex(
                name: "IX_ImpostosNotas_TenantId",
                table: "ImpostosNotas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_ClinicaId_DataLancamento",
                table: "LancamentosContabeis",
                columns: new[] { "ClinicaId", "DataLancamento" });

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_DocumentoOrigemId",
                table: "LancamentosContabeis",
                column: "DocumentoOrigemId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_LoteId",
                table: "LancamentosContabeis",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_Origem",
                table: "LancamentosContabeis",
                column: "Origem");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_PlanoContasId",
                table: "LancamentosContabeis",
                column: "PlanoContasId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_TenantId",
                table: "LancamentosContabeis",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_Tipo",
                table: "LancamentosContabeis",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoContas_ClinicaId_Analitica",
                table: "PlanoContas",
                columns: new[] { "ClinicaId", "Analitica" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanoContas_ClinicaId_Ativa",
                table: "PlanoContas",
                columns: new[] { "ClinicaId", "Ativa" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanoContas_ClinicaId_Codigo",
                table: "PlanoContas",
                columns: new[] { "ClinicaId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanoContas_ContaPaiId",
                table: "PlanoContas",
                column: "ContaPaiId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoContas_TenantId",
                table: "PlanoContas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoContas_Tipo",
                table: "PlanoContas",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveries_CreatedAt",
                schema: "crm",
                table: "WebhookDeliveries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveries_NextRetryAt",
                schema: "crm",
                table: "WebhookDeliveries",
                column: "NextRetryAt");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveries_Status",
                schema: "crm",
                table: "WebhookDeliveries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveries_SubscriptionId",
                schema: "crm",
                table: "WebhookDeliveries",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveries_TenantId",
                schema: "crm",
                table: "WebhookDeliveries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookSubscriptions_IsActive",
                schema: "crm",
                table: "WebhookSubscriptions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookSubscriptions_TenantId",
                schema: "crm",
                table: "WebhookSubscriptions",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElectronicInvoices_ApuracoesImpostos_ApuracaoImpostosId",
                table: "ElectronicInvoices",
                column: "ApuracaoImpostosId",
                principalTable: "ApuracoesImpostos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElectronicInvoices_ApuracoesImpostos_ApuracaoImpostosId",
                table: "ElectronicInvoices");

            migrationBuilder.DropTable(
                name: "ApuracoesImpostos");

            migrationBuilder.DropTable(
                name: "ConfiguracoesFiscais");

            migrationBuilder.DropTable(
                name: "ImpostosNotas");

            migrationBuilder.DropTable(
                name: "LancamentosContabeis");

            migrationBuilder.DropTable(
                name: "WebhookDeliveries",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "PlanoContas");

            migrationBuilder.DropTable(
                name: "WebhookSubscriptions",
                schema: "crm");

            migrationBuilder.DropIndex(
                name: "IX_ElectronicInvoices_ApuracaoImpostosId",
                table: "ElectronicInvoices");

            migrationBuilder.DropColumn(
                name: "ApuracaoImpostosId",
                table: "ElectronicInvoices");
        }
    }
}
