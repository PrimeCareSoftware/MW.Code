using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddFinancialModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountsReceivable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true),
                    HealthInsuranceOperatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SettlementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InstallmentNumber = table.Column<int>(type: "integer", nullable: true),
                    TotalInstallments = table.Column<int>(type: "integer", nullable: true),
                    InterestRate = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    FineRate = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    DiscountRate = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsReceivable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountsReceivable_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountsReceivable_HealthInsuranceOperators_HealthInsurance~",
                        column: x => x.HealthInsuranceOperatorId,
                        principalTable: "HealthInsuranceOperators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountsReceivable_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialClosures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    HealthInsuranceOperatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClosureNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    ClosureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PatientAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    InsuranceAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SettlementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    DiscountReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialClosures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialClosures_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialClosures_HealthInsuranceOperators_HealthInsuranceO~",
                        column: x => x.HealthInsuranceOperatorId,
                        principalTable: "HealthInsuranceOperators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialClosures_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TradeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DocumentNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    ZipCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BankAccount = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PixKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceivablePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivablePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceivablePayments_AccountsReceivable_ReceivableId",
                        column: x => x.ReceivableId,
                        principalTable: "AccountsReceivable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialClosureItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClosureId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CoverByInsurance = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialClosureItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialClosureItems_FinancialClosures_ClosureId",
                        column: x => x.ClosureId,
                        principalTable: "FinancialClosures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountsPayable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InstallmentNumber = table.Column<int>(type: "integer", nullable: true),
                    TotalInstallments = table.Column<int>(type: "integer", nullable: true),
                    BankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BankAccount = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PixKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsPayable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountsPayable_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashFlowEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReceivableId = table.Column<Guid>(type: "uuid", nullable: true),
                    PayableId = table.Column<Guid>(type: "uuid", nullable: true),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BankAccount = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashFlowEntries_AccountsPayable_PayableId",
                        column: x => x.PayableId,
                        principalTable: "AccountsPayable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashFlowEntries_AccountsReceivable_ReceivableId",
                        column: x => x.ReceivableId,
                        principalTable: "AccountsReceivable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashFlowEntries_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashFlowEntries_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayablePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayablePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayablePayments_AccountsPayable_PayableId",
                        column: x => x.PayableId,
                        principalTable: "AccountsPayable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountsPayable_Category",
                table: "AccountsPayable",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsPayable_DocumentNumber",
                table: "AccountsPayable",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsPayable_DueDate",
                table: "AccountsPayable",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsPayable_Status",
                table: "AccountsPayable",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsPayable_SupplierId",
                table: "AccountsPayable",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsPayable_TenantId",
                table: "AccountsPayable",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_AppointmentId",
                table: "AccountsReceivable",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_DocumentNumber",
                table: "AccountsReceivable",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_DueDate",
                table: "AccountsReceivable",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_HealthInsuranceOperatorId",
                table: "AccountsReceivable",
                column: "HealthInsuranceOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_PatientId",
                table: "AccountsReceivable",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_Status",
                table: "AccountsReceivable",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_TenantId",
                table: "AccountsReceivable",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowEntries_AppointmentId",
                table: "CashFlowEntries",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowEntries_Category",
                table: "CashFlowEntries",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowEntries_PayableId",
                table: "CashFlowEntries",
                column: "PayableId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowEntries_PaymentId",
                table: "CashFlowEntries",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowEntries_ReceivableId",
                table: "CashFlowEntries",
                column: "ReceivableId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowEntries_TenantId",
                table: "CashFlowEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowEntries_TransactionDate",
                table: "CashFlowEntries",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowEntries_Type",
                table: "CashFlowEntries",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosureItems_ClosureId",
                table: "FinancialClosureItems",
                column: "ClosureId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosureItems_TenantId",
                table: "FinancialClosureItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosures_AppointmentId",
                table: "FinancialClosures",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosures_ClosureDate",
                table: "FinancialClosures",
                column: "ClosureDate");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosures_ClosureNumber",
                table: "FinancialClosures",
                column: "ClosureNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosures_HealthInsuranceOperatorId",
                table: "FinancialClosures",
                column: "HealthInsuranceOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosures_PatientId",
                table: "FinancialClosures",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosures_Status",
                table: "FinancialClosures",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClosures_TenantId",
                table: "FinancialClosures",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PayablePayments_PayableId",
                table: "PayablePayments",
                column: "PayableId");

            migrationBuilder.CreateIndex(
                name: "IX_PayablePayments_PaymentDate",
                table: "PayablePayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_PayablePayments_TenantId",
                table: "PayablePayments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivablePayments_PaymentDate",
                table: "ReceivablePayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivablePayments_ReceivableId",
                table: "ReceivablePayments",
                column: "ReceivableId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivablePayments_TenantId",
                table: "ReceivablePayments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_DocumentNumber",
                table: "Suppliers",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_IsActive",
                table: "Suppliers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Name",
                table: "Suppliers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TenantId",
                table: "Suppliers",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashFlowEntries");

            migrationBuilder.DropTable(
                name: "FinancialClosureItems");

            migrationBuilder.DropTable(
                name: "PayablePayments");

            migrationBuilder.DropTable(
                name: "ReceivablePayments");

            migrationBuilder.DropTable(
                name: "FinancialClosures");

            migrationBuilder.DropTable(
                name: "AccountsPayable");

            migrationBuilder.DropTable(
                name: "AccountsReceivable");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
